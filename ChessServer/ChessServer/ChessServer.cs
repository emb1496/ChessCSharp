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
        private static List<List<GameState>> allGames = new List<List<GameState>>();
        private static List<IPEndPoint> myEndPoints = new List<IPEndPoint>();
        private static List<Socket> myClients = new List<Socket>();
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

        private static void ProcessNewGame(int x)
        {
            int which = 0;
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
            if(allGames[which].Count % 2 == 0)
            {
                GenerateNewGamestate(which);
                allGames[which].Last().Player1 = myClients.Last();
                allGames[which].Last().WaitingForSecondPlayer = true;
                allGames[which].Last().EndPoint1 = myEndPoints.Last();
                SendClientsGameState(allGames[which].Last(), null);
            }
            else
            {
                allGames[which].Last().Player2 = myClients.Last();
                allGames[which].Last().WaitingForSecondPlayer = false;
                allGames[which].Last().EndPoint2 = myEndPoints.Last();
                allGames[which].Last().AddToAllPositions(allGames[which].Last().Board);
                SendClientsGameState(allGames[which].Last(), null);
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
            allGames[which].Add(gameState);
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
            sendState.WhiteTimeLeft = state.WhiteTimeLeft;
            sendState.BlackTimeLeft = state.BlackTimeLeft;
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

        private static void Play(Object inde)
        {
            int index = Convert.ToInt32(inde);
            ArrayList checkRead = new ArrayList();
            ArrayList checkWrite = new ArrayList();
            ArrayList checkError = new ArrayList();
            Piece[,] board = new Piece[8, 8];
            char[] buffer = new char[1024];
            byte[] ByteBuff = new byte[1024];
            while (true)
            {
                // reset checkRead and checkWrite and checkError
            
                if(allGames[index].Count> 0)
                {
                    checkRead.RemoveRange(0, checkRead.Count);
                    checkWrite.RemoveRange(0, checkWrite.Count);
                    checkError.RemoveRange(0, checkError.Count);
                    foreach (GameState a_State in allGames[index])
                    {
                        if (a_State.Player2 != null)
                        {
                            Socket socket1 = a_State.Player1;
                            checkRead.Add(socket1);
                            checkWrite.Add(socket1);
                            checkError.Add(socket1);
                            Socket socket2 = a_State.Player2;
                            checkRead.Add(socket2);
                            checkWrite.Add(socket2);
                            checkError.Add(socket2);
                        }
                    }
                    if (checkRead.Count > 0)
                    {
                        Socket.Select(checkRead, null, checkError, -1);
                    }
                }

                for (int i = 0; i < checkRead.Count; i++)
                {
                    List<GameState> temp = new List<GameState>();
                    temp = allGames[index];
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
                        // find which index in temp we are referencing?
                        int newIndex = 0;
                        foreach(GameState tempState in temp)
                        {
                            if (tempState.Player1 == (Socket)checkRead[i] || tempState.Player2 == (Socket)checkRead[i])
                            {
                                break;
                            }
                            newIndex++;
                        }
                        if(!IsSameBoard(state.Board, temp.ElementAt(newIndex).Board))
                        {
                            temp.ElementAt(newIndex).WhiteToMove = state.WhiteToMove;
                            temp.ElementAt(newIndex).WhiteTimeLeft = state.WhiteTimeLeft;
                            temp.ElementAt(newIndex).BlackTimeLeft = state.BlackTimeLeft;
                            temp.ElementAt(newIndex).AllPositions = state.AllPositions;
                            CheckForDrawByRepitition(temp.ElementAt(newIndex));
                        }

                        temp.ElementAt(newIndex).Board = state.Board;
                        temp.ElementAt(newIndex).Chat = state.Chat;
                        temp.ElementAt(newIndex).Notation = state.Notation;
                        SendClientsGameState(temp.ElementAt(newIndex), (Socket)checkRead[i]);
                        if(temp.ElementAt(newIndex).DrawByRepitition && (Socket)checkRead[i] == temp.ElementAt(newIndex).Player1)
                        {
                            SendClientsGameState(temp.ElementAt(newIndex), temp.ElementAt(newIndex).Player1);
                        }
                        else if(temp.ElementAt(newIndex).DrawByRepitition)
                        {
                            SendClientsGameState(temp.ElementAt(newIndex), temp.ElementAt(newIndex).Player2);
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
                    allGames.Add(new List<GameState>());
                }
                Thread t = new Thread(ProcessClientRequests);
                t.Start();
                Thread t1 = new Thread(Play);
                Thread t2 = new Thread(Play);
                Thread t3 = new Thread(Play);
                Thread t4 = new Thread(Play);
                Thread t5 = new Thread(Play);
                Thread t6 = new Thread(Play);
                Thread t7 = new Thread(Play);
                Thread t8 = new Thread(Play);
                while(t1.ThreadState == ThreadState.Unstarted || t2.ThreadState == ThreadState.Unstarted || t3.ThreadState == ThreadState.Unstarted || t4.ThreadState == ThreadState.Unstarted || t5.ThreadState == ThreadState.Unstarted || t6.ThreadState == ThreadState.Unstarted || t7.ThreadState == ThreadState.Unstarted || t8.ThreadState == ThreadState.Unstarted)
                {
                    if(t1.ThreadState == ThreadState.Unstarted && allGames[0].Count != 0)
                    {
                        t1.Start(0);
                    }
                    if (t2.ThreadState == ThreadState.Unstarted && allGames[1].Count != 0)
                    {
                        t2.Start(1);
                    }

                    if (t3.ThreadState == ThreadState.Unstarted && allGames[2].Count != 0)
                    {
                        t3.Start(2);
                    }

                    if (t4.ThreadState == ThreadState.Unstarted && allGames[3].Count != 0)
                    {
                        t4.Start(3);
                    }
                    if (t5.ThreadState == ThreadState.Unstarted && allGames[4].Count != 0)
                    {
                        t5.Start(4);
                    }

                    if (t6.ThreadState == ThreadState.Unstarted && allGames[5].Count != 0)
                    {
                        t6.Start(5);
                    }

                    if (t7.ThreadState == ThreadState.Unstarted && allGames[6].Count != 0)
                    {
                        t7.Start(6);
                    }

                    if (t8.ThreadState == ThreadState.Unstarted && allGames[7].Count != 0)
                    {
                        t8.Start(7);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}