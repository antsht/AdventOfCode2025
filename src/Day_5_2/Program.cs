string[] lines = File.ReadAllLines("input1.txt");

var ranges = new List<(long start, long end)>();
foreach (var line in lines){
    ranges.Add((long.Parse(line.Split('-')[0]), long.Parse(line.Split('-')[1])));
}

ranges.Sort((a, b) => a.start.CompareTo(b.start));

for (int i = 0; i < ranges.Count-1; i++){
    if (ranges[i+1].start >= ranges[i].start && ranges[i+1].start <= ranges[i].end){
        ranges[i+1] = (ranges[i].start, Math.Max(ranges[i+1].end, ranges[i].end));
        ranges[i] = (0, 0);
    }
}

ranges.RemoveAll(range => range.start == 0 && range.end == 0);

long total = 0;
foreach (var range in ranges)
{
    total += range.end - range.start + 1;
}

Console.WriteLine(total);