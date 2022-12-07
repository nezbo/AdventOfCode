using Nezbo.AdventOfCode.Extensions;

namespace Nezbo.AdventOfCode;

internal class Day1Solution : ISolution
{
    public string SolvePartOne(string[] input)
    {
        return input
            .SplitAt(string.IsNullOrEmpty)
            .Select(g => g.Sum(int.Parse))
            .Max()
            .ToString();
    }

    public string SolvePartTwo(string[] input)
    {
        return input
            .SplitAt(string.IsNullOrEmpty)
            .Select(g => g.Sum(int.Parse))
            .OrderDescending()
            .Take(3)
            .Sum()
            .ToString();
    }
}