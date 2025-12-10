string[] lines = File.ReadAllLines("input.txt");

var points = new List<(long x, long y)>();
foreach (var line in lines)
{
    var parts = line.Split(',');
    points.Add((long.Parse(parts[0]), long.Parse(parts[1])));
}

long maxArea = 0;
foreach (var point in points)
{
    long area = 0;
    foreach (var otherPoint in points)
    {
        area = (Math.Abs(point.x - otherPoint.x)+1) * (Math.Abs(point.y - otherPoint.y)+1);
        maxArea = Math.Max(maxArea, area);
    }
}
Console.WriteLine(maxArea);