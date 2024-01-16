using HercAndHippoLibCs;
using HercAndHippoConsole;

GameController keyboard = new KeyboardController();
DisplayLoop mainLoop = new(state: DemoLevels.Clones, frequency_hz: 40);
mainLoop.RunGame(controller: keyboard);
