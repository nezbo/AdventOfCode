using System.Collections.Generic;
using Nezbo.AdventOfCode;
using Nezbo.AdventOfCode.Extensions;

namespace AdventOfCode.Solutions;

internal class Day5Solution : ISolution
{
    public string SolvePartOne(string[] input)
    {
        Stack<char>[] stacks = ParseStacks(input);
        ParseMoves(input).ForEach(m => ExecuteMove(stacks, m));
        return new string(stacks.Select(s => s.Peek()).ToArray());
    }

    private IEnumerable<Move> ParseMoves(string[] input)
    {
        return input
            .SkipWhile(l => !l.StartsWith('m'))
            .Select(Move.Parse);
    }

    private Stack<char>[] ParseStacks(string[] input)
    {
        var stackLines = input.TakeWhile(l => l.TrimStart().StartsWith('[')).Reverse();
        int stackCount = stackLines.First().Split(' ').Count();
        Stack<char>[] stacks = new Stack<char>[stackCount];

        for(int i = 0; i < stacks.Length; i++)
            stacks[i] = new Stack<char>();

        foreach(string line in stackLines){
            line.AsEnumerable().SplitEvery(4).ForEach((s,i) => 
            {
                if(char.IsLetter(s.ElementAt(1)))
                    stacks[i].Push(s.ElementAt(1));
            });
        }

        return stacks;
    }

    private void ExecuteMove(Stack<char>[] stacks, Move move)
    {
        for(int i = 0; i < move.Times; i++){
            char crate = stacks[move.FromStack - 1].Pop();
            stacks[move.ToStack - 1].Push(crate);
        }
    }

    public string SolvePartTwo(string[] input)
    {
        Stack<char>[] stacks = ParseStacks(input);
        ParseMoves(input).ForEach(m => ExecuteMultiMove(stacks, m));
        return new string(stacks.Select(s => s.Peek()).ToArray());
    }

    private void ExecuteMultiMove(Stack<char>[] stacks, Move move)
    {
        IEnumerable<char> crates = Enumerable.Range(0, move.Times).Select(i => stacks[move.FromStack - 1].Pop()).Reverse();
        crates.ForEach(c => stacks[move.ToStack - 1].Push(c));
    }

    private class Move
    {
        public static Move Parse(string line){
            var splitted = line.Split(' ');
            return new Move
            {
                Times = int.Parse(splitted[1]),
                FromStack = int.Parse(splitted[3]),
                ToStack = int.Parse(splitted[5])
            };
        }

        public int Times {get;set;}
        public int FromStack {get;set;}
        public int ToStack {get;set;}
    }
}