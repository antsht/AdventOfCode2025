string[] lines = File.ReadAllLines("input.txt");

// Convert 3-letter code to int (base-26 encoding)
static int ToCode(string s) => (s[0] - 'a') * 676 + (s[1] - 'a') * 26 + (s[2] - 'a');

// Store codes for "you" and "out"
int YOU = ToCode("you");
int OUT = ToCode("out");

// Build the graph: device code -> list of output device codes
Dictionary<int, List<int>> deviceOutputs = new Dictionary<int, List<int>>();

foreach (var line in lines)
{
    string[] parts = line.Split(' ');
    // parts[0] is the device name with ':', parts[1..] are the output devices
    int device = ToCode(parts[0].TrimEnd(':'));
    List<int> outputs = parts.Skip(1).Select(ToCode).ToList();
    deviceOutputs[device] = outputs;
}

// Count all paths from "you" to "out" using DFS
int pathCount = CountPaths(deviceOutputs, YOU, OUT);
Console.WriteLine($"Number of paths from 'you' to 'out': {pathCount}");

static int CountPaths(Dictionary<int, List<int>> graph, int current, int target)
{
    // Base case: reached the target
    if (current == target)
    {
        return 1;
    }
    
    // If current device has no outputs (dead end that's not the target)
    if (!graph.ContainsKey(current))
    {
        return 0;
    }
    
    // Recursively count paths through each output
    int totalPaths = 0;
    foreach (int output in graph[current])
    {
        totalPaths += CountPaths(graph, output, target);
    }
    
    return totalPaths;
}