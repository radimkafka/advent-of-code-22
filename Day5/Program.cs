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
        var cargoInput = "";
        var readingMoves = false;
        var cargo = default(Cargo);
        var cargo9001 = default(Cargo);

        foreach (var line in lines)
        {
            if (line == "")
            {
                cargo = Cargo.Parse(cargoInput);
                cargo9001 = Cargo.Parse(cargoInput);
                readingMoves = true;
                continue;
            }

            if (readingMoves)
            {
                var move = Move.Parse(line);
                cargo?.MoveByCrateMover9000(move);
                cargo9001?.MoveByCrateMover9001(move);
            }
            else
            {
                cargoInput += line + Environment.NewLine;
            }
        }
        Console.WriteLine($"On the top of the stacks are: {cargo.GetTopItems()}");
        Console.WriteLine($"On the top of the stacks sorted by CrateMover 9001 are: {cargo9001.GetTopItems()}");
        //Console.WriteLine($"{overlapped} assignment pairs overlappes.");
        Console.ReadLine();
    }
}

class Cargo
{
    public List<Stack<string>> Stacks { get; set; }
    public Cargo(List<Stack<string>> stacks)
    {
        Stacks = stacks;
    }
    internal static Cargo Parse(string input)
    {
        var stacks = new List<Stack<string>>();
        var rows = input.Split(Environment.NewLine);
        var indexingLine = rows[^2];
        var indexes = indexingLine.Split(" ").Where(a => a != "").Select(a => (column: int.Parse(a) - 1, itemIndex: indexingLine.IndexOf(a))).ToList();
        indexes.ForEach(_ => stacks.Add(new Stack<string>()));

        for (int i = rows.Length - 3; i >= 0; i--)
        {
            foreach (var index in indexes)
            {
                var item = rows[i][index.itemIndex].ToString();
                if (item != "" && item != " ") stacks[index.column].Push(item);
            }
        }

        return new Cargo(stacks);
    }

    public void MoveByCrateMover9000(Move move)
    {
        for (int i = 0; i < move.Amount; i++)
        {
            Stacks[move.To - 1].Push(Stacks[move.From - 1].Pop());
        }
    }

    public void MoveByCrateMover9001(Move move)
    {
        var moved = new Stack<string>();
        for (int i = 0; i < move.Amount; i++)
        {
            moved.Push(Stacks[move.From - 1].Pop());
        }

        while (moved.Any())
        {
            Stacks[move.To - 1].Push(moved.Pop());
        }
    }

    public string GetTopItems()
    {
        var items = "";
        foreach (var stack in Stacks)
        {
            items += stack.FirstOrDefault() ?? " ";
        }

        return items;
    }
}

internal record Move(int Amount, int From, int To)
{
    internal static Move Parse(string input)
    {
        var parts = input.Split(" ");
        if (parts.Length < 6) return new(0, 0, 0);

        var validAmount = int.TryParse(parts[1], out var amount);
        var validFrom = int.TryParse(parts[3], out var from);
        var validTo = int.TryParse(parts[5], out var to);

        if (!validAmount || !validFrom || !validTo) return new(0, 0, 0);

        return new(amount, from, to);
    }
}