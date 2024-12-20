using System.Collections.Immutable;

using AdventOfCode.Lib;

namespace AdventOfCode._2024.Day20;

[ProblemName("Race Condition")]
public class Solution : ISolver
{
    public object PartOne(string input) => Solve(input, 2);

    public object PartTwo(string input) => Solve(input, 20);

    private static int Solve(string input, int maxCheatSteps)
    {
        var track = ParseInput(input);

        var path = FindTrack(track);
        var cheats = CountCheats(path, maxCheatSteps);

        return cheats;
    }

    private static int CountCheats(List<Point> path, int maxCheatSteps)
    {
        var cheatSavings = new List<int>();

        for (var i = 0; i < path.Count - 3; i++)
        {
            for (var j = i + 3; j < path.Count; j++)
            {
                var distance = Math.Abs(path[j].X - path[i].X) + Math.Abs(path[j].Y - path[i].Y);

                if (distance <= maxCheatSteps)
                {
                    cheatSavings.Add(j - i - distance);
                }
            }
        }

        return cheatSavings.Count(x => x > 99);
    }

    private static List<Point> FindTrack(char[][] track)
    {
        var directions = new[]
        {
            new Point(0, 1),
            new Point(0, -1),
            new Point(1, 0),
            new Point(-1, 0)
        };

        var start = FindStart(track);
        var end = FindEnd(track);

        var visited = new HashSet<Point>();
        var queue = new PriorityQueue<State, int>();
        queue.Enqueue(new State(start, 0, [start]), 0);

        while (queue.Count > 0)
        {
            var (pos, steps, path) = queue.Dequeue();

            if (pos == end)
            {
                return path.Reverse()
                    .ToList();
            }

            foreach (var dir in directions)
            {
                var newPos = pos + dir;

                if (newPos.X < 0 || newPos.Y < 0 || newPos.X >= track[0].Length || newPos.Y >= track.Length)
                {
                    continue;
                }

                if (visited.Contains(newPos))
                {
                    continue;
                }

                if (track[newPos.Y][newPos.X] == '#')
                {
                    continue;
                }

                visited.Add(newPos);
                queue.Enqueue(new State(newPos, steps + 1, path.Push(newPos)), steps + 1);
            }
        }

        throw new Exception("No path found");
    }

    private static Point FindStart(char[][] track) => FindPos(track, 'S');

    private static Point FindEnd(char[][] track) => FindPos(track, 'E');

    private static Point FindPos(char[][] track, char findChar)
    {
        for (var y = 0; y < track.Length; y++)
        {
            for (var x = 0; x < track[y].Length; x++)
            {
                if (track[y][x] == findChar)
                {
                    return new Point(x, y);
                }
            }
        }

        throw new Exception("No end found");
    }

    private static char[][] ParseInput(string input) =>
        input.Split('\n')
            .Select(x => x.ToCharArray())
            .ToArray();

    private record struct Point(int X, int Y)
    {
        public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y);
    }

    private readonly record struct State(Point Position, int Cost, ImmutableStack<Point> Path)
    {
        public override int GetHashCode() => HashCode.Combine(Position);

        public bool Equals(State other) => Position.Equals(other.Position);
    }
}
