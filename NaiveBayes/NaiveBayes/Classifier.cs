namespace NaiveBayes;

public class Classifier
{
    private int DemocratsCount { get; set; }
    private int RepublicansCount { get; set; }
    private int[,,] AttributeValueCounts { get; set; }
    
    public List<SingleRecord> Records { get; set; }

    public Classifier(List<SingleRecord> records)
    {
        this.Records = records;
        RepublicansCount = 1;
        DemocratsCount = 1;
        
        AttributeValueCounts = GetAttributeValueCounts();
        AddMissing();
    }
    public bool Classify(SingleRecord record)
    {
        var republicanPartialProbability = PartialProbability(Constants.Republican, record);
        var democratPartialProbability = PartialProbability(Constants.Democrat, record);
        
        var evidence = republicanPartialProbability + democratPartialProbability;
        
        var republicanProb = republicanPartialProbability / evidence;
        var democratProb = democratPartialProbability / evidence;
        
        return democratProb > republicanProb;
    }
    private void AddMissing()
    {
        for (var i = 0; i < Constants.AttributesCount; i++)
        {
            foreach (var record in Records)
            {
                if (record.Attributes[i] != null) continue;
                var trueValuesCount = AttributeValueCounts[i, 1, 0] + AttributeValueCounts[i, 1, 1];
                var falseValuesCount = AttributeValueCounts[i, 0, 0] + AttributeValueCounts[i, 0, 1];
                record.Attributes[i] = trueValuesCount >= falseValuesCount;
            }
        }
    }
    private double PartialProbability(string className, SingleRecord record)
    {
        var classNameIndex = className == Constants.Democrat ? 1 : 0;
        var total = className == Constants.Democrat ? DemocratsCount : RepublicansCount;

        var result = 1.0;
        for (var i = 0; i < Constants.AttributesCount; i++)
        {
            result *= (AttributeValueCounts[i, Convert.ToInt32(record.Attributes[i]), classNameIndex]+1) / (total * 1.0);
        }

        return result;
    }
    private int[,,] GetAttributeValueCounts()
    {
        var result = new int[Constants.AttributesCount, 2, 2];
        foreach (var record in Records)
        {
            //className democrat = 1 , republican = 0
            for (var i = 0; i < Constants.AttributesCount; i++)
            {
                var classNameIndex = 0;
                RepublicansCount++;

                if (record.ClassName == Constants.Democrat)
                {
                    classNameIndex = 1; 
                    DemocratsCount++;
                    RepublicansCount--;
                }

                result[i, Convert.ToInt32(record.Attributes[i]), classNameIndex]++;
            }
        }

        return result;
    }
    
}