using AdventOfCode.Lib;

namespace AdventOfCode._2018.Day01;

[ProblemName("Chronal Calibration")]
public class Solution : ISolver
{
    public object PartOne(string input) =>
        ParseInput(input)
            .Sum();

    public object PartTwo(string input)
    {
        var inputList = ParseInput(input)
            .ToList();
        var frequency = 0;
        var frequencies = new HashSet<int> { frequency };
        var index = 0;

        while (true)
        {
            frequency += inputList[index];

            if (!frequencies.Add(frequency))
            {
                return frequency;
            }

            index = (index + 1) % inputList.Count;
        }
    }

    private static IEnumerable<int> ParseInput(string input) =>
        input.Split('\n')
            .Select(int.Parse);
}
