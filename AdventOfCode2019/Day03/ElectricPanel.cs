using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AdventOfCode2019.Day03
{
    public class ElectricPanel
    {
        private List<List<(int x, int y)>> _wirePathCoordinates = new List<List<(int x, int y)>>();
        private readonly List<string[]> _wirePaths;

        public ElectricPanel(List<string[]> wirePaths)
        {
            _wirePaths = wirePaths;
        }

        public void ProcessPaths()
        {
            foreach (var path in _wirePaths)
            {
                ProcessPath(path);
            }
        }

        public (int x, int y, int z) FindClosesIntersectionToOriginByManhattanDistance()
        {
            var intersections = FindIntersectionsOfWires(_wirePathCoordinates[0], _wirePathCoordinates[1]);
            var (x, y, z) = intersections.Where(t => t.x != 0 || t.y != 0).OrderBy(t => Math.Abs(t.x) + Math.Abs(t.y))
                .First();
            return (x, y, z);
        }

        public (int x, int y, int z) FindClosesIntersectionToOriginByWireLength()
        {
            var intersections = FindIntersectionsOfWires(_wirePathCoordinates[0], _wirePathCoordinates[1]);
            var (x, y, z) = intersections.Where(t => t.x != 0 || t.y != 0).OrderBy(t => t.z).First();
            return (x, y, z);
        }

        private static IEnumerable<(int x, int y, int z)> FindIntersectionsOfWires(List<(int x, int y)> wire1,
            List<(int x, int y)> wire2)
        {
            var intersections = new List<(int x, int y, int z)>();

            var wire2XLookup = wire2.ToLookup(w => w.x);

            for (var stepWire1 = 0; stepWire1 < wire1.Count; stepWire1++)
            {
                var coordinate = wire1[stepWire1];
                var xMatches = wire2XLookup[coordinate.x];
                var matches = xMatches.Where(match => match.x == coordinate.x && match.y == coordinate.y);
                intersections.AddRange(matches.Select(m => (m.x, m.y, wire2.IndexOf(m) + stepWire1)));
            }

            return intersections;
        }

        private void ProcessPath(string[] path)
        {
            var coordinates = new List<(int x, int y)>();
            var currentX = 0;
            var currentY = 0;
            foreach (var instruction in path)
            {
                (currentX, currentY) = ProcessInstruction(instruction, currentX, currentY, coordinates);
            }
            _wirePathCoordinates.Add(coordinates);
        }
        
        private (int x, int y) ProcessInstruction(string instruction, int currentX, int currentY, List<(int x, int y)> path)
        {

            var direction = instruction[0];
            var distance = int.Parse(instruction.Substring(1));

            while (distance > 0)
            {
                path.Add(direction switch
                {
                    'U' => (currentX, currentY++),
                    'D' => (currentX, currentY--),
                    'R' => (currentX++, currentY),
                    'L' => (currentX--, currentY),
                    _ => throw new InvalidDataException($"Unable to process instruction {instruction}")
                });
                distance--;
            }

            return (currentX, currentY);
        }
        
        public static async Task<List<string[]>> ParseImportFileForWirePaths()
        {
            var moduleText =
                await File.ReadAllTextAsync(@"C:\Projects\Advent-Of-Code-2019\AdventOfCode2019\Day03\wirepaths.json");
            var unseparatedPaths = JsonConvert.DeserializeObject<List<string>>(moduleText);
            return unseparatedPaths.Select(p => p.Split(',')).ToList();
        }
    }
}