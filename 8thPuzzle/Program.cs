using System.Diagnostics;
using System.Text;
using EightPuzzle;

//------------ INPUT -----------------------
var tiles = int.Parse(Console.ReadLine()!);
var indexOfZero = int.Parse(Console.ReadLine()!);
var boardSize = Convert.ToInt32(Math.Sqrt(tiles + 1));
var boardInput = new int[boardSize, boardSize];
var goalState = new int[boardSize, boardSize];
//GOAL State
var counter = 1;
for (var i = 0; i < boardSize; i++)
for (var j = 0; j < boardSize; j++)
    goalState[i, j] = (i * boardSize + j == indexOfZero) ? 0 : counter++;

if (indexOfZero == -1) goalState[boardSize - 1, boardSize - 1] = 0;
//

for (int i = 0; i < boardSize; i++)
{
    var row = Console.ReadLine()!.Trim().Split().Select(sbyte.Parse).ToArray();
    for (int j = 0; j < boardSize; j++)
    {
        boardInput[i, j] = row[j];
    }
}
// --------------------------------------------

var board = new Board(boardSize, boardInput, indexOfZero, goalState);
Console.WriteLine("--------Start--------");
var watch = Stopwatch.StartNew();
board.Solve();
watch.Stop();
PrintSolution(board);
Console.WriteLine(watch.Elapsed.ToString(@"mm\:ss\:fff"));


string PrintField(int[,] array)
{
    var size = array.GetLength(0);
    var str = new StringBuilder();
    str.Append(size);
    str.AppendLine();
    for (var i = 0; i < size; i++)
    {
        for (int j = 0; j < size; j++)
        {
            str.Append(array[i, j]);
            str.Append(' ');
        }

        str.AppendLine();
    }

    return str.ToString();
}

void PrintSolution(Board board)
{
    Console.WriteLine(string.Join(Environment.NewLine, board.PathString));
    Console.WriteLine(board.PathString.Count);
}
/*
---HARDEST POSSIBLE---
8
-1
8 6 7
2 5 4 
3 0 1
31 moves// 00:522 //00:112

8
-1
6 4 7
8 5 0
3 2 1
31 moves// 00:106
----------------------
8
-1
0 1 3
4 2 5
7 8 6
L U L U - 00:013

8
-1
8 1 3
4 0 2
7 6 5
L U R R D D L U R U L D L U - 0:015

8
0
3 1 0
4 5 2
6 7 8
U R R D - 00:004

8
-1
2 5 8
4 0 6
7 3 1
U L D D R U U R D L L D R R U U L D L U - 00:013

8
-1
6 7 0
3 8 2
4 1 5
24 Solved - 00:022

8
0
7 3 6
1 5 4
0 8 2
00:024

8
-1
1 5 4
7 0 3
6 2 8
20 - 01:273 // 00:147 // 00:025

8
-1
6 7 1
3 2 4
8 5 0
24 Steps - 01:837 // 00:439 // 00:072 //00:0034

8
-1
2 4 5
6 0 7
3 1 8
07:241//00:678//00:069


8
-1
2 1 4
3 0 8
7 6 5
//00:205

8
-1
8 7 0
6 3 2
5 4 1
08:364 // 00:743(insideTheMatrix method) //00:066(Linq) // 00:53

15
-1
2 0 5 4
10 3 6 7
13 1 9 15
8 11 12 14 
37 Moves //03:29//00:394// 00:301 // 00:122

15
-1
5 1 3 4
9 2 8 0
13 7 6 11
14 10 15 12
14 Steps- 00:013

15
-1
5 6 3 4
8 0 1 15
10 7 2 11
12 9 14 13
40 Moves //02:052 // 01:316(removing Linq) //00:754 (stringBuilder in ToString)

---UNSOLVABLE---
8
-1
1 2 3
4 5 6
8 7 0

8
-1
6 2 5
3 8 0
4 7 1

8
-1
1 2 3
8 4 6
0 5 7

15
-1
1 2 3 4
5 6 7 8
9 10 11 12
13 15 14 0
*/