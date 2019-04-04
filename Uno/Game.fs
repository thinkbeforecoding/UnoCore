module Game

type [<Struct>] PlayerCount = Players of int

type [<Struct>] PlayerId = PlayerId of int


type Command = 
| StartGame of StartGame

and StartGame = {
    Players: PlayerCount
    FirstCard: Card 
}









type Event = 
| GameStarted of GameStarted

and GameStarted = {
    Players: PlayerCount
    FirstCard: Card
}

type State = 
| InitialState





exception TooFewPlayers
exception GameAlreadyStarted















type Decide = Command -> State -> Event list
















type Evolve = State -> Event -> State

// Step 1:
// Make the simplest implementation for the following signature
// Command -> State -> Event list Result

let decide : Decide = 
    fun  command state -> failwith "Not Implemented"
























// Step 2:
// Make the simplest implementation for the following signature
// State -> Event list -> State
// s -> Event [] -> sbyte

//  (s + ([a;b;c] @ [e;f;g])) = (s + [a;b;c]) + [e; f; g]) 
let evolve : Evolve =
    fun state event -> failwith "Not Implemented"

