using AdventOfCode.Lib;

namespace AdventOfCode._2024.Day21;

[ProblemName("Keypad Conundrum")]
public class Solution : ISolver
{
    public object PartOne(string input) => Solve(input, 2);

    public object PartTwo(string input) => Solve(input, 25);

    private static long Solve(string input, int depth)
    {
        var numpad = ParseKeypad("789\n456\n123\n 0A");
        var keypad = ParseKeypad(" ^A\n<v>");
        var keypads = Enumerable.Repeat(keypad, depth)
            .Prepend(numpad)
            .ToArray();

        var cache = new Dictionary<(char current, char next, int depth), long>();
        var result = 0L;

        var lines = input.Split('\n');

        foreach (var line in lines)
        {
            var num = int.Parse(line[..^1]);
            result += num * EncodeKeys(line, keypads, cache);
        }

        return result;
    }

    private static long EncodeKeys(
        string keys,
        Dictionary<Point, char>[] keypads,
        Dictionary<(char current, char next, int depth), long> cache)
    {
        if (keypads.Length == 0)
        {
            return keys.Length;
        }

        var current = 'A';
        var length = 0L;

        foreach (var next in keys)
        {
            length += EncodeKey(current, next, keypads, cache);
            current = next;
        }

        return length;
    }

    private static long EncodeKey(
        char current,
        char next,
        Dictionary<Point, char>[] keypads,
        Dictionary<(char current, char next, int depth), long> cache)
    {
        var cacheKey = (current, next, keypads.Length);

        if (cache.TryGetValue(cacheKey, out var key))
        {
            return key;
        }

        var keypad = keypads[0];

        var currentPos = keypad.Single(e => e.Value == current)
            .Key;
        var nextPos = keypad.Single(e => e.Value == next)
            .Key;

        var dy = nextPos.Y - currentPos.Y;
        var dx = nextPos.X - currentPos.X;

        var vertical = new string(
            dy < 0
                ? 'v'
                : '^',
            Math.Abs(dy));
        var horizontal = new string(
            dx < 0
                ? '<'
                : '>',
            Math.Abs(dx));

        var cost = long.MaxValue;

        if (keypad[new Point(currentPos.X, nextPos.Y)] != ' ')
        {
            cost = Math.Min(cost, EncodeKeys($"{vertical}{horizontal}A", keypads[1..], cache));
        }

        if (keypad[new Point(nextPos.X, currentPos.Y)] != ' ')
        {
            cost = Math.Min(cost, EncodeKeys($"{horizontal}{vertical}A", keypads[1..], cache));
        }

        cache.Add(cacheKey, cost);

        return cost;
    }

    private static Dictionary<Point, char> ParseKeypad(string keypad)
    {
        var lines = keypad.Split('\n');
        var result = new Dictionary<Point, char>();

        for (var y = 0; y < lines.Length; y++)
        {
            for (var x = 0; x < lines[y].Length; x++)
            {
                result.Add(new Point(x, -y), lines[y][x]);
            }
        }

        return result;
    }

    private record struct Point(int X, int Y);
}
