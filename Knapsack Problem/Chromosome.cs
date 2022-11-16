using Microsoft.VisualBasic;

class Chromosome
{
    public char[] chromosome { get; set; }
    public int fitness { get; set; }
    public int weight { get; set; }
    public string chromosomeString { get; set; }

    public Chromosome(char[] chromosome, int fitness, int weight)
    {
        this.chromosome = chromosome;
        this.fitness = fitness;
        this.weight = weight;
        this.chromosomeString = chromosome == null ? " " : string.Join(',',chromosome);
    }
}