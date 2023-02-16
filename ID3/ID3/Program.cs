using ID3;

var records = File.ReadAllLines("breast-cancer.csv")
    .Skip(1)
    .Select(line => new SingleRecord(line))
    .ToList();

Shuffle(records);

var random = new Random();
var dataCount = records.Count;
var recordsInSection = dataCount / Constants.SectionsCount;
var dataForTestStart = random.Next(0, dataCount);

var index = 0;
var rates = new List<double>();
do
{
    dataForTestStart += recordsInSection;
    dataForTestStart %= dataCount;
    var dataForTestEnd = dataForTestStart + recordsInSection;
    List<SingleRecord> dataForTest;
    
    if (dataForTestEnd >= dataCount)
    {
        dataForTest = records.GetRange(dataForTestStart, dataCount - dataForTestStart);
        dataForTest.AddRange(records.GetRange(0, dataForTestEnd - dataCount));
    }
    else
    {
        dataForTest = records.GetRange(dataForTestStart, dataForTestEnd - dataForTestStart);
    }

    var dataForTraining = records.FindAll(record => !dataForTest.Contains(record));

    var classifier = new Classifier(dataForTraining);

    var right = 0;
    var wrong = 0;
    
    foreach (var record in dataForTest)
    {
        var className = classifier.Classify(record);

        if (className == record.ClassName)
        {
            right++;
        }
        else
        {
            wrong++;
        }
    }
    
    var success = (right * 1.0) / (right + wrong);
    rates.Add(success);
    Console.WriteLine($"Wrong: {wrong}; Right: {right}; Index: {index + 1}; Accuracy: {success:P}");
    index++;
} while (index < Constants.SectionsCount);

Console.WriteLine($"Avg: {rates.Average():P}");

void Shuffle<T>(List<T> list)
{
    var random = new Random();
    var n = list.Count;
    while (n > 1)
    {
        n--;
        var k = random.Next(n + 1);
        (list[k], list[n]) = (list[n], list[k]);
    }
}