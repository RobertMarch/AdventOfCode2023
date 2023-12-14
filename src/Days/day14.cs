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
        long maxY = rolledState.GetMaxY();
        return rolledState.roundRocks
            .Select(p => maxY - p.Y + 1)
            .Sum()
            .ToString();
    }

    protected override string SolvePartTwo(string input)
    {
        return "Not yet implemented";
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

        return new RockState(cubeRocks, roundRocks);
    }

    private RockState TiltInDirection(RockState initialState, Point direction)
    {
        HashSet<Point> newRoundRocks = new HashSet<Point>();
        long maxY = initialState.GetMaxY();

        for (long y = 0; y <= maxY; y++)
        {
            IEnumerable<Point> pointsOnY = initialState.roundRocks.Where(p => p.Y == y);
            foreach (Point point in pointsOnY)
            {
                Point newPoint = point;
                while (true)
                {
                    Point nextPoint = newPoint.AddPoint(direction);
                    if (nextPoint.Y < 0 || initialState.cubeRocks.Contains(nextPoint) || newRoundRocks.Contains(nextPoint))
                    {
                        newRoundRocks.Add(newPoint);
                        break;
                    }
                    newPoint = nextPoint;
                }
            }
        }

        return new RockState(initialState.cubeRocks, newRoundRocks);
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
#OO..#....", "136", null),
        ];
    }
}