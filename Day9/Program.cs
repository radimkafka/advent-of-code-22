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
        var bridge = new RopeBridge();
        foreach (var line in input)
        {
            Move? move = Move.Parse(line);
            bridge.MakeMove(move);
        }

        Console.WriteLine($"Tail visited: {bridge.TailPositions.Distinct().Count()} positions");
        Console.ReadLine();
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
    private Position Head { get; set; } = new(0, 0);

    private Position Tail { get; set; } = new(0, 0);

    public List<Position> TailPositions { get; } = new();

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
        if (!Head.IsAdjacent(Tail) || previousHeadPosition == Tail)
        {
            Tail = Tail.Follow(Head);
            TailPositions.Add(Tail);
            Moves.Add(direction);
        }
    }

}

internal static class Extensions
{

}
