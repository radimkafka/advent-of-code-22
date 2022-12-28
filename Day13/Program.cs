namespace Day1
{
    class Program
    {
        static void Main(string[] args)
        {
            //var filePath = "../../../input.basic.txt";
            var filePath = "../../../input.txt";
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File does not exist!");
                return;
            }

            var pairsString = File.ReadAllText(filePath)
                .Split(Environment.NewLine + Environment.NewLine)
                .Select(a => a.Split(Environment.NewLine));

            //var pairs = pairsString.Select(a => new Pair(Packet.Parse(a[0]), Packet.Parse(a[1]), a[0], a[1])).ToList();
            var pairs = pairsString.Select(a => new Pair(ValueOrArray.Parse(a[0]), ValueOrArray.Parse(a[1]), a[0], a[1])).ToList();
            var sum = 0;
            for (int i = 0; i < pairs.Count; i++)
            {
                // TODO Pair 7 dělá brikule - když nesedí počet zanořených kolekcí
                if (pairs[i].IsLeftSmaller())
                {
                    sum += i + 1;
                    Console.WriteLine(i + 1);
                }
            }
            //var count = pairs.Select((a, i) => a.IsLeftSmaller() ? i : 0).ToArray();
            Console.WriteLine();
            Console.WriteLine(sum);
            Console.ReadLine();
        }
    }

    record Pair(ValueOrArray Left, ValueOrArray Right, string LeftString, string RightString)
    {
        public bool IsLeftSmaller()
        {
            return Left.IsSmaller(Right);
        }
    }

    class ValueOrArray
    {
        public int? Value { get; set; }

        public List<ValueOrArray>? Items { get; set; }

        public bool IsValue { get; private set; }

        public bool IsArtificial { get; set; }

        public ValueOrArray(int? value)
        {
            Value = value;
            IsValue = true;
        }

        public ValueOrArray(List<ValueOrArray> items, bool isArtificial = false)
        {
            Items = items;
            IsArtificial = isArtificial;
        }

        public static ValueOrArray Parse(string input)
        {
            ValueOrArray? item;
            if (input.StartsWith("[") && input.EndsWith("]"))
            {
                var list = input[1..^1].SplitGroups().Select(a => Parse(a)).ToList();
                item = new ValueOrArray(list);
            }
            else
            {
                item = new ValueOrArray(input.TryParseNull());
            }
            return item;
        }

        internal bool IsSmaller(ValueOrArray right)
        {
            // TODO null
            if (IsValue && right.IsValue) return Value <= right.Value;
            if (!IsValue && !right.IsValue)
            {
                if (IsArtificial && right.Items.Count == 0) return true;
                if (right.IsArtificial && Items.Count == 0) return true;
                if (right.IsArtificial || IsArtificial)
                {
                    return Items[0].IsSmaller(right.Items[0]);
                }

                var length = Items!.Count > right.Items!.Count ? Items.Count : right.Items.Count;
                for (int i = 0; i < length; i++)
                {
                    if (i >= Items.Count) return true;
                    if (i >= right.Items.Count) return false;
                    if (!Items[i].IsSmaller(right.Items[i])) return false;
                }
                return true;
            }

            if (IsValue) return new ValueOrArray(new List<ValueOrArray>() { this }, true).IsSmaller(right);

            return IsSmaller(new ValueOrArray(new List<ValueOrArray>() { right }, true));
        }
    }

    internal static class Extensions
    {
        public static int TryParse(this string input)
        {
            int.TryParse(input, out int result);
            return result;
        }

        public static int? TryParseNull(this string input)
        {
            if (int.TryParse(input, out int result))
            {
                return result;
            }
            return null;
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

        public static string[] SplitGroups(this string input)
        {
            var nestedLevel = 0;
            var start = 0;
            var parts = new List<string>();
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '[') nestedLevel++;
                else if (input[i] == ']') nestedLevel--;
                else if (input[i] == ',' && nestedLevel == 0)
                {
                    parts.Add(input.Substring(start, i - start));
                    start = i + 1;
                }
            }
            var last = input.Substring(start);
            if (!string.IsNullOrEmpty(last)) parts.Add(last);

            return parts.ToArray();
        }
    }
}
