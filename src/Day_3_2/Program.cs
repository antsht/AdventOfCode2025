
string[] lines = File.ReadAllLines("input.txt");

long sum = 0;

foreach (string line in lines)
{
    int[] digits = line.Select(c => int.Parse(c.ToString())).ToArray();
    int n = digits.Length;
    int k = 12; // number of batteries to select
    
    List<int> selectedIndices = new List<int>();
    int lastIndex = -1;
    
    // Greedy algorithm: for each position in the result, pick the largest digit from the valid range
    for (int pos = 0; pos < k; pos++)
    {
        int searchStart = lastIndex + 1;
        int searchEnd = n - (k - pos);
        
        int maxIndex = searchStart;
        for (int i = searchStart; i <= searchEnd; i++)
        {
            if (digits[i] > digits[maxIndex])
            {
                maxIndex = i;
            }
        }
        
        selectedIndices.Add(maxIndex);
        lastIndex = maxIndex;
    }
    
    string joltageStr = string.Join("", selectedIndices.Select(i => digits[i]));
    long joltage = long.Parse(joltageStr);
    sum += joltage;
}
Console.WriteLine(sum);

