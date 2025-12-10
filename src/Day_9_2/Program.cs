string[] lines = File.ReadAllLines("input.txt");

var redTiles = new HashSet<(int x, int y)>();
foreach (var line in lines)
{
    var parts = line.Split(',');
    redTiles.Add((int.Parse(parts[1]), int.Parse(parts[0])));
}

int maxX = redTiles.Max(p => p.x);
int maxY = redTiles.Max(p => p.y);

var greenTiles = new HashSet<(int x, int y)>();


/*
// Print as grid
for (int i = 0; i < maxX+1; i++)
{
    for (int j = 0; j < maxY+1; j++)
    {
        Console.Write(redTiles.Contains((i, j)) ? 'R' : '.');
    }
    Console.WriteLine();
}
*/
// Add green tiles on the straight path between any two red tiles
//verbose
Console.WriteLine("Adding green tiles on the straight path between any two red tiles");
var redTilesList = redTiles.ToList();
for (int i = 0; i < redTilesList.Count; i++)
{
   var current = redTilesList[i];
   for (int j = i + 1; j < redTilesList.Count; j++)
   {
    var next = redTilesList[j];
    if (current.x == next.x)
    {
        // Vertical line
        for (int y = Math.Min(current.y, next.y); y <= Math.Max(current.y, next.y); y++)
        {
            greenTiles.Add((current.x, y));
        }
    }
    else if (current.y == next.y)
    {
        // Horizontal line
        for (int x = Math.Min(current.x, next.x); x <= Math.Max(current.x, next.x); x++)
        {
            greenTiles.Add((x, current.y));
        }
    }
   }
}
/*
// Print grid
for (int i = 0; i < maxX+1; i++)
{
    for (int j = 0; j < maxY+1; j++)
    {
        Console.Write(redTiles.Contains((i, j)) ? 'R' : greenTiles.Contains((i, j)) ? 'G' : '.');
    }
    Console.WriteLine();
}
*/
//fill area inside the G tiles with 'G'
// First, mark all tiles outside the G boundary using flood fill from edges
//verbose
Console.WriteLine("Marking all tiles outside the G boundary using flood fill from edges");
var outsideTiles = new HashSet<(int x, int y)>();
var queue = new Queue<(int x, int y)>();

// Start flood fill from all edge tiles that are not G
for (int i = 0; i <= maxX; i++)
{
    if (!greenTiles.Contains((i, 0))) queue.Enqueue((i, 0));
    if (!greenTiles.Contains((i, maxY))) queue.Enqueue((i, maxY));
}
for (int j = 0; j <= maxY; j++)
{
    if (!greenTiles.Contains((0, j))) queue.Enqueue((0, j));
    if (!greenTiles.Contains((maxX, j))) queue.Enqueue((maxX, j));
}

// Flood fill from point (1,1) to add all outside tiles to emptyTiles hashset
var emptyTiles = new HashSet<(int x, int y)>();
var queue2 = new Queue<(int x, int y)>();
queue2.Enqueue((1, 1));
emptyTiles.Add((1, 1));
while (queue2.Count > 0)
{
    var current = queue2.Dequeue();
    
    // Check all 4 neighbors
    (int x, int y)[] neighbors = new[] {
        (current.x + 1, current.y),
        (current.x - 1, current.y),
        (current.x, current.y + 1),
        (current.x, current.y - 1)
    };
    
    foreach (var neighbor in neighbors)
    {
        // Check bounds, not visited, and not a green tile
        if (neighbor.x >= 0 && neighbor.x <= maxX && 
            neighbor.y >= 0 && neighbor.y <= maxY &&
            !emptyTiles.Contains(neighbor) &&
            !greenTiles.Contains(neighbor))
        {
            emptyTiles.Add(neighbor);
            queue2.Enqueue(neighbor);
        }
    }
}
/*
// Print grid
for (int i = 0; i <= maxX; i++)
{
    for (int j = 0; j <= maxY; j++)
    {
        Console.Write(emptyTiles.Contains((i, j)) ? 'E' : redTiles.Contains((i, j)) ? 'R' : greenTiles.Contains((i, j)) ? 'G' : '.');
    }
    Console.WriteLine();
}*/


// Find largest rectangle with opposite corners on red tiles and filled with G's
//verbose
Console.WriteLine("Finding largest rectangle with opposite corners on red tiles and filled with G's");
long maxArea = 0;
(long x1, long y1, long x2, long y2) bestRectangle = (0, 0, 0, 0);

// Convert redTiles to list for iteration
var redTilesList2 = redTiles.ToList();

// Try all pairs of red tiles
for (int i = 0; i < redTilesList2.Count; i++)
{
    for (int j = i + 1; j < redTilesList2.Count; j++)
    {
        var tile1 = redTilesList2[i];
        var tile2 = redTilesList2[j];
        
        // They must have different x and y coordinates to form a rectangle
        if (tile1.x == tile2.x || tile1.y == tile2.y)
            continue;
        
        // Define the rectangle boundaries
        long minRectX = Math.Min(tile1.x, tile2.x);
        long maxRectX = Math.Max(tile1.x, tile2.x);
        long minRectY = Math.Min(tile1.y, tile2.y);
        long maxRectY = Math.Max(tile1.y, tile2.y);
        
        // Check if no tiles in the rectangle (including boundaries) are in emptyTiles
        bool isValid = true;
        for (long x = minRectX; x <= maxRectX && isValid; x++)
        {
            for (long y = minRectY; y <= maxRectY && isValid; y++)
            {
                if (emptyTiles.Contains(((int)x, (int)y)))
                {
                    isValid = false;
                }
            }
        }
        
        if (isValid)
        {
            long area = (maxRectX - minRectX + 1) * (maxRectY - minRectY + 1);
            if (area > maxArea)
            {
                maxArea = area;
                bestRectangle = (minRectX, minRectY, maxRectX, maxRectY);
            }
        }
    }
}

Console.WriteLine($"\nLargest rectangle area: {maxArea}");
Console.WriteLine($"Rectangle corners: ({bestRectangle.x1},{bestRectangle.y1}) to ({bestRectangle.x2},{bestRectangle.y2})");


