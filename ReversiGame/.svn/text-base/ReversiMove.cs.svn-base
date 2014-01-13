using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameAI;
namespace ReversiGame
{
    public class ReversiMove : IMove
    {
        int row;
        int column;
        int playerID;
        double moveScore;

        public ReversiMove(int row, int column, int playerID, double moveScore)
        {
            this.row = row;
            this.column = column;
            this.playerID = playerID;
            this.moveScore = moveScore;
        }
        public int Column { get { return column; } }
        public int Row { get { return row; } }

        #region IMove Members
        public bool IsEqual(IMove reversiMove)
        {
            throw new NotImplementedException();
        }

        public double GetMoveScore()
        {
            return moveScore;
        }

        public int GetRank()
        {
            throw new NotImplementedException();
        
        }
        public void SetRank(int rank)
        {
            throw new NotImplementedException();
        }

        public void AddToScore(double value)
        {
            moveScore += value;
        }
        public void SetMoveScore(double value)
        {
            this.moveScore = value;
        }
        public int PlayerID
        {
            get { return playerID; }
        }

        #endregion
    }
}
