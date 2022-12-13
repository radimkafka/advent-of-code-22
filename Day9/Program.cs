using System.Net.Http.Headers;

internal class Program
{
    private static void Main(string[] args)
    {
        //Run("../../../input.txt", 1); // 6018
        //Run("../../../input.basic.txt", 1); // 13
        Run("../../../input.txt", 9); //1
        //Run("../../../input.basic2.txt", 9);
        Console.ReadLine();
    }

    private static void Run(string filePath, int tailLength)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File does not exist!");
            return;
        }
        var input = File.ReadAllLines(filePath);
        var bridge = new RopeBridge() { TailLength = tailLength };
        foreach (var line in input)
        {
            Move? move = Move.Parse(line);
            bridge.MakeMove(move);
            //var str = bridge.ToGridString();
        }
        // když je 0 tak je 1 bo navštívil konec původní pozici
        Console.WriteLine($"Tail visited: {bridge.TailEndPositions.Distinct().Count()} positions");
    }
}

enum Direction { None, Left, Right, Up, Down };

internal record Move(Direction Direction, int Amount)
{
    internal static Move Parse(string input)
    {
        var parts = input.Split(' ');
        var direction = parts[0] switch
        {
            "L" => Direction.Left,
            "R" => Direction.Right,
            "U" => Direction.Up,
            "D" => Direction.Down,
            _ => Direction.None,
        };

        var _ = int.TryParse(parts[1], out int amount);
        return new Move(direction, amount);
    }
}

internal record Position(int X, int Y)
{
    internal bool IsAdjacent(Position posi)
    {
        return Math.Abs(X - posi.X) <= 1 && Math.Abs(Y - posi.Y) <= 1;
    }

    internal Position Move(Move move)
    {
        var (nextHeadX, nextHeadY) = this;
        switch (move.Direction)
        {
            case Direction.Left:
                nextHeadX -= move.Amount;
                break;
            case Direction.Right:
                nextHeadX += move.Amount;
                break;
            case Direction.Up:
                nextHeadY += move.Amount;
                break;
            case Direction.Down:
                nextHeadY -= move.Amount;
                break;
            default:
                break;
        }
        return new(nextHeadX, nextHeadY);
    }

    internal Position MoveOne(Direction direction)
    {
        return Move(new(direction, 1));
    }

    internal Position Follow(Position position)
    {
        var (nextHeadX, nextHeadY) = this;
        var distanceX = X - position.X;
        var distanceY = Y - position.Y;

        var absDistanceX = Math.Abs(distanceX);
        var absDistanceY = Math.Abs(distanceY);

        absDistanceX = absDistanceX == 0 ? 0 : absDistanceX - 1;
        absDistanceY = absDistanceY == 0 ? 0 : absDistanceY - 1;


        if (distanceX == 0 || distanceY == 0)
        {
            return new(X + absDistanceX * (distanceX > 0 ? -1 : 1), Y + absDistanceY * (distanceY > 0 ? -1 : 1));
        }
        else
        {
            return new(nextHeadX + (distanceX > 0 ? -1 : 1), nextHeadY + (distanceY > 0 ? -1 : 1));
        }
    }

    internal Position Distance(Position position)
    {
        return new Position(Math.Abs(X - position.X), Math.Abs(Y - position.Y));
    }
}


class RopeBridge
{
    public required int TailLength { get; init; }

    public Position Head { get; private set; } = new(0, 0);

    public List<Position> Tails { get; private set; } = new List<Position>();

    public List<Position> TailEndPositions { get; } = new();

    public List<Direction> Moves { get; } = new();

    public void MakeMove(Move move)
    {
        for (int i = 0; i < move.Amount; i++)
        {
            MakeMove(move.Direction);
        }
    }

    private void MakeMove(Direction direction)
    {
        var previousHeadPosition = Head;
        Head = Head.MoveOne(direction);

        if (!Tails.Any())
        {
            Tails.Add(previousHeadPosition);
            TailEndPositions.Add(previousHeadPosition);
        }
        var lastTail = Tails[^1];
        if (!Head.IsAdjacent(Tails[0]) || previousHeadPosition == Tails[0])
        {
            
            Tails[0] = Tails[0].Follow(Head);
            for (int i = 1; i <= Tails.Count - 1; i++)
            {
                if (!Tails[i].IsAdjacent(Tails[i - 1]))
                    Tails[i] = Tails[i].Follow(Tails[i - 1]);

                //var str = ToGridString();
            }
            if (Tails.Count == TailLength)
            {
                TailEndPositions.Add(Tails[^1]);
            }
        }
        if (Tails.Count != TailLength && lastTail != Tails[^1])
        {
            Tails.Add(lastTail);
        }

        //var str2 = ToGridString();
        Moves.Add(direction);
    }

    public string ToGridString()
    {
        var grid = "";
        var all = Tails.Append(Head).Concat(TailEndPositions).ToArray();

        var x = all.Select(a => a.X).ToArray();
        var minX = x.Min();
        var maxX = x.Max();

        var y = all.Select(a => a.Y).ToArray();
        var minY = y.Min();
        var maxY = y.Max();

        var normalizeX = minX < 0 ? minX * -1 : 0;
        var normalizeY = minY < 0 ? minY * -1 : 0;

        var xLength = maxX + normalizeX;
        var yLength = maxY + normalizeY;

        var gridList = new List<List<string>>();
        for (int i = 0; i <= yLength; i++)
        {
            gridList.Add(new List<string>());
            var item = gridList[^1];
            for (int j = 0; j <= xLength; j++)
            {
                item.Add(".");
            }
        }

        
        gridList[0 + normalizeY][0 + normalizeX] = "S";
        for (int i = 0; i < Tails.Count ; i++)
        {
            gridList[Tails[i].Y + normalizeY][Tails[i].X + normalizeX] = (i + 1).ToString();
        }
        gridList[Head.Y + normalizeY][Head.X + normalizeX] = "H";

        gridList.Reverse();
        gridList.ForEach(row =>
        {
            row.ForEach(item => { grid += item; });
            grid += Environment.NewLine;
        });



        return grid;
    }
}



internal static class Extensions
{

}
