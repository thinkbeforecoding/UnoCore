module CommandHandler

[<Struct>]
type StreamId = StreamId of string

[<Struct>]
type EventNumber = EventNumber of int64
    with
    static member Start = EventNumber 0L



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
            | Next next -> return! fold f s next }


type Read<'e> = StreamId -> EventNumber -> EventStream<'e, EventNumber>
type Append<'e> = StreamId -> EventNumber -> 'e list -> EventNumber Async


open Game
let handler (read: _ Read) (append: _ Append) stream command =
    let rec load initialState startVersion =
        read stream startVersion |>EventStream.fold evolve initialState 
    async {
        let! game, version = load InitialState EventNumber.Start
        match decide command game with
        | Ok newEvents ->
            let! _ = append stream version newEvents
            return Ok()
        | Error error -> 
            return Error error }

type Agent<'t> = MailboxProcessor<'t>

module EventNumber =
    let dist (EventNumber x) (EventNumber y) = x - y  

let game read append loadSnapshot writeSnapshot stream =
    Agent.Start (fun mailbox ->
        let rec loop state version snapshotVersion =
            async {
                let! command, reply = mailbox.Receive()
                let newEvents = decide command state
                match newEvents with
                |Ok newEvents ->
                    let! newVersion = append stream version newEvents
                    let newState = List.fold evolve state newEvents
                    reply (Ok())

                    let newSnapshotVersion =
                        if EventNumber.dist newVersion snapshotVersion > 1000L then
                            writeSnapshot "snap-game" newState newVersion
                            newVersion
                        else
                            snapshotVersion

                    return! loop newState newVersion newSnapshotVersion
                | Error err ->
                    reply (Error err)
                    return! loop state version snapshotVersion    }
        let rec load initialState startVersion =
                read stream startVersion
                |> EventStream.fold evolve initialState
        async {
            let! snapshotState, snapshotVersion = loadSnapshot "snap-game"
            let! state, version = load snapshotState snapshotVersion
            return! loop state version snapshotVersion }) 

// load InitialState 0 |> writeSnapshot "..."

// let game1 = game x x "Game-1"
// async {
//     let! result = game1.PostAndAsyncReply(fun channel -> StartGame { Players = 4; FirstCard = Digit(Five,Red) }, channel.Reply)
//     let! otherResult =
//         game1.PostAndAsyncReply (fun channel -> PlayCard { Player = 1; Card = Digit(Three,Red) }, channel.Reply)
// } |> Async.Start


module EventStore =
    open EventStore.ClientAPI
    open Serialisation

    let ofEvent (event: ResolvedEvent) : SerializedEvent =
        let eventType = event.Event.EventType
        let data = System.Text.Encoding.UTF8.GetString(event.Event.Data)
        struct(eventType, data)

    let toEvent (struct(eventType, data): SerializedEvent) =
        EventData(
            System.Guid.NewGuid(),
            eventType,
            true,
            System.Text.Encoding.UTF8.GetBytes(data: string),
            null)
 

    let read (store:IEventStoreConnection) : Read<_> =
        fun (StreamId stream) (EventNumber version) ->
            let rec readSlice version =
                async {
                    let! slice =
                        store.ReadStreamEventsForwardAsync(stream, version, 1000, true )
                        |> Async.AwaitTask

                    let events =
                        slice.Events
                        |> Array.toList
                        |> List.collect(ofEvent >> GameEvents.deserialize)
                    
                    if slice.IsEndOfStream then
                        return Slice(events, Last(EventNumber slice.LastEventNumber))
                    else
                        return Slice(events, Next (readSlice slice.NextEventNumber) )
                }
            readSlice version

    let append (store: IEventStoreConnection) : _ Append =
        fun (StreamId stream) (EventNumber expectedVersion) (events: Event list) ->
            async {
                let eventData =
                    events
                    |> List.map(GameEvents.serialize >> toEvent)
                    |> List.toArray
                    
                let! result =
                    store.AppendToStreamAsync(stream,expectedVersion,null,eventData)
                    |> Async.AwaitTask
                return EventNumber result.NextExpectedVersion
            }
