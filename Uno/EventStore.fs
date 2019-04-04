module EventStore

open System
open EventStore.ClientAPI
open Serialisation
open Game
open CommandHandler

let utf8 = System.Text.Encoding.UTF8

let ofEvent (event: ResolvedEvent) : SerializedEvent =
    let eventType = event.Event.EventType
    let data = utf8.GetString(event.Event.Data)
    eventType, data

let toEvent ((eventType, data): SerializedEvent) =
    EventData(
        Guid.NewGuid(),
        eventType,
        true,
        utf8.GetBytes(data: string),
        null)

/// Read events from the event store
let read (store:IEventStoreConnection) deserialize: _ Read =
    fun (StreamId stream) (EventNumber version) ->
        let slice =
            store.ReadStreamEventsForwardAsync(stream, version, 5000, true )
            |> Async.AwaitTask
            |> Async.RunSynchronously

        let events =
            slice.Events
            |> Array.toList
            |> List.collect(ofEvent >> deserialize)
        
        events, EventNumber slice.LastEventNumber


/// Append events to the event store
let append (store: IEventStoreConnection) serialize : _ Append =
    fun (StreamId stream) (EventNumber expectedVersion) (events: Event list) ->
        let eventData =
            events
            |> List.map (serialize >> toEvent)
            |> List.toArray
            
        let result =
            store.AppendToStreamAsync(stream,expectedVersion,null,eventData)
            |> Async.AwaitTask
            |> Async.RunSynchronously

        EventNumber result.NextExpectedVersion

let loadSnapshot (store: IEventStoreConnection) (deserialize: string -> ('s * EventNumber) option) : 's LoadSnapshot =
    fun (StreamId stream) ->
        let slice =
            store.ReadStreamEventsBackwardAsync(stream, int64 StreamPosition.End, 1, false)
            |> Async.AwaitTask
            |> Async.RunSynchronously
    
        match slice.Events with
        | [| e |] -> 
            let _, d = ofEvent e
            deserialize d
        | _ -> None

let saveSnapshot (store: IEventStoreConnection) (serialize: 's -> EventNumber -> string) : 's SaveSnapshot =
    fun (StreamId stream) state version ->
        let data = toEvent("Snapshot", serialize state version)
        store.AppendToStreamAsync(stream, ExpectedVersion.Any, data)
        |> Async.AwaitTask
        |> Async.RunSynchronously
        |> ignore
