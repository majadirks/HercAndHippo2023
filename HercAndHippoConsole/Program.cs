using HercAndHippoLibCs;
using HercAndHippoConsole;

GameController keyboard = new KeyboardController();
GameController bot = new DeepSeekTrophyController(DemoLevels.IntroducingTheHippo, depth: 50, iterations: 200);
DisplayLoop mainLoop = new(state: DemoLevels.IntroducingTheHippo, frequency_hz: 40);
mainLoop.RunGame(controller: bot);
