using Nezbo.AdventOfCode.Extensions;

namespace Nezbo.AdventOfCode.Solutions;

internal class Day16Solution : ISolution
{
    public string SolvePartOne(string[] input)
    {
        Dictionary<string,Valve> valves = ParseInput(input);
        int released = CalculateMaximumPressureReleased(valves["AA"]);
        return released.ToString();
    }

    public string SolvePartTwo(string[] input)
    {
        throw new NotImplementedException();
    }

    private int CalculateMaximumPressureReleased(Valve startValve){
        return CalculateMaximumPressureReleased(startValve, 30, 0, 0, Enumerable.Empty<string>(), Enumerable.Empty<string>());
    }

    private int CalculateMaximumPressureReleased( 
        Valve current,
        int timeLeft,
        int released,
        int curFlow,
        IEnumerable<string> openValves,
        IEnumerable<string> visitedSinceOpen)
    {
        if(timeLeft == 0)
            return released;

        // progress
        released += curFlow;
        timeLeft--;

        // investigate moves
        int resultActionOpen = current.FlowRate == 0 || openValves.Contains(current.Name) 
            ? 0 
            : CalculateMaximumPressureReleased(
                current, 
                timeLeft, 
                released, 
                curFlow + current.FlowRate, 
                openValves.Append(current.Name), 
                Enumerable.Empty<string>());

        var possibleMoves = current.ConnectedValves
            .Except(v => visitedSinceOpen.Contains(v.Name));
        int resultsActionMove = possibleMoves.Count() == 0
            ? released 
            : possibleMoves
                .Select(v => CalculateMaximumPressureReleased(
                    v,
                    timeLeft,
                    released,
                    curFlow,
                    openValves,
                    visitedSinceOpen.Append(current.Name)
                ))
                .Max();
        
        return Math.Max(resultActionOpen, resultsActionMove);
    }

    private Dictionary<string,Valve> ParseInput(string[] input)
    {
        Dictionary<string,Valve> valves = input.Except(string.IsNullOrWhiteSpace)
            .Select(ParseCave)
            .ToDictionary(v => v.Name);
        valves.Values.ForEach(v => v.ConnectedValves.AddRange(v.Tunnels.Select(t => valves[t])));
        return valves;
    }

    private Valve ParseCave(string line)
    {
        var splitted = line.Split(' ');
        return new Valve
            {
                Name = splitted[1],
                FlowRate = int.Parse(splitted[4].Split('=').Last().Trim(';')),
                Tunnels = ParseTunnels(line)
            };
    }

    private string[] ParseTunnels(string line)
    {
        return line.Split(" to ")
            .Last()
            .Split(" ")
            .Skip(1)
            .Select(s => s.Trim(','))
            .ToArray();
    }

    private class Valve
    {
        public string Name { get; set; }
        public int FlowRate { get; set; }
        public string[] Tunnels { get; set; }
        public List<Valve> ConnectedValves { get; set; } = new List<Valve>();
    }
}