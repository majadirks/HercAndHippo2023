using HercAndHippoLibCs;
using HercAndHippoConsole;


DisplayLoop mainLoop = new(DemoLevels.IntroducingTheHippo);
mainLoop.RunGame(KeyboardInputs());

static IEnumerable<ActionInput> KeyboardInputs()
{
    ConsoleKeyInfo keyInfo = Console.KeyAvailable ? Console.ReadKey() : default;
    while (true)
    {
        keyInfo = Console.KeyAvailable ? Console.ReadKey() : default;
        yield return keyInfo.ToActionInput();
    }
}

