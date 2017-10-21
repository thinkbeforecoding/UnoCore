module Tests

open Expecto
open Game

// Step 3:
// Implement => to make the test run
let (=>) events command = failwith "Not implemented"

let (==) result expected =
    Expect.equal result (Ok expected) "Should equal expected"

let (=!) result expected =
    Expect.equal result (Error expected) "Should be expected error"
    

[<Tests>]
let tests =
  testList "samples" [
    // Step 4:
    // Change the decide function to make this test pass
    testCase "Game should start" <| fun _ ->
        []
        => StartGame { Players = PlayerCount 3; FirstCard = Digit(Three, Red)}
        == [ GameStarted { Players = PlayerCount 3; FirstCard = Digit(Three, Red) } ]

    // Step 5:
    // Change the decide function to make this test pass
    testCase "Playing alone is not fun" <| fun _ ->
      []
      => StartGame { Players = PlayerCount 1; FirstCard = Digit( Seven, Yellow)}
      //=! TooFewPlayers
      

    // Step 6:
    // What should you change to make this test pass ?
    testCase "Game should not be started twice" <| fun _ ->
        [ GameStarted { Players= PlayerCount 2; FirstCard = Digit(Nine, Yellow) } ]
        => StartGame { Players = PlayerCount 3; FirstCard = Digit(Three, Red)}
        =! GameAlreadyStarted


    // Step 7:
    // Make this two tests pass... doing the simplest thing that work
    testCase "Card with same value can be played" <| fun _ ->
        failwith "Not implemented"

    testCase "Card with same color can be played" <| fun _ ->
        failwith "Not implemented"

    // Step 8:
    // Make this test pass
    testCase "Card can be played only once game is started" <| fun _ ->
        failwith "Not implemented"

    // Step 9:
    // What happens here ?!
    testCase "Card should be same color or same value" <| fun _ ->
        failwith "Not implemented"
      // ...

    // Step 10:
    // What happens here ?!
    testCase "Player should play during his turn" <| fun _ ->
        failwith "Not implemented"

    // Step 11:
    // Testing a full round
    testCase "The after a table round, the dealer plays" <| fun _ ->
        failwith "Not implemented"

    testCase "The after a table round, the dealer turn start" <| fun _ ->
        failwith "Not implemented"
    // Step 12:
    // Look at the evolve function...
    // It starts to contains logic.
    // Try to remove the logic from the evolve function 
    // to put it back in the decide function 

    // Step 13:
    // Make this test pass
    testCase "Player can interrupt" <| fun _ ->
        failwith "Not implemented"

    // Step 14:
    // Missing an interrupt is not concidered as playing at the wrong turn.
    // So what happens here ?
    testCase "Player get no penalty when missing an interrupt" <| fun _ ->
        failwith "Not implemented"


    // Step 15:
    // Uncomment the Kickback card and implement it.
    // The kickback changes the direction of the game.
        
  ]