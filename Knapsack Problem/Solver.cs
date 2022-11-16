using Microsoft.VisualBasic;

class Solver
{
    private int capacityOfBag;
    private int numberOfItems;
    private List<KeyValuePair<int, int>> items; //0-weight, 1-value
    private Chromosome elite;
    private List<Chromosome> population = new List<Chromosome>();
    private List<int> fitnesses;
    private List<Chromosome> children;
    private List<Chromosome> parents;
    private const double mutationRate = 0.01;
    private int generationSize;
    private int crossoverSize;
    private int generationsWithoutChangeInElite = 0;

    public Solver(int capacityOfBag, int numberOfItems, List<KeyValuePair<int, int>> items)
    {
        this.elite = new Chromosome(null, -1, -1);
        this.items = items;
        this.capacityOfBag = capacityOfBag;
        this.numberOfItems = numberOfItems;
        this.generationSize = items.Count < 8 ? (int)Math.Pow(2, items.Count) : 100;
        this.crossoverSize = (int)(Math.Round((double)(generationSize / 4), MidpointRounding.AwayFromZero) * 2);
    }

    public void Solve()
    {
        const int generationsLimit = 10000;
        population = GeneratePopulation(generationSize);
        for (int j = 1; j <= generationsLimit; j++)
        {
            Selection();
            Crossover();
            Mutation();
            EvaluateChildren();
            if (generationsWithoutChangeInElite >= generationsLimit * 0.1 && elite.fitness != 0)
            {
                PrintElite(j);
                break;
            }

            population.AddRange(children);
            population.Add(elite);
            population = population
                .OrderByDescending(x => x.fitness)
                .ThenBy(x => x.weight)
                .DistinctBy(x => string.Join(',', x.chromosome))
                .Take(generationSize)
                .ToList();
            if (j is 10 or 100 or 300 or 500 or generationsLimit)
            {
                PrintElite(j);
            }
        }
    }

    private void EvaluateChildren()
    {
        foreach (var child in children)
        {
            var result = Fitness(child.chromosome);
            child.fitness = result.fitness;
            child.weight = result.weight;
        }
    }

    private List<Chromosome> GeneratePopulation(int sizeOfPopulation)
    {
        var generatePopulation = new List<Chromosome>();
        for (int i = 0; i < sizeOfPopulation; i++)
        {
            var child = GenerateChromosome();
            var currentFitness = Fitness(child);
            var newChromosome = new Chromosome(child, currentFitness.fitness, currentFitness.weight);
            generatePopulation.Add(newChromosome);
        }

        return generatePopulation;
    }

    private char[] GenerateChromosome()
    {
        var child = new char[numberOfItems];
        for (int j = 0; j < numberOfItems; j++)
        {
            child[j] = (char)(Random.Shared.Next(2) + '0');
        }

        return child;
    }

    private (int weight, int fitness) Fitness(char[] child)
    {
        int weight = 0;
        int fitness = 0;
        int i = 0;

        foreach (var item in items)
        {
            if (child[i] == '1')
            {
                weight += item.Key;
                fitness += item.Value;
            }

            i++;
        }

        return (weight, weight > capacityOfBag ? 0 : fitness);
    }

    private void Selection()
    {
        this.fitnesses = new List<int>();
        double sum = 0;
        foreach (var child in population)
        {
            if (child.fitness > elite.fitness || (child.fitness == elite.fitness && elite.weight > child.weight))
            {
                elite = child;
                generationsWithoutChangeInElite = 0;
            }

            fitnesses.Add(child.fitness);
            sum += child.fitness;
        }

        generationsWithoutChangeInElite++;

        // --- Probability to get picked 
        double[] probArr = new double[fitnesses.Count];
        for (int i = 0; i < fitnesses.Count; i++)
        {
            probArr[i] = fitnesses[i] / sum;
        }

        double sumProb = probArr.Sum();
        var selected = new List<Chromosome>();

        for (int i = 0; i < crossoverSize; i++)
        {
            selected.Add(BiasedRouletteWheel(probArr, sumProb));
        }

        parents = selected;
    }

    private Chromosome BiasedRouletteWheel(double[] probArr, double sumProb)
    {
        if (double.IsNaN(sumProb))
        {
            return population[Random.Shared.Next(0, population.Count)];
        }

        double randomNumber = Random.Shared.NextDouble();
        var index = 0;
        while (randomNumber > 0)
        {
            randomNumber -= probArr[index++];
        }

        return population[index - 1];
    }

    private void Crossover()
    {
        children = new List<Chromosome>();
        for (int i = 0; i < parents.Count; i += 2)
        {
            UniformCrossover(parents[i], parents[i + 1]);
        }
    }

    private void UniformCrossover(Chromosome p1, Chromosome p2)
    {
        char[] p1Copy = new char[p1.chromosome.Length];
        char[] p2Copy = new char[p2.chromosome.Length];

        for (int i = 0; i < p1.chromosome.Length; i++)
        {
            int num = Random.Shared.Next(1, 3);
            if (num == 1)
            {
                p1Copy[i] = p1.chromosome[i];
                p2Copy[i] = p2.chromosome[i];
            }
            else
            {
                p1Copy[i] = p2.chromosome[i];
                p2Copy[i] = p1.chromosome[i];
            }
        }

        var f1 = Fitness(p1Copy);
        var f2 = Fitness(p2Copy);

        var c1 = new Chromosome(p1Copy, f1.fitness, f1.weight);
        var c2 = new Chromosome(p2Copy, f2.fitness, f2.weight);

        children.Add(c1);
        children.Add(c2);
    }

    private void TwoPointCrossover(Chromosome p1, Chromosome p2)
    {
        int firstPoint = Random.Shared.Next((int)(p1.chromosome.Length * 0.20), (int)(p1.chromosome.Length * 0.45));
        int secondPoint = Random.Shared.Next((int)(p1.chromosome.Length * 0.65), (int)(p1.chromosome.Length * 0.85));

        char[] p1FirstPart = p1.chromosome[0..firstPoint].ToArray();
        char[] p2FirstPart = p2.chromosome[0..firstPoint].ToArray();

        char[] p1Middle = p1.chromosome[firstPoint..secondPoint].ToArray();
        char[] p2Middle = p2.chromosome[firstPoint..secondPoint].ToArray();

        char[] p1SecondPart = p1.chromosome[secondPoint..p1.chromosome.Length].ToArray();
        char[] p2SecondPart = p2.chromosome[secondPoint..p2.chromosome.Length].ToArray();

        var p1Copy = p2SecondPart.Concat(p1Middle).Concat(p2FirstPart).ToArray();
        var p2Copy = p1SecondPart.Concat(p2Middle).Concat(p1FirstPart).ToArray();

        var f1 = Fitness(p1Copy);
        var f2 = Fitness(p2Copy);

        var c1 = new Chromosome(p1Copy, f1.fitness, f1.weight);
        var c2 = new Chromosome(p2Copy, f2.fitness, f2.weight);

        children.Add(c1);
        children.Add(c2);
    }

    private void Mutation()
    {
        foreach (var child in children)
        {
            MutationPerChromosome(child);
            //MutationPerGene(child);
        }
    }

    private void MutationPerChromosome(Chromosome child)
    {
        var rand = Random.Shared.Next(1, 100);
        if (rand > 5 * mutationRate * 100) return;
        var randIndex = Random.Shared.Next(0, items.Count);
        child.chromosome[randIndex] = child.chromosome[randIndex] == '0' ? '1' : '0';
    }

    private static void MutationPerGene(char[] chromosome, double rate)
    {
        for (int j = 0; j < chromosome.Length; j++)
        {
            if (!SuccessfulMutation(rate)) continue;
            if (chromosome[j] == '0') chromosome[j] = '1';
            else if (chromosome[j] == '1') chromosome[j] = '0';
        }
    }

    private static bool SuccessfulMutation(double rate)
    {
        var rnd = Random.Shared.Next(1, 100);
        return rnd <= rate * 100;
    }

    private void PrintElite(int generation)
    {
        Console.WriteLine(
            $"Best Solution After {generation} Generations - Weight - {elite.weight} Value - {elite.fitness}");
    }

    private void Shuffle(char[] probArr)
    {
        int n = probArr.Length;
        while (n > 1)
        {
            n--;
            int k = Random.Shared.Next(n + 1);
            (probArr[k], probArr[n]) = (probArr[n], probArr[k]);
        }
    }

    private void Shuffle(List<Chromosome> chromosomes)
    {
        int n = chromosomes.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Shared.Next(n + 1);
            (chromosomes[k], chromosomes[n]) = (chromosomes[n], chromosomes[k]);
        }
    }
}