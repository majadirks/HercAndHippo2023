using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HercAndHippoLibCs
{
    public readonly struct Health
    {
        private const int MIN_HEALTH = 0;
        private const int MAX_HEALTH = 200;
        private const int DEFAULT_STARTING_HEALTH = 100;
        private readonly int HealthAmt { get; init; }
        public Health(int health = DEFAULT_STARTING_HEALTH)
        {
            if (!IsValid(health)) 
                throw new ArgumentException($"Invalid health value {health} (must be between {MIN_HEALTH} and {MAX_HEALTH}");
            HealthAmt = health;
        }
        private static bool IsValid(int health) => MIN_HEALTH <= health && health <= MAX_HEALTH;
        public static Health operator -(Health health, int subtrahend) => Math.Max(0, health.HealthAmt - subtrahend);
        public static Health operator +(Health health,int addend) => Math.Min(MAX_HEALTH, health.HealthAmt + addend);
       
        public static bool operator >(Health health, int comparandum) => health.HealthAmt > comparandum;
        public static bool operator >=(Health health, int comparandum) => health.HealthAmt >= comparandum;
        public static bool operator <(Health health, int comparandum) => health.HealthAmt < comparandum;
        public static bool operator <=(Health health, int comparandum) => health.HealthAmt <= comparandum;
        public static bool operator ==(Health health, int comparandm) => health.HealthAmt == comparandm;
        public static bool operator !=(Health health, int comparandm) => health.HealthAmt != comparandm;

        public static implicit operator Health(int health) => new(health);

        public override bool Equals(object? obj) => obj is Health other && other.HealthAmt == this.HealthAmt;
        public override int GetHashCode() => this.HealthAmt.GetHashCode();
    }

    public record Player(Location Location, Health Health) : IDisplayable, IShootable
    {
        public Color Color => Color.Blue;
        public Level OnShot(Level level, Direction shotFrom) 
            => level.WithPlayer(this with { Health = Health - 5 });

        private Level TryMoveTo(Location newLocation, Direction approachFrom, Level curState)
        => curState.ObjectAt(newLocation).FirstOrDefault() switch
            {
                ITouchable touchableAtLocation => touchableAtLocation.OnTouch(curState, approachFrom),
                _ => curState.WithPlayer(this with { Location = newLocation })
            };
          
        public Level MoveLeft(Level level) => TryMoveTo((Location.Col - 1, Location.Row), approachFrom: Direction.East, curState: level);
        public Level MoveRight(Level level) => TryMoveTo((Location.Col + 1, Location.Row), Direction.West, curState: level);
        public Level MoveUp(Level level) => TryMoveTo((Location.Col, Location.Row - 1), Direction.South, curState: level);
        public Level MoveDown(Level level) => TryMoveTo((Location.Col, Location.Row + 1), Direction.North, curState: level);


    }


}
