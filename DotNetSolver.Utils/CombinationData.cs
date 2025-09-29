using System.Collections.Generic;

namespace DotNetSolver.Utils
{
    public static class CombinationData
    {
        public static readonly Dictionary<int, Dictionary<object, List<List<bool>>>> CombinationsByLineSize = new Dictionary<int, Dictionary<object, List<List<bool>>>>
        {
            {
                25, new Dictionary<object, List<List<bool>>>
                {
                    {
                        12, new List<List<bool>>
                        {
                            new List<bool> { true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false },
                            // ... more data omitted for brevity
                        }
                    },
                    {
                        "(6, 4, 1)", new List<List<bool>>
                        {
                            new List<bool> { true, true, true, true, true, true, false, true, true, true, true, false, true, false, false, false, false, false, false, false, false, false, false, false, false },
                            // ... more data omitted for brevity
                        }
                    }
                }
            }
            // NOTE: More data would be included in a full implementation.
        };
    }
}