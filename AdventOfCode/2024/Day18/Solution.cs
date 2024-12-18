using AdventOfCode.Lib;

namespace AdventOfCode._2024.Day18;

[ProblemName("RAM Run")]
public class Solution : ISolver
{
    public object PartOne(string input)
    {
        var bytes = ParseInput(input);
        var grid = MakeGrid(71, 71, bytes.Take(1024));
        var start = new Point(0, 0);
        var path = FindShortestPath(grid, start, new Point(70, 70));

        return path;
    }

    public object PartTwo(string input)
    {
        var bytes = ParseInput(input);
        var start = new Point(0, 0);
        var end = new Point(70, 70);

        var low = 0;
        var high = bytes.Count - 1;

        while (low < high)
        {
            var mid = (low + high) / 2;

            var testGrid = MakeGrid(71, 71, bytes.Take(mid + 1));
            var path = FindShortestPath(testGrid, start, end);

            if (path == -1)
            {
                high = mid;
            }
            else
            {
                low = mid + 1;
            }
        }

        return bytes[low]
            .ToString();
    }

    private static int FindShortestPath(char[][] grid, Point start, Point end)
    {
        var queue = new Queue<(Point, int)>();
        queue.Enqueue((start, 0));
        var visited = new HashSet<Point>();

        while (queue.Count > 0)
        {
            var (current, steps) = queue.Dequeue();

            if (current == end)
            {
                return steps;
            }

            foreach (var neighbour in GetNeighbours(grid, current))
            {
                if (!visited.Add(neighbour))
                {
                    continue;
                }

                queue.Enqueue((neighbour, steps + 1));
            }
        }

        return -1;
    }

    private static IEnumerable<Point> GetNeighbours(char[][] grid, Point current)
    {
        var (x, y) = current;

        if (x > 0 && grid[y][x - 1] == '.')
        {
            yield return new Point(x - 1, y);
        }

        if (x < grid[0].Length - 1 && grid[y][x + 1] == '.')
        {
            yield return new Point(x + 1, y);
        }

        if (y > 0 && grid[y - 1][x] == '.')
        {
            yield return new Point(x, y - 1);
        }

        if (y < grid.Length - 1 && grid[y + 1][x] == '.')
        {
            yield return new Point(x, y + 1);
        }
    }

    private static char[][] MakeGrid(int width, int height, IEnumerable<Point> bytes)
    {
        var grid = new char[height][];

        for (var y = 0; y < height; y++)
        {
            grid[y] = new char[width];

            for (var x = 0; x < width; x++)
            {
                grid[y][x] = '.';
            }
        }

        foreach (var (x, y) in bytes)
        {
            grid[y][x] = '#';
        }

        return grid;
    }

    private static List<Point> ParseInput(string input) =>
        input.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Split(','))
            .Select(coords => new Point(int.Parse(coords[0]), int.Parse(coords[1])))
            .ToList();

    private readonly record struct Point(int X, int Y)
    {
        public override string ToString() => $"{X},{Y}";
    }
}
