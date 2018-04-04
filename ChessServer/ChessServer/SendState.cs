using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessServer
{
    /// <summary>
    /// This class is a gamestate without any sockets or endpoints. This is what will be sent and recieved by the server client programs
    /// </summary>
    public class SendState
    {
        // all private variables to gamestate
        private List<Piece[,]> allPositions;    // history of all positions
        private string chat;                    // chat string
        private Piece[,] board;                 // current board
        private string notation;                // notation string
        private bool white;                     // is user white?
        private bool checkMate;                 // is it checkmate
        private bool staleMate;                 // is it stalemate
        private bool drawByRepitition;          // is it draw by repititon
        private bool whiteToMove;               // is it white to mive
        private bool waitingForSecondPlayer;    // Are we waiting for a second client
        private int blackTimeLeft;              // Time left in game
        private int whiteTimeLeft;              // ""
        private bool serverError;               // Did we lose a client
        private bool gameOver;                  // Is the game still going
        private bool opponentDisconnected;      // opponent still connected to game
        private int timeNumber;
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

        public int TimeNumber
        {
            get
            {
                return timeNumber;
            }
            set
            {
                timeNumber = value;
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

        /// <summary>
        /// Sendstate constructor
        /// Initializes to a state capable of playing chess with
        /// </summary>
        public SendState()
        {
            checkMate = false;
            staleMate = false;
            WhiteToMove = true;
            chat = String.Empty;
            notation = String.Empty;
            waitingForSecondPlayer = true;
            allPositions = new List<Piece[,]>();
            drawByRepitition = false;
            whiteTimeLeft = 10000;
            blackTimeLeft = 10000;
            serverError = false;
            opponentDisconnected = false;
            takenPieces = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        }
    }
}