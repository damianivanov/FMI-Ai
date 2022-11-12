using System.Diagnostics;
using System.Linq;
using N_Queens;

// int n = int.Parse(Console.ReadLine()!);
int n = 1_000_000;
for (int i = 0; i < 10; i++)
{
    var watch = Stopwatch.StartNew();
    SolveSwaps(n);
    watch.Stop();
    Console.WriteLine($"{watch.Elapsed:ss\\:fff} seconds");
}

void SolveSwaps(int n)
{
    while (true)
    {
        var field = new Field(n);
        for (int p = 0; p < 3; p++)
        {
            int swaps = 0;
            for (int i = 0; i < n; i++)
            {
                var queenIConflicts = field.Conflicts(i);
                if (queenIConflicts == 0) continue;
                for (int j = i + 1; j < n; j++)
                {
                    var queenJConflicts = field.Conflicts(j);
                    int sumConflicts = queenIConflicts + queenJConflicts;
                    if (sumConflicts > 0)
                    {
                        if (field.SwapReducesConflicts(sumConflicts, i, j))
                        {
                            swaps++;
                            queenIConflicts = field.Conflicts(i);
                            if (queenIConflicts == 0) break;
                        }
                    }
                }
            }

            if (swaps != 0) continue;
            if (field.IsSolved())
            {
                field.Print();
                return;
            }
        }

        field.RandomInit();
    }
}

void SolveMinConflicts(int n)
{
    const int k = 4;
    var field = new Field(n);
    while (true)
    {
        for (int j = 0; j < k; j++)
        {
            for (int i = 0; i < n; i++)
            {
                var col = field.GetColWithQueenWithMaxConf(i);
                if (col == -1)
                {
                    if (field.IsSolved())
                    {
                        field.Print();
                        return;
                    }

                    break;
                }

                field.GetRowWithMinConflict(col);
            }
        }

        field.RandomInit();
    }
}