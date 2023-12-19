namespace AdventOfCode;

public class Day19 : BaseDay
{
    public Day19() : base(19, GetTestCases())
    { }

    private record PartRating(long X, long M, long A, long S)
    {
        public static PartRating Parse(string line)
        {
            string[] parts = line.Split(new string[] { "{", "=", ",", "}" }, StringSplitOptions.RemoveEmptyEntries);

            return new PartRating(long.Parse(parts[1]), long.Parse(parts[3]), long.Parse(parts[5]), long.Parse(parts[7]));
        }

        public long GetTotalRating()
        {
            return X + M + A + S;
        }
    }

    private class PartRange
    {
        public long MinX { get; set; }
        public long MaxX { get; set; }
        public long MinM { get; set; }
        public long MaxM { get; set; }
        public long MinA { get; set; }
        public long MaxA { get; set; }
        public long MinS { get; set; }
        public long MaxS { get; set; }

        public PartRange(long minX, long maxX, long minM, long maxM, long minA, long maxA, long minS, long maxS)
        {
            MinX = minX;
            MaxX = maxX;
            MinM = minM;
            MaxM = maxM;
            MinA = minA;
            MaxA = maxA;
            MinS = minS;
            MaxS = maxS;
        }

        public PartRange Clone()
        {
            return new PartRange(
                MinX,
                MaxX,
                MinM,
                MaxM,
                MinA,
                MaxA,
                MinS,
                MaxS
            );
        }

        public long GetCombinations()
        {
            return Math.Max(0, MaxX - MinX + 1) * Math.Max(0, MaxM - MinM + 1) * Math.Max(0, MaxA - MinA + 1) * Math.Max(0, MaxS - MinS + 1);
        }
    }

    private record Condition(string Property, string CheckType, long CheckValue, string Target)
    {
        public static Condition Parse(string line)
        {
            string checkType = line.Where(c => c == '<' || c == '>').First().ToString();
            string[] parts = line.Split(new string[] { ":", checkType }, StringSplitOptions.None);

            return new Condition(parts[0], checkType, long.Parse(parts[1]), parts[2]);
        }
    }

    private class Workflow
    {
        public string Name { get; init; }
        public List<Condition> Conditions { get; init; }
        public string DefaultTarget { get; init; }

        public Workflow(string name, List<Condition> conditions, string defaultTarget)
        {
            Name = name;
            Conditions = conditions;
            DefaultTarget = defaultTarget;
        }

        public static Workflow Parse(string line)
        {
            string[] parts = line.Split(new string[] { "{", ",", "}" }, StringSplitOptions.RemoveEmptyEntries);
            List<Condition> conditions = parts.Skip(1).SkipLast(1).Select(part => Condition.Parse(part)).ToList();
            return new Workflow(parts[0], conditions, parts.Last());
        }
    }

    protected override string SolvePartOne(string input)
    {
        List<string> inputBlocks = new List<string>(input.Split(new string[] { "\r\n\r\n", "\r\r", "\n\n" }, StringSplitOptions.None));
        Dictionary<string, Workflow> workflows = SplitStringToLines(inputBlocks[0]).Select(Workflow.Parse).ToDictionary(w => w.Name);
        List<PartRating> partRatings = SplitStringToLines(inputBlocks[1]).Select(PartRating.Parse).ToList();

        long result = 0;

        partRatings.ForEach((PartRating part) =>
        {
            string currentLocation = "in";
            while (currentLocation != "A" && currentLocation != "R")
            {
                Workflow currentWorkflow = workflows[currentLocation];
                currentLocation = GetNextWorkflow(part, currentWorkflow);
            }
            if (currentLocation == "A")
            {
                result += part.GetTotalRating();
            }
        });

        return result.ToString();
    }

    private string GetNextWorkflow(PartRating part, Workflow workflow)
    {
        foreach (Condition condition in workflow.Conditions)
        {
            long partProperty = GetPartProperty(part, condition.Property);
            if (condition.CheckType == "<" && partProperty < condition.CheckValue)
            {
                return condition.Target;
            }
            if (condition.CheckType == ">" && partProperty > condition.CheckValue)
            {
                return condition.Target;
            }
        }
        return workflow.DefaultTarget;
    }

    private long GetPartProperty(PartRating part, string propertyName)
    {
        switch (propertyName)
        {
            case "x":
                return part.X;
            case "m":
                return part.M;
            case "a":
                return part.A;
            case "s":
                return part.S;
        }
        throw new Exception($"Unexpected property {propertyName}");
    }

    protected override string SolvePartTwo(string input)
    {
        List<string> inputBlocks = new List<string>(input.Split(new string[] { "\r\n\r\n", "\r\r", "\n\n" }, StringSplitOptions.None));
        Dictionary<string, Workflow> workflows = SplitStringToLines(inputBlocks[0]).Select(Workflow.Parse).ToDictionary(w => w.Name);

        return GetTotalCombinations(new PartRange(1, 4000, 1, 4000, 1, 4000, 1, 4000), "in", workflows).ToString();
    }

    private long GetTotalCombinations(PartRange range, string currentLocation, Dictionary<string, Workflow> workflows)
    {
        if (currentLocation == "R")
        {
            return 0;
        }
        if (currentLocation == "A")
        {
            return range.GetCombinations();
        }

        Workflow workflow = workflows[currentLocation];
        PartRange remainingRange = range.Clone();

        long totalCombinations = 0;

        foreach (Condition condition in workflow.Conditions)
        {
            PartRange conditionRange = SplitRangeForCondition(remainingRange, condition);
            if (conditionRange.GetCombinations() > 0)
            {
                totalCombinations += GetTotalCombinations(conditionRange, condition.Target, workflows);
            }
        }
        return totalCombinations + GetTotalCombinations(remainingRange, workflow.DefaultTarget, workflows);
    }

    private PartRange SplitRangeForCondition(PartRange currentRange, Condition condition)
    {
        PartRange newRange = currentRange.Clone();
        switch (condition.Property)
        {
            case "x":
                if (condition.CheckType == "<")
                {
                    currentRange.MinX = condition.CheckValue;
                    newRange.MaxX = condition.CheckValue - 1;
                }
                else
                {
                    currentRange.MaxX = condition.CheckValue;
                    newRange.MinX = condition.CheckValue + 1;
                }
                break;
            case "m":
                if (condition.CheckType == "<")
                {
                    currentRange.MinM = condition.CheckValue;
                    newRange.MaxM = condition.CheckValue - 1;
                }
                else
                {
                    currentRange.MaxM = condition.CheckValue;
                    newRange.MinM = condition.CheckValue + 1;
                }
                break;
            case "a":
                if (condition.CheckType == "<")
                {
                    currentRange.MinA = condition.CheckValue;
                    newRange.MaxA = condition.CheckValue - 1;
                }
                else
                {
                    currentRange.MaxA = condition.CheckValue;
                    newRange.MinA = condition.CheckValue + 1;
                }
                break;
            case "s":
                if (condition.CheckType == "<")
                {
                    currentRange.MinS = condition.CheckValue;
                    newRange.MaxS = condition.CheckValue - 1;
                }
                else
                {
                    currentRange.MaxS = condition.CheckValue;
                    newRange.MinS = condition.CheckValue + 1;
                }
                break;
        }
        return newRange;
    }

    private static TestCase[] GetTestCases()
    {
        return [
            new TestCase(@"px{a<2006:qkq,m>2090:A,rfg}
pv{a>1716:R,A}
lnx{m>1548:A,A}
rfg{s<537:gd,x>2440:R,A}
qs{s>3448:A,lnx}
qkq{x<1416:A,crn}
crn{x>2662:A,R}
in{s<1351:px,qqz}
qqz{s>2770:qs,m<1801:hdj,R}
gd{a>3333:R,R}
hdj{m>838:A,pv}

{x=787,m=2655,a=1222,s=2876}
{x=1679,m=44,a=2067,s=496}
{x=2036,m=264,a=79,s=2244}
{x=2461,m=1339,a=466,s=291}
{x=2127,m=1623,a=2188,s=1013}", "19114", "167409079868000"),
        ];
    }
}