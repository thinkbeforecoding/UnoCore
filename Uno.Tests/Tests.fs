module Tests

#if INTERACTIVE
#load "../.paket/load/netstandard2.0/main.group.fsx"
#I "../Uno"
#load "Model.fs" "Game.fs"
#endif

open Game
open Swensen.Unquote
open Xunit

let p = PlayerId    
let dealer = p 0





// Step 3:
// Implement => to make the test run
let (=>) events command =
    events
    |> List.fold evolve InitialState
    |> decide command 



let (==) = (=)



// Step 4:
// Change the decide function to make this test pass
[<Fact>]
let ``04: Game should start``() =
    test
        <@  []
            => StartGame { Players = Players 3; FirstCard = Three * Red}
            == [ GameStarted { Players = Players 3; FirstCard = Three * Red }
                 TurnStarted { Player = p 1 } ]  @> 
                
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
    test
        <@  [ GameStarted { Players = Players 3; FirstCard = Three * Red }
              TurnStarted { Player = p 1 } ]
            => PlayCard { Player = p 1; Card = Three * Yellow }
            == [ CardPlayed { Player = p 1; Card = Three * Yellow }
                 TurnStarted { Player = p 2 }] @> 

[<Fact>]
let ``07b: Card with same color can be played``() =
    test
        <@  [ GameStarted { Players = Players 3; FirstCard = Three * Red }
              TurnStarted { Player = p 1 } ]
            => PlayCard { Player = p 1; Card = Seven * Red }
            == [ CardPlayed { Player = p 1; Card = Seven * Red }
                 TurnStarted { Player = p 2 } ] @> 

// Step 8:
// Make this test pass
[<Fact>]
let ``08: Card can be played only once game is started``() =
    raises<GameNotStarted>
        <@  []
            => PlayCard { Player = p 1; Card = Seven * Red } @> 

// Step 9:
// What happens here ?!
[<Fact>]
let ``09: Card should be same color or same value``() =
    test
        <@  [ GameStarted { Players = Players 3; FirstCard = Three * Red }
              TurnStarted { Player = p 1 } ]
            => PlayCard { Player = p 1; Card = Seven * Yellow }
            == [ WrongCardPlayed { Player = p 1;Card = Seven * Yellow } ] @> 

      // ...

// Step 10:
// What happens here ?!
[<Fact>]
let ``10: Player should play during his turn``() =
    test
        <@  [ GameStarted { Players = Players 3; FirstCard = Three * Red }
              TurnStarted { Player = p 1 } ]
            => PlayCard { Player = p 3; Card = Seven * Red }
            == [ PlayerPlayedAtWrongTurn { Player = p 3;Card = Seven * Red } ] @> 

// Step 11:
// Testing a full round
[<Fact>]
let ``11: The after a table round, the dealer plays``() =
    test
        <@  [ GameStarted { Players = Players 3; FirstCard = Three * Red }
              TurnStarted { Player = p 1 }
              CardPlayed { Player = p 1; Card = Seven * Red }
              TurnStarted { Player = p 2}
              CardPlayed { Player = p 2; Card = Seven * Green }
              TurnStarted { Player = p 0} ]
            => PlayCard { Player = p 0; Card = Four * Green }
            == [ CardPlayed { Player = p 0;Card = Four * Green }
                 TurnStarted { Player = p 1 } ] @> 

// Step 12:
[<Fact>]
let ``12: The after a table round, the dealer turn start``() =
    // Look at the evolve function...
    // It starts to contains logic.
    // Try to remove the logic from the evolve function 
    // to put it back in the decide function 
    test
        <@  [ GameStarted { Players = Players 3; FirstCard = Three * Red }
              TurnStarted { Player = p 1 }
              CardPlayed { Player = p 1; Card = Seven * Red }
              TurnStarted { Player = p 2} ]
            => PlayCard { Player = p 2; Card = Seven * Green }
            == [ CardPlayed { Player = p 2;Card = Seven * Green }
                 TurnStarted { Player = p 0 } ] @> 

// Step 13:
// Make this test pass
[<Fact>]
let ``13: Player can interrupt``() =
    test
        <@  [ GameStarted { Players = Players 3; FirstCard = Three * Red }
              TurnStarted { Player = p 1 }
              CardPlayed { Player = p 1; Card = Seven * Red }
              TurnStarted { Player = p 2 }]
            => PlayCard { Player = p 0; Card = Seven * Red }
            == [ InterruptionSucceeded { Player = p 0;Card = Seven * Red }] @> 
            // Player 0 could play her card because it same value and same color
            // but it doesn't change current player turn

// Step 14:
// Missing an interrupt is not concidered as playing at the wrong turn.
// So what happens here ?
[<Fact>]
let ``14: Player get no penalty when missing an interrupt``() =
    test
        <@  [ GameStarted { Players = Players 3; FirstCard = Three * Red }
              TurnStarted { Player = p 1 }
              CardPlayed { Player = p 1; Card = Seven * Red }
              TurnStarted { Player = p 2 }
              CardPlayed { Player= p 2; Card = Seven * Green } ]
            => PlayCard { Player = p 0; Card = Seven * Red }
            == [ InterruptionMissed { Player = p 0;Card = Seven * Red }] @> 


// Step 15:
// Uncomment the Kickback card and implement it.
// The kickback changes the direction of the game.





/// This is a property-based test to check 
/// that serialization can round trip
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

    
