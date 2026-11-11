using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode2019.Day07
{
    public enum IntcodeState
    {
        NotStarted,
        AwaitingInput,
        Halted
    }

    public class IntcodeComputer
    {
        private const int POSITION_MODE = 0;
        private const int IMMEDIATE_MODE = 1;

        private readonly long[] _memory;
        private readonly Queue<long> _inputs = new Queue<long>();
        private readonly Queue<long> _outputs = new Queue<long>();
        private int _instructionPointer;

        public IntcodeState State { get; private set; } = IntcodeState.NotStarted;

        public IntcodeComputer(long[] program)
        {
            // Each computer gets its own copy so memory is never shared between instances.
            _memory = (long[])program.Clone();
        }

        public static async Task<long[]> ParseImportFile(string path)
        {
            var text = await File.ReadAllTextAsync(path);
            return text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(long.Parse)
                .ToArray();
        }

        public void AddInput(long value) => _inputs.Enqueue(value);

        public bool HasOutput => _outputs.Count > 0;

        public long ReadOutput() => _outputs.Dequeue();

        // Runs until the program halts or needs an input that has not been provided yet.
        public IntcodeState Run()
        {
            if (State == IntcodeState.Halted)
                throw new InvalidOperationException("Cannot run a halted IntcodeComputer");

            while (true)
            {
                var instruction = _memory[_instructionPointer];
                var opCode = (int)(instruction % 100);
                var modes = new[]
                {
                    (int)(instruction / 100 % 10),
                    (int)(instruction / 1000 % 10),
                    (int)(instruction / 10000 % 10)
                };

                switch (opCode)
                {
                    case 1: //Add
                        Write(3, modes, Read(1, modes) + Read(2, modes));
                        _instructionPointer += 4;
                        break;
                    case 2: //Multiply
                        Write(3, modes, Read(1, modes) * Read(2, modes));
                        _instructionPointer += 4;
                        break;
                    case 3: //Input
                        if (_inputs.Count == 0)
                            return State = IntcodeState.AwaitingInput;
                        Write(1, modes, _inputs.Dequeue());
                        _instructionPointer += 2;
                        break;
                    case 4: //Output
                        _outputs.Enqueue(Read(1, modes));
                        _instructionPointer += 2;
                        break;
                    case 5: //Jump-if-true
                        _instructionPointer = Read(1, modes) != 0 ? (int)Read(2, modes) : _instructionPointer + 3;
                        break;
                    case 6: //Jump-if-false
                        _instructionPointer = Read(1, modes) == 0 ? (int)Read(2, modes) : _instructionPointer + 3;
                        break;
                    case 7: //Less than
                        Write(3, modes, Read(1, modes) < Read(2, modes) ? 1 : 0);
                        _instructionPointer += 4;
                        break;
                    case 8: //Equals
                        Write(3, modes, Read(1, modes) == Read(2, modes) ? 1 : 0);
                        _instructionPointer += 4;
                        break;
                    case 99: //Halt
                        return State = IntcodeState.Halted;
                    default:
                        throw new InvalidOperationException(
                            $"Unknown opcode {instruction} at position {_instructionPointer}");
                }
            }
        }

        private long Read(int parameter, int[] modes)
        {
            var value = _memory[_instructionPointer + parameter];
            return modes[parameter - 1] switch
            {
                POSITION_MODE => _memory[value],
                IMMEDIATE_MODE => value,
                _ => throw new InvalidOperationException($"Unknown parameter mode {modes[parameter - 1]}")
            };
        }

        private void Write(int parameter, int[] modes, long value)
        {
            if (modes[parameter - 1] != POSITION_MODE)
                throw new InvalidOperationException("Write parameters must be in position mode");
            _memory[_memory[_instructionPointer + parameter]] = value;
        }
    }
}
