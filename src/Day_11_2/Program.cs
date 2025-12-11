string[] lines = File.ReadAllLines("input.txt");

// Convert 3-letter code to int (base-26 encoding)
static int ToCode(string s) => (s[0] - 'a') * 676 + (s[1] - 'a') * 26 + (s[2] - 'a');

// Store codes for "svr", "dac", "fft" and "out"
int SVR = ToCode("svr");
int DAC = ToCode("dac");
int FFT = ToCode("fft");
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

// Memoization cache: (current, visitedDac, visitedFft) -> path count
Dictionary<(int, bool, bool), long> memo = new Dictionary<(int, bool, bool), long>();

// Count all paths from "svr" to "out" that visit both dac and fft (in any order)
long pathCount = CountPaths(deviceOutputs, SVR, OUT, DAC, FFT, false, false, memo);
Console.WriteLine($"Number of paths from 'svr' to 'out' visiting both dac and fft: {pathCount}");

static long CountPaths(Dictionary<int, List<int>> graph, int current, int target, int dac, int fft, 
                       bool visitedDac, bool visitedFft, Dictionary<(int, bool, bool), long> memo)
{
    // Update visited flags if we're at dac or fft
    if (current == dac) visitedDac = true;
    if (current == fft) visitedFft = true;
    
    // Base case: reached the target - only count if we visited both dac and fft
    if (current == target)
    {
        return (visitedDac && visitedFft) ? 1 : 0;
    }
    
    // If current device has no outputs (dead end that's not the target)
    if (!graph.ContainsKey(current))
    {
        return 0;
    }
    
    // Check memo cache
    var key = (current, visitedDac, visitedFft);
    if (memo.TryGetValue(key, out long cached))
    {
        return cached;
    }
    
    // Recursively count paths through each output
    long totalPaths = 0;
    foreach (int output in graph[current])
    {
        totalPaths += CountPaths(graph, output, target, dac, fft, visitedDac, visitedFft, memo);
    }
    
    // Store in cache and return
    memo[key] = totalPaths;
    return totalPaths;
}