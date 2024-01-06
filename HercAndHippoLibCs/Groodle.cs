using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HercAndHippoLibCs;

public record Groodle(Location Location, Direction Whither) : HercAndHippoObj, IShootable, ITouchable, ILocatable, IConsoleDisplayable, ICyclable
{
    public bool StopsBullet => true;

    public Color Color => Color.White;

    public Color BackgroundColor => Color.Black;

    public string ConsoleDisplayString => "τ";

    public override bool BlocksMotion(Level level, ILocatable toBlock) => false;

    public Level Cycle(Level level, ActionInput actionInput)
    {
        Level nextLevel = level;
        Groodle nextGroodle = this;
        if (Location == nextLevel.Player.Location)
            nextLevel = OnTouch(nextLevel, Direction.Idle, nextLevel.Player);
        if (Location == nextLevel.Hippo?.Location)
            nextLevel = OnTouch(nextLevel, Direction.Idle, nextLevel.Hippo);

        if (Whither == Direction.East)
        {
            Location nextEast = new(Location.Col.NextEast(nextLevel.Width), Location.Row);
            if (this.MotionBlockedTo(nextLevel, Direction.East))
            {
                nextGroodle = this with { Whither = Direction.West };
                nextLevel = nextLevel.Replace(this, nextGroodle);

                var touchables = nextLevel
                   .ObjectsAt(nextEast)
                   .Where(obj => obj.IsTouchable)
                   .Cast<ITouchable>()
                   .ToList();
                // Call touch methods for any touchables at nextEast
                nextLevel = touchables
                    .Aggregate(
                    seed: nextLevel,
                    func: (state, touchable) => touchable.OnTouch(state, Direction.West, nextGroodle));
                // Call Groodle OnTouch methods for any touchables at nextWest
                nextLevel = touchables
                    .Aggregate(
                    seed: nextLevel,
                    func: (state, touchable) => nextGroodle.OnTouch(state, Direction.West, touchable));
            }
            else
            {
                nextGroodle = this with { Location = nextEast };
                nextLevel = nextLevel.Replace(this, nextGroodle);
            }
        }
        else if (Whither == Direction.West)
        {
            Location nextWest = new(Location.Col.NextWest(), Location.Row);
            if (this.MotionBlockedTo(nextLevel, Direction.West))
            {
                nextGroodle = this with { Whither = Direction.East };
                nextLevel = nextLevel.Replace(this, nextGroodle);
                var touchables = nextLevel
                    .ObjectsAt(nextWest)
                    .Where(obj => obj.IsTouchable)
                    .Cast<ITouchable>()
                    .ToList();
                // Call touch methods for any touchables at nextWest
                nextLevel = touchables
                    .Aggregate(
                    seed: nextLevel,
                    func: (state, touchable) => touchable.OnTouch(state, Direction.East, nextGroodle));
                // Call Groodle OnTouch methods for any touchables at nextWest
                nextLevel = touchables
                    .Aggregate(
                    seed: nextLevel,
                    func: (state, touchable) => nextGroodle.OnTouch(state, Direction.East, touchable));
            }
            else
            {
                nextGroodle = this with { Location = nextWest };
                nextLevel = nextLevel.Replace(this, nextGroodle);
            }
        }

        nextLevel = Behaviors.ApplyGravity(nextLevel, nextGroodle);
        return nextLevel;
    }

    public Level OnShot(Level level, Direction shotFrom, Bullet shotBy) => Behaviors.Die(level, this);

    public Level OnTouch(Level level, Direction touchedFrom, ITouchable touchedBy)
    {
        if (touchedBy is Player player)
        {
            Health nextHealth = player.Health - 5;
            Player nextPlayer = player with { Health = nextHealth };
            return level.WithPlayer(nextPlayer);
        }
        else if (touchedBy is Hippo hippo)
        {
            Health nextHealth = hippo.Health - 5;
            Hippo nextHippo = hippo with { Health = nextHealth };
            return level.Replace(hippo, nextHippo);
        }
        else
            return Behaviors.NoReaction(level);
    }
}
