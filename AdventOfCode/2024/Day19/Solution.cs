using AdventOfCode.Lib;

namespace AdventOfCode._2024.Day19;

[ProblemName("Linen Layout")]
public class Solution : ISolver
{
    public object PartOne(string input)
    {
        var (patterns, displays) = ParseInput(input);

        return displays
            .Count(e => CountPossibleCombinations(patterns, e) > 0);
    }

    public object PartTwo(string input)
    {
        var (patterns, displays) = ParseInput(input);

        return displays
            .Sum(e => CountPossibleCombinations(patterns, e));
    }

    private static long CountPossibleCombinations(string[] patterns, ReadOnlySpan<char> display)
    {
        var dp = new long[display.Length + 1];
        dp[0] = 1;

        for (var i = 0; i < display.Length; i++)
        {
            if (dp[i] == 0)
            {
                continue;
            }

            foreach (var pattern in patterns)
            {
                if (i + pattern.Length <= display.Length && display.Slice(i, pattern.Length)
                        .SequenceEqual(pattern))
                {
                    dp[i + pattern.Length] += dp[i];
                }
            }
        }

        return dp[display.Length];
    }

    private static (string[] Patterns, string[] Displays) ParseInput(string input)
    {
        var parts = input.Split("\n\n");

        var patterns = parts[0]
            .Split(", ");
        var displays = parts[1]
            .Split('\n');

        return (patterns, displays);
    }
}
