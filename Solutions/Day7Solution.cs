using Nezbo.AdventOfCode;

namespace Nezbo.AdventOfCode.Solutions;

internal class Day7Solution : ISolution
{
    public string SolvePartOne(string[] input)
    {
        return CreateFolderStructure(input)
            .Descendants
            .Where(d => d.Size <= 100_000)
            .Sum(d => d.Size)
            .ToString();
    }

    private Folder CreateFolderStructure(string[] input)
    {
        Folder root = new Folder(null, "/");
        Folder current = null;
        foreach(string line in input)
        {
            switch(line.Split(' '))
            {
                case ["$", "ls"]: break;
                case ["$", "cd", "/"]: current = root; break;
                case ["$", "cd", ".."]: current = current.Parent; break;
                case ["$", "cd", var dir]: current = current.Children.First(d => d.Name == dir); break;
                case ["dir", var dir]: current.Children.Add(new Folder(current, dir)); break;
                case [var size, var file]: current.Files.Add(file, int.Parse(size)); break;
                default: break;
            }
        }

        return root;
    }

    public string SolvePartTwo(string[] input)
    {
        Folder root = CreateFolderStructure(input);
        int freeSpace = 70_000_000 - root.Size;
        int requiredSpace = 30_000_000 - freeSpace;
        return root.Descendants
            .Where(d => d.Size >= requiredSpace)
            .OrderBy(d => d.Size)
            .First().Size.ToString();
    }

    private class Folder
    {
        public Folder Parent { get; set; }
        public string Name { get; set; }
        public Dictionary<string,int> Files { get; } = new Dictionary<string, int>();
        public List<Folder> Children { get; } = new List<Folder>();

        public Folder(Folder parent, string name)
        {
            this.Parent = parent;
            this.Name = name;
        }

        public int Size => Files.Values.Sum() + Children.Sum(f => f.Size);
        public IEnumerable<Folder> Descendants => Children.Union(Children.SelectMany(c => c.Descendants));

        public override string ToString()
        {
            return Parent == null ? Name : $"{Parent.ToString()}{Name}/";
        }
    }
}