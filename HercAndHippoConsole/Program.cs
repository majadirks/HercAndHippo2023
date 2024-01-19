using HercAndHippoLibCs;
using HercAndHippoConsole;

GameController keyboard = new KeyboardController();
DisplayLoop mainLoop = new(state: DemoLevels.IntroducingTheHippo, frequency_hz: 35);
mainLoop.RunGame(controller: keyboard);
