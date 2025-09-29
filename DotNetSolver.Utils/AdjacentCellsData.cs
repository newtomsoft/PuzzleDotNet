using System.Collections;
using System.Collections.Generic;

namespace DotNetSolver.Utils
{
    public static class AdjacentCellsData
    {
        public static readonly Dictionary<int, Dictionary<int, List<List<bool>>>> AdjacentCellsByLineSize = new Dictionary<int, Dictionary<int, List<List<bool>>>>
        {
            {
                5, new Dictionary<int, List<List<bool>>>
                {
                    { 0, new List<List<bool>> { new List<bool> { false, false, false, false, false } } },
                    { 1, new List<List<bool>> {
                        new List<bool> { true, false, false, false, false },
                        new List<bool> { false, true, false, false, false },
                        new List<bool> { false, false, true, false, false },
                        new List<bool> { false, false, false, true, false },
                        new List<bool> { false, false, false, false, true }
                    }},
                    { 2, new List<List<bool>> {
                        new List<bool> { true, true, false, false, false },
                        new List<bool> { false, true, true, false, false },
                        new List<bool> { false, false, true, true, false },
                        new List<bool> { false, false, false, true, true }
                    }},
                    { 3, new List<List<bool>> {
                        new List<bool> { true, true, true, false, false },
                        new List<bool> { false, true, true, true, false },
                        new List<bool> { false, false, true, true, true }
                    }},
                    { 4, new List<List<bool>> {
                        new List<bool> { true, true, true, true, false },
                        new List<bool> { false, true, true, true, true }
                    }},
                    { 5, new List<List<bool>> { new List<bool> { true, true, true, true, true } } }
                }
            }
            // NOTE: Data for sizes 10, 15, 20, 25 has been omitted for brevity,
            // but would be included in a full implementation.
        };

        // NOTE: The BitArray version has been omitted as it can be generated from the above data.
    }
}