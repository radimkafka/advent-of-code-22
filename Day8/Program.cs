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
        var input = File.ReadAllLines(filePath);
        var grid = Parse(input);
        var count = VisibleTreeCount(grid);
        Console.WriteLine($"Number of visible trees is: {count}, Edge: {Edge}, Inside: {count - Edge}");
        Console.WriteLine($"Highest scenic score is: {GetHighestTreeScenicScore(grid)}");

        Console.ReadLine();
    }

    private static int[][] Parse(string[] input)
    {
        var parsed = new List<List<int>>();
        foreach (var item in input)
        {
            parsed.Add(new List<int>());
            foreach (var tree in item)
            {
                if (int.TryParse(tree.ToString(), out int height)) parsed.Last().Add(height);
            }
        }
        return parsed.Select(x => x.ToArray()).ToArray();
    }

    private static int Edge = 0;
    private static bool IsVisible(int[][] grid, int x, int y)
    {
        if (x <= 0 || y <= 0 || x >= grid.Length - 1 || y >= grid[x].Length - 1)
        {
            Edge++;
            return true;
        }


        var (rowLeft, rowRight, colUp, colDown) = GetTreesToTheEdge(grid, x, y);
        var value = grid[y][x];
        var visibleFromLeft = !rowLeft.Any(a => a >= value);
        var visibleFromRight = !rowRight.Any(a => a >= value);
        var visibleFromTop = !colUp.Any(a => a >= value);
        var visibleFromBottom = !colDown.Any(a => a >= value);

        var visible = visibleFromLeft || visibleFromRight || visibleFromTop || visibleFromBottom;
        //Console.WriteLine($"{x},{y}:{!visible}");
        return visible;
    }

    private static (List<int> rowLeft, List<int> rowRight, List<int> colUp, List<int> colDown) GetTreesToTheEdge(int[][] grid, int x, int y)
    {
        var rowLeft = new List<int>();
        var rowRight = new List<int>();
        var colUp = new List<int>();
        var colDown = new List<int>();


        for (int i = 0; i < grid.Length; i++)
        {
            if (i < y) colUp.Add(grid[i][x]);
            else if (i > y) colDown.Add(grid[i][x]);
        }

        for (int i = 0; i < grid[y].Length; i++)
        {
            if (i < x) rowLeft.Add(grid[y][i]);
            else if (i > x) rowRight.Add(grid[y][i]);
        }
        return (rowLeft, rowRight, colUp, colDown);
    }

    private static int VisibleTreeCount(int[][] grid)
    {
        var visibleTrees = 0;

        grid.ForeachTree((x, y) =>
        {
            if (IsVisible(grid, x, y)) visibleTrees++;
        });


        return visibleTrees;
    }

    private static int CalculateTreeScenicScore(int left, int right, int top, int down) => left * right * top * down;

    private static int GetViewingDistance(int value, List<int> treesAhead)
    {
        var distance = 0;
        foreach (var item in treesAhead)
        {
            distance++;
            if (item >= value) break;
        }

        return distance;
    }

    private static int GetHighestTreeScenicScore(int[][] grid)
    {
        var highestTreeScenicScore = 0;
        grid.ForeachTree((x, y) =>
        {
            var value = grid[y][x];
            var (rowLeft, rowRight, colUp, colDown) = GetTreesToTheEdge(grid, x, y);
            rowLeft.Reverse();
            colUp.Reverse();

            var left = GetViewingDistance(value, rowLeft);
            var right = GetViewingDistance(value, rowRight);
            var top = GetViewingDistance(value, colUp);
            var down = GetViewingDistance(value, colDown);

            var score = CalculateTreeScenicScore(left, right, top, down);

            if (score > highestTreeScenicScore) highestTreeScenicScore = score;

        });

        return highestTreeScenicScore;
    }
}

internal static class Extensions
{
    internal static void ForeachTree(this int[][] grid, Action<int, int> action)
    {
        for (int y = 0; y < grid.Length; y++)
        {
            for (int x = 0; x < grid[y].Length; x++)
            {
                action(y, x);
            }
        }
    }
}
