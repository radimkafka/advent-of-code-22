using System;
using System.IO;
using System.Linq;

namespace Day2
{
    class Program
    {
        public enum Variant
        {
            None,
            Rock,
            Paper,
            Scissors
        }

        static Variant GetVariant(string c)
        {
            if (c == "A" || c == "X") return Variant.Rock;
            if (c == "B" || c == "Y") return Variant.Paper;
            if (c == "C" || c == "Z") return Variant.Scissors;
            return Variant.None;
        }

        static int Result(Variant v1, Variant v2)
        {
            Console.Write($"{v1} {v2} ");
            if (v1 == v2)
            {
                Console.WriteLine($"3");
                return 3;
            }
            if (v1 == Variant.Scissors && v2 == Variant.Rock)
            {
                Console.WriteLine($"0");
                return 0;
            }
            if (v1 > v2|| (v1 == Variant.Rock && v2 == Variant.Scissors))
            {
                Console.WriteLine($"6");
                return 6;
            }
            Console.WriteLine($"0");            
            return 0;
        }

        public class Round
        {
            public Variant Elf { get; set; }

            public Variant Me { get; set; }

            public Round(string input)
            {
                var inputs = input.Split(' ');
                Elf = inputs.Length > 0 ? GetVariant(inputs[0]) : Variant.None;
                Me = inputs.Length > 1 ? GetVariant(inputs[1]) : Variant.None;
            }

            public int Points()
            {
                return Result(Me, Elf) + (int)Me;
            }
        }

        static void Main(string[] args)
        {
            var filePath = "../../../input.txt";
            //var filePath = "../../../input.basic.txt";
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File does not exist!");
                return;
            }

            var lines = File.ReadAllLines(filePath);
            var sum = 0;
            foreach (var item in lines)
            {
                sum += new Round(item).Points();
            }

            Console.WriteLine($"if everything goes exactly according to the strategy guide I will have: {sum} points.");
            Console.ReadLine();
        }
    }
}
