
string line = File.ReadLines("input.txt").First();
string[] ranges = line.Split(',');
long total = 0;
foreach (var range in ranges) {
     (long rangeStart, long rangeEnd) = (long.Parse(range.Split('-')[0]), long.Parse(range.Split('-')[1]));
     for (long i = rangeStart; i <= rangeEnd; i++) {
        total += CheckValue(i.ToString())? i : 0;
     }
}
Console.WriteLine(total);

bool CheckValue(string value) {
    int digits = value.Length;
    if (digits % 2 == 1) {
        return false;
    }
    if (value[0..(digits/2)] != value[(digits/2)..digits]) {
        return false;
    }
    return true;
}