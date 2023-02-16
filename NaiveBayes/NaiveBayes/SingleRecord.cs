namespace NaiveBayes;

public class SingleRecord
{
    public string ClassName { get; set; }
    public bool?[] Attributes { get; set; }

    public SingleRecord(string line)
    {
        var tokens = line.Split(',');
        this.ClassName = tokens[0];
        Attributes = new bool?[Constants.AttributesCount];
        ClassName = tokens[0];
        for (var i = 1; i <= Constants.AttributesCount; i++)
        {
            Attributes[i-1] = null;
            if (tokens[i] != "?")
            {
                Attributes[i-1] = Convert.ToBoolean(tokens[i]);
            }
        }
    }
}