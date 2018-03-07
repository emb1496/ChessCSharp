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
        private static List<GameState> allGames = new List<GameState>();
        private static List<IPEndPoint> myEndPoints = new List<IPEndPoint>();
        private static List<Socket> myClients = new List<Socket>();
        private static Socket socket;

        private static void ProcessClientRequests()
        {
            //IPHostEntry iPHost = Dns.GetHostEntry("cs.ramapo.edu");
            //IPAddress iPAddress = iPHost.AddressList[0];
            IPAddress iPAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint ip = new IPEndPoint(iPAddress, 1234);
            IPEndPoint newEndPoint = null;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket client = null;
            try
            {
                socket.Bind(ip);
                socket.Listen(20);
                while (true)
                {
                    client = socket.Accept();
                    myClients.Add(client);
                    newEndPoint = (IPEndPoint)client.RemoteEndPoint;
                    myEndPoints.Add(newEndPoint);
                    ProcessNewGame();
                    newEndPoint = null;
                    client = null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void ProcessNewGame()
        {
            if(myClients.Count % 2 == 1)
            {
                GenerateNewGamestate();
                allGames.Last().Player1 = myClients.Last();
                allGames.Last().WaitingForSecondPlayer = true;
                allGames.Last().EndPoint1 = myEndPoints.Last();
                SendClientsGameState(allGames.Last(), null);
            }
            else
            {
                allGames.Last().Player2 = myClients.Last();
                allGames.Last().WaitingForSecondPlayer = false;
                allGames.Last().EndPoint2 = myEndPoints.Last();
                allGames.Last().AddToAllPositions(allGames.Last().Board);
                SendClientsGameState(allGames.Last(), allGames.Last().Player1);
            }
        }

        // returns index of most recent board
        private static void GenerateNewGamestate()
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
            gameState.Board = board;
            gameState.WhiteToMove = true;
            allGames.Add(gameState);
        }

        private static void SendClientsGameState(GameState state, Socket socket)
        {
            SendState sendState = new SendState();
            NetworkStream ns;
            StreamReader sr;
            StreamWriter sw;
            sendState.AllPositions = state.AllPositions;
            sendState.Board = state.Board;
            sendState.Chat = state.Chat;
            sendState.CheckMate = state.CheckMate;
            sendState.Notation = state.Notation;
            sendState.StaleMate = state.StaleMate;
            sendState.WaitingForSecondPlayer = state.WaitingForSecondPlayer;
            sendState.WhiteToMove = state.WhiteToMove;
            if(state.Player1 != socket)
            {
                sendState.White = true;
                string message = JsonConvert.SerializeObject(sendState);
                ns = new NetworkStream(state.Player1);
                sr = new StreamReader(ns);
                sw = new StreamWriter(ns);
                sw.WriteLine(message);
                sw.Flush();
            }
            if (state.Player2 == null)
            {
                return;
            }
            if(state.Player2 != socket)
            {
                sendState.White = false;
                string message = JsonConvert.SerializeObject(sendState);
                ns = new NetworkStream(state.Player2);
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

        private static void CheckForDrawByRepitition(GameState state)
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

        private static void Play()
        {
            ArrayList checkRead = new ArrayList();
            ArrayList checkWrite = new ArrayList();
            ArrayList checkError = new ArrayList();

            Piece[,] board = new Piece[8, 8];
            char[] buffer = new char[1024];
            byte[] ByteBuff = new byte[1024];
            while (allGames.Count == 0) ;
            while (true)
            {
                // reset checkRead and checkWrite and checkError
                if(myClients.Count > 0)
                {
                    checkRead.RemoveRange(0, checkRead.Count);
                    checkWrite.RemoveRange(0, checkWrite.Count);
                    checkError.RemoveRange(0, checkError.Count);
                    foreach (Socket socket in myClients)
                    {
                        checkRead.Add(socket);
                        checkWrite.Add(socket);
                        checkError.Add(socket);
                    }
                }

                if (myClients.Count > 0)
                {
                    Socket.Select(checkRead, null, checkError, -1);
                }
            
                for (int i = 0; i < checkRead.Count; i++)
                {
                    int index = myClients.IndexOf((Socket)checkRead[i]);
                    try
                    {
                        NetworkStream ns = new NetworkStream(myClients.ElementAt(index));
                        StreamReader sr = new StreamReader(ns);
                        StreamWriter sw = new StreamWriter(ns);
                        string message = sr.ReadLine();
                        SendState state = new SendState();
                        state = JsonConvert.DeserializeObject<SendState>(message);
                        if(!IsSameBoard(state.Board, allGames.ElementAt(index / 2).Board))
                        {
                            allGames.ElementAt(index / 2).WhiteToMove = !state.WhiteToMove;
                            allGames.ElementAt(index / 2).AddToAllPositions(allGames.ElementAt(index / 2).Board);
                            CheckForDrawByRepitition(allGames.ElementAt(index / 2));
                        }

                        allGames.ElementAt(index / 2).Board = state.Board;
                        allGames.ElementAt(index / 2).Chat = state.Chat;
                        allGames.ElementAt(index / 2).Notation = state.Notation;
                        SendClientsGameState(allGames.ElementAt(index / 2), (Socket)checkRead[i]);
                        if(allGames.ElementAt(index / 2).DrawByRepitition && (Socket)checkRead[i] == allGames.ElementAt(index / 2).Player1)
                        {
                            SendClientsGameState(allGames.ElementAt(index / 2), allGames.ElementAt(index / 2).Player1);
                        }
                        else if(allGames.ElementAt(index / 2).DrawByRepitition)
                        {
                            SendClientsGameState(allGames.ElementAt(index / 2), allGames.ElementAt(index / 2).Player2);
                        }
                    }
                    catch(Exception except)
                    {
                        if(except.HResult == -2147467259)
                        {
                            myClients.ElementAt(i).Disconnect(true);
                            myClients.Remove(myClients.ElementAt(i));
                        }
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            try
            {
                while (true)
                {
                    Thread t = new Thread(ProcessClientRequests);
                    t.Start();
                    Play();
                }
            }
             catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}