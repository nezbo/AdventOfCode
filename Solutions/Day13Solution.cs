using Nezbo.AdventOfCode.Extensions;

namespace Nezbo.AdventOfCode.Solutions;

internal class Day13Solution : ISolution
{
    private static bool DEBUG = false;

    public string SolvePartOne(string[] input)
    {
        return input
            .SplitAt(string.IsNullOrEmpty)
            .Select(g => (ParsePacket(g.First()), ParsePacket(g.Last())))
            .IndicesOf(t => t.Item1.CompareTo(t.Item2) <= 0)
            .Select(i => i + 1)
            .Sum()
            .ToString();
    }

    public string SolvePartTwo(string[] input)
    {
        return input
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Append("[[2]]")
            .Append("[[6]]")
            .Select(ParsePacket)
            .OrderBy(p => p)
            .IndicesOf(p => p.ToString().Equals("[[2]]") || p.ToString().Equals("[[6]]"))
            .Select(i => i + 1)
            .Mod()
            .ToString();
    }

    private IPacket ParsePacket(string str){
        if(str.StartsWith('[')){
            string content = str.Substring(1, str.Length - 2);
            IEnumerable<string> splitted = SplitFirstLevel(content);
            return new PacketList {
                Content = splitted.Select(ParsePacket).ToList()
            };
        }
        return new PacketLeaf(int.Parse(str));
    }

    private static IEnumerable<string> SplitFirstLevel(string content)
    {
        int open = 0;
        int close = 0;
        return content
            .SplitAt(c => { CountBrackets(c, ref open, ref close); return c == ',' && open == close; } )
            .Select(a => new string(a.ToArray()));
    }

    private static void CountBrackets(char c, ref int open, ref int close)
    {
        switch(c){
            case '[': open++; break;
            case ']': close++; break;
        }
    }

    private interface IPacket : IComparable<IPacket>{
    }

    private class PacketList : IPacket
    {
        public static PacketList FromItem(IPacket item){
            return new PacketList { Content = new List<IPacket> {item}};
        }

        public List<IPacket> Content { get; set; }

        public override string ToString()
        {
            return $"[{string.Join(',', Content)}]";
        }

        public int CompareTo(IPacket other)
        {
            if(other is not PacketList)
                return CompareTo(PacketList.FromItem(other));

            Debug($"- Compare {this} vs {other}");
            PacketList otherList = other as PacketList;
            for(int i = 0; i < Content.Count; i++){
                if(otherList.Content.Count <= i)
                {
                    Debug("- Right side ran out of items, so inputs are not in the right order");
                    return 1;
                }
                    
                var comp = Content[i].CompareTo(otherList.Content[i]);
                if(comp > 0){
                    Debug("- Right side is smaller, so inputs are not in the right order");
                    return comp;
                }
                else if(comp < 0){
                    Debug("- Left side is smaller, so inputs are in the right order");
                    return comp;
                }
            }

            if(Content.Count == otherList.Content.Count){
                return 0;
            }
                
            Debug("- Left side ran out of items, so inputs are in the right order");
            return -1;
        }

        private void Debug(string text){
            if(DEBUG)
                Console.WriteLine(text);
        }
    }

    private class PacketLeaf : IPacket{
        public int Value { get; private set; }

        public PacketLeaf(int value)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public int CompareTo(IPacket other)
        {
            if(other is PacketLeaf otherLeaf){
                if(DEBUG)
                    Console.WriteLine($"- Compare {this} vs {other}");
                return this.Value - otherLeaf.Value;
            }
            return PacketList.FromItem(this).CompareTo(other);
        }
    }
}