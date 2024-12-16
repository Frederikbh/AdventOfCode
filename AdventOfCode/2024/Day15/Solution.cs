using AdventOfCode.Lib;

namespace AdventOfCode._2024.Day15;

[ProblemName("Warehouse Woes")]
public class Solution : ISolver
{
    private static readonly Dictionary<char, Point> s_moves = new()
    {
        { '^', new Point(0, -1) },
        { 'v', new Point(0, 1) },
        { '<', new Point(-1, 0) },
        { '>', new Point(1, 0) }
    };

    public object PartOne(string input)
    {
        return Solve(input, false);
    }

    public object PartTwo(string input)
    {
        return Solve(input, true);
    }

    private static object Solve(string input, bool expandMap)
    {
        var (map, moves) = ParseInput(input, expandMap);
        var position = FindStart(map);

        foreach (var move in moves)
        {
            position = Move(map, position, s_moves[move]);
        }

        return Score(map);
    }

    private static long Score(char[][] map)
    {
        var score = 0L;

        for (var y = 0; y < map.Length; y++)
        {
            for (var x = 0; x < map[y].Length; x++)
            {
                if (map[y][x] is 'O' or '[')
                {
                    score += y * 100 + x;
                }
            }
        }

        return score;
    }

    private static Point Move(char[][] map, Point position, Point move)
    {
        var newPosition = new Point(position.X + move.X, position.Y + move.Y);
        var newTile = map[newPosition.Y][newPosition.X];

        return newTile switch
        {
            '.' => MoveToEmpty(map, position, newPosition),
            '#' => position,
            'O' => MoveSingleRock(map, position, move, newPosition),
            '[' or ']' => MoveDoubleRock(map, newPosition, move),
            _ => newPosition
        };
    }

    private static Point MoveToEmpty(char[][] map, Point position, Point newPosition)
    {
        map[position.Y][position.X] = '.';
        map[newPosition.Y][newPosition.X] = '@';
        return newPosition;
    }

    private static Point MoveDoubleRock(char[][] map, Point position, Point move)
    {
        var cluster = new Stack<Point>();
        var visited = new HashSet<Point>();
        var queue = new Queue<Point>();
        queue.Enqueue(position);
        var canMove = true;

        while (queue.Count > 0)
        {
            var pos = queue.Dequeue();

            if (!visited.Add(pos))
            {
                continue;
            }

            var tile = map[pos.Y][pos.X];

            if (tile == '#')
            {
                canMove = false;
                break;
            }

            if (!IsRock(tile))
            {
                continue;
            }

            cluster.Push(pos);
            var partnerPos = tile == '['
                ? new Point(pos.X + 1, pos.Y)
                : new Point(pos.X - 1, pos.Y);
            queue.Enqueue(partnerPos);

            var next = new Point(pos.X + move.X, pos.Y + move.Y);
            queue.Enqueue(next);
        }

        if (!canMove)
        {
            return new Point(position.X - move.X, position.Y - move.Y);
        }

        while (cluster.Count > 0)
        {
            var pos = cluster.Pop();
            map[pos.Y + move.Y][pos.X + move.X] = map[pos.Y][pos.X];
            map[pos.Y][pos.X] = '.';
        }

        map[position.Y][position.X] = '@';
        map[position.Y - move.Y][position.X - move.X] = '.';

        return position;
    }

    private static bool IsRock(char tile) => tile is 'O' or '[' or ']';

    private static Point MoveSingleRock(char[][] map, Point position, Point move, Point newPosition)
    {
        var boxEnd = newPosition;

        while (map[boxEnd.Y][boxEnd.X] == 'O')
        {
            boxEnd = new Point(boxEnd.X + move.X, boxEnd.Y + move.Y);
        }

        if (map[boxEnd.Y][boxEnd.X] == '#')
        {
            return position;
        }

        while (boxEnd != newPosition)
        {
            map[boxEnd.Y][boxEnd.X] = 'O';
            boxEnd = new Point(boxEnd.X - move.X, boxEnd.Y - move.Y);
        }

        map[position.Y][position.X] = '.';
        map[newPosition.Y][newPosition.X] = '@';

        return newPosition;
    }

    private static Point FindStart(char[][] map)
    {
        for (var y = 0; y < map.Length; y++)
        {
            for (var x = 0; x < map[y].Length; x++)
            {
                if (map[y][x] == '@')
                {
                    return new Point(x, y);
                }
            }
        }

        throw new Exception("Start not found");
    }

    private static (char[][], char[]) ParseInput(string input, bool expandMap = false)
    {
        var parts = input.Split("\n\n");

        var mapStr = parts[0];

        if (expandMap)
        {
            mapStr = mapStr.Replace("#", "##")
                .Replace("O", "[]")
                .Replace(".", "..")
                .Replace("@", "@.");
        }

        var map = mapStr
            .Split('\n')
            .Select(x => x.ToCharArray())
            .ToArray();

        var moveLines = parts[1]
            .Split('\n');
        var moves = moveLines.SelectMany(e => e.ToCharArray())
            .ToArray();

        return (map, moves);
    }

    private record struct Point(int X, int Y);
}
