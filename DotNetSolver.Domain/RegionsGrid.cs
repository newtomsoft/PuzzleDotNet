using System.Collections.Generic;
using System.Linq;

namespace DotNetSolver.Domain
{
    public class RegionsGrid : Grid<int?>
    {
        private readonly Grid<HashSet<string>> _sourceGrid;

        public RegionsGrid(Grid<HashSet<string>> sourceGrid) : base(new int?[sourceGrid.RowsNumber, sourceGrid.ColumnsNumber])
        {
            _sourceGrid = sourceGrid;
            ComputeRegionsGrid();
        }

        private void ComputeRegionsGrid()
        {
            var visitedRegions = new HashSet<Position>();
            int regionsCount = 0;

            foreach (var (position, _) in _sourceGrid)
            {
                if (visitedRegions.Contains(position))
                {
                    continue;
                }
                regionsCount++;
                var region = DepthFirstSearchRegions(position);
                foreach (var pos in region)
                {
                    this[pos] = regionsCount;
                    visitedRegions.Add(pos);
                }
            }
        }

        private HashSet<Position> DepthFirstSearchRegions(Position currentPosition, HashSet<Position>? visited = null)
        {
            visited ??= new HashSet<Position>();
            if (visited.Contains(currentPosition))
            {
                return visited;
            }
            visited.Add(currentPosition);

            var positions = new Dictionary<string, Position>
            {
                { "right", new Position(0, 1) },
                { "left", new Position(0, -1) },
                { "down", new Position(1, 0) },
                { "up", new Position(-1, 0) }
            };

            var openedOn = _sourceGrid[currentPosition];
            if (openedOn != null)
            {
                foreach (var direction in openedOn)
                {
                    if (positions.TryGetValue(direction, out var offset))
                    {
                        var newPosition = new Position(currentPosition.R + offset.R, currentPosition.C + offset.C);
                        if (_sourceGrid.Contains(newPosition))
                        {
                            DepthFirstSearchRegions(newPosition, visited);
                        }
                    }
                }
            }

            return visited;
        }
    }
}