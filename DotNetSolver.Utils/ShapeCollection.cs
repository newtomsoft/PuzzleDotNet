using System;
using System.Collections.Generic;

namespace DotNetSolver.Utils
{
    public static class ShapeCollection
    {
        public static readonly Dictionary<Tuple<int, int>, List<Tuple<HashSet<Position>, HashSet<Position>>>> ShapesAndAround = new Dictionary<Tuple<int, int>, List<Tuple<HashSet<Position>, HashSet<Position>>>>
        {
            {
                Tuple.Create(1, 1), new List<Tuple<HashSet<Position>, HashSet<Position>>>
                {
                    Tuple.Create(
                        new HashSet<Position> { new Position(0, 0) },
                        new HashSet<Position> { new Position(-1, 0), new Position(1, 0), new Position(0, -1), new Position(0, 1) }
                    )
                }
            },
            {
                Tuple.Create(1, 2), new List<Tuple<HashSet<Position>, HashSet<Position>>>
                {
                    Tuple.Create(
                        new HashSet<Position> { new Position(0, 1), new Position(0, 0) },
                        new HashSet<Position> { new Position(-1, 1), new Position(1, 1), new Position(-1, 0), new Position(0, 2), new Position(1, 0), new Position(0, -1) }
                    )
                }
            },
            {
                Tuple.Create(2, 2), new List<Tuple<HashSet<Position>, HashSet<Position>>>
                {
                    Tuple.Create(
                        new HashSet<Position> { new Position(0, 1), new Position(1, 0), new Position(0, 0) },
                        new HashSet<Position> { new Position(-1, 1), new Position(1, 1), new Position(2, 0), new Position(1, -1), new Position(-1, 0), new Position(0, 2), new Position(0, -1) }
                    ),
                    Tuple.Create(
                        new HashSet<Position> { new Position(0, 1), new Position(1, 1), new Position(0, 0) },
                        new HashSet<Position> { new Position(1, 2), new Position(2, 1), new Position(-1, 1), new Position(-1, 0), new Position(0, 2), new Position(1, 0), new Position(0, -1) }
                    )
                    // ... more data omitted for brevity
                }
            }
            // NOTE: More data would be included in a full implementation.
        };
    }
}