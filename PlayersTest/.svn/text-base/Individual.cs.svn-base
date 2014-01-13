using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayersTest
{
    class Individual
    {
        public double[] Weights {  get; private set;}
        public string Name {  get; private set; }
        public int SearchDepth { get; set; }
        public TimeSpan PruningTime { get; set; }
        public TimeSpan NonPruningTime { get; set; }
        public int MissScoreCalculation { get; set; }
        public int MoveCount { get; set; }
        public Individual(double[] Weights, string Name)
        {
            this.Name = Name;
            this.Weights = Weights;
        
        }
        public int TotalWhiteWins {set; get;}
        public int TotalBlackWins { set; get; }
        public int Loses { set; get; }
        public int Draws { set; get; }
        
        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Performance of "+this.Name+" is:"+ Environment.NewLine);
            sb.Append("Loses = " + Loses.ToString() + Environment.NewLine);
            sb.Append("Draws = " + Draws.ToString() + Environment.NewLine);
            sb.Append("Total White Games won = " + TotalWhiteWins.ToString() + Environment.NewLine);
            sb.Append("Total Black Games won = " + TotalBlackWins.ToString() + Environment.NewLine);
            return sb.ToString();
        }
    }
}
