using System;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using CheckersGame;
using GameAI;
namespace CheckersWebUIASP
{
    public partial class _Default : System.Web.UI.Page
    {

        private readonly static double[] x = { 1, 1.5, 0.304941560283742, 0.398957574925831, 0.808534060050051,
                                 0.0123464008850727, 0.947831550123092, 0.983976080074895, 0.157787902819825,
                                 0.999516903422548, 0.164692896960626 };
        CheckersBoard board;
        Algorithms alg;
        List<CheckersMove> gameHistory;       
        CheckersMove lastComputerMove = null;
        private char AIID;
        Random rand = new Random(DateTime.Now.Millisecond);
        protected void Page_Load(object sender, EventArgs e)
        {
            
            
            Session.Timeout = 5;
            this.board = this.Session["board"] as CheckersBoard;
            this.alg = this.Session["alg"] as Algorithms;
            this.gameHistory = this.Session["gameHistory"] as List<CheckersMove>;
            if (this.Session["AIID"] != null)
                this.AIID = this.Session["AIID"].ToString()[0];
            if (board != null && alg != null && this.gameHistory != null && !this.board.GameIsOver())
            {
                this.board.MakeMove(parsePlayerMove());

                if (!board.GameIsOver())
                {
                    this.computerMove();
                    this.generateBoardElements();
                }
                if (this.board.GameIsOver())
                {
                    this.winner.Text = this.board.Winner == 1 ? "The player RED is the winner." : "The player BLACK is the winner.";
                    Response.Redirect("~/Results.aspx");

                }
                
            }
        }

        private IMove parsePlayerMove()
        {
            List<IMove> legalMoves = board.GetLegalMoves();
            foreach (var item in legalMoves)
            {
                CheckersMove move = item as CheckersMove;
                if (move.ToShortString() == playerMove.Text.TrimEnd(' '))
                {
                    this.gameHistory.Add(move);
                    return move;
                }
            }
            return null;

        }



        private void computerMove()
        {
            CheckersMove move = null;
            switch(this.AIID)
            {
                case 'H': 
                    move = alg.ABNegamax(board, 8, 0, double.MinValue, double.MaxValue) as CheckersMove;
                    break;
                case 'E': 
                    move = alg.ABNegamax(board, 2, 0, double.MinValue, double.MaxValue) as CheckersMove;
                    break;
                case 'P':
                    move = alg.POSM(board, 0.5d) as CheckersMove;
                    break;
            }
            if (move == null)
                throw new ArgumentException("The move has returned null!");
            this.lastComputerMove = move;
            this.gameHistory.Add(move);
            board.MakeMove(move);
        }
        private void generateBoardElements()
        {
            StringBuilder sb = new StringBuilder();
            int blackWhite = 1;
            for (int row = 0; row < 8; ++row)
            {
                HtmlTableRow newRow = new HtmlTableRow();
                for (int col = 0; col < 8; ++col)
                {
                    //If it's a black cell we take care of what is on it
                    if (blackWhite % 2 == 0)
                    {
                        newRow.Cells.Add(generateCell(row, col));
                    }
                    //We don't care about white cells...
                    else
                    {
                        HtmlTableCell cell = new HtmlTableCell();
                        cell.InnerHtml = "<img src=\"Content/Images/WhiteSquare.png\"/>";
                        cell.Height = "50";
                        cell.Width = "50";
                        newRow.Cells.Add(cell);

                    }

                    blackWhite++;
                }


                visualBoard.Rows.Add(newRow);
                blackWhite++;
            }


        }
        private HtmlTableCell generateCell(int row, int col)
        {
            HtmlTableCell cell = new HtmlTableCell();
            cell.Height = "50";
            cell.Width = "50";

            cell.ID = row.ToString() + col.ToString();
            StringBuilder sb = new StringBuilder();
            string movePaths = generateMovePaths(row, col);
            string imageId = "img" + row.ToString() + col.ToString();
            switch (board.Board[row, col])
            {
                case -1:

                    if (movePaths != string.Empty)
                    {

                        sb.Append("<img id=" + imageId + " src=\"Content/Images/BlackChecker.png\" onclick =\"doClick('" + imageId + "')\" />");
                        sb.Append(movePaths);
                    }
                    else
                        sb.Append("<img id=" + imageId + " src=\"Content/Images/BlackChecker.png\"/>");
                    cell.InnerHtml = sb.ToString();
                    break;
                case 1:
                    sb = new StringBuilder();
                    if (movePaths != string.Empty)
                    {

                        sb.Append("<img id=" + imageId + " src=\"Content/Images/WhiteChecker.png\" onclick =\"doClick('" + imageId + "')\" />");
                        sb.Append(movePaths);
                    }
                    else
                        sb.Append("<img id=" + imageId + " src=\"Content/Images/WhiteChecker.png\"/>");
                    cell.InnerHtml = sb.ToString();


                    break;
                case -2:

                    if (movePaths != string.Empty)
                    {

                        sb.Append("<img id=" + imageId + " src=\"Content/Images/BlackKing.png\" onclick =\"doClick('" + imageId + "')\" />");
                        sb.Append(movePaths);
                    }
                    else
                        sb.Append("<img id=" + imageId + " src=\"Content/Images/BlackKing.png\"/>");
                    cell.InnerHtml = sb.ToString();

                    break;
                case 2:
                    if (movePaths != string.Empty)
                    {

                        sb.Append("<img id=" + imageId + " src=\"Content/Images/WhiteKing.png\" onclick =\"doClick('" + imageId + "')\" />");
                        sb.Append(movePaths);
                    }
                    else
                        sb.Append("<img id=" + imageId + " src=\"Content/Images/WhiteKing.png\"/>");
                    cell.InnerHtml = sb.ToString();
                    break;
                case 0:
                    if (this.lastComputerMove != null &&
                        this.pathContainsPoint(row, col))
                        cell.InnerHtml = "<img id=" + imageId + " src=\"Content/Images/LastComputerMove.png\"/>";
                    else
                        cell.InnerHtml = "<img id=" + imageId + " src=\"Content/Images/BlackSquare.png\"/>";
                    break;

            }

            return cell;
        }
        private bool pathContainsPoint(int row, int col)
        {
            for (int index = 0; index < this.lastComputerMove.GetPath().Count; index++)
            {
                if (this.lastComputerMove.GetPathAt(index).IsEqual(new Point(row, col)))
                    return true;
            }
            return false;
        }
        private string generateMovePaths(int row, int col)
        {
            StringBuilder sb = new StringBuilder();
            List<CheckersMove> list = board.GetMovesForPiece(row, col);
            list = board.GetMovesForPiece(row, col);
            int index = 0;
            foreach (var item in list)
            {
                StringBuilder value = new StringBuilder();
                foreach (var movePart in item.GetPath())
                {
                    value.Append(movePart.X.ToString() + " " + movePart.Y.ToString() + " ");
                }
                value.Remove(value.Length - 1, 1);
                sb.Append("<input type=\"hidden\" id=\"Text" + row.ToString() +
                    col.ToString() + index.ToString() + "\" type=\"text\" value=\"" + value.ToString() + "\" />" +
                    Environment.NewLine);
                index++;
            }
            return sb.ToString();

        }
        protected void Button1_Click(object sender, EventArgs e)
        {

        }
        protected void startButton_Click(object sender, EventArgs e)
        {
           
            if (startButton.Enabled == true)
            {
                this.startButton.Enabled = false;
                this.winner.Visible = true;
                this.winner.Text = "Game in progress!";
                this.pickPlayerLabel.Visible = false;
                this.playBlack.Visible = false;
                this.playRed.Visible = false;
                //X is a double[] that contains the weigths for the 
                //evaluation function
                this.board = new CheckersBoard(0, x);
                double random = rand.NextDouble();
                if (random < 0.33)
                {
                    this.AIID = 'H';
                }
                else if (random > 0.66)
                {
                    this.AIID = 'E';
                }
                else
                {
                    this.AIID = 'P';
                }
                this.gameHistory = new List<CheckersMove>();
                if (playBlack.Checked)
                {
                    this.alg = new Algorithms(board.GetCurrentPlayer(),0.5);
                    this.computerMove();
                }
                else
                {
                    alg = new Algorithms(new CheckersGame.CheckersPlayer(1));
                }
                board.GameMaxLength = 150;
                playerMove.Text = "nothing";
                generateBoardElements();
                Session.Add("board", this.board);
                Session.Add("alg", this.alg);
                Session.Add("gameHistory", this.gameHistory);
                Session.Add("AIID", this.AIID);
            }
        }

    }
}
