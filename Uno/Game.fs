module Game


type [<Struct>] PlayerCount = Players of int

type [<Struct>] PlayerId = PlayerId of int



type Command = 
| StartGame of StartGame
| PlayCard of PlayCard

and StartGame = {
    Players: PlayerCount
    FirstCard: Card 
}

and PlayCard = {
    Player: PlayerId
    Card: Card
}


type Event = 
| GameStarted of GameStarted
| CardPlayed of CardPlayed
| WrongCardPlayed of CardPlayed
| PlayerPlayedAtWrongTurn of CardPlayed
| TurnStarted of TurnStarted
| InterruptionSucceeded of CardPlayed
| InterruptionMissed of CardPlayed

and GameStarted = {
    Players: PlayerCount
    FirstCard: Card
}

and CardPlayed = {
    Player: PlayerId
    Card: Card;
}

and TurnStarted = {
    Player: PlayerId
}

type Turn = {
    Player: PlayerId
    Count: PlayerCount
}

module Turn =
    let dealer count =
        { Player = PlayerId 0
          Count = count}

    let next turn =
        let (PlayerId p) = turn.Player 
        let (Players c) = turn.Count
        { turn with Player = PlayerId ((p + 1) % c)}
    
    let player turn = turn.Player

type Deck = {
    Top: Card
    Next: Card option
}

module Deck =
    let turnFirstCard card = { Top = card; Next = None}
    let put card deck = 
        { Top = card; Next = Some deck.Top }

type State = 
| InitialState
| Started of Started

and Started = {
    Deck: Deck
    Turn: Turn
}



exception TooFewPlayers
exception GameAlreadyStarted
exception GameNotStarted















type Decide = Command -> State -> Event list
















type Evolve = State -> Event -> State


















// Step 1:
// Make the simplest implementation for the
// following signature
// Command -> State -> Event list Result

let inInitialState command =
    match command with
    | StartGame c when c.Players < Players 2->
            raise TooFewPlayers
    | StartGame c ->
        [ GameStarted { Players = c.Players; FirstCard = c.FirstCard }
          TurnStarted { Player = Turn.dealer c.Players |> Turn.next |> Turn.player } ]

    | _ -> raise GameNotStarted

let isInterrupt card deck = card = deck.Top
let isMissedInterrupt card deck = deck.Next = Some card 

let inStartedState s command =
    match command with
    | StartGame _ -> raise GameAlreadyStarted
    | PlayCard c ->
        if c.Player <> s.Turn.Player then
            if isInterrupt c.Card s.Deck then
                [ InterruptionSucceeded { Card = c.Card; Player = c.Player } ]
            elif isMissedInterrupt c.Card s.Deck then
                [ InterruptionMissed { Card = c.Card; Player = c.Player} ]
            else
                [ PlayerPlayedAtWrongTurn { Card = c.Card; Player = c.Player }]
        else
            match s.Deck.Top, c.Card with
            | Digit(d1,c1), Digit(d2, c2) 
                when d1 = d2 || c1 = c2 ->
                [ CardPlayed { Card = c.Card; Player = c.Player }
                  TurnStarted { Player = s.Turn |> Turn.next |> Turn.player }
                ]
            | _ -> [ WrongCardPlayed { Card = c.Card; Player = c.Player }]


let decide : Decide = 
    fun command state ->
        match state with
        | InitialState -> inInitialState command
        | Started s -> inStartedState s command

// Step 2:
// Make the simplest implementation for the following signature
// State -> Event list -> State
// s -> Event [] -> sbyte

//  (s + ([a;b;c] @ [e;f;g])) = (s + [a;b;c]) + [e; f; g]) 
let evolve : Evolve =

    fun state event -> 
        match state, event with
        | _, GameStarted e -> Started { Deck = Deck.turnFirstCard e.FirstCard; Turn = Turn.dealer e.Players }
        | Started s, (CardPlayed e | InterruptionSucceeded e) -> Started { s with Deck = s.Deck |> Deck.put e.Card }
        | Started s, TurnStarted e -> Started { s with Turn = { s.Turn with Player = e.Player } }
        | _ -> state

