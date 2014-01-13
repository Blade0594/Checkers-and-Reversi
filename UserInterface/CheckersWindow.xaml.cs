using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Amib.Threading;
using System.Threading;
using CheckersGame;
using GameAI;
namespace UserInterface
{
    /// <summary>
    /// Interaction logic for CheckersWindow.xaml
    /// </summary> 
    public partial class CheckersWindow : Window
    {
        private StackPanel currentItemContainer = null;
        private CheckersBoard board;
        private System.Windows.Point startPoint;
        private List<BlackSquare> blackSquares = new List<BlackSquare>();
        private List<CheckersMove> movesForCurrentPiece;
        private BlackSquare parentSquare;
        private Algorithms alg;
        public static int SearchDepth;
        public static int ComputerPlayer;
        CheckersMove lastPlayerMove;
        List<IMove> lastPlayerPossibleMoves;
        Thread computerThread;
        CheckersBoard evaluator;
        CheckersMove lastMove;
        delegate void EndGame();
        EndGame visualGameOver;
        delegate void Repaint(CheckersMove move);
        private Repaint myRepaint;
        private int moveIndex;
        private double[] x = { 1, 1.5, 0.304941560283742, 0.398957574925831, 0.808534060050051,
                                 0.0123464008850727, 0.947831550123092, 0.983976080074895, 0.157787902819825,
                                 0.999516903422548, 0.164692896960626 };
        bool isInMove;
        public CheckersWindow()
        {
            this.InitializeComponent();
            board = new CheckersBoard(0, x);
            board.GameMaxLength = 150;
            lastMove = new CheckersMove(null, false, 1);
            moveList.Tag = new List<CheckersMove>();
            myRepaint = new Repaint(this.repaint);
            CheckersPlayer player = new CheckersPlayer(ComputerPlayer);
            alg = new Algorithms(player);
            int blackWhite = 1;
            for (int row = 0; row < 8; ++row)
            {
                for (int col = 0; col < 8; ++col)
                {

                    if (blackWhite % 2 == 0)
                    {
                        BlackSquare square = new BlackSquare();
                        square.SetValue(Grid.ColumnProperty, col);
                        square.SetValue(Grid.RowProperty, row);
                        square.Tag = new System.Windows.Point(row, col);
                        square.DragEnter += new DragEventHandler(square_DragEnter);
                        square.Drop += new DragEventHandler(square_Drop);
                        square.MouseLeftButtonDown += new MouseButtonEventHandler(square_MouseLeftButtonDown);
                        UIBoard.Children.Add(square);
                        blackSquares.Add(square);


                    }
                    else
                    {
                        WhiteSquare square = new WhiteSquare();
                        square.SetValue(Grid.ColumnProperty, col);
                        square.SetValue(Grid.RowProperty, row);
                        UIBoard.Children.Add(square);

                    }
                    blackWhite++;
                }
                blackWhite++;
            }
            StringBuilder stringBuilder = new StringBuilder("Current Player is:\n");
            stringBuilder.Append(board.GetCurrentPlayer().GetPlayerID().ToString() == "-1" ? "Black" : "White");
            currentPlayerLabel.Content = stringBuilder.ToString();
            repaint(null);
            board.GameOver += new CheckersBoard.EndGame(board_GameOver);
            if (board.GetCurrentPlayer().GetPlayerID() == alg.ComputerPlayerID)
            {
                computerThread = new Thread(this.computerMove);
                computerThread.Start();
            }
            this.visualGameOver = new EndGame(this.theGameIsOver);


        }
        void board_GameOver()
        {
            Dispatcher.Invoke(this.visualGameOver);
        }
        void theGameIsOver()
        {
            winnerLabel.Visibility = Visibility.Visible;
            switch (board.Winner)
            {
                case 1: winnerLabel.Content = "Red wins!"; MessageBox.Show("Red Wins!"); break;
                case -1: winnerLabel.Content = "Black wins!"; MessageBox.Show("Black wins!"); break;
                case 0: winnerLabel.Content = "It's a draw!"; MessageBox.Show("It's a draw!"); break;

            }
            EndGameWindow end = new EndGameWindow(this);
            end.ShowDialog();
            this.Close();

        }
        void square_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.parentSquare = sender as BlackSquare;

        }
        void square_Drop(object sender, DragEventArgs e)
        {
            BlackSquare square = sender as BlackSquare;
            if (currentItemContainer != null)
            {
                if (square.Opacity != 1)
                {

                    currentItemContainer.Children.Clear();
                    currentItemContainer = null;
                    square.backGround.Children.Add((Ellipse)e.Data.GetData(typeof(Ellipse)));
                    parentSquare.Opacity = 1;
                    parentSquare = null;

                    System.Windows.Point squareCoords = (System.Windows.Point)square.Tag;
                    if (isInMove == false)
                    {
                        isInMove = true;
                    }
                    List<CheckersMove> newLegalMoves = new List<CheckersMove>();
                    foreach (var item in movesForCurrentPiece)
                    {
                        CheckersGame.Point moveCoords = item.GetPathAt(moveIndex);
                        if (moveCoords != null && moveCoords.IsEqual((int)squareCoords.X, (int)squareCoords.Y))
                        {
                            newLegalMoves.Add(item);
                        }
                    }
                    movesForCurrentPiece = newLegalMoves;
                    ++moveIndex;
                    if (movesForCurrentPiece.Count == 1)// && moveIndex == movesForCurrentPiece[0].GetPath().Count)
                    {
                        if (SearchDepth == 8)
                        {
                            lastPlayerMove = movesForCurrentPiece[0];
                            evaluator = new CheckersBoard(board, false);
                        }
                        board.MakeMove(movesForCurrentPiece[0]);
                        repaint(movesForCurrentPiece[0]);
                        if (!board.GameIsOver())
                        {
                            computerThread = new Thread(this.computerMove);
                            computerThread.Start();

                        }
                        isInMove = false;
                    }
                }
                //else
                //    MessageBox.Show("Illegal move!", "Illegal", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            }


            foreach (var elem in blackSquares)
            {
                elem.Opacity = 1.0;
                foreach (var item in elem.backGround.Children)
                    if (item is Ellipse)
                    {
                        Ellipse elli = item as Ellipse;
                        int row = (int)elem.GetValue(Grid.RowProperty);
                        int col = (int)elem.GetValue(Grid.ColumnProperty);
                        elli.Tag = new System.Windows.Point(row, col);

                    }
            }
        }

        private void computerMove()
        {
            List<IMove> moves = board.GetLegalMoves();
            if (SearchDepth == 8 && lastPlayerMove != null)
            {
                lastPlayerPossibleMoves = alg.EvaluateMoves(evaluator, SearchDepth, 0, double.MinValue, double.MaxValue);
            }
            if (moves != null && moves.Count == 1)
            {
                CheckersMove move = moves[0] as CheckersMove;
                if (move.GetPath() != null)
                    board.MakeMove(move);
                Dispatcher.Invoke(myRepaint, move);
            }
            else
            {

                DateTime date = DateTime.Now;
                CheckersMove move;
                if (SearchDepth == 8 && lastPlayerMove != null)
                    move = alg.AdaptiveABNegamax(board, SearchDepth, 0, double.MinValue, double.MaxValue, lastPlayerPossibleMoves, lastPlayerMove) as CheckersMove;
                else if (SearchDepth == 15)
                    move = alg.POSM(board, 0.5) as CheckersMove;
                else
                    move = alg.ABNegamax(new CheckersBoard(board, false), SearchDepth, 0, double.MinValue, double.MaxValue) as CheckersMove;
                DateTime newDate = DateTime.Now;
                TimeSpan span = newDate - date;
                move.Duration = span;
                if (move != null)
                {

                    if (move.GetPath() != null)
                        board.MakeMove(move);

                    Dispatcher.Invoke(myRepaint, move);
                }
            }
        }
        void square_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(Ellipse)) || sender == e.Source)
            {
                e.Effects = DragDropEffects.None;

            }
        }
        private void repaint(CheckersMove move)
        {

            if (move != null)
            {

                StringBuilder builder = new StringBuilder(move.PlayerID.ToString() == "1" ? "White: " : "Black: ");
                foreach (var item in move.GetPath())
                {
                    builder.Append(Convert.ToChar(65 + item.Y));
                    builder.Append((item.X + 1).ToString());
                    builder.Append('-');
                }
                //builder.Remove(builder.Length - 1, 1);
                builder.Append(move.Duration.TotalSeconds.ToString());
                moveList.Items.Add(builder.ToString());
                List<CheckersMove> list = moveList.Tag as List<CheckersMove>;
                list.Add(move);
                moveList.ScrollIntoView(moveList.Items[moveList.Items.Count - 1]);
                lastMove = move;

            }
            if (!board.GameIsOver())
                winnerLabel.Visibility = Visibility.Hidden;
            StringBuilder stringBuilder = new StringBuilder("Current Player is:\n");
            stringBuilder.Append(board.GetCurrentPlayer().GetPlayerID().ToString() == "-1" ? "Black" : "White");
            currentPlayerLabel.Content = stringBuilder.ToString();
            whitePieces.Content = "White Pieces : " + board.WhitePieces.ToString();
            blackPieces.Content = "Black Pieces : " + board.BlackPieces.ToString();
            foreach (var item in this.blackSquares)
            {

                BlackSquare square = item as BlackSquare;
                int row = (int)square.GetValue(Grid.RowProperty);
                int col = (int)square.GetValue(Grid.ColumnProperty);
                int pieceType = board.GetPiece(row, col);
                switch (pieceType)
                {
                    case 0: square.backGround.Children.Clear(); break;
                    case 1:
                        square.backGround.Children.Clear();
                        square.backGround.Children.Add(ellipseBuilder(row, col, pieceType));
                        break;


                    case -1:
                        square.backGround.Children.Clear();
                        square.backGround.Children.Add(ellipseBuilder(row, col, pieceType));
                        break;
                    case 2:
                        square.backGround.Children.Clear();
                        square.backGround.Children.Add(ellipseBuilder(row, col, pieceType));
                        break;
                    case -2:
                        square.backGround.Children.Clear();
                        square.backGround.Children.Add(ellipseBuilder(row, col, pieceType));
                        break;
                }


            }

        }
        private Ellipse ellipseBuilder(int row, int col, int pieceType)
        {
            Ellipse elli = new Ellipse();
            elli.Height = 48;
            elli.Width = 48;
            switch (pieceType)
            {
                case 1: elli.Fill = this.Resources["WhiteEllipseBrush"] as VisualBrush; break;
                case -1: elli.Fill = this.Resources["BlackEllipseBrush"] as VisualBrush; break;
                case -2: elli.Fill = this.Resources["BlackKingEllipseBrush"] as VisualBrush; break;
                case 2: elli.Fill = this.Resources["WhiteKingEllipseBrush"] as VisualBrush; break;
            }
            elli.Tag = new System.Windows.Point(row, col);
            elli.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(elli_PreviewMouseLeftButtonDown);
            elli.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(elli_PreviewMouseLeftButtonUp);
            elli.MouseMove += new MouseEventHandler(elli_MouseMove);
            return elli;
        }
        void elli_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            foreach (var elem in blackSquares)
            {
                elem.Opacity = 1.0;
                foreach (var item in elem.backGround.Children)
                    if (item is Ellipse)
                    {
                        Ellipse elli = item as Ellipse;
                        int row = (int)elem.GetValue(Grid.RowProperty);
                        int col = (int)elem.GetValue(Grid.ColumnProperty);
                        elli.Tag = new System.Windows.Point(row, col);

                    }
            }
        }
        void elli_MouseMove(object sender, MouseEventArgs e)
        {
            if (board.GetCurrentPlayer().GetPlayerID() != alg.ComputerPlayerID)
            {
                Vector difference = startPoint - e.GetPosition(null);
                if (e.LeftButton == MouseButtonState.Pressed &&
                    Math.Abs(difference.X) > SystemParameters.MinimumHorizontalDragDistance &&
                    Math.Abs(difference.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    Ellipse elli = sender as Ellipse;
                    currentItemContainer = elli.Parent as StackPanel;
                    DragDrop.DoDragDrop(elli, elli, DragDropEffects.Move);
                }
            }

        }
        void elli_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
            Ellipse eli = sender as Ellipse;
            if (eli != null && board.GetCurrentPlayer().GetPlayerID() != alg.ComputerPlayerID)
            {
                System.Windows.Point point = (System.Windows.Point)eli.Tag;
                if (!isInMove)
                    movesForCurrentPiece = board.GetMovesForPiece((int)point.X, (int)point.Y);
                if (movesForCurrentPiece.Count > 0)
                {
                    if (isInMove == false)
                        moveIndex = 1;
                    foreach (var item in movesForCurrentPiece)
                    {
                        List<CheckersGame.Point> movePath = item.GetPath();
                        for (int index = moveIndex; index < movePath.Count; ++index)
                        {
                            foreach (var elem in blackSquares)
                            {
                                System.Windows.Point squareCoords = (System.Windows.Point)elem.Tag;
                                if (item.GetPathAt(index).IsEqual((int)squareCoords.X, (int)squareCoords.Y))
                                    elem.Opacity = 0.7;

                            }
                        }

                    }
                }
            }

        }
        private void load_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog fileDiag = new System.Windows.Forms.OpenFileDialog();
            System.Windows.Forms.DialogResult result = fileDiag.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                FileStream file = new FileStream(fileDiag.FileName, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(file);
                string currentString = reader.ReadLine();
                List<CheckersMove> list = new List<CheckersMove>();
                try
                {
                    while (currentString != null)
                    {
                        string[] parseData = currentString.Split(' ');
                        int pathLength = int.Parse(parseData[0]);
                        List<CheckersGame.Point> pathPoints = new List<CheckersGame.Point>();
                        for (int index = 0; index < pathLength; ++index)
                        {
                            pathPoints.Add(new CheckersGame.Point(int.Parse(parseData[index * 2 + 1]), int.Parse(parseData[(index + 1) * 2])));
                        }
                        bool isJump = false;
                        if (parseData[pathLength * 2 + 1] == "True")
                            isJump = true;
                        int playerID = int.Parse(parseData[parseData.Length - 1]);
                        list.Add(new CheckersMove(pathPoints, isJump, playerID));
                        currentString = reader.ReadLine();
                    }
                }
                catch
                {

                    MessageBox.Show("This is not a valid document");
                    return;
                }
                moveList.Tag = list;
                moveList.Items.Clear();

                board = new CheckersBoard(0, x);
                board.GameOver += new CheckersBoard.EndGame(board_GameOver);
                board.GameMaxLength = 200;
                foreach (var move in list)
                {
                    board.MakeMove(move);

                    StringBuilder builder = new StringBuilder(move.PlayerID.ToString() == "1" ? "White: " : "Black: ");
                    foreach (var item in move.GetPath())
                    {
                        builder.Append(Convert.ToChar(65 + item.Y));
                        builder.Append((item.X + 1).ToString());
                        builder.Append('-');
                    }
                    builder.Remove(builder.Length - 1, 1);
                    moveList.Items.Add(builder.ToString());

                }
                moveList.ScrollIntoView(moveList.Items[moveList.Items.Count - 1]);
                this.repaint(null);
                if (board.GetCurrentPlayer().GetPlayerID() == alg.ComputerPlayerID)
                {
                    computerThread = new Thread(computerMove);
                    computerThread.Start();

                }

                reader.Close();
                file.Close();
            }

        }
        private void save_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog fileDialog = new System.Windows.Forms.SaveFileDialog();
            System.Windows.Forms.DialogResult result = fileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                FileStream file = new FileStream(fileDialog.FileName, FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter writer = new StreamWriter(file);
                StringBuilder builder = new StringBuilder();
                List<CheckersMove> list = moveList.Tag as List<CheckersMove>;
                if (list != null)
                {
                    foreach (var item in list)
                        writer.WriteLine(item.ToString());
                }
                writer.Close();
                file.Close();
            }
        }
        private void moveList_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
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

                        List<CheckersMove> moves = moveList.Tag as List<CheckersMove>;
                        int selectedIndex = list.SelectedIndex;
                        for (int index = list.Items.Count - 1; index > selectedIndex; --index)
                        {

                            list.Items.RemoveAt(index);
                            moves.RemoveAt(index);
                        }
                        board = new CheckersBoard(0, x);
                        board.GameOver += new CheckersBoard.EndGame(board_GameOver);
                        board.GameMaxLength = 200;
                        foreach (var move in moves)
                        {
                            board.MakeMove(move);
                            StringBuilder builder = new StringBuilder(move.PlayerID.ToString() == "1" ? "White: " : "Black: ");
                            foreach (var item in move.GetPath())
                            {
                                builder.Append(Convert.ToChar(65 + item.Y));
                                builder.Append((item.X + 1).ToString());
                                builder.Append('-');
                            }
                            // builder.Remove(builder.Length - 1, 1);
                            builder.Append(move.Duration.TotalSeconds.ToString());


                        }
                        moveList.ScrollIntoView(moveList.Items[moveList.Items.Count - 1]);
                        this.repaint(null);
                        if (alg.ComputerPlayerID == board.GetCurrentPlayer().GetPlayerID())
                        {
                            computerThread = new Thread(this.computerMove);
                            computerThread.Start();
                        }
                    }

                }

            }
            e.Handled = true;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(CheckersWindow.SearchDepth.ToString() + ' ');
            sb.Append(CheckersWindow.ComputerPlayer.ToString() + ' ');
            sb.Append(board.Winner.ToString() + ' ');
            return sb.ToString();
        }
    }
}