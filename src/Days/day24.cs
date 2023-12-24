namespace AdventOfCode;

public class Day24 : BaseDay
{
    public Day24() : base(24, GetTestCases())
    { }

    private enum Axis
    {
        X,
        Y,
        Z,
    }

    private record Hailstone(Dictionary<Axis, float> Position, Dictionary<Axis, float> Velocity)
    {
        public static Hailstone ParseLine(string line)
        {
            List<Axis> axisOrder = [Axis.X, Axis.Y, Axis.Z];
            List<Dictionary<Axis, float>> parts = line.Split(" @ ")
                .Select(coords => coords.Split(", ").Select((coord, index) => KeyValuePair.Create(axisOrder[index], float.Parse(coord))).ToDictionary()).ToList();
            return new Hailstone(parts[0], parts[1]);
        }

        public (float gradient, float intersect) GetXyEquation()
        {
            float gradient = Velocity[Axis.Y] / Velocity[Axis.X];
            float intersect = Position[Axis.Y] - Position[Axis.X] * gradient;
            return (gradient, intersect);
        }

        public float GetTimeAtXPosition(float xPosition)
        {
            return (xPosition - Position[Axis.X]) / Velocity[Axis.X];
        }

        public (float X, float Y, float Z) GetNormalisedVelocity()
        {
            float scale = Velocity[Axis.X] + Velocity[Axis.Y] + Velocity[Axis.Z];
            return (Velocity[Axis.X] / scale, Velocity[Axis.Y] / scale, Velocity[Axis.Z] / scale);
        }
    }

    protected override string SolvePartOne(string input)
    {
        List<Hailstone> hailstones = SplitStringToLines(input).Select(line => Hailstone.ParseLine(line)).ToList();

        return hailstones.Select((Hailstone h1, int index) =>
            hailstones.Skip(index + 1)
                .Where(h2 => IsLineIntersectValid(h1, h2))
                .Count()
        ).Sum().ToString();
    }

    private bool IsLineIntersectValid(Hailstone hailstone1, Hailstone hailstone2)
    {
        (float xIntersect, float yIntersect, float t1, float t2) = GetLinesIntersect(hailstone1, hailstone2);
        if (t1 == -1)
        {
            return false;
        }

        float minValue = 200000000000000;
        float maxValue = 400000000000000;
        return (minValue <= xIntersect && xIntersect <= maxValue)
            && (minValue <= yIntersect && yIntersect <= maxValue)
            && t1 >= 0
            && t2 >= 0;
    }

    private (float xIntersect, float yIntersect, float t1, float t2) GetLinesIntersect(Hailstone hailstone1, Hailstone hailstone2)
    {
        (float m1, float c1) = hailstone1.GetXyEquation();
        (float m2, float c2) = hailstone2.GetXyEquation();
        if (m1 == m2)
        {
            return (-1, -1, -1, -1);
        }

        float xIntersect = (c2 - c1) / (m1 - m2);
        float yIntersect = m1 * xIntersect + c1;
        float t1 = hailstone1.GetTimeAtXPosition(xIntersect);
        float t2 = hailstone2.GetTimeAtXPosition(xIntersect);

        return (xIntersect, yIntersect, t1, t2);
    }

    protected override string SolvePartTwo(string input)
    {
        List<Hailstone> hailstones = SplitStringToLines(input).Select(line => Hailstone.ParseLine(line)).ToList();

        Hailstone h1 = hailstones[0];
        Hailstone h2 = hailstones[1];
        Hailstone h3 = hailstones[3];

        for (long t1 = 0; t1 < 1000000000; t1++)
        {
            // Point3D p1 = 
            long t3 = 0; // Calculate this

            if (t1 % 100000 == 0)
            {
                Console.WriteLine(t1);
            }
        }

        HashSet<(float, float, float)> normalisedVelocitites = hailstones.Select(h => h.GetNormalisedVelocity()).ToHashSet();

        Console.WriteLine($"{hailstones.Count}, {normalisedVelocitites.Count}");
        return "Not yet implemented";
    }

    private static TestCase[] GetTestCases()
    {
        return [
            new TestCase(@"19, 13, 30 @ -2,  1, -2
18, 19, 22 @ -1, -1, -2
20, 25, 34 @ -2, -2, -4
12, 31, 28 @ -1, -2, -1
20, 19, 15 @  1, -5, -3", "2", "47"),
        ];
    }
}