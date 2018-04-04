using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGUI
{
    class GameState
    {
        private List<Piece[,]> allPositions;
        private string chat;
        private Piece[,] board;
        private string notation;
        private bool white;
        private bool checkMate;
        private bool staleMate;
        private bool whiteToMove;
        private bool drawByRepitition;
        private int whiteTimeLeft;
        private int blackTimeLeft;
        private int timeNumber;
        private bool watingForSecondPlayer;
        private bool gameOver;
        private bool serverError;
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

        public bool WaitingForSecondPlayer
        {
            get
            {
                return watingForSecondPlayer;
            }
            set
            {
                watingForSecondPlayer = value;
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

        public string BlackTimeLeftToString()
        {
            if(blackTimeLeft > 3600)
            {
                return BlackTimeLeft / 3600 + ":" + (BlackTimeLeft / 60 % 60).ToString("00") + ":" + (BlackTimeLeft % 60).ToString("00");
            }
            else
            {
                return BlackTimeLeft / 60 % 60 + ":" + (BlackTimeLeft % 60).ToString("00");
            }
        }

        public string WhiteTimeLeftToString()
        {
            if (WhiteTimeLeft > 3600)
            {
                return WhiteTimeLeft / 3600 + ":" + (WhiteTimeLeft / 60 % 60).ToString("00") + ":" + (WhiteTimeLeft % 60).ToString("00");
            }
            else
            {
                return WhiteTimeLeft / 60 % 60 + ":" + (WhiteTimeLeft % 60).ToString("00");
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
            board = new Piece[8,8];
            checkMate = false;
            staleMate = false;
            WhiteToMove = true;
            chat = String.Empty;
            notation = String.Empty;
            takenPieces = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        }
    }
}
