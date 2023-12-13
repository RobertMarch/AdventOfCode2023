namespace AdventOfCode;

public class Day13 : BaseDay
{
    public Day13() : base(13, GetTestCases())
    {}

    protected override string SolvePartOne(string input)
    {
        return input.Split(new string[] {"\r\n\r\n", "\r\r", "\n\n"}, StringSplitOptions.None)
            .Select(CalculateLineOfReflectionSummaryValue)
            .Sum()
            .ToString();
    }

    private long CalculateLineOfReflectionSummaryValue(string input)
    {
        List<string> lines = SplitStringToLines(input);

        long result = CalculateLineOfReflection(lines) * 100 + CalculateLineOfReflection(FlipString(lines));
        Console.WriteLine($"{result}");
        return result;
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

    private long CalculateLineOfReflection(List<string> lines)
    {
        for (int mirrorLine = 1; mirrorLine < lines.Count; mirrorLine++)
        {
            bool isMirror = true;
            for (int checkIndex = 0; checkIndex < mirrorLine; checkIndex++)
            {
                if (mirrorLine + checkIndex > lines.Count - 1)
                {
                    break;
                }
                if (lines[mirrorLine - checkIndex - 1] != lines[mirrorLine + checkIndex])
                {
                    isMirror = false;
                }
            }
            if (isMirror)
            {
                return mirrorLine;
            }
        }

        return 0;
    }

    protected override string SolvePartTwo(string input)
    {
        return "Not yet implemented";
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
#....#..#", "405", null),
        ];
    }
}