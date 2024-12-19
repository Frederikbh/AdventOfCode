using AdventOfCode.Lib;

namespace AdventOfCode._2017.Day18;

[ProblemName("Duet")]
public class Solution : ISolver
{
    public object PartOne(string input) =>
        new Machine1()
            .Execute(input)
            .First(received => received != null)!
            .Value;

    public object PartTwo(string input)
    {
        var p0Input = new Queue<long>();
        var p1Input = new Queue<long>();

        var machine1 = new Machine2(0, p0Input, p1Input);
        var machine2 = new Machine2(1, p1Input, p0Input);

        return machine1.Execute(input)
            .Zip(machine2.Execute(input))
            .First(x => !x.First.IsRunning && !x.Second.IsRunning)
            .Second.ValueSent;
    }

    private abstract class Machine<TState>
    {
        private readonly Dictionary<string, long> _registers = new();

        protected bool Running;
        protected int Ip;

        protected long this[string reg]
        {
            get =>
                long.TryParse(reg, out var n)
                    ? n
                    : _registers.GetValueOrDefault(reg, 0);
            set => _registers[reg] = value;
        }

        public IEnumerable<TState> Execute(string input)
        {
            var prog = input.Split('\n')
                .ToArray();

            while (Ip >= 0 && Ip < prog.Length)
            {
                Running = true;
                var line = prog[Ip];
                var parts = line.Split(' ');

                Ip = parts[0] switch
                {
                    "snd" => Snd(parts[1]),
                    "rcv" => Rcv(parts[1]),
                    "set" => Set(parts[1], parts[2]),
                    "add" => Add(parts[1], parts[2]),
                    "mul" => Mul(parts[1], parts[2]),
                    "mod" => Mod(parts[1], parts[2]),
                    "jgz" => Jgz(parts[1], parts[2]),
                    _ => throw new Exception("Cannot parse " + line)
                };

                yield return State();
            }

            Running = false;

            yield return State();
        }

        protected abstract TState State();

        protected abstract int Snd(string reg);

        protected abstract int Rcv(string reg);

        private int Set(string reg0, string reg1)
        {
            this[reg0] = this[reg1];

            return Ip + 1;
        }

        private int Add(string reg0, string reg1)
        {
            this[reg0] += this[reg1];

            return Ip + 1;
        }

        private int Mul(string reg0, string reg1)
        {
            this[reg0] *= this[reg1];

            return Ip + 1;
        }

        private int Mod(string reg0, string reg1)
        {
            this[reg0] %= this[reg1];

            return Ip + 1;
        }

        private int Jgz(string reg0, string reg1) =>
            Ip + (this[reg0] > 0
                ? (int)this[reg1]
                : 1);
    }

    private class Machine1 : Machine<long?>
    {
        private long? _sent;
        private long? _received;

        protected override long? State() => _received;

        protected override int Snd(string reg)
        {
            _sent = this[reg];

            return Ip + 1;
        }

        protected override int Rcv(string reg)
        {
            if (this[reg] != 0)
            {
                _received = _sent;
            }

            return Ip + 1;
        }
    }

    private class Machine2 : Machine<(bool IsRunning, int ValueSent)>
    {
        private int _valueSent;
        private readonly Queue<long> _qIn;
        private readonly Queue<long> _qOut;

        public Machine2(long p, Queue<long> qIn, Queue<long> qOut)
        {
            this["p"] = p;
            _qIn = qIn;
            _qOut = qOut;
        }

        protected override (bool IsRunning, int ValueSent) State() => (IsRunning: Running, ValueSent: _valueSent);

        protected override int Snd(string reg)
        {
            _qOut.Enqueue(this[reg]);
            _valueSent++;

            return Ip + 1;
        }

        protected override int Rcv(string reg)
        {
            if (_qIn.Count != 0)
            {
                this[reg] = _qIn.Dequeue();

                return Ip + 1;
            }
            else
            {
                Running = false;

                return Ip;
            }
        }
    }
}
