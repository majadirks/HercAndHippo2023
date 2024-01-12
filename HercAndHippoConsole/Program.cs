using HercAndHippoLibCs;
using HercAndHippoConsole;

IEnumerable<ActionInput> controller = new KeyboardController();
DisplayLoop mainLoop = new(DemoLevels.IntroducingTheHippo);

mainLoop.RunGame(controller);
