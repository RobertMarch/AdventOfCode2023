using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day12 : BaseDay
{

    private record HistoryKey(string records, string groups);

    public Day12() : base(12, GetTestCases())
    {}

    protected override string SolvePartOne(string input)
    {
        return SplitStringToLines(input)
            .Select(line => GetArrangementCount(line, 1))
            .Sum()
            .ToString();
    }

    protected override string SolvePartTwo(string input)
    {
        return SplitStringToLines(input)
            .Select(line => GetArrangementCount(line, 5))
            .Sum()
            .ToString();
    }

    private long GetArrangementCount(string line, int repeatCount)
    {
        string[] parts = line.Split(" ");
        List<int> groupLengths = string.Join(",", Enumerable.Repeat(parts[1], repeatCount)).Split(",").Select(int.Parse).ToList();
        string records = Regex.Replace(string.Join("?", Enumerable.Repeat(parts[0], repeatCount)).Trim('.'), "\\.+", ".");

        int requiredGroupPlusSpaceLength = groupLengths.Sum() + groupLengths.Count - 1;

        if (requiredGroupPlusSpaceLength == records.Length
                || groupLengths.Sum() == records.Where(c => c != '.').Count()
                || groupLengths.Sum() == records.Where(c => c == '#').Count())
        {
            return 1;
        }

        while (records.StartsWith('#'))
        {
            records = records.Remove(0, groupLengths.First() + 1);
            groupLengths.RemoveAt(0);
        }

        while (records.EndsWith('#'))
        {
            records = records.Remove(records.Length - groupLengths.Last() - 1);
            groupLengths.RemoveAt(groupLengths.Count - 1);
        }
        
        if (groupLengths.Count == 0)
        {
            return 1;
        }

        long options = PlaceRemainingGroups(records, groupLengths, new Dictionary<HistoryKey, long>());

        return options;
    }

    private long PlaceRemainingGroups(string records, List<int> groupLengths, Dictionary<HistoryKey, long> history)
    {
        long count = 0;

        HistoryKey key = new HistoryKey(records, string.Join(",", groupLengths));
        if (history.ContainsKey(key))
        {
            return history[key];
        }

        int spacesToAdd = records.Length - (groupLengths.Sum() + groupLengths.Count - 1);
        int groupLengthToPlace = groupLengths.First();

        for (int i = 0; i <= spacesToAdd; i++)
        {
            IEnumerable<char> charactersBefore = records.Take(i);
            IEnumerable<char> groupCharacters = records.Skip(i).Take(groupLengthToPlace);
            IEnumerable<char> remainingCharacters = records.Skip(i + groupLengthToPlace);

            if (charactersBefore.Where(c => c == '#').Count() > 0
                    || groupCharacters.Where(c => c == '.').Count() > 0
                    || remainingCharacters.FirstOrDefault('.') == '#')
            {
                continue;
            }

            if (groupLengths.Count == 1)
            {
                if (remainingCharacters.Where(c => c == '#').Count() == 0)
                {
                    count += 1;
                }
            } else {
                count += PlaceRemainingGroups(records.Substring(i + groupLengthToPlace + 1), groupLengths.Skip(1).ToList(), history);
            }
        }
        history[key] = count;
        return count;
    }

    private static TestCase[] GetTestCases()
    {
        return [
            new TestCase(@"???.### 1,1,3
.??..??...?##. 1,1,3
?#?#?#?#?#?#?#? 1,3,1,6
????.#...#... 4,1,1
????.######..#####. 1,6,5
?###???????? 3,2,1", "21", "525152"),
        ];
    }
}