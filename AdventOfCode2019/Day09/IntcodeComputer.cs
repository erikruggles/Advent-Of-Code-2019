using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode2019.Day09
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
        private const int RELATIVE_MODE = 2;

        private long[] _memory;
        private readonly Queue<long> _inputs = new Queue<long>();
        private readonly Queue<long> _outputs = new Queue<long>();
        private long _instructionPointer;
        private long _relativeBase;

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

        public IReadOnlyList<long> ReadAllOutputs()
        {
            var outputs = _outputs.ToList();
            _outputs.Clear();
            return outputs;
        }

        // Runs until the program halts or needs an input that has not been provided yet.
        public IntcodeState Run()
        {
            if (State == IntcodeState.Halted)
                throw new InvalidOperationException("Cannot run a halted IntcodeComputer");

            while (true)
            {
                var instruction = ReadMemory(_instructionPointer);
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
                        _instructionPointer = Read(1, modes) != 0 ? Read(2, modes) : _instructionPointer + 3;
                        break;
                    case 6: //Jump-if-false
                        _instructionPointer = Read(1, modes) == 0 ? Read(2, modes) : _instructionPointer + 3;
                        break;
                    case 7: //Less than
                        Write(3, modes, Read(1, modes) < Read(2, modes) ? 1 : 0);
                        _instructionPointer += 4;
                        break;
                    case 8: //Equals
                        Write(3, modes, Read(1, modes) == Read(2, modes) ? 1 : 0);
                        _instructionPointer += 4;
                        break;
                    case 9: //Relative base offset
                        _relativeBase += Read(1, modes);
                        _instructionPointer += 2;
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
            var value = ReadMemory(_instructionPointer + parameter);
            return modes[parameter - 1] == IMMEDIATE_MODE
                ? value
                : ReadMemory(GetAddress(value, modes[parameter - 1]));
        }

        private void Write(int parameter, int[] modes, long value)
        {
            if (modes[parameter - 1] == IMMEDIATE_MODE)
                throw new InvalidOperationException("Write parameters cannot be in immediate mode");
            WriteMemory(GetAddress(ReadMemory(_instructionPointer + parameter), modes[parameter - 1]), value);
        }

        private long GetAddress(long parameterValue, int mode) => mode switch
        {
            POSITION_MODE => parameterValue,
            RELATIVE_MODE => _relativeBase + parameterValue,
            _ => throw new InvalidOperationException($"Unknown parameter mode {mode}")
        };

        // Memory beyond the program reads as 0 until written.
        private long ReadMemory(long address)
        {
            ValidateAddress(address);
            return address < _memory.Length ? _memory[address] : 0;
        }

        private void WriteMemory(long address, long value)
        {
            ValidateAddress(address);
            if (address >= _memory.Length)
                Array.Resize(ref _memory, (int)Math.Max(address + 1, _memory.Length * 2L));
            _memory[address] = value;
        }

        private static void ValidateAddress(long address)
        {
            if (address < 0)
                throw new InvalidOperationException($"Invalid negative memory address {address}");
            if (address > Array.MaxLength)
                throw new InvalidOperationException($"Memory address {address} is too large");
        }
    }
}
