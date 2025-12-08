string[] lines = File.ReadAllLines("input.txt");

List<JunctionBox> junctionBoxes = new List<JunctionBox>();

foreach (var line in lines){
    var parts = line.Split(',');
    junctionBoxes.Add(new JunctionBox(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2])));
}

// Create all possible pairs and calculate distances
List<Connection> connections = new List<Connection>();
for(int i = 0; i < junctionBoxes.Count; i++){
    for(int j = i + 1; j < junctionBoxes.Count; j++){
        double distance = junctionBoxes[i].CalculateDistance(junctionBoxes[j]);
        connections.Add(new Connection(i, j, distance));
    }
}

// Sort connections by distance (shortest first)
connections.Sort((a, b) => a.Distance.CompareTo(b.Distance));

// Union-Find to track circuits
UnionFind uf = new UnionFind(junctionBoxes.Count);

// Process the 1000 shortest connection attempts (including skipped ones)
int successfulConnections = 0;
for(int i = 0; i < Math.Min(1000, connections.Count); i++){
    var connection = connections[i];
    
    // Try to connect these two boxes
    bool connected = uf.Union(connection.Box1Index, connection.Box2Index);
    if(connected){
        successfulConnections++;
        Console.WriteLine($"Attempt {i+1}: Connected Box {connection.Box1Index} ({junctionBoxes[connection.Box1Index].X},{junctionBoxes[connection.Box1Index].Y},{junctionBoxes[connection.Box1Index].Z}) to Box {connection.Box2Index} ({junctionBoxes[connection.Box2Index].X},{junctionBoxes[connection.Box2Index].Y},{junctionBoxes[connection.Box2Index].Z}) - Distance: {connection.Distance:F2}");
    } else {
        Console.WriteLine($"Attempt {i+1}: Skipped - Box {connection.Box1Index} and Box {connection.Box2Index} already in same circuit - Distance: {connection.Distance:F2}");
    }
}

// Count circuit sizes
Dictionary<int, int> circuitSizes = new Dictionary<int, int>();
for(int i = 0; i < junctionBoxes.Count; i++){
    int root = uf.Find(i);
    if(!circuitSizes.ContainsKey(root)){
        circuitSizes[root] = 0;
    }
    circuitSizes[root]++;
}

Console.WriteLine($"\nTotal circuits: {circuitSizes.Count}");
Console.WriteLine("\nCircuit sizes:");

// Sort circuit sizes from largest to smallest
List<int> sizes = circuitSizes.Values.ToList();
sizes.Sort((a, b) => b.CompareTo(a));

foreach(var size in sizes){
    Console.WriteLine(size);
}

// Multiply the three largest circuit sizes
int result = sizes[0] * sizes[1] * sizes[2];
Console.WriteLine($"\nResult: {sizes[0]} * {sizes[1]} * {sizes[2]} = {result}");

class JunctionBox {
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }

    public JunctionBox(int x, int y, int z){
        X = x;
        Y = y;
        Z = z;
    }

    public double CalculateDistance(JunctionBox other){
        return Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2) + Math.Pow(Z - other.Z, 2));
    }
}

class Connection {
    public int Box1Index { get; set; }
    public int Box2Index { get; set; }
    public double Distance { get; set; }

    public Connection(int box1, int box2, double distance){
        Box1Index = box1;
        Box2Index = box2;
        Distance = distance;
    }
}

class UnionFind {
    private int[] parent;
    private int[] rank;

    public UnionFind(int size){
        parent = new int[size];
        rank = new int[size];
        for(int i = 0; i < size; i++){
            parent[i] = i;
            rank[i] = 0;
        }
    }

    public int Find(int x){
        if(parent[x] != x){
            parent[x] = Find(parent[x]); // Path compression
        }
        return parent[x];
    }

    public bool Union(int x, int y){
        int rootX = Find(x);
        int rootY = Find(y);

        if(rootX == rootY){
            return false; // Already in same set
        }

        // Union by rank
        if(rank[rootX] < rank[rootY]){
            parent[rootX] = rootY;
        } else if(rank[rootX] > rank[rootY]){
            parent[rootY] = rootX;
        } else {
            parent[rootY] = rootX;
            rank[rootX]++;
        }
        return true;
    }
}