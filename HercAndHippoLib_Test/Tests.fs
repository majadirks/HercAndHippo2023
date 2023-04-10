namespace HercAndHippoLib_Test
open HercAndHippoLib.Motion

open System
open Microsoft.VisualStudio.TestTools.UnitTesting



[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.PointProjectileMotion () =
        // Arrange
        let startPoint = Point.Default
        let startVelocity = {Point.X = 2; Y = 20}
        let expectedOutput = [
            {Point.X = 0; Y = 0}; 
            {Point.X = 2; Y = 18};
            {Point.X = 4; Y = 30};
            {Point.X = 6; Y = 38};
            {Point.X = 8; Y = 40};
            {Point.X = 10; Y = 38};
            {Point.X = 12; Y = 30};
            {Point.X = 14; Y = 18};
            {Point.X = 16; Y = 0}]
        // Act
        let throwList =
            [0 .. 8]
            |> List.map (fun time -> ProjectileMotion(5, startPoint, startVelocity, time))
        // Assert
        let doesMatch = expectedOutput = throwList
        Assert.IsTrue(doesMatch)