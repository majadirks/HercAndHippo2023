﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HercAndHippoLibCs;
namespace HercAndHippoConsole;

public static class IntroducingTheHippoAutoPlay
{
    public static readonly GameController HippoAutoPlayController = new EnumerableController(new ActionInput[]
    {
        ActionInput.MoveWest,
        ActionInput.MoveWest,
        ActionInput.MoveEast,
        ActionInput.MoveEast,
        ActionInput.MoveEast,
        ActionInput.MoveEast,
        ActionInput.MoveEast,
        ActionInput.MoveEast,
        ActionInput.MoveEast,
        ActionInput.MoveEast,
        ActionInput.MoveEast,
        ActionInput.MoveNorth,
        ActionInput.NoAction,
        ActionInput.NoAction,
        ActionInput.NoAction,
        ActionInput.NoAction,
        ActionInput.NoAction,
        ActionInput.ShootSouth,
        ActionInput.NoAction,
        ActionInput.NoAction,
        ActionInput.NoAction,
        ActionInput.NoAction,
        ActionInput.NoAction,
        ActionInput.NoAction,
        ActionInput.NoAction,
        ActionInput.NoAction,
        ActionInput.NoAction,
        ActionInput.NoAction,
        ActionInput.ShootSouth,
        ActionInput.ShootSouth,
        ActionInput.NoAction,
        ActionInput.NoAction,
        ActionInput.NoAction,
        ActionInput.NoAction,
        ActionInput.NoAction,
        ActionInput.NoAction,
        ActionInput.NoAction,
        ActionInput.NoAction,
        ActionInput.NoAction,
        ActionInput.NoAction,
        ActionInput.NoAction,
        ActionInput.NoAction,
        ActionInput.NoAction,
        ActionInput.NoAction,
        ActionInput.ShootWest,
        ActionInput.MoveWest,
        ActionInput.MoveWest,
        ActionInput.MoveWest,
        ActionInput.MoveWest,
        ActionInput.MoveWest,
        ActionInput.MoveWest,
        ActionInput.MoveWest,
        ActionInput.MoveWest,
        ActionInput.MoveWest,
        ActionInput.MoveWest,
        ActionInput.MoveWest,
        ActionInput.MoveWest,
        ActionInput.MoveWest,
        ActionInput.NoAction,
        ActionInput.NoAction,
    });
}
