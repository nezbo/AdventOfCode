namespace Nezbo.AdventOfCode.Solutions;

internal class Day9Solution : ISolution
{
    public string SolvePartOne(string[] input)
    {
        return Solve(input, 2);
    }

    private static string Solve(string[] input, int knots)
    {
        var moves = input.Where(l => !string.IsNullOrEmpty(l))
                    .Select(l => new { dir = (Direction)Enum.Parse(typeof(Direction), l.Substring(0, 1)), count = int.Parse(l.Substring(2)) });

        Knot head = new Knot(knots - 1);

        foreach (var move in moves)
        {
            //Console.WriteLine($"== {move.dir} x {move.count} ==\n");
            for (int i = 0; i < move.count; i++)
            {
                head.Move(move.dir);

                //PrintVisually((head.X,head.Y),(head.Tail.X, head.Tail.Y), head.Tail.History, head.History);
            }
        }

        return head.GetEndTail().History.Distinct().Count().ToString();
    }

    private void PrintVisually((int, int) headPos, (int, int) tailPos, IEnumerable<(int,int)> visited, IEnumerable<(int,int)> history)
    {
        int xMin = Math.Min(headPos.Item1, history.Min(t => t.Item1));
        int xMax = Math.Max(headPos.Item1, history.Max(t => t.Item1));
        int yMin = Math.Min(headPos.Item2, history.Min(t => t.Item2));
        int yMax = Math.Max(headPos.Item2, history.Max(t => t.Item2));
        for(int y = yMin; y <= yMax; y++){
                Console.WriteLine(string.Join("", Enumerable.Range(xMin, xMax - xMin+1).Select(x => GetLetter(x,y,headPos,tailPos,visited))));
        }

        Console.WriteLine();
    }

    private string GetLetter(int x, int y, (int, int) headPos, (int, int) tailPos, IEnumerable<(int,int)> visited)
    {
        if(x == headPos.Item1 && y == headPos.Item2)
            return "H";
        if(x == tailPos.Item1 && y == tailPos.Item2)
            return "T";
        if(x == 0 && y == 0)
            return "s";
        if(visited.Contains((x,y)))
            return "#";
        return ".";
    }


    public string SolvePartTwo(string[] input)
    {
        return Solve(input, 10);
    }

    public enum Direction
        {
            None = 0,
            L = 1,
            R = 2,
            U = 4,
            D = 8,
            LU = L + U,
            LD = L + D,
            RU = R + U,
            RD = R + D
        }

    private class Knot
    {
        public int X { get; set; }
        public int Y { get; set; }
        public List<(int,int)> History { get; set; } = new ();
        public Knot Tail { get; set; }

        public Knot(int length)
        {
            History.Add((X = 0, Y = 0));
            if(length > 0){
                Tail = new Knot(length - 1);
            }
        }

        public void Move(Direction dir){
            var offset = GetMoveOffset(dir);
            History.Add((X += offset.Item1, Y += offset.Item2));

            Tail?.Move(GetDirectionTo(Tail, this));
        }

        private Direction GetDirectionTo(Knot from, Knot to)
        {
            if(Math.Abs(from.X - to.X) <= 1 && Math.Abs(from.Y - to.Y) <= 1)
                return Direction.None;

            Direction x = from.X == to.X ? Direction.None : (from.X < to.X ? Direction.R : Direction.L);
            Direction y = from.Y == to.Y ? Direction.None : (from.Y < to.Y ? Direction.D : Direction.U);
            return (Direction)(int)x + (int)y;
        }

        private (int,int) GetMoveOffset(Direction dir){
            return dir switch
            {
                Direction.L => (-1, 0),
                Direction.R => (1, 0),
                Direction.U => (0, -1),
                Direction.D => (0, 1),
                Direction.LU => (-1, -1),
                Direction.LD => (-1, 1),
                Direction.RU => (1, -1),
                Direction.RD => (1, 1),
                _ => (0,0)
            };
        }

        public Knot GetEndTail(){
            return Tail?.GetEndTail() ?? this;
        }
    }
}