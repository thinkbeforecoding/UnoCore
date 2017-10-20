module Tests

open Expecto
open Game

let (=>) events command =
  events
  |> List.fold evolve InitialState
  |> decide command

let (==) result expected =
    Expect.equal result (Ok expected) "Should equal expected"

let (=!) result expected =
    Expect.equal result (Error expected) "Should be expected error"
    

[<Tests>]
let tests =
  testList "samples" [
    testCase "Game should start" <| fun _ ->
        []
        => StartGame { Players = PlayerCount 3; FirstCard = Digit(Three, Red)}
        == [ GameStarted { Players = PlayerCount 3; FirstCard = Digit(Three, Red) } ]

    testCase "Game cannot be started twice" <| fun _ ->
        [ GameStarted { Players= PlayerCount 2; FirstCard = Digit(Nine, Yellow) } ]
        => StartGame { Players = PlayerCount 3; FirstCard = Digit(Three, Red)}
        =! GameAlreadyStarted

    // testCase "Playing same color should be ok" <| fun _ ->
    //     ...
    // testCase "Playing same value should be ok" <| fun _ ->
    //     ...
    // testCase "Playing different color and different value should be rejected" <| fun _ ->
    //     ...
    // testCase "Player can play at his turn" <| fun _ ->
    //     ...
    // testCase "Player cannnot play outside of his turn" <| fun _ ->
    //     ...
        
  ]