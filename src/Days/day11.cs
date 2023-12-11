namespace AdventOfCode;

public class Day11 : BaseDay
{
    public Day11() : base(11, GetTestCases())
    {}

    protected override string SolvePartOne(string input) {
        return SolveForExpansionAmount(input, 2);
    }

    protected override string SolvePartTwo(string input) {
        return SolveForExpansionAmount(input, 1000000);
    }

    private string SolveForExpansionAmount(string input, int expansionAmount)
    {
        List<Point> galaxies = GetGalaxyLocations(input, expansionAmount);

        long total = 0;
        foreach ((Point galaxy, int index) in galaxies.Select((Point p, int index) => (p, index)))
        {
            foreach (Point otherGalaxy in galaxies.Skip(index + 1))
            {
                total += galaxy.GetVector(otherGalaxy).GetManhattenDist();
            }
        }

        return total.ToString();
    }

    private List<Point> GetGalaxyLocations(string input, int expansionAmount)
    {
        List<string> lines = SplitStringToLines(input);
        List<Point> galaxies = new List<Point>();

        for (int y = 0; y < lines.Count; y++) {
            string line = lines[y];
            for (int x = 0; x < line.Length; x++) {
                if (line.ElementAt(x).Equals('#')) {
                    galaxies.Add(new Point(x, y));
                }
            }
        }

        return ExpandSpace(galaxies, expansionAmount);
    }

    private List<Point> ExpandSpace(List<Point> initialGalaxies, int expansionAmount)
    {
        expansionAmount -= 1;
        HashSet<long> galaxyXLocations = initialGalaxies.Select(p => p.X).ToHashSet();
        HashSet<long> galaxyYLocations = initialGalaxies.Select(p => p.Y).ToHashSet();

        return initialGalaxies.Select(point => {
            long increaseX = point.X - galaxyXLocations.Where(x => x < point.X).Count();
            long increaseY = point.Y - galaxyYLocations.Where(y => y < point.Y).Count();
            return point.AddPoint(new Point(increaseX * expansionAmount, increaseY * expansionAmount));
        }).ToList();
    }

    private static TestCase[] GetTestCases()
    {
        return [
            new TestCase(@"...#......
.......#..
#.........
..........
......#...
.#........
.........#
..........
.......#..
#...#.....", "374", "82000210"),
        ];
    }
}