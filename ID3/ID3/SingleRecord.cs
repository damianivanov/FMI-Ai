namespace ID3;

public class SingleRecord
{
    public string ClassName { get; set; }
    public Dictionary<string,string> AttributeDictionary { get; set; }
    public SingleRecord(string line)
    {
        AttributeDictionary = new Dictionary<string, string>();
        var tokens = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
        ClassName = tokens[0];
        for (int i = 1; i < tokens.Length; i++)
        {
            var value = Constants.Attributes[Constants.Keys[i-1]].Contains(tokens[i]) ? tokens[i]: "?";
            AttributeDictionary.Add(Constants.Keys[i - 1], value);
        }

    }
}