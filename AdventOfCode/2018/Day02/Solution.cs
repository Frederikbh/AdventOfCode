using System.Text;

using AdventOfCode.Lib;

namespace AdventOfCode._2018.Day02;

[ProblemName("Inventory Management System")]
public class Solution : ISolver
{
    public object PartOne(string input)
    {
        var parsedInput = ParseInput(input);
        return parsedInput.Count(e => HasCharWithCountOf(e, 2)) * parsedInput.Count(e => HasCharWithCountOf(e, 3));
    }

    public object PartTwo(string input)
    {
        var candidates = ParseInput(input);

        for (var i = 0; i < candidates.Length; i++)
        {
            for (var j = i + 1; j < candidates.Length; j++)
            {
                var common = CommonLetters(candidates[i], candidates[j]);

                if (common.Length == candidates[i].Length - 1)
                {
                    return common;
                }
            }
        }

        return 0;
    }

    private static string CommonLetters(string a, string b)
    {
        var sb = new StringBuilder();

        for (var i = 0; i < a.Length; i++)
        {
            if (a[i] == b[i])
            {
                sb.Append(a[i]);
            }
        }

        return sb.ToString();
    }

    private static string[] ParseInput(string input) => input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

    private static bool HasCharWithCountOf(string str, int n)
    {
        var charCount = new Dictionary<char, int>();
        foreach (var c in str)
        {
            if (!charCount.TryAdd(c, 1))
            {
                charCount[c]++;
            }
        }
        return charCount.ContainsValue(n);
    }
}
