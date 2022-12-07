namespace Nezbo.AdventOfCode;

public class Day2Solution : ISolution
{
    private enum Shape {
        Rock = 1,
        Paper = 2,
        Scissor = 3
    }

    public string SolvePartOne(string[] input)
    {
        return input
        .Select(ParseLine)
        .Select(GetRoundResult)
        .Sum()
        .ToString();
    }

    private Tuple<Shape,Shape> ParseLine(string line){
        return new Tuple<Shape, Shape>(ParseChoice(line[0]), ParseChoice(line[2]));
    }

    private Shape ParseChoice(char c){
        return c switch {
            'A' => Shape.Rock,
            'B' => Shape.Paper,
            'C' => Shape.Scissor,
            'X' => Shape.Rock,
            'Y' => Shape.Paper,
            'Z' => Shape.Scissor,
            _ => throw new InvalidDataException()
        };
    }

    private int GetRoundResult(Tuple<Shape,Shape> round)
    {
        return PointsOutcome(round) + (int)round.Item2;
    }

    private int PointsOutcome(Tuple<Shape, Shape> round)
    {
        return round switch{
            (Shape.Paper, Shape.Rock) => 0,
            (Shape.Paper, Shape.Scissor) => 6,
            (Shape.Rock, Shape.Paper) => 6,
            (Shape.Rock, Shape.Scissor) => 0,
            (Shape.Scissor, Shape.Rock) => 6,
            (Shape.Scissor, Shape.Paper) => 0,
            _ => 3
        };
    }

    public string SolvePartTwo(string[] input)
    {
        return input
        .Select(ParseLineTwo)
        .Select(GetRoundResult)
        .Sum()
        .ToString();
    }

        private Tuple<Shape,Shape> ParseLineTwo(string line){
        Shape item1 = ParseChoice(line[0]);
        Shape item2 = DetermineChoice(item1, line[2]);
        return new Tuple<Shape, Shape>(item1, item2);
    }

    private Shape DetermineChoice(Shape them, char rule)
    {
        return (them, rule) switch {
            (Shape.Rock, 'X') => Shape.Scissor,
            (Shape.Rock, 'Z') => Shape.Paper,
            (Shape.Paper, 'X') => Shape.Rock,
            (Shape.Paper, 'Z') => Shape.Scissor,
            (Shape.Scissor, 'X') => Shape.Paper,
            (Shape.Scissor, 'Z') => Shape.Rock,
            _ => them
        };
    }
}