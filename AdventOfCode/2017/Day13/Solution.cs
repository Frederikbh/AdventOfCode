using AdventOfCode.Lib;

namespace AdventOfCode._2017.Day13;

[ProblemName("Packet Scanners")]
public class Solution : ISolver
{
    public object PartOne(string input)
    {
        var scanners = ParseInput(input);

        return Travel(scanners, 0)
            .Score;
    }

    public object PartTwo(string input)
    {
        var scanners = ParseInput(input);
        var delay = 0;

        while (IsCaught(scanners, delay))
        {
            delay++;
        }

        return delay;
    }

    private static (int Score, bool Caught) Travel(Dictionary<int, int> scanners, int delay)
    {
        var score = 0;
        var caught = false;

        foreach (var (position, depth) in scanners)
        {
            if ((position + delay) % (2 * (depth - 1)) == 0)
            {
                caught = true;
                score += position * depth;
            }
        }

        return (score, caught);
    }

    private static bool IsCaught(Dictionary<int, int> scanners, int delay)
    {
        foreach (var scanner in scanners)
        {
            if ((scanner.Key + delay) % (2 * (scanner.Value - 1)) == 0)
            {
                return true;
            }
        }

        return false;
    }

    private static Dictionary<int, int> ParseInput(string input) =>
        input.Split('\n')
            .Select(line => line.Split(": "))
            .ToDictionary(parts => int.Parse(parts[0]), parts => int.Parse(parts[1]));
}
