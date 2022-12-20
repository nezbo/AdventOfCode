using System.Collections.Concurrent;
using System.Numerics;
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
        int range = 4_000_000;
        Cave cave = ParseSensors(input, 0, range);
        cave.MarkEmptySpaces();
        var pos = FindSingleUnknownSpace(cave, range);
        return (4000000 * new BigInteger(pos.Item1) + new BigInteger(pos.Item2)).ToString();
    }

    private static (int,int) FindSingleUnknownSpace(Cave cave, int yMax)
    {
        var yMatch = Enumerable.Range(0, yMax)
            .AsParallel()
            .AsUnordered()
            .Where(y => cave.RowEmptyRanges[y].Sum(l => l.Length) <= yMax)
            .FirstOrDefault();
        return FindGap(cave, yMatch);
    }

    private static (int, int) FindGap(Cave cave, int y)
    {
        return (cave.RowEmptyRanges[y][0].Max + 1, y);
    }

    private Cave ParseSensors(string[] input, int min = int.MinValue, int max = int.MaxValue)
    {
        var cave = new Cave(min,max);
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

    private record Line(int Min, int Max)
    {
        public int Length => Max - Min + 1;
    }

    private class Cave {
        public Dictionary<(int,int),(int,int)> ClosestBeacon = new ();
        public IDictionary<int,List<Line>> RowEmptyRanges = new ConcurrentDictionary<int, List<Line>>();

        public int RangeMin { get; }
        public int RangeMax { get; }

        public Cave(int rangeMin = int.MinValue, int rangeMax = int.MaxValue)
        {
            RangeMin = rangeMin;
            RangeMax = rangeMax;
        }

        public void AddSensor((int,int) sensor, (int,int) closestBeacon){

                ClosestBeacon[sensor] = closestBeacon;
        }

        public void MarkEmptySpaces()
        {
            Parallel.ForEach(ClosestBeacon, beacon => {
                //Console.WriteLine($"Starting beacon ({beacon.Key.Item1},{beacon.Key.Item2})");
                var c = beacon.Key;
                var dist = beacon.Key.ManhattanDistance(beacon.Value);

                //Console.WriteLine($"Distance = {dist}");
                for(int y = Math.Max(RangeMin, c.Item2 - dist); y <= Math.Min(RangeMax, c.Item2 + dist); y++){
                    //Console.WriteLine($"Marking line {y}");
                    int xMin = Math.Max(RangeMin, c.Item1 - dist + Math.Abs(y - c.Item2));
                    int xMax = Math.Min(RangeMax, c.Item1 + dist - Math.Abs(y - c.Item2));
                    AddEmptyRange(y, xMin, xMax);
                }
            });
        }

        public IEnumerable<int> GetConfirmedEmptySpaces(int y){
            var ranges = RowEmptyRanges[y];
            int xMin = ranges.Min(r => r.Min);
            int xMax = ranges.Max(r => r.Max);
            HashSet<int> markers = new HashSet<int>(ClosestBeacon
                .SelectMany(sb => new List<(int,int)>{ sb.Key, sb.Value })
                .Where(t => t.Item2 == y)
                .Select(t => t.Item1));
            return Enumerable.Range(xMin, xMax - xMin + 1)
                .Where(i => !markers.Contains(i) && ranges.Any(r => XIsWithin(i, r.Min, r.Max)));
        }

        public IEnumerable<int> GetUnknownSpaces(int y, int xMin, int xMax){
            var ranges = RowEmptyRanges[y];
            HashSet<int> markers = new HashSet<int>(ClosestBeacon
                .SelectMany(sb => new List<(int,int)>{ sb.Key, sb.Value })
                .Where(t => t.Item2 == y)
                .Select(t => t.Item1));
            return Enumerable.Range(xMin, xMax - xMin + 1)
                .Where(i => !markers.Contains(i) && ranges.All(r => !XIsWithin(i, r.Min, r.Max)));
        }

        private bool XIsWithin(int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        private void AddEmptyRange(int y, int xMin, int xMax){
            lock(RowEmptyRanges){
                if(!RowEmptyRanges.ContainsKey(y)){
                        RowEmptyRanges.Add(y, new List<Line>());
                }
            }

            lock(RowEmptyRanges[y]){
                AddRange(RowEmptyRanges[y],xMin,xMax);
            }
        }

        private void AddRange(List<Line> areas, int xMin, int xMax)
        {
            if(areas.Any(a => a.Min <= xMin && a.Max >= xMax))
                return;

            List<int> overlaps = new ();
            areas.ForEach((a,i) => {
                if((xMin <= a.Max + 1 && xMax > a.Max) // appends to a line
                    || (xMin < a.Min && xMax >= a.Min - 1) // prepends to a line
                    )
                {
                    overlaps.Add(i);
                }
            });

            if(overlaps.Count == 0){
                int index = areas.Count == 0 ? 0 : areas.FindIndex(0, l => l.Min > xMin);
                if(index < 0)
                    index = areas.Count;
                areas.Insert(index, new Line(xMin,xMax));
            }
            else{
                int min = Math.Min(xMin, overlaps.Min(i => areas[i].Min));
                int max = Math.Max(xMax, overlaps.Max(i => areas[i].Max));
                overlaps.AsEnumerable().Reverse().ForEach(areas.RemoveAt);
                areas.Insert(overlaps.Min(), new Line(min,max));
            }
        }

        public int CountSensorsAndBeacons(int y)
        {
            return ClosestBeacon.Keys
                .Union(ClosestBeacon.Values)
                .Count(p => p.Item2 == y);
        }
    }
}