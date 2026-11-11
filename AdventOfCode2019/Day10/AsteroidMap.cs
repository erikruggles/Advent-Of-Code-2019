using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode2019.Day10
{
    public class AsteroidMap
    {
        private const char Asteroid = '#';
        private const char Empty = '.';

        public IReadOnlyList<(int x, int y)> Asteroids { get; }

        public AsteroidMap(IEnumerable<string> mapRows)
        {
            var asteroids = new List<(int x, int y)>();
            var y = 0;
            foreach (var row in mapRows.Select(r => r.Trim()).Where(r => r.Length > 0))
            {
                for (var x = 0; x < row.Length; x++)
                {
                    if (row[x] == Asteroid)
                        asteroids.Add((x, y));
                    else if (row[x] != Empty)
                        throw new FormatException($"Invalid map character '{row[x]}' at {x},{y}");
                }

                y++;
            }

            Asteroids = asteroids;
        }

        public static async Task<AsteroidMap> BuildFromImportFile()
        {
            var rows = await File.ReadAllLinesAsync(@"C:\s\Advent-Of-Code-2019\AdventOfCode2019\Day10\asteroid-map.txt");
            return new AsteroidMap(rows);
        }

        // Finds the asteroid that can detect the most other asteroids.
        public ((int x, int y) location, int detected) FindBestMonitoringStation()
        {
            if (Asteroids.Count == 0)
                throw new InvalidOperationException("The map contains no asteroids");

            return Asteroids
                .Select(a => (location: a, detected: CountDetectable(a)))
                .MaxBy(r => r.detected);
        }

        // Asteroids on the same line of sight share a reduced direction (dx, dy divided by their GCD),
        // and only the nearest one in each direction is visible, so the count of directions is the count detected.
        public int CountDetectable((int x, int y) station)
        {
            return Asteroids
                .Where(a => a != station)
                .Select(a => GetDirection(station, a))
                .Distinct()
                .Count();
        }

        // Returns every other asteroid in the order a laser at the station vaporizes them: it starts pointing up,
        // rotates clockwise, and destroys only the nearest asteroid in each direction per rotation.
        public List<(int x, int y)> GetVaporizationOrder((int x, int y) station)
        {
            // One queue per line of sight, nearest asteroid first, with the lines ordered clockwise from up.
            var linesOfSight = Asteroids
                .Where(a => a != station)
                .GroupBy(a => GetDirection(station, a))
                .OrderBy(g => GetClockwiseAngleFromUp(g.Key))
                .Select(g => new Queue<(int x, int y)>(
                    g.OrderBy(a => Math.Abs(a.x - station.x) + Math.Abs(a.y - station.y))))
                .ToList();

            var order = new List<(int x, int y)>();
            while (linesOfSight.Count > 0)
            {
                foreach (var line in linesOfSight)
                {
                    order.Add(line.Dequeue());
                }

                linesOfSight.RemoveAll(line => line.Count == 0);
            }

            return order;
        }

        // Y increases downward on the map, so "up" is -dy. Returns radians in [0, 2π), increasing clockwise.
        private static double GetClockwiseAngleFromUp((int dx, int dy) direction)
        {
            var angle = Math.Atan2(direction.dx, -direction.dy);
            return angle < 0 ? angle + 2 * Math.PI : angle;
        }

        private static (int dx, int dy) GetDirection((int x, int y) from, (int x, int y) to)
        {
            var dx = to.x - from.x;
            var dy = to.y - from.y;
            var gcd = GreatestCommonDivisor(Math.Abs(dx), Math.Abs(dy));
            return (dx / gcd, dy / gcd);
        }

        private static int GreatestCommonDivisor(int a, int b)
        {
            while (b != 0)
            {
                (a, b) = (b, a % b);
            }

            return a;
        }
    }
}
