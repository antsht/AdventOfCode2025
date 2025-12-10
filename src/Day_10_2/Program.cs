string[] lines = File.ReadAllLines("input.txt");

long totalPresses = 0;

foreach (string line in lines)
{
    if (string.IsNullOrWhiteSpace(line)) continue;
    
    // Parse buttons and joltage
    int endBracket = line.IndexOf(']');
    int startCurly = line.IndexOf('{');
    int endCurly = line.IndexOf('}');
    
    string buttonSection = line.Substring(endBracket + 1, startCurly - endBracket - 1);
    string joltageStr = line.Substring(startCurly + 1, endCurly - startCurly - 1);
    
    int[] targets = joltageStr.Split(',').Select(s => int.Parse(s.Trim())).ToArray();
    int numCounters = targets.Length;
    
    List<int[]> buttons = new List<int[]>();
    int pos = 0;
    while (true)
    {
        int startParen = buttonSection.IndexOf('(', pos);
        if (startParen == -1) break;
        int endParen = buttonSection.IndexOf(')', startParen);
        string buttonStr = buttonSection.Substring(startParen + 1, endParen - startParen - 1);
        buttons.Add(buttonStr.Split(',').Select(s => int.Parse(s.Trim())).ToArray());
        pos = endParen + 1;
    }
    
    int minPresses = SolveGaussianElimination(targets, buttons);
    totalPresses += minPresses;
}

Console.WriteLine(totalPresses);

static int SolveGaussianElimination(int[] targets, List<int[]> buttons)
{
    int numCounters = targets.Length;
    int numButtons = buttons.Count;
    
    // Build augmented matrix [A | b] using integers
    int[,] matrix = new int[numCounters, numButtons + 1];
    for (int i = 0; i < numCounters; i++)
    {
        for (int j = 0; j < numButtons; j++)
        {
            matrix[i, j] = buttons[j].Contains(i) ? 1 : 0;
        }
        matrix[i, numButtons] = targets[i];
    }
    
    // Forward elimination (row echelon form)
    var pivotCols = new List<int>();
    int currentRow = 0;
    
    for (int col = 0; col < numButtons && currentRow < numCounters; col++)
    {
        // Find pivot
        int pivotRow = -1;
        for (int row = currentRow; row < numCounters; row++)
        {
            if (matrix[row, col] != 0)
            {
                pivotRow = row;
                break;
            }
        }
        
        if (pivotRow == -1) continue;
        
        // Swap rows
        if (pivotRow != currentRow)
        {
            for (int j = 0; j <= numButtons; j++)
            {
                (matrix[currentRow, j], matrix[pivotRow, j]) = (matrix[pivotRow, j], matrix[currentRow, j]);
            }
        }
        
        pivotCols.Add(col);
        
        // Eliminate below
        for (int row = currentRow + 1; row < numCounters; row++)
        {
            if (matrix[row, col] != 0)
            {
                int factor = matrix[row, col];
                int pivotVal = matrix[currentRow, col];
                
                for (int j = col; j <= numButtons; j++)
                {
                    matrix[row, j] = matrix[row, j] * pivotVal - matrix[currentRow, j] * factor;
                }
                
                // Reduce by GCD to prevent coefficient explosion
                int gcd = 0;
                for (int j = col; j <= numButtons; j++)
                {
                    if (matrix[row, j] != 0)
                    {
                        gcd = gcd == 0 ? Math.Abs(matrix[row, j]) : GCD(gcd, Math.Abs(matrix[row, j]));
                    }
                }
                if (gcd > 1)
                {
                    for (int j = col; j <= numButtons; j++)
                    {
                        matrix[row, j] /= gcd;
                    }
                }
            }
        }
        
        currentRow++;
    }
    
    // Find free variables
    var pivotSet = new HashSet<int>(pivotCols);
    var freeVars = new List<int>();
    for (int i = 0; i < numButtons; i++)
    {
        if (!pivotSet.Contains(i))
        {
            freeVars.Add(i);
        }
    }
    
    // No free variables - unique solution or no solution
    if (freeVars.Count == 0)
    {
        var sol = TrySolution(matrix, numButtons, numCounters, pivotCols, freeVars, new int[0]);
        return sol != null ? sol.Sum() : -1;
    }
    
    int[]? bestSolution = null;
    int bestSum = int.MaxValue;
    
    // Compute search bounds based on free variable count
    int maxVal = freeVars.Count switch
    {
        1 => Math.Max(matrix[0, numButtons] * 3, 2000),
        2 => ComputeMaxVal(matrix, pivotCols, numButtons),
        3 => 500,
        4 => 200,
        5 => 100,
        _ => 0
    };
    
    if (freeVars.Count > 5)
    {
        var sol = TrySolution(matrix, numButtons, numCounters, pivotCols, freeVars, new int[freeVars.Count]);
        return sol != null ? sol.Sum() : -1;
    }
    
    int[] current = new int[freeVars.Count];
    SearchFreeVars(0);
    
    return bestSolution != null ? bestSum : -1;
    
    void SearchFreeVars(int depth)
    {
        if (depth == freeVars.Count)
        {
            var solution = TrySolution(matrix, numButtons, numCounters, pivotCols, freeVars, current);
            if (solution != null)
            {
                int totalPresses = solution.Sum();
                if (totalPresses < bestSum)
                {
                    bestSum = totalPresses;
                    bestSolution = solution;
                }
            }
            return;
        }
        
        int limit = freeVars.Count == 1 ? maxVal + 1 : maxVal;
        
        // Pruning: skip if current partial sum already exceeds best
        int partialSum = 0;
        for (int i = 0; i < depth; i++)
        {
            partialSum += current[i];
        }
        
        for (int v = 0; v < limit; v++)
        {
            if (partialSum + v >= bestSum)
                break;
            
            current[depth] = v;
            SearchFreeVars(depth + 1);
        }
    }
    
    static int ComputeMaxVal(int[,] matrix, List<int> pivotCols, int numButtons)
    {
        int maxVal = 0;
        for (int row = 0; row < pivotCols.Count; row++)
        {
            maxVal = Math.Max(maxVal, Math.Abs(matrix[row, numButtons]));
        }
        return Math.Min(Math.Max(maxVal * 2, 500), 2000);
    }
}

static int[]? TrySolution(int[,] matrix, int numButtons, int numCounters, List<int> pivotCols, List<int> freeVars, int[] freeValues)
{
    int[] solution = new int[numButtons];
    
    // Set free variables
    for (int i = 0; i < freeVars.Count; i++)
    {
        solution[freeVars[i]] = freeValues[i];
    }
    
    // Back substitution for pivot variables
    for (int i = pivotCols.Count - 1; i >= 0; i--)
    {
        int row = i;
        int col = pivotCols[i];
        int total = matrix[row, numButtons];
        
        for (int j = col + 1; j < numButtons; j++)
        {
            total -= matrix[row, j] * solution[j];
        }
        
        if (matrix[row, col] == 0 || total % matrix[row, col] != 0)
            return null;
        
        int val = total / matrix[row, col];
        if (val < 0) return null;
        
        solution[col] = val;
    }
    
    // Verify solution against original equations
    for (int i = 0; i < numCounters; i++)
    {
        int sum = 0;
        for (int j = 0; j < numButtons; j++)
        {
            sum += matrix[i, j] * solution[j];
        }
        if (sum != matrix[i, numButtons]) return null;
    }
    
    return solution;
}

static int GCD(int a, int b)
{
    while (b != 0)
    {
        int temp = b;
        b = a % b;
        a = temp;
    }
    return a;
}
