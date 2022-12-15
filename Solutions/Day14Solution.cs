using Nezbo.AdventOfCode.Extensions;

namespace Nezbo.AdventOfCode.Solutions;

internal class Day14Solution : ISolution
{
    public string SolvePartOne(string[] input)
    {
        var cave = ParseCave(input);

        int grains = SimulateSand(cave);
        
        return grains.ToString();
    }

    public string SolvePartTwo(string[] input)
    {
        var cave = ParseCave(input);

        int grains = SimulateSand(cave, bottomIsFloor: true);
        
        return grains.ToString();
    }

    private int SimulateSand(char[,] cave, bool bottomIsFloor = false)
    {
        int i = 0;
        while(true){
            (int,int) pos = (500,0);
            while(IsWithinCave(cave, pos)){
                var newPos = MoveGrain(cave,pos);
                if(newPos == pos)
                    break;
                pos = newPos;
            }
            if(!IsWithinCave(cave, pos) || pos == (500,0))
                return i;
            cave[pos.Item1,pos.Item2] = 'o';
            i++;
            PrintCave(cave, 494, 503);
        }
    }

    private (int,int) MoveGrain(char[,] cave, (int, int) pos)
    {
        if(pos.Item2 + 1 == cave.GetLength(1) 
            || cave[pos.Item1,pos.Item2 + 1] == '.')
            return (pos.Item1,pos.Item2 + 1);
        if(cave[pos.Item1 - 1,pos.Item2 + 1] == '.')
            return (pos.Item1 - 1,pos.Item2 + 1);
        if(cave[pos.Item1 + 1,pos.Item2 + 1] == '.')
            return (pos.Item1 + 1,pos.Item2 + 1);
        return pos;
    }

    private bool IsWithinCave(char[,] cave, (int, int) pos)
    {
        return pos.Item1 >= 0 && pos.Item2 >= 0
            && pos.Item1 < cave.GetLength(0)
            && pos.Item2 < cave.GetLength(1);
    }

    private void PrintCave(char[,] cave, int xMin, int xMax)
    {
        for(int y = 0; y < cave.GetLength(1); y++){
            Console.WriteLine(new string(Enumerable.Range(xMin,xMax-xMin+1).Select(x => cave[x,y]).ToArray()));
        }
    }

    private char[,] ParseCave(string[] input)
    {
        input = input.Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
        var coords = input.SelectMany(l => l.Split("->")).Select(c => c.Split(','));
        int xMax = coords.Max(c => int.Parse(c.First()));
        int yMax = coords.Max(c => int.Parse(c.Last()));

        char[,] cave = new char[xMax+1,yMax+1];
        SeedWithChar(cave, '.');

        cave[500,0] = '+';

        input.ForEach(l => {
            l.Split("->")
                .Select(s => { var splitted = s.Split(','); return new[]{int.Parse(splitted[0]), int.Parse(splitted[1])}; })
                .ForEachPair((e1,e2) => {
                    int drawAlong = e1[0] == e2[0] ? 0 : 1;
                    int otherAxis = drawAlong == 0 ? 1 : 0;
                    int min = Math.Min(e1[otherAxis], e2[otherAxis]);
                    int max = Math.Max(e1[otherAxis], e2[otherAxis]);
                    for(int c = min; c <= max; c++){
                        if(drawAlong == 0){
                            cave[e1[0], c] = '#';
                        }
                        else{
                            cave[c,e1[1]] = '#';
                        }
                    }
                });
        });

        return cave;
    }

    private void MakeFloorMaxY(char[,] cave)
    {
        int yMax = cave.GetLength(1) - 1;
        for(int x = 0; x < cave.GetLength(0); x++){
            cave[x,yMax] = '#';
        }
    }

    private void SeedWithChar(char[,] cave, char empty)
    {
        for(int x = 0; x < cave.GetLength(0); x++){
            for(int y = 0; y < cave.GetLength(1); y++){
                cave[x,y] = empty;
            }
        }
    }
}