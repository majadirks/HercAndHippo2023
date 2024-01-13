using HercAndHippoLibCs;
using HercAndHippoConsole;

GameController keyboard = new KeyboardController();
GameController bot = new SeekTrophyController(DemoLevels.IntroducingTheHippo, depth: 30, iterations: 5000);
DisplayLoop mainLoop = new(state: DemoLevels.IntroducingTheHippo, frequency_hz: 30);
mainLoop.RunGame(controller: bot);
