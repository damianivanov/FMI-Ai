using System.Reflection.PortableExecutable;
using System.Security;

class Solver
{
    private int capacityOfBag;
    private int numberOfItems;
    private Dictionary<int, int> items; //0-weight, 1-value
    private Tuple<int, char[]> elite;
    private List<char[]> population = new List<char[]>();
    private List<int> fitnesses;
    private List<char[]> children;
    private List<char[]> parents;
    private List<int> previousElites = new List<int>();
    private const double mutationRate = 0.01;

    public Solver(int capacityOfBag, int numberOfItems, Dictionary<int, int> items)
    {
        this.items = items;
        this.capacityOfBag = capacityOfBag;
        this.numberOfItems = numberOfItems;
    }

    private List<char[]> GeneratePopulation(int sizeOfPopulation)
    {
        var generatePopulation = new List<char[]>();
        for (int i = 0; i < sizeOfPopulation; i++)
        {
            var child = new char[numberOfItems];
            for (int j = 0; j < numberOfItems; j++)
            {
                child[j] = (char)(Random.Shared.Next(2) + '0');
            }

            generatePopulation.Add(child);
        }

        return generatePopulation;
    }

    public void Solve()
    {
        const int generationSize = 500;
        const int generationsLimit = 10000;
        population.AddRange(GeneratePopulation(generationSize));
        for (int j = 0; j < generationsLimit; j++)
        {
            Selection();
            children = new List<char[]>();
            for (int i = 0; i < parents.Count; i += 2)
            {
                Crossover(parents[i], parents[i + 1]);
            }

            Mutation();
            var oldPopulationSize = population.Count - children.Count - 1;
            if (j % 10 == 0) {oldPopulationSize -= 10;}
            population = population.Except(parents).Take(oldPopulationSize).ToList();
            population.Add(elite.Item2);
            population = population.Concat(children).ToList();
            if(j%10 ==0){population.AddRange(GeneratePopulation(generationSize-population.Count));}
            if (j % 10 == 0)
                Console.WriteLine($"Best Solution After {j} Generations - {GetWeight(elite.Item2)} {elite.Item1}");
        }
        Console.WriteLine($"Best Solution After {generationsLimit} Generations - {GetWeight(elite.Item2)} {elite.Item1}");
    }

    private int Fitness(char[] child)
    {
        int weight = 0;
        int fitness = 0;
        int i = 0;

        foreach (var key in items.Keys)
        {
            if (child[i] == '1')
            {
                weight += key;
                fitness += items[key];
            }

            i++;
        }

        return weight > capacityOfBag ? 0 : fitness;
    }

    private void Selection()
    {
        //----Calculate fitness for every chromosome 
        this.fitnesses = new List<int>();
        double sum = 0;
        var eliteFitness = -1;
        char[] eliteChromosome = null;
        foreach (var child in population)
        {
            int currFitness = Fitness(child);
            if (currFitness > eliteFitness)
            {
                eliteFitness = currFitness;
                eliteChromosome = child;
            }

            fitnesses.Add(currFitness);
            sum += currFitness;
        }

        elite = new Tuple<int, char[]>(eliteFitness, eliteChromosome!);
        previousElites.Add(eliteFitness);
        // --- Probability to get picked 
        double[] probArr = new double[fitnesses.Count];
        for (int i = 0; i < fitnesses.Count; i++)
        {
            probArr[i] = fitnesses[i] / sum;
        }

        double sumProb = probArr.Sum();
        List<char[]> selected = new List<char[]>();
        double crossoverSize = Math.Round((double)(population.Count / 4), MidpointRounding.AwayFromZero) * 2;

        for (int i = 0; i < crossoverSize; i++)
        {
            selected.Add(BiasedRouletteWheel(probArr, sumProb));
        }

        // var sizeFromOldPopulation = (int)(population.Count / 2) - 1;
        // population = Shuffle(population).Take(sizeFromOldPopulation).ToList();
        parents = selected;
    }

    private char[] BiasedRouletteWheel(double[] probArr, double sumProb)
    {
        double sum = 0;
        for (int i = 0; i < population.Count; i++)
        {
            // double percentage = (probArr[i]) * 100;
            double randomNumber = Random.Shared.NextDouble() * sumProb;
            if (randomNumber <= (sum += probArr[i]))
            {
                return population[i];
            }
        }

        return population[0];
    }

    private List<char[]> Shuffle(List<char[]> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Shared.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }

        return list;
    }

    private void Crossover(char[] p1, char[] p2)
    {
        int firstPoint = Random.Shared.Next(1, p1.Length);
        int secondPoint = 0;
        do
        {
            secondPoint = Random.Shared.Next(1, p1.Length);
        } while (firstPoint == secondPoint);

        if (firstPoint > secondPoint)
        {
            (firstPoint, secondPoint) = (secondPoint, firstPoint);
        }

        char[] p1FirstPart = p1[0..firstPoint];
        char[] p2FirstPart = p2[0..firstPoint];

        char[] p1Middle = p1[firstPoint..secondPoint];
        char[] p2Middle = p2[firstPoint..secondPoint];

        char[] p1SecondPart = p1[secondPoint..p1.Length];
        char[] p2SecondPart = p2[secondPoint..p2.Length];

        p1 = p2FirstPart.Concat(p1Middle).Concat(p2SecondPart).ToArray();
        p2 = p1FirstPart.Concat(p2Middle).Concat(p1SecondPart).ToArray();

        children.Add(p1);
        children.Add(p2);
    }

    private void Mutation()
    {
        for (int i = 0; i < children.Count; i++)
        {
            for (int j = 0; j < children[i].Length; j++)
            {
                var child = children[j];
                if (!SuccessfulMutation()) continue;
                if (child[j] == '0') child[j] = '1';
                else if (child[j] == '1') child[j] = '0';
            }
        }
    }

    private int GetWeight(char[] chromosomes)
    {
        int value = 0;
        for (int i = 0; i < chromosomes.Length; i++)
        {
            if (chromosomes[i] == '1')
            {
                value += items.ElementAt(i).Key;
            }
        }

        return value;
    }

    private bool SuccessfulMutation()
    {
        int rnd = Random.Shared.Next(1, 100);
        return rnd <= mutationRate * 100;
    }
}