using System;
using System.Collections.Generic;
using System.Text;
using GameAI;
namespace CheckersGame
{
    public class CheckersPlayer : IPlayer
    {
        private int playerID;
        public CheckersPlayer(int playerID)
        {
            this.playerID = playerID;
        
        }
        #region IPlayer Members

        public int GetPlayerID()
        {
            return this.playerID;
        }
        public override string ToString()
        {
            if (playerID == 1)
                return "White";
            else
                return "Black";
        }
        #endregion
    }
}
