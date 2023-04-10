namespace HercAndHippoLib
module Level =
        
    type Visibility =
        | Clear
        | Foggy // see where you've been
        | Dark // see nearby area with torch

    type LevelObject =
        | Goody of HercAndHippoLib.Goodies.Goody
        | EnvironmentObject of HercAndHippoLib.Environment.EnvironmentObject

    // need to be able to add environment, enemies, player, objects, etc...
    type Level(levelId: int, levelName: string, gravity: float, timeLimit: int, visibility: Visibility, rowCount: int, colCount: int, objects: LevelObject list) = 
        member this.LevelId = levelId
        member this.LevelName = levelName
        member this.Gravity = gravity
        member this.TimeLimit = timeLimit
        member this.Visibility = visibility
        member this.RowCount = rowCount
        member this.ColCount = colCount
        member this.LevelObjects = objects

