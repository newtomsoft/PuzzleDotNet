using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace DotNetSolver.Domain
{
    public class GridBase<T> : IEnumerable<KeyValuePair<Position, T>>
    {
        protected readonly T[,] Matrix;
        private ISet<ImmutableHashSet<Position>> _walls;

        public int RowsNumber { get; }
        public int ColumnsNumber { get; }

        public GridBase(T[,] matrix)
        {
            Matrix = matrix;
            RowsNumber = matrix.GetLength(0);
            ColumnsNumber = matrix.GetLength(1);
            _walls = new HashSet<ImmutableHashSet<Position>>();
        }

        public GridBase(List<List<T>> matrix)
        {
            RowsNumber = matrix.Count;
            ColumnsNumber = matrix.Count > 0 ? matrix[0].Count : 0;
            Matrix = new T[RowsNumber, ColumnsNumber];
            for (int r = 0; r < RowsNumber; r++)
            {
                for (int c = 0; c < ColumnsNumber; c++)
                {
                    Matrix[r, c] = matrix[r][c];
                }
            }
            _walls = new HashSet<ImmutableHashSet<Position>>();
        }

        public T this[Position key]
        {
            get => Matrix[(int)key.R, (int)key.C];
            set => Matrix[(int)key.R, (int)key.C] = value;
        }

        public T this[int r, int c]
        {
            get => Matrix[r, c];
            set => Matrix[r, c] = value;
        }

        public ISet<ImmutableHashSet<Position>> Walls => _walls;

        public static GridBase<T> Empty() => new GridBase<T>(new T[0, 0]);

        public bool IsEmpty() => RowsNumber == 0 || ColumnsNumber == 0;

        public bool Contains(Position item) =>
            item.R >= 0 && item.R < RowsNumber &&
            item.C >= 0 && item.C < ColumnsNumber;

        public void SetValue(Position position, T value)
        {
            if (Contains(position))
            {
                this[position] = value;
            }
        }

        public int GetIndexFromPosition(Position position) => (int)position.R * ColumnsNumber + (int)position.C;
        public Position GetPositionFromIndex(int index) => new Position(index / ColumnsNumber, index % ColumnsNumber);

        public IReadOnlyDictionary<T, IReadOnlySet<Position>> GetRegions()
        {
            var regions = new Dictionary<T, HashSet<Position>>();
            foreach (var (position, cell) in this)
            {
                if (!regions.TryGetValue(cell, out var positions))
                {
                    positions = new HashSet<Position>();
                    regions[cell] = positions;
                }
                positions.Add(position);
            }
            return regions.ToDictionary(kvp => kvp.Key, kvp => (IReadOnlySet<Position>)kvp.Value.ToImmutableHashSet());
        }

        public bool AreCellsConnected(T value, string mode = "orthogonal")
        {
            var positionsOfValue = this.Where(kvp => EqualityComparer<T>.Default.Equals(kvp.Value, value)).ToList();
            if (!positionsOfValue.Any())
            {
                return true; // Or false, depending on desired behavior for no cells
            }
            var startPosition = positionsOfValue.First().Key;
            var visited = DepthFirstSearch(startPosition, value, mode);
            return visited.Count == positionsOfValue.Count;
        }

        public ISet<IReadOnlySet<Position>> GetAllShapes(T value, string mode = "orthogonal")
        {
            var shapes = new HashSet<IReadOnlySet<Position>>(HashSet<Position>.CreateSetComparer());
            var visitedOverall = new HashSet<Position>();

            foreach (var (pos, val) in this)
            {
                if (EqualityComparer<T>.Default.Equals(val, value) && !visitedOverall.Contains(pos))
                {
                    var shape = DepthFirstSearch(pos, value, mode);
                    shapes.Add(shape.ToImmutableHashSet());
                    visitedOverall.UnionWith(shape);
                }
            }
            return shapes;
        }

        private HashSet<Position> DepthFirstSearch(Position startPosition, T value, string mode)
        {
            var visited = new HashSet<Position>();
            var stack = new Stack<Position>();

            stack.Push(startPosition);

            while (stack.Count > 0)
            {
                var position = stack.Pop();
                if (visited.Contains(position) || !Contains(position) || !EqualityComparer<T>.Default.Equals(this[position], value))
                    continue;

                visited.Add(position);

                foreach (var neighbor in NeighborsPositions(position, mode))
                {
                    stack.Push(neighbor);
                }
            }
            return visited;
        }

        public void AddWall(Position p1, Position p2)
        {
            _walls.Add(new[] { p1, p2 }.ToImmutableHashSet(PositionComparer.Default));
        }

        public class PositionComparer : IEqualityComparer<Position>
        {
            public static readonly PositionComparer Default = new PositionComparer();
            public bool Equals(Position x, Position y) => x.Equals(y);
            public int GetHashCode(Position obj) => obj.GetHashCode();
        }

        public IEnumerable<Position> NeighborsPositions(Position position, string mode = "orthogonal")
        {
            var neighbors = new List<Position>();
            if (mode is "orthogonal" or "diagonal")
            {
                if (position.Up.R >= 0 && !IsWall(position, position.Up)) neighbors.Add(position.Up);
                if (position.Down.R < RowsNumber && !IsWall(position, position.Down)) neighbors.Add(position.Down);
                if (position.Left.C >= 0 && !IsWall(position, position.Left)) neighbors.Add(position.Left);
                if (position.Right.C < ColumnsNumber && !IsWall(position, position.Right)) neighbors.Add(position.Right);
            }
            if (mode is "diagonal" or "diagonal_only")
            {
                 if (Contains(position.UpLeft)) neighbors.Add(position.UpLeft);
                 if (Contains(position.UpRight)) neighbors.Add(position.UpRight);
                 if (Contains(position.DownLeft)) neighbors.Add(position.DownLeft);
                 if (Contains(position.DownRight)) neighbors.Add(position.DownRight);
            }
            return neighbors.Where(Contains);
        }

        private bool IsWall(Position p1, Position p2)
        {
            var wall = new[] { p1, p2 }.ToImmutableHashSet(PositionComparer.Default);
            return _walls.Contains(wall);
        }

        public static (GridBase<bool>, Position) FromPositions(IEnumerable<Position> positions)
        {
            var posList = positions.ToList();
            if (!posList.Any())
                return (new GridBase<bool>(new bool[0,0]), new Position(0,0));

            var minR = posList.Min(p => p.R);
            var maxR = posList.Max(p => p.R);
            var minC = posList.Min(p => p.C);
            var maxC = posList.Max(p => p.C);

            var matrix = new bool[(int)(maxR - minR + 1), (int)(maxC - minC + 1)];
            var offset = new Position(minR, minC);

            foreach (var pos in posList)
            {
                matrix[(int)(pos.R - minR), (int)(pos.C - minC)] = true;
            }

            return (new GridBase<bool>(matrix), offset);
        }

        public IEnumerator<KeyValuePair<Position, T>> GetEnumerator()
        {
            for (int r = 0; r < RowsNumber; r++)
            {
                for (int c = 0; c < ColumnsNumber; c++)
                {
                    yield return new KeyValuePair<Position, T>(new Position(r, c), Matrix[r, c]);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (int r = 0; r < RowsNumber; r++)
            {
                for (int c = 0; c < ColumnsNumber; c++)
                {
                    sb.Append(this[r, c]?.ToString() ?? ".");
                    if (c < ColumnsNumber - 1) sb.Append(" ");
                }
                if (r < RowsNumber - 1) sb.AppendLine();
            }
            return sb.ToString();
        }

        public override bool Equals(object? obj)
        {
            if (obj is not GridBase<T> other) return false;
            if (RowsNumber != other.RowsNumber || ColumnsNumber != other.ColumnsNumber) return false;

            for (int r = 0; r < RowsNumber; r++)
            {
                for (int c = 0; c < ColumnsNumber; c++)
                {
                    if (!EqualityComparer<T>.Default.Equals(this[r, c], other[r, c]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            int hashCode = 17;
            for (int r = 0; r < RowsNumber; r++)
            {
                for (int c = 0; c < ColumnsNumber; c++)
                {
                    hashCode = hashCode * 31 + EqualityComparer<T>.Default.GetHashCode(this[r, c]);
                }
            }
            return hashCode;
        }
    }
}