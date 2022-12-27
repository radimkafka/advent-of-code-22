internal interface ITestInterface
{

}

internal interface AaaPterface123
{

}

internal class Program
{
    private static void Main(string[] args)
    {
        //var filePath = "../../../input.basic.txt";
        var filePath = "../../../input.txt";
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File does not exist!");
            return;
        }
        if (File.Exists(filePath))
        {
            Console.WriteLine($"Aaaa!");
        }

        var input = File.ReadAllLines(filePath);
        var instuctions = new List<Instruction>();
        foreach (var line in input)
        {
            instuctions.Add(Instruction.Parse(line));
        }

        var runs = new[] { 20, 60, 100, 140, 180, 220 }.Select(a => new Device(instuctions)
        .Run(a, null) * a).ToList();
        var sum = runs.Sum();
        Console.WriteLine($"Signal strength is: {sum}.");
        Display(new Device(instuctions));
        _ = Console.ReadLine();
    }

    private static void Display(Device device)
    {
        var index = 0;
        _ = device.Run(240, (cycle, x) =>
        {
            if (cycle % 40 == 0 && cycle != 0)
            {
                index = 0;
                Console.WriteLine();
            }

            if (x == index || x == index - 1 || x == index + 1)
            {
                Console.Write("#");
            }
            else
            {
                Console.Write(".");
            }
            index++;

        });
    }
}

internal enum InstructionType
{
    None,
    NoOp,
    Addx
}

internal record Instruction(InstructionType Type, int Param)
{
    public static Instruction Parse(string input)
    {
        var parts = input.Split(" ");
        var type = parts[0] switch
        {
            "noop" => InstructionType.NoOp,
            "addx" => InstructionType.Addx,
            _ => InstructionType.None
        };
        var param = 0;
        if (parts.Length > 1)
        {
            _ = int.TryParse(parts[1], out param);
        }

        return new(type, param);
    }
}

internal class InstructionSchedule
{
    public Instruction Instruction { get; private set; }
    public int CyclesLeft { get; set; }
    public InstructionSchedule(Instruction instruction, int cyclesLeft)
    {
        Instruction = instruction;
        CyclesLeft = cyclesLeft;
    }
}

internal class Device
{
    public int X { get; private set; } = 1;

    public int NumberOfCycles { get; set; } = 0;

    public Stack<Instruction> Instructions { get; set; } = new();

    public List<InstructionSchedule> NextInstructions { get; private set; } = new List<InstructionSchedule>();

    public List<Instruction> RanInstruction { get; set; } = new();

    public Device()
    {

    }

    public Device(List<Instruction> instructions)
    {
        instructions.ForEach(AddInstruction);
    }

    //public int Run()
    //{
    //    while (Instructions.Any())
    //    {
    //        NumberOfCycles++;
    //        RunInstruction(Instructions.Pop());

    //    }
    //    return X;
    //}

    public int Run(int cycles, Action<int, int>? action)
    {
        var runs = 0;
        var runningInstruction = NextInstructions.FirstOrDefault();

        while (runs < cycles)
        {
            if (runningInstruction?.CyclesLeft == 0)
            {
                ApplyInstruction(runningInstruction?.Instruction);
                // start
                runningInstruction = NextInstructions[RanInstruction.Count];
            }

            if (runningInstruction is not null)
            {
                runningInstruction.CyclesLeft--;
            }
            // during
            if (action is not null)
            {
                action(runs, X);
            }

            runs++;
            if (runs >= cycles)
            {
                break;
            }
        }
        return X;
    }

    private void ApplyInstruction(Instruction? ins)
    {
        if (ins?.Type == InstructionType.Addx)
        {
            X += ins.Param;
        }

        if (ins is not null)
        {
            RanInstruction.Add(ins);
        }
    }

    private void AddInstruction(Instruction ins)
    {
        switch (ins.Type)
        {
            case InstructionType.NoOp:
                NextInstructions.Add(new(ins, 1));
                break;
            case InstructionType.Addx:
                NextInstructions.Add(new(ins, 2));
                break;
            case InstructionType.None:
            default:
                break;
        }
    }
}

internal static class Extensions
{

}
