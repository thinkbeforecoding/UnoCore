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
let read (store:IEventStoreConnection) deserialize: Read<_> =
    fun (StreamId stream) (EventNumber version) ->
        let rec readSlice version =
            async {
                let! slice =
                    store.ReadStreamEventsForwardAsync(stream, version, 1000, true )
                    |> Async.AwaitTask

                let events =
                    slice.Events
                    |> Array.toList
                    |> List.collect(ofEvent >> deserialize)
                
                if slice.IsEndOfStream then
                    return Slice(events, Last(EventNumber slice.LastEventNumber))
                else
                    return Slice(events, Next (readSlice slice.NextEventNumber) )
            }
        readSlice version

/// Append events to the event store
let append (store: IEventStoreConnection) serialize : _ Append =
    fun (StreamId stream) (EventNumber expectedVersion) (events: Event list) ->
        async {
            let eventData =
                events
                |> List.map (serialize >> toEvent)
                |> List.toArray
                
            let! result =
                store.AppendToStreamAsync(stream,expectedVersion,null,eventData)
                |> Async.AwaitTask
            return EventNumber result.NextExpectedVersion
        }
