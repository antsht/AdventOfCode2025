string[] shapeLines = File.ReadAllLines("shapes.txt");
string[] zoneLines = File.ReadAllLines("zones.txt");

// Parse shapes and pre-compute all orientations
var shapes = ParseShapes(shapeLines);
var allOrientations = shapes.Select(GetAllOrientations).ToList();
var shapeSizes = shapes.Select(s => s.Count).ToArray();

// Pre-compute all possible white cell counts for each shape (considering all orientations and positions)
var shapePossibleWhites = new List<HashSet<int>>();
for (int i = 0; i < shapes.Count; i++)
{
    var possibleWhites = new HashSet<int>();
    foreach (var orientation in allOrientations[i])
    {
        int white = 0;
        foreach (var (r, c) in orientation)
        {
            if ((r + c) % 2 == 0) white++;
        }
        // At even position: contributes 'white' white cells
        // At odd position: contributes 'size - white' white cells
        possibleWhites.Add(white);
        possibleWhites.Add(shapeSizes[i] - white);
    }
    shapePossibleWhites.Add(possibleWhites);
}

// Process zones in parallel for speed
int count = zoneLines
    .AsParallel()
    .Count(zoneLine =>
    {
        var (width, height, presentCounts) = ParseZone(zoneLine);
        
        // Quick area check
        int totalCells = 0;
        for (int i = 0; i < presentCounts.Length && i < shapeSizes.Length; i++)
        {
            totalCells += presentCounts[i] * shapeSizes[i];
        }
        
        if (totalCells > width * height)
        {
            return false;
        }
        
        // Parity check - shapes must be able to fit within grid's white/black cell budget
        if (!ParityCheckPossible(width, height, presentCounts, shapePossibleWhites, shapeSizes))
        {
            return false;
        }
        
        // Area + parity checks passed - shapes can fit
        return true;
    });

Console.WriteLine($"Number of regions that can fit all presents: {count}");

// Check if parity constraints can be satisfied (shapes must fit within grid's white/black budget)
static bool ParityCheckPossible(int width, int height, int[] counts, List<HashSet<int>> shapePossibleWhites, int[] shapeSizes)
{
    // Calculate grid white/black cells (checkerboard pattern)
    int gridWhite = ((height + 1) / 2) * ((width + 1) / 2) + (height / 2) * (width / 2);
    int gridBlack = width * height - gridWhite;
    
    // For each shape type, calculate min and max white cells it can contribute per piece
    int minWhiteTotal = 0, maxWhiteTotal = 0;
    int minBlackTotal = 0, maxBlackTotal = 0;
    
    for (int i = 0; i < counts.Length && i < shapePossibleWhites.Count; i++)
    {
        if (counts[i] == 0) continue;
        
        int size = shapeSizes[i];
        int minW = shapePossibleWhites[i].Min();
        int maxW = shapePossibleWhites[i].Max();
        
        // For n pieces, white can range from n*minW to n*maxW
        minWhiteTotal += counts[i] * minW;
        maxWhiteTotal += counts[i] * maxW;
        
        // Black is size - white, so black ranges from n*(size-maxW) to n*(size-minW)
        minBlackTotal += counts[i] * (size - maxW);
        maxBlackTotal += counts[i] * (size - minW);
    }
    
    // Check if there's an achievable (white, black) that fits within grid budget
    // We need: minWhiteTotal <= white <= min(maxWhiteTotal, gridWhite)
    //      AND: minBlackTotal <= black <= min(maxBlackTotal, gridBlack)
    // Since white + black = totalCells (constant), we just check if ranges overlap with grid constraints
    
    return minWhiteTotal <= gridWhite && minBlackTotal <= gridBlack;
}

// Parse shapes from input
static List<HashSet<(int row, int col)>> ParseShapes(string[] lines)
{
    var shapes = new List<HashSet<(int row, int col)>>();
    var currentShape = new HashSet<(int row, int col)>();
    int row = 0;
    
    foreach (var line in lines)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            if (currentShape.Count > 0)
            {
                shapes.Add(currentShape);
                currentShape = new HashSet<(int row, int col)>();
                row = 0;
            }
            continue;
        }
        
        if (line.Contains(':'))
        {
            continue;
        }
        
        for (int col = 0; col < line.Length; col++)
        {
            if (line[col] == '#')
            {
                currentShape.Add((row, col));
            }
        }
        row++;
    }
    
    if (currentShape.Count > 0)
    {
        shapes.Add(currentShape);
    }
    
    return shapes;
}

// Parse zone line like "4x4: 0 0 0 0 2 0"
static (int width, int height, int[] counts) ParseZone(string line)
{
    var parts = line.Split(':');
    var dims = parts[0].Split('x');
    int width = int.Parse(dims[0]);
    int height = int.Parse(dims[1]);
    var counts = parts[1].Trim().Split(' ').Select(int.Parse).ToArray();
    return (width, height, counts);
}

// Generate all rotations and flips of a shape (normalized)
static List<HashSet<(int row, int col)>> GetAllOrientations(HashSet<(int row, int col)> shape)
{
    var orientations = new List<HashSet<(int row, int col)>>();
    var current = shape;
    
    for (int r = 0; r < 4; r++)
    {
        orientations.Add(Normalize(current));
        orientations.Add(Normalize(FlipHorizontal(current)));
        current = Rotate90(current);
    }
    
    // Remove duplicates
    var unique = new List<HashSet<(int row, int col)>>();
    foreach (var orientation in orientations)
    {
        bool isDuplicate = unique.Any(existing => existing.SetEquals(orientation));
        if (!isDuplicate)
        {
            unique.Add(orientation);
        }
    }
    
    return unique;
}

static HashSet<(int row, int col)> Rotate90(HashSet<(int row, int col)> shape)
    => shape.Select(p => (p.col, -p.row)).ToHashSet();

static HashSet<(int row, int col)> FlipHorizontal(HashSet<(int row, int col)> shape)
    => shape.Select(p => (p.row, -p.col)).ToHashSet();

static HashSet<(int row, int col)> Normalize(HashSet<(int row, int col)> shape)
{
    int minRow = shape.Min(p => p.row);
    int minCol = shape.Min(p => p.col);
    return shape.Select(p => (p.row - minRow, p.col - minCol)).ToHashSet();
}
