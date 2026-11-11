using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode2019.Day07
{
    public class AmplifierCircuit
    {
        private readonly long[] _program;

        public AmplifierCircuit(long[] program)
        {
            _program = program;
        }

        public static async Task<AmplifierCircuit> BuildFromImportFile()
        {
            var program = await IntcodeComputer.ParseImportFile(
                @"C:\s\Advent-Of-Code-2019\AdventOfCode2019\Day07\amp-ctrl-sftwr.txt");
            return new AmplifierCircuit(program);
        }

        // Tries every ordering of the phase settings and returns the best thruster signal found.
        public (long signal, int[] phaseSettings) FindMaxThrusterSignal(IEnumerable<int> phaseSettings) =>
            FindMaxSignal(phaseSettings, RunInSeries);

        // Same as FindMaxThrusterSignal, but with amplifier E's output looped back into amplifier A.
        public (long signal, int[] phaseSettings) FindMaxFeedbackLoopSignal(IEnumerable<int> phaseSettings) =>
            FindMaxSignal(phaseSettings, RunInFeedbackLoop);

        private static (long signal, int[] phaseSettings) FindMaxSignal(
            IEnumerable<int> phaseSettings, Func<int[], long> runCircuit)
        {
            return GetPermutations(phaseSettings.ToArray())
                .Select(p => (signal: runCircuit(p), phaseSettings: p))
                .OrderByDescending(r => r.signal)
                .First();
        }

        // Connects the last amplifier's output back into the first and keeps passing signals around
        // until the last amplifier halts. Its final output is the thruster signal.
        public long RunInFeedbackLoop(int[] phaseSettings)
        {
            var amplifiers = phaseSettings.Select(phase =>
            {
                var amplifier = new IntcodeComputer(_program);
                amplifier.AddInput(phase);
                return amplifier;
            }).ToArray();
            amplifiers[0].AddInput(0);

            long? lastSignalFromFinalAmplifier = null;
            var last = amplifiers.Length - 1;
            while (amplifiers[last].State != IntcodeState.Halted)
            {
                var signalsPassed = false;
                for (var i = 0; i < amplifiers.Length; i++)
                {
                    if (amplifiers[i].State == IntcodeState.Halted)
                        continue;

                    amplifiers[i].Run();
                    var next = amplifiers[(i + 1) % amplifiers.Length];
                    while (amplifiers[i].HasOutput)
                    {
                        var signal = amplifiers[i].ReadOutput();
                        if (i == last)
                            lastSignalFromFinalAmplifier = signal;
                        next.AddInput(signal);
                        signalsPassed = true;
                    }
                }

                // Every amplifier is waiting on input that will never come.
                if (!signalsPassed && amplifiers[last].State != IntcodeState.Halted)
                    throw new InvalidOperationException("Feedback loop stalled before the final amplifier halted");
            }

            return lastSignalFromFinalAmplifier
                   ?? throw new InvalidOperationException("Final amplifier halted without producing a signal");
        }

        // Runs each amplifier once, feeding each one's output into the next, starting from a signal of 0.
        public long RunInSeries(int[] phaseSettings)
        {
            long signal = 0;
            foreach (var phase in phaseSettings)
            {
                var amplifier = new IntcodeComputer(_program);
                amplifier.AddInput(phase);
                amplifier.AddInput(signal);
                amplifier.Run();
                if (!amplifier.HasOutput)
                    throw new InvalidOperationException($"Amplifier with phase {phase} produced no output");
                signal = amplifier.ReadOutput();
            }

            return signal;
        }

        private static IEnumerable<int[]> GetPermutations(int[] items)
        {
            if (items.Length <= 1)
            {
                yield return items;
                yield break;
            }

            for (var i = 0; i < items.Length; i++)
            {
                var rest = items.Where((_, index) => index != i).ToArray();
                foreach (var permutation in GetPermutations(rest))
                {
                    yield return new[] { items[i] }.Concat(permutation).ToArray();
                }
            }
        }
    }
}
