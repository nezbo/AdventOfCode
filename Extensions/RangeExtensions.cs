public static class RangeExtensions
{
    public static bool Contains(this Range first, Range other)
    {
        return first.Start.Value <= other.Start.Value && first.End.Value >= other.End.Value;
    }

    public static bool Overlaps(this Range first, Range other)
    {
        return (first.Start.Value <= other.Start.Value && first.End.Value >= other.Start.Value)
            || (other.Start.Value <= first.Start.Value && other.End.Value >= first.Start.Value);
    }
}