using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionLearner
{
    class Individaul
    {
        double[] weights;
        double fitness;
        public double ProbabilityOfSelection { get; set; }
        public double CumulativeProbability { get; set; }
        public Individaul(int lenght)
        {
            weights = new double[lenght];
            fitness = 0;
        }
        public Individaul(double[] weights)
        {
            this.weights = new double[weights.Length];
            for (int index = 0; index < weights.Length; ++index)
                this.weights[index] = weights[index];
            fitness = 0;
        }
        public void AddToFitness(int score)
        {
            fitness += score;
        }
        public void ResetFitness()
        {
            fitness = 0;
        }
        public void CrossOver(Individaul pair, Random rand)
        {
            int whereToSplit = rand.Next(weights.Length - 1);
            for (int index = whereToSplit; index < weights.Length; index++)
            {
                double aux = this.weights[index];
                this.weights[index] = pair.weights[index];
                pair.weights[index] = aux;
            
            }
        
        }
        public void Mutation(double probability, Random rand)
        {
            for (int index = 0; index < this.weights.Length; index++)
                if (rand.NextDouble() < probability)
                    weights[index] = rand.NextDouble();
        }
        public double Fitness { get { return fitness; } }
        public double[] Weights { get { return this.weights; } }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int index = 0; index < weights.Length; ++index)
            {
                sb.Append(weights[index]);
                sb.Append(' ');
            }
            //sb.Append("Fitness = " + fitness.ToString());
            return sb.ToString();
        }
    }
}
