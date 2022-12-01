using System;
using System.IO;
using System.Linq;

namespace Day1
{
    class Program
    {
        static void Main(string[] args)
        {

            var filePath = "../../../input.txt";
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File does not exist!");
                return;
            }

            var lines = File.ReadAllLines(filePath);
            var maxSum = 0;
            var elfWithMostCal = 0;

            var currentSum = 0;
            var elfCount = 0;
            var topElves = new int[] { 0, 0, 0 };
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] == string.Empty)
                {
                    elfCount++;
                    if (topElves.Any(a => a < currentSum))
                    {
                        topElves[0] = currentSum;
                        Array.Sort(topElves);
                    }
                    maxSum = currentSum > maxSum ? currentSum : maxSum;
                    elfWithMostCal = currentSum > maxSum ? elfCount : elfWithMostCal;
                    currentSum = 0;
                    continue;
                }
                if (int.TryParse(lines[i], out int value))
                {
                    currentSum += value;
                }
            }
            Console.WriteLine($"Count of elfs: {elfCount}");
            Console.WriteLine($"Elf carrying the most Calories is on the position: {elfWithMostCal}: {maxSum}");

            Console.WriteLine($"Top 3 elves has: {string.Join(", ",topElves)} and {topElves.Sum()} in total.");
            Console.ReadLine();
        }
       
    }
}
