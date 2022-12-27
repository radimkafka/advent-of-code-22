using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;

namespace Day1
{
    class Program
    {
        static void Main(string[] args)
        {
            var filePath = "../../../input.basic.txt";
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File does not exist!");
                return;
            }

            var pairsString = File.ReadAllText(filePath)
                .Split(Environment.NewLine + Environment.NewLine)
                .Select(a => a.Split(Environment.NewLine));

            var pairs = pairsString.Select(a => new Pair(Packet.Parse(a[0]), Packet.Parse(a[1]))).ToList();
            var sum = 0;
            for (int i = 0; i < pairs.Count; i++)
            {               
                // TODO Pair 7 dělá brikule - když nesedí počet zanořených kolekcí
                if (pairs[i].IsLeftSmaller())
                {
                    sum += i+1;
                }
            }
            //var count = pairs.Select((a, i) => a.IsLeftSmaller() ? i : 0).ToArray();
            Console.WriteLine(sum);
            Console.ReadLine();
        }
    }

    record Pair(Packet Left, Packet Right)
    {
        public bool IsLeftSmaller()
        {
            for (int i = 0; i < Left.Values.Length; i++)
            {
                if (!Left.IsSmaller(Right, i)) return false;
            }
            for (int i = 0; i < Left.Children.Length; i++)
            {
                if (!Left.Children[i].IsSmaller(Right, i)) return false;
            }
            return true;
        }
    }

    record PacketValue(int Position, int Value);

    class Packet
    {
        public int Position { get; set; }

        public PacketValue[] Values { get; set; }

        public Packet[] Children { get; set; }

        public static Packet Parse(string input)
        {
            var tempInput = input;
            var parts = new List<string>();
            while (tempInput.Contains('['))
            {
                var indexOfStart = tempInput.LastIndexOf('[');
                var indexOfEnd = tempInput.ClosestIndexOf(']', indexOfStart) + 1;
                parts.Add(tempInput.Substring(indexOfStart, indexOfEnd - indexOfStart));
                tempInput = tempInput.Replace(parts.Last(), $"*{parts.Count - 1}*");
            }
            return ParsePart(parts.Last(), parts.Take(parts.Count - 1).ToList(), 0);
        }

        public bool IsSmaller(Packet packet, int index)
        {
            var valueAtIndex = ValueAtIndex(index);
            var packetValueAtIndex = packet.ValueAtIndex(index);

            if (valueAtIndex > packetValueAtIndex) return false;
            return true;
        }

        public int ValueAtIndex(int index)
        {
            var value = Values.FirstOrDefault(a => a.Position == index);
            if (value != null) return value.Value;

            return Children.FirstOrDefault(a => a.Position == index)?.ValueAtIndex(0) ?? -1;
        }

        private static Packet ParsePart(string input, List<string> children, int position)
        {
            var parts = input.Substring(1, input.Length - 2).Split(',');
            var packet = new Packet()
            {
                Position = position
            };

            var values = new List<PacketValue>();
            var childrenPackets = new List<Packet>();

            for (int i = 0; i < parts.Length; i++)
            {
                var item = parts[i];
                if (item.StartsWith('*'))
                {
                    childrenPackets.Add(ParsePart(children[item.IndexInChildren()], children, i));
                }
                else
                {
                    values.Add(new(i, item.TryParse()));
                }
            }

            packet.Values = values.ToArray();
            packet.Children = childrenPackets.ToArray();

            return packet;
        }
    }

    internal static class Extensions
    {
        public static int TryParse(this string input)
        {
            int.TryParse(input, out int result);
            return result;
        }

        public static int ClosestIndexOf(this string input, char value, int index)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == value && i > index) return i;
            }
            return -1;
        }

        public static int IndexInChildren(this string input)
        {
            return input.Replace("*", "").TryParse();
        }



    }
}
