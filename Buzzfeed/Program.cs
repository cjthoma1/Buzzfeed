using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buzzfeed
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlCommand command;
            SqlDataReader reader, answerReader;
            int userTestChoice, questionOrder = 0, questionID=0, userQuestionChoice=0, SessionsID=0, userQuizTotal=0;
            bool questionLoop=true;

            //SqlConnection connection = new SqlConnection(@"Data Source=10.1.10.148;Initial Catalog=Buzzfeed-Session2; User ID=academy_admin;Password=12345");
            SqlConnection connection = new SqlConnection(@"Data Source=(LocalDb)\MSSQLLocalDB;AttachDbFilename=\\mac\home\documents\visual studio 2015\Projects\Buzzfeed\Buzzfeed\LocalBuzzFeed.mdf; Integrated Security=True");

            connection.Open();
            command = new SqlCommand("Insert INTO Sessions(Time) Values(SYSDATETIME()); SELECT @@IDENTITY AS SessionID;", connection);
            reader = command.ExecuteReader();
            reader.Read();                
            SessionsID = Convert.ToInt32(reader["SessionID"]);                           
            reader.Close();
            Console.WriteLine("WELCOME TO TIMEWASTER!!!");

            command = new SqlCommand("Select Tests.Title, Tests.TestID From Tests", connection);
            reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                Console.WriteLine("Please Select a Quiz Number");
                while (reader.Read())
                {
                    Console.WriteLine(reader["TestID"] +" "+reader["Title"]);
                }
            }
            reader.Close();
            Console.WriteLine("So what Quiz would you like to do?");
             userTestChoice=Convert.ToInt32(Console.ReadLine());
            Console.Clear();
            command = new SqlCommand(" Select Tests.Title From Tests Where Tests.TestId ="+ userTestChoice , connection);
            reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                if (reader.Read())
                {
                    Console.WriteLine(reader["Title"]);
                    Console.WriteLine();
                }
            }
            reader.Close();
            while (questionLoop)
            {
                command = new SqlCommand("Select Questions.Question, Questions.QuestionID From Questions Where Questions.TestId =" + userTestChoice + "AND Questions.SortOrder =" + ++questionOrder, connection);
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        questionID = Convert.ToInt32(reader["QuestionID"]);
                        Console.WriteLine(questionOrder + ". " + reader["Question"]);
                        reader.Close();
                    }
                    command = new SqlCommand("Select Answers.Answer, Answers.AnswerID From Answers WHERE Answers.QuestionId =" + questionID, connection);
                    answerReader = command.ExecuteReader();
                    if (answerReader.HasRows)
                    {
                        while (answerReader.Read())
                        {
                            Console.WriteLine(answerReader["AnswerID"] + " " + answerReader["Answer"]);
                        }
                    }
                    answerReader.Close();
                    userQuestionChoice = Convert.ToInt32(Console.ReadLine());
                    command = new SqlCommand("Insert INTO Responses (SessionID, AnswerID) Values(" + SessionsID + "," + userQuestionChoice + ")", connection);
                    command.ExecuteNonQuery();
                }
                else
                {
                    questionLoop = false;
                    reader.Close();
                }
            }                   
            command = new SqlCommand("Select Sum(Value) AS total From Responses Join Answers On Responses.AnswerId = Answers.AnswerId Where SessionID =" +SessionsID,connection);
            reader = command.ExecuteReader();
            reader.Read();
           Console.WriteLine(reader["total"]);             
           userQuizTotal=Convert.ToInt32(reader["total"]);
            reader.Close();

            command = new SqlCommand("Select Top 1 Results.Text, Results.Title From Results Where [Value] <=" + userQuizTotal, connection);
            reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                if (reader.Read())
                {
                    Console.WriteLine("YOUR RESULT IS:");
                    Console.WriteLine(reader["Title"]);
                    Console.WriteLine(reader["Text"]);
                }
                reader.Close();
            }
            connection.Close();
            Console.ReadLine();
        }
    }
}
