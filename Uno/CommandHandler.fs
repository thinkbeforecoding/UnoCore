module CommandHandler


[<Struct>]
type StreamId = StreamId of string

[<Struct>]
type EventNumber = EventNumber of int64
    with
    static member Start = EventNumber 0L


type Read<'e> = StreamId -> EventNumber -> 'e List * EventNumber
type Append<'e> = StreamId -> EventNumber -> 'e list -> EventNumber


open Game

// Step 16:
// Implement the command handler
let handler (read: _ Read) (append: _ Append) stream command =
    
    let events, version = read stream EventNumber.Start
    events
    |> List.fold evolve InitialState
    |> decide command
    |> append stream version
    |> ignore




















//let events, v = read stream EventNumber.Start

//events
//|> List.fold evolve InitialState
//|> decide command
//|> append stream v
//|> ignore



// Step 17:
// Add keep state in memory
// Step 18:
// Implement Snapshoting



type Agent<'t> = MailboxProcessor<'t>

let game (read: _ Read) (append: _ Append) stream =
    Agent.Start (fun mailbox ->
         let rec loop state version =
             async {
                 let! command, reply = mailbox.Receive()
                 try 
                     let newEvents = decide command state
                     let newVersion = append stream version newEvents
                     let newState = List.fold evolve state newEvents
                     reply (Ok())

                     return! loop newState newVersion
                 with
                 | err ->
                     reply (Error err)
                     return! loop state version }
         let rec load initialState startVersion =
                 let events, v = read stream startVersion
                 List.fold evolve initialState events, v

         async {
             let state, version = load InitialState EventNumber.Start
             return! loop state version }) 

let send cmd (agent: _ Agent) =
    agent.PostAndAsyncReply(fun r -> cmd, r.Reply)



//type LoadSnapshot<'s> = StreamId -> ('s * EventNumber) option
//type SaveSnapshot<'s> = StreamId -> 's -> EventNumber -> unit

//module EventNumber =
//    let dist (EventNumber x) (EventNumber y) = x - y

//let snapStream (StreamId s) = StreamId ("Snap-" + s)

//let game (read: _ Read) (append: _ Append) (loadSnapshot: _ LoadSnapshot) (saveSnapshot: _ SaveSnapshot) stream =
//    let snapStream = snapStream stream
//    Agent.Start (fun mailbox ->
//         let rec loop state version snapshotVersion =
//             async {
//                 let! command, reply = mailbox.Receive()
//                 try 
//                     let newEvents = decide command state
//                     let newVersion = append stream version newEvents
//                     let newState = List.fold evolve state newEvents
//                     reply (Ok())

//                     let newSnapshotVersion =
//                         if EventNumber.dist newVersion snapshotVersion > 1000L then
//                             saveSnapshot snapStream newState newVersion
//                             newVersion
//                         else
//                             snapshotVersion

//                     return! loop newState newVersion newSnapshotVersion
//                 with
//                 | err ->
//                     reply (Error err)
//                     return! loop state version snapshotVersion    }
//         let rec load initialState startVersion =
//                 let events, v = read stream startVersion
//                 List.fold evolve initialState events, v

//         async {
//             let snapshotState, snapshotVersion =
//                 match loadSnapshot snapStream with
//                 | Some (s, v) -> s,v
//                 | None -> InitialState, EventNumber.Start

//             let state, version = load snapshotState snapshotVersion
//             return! loop state version snapshotVersion }) 

