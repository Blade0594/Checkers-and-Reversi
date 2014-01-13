using System;
using System.Collections.Generic;
using System.Text;

using GameAI;
namespace CheckersGame
{
    public class CheckersMove : IMove
    {

        private List<Point> path;
        private bool isJumpMove;
        private double moveScore;
        private int rank;
        public TimeSpan Duration{get;set;}
        public int PlayerID { private set; get; }
        public CheckersMove(List<Point> path, bool isJumpMove, int playerID)
        {
            this.PlayerID = playerID;
            this.path = new List<Point>();
            this.path = path;
            this.isJumpMove = isJumpMove;
            this.moveScore = double.MinValue;
        }
        public int GetRank()
        {

            return this.rank;
        }
        public void SetRank(int rank)
        {
            this.rank = rank;        
        }
        public Point GetPiece()
        {
            if (path.Count == 0) throw new ArgumentNullException();
            return path[0];

        }
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(path.Count.ToString() + ' ');
            foreach (var item in path)
            {
                builder.Append(item.X.ToString() + ' ' + item.Y + ' ');

            }
            builder.Append(IsJump.ToString() + ' ');
            builder.Append(PlayerID.ToString()); 
            return builder.ToString();
        }
        public string ToShortString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in path)
            {
                builder.Append(item.X.ToString() + ' ' + item.Y + ' ');

            }
            return builder.ToString().TrimEnd(' ');
        
        }
        public Point GetPathAt(int index)
        {
            if (index >= path.Count)
                return null;
            return path[index];
        }
        public double GetMoveScore()
        {
            return this.moveScore;
        }
        public int NumberOfMoves
        {
            get { return this.path.Count; }
        }
        public bool IsJump
        {
            get { return this.isJumpMove; }
        }
        public void AddToScore(double value)
        {
            this.moveScore += value;
        }

        public void SetMoveScore(double value)
        {
            this.moveScore = value;
        }
        public List<Point> GetPath()
        {
            return this.path;
        }


        #region IMove Members


        public bool IsEqual(IMove Move)
        {
            CheckersMove move = Move as CheckersMove;
            if (move.isJumpMove != this.isJumpMove)
                return false;
            if (move.GetPath().Count != this.GetPath().Count) return false;
            for (int index = 0; index < move.GetPath().Count; index++)
                if (!move.GetPathAt(index).IsEqual(this.GetPathAt(index))) 
                    return false;
            return true;
        }

        #endregion
    }
}
