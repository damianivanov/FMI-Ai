namespace TicTacToe;
class Move
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Score { get; set; }

    public Move(int x, int y, int score)
    {
        X = x;
        Y = y;
        Score = score;
    }
}