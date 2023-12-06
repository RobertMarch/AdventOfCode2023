namespace AdventOfCode;

public class Day06 : BaseDay
{
    public Day06() : base(
        6,
        [
            new TestCase(@"Time:      7  15   30
Distance:  9  40  200", "288", "71503"),
        ]
    ) {}

    protected override string SolvePartOne(string input) {
        List<string> lines = SplitStringToLines(input);
        List<long> times = ParseAllRaceValues(lines[0]);
        List<long> distances = ParseAllRaceValues(lines[1]);

        long result = 1;
        
        for (int raceId = 0; raceId < times.Count; raceId++)
        {
            result *= GetOptionsForRace(times[raceId], distances[raceId]);
        }

        return result.ToString();
    }

    protected override string SolvePartTwo(string input) {
        List<string> lines = SplitStringToLines(input);
        long time = long.Parse(lines[0].Split(":")[1].Replace(" ", ""));
        long distance = long.Parse(lines[1].Split(":")[1].Replace(" ", ""));

        return GetOptionsForRace(time, distance).ToString();
    }

    private List<long> ParseAllRaceValues(string line)
    {
        return line.Split(" ", StringSplitOptions.RemoveEmptyEntries)
            .Skip(1)
            .Select(long.Parse)
            .ToList();
    }

    private long GetOptionsForRace(long time, long distance)
    {
        for (long i = 0; i < time; i++)
        {
            if (i * (time - i) > distance)
            {
                return time - 2 * i + 1;
            }
        }

        return 0;
    }
}