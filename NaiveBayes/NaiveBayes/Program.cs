using NaiveBayes;
var records = File.ReadAllLines("house-votes-84.csv")
    .Skip(1)
    .Select(line => new SingleRecord(line))
    .ToList();
    
Shuffle(records);

var rnd = new Random();
var recordsInSection = records.Count / Constants.Sections;
var testRecordsIndex = rnd.Next(0, records.Count);
var i = 0;
var acc = new List<double>();
do
{
    testRecordsIndex += recordsInSection;
    testRecordsIndex %= records.Count;
    
    var testRecordEndIndex = testRecordsIndex + recordsInSection;
    List<SingleRecord> testRecords;
    if (testRecordEndIndex < records.Count)
    {
        testRecords = records.GetRange(testRecordsIndex, testRecordEndIndex - testRecordsIndex);
    }
    else
    {
        testRecords = records.GetRange(testRecordsIndex, records.Count - testRecordsIndex);
        testRecords.AddRange(records.GetRange(0,testRecordEndIndex-records.Count));
    }
    
    var trainingRecords = records.FindAll(record => !testRecords.Contains(record));
    var classifier = new Classifier(trainingRecords);
    int wrong = 0;
    int right = 0;
    foreach (var testRecord in testRecords)
    {
        var isDemocrat = classifier.Classify(testRecord);
        if ((isDemocrat && testRecord.ClassName == Constants.Democrat) || (!isDemocrat && testRecord.ClassName == Constants.Republican))
            right++;
        else
            wrong++;
    }
    Console.WriteLine($"Wrong: {wrong}; Right: {right}; Accuracy: {(right * 1.0)*100 / (right + wrong):F2}%");
    acc.Add(right* 100.0 / (right + wrong));
    i++;
} while (i < Constants.Sections);

Console.WriteLine($"Mean Accuracy: {acc.Average():F2}");

List<T> Shuffle<T>(List<T> list)
{
    var random = new Random();
    var n = list.Count;
    while (n > 1)
    {
        n--;
        var k = random.Next(n + 1);
        (list[k], list[n]) = (list[n], list[k]);
    }
    return list;
}