using System.Diagnostics;

string[] inputTokens = Console.ReadLine()!.Split(' ', StringSplitOptions.RemoveEmptyEntries);
int capacityOfBag = int.Parse(inputTokens[0]);
int numberOfItems = int.Parse(inputTokens[1]);

List<KeyValuePair<int,int>> items = new List<KeyValuePair<int,int>>();
for (int i = 0; i < numberOfItems; i++)
{
    int[] itemInput = Console.ReadLine()!.Split(' ',StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
    items.Add(new KeyValuePair<int,int>(itemInput[0],itemInput[1]));
}
var watch = new Stopwatch();
var solver = new Solver(capacityOfBag, numberOfItems, items);

watch.Start();
solver.Solve();
watch.Stop();

Console.WriteLine($"{watch.Elapsed:ss\\:fff} seconds");
