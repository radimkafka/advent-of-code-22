using System.Net;
using System.Net.Http.Headers;

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
        var input = File.ReadAllLines(filePath);
        var instuctions = new List<Instruction>();
        foreach (var line in input)
        {
            instuctions.Add(Instruction.Parse(line));
        }


        var runs = new[] { 20, 60, 100, 140, 180, 220 }.Select(a => new Device(instuctions).Run(a)*a).ToList();
        var sum = runs.Sum();

        Console.WriteLine($"Signal strength is: {sum}.");
        Console.ReadLine();
    }


}

enum InstructionType
{
    None,
    NoOp,
    Addx
}


record Instruction(InstructionType Type, int Param)
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
        if (parts.Length > 1) int.TryParse(parts[1], out param);

        return new(type, param);
    }
}


class InstructionSchedule
{
    public Instruction Instruction { get; private set; }
    public int CyclesLeft { get; set; }
    public InstructionSchedule(Instruction instruction, int cyclesLeft)
    {
        Instruction = instruction;
        CyclesLeft = cyclesLeft;
    }
}

class Device
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

    public int Run(int cycles)
    {
        var runs = 0;
        var runningInstruction = NextInstructions.FirstOrDefault();

        while (runs<cycles)
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
            runs++;
            if (runs >= cycles) break;
        }       
        return X;
    }


    private void RunInstruction(Instruction ins)
    {
        AddInstruction(ins);
        NextInstructions.ForEach(instruction => { instruction.CyclesLeft--; });

        var instructionsToRun = NextInstructions.Where(a => a.CyclesLeft == 0).ToList();
        if (instructionsToRun.Any())
        {
            NextInstructions = NextInstructions.Where((a) => a.CyclesLeft > 0).ToList();
            instructionsToRun.ForEach(schedule => { ApplyInstruction(schedule.Instruction); });
        }

    }

    private void ApplyInstruction(Instruction? ins)
    {
        if (ins?.Type == InstructionType.Addx) X += ins.Param;
        RanInstruction.Add(ins);
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
