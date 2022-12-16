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
        var input = File.ReadAllLines(filePath);
        var map = new char[input.Length, input[0].Length];
        var start = new Position(0, 0);
        var end = new Position(0, 0);
        for (int y = 0; y < input.Length; y++)
        {
            for (int x = 0; x < input[y].Length; x++)
            {
                map[y, x] = input[y][x];
                if (input[y][x] == 'S') start = new Position(x, y);
                else if (input[y][x] == 'E') end = new Position(x, y);
            }
        }


        Console.ReadLine();
    }
}

record Position(int X, int Y);

class Node
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public char Value { get; private set; }

    public List<Node> Nodes { get; private set; } = new List<Node>();

    public Node(Position position, char value)
    {
        X = position.X;
        Y = position.Y;
        Value = value;
    }
}

class Graph
{
    public int[,] Data { get; set; }

    public Position Root { get; set; }

    public List<Node> Nodes { get; set; }

    public void Init(int[,]data, Position root)
    {
        Root = root;
        Data = data;


    }
}

internal static class Extensions
{
    public static int TryParse(this string input)
    {
        int.TryParse(input, out int result);
        return result;
    }

    public static void FindPath(char[,] map, Position start, Position end)
    {

    }

}
