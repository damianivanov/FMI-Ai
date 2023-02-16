using Microsoft.VisualBasic;
using SkiaSharp;

class KMeans
{
    public List<List<double>> Data { get; set; }
    public int ClustersCount { get; set; }
    private int MaxIterations { get; set; }
    private List<List<int>> ClusterMeans { get; set; }
    private int FeaturesCount { get; set; }
    private Random Random { get; set; }

    private Dictionary<int, SKColor> Colors { get; set; }

    public KMeans(List<List<double>> data, int clustersCount)
    {
        Data = data;
        ClustersCount = clustersCount;
        MaxIterations = 1000000;
        FeaturesCount = data[0].Count;
        Colors = new Dictionary<int, SKColor>();
        Random = new Random();
    }

    public void Execute()
    {
        var means = CalculateMeans();
        var belongsTo = FindClusters(means);

        //IList<IList<double>> means = new List<IList<double>>();
        //IList<int> belongsTo = new List<int>();

        //var bestDensity = double.MaxValue;

        //var index = 0;
        //do
        //{
        //    var currentMeans = CalculateMeans();
        //    var currentBelongsTo = FindClusters(currentMeans);

        //    var maxClusterDensity = CalculateMaxClusterVariance(currentMeans, currentBelongsTo);
        //    if (bestDensity > maxClusterDensity)
        //    {
        //        means = currentMeans;
        //        belongsTo = currentBelongsTo;
        //        bestDensity = maxClusterDensity;
        //    }

        //    index++;
        //} while (index < 100);

        GenerateImage(belongsTo, means);
    }

    private double CalculateMaxClusterVariance(IList<IList<double>> means, IList<int> belongsTo)
    {
        var maxDistance = double.MinValue;
        for (var i = 0; i < ClustersCount; i++)
        {
            maxDistance = Data
                .Where((t, j) => belongsTo[j] == i)
                .Select(t => Distance(means[i], t))
                .Concat(new[] { maxDistance })
                .Max();
        }

        return maxDistance;
    }

    private void GenerateImage(IList<int> belongsTo, IList<IList<double>> means)
    {
        const int width = 600;
        const int height = 600;

        var xMin = Data.Min(i => i[0]);
        var xDiff = Data.Max(i => i[0]) - xMin;
        var yMin = Data.Min(i => i[1]);
        var yDiff = Data.Max(i => i[1]) - yMin;

        const int margin = 10;
        SKBitmap bmp = new(width, width);
        using SKCanvas canvas = new(bmp);
        canvas.Clear(SKColors.White);
        
        for (var i = 0; i < Data.Count; i++)
        {
            var record = Data[i];
            var point = TransformPoint(xMin, yMin, xDiff, yDiff, margin, margin, width - margin,
                height - margin, record[0], record[1]);

            var brush = new SKPaint { Color = GetColor(belongsTo[i]) };
            canvas.DrawOval(point.X, point.Y, 10, 10, brush);
            canvas.Save();
        }

        for (var i = 0; i < ClustersCount; i++)
        {
            var record = means[i];
            var point = TransformPoint(xMin, yMin, xDiff, yDiff, margin, margin, width - margin,
                height - margin, record[0], record[1]);

            canvas.DrawLine(point.X - 6, point.Y, point.X + 6, point.Y,
                new SKPaint { Color = SKColors.Red, StrokeWidth = 2 });
            canvas.DrawLine(point.X, point.Y - 6, point.X, point.Y + 6,
                new SKPaint { Color = SKColors.Red, StrokeWidth = 2 });
            canvas.Save();
        }


        using SKWStream fs = new SKFileWStream("demo.png");
        bmp.Encode(fs, SKEncodedImageFormat.Png, quality: 100);
    }
        

private SKPoint TransformPoint(double sourceX, double sourceY, double sourceW, double sourceH, double destinationX,
        double destinationY, double destinationW, double destinationH, double x, double y)
    {
        var pointY = (int)(((y - sourceY) / sourceH) * destinationH + destinationY);
        var pointX = (int)(((x - sourceX) / sourceW) * destinationW + destinationX);
        return new SKPoint(pointX, pointY);
    }


    private SKColor GetColor(int index)
    {
        if (Colors.ContainsKey(index))
            return Colors[index];

        var newColor = SKColor.FromHsl(Random.Next(0, 255), Random.Next(0, 255), Random.Next(0, 255));
        Colors.Add(index, newColor);

        return newColor;
    }
    private int[] FindClusters(IList<IList<double>> means)
    {
        var belongsTo = new int[Data.Count];

        for (int i = 0; i < Data.Count; i++)
        {
            var clusterIndex = Classify(means, Data[i]);

            belongsTo[i] = clusterIndex;
        }

        return belongsTo;
    }

    private IList<IList<double>> CalculateMeans()
    {
        var ranges = FindRanges();

        var mins = ranges.Item1;
        var maxes = ranges.Item2;

        var means = InitMeans(mins, maxes);
        var clusterSizes = Enumerable.Repeat(0, ClustersCount).ToArray();
        var belongsTo = Enumerable.Repeat(-1, Data.Count).ToArray();

        for (var i = 0; i < MaxIterations; i++)
        {
            var hasChange = true;
            for (var j = 0; j < Data.Count; j++)
            {
                var record = Data[j];

                var clusterIndex = Classify(means, record);

                if (clusterIndex == belongsTo[j])
                {
                    hasChange = false;
                    continue;
                }

                clusterSizes[clusterIndex]++;

                var oldClusterIndex = belongsTo[j];
                belongsTo[j] = clusterIndex;

                UpdateMean(clusterSizes[clusterIndex], means[clusterIndex], record, true);

                if (oldClusterIndex != -1)
                {
                    clusterSizes[oldClusterIndex]--;
                    UpdateMean(clusterSizes[oldClusterIndex], means[oldClusterIndex], record, false);
                }
            }

            for (var j = 0; j < ClustersCount; j++)
            {
                if (belongsTo.All(x => x != j))
                {
                    means[j] = InitMean(mins, maxes);
                    hasChange = true;
                }
            }

            if (!hasChange) break;
        }

        return means;
    }

    private int Classify(IList<IList<double>> means, IList<double> record)
    {
        var minDistance = double.MaxValue;
        var clusterIndex = -1;

        for (var i = 0; i < means.Count; i++)
        {
            var distance = Distance(record, means[i]);

            if (distance < minDistance)
            {
                minDistance = distance;
                clusterIndex = i;
            }
        }

        return clusterIndex;
    }

    private IList<IList<double>> InitMeans(IList<double> mins, IList<double> maxes)
    {
        var means = new List<IList<double>>();
        for (var j = 0; j < ClustersCount; j++)
        {
            means.Add(InitMean(mins, maxes));
        }

        return means;
    }

    private IList<double> InitMean(IList<double> mins, IList<double> maxes)
    {
        var mean = new List<double>();

        for (var i = 0; i < FeaturesCount; i++)
        {
            var min = (int)(mins[i] + 1);
            var max = (int)(maxes[i] - 1);
            mean.Add(Random.Next((int)(mins[i] + 1), (int)(maxes[i] - 1)));
        }

        return mean;
    }

    private void UpdateMean(int clusterSize, IList<double> mean, IList<double> record, bool isRecordAdded)
    {
        for (var i = 0; i < FeaturesCount; i++)
        {
            var featureMean = mean[i];
            featureMean = isRecordAdded
                ? (featureMean * (clusterSize - 1) + record[i]) / clusterSize
                : (featureMean * (clusterSize + 1) - record[i]) / clusterSize;
            mean[i] = featureMean;
        }
    }

    private Tuple<List<double>, List<double>> FindRanges()
    {
        var min = Enumerable.Repeat(double.MaxValue, FeaturesCount).ToList();
        var max = Enumerable.Repeat(double.MinValue, FeaturesCount).ToList();

        foreach (var record in Data)
        {
            for (var i = 0; i < FeaturesCount; i++)
            {
                if (record[i] < min[i])
                    min[i] = record[i];

                if (record[i] > max[i])
                    max[i] = record[i];
            }
        }

        return new Tuple<List<double>, List<double>>(min, max);
    }

    private double Distance(IList<double> x, IList<double> y)
    {
        var sum = 0.0;
        for (int i = 0; i < FeaturesCount; i++)
        {
            sum += Math.Pow(x[i] - y[i], 2);
        }

        return Math.Sqrt(sum);
    }
}