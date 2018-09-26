module Game

type [<Struct>] PlayerCount = PlayerCount of int


type [<Struct>] PlayerId = PlayerId of int

type Command = 
    | StartGame of StartGame

and StartGame = {
    Players: PlayerCount
    FirstCard: Card }

type Event = 
    | GameStarted of GameStarted
    | CardPlayed of CardPlayed

and GameStarted = {
    Players: PlayerCount
    FirstCard: Card
}
and CardPlayed = {
    Player: PlayerId
    Card: Card
}

type State = 
    | InitialState

type GameError = Unit
    // | GameAlreadyStarted

type Decide = Command -> State -> Result<Event list, GameError>
type Evolve = State -> Event -> State

// Step 1:
// Make the simplest implementation for the following signature
// Command -> State -> Event list Result

let decide : Decide = fun  command state -> failwith "Not Implemented" // failwith "Not Implemented"

// Step 2:
// Make the simplest implementation for the following signature
// State -> Event list -> State
// s -> Event [] -> sbyte

//  (s + ([a;b;c] @ [e;f;g])) = (s + [a;b;c]) + [e; f; g]) 
let evolve : Evolve =
    fun state event -> failwith "Not Implemented"

