using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using AdventOfCode2019.Day01;
using Newtonsoft.Json;

namespace AdventOfCode2019.Day05
{
    public class IntcodeComputer
    {
        private const int POSITION_MODE = 0;
        private readonly int _testId;
        
        private int[] _memory;

        public IntcodeComputer(int[] startingMemory, int testId)
        {
            _memory = new int[startingMemory.Length];
            startingMemory.CopyTo(_memory, 0);
            _testId = testId;
        }
        
        public static async Task<int[]> ParseImportFileForIntCode()
        {
            var moduleText =
                await File.ReadAllTextAsync(@"C:\Projects\Advent-Of-Code-2019\AdventOfCode2019\Day05\intcode.json");
            return JsonConvert.DeserializeObject<int[]>(moduleText);
        }
        
        public decimal ProcessIntcode()
        {
            Console.WriteLine($"Performing Test {_testId}");
            var instructionPointer = 0;
            while (PerformInstruction(instructionPointer, out var newInstructionPointer))
            {
                instructionPointer = newInstructionPointer;
            }

            return _memory[0];
        }
        
        private bool PerformInstruction(int instructionPointer, out int newInstructionPointer)
        {
            var instruction = _memory[instructionPointer].ToString();
            var opCode = instruction.Length <= 2
                ? int.Parse(instruction)
                : int.Parse(instruction.Substring(instruction.Length - 2));
            var modeThirdParameter = instruction.Length <= 4 ? POSITION_MODE : int.Parse(instruction[0].ToString());
            var modeSecondParameter = instruction.Length <= 3 ? POSITION_MODE : int.Parse(instruction[^4].ToString());
            var modeFirstParameter = instruction.Length <= 2 ? POSITION_MODE : int.Parse(instruction[^3].ToString());
            switch (opCode)
            {
                case 1: //Add
                    _memory[GetMemoryAddress(instructionPointer + 3, modeThirdParameter)] =
                        _memory[GetMemoryAddress(instructionPointer + 1, modeFirstParameter)] +
                        _memory[GetMemoryAddress(instructionPointer + 2, modeSecondParameter)];
                    newInstructionPointer = instructionPointer + 4;
                    return true;
                case 2: //Multiply
                    _memory[GetMemoryAddress(instructionPointer + 3, modeThirdParameter)] =
                        _memory[GetMemoryAddress(instructionPointer + 1, modeFirstParameter)] *
                        _memory[GetMemoryAddress(instructionPointer + 2, modeSecondParameter)];
                    newInstructionPointer = instructionPointer + 4;
                    return true;
                case 3: //Input
                    _memory[GetMemoryAddress(instructionPointer + 1, modeFirstParameter)] = _testId;
                    newInstructionPointer = instructionPointer + 2;
                    return true;
                case 4: //Output
                    Console.WriteLine(
                        $"IntcodeComputer Output {_memory[GetMemoryAddress(instructionPointer + 1, modeFirstParameter)]}");
                    newInstructionPointer = instructionPointer + 2;
                    return true;
                case 5: //Jump-if-true
                    newInstructionPointer = _memory[GetMemoryAddress(instructionPointer + 1, modeFirstParameter)] != 0
                        ? _memory[GetMemoryAddress(instructionPointer + 2, modeSecondParameter)]
                        : instructionPointer + 3;
                    return true;
                case 6: //Jump-if-false
                    newInstructionPointer = _memory[GetMemoryAddress(instructionPointer + 1, modeFirstParameter)] == 0
                        ? _memory[GetMemoryAddress(instructionPointer + 2, modeSecondParameter)]
                        : instructionPointer + 3;
                    return true;
                case 7: //Less than
                    _memory[GetMemoryAddress(instructionPointer + 3, modeThirdParameter)] =
                        _memory[GetMemoryAddress(instructionPointer + 1, modeFirstParameter)] <
                        _memory[GetMemoryAddress(instructionPointer + 2, modeSecondParameter)] ? 1 : 0;
                    newInstructionPointer = instructionPointer + 4;
                    return true;
                case 8: //equals than
                    _memory[GetMemoryAddress(instructionPointer + 3, modeThirdParameter)] =
                        _memory[GetMemoryAddress(instructionPointer + 1, modeFirstParameter)] ==
                        _memory[GetMemoryAddress(instructionPointer + 2, modeSecondParameter)] ? 1 : 0;
                    newInstructionPointer = instructionPointer + 4;
                    return true;
                case 99: //Halt
                    newInstructionPointer = instructionPointer + 1;
                    Console.WriteLine($"IntcodeComputer Halted");
                    return false;
                default:
                    throw new InvalidOperationException($"Unknown opcode of {_memory[instructionPointer]}");
            }
        }

        private int GetMemoryAddress(int currentMemoryAddress, int parameterMode)
        {
            return parameterMode == POSITION_MODE ? _memory[currentMemoryAddress] : currentMemoryAddress;
        }
    }
}