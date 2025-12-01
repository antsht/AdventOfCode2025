// if TEST is defined, read input.txt
//#define TEST
#if TEST
// read input.txt
string[] input = File.ReadAllLines("input.txt");
#else
// get input from https://adventofcode.com/2025/day/1/input
// add cookie to the request
HttpClient client = new HttpClient();
client.DefaultRequestHeaders.Add("Cookie", "session=MYSECRETSESSIONCOOKIE;");
string inputRaw = await client.GetStringAsync("https://adventofcode.com/2025/day/1/input");
Console.WriteLine(inputRaw);
// to array of lines except the last line
string[] input = inputRaw.Split('\n')[..^1];
#endif




// parse input L00 or R00
var parsedInput = input.Select(line => line[0]=='L' ? -int.Parse(line[1..]) : int.Parse(line[1..])).ToList();

// calculate the final position. Strat from 50, result is 0-99. 0-1 gives 99, 99-1 gives 98, etc.
int startPosition = 50;
int currentPosition = startPosition;
int zeroes = 0;
foreach (var number in parsedInput) {
    zeroes+= Math.Abs(number) / 100;
    int number_normalized = number % 100;
    if ((currentPosition!= 0 && currentPosition + number_normalized < 0) || currentPosition + number_normalized > 100) {
        zeroes++;
    }
    
    currentPosition = ((currentPosition + number_normalized)<0 ? 100+(currentPosition + number_normalized):(currentPosition+number_normalized)) % 100;
    if (currentPosition == 0) {
        zeroes++;
    }
}
Console.WriteLine(zeroes);