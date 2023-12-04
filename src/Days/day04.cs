namespace AdventOfCode;

public class Day04 : BaseDay
{
    public Day04() : base(
        4,
        [
            new TestCase(@"Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11", "13", "30"),
        ]
    ) {}

    protected override string SolvePartOne(string input) {
        double total = 0;
        foreach (string line in SplitStringToLines(input))
        {
            string[] parts = line.Split(new string[] {":", "|"}, StringSplitOptions.TrimEntries);

            HashSet<int> winningNumbers = SplitToNumberSet(parts[1]);
            HashSet<int> numbersYouHave = SplitToNumberSet(parts[2]);
            
            int overlap = winningNumbers.Intersect(numbersYouHave).Count();

            if (overlap > 0)
            {
                total += Math.Pow(2, overlap - 1);
            }
        }
        return total.ToString();
    }

    protected override string SolvePartTwo(string input) {
        List<string> lines = SplitStringToLines(input);
        int totalCards = lines.Count;

        Dictionary<int, long> cardCounts = new Dictionary<int, long>();

        for (int i = 0; i < totalCards; i++)
        {
            cardCounts.Add(i, 1);
        }

        for (int lineIndex = 0; lineIndex < totalCards; lineIndex++)
        {
            string[] parts = lines[lineIndex].Split(new string[] {":", "|"}, StringSplitOptions.TrimEntries);

            HashSet<int> winningNumbers = SplitToNumberSet(parts[1]);
            HashSet<int> numbersYouHave = SplitToNumberSet(parts[2]);
            
            int overlap = winningNumbers.Intersect(numbersYouHave).Count();

            for (int nextCard = 0; nextCard < overlap; nextCard++)
            {
                int nextCardIndex = nextCard + lineIndex + 1;
                if (nextCardIndex < totalCards) {
                    cardCounts[nextCardIndex] += cardCounts[lineIndex];
                }
            }
        }
        // Console.WriteLine(String.Join(", ", cardCounts.Select(e => e.ToString())));
        return cardCounts.Values.Sum().ToString();
    }

    private HashSet<int> SplitToNumberSet(string linePart)
    {
        return new HashSet<int>(
            linePart.Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
        );
    }
}