using System.Collections.Generic;

namespace DotNetSolver.Domain
{
    public class WrappedGrid<T> : GridBase<T>
    {
        public WrappedGrid(T[,] matrix) : base(matrix)
        {
        }

        public WrappedGrid(List<List<T>> matrix) : base(matrix)
        {
        }

        public new T this[Position key]
        {
            get
            {
                var r = (int)key.R % RowsNumber;
                var c = (int)key.C % ColumnsNumber;
                if (r < 0) r += RowsNumber;
                if (c < 0) c += ColumnsNumber;
                return Matrix[r, c];
            }
            set
            {
                var r = (int)key.R % RowsNumber;
                var c = (int)key.C % ColumnsNumber;
                if (r < 0) r += RowsNumber;
                if (c < 0) c += ColumnsNumber;
                Matrix[r, c] = value;
            }
        }

        public new bool Contains(Position item) => true;

        public new IEnumerable<Position> NeighborsPositions(Position position, string mode = "orthogonal")
        {
            var neighbors = new List<Position>();
            var r = (int)position.R;
            var c = (int)position.C;

            if (mode is "orthogonal" or "diagonal")
            {
                neighbors.Add(new Position((r - 1 + RowsNumber) % RowsNumber, c));
                neighbors.Add(new Position((r + 1) % RowsNumber, c));
                neighbors.Add(new Position(r, (c - 1 + ColumnsNumber) % ColumnsNumber));
                neighbors.Add(new Position(r, (c + 1) % ColumnsNumber));
            }
            if (mode is "diagonal" or "diagonal_only")
            {
                neighbors.Add(new Position((r - 1 + RowsNumber) % RowsNumber, (c - 1 + ColumnsNumber) % ColumnsNumber));
                neighbors.Add(new Position((r - 1 + RowsNumber) % RowsNumber, (c + 1) % ColumnsNumber));
                neighbors.Add(new Position((r + 1) % RowsNumber, (c - 1 + ColumnsNumber) % ColumnsNumber));
                neighbors.Add(new Position((r + 1) % RowsNumber, (c + 1) % ColumnsNumber));
            }

            return neighbors;
        }
    }
}