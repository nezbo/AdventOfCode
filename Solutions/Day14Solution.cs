using Nezbo.AdventOfCode.Collections;
using Nezbo.AdventOfCode.Extensions;

namespace Nezbo.AdventOfCode.Solutions;

internal class Day14Solution : ISolution
{
    private readonly bool DEBUG = false;

    public string SolvePartOne(string[] input)
    {
        var cave = ParseCave(input);

        int grains = SimulateSand(cave);
        
        return grains.ToString();
    }

    public string SolvePartTwo(string[] input)
    {
        var cave = ParseCave(input);

        int grains = SimulateSand(cave, virtualFloor: true);
        
        return (grains+1).ToString();
    }

    private int SimulateSand(SparseArray2D<char> cave, bool virtualFloor = false)
    {
        int virtualFloorY = cave.CurrentHeight + 1;
        int i = 0;
        while(true){
            (int,int) pos = (500,0);
            while(virtualFloor || IsWithinCave(cave, pos)){
                var newPos = MoveGrain(cave,pos);
                if(newPos == pos || (virtualFloor && newPos.Item2 == virtualFloorY))
                    break;
                pos = newPos;
            }
            if((!virtualFloor && !IsWithinCave(cave, pos)) || pos == (500,0))
                return i;
            cave[pos.Item1,pos.Item2] = 'o';
            i++;
            
            if(DEBUG)
                PrintCave(cave, 490, 510);
        }
    }

    private (int,int) MoveGrain(SparseArray2D<char> cave, (int, int) pos)
    {
        if(pos.Item2 + 1 == cave.CurrentHeight
            || cave[pos.Item1,pos.Item2 + 1] == '.')
            return (pos.Item1,pos.Item2 + 1);
        if(cave[pos.Item1 - 1,pos.Item2 + 1] == '.')
            return (pos.Item1 - 1,pos.Item2 + 1);
        if(cave[pos.Item1 + 1,pos.Item2 + 1] == '.')
            return (pos.Item1 + 1,pos.Item2 + 1);
        return pos;
    }

    private bool IsWithinCave(SparseArray2D<char> cave, (int, int) pos)
    {
        return pos.Item1 >= 0 && pos.Item2 >= 0
            && pos.Item1 < cave.CurrentWidth
            && pos.Item2 < cave.CurrentHeight;
    }

    private void PrintCave(SparseArray2D<char> cave, int xMin, int xMax)
    {
        for(int y = 0; y < cave.CurrentHeight; y++){
            Console.WriteLine(new string(Enumerable.Range(xMin,xMax-xMin+1).Select(x => cave[x,y]).ToArray()));
        }
    }

    private SparseArray2D<char> ParseCave(string[] input)
    {
        SparseArray2D<char> cave = new SparseArray2D<char>('.');

        cave[500,0] = '+';

        input.Where(l => !string.IsNullOrWhiteSpace(l))
            .ForEach(l => {
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
}