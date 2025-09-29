using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetSolver.Domain;

namespace DotNetSolver.Utils
{
    public static class ShapePrinter
    {
        public static string GetShapeWithBackgroundString(ICollection<Position> shape)
        {
            if (!shape.Any()) return "";

            var maxR = (int)shape.Max(p => p.R) + 1;
            var maxC = (int)shape.Max(p => p.C) + 1;
            var emptyCell = "   ";
            var fullCell = "███";

            var sb = new StringBuilder();
            for (int r = -1; r < maxR; r++)
            {
                for (int c = -1; c < maxC; c++)
                {
                    sb.Append(shape.Contains(new Position(r, c)) ? fullCell : emptyCell);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}