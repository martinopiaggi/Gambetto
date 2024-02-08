using UnityEngine;

namespace Gambetto.Scripts.Utils
{
    public static class RomanNumeralGenerator
    {
        /// <summary>
        /// Generates a roman numeral from a number. Requires a number between 1 and 3999.
        /// </summary>
        /// <param name="number"></param>
        /// <returns>Roman numeral.</returns>
        public static string GenerateNumeral(int number)
        {
            if (number is < 0 or > 3999)
            {
                Debug.LogError("Number" + number + " out of range for Roman numerals (1-3999).");
                return string.Empty;
            }

            if (number == 0)
            {
                return string.Empty;
            }

            string[] romanNumerals =
            {
                "I",
                "IV",
                "V",
                "IX",
                "X",
                "XL",
                "L",
                "XC",
                "C",
                "CD",
                "D",
                "CM",
                "M"
            };
            int[] values = { 1, 4, 5, 9, 10, 40, 50, 90, 100, 400, 500, 900, 1000 };

            string result = "";

            for (int i = values.Length - 1; i >= 0; i--)
            {
                while (number >= values[i])
                {
                    result += romanNumerals[i];
                    number -= values[i];
                }
            }

            return result;
        }
    }
}
