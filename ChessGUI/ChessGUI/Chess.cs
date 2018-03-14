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
        Piece[,] board = new Piece[8, 8];
        string origin = String.Empty;
        string destination = String.Empty;
        string message = String.Empty;
        string playerName = String.Empty;
        int port = 1234;
        Square[,] squares = new Square[8, 8];
        Square[,] promotionSquares = new Square[2, 2];
        IPEndPoint ip;
        Socket socket;
        NetworkStream ns;
        StreamReader sr;
        StreamWriter sw;
        GameState state = new GameState();

        public Chess()
        {
            state.WhiteTimeLeft = 10800;
            state.BlackTimeLeft = 10800;
            InitializeComponent();
        }

        private bool IsSameBoard(Piece[,] board1, Piece[,] board2)
        {
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    if(board1[i,j].Value != board2[i, j].Value)
                    {
                        return false;
                    }
                    if(board1[i,j].White != board2[i, j].White)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void ProcessServerMessages()
        {
            while (true)
            {
                string message = sr.ReadLine();
                state = JsonConvert.DeserializeObject<GameState>(message);
                if (state.CheckMate)
                {
                    MessageBox.Show("CheckMate");
                }
                else if (state.StaleMate)
                {
                    MessageBox.Show("StaleMate");
                }
                else if (state.DrawByRepitition)
                {
                    MessageBox.Show("Draw By Repitition");
                }
                Invoke(new Action(() =>
                {
                    if (state.WaitingForSecondPlayer)
                    {
                        Clicks(false);
                        textBoxChat.Text = "Waiting for second player";
                    }
                    else
                    {
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
                            Timer1.Start();
                        }
                    }
                }));
            }
        }

        private string PawnLegalMoves(Piece[,] tempBoard, int i, int j)
        {
            int brilliantNum = 0;
            string moves = String.Empty;
            if (tempBoard[i, j].White == state.White)
            {
                brilliantNum = -2;
            }
            else
            {
                brilliantNum = +2;
            }

            // two square move check
            if (i == 6 && board[i,j].HasMoved == false && brilliantNum < 0 && tempBoard[i + brilliantNum, j].Value == 0)
            {
                moves += (i + brilliantNum).ToString() + j.ToString() + ", ";
            }
            else if (i == 1 && board[i,j].HasMoved == false && brilliantNum > 0 && tempBoard[i + brilliantNum, j].Value == 0)
            {
                moves += (i + brilliantNum).ToString() + j.ToString() + ", ";
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
            if ((i + brilliantNum <= 7) && (i + brilliantNum >= 0) && tempBoard[i + brilliantNum, j].Value == 0)
            {
                moves += (i + brilliantNum).ToString() + j.ToString() + ", ";
            }

            // empessant check
            if (i == 3 && j + brilliantNum < 8 && j + brilliantNum >= 0 && tempBoard[i, j + brilliantNum].HasMoved == false && tempBoard[i, j + brilliantNum].Value == 1)
            {
                moves += (i + brilliantNum).ToString() + (j + brilliantNum).ToString() + ", ";
            }
            if (i == 3 && j - brilliantNum >= 0 && j - brilliantNum < 8 && tempBoard[i, j - brilliantNum].HasMoved == false && tempBoard[i, j - brilliantNum].Value == 1)
            {
                moves += (i + brilliantNum).ToString() + (j - brilliantNum).ToString() + ", ";
            }

            //taking pieces
            if ((i + brilliantNum <= 7) && (i + brilliantNum >= 0) && (j + brilliantNum <= 7) && (j + brilliantNum >= 0) && tempBoard[i + brilliantNum, j + brilliantNum].Value != 0 && tempBoard[i + brilliantNum, j + brilliantNum].White != tempBoard[i, j].White)
            {
                moves += (i + brilliantNum).ToString() + (j + brilliantNum).ToString() + ", ";
            }

            if ((i + brilliantNum <= 7) && (i + brilliantNum >= 0) && (j - brilliantNum <= 7) && (j - brilliantNum >= 0) && (tempBoard[i + brilliantNum, j - brilliantNum].Value != 0 && tempBoard[i + brilliantNum, j - brilliantNum].White != tempBoard[i, j].White))
            {
                moves += (i + brilliantNum).ToString() + (j - brilliantNum).ToString() + ", ";
            }
            return moves;
        }

        private string KnightLegalMoves(Piece[,] tempBoard, int i, int j)
        {
            string moves = String.Empty;
            if (i - 2 >= 0 && j - 1 >= 0 && (tempBoard[i - 2, j - 1].Value == 0 || tempBoard[i - 2, j - 1].White != tempBoard[i, j].White))
            {
                moves += (i - 2).ToString() + (j - 1).ToString() + ", ";
            }
            if (i - 2 >= 0 && j + 1 < 8 && (tempBoard[i - 2, j + 1].Value == 0 || tempBoard[i - 2, j + 1].White != tempBoard[i, j].White))
            {
                moves += (i - 2).ToString() + (j + 1).ToString() + ", ";
            }
            if (i - 1 >= 0 && j - 2 >= 0 && (tempBoard[i - 1, j - 2].Value == 0 || tempBoard[i - 1, j - 2].White != tempBoard[i, j].White))
            {
                moves += (i - 1).ToString() + (j - 2).ToString() + ", ";
            }
            if (i - 1 >= 0 && j + 2 < 8 && (tempBoard[i - 1, j + 2].Value == 0 || tempBoard[i - 1, j + 2].White != tempBoard[i, j].White))
            {
                moves += (i - 1).ToString() + (j + 2).ToString() + ", ";
            }
            if (i + 1 < 8 && j - 2 >= 0 && (tempBoard[i + 1, j - 2].Value == 0 || tempBoard[i + 1, j - 2].White != tempBoard[i, j].White))
            {
                moves += (i + 1).ToString() + (j - 2).ToString() + ", ";
            }
            if (i + 1 < 8 && j + 2 < 8 && (tempBoard[i + 1, j + 2].Value == 0 || tempBoard[i + 1, j + 2].White != tempBoard[i, j].White))
            {
                moves += (i + 1).ToString() + (j + 2).ToString() + ", ";
            }
            if (i + 2 < 8 && j - 1 >= 0 && (tempBoard[i + 2, j - 1].Value == 0 || tempBoard[i + 2, j - 1].White != tempBoard[i, j].White))
            {
                moves += (i + 2).ToString() + (j - 1).ToString() + ", ";
            }
            if (i + 2 < 8 && j + 1 < 8 && (tempBoard[i + 2, j + 1].Value == 0 || tempBoard[i + 2, j + 1].White != tempBoard[i, j].White))
            {
                moves += (i + 2).ToString() + (j + 1).ToString() + ", ";
            }
            return moves;
        }
        
        private string BishopLegalMoves(Piece[,] tempBoard, int i, int j)
        {
            string moves = String.Empty;
            for (int k = 1; k < 8; k++)
            {
                if (i + k < 8 && j + k < 8 && (tempBoard[i + k, j + k].Value == 0))
                {
                    moves += (i + k).ToString() + (j + k).ToString() + ", ";
                    continue;
                }
                else if (i + k < 8 && j + k < 8 && tempBoard[i + k, j + k].White != tempBoard[i, j].White)
                {
                    moves += (i + k).ToString() + (j + k).ToString() + ", ";
                    break;
                }
                else
                {
                    break;
                }
            }
            for (int k = 1; k < 8; k++)
            {
                if (i - k >= 0 && j + k < 8 && (tempBoard[i - k, j + k].Value == 0))
                {
                    moves += (i - k).ToString() + (j + k).ToString() + ", ";
                    continue;
                }
                else if (i - k >= 0 && j + k < 8 && tempBoard[i - k, j + k].White != tempBoard[i, j].White)
                {
                    moves += (i - k).ToString() + (j + k).ToString() + ", ";
                    break;
                }
                else
                {
                    break;
                }
            }
            for (int k = 1; k < 8; k++)
            {
                if (i + k < 8 && j - k >= 0 && (tempBoard[i + k, j - k].Value == 0))
                {
                    moves += (i + k).ToString() + (j - k).ToString() + ", ";
                    continue;
                }
                else if (i + k < 8 && j - k >= 0 && tempBoard[i + k, j - k].White != tempBoard[i, j].White)
                {
                    moves += (i + k).ToString() + (j - k).ToString() + ", ";
                    break;
                }
                else
                {
                    break;
                }
            }
            for (int k = 1; k < 8; k++)
            {
                if (i - k >= 0 && j - k >= 0 && (tempBoard[i - k, j - k].Value == 0))
                {
                    moves += (i - k).ToString() + (j - k).ToString() + ", ";
                    continue;
                }
                else if (i - k >= 0 && j - k >= 0 && tempBoard[i - k, j - k].White != tempBoard[i, j].White)
                {
                    moves += (i - k).ToString() + (j - k).ToString() + ", ";
                    break;
                }
                else
                {
                    break;
                }
            }
            return moves;
        }

        private string RookLegalMoves(Piece[,] tempBoard, int i, int j)
        {
            string moves = String.Empty;
            for (int k = 1; k < 8; k++)
            {
                if (i + k < 8 && (tempBoard[i + k, j].Value == 0))
                {
                    moves += (i + k).ToString() + (j).ToString() + ", ";
                    continue;
                }
                else if (i + k < 8 && tempBoard[i + k, j].White != tempBoard[i, j].White)
                {
                    moves += (i + k).ToString() + (j).ToString() + ", ";
                    break;
                }
                else
                {
                    break;
                }
            }
            for (int k = 1; k < 8; k++)
            {
                if (i - k >= 0 && (tempBoard[i - k, j].Value == 0))
                {
                    moves += (i - k).ToString() + (j).ToString() + ", ";
                    continue;
                }
                else if (i - k >= 0 && tempBoard[i - k, j].White != tempBoard[i, j].White)
                {
                    moves += (i - k).ToString() + (j).ToString() + ", ";
                    break;
                }
                else
                {
                    break;
                }
            }
            for (int k = 1; k < 8; k++)
            {
                if (j - k >= 0 && (tempBoard[i, j - k].Value == 0))
                {
                    moves += (i).ToString() + (j - k).ToString() + ", ";
                    continue;
                }
                else if (j - k >= 0 && tempBoard[i, j - k].White != tempBoard[i, j].White)
                {
                    moves += (i).ToString() + (j - k).ToString() + ", ";
                    break;
                }
                else
                {
                    break;
                }
            }
            for (int k = 1; k < 8; k++)
            {
                if (j + k < 8 && (tempBoard[i, j + k].Value == 0))
                {
                    moves += (i).ToString() + (j + k).ToString() + ", ";
                    continue;
                }
                else if (j + k < 8 && tempBoard[i, j + k].White != tempBoard[i, j].White)
                {
                    moves += (i).ToString() + (j + k).ToString() + ", ";
                    break;
                }
                else
                {
                    break;
                }
            }
            return moves;
        }

        private string QueenLegalMoves(Piece[,] tempBoard, int i, int j)
        {
            string moves = String.Empty;
            moves = BishopLegalMoves(tempBoard, i, j);
            moves += RookLegalMoves(tempBoard, i, j);
            return moves;
        }

        private string KingLegalMoves(Piece[,] tempBoard, int i, int j)
        {
            string moves = String.Empty;
            Piece[,] copy = new Piece[8, 8];
            int length;
            // check up to 8 squares around the king
            if (i - 1 > 0 && j - 1 > 0 && (tempBoard[i - 1, j - 1].Value == 0 || board[i - 1, j - 1].White != tempBoard[i, j].White))
            {
                moves += (i - 1).ToString() + (j - 1).ToString() + ", ";
            }
            if (i - 1 > 0 && (tempBoard[i - 1, j].Value == 0 || tempBoard[i - 1, j].White != tempBoard[i, j].White))
            {
                moves += (i - 1).ToString() + j.ToString() + ", ";
            }
            if (i - 1 > 0 && j + 1 < 8 && (board[i - 1, j + 1].Value == 0 || tempBoard[i - 1, j + 1].White != tempBoard[i, j].White))
            {
                moves += (i - 1).ToString() + (j + 1).ToString() + ", ";
            }
            if (j - 1 > 0 && (tempBoard[i, j - 1].Value == 0 || tempBoard[i, j - 1].White != tempBoard[i, j].White))
            {
                moves += i.ToString() + (j - 1).ToString() + ", ";
            }
            if (j + 1 < 8 && (tempBoard[i, j + 1].Value == 0 || tempBoard[i, j + 1].White != tempBoard[i, j].White))
            {
                moves += i.ToString() + (j + 1).ToString() + ", ";
            }
            if (i + 1 < 8 && j - 1 > 0 && (tempBoard[i + 1, j - 1].Value == 0 || tempBoard[i + 1, j - 1].White != tempBoard[i, j].White))
            {
                moves += (i + 1).ToString() + (j - 1).ToString() + ", ";
            }
            if (i + 1 < 8 && (tempBoard[i + 1, j].Value == 0 || tempBoard[i + 1, j].White != tempBoard[i, j].White))
            {
                moves += (i + 1).ToString() + j.ToString() + ", ";
            }
            if (i + 1 < 8 && j + 1 < 8 && (tempBoard[i + 1, j + 1].Value == 0 || tempBoard[i + 1, j + 1].White != tempBoard[i, j].White))
            {
                moves += (i + 1).ToString() + (j + 1).ToString() + ", ";
            }

            MakeCopy(board, copy);
            // check for castliing(king cannot castle through check and cannot castle if he or the rook has moved )
            if (!board[i, j].HasMoved)
            {
                // have to be seperate ifs to make sure king and queenside can happen same move
                if (j + 2 < 8 && board[i, j + 1].Value == 0 && board[i, j + 2].Value == 0)
                {
                    length = 1;
                    // how far are we castling?
                    while(j + length < 8 && board[i, j + length].Value == 0)
                    {
                        length++;
                    }

                    // final checks that the rook has not moved that its the same color on the edge 
                    if(board[i,j + length].Value==5 && board[i, j + length].HasMoved == false && board[i, j + length].White == board[i, j].White && j + length == 7)
                    {
                        if(length == 4)
                        {
                            length--;
                        }
                        moves += (i).ToString() + (j + 2).ToString() + ", ";
                    }
                }
                if(j - 2 >= 0 && board[i, j - 1].Value == 0 && board[i, j - 2].Value == 0)
                {
                    length = 1;
                    // how far are we castling?
                    while (j - length < 8 && board[i, j - length].Value == 0)
                    {
                        length++;
                    }

                    // final checks that the rook has not moved that its the same color on the edge 
                    if (board[i, j - length].Value == 5 && board[i , j - length].HasMoved == false && board[i , j - length].White == board[i, j].White && j - length == 0)
                    {
                        // now to make sure we are not castling through check
                        if(length == 4)
                        {
                            length--;
                        }
                        moves += i.ToString() + (j - 2).ToString() + ", ";
                    }
                }
            }
            


            return moves;
        }

        private string GetAllLegalMoves()
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
                        return moves;
                    }
                }
            }
            return moves;
        }

        private bool IsStaleMate()
        {
            Piece[,] copy = new Piece[8, 8];
            MakeCopy(board, copy);
            string moves = String.Empty;
            if (!IsCheck(copy, state.White) && !IsCheck(copy, !state.White))
            {
                moves = GetAllLegalMoves();
                if (moves == String.Empty)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsCheckMate()
        {
            string moves = String.Empty;
            Piece[,] copy = new Piece[8, 8];
            MakeCopy(board, copy);
            if (IsCheck(copy, !state.White))
            {
                moves = GetAllLegalMoves();
                if (moves == String.Empty)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsCheck(Piece[,] copy, bool isWhite)
        {
            string moves = String.Empty;
            string []movesarray;
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    if(copy[i,j].White == isWhite || copy[i, j].Value == 0)
                    {
                        continue;
                    }
                    moves = FindLegalMoves(copy, i, j);
                    movesarray = moves.Split(' ', ',');
                    foreach (string move in movesarray) {
                        if(move.Length == 0)
                        {
                            continue;
                        }
                        int tempI = Convert.ToInt32(move.ElementAt(0)) - 48;
                        int tempJ = Convert.ToInt32(move.ElementAt(1)) - 48;
                        if(copy[tempI, tempJ].Value == 9 && copy[tempI, tempJ].White == isWhite)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private string FindLegalMoves(Piece[,]tempBoard, int i, int j)
        {
            string moves = String.Empty;
            switch (tempBoard[i, j].Value)
            {
                case 1:
                    moves = PawnLegalMoves(tempBoard, i, j);
                    break;
                case 3:
                    moves = BishopLegalMoves(tempBoard, i, j);
                    break;
                case 4:
                    moves = KnightLegalMoves(tempBoard, i, j);
                    break;
                case 5:
                    moves = RookLegalMoves(tempBoard, i, j);
                    break;
                case 8:
                    moves = QueenLegalMoves(tempBoard, i, j);
                    break;
                case 9:
                    moves = KingLegalMoves(tempBoard, i, j);
                    break;
                default:
                    break;
            }
            return moves;
        }

        private void ParseAndHighlight(int i, int j)
        {
            string moves = String.Empty;
            moves = FindLegalMoves(board, i, j);
            Piece[,] copy = new Piece[8, 8];
            string[] movesArray = moves.Split(' ', ',');
            int f = 0;
            foreach (string square in movesArray)
            {
                if (square != "")
                {
                    int iVal = Convert.ToInt32(square.ElementAt(0)) - 48;
                    int jVal = Convert.ToInt32(square.ElementAt(1)) - 48;
                    if(board[i,j].Value == 9 && Math.Abs(jVal - j) == 2)
                    {
                        if(jVal > j)
                        {
                            MakeCopy(board, copy);
                            copy[i, j + 1] = copy[i, j];
                            copy[i, j] = new Piece();
                            if(IsCheck(copy, state.White))
                            {
                                continue;
                            }
                            copy[i, j + 2] = copy[i, j + 1];
                            copy[i, j + 1] = new Piece();
                            if(IsCheck(copy, state.White))
                            {
                                continue;
                            }
                        }
                        else
                        {
                            MakeCopy(board, copy);
                            copy[i, j - 1] = copy[i, j];
                            copy[i, j] = new Piece();
                            if (IsCheck(copy, state.White))
                            {
                                continue;
                            }
                            copy[i, j - 2] = copy[i, j - 1];
                            copy[i, j - 1] = new Piece();
                            if (IsCheck(copy, state.White))
                            {
                                continue;
                            }
                        }
                    }
                    MakeCopy(board, copy);
                    copy[iVal, jVal] = copy[i, j];
                    copy[i, j] = new Piece();
                    if (IsCheck(copy, state.White))
                    {

                    }
                    else if (f % 2 == 0)
                    {
                        squares[iVal, jVal].BackColor = Color.Green;
                    }
                    else
                    {
                        squares[iVal, jVal].BackColor = Color.YellowGreen;
                    }
                    f++;
                }
            }
        }

        private void ResetColors()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (squares[i, j].BackColor != Color.Black && squares[i, j].BackColor != Color.White)
                    {
                        if (i - 1 >= 0 && squares[i - 1, j].BackColor == Color.White)
                        {
                            squares[i, j].BackColor = Color.Black;
                        }
                        else if (i - 1 >= 0 && squares[i - 1, j].BackColor == Color.Black)
                        {
                            squares[i, j].BackColor = Color.White;
                        }
                        else if (i + 1 < 8 && squares[i + 1, j].BackColor == Color.White)
                        {
                            squares[i, j].BackColor = Color.Black;
                        }
                        else if (i + 1 < 8 && squares[i + 1, j].BackColor == Color.Black)
                        {
                            squares[i, j].BackColor = Color.White;
                        }
                        else if (j + 1 < 8 && squares[i, j + 1].BackColor == Color.White)
                        {
                            squares[i, j].BackColor = Color.Black;
                        }
                        else if (j + 1 < 8 && squares[i, j + 1].BackColor == Color.Black)
                        {
                            squares[i, j].BackColor = Color.White;
                        }
                        else if (j - 1 >= 0 && squares[i, j - 1].BackColor == Color.White)
                        {
                            squares[i, j].BackColor = Color.Black;
                        }
                        else if (j - 1 >= 0 && squares[i, j - 1].BackColor == Color.Black)
                        {
                            squares[i, j].BackColor = Color.White;
                        }
                    }
                }
            }
        }

        private void Clicks(bool val)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    squares[i, j].Enabled = val;
                }
            }
        }

        private void MakeCopy(Piece[,] original, Piece[,] copy)
        {
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    copy[i, j] = original[i, j];
                }
            }
        }

        private void PopulateSquares(Square[,] promotionSquares)
        {
            string dir = Directory.GetCurrentDirectory();
            int counter = 0;
            if (state.White)
            {
                counter = 0;
                foreach (Square square in promotionSquares)
                {
                    switch (counter)
                    {
                        case 0:
                            square.BackgroundImage = System.Drawing.Image.FromFile(dir + "\\Images\\BW.gif"); break;
                        case 1:
                            square.BackgroundImage = System.Drawing.Image.FromFile(dir + "\\Images\\NW.gif"); break;
                        case 2:
                            square.BackgroundImage = System.Drawing.Image.FromFile(dir + "\\Images\\RW.gif"); break;
                        case 3:
                            square.BackgroundImage = System.Drawing.Image.FromFile(dir + "\\Images\\QW.gif"); break;
                        default:
                            break;
                    }
                    counter++;
                }
            }
            else
            {
                counter = 0;
                foreach (Square square in promotionSquares)
                {
                    switch (counter)
                    {
                        case 0:
                            square.BackgroundImage = System.Drawing.Image.FromFile(dir + "\\Images\\BB.gif"); break;
                        case 1:
                            square.BackgroundImage = System.Drawing.Image.FromFile(dir + "\\Images\\NB.gif"); break;
                        case 2:
                            square.BackgroundImage = System.Drawing.Image.FromFile(dir + "\\Images\\RB.gif"); break;
                        case 3:
                            square.BackgroundImage = System.Drawing.Image.FromFile(dir + "\\Images\\QB.gif"); break;
                        default:
                            break;
                    }
                    counter++;
                }
            }
        }

        private void HideSquares()
        {
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    squares[i, j].Hide();
                }
            }
        }

        private void ShowSquares()
        {
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    squares[i, j].Show();
                }
            }
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
            if (state.AllPositions.Count % 2 == 0)
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
                    move += 'K';
                    break;
                default:
                    break;
            }
            string others = GetOtherPieces(destI, destJ, origI, origJ);
            if (others != String.Empty)
            {
                int i = Convert.ToInt32(others.ElementAt(0) - 48);
                int j = Convert.ToInt32(others.ElementAt(1) - 48);
                if(i == origI)
                {
                    move += ConvertJToLetter(origJ);
                }
                else
                {
                    move += origI.ToString();
                }
            }
            if(state.Board[destI, destJ].Value > 0)
            {
                move += 'x';
            }
            move += ConvertJToLetter(destJ);
            if (!state.White)
            {
                destI = 7 - destI;
            }
            move += (destI).ToString();
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
                        state.CheckMate = true;
                        MessageBox.Show("Checkmate");
                    }
                    else if (IsStaleMate())
                    {
                        state.StaleMate = true;
                        MessageBox.Show("Stalemate");
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
                        ResetColors();
                        Drawing(board);
                        Clicks(false);
                        if (IsCheckMate())
                        {
                            state.CheckMate = true;
                            MessageBox.Show("Checkmate");
                        }
                        else if (IsStaleMate())
                        {
                            state.StaleMate = true;
                            MessageBox.Show("Stalemate");
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

        private void Drawing(Piece[,] board)
        {
            string dir = Directory.GetCurrentDirectory();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j].White == true)
                    {
                        switch (board[i, j].Value)
                        {
                            case 0:
                                squares[i, j].BackgroundImage = null; break;
                            case 1:
                                squares[i, j].BackgroundImage = System.Drawing.Image.FromFile(dir + "\\Images\\pW.gif"); break;
                            case 3:
                                squares[i, j].BackgroundImage = System.Drawing.Image.FromFile(dir + "\\Images\\BW.gif"); break;
                            case 4:
                                squares[i, j].BackgroundImage = System.Drawing.Image.FromFile(dir + "\\Images\\NW.gif"); break;
                            case 5:
                                squares[i, j].BackgroundImage = System.Drawing.Image.FromFile(dir + "\\Images\\RW.gif"); break;
                            case 8:
                                squares[i, j].BackgroundImage = System.Drawing.Image.FromFile(dir + "\\Images\\QW.gif"); break;
                            case 9:
                                squares[i, j].BackgroundImage = System.Drawing.Image.FromFile(dir + "\\Images\\KW.gif"); break;
                        }
                    }
                    else
                    {
                        switch (board[i, j].Value)
                        {
                            case 0:
                                squares[i, j].BackgroundImage = null; break;
                            case 1:
                                squares[i, j].BackgroundImage = System.Drawing.Image.FromFile(dir + "\\Images\\pB.gif"); break;
                            case 3:
                                squares[i, j].BackgroundImage = System.Drawing.Image.FromFile(dir + "\\Images\\BB.gif"); break;
                            case 4:
                                squares[i, j].BackgroundImage = System.Drawing.Image.FromFile(dir + "\\Images\\NB.gif"); break;
                            case 5:
                                squares[i, j].BackgroundImage = System.Drawing.Image.FromFile(dir + "\\Images\\RB.gif"); break;
                            case 8:
                                squares[i, j].BackgroundImage = System.Drawing.Image.FromFile(dir + "\\Images\\QB.gif"); break;
                            case 9:
                                squares[i, j].BackgroundImage = System.Drawing.Image.FromFile(dir + "\\Images\\KB.gif"); break;
                        }
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
                    squares[i, j].Location = new Point(j * 50 + 13, i * 50 + 13);
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

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ip);
                byte[] buffer = new byte[10];
                sbyte byte1 = Convert.ToSByte(state.TimePortOffset);
                buffer[0] = Convert.ToByte(byte1);
                socket.Send(buffer, SocketFlags.None);
                ns = new NetworkStream(socket);
                sr = new StreamReader(ns);
                sw = new StreamWriter(ns);
                message = sr.ReadLine();
                GameState temp = new GameState();
                temp = JsonConvert.DeserializeObject<GameState>(message);
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
                    Timer1.Start();
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
                if(state.WhiteTimeLeft > 0)
                {

                }
                else
                {

                }
                //LowerTimeLabel.Text = "Time's up";
                MessageBox.Show("Out of time");
            }
        }

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
    }
}