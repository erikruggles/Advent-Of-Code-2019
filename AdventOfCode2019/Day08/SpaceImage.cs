using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode2019.Day08
{
    public class SpaceImage
    {
        public int Width { get; }
        public int Height { get; }

        // Each layer is a flat array of pixel digits, filled left-to-right then top-to-bottom.
        public IReadOnlyList<int[]> Layers { get; }

        public SpaceImage(string imageData, int width, int height)
        {
            Width = width;
            Height = height;

            var digits = imageData.Trim();
            var layerSize = width * height;
            if (digits.Length == 0 || digits.Length % layerSize != 0)
                throw new ArgumentException(
                    $"Image data length {digits.Length} is not a multiple of the layer size {layerSize}",
                    nameof(imageData));

            Layers = digits
                .Select(c => char.IsDigit(c)
                    ? c - '0'
                    : throw new FormatException($"Invalid pixel '{c}' in image data"))
                .Chunk(layerSize)
                .ToList();
        }

        public static async Task<SpaceImage> BuildFromImportFile(int width, int height)
        {
            var imageData =
                await File.ReadAllTextAsync(@"C:\s\Advent-Of-Code-2019\AdventOfCode2019\Day08\encoded-image.txt");
            return new SpaceImage(imageData, width, height);
        }

        // Finds the layer with the fewest 0 digits and multiplies its count of 1 digits by its count of 2 digits.
        public int CalculateCorruptionCheck()
        {
            var layer = Layers.MinBy(l => CountDigit(l, 0));
            return CountDigit(layer, 1) * CountDigit(layer, 2);
        }

        public const int Black = 0;
        public const int White = 1;
        public const int Transparent = 2;

        // Stacks the layers with the first in front: each pixel takes the first non-transparent value behind it.
        public int[] Decode()
        {
            return Enumerable.Range(0, Width * Height)
                .Select(i => Layers.Select(layer => layer[i]).FirstOrDefault(p => p != Transparent, Transparent))
                .ToArray();
        }

        // Draws the decoded image as text, one line per row. White pixels are '#', everything else is a space.
        public string Render()
        {
            var pixels = Decode();
            return string.Join(Environment.NewLine, pixels
                .Chunk(Width)
                .Select(row => new string(row.Select(p => p == White ? '#' : ' ').ToArray())));
        }

        private static int CountDigit(int[] layer, int digit) => layer.Count(pixel => pixel == digit);
    }
}
