using HercAndHippoLibCs;
namespace HercAndHippoConsole;
public class Program
{
    [STAThread]
    static void Main()
    {
        GameController keyboard = new KeyboardController();
        DisplayLoop mainLoop = new(state: DemoLevels.IntroducingTheHippo, frequency_hz: 35);
        mainLoop.RunGame(controller: keyboard);
    }
}