namespace AdventOfCode;

public class Day25 : BaseDay
{
    public Day25() : base(25, GetTestCases())
    { }

    private class Connection
    {
        public string Left { get; init; }
        public string Right { get; init; }

        public Connection(string[] nodes)
        {
            IEnumerable<string> sortedNodes = nodes.Order();
            Left = sortedNodes.First();
            Right = sortedNodes.Last();
        }

        public bool EqualsOther(string a, string b)
        {
            if (a.CompareTo(b) > 0)
            {
                return b == Left && a == Right;
            }
            else
            {
                return a == Left && b == Right;
            }
        }
    }

    protected override string SolvePartOne(string input)
    {
        Dictionary<string, HashSet<string>> connectionMap = [];
        List<Connection> connections = [];
        SplitStringToLines(input).ForEach(line =>
        {
            string[] parts = line.Split(new string[] { ": ", " " }, StringSplitOptions.RemoveEmptyEntries);
            if (!connectionMap.ContainsKey(parts[0]))
            {
                connectionMap[parts[0]] = [];
            }
            parts.Skip(1).ToList().ForEach(part =>
            {
                connections.Add(new Connection([parts[0], part]));
                if (!connectionMap.ContainsKey(part))
                {
                    connectionMap[part] = [];
                }
                connectionMap[parts[0]].Add(part);
                connectionMap[part].Add(parts[0]);
            });
        });

        List<HashSet<Connection>> connectionsPathsMinimal = GetConnectionPathsToUse(connectionMap, connections);

        return connectionsPathsMinimal.ElementAt(0).Select(c1 =>
            connectionsPathsMinimal.ElementAt(1).Select(c2 =>
                connectionsPathsMinimal.ElementAt(2).Select(c3 =>
                    SubsetSizes(connectionMap, [c1, c2, c3])
                ).Max()
            ).Max()
        ).Max().ToString();
    }

    private List<HashSet<Connection>> GetConnectionPathsToUse(Dictionary<string, HashSet<string>> connectionMap, List<Connection> connections)
    {
        foreach (Connection connection in connections)
        {
            List<HashSet<Connection>> connectionsPaths = GetConnectionPaths(connectionMap, connection, [[connection]]);
            if (connectionsPaths.Count == 3)
            {
                return connectionsPaths;
            }
        }

        throw new Exception("Path not found");
    }

    private List<HashSet<Connection>> GetConnectionPaths(Dictionary<string, HashSet<string>> connectionMap, Connection connection, List<HashSet<Connection>> previousConnectionPaths)
    {
        HashSet<Connection> previousConnections = previousConnectionPaths.SelectMany(path => path).ToHashSet();

        Dictionary<string, List<string>> shortestPaths = [];
        shortestPaths[connection.Left] = [connection.Left];

        List<string> currentFront = [connection.Left];

        while (currentFront.Count > 0 && !shortestPaths.ContainsKey(connection.Right))
        {
            string current = currentFront.First();
            currentFront.Remove(current);

            List<string> nextNodes = connectionMap[current]
                .Where(c => !previousConnections.Any(previousConn => previousConn.EqualsOther(current, c)) && !shortestPaths.ContainsKey(c) && !currentFront.Contains(c))
                .ToList();

            foreach (string node in nextNodes)
            {
                if (!currentFront.Contains(node))
                {
                    shortestPaths.Add(node, shortestPaths[current].ToList());
                    shortestPaths[node].Add(node);
                    currentFront.Add(node);
                };
            }
        }

        if (!shortestPaths.ContainsKey(connection.Right))
        {
            return previousConnectionPaths;
        }

        HashSet<Connection> path = shortestPaths[connection.Right].SkipLast(1)
            .Select((node, index) => new Connection([node, shortestPaths[connection.Right][index + 1]]))
            .ToHashSet();
        previousConnectionPaths.Add(path);

        if (previousConnectionPaths.Count > 3)
        {
            return previousConnectionPaths;
        }
        return GetConnectionPaths(connectionMap, connection, previousConnectionPaths);
    }

    private long SubsetSizes(Dictionary<string, HashSet<string>> connectionMap, HashSet<Connection> removedConnections)
    {
        HashSet<string> reachablePoints = [];

        List<string> currentPoints = [connectionMap.Keys.First()];

        while (currentPoints.Count > 0)
        {
            string current = currentPoints.First();
            reachablePoints.Add(current);
            currentPoints.Remove(current);

            currentPoints.AddRange(connectionMap[current]
                .Where(c => !reachablePoints.Contains(c) && !removedConnections.Any(conn => conn.EqualsOther(current, c)))
            );
        }

        return reachablePoints.Count * (connectionMap.Count - reachablePoints.Count);
    }

    protected override string SolvePartTwo(string input)
    {
        return "No part two";
    }

    private static TestCase[] GetTestCases()
    {
        return [
            new TestCase(@"jqt: rhn xhk nvd
rsh: frs pzl lsr
xhk: hfx
cmg: qnr nvd lhk bvb
rhn: xhk bvb hfx
bvb: xhk hfx
pzl: lsr hfx nvd
qnr: nvd
ntq: jqt hfx bvb xhk
nvd: lhk
lsr: lhk
rzs: qnr cmg lsr rsh
frs: qnr lhk lsr", "54", null),
        ];
    }
}