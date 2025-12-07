string[] lines = File.ReadAllLines("input.txt");

lines[0] = lines[0].Replace("S", "|");

char[,] grid = new char[lines.Length, lines[0].Length];
for (int i = 0; i < lines.Length; i++)
{
    for (int j = 0; j < lines[i].Length; j++)
    {
        grid[i, j] = lines[i][j];
    }
}


long countSplits = 0;
for (int i = 1; i < grid.GetLength(0); i++)
{
    for (int j = 0; j < grid.GetLength(1); j++)
    {
        if (grid[i - 1, j] == '|')
        {
            if (grid[i, j] == '^')
            {
                countSplits++;
                if (j > 0 && grid[i, j - 1] == '.')
                {
                    grid[i, j - 1] = '|';
                }
                if (j < grid.GetLength(1) - 1 && grid[i, j + 1] == '.')
                {
                    grid[i, j + 1] = '|';
                }
            }
            else
            {
                grid[i, j] = '|';
            }
        }
    }
}



long[,] dp = new long[grid.GetLength(0), grid.GetLength(1)];
for (int j = 0; j < grid.GetLength(1); j++)
{
    if (grid[0, j] == '|')
        dp[0, j] = 1;
}

for (int i = 0; i < grid.GetLength(0) - 1; i++)
{
    for (int j = 0; j < grid.GetLength(1); j++)
    {
        if (grid[i + 1, j] == '|' && grid[i, j] == '|')
        {
            dp[i + 1, j] += dp[i, j];
        }
        if (grid[i + 1, j] == '^')
        {
            if (j > 0 && grid[i + 1, j - 1] == '|')
            {
                dp[i + 1, j - 1] += dp[i, j];
            }
            if (j < grid.GetLength(1) - 1 && grid[i + 1, j + 1] == '|')
            {
                dp[i + 1, j + 1] += dp[i, j];
            }
        }
    }

}

long sum = 0;
for (int j = 0; j < dp.GetLength(1); j++)
{
    sum += dp[dp.GetLength(0) - 1, j];
}
Console.WriteLine(sum);