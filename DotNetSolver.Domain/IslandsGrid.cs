using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetSolver.Domain
{
    public class IslandsGrid : Grid<Island>
    {
        public Dictionary<Position, Island> Islands { get; }
        public List<Dictionary<Position, Direction>> PossibleCrossoverBridges { get; }

        public IslandsGrid(List<List<int>> matrix) : this(ConvertIntMatrixToIslandMatrix(matrix))
        {
        }

        public IslandsGrid(List<List<Island>> matrix) : base(matrix)
        {
            Islands = new Dictionary<Position, Island>();
            foreach (var (position, island) in this)
            {
                if (island != null && (island.BridgesCount != 0 || island.DirectionPositionBridges.Any()))
                {
                    Islands[position] = island;
                }
            }

            if (Islands.Any())
            {
                ComputePossibleBridges();
                PossibleCrossoverBridges = ComputePossibleCrossoverBridges();
            }
            else
            {
                PossibleCrossoverBridges = new List<Dictionary<Position, Direction>>();
            }
        }

        private static List<List<Island>> ConvertIntMatrixToIslandMatrix(List<List<int>> intMatrix)
        {
            var islandMatrix = new List<List<Island>>();
            for (int r = 0; r < intMatrix.Count; r++)
            {
                var row = new List<Island>();
                for (int c = 0; c < intMatrix[r].Count; c++)
                {
                    row.Add(new Island(new Position(r, c), intMatrix[r][c]));
                }
                islandMatrix.Add(row);
            }
            return islandMatrix;
        }

        private void ComputePossibleBridges()
        {
            foreach (var island in Islands.Values)
            {
                var min_distances = new Dictionary<Direction, (double, Position)>();
                foreach (var other_island in Islands.Values)
                {
                    if (island == other_island) continue;

                    var direction = island.Position.DirectionTo(other_island.Position);
                    if (direction == Direction.None || (direction != Direction.Right && direction != Direction.Left && direction != Direction.Up && direction != Direction.Down))
                    {
                        continue;
                    }

                    var distance = island.Position.DistanceTo(other_island.Position);
                    if (!min_distances.ContainsKey(direction) || distance < min_distances[direction].Item1)
                    {
                        min_distances[direction] = (distance, other_island.Position);
                    }
                }

                if (min_distances.ContainsKey(Direction.Right)) island.DirectionPositionBridges[Direction.Right] = (min_distances[Direction.Right].Item2, 0);
                if (min_distances.ContainsKey(Direction.Left)) island.DirectionPositionBridges[Direction.Left] = (min_distances[Direction.Left].Item2, 0);
                if (min_distances.ContainsKey(Direction.Up)) island.DirectionPositionBridges[Direction.Up] = (min_distances[Direction.Up].Item2, 0);
                if (min_distances.ContainsKey(Direction.Down)) island.DirectionPositionBridges[Direction.Down] = (min_distances[Direction.Down].Item2, 0);
            }
        }

        private List<Dictionary<Position, Direction>> ComputePossibleCrossoverBridges()
        {
            var possibleMultiplesCrossoverPositions = new Dictionary<Position, Dictionary<Position, Direction>>();
            foreach (var island in Islands.Values)
            {
                foreach (var (direction, (otherPosition, _)) in island.DirectionPositionBridges)
                {
                    if (island.Position.DistanceTo(otherPosition) <= 1 || direction == Direction.Left || direction == Direction.Up) continue;

                    var crossPositions = island.Position.AllPositionsBetween(otherPosition);
                    foreach (var crossPosition in crossPositions)
                    {
                        if (!possibleMultiplesCrossoverPositions.ContainsKey(crossPosition))
                        {
                            possibleMultiplesCrossoverPositions[crossPosition] = new Dictionary<Position, Direction>();
                        }
                        possibleMultiplesCrossoverPositions[crossPosition][island.Position] = direction;
                    }
                }
            }
            return possibleMultiplesCrossoverPositions.Values.Where(d => d.Count == 2).ToList();
        }

        public List<HashSet<Position>> GetConnectedPositions(bool excludeWithoutBridge = false)
        {
            var concernedIslands = excludeWithoutBridge ? Islands.Values.Where(i => i.BridgesCount != 0).ToList() : Islands.Values.ToList();
            var visitedList = new List<HashSet<Position>>();
            var visitedFlat = new HashSet<Position>();

            while (visitedFlat.Count != concernedIslands.Count)
            {
                var position = concernedIslands.First(i => !visitedFlat.Contains(i.Position)).Position;
                var visited = DepthFirstSearchIslands(position);
                visitedList.Add(visited);
                visitedFlat.UnionWith(visited);
            }
            return visitedList;
        }

        private HashSet<Position> DepthFirstSearchIslands(Position position, HashSet<Position>? visitedPositions = null)
        {
            visitedPositions ??= new HashSet<Position>();
            if (visitedPositions.Contains(position)) return visitedPositions;

            visitedPositions.Add(position);

            var nextPositions = Islands[position].DirectionPositionBridges.Values
                                    .Where(pb => pb.Item2 > 0 && !visitedPositions.Contains(pb.Item1))
                                    .Select(pb => pb.Item1);

            foreach (var currentPosition in nextPositions)
            {
                DepthFirstSearchIslands(currentPosition, visitedPositions);
            }

            return visitedPositions;
        }

        public bool IsLoop(out HashSet<Position> loopPositions)
        {
            var allIslands = Islands.Values.Where(i => i.BridgesCount > 0).ToList();
            var visited = new HashSet<Position>();

            foreach(var island in allIslands)
            {
                if (!visited.Contains(island.Position))
                {
                    if (IsLoopRecursive(island.Position, null, visited, out loopPositions))
                    {
                        return true;
                    }
                }
            }

            loopPositions = new HashSet<Position>();
            return false;
        }

        private bool IsLoopRecursive(Position current, Position? parent, HashSet<Position> visited, out HashSet<Position> loop)
        {
            visited.Add(current);
            loop = new HashSet<Position> { current };

            var neighbors = Islands[current].DirectionPositionBridges.Values
                .Where(pb => pb.Item2 > 0)
                .Select(pb => pb.Item1);

            foreach (var neighbor in neighbors)
            {
                if (neighbor.Equals(parent)) continue;

                if (visited.Contains(neighbor))
                {
                    // Found a cycle
                    return true;
                }

                if (IsLoopRecursive(neighbor, current, visited, out var subLoop))
                {
                    loop.UnionWith(subLoop);
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            if (IsEmpty()) return "IslandGrid.Empty()";

            return base.ToString();
        }
    }
}