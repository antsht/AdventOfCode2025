
string[] lines = File.ReadAllLines("input.txt");

int sum = 0;

foreach (string line in lines)
{
    int[] digits = line.Select(c => int.Parse(c.ToString())).ToArray();
    
    int maxJoltage = 0;
    
    // Try all possible pairs of positions, maintaining order
    for (int i = 0; i < digits.Length; i++)
    {
        for (int j = i + 1; j < digits.Length; j++)
        {
            // Only use the order as they appear in the string (i before j)
            int joltage = digits[i] * 10 + digits[j];
            maxJoltage = Math.Max(maxJoltage, joltage);
        }
    }
    
    sum += maxJoltage;
}
Console.WriteLine(sum);

