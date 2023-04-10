namespace HercAndHippoLib
open Motion
open Environment

module Goodies =

    type SaveGame = {Location: Point}
    type Ammo = {Count: int; Location: Point}
    type Key = {Color: Color; Location: Point}
    type Trophy = {Location: Point; Destination: string}
    type Torch = {Location: Point; TimeLimit: int}

    type Goody =
        | SaveGame
        | Ammo
        | Key
        | Trophy
        | Torch
