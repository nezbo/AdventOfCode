namespace Nezbo.AdventOfCode;

internal class Program
{
    private static void Main(string[] args)
    {
        for(int day = 1; day <= 25; day++)
        {
            ISolution solution = GetSolutionInstance(day);
            string inputFilename = GetInputFilename(day);

            if(solution == null || !File.Exists(inputFilename))
                continue;

            string[] input = File.ReadAllLines(inputFilename);
            
            Console.WriteLine($"--- Day {day} ---");
            Console.WriteLine($"Part One: {solution.SolvePartOne(input)}");
            Console.WriteLine($"Part Two: {solution.SolvePartTwo(input)}");
        }
    }

    private static ISolution GetSolutionInstance(int day)
    {
        var typeMatch = typeof(Program).Assembly
            .DefinedTypes
            .Where(t => t.ImplementedInterfaces.Contains(typeof(ISolution)))
            .SingleOrDefault(t => t.Name.Equals($"Day{day}Solution"));

        if(typeMatch == null)
            return null;
        return Activator.CreateInstance(typeMatch) as ISolution ?? null;
    }

    private static string GetInputFilename(int day) => $"Data/day{day}input.txt";
    

}