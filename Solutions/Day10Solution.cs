using System.Text;

namespace Nezbo.AdventOfCode.Solutions;

internal class Day10Solution : ISolution
{
    public string SolvePartOne(string[] input)
    {
        List<int> values = ParseAndRunInstructions(input);

        return values.Select((v, i) => new { i = i + 1, v })
            .Where(o => o.i == 20 || (o.i - 20) % 40 == 0)
            .Sum(o => o.i * o.v)
            .ToString();
    }

    private static List<int> ParseAndRunInstructions(string[] input)
    {
        var instructions = input.Select(l => { var s = l.Split(' '); return new { command = s[0], val = (s.Length > 1 ? int.Parse(s[1]) : 0) }; });

        List<int> values = new(input.Length);
        int lastValue = 1;
        foreach (var inst in instructions)
        {
            switch (inst.command)
            {
                case "noop": values.Add(lastValue); break;
                case "addx": values.Add(lastValue); values.Add(lastValue); lastValue += inst.val; break;
                default: break;
            }
        }

        return values;
    }

    public string SolvePartTwo(string[] input)
    {
        List<int> values = ParseAndRunInstructions(input);

        StringBuilder b = new StringBuilder("\n");
        for(int i = 0; i < values.Count; i++){
            int x = i % 40;
            bool hasSprite = Math.Abs(values[i] - x) <= 1;
            b.Append(hasSprite ? '#' : '.');

            if(x == 39)
                b.Append('\n');
        }

        return b.ToString();
    }
}