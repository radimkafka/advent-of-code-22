using System.Drawing;
using System.Text.RegularExpressions;

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
        var item = Item.Parse(lines);


        Console.WriteLine($"Total size is: {ItemCrawler.GetAllDirsOfSize(item, 100000)}");

        var sizeToDelete = 30000000-(70000000- item.TotalSize());
        Console.WriteLine($"Folder to delete: {ItemCrawler.GetClosestDirectoryOfSize(item, sizeToDelete)}");
        Console.ReadLine();
    }
}

internal enum ItemType
{
    Dir,
    File
}

internal class Item
{
    public required ItemType Type { get; init; }
    public int? Size { get; init; } = null;
    public required string Name { get; init; }
    public List<Item> Content { get; } = new List<Item>();
    public Item? Parent { get; set; }

    internal static Item Parse(string[] result)
    {
        var root = new Item { Type = ItemType.Dir, Name = "/" };
        List<Command> commands = new();

        var actualItem = root;
        foreach (var item in result)
        {
            if (item.StartsWith('$'))
            {
                var (type, param) = ItemParser.GetCommand(item);
                commands.Add(new(type, param, new List<string>()));
            }
            else
                commands.Last().Outut.Add(item);
        }

        foreach (var item in commands)
        {
            switch (item.Type)
            {
                case CommandType.Cd:
                    var nextItem = actualItem;
                    if (item.Param == "/") nextItem = actualItem;
                    else if (item.Param == "..") nextItem = actualItem.Parent;
                    else nextItem = actualItem.Content.FirstOrDefault(a => a.Name == item.Param);

                    actualItem = nextItem;
                    break;
                case CommandType.Ls:
                    actualItem.Content.AddRange(item.Outut.Select(a => ItemParser.ParseOutput(a, actualItem)));
                    break;
                case CommandType.None:
                default:
                    break;
            }
        }

        return root;
    }

    internal int TotalSize()
    {
        if (Type == ItemType.File) return Size ?? 0;
        return Content.Aggregate(0, (acc, item) => acc += item.TotalSize());
    }

    internal static string Print()
    {
        var output = "";

        return output;
    }
}

internal enum CommandType
{
    None,
    Cd,
    Ls
}

internal record Command(CommandType Type, string? Param, List<string> Outut);

internal static class ItemParser
{
    internal static (CommandType type, string? param) GetCommand(string command)
    {
        var parts = command.Substring(2).Split(' ');
        return parts[0] switch
        {
            "cd" => (CommandType.Cd, parts[1]),
            "ls" => (CommandType.Ls, null),
            _ => (CommandType.None, null),
        };
    }

    internal static Item ParseOutput(string command, Item parent)
    {
        var parts = command.Split(' ');
        return (Regex.Match(parts[0], @"^\d*$").Success) ?
            new Item { Size = int.Parse(parts[0]), Name = parts[1], Type = ItemType.File, Parent = parent } :
            new Item { Name = parts[1], Type = ItemType.Dir, Parent = parent };
    }
}

internal static class ItemCrawler
{
    internal static int GetAllDirsOfSize(Item item, int size)
    {
        var dirs = Flatten(item).Where(a => a.Type == ItemType.Dir && a.TotalSize() <= size).ToList();
        return dirs.Aggregate(0, (acc, a) => acc + a.TotalSize());
    }

    internal static int GetClosestDirectoryOfSize(Item item, int size)
    {
        var dirs = Flatten(item)
            .Where(a => a.Type == ItemType.Dir && a.TotalSize() >= size)
            .Select(a => (a.Name, Size: a.TotalSize()))
            .OrderBy(a => a.Size)
            .ToList();

        var dirs2 = Flatten(item)
            .Where(a => a.Type == ItemType.Dir)
            .Select(a => (a.Name, Size: a.TotalSize()))
            .OrderBy(a => a.Size)
            .ToList();

        return dirs.Count == 0 ? 0 : dirs.First().Size;
    }

    internal static List<Item> Flatten(Item item)
    {
        var list = new List<Item> { item };
        list.AddRange(item.Content.Select(Flatten).SelectMany(a => a));
        return list;
    }
}