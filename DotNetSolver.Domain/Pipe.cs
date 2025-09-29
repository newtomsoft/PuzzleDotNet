using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace DotNetSolver.Domain
{
    public class Pipe
    {
        private readonly string _stringValue;
        public char Shape { get; }
        public int CounterClockwiseRotation { get; }

        private static readonly ImmutableDictionary<string, string> ShapeStringToString = new Dictionary<string, string>
        {
            { " └─", "L0" }, { "─┘ ", "L1" }, { "─┐ ", "L2" }, { " ┌─", "L3" },
            { " │ ", "I0" }, { "───", "I1" },
            { "─┬─", "T0" }, { " ├─", "T1" }, { "─┴─", "T2" }, { "─┤ ", "T3" },
            { " ╶─", "E0" }, { " ╵ ", "E1" }, { "─╴ ", "E2" }, { " ╷ ", "E3" }
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
            { ImmutableHashSet.Create(Direction.Right), "E0" },
            { ImmutableHashSet.Create(Direction.Up), "E1" },
            { ImmutableHashSet.Create(Direction.Left), "E2" },
            { ImmutableHashSet.Create(Direction.Down), "E3" }
        }.ToImmutableDictionary(new ImmutableHashSetComparer<Direction>());

        private static readonly ImmutableDictionary<string, ImmutableHashSet<Direction>> StringToConnections = ConnectionsToString.ToImmutableDictionary(kv => kv.Value, kv => kv.Key);

        public Pipe(string inputString)
        {
            _stringValue = inputString;
            Shape = inputString[0];
            CounterClockwiseRotation = int.Parse(inputString.AsSpan(1));
        }

        public static Pipe FromConnection(ImmutableHashSet<Direction> directions)
        {
            return new Pipe(ConnectionsToString[directions]);
        }

        public static Pipe FromRepr(string shapeString) => new Pipe(ShapeStringToString[shapeString]);

        public ImmutableHashSet<Direction> GetConnectedTo() => StringToConnections[_stringValue];

        public override string ToString() => StringToShapeString[_stringValue];
    }
}