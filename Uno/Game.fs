module Game
open System.Numerics

type [<Struct>] PlayerCount = PlayerCount of int


type [<Struct>] PlayerId = PlayerId of int

type Direction = Clockwise | CounterClockwise

type Player = Player of PlayerId * PlayerCount * Direction

module Player =

    let next (Player(PlayerId p, PlayerCount count, d)) = 
        match d with
        | Clockwise ->
            Player(PlayerId ((p+1) % count), PlayerCount count , d)
        | CounterClockwise ->
            Player(PlayerId ((p-1 + count) % count), PlayerCount count ,d)


    let skip p = p |> next |> next

    let init count =
        Player(PlayerId 0, count, Clockwise)

    let reverse (Player(p,c,d)) =
        match d with
        | Clockwise ->
            Player(p,c, CounterClockwise)
        | CounterClockwise ->
            Player(p,c, Clockwise)

    let set id (Player(_, count, d)) =
        Player(id, count, d) 

    let id (Player(p, _, _)) = p


type Command = 
    | StartGame of StartGame
    | PlayCard of PlayCard

and StartGame = {
    Players: PlayerCount
    FirstCard: Card }

and PlayCard = {
    Player: PlayerId
    Card: Card
}

type Event = 
    | GameStarted of GameStarted
    | CardPlayed of CardPlayed
    | WrongCardPlayed of CardPlayed
    | PlayerPlayedAtWrongTurn of CardPlayed
    | TurnBegan of TurnBegan

and GameStarted = {
    Players: PlayerCount
    FirstCard: Card
}
and CardPlayed = {
    Player: PlayerId
    Card: Card
}
and TurnBegan = {
    Player: PlayerId
}

type State = 
    | InitialState
    | Started of Started
and Started = {
    TopCard: Card
    CurrentPlayer: Player
    }

type GameError = 
    | TooFewPlayers
    | GameAlreadyStarted
    | GameNotStarted

type Decide = Command -> State -> Result<Event list, GameError>
type Evolve = State -> Event -> State

// Step 1:
// Make the simplest implementation for the following signature
// Command -> State -> Event list Result

type CardValue =
    | DigitValue of Digit
    | SkipValue
    | KickbackValue

let (|Color|) card =
    match card with
    | Digit(_,color)
    | Skip color 
    | Kickback color->
        color

let (|Value|) card =
    match card with
    |Digit(value, _) -> DigitValue value
    | Skip _ -> SkipValue 
    | Kickback _ -> KickbackValue


let (|SameColor|_|) (cx,cy) = 
    match cx, cy with
    | Color x, Color y when x = y -> Some()
    | _ -> None

let (|SameValue|_|) = function
    | Value x, Value y when x = y -> Some()
    | _ -> None

let nextPlayer card p =
    match card with
    | Value SkipValue -> p |> Player.skip
    | Value (DigitValue _) -> p |> Player.next
    | Value KickbackValue -> p |> Player.reverse |> Player.next
    
let decide : Decide = fun command state ->
    match state, command with
    | _ , StartGame c when c.Players < PlayerCount 2 ->
        Error TooFewPlayers

    | Started _ , StartGame _ ->
        Error GameAlreadyStarted
        
    | _, StartGame c ->
        Ok [ GameStarted { Players = c.Players; FirstCard = c.FirstCard }
             TurnBegan { Player = Player.init c.Players |> nextPlayer c.FirstCard |> Player.id } ]
        
    | InitialState, PlayCard _ ->
        Error GameNotStarted

    | Started s, PlayCard c when
        Player.id s.CurrentPlayer <>  c.Player ->
            match s.TopCard, c.Card with
            | SameColor & SameValue ->
                Ok [ CardPlayed { Player = c.Player; Card = c.Card}  ]
            | _ -> Ok [ PlayerPlayedAtWrongTurn { Player = c.Player; Card = c.Card }]

    | Started s, PlayCard c when 
            match s.TopCard, c.Card with
            | SameColor | SameValue -> true
            | _ -> false
            ->

        
        Ok [ CardPlayed { Player = c.Player; Card = c.Card }
             TurnBegan { Player = s.CurrentPlayer |> nextPlayer c.Card |> Player.id } ]

    | Started _, PlayCard c ->
        Ok [ WrongCardPlayed { Player = c.Player; Card = c.Card}]


    | _ -> Ok []

// Step 2:
// Make the simplest implementation for the following signature
// State -> Event list -> State
// s -> Event [] -> sbyte

//  (s + ([a;b;c] @ [e;f;g])) = (s + [a;b;c]) + [e; f; g]) 
let evolve : Evolve =
    fun state event -> 
        match state, event with
        | _, GameStarted e -> Started { TopCard = e.FirstCard; CurrentPlayer = Player.init e.Players }
        | Started s, CardPlayed e ->
            Started { s with TopCard = e.Card}
        | Started s, TurnBegan e ->
            Started { s with CurrentPlayer = s.CurrentPlayer |> Player.set e.Player }
        
        | _ -> state


