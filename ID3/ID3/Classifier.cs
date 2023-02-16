using System.Dynamic;
using System.Security.Cryptography.X509Certificates;

namespace ID3;

public class Node
{
    public IList<Node> Children { get; set; }
    public string Value { get; set; }
    public string Decision { get; set; }
    public string Attribute { get; set; }

    public Node()
    {
        this.Children = new List<Node>();
    }
}
public class Classifier
{
    public IList<SingleRecord> Data { get; set; }
    private Node Root { get; set; }
    public Classifier(List<SingleRecord> trainingData)
    {
        Data = trainingData;
        Root = new Node();
        CreateTree(Root,Constants.Keys,Data);
    }

    public string Classify(SingleRecord singleRecord)
    {
        return Classify(Root, singleRecord);
    }
    private string Classify(Node node,SingleRecord singleRecord)
    {
        if (!string.IsNullOrEmpty(node.Decision))
        {
            return node.Decision;
        }

        var child = node.Children.First(c => c.Value == singleRecord.AttributeDictionary[node.Attribute]);
        return Classify(child, singleRecord);
    }
    public void CreateTree(Node node, IList<string> attributes, IList<SingleRecord> data)
    {
        if (!data.Any())
        {
            node.Decision = "?";
            return;
        }

        if (data.All(d => d.ClassName == Constants.Positive))
        {
            node.Decision = Constants.Positive;
            return;
        }
        if(data.All(d => d.ClassName == Constants.Negative))
        {
            node.Decision = Constants.Negative;
            return;
        }
        if (data.Count <= Constants.OverfittingThreshold || !attributes.Any())
        {
            node.Decision = data.Count(r => r.ClassName == Constants.Positive) > (data.Count / 2)
                ? Constants.Positive
                : Constants.Negative;
            return;
        }
        var maxGain = double.MinValue;
        foreach (var attribute in attributes)
        {
            var attributeGain = CalculateGain(attribute, data);
            if (attributeGain > maxGain)
            {
                maxGain = attributeGain;
                node.Attribute = attribute;
            }
        }
        
        var newAttributes = attributes.Where(attr => attr != node.Attribute).ToList();
        foreach (var value in Constants.Attributes[node.Attribute])
        {
            var child = new Node() { Value = value };
            node.Children.Add(child);
            CreateTree(child, newAttributes,
                data.Where(r => r.AttributeDictionary[node.Attribute] == value).ToList());
        }
        
    }
    private double CalculateEntropy(int positive, int negative)
    {
        if (positive == 0 && negative == 0) return 1;
        if (positive == 0 || negative == 0) return 0;

        var positivePart = (positive * 1.0) / (positive + negative);
        var negativePart = (negative * 1.0) / (positive + negative);

        return -(positivePart * Math.Log(positivePart, 2)) - (negativePart * Math.Log(negativePart, 2));
    }

    private double CalculateEntropy(string attribute, IList<SingleRecord> data)
    {
        var result = 0.0;
        foreach (var attributeValue in Constants.Attributes[attribute])
        {
            var filteredData = data.Where(record => record.AttributeDictionary[attribute] == attributeValue).ToList();
            var positive = filteredData.Count(record => record.ClassName == Constants.Positive);
            var negative = filteredData.Count(record => record.ClassName == Constants.Negative);
            var attributeValueProbability = filteredData.Count / (data.Count * 1.0);
            result += attributeValueProbability * CalculateEntropy(positive, negative);
        }

        return result;
    }
    private double CalculateGain(string attribute, IList<SingleRecord> data)
    {
        var classEntropy = CalculateEntropy(data.Count(r => r.ClassName == Constants.Positive),
            data.Count(r => r.ClassName == Constants.Negative));
        return classEntropy - CalculateEntropy(attribute, data);
    }
   
}