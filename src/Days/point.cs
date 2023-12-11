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
