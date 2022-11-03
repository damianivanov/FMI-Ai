using System.Diagnostics;
using System.Text;

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
    var row = Console.ReadLine()!.Trim().Split().Select(int.Parse).ToArray();
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
Console.WriteLine(watch.Elapsed.ToString(@"ss\:fff"));
PrintSolution(board);


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
    List<string> finalPath = board.PathString.ToList();
    finalPath.Reverse();
    Console.WriteLine(finalPath.Count);
    Console.WriteLine(string.Join(Environment.NewLine, finalPath));
}
/*
8
-1
1 2 3
4 5 6
0 7 8
L L - 00:013

8
-1
0 1 3
4 2 5
7 8 6
L U L U - 00:015

8
-1
8 1 3
4 0 2
7 6 5
L U R R D D L U R U L D L U - 0:035

8
0
3 1 0
4 5 2
6 7 8
U R R D - 00:018

8
-1
2 5 8
4 0 6
7 3 1
U L D D R U U R D L L D R R U U L D L U - 00:263

8
-1
6 7 0
3 8 2
4 1 5
24 Solved - 00:375

8
-1
1 5 4
7 0 3
6 2 8
20 - 01:273 // 00:147

8
-1
6 7 1
3 2 4
8 5 0
24 Steps - 01:837 // 00:439 // 00:072

8
0
7 3 6
1 5 4
0 8 2
00:532

8
-1
2 4 5
6 0 7
3 1 8
07:241 //00:678

8
-1
5 8 3
4 0 2
1 7 6

8
5
2 1 4
3 0 8
7 6 5
//00:205

8
-1
8 7 0
6 3 2
5 4 1
08:364 // 00:743

15
-1
2 0 5 4
10 3 6 7
13 1 9 15
8 11 12 14 
36 Moves //03:41

15
-1
2 3 0 8
15 12 6 7
13 1 4 9
14 11 10 5


99
-1
1 2 3 4 5 6 7 8 9 10
11 12 13 14 15 16 17 18 19 20
21 22 23 24 25 26 27 28 29 30
31 32 33 34 35 36 47 37 39 40
41 42 43 44 45 46 0 38 49 50
51 52 53 54 55 56 57 48 59 60
61 62 63 64 65 66 67 58 69 80
71 72 73 74 75 76 77 68 70 78
81 82 83 84 85 86 87 88 79 89
91 92 93 94 95 96 97 98 99 90

15
-1
5 1 3 4
9 2 8 0
13 7 6 11
14 10 15 12
14 Steps- 00:019


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