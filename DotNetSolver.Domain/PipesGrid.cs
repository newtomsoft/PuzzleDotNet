using System.Collections.Generic;
using System.Linq;

namespace DotNetSolver.Domain
{
    public class PipesGrid : Grid<Pipe>
    {
        public PipesGrid(List<List<Pipe>> matrix) : base(matrix)
        {
        }

        public (List<HashSet<Position>>, bool) GetConnectedPositionsAndIsLoop()
        {
            var totalPositions = ColumnsNumber * RowsNumber;
            var visitedList = new List<HashSet<Position>>();
            var visitedFlat = new HashSet<Position>();
            bool isLoop = false;

            while (visitedFlat.Count != totalPositions)
            {
                var position = this.First(kvp => !visitedFlat.Contains(kvp.Key)).Key;
                var (visitedInThisPass, isLoopInThisPass) = DepthFirstSearchPipesAndIsLoop(position);
                if (isLoopInThisPass)
                {
                    isLoop = true;
                }
                visitedList.Add(visitedInThisPass);
                visitedFlat.UnionWith(visitedInThisPass);
            }

            return (visitedList, isLoop);
        }

        private (HashSet<Position>, bool) DepthFirstSearchPipesAndIsLoop(Position position, HashSet<Position>? visitedPositions = null, Direction? forbiddenDirection = null)
        {
            visitedPositions ??= new HashSet<Position>();
            if (visitedPositions.Contains(position))
            {
                return (visitedPositions, true);
            }
            visitedPositions.Add(position);

            var currentPipe = this[position];
            var connectedTo = currentPipe.GetConnectedTo();
            bool loop = false;

            foreach (var direction in connectedTo.Where(d => d != forbiddenDirection))
            {
                var nextPos = position.After(direction);
                if (!Contains(nextPos) || Walls.Any(wall => wall.SetEquals(new[]{position, nextPos})))
                {
                    continue;
                }

                var nextPipe = this[nextPos];
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
    }
}