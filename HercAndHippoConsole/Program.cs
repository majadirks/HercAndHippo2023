using HercAndHippoLibCs;
using HercAndHippoConsole;

GameController keyboard = new KeyboardController();
GameController bot = new SeekTrophyController(DemoLevels.IntroducingTheHippo, depth: 50, iterations: 500);
DisplayLoop mainLoop = new(state: DemoLevels.IntroducingTheHippo, frequency_hz: 30);
mainLoop.RunGame(controller: bot);
