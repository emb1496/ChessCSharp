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

    public class SendState
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
        private bool waitingForSecondPlayer;
        private int blackTimeLeft;
        private int whiteTimeLeft;
        private int timePortOffset;

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
        }
    }

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
            allPositions = new List<Piece[,]>();
            whiteTimeLeft = 10000;
            blackTimeLeft = 10000;
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
                listeningSocket.Listen(100000);
                while (true)
                {
                    client = listeningSocket.Accept();
                    myClients.Add(client);
                    newEndPoint = (IPEndPoint)client.RemoteEndPoint;
                    myEndPoints.Add(newEndPoint);
                    int x = 2;
                    x = client.Receive(buffer, 10, SocketFlags.None);
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

        private static SendState ConvertGameStateToSendState(GameState gameState)
        {
            SendState state = new SendState
            {
                AllPositions = gameState.AllPositions,
                BlackTimeLeft = gameState.BlackTimeLeft,
                Board = gameState.Board,
                Chat = gameState.Chat,
                CheckMate = gameState.CheckMate,
                DrawByRepitition = gameState.DrawByRepitition,
                Notation = gameState.Notation,
                StaleMate = gameState.StaleMate,
                WaitingForSecondPlayer = gameState.WaitingForSecondPlayer,
                WhiteTimeLeft = gameState.WhiteTimeLeft,
                WhiteToMove = gameState.WhiteToMove
            };
            return state;
        }

        private static void ProcessNewGame(int x)
        {
            int which = 0;
            SendState state = new SendState();
            switch (x)
            {
                case 1:
                    which = 0;
                    break;
                case 3:
                    which = 1;
                    break;
                case 5:
                    which = 2;
                    break;
                case 10:
                    which = 3;
                    break;
                case 15:
                    which = 4;
                    break;
                case 30:
                    which = 5;
                    break;
                case 60:
                    which = 6;
                    break;
                case 75:
                    which = 7;
                    break;
                default:
                    break;
            }
            if(tempStateHolder.ElementAt(which).Player1 == null)
            {
                GenerateNewGamestate(which);
                tempStateHolder.ElementAt(which).Player1 = myClients.Last();
                tempStateHolder.ElementAt(which).WaitingForSecondPlayer = true;
                tempStateHolder.ElementAt(which).EndPoint1 = myEndPoints.Last();
                state = ConvertGameStateToSendState(tempStateHolder.ElementAt(which));
                SendClientsGameState(state, tempStateHolder.ElementAt(which).Player1, null, null);
            }
            else
            {
                tempStateHolder.ElementAt(which).Player2 = myClients.Last();
                
                tempStateHolder.ElementAt(which).WaitingForSecondPlayer = false;
                tempStateHolder.ElementAt(which).EndPoint2 = myEndPoints.Last();
                tempStateHolder.ElementAt(which).AddToAllPositions(tempStateHolder.ElementAt(which).Board);
                state = ConvertGameStateToSendState(tempStateHolder.ElementAt(which));
                SendClientsGameState(state, tempStateHolder.ElementAt(which).Player1, tempStateHolder.ElementAt(which).Player2, null);
                Thread thread = new Thread(() =>
                    Play(tempStateHolder[which].Player1, tempStateHolder[which].Player2, state));
                thread.Start();
                tempStateHolder[which].Player1 = null;
                tempStateHolder[which].Player2 = null;
            }
        }

        private static void GenerateNewGamestate(int which)
        {
            Piece[,] board = new Piece[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board[i, j] = new Empty();
                }
            }
            board[0, 0] = new Rook(false);
            board[0, 1] = new Knight(false);
            board[0, 2] = new Bishop(false);
            board[0, 3] = new Queen(false);
            board[0, 4] = new King(false);
            board[0, 5] = new Bishop(false);
            board[0, 6] = new Knight(false);
            board[0, 7] = new Rook(false);
            for (int i = 0; i < 8; i++)
            {
                board[1, i] = new Pawn(false);
                board[6, i] = new Pawn(true);
            }
            board[7, 0] = new Rook(true);
            board[7, 1] = new Knight(true);
            board[7, 2] = new Bishop(true);
            board[7, 3] = new Queen(true);
            board[7, 4] = new King(true);
            board[7, 5] = new Bishop(true);
            board[7, 6] = new Knight(true);
            board[7, 7] = new Rook(true);
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    board[i, j].HasMoved = false;
                }
            }
            GameState gameState = new GameState();
            switch (which)
            {
                case 0:
                    gameState.WhiteTimeLeft = 60;
                    gameState.BlackTimeLeft = 60;
                    break;
                case 1:
                    gameState.WhiteTimeLeft = 180;
                    gameState.BlackTimeLeft = 180;
                    break;
                case 2:
                    gameState.WhiteTimeLeft = 300;
                    gameState.BlackTimeLeft = 300;
                    break;
                case 3:
                    gameState.WhiteTimeLeft = 600;
                    gameState.BlackTimeLeft = 600;
                    break;
                case 4:
                    gameState.WhiteTimeLeft = 900;
                    gameState.BlackTimeLeft = 900;
                    break;
                case 5:
                    gameState.WhiteTimeLeft = 1800;
                    gameState.BlackTimeLeft = 1800;
                    break;
                case 6:
                    gameState.WhiteTimeLeft = 3600;
                    gameState.BlackTimeLeft = 3600;
                    break;
                case 7:
                    gameState.WhiteTimeLeft = 10800;
                    gameState.BlackTimeLeft = 10800;
                    break;
                default:
                    break;
            }
            gameState.Board = board;
            gameState.WhiteToMove = true;
            tempStateHolder[which] = gameState;
        }

        private static void SendClientsGameState(SendState state, Socket white, Socket black, Socket socket)
        {

            NetworkStream ns;
            StreamReader sr;
            StreamWriter sw;
            if(white != socket)
            {
                state.White = true;
                string message = JsonConvert.SerializeObject(state);
                ns = new NetworkStream(white);
                sr = new StreamReader(ns);
                sw = new StreamWriter(ns);
                sw.WriteLine(message);
                sw.Flush();
            }
            if (black == null)
            {
                return;
            }
            if(black != socket)
            {
                state.White = false;
                string message = JsonConvert.SerializeObject(state);
                ns = new NetworkStream(black);
                sr = new StreamReader(ns);
                sw = new StreamWriter(ns);
                sw.WriteLine(message);
                sw.Flush();
            }
        }

        private static bool IsSameBoard(Piece[,] board1, Piece[,] board2)
        {
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    if(board1[i, j].White != board2[i, j].White)
                    {
                        return false;
                    }
                    if(board1[i, j].Value != board2[i, j].Value)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static void CheckForDrawByRepitition(SendState state)
        {
            int count;
            for(int i = 0; i < state.AllPositions.Count; i++)
            {
                count = 0;
                for(int j = i + 1; j < state.AllPositions.Count; j++)
                {
                    if(IsSameBoard(state.AllPositions.ElementAt(i), state.AllPositions.ElementAt(j)))
                    {
                        count++;
                    }
                }
                if(count < 2)
                {
                    continue;
                }
                state.DrawByRepitition = true;
                break;
            }
        }

        private static void Play(Object whiteSocket, Object blackSocket, Object initialGameState)
        {
            Socket white = (Socket)whiteSocket;
            Socket black = (Socket)blackSocket;
            SendState gameState = (SendState)initialGameState;
            ArrayList checkRead = new ArrayList();
            ArrayList checkWrite = new ArrayList();
            ArrayList checkError = new ArrayList();
            Piece[,] board = new Piece[8, 8];
            char[] buffer = new char[1024];
            byte[] ByteBuff = new byte[1024];
            while (true)
            {
                // reset checkRead and checkWrite and checkError
                checkRead.RemoveRange(0, checkRead.Count);
                checkWrite.RemoveRange(0, checkWrite.Count);
                checkError.RemoveRange(0, checkError.Count);
                checkRead.Add(white);
                checkRead.Add(black);
                if (checkRead.Count > 0)
                {
                    Socket.Select(checkRead, null, checkError, -1);
                }

                for (int i = 0; i < checkRead.Count; i++)
                {
                    try
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
                        

                        if(!IsSameBoard(state.Board, gameState.Board))
                        {
                            gameState.WhiteToMove = state.WhiteToMove;
                            gameState.WhiteTimeLeft = state.WhiteTimeLeft;
                            gameState.BlackTimeLeft = state.BlackTimeLeft;
                            gameState.AllPositions = state.AllPositions;
                            CheckForDrawByRepitition(gameState);
                        }

                        gameState.Board = state.Board;
                        gameState.Chat = state.Chat;
                        gameState.Notation = state.Notation;
                        SendClientsGameState(gameState, white, black, (Socket)checkRead[i]);
                        if(gameState.DrawByRepitition && (Socket)checkRead[i] == white)
                        {
                            SendClientsGameState(gameState, white, black, white);
                        }
                        else if(gameState.DrawByRepitition)
                        {
                            SendClientsGameState(gameState, white, black, black);
                        }
                    }
                    catch(Exception except)
                    {
                        Console.WriteLine(except.Message);
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            try
            {
                for (int i = 0; i <= 7; i++)
                {
                    tempStateHolder.Add(new GameState());
                }
                Thread t = new Thread(ProcessClientRequests);
                t.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}