namespace AdventOfCode;

public class Day09 : BaseDay
{
    public Day09() : base(
        9,
        [
            new TestCase(@"0 3 6 9 12 15
1 3 6 10 15 21
10 13 16 21 30 45", "114", "2"),
        ]
    ) {}

    protected override string SolvePartOne(string input) {
        return SplitStringToLines(input)
            .Select(line => {
                List<long> values = line.Split(" ").Select(long.Parse).ToList();
                PredictNextValue(values);
                return values.Last();
            })
            .Sum()
            .ToString();
    }

    protected override string SolvePartTwo(string input) {
        return SplitStringToLines(input)
            .Select(line => {
                List<long> values = line.Split(" ").Select(long.Parse).ToList();
                PredictNextValue(values);
                return values.First();
            })
            .Sum()
            .ToString();
    }

    private void PredictNextValue(List<long> currentLine)
    {
        List<long> differences = new List<long>();
        for (int i = 1; i < currentLine.Count; i++)
        {
            differences.Add(currentLine[i] - currentLine[i - 1]);
        }

        if (!differences.All(val => val == 0))
        {
            PredictNextValue(differences);
        } else
        {
            differences.Add(0);
        }

        currentLine.Add(currentLine.Last() + differences.Last());
        currentLine.Insert(0, currentLine.First() - differences.First());
    }
}