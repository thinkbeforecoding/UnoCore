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

let p = PlayerId    

let dealer = p 0

// Step 4:
// Change the decide function to make this test pass
[<Fact>]
let ``04: Game should start``() =
    test
        <@  []
            => StartGame { Players = Players 3; FirstCard = Three * Red}
            == [ GameStarted { Players = Players 3; FirstCard = Three * Red } ] @> 
                
// Step 5:
// Change the decide function to make this test pass
[<Fact>]
let ``05: Playing alone is not fun``() =
    raises<TooFewPlayers>
        <@
          []
          => StartGame { Players = Players 1; FirstCard = Seven * Yellow } @>

// Step 6:
// What should you change to make this test pass ?
[<Fact>]
let ``06: Game should not be started twice``() =
    raises<GameAlreadyStarted>
        <@ [ GameStarted { Players= Players 2; FirstCard = Nine * Yellow } ]
           => StartGame { Players = Players 3; FirstCard = Three * Red } @>


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
let ``11: The after a table round, the dealer plays``() =
        failwith "Not implemented"

// Step 12:
[<Fact>]
let ``12: The after a table round, the dealer turn start``() =
        failwith "Not implemented"
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


open FsCheck.Xunit
open FsCheck
type Arbs =
    static member players() =
        Gen.choose(2,9)
        |> Gen.map Players
        |> Arb.fromGen

    static member player() =
        Gen.choose(0,9)
        |> Gen.map PlayerId
        |> Arb.fromGen

[<Property(Arbitrary = [|typeof<Arbs>|])>]
let ``Event serialization roundtrips`` e =
    let result = 
        e
        |> Serialisation.GameEvents.serialize
        |> Serialisation.GameEvents.deserialize
    test <@ result = [ e ] @>

    
