using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AdventOfCode2019.Day01
{
    public class RocketModule
    {
        public RocketModule(int mass)
        {
            Mass = mass;
        }
        
        public decimal Mass { get;  }

        public decimal CalculateBaseFuelFromMass() => Math.Floor(Mass / 3) - 2;

        public decimal CalculateTotalFuelRequired(decimal amountOfFuelAlreadyRequired) =>
            amountOfFuelAlreadyRequired <= 0
                ? 0
                : amountOfFuelAlreadyRequired +
                  CalculateTotalFuelRequired(Math.Floor(amountOfFuelAlreadyRequired / 3) - 2);
    }

    public class Rocket
    {
        public List<RocketModule> Modules { get; set; }

        public static async Task<Rocket> BuildRocketFromImportedModules()
        {
            var moduleText =
                await File.ReadAllTextAsync(@"C:\Projects\Advent-Of-Code-2019\AdventOfCode2019\Day01\modules.json");
            return JsonConvert.DeserializeObject<Rocket>(moduleText);
        }

        public decimal CalculateTotalFuelRequiredByMass() => Modules.Sum(m => m.CalculateBaseFuelFromMass());

        public decimal CalculateTotalFuelRequired() =>
            Modules.Sum(m => m.CalculateTotalFuelRequired(m.CalculateBaseFuelFromMass()));
    }
}