module CommandHandler

[<Struct>]
type StreamId = StreamId of string

[<Struct>]
type EventNumber = EventNumber of int64
    with
    static member Start = EventNumber 0L
    static member (+) (EventNumber n,x) = EventNumber (n+x) 

type EventStream<'e,'v> = Slice<'e,'v> Async
and Slice<'e,'v> = Slice of 'e list * Continuation<'e,'v>
and Continuation<'e,'v> =
    | Next of EventStream<'e,'v>
    | Last of 'v

module EventStream =
    let rec fold f s (sliced: EventStream<_,_>) =
        async {
            let! (Slice (es, continuation)) = sliced
            let s' = List.fold f s es
            match continuation with
            | Last v -> return s',v
            | Next next -> return! fold f s' next }


type Read<'e> = StreamId -> EventNumber -> EventStream<'e, EventNumber>
type Append<'e> = StreamId -> EventNumber -> 'e list -> EventNumber Async


open Game

// Step 16:
// Implement the command handler
let handler (read: _ Read) (append: _ Append) (*readSnapshot saveSnapshot*) stream command =
    async {
(*        let! startVersion, startState =
            async {
            match! readSnapshot stream with
            | Some(snapVersion, snapshot) ->
                return snapVersion, snapshot
            | None -> return EventNumber.Start, InitialState } *)
        let! state, version = 
            read stream EventNumber.Start (*startVersion*)
            |> EventStream.fold evolve InitialState (*startState*)
        match decide command state with
        | Error e ->
            return Error e
        | Ok events ->
            let! nextVersion = append stream version events
            (*if version > startVersion + 100L then
                saveSnapshot stream (version, state)  *)

            return Ok events
    }


type Agent<'t> = MailboxProcessor<'t>

let inMemoryhandler (read: _ Read) (append: _ Append) stream =
    Agent.Start <| fun mailbox ->
        let rec loop state version =
            async {
                let! command = mailbox.Receive()
                match decide command state with
                | Error e ->
                    // notify someone..
                    return! loop state version
                | Ok events ->
                    let! newVersion = append stream version events
                    let newState = 
                        events
                        |> List.fold evolve state 

                    return! loop newState newVersion }
        async { 
            let! state, version =
                read stream EventNumber.Start 
                |> EventStream.fold evolve InitialState
            return! loop state version
        }





 















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


