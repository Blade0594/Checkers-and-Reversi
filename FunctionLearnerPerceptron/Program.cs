using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amib.Threading;
using GameAI;
using CheckersGame;
using System.IO;
namespace FunctionLearnerPerceptron
{
    class Program
    {
        private static readonly Random rand = new Random(DateTime.Now.Millisecond);
        //Number of players in the initial population
        private static readonly int initialPopulationSize = 10;
        //Number of games that each player plays against each other
        private static readonly double numberOfGames = 15;
        //Maximum number of games to be played for training
        //Number of features on the eval function
        private static readonly int numberOfFeatures = 11;
        private static readonly double sigma = 1;
        private static Individual tetha;
        private static Individual xStar;
        private static List<Individual> population;
        static void Main(string[] args)
        {
            //DataReader data = new DataReader("Initial.txt");
            //Individual weigthsPerceptron = data.GetPerceptronWeights(numberOfFeatures);
            //List<Individual> players = data.GetPopulation(numberOfFeatures);
           
             int gameCounter = 0;
           
             double[] initialTetha = new double[numberOfFeatures];
             double[] initialXStar = new double[numberOfFeatures];
             for (int index = 0; index < numberOfFeatures; index++)
             {
                 initialTetha[index] = rand.NextDouble();
                 initialXStar[index] = rand.NextDouble();

             }
             xStar = new Individual(initialXStar, "Star", initialPopulationSize + 1);
             tetha = new Individual(initialTetha, "Tetha");
             int iteration = 0;
            
             DateTime maxDate = new DateTime(2011, 10, 24, 10, 30, 00);
             do
             {
                 if (iteration % 100 == 0)
                 {
                     population = initializePopulation();
                     FileStream file = new FileStream("Results.txt", FileMode.Append, FileAccess.Write);
                     StreamWriter sw = new StreamWriter(file);
                     sw.WriteLine("Played games {0}, iteration {1}", gameCounter, iteration);
                     sw.WriteLine("Tetha " + tetha.ToString());
                     sw.WriteLine("xStar " + xStar.ToString());
                     sw.Close();
                     file.Close();
                 }
                 for (int index = 0; index < population.Count; index++)
                     for (int index2 = 0; index2 < population.Count; index2++)
                     {
                         if (index == index2 || population[index].Labels[index2] != 0)
                             continue;
                         Individual[] weights = { population[index], population[index2] };
                         int classLabel = calculateLabel(weights);
                         gameCounter += (int)numberOfGames;
                         population[index].Labels[index2] = classLabel;
                         population[index2].Labels[index] = (int)numberOfGames - classLabel;
                         //population[index].Labels[index2] = rand.Next((int)numberOfGames);
                         //population[index2].Labels[index] = (int)numberOfGames - population[index].Labels[index2];

                     }

                 double epsg = 0.0000000001;
                 double epsf = 0;
                 double epsx = 0;
                 double stpmax = 0.1;
                 int maxits = 0;
                 alglib.mincgstate state;
                 alglib.mincgreport rep;

                 // tetha run
                 double[] newTetha = new double[numberOfFeatures];
                 for (int index = 0; index < newTetha.Length; index++)
                     newTetha[index] = tetha.Weights[index];
                 alglib.mincgcreate(newTetha, out state);
                 alglib.mincgsetcond(state, epsg, epsf, epsx, maxits);
                 alglib.mincgsetstpmax(state, stpmax);
                 alglib.mincgoptimize(state, function1_grad, null, null);
                 alglib.mincgresults(state, out newTetha, out rep);
                 for (int index = 0; index < newTetha.Length; index++)
                      tetha.Weights[index] = newTetha[index];
                 //xStarRun
                 double[] newXStar =new double[numberOfFeatures];
                 for (int index = 0; index < newTetha.Length; index++)
                     newXStar[index] = xStar.Weights[index];
                 alglib.mincgcreate(newXStar, out state);
                 alglib.mincgsetcond(state, epsg, epsf, epsx, maxits);
                 alglib.mincgsetstpmax(state, stpmax);
                 alglib.mincgoptimize(state, function2_grad, null, null);
                 alglib.mincgresults(state, out newXStar, out rep);
                 for (int index = 0; index < newTetha.Length; index++)
                     xStar.Weights[index] = newXStar[index];
                 xStar.ResetLables();
                 xStar.Normalize();  
                 iteration++;
                 //for (int index = 0; index < population.Count; index++)
                 //    population[index].ResetLables();
             }
             while (true);


        }
        public static void function1_grad(double[] x, ref double func, double[] grad, object obj)
        {
            // this callback calculates the theta values
            // and its derivatives 
            Individual toBeOptimized = new Individual(x);
            func = tethaFunctionValue(toBeOptimized, population);
            double[] gradCopy = tethaGradientValue(toBeOptimized, population);
            for (int index = 0; index < grad.Length; index++)
                grad[index] = gradCopy[index];
        }
        public static void function2_grad(double[] x, ref double func, double[] grad, object obj)
        {
            // this callback calculates xStar
            // and its derivatives 
            Individual toBeOptimized = new Individual(x, "", 0);
            func = xStarFunctionValue(toBeOptimized, population);
            double[] gradCopy = xStarGradientValue(toBeOptimized, population);
            for (int index = 0; index < grad.Length; index++)
                grad[index] = gradCopy[index];
        }
        private static double[] xStarGradientValue(Individual xStar2, List<Individual> population)
        {
            Individual result = new Individual(tetha * (double)(-population.Count));
           
            for (int index = 1; index < population.Count; index++)
            {
                double vValue = (new Individual(xStar2 - population[index])).DotProduct(tetha.Weights);
                double gOfXY = (Math.Exp(vValue) - Math.Exp(-vValue)/(Math.Exp(vValue) + Math.Exp(-vValue)));
                Individual newIndi = new Individual(tetha * gOfXY);
                result = new Individual(result + newIndi);
            }
            return result.Weights;
        }
        private static double xStarFunctionValue(Individual xStar2, List<Individual> population)
        {
            Individual result = new Individual(xStar2 * (double)population.Count);
            Individual sum = new Individual(new double[numberOfFeatures]);
            double gOfXY = 0;
            for (int index = 1; index < population.Count; index++)
            {
                sum = new Individual(sum + population[index]);
                Individual newIndiv = new Individual(xStar2 - population[index]);
                double localValue = newIndiv.DotProduct(tetha.Weights);
                gOfXY += Math.Log(Math.Exp(localValue) + Math.Exp(-localValue));
            }
            result = new Individual(sum - result);
            return result.DotProduct(tetha.Weights) + gOfXY;
        }
        private static double tethaFunctionValue(Individual tetha2, List<Individual> population)
        {
            double result = 0;
            result += Math.Pow(tetha2.EuclideanNorm(),2) / 2 * Math.Pow(sigma, 2);

            for (int row = 0; row < population.Count; row++)
            {
                for (int col = row; col < population.Count; col++)
                {
                    if (row == col) continue;
                    Individual newIndiv = new Individual(population[row] - population[col]);
                    double localValue = newIndiv.DotProduct(tetha2.Weights);
                    double gOfXY = Math.Log(Math.Exp(localValue) + Math.Exp(-localValue));
                    localValue = localValue * (double)(numberOfGames - 2 * population[row].Labels[col]) + (double)numberOfGames * gOfXY;
                    result += localValue;

                }
            }
            return result;

        }
        private static double[] tethaGradientValue(Individual tetha2, List<Individual> population)
        {
            Individual result = new Individual(tetha2 / Math.Pow(sigma, 2)); ;
            for (int row = 0; row < population.Count; row++)
            {
                for (int col = row; col < population.Count; col++)
                {
                    if (row == col) continue;
                    Individual secondTerm = new Individual(new Individual(population[row] - population[col]) * (double)(numberOfGames - 2 * population[row].Labels[col]));
                    result = new Individual(result + secondTerm);
                    Individual difference = new Individual(population[row] - population[col]);
                    double vValue = difference.DotProduct(tetha2.Weights);
                    Individual gOfXY = new Individual(difference * ((Math.Exp(vValue) - Math.Exp(-vValue)) / (Math.Exp(vValue) + Math.Exp(-vValue))));
                    result = new Individual(result + gOfXY);

                }
            }
            return result.Weights;

        }
        private static List<Individual> initializePopulation()
        {
            double[] randomWeights = new double[numberOfFeatures];    
            List<Individual> players = new List<Individual>();
            players.Add(xStar);
            for (int counter = 0; counter < initialPopulationSize; counter++)
            {
                randomWeights = new double[numberOfFeatures];
                for (int index = 0; index < randomWeights.Length; index++)
                {

                    randomWeights[index] = rand.NextDouble();
                }
                players.Add(new Individual(randomWeights, string.Empty, initialPopulationSize + 1));
            }
            return players;
        }
        private static int calculateLabel(Individual[] weights)
        {

            Individual[] reversedWeights = { weights[1], weights[0] };
            weights[1].ResetScores();
            weights[0].ResetScores();
            SmartThreadPool smtp = new SmartThreadPool();
            for (int index = 0; index < numberOfGames; index++)
            {

                if (index % 2 == 1)
                {
                    smtp.QueueWorkItem(new WorkItemCallback(threadProc), weights);
                }
                else
                {
                    smtp.QueueWorkItem(new WorkItemCallback(threadProc), reversedWeights);
                }

            }
            smtp.WaitForIdle();
            smtp.Shutdown();
            //double strengthRatio = ((double)weights[0].Wins + (double)weights[0].Draws / 2) / numberOfGames;
            //return strengthRatio > 0.5 ? 1 : -1;
            return weights[0].Wins + 1/2 * weights[0].Draws;
        }
        private static object threadProc(Object data)
        {
            Individual[] weights = data as Individual[];
            Algorithms alg = new Algorithms(new CheckersPlayer(1));
            CheckersBoard whiteBoard = new CheckersBoard(0, weights[0].Weights);
            CheckersBoard blackBoard = new CheckersBoard(0, weights[1].Weights);
            whiteBoard.GameMaxLength = 150;
            blackBoard.GameMaxLength = 150;
            while (!blackBoard.GameIsOver())
            {
                CheckersMove move = alg.ABNegamax(blackBoard, weights[1].SearchDepth, 0, double.MinValue, double.MaxValue) as CheckersMove;
                whiteBoard.MakeMove(move);
                blackBoard.MakeMove(move);
                if (blackBoard.GameIsOver())
                    break;
                else
                {
                    move = alg.ABNegamax(whiteBoard, weights[0].SearchDepth, 0, double.MinValue, double.MaxValue) as CheckersMove;
                    whiteBoard.MakeMove(move);
                    blackBoard.MakeMove(move);
                }
            }
            switch (whiteBoard.Winner)
            {
                case 0:
                    weights[0].Draws += 1;
                    weights[1].Draws += 1;
                    break;
                case 1:
                    weights[0].TotalWhiteWins += 1;
                    weights[1].Loses += 1;
                    break;
                case -1:
                    weights[1].TotalBlackWins += 1;
                    weights[0].Loses += 1;
                    break;
            }
            return weights as object;
        }
    }
}
