
string[] lines = File.ReadAllLines("input.txt");

int[,] grid = new int[lines[0].Length, lines.Length];

for(int i = 0; i < lines.Length; i++)
{
    for(int j = 0; j < lines[i].Length; j++)
    {
        grid[i, j] = lines[i][j] == '@' ? 1 : 0;
    }
}
bool hasRollToRemove;
do {
hasRollToRemove = false;
for(int i = 0; i < grid.GetLength(0); i++)
{
    for(int j = 0; j < grid.GetLength(1); j++)
    {
        // skip not '@'
        if(grid[i, j] !=1 ) continue;

        if(countAdjacent(i, j) < 4){
            grid[i, j] = 2; //mark going to be removed roll
            hasRollToRemove = true;
        }
    }
}
// remove marked rolls
for(int i = 0; i < grid.GetLength(0); i++)
{
    for(int j = 0; j < grid.GetLength(1); j++)
    {
        if(grid[i, j] == 2) grid[i, j] = 3;
    }
}

} while (hasRollToRemove);

int count = 0;
for(int i = 0; i < grid.GetLength(0); i++)
{
    for(int j = 0; j < grid.GetLength(1); j++)
    {
        count += grid[i, j] == 3 ? 1 : 0;
    }
}
Console.WriteLine(count);

int countAdjacent(int i, int j)
{
    int count = 0;
    for(int x = -1; x <= 1; x++)
    {
        for(int y = -1; y <= 1; y++)
        {
            if(x == 0 && y == 0) continue;
            if(i + x < 0 || i + x >= grid.GetLength(0) || j + y < 0 || j + y >= grid.GetLength(1)) continue;
            count += grid[i + x, j + y] == 1 || grid[i + x, j + y] == 2 ? 1 : 0;
        }
    }
    return count;
}