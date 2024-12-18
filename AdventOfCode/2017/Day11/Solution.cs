using AdventOfCode.Lib;

// ReSharper disable InconsistentNaming

namespace AdventOfCode._2017.Day11;

[ProblemName("Hex Ed")]
public class Solution : ISolver
{
    private static readonly Dictionary<Direction, Coordinate> s_moves = new()
    {
        { Direction.N, new Coordinate(0, -1, +1) },
        { Direction.NE, new Coordinate(1, -1, 0) },
        { Direction.SE, new Coordinate(1, 0, -1) },
        { Direction.S, new Coordinate(0, 1, -1) },
        { Direction.SW, new Coordinate(-1, 1, 0) },
        { Direction.NW, new Coordinate(-1, 0, 1) }
    };

    public object PartOne(string input)
    {
        var directions = ParseInput(input);
        var position = new Coordinate(0, 0, 0);
        var childPosition = Travel(position, directions);
        var steps = CalculateSteps(position, childPosition);
        return steps;
    }

    public object PartTwo(string input)
    {
        var directions = ParseInput(input);
        var position = new Coordinate(0, 0, 0);

        var maxDistance = 0;

        foreach (var direction in directions)
        {
            position = Travel(position, [direction]);
            var steps = CalculateSteps(new Coordinate(0, 0, 0), position);
            maxDistance = Math.Max(maxDistance, steps);
        }

        return maxDistance;
    }

    private static Coordinate Travel(Coordinate position, List<Direction> directions) =>
        directions.Aggregate(
            position,
            (current, direction) => new Coordinate(
                current.Q + s_moves[direction].Q,
                current.R + s_moves[direction].R,
                current.S + s_moves[direction].S));

    private static int CalculateSteps(Coordinate start, Coordinate end) =>
        (Math.Abs(start.Q - end.Q) + Math.Abs(start.R - end.R) + Math.Abs(start.S - end.S)) / 2;

    private static List<Direction> ParseInput(string input) =>
        input.Split(',')
            .Select(e => Enum.Parse<Direction>(e, true))
            .ToList();

    private record struct Coordinate(int Q, int R, int S);

    private enum Direction
    {
        N,
        NE,
        SE,
        S,
        SW,
        NW
    }
}
