using Nezbo.AdventOfCode.Extensions;

namespace Nezbo.AdventOfCode.Solutions;

internal class Day12Solution : ISolution
{
    public string SolvePartOne(string[] input)
    {
        char[,] map = ParseMap(input);
        (int,int) pos = FindPositions(map, 'S').Single();
        IEnumerable<(int,int)> path = FindShortestPath(map, pos);
    
        return path.Count().ToString();
    }

    public string SolvePartTwo(string[] input)
    {
        char[,] map = ParseMap(input);
        var aPos = FindPositions(map, 'a');
        return aPos.Min(p => FindShortestPath(map, p)?.Count() ?? int.MaxValue).ToString();
    }

    private char[,] ParseMap(string[] input)
    {
        char[,] map = new char[input[0].Length, input.Count(l => !string.IsNullOrWhiteSpace(l))];
        input.ForEach((l,y) => l.ForEach((c,x) => map[x,y] = c));
        return map;
    }

    private IEnumerable<(int, int)> FindPositions(char[,] map, char target)
    {
        for(int y = 0; y < map.GetLength(1); y++){
            for(int x = 0; x < map.GetLength(0); x++){
                if(map[x,y] == target)
                    yield return (x,y);
            }
        }
    }

    private IEnumerable<(int, int)> FindShortestPath(char[,] map, 
        (int, int) startPos)
    {
        var goalPos = FindPositions(map, 'E').Single();
        var pathTo = new Dictionary<(int,int),(int,int)>();
        var edges = new HashSet<(int,int)>();
        edges.Add(startPos);

        while(edges.Count > 0 && !pathTo.ContainsKey(goalPos)){
            var curEdges = edges.ToList();
            edges.Clear();
            foreach(var pos in curEdges){
                GetPossibleDirections(map, pos)
                    .Except(pathTo.Keys)
                    .ForEach(p => { pathTo.Add(p, pos); edges.Add(p); });
            }
        }

        if(pathTo.ContainsKey(goalPos))
            return RetracePath(pathTo, startPos, goalPos);
        
        return null;
    }

    private IEnumerable<(int, int)> RetracePath(Dictionary<(int, int), (int, int)> pathTo, (int,int) startPos, (int, int) goalPos)
    {
        var curPos = goalPos;
        while(curPos != startPos){
            yield return curPos;
            curPos = pathTo[curPos];
        }
    }

    private IEnumerable<(int,int)> GetPossibleDirections(char[,] map, (int, int) pos)
    {
        return GetDirections(pos)
            .Where(d => IsWithinMap(map, d))
            .Where(d => NotTooSteep(map, pos, d));
    }

    private bool NotTooSteep(char[,] map, (int, int) from, (int, int) to)
    {
        char cur = map[from.Item1, from.Item2];
        char next = map[to.Item1, to.Item2];

        return (next >= 'a' && next <= cur + 1)
            || (cur == 'S' && next <= 'b') 
            || (cur >= 'y' && next == 'E');
    }

    private bool IsWithinMap(char[,] map, (int, int) pos)
    {
        return pos.Item1 >= 0 && pos.Item1 < map.GetLength(0)
            && pos.Item2 >=0 && pos.Item2 < map.GetLength(1);
    }

    private IEnumerable<(int,int)> GetDirections((int, int) pos)
    {
        yield return (pos.Item1 - 1, pos.Item2);
        yield return (pos.Item1 + 1, pos.Item2);
        yield return (pos.Item1, pos.Item2 - 1);
        yield return (pos.Item1, pos.Item2 + 1);
    }
}