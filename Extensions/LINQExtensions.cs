namespace Nezbo.AdventOfCode.Extensions;

public static class LINQExtensions 
{
    #region Grouping

    public static IEnumerable<IEnumerable<T>> SplitEvery<T>(this IEnumerable<T> source, int count){
        return source.SplitAt((i,e) => i % count == count - 1, true);
    }
    public static IEnumerable<IEnumerable<T>> SplitAt<T>(this IEnumerable<T> source, Predicate<T> splitAt, bool keepSplitter = false){
        return source.SplitAt((i,e) => splitAt(e), keepSplitter);
    }

    public static IEnumerable<IEnumerable<T>> SplitAt<T>(this IEnumerable<T> source, Func<int,T,bool> splitAt, bool keepSplitter = false){
        List<T> block = new();
        int i = 0;
        foreach(var element in source){
            if(splitAt(i, element)){
                if(keepSplitter){
                    block.Add(element);
                }
                yield return block;
                block = new List<T>();
            }else{
                block.Add(element);
            }

            i++;
        }
        if(block.Count > 0)
            yield return block;
    }

    #endregion

    #region Looping

    public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T,int> action)
    {
        int i = 0;
        foreach(T item in enumeration)
        {
            action(item, i);
            i++;
        }
    }

    public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
    {
        enumeration.ForEach((e,i) => action(e));
    }

    public static void ForEachPair<T>(this IEnumerable<T> enumeration, Action<T,T> action){
        T prev = enumeration.First();
        foreach(T item in enumeration.Skip(1)){
            action(prev,item);
            prev = item;
        }
    }

    #endregion

    #region Indices

    public static IEnumerable<int> IndicesOf<T>(this IEnumerable<T> enumeration, Predicate<T> predicate){
        int i = 0;
        foreach(T item in enumeration){
            if(predicate(item))
                yield return i;
            i++;
        }
    }

    #endregion

    #region Terminal

    public static int Mod(this IEnumerable<int> enumeration){
        return enumeration.Aggregate(1, (x,y) => x * y);
    }

    public static long Mod(this IEnumerable<long> enumeration){
        return enumeration.Aggregate(1L, (x,y) => x * y);
    }

    #endregion
}