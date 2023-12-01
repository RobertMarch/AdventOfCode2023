public abstract class BaseDay
{

    private int DayNumber { get; init; }

    private TestCase[] TestCases { get; init; }

    protected BaseDay(int dayNumber, TestCase[] testCases)
    {
        DayNumber = dayNumber;
        TestCases = testCases;
    }

    protected string ReadFileAsString()
    {
        return File.ReadAllText($"inputs/day{DayNumber:D2}.txt");
    }

    protected List<string> SplitStringToLines(string input)
    {
        return new List<string>(input.Split(new string[] {"\r\n", "\r", "\n"}, StringSplitOptions.None));
    }

    public string SolvePartOneFromFile()
    {
        string inputFile = ReadFileAsString();
        return SolvePartOne(inputFile);
    }

    public void SolvePartOneTestCases()
    {
        foreach (var testCase in TestCases)
        {
            if (testCase.expectedSolutionPartOne == null) {
                continue;
            }

            string result = SolvePartOne(testCase.input);

            Console.WriteLine(
                (result == testCase.expectedSolutionPartOne ? "Success!" : "Failed,")
                + " Expected result: " + testCase.expectedSolutionPartOne
                + ", Actual result: " + result
            );
        }
    }

    public string SolvePartTwoFromFile()
    {
        string inputFile = ReadFileAsString();
        return SolvePartTwo(inputFile);
    }

    public void SolvePartTwoTestCases()
    {
        foreach (var testCase in TestCases)
        {
            if (testCase.expectedSolutionPartTwo == null) {
                continue;
            }

            string result = SolvePartTwo(testCase.input);

            Console.WriteLine(
                (result == testCase.expectedSolutionPartTwo ? "Success!" : "Failed,")
                + " Expected result: " + testCase.expectedSolutionPartTwo
                + ", Actual result: " + result
            );
        }
    }

    protected abstract string SolvePartOne(string input);
    protected abstract string SolvePartTwo(string input);
}