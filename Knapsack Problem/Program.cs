using System.Diagnostics;
/*
5000 24
90 150
130 35
1530 200
500 160
150 60
680 45
270 60
390 40
230 30
520 10
110 70
320 30
240 15
480 10
730 40
420 70
430 75
220 80
70 20
180 12
40 50
300 10
900 1
2000 150
*/

string[] inputTokens = Console.ReadLine()!.Split(' ', StringSplitOptions.RemoveEmptyEntries);
int capacityOfBag = int.Parse(inputTokens[0]);
int numberOfItems = int.Parse(inputTokens[1]);

Dictionary<int, int> items = new Dictionary<int, int>();
for (int i = 0; i < numberOfItems; i++)
{
    int[] itemInput = Console.ReadLine()!.Split(' ',StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
    items[itemInput[0]] = itemInput[1];
}

var watch = new Stopwatch();
var solver = new Solver(capacityOfBag, numberOfItems, items);
watch.Start();
solver.Solve();
watch.Stop();
Console.WriteLine($"{watch.Elapsed:ss\\:fff} seconds");
