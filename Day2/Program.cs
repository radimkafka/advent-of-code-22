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

        public enum ExpectedResult
        {
            None,
            Win,
            Loose,
            Draw
        }

        static ExpectedResult GetExpectedResult(string c)
        {
            if (c == "X") return ExpectedResult.Loose;
            if (c == "Y") return ExpectedResult.Draw;
            if (c == "Z") return ExpectedResult.Win;
            return ExpectedResult.None;
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
            if (v1 == v2)
            {
                return 3;
            }
            if (v1 == Variant.Scissors && v2 == Variant.Rock)
            {

                return 0;
            }
            if (v1 > v2 || (v1 == Variant.Rock && v2 == Variant.Scissors))
            {

                return 6;
            }

            return 0;
        }

        static Variant GuessShape(Variant v1, ExpectedResult v2)
        {
            if (v2 == ExpectedResult.Draw)
            {
                return v1;
            }
            if (v2 == ExpectedResult.Loose)
            {
                return v1 == Variant.Rock ? Variant.Scissors : v1 - 1;
            }
            if (v2 == ExpectedResult.Win)
            {
                return v1 == Variant.Scissors ? Variant.Rock : v1 + 1;
            }
            return Variant.None;
        }


        public class Round
        {
            public Variant Input1 { get; set; }

            public Variant Input2 { get; set; }

            public Round(string input, bool guessShape = false)
            {
                var inputs = input.Split(' ');
                Input1 = inputs.Length > 0 ? GetVariant(inputs[0]) : Variant.None;
                if (!guessShape) Input2 = inputs.Length > 1 ? GetVariant(inputs[1]) : Variant.None;
                else Input2 = inputs.Length > 1 ? GuessShape(Input1, GetExpectedResult(inputs[1])) : Variant.None;
            }

            public int Points()
            {
                return Result(Input2, Input1) + (int)Input2;
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
            var sum2 = 0;
            foreach (var item in lines)
            {
                sum += new Round(item).Points();
                sum2 += new Round(item, true).Points();
            }

            Console.WriteLine($"if everything goes exactly according to the strategy guide I will have: {sum} points.");
            Console.WriteLine($"(2) if everything goes exactly according to the strategy guide I will have: {sum2} points.");
            Console.ReadLine();
        }
    }
}
