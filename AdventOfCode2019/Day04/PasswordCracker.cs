using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2019.Day04
{
    public class PasswordCracker
    {
        private readonly int _lowerRange;
        private readonly int _upperRange;

        public PasswordCracker(int lowerRange, int upperRange)
        {
            _lowerRange = lowerRange;
            _upperRange = upperRange;
        }

        public List<int> FindValidPasswords(bool restrictRepeatedNumberToOnlyDoubles)
        {
            var passwords = new List<int>();
            for (var i = _lowerRange; i < _upperRange; i++)
            {
                passwords.Add(i);
            }

            return passwords.Where(p => IsPasswordValidFormat(p, restrictRepeatedNumberToOnlyDoubles)).ToList();
        }
        
        
        private bool IsPasswordValidFormat(int password, bool restrictRepeatedNumberToOnlyDoubles)
        {
            var passwordString = password.ToString();
            var hasRepeatedNumber = false;
            var numberOfTimesNumberRepeated = 0;
            var previousNumber = -1;
            foreach (var currentNumber in passwordString.Select(t => int.Parse(t.ToString())))
            {
                if (previousNumber == currentNumber)
                {
                    if (!restrictRepeatedNumberToOnlyDoubles)
                    {
                        hasRepeatedNumber = true;
                    }

                    numberOfTimesNumberRepeated++;
                }
                else
                {
                    if (restrictRepeatedNumberToOnlyDoubles && numberOfTimesNumberRepeated == 1)
                    {
                        hasRepeatedNumber = true;
                    }

                    numberOfTimesNumberRepeated = 0;
                }

                if (previousNumber > currentNumber)
                {
                    return false;
                }

                previousNumber = currentNumber;
            }

            if (restrictRepeatedNumberToOnlyDoubles && numberOfTimesNumberRepeated == 1)
            {
                hasRepeatedNumber = true;
            }
            
            return hasRepeatedNumber;
        }
    }
}