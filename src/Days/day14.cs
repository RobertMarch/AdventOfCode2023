namespace AdventOfCode;

public class Day14 : BaseDay
{
    private record RockState(HashSet<Point> cubeRocks, HashSet<Point> roundRocks)
    {
        public long GetMaxY()
        {
            return cubeRocks.Select(p => p.Y).Max();
        }

        public long GetMaxX()
        {
            return cubeRocks.Select(p => p.X).Max();
        }
    };

    public Day14() : base(14, GetTestCases())
    {}

    protected override string SolvePartOne(string input)
    {
        RockState initialState = GetRockLocations(input);
        RockState rolledState = TiltInDirection(initialState, new Point(0, -1));
        return CalculateLoad(rolledState).ToString();
    }

    protected override string SolvePartTwo(string input)
    {
        RockState rockState = GetRockLocations(input);
        List<HashSet<Point>> previousRoundRocks = [rockState.roundRocks];

        for (long cycle = 1; cycle <= 1000000000; cycle++)
        {
            rockState = TiltInDirection(rockState, new Point(0, -1));
            rockState = TiltInDirection(rockState, new Point(-1, 0));
            rockState = TiltInDirection(rockState, new Point(0, 1));
            rockState = TiltInDirection(rockState, new Point(1, 0));

            IEnumerable<int> previouslySeen = previousRoundRocks.Select((HashSet<Point> state, int index) => (state, index))
                .Where(state => rockState.roundRocks.SetEquals(state.state))
                .Select(state => state.index);

            if (previouslySeen.Count() > 0)
            {
                long previous = previouslySeen.First();
                long loopSize = cycle - previous;

                long remainderAfterLoops = (1000000000 - cycle) % loopSize;

                RockState finalState = new RockState(rockState.cubeRocks, previousRoundRocks[(int) (previous + remainderAfterLoops)]);

                return CalculateLoad(finalState).ToString();
            }
            previousRoundRocks.Add(rockState.roundRocks);
        }

        return "No answer found";
    }

    private RockState GetRockLocations(string input)
    {
        HashSet<Point> cubeRocks = new HashSet<Point>();
        HashSet<Point> roundRocks = new HashSet<Point>();

        SplitStringToLines(input)
            .Select((string line, int index) => (line, index))
            .ToList()
            .ForEach(line => {
                for (int x = 0; x < line.line.Length; x++)
                {
                    if (line.line[x] == '#')
                    {
                        cubeRocks.Add(new Point(x, line.index));
                    }
                    if (line.line[x] == 'O')
                    {
                        roundRocks.Add(new Point(x, line.index));
                    }
                }
            });
        long maxX = cubeRocks.Select(p => p.X).Max();
        long maxY = cubeRocks.Select(p => p.Y).Max();

        for (long x = 0; x <= maxX; x++)
        {
            cubeRocks.Add(new Point(x, -1));
            cubeRocks.Add(new Point(x, maxY + 1));
        }

        for (long y = 0; y <= maxY; y++)
        {
            cubeRocks.Add(new Point(-1, y));
            cubeRocks.Add(new Point(maxX + 1, y));
        }

        return new RockState(cubeRocks, roundRocks);
    }

    private RockState TiltInDirection(RockState initialState, Point direction)
    {
        HashSet<Point> newRoundRocks = new HashSet<Point>();
        long maxX = initialState.GetMaxX();
        long maxY = initialState.GetMaxY();

        foreach (Point cubeRock in initialState.cubeRocks)
        {
            int roundRockCount = 0;
            Point nextPoint = cubeRock.GetVector(direction);
            while (!initialState.cubeRocks.Contains(nextPoint) && nextPoint.X >= 0 && nextPoint.Y >= 0 && nextPoint.X <= maxX && nextPoint.Y <= maxY)
            {
                if (initialState.roundRocks.Contains(nextPoint))
                {
                    roundRockCount += 1;
                }
                nextPoint = nextPoint.GetVector(direction);
            }

            if (roundRockCount == 0)
            {
                continue;
            }
            
            Point nextRoundRock = cubeRock.GetVector(direction);
            for (long i = 0; i < roundRockCount; i++)
            {
                newRoundRocks.Add(nextRoundRock);
                nextRoundRock = nextRoundRock.GetVector(direction);
            }
        }

        return new RockState(initialState.cubeRocks, newRoundRocks);
    }

    private long CalculateLoad(RockState rockState)
    {
        long maxY = rockState.GetMaxY();
        return rockState.roundRocks
            .Select(p => maxY - p.Y)
            .Sum();
    }

    private static TestCase[] GetTestCases()
    {
        return [
            new TestCase(@"O....#....
O.OO#....#
.....##...
OO.#O....O
.O.....O#.
O.#..O.#.#
..O..#O..O
.......O..
#....###..
#OO..#....", "136", "64"),
        ];
    }
}