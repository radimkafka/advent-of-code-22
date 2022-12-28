using System.Drawing;
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
        var map = new Map(input);

        var plan = new int[map.Plan.Length, map.Plan[0].Length];
        for (int y = 0; y < map.Plan.Length; y++)
        {
            for (int x = 0; x < map.Plan[y].Length; x++)
            {
                plan[y, x] = map.Plan[y][x].Value;
            }
        }

        //for (int y = 0; y < plan.GetLength(0); y++)
        //    for (int x = 0; x < plan.GetLength(1); x++)
        //        plan[y, x] = plan[y, x] == 0 || plan[y, x] == 1 ? 0 : 1;


        var stringPlan = new StringBuilder();
        for (int y = 0; y < plan.GetLength(0); y++)
        {
            for (int x = 0; x < plan.GetLength(1); x++)
            {
                stringPlan.Append(plan[y, x]);
            }
            stringPlan.Append(Environment.NewLine);
        }        
        //DijkstraAlg.Dijkstra(plan, 0);
        var result = AStarAlg.AStar(plan, new Point(map.Start.X, map.Start.Y), new Point(map.End.X, map.End.Y));
        Console.ReadLine();
    }

}

class Map
{
    public Node Start { get; set; } = new Node(new(0, 0), 'a');

    public Node End { get; set; } = new Node(new(0, 0), 'a');

    public Node[] AllNodes { get; set; }

    public Node[][] Plan { get; set; }

    public Edge[] Lines { get; set; }


    public Map(string[] input)
    {
        var allNodes = new List<Node>();
        var map = new Node[input.Length][];
        for (int i = 0; i < map.Length; i++)
        {
            map[i] = new Node[input[0].Length];
        }
        for (var y = 0; y < input.Length; y++)
        {
            for (int x = 0; x < input[y].Length; x++)
            {

                map[y][x] = new Node(new(x, y), input[y][x]);
                if (input[y][x] == 'S') Start = new Node(new(x, y), input[y][x]);
                else if (input[y][x] == 'E') End = new Node(new(x, y), input[y][x]);
            }
        }

        Plan = map;
        AllNodes = map.SelectMany(x => x).ToArray();
    }

    public void Init()
    {

    }
}

record struct Position(int X, int Y);

class Node
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public int Value { get; private set; }

    //public List<Node> Nodes { get; private set; } = new List<Node>();

    public Node(Position position, char value)
    {
        X = position.X;
        Y = position.Y;

        Value = value switch
        {
            'S' => 'a',
            'E' => '{',
            _ => value
        } - 97;
    }
}

record Edge(Node From, Node To, int Value);

class Graph
{
    public required int[,] Data { get; init; }

    public required Position Root { get; init; }

    public List<Node> Nodes { get; private set; } = new List<Node>();
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

    public static void Getadjacents(this Node[,] Plan, Position position)
    {

    }

}

static class DijkstraAlg
{
    public static void Example(string[] args)
    {
        // Create a graph with 5 vertices
        int vertices = 5;
        int[,] graph = new int[,]
        {
                { 0, 2, 0, 6, 0 },
                { 2, 0, 3, 8, 5 },
                { 0, 3, 0, 0, 7 },
                { 6, 8, 0, 0, 9 },
                { 0, 5, 7, 9, 0 }
        };

        // Run Dijkstra's algorithm from vertex 0
        Dijkstra(graph, 0);
    }

    public static void Dijkstra(int[,] graph, int source)
    {
        int vertices = graph.GetLength(0);
        int[] distance = new int[vertices];
        bool[] visited = new bool[vertices];

        // Initialize all distances as infinity and visited as false
        for (int i = 0; i < vertices; i++)
        {
            distance[i] = int.MaxValue;
            visited[i] = false;
        }

        // Set the distance to the source vertex as 0
        distance[source] = 0;

        // Find the shortest path for all vertices
        for (int count = 0; count < vertices - 1; count++)
        {
            // Pick the minimum distance vertex that is not visited
            int min = int.MaxValue;
            int minIndex = -1;
            for (int i = 0; i < vertices; i++)
            {
                if (!visited[i] && distance[i] <= min)
                {
                    min = distance[i];
                    minIndex = i;
                }
            }

            // Mark the picked vertex as visited
            visited[minIndex] = true;

            // Update the distance values of the adjacent vertices of the picked vertex
            for (int i = 0; i < vertices; i++)
            {
                // Update distance[i] only if it is not in visited, there is an edge from
                // minIndex to i, and total weight of path from source to i through minIndex
                // is smaller than the current value of distance[i]
                if (!visited[i] && graph[minIndex, i] != 0 && distance[minIndex] != int.MaxValue && distance[minIndex] + graph[minIndex, i] < distance[i])
                {
                    distance[i] = distance[minIndex] + graph[minIndex, i];
                }
            }
        }

        // Print the shortest distances to all vertices
        Console.WriteLine("Vertex\tDistance");
        for (int i = 0; i < vertices; i++)
        {
            Console.WriteLine("{0}\t{1}", i, distance[i]);
        }
    }
}

class AStarAlg
{
    public static void Example(string[] args)
    {
        // Create a map of the grid
        int rows = 5;
        int cols = 5;
        int[,] map = new int[5, 5]
        {
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 }
        };

        // Set the start and goal positions
        Point start = new Point(0, 0);
        Point goal = new Point(4, 4);

        // Run the A* search algorithm
        List<Point> path = AStar(map, start, goal);

        // Print the path
        Console.WriteLine("Path: ");
        foreach (Point p in path)
        {
            Console.WriteLine("({0}, {1})", p.x, p.y);
        }
    }

    public static List<Point> AStar(int[,] map, Point start, Point goal)
    {
        // Create a set to store the visited nodes
        HashSet<Point> closedSet = new HashSet<Point>();

        // Create a set to store the discovered nodes that are not evaluated yet
        HashSet<Point> openSet = new HashSet<Point> { start };

        // Create a map to store the cameFrom value for each node
        Dictionary<Point, Point> cameFrom = new Dictionary<Point, Point>();

        // Create a map to store the gScore value for each node
        Dictionary<Point, int> gScore = new Dictionary<Point, int>();
        gScore[start] = 0;

        // Create a map to store the fScore value for each node
        Dictionary<Point, int> fScore = new Dictionary<Point, int>();
        fScore[start] = HeuristicCostEstimate(start, goal);

        // Loop through the open set until it is empty
        while (openSet.Count > 0)
        {
            // Find the node with the lowest fScore value
            Point current = null;
            int min = int.MaxValue;
            foreach (Point p in openSet)
            {
                if (fScore[p] < min)
                {
                    min = fScore[p];
                    current = p;
                }
            }

            // If the current node is the goal, construct the path and return it
            if (current.x == goal.x && current.y == goal.y)
            {
                return ReconstructPath(cameFrom, current);
            }

            // Remove the current node from the open set and add it to the closed set
            openSet.Remove(current);
            closedSet.Add(current);

            // Get the neighbors of the current node
            List<Point> neighbors = GetNeighbors(map, current);

            // Loop through the neighbors
            foreach (Point neighbor in neighbors)
            {
                // Skip the neighbor if it is in the closed set
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }                // Calculate the tentative gScore for the neighbor
                int tentativeGScore = gScore[current] + DistanceBetween(current, neighbor);

                // Add the neighbor to the open set if it is not already in it
                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
                // Otherwise, check if the tentative gScore is lower than the current gScore for the neighbor
                else if (tentativeGScore >= gScore[neighbor])
                {
                    // If the gScore is not lower, skip the neighbor
                    continue;
                }

                // Update the cameFrom value for the neighbor
                cameFrom[neighbor] = current;

                // Update the gScore and fScore values for the neighbor
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, goal);
            }
        }

        // If the open set is empty and the goal was not reached, return an empty path
        return new List<Point>();
    }

    // Helper function to calculate the distance between two points
    static int DistanceBetween(Point a, Point b)
    {
        // In this example, the distance is the Euclidean distance
        int dx = a.x - b.x;
        int dy = a.y - b.y;
        return (int)Math.Sqrt(dx * dx + dy * dy);
    }

    // Helper function to calculate the heuristic cost estimate between two points
    static int HeuristicCostEstimate(Point a, Point b)
    {
        // In this example, the heuristic is the Manhattan distance
        int dx = Math.Abs(a.x - b.x);
        int dy = Math.Abs(a.y - b.y);
        return dx + dy;
    }

    // Helper function to get the neighbors of a point
    static List<Point> GetNeighbors(int[,] map, Point p)
    {
        List<Point> neighbors = new List<Point>();
        int[,] dirs = new int[,] { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };
        for (int i = 0; i < dirs.GetLength(0); i++)
        {
            int x = p.x + dirs[i, 0];
            int y = p.y + dirs[i, 1];
            if (x >= 0 && x < map.GetLength(0) && y >= 0 && y < map.GetLength(1) && map[x, y] == 0)
            {
                neighbors.Add(new Point(x, y));
            }
        }
        return neighbors;
    }

    // Helper function to reconstruct the path from the cameFrom map
    static List<Point> ReconstructPath(Dictionary<Point, Point> cameFrom, Point current)
    {
        List<Point> path = new List<Point> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }
        path.Reverse();
        return path;
    }
}
// Point class to represent a point on the grid
class Point
{
    public int x;
    public int y;

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override bool Equals(object obj)
    {
        Point p = obj as Point;
        return p != null && p.x == x && p.y == y;
    }

    public override int GetHashCode()
    {
        return x ^ y;
    }
}
