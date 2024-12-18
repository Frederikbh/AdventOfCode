using System.Text;

using AdventOfCode.Lib;

namespace AdventOfCode._2017.Day14;

[ProblemName("Disk Defragmentation")]
public class Solution : ISolver
{
    private const int GridSize = 128;

    public object PartOne(string input) =>
        Parse(input)
            .SelectMany(row => row)
            .Count(c => c == '#');

    public object PartTwo(string input)
    {
        var grid = Parse(input);
        var regionCount = 0;

        for (var y = 0; y < GridSize; y++)
        {
            for (var x = 0; x < GridSize; x++)
            {
                if (grid[y][x] == '#')
                {
                    regionCount++;
                    FillRegion(grid, new Point(x, y));
                }
            }
        }

        return regionCount;
    }

    private static void FillRegion(char[][] grid, Point startPos)
    {
        var stack = new Stack<Point>();
        stack.Push(startPos);

        while (stack.Count > 0)
        {
            var pos = stack.Pop();

            if (pos.X < 0 || pos.X >= GridSize || pos.Y < 0 || pos.Y >= GridSize || grid[pos.Y][pos.X] != '#')
            {
                continue;
            }

            grid[pos.Y][pos.X] = ' ';

            stack.Push(new Point(pos.X + 1, pos.Y));
            stack.Push(new Point(pos.X - 1, pos.Y));
            stack.Push(new Point(pos.X, pos.Y + 1));
            stack.Push(new Point(pos.X, pos.Y - 1));
        }
    }

    private static char[][] Parse(string input)
    {
        var grid = new char[GridSize][];

        for (var i = 0; i < GridSize; i++)
        {
            grid[i] = KnotToString(DenseHash(input + "-" + i))
                .ToCharArray();
        }

        return grid;
    }

    private static string KnotToString(int[] knot)
    {
        var result = new StringBuilder();

        foreach (var b in knot)
        {
            for (var bit = 0; bit < 8; bit++)
            {
                result.Append(
                    (b & (1 << (7 - bit))) != 0
                        ? '#'
                        : '.');
            }
        }

        return result.ToString();
    }

    private static int[] DenseHash(string input)
    {
        var knot = KnotHash(input);
        var dense = new int[16];

        for (var i = 0; i < 16; i++)
        {
            dense[i] = knot[i * 16];

            for (var j = 1; j < 16; j++)
            {
                dense[i] ^= knot[i * 16 + j];
            }
        }

        return dense;
    }

    private static int[] KnotHash(string input)
    {
        var suffix = new[]
        {
            17,
            31,
            73,
            47,
            23
        };
        var lengths = input.Select(c => (int)c)
            .Concat(suffix)
            .ToArray();
        var list = Enumerable.Range(0, 256)
            .ToArray();
        var position = 0;
        var skip = 0;

        for (var round = 0; round < 64; round++)
        {
            foreach (var length in lengths)
            {
                position = Hash(list, position, length, skip++);
            }
        }

        return list;
    }

    public static int Hash(int[] list, int position, int length, int skip)
    {
        var listLength = list.Length;

        for (var i = 0; i < length / 2; i++)
        {
            var a = (position + i) % listLength;
            var b = (position + length - i - 1) % listLength;
            (list[a], list[b]) = (list[b], list[a]);
        }

        return (position + length + skip) % list.Length;
    }

    private record struct Point(int X, int Y);
}
