string[] lines = File.ReadAllLines("input.txt");

// transpose lines as 2d array of chars and save it to grid
char[,] grid = new char[lines[0].Length, lines.Length];
for(int i = 0; i < lines.Length; i++)
{
    for(int j = 0; j < lines[i].Length; j++)
    {
        grid[j, i] = lines[i][j];
    }
}


long result = 0;
for(int i = 0; i < grid.GetLength(0); i++)
{
    long subResult = 0;
    if(grid[i, 4] == '+'){
        while(i < grid.GetLength(0) && (grid[i, 0] != ' ' || grid[i, 1] != ' ' || grid[i, 2] != ' ' || grid[i, 3] != ' '))
        {
            subResult += long.Parse(grid[i, 0].ToString() + grid[i, 1].ToString() + grid[i, 2].ToString() + grid[i, 3].ToString());
            i++;
        }
    }
        if(i < grid.GetLength(0) && grid[i, 4] == '*'){
            subResult = 1;
        while(i < grid.GetLength(0) && (grid[i, 0] != ' ' || grid[i, 1] != ' ' || grid[i, 2] != ' ' || grid[i, 3] != ' '))
        {
            subResult *= long.Parse(grid[i, 0].ToString() + grid[i, 1].ToString() + grid[i, 2].ToString() + grid[i, 3].ToString());
            i++;
        }
    }
    result += subResult;
}

Console.WriteLine(result);