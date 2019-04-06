module Serialisation

open Game
open Newtonsoft.Json
open System
open CommandHandler


type SerializedEvent = (string * string)

let serializer = JsonSerializer.Create()

let serialize o =
    use stream = new IO.StringWriter()
    serializer.Serialize(stream, o)
    stream.ToString()

let deserialize s =
    use stream = new IO.StringReader(s)
    use reader = new JsonTextReader(stream)
    serializer.Deserialize<'t>(reader)

type GameId = GameId of int

type CardDto = {
    Value: string
    Color: string 
}

type GameStartedDto = {
    Players: int
    FirstCard: CardDto  
}

type CardPlayedDto = {
    Card: CardDto
    Player: int
}

type TurnStartedDto = {
    Player: int
}

module Parse =
    let (|Color|_|) c =
        match c with
        | "Red" -> Some Red
        | "Green" -> Some Green
        | "Blue" -> Some Blue
        | "Yellow" -> Some Yellow
        | _ -> None
        
    let (|Digit|_|) d =
        match d with
        | "Zero" -> Some Zero
        | "One" -> Some One
        | "Two" -> Some Two 
        | "Three" -> Some Three
        | "Four" -> Some Four
        | "Five" -> Some Five
        | "Six" -> Some Six
        | "Seven" -> Some Seven
        | "Eight" -> Some Eight
        | "Nine" -> Some Nine
        | _ -> None

    let (|Card|_|)  =
        function
      //| { Value = "Skip"; Color = Color c } -> Some (Skip(c))
        | { Value = Digit d; Color = Color c } -> Some (Digit(d,c))
        | _ -> None 

    let (|Players|_|) n =
        if n < 0 || n > 10 then
            None
        else
            Some (Game.Players n)

    let (|Player|_|) n =
        if n < 0 || n > 10 then
            None
        else
            Some (PlayerId n)

module GameEvents = 
    open Game

    let toCardDto = function
        | Digit(d, c ) -> { Value = string d; Color = string c}
      //| Skip(c) -> { Value = "Skip"; Color = string c }

    let serializeCard e =
        { CardPlayedDto.Card = toCardDto e.Card; Player = let (PlayerId p) = e.Player in p }
        |> serialize


    let serialize event = 
            match event with
            | GameStarted e -> 
                "GameStarted", 
                    { GameStartedDto.Players = let (Players p) = e.Players in p
                      FirstCard = toCardDto e.FirstCard } 
                    |> serialize
            | CardPlayed e -> "CardPlayed", serializeCard e
            | WrongCardPlayed e -> "WrongCardPlayed",  serializeCard e
            | InterruptionSucceeded e -> "InterruptionSucceeded",  serializeCard e
            | InterruptionMissed e -> "InterruptionMissed",  serializeCard e
            | PlayerPlayedAtWrongTurn e -> "PlayerPlayedAtWrongTurn", serializeCard e
            | TurnStarted e ->
                "TurnStarted",
                    { TurnStartedDto.Player = let (PlayerId p) = e.Player in p }
                    |> serialize

    let deserializeCard event data=
        data
        |> deserialize
        |> function 
            | { CardPlayedDto.Player = Parse.Player player; Card = Parse.Card card } ->
              [ event { CardPlayed.Player = player; Card = card } ]
            | _ -> []

    let deserialize (eventType, data) =
        match eventType with
        | "CardPlayed" -> data |> deserializeCard CardPlayed
        | "WrongCardPlayed" -> data |> deserializeCard WrongCardPlayed 
        | "PlayerPlayedAtWrongTurn" -> data |> deserializeCard PlayerPlayedAtWrongTurn
        | "InterruptionSucceeded" ->  data |> deserializeCard InterruptionSucceeded
        | "InterruptionMissed" -> data |> deserializeCard InterruptionMissed
        | "GameStarted" -> 
            data
            |> deserialize
            |> function 
                    | { GameStartedDto.Players = Parse.Players players
                        FirstCard = Parse.Card card } ->
                        [GameStarted  { Players = players; FirstCard = card }]
                    | _ -> []
        | "TurnStarted" ->
            data
            |> deserialize
            |> function
                    | { TurnStartedDto.Player = Parse.Player player} ->
                        [ TurnStarted { Player = player}]
                    | _ -> []
        | _ -> []

//type SnapshotDto = {
//    Started: bool
//    Version: int64
//    TopCard: CardDto
//    Player: int
//    PlayerCount: int
//}

//module Snapshot =
//    let deserialize data =
//        data 
//        |> deserialize
//        |> function
//            | { Started = true
//                Version = version
//                TopCard = GameEvents.Card card
//                Player = p
//                PlayerCount = c } ->
//                Some (Started { TopCard = card; Turn = { Player = PlayerId p; Count = PlayerCount c}}, EventNumber version)
//            | { Started = false
//                Version = version} -> Some (InitialState, EventNumber version)
//            | _ -> None

//    let serialize (state: State) (EventNumber v) =
//        match state with
//        | InitialState ->
//            { Started = false
//              Version = v
//              TopCard = { Value = ""; Color = "" }
//              Player = 0
//              PlayerCount = 0 }
//        | Started s ->
//            let (PlayerId p) = s.Turn.Player
//            let (PlayerCount c) = s.Turn.Count
//            { Version = v
//              TopCard = GameEvents.toCardDto s.TopCard
//              Player = p
//              PlayerCount = c }
//        |> serialize
