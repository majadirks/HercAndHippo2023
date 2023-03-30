namespace HercAndHippoLib_Test
open HercAndHippoLib.Say

open System
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.TestMethodPassing () =
        Assert.IsTrue(true);

    [<TestMethod>]
    member this.DoStuffWithX () =
        Assert.AreEqual(6, exampleFunc 2)
        Assert.AreEqual(2.01, exampleFunc2 0.1) // Dangerous comparison of doubles
