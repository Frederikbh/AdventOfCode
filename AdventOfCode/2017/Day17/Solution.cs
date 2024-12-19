using AdventOfCode.Lib;

namespace AdventOfCode._2017.Day17;

[ProblemName("Spinlock")]
public class Solution : ISolver
{
    public object PartOne(string input)
    {
        var steps = int.Parse(input);
        var buffer = new List<int> { 0 };
        var pos = 0;

        for (var i = 1; i <= 2017; i++)
        {
            pos = (pos + steps) % buffer.Count + 1;
            buffer.Insert(pos, i);
        }

        return buffer[(pos + 1) % buffer.Count];
    }

    public object PartTwo(string input)
    {
        var steps = int.Parse(input);
        var pos = 0;
        var value = 0;

        for (var i = 1; i <= 50000000; i++)
        {
            pos = (pos + steps) % i + 1;

            if (pos == 1)
            {
                value = i;
            }
        }

        return value;
    }
}
