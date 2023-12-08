namespace AdventOfCode;

public class Day08 : BaseDay
{
    public Day08() : base(
        8,
        [
            new TestCase(@"RL

AAA = (BBB, CCC)
BBB = (DDD, EEE)
CCC = (ZZZ, GGG)
DDD = (DDD, DDD)
EEE = (EEE, EEE)
GGG = (GGG, GGG)
ZZZ = (ZZZ, ZZZ)", "2", null),
            new TestCase(@"LLR

AAA = (BBB, BBB)
BBB = (AAA, ZZZ)
ZZZ = (ZZZ, ZZZ)", "6", null),
            new TestCase(@"LR

11A = (11B, XXX)
11B = (XXX, 11Z)
11Z = (11B, XXX)
22A = (22B, XXX)
22B = (22C, 22C)
22C = (22Z, 22Z)
22Z = (22B, 22B)
XXX = (XXX, XXX)", null, "6"),
        ]
    ) {}

    private record Maps(List<char> Instructions, Dictionary<string, string> LeftMap, Dictionary<string, string> RightMap);

    protected override string SolvePartOne(string input) {
        Maps maps = BuildMapsFromInput(input);

        return GetNodeRepeatLength("AAA", maps, false).ToString();
    }

    protected override string SolvePartTwo(string input) {
        Maps maps = BuildMapsFromInput(input);

        return maps.LeftMap.Keys
            .Where(node => node.EndsWith("A"))
            .Select(node => GetNodeRepeatLength(node, maps, true))
            .Aggregate(CalculateLCM)
            .ToString();
    }

    private Maps BuildMapsFromInput(string input)
    {
        List<string> lines = SplitStringToLines(input);
        List<char> instructions = lines[0].ToCharArray().ToList();

        Dictionary<string, string> leftMap = new Dictionary<string, string>();
        Dictionary<string, string> rightMap = new Dictionary<string, string>();

        foreach (var line in lines.Skip(2))
        {
            string[] parts = line.Split(new string[] {" = (", ", ", ")"}, StringSplitOptions.TrimEntries);
            string sourceNode = parts[0];
            leftMap[sourceNode] = parts[1];
            rightMap[sourceNode] = parts[2];
        }

        return new Maps(instructions, leftMap, rightMap);
    }

    private long GetNodeRepeatLength(
        string startNode,
        Maps maps,
        bool partTwo
    )
    {
        string currentNode = startNode;

        int step = 0;

        while (!currentNode.EndsWith(partTwo ? "Z" : "ZZZ"))
        {
            char instruction = maps.Instructions[step % maps.Instructions.Count];
            if (instruction == 'L')
            {
                currentNode = maps.LeftMap[currentNode];
            }
            else
            {
                currentNode = maps.RightMap[currentNode];
            }
            step++;
        }

        if (step % maps.Instructions.Count != 0) {
            Console.WriteLine($"Unexpected remainder after {step} steps for start node {startNode}, solution may fail");
        }

        return step;
    }

    private long CalculateLCM(long a, long b)
    {
        if (b > a)
        {
            long temp = a;
            a = b;
            b = temp;
        }

        long result = a;
        while (result % b != 0)
        {
            result += a;
        }
        return result;
    }
}