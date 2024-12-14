using AdventOfCode.Lib;

namespace AdventOfCode._2024.Day11;

[ProblemName("Plutonian Pebbles")]
public class Solution : ISolver
{
    public object PartOne(string input) => Solve(input, 25);

    public object PartTwo(string input) => Solve(input, 75);

    private static long Solve(string input, int iterations)
    {
        var stoneCounts = new Dictionary<long, long>();

        foreach (var stone in ParseInput(input))
        {
            if (!stoneCounts.TryAdd(stone, 1))
            {
                stoneCounts[stone]++;
            }
        }

        for (var i = 0; i < iterations; i++)
        {
            var newStoneCounts = new Dictionary<long, long>();

            foreach (var (stone, count) in stoneCounts)
            {
                foreach (var transformedStone in Transform(stone))
                {
                    if (!newStoneCounts.TryAdd(transformedStone, count))
                    {
                        newStoneCounts[transformedStone] += count;
                    }
                }
            }

            stoneCounts = newStoneCounts;
        }

        return stoneCounts.Values.Sum();
    }

    private static List<long> Transform(long stone)
    {
        if (stone == 0)
        {
            return [1];
        }

        if (IsEvenLength(stone))
        {
            var digitCount = (int)Math.Floor(Math.Log10(Math.Abs(stone))) + 1;
            var halfDigits = digitCount / 2;
            var divisor = (long)Math.Pow(10, halfDigits);

            var firstPart = stone / divisor;
            var secondPart = stone % divisor;

            return
            [
                firstPart,
                secondPart
            ];
        }

        return [stone * 2024];
    }

    private static bool IsEvenLength(long stone)
    {
        var length = (int)Math.Floor(Math.Log10(Math.Abs(stone))) + 1;

        return length % 2 == 0;
    }

    private static List<long> ParseInput(string input) =>
        input.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(long.Parse)
            .ToList();
}
