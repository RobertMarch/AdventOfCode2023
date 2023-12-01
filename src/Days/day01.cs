namespace AdventOfCode;

public class Day01 : BaseDay
{
    public Day01() : base(
        1,
        [
            new TestCase(@"1abc2
pqr3stu8vwx
a1b2c3d4e5f
treb7uchet", "142", null),
            new TestCase(@"two1nine
eightwothree
abcone2threexyz
xtwone3four
4nineeightseven2
zoneight234
7pqrstsixteen", null, "281"),
        ]
    ) {}

    protected override string SolvePartOne(string input) {
        return GetCalibrationValue(input, false).ToString();
    }

    protected override string SolvePartTwo(string input) {
        return GetCalibrationValue(input, true).ToString();
    }

    private int GetCalibrationValue(string input, bool substituteTextNumbers) {
        List<string> lines = SplitStringToLines(input);
        int total = 0;
        foreach (string lineIterable in lines)
        {
            string line = substituteTextNumbers ? SubstituteTextNumbers(lineIterable) : lineIterable;
            
            IEnumerable<char> digitChars = line.ToCharArray().Where(c => Char.IsDigit(c));
            int firstDigit = int.Parse(digitChars.First().ToString());
            int lastDigit = int.Parse(digitChars.Last().ToString());
            int value = firstDigit * 10 + lastDigit;
            total += value;
        }

        return total;
    }

    private string SubstituteTextNumbers(string line) {
        // Replace by number and first/last digits to handle cases with overlapping words.
        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "one", "o1e" },
            { "two", "t2o" },
            { "three", "t3e" },
            { "four", "4" },
            { "five", "5e" },
            { "six", "6" },
            { "seven", "7n" },
            { "eight", "e8t" },
            { "nine", "n9e" },
        };

        foreach (KeyValuePair<string, string> entry in replacements)
        {
            line = line.Replace(entry.Key, entry.Value);
        }
        return line;
    }
}