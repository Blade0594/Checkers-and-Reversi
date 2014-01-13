using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameAI;
namespace ReversiGame
{
    public class ReversiPlayer : IPlayer
    {
        int playerID = 0;
        public ReversiPlayer(int playerID)
        {
            this.playerID = playerID;
        
        }

        #region IPlayer Members

        public int GetPlayerID()
        {
            return playerID;
        }

        #endregion
    }
}
