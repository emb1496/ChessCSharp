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
        /// ChessGUI.Chess.Chess()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.Chess - Initializes Chess GUI
        ///     
        /// SYNOPIS
        /// 
        ///     public Chess();
        ///     
        /// DESCRIPTION
        /// 
        ///     Becuase on initialization of the component the 3 hour game is checked it sets the time left to 3 hours each and then
        ///     Initializes the Form
        ///     
        /// RETURNS
        /// 
        ///     No Return
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
        ///     
        /// </summary>
        public Chess()
        {
            state.WhiteTimeLeft = 10800;
            state.BlackTimeLeft = 10800;
            InitializeComponent();
        }
        /*public Chess();*/

        /// <summary>
        ///     Checks if 3 given boards are the same by checking each locations value and color
        /// </summary>
        /// ChessGUI.Chess.IsSameBoard()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.IsSameBoard - Check for same or different boards
        ///     
        /// SYNOPIS
        /// 
        ///     private bool IsSameBoard(Piece[,] a_board1, Piece[,] a_board2);
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
        ///     
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
        ///     Only called once game is over, this checks why the game ended and builds a message to the user from that
        ///     Then it displays an interactive message box, if the user selects no then it will close the application
        ///     If the user selects yes it will reset to initial state and start again
        /// </summary>
        /// ChessGUI.Chess.OfferNewGame()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.OfferNewGame - Offers user to play again
        ///     
        /// SYNOPIS
        /// 
        ///     private void OfferNewGame();
        ///         
        /// RETURNS
        /// 
        ///     void
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
        ///     
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
        ///     In a forever loop this method calls ReadLine from the server then deserializes the message into a gamestate
        ///     It then resets the current position and index showing to correct values
        ///     If the game is over naturally it will call OfferNewGame
        ///     Otherwise it invokes an action where it enables review buttons makes sure the timer is running and updates the board and notation and chat
        ///     If the game is over due to server error or oppenent disconnect it will stop the timer and call OfferNewGame
        /// </summary>
        /// ChessGUI.Chess.ProcessServerMessages()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.ProcessServerMessages - Recieves message from server and does makes necessary changes
        ///     
        /// SYNOPIS
        /// 
        ///     private void OfferNewGame();
        ///     
        /// RETURNS
        /// 
        ///     void
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
        ///     
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
        ///     Accepts a copy of a board and an index checks if the pawn is on its initial starting point and can move 2 spots
        ///     Then checks for a one square move then checks for empessant then checks for a regular take
        /// </summary>
        /// ChessGUI.Chess.PawnLegalMoves()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.PawnLegalMoves - finds all legal moves a pawn has
        ///     
        /// SYNOPIS
        /// 
        ///     private string PawnLegalMoves(Piece[,] a_board, int a_i, int a_j);
        ///         
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
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
        ///     uses short-circuit evaluation to check 8 possible indexes on the board copy to see if the knight can be moved there
        /// </summary>
        /// ChessGUI.Chess.KnightLegalMoves()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.KnightLegalMoves - finds all legal moves a knight has
        ///     
        /// SYNOPIS
        /// 
        ///     private string KnightLegalMoves(Piece[,] a_board, int a_i, int a_j);
        ///         
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
        /// <param name="a_board">board copy</param>
        /// <param name="a_i">row index</param>
        /// <param name="a_j">col index</param>
        /// <returns>string of comma seperated indexes on the board that the piece can move</returns>
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
        ///     Uses short-circuit evaluation to preform checks boundaries on diagonal moves for bishop for as long as there are empty squares it continues
        ///     once it hits a piece if hits the same color it doesnt add it but once it hits a piece of the opposite color it will add it as the ast move
        /// </summary>
        /// ChessGUI.Chess.BishopLegalMoves()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.BishopLegalMoves - finds all legal moves a bishop has
        ///     
        /// SYNOPIS
        /// 
        ///     private string BishopLegalMoves(Piece[,] a_board, int a_i, int a_j);
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
        /// <param name="a_board">board copy</param>
        /// <param name="a_i">row index</param>
        /// <param name="a_j">col index</param>
        /// <returns>string of comma seperated indexes on the board that the piece can move</returns>
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
        ///     Uses short-circuit evaluation to preform boundary checks for all direction, right, left, up, and down
        /// </summary>
        /// ChessGUI.Chess.RookLegalMoves()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.RookLegalMoves - finds all legal moves a rook has
        ///     
        /// SYNOPIS
        /// 
        ///     private string RookLegalMoves(Piece[,] a_board, int a_i, int a_j);
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
        /// <param name="a_board">board copy</param>
        /// <param name="a_i">row index</param>
        /// <param name="a_j">col index</param>
        /// <returns>string of comma seperated indexes on the board that the piece can move</returns>
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
        ///     Combines the legal rook and bishop moves together and returns them
        /// </summary>
        /// ChessGUI.Chess.QueenLegalMoves()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.QueenLegalMoves - finds all legal moves a Queen has
        ///     
        /// SYNOPIS
        /// 
        ///     private string QueenLegalMoves(Piece[,] a_board, int a_i, int a_j);
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
        /// <param name="a_board">board copy</param>
        /// <param name="a_i">row index</param>
        /// <param name="a_j">col index</param>
        /// <returns>string of comma seperated indexes on the board that the piece can move</returns>
        private string QueenLegalMoves(Piece[,] a_board, int a_i, int a_j)
        {
            string m_moves = String.Empty;
            m_moves = BishopLegalMoves(a_board, a_i, a_j);
            m_moves += RookLegalMoves(a_board, a_i, a_j);
            return m_moves;
        }
        /*private string QueenLegalMoves(Piece[,] a_board, int a_i, int a_j);*/

        /// <summary>
        ///     Method returns all legal king moves
        ///     First it checks the 8 squares immediately around the king
        ///     Then if the king has not moved it checks if the king can castle
        ///     The rules to not be allowed to castle are if the king or rook has moved or if the king is currently or will castle through check
        /// </summary>
        /// ChessGUI.Chess.KingLegalMoves()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.KingLegalMoves - finds all legal moves a king has
        ///     
        /// SYNOPIS
        /// 
        ///     private string KingLegalMoves(Piece[,] a_board, int a_i, int a_j);
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
        /// <param name="a_board">board copy</param>
        /// <param name="a_i">row index</param>
        /// <param name="a_j">col index</param>
        /// <returns>string of comma seperated indexes on the board that the piece can move</returns>
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
        ///     This method returns if the opponent has moves, the idea being that if the opponent has no moves its either checkmate or stalemate
        ///     Loops through every square and checks that the square is not empty and is not your piece
        ///     Then calls find legal moves for that square
        ///     If there is a legal move for that square it will just return true
        ///     If the loop ends it will return false
        /// </summary>
        /// ChessGUI.Chess.OpponentHasMoves()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.OpponentHasMoves - finds if other player has a move
        ///     
        /// SYNOPIS
        /// 
        ///     private bool OppenentHasMoves();
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
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
        ///     Based on piece value it returns the appropriate function call
        /// </summary>
        /// ChessGUI.Chess.FindLegalMoves()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.FindLegalMoves - finds moves on a board
        ///     
        /// SYNOPIS
        /// 
        ///     private string FindLegalMoves(Piece[,] a_tempBoard, int a_i, int a_j);
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
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
        ///     Highlights all the moves for a given index of the board (a_i, a_j)
        ///     First it calls FindLegalMoves for the index and splits the result into an array of moves
        ///     Then for every move as long as it is a move it will convert the 2 characters into integers which represent indexes
        ///     Then parse and highlight will check if castling is a possibility by copying the board and moving the king those 2 spots and checking if either would be castling through check
        ///     Then it simply makes a copy of the board and makes the move on the copy and checks that you are not in check after you make the move in the copy
        ///     If you are not in check it highlights that destination square to a green color
        /// </summary>
        /// ChessGUI.Chess.ParseAndHighlight()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.ParseAndHighlight - highlights moves on a board
        ///     
        /// SYNOPIS
        /// 
        ///     private void ParseAndHighlight(int a_i, int a_j);
        ///    
        /// RETURNS
        /// 
        ///     void
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
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
        ///     Checks if the board is in stalemate,
        ///     If there is only kings on the board it is an automatic draw
        ///     Then if it is not check for either piece and the opponent has no moves it is stalemate
        /// </summary>
        /// ChessGUI.Chess.IsStaleMate()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.IsStaleMate - Checks for stalemate
        ///     
        /// SYNOPIS
        /// 
        ///     private bool IsStaleMate();
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
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
        ///     If the opponent is in check and have no legal moves then it is checkmate
        /// </summary>
        /// ChessGUI.Chess.IsCheckMate()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.IsCheckMate - checks if its checkmate
        ///     
        /// SYNOPIS
        /// 
        ///     private bool IsCheckMate();
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
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
        ///     Loops through the whole board, if the square is empty or the wrong color it just continues
        ///     If the piece is the right color then it finds all the legal moves that piece has
        ///     Then if checks through that pieces legal moves to check if that square is a king
        ///     If it is a king of the opposite color then it is check if the loop finished it is not
        /// </summary>
        /// ChessGUI.Chess.IsCheck()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.IsCheck - checks if its check
        ///     
        /// SYNOPIS
        /// 
        ///     private bool IsCheck(Piece[,] a_copy, bool a_isWhite);
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
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
        ///     Loops through each square on the board, if the square is not white or black then it checks the squares 
        ///     around it to figure out what color it is supposed to be and set the color
        /// <remarks>If you are in the corner and the corner 4 pieces are are all green it will not reset that square
        /// Therefore when calling ResetColors it gets called twice just to ensure that edge case is accounted for
        /// </remarks>
        /// </summary>
        /// ChessGUI.Chess.ResetColors()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.ResetColors - resets the board to checkered colors
        ///     
        /// SYNOPIS
        /// 
        ///     private void ResetColors();
        ///     
        /// RETURNS
        /// 
        ///     void
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
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
        ///     Sets the squares to clickable or not based on parameter passed
        ///     Loops through the squares and sets their Enabled property to a_val
        /// </summary>
        /// ChessGUI.Chess.Clicks()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.Clicks - makes the board clickable or not
        ///     
        /// SYNOPIS
        /// 
        ///     private void Clicks(bool a_val);
        ///     
        /// RETURNS
        /// 
        ///     void
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
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
        ///     Copies the value and contents of each index of a_original into a_copy
        /// </summary>
        /// ChessGUI.Chess.MakeCopy()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.MakeCopy - makes a copy of a board
        ///     
        /// SYNOPIS
        /// 
        ///     private void MakeCopy(Piece[,] a_original, Piece[,] a_copy);
        ///     
        /// RETURNS
        /// 
        ///     void
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
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
        ///     This takes the 4 promotion squares and depending on the user color displays either black or white ->
        ///     knight, bishop, rook, and queen
        /// </summary>
        /// ChessGUI.Chess.PopulateSquares()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.PopulateSquares - shows the pieces on promotion squares
        ///     
        /// SYNOPIS
        /// 
        ///     private void PopulateSquares(Square[,] a_promotionSquares);
        ///     
        /// RETURNS
        /// 
        ///     void
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
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
        ///     Hides the 8x8 board so the user can no longer see it
        /// </summary>
        /// ChessGUI.Chess.HideSquares()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.HideSquares - hides the board
        ///     
        /// SYNOPIS
        /// 
        ///     private void HideSquares();
        ///     
        /// RETURNS
        /// 
        ///     void
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
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
        ///     Shows the 8x8 board so the user can see it
        /// </summary>
        /// ChessGUI.Chess.ShowSquares()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.ShowSquares - shows the board
        ///     
        /// SYNOPIS
        /// 
        ///     private void ShowSquares();
        ///     
        /// RETURNS
        /// 
        ///     void
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
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

        /// <summary>
        ///     based on the board passed in this method will loop through the board and based on the color and value of the piece 
        ///     it will display the appropriate image
        /// </summary>
        /// ChessGUI.Chess.Drawing()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.Drawing - drawes the pictures of the pieces on the squares where they belong
        ///     
        /// SYNOPIS
        /// 
        ///     private void Drawing(Piece[,] a_board);
        ///     
        /// RETURNS
        /// 
        ///     void
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
        /// <param name="a_board">8x8 chess board to draw</param>
        private void Drawing(Piece[,] a_board)
        {
            char m_pieceColor = '\0';
            string m_dir = Directory.GetCurrentDirectory();
            for (int m_i = 0; m_i < 8; m_i++)
            {
                for (int m_j = 0; m_j < 8; m_j++)
                {
                    if (a_board[m_i, m_j].White == true)
                    {
                        m_pieceColor = 'W';
                    }
                    else
                    {
                        m_pieceColor = 'B';
                    }
                    switch (a_board[m_i, m_j].Value)
                    {
                        case 0:
                            squares[m_i, m_j].BackgroundImage = null;
                            break;
                        case 1:
                            squares[m_i, m_j].BackgroundImage = System.Drawing.Image.FromFile(m_dir + "\\Images\\p" + m_pieceColor + ".gif");
                            break;
                        case 3:
                            squares[m_i, m_j].BackgroundImage = System.Drawing.Image.FromFile(m_dir + "\\Images\\B" + m_pieceColor + ".gif");
                            break;
                        case 4:
                            squares[m_i, m_j].BackgroundImage = System.Drawing.Image.FromFile(m_dir + "\\Images\\N" + m_pieceColor + ".gif");
                            break;
                        case 5:
                            squares[m_i, m_j].BackgroundImage = System.Drawing.Image.FromFile(m_dir + "\\Images\\R" + m_pieceColor + ".gif");
                            break;
                        case 8:
                            squares[m_i, m_j].BackgroundImage = System.Drawing.Image.FromFile(m_dir + "\\Images\\Q" + m_pieceColor + ".gif");
                            break;
                        case 9:
                            squares[m_i, m_j].BackgroundImage = System.Drawing.Image.FromFile(m_dir + "\\Images\\K" + m_pieceColor + ".gif");
                            break;
                    }
                }
            }
        }
        /*private void Drawing(Piece[,] a_board);*/

        /// <summary>
        ///     Fills a_squares with 64 squares, sets their locations and attributes adds onClick event handler and sets the color
        ///     and image location to center
        /// </summary>
        /// ChessGUI.Chess.MakeSquares()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.MakeSquares - makes the board
        ///     
        /// SYNOPIS
        /// 
        ///     private void MakeSquares(Square[,] a_squares);
        ///     
        /// RETURNS
        /// 
        ///     void
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
        /// <param name="a_squares">8x8 array of Square</param>
        private void MakeSquares(Square[,] a_squares)
        {
            for (int m_i = 0; m_i < 8; m_i++)
            {
                for (int m_j = 0; m_j < 8; m_j++)
                {
                    a_squares[m_i, m_j] = new Square
                    {
                        TopLevel = false,
                        Parent = this,
                        Location = new Point(m_j * 55 + 13, m_i * 55 + 50),
                        posX = m_j,
                        posY = m_i,
                        Size = new Size(50, 50)
                    };
                    a_squares[m_i, m_j].Click += new EventHandler(Square_Click);
                    if (m_i % 2 == 0)
                    {
                        if (m_j % 2 == 1)
                        {
                            a_squares[m_i, m_j].BackColor = Color.Black;
                        }
                        else
                        {
                            a_squares[m_i, m_j].BackColor = Color.White;
                        }
                    }
                    else
                    {
                        if (m_j % 2 == 1)
                        {
                            a_squares[m_i, m_j].BackColor = Color.White;
                        }
                        else
                        {
                            a_squares[m_i, m_j].BackColor = Color.Black;
                        }
                    }
                    a_squares[m_i, m_j].BackgroundImageLayout = ImageLayout.Center;
                    a_squares[m_i, m_j].Show();
                }
            }
        }
        /*private void MakeSquares(Square[,] a_squares)*/

        /// <summary>
        ///     Reverses the board so that each original index maps to 7-index,
        ///     So we loop 0,0 -> 7,7 and make a copy of the board
        ///     Then we loop 7,7 -> 0,0 and set it to 7-row, 7-col
        /// </summary>
        /// ChessGUI.Chess.ReverseBoard()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.ReverseBoard - reverses the board
        ///     
        /// SYNOPIS
        /// 
        ///     private void ReverseBoard();
        ///     
        /// RETURNS
        /// 
        ///     void
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
        private void ReverseBoard()
        {
            Piece[,] m_temp = new Piece[8, 8];
            for (int m_i = 0; m_i < 8; m_i++)
            {
                for (int m_j = 0; m_j < 8; m_j++)
                {
                    m_temp[m_i, m_j] = board[m_i, m_j];
                }
            }
            for (int m_i = 0; m_i < 8; m_i++)
            {
                for (int m_j = 0; m_j < 8; m_j++)
                {
                    board[7 - m_i, 7 - m_j] = m_temp[m_i, m_j];
                }
            }
            state.Board = board;
        }
        /*private void ReverseBoard();*/


        /// <summary>
        /// Sets clicks to false, hides the board, then makes the promotion squares and calls PopulateSquares()
        /// </summary>
        /// ChessGUI.Chess.PromotePawn()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.PromotePawn - called when pawn gets to the end of the board
        ///     
        /// SYNOPIS
        /// 
        ///     private void PromotePawn();
        ///     
        /// RETURNS
        /// 
        ///     void
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
        private void PromotePawn()
        {
            Clicks(false);
            int m_counter = 0;
            HideSquares();
            for(int m_x = 0; m_x < 2; m_x++)
            {
                for(int m_y = 0; m_y < 2; m_y++)
                {
                    promotionSquares[m_x, m_y] = new Square
                    {
                        TopLevel = false,
                        Parent = this,
                        Location = new Point(m_x * 50 + 150, m_y * 50 + 150),
                        posX = 100 + m_counter,
                        posY = 100 + m_counter,
                        Size = new Size(50, 50)
                    };
                    promotionSquares[m_x, m_y].Click += new EventHandler(Square_Click);
                    if (m_counter % 2 == 0)
                    {
                        promotionSquares[m_x, m_y].BackColor = Color.Black;
                    }
                    else
                    {
                        promotionSquares[m_x, m_y].BackColor = Color.White;
                    }
                    promotionSquares[m_x, m_y].BackgroundImageLayout = ImageLayout.Center;
                    promotionSquares[m_x, m_y].Show();
                    m_counter++;
                }
            }
            PopulateSquares(promotionSquares);
        }
        /*private void PromotePawn();*/

        /// <summary>
        ///     Makes the move on the board, checks king edge cases to make sure if its castling, checks if pawn is promoting
        ///     <remarks>
        ///         In order for empessant to work after a 2 square pawn move thiss function will keep the HasMoved property set to false and the next move it will be set to true
        ///     </remarks>
        /// </summary>
        /// ChessGUI.Chess.MakeMove()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.MakeMove - makes the move
        ///     
        /// SYNOPIS
        /// 
        ///     private void MakeMove(int a_i, int a_j, int a_tempI, int a_tempJ);
        ///     
        /// RETURNS
        /// 
        ///     void
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
        /// <param name="a_i">destination row</param>
        /// <param name="a_j">destination col</param>
        /// <param name="a_tempI">origin row</param>
        /// <param name="a_tempJ">origin col</param>
        private void MakeMove(int a_i, int a_j, int a_tempI, int a_tempJ)
        {
            if (board[a_tempI, a_tempJ].Value == 9)
            {
                if (a_tempJ - 2 == a_j)
                {
                    board[a_i, a_j + 1] = board[a_i, 0];
                    board[a_i, a_j + 1].HasMoved = true;
                    board[a_i, 0] = new Piece();
                }
                else if (a_tempJ + 2 == a_j)
                {
                    board[a_i, a_j - 1] = board[a_i, 7];
                    board[a_i, a_j - 1].HasMoved = true;
                    board[a_i, 7] = new Piece();
                }
                board[a_i, a_j] = board[a_tempI, a_tempJ];
                board[a_tempI, a_tempJ] = new Piece();
                board[a_i, a_j].HasMoved = true;
            }
            else if (board[a_tempI, a_tempJ].Value == 1 && Math.Abs(a_i - a_tempI) == 2)
            {
                board[a_i, a_j] = board[a_tempI, a_tempJ];
                board[a_tempI, a_tempJ] = new Piece();
                board[a_i, a_j].HasMoved = false;
            }
            else if(board[a_tempI, a_tempJ].Value == 1 && a_i != a_tempI && a_j != a_tempJ && board[a_i,a_j].Value == 0)
            {
                board[a_i, a_j] = board[a_tempI, a_tempJ];
                board[a_tempI, a_tempJ] = new Piece();
                board[a_i, a_j].HasMoved = true;
                board[a_tempI, a_j] = new Piece();
            }
            else if(board[a_tempI, a_tempJ].Value == 1 && (a_i == 7 || a_i == 0))
            {
                board[a_i, a_j] = board[a_tempI, a_tempJ];
                board[a_tempI, a_tempJ] = new Piece();
                PromotePawn();
            }
            else
            {
                board[a_i, a_j] = board[a_tempI, a_tempJ];
                board[a_tempI, a_tempJ] = new Piece();
                board[a_i, a_j].HasMoved = true;
            }
        }
        /*private void MakeMove(int a_i, int a_j, int a_tempI, int a_tempJ);*/

        /// <summary>
        ///     Loops through the board and finds pawns who are not on their original squares to set their HasMoved property to true
        ///     This is necessary to make the empessant opportunity expire
        /// </summary>
        /// ChessGUI.Chess.ParseForPawns()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.ParseForPawns - finds pawns that have moved
        ///     
        /// SYNOPIS
        /// 
        ///     private void ParseForPawns();
        ///     
        /// RETURNS
        /// 
        ///     void
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
        private void ParseForPawns()
        {
            for(int m_i = 0; m_i < 8; m_i++)
            {
                for(int m_j = 0; m_j < 8; m_j++)
                {
                    if (board[m_i, m_j].HasMoved || board[m_i,m_j].Value != 1)
                    {
                        continue;
                    }
                    if(board[m_i,m_j].White == state.White && m_i != 6 && m_i != 1)
                    {
                        board[m_i, m_j].HasMoved = true;
                    }
                }
            }
        }
        /*ParseForPawns();*/

        /// <summary>
        ///     Finds the pawn on the promotion rank and returns its location as a key value pair
        /// </summary>
        /// ChessGUI.Chess.PawnLocation()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.PawnLocation - finds promoted pawn
        ///     
        /// SYNOPIS
        /// 
        ///     private KeyValuePair<int, int> PawnLocation();
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
        /// <returns>Key value pair of ints representing a pawns location on the board when it promotess</returns>
        private KeyValuePair<int, int> PawnLocation()
        {
            for(int m_i = 0; m_i < 8; m_i++)
            {
                if(board[0, m_i].Value == 1)
                {
                    return new KeyValuePair<int, int>(0, m_i);
                }
                else if(board[7, m_i].Value == 1)
                {
                    return new KeyValuePair<int, int>(7, m_i);
                }
            }
            return new KeyValuePair<int, int>(-1, -1);
        }
        /*private KeyValuePair<int, int> PawnLocation();*/

        /// <summary>
        ///     Finds the location of another piece with the same value as the piece moving that can move to the same square and returns that
        /// </summary>
        /// ChessGUI.Chess.GetOtherPieces()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.GetOtherPieces - finds other pieces that can make the same move
        ///     
        /// SYNOPIS
        /// 
        ///     private string GetOtherPieces(int a_destI int a_destJ, int a_origI, int a_origJ);
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
        /// <param name="a_destI">destination row</param>
        /// <param name="a_destJ">destination col</param>
        /// <param name="a_origI">origin row</param>
        /// <param name="a_origJ">origin col</param>
        /// <returns>location of another piece as a string consisting of 2 chars or empty</returns>
        private string GetOtherPieces(int a_destI, int a_destJ, int a_origI, int a_origJ)
        {
            string m_otherSquares = String.Empty;
            string[] m_otherStrings;
            Piece[,] m_copy = new Piece[8, 8];
            for(int m_i = 0; m_i < 8; m_i++)
            {
                for(int m_j = 0; m_j < 8; m_j++)
                {
                    if(state.Board[m_i,m_j].Value != state.Board[a_origI, a_origJ].Value || state.Board[m_i,m_j].White != state.Board[a_origI, a_origJ].White)
                    {
                        continue;
                    }
                    MakeCopy(state.Board, m_copy);
                    m_otherSquares = FindLegalMoves(m_copy, m_i, m_j);
                    m_otherStrings = m_otherSquares.Split(' ', ',');
                    foreach(string m_square in m_otherStrings)
                    {
                        if (m_square.Length < 2)
                        {
                            continue;
                        }
                        int m_destI = Convert.ToInt32(m_square.ElementAt(0) - 48);
                        int m_destJ = Convert.ToInt32(m_square.ElementAt(1) - 48);
                        if(m_destI == a_destI && m_destJ == a_destJ)
                        {
                            if ((m_i != a_origI || m_j != a_origJ) && m_otherSquares != String.Empty)
                            {
                                return m_i.ToString() + m_j.ToString();
                            }
                        }
                    }
                    m_otherSquares = String.Empty;
                }
            }
            return m_otherSquares;
        }
        /*private string GetOtherPieces(int a_destI, int a_destJ, int a_origI, int a_origJ);*/

        /// <summary>
        ///     Returns a letter corresponding to the chess notation
        /// </summary>
        /// ChessGUI.Chess.ConvertJToLetter()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.ConvertJToLetter - converts a number to a letter indeex
        ///     
        /// SYNOPIS
        /// 
        ///     private char ConvertJToLetter(int a_j);
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
        /// <param name="a_j">column to convert into letter</param>
        /// <returns>character representing the rank on the board</returns>
        private char ConvertJToLetter(int a_j)
        {
            if (!state.White)
            {
                a_j = 7 - a_j;
            }
            switch (a_j)
            {
                case 0:
                    return 'a';
                case 1:
                    return 'b';
                case 2:
                    return 'c';
                case 3:
                    return 'd';
                case 4:
                    return 'e';
                case 5:
                    return 'f';
                case 6:
                    return 'g';
                case 7:
                    return 'h';
                default:
                    return '\0';
            }
        }
        /*private char ConvertJToLetter(int a_j)*/

        /// <summary>
        /// Does the first part of notation for a particular move which is based on previous position to the move, it checks value of origin whether we are taking anything and where the destination is and 
        /// converts that into a string which is readable to chess players and adds it to state.Notation
        /// </summary>
        /// ChessGUI.Chess.AddToNotation()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.AddToNotation - makes the first part of notation
        ///     
        /// SYNOPIS
        /// 
        ///     private void AddToNotation(int a_destI, int a_destJ, int a_origI, int a_origJ);
        ///     
        /// RETURNS
        /// 
        ///     void
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
        /// <param name="a_destI">destination row</param>
        /// <param name="a_destJ">destination col</param>
        /// <param name="a_origI">origin row</param>
        /// <param name="a_origJ">origin col</param>
        private void AddToNotation(int a_destI, int a_destJ, int a_origI, int a_origJ)
        {
            string m_move = String.Empty;
            if (state.AllPositions.Count % 2 == 1)
            {
                m_move += (state.AllPositions.Count / 2 + 1).ToString() + ".  ";
            }
            switch(state.Board[a_origI, a_origJ].Value)
            {
                case 1:
                    break;
                case 3:
                    m_move += 'B';
                    break;
                case 4:
                    m_move += 'N';
                    break;
                case 5:
                    m_move += 'R';
                    break;
                case 8:
                    m_move += 'Q';
                    break;
                case 9:
                    if(Math.Abs(a_destJ - a_origJ) > 1)
                    {
                        if((!state.White && a_destJ != 5) || (state.White && a_destJ == 6))
                        {
                            m_move += "O-O";
                        }
                        else
                        {
                            m_move += "O-O-O";
                        }
                        break;
                    }
                    m_move += 'K';
                    break;
                default:
                    break;
            }
            string m_others = GetOtherPieces(a_destI, a_destJ, a_origI, a_origJ);
            if (m_others != String.Empty && m_move[0] != 'O')
            {
                int i = Convert.ToInt32(m_others.ElementAt(0) - 48);
                int j = Convert.ToInt32(m_others.ElementAt(1) - 48);
                if(i == a_origI)
                {
                    m_move += ConvertJToLetter(a_origJ);
                }
                else if(j != a_origJ)
                {
                    m_move += ConvertJToLetter(a_origJ);
                }
                else
                {
                    m_move += (a_origI + 1).ToString();
                }
            }
            if(m_move.Length == 0 || m_move.ElementAt(m_move.Length-1) != 'O')
            {
                if (state.Board[a_destI, a_destJ].Value > 0)
                {
                    m_move += 'x';
                }
                m_move += ConvertJToLetter(a_destJ);
                if (state.White)
                {
                    a_destI = 7 - a_destI;
                }
                m_move += (a_destI + 1).ToString();
            }
            state.Notation += m_move;
        }
        /* private void AddToNotation(int a_destI, int a_destJ, int a_origI, int a_origJ)*/

        /// <summary>
        /// adds sign for checkmate or check to notation and then either adds a tab or a new column depending on color
        /// </summary>
        /// ChessGUI.Chess.AddExtrasToNotation()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.AddExtrasToNotation - adds extra symbols to the notation
        ///     
        /// SYNOPIS
        /// 
        ///     private void AddExtrasToNotation();
        ///     
        /// RETURNS
        /// 
        ///     void
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
        private void AddExtrasToNotation()
        {
            string m_extra = String.Empty;
            Piece[,] m_copy = new Piece[8, 8];
            MakeCopy(state.Board, m_copy);
            if (IsCheckMate())
            {
                state.CheckMate = true;
                m_extra += "#";
            }
            else if (IsCheck(m_copy, !state.White))
            {
                m_extra += "+";
            }
            if (state.White)
            {
                m_extra += '\t';
            }
            else
            {
                m_extra += "\r\n";
            }
            state.Notation += m_extra;
            textBoxNotation.Text = state.Notation;
        }
        /*private void AddExtrasToNotation*/

        /// <summary>
        /// Checks if only 2 kings are on the board in which case it is stalemate
        /// </summary>
        /// ChessGUI.Chess.ChekcForKingsOnly()
        /// 
        /// NAME
        ///     
        ///     ChessGUI.Chess.CheckForKingsOnly - checks if the board is down to 2 kings
        ///     
        /// SYNOPIS
        /// 
        ///     private bool CheckForKingsOnly();
        ///     
        /// AUTHOR
        /// 
        ///     Elliott Barinberg
        ///     
        /// DATE
        /// 
        ///     1:50 PM 3/30/2018
        /// <returns>false if there are any other pieces on the board, true otherwise</returns>
        private bool CheckForKingsOnly()
        {
            for(int m_i = 0; m_i < 8; m_i++)
            {
                for(int m_j = 0; m_j < 8; m_j++)
                {
                    if(state.Board[m_i,m_j].Value != 0 && state.Board[m_i,m_j].Value != 9)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        /*private bool CheckForKingsOnly*/

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
                    ResetColors();
                    ResetColors();
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
                    state.WhiteToMove = !state.WhiteToMove;
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
                        int tempI = Convert.ToInt32(origin.ElementAt(0) - 48);
                        int tempJ = Convert.ToInt32(origin.ElementAt(1) - 48);
                        if(board[tempI, tempJ].Value == 1 && (i == 7 || i == 0))
                        {
                            MakeMove(i, j, tempI, tempJ);
                            return;
                        }
                        else
                        {
                            state.WhiteToMove = !state.WhiteToMove;
                        }

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