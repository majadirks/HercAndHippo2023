﻿namespace HercAndHippoLibCs
{
    public record BreakableWall(Color Color, Location Location) : HercAndHippoObj, ILocatable, IShootable, IConsoleDisplayable
    {
        public string ConsoleDisplayString => "▓";
        public Color BackgroundColor => Color.Black;

        public bool StopsBullet => true;

        public override bool BlocksMotion(Level level, ILocatable toBlock) => true;

        public Level OnShot(Level level, Direction shotFrom, Bullet shotBy) => this.Die(level);
    }
}
