using AdventOfCode.Lib;        

namespace AdventOfCode._2024.Day17;

[ProblemName("Chronospatial Computer")]
public class Solution : ISolver 
{

    public object PartOne(string input) 
    {
        var computer = ParseInput(input);
        var result = computer.Execute();
        return string.Join(',', result);
    }

    public object PartTwo(string input) 
    {
        var computer = ParseInput(input);
        var output = computer.FindSelfCopying();
        return output;
    }

    private static Computer ParseInput(string input)
    {
        var lines = input.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        var a = int.Parse(lines[0].Split(" ").Last());
        var b = int.Parse(lines[1].Split(" ").Last());
        var c = int.Parse(lines[2].Split(" ").Last());

        var program = lines[3]
            .Split(" ")[1]
            .Split(',')
            .Select(long.Parse)
            .ToArray();

        return new Computer([a, b, c], program);
    }

    private class Computer
    {
        private long[] _registers;
        private readonly long[] _program;
        private long _pointer;
        private List<long> _outputs;

        public Computer(long[] registers, long[] program)
        {
            _registers = registers;
            _program = program;
            _outputs = [];
        }

        public long FindSelfCopying()
        {
            var possibleValues = Dfs(0, 0);
            return possibleValues.Min();
        }

        private List<long> Dfs(long value, int depth)
        {
            var result = new List<long>();

            if (depth > _program.Length)
            {
                return result;
            }

            var temp = value << 3;

            for (var i = 0; i < 8; i++)
            {
                SetComputerMemory(temp + i);
                var tempResult = Execute();

                if (tempResult.SequenceEqual(_program.TakeLast(depth + 1)))
                {
                    if (depth + 1 == _program.Length)
                    {
                        result.Add(temp + i);
                    }
                    result.AddRange(Dfs(temp + i, depth + 1));
                }
            }

            return result;
        }

        private void SetComputerMemory(long a)
        {
            _registers = [a, 0, 0];
            _pointer = 0;
            _outputs = [];
        }

        public List<long> Execute()
        {
            while (_pointer < _program.Length)
            {
                var opcode = _program[_pointer];
                var operand = _program[_pointer + 1];

                _pointer = opcode switch
                {
                    0 => Adv(Combo(operand)),
                    1 => Bxl(operand),
                    2 => Bst(Combo(operand)),
                    3 => Jnz(operand),
                    4 => Bxc(operand),
                    5 => Out(Combo(operand)),
                    6 => Bdv(Combo(operand)),
                    7 => Cdv(Combo(operand)),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            return _outputs;
        }

        private long Adv(long operand)
        {
            var result = _registers[0] / Math.Pow(2, operand);
            _registers[0] = (long)Math.Floor(result);
            return _pointer + 2;
        }

        private long Bxl(long operand)
        {
            var result = _registers[1] ^ operand;
            _registers[1] = result;
            return _pointer + 2;
        }

        private long Bst(long operand)
        {
            _registers[1] = operand % 8;
            return _pointer + 2;
        }

        private long Jnz(long operand) =>
            _registers[0] == 0
                ? _pointer + 2
                : operand;

        private long Bxc(long _)
        {
            var result = _registers[1] ^ _registers[2];
            _registers[1] = result;
            return _pointer + 2;
        }

        private long Out(long operand)
        {
            _outputs.Add(operand % 8);
            return _pointer + 2;
        }

        private long Bdv(long operand)
        {
            var result = _registers[0] / Math.Pow(2, operand);
            _registers[1] = (long)Math.Floor(result);
            return _pointer + 2;
        }

        private long Cdv(long operand)
        {
            var result = _registers[0] / Math.Pow(2, operand);
            _registers[2] = (long)Math.Floor(result);
            return _pointer + 2;
        }

        private long Combo(long value) =>
            value < 4
                ? value
                : _registers[value - 4];
    }
}
