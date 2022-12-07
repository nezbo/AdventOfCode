using Nezbo.AdventOfCode;

namespace Nezbo.AdventOfCode.Solutions;

internal class Day6Solution : ISolution
{
    public string SolvePartOne(string[] input)
    {
        return DetectSignal(input[0], 4);
    }

    private static string DetectSignal(string input, int signalLength)
    {
        for (int i = signalLength; i <= input.Length; i++)
        {
            var slice = input.ToCharArray()[(i - signalLength)..i];
            if (new HashSet<char>(slice).Count == signalLength)
                return i.ToString();
        }
        return string.Empty;
    }

    public string SolvePartTwo(string[] input)
    {
        return DetectSignal(input[0], 14);
    }
}