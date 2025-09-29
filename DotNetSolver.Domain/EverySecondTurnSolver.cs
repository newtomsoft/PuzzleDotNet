using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Z3;

namespace DotNetSolver.Domain
{
    public class EverySecondTurnSolver : GameSolver<Island>
    {
        private readonly Grid<char> _inputGrid;
        private IslandsGrid? _islandGrid;
        private readonly Context _ctx;
        private readonly Solver _solver;
        private Dictionary<Position, Dictionary<Direction, IntExpr>> _islandBridgesZ3;
        private IslandsGrid? _previousSolution;

        private const char CircleChar = 'X';
        private const char EmptyChar = '.';

        public EverySecondTurnSolver(Grid<char> grid)
        {
            _inputGrid = grid;
            _ctx = new Context();
            _solver = _ctx.MkSolver();
            _islandBridgesZ3 = new Dictionary<Position, Dictionary<Direction, IntExpr>>();
            InitIslandGrid();
        }

        private void InitIslandGrid()
        {
            var matrix = new List<List<Island>>();
            for (int r = 0; r < _inputGrid.RowsNumber; r++)
            {
                var row = new List<Island>();
                for (int c = 0; c < _inputGrid.ColumnsNumber; c++)
                {
                    row.Add(new Island(new Position(r, c), 2));
                }
                matrix.Add(row);
            }
            _islandGrid = new IslandsGrid(matrix);
        }

        private void InitSolver()
        {
            _islandBridgesZ3 = new Dictionary<Position, Dictionary<Direction, IntExpr>>();
            foreach (var island in _islandGrid!.Islands.Values)
            {
                var directionDict = new Dictionary<Direction, IntExpr>();
                foreach (var direction in Direction.Orthogonals)
                {
                    directionDict[direction] = (IntExpr)_ctx.MkIntConst($"{island.Position}_{direction}");
                }
                _islandBridgesZ3[island.Position] = directionDict;
            }
            AddConstraints();
        }

        public override Grid<Island> GetSolution()
        {
            if (_solver.Assertions.Length == 0)
            {
                InitSolver();
            }

            var (solution, _) = EnsureAllIslandsConnected();
            return solution;
        }

        private (IslandsGrid, int) EnsureAllIslandsConnected()
        {
            int propositionCount = 0;
            while (_solver.Check() == Status.SATISFIABLE)
            {
                var model = _solver.Model;
                propositionCount++;

                var newIslandMatrix = new List<List<Island>>();
                for(int r = 0; r < _inputGrid.RowsNumber; r++)
                {
                    var row = new List<Island>();
                    for(int c = 0; c < _inputGrid.ColumnsNumber; c++)
                    {
                         row.Add(new Island(new Position(r,c), 2));
                    }
                    newIslandMatrix.Add(row);
                }
                var tempIslandGrid = new IslandsGrid(newIslandMatrix);


                foreach (var (position, directionBridges) in _islandBridgesZ3)
                {
                    foreach (var (direction, bridgesExpr) in directionBridges)
                    {
                        var bridgesNumber = ((IntNum)model.Eval(bridgesExpr)).Int;
                        if (bridgesNumber > 0)
                        {
                            var neighborPosition = tempIslandGrid[position].DirectionPositionBridges[direction].Item1;
                            tempIslandGrid[position].SetBridgeToPosition(neighborPosition, bridgesNumber);
                        }
                    }
                }
                foreach(var island in tempIslandGrid.Islands.Values)
                {
                    island.SetBridgesCountAccordingToDirectionsBridges();
                }

                var connectedPositions = tempIslandGrid.GetConnectedPositions(true);
                if (connectedPositions.Count == 1)
                {
                    _previousSolution = tempIslandGrid;
                    return (tempIslandGrid, propositionCount);
                }

                var notLoopConstraints = new List<BoolExpr>();
                foreach (var positions in connectedPositions)
                {
                    var cellConstraints = new List<BoolExpr>();
                    foreach (var position in positions)
                    {
                        foreach (var (direction, (_, value)) in tempIslandGrid[position].DirectionPositionBridges)
                        {
                            cellConstraints.Add(_ctx.MkEq(_islandBridgesZ3[position][direction], _ctx.MkInt(value)));
                        }
                    }
                    notLoopConstraints.Add(_ctx.MkNot(_ctx.MkAnd(cellConstraints)));
                }
                _solver.Add(_ctx.MkAnd(notLoopConstraints));
            }

            return (new IslandsGrid(new List<List<Island>>()), propositionCount);
        }

        public override Grid<Island> GetOtherSolution()
        {
            var previousSolutionConstraints = new List<BoolExpr>();
            foreach (var island in _previousSolution!.Islands.Values)
            {
                foreach (var (direction, (_, value)) in island.DirectionPositionBridges)
                {
                    previousSolutionConstraints.Add(_ctx.MkEq(_islandBridgesZ3[island.Position][direction], _ctx.MkInt(value)));
                }
            }
            _solver.Add(_ctx.MkNot(_ctx.MkAnd(previousSolutionConstraints)));

            InitIslandGrid();
            return GetSolution();
        }

        private void AddConstraints()
        {
            AddInitialConstraints();
            AddOppositeBridgesConstraints();
            AddLinksConstraints();
            AddTurnAtEveryCircleConstraints();
        }

        private void AddInitialConstraints()
        {
            foreach (var directionBridges in _islandBridgesZ3.Values)
            {
                var bridgesCountVars = directionBridges.Values.ToArray();
                _solver.Add(_ctx.MkEq(_ctx.MkAdd(bridgesCountVars), _ctx.MkInt(2)));
                foreach (var bridges in bridgesCountVars)
                {
                    _solver.Add(_ctx.MkAnd(_ctx.MkGe(bridges, _ctx.MkInt(0)), _ctx.MkLe(bridges, _ctx.MkInt(1))));
                }
            }
        }

        private void AddOppositeBridgesConstraints()
        {
            foreach (var island in _islandGrid!.Islands.Values)
            {
                foreach (var direction in Direction.Orthogonals)
                {
                    if (island.DirectionPositionBridges.TryGetValue(direction, out var bridgeInfo))
                    {
                        _solver.Add(_ctx.MkEq(_islandBridgesZ3[island.Position][direction], _islandBridgesZ3[bridgeInfo.Item1][direction.Opposite]));
                    }
                    else
                    {
                        _solver.Add(_ctx.MkEq(_islandBridgesZ3[island.Position][direction], _ctx.MkInt(0)));
                    }
                }
            }
        }

        private void AddLinksConstraints()
        {
            var circlePositions = _inputGrid.Where(kvp => kvp.Value == CircleChar).Select(kvp => kvp.Key).ToList();
            foreach (var pos in circlePositions)
            {
                var linkedCirclesConstraints = CirclesLinkedConstraints(pos, circlePositions);
                _solver.Add(_ctx.MkEq(_ctx.MkAdd(linkedCirclesConstraints.Select(c => _ctx.MkITE(c, _ctx.MkInt(1), _ctx.MkInt(0)))), _ctx.MkInt(1)));
            }
        }

        private List<BoolExpr> CirclesLinkedConstraints(Position circlePos, List<Position> allCirclePositions)
        {
            var constraints = new List<BoolExpr>();
            foreach (var otherCirclePos in allCirclePositions)
            {
                if (circlePos.R == otherCirclePos.R || circlePos.C == otherCirclePos.C) continue;

                var horTurnPos = new Position(circlePos.R, otherCirclePos.C);
                var vert_turn_pos = new Position(otherCirclePos.R, circlePos.C);
                var horDirection = otherCirclePos.C > circlePos.C ? Direction.Right : Direction.Left;
                var vertDirection = otherCirclePos.R > circlePos.R ? Direction.Down : Direction.Up;

                var horFirstConstraint = ToOtherCircleConstraint(circlePos, otherCirclePos, horTurnPos, horDirection, vertDirection);
                var vertFirstConstraint = ToOtherCircleConstraint(circlePos, otherCirclePos, vert_turn_pos, vertDirection, horDirection);

                if (horFirstConstraint != null && vertFirstConstraint != null)
                    constraints.Add(_ctx.MkOr(horFirstConstraint, vertFirstConstraint));
                else if (horFirstConstraint != null)
                    constraints.Add(horFirstConstraint);
                else if (vertFirstConstraint != null)
                    constraints.Add(vertFirstConstraint);
            }
            return constraints;
        }

        private BoolExpr? ToOtherCircleConstraint(Position circlePos, Position otherCirclePos, Position turnPosition, Direction firstDirection, Direction secondDirection)
        {
            var constraints = new List<BoolExpr> { _ctx.MkEq(_islandBridgesZ3[circlePos][firstDirection], _ctx.MkInt(1)) };
            var currentPosition = circlePos.After(firstDirection);

            while (_inputGrid.Contains(currentPosition) && _inputGrid[currentPosition] == EmptyChar && currentPosition != turnPosition)
            {
                constraints.Add(_ctx.MkEq(_islandBridgesZ3[currentPosition][firstDirection], _ctx.MkInt(1)));
                currentPosition = currentPosition.After(firstDirection);
            }

            if (!_inputGrid.Contains(currentPosition) || _inputGrid[currentPosition] != EmptyChar) return null;

            constraints.Add(_ctx.MkEq(_islandBridgesZ3[currentPosition][secondDirection], _ctx.MkInt(1)));
            currentPosition = currentPosition.After(secondDirection);

            while (_inputGrid.Contains(currentPosition) && _inputGrid[currentPosition] == EmptyChar && currentPosition != otherCirclePos)
            {
                constraints.Add(_ctx.MkEq(_islandBridgesZ3[currentPosition][secondDirection], _ctx.MkInt(1)));
                currentPosition = currentPosition.After(secondDirection);
            }

            if (currentPosition != otherCirclePos) return null;

            return _ctx.MkAnd(constraints);
        }

        private void AddTurnAtEveryCircleConstraints()
        {
            var circlePositions = _inputGrid.Where(kvp => kvp.Value == CircleChar).Select(kvp => kvp.Key);
            foreach (var pos in circlePositions)
            {
                var right = _islandBridgesZ3[pos][Direction.Right];
                var up = _islandBridgesZ3[pos][Direction.Up];
                var left = _islandBridgesZ3[pos][Direction.Left];
                var down = _islandBridgesZ3[pos][Direction.Down];
                _solver.Add(_ctx.MkOr(
                    _ctx.MkAnd(_ctx.MkEq(right, _ctx.MkInt(1)), _ctx.MkEq(up, _ctx.MkInt(1)), _ctx.MkEq(left, _ctx.MkInt(0)), _ctx.MkEq(down, _ctx.MkInt(0))),
                    _ctx.MkAnd(_ctx.MkEq(right, _ctx.MkInt(1)), _ctx.MkEq(up, _ctx.MkInt(0)), _ctx.MkEq(left, _ctx.MkInt(0)), _ctx.MkEq(down, _ctx.MkInt(1))),
                    _ctx.MkAnd(_ctx.MkEq(right, _ctx.MkInt(0)), _ctx.MkEq(up, _ctx.MkInt(0)), _ctx.MkEq(left, _ctx.MkInt(1)), _ctx.MkEq(down, _ctx.MkInt(1))),
                    _ctx.MkAnd(_ctx.MkEq(right, _ctx.MkInt(0)), _ctx.MkEq(up, _ctx.MkInt(1)), _ctx.MkEq(left, _ctx.MkInt(1)), _ctx.MkEq(down, _ctx.MkInt(0)))
                ));
            }
        }
    }
}