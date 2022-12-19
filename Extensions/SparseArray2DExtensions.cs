using Nezbo.AdventOfCode.Collections;

public static class SparseArray2DExtensions{
    public static void PrintGrid<T>(this SparseArray2D<T> array, Func<T,char> mapper, int xMin, int xMax, int yMin, int yMax)
    {
        for(int y = yMin; y <= yMax; y++){
            Console.WriteLine(new string(Enumerable.Range(xMin,xMax-xMin+1).Select(x => mapper(array[x,y])).ToArray()));
        }
    }

    public static void PrintGrid(this SparseArray2D<char> array, int xMin, int xMax, int yMin, int yMax)
    {
        array.PrintGrid(c => c, xMin, xMax, yMin, yMax);
    }
}