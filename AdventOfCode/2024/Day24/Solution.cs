using AdventOfCode.Lib;

namespace AdventOfCode._2024.Day24;

[ProblemName("Crossed Wires")]
public class Solution : ISolver
{
    public object PartOne(string input)
    {
        var (inputs, circuit) = ParseInput(input);

        var result = 0L;
        var zWires = circuit.Keys.Where(w => w.StartsWith('z'))
            .OrderByDescending(e => e)
            .ToArray();

        foreach (var wire in zWires)
        {
            result = result * 2 + GetValue(wire, circuit, inputs);
        }

        return result;
    }

    public object PartTwo(string input)
    {
        var (_, circuit) = ParseInput(input);

        return string.Join(
            ",",
            Fix(circuit)
                .OrderBy(e => e));
    }

    private static int GetValue(string wire, Dictionary<string, Gate> circuit, Dictionary<string, int> inputs)
    {
        if (inputs.TryGetValue(wire, out var value))
        {
            return value;
        }

        return circuit[wire] switch
        {
            (var first, "AND", var second) => GetValue(first, circuit, inputs) & GetValue(second, circuit, inputs),
            (var first, "OR", var second) => GetValue(first, circuit, inputs) | GetValue(second, circuit, inputs),
            (var first, "XOR", var second) => GetValue(first, circuit, inputs) ^ GetValue(second, circuit, inputs),
            _ => throw new Exception()
        };
    }

    private static IEnumerable<string> Fix(Dictionary<string, Gate> circuit)
    {
        var cin = Output(circuit, "x00", "AND", "y00");

        for (var i = 1; i < 45; i++)
        {
            var x = $"x{i:D2}";
            var y = $"y{i:D2}";
            var z = $"z{i:D2}";

            var xor1 = Output(circuit, x, "XOR", y);
            var and1 = Output(circuit, x, "AND", y);
            var xor2 = Output(circuit, cin, "XOR", xor1);
            var and2 = Output(circuit, cin, "AND", xor1);

            if (xor2 == null && and2 == null)
            {
                return SwapAndFix(circuit, xor1, and1);
            }

            var carry = Output(circuit, and1, "OR", and2);

            if (xor2 != z)
            {
                return SwapAndFix(circuit, z, xor2);
            }
            else
            {
                cin = carry;
            }
        }

        return [];
    }

    private static IEnumerable<string> SwapAndFix(Dictionary<string, Gate> circuit, string? out1, string? out2)
    {
        (circuit[out1], circuit[out2]) = (circuit[out2], circuit[out1]);

        return Fix(circuit)
            .Concat([out1, out2]);
    }

    private static string? Output(Dictionary<string, Gate> circuit, string? x, string kind, string? y) =>
        circuit.SingleOrDefault(
                pair =>
                    (pair.Value.First == x && pair.Value.Kind == kind && pair.Value.Second == y) ||
                    (pair.Value.First == y && pair.Value.Kind == kind && pair.Value.Second == x)
            )
            .Key;

    private static (Dictionary<string, int> Inputs, Dictionary<string, Gate> Circuit) ParseInput(string input)
    {
        var inputs = new Dictionary<string, int>();
        var circuit = new Dictionary<string, Gate>();

        var split = input.Split("\n\n");
        var inputLines = split[0]
            .Split("\n");
        var circuitLines = split[1]
            .Split("\n");

        foreach (var inputLine in inputLines)
        {
            var parts = inputLine.Split(": ");
            inputs[parts[0]] = int.Parse(parts[1]);
        }

        foreach (var circuitLine in circuitLines)
        {
            var parts = circuitLine.Split(" -> ");
            var gateParts = parts[0]
                .Split(" ");
            circuit[parts[1]] = new Gate(
                gateParts[0],
                gateParts[1],
                gateParts.Length == 3
                    ? gateParts[2]
                    : "");
        }

        return (inputs, circuit);
    }

    private record struct Gate(string First, string Kind, string Second);
}
