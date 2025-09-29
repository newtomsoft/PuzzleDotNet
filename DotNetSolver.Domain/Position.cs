using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetSolver.Domain
{
    public readonly struct Position : IEquatable<Position>
    {
        public double R { get; }
        public double C { get; }

        public Position(double r, double c)
        {
            R = r;
            C = c;
        }

        public IEnumerable<Position> Neighbors(string mode = "orthogonal")
        {
            if (mode == "orthogonal")
            {
                return new[] { Up, Left, Down, Right };
            }
            if (mode == "diagonal")
            {
                return new[] { Up, UpLeft, Left, DownLeft, Down, DownRight, Right, UpRight };
            }
            throw new ArgumentException($"Unknown mode {mode}");
        }

        public HashSet<Position> StraddledNeighbors()
        {
            var rFloor = Math.Floor(R);
            var cFloor = Math.Floor(C);
            var rCeil = Math.Ceiling(R);
            var cCeil = Math.Ceiling(C);
            return new HashSet<Position>
            {
                new Position(rFloor, cFloor),
                new Position(rFloor, cCeil),
                new Position(rCeil, cFloor),
                new Position(rCeil, cCeil)
            };
        }

        public Direction DirectionTo(Position other)
        {
            if (this == other)
            {
                return Direction.None;
            }
            if (R == other.R)
            {
                return C < other.C ? Direction.Right : Direction.Left;
            }
            if (C == other.C)
            {
                return R < other.R ? Direction.Down : Direction.Up;
            }
            return Direction.None;
        }

        public Direction DirectionFrom(Position other) => other.DirectionTo(this);

        public double DistanceTo(Position other)
        {
            if (R == other.R) return Math.Abs(C - other.C);
            if (C == other.C) return Math.Abs(R - other.R);
            return Math.Sqrt(Math.Pow(R - other.R, 2) + Math.Pow(C - other.C, 2));
        }

        public Position After(Direction direction, int count = 1)
        {
            if (direction == Direction.Down) return new Position(R + count, C);
            if (direction == Direction.Right) return new Position(R, C + count);
            if (direction == Direction.Up) return new Position(R - count, C);
            if (direction == Direction.Left) return new Position(R, C - count);
            return this;
        }

        public Position Before(Direction direction, int count = 1) => After(direction.Opposite, count);

        public Position Left => new Position(R, C - 1);
        public Position Right => new Position(R, C + 1);
        public Position Up => new Position(R - 1, C);
        public Position Down => new Position(R + 1, C);
        public Position UpLeft => new Position(R - 1, C - 1);
        public Position UpRight => new Position(R - 1, C + 1);
        public Position DownLeft => new Position(R + 1, C - 1);
        public Position DownRight => new Position(R + 1, C + 1);

        public IEnumerable<Position> AllPositionsBetween(Position other)
        {
            if (R == other.R)
            {
                int minC = (int)Math.Min(C, other.C);
                int maxC = (int)Math.Max(C, other.C);
                return Enumerable.Range(minC + 1, maxC - minC - 1).Select(c => new Position(R, c));
            }
            if (C == other.C)
            {
                int minR = (int)Math.Min(R, other.R);
                int maxR = (int)Math.Max(R, other.R);
                return Enumerable.Range(minR + 1, maxR - minR - 1).Select(r => new Position(r, C));
            }
            return Enumerable.Empty<Position>();
        }

        public IEnumerable<Position> AllPositionsAndBoundsBetween(Position other)
        {
            if (R == other.R)
            {
                int minC = (int)Math.Min(C, other.C);
                int maxC = (int)Math.Max(C, other.C);
                return Enumerable.Range(minC, maxC - minC + 1).Select(c => new Position(R, c));
            }
            if (C == other.C)
            {
                int minR = (int)Math.Min(R, other.R);
                int maxR = (int)Math.Max(R, other.R);
                return Enumerable.Range(minR, maxR - minR + 1).Select(r => new Position(r, C));
            }
            return Enumerable.Empty<Position>();
        }

        public Position Symmetric(Position position, bool toInt = true)
        {
            if (toInt)
            {
                return new Position((int)(2 * position.R - R), (int)(2 * position.C - C));
            }
            return new Position(2 * position.R - R, 2 * position.C - C);
        }

        public bool IsOnRow() => R == Math.Floor(R);
        public bool IsOnColumn() => C == Math.Floor(C);

        public Position Floor() => new Position(Math.Floor(R), Math.Floor(C));
        public Position Ceiling() => new Position(Math.Ceiling(R), Math.Ceiling(C));

        public override bool Equals(object? obj) => obj is Position other && Equals(other);
        public bool Equals(Position other) => R == other.R && C == other.C;
        public override int GetHashCode() => HashCode.Combine(R, C);

        public static bool operator ==(Position left, Position right) => left.Equals(right);
        public static bool operator !=(Position left, Position right) => !left.Equals(right);

        public static Position operator +(Position a, Position b) => new Position(a.R + b.R, a.C + b.C);
        public static Position operator -(Position a, Position b) => new Position(a.R - b.R, a.C - b.C);
        public static Position operator *(Position p, double scalar) => new Position(p.R * scalar, p.C * scalar);
        public static Position operator /(Position p, double scalar) => new Position(p.R / scalar, p.C / scalar);
        public static Position operator -(Position p) => new Position(-p.R, -p.C);

        public static bool operator <(Position left, Position right) => left.R < right.R || (left.R == right.R && left.C < right.C);
        public static bool operator <=(Position left, Position right) => left.R < right.R || (left.R == right.R && left.C <= right.C);
        public static bool operator >(Position left, Position right) => left.R > right.R || (left.R == right.R && left.C > right.C);
        public static bool operator >=(Position left, Position right) => left.R > right.R || (left.R == right.R && left.C >= right.C);

        public override string ToString() => $"({R}, {C})";

        public double this[int index] => index == 0 ? R : C;

        public IEnumerator<double> GetEnumerator()
        {
            yield return R;
            yield return C;
        }
    }
}