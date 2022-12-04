internal class Program
{
    private static void Main(string[] args)
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

        var group = new List<Ruck>();
        var sumOfBadges = 0;


        foreach (var line in lines)
        {
            var comp = GetCompartments(line);
            var duplicates = comp?.GetDuplicatedItem();
            sum += GetItemsSumPriority(duplicates);

            group.Add(comp);
            if (group.Count == 3)
            {
                sumOfBadges += GetItemsSumPriority(GetItemOfGroup(group));
                group.Clear();
            }
        }

        Console.WriteLine(sum);
        Console.WriteLine($"Sum of badges: {sumOfBadges}");
        Console.ReadLine();
    }

    static string? GetItemOfGroup(List<Ruck> group)
    {
        if (group.Count < 2) return null;
        var firstRuck = group.First().FirstCompartment + group.First().SecondCompartment;
        var rest = group.Skip(1).Select(a => a.FirstCompartment + a.SecondCompartment).ToArray();

        return new string(firstRuck.Where(a => rest.All(b => b.Contains(a))).Distinct().ToArray());
    }

    static Ruck GetCompartments(string ruckContent)
    {
        return new Ruck(ruckContent.Substring(0, ruckContent.Length / 2), ruckContent.Substring(ruckContent.Length / 2));
    }

    static int GetItemsSumPriority(string? items)
    {
        if (items is null) return 0;
        var sum = 0;
        foreach (var item in items)
        {
            var number = (int)item;
            if (number is >= 97 and <= 122) sum += number - 96;
            else if (number is >= 65 and <= 90) sum += number - 64 + 26;
        }
        return sum;
    }
}

record Ruck(string FirstCompartment, string SecondCompartment);

internal static class CompartmentExtensions
{
    internal static string GetDuplicatedItem(this Ruck comp)
    {
        var duplicated = comp.FirstCompartment.Where(a => comp.SecondCompartment.Contains(a)).Distinct().ToArray();
        return new string(duplicated);
    }
}