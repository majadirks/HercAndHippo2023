namespace HercAndHippoLibCs;
public record BounceBuddy(Location Location, Direction Whither, Slowness Slowness) : HercAndHippoObj, IShootable, ITouchable, ILocatable, IConsoleDisplayable, ICyclable
{
    public bool StopsBullet => true;

    public Color Color => Color.White;

    public Color BackgroundColor => Color.Black;

    public string ConsoleDisplayString => "o";

    /// <summary>
    /// A BounceBuddy blocks motion for anything coming from above,
    /// but nothing else.
    /// </summary>
    public override bool BlocksMotion(Level level, ILocatable toBlock) 
        => toBlock.Location.Row < this.Location.Row;

    public Level Cycle(Level level, ActionInput actionInput)
    {
        Level nextLevel = level;
        BounceBuddy nextBuddy = this;

        if (Location == nextLevel.Player.Location)
            nextLevel = OnTouch(nextLevel, Direction.Idle, nextLevel.Player);
        if (Location == nextLevel.Hippo?.Location)
            nextLevel = OnTouch(nextLevel, Direction.Idle, nextLevel.Hippo);

        bool moving = Slowness.Applies(nextLevel);
        if (moving && Whither == Direction.East)
        {
            Location nextEast = new(Location.Col.NextEast(nextLevel.Width), Location.Row);
            if (this.MotionBlockedTo(nextLevel, Direction.East))
            {
                nextBuddy = this with { Whither = Direction.West };
                nextLevel = nextLevel.ReplaceIfPresent(this, nextBuddy);
                nextLevel = nextBuddy.MutualTouch(nextLevel, nextEast, touchFrom: Direction.West);
            }
            else
            {
                nextBuddy = this with { Location = nextEast };
                nextLevel = nextLevel.ReplaceIfPresent(this, nextBuddy);
            }
        }
        else if (moving && Whither == Direction.West)
        {
            Location nextWest = new(Location.Col.NextWest(), Location.Row);
            if (this.MotionBlockedTo(nextLevel, Direction.West))
            {
                nextBuddy = this with { Whither = Direction.East };
                nextLevel = nextLevel.ReplaceIfPresent(this, nextBuddy);
                nextLevel = nextBuddy.MutualTouch(nextLevel, nextWest, touchFrom: Direction.East);
            }
            else
            {
                nextBuddy = this with { Location = nextWest };
                nextLevel = nextLevel.ReplaceIfPresent(this, nextBuddy);
            }
        }

        nextLevel = nextBuddy.ApplyGravity(nextLevel);
        return nextLevel;
    }

    public Level OnShot(Level level, Direction shotFrom, Bullet shotBy)
        => this.Die(level);

    /// <summary>
    /// If the BounceBuddy is touched by the player or hippo,
    /// the player's health or hippo's health is decreased
    /// </summary>
    public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy)
    {
        if (touchedBy is Player player && player.Location.Row >= this.Location.Row)
        {
            // Player touched, but not from above; inflict damage
            Health nextHealth = player.Health - 5;
            Player nextPlayer = player with { Health = nextHealth };
            return level.WithPlayer(nextPlayer).AddSecondaryObject(Message.Ouch);
        }
        else if (touchedBy is Player abovePlayer && abovePlayer.Location.Row < this.Location.Row)
        {
            // Player touched from above, so bounce
            Player nextPlayer = abovePlayer with { KineticEnergy = Math.Abs(abovePlayer.KineticEnergy) + 2 * abovePlayer.JumpStrength };
            return level.WithPlayer(nextPlayer);
        }
        else if (touchedBy is Hippo hippo)
        {
            Health nextHealth = hippo.Health - 5;
            Hippo nextHippo = hippo with { Health = nextHealth };
            return level.ReplaceIfPresent(hippo, nextHippo).AddSecondaryObject(Message.Ouch);
        }
        else
            return level.NoReaction();
    }
}
