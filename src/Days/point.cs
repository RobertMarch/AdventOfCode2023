namespace AdventOfCode;

public record Point(long X, long Y)
{
    public Point AddPoint(Point other)
    {
        return new Point(X + other.X, Y + other.Y);
    }

    public Point GetVector(Point other)
    {
        return new Point(X - other.X, Y - other.Y);
    }

    public long GetManhattenDist()
    {
        return Math.Abs(X) + Math.Abs(Y);
    }
}

public record Point3D(long X, long Y, long Z)
{
    public Point3D AddPoint(Point3D other)
    {
        return new Point3D(X + other.X, Y + other.Y, Z + other.Z);
    }

    public Point3D GetVector(Point3D other)
    {
        return new Point3D(X - other.X, Y - other.Y, Z - other.Z);
    }

    public long GetManhattenDist()
    {
        return Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
    }
}
