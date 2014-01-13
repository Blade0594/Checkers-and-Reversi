using System;
using System.Collections.Generic;
using System.Web;
using MySql.Data.MySqlClient;

using GameAI;
using CheckersGame;
namespace CheckersWebUIASP
{
    public class DBOperations
    {
        private readonly string connectionString = "server=localhost;User Id=root;Pwd=Gigibecali86;database=checkers";

        public DBOperations()
        {

        }

        public void SaveData(string playerType, CheckersBoard board, List<CheckersMove> moveHistory, Algorithms alg, string q1Answer,string q2Answer, string q3Answer,int aIStrength = 0)
        {
            MySql.Data.MySqlClient.MySqlConnection con = new MySql.Data.MySqlClient.MySqlConnection(this.connectionString);
            MySqlTransaction transaction;
            int computerPlayerIndex = alg.ComputerPlayerID == -1 ? 0 : 1;
            try
            {
                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                    transaction = con.BeginTransaction();
                    try
                    {
                        MySqlCommand command = new MySqlCommand("INSERT INTO games (AIID,AIColor,AIStrength,Outcome,Q1,Q2,Q3) VALUES (@AIID,@AIColor,@AIStrength,@Outcome,@Q1,@Q2,@Q3);");
                        command.Connection = con;
                        
                        command.Transaction = transaction;
                        command.Parameters.Add("@AIID", MySqlDbType.VarChar, 45).Value = playerType;
                        command.Parameters.Add("@AIColor", MySqlDbType.VarChar, 45).Value = alg.ComputerPlayerID;
                        command.Parameters.Add("@AIStrength", MySqlDbType.Int32).Value = aIStrength;
                        command.Parameters.Add("@Outcome", MySqlDbType.Int32).Value = board.Winner;
                        command.Parameters.Add("@Q1", MySqlDbType.String, 45).Value = q1Answer;
                        command.Parameters.Add("@Q2", MySqlDbType.String, 45).Value = q2Answer;
                        command.Parameters.Add("@Q3", MySqlDbType.String, 45).Value = q3Answer;
                        command.ExecuteNonQuery();
                        command = new MySqlCommand("SELECT LAST_INSERT_ID() as idGames;", con,transaction);
                        int newId = -1;
                        System.Data.IDataReader reader = command.ExecuteReader();
                        if (reader != null && reader.Read())
                        {
                            newId = reader.GetInt32(0);
                            Console.WriteLine(newId);

                        }
                        reader.Close();
                      
                        for (int index = 0; index < moveHistory.Count; index++)
                        {
                            command = new MySqlCommand("INSERT INTO moves (idGames,MoveNumber,Observation,Prediction,MoveString) VALUES (@idGames,@MoveNumber,@Observation,@Prediction,@MoveString);"
                                                            , con, transaction);
                            command.Parameters.Add("@idGames", MySqlDbType.Int32).Value = newId;
                            command.Parameters.Add("@MoveNumber", MySqlDbType.Int32).Value = index;
                            if (moveHistory[index].PlayerID == alg.ComputerPlayerID)
                            {
                                if (playerType == "P")
                                {
                                    if ((index + computerPlayerIndex) % 2 == 0 &&
                                        (index + computerPlayerIndex) / 2 < alg.Statistics.Count)
                                    {
                                        command.Parameters.Add("@Observation", MySqlDbType.Double).Value = alg.Statistics[(index + computerPlayerIndex) / 2].Observation;
                                        command.Parameters.Add("@Prediction", MySqlDbType.Double).Value = alg.Statistics[(index + computerPlayerIndex) / 2].Prediction;
                                    }
                                    else
                                    {
                                        command.Parameters.Add("@Observation", MySqlDbType.Double).Value = 0;
                                        //this 100 is impossible. That's why we use it for a "oh shit" marker.
                                        command.Parameters.Add("@Prediction", MySqlDbType.Double).Value = 100;
                                    }
                                }
                                else
                                {
                                    command.Parameters.Add("@Observation", MySqlDbType.Double).Value = 0;
                                    command.Parameters.Add("@Prediction", MySqlDbType.Double).Value = 0;

                                }
                            }
                            else
                            {
                                command.Parameters.Add("@Observation", MySqlDbType.Double).Value = 0;
                                command.Parameters.Add("@Prediction", MySqlDbType.Double).Value = 0;

                            }
                            command.Parameters.Add("@MoveString", MySqlDbType.VarChar, 45).Value = moveHistory[index].ToString();
                            command.ExecuteNonQuery();


                        }
                        transaction.Commit();
                      
                      
                    } 
                    catch (MySqlException ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {

                System.Console.WriteLine(ex.Message);
            }
            finally
            {
                con.Close();
            }

        }


    }
}