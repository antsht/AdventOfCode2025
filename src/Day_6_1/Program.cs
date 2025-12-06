string[] lines = File.ReadAllLines("input.txt");

List<long> numbers1 = lines[0].Split(' ').Select(long.Parse).ToList();
List<long> numbers2 = lines[1].Split(' ').Select(long.Parse).ToList();
List<long> numbers3 = lines[2].Split(' ').Select(long.Parse).ToList();
List<long> numbers4 = lines[3].Split(' ').Select(long.Parse).ToList();

List<char> operations = new List<char>();
for(int i = 0; i < lines[4].Length; i++)
{
    if(lines[4][i] == '+')
    {
        operations.Add('+');
    }
    else if(lines[4][i] == '*')
    {
        operations.Add('*');
    }
}

long result = 0;
for(int i = 0; i < numbers1.Count; i++)
{
    switch (operations[i])
    {
        case '+':
            result += numbers1[i] + numbers2[i] + numbers3[i] + numbers4[i];
            break;
        case '*':
            result += numbers1[i] * numbers2[i] * numbers3[i] * numbers4[i];
            break;
    }
}
Console.WriteLine(result);