using Nezbo.AdventOfCode.Extensions;

namespace Nezbo.AdventOfCode.Solutions;

internal class Day8Solution : ISolution
{
    public string SolvePartOne(string[] input)
    {
        int[,] forest = ParseForest(input);
        int w = forest.GetLength(0);
        int h = forest.GetLength(1);

        int edgeCount = w * 2 + h * 2 - 4;
        int counter = 0;
        for(int c = 1; c < w - 1; c++){
            var col = forest.SliceColumn(c).ToArray();
            for(int r = 1; r < h - 1; r++){
                var row = forest.SliceRow(r).ToArray();
                var cur = forest[c,r];
                
                if(row[..c].All(n => n < cur) // left
                    || row[(c+1)..].All(n => n < cur) // right
                    || col[..r].All(n => n < cur) // top
                    || col[(r+1)..].All(n => n < cur) // bottom
                ){
                    counter++;
                }
            }
        }

        return (edgeCount + counter).ToString();
    }

    private int[,] ParseForest(string[] input)
    {
        int[,] result = new int[input[0].Length, input.Count(l => !string.IsNullOrEmpty(l))];
        input.ForEach((l,i) => l.ForEach((c,j) => result[j,i] = c - '0'));

        return result;
    }

    public string SolvePartTwo(string[] input)
    {
        int[,] forest = ParseForest(input);
        int w = forest.GetLength(0);
        int h = forest.GetLength(1);

        int edgeCount = w * 2 + h * 2 - 4;
        int maxScore = 0;
        for(int c = 1; c < w - 1; c++){
            var col = forest.SliceColumn(c).ToArray();
            for(int r = 1; r < h - 1; r++){
                var row = forest.SliceRow(r).ToArray();
                var cur = forest[c,r];
                
                var left = ViewDist(row[..c].Reverse(),cur);
                var right = ViewDist(row[(c+1)..],cur);
                var top = ViewDist(col[..r].Reverse(),cur);
                var bottom = ViewDist(col[(r+1)..],cur);

                var score = left * right * top * bottom;
                if(score > maxScore)
                    maxScore = score;
            }
        }

        return maxScore.ToString();
    }

    private int ViewDist(IEnumerable<int> view, int cur)
    {
        return view.TakeWhile(i => i < cur).Count() + (view.Any(i => i >= cur) ? 1 : 0);
    }
}