using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionLearnerPerceptron
{
    public class Individual
    {
        public double[] Weights { get; set; }
        public string Name { get; private set; }
        public int SearchDepth { get; set; }
        public TimeSpan PruningTime { get; set; }
        public TimeSpan NonPruningTime { get; set; }
        public int MissScoreCalculation { get; set; }
        public int MoveCount { get; set; }
        public List<int> Labels { get; set; }
        public Individual(double[] Weights, string Name ="", int initialPopulationSize = 0)
        {
            this.Name = Name;
            this.Weights = Weights;
            this.SearchDepth = 8;
            this.Labels = new List<int>();
            for (int index = 0; index < initialPopulationSize; index++)
                Labels.Add(0);
        }
        public int TotalWhiteWins { set; get; }
        public int TotalBlackWins { set; get; }
        public int Loses { set; get; }
        public int Draws { set; get; }
        public int Wins { get { return TotalWhiteWins + TotalBlackWins; } }
        public static double[] operator +(Individual indiv1, Individual indiv2)
        {
            double[] weights = new double[indiv1.Weights.Length];
            for (int index = 0; index < weights.Length; index++)
                weights[index] = indiv1.Weights[index] + indiv2.Weights[index];
            return weights;
        }
        public static double[] operator -(Individual indiv1, Individual indiv2)
        {
            double[] weights = new double[indiv1.Weights.Length];
            for (int index = 0; index < weights.Length; index++)
                weights[index] = indiv1.Weights[index] - indiv2.Weights[index];
            return weights;
        }
        public static double[] operator *(Individual indiv, double number)
        {
            double[] newWeights = new double[indiv.Weights.Length];
            for (int index = 0; index < indiv.Weights.Length; index++)
                newWeights[index] = indiv.Weights[index] * number;
            return newWeights;
        }
        public static double[] operator /(Individual indiv, double number)
        {
            double[] newWeights = new double[indiv.Weights.Length];
            for (int index = 0; index < indiv.Weights.Length; index++)
                newWeights[index] = indiv.Weights[index] / number;
            return newWeights;
        }
        public void ResetLables()
        {
            for (int index = 0; index < Labels.Count; index++)
                this.Labels[index] = 0;
        }
        public double EuclideanNorm()
        {
            double norm = 0;
            for (int index = 0; index < this.Weights.Length; index++)
                norm += Math.Pow(Weights[index], 2);
            return Math.Sqrt(norm);
        }
        //Modified version because of indiv having 0 on the last pos always.
        public double DotProduct(double[] indiv)
        {
            double sum = 0;
            for (int index = 0; index < this.Weights.Length -1; index++)
            {
                sum += this.Weights[index] * indiv[index];
            }
            sum += this.Weights[Weights.Length - 1];
            return sum;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            //sb.Append("Performance of " + this.Name + " is:" + Environment.NewLine);
            //sb.Append("Loses = " + Loses.ToString() + Environment.NewLine);
            //sb.Append("Draws = " + Draws.ToString() + Environment.NewLine);
            //sb.Append("Total White Games won = " + TotalWhiteWins.ToString() + Environment.NewLine);
            //sb.Append("Total Black Games won = " + TotalBlackWins.ToString() + Environment.NewLine);
            for (int index = 0; index < Weights.Length; index++)
                sb.Append(Weights[index].ToString() + " ");
            return sb.ToString().TrimEnd();
        }
        public void SetLastPos()
        {
            this.Weights[Weights.Length - 1] = 1;
        }
        internal void ResetScores()
        {
            this.TotalBlackWins = 0;
            this.TotalWhiteWins = 0;
            this.Loses = 0;
            this.Draws = 0;
        }

        internal void Normalize()
        {
            double norm = this.EuclideanNorm();
            for (int index = 0; index < this.Weights.Length; index++)
                Weights[index] /= norm;
        }
    }
}
