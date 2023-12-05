namespace AdventOfCode;

public class Day05 : BaseDay
{
    public Day05() : base(
        5,
        [
            new TestCase(@"seeds: 79 14 55 13

seed-to-soil map:
50 98 2
52 50 48

soil-to-fertilizer map:
0 15 37
37 52 2
39 0 15

fertilizer-to-water map:
49 53 8
0 11 42
42 0 7
57 7 4

water-to-light map:
88 18 7
18 25 70

light-to-temperature map:
45 77 23
81 45 19
68 64 13

temperature-to-humidity map:
0 69 1
1 0 69

humidity-to-location map:
60 56 37
56 93 4", "35", "46"),
        ]
    ) {}

    private record MappingRange(long SourceStart, long DestinationStart, long Length)
    {
        // Gets the non-inclusive end value
        public long GetEnd()
        {
            return SourceStart + Length;
        }

        public long GetNewValue(long valueToMap) {
            return valueToMap - SourceStart + DestinationStart;
        }
    }

    private static MappingRange DefaultMapping = new MappingRange(0, 0, long.MaxValue);

    private class SeedRange()
    {
        public SeedRange(long Start, long Length) : this()
        {
            this.Start = Start;
            this.Length = Length;
        }

        public long Start { get; set; }
        public long Length { get; set; }

        public long LastValue() {
            return Start + Length - 1;
        }

        public SeedRange SplitRange(long splitPosition)
        {
            long oldLength = Length;
            Length = splitPosition - Start;

            return new SeedRange(splitPosition, oldLength - Length);
        }

        public override string ToString()
        {
            return $"{{Start: {Start}, Length: {Length}}}";
        }
    }

    protected override string SolvePartOne(string input) {
        List<string> inputBlocks = new List<string>(input.Split(new string[] {"\r\n\r\n", "\r\r", "\n\n"}, StringSplitOptions.None));

        List<long> seedNumbers = inputBlocks[0].Split(" ").Skip(1).Select(long.Parse).ToList();

        List<List<MappingRange>> mappings = ParseMappings(inputBlocks);

        long minSeed = long.MaxValue;

        foreach (long seed in seedNumbers)
        {
            long currentValue = seed;

            foreach (List<MappingRange> mappingStage in mappings)
            {
                MappingRange range = mappingStage.Where(mapping => mapping.SourceStart <= currentValue && mapping.SourceStart + mapping.Length > currentValue)
                    .FirstOrDefault(DefaultMapping);

                currentValue = range.GetNewValue(currentValue);
            }

            minSeed = Math.Min(minSeed, currentValue);
        }

        return minSeed.ToString();
    }

    protected override string SolvePartTwo(string input) {
        List<string> inputBlocks = new List<string>(input.Split(new string[] {"\r\n\r\n", "\r\r", "\n\n"}, StringSplitOptions.None));

        List<SeedRange> seedRanges = inputBlocks[0].Split(" ")
            .Skip(1)
            .Select(long.Parse)
            .Chunk(2)
            .Select(parts => new SeedRange(parts[0], parts[1]))
            .ToList();

        List<List<MappingRange>> mappings = ParseMappings(inputBlocks);

        foreach (List<MappingRange> mappingStage in mappings)
        {
            for (int seedIndex = 0; seedIndex < seedRanges.Count(); seedIndex++)
            {
                SeedRange seedRange = seedRanges[seedIndex];
                MappingRange mappingRange = mappingStage.Where(mapping =>
                        mapping.SourceStart <= seedRange.LastValue()
                        && mapping.GetEnd() > seedRange.Start)
                    .OrderBy(m => m.SourceStart)
                    .FirstOrDefault(DefaultMapping);

                if (mappingRange.GetEnd() <= seedRange.LastValue()) {
                    SeedRange newSeedRange = seedRange.SplitRange(mappingRange.GetEnd());
                    
                    seedRanges.Add(newSeedRange);
                }

                seedRange.Start = mappingRange.GetNewValue(seedRange.Start);
            }
        }

        return seedRanges.Select(range => range.Start).Min().ToString();
    }

    private List<List<MappingRange>> ParseMappings(List<string> inputBlocks)
    {
        List<List<MappingRange>> mappings = inputBlocks.Skip(1)
            .Select(block => {
                List<string> lines = SplitStringToLines(block);
                List<MappingRange> definedRanges = lines.Skip(1).Select(line => {
                    List<long> parts = line.Split(" ").Select(long.Parse).ToList();
                    return new MappingRange(parts[1], parts[0], parts[2]);
                }).OrderBy(v => v.SourceStart).ToList();

                long val = 0;

                while (val < 10000000000)
                {
                    IEnumerable<MappingRange> overlappingRanges = definedRanges.Where(range => range.SourceStart == val);
                    if (overlappingRanges.Count() == 1)
                    {
                        MappingRange range = overlappingRanges.First();
                        val = range.GetEnd();
                        continue;
                    }

                    MappingRange nextRange = definedRanges.Where(range => range.SourceStart > val)
                        .OrderBy(r => r.SourceStart)
                        .FirstOrDefault(new MappingRange(10000000000, 10000000000, 1));

                    definedRanges.Add(new MappingRange(val, val, nextRange.SourceStart - val));
                    val = nextRange.GetEnd();
                }

                return definedRanges;
            }).ToList();

        return mappings;
    }
}