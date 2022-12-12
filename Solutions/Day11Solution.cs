using System.Numerics;
using Nezbo.AdventOfCode.Extensions;

namespace Nezbo.AdventOfCode.Solutions;

internal class Day11Solution : ISolution
{
    public string SolvePartOne(string[] input)
    {
        List<Monkey> monkeys = ParseMonkeys(input, true);
        long[] counts = SimulateRounds(monkeys, 20);
        return GetModOfMostInspected(counts);
    }

    private static string GetModOfMostInspected(long[] counts)
    {
        return counts
            .OrderByDescending(i => i)
            .Take(2)
            .Mod()
            .ToString();
    }

    public string SolvePartTwo(string[] input)
    {
        List<Monkey> monkeys = ParseMonkeys(input, false);
        long[] counts = SimulateRounds(monkeys, 10_000);
        return GetModOfMostInspected(counts);
    }

    private List<Monkey> ParseMonkeys(string[] input, bool includeRelaxing)
    {
        return input.SplitAt(string.IsNullOrWhiteSpace)
            .Select(g => g.Skip(1).ToArray())
            .Select(g => new Monkey(ParseItems(g[0]), ParseOperation(g[1]), ParseLastNumber(g[2]), ParseLastNumber(g[3]), ParseLastNumber(g[4]), includeRelaxing))
            .ToList();
    }

    private int ParseLastNumber(string line)
    {
        return int.Parse(line.Split(' ').Last());
    }

    private Func<long, long> ParseOperation(string line)
    {
        string equation = line.Split('=')[1];
        char op = equation.Contains('*') ? '*' : '+';
        int?[] values = equation.Split(op).Select(GetIntOrNull).ToArray();
        return (i => ApplyOperator(op, values, i));
    }

    private int? GetIntOrNull(string str)
    {
        if(int.TryParse(str, out int value))
            return value;
        return null;
    }

    private long ApplyOperator(char op, int?[] values, long input)
    {
        long first = values[0] ?? input;
        long second = values[1] ?? input;
        return op switch {
            '+' => first + second,
            '*' => first * second,
            _ => input
        };
    }

    private IEnumerable<long> ParseItems(string line)
    {
        return line.Split(':')[1].Split(',').Select(long.Parse);
    }

    private long[] SimulateRounds(List<Monkey> monkeys, int rounds)
    {
        for(int i = 0; i < rounds; i++){
            foreach(Monkey m in monkeys){
                m.TakeTurn(monkeys);
            }
            ManageItems(monkeys);
        }

        return monkeys.Select(m => m.InspectionCount).ToArray();
    }

    private void ManageItems(List<Monkey> monkeys)
    {
        long divideBy = monkeys.Select(m => (long)m.TestDivisibleBy).Mod();
        monkeys.ForEach(m => m.DivideItemsBy(divideBy));
    }

    private class Monkey
    {
        public Queue<long> Items { get; private set; }
        public long InspectionCount {get; private set;} = 0;
        private Func<long,long> Operation {get; set;}
        public int TestDivisibleBy { get; set; }
        private int ThrowToPositive { get; set; }
        private int ThrowToNegative { get; set; }
        private bool IncludeRelaxing {get; set; }

        public Monkey(IEnumerable<long> items,
            Func<long, long> operation,
            int testDivisibleBy,
            int throwToPositive,
            int throwToNegative,
            bool includeRelaxing)
        {
            this.Items = new Queue<long>(items);
            this.Operation = operation;
            this.TestDivisibleBy = testDivisibleBy;
            this.ThrowToPositive = throwToPositive;
            this.ThrowToNegative = throwToNegative;
            this.IncludeRelaxing = includeRelaxing;
        }

        public void Catch(long item){
            Items.Enqueue(item);
        }

        public void TakeTurn(List<Monkey> monkeys){
            while(Items.Count > 0){
                InspectionCount++;
                long item = Items.Dequeue();
                item = Operation(item);

                if(this.IncludeRelaxing)
                    item /= 3;

                monkeys[item % TestDivisibleBy == 0 ? ThrowToPositive : ThrowToNegative]
                    .Catch(item);
            }
        }

        public void DivideItemsBy(long value){
            this.Items = new Queue<long>(this.Items.Select(i => i % value));
        }
    }
}