using AdventOfCode.Lib;

namespace AdventOfCode._2024.Day10;

[ProblemName("Hoof It")]
public class Solution : ISolver
{
    private static readonly (int, int)[] s_directions =
    {
        (0, 1),
        (0, -1),
        (1, 0),
        (-1, 0)
    };

    public object PartOne(string input)
    {
        var map = ParseMap(input);

        return TotalScore(map, true);
    }

    public object PartTwo(string input)
    {
        var map = ParseMap(input);

        return TotalScore(map, false);
    }

    private static int TotalScore(int[][] map, bool countUniqueRoutes)
    {
        var totalScore = 0;

        for (var y = 0; y < map.Length; y++)
        {
            for (var x = 0; x < map[y].Length; x++)
            {
                if (map[y][x] == 0)
                {
                    totalScore += CalculateTrailheadScore(map, x, y, countUniqueRoutes);
                }
            }
        }

        return totalScore;
    }

    private static int CalculateTrailheadScore(int[][] map, int x, int y, bool countUniqueRoutes)
    {
        var trailTops = new HashSet<(int x, int y)>();
        var score = 0;
        var visited = new HashSet<(int x, int y)>();
        var queue = new Queue<(int x, int y)>();
        queue.Enqueue((x, y));

        while (queue.Count > 0)
        {
            var (currentX, currentY) = queue.Dequeue();

            if (countUniqueRoutes && !visited.Add((currentX, currentY)))
            {
                continue;
            }

            var currentHeight = map[currentY][currentX];

            if (currentHeight == 9)
            {
                trailTops.Add((currentX, currentY));
                score++;

                continue;
            }

            foreach (var (dx, dy) in s_directions)
            {
                var nextX = currentX + dx;
                var nextY = currentY + dy;

                if (IsValidCoordinate(nextX, nextY, map.Length, map[0].Length) &&
                    map[nextY][nextX] == currentHeight + 1)
                {
                    queue.Enqueue((nextX, nextY));
                }
            }
        }

        return countUniqueRoutes
            ? trailTops.Count
            : score;
    }

    private static bool IsValidCoordinate(int x, int y, int height, int width) =>
        x >= 0 && x < width && y >= 0 && y < height;

    private static int[][] ParseMap(string input) =>
        input.Split('\n')
            .Select(
                line => line.ToCharArray()
                    .Select(c => c - '0')
                    .ToArray())
            .ToArray();
}
