using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GameAI;
namespace ReversiGame
{
    public class ReversiBoard : IBoardGame
    {
        private int maxScore = int.MaxValue - 64;
        private static readonly int Black = -1;
        private static readonly int White = 1;
        private static readonly int Empty = 0;
        private int noMovesWeight = 35;
        private int stabilityWeight = 70;
        private int frontierWeight = 10;    
        private int mobilityWeight = 5;
        private ReversiMove lastMove;
        private bool gameOver = false;
        public delegate void HasMoves(bool lastPlayerHadMoves);
        public delegate void EndGame();
        public delegate void BoardMove(int row , int col , int playerID);
        public event BoardMove MoveComplete;
        public event HasMoves NoLegalMoveAvailable;
        public event EndGame GameOver;
        public bool previousPlayerHadMoves = true;
        private int whiteCount;
        private int blackCount;
        private int emptyCount;
        private int whiteFrotierCount;
        private int blackFrontierCount;
        private int blackSafeDiscCount;
        private int whiteSafeDiscCount;
        private int validMovesCount;
        private int [,] gameBoard;
        private bool[,] validMoves;
        private bool[,] safeDisc = new bool[8,8];
        ReversiPlayer[] players;
        int currentPlayer;
        #region Constructors
        public ReversiBoard (int currentPlayer)
        {
            if (currentPlayer != 0 && currentPlayer != 1) throw new ArgumentOutOfRangeException();
            this.gameBoard = new int[8, 8];
            this.gameBoard[4, 4] = ReversiBoard.White;
            this.gameBoard[3, 4] = ReversiBoard.Black;
            this.gameBoard[4, 3] = ReversiBoard.Black;
            this.gameBoard[3, 3] = ReversiBoard.White;
            this.players = new ReversiPlayer[2];
            this.players[0] = new ReversiPlayer(1);
            this.players[1] = new ReversiPlayer(-1);
            this.currentPlayer = currentPlayer;
            this.blackCount = 2;
            this.whiteCount = 2;
            this.emptyCount = 60;
            updateValidMoves(true);
            GameOver += new EndGame(calculateWinner);
          
        }
        public ReversiBoard(ReversiBoard board, bool isVisualBoard)
        {
            this.gameBoard = new int[8, 8];
            for (int i = 0; i < 8; ++i)
                for (int j = 0; j < 8; ++j)
                {
                    this.gameBoard[i, j] = board.gameBoard[i, j];
                    this.safeDisc[i, j] = board.safeDisc[i, j];
                }
            this.currentPlayer = board.currentPlayer;
            this.blackCount = board.blackCount;
            this.whiteCount = board.whiteCount;
            this.emptyCount = board.emptyCount;
            GameOver += new EndGame(calculateWinner);
            this.players = board.players;
            this.validMoves = new bool[8, 8];
            if (isVisualBoard)
            {
                this.MoveComplete = board.MoveComplete;
                this.NoLegalMoveAvailable = board.NoLegalMoveAvailable;
                this.GameOver += board.GameOver;
            }
            updateValidMoves(board.previousPlayerHadMoves);
            updateCounters();
        }
        #endregion
        #region Private Members
        private void calculateWinner()
        {
            gameOver = true;
            updateCounters();
            if (blackCount > whiteCount)  Winner = players[1].GetPlayerID();
            else if (blackCount < whiteCount) Winner = players[0].GetPlayerID();
            else Winner = 0;
        }
        private int getNextPlayer()
        {
            return Math.Abs(currentPlayer - 1);
        }
        
        private void updateCounters()
        {
            whiteCount = 0;
            blackCount = 0;
            emptyCount = 0;
            whiteFrotierCount = 0;
            blackFrontierCount = 0;
            blackSafeDiscCount = 0;
            whiteSafeDiscCount = 0;
            bool newSafeDisc = true;  
            while (newSafeDisc)
            {
                newSafeDisc = false;

                for (int row = 0; row < 8; ++row)
                    for (int col = 0; col < 8; ++col)
                        if (!safeDisc[row, col] && gameBoard[row, col] != ReversiBoard.Empty && !isFlankable(row, col))
                        {
                            safeDisc[row, col] = true;
                            newSafeDisc = true;
                           
                        }

                     
                    

            }
          
            for(int row =0; row < 8; ++row)
                for (int col = 0; col < 8; ++col)
                {
                    bool isFrontier = false;
                    if (gameBoard[row, col] != ReversiBoard.Empty)
                        for (int rowDirection = -1; rowDirection <= 1; rowDirection++)
                            for (int colDirection = -1; colDirection <= 1; colDirection++)
                                if (!(rowDirection == 0 && colDirection == 0)
                                     && row + rowDirection < 8
                                     && col + colDirection < 8
                                     && row + rowDirection >= 0
                                     && col + colDirection >= 0
                                     && gameBoard[row+ rowDirection, col+colDirection] == ReversiBoard.Empty)
                                    isFrontier = true;
                    switch (gameBoard[row, col])
                    {
                        case 0: ++emptyCount; break;
                        case 1: ++whiteCount; if (isFrontier) whiteFrotierCount++;if(safeDisc[row,col]) whiteSafeDiscCount++ ; break;
                        case -1: ++blackCount; if (isFrontier) blackFrontierCount++; if (safeDisc[row, col]) blackSafeDiscCount++ ; break;
                    }
                
                
                }
         
        }

        private bool isFlankable(int row, int col)
        {          
            int color = this.gameBoard[row, col]; 
            int rowDirection, colDirection;
            bool hasEmptySide1, hasEmptySide2;
            bool hasUnsafeSide1, hasUnsafeSide2;
            hasEmptySide1 = false;
            hasUnsafeSide1 = false;
            hasEmptySide2 = false;
            hasUnsafeSide2 = false;         
            for (colDirection = 0; colDirection < col && !hasEmptySide1; colDirection++)
                if (this.gameBoard[row, colDirection] == ReversiBoard.Empty)
                    hasEmptySide1 = true;
                else if (this.gameBoard[row, colDirection] != color || !this.safeDisc[row, colDirection])
                    hasUnsafeSide1 = true;
           
            for (colDirection = col + 1; colDirection < 8 && !hasEmptySide2; colDirection++)
                if (this.gameBoard[row, colDirection] == ReversiBoard.Empty)
                    hasEmptySide2 = true;
                else if (this.gameBoard[row, colDirection] != color || !this.safeDisc[row, colDirection])
                    hasUnsafeSide2 = true;
            if ((hasEmptySide1 && hasEmptySide2) ||
                (hasEmptySide1 && hasUnsafeSide2) ||
                (hasUnsafeSide1 && hasEmptySide2))
                return true;

            
            hasEmptySide1 = false;
            hasEmptySide2 = false;
            hasUnsafeSide1 = false;
            hasUnsafeSide2 = false;
          
            for (rowDirection = 0; rowDirection < row && !hasEmptySide1; rowDirection++)
                if (this.gameBoard[rowDirection, col] == ReversiBoard.Empty)
                    hasEmptySide1 = true;
                else if (this.gameBoard[rowDirection, col] != color || !this.safeDisc[rowDirection, col])
                    hasUnsafeSide1 = true;
            
            for (rowDirection = row + 1; rowDirection < 8 && !hasEmptySide2; rowDirection++)
                if (this.gameBoard[rowDirection, col] == ReversiBoard.Empty)
                    hasEmptySide2 = true;
                else if (this.gameBoard[rowDirection, col] != color || !this.safeDisc[rowDirection, col])
                    hasUnsafeSide2 = true;
            if ((hasEmptySide1 && hasEmptySide2) ||
                (hasEmptySide1 && hasUnsafeSide2) ||
                (hasUnsafeSide1 && hasEmptySide2))
                return true;

           
            hasEmptySide1 = false;
            hasEmptySide2 = false;
            hasUnsafeSide1 = false;
            hasUnsafeSide2 = false;
           
            rowDirection = row - 1;
            colDirection = col - 1;
            while (rowDirection >= 0 && colDirection >= 0 && !hasEmptySide1)
            {
                if (this.gameBoard[rowDirection, colDirection] == ReversiBoard.Empty)
                    hasEmptySide1 = true;
                else if (this.gameBoard[rowDirection, colDirection] != color || !this.safeDisc[rowDirection, colDirection])
                    hasUnsafeSide1 = true;
                rowDirection--;
                colDirection--;
            }
            
            rowDirection = row + 1;
            colDirection = col + 1;
            while (rowDirection < 8 && colDirection < 8 && !hasEmptySide2)
            {
                if (this.gameBoard[rowDirection, colDirection] == ReversiBoard.Empty)
                    hasEmptySide2 = true;
                else if (this.gameBoard[rowDirection, colDirection] != color || !this.safeDisc[rowDirection, colDirection])
                    hasUnsafeSide2 = true;
                rowDirection++;
                colDirection++;
            }
            if ((hasEmptySide1 && hasEmptySide2) ||
                (hasEmptySide1 && hasUnsafeSide2) ||
                (hasUnsafeSide1 && hasEmptySide2))
                return true;

            
            hasEmptySide1 = false;
            hasEmptySide2 = false;
            hasUnsafeSide1 = false;
            hasUnsafeSide2 = false;
           
            rowDirection = row - 1;
            colDirection = col + 1;
            while (rowDirection >= 0 && colDirection < 8 && !hasEmptySide1)
            {
                if (this.gameBoard[rowDirection, colDirection] == ReversiBoard.Empty)
                    hasEmptySide1 = true;
                else if (this.gameBoard[rowDirection, colDirection] != color || !this.safeDisc[rowDirection, colDirection])
                    hasUnsafeSide1 = true;
                rowDirection--;
                colDirection++;
            }
           
            rowDirection = row + 1;
            colDirection = col - 1;
            while (rowDirection < 8 && colDirection >= 0 && !hasEmptySide2)
            {
                if (this.gameBoard[rowDirection, colDirection] == ReversiBoard.Empty)
                    hasEmptySide2 = true;
                else if (this.gameBoard[rowDirection, colDirection] != color || !this.safeDisc[rowDirection, colDirection])
                    hasUnsafeSide2 = true;
                rowDirection++;
                colDirection--;
            }
            if ((hasEmptySide1 && hasEmptySide2) ||
                (hasEmptySide1 && hasUnsafeSide2) ||
                (hasUnsafeSide1 && hasEmptySide2))
                return true;
            return false;
        }
        private bool updateValidMoves(bool lastPlayerHadMoves)
        {
            if (whiteCount + blackCount == 64 && lastPlayerHadMoves)
            {
                if (GameOver != null)
                    GameOver.Invoke();
                return false;
            }
            validMoves = new bool[8, 8];
            bool hasMoves = false;
            validMovesCount = 0;
            for (int row = 0; row < 8; ++row)
                for (int col = 0; col < 8; ++col)
                {
                    validMoves[row, col] = isValidMove(row, col);
                    if (validMoves[row, col] == true)
                    {
                        hasMoves = true;
                        ++validMovesCount;
                    }
                }

            if (!hasMoves)
            {
                if (!lastPlayerHadMoves)
                {
                    if (GameOver != null) 
                        GameOver.Invoke();
                }
                else
                {
                    previousPlayerHadMoves = false;
                    ChangePlayer();
                    updateCounters();
                    updateValidMoves(previousPlayerHadMoves);
                    if (this.NoLegalMoveAvailable != null && !gameOver)
                    {
                        if (this.MoveComplete != null)
                            this.MoveComplete.Invoke(lastMove.Row, lastMove.Column, lastMove.PlayerID);
                        this.NoLegalMoveAvailable.Invoke(false);
                    }

                }
            }
            return hasMoves;
            
        }


        private bool isValidMove(int row, int col)
        {
            if (gameBoard[row, col] != ReversiBoard.Empty) return false;
            for (int rowDirection = -1; rowDirection <= 1; ++rowDirection)
                for (int colDirection = -1; colDirection <= 1; ++colDirection)
                    if (!(rowDirection == 0 && colDirection == 0) && isFlanking(row, col, rowDirection, colDirection))
                        return true;

            return false;
        }

        private bool isFlanking(int row, int col, int rowDirection, int colDirection)
        {
            int nextPlayer = this.getNextPlayer();
            int rowIndex = row + rowDirection;
            int colIndex = col + colDirection;
            while (rowIndex >= 0 && rowIndex < 8 && colIndex >= 0 && colIndex < 8 && gameBoard[rowIndex, colIndex] == players[nextPlayer].GetPlayerID())
            {
                rowIndex += rowDirection;
                colIndex += colDirection;
            }
            if (rowIndex < 0 || rowIndex > 7 || colIndex > 7 || colIndex < 0 || (rowIndex - rowDirection == row && colIndex - colDirection == col) || gameBoard[rowIndex, colIndex] != players[currentPlayer].GetPlayerID())
                return false;
            return true;
        }
        #endregion
        
        #region Public Members
        #region Getters and Setters
        public int GetPieceColor(int row, int col)
        {
            return gameBoard[row, col];
        }
        public bool GetIsSafe(int row, int col)
        {
            return safeDisc[row, col];
        }
        public int GetCurrentPlayerID()
        {
            return players[currentPlayer].GetPlayerID();
        }
        public int GetBlackSafeDiscCount
        {
            get { return blackSafeDiscCount; }

        }
        public int WhiteSquares
        {
            get { return whiteCount; }
        }
        public int BlackSquares
        {
            get { return blackCount; }
        }
        public int Winner { get; private set; }
        #endregion
        public void ChangePlayer()
        {
            currentPlayer = Math.Abs(currentPlayer - 1);
        }
        public bool IsValidMove(int row, int col)
        {
            return validMoves[row, col];

        }
 
     
        #region IBoardGame Members
       
        public List<IMove> GetLegalMoves()
        {
            List<IMove> list = new List<IMove>();
            updateValidMoves(true);
            for (int row = 0; row < 8; ++row)
                for (int col = 0; col < 8; ++col)
                {
                    if (validMoves[row, col]) list.Add(new ReversiMove(row, col, players[currentPlayer].GetPlayerID(), 0));

                }
            return list;
        }

        public List<IMove> GetLegalMoves(IPlayer player)
        {
            List<IMove> list = new List<IMove>();
            for(int row =0;row<7; ++row)
               for (int col = 0; col < 7; ++col)
               {
                   if (validMoves[row, col])
                   {
                       ReversiMove move = new ReversiMove(row, col, players[currentPlayer].GetPlayerID(),0);
                       list.Add(move);
                   }
                    
               
               }
           return list;
        }
        public void MakeMove(IMove move)
        {
            ReversiMove currentMove = move as ReversiMove;
            gameBoard[currentMove.Row, currentMove.Column] = players[currentPlayer].GetPlayerID();
            for (int rowDirection = -1; rowDirection <= 1; ++rowDirection)
                for (int colDirection = -1; colDirection <= 1; ++colDirection)
                    if (!(rowDirection == 0 && colDirection == 0) && isFlanking(currentMove.Row, currentMove.Column, rowDirection, colDirection))
                    {
                        int rowIndex = currentMove.Row + rowDirection;
                        int colIndex = currentMove.Column + colDirection;
                        while (gameBoard[rowIndex, colIndex] != players[currentPlayer].GetPlayerID())
                        {
                            gameBoard[rowIndex, colIndex] = players[currentPlayer].GetPlayerID();
                            rowIndex += rowDirection;
                            colIndex += colDirection;

                        }


                    }

            ChangePlayer();     
            updateCounters();
            lastMove = currentMove;
            if (this.MoveComplete != null && updateValidMoves(true))
                MoveComplete.Invoke(currentMove.Row,currentMove.Column,currentMove.PlayerID);
            
        
        }
        public IBoardGame TestMove(IMove move)
        {
            ReversiBoard board = new ReversiBoard(this , false);
            ReversiMove currentMove = move as ReversiMove;
            board.gameBoard[currentMove.Row, currentMove.Column] = players[currentPlayer].GetPlayerID();
            for (int rowDirection = -1; rowDirection <= 1; ++rowDirection)
                for (int colDirection = -1; colDirection <= 1; ++colDirection)
                    if (!(rowDirection == 0 && colDirection == 0) && isFlanking(currentMove.Row, currentMove.Column, rowDirection, colDirection))
                    {
                        int rowIndex = currentMove.Row + rowDirection;
                        int colIndex = currentMove.Column + colDirection;
                        while (board.gameBoard[rowIndex, colIndex] != players[currentPlayer].GetPlayerID())
                        {
                            board.gameBoard[rowIndex, colIndex] = players[currentPlayer].GetPlayerID();
                            rowIndex += rowDirection;
                            colIndex += colDirection;

                        }


                    }

            board.ChangePlayer();
            board.updateCounters();
            board.updateValidMoves(true);
            return board as IBoardGame;
        }
       
        public int EvaluatePosition(IBoardGame lastBoardPosition)
        {
            int score = whiteCount - blackCount;
            int nextColor = getNextPlayer();
            int noMoves = 0;
            ReversiBoard rev = lastBoardPosition as ReversiBoard;
            int validMoveCount = rev.validMovesCount;
            bool endGame = false;       
            int opponentValidMoveCount = this.validMovesCount;
            if (opponentValidMoveCount == 0) 
            {
                noMoves = players[currentPlayer].GetPlayerID();
                
                if (this.validMovesCount == 0)
                    endGame = true;
            
            }
            if (endGame)
            {
                if (score < 0)
                    return -maxScore + score;
                if (score > 0)
                    return maxScore + score;
                else return 0;
            }


            else
            {
                int finalScore = noMoves * noMovesWeight +
                    score +
                    frontierWeight * (blackFrontierCount - whiteFrotierCount) +
                    mobilityWeight * rev.players[rev.currentPlayer].GetPlayerID()* (validMoveCount - opponentValidMoveCount) +
                    stabilityWeight * (whiteSafeDiscCount - blackSafeDiscCount);
                    
                return finalScore;
            }
        }
        public IMove EvaluatePosition()
        {
            throw new NotImplementedException();
        }

        public IPlayer GetCurrentPlayer()
        {
            return players[currentPlayer];
        }
        public bool GameIsOver()
        {

            return gameOver;
        }

        #endregion
        #endregion
    }
}
