using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using ReversiGame;
using GameAI;
namespace UserInterface
{
    public partial class ReversiWindow : Window
    {
        ReversiBoard board;
        Algorithms alg;
        Random x = new Random();
        Brush sqareBaseColor;
        bool gameOver = false;
        List<ReversiBoard> gameHistory;
        Thread computerThread;
        public static int SearchDepth;
        public static int ComputerPlayer;
        delegate void Repaint(int row, int col,int playerID);
        delegate void GameOver();
        private event GameOver UpdateLabel;
        private Repaint myRepaint;
        public ReversiWindow()
        {
            InitializeComponent();
            winnerLabel.Visibility = Visibility.Hidden;
            gameHistory = new List<ReversiBoard>();
            myRepaint = new Repaint(this.repaint);
            board = new ReversiBoard(0);
            ReversiPlayer player = new ReversiPlayer(ComputerPlayer);
            alg = new Algorithms(player);
            intializeVisualBoard();
            this.UpdateLabel += new ReversiWindow.GameOver(updateWinnerLabel);
            upDateGameStatus(-1, -1, 0);
            board.NoLegalMoveAvailable += new ReversiBoard.HasMoves(board_NoLegalMoveAvailable);
            board.GameOver += new ReversiBoard.EndGame(board_GameOver);
            board.MoveComplete += new ReversiBoard.BoardMove(board_MoveComplete);
            if (board.GetCurrentPlayer().GetPlayerID() == alg.ComputerPlayerID)
            {
                computerThread = new Thread(this.computerMove);
                computerThread.Start();
            }
        }
        private void board_MoveComplete(int row, int col, int playerID)
        {
            Object []args = {row,col,playerID};
            Dispatcher.Invoke(myRepaint, args);
        }
        private void intializeVisualBoard()
        {
            UIBoard.Children.Clear();
            for (int row = 0; row < 8; ++row)
                for (int col = 0; col < 8; ++col)
                {

                    switch (board.GetPieceColor(row, col))
                    {
                        case 0:
                            Square square = new Square();
                            square.SetValue(Grid.ColumnProperty, col);
                            square.SetValue(Grid.RowProperty, row);
                            square.MouseDown += new MouseButtonEventHandler(square_MouseDown);
                            square.Tag = 6;
                            sqareBaseColor = square.rectangle.Fill;
                            if (board.IsValidMove(row, col))
                                square.rectangle.Fill = Brushes.Red;

                            UIBoard.Children.Add(square);
                            break;
                        case 1:
                            WhiteReversiSquare ws = new WhiteReversiSquare();
                            ws.SetValue(Grid.ColumnProperty, col);
                            ws.SetValue(Grid.RowProperty, row);
                            ws.Tag = 1;
                            UIBoard.Children.Add(ws);
                            break;
                        case -1:
                            BlackReversiSquare bs = new BlackReversiSquare();
                            bs.Tag = -1;
                            bs.SetValue(Grid.ColumnProperty, col);
                            bs.SetValue(Grid.RowProperty, row);
                            UIBoard.Children.Add(bs);
                            break;

                    }



                }
        }
        void square_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Square square = sender as Square;
            if (square != null)
            {
                if (square.rectangle.Fill == Brushes.Red)
                {

                    int row = (int)square.GetValue(Grid.RowProperty);
                    int col = (int)square.GetValue(Grid.ColumnProperty);
                    ReversiMove move = new ReversiMove(row, col, board.GetCurrentPlayerID(), 0);
                    board.MakeMove(move);
                    if (board.GetCurrentPlayerID() == alg.ComputerPlayerID)
                    {
                        computerThread = new Thread(this.computerMove);
                        computerThread.Start();
                    }
                }
            }
        }
        void computerMove()
        {
            ReversiMove move = alg.MiniMax(new ReversiBoard(board,false), SearchDepth, int.MaxValue, int.MinValue) as ReversiMove;
            if (move != null)
            {
                       
                board.MakeMove(move);
            }
        }
        void board_GameOver()
        {
            Dispatcher.Invoke(this.UpdateLabel, null);

        }
        private void updateWinnerLabel()
        {

            if (!gameOver)
            {
                winnerLabel.Visibility = Visibility.Visible;
                switch (board.Winner)
                {
                    case 1: winnerLabel.Content = "Red wins!"; MessageBox.Show("Red Wins!"); break;
                    case -1: winnerLabel.Content = "Black wins!"; MessageBox.Show("Black wins!"); break;
                    case 0: winnerLabel.Content = "It's a draw!"; MessageBox.Show("It's a draw!"); break;
                }
                gameOver = true;
            }
        }
        void upDateGameStatus(int row, int col,int playerID)
        {
            if (row != -1)
            {


                StringBuilder builder = new StringBuilder(playerID.ToString() == "1" ? "White: " : "Black: ");
                builder.Append(Convert.ToChar(65 + col));
                builder.Append((row + 1).ToString());
                moveList.Items.Add(builder.ToString());            
                moveList.ScrollIntoView(moveList.Items[moveList.Items.Count - 1]);
                gameHistory.Add(new ReversiBoard(board,true));

            }
            blackSquareCount.Content = board.BlackSquares.ToString();
            whiteSquareCount.Content = board.WhiteSquares.ToString();
			if(board.GetCurrentPlayerID() == 1)
			{
				whitePlayer.Visibility = System.Windows.Visibility.Visible;
				blackPlayer.Visibility = System.Windows.Visibility.Hidden;
                
			}
			else 
			{
               
				whitePlayer.Visibility = System.Windows.Visibility.Hidden;				
				blackPlayer.Visibility = System.Windows.Visibility.Visible;
			}
				
            if (!board.GameIsOver())
                winnerLabel.Visibility = System.Windows.Visibility.Hidden;
        }
        void board_NoLegalMoveAvailable(bool lastPlayerHadMoves)
        {

            MessageBox.Show( (board.GetCurrentPlayer().GetPlayerID().ToString() == "-1"? "White" : "Black") + "player has no legal moves , switching player");
            if (board.GetCurrentPlayer().GetPlayerID() == alg.ComputerPlayerID)
            {
                computerThread = new Thread(this.computerMove);
                computerThread.Start();
                computerThread.Join();
            }
            else
            {
                Object[] args = { -1,-1,0 };
                Dispatcher.Invoke(myRepaint, args);
                          
            }

        }
        private void repaint(int row1, int col1, int playerID)
        {
            upDateGameStatus(row1, col1, playerID);
            List<Object> visualItems = new List<Object>();
            foreach (var item in UIBoard.Children)
                visualItems.Add(item);
            UIBoard.Children.Clear();
            foreach (var item in visualItems)
            {

                if (item is Square)
                {

                    Square square = item as Square;
                    int row = (int)square.GetValue(Grid.RowProperty);
                    int col = (int)square.GetValue(Grid.ColumnProperty);
                    switch (board.GetPieceColor(row, col))
                    {
                        case 0:
                            if (board.IsValidMove(row, col) && board.GetCurrentPlayerID() != alg.ComputerPlayerID)
                            {
                                square.rectangle.Fill = Brushes.Red;
                                UIBoard.Children.Add(square);
                            }
                            else
                            {
                                square.rectangle.Fill = sqareBaseColor;
                                UIBoard.Children.Add(square);
                            }
                            break;
                           
                        case 1:                         
                            WhiteReversiSquare ws = new WhiteReversiSquare();
                            ws.SetValue(Grid.ColumnProperty, col);
                            ws.SetValue(Grid.RowProperty, row);
                            ws.Tag = 1;
                            UIBoard.Children.Add(ws);
                            break;
                        case -1:
                            BlackReversiSquare bs = new BlackReversiSquare();
                            bs.Tag = -1;
                            bs.SetValue(Grid.ColumnProperty, col);
                            bs.SetValue(Grid.RowProperty, row);
                            UIBoard.Children.Add(bs);
                            break;
                          

                    }
                }
                if (item is BlackReversiSquare)
                {
                    BlackReversiSquare bs = item as BlackReversiSquare;
                    int row = (int)bs.GetValue(Grid.RowProperty);
                    int col = (int)bs.GetValue(Grid.ColumnProperty);
                    int oldColor = int.Parse(bs.Tag.ToString());
                    int newColor = board.GetPieceColor(row,col);
                    UIBoard.Children.Add(bs);
                    if (newColor != oldColor)
                    {
                        bs.Tag = newColor.ToString();
                        if (oldColor == 1)
                        {
                            Storyboard story = bs.FindResource("SpinWhiteBlack") as Storyboard;
                            story.Begin();

                        }
                        else
                        {
                            Storyboard story = bs.FindResource("SpinBlackWhite") as Storyboard;
                            story.Begin();
                        }
                    }
                }
                if (item is WhiteReversiSquare)
                {
                    WhiteReversiSquare ws = item as WhiteReversiSquare;
                    int row = (int)ws.GetValue(Grid.RowProperty);
                    int col = (int)ws.GetValue(Grid.ColumnProperty);
                    int oldColor = int.Parse(ws.Tag.ToString());
                    int newColor = board.GetPieceColor(row, col);
                    UIBoard.Children.Add(ws);
                    if (newColor != oldColor)
                    {
                        ws.Tag = newColor.ToString();
                        if (oldColor == 1)
                        {
                            Storyboard story = ws.FindResource("SpinWhiteBlack") as Storyboard;
                            story.Begin();

                        }
                        else
                        {
                            Storyboard story = ws.FindResource("SpinBlackWhite") as Storyboard;
                            story.Begin();
                        }
                    }
                }



            }
        }
        private void moveList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListView list = sender as ListView;
            if (list != null)
            {
                if (list.SelectedIndex != -1)
                {
                    MessageBoxResult result = MessageBox.Show("Do you wish to restore the game from this position? ",
                        "Restore",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Exclamation,
                        MessageBoxResult.No);
                    if (result == MessageBoxResult.Yes)
                    {
                        this.board = new ReversiBoard(gameHistory[list.SelectedIndex],true);
                        int selectedIndex = list.SelectedIndex;
                        for (int index = list.Items.Count - 1; index > selectedIndex; --index)
                        {
                            gameHistory.RemoveAt(index);
                            list.Items.RemoveAt(index);
                        }
                        this.intializeVisualBoard();
                        this.repaint(-1, -1, 0);
                        if (alg.ComputerPlayerID == board.GetCurrentPlayerID())
                        {
                            computerThread = new Thread(this.computerMove);
                            computerThread.Start();
                        }
                    }

                }
            }
            list.SelectedIndex = -1;
            e.Handled = true;
        }

     

    }
}
