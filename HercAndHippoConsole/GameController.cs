﻿using HercAndHippoLibCs;
using System.Runtime.InteropServices;
using System.Windows;
//using System.Windows.Input;
namespace HercAndHippoConsole;

public abstract class GameController
{
    public abstract ActionInputPair NextAction(Level state);
}
internal partial class KeyboardController : GameController
{
    //https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
    const int VK_LSHIFT = 0xA0;
    const int VK_RSHIFT = 0xA1;
    const int VK_SPACE = 0x20;
    const int VK_LEFT = 0x25;
    const int VK_UP = 0x26;
    const int VK_RIGHT = 0x27;
    const int VK_DOWN = 0x28;    

    [LibraryImport("user32.dll")]
    public static partial short GetKeyState(int nVirtKey);
    public override  ActionInputPair NextAction(Level state)
    {
        // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getkeystate
        // "If the high-order bit is 1, the key is down; otherwise, it is up."
        bool lshift = (((ushort)GetKeyState(VK_LSHIFT)) >> 15) == 1;
        bool rshift = (((ushort)GetKeyState(VK_RSHIFT)) >> 15) == 1;
        bool shooting = lshift || rshift;
        bool north = (((ushort)GetKeyState(VK_UP)) >> 15) == 1;
        bool south = (((ushort)GetKeyState(VK_DOWN)) >> 15) == 1;
        bool west = (((ushort)GetKeyState(VK_LEFT)) >> 15) == 1;
        bool east = (((ushort)GetKeyState(VK_RIGHT)) >> 15) == 1;
        bool droppingHippo = (((ushort)GetKeyState(VK_SPACE)) >> 15) == 1;
      
        if (shooting) // cannot shoot while doing something else
        {
            if (north) return ActionInput.ShootNorth;
            else if (east) return ActionInput.ShootEast;
            else if (south) return ActionInput.ShootSouth;
            else if (west) return ActionInput.ShootWest;
            else return ActionInput.NoAction;
        }
        
        if (north) // jumping
        {
            if (east) 
                return new(ActionInput.MoveEast, ActionInput.MoveNorth);
            else if (west) 
                return new(ActionInput.MoveWest, ActionInput.MoveNorth);
            else // jumping, no horizontal motion
                return ActionInput.MoveNorth;
        }

        if (south)
        {
            if (east)
                return new(ActionInput.MoveEast, ActionInput.MoveSouth);
            else if (west)
                return new(ActionInput.MoveWest, ActionInput.MoveSouth);
            else
                return ActionInput.MoveSouth;
        }

        if (east)
            return ActionInput.MoveEast;
        else if (west)
            return ActionInput.MoveWest;
        else if (south)
            return ActionInput.MoveSouth;
        else if (droppingHippo)
            return ActionInput.DropHippo;
        else
            return ActionInput.NoAction;
    }
}

public class EnumerableController : GameController
{
    private readonly IEnumerator<ActionInput> enumerator;
    public EnumerableController(IEnumerable<ActionInput> actions)
    {
        this.enumerator = actions.GetEnumerator();
    }
    public override ActionInputPair NextAction(Level state)
    {
        bool read = enumerator.MoveNext();
        return new ActionInputPair(read ? enumerator.Current: ActionInput.NoAction);
    }
}