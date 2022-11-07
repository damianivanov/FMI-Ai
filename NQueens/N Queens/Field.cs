class Field
{
    public int[] queens;
    private int[] queensPerRow;
    private int[] queensPerMD;
    private int[] queensPerSD;
    public int n;
    private int diagonals;

    public Field(int n)
    {
        this.n = n;
        this.diagonals = ((2 * n) - 1);

        //InitMinConflict();
         RandomInit();
    }

 
    public void InitMinConflict()
    {
        this.queens = new int[n];
        this.queensPerRow = new int[n];
        this.queensPerMD = new int[diagonals];
        this.queensPerSD = new int[diagonals];

        var list = randomArray().ToList();
        for (int col = 0; col < n; col++)
        {
            var minConflicts = n;
            int row = -1;
            for (int j = 0; j < list.Count; j++)
            {
                int currConflicts = Conflicts(col, list[j]);
                if (currConflicts < minConflicts)
                {
                    minConflicts = currConflicts;
                    row = j;
                    if (minConflicts == 0) break;
                }
            }
            var item = list[row];
            queens[col] = item;
            list.RemoveAt(row);
            int mainIndex = (col - item) + n - 1;
            int secondIndex = item + col;
            queensPerMD[mainIndex]++;
            queensPerSD[secondIndex]++;
            queensPerRow[item]++;
        }
    }
    private int[] randomArray()
    {
        var list = Enumerable.Range(0, n).ToArray();
        var rnd = new Random();
        int copyOfn = n;
        while (copyOfn > 1)
        {
            copyOfn--;
            var k = rnd.Next(copyOfn + 1);
            (list[k], list[copyOfn]) = (list[copyOfn], list[k]);
        }
        return list; ;
    }
    public void RandomInit()
    {
        queens = randomArray();
        CalculateDiagonals();
    }

    public void CalculateDiagonals()
    {
        this.queensPerRow = Enumerable.Repeat(1, n).ToArray();
        this.queensPerMD = new int[diagonals];
        this.queensPerSD = new int[diagonals];

        for (int i = 0; i < n; i++)
        {
            var row = queens[i];
            int mainIndex = (i - row) + n - 1;
            int secondIndex = row + i;
            queensPerMD[mainIndex]++;
            queensPerSD[secondIndex]++;
        }
    }

    public bool IsSolved()
    {
        return queensPerRow.All(i => i < 2) &&
               queensPerMD.All(i => i < 2) &&
               queensPerSD.All(i => i < 2);
    }

    public int GetColWithQueenWithMaxConf(int from = 0)
    {
        int maxConflict = 0;
        int indexMaxConflict = -1;
        for (int i = from; i < n; i++)
        {
            int currConflicts = Conflicts(i);
            if (maxConflict < currConflicts)
            {
                maxConflict = currConflicts;
                indexMaxConflict = i;
            }
        }

        return indexMaxConflict;
    }

    public void GetRowWithMinConflict(int col)
    {
        int minConflicts = n;
        int rowIndex = n - 1;
        int oldRowIndex = queens[col];
        RemoveQueenAtPos(col);
        for (int i = 0; i < n; i++)
        {
            if (i == oldRowIndex) continue;
            var conflicts = Conflicts(col, i);
            if (minConflicts >= conflicts)
            {
                minConflicts = conflicts;
                rowIndex = i;
            }
        }

        queens[col] = rowIndex;
        UpdateQueenPos(col, rowIndex);
    }

    public int Conflicts(int col, int row)
    {
        int mainIndex = (col - row) + n - 1;
        int secondIndex = row + col;
        return queensPerRow[row] + queensPerMD[mainIndex] + queensPerSD[secondIndex];
    }

    public int Conflicts(int col)
    {
        int row = queens[col];

        int mainIndex = (col - row) + n - 1;
        int secondIndex = row + col;

        // int rows = queensPerRow[row] > 0 ? queensPerRow[row] - 1 : 0;
        // int d1 = queensPerMD[mainIndex] > 0 ? queensPerMD[mainIndex] - 1 : 0;
        // int d2 = queensPerSD[secondIndex] > 0 ? queensPerSD[secondIndex] - 1 : 0;

        return queensPerRow[row] - 1 + queensPerMD[mainIndex] - 1 + queensPerSD[secondIndex] - 1;
        // return rows + d1 + d2;
    }


    public bool SwapReducesConflicts(int sumConflicts, int col1, int col2)
    {
        int row1 = queens[col1];
        int row2 = queens[col2];

        RemoveQueenAtPos(col1);
        RemoveQueenAtPos(col2);

        int sum = Conflicts(col1, row2) + Conflicts(col2, row1);
        if (sum < sumConflicts)
        {
            UpdateQueenPos(col1, row2);
            UpdateQueenPos(col2, row1);
            return true;
        }

        UpdateQueenPos(col1, row1);
        UpdateQueenPos(col2, row2);
        return false;
    }

    private void RemoveQueenAtPos(int col)
    {
        int row = queens[col];
        int mainIndex = (col - row) + n - 1;
        int secondIndex = row + col;

        queensPerRow[row]--;
        queensPerMD[mainIndex]--;
        queensPerSD[secondIndex]--;
    }

    private void UpdateQueenPos(int col, int newRow)
    {
        queens[col] = newRow;
        int mainIndex = (col - newRow) + n - 1;
        int secondIndex = newRow + col;

        queensPerRow[newRow]++;
        queensPerMD[mainIndex]++;
        queensPerSD[secondIndex]++;
    }

    public void Print()
    {
        if (n < 10)
        {
            for (int k = 0; k < n; k++)
            {
                var index = Array.FindIndex(queens, ind => ind == k);
                for (int l = 0; l < n; l++)
                {
                    Console.Write(index == l ? 'Q' : '-');
                }

                Console.WriteLine();
            }
        }
    }
}


//class Field
//{
//    public int[] queens;
//    private int[] queensPerRow;
//    private int[] queensPerMD;
//    private int[] queensPerSD;
//    public int n;
//    private int diagonals;

//    public Field(int n)
//    {
//        this.n = n;
//        this.diagonals = ((2 * n) - 1);

//        //InitMinConflict();
//        RandomInit();
//    }

//    public void InitMinConflict()
//    {
//        this.queens = new int[n];
//        this.queensPerRow = new int[n];
//        this.queensPerMD = new int[diagonals];
//        this.queensPerSD = new int[diagonals];

//        var list = Enumerable.Range(0, n).OrderBy(x => Random.Shared.Next()).ToList();
//        for (int col = 0; col < n; col++)
//        {
//            var minConflicts = n;
//            int row = -1;
//            for (int j = 0; j < list.Count; j++)
//            {
//                int currConflicts = ConflictsBeforePlacing(col, list[j]);
//                if (currConflicts < minConflicts)
//                {
//                    minConflicts = currConflicts;
//                    row = j;
//                    if (minConflicts == 0) break;
//                }
//            }
//            var item = list[row];
//            queens[col] = item;
//            list.RemoveAt(row);
//            int mainIndex = (col - item) + n - 1;
//            int secondIndex = item + col;
//            queensPerMD[mainIndex]++;
//            queensPerSD[secondIndex]++;
//            queensPerRow[item]++;
//        }
//    }
//    public void RandomInit()
//    {
//        var list = Enumerable.Range(0, n).ToArray();
//        var rnd = new Random();
//        int copyOfn = n;
//        while (copyOfn > 1)
//        {
//            copyOfn--;
//            var k = rnd.Next(copyOfn + 1);
//            (list[k], list[copyOfn]) = (list[copyOfn], list[k]);
//        }

//        queens = list;
//        CalculateDiagonals();
//    }
//    public void CalculateDiagonals()
//    {
//        this.queensPerRow = new int[n];
//        this.queensPerMD = new int[diagonals];
//        this.queensPerSD = new int[diagonals];

//        for (int i = 0; i < n; i++)
//        {
//            var row = queens[i];
//            int mainIndex = (i - row) + n - 1;
//            int secondIndex = row + i;

//            queensPerRow[row]++;
//            queensPerMD[mainIndex]++;
//            queensPerSD[secondIndex]++;
//        }
//    }
//    public bool IsSolved()
//    {
//        return queensPerRow.All(i => i < 2) &&
//               queensPerMD.All(i => i < 2) &&
//               queensPerSD.All(i => i < 2);
//    }

//    public int GetColWithQueenWithMaxConf(int from = 0)
//    {
//        int maxConflict = 0;
//        int indexMaxConflict = -1;
//        var list = new List<int>() {-1};
//        for (int i = from; i < n; i++)
//        {
//            int currConflicts = ConflictsWithoutMoving(i);
//            if (maxConflict < currConflicts)
//            {
//                //if (maxConflict < currConflicts)
//                //{
//                //    maxConflict = currConflicts;
//                //    list.Clear();
//                //}
//                maxConflict = currConflicts;
//                indexMaxConflict = i;
//                //list.Add(i);

//            }
//        }
//        return indexMaxConflict;
//    }

//    public void GetRowWithMinConflict(int col)
//    {
//        int minConflicts = n;
//        int rowIndex = n - 1;
//        int oldRowIndex = queens[col];
//        RemoveQueenAtPos(col);
//        for (int i = 0; i < n; i++)
//        {
//            if (i == oldRowIndex) continue;
//            var conflicts = ConflictsBeforePlacing(col, i);
//            if (minConflicts > conflicts)
//            {
//                minConflicts = conflicts;
//                rowIndex = i;

//                if (minConflicts == 0) 
//                    break;
//            }
//        }
//        queens[col] = rowIndex;
//        UpdateQueenPos(col, rowIndex);
//    }

//    private int ConflictsBeforePlacing(int col, int row)
//    {
//        int mainIndex = (col - row) + n - 1;
//        int secondIndex = row + col;
//        return queensPerRow[row] + queensPerMD[mainIndex] + queensPerSD[secondIndex];
//    }
//    public int ConflictsWithoutMoving(int col)
//    {
//        int row = queens[col];
//        int mainIndex = (col - row) + n - 1;
//        int secondIndex = row + col;

//        //int mainIndex = MDPos(col,row);
//        //int secondIndex = SDPos(col,row);

//        //int rows = queensPerRow[row] > 0 ? queensPerRow[row] - 1 : 0;
//        //int d1 = queensPerMD[mainIndex] > 0 ? queensPerMD[mainIndex] - 1 : 0;
//        //int d2 = queensPerSD[secondIndex] > 0 ? queensPerSD[secondIndex] - 1 : 0;

//        return queensPerRow[row] - 1 + queensPerMD[mainIndex] - 1 + queensPerSD[secondIndex] - 1;
//        //return rows + d1 + d2;
//    }


//    public bool SwapReducesConflicts(int sumConflicts, int col1, int col2)
//    {
//        int row1 = queens[col1];
//        int row2 = queens[col2];

//        RemoveQueenAtPos(col1);
//        RemoveQueenAtPos(col2);

//        int sum = ConflictsBeforePlacing(col1, row2) + ConflictsBeforePlacing(col2, row1);
//        if (sum < sumConflicts)
//        {
//            UpdateQueenPos(col1, row2);
//            UpdateQueenPos(col2, row1);
//            return true;
//        }

//        UpdateQueenPos(col1, row1);
//        UpdateQueenPos(col2, row2);
//        return false;
//    }

//    private void RemoveQueenAtPos(int col)
//    {
//        int row = queens[col];
//        int mainIndex = (col - row) + n - 1;
//        int secondIndex = row + col;
//        queensPerRow[row]--;
//        queensPerMD[mainIndex]--;
//        queensPerSD[secondIndex]--;
//    }

//    private void UpdateQueenPos(int col, int newRow)
//    {
//        queens[col] = newRow;
//        int mainIndex = (col - newRow) + n - 1;
//        int secondIndex = newRow + col;

//        queensPerRow[newRow]++;
//        queensPerMD[mainIndex]++;
//        queensPerSD[secondIndex]++;
//    }

//    public void Print()
//    {
//        if (n < 10)
//        {
//            for (int k = 0; k < n; k++)
//            {
//                var index = Array.FindIndex(queens, ind => ind == k);
//                for (int l = 0; l < n; l++)
//                {
//                    Console.Write(index == l ? 'Q' : '-');
//                }

//                Console.WriteLine();
//            }
//        }
//    }
//}