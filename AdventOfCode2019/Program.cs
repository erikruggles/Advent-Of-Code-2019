using System;
using System.Threading.Tasks;
using AdventOfCode2019.Day01;
using AdventOfCode2019.Day02;
using AdventOfCode2019.Day03;
using AdventOfCode2019.Day04;

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
            
            //Day 3
            var electricalPanel = new ElectricPanel(await ElectricPanel.ParseImportFileForWirePaths());
            electricalPanel.ProcessPaths();
            var (x, y, _) = electricalPanel.FindClosesIntersectionToOriginByManhattanDistance();
            Console.WriteLine($"Manhattan Distance to closest intersection: {Math.Abs(x)+Math.Abs(y)}");
            var (_, _, z) = electricalPanel.FindClosesIntersectionToOriginByWireLength();
            Console.WriteLine($"Wire Distance to closest intersection: {z}");
            
            //Day 4
            var passwordCracker = new PasswordCracker(183564, 657474);
            var validPasswords = passwordCracker.FindValidPasswords(false);
            Console.WriteLine($"Number of valid passwords with no length restriction on repeats: {validPasswords.Count}");
            validPasswords = passwordCracker.FindValidPasswords(true);
            Console.WriteLine($"Number of valid passwords with a repeat of exactly two: {validPasswords.Count}");
            
            //Day 5
            var intcodeDay5 = await Day05.IntcodeComputer.ParseImportFileForIntCode();
            var intcodeComputerDay5 = new Day05.IntcodeComputer(intcodeDay5, 1);
            intcodeComputerDay5.ProcessIntcode();
            intcodeComputerDay5 = new Day05.IntcodeComputer(intcodeDay5, 5);
            intcodeComputerDay5.ProcessIntcode();
            
            Console.ReadKey();
        }
    }
}