using System.Collections.Immutable;

using AdventOfCode.Lib;

namespace AdventOfCode._2024.Day16;

[ProblemName("Reindeer Maze")]
public class Solution : ISolver
{
    private static readonly Point[] s_directions =
    [
        new(1, 0),
        new(0, 1),
        new(-1, 0),
        new(0, -1)
    ];

    public object PartOne(string input)
    {
        var maze = ParseInput(input);
        var start = FindStart(maze);
        var end = FindEnd(maze, new State(start, 0, 0, [start]));

        return end.Cost;
    }

    public object PartTwo(string input)
    {
        var maze = ParseInput(input);
        var start = FindStart(maze);
        var end = FindEnd(maze, new State(start, 0, 0, [start]));

        return end.Seats;
    }

    private static (int Cost, int Seats) FindEnd(char[][] maze, State start)
    {
        var queue = new PriorityQueue<State, int>();
        queue.Enqueue(start, 0);

        var bestPaths = new List<ImmutableStack<Point>>();
        var bestCost = int.MaxValue;
        var minScores = new Dictionary<State, int>();

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current.Cost > bestCost)
            {
                continue;
            }

            switch (maze[current.Position.Y][current.Position.X])
            {
                case 'E':
                    {
                        if (current.Cost == bestCost)
                        {
                            bestPaths.Add(current.Path);
                        }
                        else if (current.Cost < bestCost)
                        {
                            bestPaths = [current.Path];
                            bestCost = current.Cost;
                        }

                        continue;
                    }
                case '#':
                    continue;
            }

            var dir = s_directions[current.Direction];
            var forward = new Point(current.Position.X + dir.X, current.Position.Y + dir.Y);
            var forwardState = new State(forward, current.Direction, current.Cost + 1, current.Path.Push(forward));
            var turnRightState = current with
            {
                Direction = (current.Direction + 1) % s_directions.Length,
                Cost = current.Cost + 1000
            };
            var turnLeftState = current with
            {
                Direction = (current.Direction - 1 + s_directions.Length) % s_directions.Length,
                Cost = current.Cost + 1000
            };

            EnqueueIfBetter(forwardState, minScores, queue);
            EnqueueIfBetter(turnRightState, minScores, queue);
            EnqueueIfBetter(turnLeftState, minScores, queue);
        }

        var bestSeatCount = bestPaths
            .SelectMany(e => e)
            .Distinct()
            .Count();

        return (bestCost, bestSeatCount);
    }

    private static void EnqueueIfBetter(State state, Dictionary<State, int> minCosts, PriorityQueue<State, int> queue)
    {
        var minCost = minCosts.GetValueOrDefault(state, int.MaxValue);

        if (state.Cost <= minCost)
        {
            minCosts[state] = state.Cost;
            queue.Enqueue(state, state.Cost);
        }
    }

    private static Point FindStart(char[][] maze)
    {
        for (var y = 0; y < maze.Length; y++)
        {
            for (var x = 0; x < maze[y].Length; x++)
            {
                if (maze[y][x] == 'S')
                {
                    return new Point(x, y);
                }
            }
        }

        throw new InvalidOperationException("No start found");
    }

    private static char[][] ParseInput(string input) =>
        input
            .Split("\n")
            .Select(x => x.ToCharArray())
            .ToArray();

    private record struct Point(int X, int Y);

    private readonly record struct State(Point Position, int Direction, int Cost, ImmutableStack<Point> Path)
    {
        public override int GetHashCode() => HashCode.Combine(Position, Direction);

        public bool Equals(State other) => Position.Equals(other.Position) && Direction == other.Direction;
    }
}
