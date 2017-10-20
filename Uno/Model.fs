[<AutoOpen>]
module Model

type Color =
    | Red
    | Green
    | Blue
    | Yellow

type Digit = Zero | One | Two | Three |Four | Five | Six | Seven | Height | Nine

type Card =
    | Digit of Digit * Color
    | Skip of Color
