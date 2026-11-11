using System;
using System.Linq;
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
            Console.WriteLine("=== Day 1 ===");
            var rocket = await Rocket.BuildRocketFromImportedModules();
            Console.WriteLine($"Total Fuel Required By Module Mass: {rocket.CalculateTotalFuelRequiredByMass()}");
            Console.WriteLine($"Total Fuel Required: {rocket.CalculateTotalFuelRequired()}");

            //Day 2
            Console.WriteLine("=== Day 2 ===");
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
            Console.WriteLine("=== Day 3 ===");
            var electricalPanel = new ElectricPanel(await ElectricPanel.ParseImportFileForWirePaths());
            electricalPanel.ProcessPaths();
            var (x, y, _) = electricalPanel.FindClosesIntersectionToOriginByManhattanDistance();
            Console.WriteLine($"Manhattan Distance to closest intersection: {Math.Abs(x)+Math.Abs(y)}");
            var (_, _, z) = electricalPanel.FindClosesIntersectionToOriginByWireLength();
            Console.WriteLine($"Wire Distance to closest intersection: {z}");
            
            //Day 4
            Console.WriteLine("=== Day 4 ===");
            var passwordCracker = new PasswordCracker(183564, 657474);
            var validPasswords = passwordCracker.FindValidPasswords(false);
            Console.WriteLine($"Number of valid passwords with no length restriction on repeats: {validPasswords.Count}");
            validPasswords = passwordCracker.FindValidPasswords(true);
            Console.WriteLine($"Number of valid passwords with a repeat of exactly two: {validPasswords.Count}");
            
            //Day 5
            Console.WriteLine("=== Day 5 ===");
            var intcodeDay5 = await Day05.IntcodeComputer.ParseImportFileForIntCode();
            var intcodeComputerDay5 = new Day05.IntcodeComputer(intcodeDay5, 1);
            intcodeComputerDay5.ProcessIntcode();
            intcodeComputerDay5 = new Day05.IntcodeComputer(intcodeDay5, 5);
            intcodeComputerDay5.ProcessIntcode();

            //Day 6
            Console.WriteLine("=== Day 6 ===");
            var orbitMap = await Day06.OrbitMap.BuildOrbitMapFromImportFile();
            Console.WriteLine($"Direct orbits: {orbitMap.CountDirectOrbits()}");
            Console.WriteLine($"Indirect orbits: {orbitMap.CountIndirectOrbits()}");
            Console.WriteLine($"Total orbits: {orbitMap.CountTotalOrbits()}");
            var youToSanta = orbitMap.GetDistance("YOU", "SAN");
            Console.WriteLine($"Distance between YOU and SAN: {youToSanta}");
            // YOU and SAN are each one step from the objects they orbit, so the transfers needed are two fewer.
            Console.WriteLine($"Orbital transfers from YOU's orbit to SAN's orbit: {youToSanta - 2}");

            //Day 7
            Console.WriteLine("=== Day 7 ===");
            var amplifierCircuit = await Day07.AmplifierCircuit.BuildFromImportFile();
            var (maxSignal, bestPhases) = amplifierCircuit.FindMaxThrusterSignal(new[] { 0, 1, 2, 3, 4 });
            Console.WriteLine($"Highest thruster signal: {maxSignal} (phase settings {string.Join(",", bestPhases)})");
            var (maxLoopSignal, bestLoopPhases) = amplifierCircuit.FindMaxFeedbackLoopSignal(new[] { 5, 6, 7, 8, 9 });
            Console.WriteLine($"Highest feedback loop thruster signal: {maxLoopSignal} (phase settings {string.Join(",", bestLoopPhases)})");

            //Day 8
            Console.WriteLine("=== Day 8 ===");
            var spaceImage = await Day08.SpaceImage.BuildFromImportFile(25, 6);
            Console.WriteLine($"Number of 1 digits * number of 2 digits on layer with fewest 0 digits: {spaceImage.CalculateCorruptionCheck()}");
            Console.WriteLine("Decoded image:");
            Console.WriteLine(spaceImage.Render());

            //Day 9
            Console.WriteLine("=== Day 9 ===");
            var boostProgram = await Day09.IntcodeComputer.ParseImportFile(
                @"C:\s\Advent-Of-Code-2019\AdventOfCode2019\Day09\boost-input.csv");
            var boostTest = new Day09.IntcodeComputer(boostProgram);
            boostTest.AddInput(1);
            boostTest.Run();
            var boostOutputs = boostTest.ReadAllOutputs();
            if (boostOutputs.Count > 1)
                Console.WriteLine($"Malfunctioning opcodes reported: {string.Join(",", boostOutputs.Take(boostOutputs.Count - 1))}");
            Console.WriteLine($"BOOST keycode: {boostOutputs.LastOrDefault()}");
            var sensorBoost = new Day09.IntcodeComputer(boostProgram);
            sensorBoost.AddInput(2);
            sensorBoost.Run();
            Console.WriteLine($"Distress signal coordinates: {string.Join(",", sensorBoost.ReadAllOutputs())}");

            Console.Read();
        }
    }
}