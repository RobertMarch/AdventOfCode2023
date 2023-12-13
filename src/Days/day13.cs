namespace AdventOfCode;

public class Day13 : BaseDay
{
    public Day13() : base(13, GetTestCases())
    {}

    protected override string SolvePartOne(string input)
    {
        return input.Split(new string[] {"\r\n\r\n", "\r\r", "\n\n"}, StringSplitOptions.None)
            .Select(block => CalculateLineOfReflectionSummaryValue(block, 0))
            .Sum()
            .ToString();
    }

    protected override string SolvePartTwo(string input)
    {
        return input.Split(new string[] {"\r\n\r\n", "\r\r", "\n\n"}, StringSplitOptions.None)
            .Select(block => CalculateLineOfReflectionSummaryValue(block, 1))
            .Sum()
            .ToString();
    }

    private long CalculateLineOfReflectionSummaryValue(string input, long smudgeCount)
    {
        List<string> lines = SplitStringToLines(input);

        return CalculateLineOfReflection(lines, smudgeCount) * 100 + CalculateLineOfReflection(FlipString(lines), smudgeCount);
    }

    private List<string> FlipString(List<string> initialLines)
    {
        List<string> newLines = new List<string>();
        for (int newLineIndex = 0; newLineIndex < initialLines[0].Length; newLineIndex++)
        {
            newLines.Add(string.Join("", initialLines.Select(line => line[newLineIndex])));
        }
        return newLines;
    }

    private long CalculateLineOfReflection(List<string> lines, long smudgeCount)
    {
        for (int mirrorLine = 1; mirrorLine < lines.Count; mirrorLine++)
        {
            long smudgesRequired = 0;
            for (int checkIndex = 0; checkIndex < mirrorLine; checkIndex++)
            {
                if (mirrorLine + checkIndex > lines.Count - 1)
                {
                    break;
                }
                smudgesRequired += GetStringDifference(lines[mirrorLine - checkIndex - 1], lines[mirrorLine + checkIndex]);
            }
            if (smudgesRequired == smudgeCount)
            {
                return mirrorLine;
            }
        }

        return 0;
    }

    private long GetStringDifference(string a, string b)
    {
        return a.ToCharArray()
            .Where((aChar, index) => aChar != b.ElementAt(index))
            .Count();
    }

    private static TestCase[] GetTestCases()
    {
        return [
            new TestCase(@"#.##..##.
..#.##.#.
##......#
##......#
..#.##.#.
..##..##.
#.#.##.#.

#...##..#
#....#..#
..##..###
#####.##.
#####.##.
..##..###
#....#..#", "405", "400"),
        ];
    }
}