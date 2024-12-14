using AdventOfCode.Lib;

namespace AdventOfCode._2024.Day13;

[ProblemName("Claw Contraption")]
public class Solution : ISolver
{
    public object PartOne(string input)
    {
        var machines = ParseInput(input);
        var result = machines.Select(MinimumPushes)
            .Sum();

        return result;
    }

    public object PartTwo(string input)
    {
        const long Modifier = 10_000_000_000_000;

        var machines = ParseInput(input, Modifier);
        var result = machines.Select(MinimumPushes)
            .Sum();

        return result;
    }

    private static long MinimumPushes(Machine machine)
    {
        var detI = Determinant(machine.Prize, machine.ButtonB) / Determinant(machine.ButtonA, machine.ButtonB);
        var detJ = Determinant(machine.ButtonA, machine.Prize) / Determinant(machine.ButtonA, machine.ButtonB);

        var xFound = machine.ButtonA.X * detI + machine.ButtonB.X * detJ == machine.Prize.X;
        var yFound = machine.ButtonA.Y * detI + machine.ButtonB.Y * detJ == machine.Prize.Y;

        if (detI >= 0 && detJ >= 0 && xFound && yFound)
        {
            return 3 * detI + detJ;
        }

        return 0;
    }

    private static long Determinant(Point a, Point b) => a.X * b.Y - a.Y * b.X;

    private static List<Machine> ParseInput(string input, long prizeModifier = 0)
    {
        var machines = new List<Machine>();
        var machineDefinitions = input.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);

        foreach (var machineDefinition in machineDefinitions)
        {
            var lines = machineDefinition.Split('\n');

            var buttonA = ParseButton(lines[0]);
            var buttonB = ParseButton(lines[1]);
            var prize = ParsePrize(lines[2], prizeModifier);

            var machine = new Machine(buttonA, buttonB, prize);
            machines.Add(machine);
        }

        return machines;
    }

    private static Point ParseButton(string buttonDefinition)
    {
        var parts = buttonDefinition.Split(['+', ',']);
        var x = long.Parse(parts[1]);
        var y = long.Parse(parts[3]);

        return new Point(x, y);
    }

    private static Point ParsePrize(string prizeDefinition, long prizeModifier = 0)
    {
        var parts = prizeDefinition.Split(['=', ',']);
        var x = long.Parse(parts[1]) + prizeModifier;
        var y = long.Parse(parts[3]) + prizeModifier;

        return new Point(x, y);
    }

    private record struct Point(long X, long Y);

    private record Machine(Point ButtonA, Point ButtonB, Point Prize);

    private record MachineState(Point Position, int Cost);
}
