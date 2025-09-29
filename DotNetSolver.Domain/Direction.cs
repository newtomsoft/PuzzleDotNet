namespace DotNetSolver.Domain
{
    public readonly struct Direction
    {
        private readonly int _value;

        private const int NoneValue = 0;
        private const int DownValue = 1;
        private const int RightValue = 2;
        private const int UpValue = 3;
        private const int LeftValue = 4;

        private Direction(int value)
        {
            _value = value;
        }

        public static Direction Up => new(UpValue);
        public static Direction Down => new(DownValue);
        public static Direction Left => new(LeftValue);
        public static Direction Right => new(RightValue);
        public static Direction None => new(NoneValue);

        public static IEnumerable<Direction> Orthogonals => new[] { Up, Left, Down, Right };

        public Direction Opposite
        {
            get
            {
                return _value switch
                {
                    DownValue => Up,
                    UpValue => Down,
                    RightValue => Left,
                    LeftValue => Right,
                    _ => None,
                };
            }
        }

        public static Direction FromString(string value)
        {
            return value.ToLower() switch
            {
                "down" => Down,
                "right" => Right,
                "up" => Up,
                "left" => Left,
                _ => throw new ArgumentException($"Unknown direction {value}"),
            };
        }

        public override string ToString()
        {
            return _value switch
            {
                DownValue => "⊓",
                RightValue => "⊏",
                UpValue => "⊔",
                LeftValue => "⊐",
                _ => "x",
            };
        }

        public static bool operator ==(Direction a, Direction b) => a._value == b._value;
        public static bool operator !=(Direction a, Direction b) => a._value != b._value;

        public override bool Equals(object? obj)
        {
            return obj is Direction other && this == other;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
    }
}