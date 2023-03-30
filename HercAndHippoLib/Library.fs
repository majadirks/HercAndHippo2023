namespace HercAndHippoLib

module Say =
    let hello name =
        printfn "Hello %s" name

    let exampleFunc (x:int) =
        x * x + 2

    let exampleFunc2(x:double) =
        x * x + (double)2
