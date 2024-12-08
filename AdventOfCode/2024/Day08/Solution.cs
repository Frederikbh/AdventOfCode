using AdventOfCode.Lib;

namespace AdventOfCode._2024.Day08;

[ProblemName("Resonant Collinearity")]
public class Solution : ISolver
{
    public object PartOne(string input)
    {
        var (size, antennas) = ParseAntennas(input);
        var antiNodes = CalculateAntiNodes(size, antennas, false);

        return antiNodes.Count;
    }

    public object PartTwo(string input)
    {
        var (size, antennas) = ParseAntennas(input);
        var antiNodes = CalculateAntiNodes(size, antennas, true);

        return antiNodes.Count;
    }

    private static HashSet<Point> CalculateAntiNodes(int size, Dictionary<char, List<Point>> antennas, bool extend)
    {
        var antiNodes = new HashSet<Point>();

        foreach (var antenna in antennas)
        {
            for (var i = 0; i < antenna.Value.Count; i++)
            {
                for (var j = i + 1; j < antenna.Value.Count; j++)
                {
                    var p1 = antenna.Value[i];
                    var p2 = antenna.Value[j];
                    var dx = p2.X - p1.X;
                    var dy = p2.Y - p1.Y;

                    var p3 = new Point(p2.X + dx, p2.Y + dy);
                    var p4 = new Point(p1.X - dx, p1.Y - dy);

                    if (extend)
                    {
                        antiNodes.Add(p1);
                        antiNodes.Add(p2);
                        ExtendAntiNodes(size, antiNodes, p3, dx, dy);
                        ExtendAntiNodes(size, antiNodes, p4, -dx, -dy);
                    }
                    else
                    {
                        if (IsInBounds(size, p3.X, p3.Y))
                        {
                            antiNodes.Add(p3);
                        }

                        if (IsInBounds(size, p4.X, p4.Y))
                        {
                            antiNodes.Add(p4);
                        }
                    }
                }
            }
        }

        return antiNodes;
    }

    private static void ExtendAntiNodes(int size, HashSet<Point> antiNodes, Point start, int dx, int dy)
    {
        var current = start;

        while (IsInBounds(size, current.X, current.Y))
        {
            antiNodes.Add(current);
            current = new Point(current.X + dx, current.Y + dy);
        }
    }

    private static bool IsInBounds(int size, int x, int y) => x >= 0 && x < size && y >= 0 && y < size;

    private static (int Size, Dictionary<char, List<Point>> Antennas) ParseAntennas(string input)
    {
        var lines = input.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        var antennas = new Dictionary<char, List<Point>>();

        for (var y = 0; y < lines.Length; y++)
        {
            var line = lines[y];

            for (var x = 0; x < line.Length; x++)
            {
                var c = line[x];

                if (c == '.')
                {
                    continue;
                }

                if (!antennas.TryGetValue(c, out var value))
                {
                    value = [];
                    antennas[c] = value;
                }

                value.Add(new Point(x, y));
            }
        }

        return (lines.Length, antennas);
    }

    private record struct Point(int X, int Y);
}
