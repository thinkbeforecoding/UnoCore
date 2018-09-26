module Tests

open Game
open Xunit

// Step 3:
// Implement => to make the test run
let (=>) events command = 
    events
    |> List.fold evolve InitialState
    |> decide command

let (==) result expected =
    Assert.Equal(Ok expected, result)

let (=!) result expected =
    Assert.Equal(Error expected, result) 
    
let notImplemented() : unit =
    failwith "Not implemented"

// Step 4:
// Change the decide function to make this test pass
[<Fact>]
let ``Game should start``() =
    []
    => StartGame { Players = PlayerCount 3; FirstCard = Digit(Three, Red)}
    == [ GameStarted { Players = PlayerCount 3; FirstCard = Digit(Three, Red) } 
         TurnBegan {Player = PlayerId 1;} ]

// Step 5:
// Change the decide function to make this test pass
[<Fact>]
let ``Playing alone is not fun`` () =
    []
    => StartGame { Players = PlayerCount 1; FirstCard = Digit( Seven, Yellow)}
    =! TooFewPlayers

// Step 6:
// What should you change to make this test pass ?
[<Fact>]
let ``Game should not be started twice``() =
    [ GameStarted { Players= PlayerCount 2; FirstCard = Digit(Nine, Yellow) } ]
    => StartGame { Players = PlayerCount 3; FirstCard = Digit(Three, Red)}
    =! GameAlreadyStarted


// Step 7:
// Make this two tests pass... doing the simplest thing that work
[<Fact>]
let ``Card with same value can be played``() =
    [ GameStarted { Players = PlayerCount 3; FirstCard = Digit(Three, Red)} 
      TurnBegan { Player = PlayerId 1 } ]
    =>  PlayCard { Player = PlayerId 1; Card = Digit(Three, Blue)}
    == [ CardPlayed { Player = PlayerId 1; Card = Digit(Three, Blue) }
         TurnBegan {Player = PlayerId 2;} ]

[<Fact>]
let ``Card with same value can be played after game started``() =
    [ GameStarted { Players = PlayerCount 3; FirstCard = Digit(Three, Red)} 
      CardPlayed { Player = PlayerId 1; Card = Digit(Three, Blue) }
      TurnBegan { Player = PlayerId 2}]
    =>  PlayCard { Player = PlayerId 2; Card = Digit(Seven, Blue)}
    == [ CardPlayed { Player = PlayerId 2; Card = Digit(Seven, Blue)}
         TurnBegan {Player = PlayerId 0;} ]


[<Fact>]
let ``Card with same color can be played``() =
    [ GameStarted { Players = PlayerCount 3; FirstCard = Digit(Three, Red)} 
      TurnBegan { Player = PlayerId 1}]
    =>  PlayCard { Player = PlayerId 1; Card = Digit(Seven, Red)}
    == [ CardPlayed { Player = PlayerId 1; Card = Digit(Seven, Red) } 
         TurnBegan {Player = PlayerId 2;}]

// Step 8:
// Make this test pass
[<Fact>]
let ``Card can be played only once game is started``() =
    []
    => PlayCard { Player = PlayerId 1; Card = Digit(Eight, Green)}
    =! GameNotStarted

// Step 9:
// What happens here ?!
[<Fact>]
let ``Card should be same color or same value``() =
    [ GameStarted { Players = PlayerCount 3; FirstCard = Digit(Two, Blue)} 
      TurnBegan { Player = PlayerId 1}]
    => PlayCard { Player = PlayerId 1; Card = Digit( Nine, Yellow)}
    == [ WrongCardPlayed { Player = PlayerId 1; Card = Digit(Nine, Yellow) } ]
    // ...

// Step 10:
// What happens here ?!
[<Fact>]
let ``Player should play during his turn``() =
    [ GameStarted { Players = PlayerCount 3; FirstCard = Digit(Five, Yellow) }
      CardPlayed { Player = PlayerId 1; Card = Digit(Four, Yellow)}
      TurnBegan { Player = PlayerId 2}
    ]
    => PlayCard { Player = PlayerId 1; Card = Digit(Five, Yellow) }
    == [ PlayerPlayedAtWrongTurn { Player = PlayerId 1; Card = Digit(Five, Yellow)}]

// Step 11:
// Testing a full round
[<Fact>]
let ``The after a table round, the dealer plays``() =
    [ GameStarted { Players = PlayerCount 3; FirstCard = Digit(Five, Yellow) }
      CardPlayed { Player = PlayerId 1; Card = Digit(Four, Yellow)}
      CardPlayed { Player = PlayerId 2; Card = Digit(Four, Green)}
      TurnBegan { Player = PlayerId 0}
    ]
    => PlayCard { Player = PlayerId 0; Card = Digit(Nine, Green) }
    == [ CardPlayed { Player = PlayerId 0; Card = Digit(Nine, Green)}
         TurnBegan {Player = PlayerId 1}]


[<Fact>]
let ``The after a table round, the dealer turn start``() =
    notImplemented()
// Step 12:
// Look at the evolve function...
// It starts to contains logic.
// Try to remove the logic from the evolve function 
// to put it back in the decide function 

// Step 13:
// Make this test pass
[<Fact>]
let ``Player can interrupt``() =
    notImplemented()

// Step 14:
// Missing an interrupt is not concidered as playing at the wrong turn.
// So what happens here ?
[<Fact>]
let ``Player get no penalty when missing an interrupt``() =
    notImplemented()


// Step 15:
// Uncomment the Kickback card and implement it.
// The kickback changes the direction of the game.
