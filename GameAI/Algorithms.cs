using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;

namespace GameAI
{
    public class Algorithms
    {

        double[] difficultyWeights = new double[7];
        double[] stepLearningRateExponent = new double[7];
        double gameLearningFactor;
        bool isFirstTurn = true;
        int difficulty = 6;
        int difficultySum = 0;
        int numberOfMoves = 0;
        public List<POSMStatistics> Statistics {get; private set; }
        double currentObservationScore = 0;
        int computerPlayerID;
        System.Random random;
        int observation = 1;
        //Note: only used when using AdaptiveABNegamax3
        double averageRank = -1;
        private void rescaleExponents()
        {
            double minStepExponent = int.MaxValue;
          
            for (int index = 0; index < this.stepLearningRateExponent.Length; index++)
            {
                if (minStepExponent > stepLearningRateExponent[index])
                    minStepExponent = stepLearningRateExponent[index];
              
            }
            for (int index = 0; index < this.stepLearningRateExponent.Length; index++)
            {
               
                stepLearningRateExponent[index] -= minStepExponent;
            }

        }
        private void countDifficulty(int diff)
        {
            this.difficultySum += diff;
            this.numberOfMoves += 1;
            POSMStatistics newStat = new POSMStatistics();
            newStat.Observation = this.currentObservationScore;
            newStat.Prediction = diff;
            this.Statistics.Add(newStat);
            

        }
        private void resetDifficultyCounter()
        {
            this.difficultySum = 0;
            this.numberOfMoves = 0;
        }
        public Algorithms(IPlayer player, double gameLearningFactor = 0)
        {
            computerPlayerID = player.GetPlayerID();
            this.Statistics = new List<POSMStatistics>();
            random = new Random(DateTime.Now.Millisecond);
            for (int index = 0; index < difficultyWeights.Length; index++)
            {
                this.difficultyWeights[index] = 1;
                this.stepLearningRateExponent[index] = 0;
             
            }
            this.gameLearningFactor = gameLearningFactor;
        }
        /// <summary>
        /// The Minimax algortihm for the best move.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="maxDepth"></param>
        /// <param name="alfa"></param>
        /// <param name="beta"></param>
        /// <returns></returns>
        public void GameObervationUpdate(int gameResult)
        {
            int averageDiff = this.difficultySum / this.numberOfMoves;
            if (gameResult > 0)
            {
                for (int index = averageDiff; index < difficultyWeights.Length; index++)
                    this.stepLearningRateExponent[index] += gameLearningFactor;

            }
            else if (gameResult < 0)
            {
                for (int index = averageDiff; index >= 0; index--)
                    this.stepLearningRateExponent[index] += gameLearningFactor;

            }
            rescaleExponents();
            this.resetDifficultyCounter();
        }
        public IMove MiniMax(IBoardGame game, int maxDepth, double alfa, double beta)
        {
            if (maxDepth == 0)
                return null;
            IMove bestMove = null;
            var moves = (new RandomList<IMove>(game.GetLegalMoves())).Randomize();
            int color = game.GetCurrentPlayer().GetPlayerID();
            foreach (var validMove in moves)
            {


                IBoardGame testGame = game.TestMove(validMove);
                validMove.SetMoveScore(testGame.EvaluatePosition(game));
                if (bestMove == null) bestMove = validMove;
                if (!testGame.GameIsOver())
                {
                    IMove nextBestMove = MiniMax(testGame, maxDepth - 1, alfa, beta);
                    if (nextBestMove != null) validMove.SetMoveScore(nextBestMove.GetMoveScore());
                    if (color == 1 && validMove.GetMoveScore() > beta)
                    {
                        beta = validMove.GetMoveScore();
                    }
                    if (color == -1 && validMove.GetMoveScore() < alfa)
                    {
                        alfa = validMove.GetMoveScore();

                    }
                }
                if (color == 1 && validMove.GetMoveScore() > alfa)
                {
                    validMove.SetMoveScore(alfa);
                    return validMove;
                }
                if (color == -1 && validMove.GetMoveScore() < beta)
                {
                    validMove.SetMoveScore(beta);
                    return validMove;

                }
                if (color * validMove.GetMoveScore() > color * bestMove.GetMoveScore())
                    bestMove = validMove;

            }

            return bestMove;

        }
        public IMove ABNegamax(IBoardGame game, int maxDepth, int currentDepth, double alfa, double beta)
        {
            if (maxDepth == currentDepth || game.GameIsOver())
                return game.EvaluatePosition();
            IMove bestMove = null;
            var moves = (new RandomList<IMove>(game.GetLegalMoves())).Randomize();
            foreach (var validMove in moves)
            {
                if (bestMove == null) bestMove = validMove;
                IBoardGame testGame = game.TestMove(validMove);
                validMove.SetMoveScore(-(ABNegamax(testGame, maxDepth, currentDepth + 1, -beta, -Math.Max(alfa, bestMove.GetMoveScore())).GetMoveScore()));
                if (validMove.GetMoveScore() > bestMove.GetMoveScore())
                {
                    bestMove = validMove;
                }
                if (bestMove.GetMoveScore() >= beta)
                    return bestMove;

            }
            return bestMove;
        }
        public List<IMove> EvaluateMoves(IBoardGame game, int maxDepth, int currentDepth, double alfa, double beta)
        {

            List<IMove> rankedMoveList = new List<IMove>();
            var moves = (new RandomList<IMove>(game.GetLegalMoves())).Randomize();
            foreach (var validMove in moves)
            {
                IBoardGame testGame = game.TestMove(validMove);
                validMove.SetMoveScore(-(ABNegamax(testGame, maxDepth, currentDepth + 1, -beta, -alfa)).GetMoveScore());
                int moveRank = 0;
                foreach (var insertedMove in rankedMoveList)
                {
                    if (validMove.GetMoveScore() < insertedMove.GetMoveScore())
                        moveRank++;
                    else break;
                }
                rankedMoveList.Insert(moveRank, validMove);
                for (int index = 0; index < rankedMoveList.Count; index++)
                {
                    rankedMoveList[index].SetRank(index);
                }

            }

            return rankedMoveList;

        }
        public IMove AdaptiveABNegamax(IBoardGame game, int maxDepth, int currentDepth, double alfa, double beta, List<IMove> opponentsList, IMove opponentsMove)
        {
            List<IMove> rankedMoveList = EvaluateMoves(game, maxDepth, 0, alfa, beta);
            int rank = 0;
            for (int index = 0; index < opponentsList.Count; index++)
            {

                if (!opponentsList[index].IsEqual(opponentsMove)) continue;
                else
                {
                    rank = index;
                    break;

                }
            }
            if (rank < rankedMoveList.Count)
            {
                while (rank >= 0 && rankedMoveList[rank].GetMoveScore() < 0)
                {
                    rank--;
                }
                if (rank >= 0)
                    return rankedMoveList[rank];
                else
                    return rankedMoveList[0];
            }
            else
            {
                rank = rankedMoveList.Count - 1;
                while (rank >= 0 && rankedMoveList[rank].GetMoveScore() < 0)
                {
                    rank--;
                }
                if (rank >= 0)
                    return rankedMoveList[rank];
                else
                    return rankedMoveList[0];
            }
        }
        public IMove AdaptiveABNegamax2(IBoardGame game, int maxDepth, int currentDepth, double alfa, double beta, List<IMove> opponentsList, IMove opponentsMove)
        {
            List<IMove> rankedMoveList = EvaluateMoves(game, maxDepth, 0, alfa, beta);

            double opponentMoveScore = 0;
            for (int index = 0; index < opponentsList.Count; index++)
            {

                if (opponentsList[index].IsEqual(opponentsMove))
                {

                    opponentMoveScore = opponentsList[index].GetMoveScore();
                    break;
                }


            }

            if (opponentMoveScore > rankedMoveList[0].GetMoveScore())
            {
                return rankedMoveList[0];
            }
            double result = double.MaxValue;
            int returnIndex = 0;
            for (int index = 0; index < rankedMoveList.Count; index++)
            {
                if (Math.Abs(rankedMoveList[index].GetMoveScore() - opponentMoveScore) < result)
                {
                    result = Math.Abs(rankedMoveList[index].GetMoveScore() - opponentMoveScore);
                    returnIndex = index;

                }

            }
            return rankedMoveList[returnIndex];
        }
        public IMove AdaptiveABNegamax3(IBoardGame game, int maxDepth, int currentDepth, double alfa, double beta, List<IMove> opponentsList, IMove opponentsMove)
        {
            List<IMove> rankedMoveList = EvaluateMoves(game, maxDepth, 0, alfa, beta);
            int rank = 0;
            for (int index = 0; index < opponentsList.Count; index++)
            {

                if (!opponentsList[index].IsEqual(opponentsMove)) continue;
                else
                {
                    rank = index;
                    break;

                }
            }
            if (averageRank == -1) averageRank = rank;
            else if (opponentsList.Count > 1)
            {
                averageRank = (averageRank + ((double)rank) / opponentsList.Count) / 2;

            }
            double distance = double.MaxValue;
            int returnedRank = 0;
            for (int index = 0; index < rankedMoveList.Count; index++)
                if (distance > Math.Abs(((double)averageRank - (double)index) / rankedMoveList.Count))
                {
                    distance = Math.Abs((double)averageRank - (double)index / (double)rankedMoveList.Count);
                    returnedRank = index;
                }
            if (rankedMoveList[returnedRank].GetMoveScore() < 0)
                for (int index = returnedRank; index < 0; index--)
                    if (rankedMoveList[index].GetMoveScore() < 0)
                        continue;
                    else
                    {
                        returnedRank = index;
                        break;
                    }

            return rankedMoveList[returnedRank];

        }
        public IMove POSM(IBoardGame game, double stepLearningFractor)
        {


            if (isFirstTurn)
            {
                isFirstTurn = false;
                this.countDifficulty(difficulty);
                return ABNegamax(game, difficulty + 2, 0, double.MinValue, double.MaxValue);
            }
           
            //IMove observedMove = ABNegamax(game, difficulty, 0, double.MinValue, double.MaxValue);
            IMove observedMove2 = ABNegamax(game, 8, 0, double.MinValue, double.MaxValue);
           // double observedValue = observedMove.GetMoveScore() - observedMove2.GetMoveScore();
            this.currentObservationScore = observedMove2.GetMoveScore();
            if (Math.Abs(observedMove2.GetMoveScore()) < 1.5)
            {
             
                observation++;
                this.countDifficulty(difficulty);
                return ABNegamax(game, difficulty + 2, 0, double.MinValue, double.MaxValue);
            }
            if (observedMove2.GetMoveScore() > 0)
            {
                for (int index = difficulty; index < difficultyWeights.Length; index++)
                {
                    this.stepLearningRateExponent[index] += 1;
                    difficultyWeights[index] = Math.Pow(stepLearningFractor, stepLearningRateExponent[index]);
                        

                }

            }
            else
            {
                for (int index = difficulty; index >= 0; index--)
                {
                    this.stepLearningRateExponent[index] += 1;
                    difficultyWeights[index] = Math.Pow(stepLearningFractor, stepLearningRateExponent[index]);
                }

            }
            rescaleExponents();
            double[] above = new double[difficultyWeights.Length];
            double[] below = new double[difficultyWeights.Length];
            double sum = 0;
            for (int index = 0; index < difficultyWeights.Length; index++)
            {
                below[index] = sum + difficultyWeights[index];
                sum = below[index];

            }
            sum = 0;
            for (int index = difficultyWeights.Length - 1; index >= 0; index--)
            {
                above[index] = sum + difficultyWeights[index];
                sum = above[index];
            }
            double maxValue = double.MinValue;
            for (int index = 0; index < difficultyWeights.Length; index++)
            {
                if (Math.Min(above[index], below[index]) == below[index])
                {
                    if (below[index] >= maxValue)
                    {
                        maxValue = below[index];
                        difficulty = index;
                    }
                }
                else
                {
                    if (above[index] >= maxValue)
                    {
                        maxValue = above[index];
                        difficulty = index;
                    }

                }

            }
           
            observation++;
            this.countDifficulty(difficulty);
            return ABNegamax(game, difficulty + 2, 0, double.MinValue, double.MaxValue);

        }
        public int ComputerPlayerID
        {
            get { return this.computerPlayerID; }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int index = 0; index < stepLearningRateExponent.Length; index++)
                sb.Append(stepLearningRateExponent[index].ToString() + " ");
            return sb.ToString().Trim();
            
        }
    }
}
