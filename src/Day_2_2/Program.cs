
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
    int length = value.Length;
    
    for (int patternLength = 1; patternLength <= length / 2; patternLength++) {
        if (length % patternLength == 0) {
            int repetitions = length / patternLength;
            if (repetitions >= 2) {
                string pattern = value[0..patternLength];
                bool isMatch = true;
                
                for (int i = 0; i < length; i++) {
                    if (value[i] != pattern[i % patternLength]) {
                        isMatch = false;
                        break;
                    }
                }
                if (isMatch) {
                    return true;
                }
            }
        }
    }
    
    return false;
}