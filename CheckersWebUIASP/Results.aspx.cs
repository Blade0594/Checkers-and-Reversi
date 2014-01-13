using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CheckersGame;
using GameAI;

namespace CheckersWebUIASP
{
    public partial class Results : System.Web.UI.Page
    {
        CheckersBoard board;
        Algorithms alg;
        List<CheckersMove> gameHistory;
        char AIID;
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void submitAnswers_Click(object sender, EventArgs e)
        {
            this.board = this.Session["board"] as CheckersBoard;
            this.alg = this.Session["alg"] as Algorithms;
            this.gameHistory = this.Session["gameHistory"] as List<CheckersMove>;
            if (this.Session["AIID"] != null)
                this.AIID = this.Session["AIID"].ToString()[0];

            if (this.board == null || !this.board.GameIsOver())
            {
                this.incorrectAnswers.Visible = true;
                this.incorrectAnswers.Text = "The game has not ended yet";
                return;
            }
            if ((this.undecided.Checked || this.smart.Checked || this.stupid.Checked) &&
                (this.fairTrue.Checked || this.fairFalse.Checked) &&
                (this.againFalse.Checked || this.againTrue.Checked))
            {

                this.incorrectAnswers.Visible = true;
                this.incorrectAnswers.Text = "Thank you for submitting!";
                this.submitAnswers.Visible = false;
              
                string q1Answer = "debug";
                string q2Answer = "debug";
                string q3Answer = "debug";

                if (this.undecided.Checked)
                {
                    q3Answer = "undecided";
                }
                else if (this.smart.Checked)
                {
                    q3Answer = "smart";

                }
                else if (this.stupid.Checked)
                {
                    q3Answer = "stupid";
                }
                if (this.fairFalse.Checked)
                {
                    q1Answer = "false";
                }
                else if (this.fairTrue.Checked)
                {
                    q1Answer = "true";
                }
                if (this.againTrue.Checked)
                {
                    q2Answer = "true";
                }
                else if (this.againFalse.Checked)
                {
                    q2Answer = "false";
                }
                DBOperations op = new DBOperations();

                op.SaveData(this.AIID.ToString(), this.board, this.gameHistory, this.alg, q1Answer, q2Answer, q3Answer);
                
                this.submitAnswers.Enabled = false;
            }
            else
            {
                this.incorrectAnswers.Text = "Please answer all of the questions before submitting!";
                this.incorrectAnswers.Visible = true;

            }
        }
    }
}