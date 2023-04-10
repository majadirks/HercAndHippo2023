namespace HercAndHippoLib
open HercAndHippoLib.Motion

module Environment =
    
    type Color =
        | Red
        | Orange
        | Yellow
        | Green
        | Blue
        | Purple
        | Black
        | White

    type Wall = {Color: Color; Location: Point}

    type Door = {Color: Color; Location: Point}

    type BreakableWall = {Color: Color; Location: Point}

    