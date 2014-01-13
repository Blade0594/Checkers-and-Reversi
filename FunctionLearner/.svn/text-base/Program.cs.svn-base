using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using GameAI;
using CheckersGame;
using Amib.Threading;
namespace FunctionLearner
{
    
    class Program
    {
        static readonly double crossOverProbability = 0.25;
        static readonly double mutationProbability = 0.01;
        static void Main(string[] args)
        {
            Random generator = new Random(DateTime.Now.Millisecond);

            int populationSize = 70;
            int numberOfIterations = 30;
            List<Individaul> population = new List<Individaul>();
            for (int index = 0; index < populationSize; index++)
            {
                double[] random = new double[11];
                for (int j = 0; j < random.Length; ++j)
                    random[j] = generator.NextDouble();
                population.Add(new Individaul(random));
            }
          
            int popNumber = 0;
            FileStream file = new FileStream(@"D:\Weights.txt", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter writer = new StreamWriter(file);
            do
            {
                if (popNumber > 0)
                {
                    file = new FileStream(@"D:\Weights.txt", FileMode.Append, FileAccess.Write);
                    writer = new StreamWriter(file);
                }
                popNumber++;
                SmartThreadPool smtp = new SmartThreadPool();
                for (int i = 0; i < populationSize; i++)
                {
                    for (int j = 0; j < populationSize; j++)
                    {
                        if (i == j) continue;
                        Individaul[] weights = { population[i], population[j] };
                        smtp.QueueWorkItem(new WorkItemCallback(ThreadProc), weights);
                    }
                }
                smtp.WaitForIdle();
                smtp.Shutdown();
                
                Console.WriteLine(" Population number {0} time {1}", popNumber.ToString(), DateTime.Now.ToShortTimeString());
                writer.WriteLine("#generation number: {0}", popNumber.ToString());
                for (int i = 0; i < populationSize; i++)
                {
                    writer.WriteLine("cromozom " + population[i].ToString());
                    writer.WriteLine("fitness value {0}", population[i].Fitness.ToString());

                }
                writer.Flush();
                writer.Close();
                file.Close();
                double totalFitness = 0;
                for (int i = 0; i < population.Count; ++i)
                    totalFitness += population[i].Fitness;
                double cumulativeProbability = 0;
                for (int i = 0; i < population.Count; ++i)
                {

                    population[i].ProbabilityOfSelection = (double)(population[i].Fitness / totalFitness);
                    cumulativeProbability += population[i].ProbabilityOfSelection;
                    population[i].CumulativeProbability = cumulativeProbability;
                }
                List<Individaul> newPopulation = new List<Individaul>();
                for (int i = 0; i < populationSize; i++)
                {
                    double rand = generator.NextDouble();
                    for (int j = 0; j < populationSize; j++)
                        if (rand > population[j].CumulativeProbability)
                            continue;
                        else
                        {
                            newPopulation.Add(new Individaul(population[j].Weights));
                            break;
                        }

                }
                population = newPopulation;
                List<Individaul> toMutate = new List<Individaul>();
                for (int i = 0; i < populationSize; i++)
                {
                    double rand = generator.NextDouble();
                    if (rand < crossOverProbability)
                        toMutate.Add(population[i]);
                }
                if (toMutate.Count % 2 != 0 && toMutate.Count > 0)
                    toMutate.RemoveAt(toMutate.Count - 1);
                if (toMutate.Count >= 2)
                {
                    for (int i = 0; i < toMutate.Count; i = i + 2)
                    {
                        toMutate[i].CrossOver(toMutate[i + 1], generator);
                    }
                }
                for (int i = 0; i < populationSize; ++i)
                    population[i].Mutation(mutationProbability, generator);
            }
            while (popNumber < numberOfIterations);
        }
        /// <summary>
        ///  This procedure takes two individuals and makes them play against each other
        /// </summary>
        /// <param name="data">Should be an array of doubles representing two individuals</param>
        /// <returns>The individuals after playing against each other</returns>
        static object ThreadProc(Object data)
        {
            Individaul[] weights = data as Individaul[];
            GameAI.Algorithms alg = new Algorithms(new CheckersPlayer(1));
            CheckersBoard whiteBoard = new CheckersBoard(0, weights[0].Weights);
            CheckersBoard blackBoard = new CheckersBoard(0, weights[1].Weights);
            whiteBoard.GameMaxLength = 200;
            blackBoard.GameMaxLength = 200;
            while (!blackBoard.GameIsOver())
            {
                CheckersMove move = alg.ABNegamax(blackBoard, 4, 0, double.MinValue, double.MaxValue) as CheckersMove;
                whiteBoard.MakeMove(move);
                blackBoard.MakeMove(move);
                if (blackBoard.GameIsOver())
                    break;
                else
                {

                    move = alg.ABNegamax(whiteBoard, 4, 0, double.MinValue, double.MaxValue) as CheckersMove;
                    whiteBoard.MakeMove(move);
                    blackBoard.MakeMove(move);
                }
            }
            switch (whiteBoard.Winner)
            {
                case 0:
                    weights[0].AddToFitness(1);
                    weights[1].AddToFitness(1);
                    break;
                case 1:
                    weights[0].AddToFitness(3);
                    break;
                case -1:
                    weights[1].AddToFitness(3);
                    break;
            }
            return weights as object;
        }
    }
}
