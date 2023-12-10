namespace AdventOfCode;
    
public record Point(int X, int Y)
{
    public Point AddPoint(Point other)
    {
        return new Point(X + other.X, Y + other.Y);
    }
}