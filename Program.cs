// See https://aka.ms/new-console-template for more information
using AdventOfCode;

Dictionary<int, BaseDay> dayMap = new Dictionary<int, BaseDay>
{
    { 1, new Day01() },
    { 2, new Day02() },
};

int dayNumber;

if (args.Length < 2 || !int.TryParse(args[1], out dayNumber)) {
    dayNumber = 2;
    Console.WriteLine("Could not get day number from args, defaulting to ", dayNumber);
}

BaseDay day = dayMap[dayNumber];

day.SolvePartOneTestCases();
Console.WriteLine(day.SolvePartOneFromFile());

day.SolvePartTwoTestCases();
Console.WriteLine(day.SolvePartTwoFromFile());
