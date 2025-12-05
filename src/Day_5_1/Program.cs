string[] lines = File.ReadAllLines("input1.txt");

var ranges = new List<(long start, long end)>();
foreach (var line in lines){
    ranges.Add((long.Parse(line.Split('-')[0]), long.Parse(line.Split('-')[1])));
}

string[] lines2 = File.ReadAllLines("input2.txt");

var ingridients = new List<long>();
foreach (var line in lines2){
    ingridients.Add(long.Parse(line));
}

long total = 0;
foreach (var ingridient in ingridients){
    // if ingridient is in any range, add 1 to total
    if (ranges.Any(range => ingridient >= range.start && ingridient <= range.end)){
        total++;
    }
}
Console.WriteLine(total);