string[] lines = File.ReadAllLines("input.txt");

lines[0] = lines[0].Replace("S", "|");

char[,] grid = new char[lines.Length, lines[0].Length];
for(int i = 0; i < lines.Length; i++)
{
    for(int j = 0; j < lines[i].Length; j++)
    {
        grid[i, j] = lines[i][j];
    }
}

// print grid
for(int i = 0; i < grid.GetLength(0); i++)
{
    for(int j = 0; j < grid.GetLength(1); j++)
    {
        Console.Write(grid[i, j]);
    }
    Console.WriteLine();
}

long countSplits = 0;
for(int i = 1; i < grid.GetLength(0); i++)
{
    for(int j = 0; j < grid.GetLength(1); j++)
    {
        if(grid[i-1, j] == '|')
        {
            if(grid[i, j] == '^')
            {
                countSplits++;
                if(j>0 && grid[i, j-1] == '.')
                {
                    grid[i, j-1] = '|';
                }
                if(j<grid.GetLength(1)-1 && grid[i, j+1] == '.')
                {
                    grid[i, j+1] = '|';
                }
            } else {
                grid[i, j] = '|';
            }
        }
    }
}

// print grid
for(int i = 0; i < grid.GetLength(0); i++)
{
    for(int j = 0; j < grid.GetLength(1); j++)
    {
        Console.Write(grid[i, j]);
    }
    Console.WriteLine();
}
Console.WriteLine(countSplits);