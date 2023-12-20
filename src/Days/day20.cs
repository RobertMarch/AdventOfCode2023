namespace AdventOfCode;

public class Day20 : BaseDay
{
    public Day20() : base(20, GetTestCases())
    { }

    private record Pulse(string source, string destination, bool highPulse);

    private enum ModuleType
    {
        BUTTON,
        BROADCAST,
        FLIP_FLOP,
        CONJUNCTION,
    }

    private abstract class Module
    {
        protected Module(string name, ModuleType moduleType, List<string> destinations)
        {
            Name = name;
            ModuleType = moduleType;
            Destinations = destinations;
            Inputs = [];
        }

        public string Name { get; init; }
        protected ModuleType ModuleType { get; init; }
        protected List<string> Destinations { get; init; }
        protected List<string> Inputs { get; init; }

        public virtual void AddInput(string input)
        {
            Inputs.Add(input);
        }

        public void ConnectToOutputs(Dictionary<string, Module> modules)
        {
            Destinations.ForEach(destName =>
            {
                if (modules.ContainsKey(destName))
                {
                    modules[destName].AddInput(Name);
                }
            });
        }

        public abstract List<Pulse> HandleInputPulse(Pulse input);

        protected List<Pulse> SendSignalToAllDestinations(bool outputSignal)
        {
            return Destinations.Select(destName => new Pulse(Name, destName, outputSignal)).ToList();
        }
    }

    private class ButtonModule : Module
    {
        public ButtonModule() : base("button", ModuleType.BUTTON, ["broadcaster"])
        { }

        public override List<Pulse> HandleInputPulse(Pulse input)
        {
            return SendSignalToAllDestinations(false);
        }
    }

    private class BroadcastModule : Module
    {
        public BroadcastModule(List<string> destinations) : base("broadcaster", ModuleType.BROADCAST, destinations)
        { }

        public override List<Pulse> HandleInputPulse(Pulse input)
        {
            return SendSignalToAllDestinations(input.highPulse);
        }
    }

    private class FlipFlopModule : Module
    {
        public FlipFlopModule(string name, List<string> destinations) : base(name, ModuleType.FLIP_FLOP, destinations)
        {
        }

        private bool State = false;

        public override List<Pulse> HandleInputPulse(Pulse input)
        {
            if (input.highPulse)
            {
                return [];
            }
            State = !State;
            return SendSignalToAllDestinations(State);
        }
    }

    private class ConjunctionModule : Module
    {
        public ConjunctionModule(string name, List<string> destinations) : base(name, ModuleType.CONJUNCTION, destinations)
        {
            InputStates = new Dictionary<string, bool>();
        }

        private Dictionary<string, bool> InputStates { get; init; }

        public override void AddInput(string input)
        {
            Inputs.Add(input);
            InputStates[input] = false;
        }

        public override List<Pulse> HandleInputPulse(Pulse input)
        {
            InputStates[input.source] = input.highPulse;
            bool allInputsHigh = InputStates.Values.All(s => s);
            return SendSignalToAllDestinations(!allInputsHigh);
        }
    }

    protected override string SolvePartOne(string input)
    {
        Dictionary<string, Module> modules = ParseModules(input);
        long highPulses = 0;
        long lowPulses = 0;
        for (int i = 0; i < 1000; i++)
        {
            (long high, long low) = ProcessButtonPress(modules, i, []);
            highPulses += high;
            lowPulses += low;
        }

        return (highPulses * lowPulses).ToString();
    }

    protected override string SolvePartTwo(string input)
    {
        Dictionary<string, Module> modules = ParseModules(input);
        Dictionary<string, long> lastModuleCycles = new Dictionary<string, long>();

        for (long i = 1; i < 20000; i++)
        {
            ProcessButtonPress(modules, i, lastModuleCycles);
        }

        return lastModuleCycles.Values
            .Aggregate(CalculateLCM)
            .ToString();
    }

    private (long, long) ProcessButtonPress(Dictionary<string, Module> modules, long i, Dictionary<string, long> lastModuleCycles)
    {
        long highPulses = 0;
        long lowPulses = 0;
        List<Pulse> unprocessedPulses = [new Pulse("button", "broadcaster", false)];

        while (unprocessedPulses.Count > 0)
        {
            Pulse nextPulse = unprocessedPulses.First();
            unprocessedPulses.RemoveAt(0);

            HashSet<string> lastModules = ["hz", "xm", "qh", "pv"];

            if (lastModules.Contains(nextPulse.source) && nextPulse.highPulse && !lastModuleCycles.ContainsKey(nextPulse.source))
            {
                lastModuleCycles[nextPulse.source] = i;
            }

            if (nextPulse.highPulse)
            {
                highPulses += 1;
            }
            else
            {
                lowPulses += 1;
            }

            if (!modules.ContainsKey(nextPulse.destination))
            {
                continue;
            }

            unprocessedPulses.AddRange(modules[nextPulse.destination].HandleInputPulse(nextPulse));
        }
        return (highPulses, lowPulses);
    }

    private Dictionary<string, Module> ParseModules(string input)
    {
        Dictionary<string, Module> modules = SplitStringToLines(input).Select(ParseLineToModule).ToDictionary(m => m.Name);
        modules["button"] = new ButtonModule();

        foreach (Module module in modules.Values)
        {
            module.ConnectToOutputs(modules);
        }

        return modules;
    }

    private Module ParseLineToModule(string line)
    {
        string[] parts = line.Split(" -> ");
        List<string> outputs = parts[1].Split(", ").ToList();

        switch (parts[0].First())
        {
            case 'b':
                return new BroadcastModule(outputs);
            case '%':
                return new FlipFlopModule(parts[0].Substring(1), outputs);
            case '&':
                return new ConjunctionModule(parts[0].Substring(1), outputs);
        }

        throw new Exception($"Cannot parse line {line}");
    }

    private long CalculateLCM(long a, long b)
    {
        if (b > a)
        {
            long temp = a;
            a = b;
            b = temp;
        }

        long result = a;
        while (result % b != 0)
        {
            result += a;
        }
        return result;
    }

    private static TestCase[] GetTestCases()
    {
        return [
            new TestCase(@"broadcaster -> a, b, c
%a -> b
%b -> c
%c -> inv
&inv -> a", "32000000", null),
            new TestCase(@"broadcaster -> a
%a -> inv, con
&inv -> b
%b -> con
&con -> output", "11687500", null),
        ];
    }
}