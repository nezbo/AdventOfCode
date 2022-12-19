public static class TupleIntExtensions {
    public static int ManhattanDistance(this (int,int) first, (int,int) second){
        return Math.Abs(first.Item1 - second.Item1) + Math.Abs(first.Item2 - second.Item2);
    }
}