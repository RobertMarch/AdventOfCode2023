namespace AdventOfCode;

public class Day22 : BaseDay
{
    public Day22() : base(22, GetTestCases())
    { }

    private class Brick
    {
        public long Id { get; init; }
        public Point3D Start { get; set; }
        public Point3D End { get; set; }

        public static Brick Parse(string line, long lineIndex)
        {
            List<Point3D> points = line.Split("~").Select(pointString =>
            {
                List<long> coords = pointString.Split(",").Select(long.Parse).ToList();
                return new Point3D(coords[0], coords[1], coords[2]);
            }).OrderBy(p => p.GetManhattenDist()).ToList();
            return new Brick(lineIndex, points[0], points[1]);
        }

        public Brick(long id, Point3D start, Point3D end)
        {
            Id = id;
            Start = start;
            End = end;
        }

        public HashSet<Point3D> GetPoints()
        {
            (Point3D direction, long length) = GetBrickVector();
            HashSet<Point3D> points = [];
            Point3D curr = Start;
            for (long i = 1; i <= length; i++)
            {
                points.Add(curr);
                curr = curr.AddPoint(direction);
            }
            return points;
        }

        private (Point3D, long) GetBrickVector()
        {
            Point3D vector = End.GetVector(Start);
            return (new Point3D(
                vector.X != 0 ? 1 : 0,
                vector.Y != 0 ? 1 : 0,
                vector.Z != 0 ? 1 : 0
            ), vector.GetManhattenDist() + 1);
        }

        public Brick GetLoweredBrick()
        {
            return new Brick(
                Id,
                Start.AddPoint(new Point3D(0, 0, -1)),
                End.AddPoint(new Point3D(0, 0, -1))
            );
        }
    }

    protected override string SolvePartOne(string input)
    {
        List<Brick> bricks = SplitStringToLines(input).Select((line, index) => Brick.Parse(line, index)).OrderBy(brick => brick.Start.Z).ToList();

        Dictionary<long, HashSet<long>> supportedByBricks = GetSupportedByForBricks(bricks);

        HashSet<long> criticalBricks = supportedByBricks.Values.Where(value => value.Count == 1).Select(value => value.First()).ToHashSet();

        return (bricks.Count - criticalBricks.Count).ToString();
    }

    protected override string SolvePartTwo(string input)
    {
        List<Brick> bricks = SplitStringToLines(input).Select((line, index) => Brick.Parse(line, index)).OrderBy(brick => brick.Start.Z).ToList();

        Dictionary<long, HashSet<long>> supportedByBricks = GetSupportedByForBricks(bricks);

        long count = 0;
        for (long id = 0; id < bricks.Count; id++)
        {
            HashSet<long> brokenBricks = [];
            HashSet<long> newBricks = [id];

            while (newBricks.Count > 0)
            {
                brokenBricks.UnionWith(newBricks);
                HashSet<long> next = supportedByBricks.Where(keyValue => keyValue.Value.Count > 0 && !brokenBricks.Contains(keyValue.Key) && keyValue.Value.IsSubsetOf(brokenBricks)).Select(keyValue => keyValue.Key).ToHashSet();
                newBricks = next;
            }

            count += brokenBricks.Count - 1;
        }

        return count.ToString();
    }

    private Dictionary<long, HashSet<long>> GetSupportedByForBricks(List<Brick> bricks)
    {
        Dictionary<Point3D, long> placedPoints = [];
        Dictionary<long, HashSet<long>> supportedByBricks = [];

        foreach (Brick brick in bricks)
        {
            HashSet<Point3D> previousPoints = brick.GetPoints();

            Brick currentBrick = brick;
            HashSet<Point3D> currentPoints = previousPoints;

            while (currentPoints.All(p => p.Z > 0) && currentPoints.All(p => !placedPoints.ContainsKey(p)))
            {
                previousPoints = currentPoints;

                currentBrick = currentBrick.GetLoweredBrick();
                currentPoints = currentBrick.GetPoints();
            }
            supportedByBricks[brick.Id] = currentPoints.Where(placedPoints.ContainsKey).Select(p => placedPoints[p]).ToHashSet();
            foreach (Point3D point in previousPoints)
            {
                placedPoints[point] = brick.Id;
            }
        }

        return supportedByBricks;
    }

    private static TestCase[] GetTestCases()
    {
        return [
            new TestCase(@"1,0,1~1,2,1
0,0,2~2,0,2
0,2,3~2,2,3
0,0,4~0,2,4
2,0,5~2,2,5
0,1,6~2,1,6
1,1,8~1,1,9", "5", "7"),
        ];
    }
}