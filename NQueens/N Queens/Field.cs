namespace N_Queens;

class Field
{
    private int[] queens;
    private int[] queensPerRow;
    private int[] queensPerMD;
    private int[] queensPerSD;
    private int n;
    private int diagonals;

    public Field(int n)
    {
        this.n = n;
        diagonals = ((2 * n) - 1); 
        queens = new int[n];
        
        queensPerRow = new int[n];
        queensPerMD = new int[diagonals];
        queensPerSD = new int[diagonals];
        // InitMinConflict();
        RandomInit();
    }

 
    public void InitMinConflict()
    {
        CleanArrays();
        var list = RandomArray().ToList();
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
            
            var newRow = list[row];
            list.RemoveAt(row);
            UpdateQueenPos(col,newRow);
        }
    }
    private int[] RandomArray()
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
        return list;
    }
    public void RandomInit()
    {
        queens = RandomArray();
        CalculateDiagonals();
    }
    private void CleanArrays()
    {
        Array.Clear(queensPerRow);
        Array.Clear(queensPerMD);
        Array.Clear(queensPerSD);
    }
    private void CalculateDiagonals()
    {
        CleanArrays();
        for (int i = 0; i < n; i++)
        {
            var row = queens[i];
            int mainIndex = (i - row) + n - 1;
            int secondIndex = row + i;
            queensPerMD[mainIndex]++;
            queensPerSD[secondIndex]++;
            queensPerRow[row]++;
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
        List<int> indexes = new List<int>();
        int maxConflict = 0;
        int indexMaxConflict = -1;
        indexes.Add(-1);
        for (int i = from ; i < n; i++)
        {
            int currConflicts = Conflicts(i);
            if(currConflicts == maxConflict) indexes.Add(i);
            if (maxConflict < currConflicts)
            {
                maxConflict = currConflicts;
                indexes.Clear();
                indexes.Add(i);
            }
        }

        var randomIndex = Random.Shared.Next(0, indexes.Count);
        var index = indexes[randomIndex];
        indexes.Clear();
        return index ;
    }
    public void GetRowWithMinConflict(int col)
    {
        int minConflicts = Int32.MaxValue;
        int oldRowIndex = queens[col];
        var indexes = new List<int>();
        RemoveQueenAtPos(col,oldRowIndex);
        for (int i = 0; i < n; i++)
        {
            if (i == oldRowIndex) continue;
            var conflicts = Conflicts(col, i);
            if (conflicts == minConflicts) indexes.Add(i);
            if (minConflicts > conflicts)
            {
                minConflicts = conflicts;
                indexes.Clear();
                indexes.Add(i);
            }
        }
        int rowIndex = indexes[Random.Shared.Next(0,indexes.Count)];
        queens[col] = rowIndex;
        indexes.Clear();
        UpdateQueenPos(col, rowIndex);
    }
    
    /// <summary>
    /// Number of Conflicts before putting a Queen on the tile
    /// </summary>
    /// <param name="col"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    private int Conflicts(int col, int row)
    {
        int mainIndex = (col - row) + n - 1;
        int secondIndex = row + col;
        return queensPerRow[row] + queensPerMD[mainIndex] + queensPerSD[secondIndex];
    }

    /// <summary>
    /// Number of Conflicts with Queen on the tile
    /// </summary>
    /// <param name="col"></param>
    /// <returns></returns>
    public int Conflicts(int col)
    {
        int row = queens[col];

        int mainIndex = (col - row) + n - 1;
        int secondIndex = row + col;
        return queensPerRow[row] - 1 + queensPerMD[mainIndex] - 1 + queensPerSD[secondIndex] - 1;
    }
    
    public bool SwapReducesConflicts(int sumConflicts, int col1, int col2)
    {
        
        int row1 = queens[col1];
        int row2 = queens[col2];

        RemoveQueenAtPos(col1,row1);
        RemoveQueenAtPos(col2,row2);

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

    private void RemoveQueenAtPos(int col,int row)
    {
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
        if (n < 80)
        {
            for (int k = 0; k < n; k++)
            {
                var index = Array.FindIndex(queens, ind => ind == k);
                for (int l = 0; l < n; l++)
                {
                    Console.Write(index == l ? "* " : "- ");
                }

                Console.WriteLine();
            }
        }
    }
}