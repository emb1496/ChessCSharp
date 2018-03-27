using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using System.Collections;

namespace ChessServer
{

    /// <summary>
    /// This class is a gamestate without any sockets or endpoints. This is what will be send and recieved by the server client programs
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
        private int timePortOffset;             // Marks the length of the game
        private bool serverError;               // Did we lose a client
        private bool gameOver;                  // Is the game still going

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

        public int TimePortOffset
        {
            get
            {
                return timePortOffset;
            }
            set
            {
                timePortOffset = value;
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
        }
    }

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
        }
    }

    public class Piece
    {
        private int value;
        private bool white;
        private bool hasMoved;

        public bool HasMoved
        {
            get
            {
                return hasMoved;
            }
            set
            {
                hasMoved = value;
            }
        }

        public int Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
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

        public Piece()
        {
            value = -1;
            white = false;
        }

        public string toString()
        {
            string s = "";
            s += value.ToString();
            if(this.white == true)
            {
                s += "w ";
            }
            else
            {
                s += "b ";
            }
            return s;
        }
    }

    public class Empty : Piece
    {
        public Empty()
        {
            Value = 0;
            White = false;
        }
    }

    public class Pawn : Piece
    {
        public Pawn(bool white)
        {
            Value = 1;
            White = white;
        }
    }

    public class Bishop : Piece
    {
        public Bishop(bool white)
        {
            Value = 3;
            White = white;
        }
    }

    public class Knight : Piece
    {
        public Knight(bool white)
        {
            Value = 4;
            White = white;
        }
    }

    public class Rook : Piece
    {
        public Rook(bool white)
        {
            Value = 5;
            White = white;
        }
    }

    public class Queen : Piece
    {
        public Queen(bool white)
        {
            Value = 8;
            White = white;
        }
    }

    public class King : Piece
    {
        public King(bool white)
        {
            Value = 9;
            White = white;
        }
    }

    class ChessServer
    {
        // make six or seven lists to handle all the different games
        private static List<IPEndPoint> myEndPoints = new List<IPEndPoint>();
        private static List<Socket> myClients = new List<Socket>();
        private static List<GameState> tempStateHolder = new List<GameState>(8);
        private static Socket listeningSocket;
        private static object myLock = new object();

        /*
         * ChessServer.ChessServer.ProcessClientRequests()
         * 
         * NAME
         * 
         *     ChessServer.ChessServer.ProcessClientRequests - adds in new clients
         * 
         * SYNOPSIS
         * 
         *      void ProcessClientRequests();
         * 
         * DESCRIPTION
         * 
         *      This function opens a listening socket then runs in a forever loop.
         *      The forever loop accepts a new client and recieves 10 bytes from them
         *      Those ten bytes determine what kind of a game it is and the function
         *      locks the threads and calls ProcessNewGame with the 10 byte value sent
         *      
         * RETURNS
         * 
         *      void
         *      
         * AUTHOR
         *  
         *      Elliott Barinberg
         *      
         * DATE
         * 
         *      10:22 AM 3/27/2018
         * 
         */
         /**/
        private static void ProcessClientRequests()
        {
            //IPHostEntry iPHost = Dns.GetHostEntry("cs.ramapo.edu");
            //IPAddress iPAddress = iPHost.AddressList[0];
            IPAddress iPAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint ip = new IPEndPoint(iPAddress, 1234);
            IPEndPoint newEndPoint = null;
            listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket client = null;
            byte[] buffer = new byte[10];
            try
            {
                listeningSocket.Bind(ip);
                listeningSocket.Listen(10);
                while (true)
                {
                    client = listeningSocket.Accept();
                    myClients.Add(client);
                    newEndPoint = (IPEndPoint)client.RemoteEndPoint;
                    myEndPoints.Add(newEndPoint);
                    int x = 0;
                    while ((x += client.Receive(buffer, 10, SocketFlags.None)) < 10) ;
                    x = 0;
                    foreach(byte b in buffer)
                    {
                        x += Convert.ToInt32(b);
                    }
                    lock (myLock)
                    {
                        ProcessNewGame(x);
                    }
                    newEndPoint = null;
                    client = null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        /*private static void ProcessClientRequests();*/

        /*
         * ChessServer.ChessServer.ConvertGameStateToSendState(GameState gameState)
         * 
         * NAME
         * 
         *     ChessServer.ChessServer.ConvertGameStateToSendState - converts 2 sockets containing gamestate to sendable SendState
         * 
         * SYNOPSIS
         * 
         *      SendState ChessServer.ConvertGameStateToSendState(GameState a_gameState);
         *      a_gameState -> gamestate which needs to be converted
         *       
         * DESCRIPTION
         * 
         *      This function creates a new SendState out of the members of a_gameState and returns it
         *      
         * RETURNS
         * 
         *      SendState equivilant to the gamestate which was passed
         *      
         * AUTHOR
         *  
         *      Elliott Barinberg
         *      
         * DATE
         * 
         *      10:22 AM 3/27/2018
         * 
         */
        /**/
        private static SendState ConvertGameStateToSendState(GameState a_gameState)
        {
            SendState m_state = new SendState
            {
                AllPositions = a_gameState.AllPositions,
                BlackTimeLeft = a_gameState.BlackTimeLeft,
                Board = a_gameState.Board,
                Chat = a_gameState.Chat,
                CheckMate = a_gameState.CheckMate,
                DrawByRepitition = a_gameState.DrawByRepitition,
                Notation = a_gameState.Notation,
                StaleMate = a_gameState.StaleMate,
                WaitingForSecondPlayer = a_gameState.WaitingForSecondPlayer,
                WhiteTimeLeft = a_gameState.WhiteTimeLeft,
                WhiteToMove = a_gameState.WhiteToMove
            };
            return m_state;
        }
        /*private static SendState ConvertGameStateToSendState(GameState a_gameState);*/

        /*
         * ChessServer.ChessServer.ProcessNewGame(int a_message)
         * 
         * NAME
         * 
         *     ChessServer.ChessServer.ProcessNewGame - converts the client message into the gamestate which it belongs in
         * 
         * SYNOPSIS
         * 
         *      SendState ChessServer.ProcessNewGame(int a_message);
         *      a_message -> int describing how long of a game to play
         *       
         * DESCRIPTION
         * 
         *      This function first converts the game length sent by the client into an index of gamestate in tempgamestate
         *      Then if there is one client it will wait for the second
         *      If there is a second client it launches a thread with gamestate and clients in Play
         *      
         * RETURNS
         * 
         *      void
         *      
         * AUTHOR
         *  
         *      Elliott Barinberg
         *      
         * DATE
         * 
         *      10:22 AM 3/27/2018
         * 
         */
         /**/
        private static void ProcessNewGame(int a_message)
        {
            int m_index = 0;
            SendState state = new SendState();
            switch (a_message)
            {
                case 1:
                    m_index = 0;
                    break;
                case 3:
                    m_index = 1;
                    break;
                case 5:
                    m_index = 2;
                    break;
                case 10:
                    m_index = 3;
                    break;
                case 15:
                    m_index = 4;
                    break;
                case 30:
                    m_index = 5;
                    break;
                case 60:
                    m_index = 6;
                    break;
                case 75:
                    m_index = 7;
                    break;
                default:
                    break;
            }
            if(tempStateHolder.ElementAt(m_index).Player2 != null)
            {
                tempStateHolder[m_index].Player1 = null;
                tempStateHolder[m_index].Player2 = null;
            }
            if (tempStateHolder.ElementAt(m_index).Player1 == null)
            {
                GenerateNewGamestate(m_index);
                tempStateHolder.ElementAt(m_index).Player1 = myClients.Last();
                tempStateHolder.ElementAt(m_index).WaitingForSecondPlayer = true;
                tempStateHolder.ElementAt(m_index).EndPoint1 = myEndPoints.Last();
                state = ConvertGameStateToSendState(tempStateHolder.ElementAt(m_index));
                SendClientsGameState(state, tempStateHolder.ElementAt(m_index).Player1, null, null);
            }
            else
            {
                tempStateHolder.ElementAt(m_index).Player2 = myClients.Last();
                tempStateHolder.ElementAt(m_index).WaitingForSecondPlayer = false;
                tempStateHolder.ElementAt(m_index).EndPoint2 = myEndPoints.Last();
                tempStateHolder.ElementAt(m_index).AddToAllPositions(tempStateHolder.ElementAt(m_index).Board);
                state = ConvertGameStateToSendState(tempStateHolder.ElementAt(m_index));
                SendClientsGameState(state, tempStateHolder.ElementAt(m_index).Player1, tempStateHolder.ElementAt(m_index).Player2, null);
                Thread thread = new Thread(() => Play(tempStateHolder[m_index].Player1, tempStateHolder[m_index].Player2, state));
                thread.Start();
            }
        }
        /*private static void ProcessNewGame(int a_message);*/

        /*
         * ChessServer.ChessServer.GenerateNewGamestate(int a_index)
         * 
         * NAME
         * 
         *     ChessServer.ChessServer.GenerateNewGamestate - makes initial chess position and basic gamestate settings get set
         * 
         * SYNOPSIS
         * 
         *      SendState ChessServer.GenerateNewGamestate(int a_index);
         *      a_index -> index of tempGameState where the game should be stored
         *       
         * DESCRIPTION
         * 
         *      This function first converts generates the board
         *      Then it assigns the time and basic settings and adds it to tempStateHolder[index]
         *      
         * RETURNS
         * 
         *      void
         *      
         * AUTHOR
         *  
         *      Elliott Barinberg
         *      
         * DATE
         * 
         *      10:22 AM 3/27/2018
         * 
         */
         /**/
        private static void GenerateNewGamestate(int a_index)
        {
            Piece[,] m_board = new Piece[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    m_board[i, j] = new Empty();
                }
            }
            m_board[0, 0] = new Rook(false);
            m_board[0, 1] = new Knight(false);
            m_board[0, 2] = new Bishop(false);
            m_board[0, 3] = new Queen(false);
            m_board[0, 4] = new King(false);
            m_board[0, 5] = new Bishop(false);
            m_board[0, 6] = new Knight(false);
            m_board[0, 7] = new Rook(false);
            for (int i = 0; i < 8; i++)
            {
                m_board[1, i] = new Pawn(false);
                m_board[6, i] = new Pawn(true);
            }
            m_board[7, 0] = new Rook(true);
            m_board[7, 1] = new Knight(true);
            m_board[7, 2] = new Bishop(true);
            m_board[7, 3] = new Queen(true);
            m_board[7, 4] = new King(true);
            m_board[7, 5] = new Bishop(true);
            m_board[7, 6] = new Knight(true);
            m_board[7, 7] = new Rook(true);
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    m_board[i, j].HasMoved = false;
                }
            }
            GameState m_gameState = new GameState();
            switch (a_index)
            {
                case 0:
                    m_gameState.WhiteTimeLeft = 60;
                    m_gameState.BlackTimeLeft = 60;
                    break;
                case 1:
                    m_gameState.WhiteTimeLeft = 180;
                    m_gameState.BlackTimeLeft = 180;
                    break;
                case 2:
                    m_gameState.WhiteTimeLeft = 300;
                    m_gameState.BlackTimeLeft = 300;
                    break;
                case 3:
                    m_gameState.WhiteTimeLeft = 600;
                    m_gameState.BlackTimeLeft = 600;
                    break;
                case 4:
                    m_gameState.WhiteTimeLeft = 900;
                    m_gameState.BlackTimeLeft = 900;
                    break;
                case 5:
                    m_gameState.WhiteTimeLeft = 1800;
                    m_gameState.BlackTimeLeft = 1800;
                    break;
                case 6:
                    m_gameState.WhiteTimeLeft = 3600;
                    m_gameState.BlackTimeLeft = 3600;
                    break;
                case 7:
                    m_gameState.WhiteTimeLeft = 10800;
                    m_gameState.BlackTimeLeft = 10800;
                    break;
                default:
                    break;
            }
            m_gameState.Board = m_board;
            m_gameState.WhiteToMove = true;
            tempStateHolder[a_index] = m_gameState;
        }
        /*private static void GenerateNewGamestate(int a_index);*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="white"></param>
        /// <param name="black"></param>
        /// <param name="socket"></param>
        private static void SendClientsGameState(SendState a_state, Socket a_white, Socket a_black, Socket a_socket)
        {
            NetworkStream m_networkStream;
            StreamWriter m_streamWriter;
            if(a_white != a_socket)
            {
                a_state.White = true;
                string message = JsonConvert.SerializeObject(a_state);
                m_networkStream = new NetworkStream(a_white);
                m_streamWriter = new StreamWriter(m_networkStream);
                m_streamWriter.WriteLine(message);
                m_streamWriter.Flush();
            }
            if (a_black == null)
            {
                return;
            }
            if(a_black != a_socket)
            {
                a_state.White = false;
                string message = JsonConvert.SerializeObject(a_state);
                m_networkStream = new NetworkStream(a_black);
                m_streamWriter = new StreamWriter(m_networkStream);
                m_streamWriter.WriteLine(message);
                m_streamWriter.Flush();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="board1"></param>
        /// <param name="board2"></param>
        /// <returns></returns>
        private static bool IsSameBoard(Piece[,] a_board1, Piece[,] a_board2)
        {
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    if(a_board1[i, j].White != a_board2[i, j].White)
                    {
                        return false;
                    }
                    if(a_board1[i, j].Value != a_board2[i, j].Value)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        private static void CheckForDrawByRepitition(SendState a_state)
        {
            int m_count;
            for(int i = 0; i < a_state.AllPositions.Count; i++)
            {
                m_count = 0;
                for(int j = i + 1; j < a_state.AllPositions.Count; j++)
                {
                    if(IsSameBoard(a_state.AllPositions.ElementAt(i), a_state.AllPositions.ElementAt(j)))
                    {
                        m_count++;
                    }
                }
                if(m_count < 2)
                {
                    continue;
                }
                a_state.DrawByRepitition = true;
                break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whiteSocket"></param>
        /// <param name="blackSocket"></param>
        /// <param name="initialGameState"></param>
        private static void Play(Object a_whiteSocket, Object a_blackSocket, Object a_initialGameState)
        {
            Socket white = (Socket)a_whiteSocket;
            Socket black = (Socket)a_blackSocket;
            SendState gameState = (SendState)a_initialGameState;
            Exception endGame = new Exception("Game over");
            ArrayList checkRead = new ArrayList();
            Piece[,] board = new Piece[8, 8];
            char[] buffer = new char[1024];
            byte[] ByteBuff = new byte[1024];
            while (true)
            {
                try
                {
                    // reset checkRead and checkWrite and checkError
                    checkRead.RemoveRange(0, checkRead.Count);
                    checkRead.Add(white);
                    checkRead.Add(black);
                    if (checkRead.Count == 2)
                    {
                        Socket.Select(checkRead, null, null, -1);
                    }
                    else if(!gameState.WaitingForSecondPlayer && checkRead.Count == 1)
                    {
                        gameState.ServerError = true;
                        SendClientsGameState(gameState, white, black, null);
                        throw endGame;
                        // missing player!!!
                    }

                    for (int i = 0; i < checkRead.Count; i++)
                    {
                        if(((Socket)checkRead[i]).Poll(1000, SelectMode.SelectRead) == false)
                        {
                            ((Socket)checkRead[i]).Disconnect(true);
                            myClients.Remove((Socket)checkRead[i]);
                            continue;
                        }
                        NetworkStream ns = new NetworkStream((Socket)checkRead[i]);
                        StreamReader sr = new StreamReader(ns);
                        StreamWriter sw = new StreamWriter(ns);
                        string message = sr.ReadLine();
                        SendState state = new SendState();
                        state = JsonConvert.DeserializeObject<SendState>(message);
                        
                        // has the board changed if so the following is the change to be made
                        if(!IsSameBoard(state.Board, gameState.Board))
                        {
                            gameState.WhiteToMove = state.WhiteToMove;
                            gameState.WhiteTimeLeft = state.WhiteTimeLeft;
                            gameState.BlackTimeLeft = state.BlackTimeLeft;
                            gameState.AllPositions = state.AllPositions;
                            CheckForDrawByRepitition(gameState);
                            if (gameState.DrawByRepitition)
                            {
                                gameState.GameOver = true;
                                SendClientsGameState(gameState, white, black, null);
                                throw endGame;
                            }
                        }

                        // update the rest and send it out
                        gameState.Board = state.Board;
                        gameState.Chat = state.Chat;
                        gameState.Notation = state.Notation;
                        SendClientsGameState(gameState, white, black, (Socket)checkRead[i]);
                        if(gameState.DrawByRepitition && (Socket)checkRead[i] == white)
                        {
                            SendClientsGameState(gameState, white, black, white);
                            throw endGame;
                        }
                        else if(gameState.DrawByRepitition)
                        {
                            SendClientsGameState(gameState, white, black, black);
                            throw endGame;
                        }
                    }
                }
                catch (Exception e)
                {
                    if(e.Message == "Game over")
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                for (int i = 0; i <= 7; i++)
                {
                    tempStateHolder.Add(new GameState());
                }
                ProcessClientRequests();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}