module Tests

#if INTERACTIVE
#load "../.paket/load/netstandard2.0/main.group.fsx"
#I "../Uno"
#load "Model.fs" "Game.fs"
#endif

open Game
open Swensen.Unquote
open Xunit

// Step 3:
// Implement => to make the test run
let (=>) events command = failwith "Not implemented"

let (==) = (=)
    

// Step 4:
// Change the decide function to make this test pass
[<Fact>]
let ``04: Game should start``() =
    test
        <@  []
            => StartGame { Players = PlayerCount 3; FirstCard = Digit(Three, Red)}
            == [ GameStarted { Players = PlayerCount 3; FirstCard = Digit(Three, Red) } ] @> 
            
// Step 5:
// Change the decide function to make this test pass
[<Fact>]
let ``05: Playing alone is not fun``() =
    raises<TooFewPlayers>
        <@
          []
          => StartGame { Players = PlayerCount 1; FirstCard = Digit( Seven, Yellow)} @>

    // Step 6:
    // What should you change to make this test pass ?
[<Fact>]
let ``06: Game should not be started twice``() =
    raises<GameAlreadyStarted>
        <@ [ GameStarted { Players= PlayerCount 2; FirstCard = Digit(Nine, Yellow) } ]
           => StartGame { Players = PlayerCount 3; FirstCard = Digit(Three, Red)} @>


    // Step 7:
    // Make this two tests pass... doing the simplest thing that work
[<Fact>]
let ``07a: Card with same value can be played``() =
        failwith "Not implemented"

[<Fact>]
let ``07b: Card with same color can be played``() =
        failwith "Not implemented"

    // Step 8:
    // Make this test pass
[<Fact>]
let ``08: Card can be played only once game is started``() =
        failwith "Not implemented"

    // Step 9:
    // What happens here ?!
[<Fact>]
let ``09: Card should be same color or same value``() =
        failwith "Not implemented"
      // ...

    // Step 10:
    // What happens here ?!
[<Fact>]
let ``10: Player should play during his turn``() =
        failwith "Not implemented"

    // Step 11:
    // Testing a full round
[<Fact>]
let ``11a: The after a table round, the dealer plays``() =
        failwith "Not implemented"

[<Fact>]
let ``11b: The after a table round, the dealer turn start``() =
        failwith "Not implemented"
    // Step 12:
    // Look at the evolve function...
    // It starts to contains logic.
    // Try to remove the logic from the evolve function 
    // to put it back in the decide function 

    // Step 13:
    // Make this test pass
[<Fact>]
let ``13: Player can interrupt``() =
        failwith "Not implemented"

    // Step 14:
    // Missing an interrupt is not concidered as playing at the wrong turn.
    // So what happens here ?
[<Fact>]
let ``14: Player get no penalty when missing an interrupt``() =
        failwith "Not implemented"


    // Step 15:
    // Uncomment the Kickback card and implement it.
    // The kickback changes the direction of the game.
        
