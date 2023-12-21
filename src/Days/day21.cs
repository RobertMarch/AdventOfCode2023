namespace AdventOfCode;

public class Day21 : BaseDay
{
    public Day21() : base(21, GetTestCases())
    { }

    private static List<Point> Directions = [
        new Point(0, -1),
        new Point(1, 0),
        new Point(0, 1),
        new Point(-1, 0),
    ];

    protected override string SolvePartOne(string input)
    {
        (HashSet<Point> gardens, Point start) = BuildMap(input);
        return GetPointCountAfterSteps(64, (start.X + start.Y) % 2, false, start, gardens).ToString();
    }

    private long GetPointCountAfterSteps(long steps, long targetCardinality, bool extendGrid, Point start, HashSet<Point> gardens)
    {
        HashSet<Point> reachablePoints = [start];
        List<Point> borderPoints = [start];

        long maxX = gardens.Select(p => p.X).Max();
        long maxY = gardens.Select(p => p.Y).Max();

        for (int i = 1; i <= steps; i++)
        {
            List<Point> nextBorderPoints = [];
            borderPoints.ForEach(borderPoint =>
            {
                Directions.ForEach(direction =>
                {
                    Point nextPoint = borderPoint.AddPoint(direction);
                    Point pointForGardenCheck = extendGrid ? new Point(Mod(nextPoint.X, maxX + 1), Mod(nextPoint.Y, maxY + 1)) : nextPoint;

                    if (gardens.Contains(pointForGardenCheck) && !reachablePoints.Contains(nextPoint) && !borderPoints.Contains(nextPoint))
                    {
                        nextBorderPoints.Add(nextPoint);
                        reachablePoints.Add(nextPoint);
                    }
                });
            });
            borderPoints = nextBorderPoints;
        }

        return reachablePoints.Where(p => (Math.Abs(p.X + p.Y) % 2) == targetCardinality).Count();
    }

    protected override string SolvePartTwo(string input)
    {
        (HashSet<Point> gardens, Point start) = BuildMap(input);

        long totalSteps = 26501365;

        long count1 = GetPointCountAfterSteps(65, 1, true, start, gardens);
        long count2 = GetPointCountAfterSteps(65 + 262, 1, true, start, gardens);
        long count3 = GetPointCountAfterSteps(65 + 262 * 2, 1, true, start, gardens);

        long value = count1;
        long difference = count2 - count1;
        long differenceIncrease = count3 - count2 - difference;
        for (int i = 0; i < totalSteps / 262; i++)
        {
            value += difference;
            difference += differenceIncrease;
        }
        return value.ToString();
    }

    private long Mod(long a, long divisor)
    {
        long remainder = a % divisor;
        return remainder < 0 ? remainder + divisor : remainder;
    }

    private (HashSet<Point>, Point) BuildMap(string input)
    {
        HashSet<Point> gardenPoints = [];
        Point? start = null;

        SplitStringToLines(input)
            .Select((string line, int index) => (line, index))
            .ToList()
            .ForEach(line =>
            {
                for (int x = 0; x < line.line.Length; x++)
                {
                    Point point = new Point(x, line.index);
                    if (line.line[x] == '.')
                    {
                        gardenPoints.Add(point);
                    }
                    if (line.line[x] == 'S')
                    {
                        start = point;
                        gardenPoints.Add(point);
                    }
                }
            });

        if (start == null)
        {
            throw new Exception("Did not find start");
        }

        return (gardenPoints, start);
    }

    private static TestCase[] GetTestCases()
    {
        return [
            new TestCase(@"...........
.....###.#.
.###.##..#.
..#.#...#..
....#.#....
.##..S####.
.##..#...#.
.......##..
.##.#.####.
.##..##.##.
...........", null, null), // "16", "16733044"), // 16 plots after 6 steps
        ];
    }
}