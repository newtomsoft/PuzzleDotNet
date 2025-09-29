using System.Collections.Generic;
using System.Linq;

namespace DotNetSolver.Domain
{
    public class WrappedPipesGrid : WrappedGrid<Pipe>
    {
        public WrappedPipesGrid(List<List<Pipe>> matrix) : base(matrix)
        {
        }

        public (List<HashSet<Position>>, bool) GetConnectedPositionsAndIsLoop()
        {
            var totalPositions = ColumnsNumber * RowsNumber;
            var visitedList = new List<HashSet<Position>>();
            var visitedFlat = new HashSet<Position>();
            bool isLoop = false;

            for (int r = 0; r < RowsNumber; r++)
            {
                for (int c = 0; c < ColumnsNumber; c++)
                {
                    var position = new Position(r, c);
                    if (!visitedFlat.Contains(position))
                    {
                        var (visitedInThisPass, isLoopInThisPass) = DepthFirstSearchPipesAndIsLoop(position);
                        if (isLoopInThisPass)
                        {
                            isLoop = true;
                        }
                        visitedList.Add(visitedInThisPass);
                        visitedFlat.UnionWith(visitedInThisPass);
                    }
                }
            }

            return (visitedList, isLoop);
        }

        private (HashSet<Position>, bool) DepthFirstSearchPipesAndIsLoop(Position position, HashSet<Position>? visitedPositions = null, Direction? forbiddenDirection = null)
        {
            visitedPositions ??= new HashSet<Position>();

            var normalizedPosition = NormalizePosition(position);

            if (visitedPositions.Contains(normalizedPosition))
            {
                return (visitedPositions, true);
            }
            visitedPositions.Add(normalizedPosition);

            var currentPipe = this[normalizedPosition];
            var connectedTo = currentPipe.GetConnectedTo();
            bool loop = false;

            foreach (var direction in connectedTo.Where(d => d != forbiddenDirection))
            {
                var nextPos = normalizedPosition.After(direction);

                if (Walls.Any(wall => wall.SetEquals(new[]{normalizedPosition, nextPos})))
                {
                    continue;
                }

                var normalizedNextPos = NormalizePosition(nextPos);
                var nextPipe = this[normalizedNextPos];

                if (nextPipe.GetConnectedTo().Contains(direction.Opposite))
                {
                    var (_, isLoopResult) = DepthFirstSearchPipesAndIsLoop(nextPos, visitedPositions, direction.Opposite);
                    if (isLoopResult)
                    {
                        loop = true;
                    }
                }
            }

            return (visitedPositions, loop);
        }

        private Position NormalizePosition(Position position)
        {
            var r = (int)position.R % RowsNumber;
            var c = (int)position.C % ColumnsNumber;
            if (r < 0) r += RowsNumber;
            if (c < 0) c += ColumnsNumber;
            return new Position(r, c);
        }
    }
}