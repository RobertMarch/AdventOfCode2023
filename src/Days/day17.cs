namespace AdventOfCode;

public class Day17 : BaseDay
{
    public Day17() : base(17, GetTestCases())
    {}

    private record HistoryKey(Point location, Point direction, int movedDist);
    private record MoveOption(Point location, Point direction, int movedDist)
    {
        public HistoryKey ToHistoryKey()
        {
            return new HistoryKey(location, direction, movedDist);
        }
    };

    private record State(Dictionary<Point, long> heatLossMap, Dictionary<HistoryKey, long> minDistances, Dictionary<MoveOption, long> availableMoves);

    protected override string SolvePartOne(string input)
    {
        return Solve(input, 1, 3);
    }

    protected override string SolvePartTwo(string input)
    {
        return Solve(input, 4, 10);
    }

    private string Solve(string input, int minMoveDist, int maxMoveDist)
    {
        Dictionary<Point, long> heatLossMap = BuildMap(input);
        Dictionary<HistoryKey, long> minDistances = new Dictionary<HistoryKey, long>();
        minDistances[new HistoryKey(new Point(0, 0), new Point(0, 1), 0)] = 0;
        minDistances[new HistoryKey(new Point(0, 0), new Point(1, 0), 0)] = 0;

        Dictionary<MoveOption, long> availableMoves = new Dictionary<MoveOption, long>();
        availableMoves[new MoveOption(new Point(1, 0), new Point(1, 0), 1)] = heatLossMap[new Point(1, 0)];
        availableMoves[new MoveOption(new Point(0, 1), new Point(0, 1), 1)] = heatLossMap[new Point(0, 1)];

        State state = new State(heatLossMap, minDistances, availableMoves);
        
        long maxX = heatLossMap.Keys.Select(key => key.X).Max();
        long maxY = heatLossMap.Keys.Select(key => key.Y).Max();
        long targetBestValue = 10000000;

        while (availableMoves.Count > 0)
        {
            MoveOption nextMove = availableMoves.MinBy(entry => availableMoves[entry.Key]).Key;
            long nextMoveBestValue = availableMoves[nextMove];

            if (nextMoveBestValue > targetBestValue)
            {
                return targetBestValue.ToString();
            }

            if (nextMove.location.X == maxX && nextMove.location.Y == maxY)
            {
                targetBestValue = Math.Min(targetBestValue, nextMoveBestValue);
            }

            availableMoves.Remove(nextMove);

            minDistances[nextMove.ToHistoryKey()] = nextMoveBestValue;

            if (nextMove.movedDist < maxMoveDist)
            {
                GetOption(nextMove.location, nextMove.direction, nextMove.movedDist + 1, nextMoveBestValue, state);
            }
            if (nextMove.movedDist >= minMoveDist)
            {
                if (nextMove.direction.X == 0)
                {
                    GetOption(nextMove.location, new Point(1, 0), 1, nextMoveBestValue, state);
                    GetOption(nextMove.location, new Point(-1, 0), 1, nextMoveBestValue, state);
                }
                else
                {
                    GetOption(nextMove.location, new Point(0, 1), 1, nextMoveBestValue, state);
                    GetOption(nextMove.location, new Point(0, -1), 1, nextMoveBestValue, state);
                }
            }

        }

        return minDistances.Keys.Where(k => k.location.X == maxX && k.location.Y == maxY)
            .Select(k => minDistances[k])
            .Min()
            .ToString();
    }
    
    private Dictionary<Point, long> BuildMap(string input)
    {
        Dictionary<Point, long> map = new Dictionary<Point, long>();

        SplitStringToLines(input)
            .Select((string line, int index) => (line, index))
            .ToList()
            .ForEach(line => {
                for (int x = 0; x < line.line.Length; x++)
                {
                    map[new Point(x, line.index)] = long.Parse(line.line[x].ToString());
                }
            });

        return map;
    }

    private void GetOption(Point previousLocation, Point nextDirection, int movedDist, long previousHeatLossValue, State state)
    {
        MoveOption option = new MoveOption(previousLocation.AddPoint(nextDirection), nextDirection, movedDist);
        if (!state.heatLossMap.ContainsKey(option.location))
        {
            return;
        }
        long optionValue = previousHeatLossValue + state.heatLossMap[option.location];
        if (state.minDistances.ContainsKey(option.ToHistoryKey()))
        {
            return;
        }
        if (state.availableMoves.ContainsKey(option))
        {
            state.availableMoves[option] = Math.Min(state.availableMoves[option], optionValue);
        } else {
            state.availableMoves[option] = optionValue;
        }
    }

    private static TestCase[] GetTestCases()
    {
        return [
            new TestCase(@"2413432311323
3215453535623
3255245654254
3446585845452
4546657867536
1438598798454
4457876987766
3637877979653
4654967986887
4564679986453
1224686865563
2546548887735
4322674655533", "102", "94"),
        ];
    }
}