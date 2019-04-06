module Uno
open EventStore.ClientAPI
open EventStore.ClientAPI.SystemData
open Game
open System
open Serialisation

[<EntryPoint>]
let main argv =
    printfn "%A" argv

    async {
        let settings = 
            ConnectionSettings
                .Create()
                .SetDefaultUserCredentials(UserCredentials("admin", "changeit"))
                .Build()

        let store = EventStoreConnection.Create(settings, Uri "tcp://localhost:1113")
        do! store.ConnectAsync() |> Async.AwaitTask
        let read = EventStore.read store GameEvents.deserialize
        let append = EventStore.append store GameEvents.serialize
        let handler = CommandHandler.handler read append

        let streamId = CommandHandler.StreamId "Game-1"

        try
            handler streamId (StartGame { Players = Players 4; FirstCard = Five * Blue })
            handler streamId (PlayCard { Player = PlayerId 1;  Card = Seven * Blue })
            handler streamId (PlayCard { Player = PlayerId 2; Card = Seven * Red })
            handler streamId (PlayCard { Player = PlayerId 3; Card = Four * Red })
            handler streamId (PlayCard { Player = PlayerId 0; Card = Four * Red })

            printfn "Ok done"
        with
        | err -> printfn "Error: %A" err

        //let loadSnap = EventStore.loadSnapshot store Snapshot.deserialize
        //let saveSnap = EventStore.saveSnapshot store Snapshot.serialize
        //let handler = CommandHandler.game read append loadSnap saveSnap

        let handler = CommandHandler.game read append

        let agent = handler (CommandHandler.StreamId "Game-2") 
        
        let! result' = agent |> CommandHandler.send (StartGame { Players = Players 4; FirstCard = Nine * Yellow})
        let! result' = agent |> CommandHandler.send (PlayCard { Player = PlayerId 1; Card = Nine * Green })
        let! result' = agent |> CommandHandler.send (PlayCard { Player = PlayerId 2; Card = One * Green })
        let! result' = agent |> CommandHandler.send (PlayCard { Player = PlayerId 1; Card = One * Green }) // this is an interruption
        let! result' = agent |> CommandHandler.send (PlayCard { Player = PlayerId 3; Card = Six * Green })
        let! result' = agent |> CommandHandler.send (PlayCard { Player = PlayerId 0; Card = Six * Blue })
        let! result' = agent |> CommandHandler.send (PlayCard { Player = PlayerId 1; Card = Seven * Blue })
        let! result' = agent |> CommandHandler.send (PlayCard { Player = PlayerId 3; Card = Six * Blue }) // this is a missed interruption
        let! result' = agent |> CommandHandler.send (PlayCard { Player = PlayerId 2; Card = Two * Blue }) 

        match result' with
        | Ok () -> printfn "Ok done"
        | Error err -> printfn "Error: %A" err

        store.Close()

    } |> Async.RunSynchronously

    0 // return an integer exit code
