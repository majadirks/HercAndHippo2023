namespace HercAndHippoLib

open System

module Motion =

    type Point = 
        { 
        X: int
        Y: int
        }

        static member Default = { X = 0; Y = 0; }

        member this.Move(xDelta, yDelta) = 
            { 
            X = this.X + xDelta
            Y = this.Y + yDelta
            }

    let private ProjectileMotionX(startX: int, startVelocityX: int, time: float) =
        int ((float startX) + (float startVelocityX) * time)

    let private ProjectileMotionY(gravity: float, startY: int, startVelocityY: int, time: float) =
        (-0.5 * gravity * time * time) + (float startVelocityY * time) + (float startY)

    let ProjectileMotion(gravity: float, startLocation: Point, startVelocity: Point, time: float) =
        { 
        X = Convert.ToInt32(ProjectileMotionX(startLocation.X, startVelocity.X, time))
        Y = Convert.ToInt32(ProjectileMotionY(gravity, startLocation.Y, startVelocity.Y, time))
        }
            
        