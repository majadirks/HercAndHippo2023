﻿namespace HercAndHippoLib
module Level =

// need to be able to add environment, enemies, player, objects, etc...
    type Level(dummy: int) = 
        member this.Dummy = dummy
        static member Default {dummy = 0}

