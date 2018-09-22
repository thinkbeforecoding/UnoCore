module Tests

open Game
open Xunit

// Step 3:
// Implement => to make the test run
let (=>) events command = failwith "Not implemented"

let (==) result expected =
    Assert.Equal(Ok expected, result)

let (=!) result expected =
    Assert.Equal(Error expected, result) 
    

// Step 4:
// Change the decide function to make this test pass
[<Fact>]
let ``Game should start``() =
        []
        => StartGame { Players = PlayerCount 3; FirstCard = Digit(Three, Red)}
        == [ GameStarted { Players = PlayerCount 3; FirstCard = Digit(Three, Red) } ]

// Step 5:
// Change the decide function to make this test pass
[<Fact>]
let ``Playing alone is not fun`` () =
  []
  => StartGame { Players = PlayerCount 1; FirstCard = Digit( Seven, Yellow)}
  //=! TooFewPlayers
  

// Step 6:
// What should you change to make this test pass ?
[<Fact>]
let ``Game should not be started twice``() =
    [ GameStarted { Players= PlayerCount 2; FirstCard = Digit(Nine, Yellow) } ]
    => StartGame { Players = PlayerCount 3; FirstCard = Digit(Three, Red)}
    // =! GameAlreadyStarted


// Step 7:
// Make this two tests pass... doing the simplest thing that work
[<Fact>]
let ``Card with same value can be played``() =
    failwith "Not implemented"

[<Fact>]
let ``Card with same color can be played``() =
    failwith "Not implemented"

// Step 8:
// Make this test pass
[<Fact>]
let ``Card can be played only once game is started``() =
    failwith "Not implemented"

// Step 9:
// What happens here ?!
[<Fact>]
let ``Card should be same color or same value``() =
    failwith "Not implemented"
  // ...

// Step 10:
// What happens here ?!
[<Fact>]
let ``Player should play during his turn``() =
    failwith "Not implemented"

// Step 11:
// Testing a full round
[<Fact>]
let ``The after a table round, the dealer plays``() =
    failwith "Not implemented"

[<Fact>]
let ``The after a table round, the dealer turn start``() =
    failwith "Not implemented"
// Step 12:
// Look at the evolve function...
// It starts to contains logic.
// Try to remove the logic from the evolve function 
// to put it back in the decide function 

// Step 13:
// Make this test pass
[<Fact>]
let ``Player can interrupt``() =
    failwith "Not implemented"

// Step 14:
// Missing an interrupt is not concidered as playing at the wrong turn.
// So what happens here ?
[<Fact>]
let ``Player get no penalty when missing an interrupt``() =
    failwith "Not implemented"


// Step 15:
// Uncomment the Kickback card and implement it.
// The kickback changes the direction of the game.
