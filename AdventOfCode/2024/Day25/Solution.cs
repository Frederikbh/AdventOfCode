using AdventOfCode.Lib;

namespace AdventOfCode._2024.Day25;

[ProblemName("Code Chronicle")]
public class Solution : ISolver
{
    public object PartOne(string input)
    {
        var (locks, keys) = ParseInput(input);

        return locks.Sum(@lock => keys.Count(key => IsCompatible(@lock, key)));
    }

    public object PartTwo(string input) => 0;

    private static (List<int[]> Locks, List<int[]> Keys) ParseInput(string input)
    {
        var segments = input.Split("\n\n");
        var keys = new List<int[]>();
        var locks = new List<int[]>();

        foreach (var segment in segments)
        {
            var arr = segment.Split('\n')
                .Select(e => e.ToCharArray())
                .ToArray();

            if (arr.First()
                .All(e => e == '#'))
            {
                locks.Add(ParseLock(arr));
            }
            else
            {
                keys.Add(ParseKey(arr));
            }
        }

        return (locks, keys);
    }

    private static int[] ParseLock(char[][] input) => Parse(input, true);

    private static int[] ParseKey(char[][] input) => Parse(input, false);

    private static int[] Parse(char[][] input, bool isLock)
    {
        var result = new int[input[0].Length];

        for (var i = 0; i < input[0].Length; i++)
        {
            var height = 0;

            for (var j = 0; j < input.Length; j++)
            {
                var index = isLock
                    ? j
                    : input.Length - j - 1;

                if (input[index][i] != '#')
                {
                    break;
                }

                height++;
            }

            result[i] = height;
        }

        return result;
    }

    private static bool IsCompatible(int[] @lock, int[] key)
    {
        for (var i = 0; i < @lock.Length; i++)
        {
            if (key[i] + @lock[i] > 7)
            {
                return false;
            }
        }

        return true;
    }
}
