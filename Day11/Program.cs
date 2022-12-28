using System.Net;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var filePath = "../../../input.basic.txt";
        //var filePath = "../../../input.txt";
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File does not exist!");
            return;
        }
        var input = File.ReadAllText(filePath);

        var monkeysInput = input.Split(Environment.NewLine + Environment.NewLine);
        var moneys = new List<Monkey>();
        foreach (var item in monkeysInput)
        {
            moneys.Add(Monkey.Parse(item));
        }

        for (int r = 0; r < 10000; r++)
        {
            if (r is 1 or 20 or 1000)
            {
                moneys.Select(a => a.InspectCount).ToList()
                .ForEach(a =>
                {
                    Console.WriteLine(a.ToString());
                });
                Console.WriteLine();
            }
            foreach (var monkey in moneys)
            {
                for (int i = 0; i < monkey.ItemsWorryLevel.Count; i++)
                {
                    var (nextMoney, worryLevel) = monkey.InspectItem(i, false);
                    moneys.FirstOrDefault(a => a.Number == nextMoney)?.ItemsWorryLevel?.Add(new(worryLevel));

                    //if (r < 2) Console.WriteLine($"Item with worry level {worryLevel} is thrown to monkey {nextMoney}.");
                    //if (r < 2) Console.WriteLine($"Item with worry level {worryLevel} is thrown to monkey {nextMoney}.");
                }
                monkey.ItemsWorryLevel.Clear();
            }
        }
        var top2 = moneys.Select(a => a.InspectCount).OrderByDescending(a => a).Take(2).ToList();
        top2.ForEach(a =>
        {
            Console.WriteLine(a.ToString());
        });

        Console.WriteLine($"Monkey business: {top2.Aggregate((uint)1, (a, b) => a * b)}");
        Console.ReadLine();
    }
}

record Item(int WorryLevel);


enum OperationType
{
    None,
    Multiply,
    Add,

}
record Operation(OperationType Type, int Amount)
{
    public static Operation Parse(string input)
    {
        var split = input.Split(' ');
        if (split.Length < 8) return new(OperationType.None, 0);
        var type = split[6] switch
        {
            "+" => OperationType.Add,
            "*" => OperationType.Multiply,
            _ => OperationType.None,
        };

        int.TryParse(split[7], out int amount);
        if (split.LastOrDefault() == "old") amount = -1;

        return new(type, amount);
    }

    public int Calculate(int value)
    {
        var newValue = value;
        var amount = Amount == -1 ? newValue : Amount;
        if (amount > int.MaxValue / 10_000_000)
        {
            var devide = Extensions.GetGCDBySubtraction(amount, newValue);
            newValue /= devide;
            amount /= devide;
        }

        return Type switch
        {
            OperationType.Multiply => amount * newValue,
            OperationType.Add => amount + newValue,
            _ => 0
        };
    }
}

class Monkey
{
    public required List<Item> ItemsWorryLevel { get; init; } = new List<Item>();
    public required Operation Operation { get; init; }
    public required int Number { get; init; }
    public required (int ifMonkey, int elseMonkey) Destination { get; init; }
    public required int Test { get; init; }
    public uint InspectCount { get; set; }

    public static Monkey Parse(string input)
    {
        var rows = input.Split(Environment.NewLine);
        var inputSplit = rows[0].Split(' ');
        var number = inputSplit[1].Replace(":", "").TryParse();
        var itemsInputSplit = rows[1].Split(" ").Skip(4).ToArray().Select(a =>
        new Item(a.Replace(",", "").TryParse())).ToList();
        var operation = Operation.Parse(rows[2]);

        return new()
        {
            Operation = operation,
            ItemsWorryLevel = itemsInputSplit,
            Number = number,
            Test = rows[3].Split(" ")[5].TryParse(),
            Destination = (rows[4].Split(" ").Last().TryParse(), rows[5].Split(" ").Last().TryParse())
        };
    }

    public (int, int) InspectItem(int position, bool reduceWorryLevel)
    {
        if (position < ItemsWorryLevel.Count)
        {
            var item = ItemsWorryLevel[position];
            if (item is null)
            {
                return default;
            }
            InspectCount++;
            var result = Operation.Calculate(item?.WorryLevel ?? 0) / (reduceWorryLevel ? 3 : 1);
            return (result % Test == 0 ? Destination.ifMonkey : Destination.elseMonkey, result);
        }
        return default;
    }
}

internal static class Extensions
{
    public static int TryParse(this string input)
    {
        int.TryParse(input, out int result);
        return result;
    }

    public static int GetGCDBySubtraction(int value1, int value2)
    {
        while (value1 != 0 && value2 != 0)
        {
            if (value1 > value2)
                value1 -= value2;
            else
                value2 -= value1;
        }
        return Math.Max(value1, value2);
    }
}
