namespace AdventOfCode;

public class Day16 : BaseDay
{
    public Day16() : base(16, GetTestCases())
    { }

    private record HistoryValue(Point point, Point direction);

    protected override string SolvePartOne(string input)
    {
        Dictionary<Point, char> map = BuildMap(input);

        Point location = new Point(0, 0);
        Point direction = new Point(1, 0);

        return GetEnergisationForInitialState(location, direction, map).ToString();
    }

    protected override string SolvePartTwo(string input)
    {
        Dictionary<Point, char> map = BuildMap(input);

        long maxX = map.Keys.Select(p => p.X).Max();
        long maxY = map.Keys.Select(p => p.Y).Max();

        List<HistoryValue> initialStates = [];

        for (int y = 0; y <= maxY; y++)
        {
            initialStates.Add(new HistoryValue(new Point(0, y), new Point(1, 0)));
            initialStates.Add(new HistoryValue(new Point(maxX, y), new Point(-1, 0)));
        }

        for (int x = 0; x <= maxX; x++)
        {
            initialStates.Add(new HistoryValue(new Point(x, 0), new Point(0, 1)));
            initialStates.Add(new HistoryValue(new Point(x, maxY), new Point(0, -1)));
        }

        return initialStates
            .Select(state => GetEnergisationForInitialState(state.point, state.direction, map))
            .Max()
            .ToString();
    }

    private long GetEnergisationForInitialState(Point location, Point direction, Dictionary<Point, char> map)
    {
        HashSet<HistoryValue> history = new HashSet<HistoryValue>();

        FindBeamPath(location, direction, map, history);

        return history.Select(val => val.point).ToHashSet().Count();
    }

    private Dictionary<Point, char> BuildMap(string input)
    {
        Dictionary<Point, char> map = new Dictionary<Point, char>();

        SplitStringToLines(input)
            .Select((string line, int index) => (line, index))
            .ToList()
            .ForEach(line =>
            {
                for (int x = 0; x < line.line.Length; x++)
                {
                    map[new Point(x, line.index)] = line.line[x];
                }
            });

        return map;
    }

    private void FindBeamPath(Point location, Point direction, Dictionary<Point, char> map, HashSet<HistoryValue> history)
    {
        HistoryValue currentPoint = new HistoryValue(location, direction);
        if (history.Contains(currentPoint) || !map.ContainsKey(location))
        {
            return;
        }
        history.Add(currentPoint);

        char currentItem = map[location];

        List<Point> nextDirections = GetNextDirections(direction, currentItem);

        foreach (Point nextDirection in nextDirections)
        {
            Point nextLocation = location.AddPoint(nextDirection);
            FindBeamPath(nextLocation, nextDirection, map, history);
        }
    }

    private List<Point> GetNextDirections(Point currentDirection, char currentMapItem)
    {
        switch (currentMapItem)
        {
            case '.':
                return [currentDirection];
            case '/':
                return [new Point(-1 * currentDirection.Y, -1 * currentDirection.X)];
            case '\\':
                return [new Point(currentDirection.Y, currentDirection.X)];
            case '|':
                if (currentDirection.X == 0)
                {
                    return [currentDirection];
                }
                return [
                    new Point(0, -1),
                    new Point(0, 1)
                ];
            case '-':
                if (currentDirection.Y == 0)
                {
                    return [currentDirection];
                }
                return [
                    new Point(-1, 0),
                    new Point(1, 0)
                ];
        }
        return [];
    }

    private static TestCase[] GetTestCases()
    {
        return [
            new TestCase(@".|...\....
|.-.\.....
.....|-...
........|.
..........
.........\
..../.\\..
.-.-/..|..
.|....-|.\
..//.|....", "46", "51"),
        ];
    }
}