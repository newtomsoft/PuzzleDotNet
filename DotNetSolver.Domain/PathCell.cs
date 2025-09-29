using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace DotNetSolver.Domain
{
    public class PathCell
    {
        private readonly string _stringValue;
        public char Shape { get; }
        public int CounterClockwiseRotation { get; }

        private static readonly ImmutableDictionary<string, string> ShapeStringToString = new Dictionary<string, string>
        {
            { " └─", "L0" }, { "─┘ ", "L1" }, { "─┐ ", "L2" }, { " ┌─", "L3" },
            { " │ ", "I0" }, { "───", "I1" },
            { "─┬─", "T0" }, { " ├─", "T1" }, { "─┴─", "T2" }, { "─┤ ", "T3" },
            { "→  ", "E0" }, { " ↑ ", "E1" }, { "  ←", "E2" }, { " ↓ ", "E3" },
            { " ╶─", "S0" }, { " ╵ ", "S1" }, { "─╴ ", "S2" }, { " ╷ ", "S3" }
        }.ToImmutableDictionary();

        private static readonly ImmutableDictionary<string, string> StringToShapeString = ShapeStringToString.ToImmutableDictionary(kv => kv.Value, kv => kv.Key);

        private static readonly ImmutableDictionary<ImmutableHashSet<Direction>, string> ConnectionsToString = new Dictionary<ImmutableHashSet<Direction>, string>(HashSet<Direction>.CreateSetComparer())
        {
            { ImmutableHashSet.Create(Direction.Up, Direction.Right), "L0" },
            { ImmutableHashSet.Create(Direction.Up, Direction.Left), "L1" },
            { ImmutableHashSet.Create(Direction.Down, Direction.Left), "L2" },
            { ImmutableHashSet.Create(Direction.Down, Direction.Right), "L3" },
            { ImmutableHashSet.Create(Direction.Up, Direction.Down), "I0" },
            { ImmutableHashSet.Create(Direction.Left, Direction.Right), "I1" },
            { ImmutableHashSet.Create(Direction.Down, Direction.Left, Direction.Right), "T0" },
            { ImmutableHashSet.Create(Direction.Up, Direction.Down, Direction.Right), "T1" },
            { ImmutableHashSet.Create(Direction.Up, Direction.Right, Direction.Left), "T2" },
            { ImmutableHashSet.Create(Direction.Up, Direction.Left, Direction.Down), "T3" },
            { ImmutableHashSet.Create(Direction.Right), "S0" },
            { ImmutableHashSet.Create(Direction.Up), "S1" },
            { ImmutableHashSet.Create(Direction.Left), "S2" },
            { ImmutableHashSet.Create(Direction.Down), "S3" }
        }.ToImmutableDictionary(new ImmutableHashSetComparer<Direction>());

        private static readonly ImmutableDictionary<string, ImmutableHashSet<Direction>> StringToConnections = ConnectionsToString.ToImmutableDictionary(kv => kv.Value, kv => kv.Key);

        private static readonly ImmutableDictionary<Direction, string> ConnectionToEndString = new Dictionary<Direction, string>
        {
            { Direction.Right, "E0" }, { Direction.Up, "E1" }, { Direction.Left, "E2" }, { Direction.Down, "E3" }
        }.ToImmutableDictionary();

        public PathCell(string inputString)
        {
            _stringValue = inputString;
            Shape = inputString[0];
            CounterClockwiseRotation = int.Parse(inputString.AsSpan(1));
        }

        public static PathCell FromConnections(ImmutableHashSet<Direction> directions)
        {
            if (directions.Contains(Direction.None))
                throw new ArgumentException($"Invalid direction set: {string.Join(", ", directions)}");
            return new PathCell(ConnectionsToString[directions]);
        }

        public static PathCell EndFromConnection(Direction direction) => new PathCell(ConnectionToEndString[direction]);

        public static PathCell FromRepr(string shapeString) => new PathCell(ShapeStringToString[shapeString]);

        public ImmutableHashSet<Direction> GetConnectedTo() => StringToConnections[_stringValue];

        public bool IsStart() => Shape == 'S';
        public bool IsEnd() => Shape == 'E';

        public override string ToString() => StringToShapeString[_stringValue];
    }

    internal class ImmutableHashSetComparer<T> : IEqualityComparer<ImmutableHashSet<T>>
    {
        public bool Equals(ImmutableHashSet<T>? x, ImmutableHashSet<T>? y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.SetEquals(y);
        }

        public int GetHashCode(ImmutableHashSet<T> obj)
        {
            int hashCode = 0;
            foreach (T t in obj.OrderBy(i => i!.GetHashCode()))
            {
                hashCode ^= t!.GetHashCode();
            }
            return hashCode;
        }
    }
}