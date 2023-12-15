namespace AdventOfCode;

public class Day15 : BaseDay
{
    public Day15() : base(15, GetTestCases())
    {}

    protected override string SolvePartOne(string input)
    {
        return input.Split(",")
            .Select(CalculateHash)
            .Sum()
            .ToString();
    }

    private class LensPlacement
    {
        public LensPlacement(string label, long focalLength)
        {
            Label = label;
            FocalLength = focalLength;
        }

        public string Label { get; set; }
        public long FocalLength { get; set; }
    };

    protected override string SolvePartTwo(string input)
    {
        Dictionary<long, List<LensPlacement>> boxes = new Dictionary<long, List<LensPlacement>>();

        foreach (string instruction in input.Split(","))
        {
            ProcessInstruction(instruction, boxes);
        }
        return boxes
            .SelectMany(box => box.Value.Select((lens, lensIndex) => (box.Key + 1) * (lensIndex + 1) * lens.FocalLength))
            .Sum()
            .ToString();
    }

    private long CalculateHash(string input)
    {
        long currentValue = 0;
        foreach (char character in input.ToCharArray())
        {
            currentValue += (int) character;
            currentValue *= 17;
            currentValue = currentValue % 256;
        }
        return currentValue;
    }

    private void ProcessInstruction(string instruction, Dictionary<long, List<LensPlacement>> boxes)
    {
        string[] parts = instruction.Split(new string[] {"=", "-"}, StringSplitOptions.RemoveEmptyEntries);
        string label = parts[0];
        long labelHash = CalculateHash(label);

        if (!boxes.ContainsKey(labelHash))
        {
            boxes.Add(labelHash, new List<LensPlacement>());
        }

        List<LensPlacement> box = boxes[labelHash];
        IEnumerable<LensPlacement> existingLens = box.Where(lens => lens.Label == label);

        if (instruction.EndsWith('-'))
        {
            if (existingLens.Count() == 1)
            {
                box.Remove(existingLens.First());
            }
        }
        else
        {
            long focalLength = long.Parse(parts[1]);
            if (existingLens.Count() == 1)
            {
                existingLens.First().FocalLength = focalLength;
            }
            else
            {
                box.Add(new LensPlacement(label, focalLength));
            }
        }
    }

    private static TestCase[] GetTestCases()
    {
        return [
            new TestCase(@"rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7", "1320", "145"),
        ];
    }
}