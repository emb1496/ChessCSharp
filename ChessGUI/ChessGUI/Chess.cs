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
        Square[,] squares = new Square[8, 8];
        IPEndPoint ip;
        Socket socket;
        NetworkStream ns;
        StreamReader sr;
        StreamWriter sw;
        GameState state;

        public Chess()
        {
            ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ip);
            ns = new NetworkStream(socket);
            sr = new StreamReader(ns);
            sw = new StreamWriter(ns);
            string message = sr.ReadLine();
            state = JsonConvert.DeserializeObject<GameState>(message);
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
                Invoke(new Action(() =>
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
            if (i == 6 && brilliantNum < 0 && tempBoard[i + brilliantNum, j].Value == 0)
            {
                moves += (i + brilliantNum).ToString() + j.ToString() + ", ";
            }
            else if (i == 1 && brilliantNum > 0 && tempBoard[i + brilliantNum, j].Value == 0)
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

        private string GetAllLegalMoves()
        {
            string moves = String.Empty;
            string []movesArray;
            int index;
            Piece[,] copy = new Piece[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if(board[i,j].Value== 0 || board[i,j].White == state.White)
                    {
                        continue;
                    }
                    moves += FindLegalMoves(board, i, j);
                    movesArray = moves.Split(' ', ',');
                    foreach(string move in movesArray)
                    {
                        if(move.Length < 2)
                        {
                            continue;
                        }
                        int iVal = Convert.ToInt32(move.ElementAt(0)) - 48;
                        int jVal = Convert.ToInt32(move.ElementAt(1)) - 48;
                        MakeCopy(board, copy);
                        copy[iVal, jVal] = copy[i, j];
                        copy[i, j] = new Piece();
                        if(IsCheck(copy, !state.White))
                        {
                            index = moves.IndexOf(move);
                            moves = moves.Remove(index, 2);
                            if(moves.ElementAt(index) == ',')
                            {
                                moves = moves.Remove(index, 2);
                            }
                        }
                    }
                    if(moves != String.Empty)
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
            if(!IsCheck(copy, state.White) && !IsCheck(copy, !state.White))
            {
                moves = GetAllLegalMoves();
                if(moves == String.Empty)
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
            if(IsCheck(copy, !state.White))
            {
                moves = GetAllLegalMoves();
                if (moves == String.Empty)
                {
                    return true;
                }
            }
            return false;
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
                if (origin == String.Empty)
                {
                    if(board[i, j].White == state.White)
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
                        if(board[tempI, tempJ].Value == 9)
                        {
                            if (tempJ - 2 == j)
                            {
                                board[i, j + 1] = board[i, 0];
                                board[i, 0] = new Piece();
                            }
                            else if(tempJ + 2 == j)
                            {
                                board[i, j - 1] = board[i, 7];
                                board[i, 7] = new Piece();
                            }
                        }
                        board[i, j] = board[tempI, tempJ];
                        board[tempI, tempJ] = new Piece();
                        board[i, j].HasMoved = true;
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
                        //Thread t = new Thread(ProcessServerMessages);
                        //t.Start();
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
                button1.Visible = false;
                button1.Width = 0;
                button1.Height = 0;
                board = state.Board;
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
            //Thread t = new Thread(ProcessServerMessages);
            //t.Start();
        }
    }
}