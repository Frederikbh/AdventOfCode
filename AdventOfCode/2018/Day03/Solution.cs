using System.Text.RegularExpressions;

using AdventOfCode.Lib;

namespace AdventOfCode._2018.Day03;

[ProblemName("No Matter How You Slice It")]
public partial class Solution : ISolver
{
    public object PartOne(string input)
    {
        var instructions = ParseInput(input);
        var grid = CreateGrid(1000, 1000);
        FillGrid(grid, instructions);

        return CountOverlaps(grid);
    }

    public object PartTwo(string input)
    {
        var instructions = ParseInput(input);
        var grid = CreateGrid(1000, 1000);
        FillGrid(grid, instructions);

        return FindNonOverlappingId(grid, instructions);
    }

    private static int FindNonOverlappingId(int[][] grid, IEnumerable<Instruction> instructions)
    {
        foreach (var instruction in instructions)
        {
            var overlaps = false;

            for (var x = instruction.Corner.X; x < instruction.Corner.X + instruction.Width; x++)
            {
                for (var y = instruction.Corner.Y; y < instruction.Corner.Y + instruction.Height; y++)
                {
                    if (grid[x][y] > 1)
                    {
                        overlaps = true;
                        break;
                    }
                }

                if (overlaps)
                {
                    break;
                }
            }

            if (!overlaps)
            {
                return instruction.Id;
            }
        }

        return -1;
    }

    private static int[][] CreateGrid(int width, int height)
    {
        var grid = new int[width][];

        for (var i = 0; i < width; i++)
        {
            grid[i] = new int[height];
        }

        return grid;
    }

    private static void FillGrid(int[][] grid, IEnumerable<Instruction> instructions)
    {
        foreach (var instruction in instructions)
        {
            for (var x = instruction.Corner.X; x < instruction.Corner.X + instruction.Width; x++)
            {
                for (var y = instruction.Corner.Y; y < instruction.Corner.Y + instruction.Height; y++)
                {
                    grid[x][y]++;
                }
            }
        }
    }

    private static int CountOverlaps(int[][] grid) =>
        grid.Sum(row => row.Count(cell => cell > 1));

    private static List<Instruction> ParseInput(string input)
    {
        var instructions = new List<Instruction>();

        var inputRegex = InputRegex();
        foreach (var line in input.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var match = inputRegex.Match(line);

            if (match.Success)
            {
                var id = int.Parse(match.Groups[1].Value);
                var corner = new Point(int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
                var width = int.Parse(match.Groups[4].Value);
                var height = int.Parse(match.Groups[5].Value);
                instructions.Add(new Instruction(id, corner, width, height));
            }
        }

        return instructions;
    }

    private record struct Point(int X, int Y);

    private record Instruction(int Id, Point Corner, int Width, int Height);

    [GeneratedRegex(@"#(\d+) @ (\d+),(\d+): (\d+)x(\d+)")]
    private static partial Regex InputRegex();
}
