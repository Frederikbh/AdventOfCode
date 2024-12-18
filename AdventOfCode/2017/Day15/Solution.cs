using AdventOfCode.Lib;

namespace AdventOfCode._2017.Day15;

[ProblemName("Dueling Generators")]
public class Solution : ISolver
{
    private const int FactorA = 16807;
    private const int FactorB = 48271;
    private const int Mod = int.MaxValue;
    private const int Mask = (1 << 16) - 1;

    public object PartOne(string input)
    {
        var (a, b) = ParseInput(input);
        var count = 0;

        for (var i = 0; i < 40_000_000; i++)
        {
            a = (int)((long)a * FactorA % Mod);
            b = (int)((long)b * FactorB % Mod);

            if ((a & Mask) == (b & Mask))
            {
                count++;
            }
        }

        return count;
    }

    public object PartTwo(string input)
    {
        var (a, b) = ParseInput(input);
        var count = 0;

        for (var i = 0; i < 5_000_000; i++)
        {
            a = NextValidA(a);
            b = NextValidB(b);

            if ((a & Mask) == (b & Mask))
            {
                count++;
            }
        }

        return count;
    }

    private static int NextValidA(int val)
    {
        do
        {
            val = (int)((long)val * FactorA % Mod);
        } while ((val & 3) != 0);

        return val;
    }

    private static int NextValidB(int val)
    {
        do
        {
            val = (int)((long)val * FactorB % Mod);
        } while ((val & 7) != 0);

        return val;
    }

    private static (int A, int B) ParseInput(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var a = int.Parse(
            lines[0]
                .Split(' ')
                .Last());
        var b = int.Parse(
            lines[1]
                .Split(' ')
                .Last());

        return (a, b);
    }
}
