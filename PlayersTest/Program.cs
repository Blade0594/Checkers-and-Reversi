using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CheckersGame;
using System.IO;
using GameAI;
using Amib.Threading;
namespace PlayersTest
{
    class Program
    {
        static Random rand = new Random(DateTime.Now.Millisecond);
        static void Main(string[] args)
        {
            double[] naive = new double[11];
            Individual Naive = new Individual(naive, "Naive");
            Naive.SearchDepth = 2;
            double[] optimal = { 1, 1.5, 0.304941560283742, 0.398957574925831, 0.808534060050051,
                                 0.0123464008850727, 0.947831550123092, 0.983976080074895, 0.157787902819825,
                                 0.999516903422548, 0.164692896960626 };
            double[] simple = new double[11];
            simple[0] = 1;
            simple[1] = 1.5;
            Individual Simple = new Individual(simple, "Simple");
            Simple.SearchDepth = 2;
            Individual MiniMax = new Individual(simple, "MiniMax");
            MiniMax.SearchDepth = 8;
            Individual Optimal = new Individual(optimal, "Optimal");
            Optimal.SearchDepth = 7;
            Individual Adaptive = new Individual(optimal, "Adaptive");
            Adaptive.SearchDepth = 8;
            int minimaxBaseSearchDepth = 8;
            int[] generatedDepths = new int[100];
            //for (int index = 0; index < 100; index++)
            //{
            //    if (rand.NextDouble() < 0.075)
            //    {
            //        if (MiniMax.SearchDepth < 8)
            //            MiniMax.SearchDepth++;


            //    }
            //    FileStream fileSearchDepth = new FileStream("E:\\Scoala\\Minimax" + minimaxBaseSearchDepth.ToString() + "\\SearchDepths.txt",
            //                           FileMode.Append, FileAccess.Write);
            //    StreamWriter wr = new StreamWriter(fileSearchDepth);
            //    int trueDepth = MiniMax.SearchDepth;
            //    if (rand.NextDouble() < 0.1)
            //    {
            //        double random = rand.NextDouble();
            //        if (random < 0.5 && MiniMax.SearchDepth < 8)
            //            MiniMax.SearchDepth++;
            //        else if (random > 0.5 && MiniMax.SearchDepth > 2)
            //            MiniMax.SearchDepth--;
            //    }
            //    wr.Write(" " + MiniMax.SearchDepth.ToString());
            //    wr.Close();
            //    fileSearchDepth.Close();
            //    generatedDepths[index] = MiniMax.SearchDepth;
            //    MiniMax.SearchDepth = trueDepth;
            
            //}

            while (minimaxBaseSearchDepth >= 2)
            {
                GameAI.Algorithms alg = new Algorithms(new CheckersPlayer(1));
                DateTime time = DateTime.Now;
                for (int i = 0; i < 100; i++)
                {


                    MiniMax.SearchDepth = minimaxBaseSearchDepth; 
                    Individual[] weights = { Adaptive, MiniMax };
                    ThreadProc(weights, "E:\\Scoala\\Minimax" + minimaxBaseSearchDepth.ToString() + "\\Game" + i.ToString() + ".txt");//, alg);
                   
                }

                TimeSpan span = new TimeSpan();
                span = DateTime.Now - time;
                FileStream file = new FileStream(@"E:\Minimax" + minimaxBaseSearchDepth.ToString() + "vsAdaptive.txt", FileMode.Create, FileAccess.Write);
                StreamWriter writer = new StreamWriter(file);
                writer.WriteLine(MiniMax.ToString());
                writer.WriteLine(Adaptive.ToString());
                writer.WriteLine("Time for simulation " + span.ToString());
                writer.Flush();
                writer.Close();
                file.Close();
                minimaxBaseSearchDepth--;
                MiniMax.SearchDepth = minimaxBaseSearchDepth;
                MiniMax.TotalBlackWins = 0;
                MiniMax.TotalWhiteWins = 0;
                MiniMax.Draws = 0;
                MiniMax.Loses = 0;
                Adaptive.TotalWhiteWins = 0;
                Adaptive.TotalBlackWins = 0;
                Adaptive.Draws = 0;
                Adaptive.Loses = 0;
            }
        }
        static object ThreadProc(Object data, string path)//, GameAI.Algorithms alg)
        {
            Individual[] weights = data as Individual[];
            GameAI.Algorithms alg = new Algorithms(new CheckersPlayer(1));
            CheckersBoard whiteBoard = new CheckersBoard(0, weights[0].Weights);
            CheckersBoard blackBoard = new CheckersBoard(0, weights[1].Weights);
            whiteBoard.GameMaxLength = 200;
            blackBoard.GameMaxLength = 200;
            while (!blackBoard.GameIsOver())
            {
                CheckersMove move = alg.ABNegamax(blackBoard, weights[1].SearchDepth, 0, double.MinValue, double.MaxValue) as CheckersMove;
                whiteBoard.MakeMove(move);
                blackBoard.MakeMove(move);
                if (blackBoard.GameIsOver())
                    break;
                else
                {

                    //move = alg.ABNegamax(whiteBoard, weights[0].SearchDepth, 0, double.MinValue, double.MaxValue) as CheckersMove;
                    move = alg.POSM(whiteBoard, 0.5) as CheckersMove;
                    // writer.WriteLine(move.GetMoveScore().ToString());
                    whiteBoard.MakeMove(move);
                    blackBoard.MakeMove(move);
                }
            }
            FileStream file = new FileStream(path, FileMode.Append, FileAccess.Write);
            StreamWriter wr = new StreamWriter(file);
            switch (whiteBoard.Winner)
            {
                case 0:
                    weights[0].Draws += 1;
                    weights[1].Draws += 1;
                    wr.WriteLine("Draw");
                    wr.WriteLine(alg.ToString());
                    break;
                case 1:
                    weights[0].TotalWhiteWins += 1;
                    weights[1].Loses += 1;
                    wr.WriteLine("Win for Adaptive");
                    alg.GameObervationUpdate(1);
                    wr.WriteLine(alg.ToString());
                    break;
                case -1:
                    weights[1].TotalBlackWins += 1;
                    weights[0].Loses += 1;
                    wr.WriteLine("Los for Adaptive");
                    alg.GameObervationUpdate(-1);
                    wr.WriteLine(alg.ToString());
                    break;
            }
            wr.Close();
            file.Close();
            return weights as object;
        }
        static object ThreadProcAdaptive(Object data)
        {

            Individual[] weights = data as Individual[];
            GameAI.Algorithms alg = new Algorithms(new CheckersPlayer(1));
            CheckersBoard whiteBoard = new CheckersBoard(0, weights[0].Weights);
            CheckersBoard blackScore = new CheckersBoard(0, weights[0].Weights);
            CheckersBoard blackBoard = new CheckersBoard(0, weights[1].Weights);
            whiteBoard.GameMaxLength = 200;
            blackBoard.GameMaxLength = 200;
            while (!blackBoard.GameIsOver())
            {
                CheckersMove move = alg.ABNegamax(blackBoard, weights[1].SearchDepth, 0, double.MinValue, double.MaxValue) as CheckersMove;
                List<IMove> opponentsMoves = alg.EvaluateMoves(blackScore, weights[0].SearchDepth, 0, double.MinValue, double.MaxValue);
                whiteBoard.MakeMove(move);
                blackScore.MakeMove(move);
                blackBoard.MakeMove(move);
                if (blackBoard.GameIsOver())
                    break;
                else
                {

                    move = alg.AdaptiveABNegamax(whiteBoard, weights[0].SearchDepth, 0, double.MinValue, double.MaxValue, opponentsMoves, move) as CheckersMove;
                    whiteBoard.MakeMove(move);
                    blackScore.MakeMove(move);
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
