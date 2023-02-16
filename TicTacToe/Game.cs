namespace TicTacToe;

class Game
{
    private const char AiSymbol = 'X';
    private const char PlayerSymbol = 'O';
    private bool _isPlayersTurn = true;
    private readonly char[] _board;
    private int _depth;

    public Game()
    {
        _board = new char[9];
        _depth = 0;
        for (int i = 0; i < 9; i++)
            _board[i] = '-';
    }

    public void Play()
    {
        _isPlayersTurn = InputForFirst();

        for (int i = 0; i <= 9; i++)
        {
            Console.Clear();
            PrintBoard();
            if (IsOver())
            {
                PrintResult();
                break;
            }

            if (_isPlayersTurn)
            {
                Console.WriteLine($"Enter valid indexes for placing {PlayerSymbol}");
                bool validIndexes;
                int[] indexes;
                do
                {
                    indexes = Console.ReadLine()!
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(int.Parse).ToArray();
                    validIndexes = ValidInput(indexes);
                    if (!validIndexes) Console.WriteLine("Invalid indexes");
                } while (!validIndexes);

                PlaceChoice(indexes);
            }
            else
            {
                AiPlay();
            }

            _isPlayersTurn = !_isPlayersTurn;
        }
    }
    private int Minimax(int alpha, int beta, bool maximizer)
    {
        var score = 9 - _depth;
        var eval = IsFinal();
        if (eval > 0) return eval + score;
        if (eval < 0) return eval - score;
        if (IsDraw()) return 0;
        
        if (maximizer)
        {
            var max = int.MinValue;

            for (var i = 0; i < 9; i++)
            {
                if (_board[i] != '-') continue;

                _board[i] = AiSymbol;
                _depth++;

                var evaluation = Minimax(alpha, beta, false);

                _board[i] = '-';
                _depth--;

                max = Math.Max(max, evaluation);
                alpha = Math.Max(alpha, evaluation);

                if (beta <= alpha) break; //alpha-beta puring part
            }
            return max;
        }

        var min = int.MaxValue;
        for (var i = 0; i < 9; i++)
        {
            if (_board[i] != '-') continue;

            _board[i] = PlayerSymbol;
            _depth++;

            var evaluation = Minimax(alpha, beta, true);

            _board[i] = '-';
            _depth--;

            min = Math.Min(min, evaluation);
            beta = Math.Min(beta, evaluation);

            if (beta <= alpha) break; //alpha-beta puring part
        }
        return min;
    }
    private void PrintResult()
    {
        var evaluation = IsFinal();
        if (evaluation > 0)
        {
            Console.WriteLine("You lost");
            return;
        }

        Console.WriteLine("Draw");
    }
    private int NextMove()
    {
        var indexToMove = -1;
        var bestEvaluation = int.MinValue;

        for (var i = 0; i < 9; i++)
        {
            if (_board[i] != '-') continue;

            _board[i] = AiSymbol;
            _depth++;

            var curr = Minimax(int.MinValue, int.MaxValue, false);

            _board[i] = '-';
            _depth--;

            if (curr > bestEvaluation)
            {
                bestEvaluation = curr;
                indexToMove = i;
            }
        }

        return indexToMove;
    }
    private void AiPlay()
    {
        var index = NextMove();
        _board[index] = AiSymbol;
        _depth++;
    }
    private void PlaceChoice(int[] indexes)
    {
        var index = (indexes[0]-1) * 3 + (indexes[1]-1);
        _board[index] = PlayerSymbol;
        _depth++;
    }
    private bool Equal(char a, char b, char c)
    {
        return a == b && b == c && IsSymbol(a);
    }
    private int IsFinal()
    {
        char winner = '-';
        if (Equal(_board[0], _board[4], _board[8]))
            winner = _board[0];

        if (Equal(_board[2], _board[4], _board[6]))
            winner = _board[2];

        //rows
        for (int i = 0; i < 9; i += 3)
        {
            if (Equal(_board[i], _board[i + 1], _board[i + 2]))
            {
                winner = _board[i];
            }
        }

        //columns
        for (int i = 0; i < 3; i++)
        {
            if (Equal(_board[i], _board[i + 3], _board[i + 6]))
            {
                winner = _board[i];
            }
        }

        if (winner == '-') return 0;
        return winner == AiSymbol ? 1 : -1;
    }
    private bool IsDraw()
    {
        return _depth == 9;
    }
    private bool IsOver()
    {
        return IsDraw() || IsFinal() != 0;
    }
    private static bool IsSymbol(char c)
    {
        return c is PlayerSymbol or AiSymbol;
    }
    private bool InputForFirst()
    {
        Console.WriteLine("Who is first? Enter P for player, A for AI.");
        var input = Console.ReadLine()!.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
        var choice = char.Parse(input);
        return choice == 'P';
    }
    private void PrintBoard()
    {
        for (int i = 0; i < 9; i+=3)
        {
            Console.WriteLine(_board[i] + "|" + _board[i+1] + "|" + _board[i+2]);
        }
    }
    private bool ValidInput(int[] indexes)
    {
        bool validIndexes = ValidIndexes(indexes);
        if (!validIndexes) return validIndexes;
        int i = indexes[0] - 1;
        int j = indexes[1] - 1;
        char currChar = _board[(i*3)+j];
        return !IsSymbol(currChar);
    }
    private bool ValidIndexes(int[] indexes)
    {
        return indexes[0] >= 1 && indexes[0] <= 3 && indexes[1] >= 1 && indexes[1] <= 3;
    }
}