using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessServer
{
    /// <summary>
    /// This is a class of gamestates with sockets, it is used for initial load in and then the contents are pushed to a SendState and the sockets are passed to PLAY()
    /// </summary>
    public class GameState
    {
        private List<Piece[,]> allPositions;
        private string chat;
        private Piece[,] board;
        private string notation;
        private bool white;
        private bool checkMate;
        private bool staleMate;
        private bool drawByRepitition;
        private bool whiteToMove;
        private Socket player1;
        private IPEndPoint endPoint1;
        private Socket player2;
        private IPEndPoint endPoint2;
        private bool waitingForSecondPlayer;
        private int blackTimeLeft;
        private int whiteTimeLeft;
        private bool serverError;
        private bool gameOver;
        private bool opponentDisconnected;
        private int[] takenPieces;

        public int[] TakenPieces
        {
            get
            {
                return takenPieces;
            }
            set
            {
                takenPieces = value;
            }
        }

        public bool OpponentDisconnected
        {
            get
            {
                return opponentDisconnected;
            }
            set
            {
                opponentDisconnected = value;
            }
        }

        public bool GameOver
        {
            get
            {
                return gameOver;
            }
            set
            {
                gameOver = value;
            }
        }

        public bool ServerError
        {
            get
            {
                return serverError;
            }
            set
            {
                serverError = value;
            }
        }

        public int BlackTimeLeft
        {
            get
            {
                return blackTimeLeft;
            }
            set
            {
                blackTimeLeft = value;
            }
        }

        public int WhiteTimeLeft
        {
            get
            {
                return whiteTimeLeft;
            }
            set
            {
                whiteTimeLeft = value;
            }
        }

        public List<Piece[,]> AllPositions
        {
            get
            {
                return allPositions;
            }
            set
            {
                allPositions = value;
            }
        }

        public void AddToAllPositions(Piece[,] position)
        {
            allPositions.Add(position);
        }

        public bool DrawByRepitition
        {
            get
            {
                return drawByRepitition;
            }
            set
            {
                drawByRepitition = value;
            }
        }

        public IPEndPoint EndPoint1
        {
            get
            {
                return endPoint1;
            }
            set
            {
                endPoint1 = value;
            }
        }

        public IPEndPoint EndPoint2
        {
            get
            {
                return endPoint2;
            }
            set
            {
                endPoint2 = value;
            }
        }

        public bool WaitingForSecondPlayer
        {
            get
            {
                return waitingForSecondPlayer;
            }
            set
            {
                waitingForSecondPlayer = value;
            }
        }

        public Socket Player1
        {
            get
            {
                return player1;
            }
            set
            {
                player1 = value;
            }
        }

        public Socket Player2
        {
            get
            {
                return player2;
            }
            set
            {
                player2 = value;
            }
        }

        public bool WhiteToMove
        {
            get
            {
                return whiteToMove;
            }
            set
            {
                whiteToMove = value;
            }
        }

        public bool CheckMate
        {
            get
            {
                return checkMate;
            }
            set
            {
                checkMate = value;
            }
        }

        public bool StaleMate
        {
            get
            {
                return staleMate;
            }
            set
            {
                staleMate = value;
            }
        }

        public bool White
        {
            get
            {
                return white;
            }
            set
            {
                white = value;
            }
        }

        public Piece[,] Board
        {
            get
            {
                return board;
            }
            set
            {
                board = value;
            }
        }

        public string Chat
        {
            get
            {
                return chat;
            }
            set
            {
                chat = value;
            }
        }

        public string Notation
        {
            get
            {
                return notation;
            }
            set
            {
                notation = value;
            }
        }

        public GameState()
        {
            checkMate = false;
            staleMate = false;
            drawByRepitition = false;
            player1 = null;
            player2 = null;
            WhiteToMove = true;
            chat = String.Empty;
            notation = String.Empty;
            waitingForSecondPlayer = true;
            serverError = false;
            allPositions = new List<Piece[,]>();
            whiteTimeLeft = 10000;
            blackTimeLeft = 10000;
            gameOver = false;
            opponentDisconnected = false;
            takenPieces = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        }
    }
}