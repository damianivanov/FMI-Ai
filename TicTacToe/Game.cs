namespace TicTacToe;

class Game
{
    const char AISymbol = 'X';
    const char PlayerSymbol = 'O';
    private bool isPlayersTurn = true;
    private char[] board;
    private int depth = 0;

    public Game()
    {
        board = new char[9];
        depth = 0;
        for (int i = 0; i < 9; i++)
            board[i] = '-';
    }

    public void Play()
    {
        isPlayersTurn = InputForFirst();

        for (int i = 0; i <= 9; i++)
        {
            Console.Clear();
            PrintBoard();
            if (IsOver())
            {
                PrintResult();
                break;
            }

            if (isPlayersTurn)
            {
                Console.WriteLine($"Enter valid indexes for placing {PlayerSymbol}");
                bool validIndexes = false;
                int[] indexes = new int[2];
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

            isPlayersTurn = !isPlayersTurn;
        }
    }
    private int Minimax(int alpha, int beta, bool maximizer)
    {
        var score = 9 - depth;
        var eval = IsFinal();
        if (eval > 0) return eval + score;
        if (eval < 0) return eval - score;
        if (IsDraw()) return 0;
        
        if (maximizer)
        {
            var max = int.MinValue;

            for (var i = 0; i < 9; i++)
            {
                if (board[i] != '-') continue;

                board[i] = AISymbol;
                depth++;

                var evaluation = Minimax(alpha, beta, false);

                board[i] = '-';
                depth--;

                max = Math.Max(max, evaluation);
                alpha = Math.Max(alpha, evaluation);

                if (beta <= alpha) break;
            }
            return max;
        }

        var min = int.MaxValue;
        for (var i = 0; i < 9; i++)
        {
            if (board[i] != '-') continue;

            board[i] = PlayerSymbol;
            depth++;

            var evaluation = Minimax(alpha, beta, true);

            board[i] = '-';
            depth--;

            min = Math.Min(min, evaluation);
            beta = Math.Min(beta, evaluation);

            if (beta <= alpha) break;
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
            if (board[i] != '-') continue;

            board[i] = AISymbol;
            depth++;

            var curr = Minimax(int.MinValue, int.MaxValue, false);

            board[i] = '-';
            depth--;

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
        board[index] = AISymbol;
        depth++;
    }
    private void PlaceChoice(int[] indexes)
    {
        var index = (indexes[0]-1) * 3 + (indexes[1]-1);
        board[index] = PlayerSymbol;
        depth++;
    }
    private bool Equal(char a, char b, char c)
    {
        return a == b && b == c && IsSymbol(a);
    }
    private int IsFinal()
    {
        char winner = '-';
        if (Equal(board[0], board[4], board[8]))
            winner = board[0];

        if (Equal(board[2], board[4], board[6]))
            winner = board[2];

        //rows
        for (int i = 0; i < 9; i += 3)
        {
            if (Equal(board[i], board[i + 1], board[i + 2]))
            {
                winner = board[i];
            }
        }

        //columns
        for (int i = 0; i < 3; i++)
        {
            if (Equal(board[i], board[i + 3], board[i + 6]))
            {
                winner = board[i];
            }
        }

        if (winner == '-') return 0;
        return winner == AISymbol ? 1 : -1;
    }
    private bool IsDraw()
    {
        return depth == 9;
    }
    private bool IsOver()
    {
        return IsDraw() || IsFinal() != 0;
    }
    private static bool IsSymbol(char c)
    {
        return c is PlayerSymbol or AISymbol;
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
            Console.WriteLine(board[i] + "|" + board[i+1] + "|" + board[i+2]);
        }
    }
    private bool ValidInput(int[] indexes)
    {
        bool validIndexes = ValidIndexes(indexes);
        if (!validIndexes) return validIndexes;
        int i = indexes[0] - 1;
        int j = indexes[1] - 1;
        char currChar = board[(i*3)+j];
        return !IsSymbol(currChar);
    }
    private bool ValidIndexes(int[] indexes)
    {
        return indexes[0] >= 1 && indexes[0] <= 3 && indexes[1] >= 1 && indexes[1] <= 3;
    }
}