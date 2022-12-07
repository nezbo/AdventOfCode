namespace Nezbo.AdventOfCode.Solutions;

internal class Day4Solution : ISolution
{
    public string SolvePartOne(string[] input)
    {
        return input.Select(ParseRanges)
            .Count(t => t.Item1.Contains(t.Item2) || t.Item2.Contains(t.Item1))
            .ToString();
    }

    private Tuple<Range,Range> ParseRanges(string line)
    {
        var splitted = line.Split(',');
        return new Tuple<Range,Range>(ParseRange(splitted[0]), ParseRange(splitted[1]));
    }

    private Range ParseRange(string range)
    {
        var splitted = range.Split('-');
        return new Range(int.Parse(splitted[0]), int.Parse(splitted[1]));
    }

    public string SolvePartTwo(string[] input)
    {
        return input.Select(ParseRanges)
            .Count(t => t.Item1.Overlaps(t.Item2))
            .ToString();
    }
}