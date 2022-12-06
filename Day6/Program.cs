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
        var signals = File.ReadAllText(filePath);

        var nonRepeatingChars = new List<Signal>();
        var actualChar = default(Signal);
        for (int i = 0; i < signals.Length; i++)
        {
            actualChar = nonRepeatingChars.FirstOrDefault(a => a.character == signals[i]);
            if (actualChar != null)
            {
                var index = nonRepeatingChars.IndexOf(actualChar);
                nonRepeatingChars = nonRepeatingChars.Skip(index + 1).ToList();
            }
            nonRepeatingChars.Add(new(signals[i], i));

            if (nonRepeatingChars.Count == 14) { break; }
        }
        Console.WriteLine($"First non repeating char is at: {nonRepeatingChars.Last().position+1}");        
        Console.ReadLine();
    }
}

internal record Signal(char character, int position);