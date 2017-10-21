module EventStore

open EventStore.ClientAPI
open Serialisation
open Game
open CommandHandler

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
