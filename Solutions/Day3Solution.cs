using Nezbo.AdventOfCode.Extensions;

namespace Nezbo.AdventOfCode;

public class Day3Solution : ISolution
{
    public string SolvePartOne(string[] input)
    {
        return input
            .Where(l => !string.IsNullOrEmpty(l))
            .Select(GetOverlapCharacter)
            .Select(GetPriority)
            .Sum()
            .ToString();
    }

    private int GetPriority(char item)
    {
        if(char.IsUpper(item))
            return (int)item - 65 + 27;
        return (int)item - 96;
    }

    private char GetOverlapCharacter(string line){
        string firstCompartment = line.Substring(0, line.Length/2);
        string secondCompartment = line.Substring(line.Length/2);

        return GetOverlapCharacters(firstCompartment, secondCompartment).First();
    }

    private IEnumerable<char> GetOverlapCharacters(string first, string second)
    {
        var bag = new HashSet<char>(first);
        return second.Where(bag.Contains);
    }

    public string SolvePartTwo(string[] input)
    {
        return input.SplitEvery(3)
            .Select(GetCommonItem)
            .Select(GetPriority)
            .Sum()
            .ToString();
    }

    private char GetCommonItem(IEnumerable<string> lines)
    {
        var commonFirstTwo = GetOverlapCharacters(lines.First(), lines.Skip(1).First());
        return GetOverlapCharacters(new string(commonFirstTwo.ToArray()), lines.Last()).First();
    }
}