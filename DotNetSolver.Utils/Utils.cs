using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DotNetSolver.Utils
{
    public static class Utils
    {
        public static bool IsPerfectSquare(int n)
        {
            if (n < 0) return false;
            int sqrt = (int)Math.Sqrt(n);
            return sqrt * sqrt == n;
        }

        public static string CleanAnsiEscapeCodes(string text)
        {
            return Regex.Replace(text, @"\x1B[@-_][0-?]*[ -/]*[@-~]", "");
        }

        public static IEnumerable<T> CustomTqdm<T>(IEnumerable<T> iterable, string desc = "", string unit = "it", int? total = null)
        {
            var collection = iterable as ICollection<T> ?? new List<T>(iterable);
            if (total == null)
            {
                total = collection.Count;
            }

            var stopwatch = Stopwatch.StartNew();

            int i = 0;
            foreach (var item in collection)
            {
                yield return item;
                i++;
                var elapsed = stopwatch.Elapsed;
                var progress = (double)i / total.Value;
                var barLength = 40;
                var block = (int)Math.Round(barLength * progress);
                var text = $"\r{desc} [{new string('#', block)}{new string('-', barLength - block)}] {i}/{total} {unit} ({elapsed.TotalSeconds:F2}s)";
                Console.Write(text);
            }
            Console.WriteLine();
        }
    }
}