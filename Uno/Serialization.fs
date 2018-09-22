module Serialisation

open Game
open Newtonsoft.Json
open System
type SerializedEvent = (struct(string * string))

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
    Player: int
    Card: CardDto
}

module GameEvents = 
    open Game

    let toCardDto = function
        | Digit(d, c ) -> { Value = string d; Color = string c}
        | Skip(c) -> { Value = "Skip"; Color = string c }

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
        | { Value = "Skip"; Color = Color c } -> Some (Skip(c))
        | { Value = Digit d; Color = Color c } -> Some (Digit(d,c))
        | _ -> None 

    let (|Players|_|) n =
        if n < 0 || n > 10 then
            None
        else
            Some (PlayerCount n)
    let (|Player|_|) n =
        if n < 0 || n > 10 then
            None
        else
            Some (PlayerId n)

    let serialize event = 
            match event with
            | GameStarted e -> 
                struct("GameStarted", 
                    { GameStartedDto.Players = let (PlayerCount p) = e.Players in p
                      FirstCard = toCardDto e.FirstCard } 
                    |> serialize)
            | CardPlayed e -> 
                struct( "CardPlayed", 
                    { CardPlayedDto.Player = let (PlayerId p) = e.Player in p
                      Card = toCardDto e.Card }
                    |> serialize)
            

    let deserialize struct(eventType, data) =
        match eventType with
        | "CardPlayed" -> 
            data
            |> deserialize
            |> function 
                | { CardPlayedDto.Player = Player player; Card = Card card } ->
                    [CardPlayed { Player = player; Card = card }]
                | _ -> []

        | "GameStarted" -> 
            data
            |> deserialize
            |> function 
                    | { GameStartedDto.Players = Players players; FirstCard = Card card } ->
                        [GameStarted  { Players = players; FirstCard = card }]
                    | _ -> []
        | _ -> []
     