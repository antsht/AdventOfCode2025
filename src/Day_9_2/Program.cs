string[] lines = File.ReadAllLines("input.txt");

var redTiles = new List<(int x, int y)>();
foreach (var line in lines)
{
    var parts = line.Split(',');
    redTiles.Add((int.Parse(parts[1]), int.Parse(parts[0])));
}

Console.WriteLine($"Loaded {redTiles.Count} red tiles");

// Coordinate compression - reduces ~100k grid to ~500 unique coordinates
var allX = redTiles.Select(p => p.x).Distinct().OrderBy(x => x).ToList();
var allY = redTiles.Select(p => p.y).Distinct().OrderBy(y => y).ToList();

var xToIdx = new Dictionary<int, int>();
var yToIdx = new Dictionary<int, int>();
for (int i = 0; i < allX.Count; i++) xToIdx[allX[i]] = i;
for (int i = 0; i < allY.Count; i++) yToIdx[allY[i]] = i;

int nx = allX.Count;
int ny = allY.Count;

Console.WriteLine($"Compressed to {nx} x {ny} grid");

// Build vertical edges from CONSECUTIVE red tiles only (polygon edges)
var verticalEdges = new List<(int x, int y1, int y2)>();
for (int i = 0; i < redTiles.Count; i++)
{
    var (x1, y1) = redTiles[i];
    var (x2, y2) = redTiles[(i + 1) % redTiles.Count]; // Wraps around
    
    if (x1 == x2) // vertical edge
    {
        verticalEdges.Add((x1, Math.Min(y1, y2), Math.Max(y1, y2)));
    }
}

Console.WriteLine($"Found {verticalEdges.Count} vertical edges");

// Mark cells as inside/outside using ray casting
// Cell (i, j) is the region between allX[i..i+1] and allY[j..j+1]
Console.WriteLine("Marking cells inside/outside polygon...");
bool[,] isInside = new bool[nx - 1, ny - 1];

for (int i = 0; i < nx - 1; i++)
{
    for (int j = 0; j < ny - 1; j++)
    {
        // Test point: center of cell
        double cx = (allX[i] + allX[i + 1]) / 2.0;
        double cy = (allY[j] + allY[j + 1]) / 2.0;
        
        // Ray casting: count vertical edges to the left that span this y
        int count = 0;
        foreach (var (ex, ey1, ey2) in verticalEdges)
        {
            if (ex < cx && ey1 < cy && cy < ey2)
            {
                count++;
            }
        }
        isInside[i, j] = (count % 2 == 1);
    }
}

// Build 2D prefix sum of "outside" cells for O(1) rectangle queries
Console.WriteLine("Building prefix sums...");
int[,] outsidePrefix = new int[nx, ny];

for (int i = 0; i < nx - 1; i++)
{
    for (int j = 0; j < ny - 1; j++)
    {
        int val = isInside[i, j] ? 0 : 1;
        outsidePrefix[i + 1, j + 1] = val
            + outsidePrefix[i, j + 1]
            + outsidePrefix[i + 1, j]
            - outsidePrefix[i, j];
    }
}

// Function to count outside cells in rectangle [x1, x2) x [y1, y2) in compressed coords
int CountOutside(int x1, int y1, int x2, int y2)
{
    return outsidePrefix[x2, y2] - outsidePrefix[x1, y2] - outsidePrefix[x2, y1] + outsidePrefix[x1, y1];
}

// Find largest valid rectangle with red tile corners
Console.WriteLine("Finding largest rectangle...");
long maxArea = 0;
(int x1, int y1, int x2, int y2) bestRect = (0, 0, 0, 0);

for (int i = 0; i < redTiles.Count; i++)
{
    for (int j = i + 1; j < redTiles.Count; j++)
    {
        var (rx1, ry1) = redTiles[i];
        var (rx2, ry2) = redTiles[j];
        
        // Skip if same row or column (degenerate rectangle)
        if (rx1 == rx2 || ry1 == ry2) continue;
        
        int cx1 = xToIdx[rx1], cy1 = yToIdx[ry1];
        int cx2 = xToIdx[rx2], cy2 = yToIdx[ry2];
        
        int minCx = Math.Min(cx1, cx2), maxCx = Math.Max(cx1, cx2);
        int minCy = Math.Min(cy1, cy2), maxCy = Math.Max(cy1, cy2);
        
        // Check if rectangle is valid (no outside cells) using O(1) query
        if (CountOutside(minCx, minCy, maxCx, maxCy) == 0)
        {
            // Calculate actual area using original coordinates
            long width = (long)allX[maxCx] - allX[minCx] + 1;
            long height = (long)allY[maxCy] - allY[minCy] + 1;
            long area = width * height;
            
            if (area > maxArea)
            {
                maxArea = area;
                bestRect = (allX[minCx], allY[minCy], allX[maxCx], allY[maxCy]);
            }
        }
    }
}

Console.WriteLine($"\nLargest rectangle area: {maxArea}");
Console.WriteLine($"Rectangle corners: ({bestRect.x1},{bestRect.y1}) to ({bestRect.x2},{bestRect.y2})");
