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
        var fullyOverlapped = 0;
        var overlapped = 0;

        foreach (var line in lines)
        {
            var (first, second) = Day4.ParseLine(line); ;
            var pair = new AssignmentPair(AssignedSection.Parse(first), AssignedSection.Parse(second), line);
            if (pair.FullyOverlappes())
            {
                //Console.WriteLine(pair.Input);
                fullyOverlapped++;
            }
            if (pair.Overlappes())
            {
                //Console.WriteLine(pair.Input);
                overlapped++;
            }
        }
        Console.WriteLine($"{fullyOverlapped} assignment pairs fully overlappes.");
        Console.WriteLine($"{overlapped} assignment pairs overlappes.");
        Console.ReadLine();
    }
}

record AssignedSection(int From, int To)
{
    internal static AssignedSection Parse(string section)
    {
        var (from, to) = Day4.ParseSection(section);
        return new AssignedSection(from, to);
    }

    internal int SectionsCount() => To - From + 1;

    internal bool FullyOverlappes(AssignedSection section)
    {
        var largerSection = SectionsCount() > section.SectionsCount() ? this : section;
        var otherOne = largerSection.Equals(this) ? section : this;
        int[] largerSeq = Enumerable.Range(largerSection.From, largerSection.SectionsCount()).ToArray();
        int[] otherSeq = Enumerable.Range(otherOne.From, otherOne.SectionsCount()).ToArray();
        return otherSeq.All(a => largerSeq.Contains(a));
    }

    internal bool Overlappes(AssignedSection section)
    {
        var largerSection = SectionsCount() > section.SectionsCount() ? this : section;
        var otherOne = largerSection.Equals(this) ? section : this;
        int[] largerSeq = Enumerable.Range(largerSection.From, largerSection.SectionsCount()).ToArray();
        int[] otherSeq = Enumerable.Range(otherOne.From, otherOne.SectionsCount()).ToArray();
        return otherSeq.Any(a => largerSeq.Contains(a));
    }
}

record AssignmentPair(AssignedSection First, AssignedSection Second, string Input)
{
    internal bool FullyOverlappes() => First.FullyOverlappes(Second);
    internal bool Overlappes() => First.Overlappes(Second);
}

internal static class Day4
{
    internal static (string first, string second) ParseLine(string input)
    {
        var pair = input.Split(',');
        if (pair.Length != 2) throw new Exception($"{input} doest not contain pair!");
        return (pair[0], pair[1]);
    }

    internal static (int from, int to) ParseSection(string section)
    {
        var range = section.Split("-");
        if (range.Length != 2) throw new Exception($"{section} doest not contain section!");
        int.TryParse(range[0], out int from);
        int.TryParse(range[1], out int to);
        return (from, to);
    }
}
