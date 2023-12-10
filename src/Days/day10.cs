namespace AdventOfCode;

public class Day10 : BaseDay
{
    public Day10() : base(
        10,
        [
            new TestCase(@".....
.S-7.
.|.|.
.L-J.
.....", "4", "1"),
            new TestCase(@"-L|F7
7S-7|
L|7||
-L-J|
L|-JF", "4", "1"),
            new TestCase(@"..F7.
.FJ|.
SJ.L7
|F--J
LJ...", "8", "1"),
            new TestCase(@"7-F7-
.FJ|7
SJLL7
|F--J
LJ.LJ", "8", "1"),
            new TestCase(@"...........
.S-------7.
.|F-----7|.
.||.....||.
.||.....||.
.|L-7.F-J|.
.|..|.|..|.
.L--J.L--J.
...........", null, "4"),
            new TestCase(@".F----7F7F7F7F-7....
.|F--7||||||||FJ....
.||.FJ||||||||L7....
FJL7L7LJLJ||LJ.L-7..
L--J.L7...LJS7F-7L7.
....F-J..F7FJ|L7L7L7
....L7.F7||L7|.L7L7|
.....|FJLJ|FJ|F7|.LJ
....FJL-7.||.||||...
....L---J.LJ.LJLJ...", null, "8"),
        ]
    ) {}

    private class Tile
    {
        private static Dictionary<int, Point> DirectionVectors = new Dictionary<int, Point>{
            { 0, new Point(0, -1) },
            { 1, new Point(1, 0) },
            { 2, new Point(0, 1) },
            { 3, new Point(-1, 0) },
        };

        public Point Location { get; init; }
        public char TileType { get; init; }
        private bool[] Connections { get; init; }

        public Tile(Point Location, char TileType)
        {
            this.Location = Location;
            this.TileType = TileType;
            Connections = GetConnectionsFromType();
        }

        private bool[] GetConnectionsFromType()
        {
            switch (TileType)
            {
                case '|':
                    return [true, false, true, false];
                case '-':
                    return [false, true, false, true];
                case 'L':
                    return [true, true, false, false];
                case 'J':
                    return [true, false, false, true];
                case '7':
                    return [false, false, true, true];
                case 'F':
                    return [false, true, true, false];
                case '.':
                    return [false, false, false, false];
                case 'S':
                    return [true, true, true, true];
            }
            Console.WriteLine($"Unrecognised character {TileType}");
            return [false, false, false, false];
        }

        public List<Point> GetConnectedNeighbours()
        {
            return Connections.Select((bool connected, int index) => (connected, index))
                .Where(val => val.connected)
                .Select(val => Location.AddPoint(DirectionVectors[val.index]))
                .ToList();
        }

        public override string ToString()
        {
            return $"{Location}, {TileType}, {string.Join(", ", Connections)}";
        }
    }

    protected override string SolvePartOne(string input) {
        (Dictionary<Point, Tile> tileMap, Point start) = BuildTileMap(input);

        return (GetLoopPoints(tileMap, start).Count / 2).ToString();
    }

    private static Point MoveDirection = new Point(0, -1);
    
    // No need to handle 'S' as actual value is '|' for my input
    private static HashSet<char> TileTypesToCount = ['-', '7', 'J'];

    protected override string SolvePartTwo(string input) {
        (Dictionary<Point, Tile> tileMap, Point start) = BuildTileMap(input);

        HashSet<Point> loop = GetLoopPoints(tileMap, start);

        return tileMap.Keys
            .Where(point => {
                if (loop.Contains(point)) {
                    return false;
                }

                int count = 0;

                Point p = point.AddPoint(MoveDirection);
                while (tileMap.ContainsKey(p)) {
                    char tileType = tileMap[p].TileType;
                    if (loop.Contains(p) && TileTypesToCount.Contains(tileMap[p].TileType)) {
                        count += 1;
                    }
                    p = p.AddPoint(MoveDirection);
                }

                return count % 2 == 1;
            }).Count().ToString();
    }
    
    private (Dictionary<Point, Tile>, Point) BuildTileMap(string input)
    {
        List<string> lines = SplitStringToLines(input);
        Dictionary<Point, Tile> tileMap = new Dictionary<Point, Tile>();
        Point? start = null;

        for (int y = 0; y < lines.Count; y++) {
            string line = lines[y];
            for (int x = 0; x < line.Length; x++) {
                char character = line.ElementAt(x);
                Point point = new Point(x, y);
                tileMap.Add(point, new Tile(point, character));

                if (character == 'S')
                {
                    start = point;
                }
            }
        }

        if (start == null) {
            throw new Exception("Start not found");
        }

        return (tileMap, start);
    }

    private HashSet<Point> GetLoopPoints(Dictionary<Point, Tile> tileMap, Point start)
    {
        HashSet<Point> loopPoints = [start];

        Point nextPoint = tileMap[start].GetConnectedNeighbours()
            .Where(p => tileMap.ContainsKey(p) && tileMap[p].GetConnectedNeighbours().Contains(start))
            .First();

        while (!loopPoints.Contains(nextPoint) && tileMap.ContainsKey(nextPoint))
        {
            loopPoints.Add(nextPoint);
            nextPoint = tileMap[nextPoint].GetConnectedNeighbours()
                .Where(p => !loopPoints.Contains(p))
                .FirstOrDefault(new Point(-1, -1));
        }

        return loopPoints;
    }
}