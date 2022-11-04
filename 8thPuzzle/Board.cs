namespace EightPuzzle;
class Board
{
    
    public int[,] _tiles;

    private Tuple<int, int> _currIndexOfZero;
    public int ManhattanDistance { get; set; }
    public int[,] GoalState { get; private set; }
    public Stack<string> PathString { get; set; } = new Stack<string>();
    public Stack<Board> Path { get; set; } = new Stack<Board>();
    public int Size { get; }
    public int FinalIndexOfZero { get; private set; }

    public Board(int boardSize, int[,] board, int indexOfZero = -1, int[,]? goalState = null,
        Tuple<int, int>? currIndexOfZero = null)
    {
        Size = boardSize;
        FinalIndexOfZero = indexOfZero;
        this._tiles = (board.Clone() as int[,])!;
        this.GoalState = goalState;
        if (currIndexOfZero == null) this._currIndexOfZero = FindIndexOfValue(0, this._tiles);
        else _currIndexOfZero = currIndexOfZero;
        // ManhattanDistance = Manhattan();
    }

    public void Solve()
    {
        if (!IsSolvable()) Console.WriteLine("Not Solvable");
        else
        {
            Manhattan();
            Ida_star();
        }
    }

    private int InversionCount(int[] numbers)
    {
        int inversions = 0;
        for (int i = 0; i < numbers.Length; i++)
        {
            for (int j = i + 1; j < numbers.Length; j++)
            {
                if (numbers[i] > 0 && numbers[j] > 0 && numbers[i] > numbers[j]) inversions++;
            }
        }

        return inversions;
    } // O(n^2)

    private bool IsSolvable()
    {
        int[] tiles = new int[Size * Size];
        int index = 0;
        for (int i = 0; i < Size; i++)
        for (int j = 0; j < Size; j++)
            tiles[index++] = _tiles[i, j];
        // row of 0 + inversion count, ako e cheten size-a 
        if (Size % 2 == 0) return (InversionCount(tiles) + _currIndexOfZero.Item1) % 2 != 0;
        else return InversionCount(tiles) % 2 == 0;
    }

    private Dictionary<string, Board?> Neighbors()
    {
        return new Dictionary<string, Board?>()
            {
                { "Up", Up() },
                { "Down", Down() },
                { "Left", Left() },
                { "Right", Right() }
            }.Where(b => b.Value != null)
            .OrderBy(b => b.Value!.ManhattanDistance)
            .ToDictionary(b => b.Key, b => b.Value);
    }

    private Board? Move(int tilesX, int tilesY)
    {
        try
        {
            if(!InsideMatrix(_currIndexOfZero.Item1+tilesX,_currIndexOfZero.Item2+tilesY)){
                return null;
            }
            var oldZeroIndex = this._currIndexOfZero;
            var board = new Board(Size, _tiles, FinalIndexOfZero, null, this._currIndexOfZero);
            board.Swap(_currIndexOfZero, _currIndexOfZero.Item1 + tilesX, _currIndexOfZero.Item2 + tilesY);
            board._currIndexOfZero =
                new Tuple<int, int>(_currIndexOfZero.Item1 + tilesX, _currIndexOfZero.Item2 + tilesY);

            board.ManhattanDistance = this.ManhattanDistance;
            board.ManhattanDistance += board.ModifiedManhatanDistance(oldZeroIndex, board._currIndexOfZero);
            
            // board.Manhattan();
            return board;
        }
        catch (IndexOutOfRangeException)
        {
            return null;
        }
    }

    private bool InsideMatrix(int newX, int newY)
    {
        return newX >= 0 && newY >= 0 && newX < Size && newY < Size;
    }

    private int ModifiedManhatanDistance(Tuple<int, int> oldZero, Tuple<int, int> newZero)
    {
        var goalIndex = FindIndexOfValueGoalState(_tiles[oldZero.Item1, oldZero.Item2]);
        var oldDistance = Distance(newZero, goalIndex.Item1, goalIndex.Item2);
        var newDistance = Distance(oldZero, goalIndex.Item1, goalIndex.Item2);
        return newDistance - oldDistance;
    }

    private void Swap(Tuple<int, int> ind1, int newX, int newY)
    {
        (_tiles[ind1.Item1, ind1.Item2], _tiles[newX, newY]) =
            (_tiles[newX, newY], _tiles[ind1.Item1, ind1.Item2]);
    }

    private void Ida_star()
    {
        int bound = ManhattanDistance;
        Path.Push(this);
        while (Path.Count > 0)
        {
            int t = Search(Path, 0, bound);
            if (t == 0) return;
            bound = t;
        }
    }

    private int Search(Stack<Board> path, int currentCost, int bound)
    {
        var currNode = path.Peek();
        int f = currentCost + currNode.ManhattanDistance;
        if (f > bound) return f;
        if (currNode.ManhattanDistance==0 && currNode.IsGoal(this.GoalState)) return 0;
        int min = int.MaxValue;
        var neighbors = currNode.Neighbors();
        foreach (var node in neighbors)
        {
            PathString.Push(node.Key);
            path.Push(node.Value!);
            int t = Search(path, currentCost+1, bound);
            if (t == 0) return 0;
            if (t <= min) min = t;
            PathString.Pop();
            path.Pop();
        }

        return min;
    }

    private bool sameField(int[,] arr1, int[,] arr2)
    {
        for (int i = 0; i < arr1.GetLength(0); i++)
        for (int j = 0; j < arr1.GetLength(0); j++)
            if (arr1[i, j] != arr2[i, j])
                return false;
        return true;
    }

    private Tuple<int, int> FindIndexOfValue(int value, int[,] board)
    {
        for (var i = 0; i < Size; i++)
        for (var j = 0; j < Size; j++)
            if (board[i, j] == value)
                return Tuple.Create(i, j);

        return Tuple.Create(-1, -1);
    } //O(n^2)

    private Tuple<int, int> FindIndexOfValueGoalState(int value)
    {
        if (value > FinalIndexOfZero && FinalIndexOfZero != -1)
            value++;
        int i = Convert.ToInt32((value - 1) / Size);
        int j = ((value - 1) % Size);
        return Tuple.Create(i, j);
    } // O(1)

    private void Manhattan()
    {
        int manhattanDistance = 0;
        for (var i = 0; i < Size; i++)
        for (var j = 0; j < Size; j++)
            if (_tiles[i, j] != 0)
                manhattanDistance += Distance(FindIndexOfValueGoalState(_tiles[i, j]), i, j);
        this.ManhattanDistance = manhattanDistance;
    } //O(n^2)

    private int Distance(Tuple<int, int> el1, int i, int j) => Math.Abs(el1.Item1 - i) + Math.Abs(el1.Item2 - j);

    private bool IsGoal(int[,] goalState)
    {
        for (var i = 0; i < Size; i++)
        for (int j = 0; j < Size; j++)
            if (this._tiles[i, j] != goalState[i, j])
                return false;
        return true;
    }

    private Board? Down()
    {
        return Move(-1, 0);
    }

    private Board? Up()
    {
        return Move(1, 0);
    }

    private Board? Right()
    {
        return Move(0, -1);
    }

    private Board? Left()
    {
        return Move(0, 1);
    }
    

    

    // public override string ToString()
    // {
    //     return PrintField(_tiles);
    // } //O(n^2)

    private bool Equals(Board? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return ManhattanDistance == other.ManhattanDistance && 
               _tiles.Equals(other._tiles) && Size == other.Size;
    }
}