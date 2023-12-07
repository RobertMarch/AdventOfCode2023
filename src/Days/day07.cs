namespace AdventOfCode;

public class Day07 : BaseDay
{
    public Day07() : base(
        7,
        [
            new TestCase(@"32T3K 765
T55J5 684
KK677 28
KTJJT 220
QQQJA 483", "6440", "5905"),
        ]
    ) {}

    private class Hand
    {
        public Hand(string line, bool partOne)
        {
            string[] parts = line.Split( );
            Cards = parts[0].ToCharArray().Select(c => {
                switch (c) {
                    case 'A':
                        return 14;
                    case 'K':
                        return 13;
                    case 'Q':
                        return 12;
                    case 'J':
                        return partOne ? 11 : 1;
                    case 'T':
                        return 10;
                    default:
                        return int.Parse(c.ToString());
                }
            }).ToList();
            Bid = long.Parse(parts[1]);
            HandType = CalculateHandType();
            Strength = CalculateStrength();
        }

        public List<int> Cards { get; init; }
        public long Bid { get; init; }
        public int HandType { get; init; }
        public long Strength { get; init; }

        private int CalculateHandType()
        {
            Dictionary<int, int> cardValueCounts = new Dictionary<int, int>();

            // Initialise jokers to prevent error when accessing later
            cardValueCounts[1] = 0;

            Cards.ForEach((int card) => {
                if (!cardValueCounts.ContainsKey(card))
                {
                    cardValueCounts.Add(card, 0);
                }
                cardValueCounts[card] += 1;
            });

            // Get joker count and set to zero to prevent it being the max
            int jokerCount = cardValueCounts[1];
            cardValueCounts[1] = 0;

            IOrderedEnumerable<int> maxCardsCounts = cardValueCounts.Values.OrderDescending();
            // Increasing max count with jokers always maximises hand type strength
            int maxCardCount = maxCardsCounts.FirstOrDefault(0) + jokerCount;
            int secondMaxCardCount = maxCardsCounts.Skip(1).FirstOrDefault(0);

            int handType = 0;

            switch (maxCardCount) {
                case 5:
                    handType = 8;
                    break;
                case 4:
                    handType = 7;
                    break;
                case 3:
                    if (secondMaxCardCount == 2) {
                        handType = 6;
                    } else {
                        handType = 5;
                    }
                    break;
                case 2:
                    if (secondMaxCardCount == 2) {
                        handType = 4;
                    } else {
                        handType = 3;
                    }
                    break;
                case 1:
                    handType = 2;
                    break;
            }
            return handType;
        }

        private long CalculateStrength()
        {
            return long.Parse($"{HandType:D2}{Cards[0]:D2}{Cards[1]:D2}{Cards[2]:D2}{Cards[3]:D2}{Cards[4]:D2}");
        }

        public override string ToString()
        {
            return $"{string.Join(",", Cards)} {Bid} {HandType} {Strength}";
        }
    }

    protected override string SolvePartOne(string input) {
        return Solve(input, true).ToString();
    }

    protected override string SolvePartTwo(string input) {
        return Solve(input, false).ToString();
    }

    private string Solve(string input, bool partOne)
    {
        List<Hand> hands = SplitStringToLines(input).Select(line => new Hand(line, partOne)).OrderBy(hand => hand.Strength).ToList();

        long result = 0;

        for (int i = 0; i < hands.Count; i++)
        {
            result += (i + 1) * hands[i].Bid;
        }

        return result.ToString();
    }
}