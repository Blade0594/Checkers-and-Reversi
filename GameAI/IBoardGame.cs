using System;
using System.Collections.Generic;
using System.Text;

namespace GameAI
{
    public interface IBoardGame
    {
        List<IMove> GetLegalMoves();
        List<IMove> GetLegalMoves(IPlayer player);
        IBoardGame TestMove(IMove move);
        int EvaluatePosition(IBoardGame lastBoardPosition);
        IMove EvaluatePosition();
        IPlayer GetCurrentPlayer();
        bool GameIsOver();


    }
}
