using System.Text;

using AdventOfCode.Lib;

namespace AdventOfCode._2024.Day14;

[ProblemName("Restroom Redoubt")]
public class Solution : ISolver
{
    private const int Width = 101;
    private const int Height = 103;

    public object PartOne(string input)
    {
        var robots = ParseInput(input);

        for (var i = 0; i < 100; i++)
        {
            robots = robots.Select(MoveRobot)
                .ToList();
        }

        var safetyScore = SafetyScore(robots, Width, Height);

        return safetyScore;
    }

    public object PartTwo(string input)
    {
        var robots = ParseInput(input);
        var i = 0;
        var patternDetected = false;

        while (!patternDetected)
        {
            robots = robots.Select(MoveRobot)
                .ToList();
            i++;
            patternDetected = IsPatternDetected(robots);
        }

        return i;
    }

    private static bool IsPatternDetected(List<Robot> robots)
    {
        var plot = Plot(robots);

        if (!plot.Contains("#################"))
        {
            return false;
        }

        Console.WriteLine(plot);

        return true;
    }

    private static int SafetyScore(List<Robot> robots, int width, int height) =>
        robots
            .Select(e => e.Position)
            .CountBy(e => new Point(Math.Sign(e.X - width / 2), Math.Sign(e.Y - height / 2)))
            .Where(e => e.Key.X != 0 && e.Key.Y != 0)
            .Select(e => e.Value)
            .Where(e => e > 0)
            .Aggregate(1, (a, b) => a * b);

    private static Robot MoveRobot(Robot robot)
    {
        var x = (robot.Position.X + robot.Velocity.X + Width) % Width;
        var y = (robot.Position.Y + robot.Velocity.Y + Height) % Height;

        return robot with { Position = new Point(x, y) };
    }

    private static string Plot(List<Robot> robots)
    {
        var map = new char[Height, Width];

        foreach (var robot in robots)
        {
            map[robot.Position.Y, robot.Position.X] = '#';
        }

        var sb = new StringBuilder();

        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                sb.Append(
                    map[y, x] == '#'
                        ? '#'
                        : '.');
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static List<Robot> ParseInput(string input)
    {
        var robotLines = input.Split("\n", StringSplitOptions.RemoveEmptyEntries);

        var robots = new List<Robot>();

        foreach (var robotLine in robotLines)
        {
            var lineParts = robotLine.Split(['=', ',', ' '], StringSplitOptions.RemoveEmptyEntries);

            var x = int.Parse(lineParts[1]);
            var y = int.Parse(lineParts[2]);

            var velX = int.Parse(lineParts[4]);
            var velY = int.Parse(lineParts[5]);

            robots.Add(new Robot(new Point(x, y), new Point(velX, velY)));
        }

        return robots;
    }

    private record struct Point(int X, int Y);

    private record Robot(Point Position, Point Velocity);
}
