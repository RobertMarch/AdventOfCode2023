namespace AdventOfCode;

public class Day02 : BaseDay
{
    public Day02() : base(
        2,
        [
            new TestCase(@"Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green", "8", "2286"),
        ]
    ) {}

    protected override string SolvePartOne(string input) {
        List<string> lines = SplitStringToLines(input);
        int total = 0;
        foreach (string line in lines)
        {
            string[] parts = line.Split(new string[] {":", ";", ","}, StringSplitOptions.TrimEntries);

            int gameNumber = int.Parse(parts[0].Split(" ")[1]);

            if (parts.Skip(1).All(part => {
                string[] cubeParts = part.Split(" ");
                int count = int.Parse(cubeParts[0]);
                string colour = cubeParts[1];
                switch (colour) {
                    case "red":
                        return count <= 12;
                    case "green":
                        return count <= 13;
                    case "blue":
                        return count <= 14;
                }
                
                return false;
            }))
            {
                total += gameNumber;
            }
        }

        return total.ToString();
    }

    protected override string SolvePartTwo(string input) {
        List<string> lines = SplitStringToLines(input);
        int total = 0;
        foreach (string line in lines)
        {
            string[] parts = line.Split(new string[] {":", ";", ","}, StringSplitOptions.TrimEntries);

            int gameNumber = int.Parse(parts[0].Split(" ")[1]);

            int red = 0;
            int green = 0;
            int blue = 0;

            foreach (string part in parts.Skip(1))
            {
                string[] cubeParts = part.Split(" ");
                int count = int.Parse(cubeParts[0]);
                string colour = cubeParts[1];
                switch (colour) {
                    case "red":
                        red = Math.Max(red, count);
                        break;
                    case "green":
                        green = Math.Max(green, count);
                        break;
                    case "blue":
                        blue = Math.Max(blue, count);
                        break;
                }
            }

            total += red * green * blue;
        }

        return total.ToString();
    }

    private (string, int) ParseCubeInfo(string info)
    {
        string[] cubeParts = info.Split(" ");
        int count = int.Parse(cubeParts[0]);
        string colour = cubeParts[1];
        return (colour, count);
    }
}