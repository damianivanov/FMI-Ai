using System.Globalization;

var dataFile = Console.ReadLine();
var clustersCount = int.Parse(Console.ReadLine()!);
var data = File.ReadAllLines($"{dataFile}")
                .Skip(1)
                .Select(line => line.Split(' ','\t',StringSplitOptions.RemoveEmptyEntries).Select(x=>double.Parse(x,CultureInfo.InvariantCulture)).ToList())
                .ToList();

var algorithm = new KMeans(data, clustersCount);
algorithm.Execute();