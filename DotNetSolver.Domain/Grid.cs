using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetSolver.Domain
{
    public class Grid<T> : GridBase<T>
    {
        public Grid(T[,] matrix) : base(matrix)
        {
        }

        public Grid(List<List<T>> matrix) : base(matrix)
        {
        }

        public static Grid<Island> FromString(string gridStr)
        {
            var lines = gridStr.Split('\n');
            var rowsNumber = lines.Length;
            var columnsNumber = lines[0].Length / 3;

            var matrix = new List<List<Island>>();

            for (int r = 0; r < rowsNumber; r++)
            {
                var row = new List<Island>();
                for (int c = 0; c < columnsNumber; c++)
                {
                    var islandStr = lines[r].Substring(c * 3, 3);
                    row.Add(Island.FromString(new Position(r, c), islandStr));
                }
                matrix.Add(row);
            }

            return new Grid<Island>(matrix);
        }

        public Grid<T> Enlarge(int top, int bottom, int left, int right, T defaultValue)
        {
            var newRows = RowsNumber + top + bottom;
            var newCols = ColumnsNumber + left + right;
            var newMatrix = new T[newRows, newCols];

            for (int r = 0; r < newRows; r++)
            {
                for (int c = 0; c < newCols; c++)
                {
                    if (r >= top && r < RowsNumber + top && c >= left && c < ColumnsNumber + left)
                    {
                        newMatrix[r, c] = Matrix[r - top, c - left];
                    }
                    else
                    {
                        newMatrix[r, c] = defaultValue;
                    }
                }
            }
            return new Grid<T>(newMatrix);
        }

        public bool IsPositionInEdgeUp(Position position) => position.R == 0 && position.C >= 0 && position.C < ColumnsNumber;
        public bool IsPositionInEdgeDown(Position position) => position.R == RowsNumber - 1 && position.C >= 0 && position.C < ColumnsNumber;
        public bool IsPositionInEdgeLeft(Position position) => position.C == 0 && position.R >= 0 && position.R < RowsNumber;
        public bool IsPositionInEdgeRight(Position position) => position.C == ColumnsNumber - 1 && position.R >= 0 && position.R < RowsNumber;

        public IEnumerable<Position> EdgeUpPositions() => Enumerable.Range(0, ColumnsNumber).Select(c => new Position(0, c));
        public IEnumerable<Position> EdgeDownPositions() => Enumerable.Range(0, ColumnsNumber).Select(c => new Position(RowsNumber - 1, c));
        public IEnumerable<Position> EdgeLeftPositions() => Enumerable.Range(0, RowsNumber).Select(r => new Position(r, 0));
        public IEnumerable<Position> EdgeRightPositions() => Enumerable.Range(0, RowsNumber).Select(r => new Position(r, ColumnsNumber - 1));

        public ISet<Position> EdgesPositions()
        {
            var positions = new HashSet<Position>();
            positions.UnionWith(EdgeUpPositions());
            positions.UnionWith(EdgeDownPositions());
            positions.UnionWith(EdgeLeftPositions());
            positions.UnionWith(EdgeRightPositions());
            return positions;
        }
    }
}