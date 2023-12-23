namespace AdventOfCode;

public class Day23 : BaseDay
{
    public Day23() : base(23, GetTestCases())
    { }

    private static List<(Point vector, char allowedMove)> Directions = [
        (new Point(0, -1), '^'),
        (new Point(1, 0), '>'),
        (new Point(0, 1), 'v'),
        (new Point(-1, 0), '<'),
    ];

    private record Graph(Dictionary<Point, Dictionary<Point, long>> Paths, Point End);

    protected override string SolvePartOne(string input)
    {
        Graph graph = BuildGraph(input, false);

        return FindLongestPath(new Point(1, 0), [], graph).ToString();
    }

    private long FindLongestPath(Point node, HashSet<Point> previousPoints, Graph graph)
    {
        if (previousPoints.Contains(node))
        {
            return -1000000;
        }

        Dictionary<Point, long> nextPoints = graph.Paths[node];
        if (nextPoints.Count == 0 || node == graph.End)
        {
            return 0;
        }

        long longestPath = nextPoints.Select(p => p.Value + FindLongestPath(p.Key, previousPoints.Union([node]).ToHashSet(), graph)).Max();

        return longestPath;
    }

    protected override string SolvePartTwo(string input)
    {
        Graph graph = BuildGraph(input, true);

        return SolveV2(graph).ToString();
    }

    private record State(HashSet<Point> Visited, Point Last)
    {
        public override int GetHashCode()
        {
            return (string.Join(",", Visited.OrderBy(p => p.X * 1000 + p.Y)), Last).GetHashCode();
        }
    }

    private long SolveV2(Graph graph)
    {
        Dictionary<State, long> currentStates = [];
        currentStates[new State([new Point(1, 0)], new Point(1, 0))] = 0;

        long longestPath = -1;
        int loops = 1;

        while (currentStates.Count > 0)
        {
            Dictionary<State, long> nextStates = [];

            foreach ((State state, long distance) in currentStates)
            {
                Dictionary<Point, long> connections = graph.Paths[state.Last]
                    .Where(path => !state.Visited.Contains(path.Key))
                    .ToDictionary();

                if (connections.Keys.Any(p => p.Y == 22 || p.Y == 140))
                {
                    connections = connections.Where(p => p.Key.Y == 22 || p.Key.Y == 140).ToDictionary();
                }

                foreach (KeyValuePair<Point, long> connection in connections)
                {
                    State newState = new State(state.Visited.Union([connection.Key]).ToHashSet(), connection.Key);

                    if ((connection.Key.Y == 22 || connection.Key.Y == 140) && distance + connection.Value > longestPath)
                    {
                        longestPath = Math.Max(longestPath, distance + connection.Value);
                    }
                    else
                    {
                        nextStates[newState] = Math.Max(nextStates.GetValueOrDefault(newState, 0), distance + connection.Value);
                    }
                }
            }

            Console.WriteLine($"{loops}: best {longestPath}, previousStates {currentStates.Count}, nextStates {nextStates.Count}");
            loops += 1;
            currentStates = nextStates;
        }

        return longestPath;
    }

    private Graph BuildGraph(string input, bool partTwo)
    {
        Dictionary<Point, char> paths = BuildMap(input);
        Dictionary<Point, Dictionary<Point, long>> pathGraph = [];
        pathGraph.Add(new Point(1, 0), []);

        BuildGraphFromPoint(new Point(1, 0), new Point(1, 1), paths, pathGraph, partTwo);

        long maxX = paths.Keys.Select(p => p.X).Max();
        long maxY = paths.Keys.Select(p => p.Y).Max();

        return new Graph(pathGraph, new Point(maxX - 1, maxY));
    }

    private void BuildGraphFromPoint(Point previousNode, Point nextPoint, Dictionary<Point, char> paths, Dictionary<Point, Dictionary<Point, long>> pathGraph, bool partTwo)
    {
        HashSet<Point> visitedPoints = [previousNode];
        IEnumerable<Point> neighbours = GetNeighbours(nextPoint, paths).Where(p => !visitedPoints.Contains(p));

        while (neighbours.Count() == 1)
        {
            visitedPoints.Add(nextPoint);
            nextPoint = neighbours.First();
            neighbours = GetNeighbours(nextPoint, paths).Where(p => !visitedPoints.Contains(p));
        }

        if (nextPoint.Y != 0)
        {
            pathGraph[previousNode][nextPoint] = visitedPoints.Count;
        }
        if (pathGraph.ContainsKey(nextPoint))
        {
            if (partTwo && previousNode.Y != 0)
            {
                pathGraph[nextPoint][previousNode] = visitedPoints.Count;
            }
            return;
        }
        pathGraph.Add(nextPoint, []);
        if (partTwo && previousNode.Y != 0)
        {
            pathGraph[nextPoint][previousNode] = visitedPoints.Count;
        }

        List<Point> allowedNeighbours = Directions.Select(d => (nextPoint.AddPoint(d.vector), d.allowedMove))
            .Where(p => paths.ContainsKey(p.Item1) && (paths[p.Item1] == p.allowedMove || partTwo))
            .Select(p => p.Item1)
            .ToList();

        allowedNeighbours.ForEach(neighbour =>
        {
            BuildGraphFromPoint(nextPoint, neighbour, paths, pathGraph, partTwo);
        });
    }

    private List<Point> GetNeighbours(Point point, Dictionary<Point, char> paths)
    {
        return Directions.Select(d => point.AddPoint(d.vector)).Where(paths.ContainsKey).ToList();
    }

    private Dictionary<Point, char> BuildMap(string input)
    {
        Dictionary<Point, char> paths = [];

        SplitStringToLines(input)
            .Select((string line, int index) => (line, index))
            .ToList()
            .ForEach(line =>
            {
                for (int x = 0; x < line.line.Length; x++)
                {
                    Point point = new Point(x, line.index);
                    char character = line.line[x];
                    if (character != '#')
                    {
                        paths.Add(point, character);
                    }
                }
            });

        return paths;
    }

    private static TestCase[] GetTestCases()
    {
        return [
            new TestCase(@"#.#####################
#.......#########...###
#######.#########.#.###
###.....#.>.>.###.#.###
###v#####.#v#.###.#.###
###.>...#.#.#.....#...#
###v###.#.#.#########.#
###...#.#.#.......#...#
#####.#.#.#######.#.###
#.....#.#.#.......#...#
#.#####.#.#.#########v#
#.#...#...#...###...>.#
#.#.#v#######v###.###v#
#...#.>.#...>.>.#.###.#
#####v#.#.###v#.#.###.#
#.....#...#...#.#.#...#
#.#########.###.#.#.###
#...###...#...#...#.###
###.###.#.###v#####v###
#...#...#.#.>.>.#.>.###
#.###.###.#.###.#.#v###
#.....###...###...#...#
#####################.#", "94", "154"),
        ];
    }
}