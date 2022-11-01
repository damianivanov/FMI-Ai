using System.Diagnostics;

// 0 3 1 2
int n = int.Parse(Console.ReadLine()!);
//var arr = Console.ReadLine()!.Split().Select(int.Parse).ToArray();
var watch = Stopwatch.StartNew();
Solve2(n);
watch.Stop();
Console.WriteLine($"{watch.Elapsed:ss\\:fff} seconds");

// void Solve(int n)
// {
//     const int k = 3;
//     var field = new Field(n);
//     while (true)
//     {
//         // field.InitMinConflict();
//         for (int j = 0; j < k; j++)
//         {
//             for (int i = 0; i < n; i++)
//             {
//                 var col = field.GetColWithQueenWithMaxConf(i);
//                 if (col == -1)
//                 {
//                     if (field.IsSolved())
//                     {
//                         field.Print();
//                         return;
//                     }
//
//                     break;
//                 }
//
//                 field.GetRowWithMinConflict(col);
//             }
//         }
//
//         field.RandomInit();
//     }
// }

void Solve2(int n)
{
    //Algoritamut e po psevdokod ot statiq na
    //Rok Sosic and Jun Gu Department of Computer Science 2 University of Utah 
    
    var field = new Field(n);
    while (true)
    {
        int swaps = 0;
        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                int sumConflicts = field.Conflicts(i) + field.Conflicts(j);
                if (sumConflicts > 0)
                {
                    if (field.SwapReducesConflicts(sumConflicts, i, j))
                    {
                        swaps++;
                    }
                }
            }
        }

        if (swaps == 0)
        {
            if (field.IsSolved())
            {
                field.Print();
                return;
            }
            else
            {
                field.RandomInit();
            }
        }
    }
}