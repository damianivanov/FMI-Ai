using System.Xml;

namespace ID3;

public class Constants
{
    public static string Negative = "no-recurrence-events";
    public static string Positive = "recurrence-events";
    public static int OverfittingThreshold = 20;

    public static int SectionsCount = 10;
    public static  Dictionary<string, List<string>> Attributes = new Dictionary<string, List<string>>
    {
        {"age", new List<string> {"10-19", "20-29", "30-39", "40-49", "50-59", "60-69", "70-79", "80-89", "90-99", "?"}},
        {"menopause", new List<string> {"lt40", "ge40", "premeno", "?"}},
        {"tumor-size", new List<string> {"0-4", "5-9", "10-14", "15-19", "20-24", "25-29", "30-34", "35-39", "40-44", "45-49", "50-54", "55-59", "?"}},
        {"inv-nodes", new List<string> {"0-2", "3-5", "6-8", "9-11", "12-14", "15-17", "18-20", "21-23", "24-26", "27-29", "30-32", "33-35", "36-39", "?"}},
        {"node-caps", new List<string> {"yes", "no", "?"}},
        {"deg-malig", new List<string> {"1", "2", "3", "?"}},
        {"breast", new List<string> {"left", "right", "?"}},
        {"breast-quad", new List<string> {"left_up", "left_low", "right_up", "right_low", "central", "?"}},
        {"irradiat", new List<string> {"yes", "no", "?"}},
    };

    public static List<string> Keys = Attributes.Keys.ToList();


}