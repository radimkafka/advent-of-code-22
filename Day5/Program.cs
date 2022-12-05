internal class Program
{
    private static void Main(string[] args)
    {
        //var filePath = "../../../input.txt";
        var filePath = "../../../input.basic.txt";
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File does not exist!");
            return;
        }
        var lines = File.ReadAllLines(filePath);
        var readingMoves = false;

        foreach (var line in lines)
        {
            if (line == "")
            {
                readingMoves = true;
                continue;
            }

            if (readingMoves)
            {
                var move = Move.Parse(line);
            }
            else
            {

            }
        }
        Console.WriteLine($"assignment pairs fully overlappes.");
        //Console.WriteLine($"{overlapped} assignment pairs overlappes.");
        Console.ReadLine();
    }
}

record Cargo
{
    public Cargo(string input)
    {

    }


    public List<Stack<string>> Stacks { get; set; }

    public void Move(Move move)
    {

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

internal static class Day5
{
}
