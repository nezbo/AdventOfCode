namespace Nezbo.AdventOfCode.Collections;

public class SparseArray2D<T>
{
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
                var d = GetRow(row);
                return d?.ContainsKey(col) == true ? d[col] : DefaultValue;
            }
            set
            {
                var d = GetRowSafe(row);
                d[col] = value;
            }
        }

    public int CurrentHeight => Cells.Max(d => d.Value.Keys.Max() + 1);

    public int CurrentWidth => Cells.Keys.Max() + 1;

    private Dictionary<int, T> GetRow(int row){
        return Cells.GetValueOrDefault(row);
    }

    private Dictionary<int, T> GetRowSafe(int row){
        if(!Cells.ContainsKey(row)){
            Cells[row] = new Dictionary<int, T>();
        }
        return Cells[row];
    }
}