namespace AdventOfCode;

public class Day03 : BaseDay
{
    private record Point(int X, int Y);

    public Day03() : base(
        3,
        [
            new TestCase(@"467..114..
...*......
..35..633
......#...
617*......
.....+.58
..592.....
......755
...$.*....
.664.598..", "4361", "467835"),
        ]
    ) {}

    protected override string SolvePartOne(string input) {
        (HashSet<Point> symbolLocations, Dictionary<Point, int> partNumberLocations) = BuildLocationMaps(input, true);

        int total = 0;
        foreach (KeyValuePair<Point, int> partNumber in partNumberLocations)
        {
            if (GetPointsToCheck(partNumber.Key, partNumber.Value).Any(p => symbolLocations.Contains(p))) {
                total += partNumber.Value;
            }
        }

        return total.ToString();
    }

    protected override string SolvePartTwo(string input) {
        (HashSet<Point> gearLocations, Dictionary<Point, int> partNumberLocations) = BuildLocationMaps(input, true);

        int total = 0;
        foreach (Point gear in gearLocations)
        {
            IEnumerable<int> neighbours = GetPointsToCheck(new Point(gear.X - 2, gear.Y), 111)
                .Where(p => partNumberLocations.ContainsKey(p) && p.X + partNumberLocations[p].ToString().Length > gear.X - 1)
                .Select(p => partNumberLocations[p]);
            if (neighbours.Count() == 2) {
                total += neighbours.First() * neighbours.Last();
            }
        }

        return total.ToString();
    }

    private (HashSet<Point>, Dictionary<Point, int>) BuildLocationMaps(string input, bool onlyCheckGears)
    {
        List<string> lines = SplitStringToLines(input);
        HashSet<Point> symbolLocations = new HashSet<Point>();
        Dictionary<Point, int> partNumberLocations = new Dictionary<Point, int>();

        for (int y = 0; y < lines.Count; y++) {
            string line = lines[y];
            string currentNumber = "";
            for (int x = 0; x < line.Length; x++) {
                char character = line.ElementAt(x);
                if (Char.IsDigit(character)) {
                    currentNumber += character;
                    continue;
                }
                if (!character.Equals('.') && (!onlyCheckGears || character.Equals('*'))) {
                    symbolLocations.Add(new Point(x, y));
                }
                if (currentNumber.Length > 0) {
                    partNumberLocations.Add(new Point(x - currentNumber.Length, y), int.Parse(currentNumber));
                    currentNumber = "";
                }
            }
            if (currentNumber.Length > 0) {
                partNumberLocations.Add(new Point(line.Length - currentNumber.Length, y), int.Parse(currentNumber));
            }
        }

        return (symbolLocations, partNumberLocations);
    }

    private List<Point> GetPointsToCheck(Point partNumberStart, int partNumberValue) {
        List<Point> points = new List<Point>();
        for (int x = partNumberStart.X - 1; x <= partNumberStart.X + partNumberValue.ToString().Length; x++) {
            for (int y = partNumberStart.Y - 1; y <= partNumberStart.Y + 1; y++) {
                points.Add(new Point(x, y));
            }
        }
        return points;
    }
}