namespace HercAndHippoLibCs
{
    public readonly struct Mortal<T>
    {
        public readonly T? Value { get; private init; }
        public readonly bool Alive { get; private init; }
        public Mortal(T value) => (this.Value, this.Alive) = (value, true);
        private Mortal(T? value, bool alive) => (this.Value, this.Alive) = (value, alive);
        public Mortal<T> Dead() => new(this.Value, alive: false);
        public override string ToString() => $"(Mortal, {(Alive ? "Alive" : "Dead")}) {Value}";

        public static implicit operator Mortal<T>(EmptySpace _) => new(value: default, alive: false);
        public static implicit operator Mortal<T>(T value) => new(value: value, alive: true);
    }
}
