using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using Newtonsoft.Json;

namespace ChessGUI
{

    public partial class Chess : Form
    {
        // class necessary state variables
        private Piece[,] board = new Piece[8, 8];
        private string origin = String.Empty;
        private string destination = String.Empty;
        private string messageFromServer = String.Empty;
        private string playerName = String.Empty;
        private int currPosition = 0; // how many moves were made
        private int indexShowing = 0; // which board in all positions is showing on the drawing now
        private Square[,] squares = new Square[8, 8]; // squares for the board to be rendered on
        private Square[,] promotionSquares = new Square[2, 2]; // squares to have the user pick what he wants to promote to
        private IPEndPoint ip;
        private Socket socket;
        private NetworkStream ns;
        private StreamReader sr;
        private StreamWriter sw;
        private GameState state = new GameState();
        private string messageToUser = String.Empty; // this deals with offer new game, this is the message shown
        private DialogResult userInput;              // also for offer new game, this is the user response

        /// <summary>
        /// Initializes the form after setting the time left to maximum amount, the form loads with 3 hours as the checked option so
        /// on load in the game sets that time to 3 hours, if the user changes the button it will change
        /// </summary>
        public Chess()
        {
            state.WhiteTimeLeft = 10800;
            state.BlackTimeLeft = 10800;
            InitializeComponent();
        }
        /*public Chess();*/

        /// <summary>
        /// Checks if 2 boards are the same position by checking that each spot is same color and value on both boards
        /// If they are different returns false, it the loop finished then it returns true
        /// </summary>
        /// <param name="a_board1">first board</param>
        /// <param name="a_board2">second board</param>
        /// <returns>boolean, true if the boards are same false if they are different</returns>
        private bool IsSameBoard(Piece[,] a_board1, Piece[,] a_board2)
        {
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    if(a_board1[i,j].Value != a_board2[i, j].Value)
                    {
                        return false;
                    }
                    if(a_board1[i,j].White != a_board2[i, j].White)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        /*private bool IsSameBoard(Piece[,] a_board1, Piece[,] a_board2);*/

        /// <summary>
        /// Only called once game is over, this checks why the game ended and builds a message to the user from that
        /// Then it displays an interactive message box, if the user selects no then it will close the application
        /// If the user selects yes it will reset to initial state and start again
        /// </summary>
        private void OfferNewGame()
        {
            if(squares[0,0].Visible == false)
            {
                return;
            }
            if (state.CheckMate)
            {
                messageToUser = "Checkmate, would you like to play again?";
            }
            else if (state.StaleMate)
            {
                messageToUser = "Stalemate, would you like to play again?";
            }
            else if (state.DrawByRepitition)
            {
                messageToUser = "Draw by repitition, would you like to play again?";
            }
            else if(state.BlackTimeLeft == 0)
            {
                messageToUser = "Black ran out of time, however white does not have sufficient materials, the result is a draw, would you like to play again?";
                for (int i = 0; i < 8; i++)
                {
                    for(int j = 0; j < 8; j++)
                    {
                        if(board[i,j].White && board[i,j].Value > 0 && board[i, j].Value != 9)
                        {
                            messageToUser = "Black ran out of time, white wins, would you like to play again?";
                            i = 8;
                            j = 8;
                            break;
                        }
                    }
                }
            }
            else if(state.WhiteTimeLeft == 0)
            {
                messageToUser = "White ran out of time, however black does not have sufficient materials, the result is a draw, would you like to play again?";
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (!board[i, j].White && board[i, j].Value > 0 && board[i, j].Value != 9)
                        {
                            messageToUser = "White ran out of time, black wins, would you like to play again?";
                            i = 8;
                            j = 8;
                            break;
                        }
                    }
                }
            }
            else if (state.ServerError)
            {
                messageToUser = "Server Error, we apologize for the inconvenience, would you like to play again?";
            }
            else if(state.OpponentDisconnected)
            {
                messageToUser = "Your opponent has disconnected, would you like to play again?";
            }
            userInput = MessageBox.Show(messageToUser, "New Game?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
            if (userInput == DialogResult.Yes)
            {
                Invoke(new Action(() =>
                {
                    HideSquares();
                    state.Chat = String.Empty;
                    state.Notation = String.Empty;
                    textBoxChat.Text = String.Empty;
                    textBoxNotation.Text = String.Empty;
                    state.AllPositions.Clear();
                    state.WhiteTimeLeft = 10800;
                    state.BlackTimeLeft = 10800;
                    state.TimePortOffset = 75;
                    UpperTimeLabel.Visible = false;
                    LowerTimeLabel.Visible = false;
                    Timer1.Stop();
                    InitializeComponent();
                }));
            }
            else
            {
                Environment.Exit(0);
            }
        }
        /*private void OfferNewGame();*/

        /// <summary>
        /// In a forever loop this method calls ReadLine from the server then deserializes the message into a gamestate
        /// It then resets the current position and index showing to correct values
        /// If the game is over naturally it will call OfferNewGame
        /// Otherwise it invokes an action where it enables review buttons makes sure the timer is running and updates the board and notation and chat
        /// If the game is over due to server error or oppenent disconnect it will stop the timer and call OfferNewGame
        /// </summary>
        private void ProcessServerMessages()
        {
            while (true)
            {
                messageFromServer = sr.ReadLine();
                state = JsonConvert.DeserializeObject<GameState>(messageFromServer);
                currPosition = state.AllPositions.Count - 1;
                indexShowing = currPosition;
                Invoke(new Action(() =>
                {
                    if (state.WaitingForSecondPlayer)
                    {
                        Clicks(false);
                        state.Chat += "Waiting for second player";
                        textBoxChat.Text = state.Chat;
                    }
                    else
                    {
                        ButtonBackOne.Enabled = true;
                        ButtonStartOfGame.Enabled = true;
                        ButtonForwardOne.Enabled = true;
                        ButtonCurrentMove.Enabled = true;
                        Clicks(true);
                        Timer1.Start();
                        textBoxChat.Text = state.Chat;
                        if (!IsSameBoard(board, state.Board))
                        {
                            textBoxNotation.Text = state.Notation;
                            board = state.Board;
                            if (!state.White)
                            {
                                ReverseBoard();
                            }
                            Drawing(board);
                            Clicks(true);
                        }
                        if (state.GameOver)
                        {
                            Timer1.Stop();
                            OfferNewGame();
                        }
                    }
                }));
            }
        }
        /*private void ProcessServerMessages();*/

        // legal moves functions
        /// <summary>
        /// Accepts a copy of a board and an index
        /// Checks if the pawn is on its initial starting point and can move 2 spots
        /// Then checks for a one square move
        /// Then checks for empessant 
        /// Then checks for a regular take
        /// </summary>
        /// <param name="a_board">board copy</param>
        /// <param name="a_i">row index</param>
        /// <param name="a_j">col index</param>
        /// <returns>string of comma seperated indexes on the board that the piece can move</returns>
        private string PawnLegalMoves(Piece[,] a_board, int a_i, int a_j)
        {
            int brilliantNum = 0;
            string moves = String.Empty;
            if (a_board[a_i, a_j].White == state.White)
            {
                brilliantNum = -2;
            }
            else
            {
                brilliantNum = +2;
            }

            // two square move check
            if (a_i == 6 && board[a_i, a_j].HasMoved == false && brilliantNum < 0 && a_board[5, a_j].Value == 0 && a_board[a_i + brilliantNum, a_j].Value == 0)
            {
                moves += (a_i + brilliantNum).ToString() + a_j.ToString() + ", ";
            }
            else if (a_i == 1 && board[a_i, a_j].HasMoved == false && brilliantNum > 0 && a_board[2, a_j].Value == 0 && a_board[a_i + brilliantNum, a_j].Value == 0)
            {
                moves += (a_i + brilliantNum).ToString() + a_j.ToString() + ", ";
            }

            // one square move check after
            if (brilliantNum < 0)
            {
                brilliantNum++;
            }
            else
            {
                brilliantNum--;
            }
            if ((a_i + brilliantNum <= 7) && (a_i + brilliantNum >= 0) && a_board[a_i + brilliantNum, a_j].Value == 0)
            {
                moves += (a_i + brilliantNum).ToString() + a_j.ToString() + ", ";
            }

            // empessant check
            if (a_i == 3 && a_j + brilliantNum < 8 && a_j + brilliantNum >= 0 && a_board[a_i, a_j + brilliantNum].HasMoved == false && a_board[a_i, a_j + brilliantNum].Value == 1)
            {
                moves += (a_i + brilliantNum).ToString() + (a_j + brilliantNum).ToString() + ", ";
            }
            if (a_i == 3 && a_j - brilliantNum >= 0 && a_j - brilliantNum < 8 && a_board[a_i, a_j - brilliantNum].HasMoved == false && a_board[a_i, a_j - brilliantNum].Value == 1)
            {
                moves += (a_i + brilliantNum).ToString() + (a_j - brilliantNum).ToString() + ", ";
            }

            //taking pieces
            if ((a_i + brilliantNum <= 7) && (a_i + brilliantNum >= 0) && (a_j + brilliantNum <= 7) && (a_j + brilliantNum >= 0) && a_board[a_i + brilliantNum, a_j + brilliantNum].Value != 0 && a_board[a_i + brilliantNum, a_j + brilliantNum].White != a_board[a_i, a_j].White)
            {
                moves += (a_i + brilliantNum).ToString() + (a_j + brilliantNum).ToString() + ", ";
            }

            if ((a_i + brilliantNum <= 7) && (a_i + brilliantNum >= 0) && (a_j - brilliantNum <= 7) && (a_j - brilliantNum >= 0) && (a_board[a_i + brilliantNum, a_j - brilliantNum].Value != 0 && a_board[a_i + brilliantNum, a_j - brilliantNum].White != a_board[a_i, a_j].White))
            {
                moves += (a_i + brilliantNum).ToString() + (a_j - brilliantNum).ToString() + ", ";
            }
            return moves;
        }
        /*private string PawnLegalMoves(Piece[,] a_board, int a_i, int a_j);*/

        /// <summary>
        /// uses short-circuit evaluation to check 8 possible indexes on the board copy to see if the knight can be moved there
        /// </summary>
        /// <param name="a_board">board copy</param>
        /// <param name="a_i">row index</param>
        /// <param name="a_j">col index</param>
        /// <returns>string of commma seperated indexes where the knight can move</returns>
        private string KnightLegalMoves(Piece[,] a_board, int a_i, int a_j)
        {
            string moves = String.Empty;
            if (a_i - 2 >= 0 && a_j - 1 >= 0 && (a_board[a_i - 2, a_j - 1].Value == 0 || a_board[a_i - 2, a_j - 1].White != a_board[a_i, a_j].White))
            {
                moves += (a_i - 2).ToString() + (a_j - 1).ToString() + ", ";
            }
            if (a_i - 2 >= 0 && a_j + 1 < 8 && (a_board[a_i - 2, a_j + 1].Value == 0 || a_board[a_i - 2, a_j + 1].White != a_board[a_i, a_j].White))
            {
                moves += (a_i - 2).ToString() + (a_j + 1).ToString() + ", ";
            }
            if (a_i - 1 >= 0 && a_j - 2 >= 0 && (a_board[a_i - 1, a_j - 2].Value == 0 || a_board[a_i - 1, a_j - 2].White != a_board[a_i, a_j].White))
            {
                moves += (a_i - 1).ToString() + (a_j - 2).ToString() + ", ";
            }
            if (a_i - 1 >= 0 && a_j + 2 < 8 && (a_board[a_i - 1, a_j + 2].Value == 0 || a_board[a_i - 1, a_j + 2].White != a_board[a_i, a_j].White))
            {
                moves += (a_i - 1).ToString() + (a_j + 2).ToString() + ", ";
            }
            if (a_i + 1 < 8 && a_j - 2 >= 0 && (a_board[a_i + 1, a_j - 2].Value == 0 || a_board[a_i + 1, a_j - 2].White != a_board[a_i, a_j].White))
            {
                moves += (a_i + 1).ToString() + (a_j - 2).ToString() + ", ";
            }
            if (a_i + 1 < 8 && a_j + 2 < 8 && (a_board[a_i + 1, a_j + 2].Value == 0 || a_board[a_i + 1, a_j + 2].White != a_board[a_i, a_j].White))
            {
                moves += (a_i + 1).ToString() + (a_j + 2).ToString() + ", ";
            }
            if (a_i + 2 < 8 && a_j - 1 >= 0 && (a_board[a_i + 2, a_j - 1].Value == 0 || a_board[a_i + 2, a_j - 1].White != a_board[a_i, a_j].White))
            {
                moves += (a_i + 2).ToString() + (a_j - 1).ToString() + ", ";
            }
            if (a_i + 2 < 8 && a_j + 1 < 8 && (a_board[a_i + 2, a_j + 1].Value == 0 || a_board[a_i + 2, a_j + 1].White != a_board[a_i, a_j].White))
            {
                moves += (a_i + 2).ToString() + (a_j + 1).ToString() + ", ";
            }
            return moves;
        }
        /*private string KnightLegalMoves(Piece[,] a_board, int a_i, int a_j);*/

        /// <summary>
        /// Uses short-circuit evaluation to preform checks boundaries on diagonal moves for bishop for as long as there are empty squares it continues
        /// once it hits a piece if hits the same color it doesnt add it but once it hits a piece of the opposite color it will add it as the ast move
        /// </summary>
        /// <param name="a_board">board copy</param>
        /// <param name="a_i">row index</param>
        /// <param name="a_j">col index</param>
        /// <returns>string of comma seperated indexes representing possible moves</returns>
        private string BishopLegalMoves(Piece[,] a_board, int a_i, int a_j)
        {
            string m_moves = String.Empty;
            for (int m_k = 1; m_k < 8; m_k++)
            {
                if (a_i + m_k < 8 && a_j + m_k < 8 && (a_board[a_i + m_k, a_j + m_k].Value == 0))
                {
                    m_moves += (a_i + m_k).ToString() + (a_j + m_k).ToString() + ", ";
                    continue;
                }
                else if (a_i + m_k < 8 && a_j + m_k < 8 && a_board[a_i + m_k, a_j + m_k].White != a_board[a_i, a_j].White)
                {
                    m_moves += (a_i + m_k).ToString() + (a_j + m_k).ToString() + ", ";
                    break;
                }
                else
                {
                    break;
                }
            }
            for (int m_k = 1; m_k < 8; m_k++)
            {
                if (a_i - m_k >= 0 && a_j + m_k < 8 && (a_board[a_i - m_k, a_j + m_k].Value == 0))
                {
                    m_moves += (a_i - m_k).ToString() + (a_j + m_k).ToString() + ", ";
                    continue;
                }
                else if (a_i - m_k >= 0 && a_j + m_k < 8 && a_board[a_i - m_k, a_j + m_k].White != a_board[a_i, a_j].White)
                {
                    m_moves += (a_i - m_k).ToString() + (a_j + m_k).ToString() + ", ";
                    break;
                }
                else
                {
                    break;
                }
            }
            for (int m_k = 1; m_k < 8; m_k++)
            {
                if (a_i + m_k < 8 && a_j - m_k >= 0 && (a_board[a_i + m_k, a_j - m_k].Value == 0))
                {
                    m_moves += (a_i + m_k).ToString() + (a_j - m_k).ToString() + ", ";
                    continue;
                }
                else if (a_i + m_k < 8 && a_j - m_k >= 0 && a_board[a_i + m_k, a_j - m_k].White != a_board[a_i, a_j].White)
                {
                    m_moves += (a_i + m_k).ToString() + (a_j - m_k).ToString() + ", ";
                    break;
                }
                else
                {
                    break;
                }
            }
            for (int m_k = 1; m_k < 8; m_k++)
            {
                if (a_i - m_k >= 0 && a_j - m_k >= 0 && (a_board[a_i - m_k, a_j - m_k].Value == 0))
                {
                    m_moves += (a_i - m_k).ToString() + (a_j - m_k).ToString() + ", ";
                    continue;
                }
                else if (a_i - m_k >= 0 && a_j - m_k >= 0 && a_board[a_i - m_k, a_j - m_k].White != a_board[a_i, a_j].White)
                {
                    m_moves += (a_i - m_k).ToString() + (a_j - m_k).ToString() + ", ";
                    break;
                }
                else
                {
                    break;
                }
            }
            return m_moves;
        }
        /*private string BishopLegalMoves(Piece[,] a_board, int a_i, int a_j);*/

        /// <summary>
        /// Uses short-circuit evaluation to preform boundary checks for all direction, right, left, up, and down
        /// </summary>
        /// <param name="a_board">board copy</param>
        /// <param name="a_i">row index</param>
        /// <param name="a_j">col index</param>
        /// <returns>string of comma seperated indexes</returns>
        private string RookLegalMoves(Piece[,] a_board, int a_i, int a_j)
        {
            string m_moves = String.Empty;
            for (int m_k = 1; m_k < 8; m_k++)
            {
                if (a_i + m_k < 8 && (a_board[a_i + m_k, a_j].Value == 0))
                {
                    m_moves += (a_i + m_k).ToString() + (a_j).ToString() + ", ";
                    continue;
                }
                else if (a_i + m_k < 8 && a_board[a_i + m_k, a_j].White != a_board[a_i, a_j].White)
                {
                    m_moves += (a_i + m_k).ToString() + (a_j).ToString() + ", ";
                    break;
                }
                else
                {
                    break;
                }
            }
            for (int m_k = 1; m_k < 8; m_k++)
            {
                if (a_i - m_k >= 0 && (a_board[a_i - m_k, a_j].Value == 0))
                {
                    m_moves += (a_i - m_k).ToString() + (a_j).ToString() + ", ";
                    continue;
                }
                else if (a_i - m_k >= 0 && a_board[a_i - m_k, a_j].White != a_board[a_i, a_j].White)
                {
                    m_moves += (a_i - m_k).ToString() + (a_j).ToString() + ", ";
                    break;
                }
                else
                {
                    break;
                }
            }
            for (int m_k = 1; m_k < 8; m_k++)
            {
                if (a_j - m_k >= 0 && (a_board[a_i, a_j - m_k].Value == 0))
                {
                    m_moves += (a_i).ToString() + (a_j - m_k).ToString() + ", ";
                    continue;
                }
                else if (a_j - m_k >= 0 && a_board[a_i, a_j - m_k].White != a_board[a_i, a_j].White)
                {
                    m_moves += (a_i).ToString() + (a_j - m_k).ToString() + ", ";
                    break;
                }
                else
                {
                    break;
                }
            }
            for (int m_k = 1; m_k < 8; m_k++)
            {
                if (a_j + m_k < 8 && (a_board[a_i, a_j + m_k].Value == 0))
                {
                    m_moves += (a_i).ToString() + (a_j + m_k).ToString() + ", ";
                    continue;
                }
                else if (a_j + m_k < 8 && a_board[a_i, a_j + m_k].White != a_board[a_i, a_j].White)
                {
                    m_moves += (a_i).ToString() + (a_j + m_k).ToString() + ", ";
                    break;
                }
                else
                {
                    break;
                }
            }
            return m_moves;
        }
        /*private string RookLegalMoves(Piece[,] a_board, int a_i, int a_j);*/

        /// <summary>
        /// Combines the legal rook and bishop moves together and returns them
        /// </summary>
        /// <param name="a_board">board copy</param>
        /// <param name="a_i">row index</param>
        /// <param name="a_j">col index</param>
        /// <returns>A comma seperated string of indexes to possible moves</returns>
        private string QueenLegalMoves(Piece[,] a_board, int a_i, int a_j)
        {
            string m_moves = String.Empty;
            m_moves = BishopLegalMoves(a_board, a_i, a_j);
            m_moves += RookLegalMoves(a_board, a_i, a_j);
            return m_moves;
        }
        /*private string QueenLegalMoves(Piece[,] a_board, int a_i, int a_j);*/

        /// <summary>
        /// Method returns all legal king moves
        /// First it checks the 8 squares immediately around the king
        /// Then if the king has not moved it checks if the king can castle
        /// The rules to not be allowed to castle are if the king or rook has moved or if the king is currently or will castle through check
        /// </summary>
        /// <param name="a_tempBoard">board copy</param>
        /// <param name="a_i">row index</param>
        /// <param name="a_j">col index</param>
        /// <returns>string of comma seperated indexes to moves</returns>
        private string KingLegalMoves(Piece[,] a_tempBoard, int a_i, int a_j)
        {
            string m_moves = String.Empty;
            Piece[,] m_copy = new Piece[8, 8];
            int m_length;
            // check up to 8 squares around the king
            if (a_i - 1 >= 0 && a_j - 1 >= 0 && (a_tempBoard[a_i - 1, a_j - 1].Value == 0 || board[a_i - 1, a_j - 1].White != a_tempBoard[a_i, a_j].White))
            {
                m_moves += (a_i - 1).ToString() + (a_j - 1).ToString() + ", ";
            }
            if (a_i - 1 >= 0 && (a_tempBoard[a_i - 1, a_j].Value == 0 || a_tempBoard[a_i - 1, a_j].White != a_tempBoard[a_i, a_j].White))
            {
                m_moves += (a_i - 1).ToString() + a_j.ToString() + ", ";
            }
            if (a_i - 1 >= 0 && a_j + 1 < 8 && (board[a_i - 1, a_j + 1].Value == 0 || a_tempBoard[a_i - 1, a_j + 1].White != a_tempBoard[a_i, a_j].White))
            {
                m_moves += (a_i - 1).ToString() + (a_j + 1).ToString() + ", ";
            }
            if (a_j - 1 >= 0 && (a_tempBoard[a_i, a_j - 1].Value == 0 || a_tempBoard[a_i, a_j - 1].White != a_tempBoard[a_i, a_j].White))
            {
                m_moves += a_i.ToString() + (a_j - 1).ToString() + ", ";
            }
            if (a_j + 1 < 8 && (a_tempBoard[a_i, a_j + 1].Value == 0 || a_tempBoard[a_i, a_j + 1].White != a_tempBoard[a_i, a_j].White))
            {
                m_moves += a_i.ToString() + (a_j + 1).ToString() + ", ";
            }
            if (a_i + 1 < 8 && a_j - 1 >= 0 && (a_tempBoard[a_i + 1, a_j - 1].Value == 0 || a_tempBoard[a_i + 1, a_j - 1].White != a_tempBoard[a_i, a_j].White))
            {
                m_moves += (a_i + 1).ToString() + (a_j - 1).ToString() + ", ";
            }
            if (a_i + 1 < 8 && (a_tempBoard[a_i + 1, a_j].Value == 0 || a_tempBoard[a_i + 1, a_j].White != a_tempBoard[a_i, a_j].White))
            {
                m_moves += (a_i + 1).ToString() + a_j.ToString() + ", ";
            }
            if (a_i + 1 < 8 && a_j + 1 < 8 && (a_tempBoard[a_i + 1, a_j + 1].Value == 0 || a_tempBoard[a_i + 1, a_j + 1].White != a_tempBoard[a_i, a_j].White))
            {
                m_moves += (a_i + 1).ToString() + (a_j + 1).ToString() + ", ";
            }

            MakeCopy(board, m_copy);
            // check for castling(king cannot castle through check and cannot castle if he or the rook has moved )
            if (!board[a_i, a_j].HasMoved)
            {
                // have to be seperate ifs to make sure king and queenside can happen same move
                if (a_j + 2 < 8 && a_j + 1 < 8 && board[a_i, a_j + 1].Value == 0 && board[a_i, a_j + 2].Value == 0)
                {
                    m_length = 1;
                    // how far are we castling?
                    while(a_j + m_length < 8 && board[a_i, a_j + m_length].Value == 0)
                    {
                        m_length++;
                    }

                    // final checks that the rook has not moved that its the same color on the edge 
                    if(a_j + m_length < 8 && board[a_i,a_j + m_length].Value==5 && board[a_i, a_j + m_length].HasMoved == false && board[a_i, a_j + m_length].White == board[a_i, a_j].White && a_j + m_length == 7)
                    {
                        if(m_length == 4)
                        {
                            m_length--;
                        }
                        m_moves += (a_i).ToString() + (a_j + 2).ToString() + ", ";
                    }
                }
                if(a_j - 2 >= 0 && board[a_i, a_j - 1].Value == 0 && board[a_i, a_j - 2].Value == 0)
                {
                    m_length = -1;
                    // how far are we castling?
                    while (a_j + m_length >= 0 && board[a_i, a_j + m_length].Value == 0)
                    {
                        m_length--;
                    }

                    // final checks that the rook has not moved that its the same color on the edge 
                    if (a_j + m_length >= 0 && board[a_i, a_j + m_length].Value == 5 && board[a_i , a_j + m_length].HasMoved == false && board[a_i , a_j + m_length].White == board[a_i, a_j].White && a_j + m_length == 0)
                    {
                        // now to make sure we are not castling through check
                        if(m_length == -4)
                        {
                            m_length++;
                        }
                        m_moves += a_i.ToString() + (a_j - 2).ToString() + ", ";
                    }
                }
            }
            return m_moves;
        }
        /*private string KingLegalMoves(Piece[,] a_tempBoard, int a_i, int a_j);*/

        /// <summary>
        /// This method returns all the moves the opponent has, the idea being that if the opponent has no moves its either checkmate or stalemate
        /// Loops through every square and checks that the square is not empty and is not your piece
        /// Then calls find legal moves for that square
        /// If there is a legal move for that square it will just return true
        /// If the loop ends it will return false
        /// </summary>
        /// <returns>true if oppenent has legal moves, false if he has no moves</returns>
        private bool OpenentHasMoves()
        {
            string moves = String.Empty;
            string[] movesArray;
            int index;
            Piece[,] copy = new Piece[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j].Value == 0 || board[i, j].White == state.White)
                    {
                        continue;
                    }
                    moves += FindLegalMoves(board, i, j);
                    movesArray = moves.Split(' ', ',');
                    foreach (string move in movesArray)
                    {
                        if (move.Length < 2)
                        {
                            continue;
                        }
                        int iVal = Convert.ToInt32(move.ElementAt(0)) - 48;
                        int jVal = Convert.ToInt32(move.ElementAt(1)) - 48;
                        MakeCopy(board, copy);
                        copy[iVal, jVal] = copy[i, j];
                        copy[i, j] = new Piece();
                        if (IsCheck(copy, !state.White))
                        {
                            index = moves.IndexOf(move);
                            moves = moves.Remove(index, 2);
                            if (moves.ElementAt(index) == ',')
                            {
                                moves = moves.Remove(index, 2);
                            }
                        }
                    }
                    if (moves != String.Empty)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /*private bool OpenentHasMoves();*/

        /// <summary>
        /// Based on piece value it returns the appropriate function call
        /// </summary>
        /// <param name="a_tempBoard">board copy</param>
        /// <param name="a_i">row index</param>
        /// <param name="a_j">col index</param>
        /// <returns>string of comma seperated indexes to moves</returns>
        private string FindLegalMoves(Piece[,] a_tempBoard, int a_i, int a_j)
        {
            switch (a_tempBoard[a_i, a_j].Value)
            {
                case 1:
                    return PawnLegalMoves(a_tempBoard, a_i, a_j);
                case 3:
                    return BishopLegalMoves(a_tempBoard, a_i, a_j);
                case 4:
                    return KnightLegalMoves(a_tempBoard, a_i, a_j);
                case 5:
                    return RookLegalMoves(a_tempBoard, a_i, a_j);
                case 8:
                    return QueenLegalMoves(a_tempBoard, a_i, a_j);
                case 9:
                    return KingLegalMoves(a_tempBoard, a_i, a_j);
                default:
                    return "";
            }
        }
        /*private string FindLegalMoves(Piece[,] a_tempBoard, int a_i, int a_j);*/

        /// <summary>
        /// Highlights all the moves for a given index of the board (a_i, a_j)
        /// First it calls FindLegalMoves for the index and splits the result into an array of moves
        /// Then for every move as long as it is a move it will convert the 2 characters into integers which represent indexes
        /// Then parse and highlight will check if castling is a possibility by copying the board and moving the king those 2 spots and checking if either would be castling through check
        /// Then it simply makes a copy of the board and makes the move on the copy and checks that you are not in check after you make the move in the copy
        /// If you are not in check it highlights that destination square to a green color
        /// </summary>
        /// <param name="a_i">row index</param>
        /// <param name="a_j">col index</param>
        private void ParseAndHighlight(int a_i, int a_j)
        {
            string[] m_movesArray = FindLegalMoves(board, a_i, a_j).Split(' ', ',');    // all legal moves for the piece
            Piece[,] m_copy = new Piece[8, 8];                                          // for easy simulations
            foreach (string m_square in m_movesArray)
            {
                if (m_square == "")
                {
                    continue;
                }
                int m_i = Convert.ToInt32(m_square.ElementAt(0)) - 48;
                int m_j = Convert.ToInt32(m_square.ElementAt(1)) - 48;

                // if one of the squares is for castling this checks that the castle is allowed
                if (board[a_i, a_j].Value == 9 && Math.Abs(m_j - a_j) == 2)
                {
                    if (m_j > a_j)
                    {
                        MakeCopy(board, m_copy);
                        m_copy[a_i, a_j + 1] = m_copy[a_i, a_j];
                        m_copy[a_i, a_j] = new Piece();
                        if (IsCheck(m_copy, state.White))
                        {
                            continue;
                        }
                        m_copy[a_i, a_j + 2] = m_copy[a_i, a_j + 1];
                        m_copy[a_i, a_j + 1] = new Piece();
                        if (IsCheck(m_copy, state.White))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        MakeCopy(board, m_copy);
                        m_copy[a_i, a_j - 1] = m_copy[a_i, a_j];
                        m_copy[a_i, a_j] = new Piece();
                        if (IsCheck(m_copy, state.White))
                        {
                            continue;
                        }
                        m_copy[a_i, a_j - 2] = m_copy[a_i, a_j - 1];
                        m_copy[a_i, a_j - 1] = new Piece();
                        if (IsCheck(m_copy, state.White))
                        {
                            continue;
                        }
                    }
                }

                // make the move on the copy of the board and performa final operations
                MakeCopy(board, m_copy);
                m_copy[m_i, m_j] = m_copy[a_i, a_j];
                m_copy[a_i, a_j] = new Piece();
                if (!IsCheck(m_copy, state.White))
                {
                    squares[m_i, m_j].BackColor = Color.Green;
                }
            }
        }
        /*private void ParseAndHighlight(int a_i, int a_j);*/

        /// <summary>
        /// Checks if the board is in stalemate,
        /// If there is only kings on the board it is an automatic draw
        /// Then if it is not check for either piece and the opponent has no moves it is stalemate
        /// </summary>
        /// <returns>true if stalemate, false if not</returns>
        private bool IsStaleMate()
        {
            // if 2 kings its auto draw
            if (CheckForKingsOnly())
            {
                return true;
            }
            Piece[,] m_copy = new Piece[8, 8];
            MakeCopy(board, m_copy);
            // no check for either player
            if (!IsCheck(m_copy, state.White) && !IsCheck(m_copy, !state.White))
            {
                // no moves for opponent
                if (!OpenentHasMoves())
                {
                    state.GameOver = true;
                    return true;
                }
            }
            return false;
        }
        /*private bool IsStaleMate();*/

        /// <summary>
        /// If the opponent is in check and have no legal moves then it is checkmate
        /// </summary>
        /// <returns>returns true if it is checkmate and false if it is not</returns>
        private bool IsCheckMate()
        {
            Piece[,] m_copy = new Piece[8, 8];
            MakeCopy(board, m_copy);
            if (IsCheck(m_copy, !state.White))
            {
                if (!OpenentHasMoves())
                {
                    state.GameOver = true;
                    return true;
                }
            }
            return false;
        }
        /*private bool IsCheckMate();*/

        /// <summary>
        /// Loops through the whole board, if the square is empty or the wrong color it just continues
        /// If the piece is the right color then it finds all the legal moves that piece has
        /// Then if checks through that pieces legal moves to check if that square is a king
        /// If it is a king of the opposite color then it is check if the loop finished it is not
        /// </summary>
        /// <param name="a_copy">copy of the board</param>
        /// <param name="a_isWhite">color to check for</param>
        /// <returns>true if a_isWhite is in check, false if they are not</returns>
        private bool IsCheck(Piece[,] a_copy, bool a_isWhite)
        {
            string []m_movesarray;
            for(int m_i = 0; m_i < 8; m_i++)
            {
                for(int m_j = 0; m_j < 8; m_j++)
                {
                    if(a_copy[m_i, m_j].Value == 0|| a_copy[m_i,m_j].White == a_isWhite)
                    {
                        continue;
                    }
                    m_movesarray = FindLegalMoves(a_copy, m_i, m_j).Split(' ', ',');
                    foreach (string m_move in m_movesarray) {
                        if(m_move.Length == 0)
                        {
                            continue;
                        }
                        int m_tempI = Convert.ToInt32(m_move.ElementAt(0)) - 48;
                        int m_tempJ = Convert.ToInt32(m_move.ElementAt(1)) - 48;
                        if(a_copy[m_tempI, m_tempJ].Value == 9 && a_copy[m_tempI, m_tempJ].White == a_isWhite)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        /*private bool IsCheck(Piece[,] a_copy, bool a_isWhite);*/
        
        /// <summary>
        /// Loops through each square on the board, if the square is not white or black then it checks the squares 
        /// around it to figure out what color it is supposed to be and set the color
        /// <remarks>If you are in the corner and the corner 4 pieces are are all green it will not reset that square
        /// Therefore when calling ResetColors it gets called twice just to ensure that edge case is accounted 
        /// for </remarks>
        /// </summary>
        private void ResetColors()
        {
            for (int m_i = 0; m_i < 8; m_i++)
            {
                for (int m_j = 0; m_j < 8; m_j++)
                {
                    if (squares[m_i, m_j].BackColor != Color.Black && squares[m_i, m_j].BackColor != Color.White)
                    {
                        if (m_i - 1 >= 0 && squares[m_i - 1, m_j].BackColor == Color.White)
                        {
                            squares[m_i, m_j].BackColor = Color.Black;
                        }
                        else if (m_i - 1 >= 0 && squares[m_i - 1, m_j].BackColor == Color.Black)
                        {
                            squares[m_i, m_j].BackColor = Color.White;
                        }
                        else if (m_i + 1 < 8 && squares[m_i + 1, m_j].BackColor == Color.White)
                        {
                            squares[m_i, m_j].BackColor = Color.Black;
                        }
                        else if (m_i + 1 < 8 && squares[m_i + 1, m_j].BackColor == Color.Black)
                        {
                            squares[m_i, m_j].BackColor = Color.White;
                        }
                        else if (m_j + 1 < 8 && squares[m_i, m_j + 1].BackColor == Color.White)
                        {
                            squares[m_i, m_j].BackColor = Color.Black;
                        }
                        else if (m_j + 1 < 8 && squares[m_i, m_j + 1].BackColor == Color.Black)
                        {
                            squares[m_i, m_j].BackColor = Color.White;
                        }
                        else if (m_j - 1 >= 0 && squares[m_i, m_j - 1].BackColor == Color.White)
                        {
                            squares[m_i, m_j].BackColor = Color.Black;
                        }
                        else if (m_j - 1 >= 0 && squares[m_i, m_j - 1].BackColor == Color.Black)
                        {
                            squares[m_i, m_j].BackColor = Color.White;
                        }
                    }
                }
            }
        }
        /*private void ResetColors();*/

        /// <summary>
        /// Sets the squares to clickable or not based on parameter passed
        /// Loops through the squares and sets their Enabled property to a_val
        /// </summary>
        /// <param name="a_val">true or false representing whether or not the user can click the squares</param>
        private void Clicks(bool a_val)
        {
            for (int m_i = 0; m_i < 8; m_i++)
            {
                for (int m_j = 0; m_j < 8; m_j++)
                {
                    squares[m_i, m_j].Enabled = a_val;
                }
            }
        }
        /*private void Clicks(bool a_val);*/

        /// <summary>
        /// Copies the value and contents of each index of a_original into a_copy
        /// </summary>
        /// <param name="a_original">original board</param>
        /// <param name="a_copy">copy to be made </param>
        private void MakeCopy(Piece[,] a_original, Piece[,] a_copy)
        {
            for(int m_i = 0; m_i < 8; m_i++)
            {
                for(int m_j = 0; m_j < 8; m_j++)
                {
                    a_copy[m_i, m_j] = a_original[m_i, m_j];
                }
            }
        }
        /*private void MakeCopy(Piece[,] a_original, Piece[,] a_copy);*/

        /// <summary>
        /// This takes the 4 promotion squares and depending on the user color displays either black or white ->
        /// knight, bishop, rook, and queen
        /// </summary>
        /// <param name="a_promotionSquares">2x2 array of squares which appear upon pawn promotion</param>
        private void PopulateSquares(Square[,] a_promotionSquares)
        {
            string m_dir = Directory.GetCurrentDirectory();
            int m_counter = 0;
            char m_pieceColor = '\0';
            if (state.White)
            {
                m_pieceColor = 'W';
            }
            else
            {
                m_pieceColor = 'B';
            }
            m_counter = 0;
            foreach (Square m_square in a_promotionSquares)
            {
                switch (m_counter)
                {
                    case 0:
                        m_square.BackgroundImage = System.Drawing.Image.FromFile(m_dir + "\\Images\\B" + m_pieceColor +".gif"); break;
                    case 1:
                        m_square.BackgroundImage = System.Drawing.Image.FromFile(m_dir + "\\Images\\N" + m_pieceColor + ".gif"); break;
                    case 2:
                        m_square.BackgroundImage = System.Drawing.Image.FromFile(m_dir + "\\Images\\R" + m_pieceColor + ".gif"); break;
                    case 3:
                        m_square.BackgroundImage = System.Drawing.Image.FromFile(m_dir + "\\Images\\Q" + m_pieceColor + ".gif"); break;
                    default:
                        break;
                }
                m_counter++;
            }
        }
        /*private void PopulateSquares(Square[,] a_promotionSquares);*/

        /// <summary>
        /// Hides the 8x8 board so the user can no longer see it
        /// </summary>
        private void HideSquares()
        {
            for(int m_i = 0; m_i < 8; m_i++)
            {
                for(int m_j = 0; m_j < 8; m_j++)
                {
                    squares[m_i, m_j].Hide();
                }
            }
        }
        /*private void HideSquares();*/

        /// <summary>
        /// Shows the 8x8 board so the user can see it
        /// </summary>
        private void ShowSquares()
        {
            for(int m_i = 0; m_i < 8; m_i++)
            {
                for(int m_j = 0; m_j < 8; m_j++)
                {
                    squares[m_i, m_j].Show();
                }
            }
        }
        /*private void ShowSquares();*/

        
        private void Drawing(Piece[,] board)
        {
            char pieceColor = '\0';
            string dir = Directory.GetCurrentDirectory();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j].White == true)
                    {
                        pieceColor = 'W';
                    }
                    else
                    {
                        pieceColor = 'B';
                    }
                    switch (board[i, j].Value)
                    {
                        case 0:
                            squares[i, j].BackgroundImage = null;
                            break;
                        case 1:
                            squares[i, j].BackgroundImage = System.Drawing.Image.FromFile(dir + "\\Images\\p" + pieceColor + ".gif");
                            break;
                        case 3:
                            squares[i, j].BackgroundImage = System.Drawing.Image.FromFile(dir + "\\Images\\B" + pieceColor + ".gif");
                            break;
                        case 4:
                            squares[i, j].BackgroundImage = System.Drawing.Image.FromFile(dir + "\\Images\\N" + pieceColor + ".gif");
                            break;
                        case 5:
                            squares[i, j].BackgroundImage = System.Drawing.Image.FromFile(dir + "\\Images\\R" + pieceColor + ".gif");
                            break;
                        case 8:
                            squares[i, j].BackgroundImage = System.Drawing.Image.FromFile(dir + "\\Images\\Q" + pieceColor + ".gif");
                            break;
                        case 9:
                            squares[i, j].BackgroundImage = System.Drawing.Image.FromFile(dir + "\\Images\\K" + pieceColor + ".gif");
                            break;
                    }
                }
            }
        }

        private void MakeSquares(Square[,] squares)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    squares[i, j] = new Square();
                    squares[i, j].TopLevel = false;
                    squares[i, j].Parent = this;
                    squares[i, j].Location = new Point(j * 55 + 13, i * 55 + 50);
                    squares[i, j].posX = j;
                    squares[i, j].posY = i;
                    squares[i, j].Size = new Size(50, 50);
                    squares[i, j].Click += new EventHandler(Square_Click);
                    if (i % 2 == 0)
                    {
                        if (j % 2 == 1)
                        {
                            squares[i, j].BackColor = Color.Black;
                        }
                        else
                        {
                            squares[i, j].BackColor = Color.White;
                        }
                    }
                    else
                    {
                        if (j % 2 == 1)
                        {
                            squares[i, j].BackColor = Color.White;
                        }
                        else
                        {
                            squares[i, j].BackColor = Color.Black;
                        }
                    }
                    squares[i, j].BackgroundImageLayout = ImageLayout.Center;
                    squares[i, j].Show();
                }
            }
        }

        private void ReverseBoard()
        {
            Piece[,] temp = new Piece[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    temp[i, j] = board[i, j];
                }
            }
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board[7 - i, 7 - j] = temp[i, j];
                }
            }
            state.Board = board;
        }


        private void PromotePawn(int i, int j)
        {
            Clicks(false);
            int counter = 0;
            HideSquares();
            for(int x = 0; x < 2; x++)
            {
                for(int y = 0; y < 2; y++)
                {
                    promotionSquares[x, y] = new Square();
                    promotionSquares[x, y].TopLevel = false;
                    promotionSquares[x, y].Parent = this;
                    promotionSquares[x, y].Location = new Point(x * 50 + 150, y * 50 + 150);
                    promotionSquares[x, y].posX = 100 + counter;
                    promotionSquares[x, y].posY = 100 + counter;
                    promotionSquares[x, y].Size = new Size(50, 50);
                    promotionSquares[x, y].Click += new EventHandler(Square_Click);
                    if (counter % 2 == 0)
                    {
                        promotionSquares[x, y].BackColor = Color.Black;
                    }
                    else
                    {
                        promotionSquares[x, y].BackColor = Color.White;
                    }
                    promotionSquares[x, y].BackgroundImageLayout = ImageLayout.Center;
                    promotionSquares[x, y].Show();
                    counter++;
                }
            }
            PopulateSquares(promotionSquares);
        }

        private void MakeMove(int i, int j, int tempI, int tempJ)
        {
            if (board[tempI, tempJ].Value == 9)
            {
                if (tempJ - 2 == j)
                {
                    board[i, j + 1] = board[i, 0];
                    board[i, j + 1].HasMoved = true;
                    board[i, 0] = new Piece();
                }
                else if (tempJ + 2 == j)
                {
                    board[i, j - 1] = board[i, 7];
                    board[i, j - 1].HasMoved = true;
                    board[i, 7] = new Piece();
                }
                board[i, j] = board[tempI, tempJ];
                board[tempI, tempJ] = new Piece();
                board[i, j].HasMoved = true;
            }
            else if (board[tempI, tempJ].Value == 1 && Math.Abs(i - tempI) == 2)
            {
                board[i, j] = board[tempI, tempJ];
                board[tempI, tempJ] = new Piece();
                board[i, j].HasMoved = false;
            }
            else if(board[tempI, tempJ].Value == 1 && i != tempI && j != tempJ && board[i,j].Value == 0)
            {
                board[i, j] = board[tempI, tempJ];
                board[tempI, tempJ] = new Piece();
                board[i, j].HasMoved = true;
                board[tempI, j] = new Piece();
            }
            else if(board[tempI, tempJ].Value == 1 && (i == 7 || i == 0))
            {
                board[i, j] = board[tempI, tempJ];
                board[tempI, tempJ] = new Piece();
                PromotePawn(i, j);
            }
            else
            {
                board[i, j] = board[tempI, tempJ];
                board[tempI, tempJ] = new Piece();
                board[i, j].HasMoved = true;
            }
        }

        private void ParseForPawns()
        {
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    if (board[i, j].HasMoved)
                    {
                        continue;
                    }
                    if(board[i, j].Value == 1 && board[i,j].White == state.White && i != 6 && i != 1)
                    {
                        board[i, j].HasMoved = true;
                    }
                }
            }
        }

        private KeyValuePair<int, int> PawnLocation()
        {
            KeyValuePair<int, int> returnVal = new KeyValuePair<int, int>(-1, -1);
            for(int i = 0; i < 8; i++)
            {
                if(board[0, i].Value == 1)
                {
                    return new KeyValuePair<int, int>(0, i);
                }
                else if(board[7, i].Value == 1)
                {
                    return new KeyValuePair<int, int>(7, i);
                }
            }
            return returnVal;
        }

        private string GetOtherPieces(int destI, int destJ, int origI, int origJ)
        {
            string otherSquares = String.Empty;
            string[] otherStrings;
            Piece[,] copy = new Piece[8, 8];
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    if(state.Board[i,j].Value != state.Board[origI, origJ].Value || state.Board[i,j].White != state.Board[origI, origJ].White)
                    {
                        continue;
                    }
                    MakeCopy(state.Board, copy);
                    otherSquares = FindLegalMoves(copy, i, j);
                    otherStrings = otherSquares.Split(' ', ',');
                    foreach(string square in otherStrings)
                    {
                        if (square.Length < 2)
                        {
                            continue;
                        }
                        int destI2 = Convert.ToInt32(square.ElementAt(0) - 48);
                        int destJ2 = Convert.ToInt32(square.ElementAt(1) - 48);
                        if(destI2 == destI && destJ2 == destJ)
                        {
                            if ((i != origI || j != origJ) && otherSquares != String.Empty)
                            {
                                return i.ToString() + j.ToString();
                            }
                        }
                    }
                    otherSquares = String.Empty;
                }
            }
            return otherSquares;
        }

        private char ConvertJToLetter(int j)
        {
            char toRet = '\0';
            if (!state.White)
            {
                j = 7 - j;
            }
            switch (j)
            {
                case 0:
                    toRet = 'a';
                    break;
                case 1:
                    toRet = 'b';
                    break;
                case 2:
                    toRet = 'c';
                    break;
                case 3:
                    toRet = 'd';
                    break;
                case 4:
                    toRet = 'e';
                    break;
                case 5:
                    toRet = 'f';
                    break;
                case 6:
                    toRet = 'g';
                    break;
                case 7:
                    toRet = 'h';
                    break;
                default:
                    break;
            }
            return toRet;
        }

        private void AddToNotation(int destI, int destJ, int origI, int origJ)
        {
            string move = String.Empty;
            if (state.AllPositions.Count % 2 == 1)
            {
                move += (state.AllPositions.Count / 2 + 1).ToString() + ".  ";
            }
            switch(state.Board[origI, origJ].Value)
            {
                case 1:
                    break;
                case 3:
                    move += 'B';
                    break;
                case 4:
                    move += 'N';
                    break;
                case 5:
                    move += 'R';
                    break;
                case 8:
                    move += 'Q';
                    break;
                case 9:
                    if(Math.Abs(destJ - origJ) > 1)
                    {
                        if((!state.White && destJ != 5) || (state.White && destJ == 6))
                        {
                            move += "O-O";
                        }
                        else
                        {
                            move += "O-O-O";
                        }
                        break;
                    }
                    move += 'K';
                    break;
                default:
                    break;
            }
            string others = GetOtherPieces(destI, destJ, origI, origJ);
            if (others != String.Empty && move[0] != 'O')
            {
                int i = Convert.ToInt32(others.ElementAt(0) - 48);
                int j = Convert.ToInt32(others.ElementAt(1) - 48);
                if(i == origI)
                {
                    move += ConvertJToLetter(origJ);
                }
                else if(j != origJ)
                {
                    move += ConvertJToLetter(origJ);
                }
                else
                {
                    move += (origI + 1).ToString();
                }
            }
            if(move.Length == 0 || move.ElementAt(move.Length-1) != 'O')
            {
                if (state.Board[destI, destJ].Value > 0)
                {
                    move += 'x';
                }
                move += ConvertJToLetter(destJ);
                if (state.White)
                {
                    destI = 7 - destI;
                }
                move += (destI + 1).ToString();
            }
            state.Notation += move;
        }

        private void AddExtrasToNotation()
        {
            string extra = String.Empty;
            Piece[,] copy = new Piece[8, 8];
            MakeCopy(state.Board, copy);
            if (state.CheckMate)
            {
                extra += "#";
            }
            else if (IsCheck(copy, !state.White))
            {
                extra += "+";
            }
            if (state.White)
            {
                extra += '\t';
            }
            else
            {
                extra += "\r\n";
            }
            state.Notation += extra;
            textBoxNotation.Text = state.Notation;
        }

        private bool CheckForKingsOnly()
        {
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    if(state.Board[i,j].Value != 0 && state.Board[i,j].Value != 9)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        void Square_Click(object sender, EventArgs e)
        {
            if ((state.White && !state.WhiteToMove) || (!state.White && state.WhiteToMove))
            {
                Clicks(false);
                return;
            }
            else
            {
                Clicks(true);
            }
            try
            {
                if (state.WaitingForSecondPlayer)
                {
                    Clicks(false);
                    return;
                }
                else
                {
                    Clicks(true);
                }
                int i = (sender as Square).posY;
                int j = (sender as Square).posX;
                if(i >= 100 && j >= 100)
                {
                    KeyValuePair<int, int> pair = PawnLocation();
                    int x = pair.Key;
                    int y = pair.Value;
                    board[x, y] = new Piece();
                    switch (i)
                    {
                        case 100:
                            board[x, y].Value = 3;
                            break;
                        case 101:
                            board[x, y].Value = 4;
                            break;
                        case 102:
                            board[x, y].Value = 5;
                            break;
                        case 103:
                            board[x, y].Value = 8;
                            break;
                        default:
                            break;
                    }
                    board[x, y].White = state.White;
                    board[x, y].HasMoved = true;
                    foreach(Square square in promotionSquares)
                    {
                        square.Hide();
                    }
                    ShowSquares();
                    Drawing(board);
                    state.Board = board;
                    Drawing(board);
                    Clicks(false);
                    if (IsCheckMate())
                    {
                        state.GameOver = true;
                        state.CheckMate = true;
                    }
                    else if (IsStaleMate())
                    {
                        state.GameOver = true;
                        state.StaleMate = true;
                    }
                    if (!state.White)
                    {
                        ReverseBoard();
                    }
                    string json = JsonConvert.SerializeObject(state);
                    sw.WriteLine(json);
                    sw.Flush();
                    if (!state.White)
                    {
                        ReverseBoard();
                    }
                    origin = String.Empty;
                    if (state.GameOver)
                    {
                        OfferNewGame();
                    }
                }
                else if (origin == String.Empty)
                {
                    if(board[i, j].White == state.White && board[i,j].Value != 0)
                    {
                        origin += i.ToString() + j.ToString();
                        ParseAndHighlight(i, j);
                    }
                }
                else
                {
                    if (squares[i, j].BackColor != Color.Black && squares[i, j].BackColor != Color.White)
                    {
                        state.WhiteToMove = !state.WhiteToMove;
                        int tempI = Convert.ToInt32(origin.ElementAt(0) - 48);
                        int tempJ = Convert.ToInt32(origin.ElementAt(1) - 48);
                        ParseForPawns();
                        AddToNotation(i, j, tempI, tempJ);
                        MakeMove(i, j, tempI, tempJ);
                        AddExtrasToNotation();
                        state.Board = board;
                        if(!state.White)
                        {
                            ReverseBoard();
                        }
                        state.AddToAllPositions(board);
                        if(!state.White)
                        {
                            ReverseBoard();
                        }
                        indexShowing++;
                        currPosition++;
                        ResetColors();
                        ResetColors();
                        Drawing(board);
                        Clicks(false);
                        if (IsCheckMate())
                        {
                            state.CheckMate = true;
                            state.GameOver = true;
                        }
                        else if (IsStaleMate())
                        {
                            state.StaleMate = true;
                            state.GameOver = true;
                        }
                        if (!state.White)
                        {
                            ReverseBoard();
                        }
                        string json = JsonConvert.SerializeObject(state);
                        sw.WriteLine(json);
                        sw.Flush();
                        if (!state.White)
                        {
                            ReverseBoard();
                        }
                        origin = String.Empty;
                        if (state.GameOver)
                        {
                            OfferNewGame();
                        }
                    }
                    else
                    {
                        origin = String.Empty;
                        ResetColors();
                    }
                }

            }
            catch (Exception except)
            {
                MessageBox.Show(except.Message);
            }
        }


        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ip);
                byte[] buffer = new byte[10];
                sbyte byte1 = Convert.ToSByte(state.TimePortOffset);
                buffer[0] = Convert.ToByte(byte1);
                socket.Send(buffer, SocketFlags.None);
                ns = new NetworkStream(socket);
                sr = new StreamReader(ns);
                sw = new StreamWriter(ns);
                messageFromServer = sr.ReadLine();
                GameState temp = new GameState();
                temp = JsonConvert.DeserializeObject<GameState>(messageFromServer);
                temp.WhiteTimeLeft = state.WhiteTimeLeft;
                temp.BlackTimeLeft = state.BlackTimeLeft;
                state = temp;
                button1.Visible = false;
                ThreeHour.Visible = false;
                OneHour.Visible = false;
                ThirtyMin.Visible = false;
                FifteenMin.Visible = false;
                TenMin.Visible = false;
                FiveMin.Visible = false;
                OneMin.Visible = false;
                button1.Width = 0;
                button1.Height = 0;
                board = state.Board;
                if (playerName == String.Empty)
                {
                    if (state.White)
                    {
                        playerName = "White";
                    }
                    else
                    {
                        playerName = "Black";
                    }
                }
                Label2.Text = playerName;
                if (!state.White)
                {
                    ReverseBoard();
                }
                MakeSquares(squares);
                Drawing(board);
                if (!state.White && state.WhiteToMove)
                {
                    Clicks(false);
                }
                else
                {
                    if (!temp.WaitingForSecondPlayer)
                    {
                        Timer1.Start();
                    }
                }
                Thread t = new Thread(ProcessServerMessages);
                t.Start();
            }
            catch (Exception a)
            {
                MessageBox.Show(a.Message);
                Environment.Exit(0);
            }
        }

        private void Chess_Load(object sender, EventArgs e)
        {
            playerName = Microsoft.VisualBasic.Interaction.InputBox("Enter your name", "Name");
            state.TimePortOffset = 75;
        }

        private void ButtonSendMessage_Click(object sender, EventArgs e)
        {
            if(textBoxInput.Text!= String.Empty)
            {
                string message = playerName + ": " + textBoxInput.Text + "\r\n";
                state.Chat += message;
                textBoxChat.Text = state.Chat;
                textBoxInput.Text = String.Empty;
                if (!state.White)
                {
                    ReverseBoard();
                }
                message = JsonConvert.SerializeObject(state);
                sw.WriteLine(message);
                if (!state.White)
                {
                    ReverseBoard();
                }
                sw.Flush();
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            LowerTimeLabel.Text = String.Empty;
            UpperTimeLabel.Text = String.Empty;
            if (state.WhiteToMove)
            {
                state.WhiteTimeLeft--;
            }
            else
            {
                state.BlackTimeLeft--;
            }
            if(state.WhiteTimeLeft > 0 && state.BlackTimeLeft > 0)
            {
                if (state.White)
                {
                    LowerTimeLabel.Text = state.WhiteTimeLeftToString();
                    UpperTimeLabel.Text = state.BlackTimeLeftToString();
                }
                else
                {
                    LowerTimeLabel.Text = state.BlackTimeLeftToString();
                    UpperTimeLabel.Text = state.WhiteTimeLeftToString();
                }
            }
            else
            {
                Timer1.Stop();
                OfferNewGame();
            }
        }


        // radio buttons setting time variables
        private void OneMin_CheckedChanged(object sender, EventArgs e)
        {
            state.WhiteTimeLeft = 60;
            state.BlackTimeLeft = 60;
            state.TimePortOffset = 1;
        }

        private void FiveMin_CheckedChanged(object sender, EventArgs e)
        {
            state.WhiteTimeLeft = 300;
            state.BlackTimeLeft = 300;
            state.TimePortOffset = 5;
        }

        private void TenMin_CheckedChanged(object sender, EventArgs e)
        {
            state.WhiteTimeLeft = 600;
            state.BlackTimeLeft = 600;
            state.TimePortOffset = 10;
        }

        private void FifteenMin_CheckedChanged(object sender, EventArgs e)
        {
            state.WhiteTimeLeft = 900;
            state.BlackTimeLeft = 900;
            state.TimePortOffset = 15;
        }

        private void ThirtyMin_CheckedChanged(object sender, EventArgs e)
        {
            state.WhiteTimeLeft = 1800;
            state.BlackTimeLeft = 1800;
            state.TimePortOffset = 30;
        }

        private void OneHour_CheckedChanged(object sender, EventArgs e)
        {
            state.WhiteTimeLeft = 3600;
            state.BlackTimeLeft = 3600;
            state.TimePortOffset = 60;
        }

        private void ThreeHour_CheckedChanged(object sender, EventArgs e)
        {
            state.WhiteTimeLeft = 10800;
            state.BlackTimeLeft = 10800;
            state.TimePortOffset = 75;
        }


        // buttons showing different boards
        private void ButtonStartOfGame_Click(object sender, EventArgs e)
        {
            board = state.AllPositions.ElementAt(0);
            if (!state.White)
            {
                ReverseBoard();
            }
            Drawing(board);
            indexShowing = 0;
            if(currPosition == 0)
            {
                Clicks(true);
                ButtonForwardOne.Enabled = false;
                ButtonCurrentMove.Enabled = false;
            }
            else
            {
                Clicks(false);
                ButtonStartOfGame.Enabled = false;
                ButtonBackOne.Enabled = false;
                ButtonForwardOne.Enabled = true;
                ButtonCurrentMove.Enabled = true;
            }
        }

        private void ButtonBackOne_Click(object sender, EventArgs e)
        {
            if(indexShowing == 0)
            {
                ButtonStartOfGame.Enabled = false;
                ButtonBackOne.Enabled = false;
                ButtonForwardOne.Enabled = true;
                ButtonCurrentMove.Enabled = true;
                return;
            }
            indexShowing--;
            board = state.AllPositions.ElementAt(indexShowing);
            if(!state.White)
            {
                ReverseBoard();
            }
            Drawing(board);
            Clicks(false);
            if (indexShowing == 0)
            {
                ButtonStartOfGame.Enabled = false;
                ButtonBackOne.Enabled = false;
                ButtonForwardOne.Enabled = true;
                ButtonCurrentMove.Enabled = true;
                return;
            }
            else if(indexShowing < state.AllPositions.Count - 1)
            {
                ButtonStartOfGame.Enabled = true;
                ButtonBackOne.Enabled = true;
                ButtonForwardOne.Enabled = true;
                ButtonCurrentMove.Enabled = true;
            }
        }

        private void ButtonForwardOne_Click(object sender, EventArgs e)
        {
            if(indexShowing == state.AllPositions.Count - 1)
            {
                if(state.White == state.WhiteToMove)
                {
                    Clicks(true);
                }
                ButtonStartOfGame.Enabled = true;
                ButtonBackOne.Enabled = true;
                ButtonForwardOne.Enabled = false;
                ButtonCurrentMove.Enabled = false;
                return;
            }
            indexShowing++;
            board = state.AllPositions.ElementAt(indexShowing);
            if(!state.White)
            {
                ReverseBoard();
            }
            Drawing(board);
            if (indexShowing == state.AllPositions.Count - 1)
            {
                if(state.White == state.WhiteToMove)
                {
                    Clicks(true);
                }
                ButtonStartOfGame.Enabled = true;
                ButtonBackOne.Enabled = true;
                ButtonForwardOne.Enabled = false;
                ButtonCurrentMove.Enabled = false;
                return;
            }
            else if(indexShowing > 0)
            {
                ButtonStartOfGame.Enabled = true;
                ButtonBackOne.Enabled = true;
                ButtonForwardOne.Enabled = true;
                ButtonCurrentMove.Enabled = true;
            }
        }

        private void ButtonCurrentMove_Click(object sender, EventArgs e)
        {
            indexShowing = state.AllPositions.Count - 1;
            if(state.White && state.WhiteToMove)
            {
                Clicks(true);
            }
            ButtonStartOfGame.Enabled = true;
            ButtonBackOne.Enabled = true;
            ButtonForwardOne.Enabled = false;
            ButtonCurrentMove.Enabled = false;
            board = state.AllPositions.ElementAt(indexShowing);
            if(!state.White)
            {
                ReverseBoard();
            }
            Drawing(board);
        }
    }
}