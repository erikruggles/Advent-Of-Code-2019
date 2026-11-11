using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode2019.Day06
{
    public class OrbitMap
    {
        private const string CenterOfMass = "COM";

        // Maps each object to the object it directly orbits.
        private readonly Dictionary<string, string> _parents = new Dictionary<string, string>();
        private readonly Dictionary<string, int> _depthCache = new Dictionary<string, int> { [CenterOfMass] = 0 };

        public OrbitMap(IEnumerable<string> orbitRelationships)
        {
            foreach (var relationship in orbitRelationships.Select(r => r.Trim()).Where(r => r.Length > 0))
            {
                var parts = relationship.Split(')');
                if (parts.Length != 2)
                    throw new FormatException($"Invalid orbit relationship: '{relationship}'");

                _parents[parts[1]] = parts[0];
            }
        }

        public static async Task<OrbitMap> BuildOrbitMapFromImportFile()
        {
            var lines = await File.ReadAllLinesAsync(@"C:\s\Advent-Of-Code-2019\AdventOfCode2019\Day06\orbits.txt");
            return new OrbitMap(lines);
        }

        // Every object other than COM directly orbits exactly one object.
        public int CountDirectOrbits() => _parents.Count;

        // Every ancestor beyond the direct parent is an indirect orbit.
        public int CountIndirectOrbits() => CountTotalOrbits() - CountDirectOrbits();

        // An object's total orbit count equals its distance from COM.
        public int CountTotalOrbits() => _parents.Keys.Sum(GetDepth);

        // Minimum number of edges between two objects in the orbit graph.
        // The graph is a tree rooted at COM, so the shortest path runs through the closest common ancestor.
        public int GetDistance(string from, string to)
        {
            var stepsFromStart = new Dictionary<string, int>();
            var steps = 0;
            foreach (var obj in GetPathToCenterOfMass(from))
            {
                stepsFromStart[obj] = steps++;
            }

            steps = 0;
            foreach (var obj in GetPathToCenterOfMass(to))
            {
                if (stepsFromStart.TryGetValue(obj, out var stepsToCommonAncestor))
                    return stepsToCommonAncestor + steps;
                steps++;
            }

            throw new InvalidOperationException($"'{from}' and '{to}' are not connected");
        }

        // Yields the object itself, then each object it orbits, ending at COM.
        private IEnumerable<string> GetPathToCenterOfMass(string obj)
        {
            if (obj != CenterOfMass && !_parents.ContainsKey(obj))
                throw new ArgumentException($"'{obj}' is not in the orbit map", nameof(obj));

            var current = obj;
            yield return current;
            while (_parents.TryGetValue(current, out var parent))
            {
                current = parent;
                yield return current;
            }
        }

        private int GetDepth(string obj)
        {
            // Walk up until a cached depth is found, then fill the cache on the way back down.
            var chain = new Stack<string>();
            var current = obj;
            while (!_depthCache.ContainsKey(current))
            {
                if (!_parents.TryGetValue(current, out var parent))
                    throw new InvalidOperationException($"'{current}' does not connect back to {CenterOfMass}");
                chain.Push(current);
                current = parent;
            }

            var depth = _depthCache[current];
            while (chain.Count > 0)
            {
                _depthCache[chain.Pop()] = ++depth;
            }

            return _depthCache[obj];
        }
    }
}
