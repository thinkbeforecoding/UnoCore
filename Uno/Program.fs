module Uno
open EventStore.ClientAPI
open EventStore.ClientAPI.SystemData
open Game
open System

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
        let read = EventStore.read store
        let append = EventStore.append store
        let handler = CommandHandler.handler read append

        let! result = handler (CommandHandler.StreamId "Game-1") (StartGame { Players = PlayerCount 4; FirstCard = Digit(Five, Blue)})
        match result with
        | Ok () -> printfn "Ok done"
        | Error err -> printfn "Error: %A" err

    } |> Async.RunSynchronously

    0 // return an integer exit code
