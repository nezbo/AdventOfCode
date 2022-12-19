using Nezbo.AdventOfCode.Collections;
using Nezbo.AdventOfCode.Extensions;

namespace Nezbo.AdventOfCode.Solutions;

internal class Day15Solution : ISolution
{
    public string SolvePartOne(string[] input)
    {
        Cave cave = ParseSensors(input);
        cave.MarkEmptySpaces();
        //cave.Map.PrintGrid(c => c, -2, 25, -2, 22);

        return cave.GetConfirmedEmptySpaces(2_000_000).Count().ToString();
    }

    public string SolvePartTwo(string[] input)
    {
        Cave cave = ParseSensors(input);
        cave.MarkEmptySpaces();
        var pos = FindSingleUnknownSpace(cave);
        return (4000000 * pos.Item1 + pos.Item2).ToString();
    }

    private static (int,int) FindSingleUnknownSpace(Cave cave)
    {
        int i = 0;
        return Enumerable.Range(0, 4_000_000)
            .AsParallel()
            .AsUnordered()
            .Select(y =>
            {
                int x = cave.GetUnknownSpaces(y, 0, 4_000_000).FirstOrDefault();
                if(i++ % 1_000 == 0)
                    Console.WriteLine($"{i*100 / 4_000_000}% done");
                if(x != 0)
                    return (x,y);
                return (-1,-1);
            })
            .Where(t => t.Item1 >= 0 && t.Item2 >= 0)
            .Take(1)
            .FirstOrDefault();
    }

    private Cave ParseSensors(string[] input)
    {
        var cave = new Cave();
        input
            .Except(string.IsNullOrEmpty)
            .Select(ParseCoordinates)
            .ForEach(ps => cave.AddSensor(ps.Item1, ps.Item2));
        return cave;
    }

    private ((int,int),(int,int)) ParseCoordinates(string line)
    {
        var bySpace = line.Split(' ');
        var sensor = (GetNumber(bySpace[2]), GetNumber(bySpace[3]));
        var beacon = (GetNumber(bySpace[8]), GetNumber(bySpace[9]));
        return (sensor, beacon);
    }

    private int GetNumber(string str)
    {
        return int.Parse(str.Split('=').Last().TrimEnd(',', ':'));
    }

    private struct Line{
        public int Min;
        public int Max;
    }

    private class Cave {
        //public SparseArray2D<char> Map { get; } = new ('.');
        public Dictionary<(int,int),(int,int)> ClosestBeacon = new ();
        public Dictionary<int,List<Line>> RowEmptyRanges = new Dictionary<int, List<Line>>();

        public void AddSensor((int,int) sensor, (int,int) closestBeacon){
                //Map[sensor.Item1,sensor.Item2] = 'S';
                //Map[closestBeacon.Item1,closestBeacon.Item2] = 'B';

                ClosestBeacon[sensor] = closestBeacon;
        }

        public void MarkEmptySpaces()
        {
            foreach(var beacon in ClosestBeacon){
                //Console.WriteLine($"Starting beacon ({beacon.Key.Item1},{beacon.Key.Item2})");
                var c = beacon.Key;
                var dist = beacon.Key.ManhattanDistance(beacon.Value);
                //Console.WriteLine($"Distance = {dist}");
                for(int y = c.Item2 - dist; y <= c.Item2 + dist; y++){
                    //Console.WriteLine($"Marking line {y}");
                    int xMin = c.Item1 - dist + Math.Abs(y - c.Item2);
                    int xMax = c.Item1 + dist - Math.Abs(y - c.Item2);
                    AddEmptyRange(y, xMin, xMax);
                    /*for(int x = c.Item1 - dist + Math.Abs(y - c.Item2); x <= c.Item1 + dist - Math.Abs(y - c.Item2); x++){
                        if(Map[x,y] == '.'){
                            Map[x,y] = '#';
                            i++;
                        }
                    }*/
                }
            }
        }

        public IEnumerable<int> GetConfirmedEmptySpaces(int y){
            var ranges = RowEmptyRanges[y];
            int xMin = ranges.Min(r => r.Item1);
            int xMax = ranges.Max(r => r.Item2);
            HashSet<int> markers = new HashSet<int>(ClosestBeacon
                .SelectMany(sb => new List<(int,int)>{ sb.Key, sb.Value })
                .Where(t => t.Item2 == y)
                .Select(t => t.Item1));
            return Enumerable.Range(xMin, xMax - xMin + 1)
                .Where(i => !markers.Contains(i) && ranges.Any(r => XIsWithin(i, r.Item1, r.Item2)));
        }

        public IEnumerable<int> GetUnknownSpaces(int y, int xMin, int xMax){
            var ranges = RowEmptyRanges[y];
            HashSet<int> markers = new HashSet<int>(ClosestBeacon
                .SelectMany(sb => new List<(int,int)>{ sb.Key, sb.Value })
                .Where(t => t.Item2 == y)
                .Select(t => t.Item1));
            return Enumerable.Range(xMin, xMax - xMin + 1)
                .Where(i => !markers.Contains(i) && ranges.All(r => !XIsWithin(i, r.Item1, r.Item2)));
        }

        private bool XIsWithin(int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        private void AddEmptyRange(int y, int xMin, int xMax){
            if(!RowEmptyRanges.ContainsKey(y))
                RowEmptyRanges.Add(y, new List<(int,int)>());
            AddRange(RowEmptyRanges[y],xMin,xMax);
        }

        private void AddRange(List<(int, int)> areas, int xMin, int xMax)
        {
            if(areas.Any(a => a.Item1 <= xMin && a.Item2 >= xMax))
                return;

            foreach(Point a in areas){
                // appends to a line
                if(a.Item1 < xMin && a.Item2 >= xMin){
                    a.Item2 = xMax;
                }
            }
        }
    }
}