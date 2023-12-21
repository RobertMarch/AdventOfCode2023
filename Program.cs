// See https://aka.ms/new-console-template for more information
using AdventOfCode;

Dictionary<int, BaseDay> dayMap = new Dictionary<int, BaseDay>
{
    { 1, new Day01() },
    { 2, new Day02() },
    { 3, new Day03() },
    { 4, new Day04() },
    { 5, new Day05() },
    { 6, new Day06() },
    { 7, new Day07() },
    { 8, new Day08() },
    { 9, new Day09() },
    { 10, new Day10() },
    { 11, new Day11() },
    { 12, new Day12() },
    { 13, new Day13() },
    { 14, new Day14() },
    { 15, new Day15() },
    { 16, new Day16() },
    { 17, new Day17() },
    { 18, new Day18() },
    { 19, new Day19() },
    { 20, new Day20() },
    { 21, new Day21() },
    // New days here
};

int dayNumber;
Runner.Parts parts = Runner.Parts.BOTH;
Runner.InputType inputType = Runner.InputType.BOTH;

if (args.Length < 2 || !int.TryParse(args[1], out dayNumber)) {
    dayNumber = 21;
    Console.WriteLine("Could not get day number from args, defaulting to ", dayNumber);
}

if (args.Length > 2) {
    parts = (Runner.Parts) Enum.Parse(typeof(Runner.Parts), args[2].ToUpper());
}

if (args.Length > 3) {
    inputType = (Runner.InputType) Enum.Parse(typeof(Runner.InputType), args[3].ToUpper());
}

BaseDay day = dayMap[dayNumber];

if (parts != Runner.Parts.TWO)
{
    if (inputType != Runner.InputType.PUZZLE)
    {
        day.SolvePartOneTestCases();
    }
    if (inputType != Runner.InputType.EXAMPLE)
    {
        Console.WriteLine(day.SolvePartOneFromFile());
    }
}

if (parts != Runner.Parts.ONE)
{
    if (inputType != Runner.InputType.PUZZLE)
    {
        day.SolvePartTwoTestCases();
    }
    if (inputType != Runner.InputType.EXAMPLE)
    {
        Console.WriteLine(day.SolvePartTwoFromFile());
    }
}


namespace Runner
{
    enum Parts {
        ONE,
        TWO,
        BOTH,
    }

    enum InputType {
        EXAMPLE,
        PUZZLE,
        BOTH,
    }
}