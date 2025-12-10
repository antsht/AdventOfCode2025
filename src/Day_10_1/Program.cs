string[] lines = File.ReadAllLines("input.txt");

int totalPresses = 0;

foreach (string line in lines)
{
    if (string.IsNullOrWhiteSpace(line)) continue;
    
    // Parse the target pattern [...]
    int startBracket = line.IndexOf('[');
    int endBracket = line.IndexOf(']');
    string pattern = line.Substring(startBracket + 1, endBracket - startBracket - 1);
    
    // Convert pattern to target bitmask (# = on = 1, . = off = 0)
    int target = 0;
    for (int i = 0; i < pattern.Length; i++)
    {
        if (pattern[i] == '#')
            target |= (1 << i);
    }
    
    // Find where joltage starts (to exclude it from button parsing)
    int startCurly = line.IndexOf('{');
    string buttonSection = startCurly != -1 
        ? line.Substring(endBracket + 1, startCurly - endBracket - 1) 
        : line.Substring(endBracket + 1);
    
    // Parse buttons (...) - each button is a bitmask of lights it toggles
    List<int> buttons = new List<int>();
    int pos = 0;
    while (true)
    {
        int startParen = buttonSection.IndexOf('(', pos);
        if (startParen == -1) break;
        int endParen = buttonSection.IndexOf(')', startParen);
        string buttonStr = buttonSection.Substring(startParen + 1, endParen - startParen - 1);
        
        int buttonMask = 0;
        foreach (string num in buttonStr.Split(','))
        {
            int lightIndex = int.Parse(num.Trim());
            buttonMask |= (1 << lightIndex);
        }
        buttons.Add(buttonMask);
        pos = endParen + 1;
    }
    
    // Find minimum presses using brute force over all 2^n combinations
    // Since pressing a button twice cancels out, we only need 0 or 1 presses per button
    int minPresses = int.MaxValue;
    int numButtons = buttons.Count;
    
    for (int mask = 0; mask < (1 << numButtons); mask++)
    {
        int state = 0;  // All lights start off
        int presses = 0;
        
        for (int i = 0; i < numButtons; i++)
        {
            if ((mask & (1 << i)) != 0)
            {
                state ^= buttons[i];  // XOR to toggle lights
                presses++;
            }
        }
        
        if (state == target)
        {
            minPresses = Math.Min(minPresses, presses);
        }
    }
    
    totalPresses += minPresses;
}

Console.WriteLine(totalPresses);
