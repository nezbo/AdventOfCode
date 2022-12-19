using System.Collections.Immutable;

namespace Nezbo.AdventOfCode.Collections;

public class SparseArray2D<T>
{
    public int XMin { get; private set; } = 0;
    public int XMax { get; private set; } = 0;
    public int YMin { get; private set; } = 0;
    public int YMax { get; private set; } = 0;

    public T DefaultValue { get; private set; } = default;

    private Dictionary<int, Dictionary<int, T>> Cells { get; } = new ();

    public SparseArray2D(T defaultValue)
    {
        this.DefaultValue = defaultValue;
    }

    public T this[int row, int col]
        {
            get
            {
                var d = FindRow(row);
                return d?.ContainsKey(col) == true ? d[col] : DefaultValue;
            }
            set
            {
                var d = FindRowSafe(row);
                d[col] = value;

                if(XMin > row)
                    XMin = row;
                if(XMax < row)
                    XMax = row;
                if(YMin > col)
                    YMin = col;
                if(YMax < col)
                    YMax = col;
            }
        }

    public bool HasElement(int x, int y){
        return FindRow(x)?.ContainsKey(y) == true;
    }

    public IDictionary<int, T> GetRow(int y){
        return Enumerable.Range(XMin, XMax - XMin + 1)
            .Where(x => HasElement(x,y))
            .ToDictionary(x => x, x => this[x,y]);
    }

    public IDictionary<int, T> GetColumn(int x){
        return FindRowSafe(x).ToImmutableDictionary();
    }

    private Dictionary<int, T> FindRow(int row){
        return Cells.GetValueOrDefault(row);
    }

    private Dictionary<int, T> FindRowSafe(int row){
        if(!Cells.ContainsKey(row)){
            Cells[row] = new Dictionary<int, T>();
        }
        return Cells[row];
    }
}