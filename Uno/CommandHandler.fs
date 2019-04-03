module CommandHandler


[<Struct>]
type StreamId = StreamId of string

[<Struct>]
type EventNumber = EventNumber of int64
    with
    static member Start = EventNumber 0L


type Stream<'e> = Slice<'e> Async

and Slice<'e> = Slice of 'e list * Continuation<'e>

and Continuation<'e> =
| Next of Stream<'e>
| Last of EventNumber

module EventStream =

    let rec fold f state (sliced: _ Stream) =
        async {
            let! (Slice (es, continuation)) = sliced
            let s' = List.fold f state es
            match continuation with
            | Last v -> return s',v
            | Next next -> return! fold f s' next }


type Read<'e> = StreamId -> EventNumber -> 'e Stream
type Append<'e> = StreamId -> EventNumber -> 'e list -> EventNumber Async


open Game

// Step 16:
// Implement the command handler
let handler (read: _ Read) (append: _ Append) stream command =
    failwith "Not implemented"



















    // let rec load initialState startVersion =
    //     read stream startVersion
    //     |>EventStream.fold evolve initialState 

    // async {
    //     let! game, version = load InitialState EventNumber.Start
    //     match decide command game with
    //     | Ok newEvents ->
    //         let! _ = append stream version newEvents
    //         return Ok()
    //     | Error error -> 
    //         return Error error }



// Step 17:
// Add keep state in memory
// Step 18:
// Implement Snapshoting


// type Agent<'t> = MailboxProcessor<'t>

// module EventNumber =
//     let dist (EventNumber x) (EventNumber y) = x - y  

// let game read append loadSnapshot writeSnapshot stream =
//     Agent.Start (fun mailbox ->
//         let rec loop state version snapshotVersion =
//             async {
//                 let! command, reply = mailbox.Receive()
//                 let newEvents = decide command state
//                 match newEvents with
//                 |Ok newEvents ->
//                     let! newVersion = append stream version newEvents
//                     let newState = List.fold evolve state newEvents
//                     reply (Ok())

//                     let newSnapshotVersion =
//                         if EventNumber.dist newVersion snapshotVersion > 1000L then
//                             writeSnapshot "snap-game" newState newVersion
//                             newVersion
//                         else
//                             snapshotVersion

//                     return! loop newState newVersion newSnapshotVersion
//                 | Error err ->
//                     reply (Error err)
//                     return! loop state version snapshotVersion    }
//         let rec load initialState startVersion =
//                 read stream startVersion
//                 |> EventStream.fold evolve initialState

//         async {
//             let! snapshotState, snapshotVersion = loadSnapshot "snap-game"
//             let! state, version = load snapshotState snapshotVersion
//             return! loop state version snapshotVersion }) 


