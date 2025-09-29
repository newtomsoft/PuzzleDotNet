using System.Collections.Generic;
using System.Linq;
using DotNetSolver.Domain;

namespace DotNetSolver.Utils
{
    public static class ShapeGenerator
    {
        public static List<HashSet<Position>> GetAllShapes(int rowsNumber, int columnsNumber)
        {
            var allShapes = new List<HashSet<Position>>();
            var minCellsNumber = rowsNumber + columnsNumber - 1;
            var maxCellsNumber = rowsNumber * columnsNumber - (rowsNumber / 2) * (columnsNumber / 2);

            for (int cellsNumber = minCellsNumber; cellsNumber <= maxCellsNumber; cellsNumber++)
            {
                var shapes = GenerateShapes(rowsNumber, columnsNumber, cellsNumber);
                allShapes.AddRange(shapes);
            }
            return allShapes;
        }

        public static HashSet<Position> AroundShape(ICollection<Position> shape)
        {
            var shapeSet = new HashSet<Position>(shape);
            var enlargedShape = new HashSet<Position>();
            foreach (var position in shape)
            {
                foreach (var neighbor in position.Neighbors())
                {
                    if (!shapeSet.Contains(neighbor))
                    {
                        enlargedShape.Add(neighbor);
                    }
                }
            }
            return enlargedShape;
        }

        public static HashSet<Position> Edges(ICollection<Position> shape)
        {
            var shapeSet = new HashSet<Position>(shape);
            var edgePositions = new HashSet<Position>();
            foreach (var position in shape)
            {
                foreach (var neighborPosition in position.Neighbors())
                {
                    if (!shapeSet.Contains(neighborPosition))
                    {
                        edgePositions.Add(position);
                        break;
                    }
                }
            }
            return edgePositions;
        }

        private static IEnumerable<HashSet<Position>> GenerateShapes(int rowsNumber, int columnsNumber, int cellsNumber)
        {
            var allCells = new List<Position>();
            for (int r = 0; r < rowsNumber; r++)
            {
                for (int c = 0; c < columnsNumber; c++)
                {
                    allCells.Add(new Position(r, c));
                }
            }

            return GetCombinations(allCells, cellsNumber)
                .Where(combination =>
                    CoversEntireGrid(combination, rowsNumber, columnsNumber) &&
                    IsConnected(combination) &&
                    !ContainsSquare(combination))
                .Select(c => c.ToHashSet());
        }

        private static bool CoversEntireGrid(IEnumerable<Position> shape, int rowsNumber, int columnsNumber)
        {
            var rows = new HashSet<double>();
            var cols = new HashSet<double>();
            foreach (var pos in shape)
            {
                rows.Add(pos.R);
                cols.Add(pos.C);
            }
            return rows.Count == rowsNumber && cols.Count == columnsNumber;
        }

        private static bool IsConnected(ICollection<Position> shape)
        {
            if (!shape.Any()) return true;

            var visited = new HashSet<Position>();
            var stack = new Stack<Position>();
            stack.Push(shape.First());

            while (stack.Count > 0)
            {
                var position = stack.Pop();
                if (visited.Contains(position)) continue;

                visited.Add(position);

                foreach (var neighbor in position.Neighbors())
                {
                    if (shape.Contains(neighbor))
                    {
                        stack.Push(neighbor);
                    }
                }
            }

            return visited.Count == shape.Count;
        }

        private static bool ContainsSquare(ICollection<Position> shape)
        {
            var shapeSet = new HashSet<Position>(shape);
            return shape.Any(pos =>
                shapeSet.Contains(new Position(pos.R + 1, pos.C)) &&
                shapeSet.Contains(new Position(pos.R, pos.C + 1)) &&
                shapeSet.Contains(new Position(pos.R + 1, pos.C + 1)));
        }

        private static IEnumerable<IEnumerable<T>> GetCombinations<T>(ICollection<T> items, int k)
        {
            if (k == 0)
            {
                yield return new T[0];
                yield break;
            }

            int i = 0;
            foreach (var item in items)
            {
                if (k == 1)
                {
                    yield return new T[] { item };
                }
                else
                {
                    foreach (var result in GetCombinations(items.Skip(i + 1).ToList(), k - 1))
                    {
                        yield return new T[] { item }.Concat(result);
                    }
                }
                i++;
            }
        }
    }
}