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
        var list = Enumerable.Range(0, n).ToList();
        var rnd = new Random();
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
                }
            }

            queens[col] = list[row];
            list.RemoveAt(row);
            int mainIndex = (col - row) + n - 1;
            int secondIndex = row + col;
            queensPerMD[mainIndex]++;
            queensPerSD[secondIndex]++;
            queensPerRow[row]++;
        }
    }

    public void RandomInit()
    {
        var list = Enumerable.Range(0, n).ToList();
        var rnd = new Random();
        int copyOfn = n;
        while (copyOfn > 1)
        {
            copyOfn--;
            var k = rnd.Next(copyOfn + 1);
            (list[k], list[copyOfn]) = (list[copyOfn], list[k]);
        }

        queens = list.ToArray();
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