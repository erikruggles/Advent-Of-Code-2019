using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AdventOfCode2019.Day01;
using Newtonsoft.Json;

namespace AdventOfCode2019.Day02
{
    public class IntcodeComputer
    {
        private int[] _memory;

        public IntcodeComputer(int[] startingMemory, int noun, int verb)
        {
            _memory = new int[startingMemory.Length];
            startingMemory.CopyTo(_memory, 0);
            _memory[1] = noun;
            _memory[2] = verb;
        }
        
        public static async Task<int[]> ParseImportFileForIntCode()
        {
            var moduleText =
                await File.ReadAllTextAsync(@"C:\Projects\Advent-Of-Code-2019\AdventOfCode2019\Day02\intcode.json");
            return JsonConvert.DeserializeObject<int[]>(moduleText);
        }
        
        public decimal ProcessIntcode()
        {
            int currentInstructionPointer = 0;
            while (PerformInstruction(currentInstructionPointer))
            {
                currentInstructionPointer += 4;
            }

            return _memory[0];
        }
        
        private bool PerformInstruction(int instructionPointer)
        {
            switch (_memory[instructionPointer])
            {
                case 1:
                    _memory[_memory[instructionPointer + 3]] =
                        _memory[_memory[instructionPointer + 1]] + _memory[_memory[instructionPointer + 2]];
                    return true;
                case 2:
                    _memory[_memory[instructionPointer + 3]] =
                        _memory[_memory[instructionPointer + 1]] * _memory[_memory[instructionPointer + 2]];
                    return true;
                case 99:
                    return false;
                default:
                    throw new InvalidOperationException($"Unknown opcode of {_memory[instructionPointer]}");
            }
        }
    }
}