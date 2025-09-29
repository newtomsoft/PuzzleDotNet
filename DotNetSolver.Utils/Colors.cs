using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DotNetSolver.Utils
{
    public static class Colors
    {
        public static readonly Dictionary<object, string> ConsoleBackgroundColors = new Dictionary<object, string>
        {
            { 0, "\u001b[40m" },   // Black background
            { 1, "\u001b[107m" },  // White background
            { 2, "\u001b[43m" },   // Yellow background
            { 3, "\u001b[44m" },   // Blue background
            { 4, "\u001b[45m" },   // Magenta background
            { 5, "\u001b[48;5;164m" }, // Light blue background
            { 6, "\u001b[42m" },   // Green background
            { 7, "\u001b[48;5;162m" }, // Pink background
            { 8, "\u001b[101m" },  // Light red background
            { 9, "\u001b[102m" },  // Light green background
            { 10, "\u001b[103m" }, // Light yellow background
            { 11, "\u001b[105m" }, // Light magenta background
            { 12, "\u001b[48;5;208m" }, // Orange background
            { 13, "\u001b[48;5;226m" }, // Light yellow background
            { 14, "\u001b[48;5;27m" },  // Light blue background
            { 15, "\u001b[48;5;160m" }, // Red background
            { 16, "\u001b[41m" },   // Red background
            { 17, "\u001b[107m" },  // Light grey background
            { 18, "\u001b[46m" },   // Cyan background
            { 19, "\u001b[48;5;165m" }, // Light magenta background
            { 20, "\u001b[48;5;166m" }, // Light orange background
            { 21, "\u001b[48;5;167m" }, // Light yellow background
            { 22, "\u001b[48;5;168m" }, // Light green background
            { "end", "\u001b[0m" }     // Reset
        };

        public static readonly Dictionary<object, string> ConsolePoliceColors = new Dictionary<object, string>
        {
            { 0, "\u001b[95m" },   // Magenta light
            { 1, "\u001b[93m" },   // Yellow light
            { 2, "\u001b[96m" },   // Cyan light
            { 3, "\u001b[92m" },   // Green light
            { 4, "\u001b[97m" },   // White
            { 5, "\u001b[91m" },   // Red light
            { 6, "\u001b[32m" },   // Green
            { 7, "\u001b[90m" },   // Grey
            { 8, "\u001b[35m" },   // Magenta
            { 9, "\u001b[36m" },   // Cyan
            { 10, "\u001b[34m" },  // Blue
            { 11, "\u001b[31m" },  // Red
            { 12, "\u001b[33m" },  // Yellow
            { 13, "\u001b[94m" },  // Blue light
            { 14, "\u001b[38;5;208m" }, // Orange
            { 15, "\u001b[38;5;226m" }, // Light yellow
            { 16, "\u001b[30m" },  // Black
            { "end", "\u001b[0m" }
        };
    }
}