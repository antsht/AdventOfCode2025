int pos = 50, zeros = 0;
foreach (var line in File.ReadLines("input.txt"))
{
    int d = (line[0] == 'L' ? -1 : 1) * int.Parse(line[1..]);
    int norm = d % 100, newPos = pos + norm;
    zeros += Math.Abs(d) / 100;
    if ((pos != 0 && newPos < 0) || newPos > 100) zeros++;
    pos = (newPos % 100 + 100) % 100;
    if (pos == 0) zeros++;
}
Console.WriteLine(zeros);