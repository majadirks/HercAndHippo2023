using HercAndHippoLibCs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HercAndHippoConsole;

internal class KeyboardController : IEnumerable<ActionInput>
{
    public IEnumerator<ActionInput> GetEnumerator() => KeyboardInputs().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => KeyboardInputs().GetEnumerator();
    private static IEnumerable<ActionInput> KeyboardInputs()
    {
        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.KeyAvailable ? Console.ReadKey() : default;
            yield return keyInfo.ToActionInput();
        }
    }


}
