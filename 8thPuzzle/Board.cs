namespace EightPuzzle;

struct Coordinates
{
    public Coordinates(sbyte x, sbyte y)
    {
        X = x;
        Y = y;
    }

    public sbyte X { get; set; }
    public sbyte Y { get; set; }
    
}
class Board
{
    
    public int[,] _tiles;

    private Coordinates _currIndexOfZero;
    public int ManhattanDistance { get; set; }
    public int[,] GoalState { get; private set; }
    public Stack<string> PathString { get; set; } = new Stack<string>();
    public Stack<Board> Path { get; set; } = new Stack<Board>();
    public int Size { get; }
    public int FinalIndexOfZero { get; private set; }

    public Board(int boardSize, int[,] board, int indexOfZero = -1, int[,]? goalState = null,
        Coordinates? currIndexOfZero = null)
    {
        Size = boardSize;
        FinalIndexOfZero = indexOfZero;
        this._tiles = (board.Clone() as int[,])!;
        this.GoalState = goalState;
        if (currIndexOfZero == null) this._currIndexOfZero = FindIndexOfValue(0, this._tiles);
        else _currIndexOfZero = (Coordinates)currIndexOfZero;
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
        if (Size % 2 == 0) return (InversionCount(tiles) + _currIndexOfZero.X) % 2 != 0;
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

    private Board? Move(sbyte tilesX, sbyte tilesY)
    {
        try
        {
            if(!InsideMatrix(_currIndexOfZero.X+tilesX,_currIndexOfZero.Y+tilesY)){
                return null;
            }
            var oldZeroIndex = this._currIndexOfZero;
            var board = new Board(Size, _tiles, FinalIndexOfZero, null, this._currIndexOfZero);
            board.Swap(_currIndexOfZero, _currIndexOfZero.X + tilesX, _currIndexOfZero.Y + tilesY);
            board._currIndexOfZero.X += tilesX;
            board._currIndexOfZero.Y += tilesY;

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

    private int ModifiedManhatanDistance(Coordinates oldZero, Coordinates newZero)
    {
        var goalIndex = FindIndexOfValueGoalState(_tiles[oldZero.X, oldZero.Y]);
        var oldDistance = Distance(newZero, goalIndex.X, goalIndex.Y);
        var newDistance = Distance(oldZero, goalIndex.X, goalIndex.Y);
        return newDistance - oldDistance;
    }

    private void Swap(Coordinates coordinates, int newX, int newY)
    {
        (_tiles[coordinates.X, coordinates.Y], _tiles[newX, newY]) =
            (_tiles[newX, newY], _tiles[coordinates.X, coordinates.Y]);
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
    private Coordinates FindIndexOfValue(int value, int[,] board)
    {
        for (sbyte i = 0; i < Size; i++)
        for (sbyte j = 0; j < Size; j++)
            if (board[i, j] == value)
                return new Coordinates(i,j);

        return new Coordinates(-1,-1);
    } //O(n^2)

    private Coordinates FindIndexOfValueGoalState(int value)
    {
        if (value > FinalIndexOfZero && FinalIndexOfZero != -1)
            value++;
        sbyte i = Convert.ToSByte((value - 1) / Size);
        sbyte j = Convert.ToSByte((value - 1) % Size);
        return new Coordinates(i,j);
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

    private int Distance(Coordinates el1, int i, int j) => Math.Abs(el1.X - i) + Math.Abs(el1.Y - j);

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