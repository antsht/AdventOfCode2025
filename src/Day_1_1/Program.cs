// read input.txt
string[] input = File.ReadAllLines("input.txt");
// parse input L00 or R00
var parsedInput = input.Select(line => line[0]=='L' ? -int.Parse(line[1..]) : int.Parse(line[1..])).ToList();

// calculate the final position. Strat from 50, result is 0-99. 0-1 gives 99, 99-1 gives 98, etc.
int startPosition = 50;
int currentPosition = startPosition;
int zeroes = 0;
foreach (var number in parsedInput) {
    currentPosition = ((currentPosition + number)<0 ? 100+(currentPosition + number):(currentPosition+number)) % 100;
    if (currentPosition == 0) {
        zeroes++;
    }
}
Console.WriteLine(zeroes);