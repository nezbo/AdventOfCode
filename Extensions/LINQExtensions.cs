namespace Nezbo.AdventOfCode.Extensions;

public static class LINQExtensions {

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
}