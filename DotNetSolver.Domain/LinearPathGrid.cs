using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace DotNetSolver.Domain
{
    public class LinearPathGrid : Grid<PathCell?>
    {
        public IReadOnlyList<Position>? Path { get; private set; }

        public LinearPathGrid(List<List<PathCell?>> matrix) : base(matrix)
        {
        }

        public static LinearPathGrid FromGridAndCheckpoints(Grid<int> grid, IReadOnlyDictionary<int, Position> checkpoints)
        {
            var path = ComputePath(grid, checkpoints);
            if (path == null || !path.Any())
            {
                return new LinearPathGrid(new List<List<PathCell?>>());
            }

            var linearPath = new LinearPath(path);
            var pathGrid = linearPath.GetGrid();
            pathGrid._set_path(path);
            return pathGrid;
        }

        private void _set_path(IReadOnlyList<Position> path)
        {
            Path = path;
        }

        private static List<Position>? ComputePath(Grid<int> grid, IReadOnlyDictionary<int, Position> checkpoints)
        {
            var path = new List<Position>();
            var sortedCheckpoints = checkpoints.OrderBy(kvp => kvp.Key).ToList();

            for (int i = 0; i < sortedCheckpoints.Count; i++)
            {
                var (checkpointValue, startPosition) = sortedCheckpoints[i];
                var gridPositionsWithCheckpointValue = grid
                    .Where(kvp => kvp.Value == checkpointValue)
                    .Select(kvp => kvp.Key)
                    .ToHashSet();

                if (!gridPositionsWithCheckpointValue.Any()) continue;

                Position? nextCheckpointPosition = (i + 1 < sortedCheckpoints.Count) ? sortedCheckpoints[i + 1].Value : null;

                var checkpointPath = FindCheckpointPath(grid, startPosition, gridPositionsWithCheckpointValue, nextCheckpointPosition);
                if (checkpointPath == null) return null;

                path.AddRange(checkpointPath);
            }
            return path;
        }

        private static List<Position>? FindCheckpointPath(Grid<int> grid, Position start, ISet<Position> positions, Position? end, List<Position>? currentPath = null)
        {
            currentPath ??= new List<Position>();
            currentPath.Add(start);

            if (currentPath.Count == positions.Count)
            {
                if (end == null || grid.NeighborsPositions(start).Contains(end.Value))
                {
                    return currentPath;
                }
            }

            foreach (var neighbor in grid.NeighborsPositions(start))
            {
                if (positions.Contains(neighbor) && !currentPath.Contains(neighbor))
                {
                    var result = FindCheckpointPath(grid, neighbor, positions, end, new List<Position>(currentPath));
                    if (result != null) return result;
                }
            }
            return null;
        }

        public override string ToString()
        {
            if (IsEmpty()) return "Grid.Empty()";
            return string.Join("\n", Enumerable.Range(0, RowsNumber)
                .Select(r => string.Join("", Enumerable.Range(0, ColumnsNumber)
                    .Select(c => this[r, c]?.ToString() ?? " · "))));
        }

        private class LinearPath
        {
            private readonly LinearPathGrid _grid;

            public LinearPath(IReadOnlyList<Position> path)
            {
                var maxRow = path.Max(p => p.R);
                var maxCol = path.Max(p => p.C);
                var rows = (int)maxRow + 1;
                var cols = (int)maxCol + 1;

                var pathCells = new List<PathCell>();
                var firstDirection = path[0].DirectionTo(path[1]);
                pathCells.Add(PathCell.FromConnections(ImmutableHashSet.Create(firstDirection)));

                var previousDirection = firstDirection.Opposite;
                for (int i = 1; i < path.Count - 1; i++)
                {
                    var currentDirection = path[i].DirectionTo(path[i + 1]);
                    pathCells.Add(PathCell.FromConnections(ImmutableHashSet.Create(previousDirection, currentDirection)));
                    previousDirection = currentDirection.Opposite;
                }
                pathCells.Add(PathCell.EndFromConnection(previousDirection.Opposite));

                var matrix = new List<List<PathCell?>>(Enumerable.Range(0, rows).Select(_ => new List<PathCell?>(new PathCell[cols])));

                for(int i = 0; i < path.Count; i++)
                {
                    matrix[(int)path[i].R][(int)path[i].C] = pathCells[i];
                }

                _grid = new LinearPathGrid(matrix);
            }

            public LinearPathGrid GetGrid() => _grid;
        }
    }
}