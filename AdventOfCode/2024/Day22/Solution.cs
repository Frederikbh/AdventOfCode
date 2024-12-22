using AdventOfCode.Lib;

namespace AdventOfCode._2024.Day22;

[ProblemName("Monkey Market")]
public class Solution : ISolver
{
    public object PartOne(string input)
    {
        var numbers = ParseInput(input);
        long sum = 0;

        foreach (var n in numbers)
        {
            var secretArr = SecretNumbers(n);
            sum += secretArr[^1];
        }

        return sum;
    }

    public object PartTwo(string input)
    {
        var numbers = ParseInput(input);
        var bestWindow = new Dictionary<int, long>();

        foreach (var number in numbers)
        {
            var windows = ComputeWindows(number);

            foreach (var (key, val) in windows)
            {
                if (!bestWindow.TryAdd(key, val))
                {
                    bestWindow[key] += val;
                }
            }
        }

        return bestWindow.Values.Max();
    }

    private static long[] SecretNumbers(long number)
    {
        var arr = new long[2001];
        arr[0] = number;

        for (var i = 0; i < 2000; i++)
        {
            var first = MixAndPrune(number, number * 64);
            var second = MixAndPrune(first, (long)Math.Floor(first / 32d));
            number = MixAndPrune(second, second * 2048);
            arr[i + 1] = number;
        }

        return arr;
    }

    private static Dictionary<int, long> ComputeWindows(long number)
    {
        var secretArr = SecretNumbers(number);
        var bananas = new int[secretArr.Length];

        for (var i = 0; i < secretArr.Length; i++)
        {
            bananas[i] = (int)(secretArr[i] % 10);
        }

        var windows = new Dictionary<int, long>();

        for (var i = 5; i < bananas.Length; i++)
        {
            var d0 = bananas[i - 4] - bananas[i - 5];
            var d1 = bananas[i - 3] - bananas[i - 4];
            var d2 = bananas[i - 2] - bananas[i - 3];
            var d3 = bananas[i - 1] - bananas[i - 2];

            // Shift each diff by 9 (from range -9..9 to 0..18),
            var dd0 = (int)(d0 + 9);
            var dd1 = (int)(d1 + 9);
            var dd2 = (int)(d2 + 9);
            var dd3 = (int)(d3 + 9);

            // Combine them into a 20-bit int
            var key = dd0
                | (dd1 << 5)
                | (dd2 << 10)
                | (dd3 << 15);

            if (!windows.ContainsKey(key))
            {
                windows[key] = bananas[i - 1];
            }
        }

        return windows;
    }

    private static long MixAndPrune(long number, long secret) => (number ^ secret) % 16777216;

    private static long[] ParseInput(string input) =>
        input.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(long.Parse)
            .ToArray();
}
