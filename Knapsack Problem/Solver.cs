namespace Knapsack_Problem;

class Solver
{
    private readonly int capacityOfBag;
    private readonly int numberOfItems;
    private List<KeyValuePair<int, int>> items; //0-weight, 1-value
    private Chromosome elite;
    private List<Chromosome> population = new List<Chromosome>();
    private List<int> fitnesses;
    private List<Chromosome> children;
    private List<Chromosome> parents;

    private const double mutationRate = 0.01;
    private const double generationsWithoutChangePerc = 0.5;
    private const int generationSize = 100;
    private const int generationsLimit = 5000;

    private int crossoverSize;
    private int generationsWithoutChangeInElite;

    public Solver(int capacityOfBag, int numberOfItems, List<KeyValuePair<int, int>> items)
    {
        this.elite = new Chromosome(null, -1, -1);
        this.items = items;
        this.capacityOfBag = capacityOfBag;
        this.numberOfItems = numberOfItems;
        this.crossoverSize = (int)(Math.Round((double)(generationSize / 4), MidpointRounding.AwayFromZero) * 2);
    }

    public void Solve()
    {
        population = GeneratePopulation(generationSize);
        for (int j = 1; j <= generationsLimit; j++)
        {
            Selection();
            Crossover();
            Mutation();
            EvaluateChildren();

            //Checking for local extremum
            if (generationsWithoutChangeInElite >= generationsLimit * generationsWithoutChangePerc &&
                elite.fitness != 0)
            {
                PrintElite(j);
                break;
            }

            population.AddRange(children);
            population = population
                .OrderByDescending(x => x.fitness)
                .ThenBy(x => x.weight)
                .DistinctBy(x => x.chromosomeString)
                .Take(generationSize)
                .ToList();
            if (j is 10 or 100 or 300 or 1500 or generationsLimit) PrintElite(j);
        }
    }

    private void EvaluateChildren()
    {
        foreach (var child in children)
        {
            var result = Fitness(child.chromosome);
            child.fitness = result.fitness;
            child.weight = result.weight;
            child.UpdateString();
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
        char[] firstChild = new char[p1.chromosome.Length];
        char[] secondChild = new char[p2.chromosome.Length];
        for (int i = 0; i < p1.chromosome.Length; i++)
        {
            int num = Random.Shared.Next(1, 3);
            if (num == 1)
            {
                firstChild[i] = p1.chromosome[i];
                secondChild[i] = p2.chromosome[i];
            }
            else
            {
                firstChild[i] = p2.chromosome[i];
                secondChild[i] = p1.chromosome[i];
            }
        }

        var r1 = Fitness(firstChild);
        var r2 = Fitness(secondChild);

        var c1 = new Chromosome(firstChild, r1.fitness, r1.weight);
        var c2 = new Chromosome(secondChild, r2.fitness, r2.weight);

        children.Add(c1);
        children.Add(c2);
    }

    private void Mutation()
    {
        foreach (var child in children)
        {
            MutationPerChromosome(child);
        }
    }

    private void MutationPerChromosome(Chromosome child)
    {
        var rand = Random.Shared.Next(1, 100);
        if (rand > 5 * mutationRate * 100) return;
        var randIndex = Random.Shared.Next(0, numberOfItems);
        child.chromosome[randIndex] = child.chromosome[randIndex] == '0' ? '1' : '0';
    }

    private void PrintElite(int generation)
    {
        Console.WriteLine(
            $"Best Solution After {generation} Generations - Weight - {elite.weight} Value - {elite.fitness}");
    }
}