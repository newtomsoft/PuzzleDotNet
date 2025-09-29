namespace DotNetSolver.Domain
{
    public abstract class GameSolver<T>
    {
        public abstract Grid<T> GetSolution();
        public abstract Grid<T> GetOtherSolution();
    }
}