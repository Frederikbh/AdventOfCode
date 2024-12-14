using AdventOfCode.Lib;

namespace AdventOfCode._2024.Day12;

[ProblemName("Garden Groups")]
public class Solution : ISolver
{
    private static readonly (int dx, int dy)[] s_directions =
    [
        (0, 1),
        (0, -1),
        (1, 0),
        (-1, 0)
    ];

    public object PartOne(string input)
    {
        var map = ParseInput(input);
        var regions = GetRegions(map);
        var score = regions.Select(e => e.Count * CalculatePerimeter(map, e))
            .Sum();

        return score;
    }

    public object PartTwo(string input)
    {
        var map = ParseInput(input);
        var regions = GetRegions(map);
        var score = regions.Select(e => e.Count * CalculateSides(map, e))
            .Sum();

        return score;
    }

    private static List<HashSet<Point>> GetRegions(char[][] map)
    {
        var visited = new HashSet<Point>();
        var regions = new List<HashSet<Point>>();

        for (var y = 0; y < map.Length; y++)
        {
            for (var x = 0; x < map[y].Length; x++)
            {
                var position = new Point(x, y);

                if (!visited.Contains(position))
                {
                    var region = new HashSet<Point>();
                    ExploreRegion(map, visited, position, map[y][x], region);
                    regions.Add(region);
                }
            }
        }

        return regions;
    }

    private static void ExploreRegion(
        char[][] map,
        HashSet<Point> visited,
        Point position,
        char regionChar,
        HashSet<Point> region)
    {
        if (visited.Contains(position) || !IsInBounds(map, position.X, position.Y) ||
            map[position.Y][position.X] != regionChar)
        {
            return;
        }

        visited.Add(position);
        region.Add(position);

        foreach (var direction in s_directions)
        {
            var newPosition = new Point(position.X + direction.dx, position.Y + direction.dy);
            ExploreRegion(map, visited, newPosition, regionChar, region);
        }
    }

    private static int CalculatePerimeter(char[][] map, HashSet<Point> region)
    {
        var perimeter = 0;

        foreach (var point in region)
        {
            foreach (var direction in s_directions)
            {
                var newPosition = new Point(point.X + direction.dx, point.Y + direction.dy);

                if (!IsInBounds(map, newPosition.X, newPosition.Y) || !region.Contains(newPosition))
                {
                    perimeter++;
                }
            }
        }

        return perimeter;
    }

    private static int CalculateSides(char[][] map, HashSet<Point> region) => region.Sum(point => FindCorners(map, point));

    private static int FindCorners(char[][] map, Point pt)
    {
        var res = 0;
        var regionChar = map[pt.Y][pt.X];

        var cornerPairs = new[]
        {
            ((0, -1), (1, 0)), // Up, Right
            ((1, 0), (0, 1)), // Right, Down
            ((0, 1), (-1, 0)), // Down, Left
            ((-1, 0), (0, -1)) // Left, Up
        };

        foreach (var ((dx1, dy1), (dx2, dy2)) in cornerPairs)
        {
            var neighbor1 = new Point(pt.X + dx1, pt.Y + dy1);
            var neighbor2 = new Point(pt.X + dx2, pt.Y + dy2);
            var diagonal = new Point(pt.X + dx1 + dx2, pt.Y + dy1 + dy2);

            // convex corner: both neighbors outside region
            if (GetValueOrDefault(map, neighbor1) != regionChar &&
                GetValueOrDefault(map, neighbor2) != regionChar)
            {
                res++;
            }

            // concave corner: both neighbors inside region, diagonal not inside
            if (GetValueOrDefault(map, neighbor1) == regionChar &&
                GetValueOrDefault(map, neighbor2) == regionChar &&
                GetValueOrDefault(map, diagonal) != regionChar)
            {
                res++;
            }
        }

        return res;
    }

    private static char? GetValueOrDefault(char[][] map, Point pt) =>
        IsInBounds(map, pt.X, pt.Y)
            ? map[pt.Y][pt.X]
            : null;

    private static bool IsInBounds(char[][] map, int x, int y) =>
        x >= 0 && y >= 0 && y < map.Length && x < map[y].Length;

    private static char[][] ParseInput(string input) =>
        input
            .Split("\n")
            .Select(x => x.ToCharArray())
            .ToArray();

    private record struct Point(int X, int Y);
}
