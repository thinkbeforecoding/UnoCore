module Game

[<Struct>]
type PlayerCount = PlayerCount of int

[<Struct>]
type PlayerId = PlayerId of int

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

type GameError =
    | GameAlreadyStarted

type Decide = Command -> State -> Result<Event list, GameError>
type Evolve = State -> Event -> State

// Step 1: find the minimal implementation that doesn't raise an error

let decide : Decide = fun  _ _ -> failwith "Not Implemented"
       
let evolve : Evolve =
    fun _ _ -> failwith "Not Implemented"

