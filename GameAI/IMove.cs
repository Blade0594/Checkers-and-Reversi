using System;
using System.Collections.Generic;
using System.Text;

namespace GameAI
{
    public interface IMove
    {
         double GetMoveScore();
         void AddToScore(double value);
         void SetMoveScore(double value);
         int GetRank();
         void SetRank(int rank);
         bool IsEqual(IMove Move);
    }
    
}
