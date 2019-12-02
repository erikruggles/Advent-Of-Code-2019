using System;
using System.Threading.Tasks;
using AdventOfCode2019.Day01;
using AdventOfCode2019.Day02;

namespace AdventOfCode2019
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Day 1
            var rocket = await Rocket.BuildRocketFromImportedModules();
            Console.WriteLine($"Total Fuel Required By Module Mass: {rocket.CalculateTotalFuelRequiredByMass()}");
            Console.WriteLine($"Total Fuel Required: {rocket.CalculateTotalFuelRequired()}");

            //Day 2
            var intcode = await IntcodeComputer.ParseImportFileForIntCode();
            var intcodeComputer = new IntcodeComputer(intcode, 12, 2);
            Console.WriteLine($"Value at position 0: {intcodeComputer.ProcessIntcode()}");
            var foundNounAndVerb = false;
            for (int noun = 0; noun < 99; noun++)
            {
                if (foundNounAndVerb) break;
                for (int verb = 0; verb < 99; verb++)
                {
                    if (foundNounAndVerb) break;
                    var tempComputer = new IntcodeComputer(intcode, noun, verb);
                    try
                    {
                        if (tempComputer.ProcessIntcode() == 19690720)
                        {
                            Console.WriteLine($"100 * Noun + Verb: {100 * noun + verb}");
                            foundNounAndVerb = true;
                        }
                    }
                    catch (Exception)
                    {
                        //Do nothing
                    }
                }
            }
            
            Console.ReadKey();
        }
    }
}