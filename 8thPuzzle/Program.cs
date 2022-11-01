using System.Diagnostics;

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
    var row = Console.ReadLine()!.Split().Select(int.Parse).ToArray();
    for (int j = 0; j < boardSize; j++)
    {
        boardInput[i, j] = row[j];
    }
}
// --------------------------------------------

var board = new Board(boardSize, boardInput, indexOfZero,goalState);
Console.WriteLine("---------START-------");
var watch = Stopwatch.StartNew();
board.Solve();
watch.Stop();
board.PrintSolution();
Console.WriteLine("--------FINISH-------");
Console.WriteLine(watch.Elapsed.ToString(@"ss\:fff"));


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
20 - 01:273

8
-1
6 7 1
3 2 4
8 5 0
24 Steps - 01:837 // 00:439

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
07:678


8
-1
2 1 4
3 0 8
7 6 5



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