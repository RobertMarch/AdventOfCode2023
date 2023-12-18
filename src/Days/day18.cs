namespace AdventOfCode;

public class Day18 : BaseDay
{
    public Day18() : base(18, GetTestCases())
    {}

    private static Dictionary<char, Point> DirectionVectorsFromChar = new Dictionary<char, Point>{
        { 'U', new Point(0, -1) },
        { 'R', new Point(1, 0) },
        { 'D', new Point(0, 1) },
        { 'L', new Point(-1, 0) },
    };

    private static Dictionary<int, Point> DirectionVectorsFromInt = new Dictionary<int, Point>{
        { 3, new Point(0, -1) },
        { 0, new Point(1, 0) },
        { 1, new Point(0, 1) },
        { 2, new Point(-1, 0) },
    };

    protected override string SolvePartOne(string input)
    {
        (List<Point> cornerPoints, long edgePointCount) = CalculateEdgeCorners(input, true);
        return CalculateArea(cornerPoints, edgePointCount).ToString();
    }

    protected override string SolvePartTwo(string input)
    {
        (List<Point> cornerPoints, long edgePointCount) = CalculateEdgeCorners(input, false);
        return CalculateArea(cornerPoints, edgePointCount).ToString();
    }

    private (List<Point>, long) CalculateEdgeCorners(string input, bool partOne)
    {
        long edgePointCount = 0;
        Point currentLocation = new Point(0, 0);
        List<Point> edgeCorners = [currentLocation];

        SplitStringToLines(input).ForEach((string line) => {
            (long distance, Point direction) = ParseDistanceAndDirection(line, partOne);

            Point edgeVector = new Point(direction.X * distance, direction.Y * distance);
            
            currentLocation = currentLocation.AddPoint(edgeVector);
            edgeCorners.Add(currentLocation);
            edgePointCount += distance;
        });
        return (edgeCorners, edgePointCount);
    }

    private (long, Point) ParseDistanceAndDirection(string line, bool partOne)
    {
        if (partOne)
        {
            string[] parts = line.Split(" ");
            Point direction = DirectionVectorsFromChar[parts[0][0]];
            return (long.Parse(parts[1]), direction);
        }
        else
        {
            string hexCode = line.Split(" ")[2];
            long distance = long.Parse(hexCode.Substring(2, 5), System.Globalization.NumberStyles.HexNumber);
            Point direction = DirectionVectorsFromInt[int.Parse(hexCode[7].ToString())];
            return (distance, direction);
        }
    }

    // Use shoelace formula and adjust for inclusion of edge points
    private long CalculateArea(List<Point> corners, long edgePointCount)
    {
        long area = 0;

        for (int i = 0; i < corners.Count - 1; i++)
        {
            Point p1 = corners[i];
            Point p2 = corners[i + 1];

            area += p1.X * p2.Y - p1.Y * p2.X;
        }

        return area / 2 + edgePointCount / 2 + 1;
    }

    private static TestCase[] GetTestCases()
    {
        return [
            new TestCase(@"R 6 (#70c710)
D 5 (#0dc571)
L 2 (#5713f0)
D 2 (#d2c081)
R 2 (#59c680)
D 2 (#411b91)
L 5 (#8ceee2)
U 2 (#caa173)
L 1 (#1b58a2)
U 2 (#caa171)
R 2 (#7807d2)
U 3 (#a77fa3)
L 2 (#015232)
U 2 (#7a21e3)", "62", "952408144115"),
        ];
    }
}