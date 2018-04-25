using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using System.Collections;

namespace ChessServer
{
    /// <summary>
    /// Main GamePlay class
    /// </summary>
    public class ChessServer
    {
        // make six or seven lists to handle all the different games
        private static List<IPEndPoint> myEndPoints = new List<IPEndPoint>();
        private static List<Socket> myClients = new List<Socket>();
        private static List<GameState> tempStateHolder = new List<GameState>(8);
        private static Socket listeningSocket;
        private static object myLock = new object();

        /// <summary>
        /// Undoes the hash funtion in the client to allow the server to parse the message sent from the client
        /// </summary>
        ///  ChessServer.ChessServer.DeserializeInitialMessage()
        /// 
        ///   NAME
        ///     
        ///     ChessServer.ChessServer.DeserializeInitialMessage - parses clients first message and makes it readable
        ///     
        ///   SYNOPSIS
        ///    
        ///      string DeserializeInitialMessage(ref string a_message);
        ///   
        ///   AUTHOR
        ///           
        ///     Elliott Barinberg
        ///    
        ///   DATE
        ///
        ///     10:22 AM 3/27/2018
        ///     
        /// <param name="a_message">message from client</param>
        /// <returns>original string with no hash</returns>
        private static string DeSerializeInitialMessage(ref string a_message)
        {
            string m_toRet = String.Empty;
            for (int m_i = 0; m_i < a_message.Length; m_i++)
            {
                m_toRet += Convert.ToChar(a_message.ElementAt(m_i) + 2);
            }
            return m_toRet;
        }
        /*private static string DeSerializeInitialMessage(ref string a_message);*/

        /// <summary>
        ///     This method opens a listening socket then runs in a forever loop.
        ///
        ///     The forever loop accepts a new client and recieves 10 bytes from them
        ///     Those ten bytes determine what kind of a game it is and the function
        ///     locks the threads and calls ProcessNewGame with the 10 byte value sent
        /// </summary>
        ///  ChessServer.ChessServer.ProcessClientRequests()
        /// 
        ///   NAME
        ///     
        ///     ChessServer.ChessServer.ProcessClientRequests - adds in new clients
        ///     
        ///   SYNOPSIS
        ///    
        ///      void ProcessClientRequests();
        ///          
        ///   RETURNS
        ///          
        ///     void
        ///   
        ///   AUTHOR
        ///           
        ///     Elliott Barinberg
        ///    
        ///   DATE
        ///
        ///     10:22 AM 3/27/2018
        ///     
        private static void ProcessClientRequests()
        {
            IPAddress m_iPAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint m_ip = new IPEndPoint(m_iPAddress, 1234);
            IPEndPoint m_newEndPoint = null;
            listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket m_client = null;
            string m_message = String.Empty;
            try
            {
                listeningSocket.Bind(m_ip);
                listeningSocket.Listen(10);
                while (true)
                {
                    m_client = listeningSocket.Accept();
                    myClients.Add(m_client);
                    m_newEndPoint = (IPEndPoint)m_client.RemoteEndPoint;
                    myEndPoints.Add(m_newEndPoint);
                    NetworkStream m_networkStream = new NetworkStream(m_client);
                    StreamReader m_streamReader = new StreamReader(m_networkStream);
                    try
                    {
                        m_message = TimedReader.ReadLine(m_streamReader);
                        m_message = DeSerializeInitialMessage(ref m_message);
                        SendState m_tempState = JsonConvert.DeserializeObject<SendState>(m_message);
                        lock (myLock)
                        {
                            ProcessNewGame(m_tempState.TimeNumber);
                        }
                        m_newEndPoint = null;
                        m_client = null;
                    }
                    catch (TimeoutException)
                    {

                    }
                    catch (Exception e)
                    {
                        if(e.HResult != -2146233088)
                        {
                            throw e;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        /*private static void ProcessClientRequests();*/

        /// <summary>
        ///     This method creates a new SendState out of the members of a_gameState and returns it
        /// </summary>
        /// 
        ///     ChessServer.ChessServer.ConvertGameStateToSendState(GameState gameState)
        ///         
        ///     NAME
        ///          
        ///         ChessServer.ChessServer.ConvertGameStateToSendState - converts 2 sockets containing gamestate to sendable SendState
        ///          
        ///     SYNOPSIS
        ///         
        ///         SendState ChessServer.ConvertGameStateToSendState(GameState a_gameState);
        ///                
        ///     DESCRIPTION
        ///         
        ///         
        ///               
        ///     AUTHOR
        ///           
        ///         Elliott Barinberg
        ///               
        ///     DATE
        ///         
        ///         10:22 AM 3/27/2018
        /// <param name="a_gameState">gamestate which needs to be converted</param>
        /// <returns>SendState equivilant to the gamestate which was passed</returns>
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

        /// <summary>
        ///     This function first converts the game length sent by the client into an index of gamestate in tempgamestate
        ///     Then if there is one client it will wait for the second
        ///     If there is a second client it launches a thread with gamestate and clients in Play
        /// </summary>
        ///     ChessServer.ChessServer.ProcessNewGame(int a_message)
        ///         
        ///     NAME
        ///          
        ///         ChessServer.ChessServer.ProcessNewGame - converts the client message into the gamestate which it belongs in
        ///          
        ///     SYNOPSIS
        ///         
        ///         SendState ChessServer.ProcessNewGame(int a_message);
        ///              a_message -> int describing how long of a game to play
        ///         
        ///     RETURNS
        ///          
        ///         void
        ///         
        ///     AUTHOR
        ///          
        ///         Elliott Barinberg
        ///               
        ///     DATE
        ///         
        ///         10:22 AM 3/27/2018
        /// <param name="a_message">int describing how long of a game to play</param>
        private static void ProcessNewGame(int a_message)
        {
            int m_index = 0;
            SendState state = new SendState();
            // make index based on specified game length
            switch (a_message)
            {
                case 1:
                    m_index = 0;
                    break;
                case 5:
                    m_index = 1;
                    break;
                case 10:
                    m_index = 2;
                    break;
                case 15:
                    m_index = 3;
                    break;
                case 30:
                    m_index = 4;
                    break;
                case 60:
                    m_index = 5;
                    break;
                case 75:
                    m_index = 6;
                    break;
                default:
                    break;
            }

            // if 2 players already at the index, reset the players
            if(tempStateHolder.ElementAt(m_index).Player2 != null)
            {
                tempStateHolder[m_index].Player1 = null;
                tempStateHolder[m_index].Player2 = null;
            }


            // if the first player is not there generate the gamestate and make them player 1
            if (tempStateHolder.ElementAt(m_index).Player1 == null)
            {
                GenerateNewGamestate(m_index);
                tempStateHolder.ElementAt(m_index).Player1 = myClients.Last();
                tempStateHolder.ElementAt(m_index).WaitingForSecondPlayer = true;
                tempStateHolder.ElementAt(m_index).EndPoint1 = myEndPoints.Last();
                state = ConvertGameStateToSendState(tempStateHolder.ElementAt(m_index));
                SendClientsGameState(state, tempStateHolder.ElementAt(m_index).Player1, null, null);
            }

            // add player 2 and start the thread
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

        /// <summary>
        ///     This function first converts generates the board
        ///     Then it assigns the time and basic settings and adds it to tempStateHolder[index]
        /// </summary>
        ///     ChessServer.ChessServer.GenerateNewGamestate(int a_index)
        ///         
        ///     NAME
        ///          
        ///         ChessServer.ChessServer.GenerateNewGamestate - makes initial chess position and basic gamestate settings get set
        ///          
        ///     SYNOPSIS
        ///         
        ///         SendState ChessServer.GenerateNewGamestate(int a_index);
        ///         
        ///     RETURNS
        ///         
        ///         void
        ///    
        ///     AUTHOR
        ///           
        ///         Elliott Barinberg
        ///               
        ///     DATE
        ///         
        ///         10:22 AM 3/27/2018
        /// <param name="a_index">index of tempGameState where the game should be stored</param>
        private static void GenerateNewGamestate(int a_index)
        {
            // create board
            Piece[,] m_board = new Piece[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    m_board[i, j] = new Piece(0, false);
                }
            }
            m_board[0, 0] = new Piece(5, false);
            m_board[0, 1] = new Piece(4, false);
            m_board[0, 2] = new Piece(3, false);
            m_board[0, 3] = new Piece(8, false);
            m_board[0, 4] = new Piece(9, false);
            m_board[0, 5] = new Piece(3, false);
            m_board[0, 6] = new Piece(4, false);
            m_board[0, 7] = new Piece(5, false);
            for (int i = 0; i < 8; i++)
            {
                m_board[1, i] = new Piece(1, false);
                m_board[6, i] = new Piece(1, true);
            }
            m_board[7, 0] = new Piece(5, true);
            m_board[7, 1] = new Piece(4, true);
            m_board[7, 2] = new Piece(3, true);
            m_board[7, 3] = new Piece(8, true);
            m_board[7, 4] = new Piece(9, true);
            m_board[7, 5] = new Piece(3, true);
            m_board[7, 6] = new Piece(4, true);
            m_board[7, 7] = new Piece(5, true);
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
                    m_gameState.WhiteTimeLeft = 300;
                    m_gameState.BlackTimeLeft = 300;
                    break;
                case 2:
                    m_gameState.WhiteTimeLeft = 600;
                    m_gameState.BlackTimeLeft = 600;
                    break;
                case 3:
                    m_gameState.WhiteTimeLeft = 900;
                    m_gameState.BlackTimeLeft = 900;
                    break;
                case 4:
                    m_gameState.WhiteTimeLeft = 1800;
                    m_gameState.BlackTimeLeft = 1800;
                    break;
                case 5:
                    m_gameState.WhiteTimeLeft = 3600;
                    m_gameState.BlackTimeLeft = 3600;
                    break;
                case 6:
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
        ///     This function first checks that a_white is a_socket, if not it will activate the network
        ///     stream using a_white convert the sendstate to a message with the player as white and send the state
        ///     Then if a_black exists and is not equal to a_socket it does the same for the socket a_black with one
        ///     minor change to the state to have the client be the black player
        /// </summary>
        ///     ChessServer.ChessServer.SendClientsGameState(SendState a_state, Socket a_white, Socket a_black, Socket a_socket)
        ///         
        ///     NAME
        ///          
        ///         ChessServer.ChessServer.SendClientsGameState - function to write state to appropriate clients
        ///         
        ///     SYNOPSIS
        ///          
        ///         void SendClientsGameState(SendState a_state, Socket a_white, Socket a_black, Socket a_socket);
        ///               
        ///     RETURNS
        ///         
        ///         void
        ///    
        ///     AUTHOR
        ///         
        ///         Elliott Barinberg
        ///    
        ///     DATE
        ///         
        ///         10:22 AM 3/27/2018
        /// <param name="a_state">sendstate to serialize and send</param>
        /// <param name="a_white">white player socket</param>
        /// <param name="a_black">black player socket</param>
        /// <param name="a_socket">socket NOT to send data to this is the socket that just made the move</param>
        private static void SendClientsGameState(SendState a_state, Socket a_white, Socket a_black, Socket a_socket)
        {
            NetworkStream m_networkStream;
            StreamWriter m_streamWriter;
            if(a_white != a_socket && a_white.Connected)
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
            if(a_black != a_socket && a_black.Connected)
            {
                a_state.White = false;
                string message = JsonConvert.SerializeObject(a_state);
                m_networkStream = new NetworkStream(a_black);
                m_streamWriter = new StreamWriter(m_networkStream);
                m_streamWriter.WriteLine(message);
                m_streamWriter.Flush();
            }
        }
        /*private static void SendClientsGameState(SendState a_state, Socket a_white, Socket a_black, Socket a_socket);*/

        /// <summary>
        ///     This method will loop through both boards and check the the pieces at each given index are the same value and color
        ///     If at any point there is a difference it will return false but if the loops finish without differences it will return true
        /// </summary>
        ///   
        ///     ChessServer.ChessServer.IsSameBoard(Piece[,] a_board1, Piece[,] a_board2)
        ///         
        ///     NAME
        ///      
        ///         ChessServer.ChessServer.IsSameBoard - function to determine if 2 boards are the same arrangement of pieces
        ///     
        ///     SYNOPSIS
        ///      
        ///         void IsSameBoard(Piece[,] a_board1, Piece[,] a_board2);
        ///           
        ///     AUTHOR
        ///     
        ///         Elliott Barinberg
        ///
        ///     DATE
        ///      
        ///         10:22 AM 3/27/2018
        /// <param name="a_board1">[8,8] array of Pieces representing the first chess board</param>
        /// <param name="a_board2">[8,8] array of Pieces representing the second chess board</param>
        /// <returns>bool true if same boards false if different</returns>
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
        /*private static bool IsSameBoard(Piece[,] a_board1, Piece[,] a_board2);*/

        /// <summary>
        ///     This function will loop through the list of board in a_state.AllPositions
        ///     From there it loops through the rest of the boards and checks if they are the same
        ///     If so it increments m_count
        ///     If by the end of the list m_count >= 2 it sets a_state.DrawByRepitition to true and breaks
        ///     Otherwise it just continues on with no change to the state
        /// </summary>
        ///     ChessServer.ChessServer.CheckForDrawByRepitition(SendState a_state)
        ///         
        ///     NAME
        ///         
        ///         ChessServer.ChessServer.CheckForDrawByRepitition - function to determine if the same board has occured 3 times
        ///         
        ///     SYNOPSIS
        ///          
        ///         void CheckForDrawByRepitition(SendState a_state);
        ///               
        ///     DESCRIPTION
        ///         
        ///         
        ///         
        ///     RETURNS
        ///          
        ///         void
        ///         
        ///     AUTHOR
        ///           
        ///         Elliott Barinberg
        ///               
        ///     DATE
        ///         
        ///         10:22 AM 3/27/2018
        /// <param name="a_state">current state of game to check</param>
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
        /*private static void CheckForDrawByRepitition(SendState a_state);*/

        /// <summary>
        ///     This function will be the main gameplay function
        ///     It will be spawned in a new thread and then it converts the arguments back into the objects they actually are(Socket, Socket, SendState)
        ///     Then in a forever loop it will call select on both sockets
        ///     Whichever responds it will read the gamestate from
        ///     If the board changed between gamestates it will check for draw by repitition
        ///     after updating whose move it is and how much time is left, if no change in board the above will not happen
        ///     the board, chat, and notation will be updated and sent to the other client
        ///     If the game is over an exception will be thrown to take the function out of the forever loop
        /// </summary>
        ///     ChessServer.ChessServer.Play(Object a_whiteSocket, Object a_blackSocket, Object a_initialGameState)
        ///         
        ///     NAME
        ///          
        ///         ChessServer.ChessServer.Play - function to keep a game going between 2 users
        ///         
        ///     SYNOPSIS
        ///          
        ///         void Play(Object a_whiteSocket, Object a_blackSocket, Object a_initialGameState);
        ///               
        ///     RETURNS
        ///         
        ///         void
        ///    
        ///     AUTHOR
        ///           
        ///         Elliott Barinberg
        ///               
        ///     DATE
        ///         
        ///         10:22 AM 3/27/2018
        /// <param name="a_whiteSocket">white socket passed as thread parameter</param>
        /// <param name="a_blackSocket">black socket passed as thread parameter</param>
        /// <param name="a_initialGameState">inital SendState passed as thread parameter</param>
        private static void Play(Object a_whiteSocket, Object a_blackSocket, Object a_initialGameState)
        {
            Socket m_white = (Socket)a_whiteSocket;
            Socket m_black = (Socket)a_blackSocket;
            SendState m_gameState = (SendState)a_initialGameState;
            Exception m_endGame = new Exception("Game over");
            ArrayList m_checkRead = new ArrayList();
            NetworkStream m_networkStream;
            StreamReader m_streamReader;
            string m_message = String.Empty;
            while (true)
            {
                try
                {
                    // reset checkRead 
                    m_checkRead.RemoveRange(0, m_checkRead.Count);
                    m_checkRead.Add(m_white);
                    m_checkRead.Add(m_black);
                    if (m_checkRead.Count == 2 && m_white.Connected && m_black.Connected)
                    {
                        Socket.Select(m_checkRead, null, null, -1);
                    }
                    else
                    {
                        m_gameState.ServerError = true;
                        SendClientsGameState(m_gameState, m_white, m_black, null);
                    }

                    for (int i = 0; i < m_checkRead.Count; i++)
                    {
                        m_networkStream = new NetworkStream((Socket)m_checkRead[i]);
                        m_streamReader = new StreamReader(m_networkStream);
                        m_message = m_streamReader.ReadLine(); // throws exception if socket disconnected
                        SendState state = new SendState();
                        state = JsonConvert.DeserializeObject<SendState>(m_message);
                        
                        // has the board changed if so the following is the change to be made
                        if(!IsSameBoard(state.Board, m_gameState.Board))
                        {
                            m_gameState.TakenPieces = state.TakenPieces;
                            m_gameState.CheckMate = state.CheckMate;
                            m_gameState.StaleMate = state.StaleMate;
                            m_gameState.GameOver = state.GameOver;
                            m_gameState.WhiteToMove = state.WhiteToMove;
                            m_gameState.WhiteTimeLeft = state.WhiteTimeLeft;
                            m_gameState.BlackTimeLeft = state.BlackTimeLeft;
                            m_gameState.AllPositions = state.AllPositions;
                            CheckForDrawByRepitition(m_gameState);
                            if (m_gameState.DrawByRepitition)
                            {
                                m_gameState.GameOver = true;
                                SendClientsGameState(m_gameState, m_white, m_black, null);
                                throw m_endGame;
                            }
                        }

                        // update the rest and send it out
                        m_gameState.Board = state.Board;
                        m_gameState.Chat = state.Chat;
                        m_gameState.Notation = state.Notation;
                        SendClientsGameState(m_gameState, m_white, m_black, (Socket)m_checkRead[i]);
                        if(m_gameState.DrawByRepitition && (Socket)m_checkRead[i] == m_white)
                        {
                            SendClientsGameState(m_gameState, m_white, m_black, m_white);
                            throw m_endGame;
                        }
                        else if(m_gameState.DrawByRepitition)
                        {
                            SendClientsGameState(m_gameState, m_white, m_black, m_black);
                            throw m_endGame;
                        }
                    }
                }
                catch (Exception e)
                {
                    if(e.Message == "Game over")
                    {
                        // game ended naturally
                        break;
                    }
                    else if(e.HResult == -2146232800)
                    {
                        // disconnect
                        m_gameState.GameOver = true;
                        m_gameState.OpponentDisconnected = true;
                        SendClientsGameState(m_gameState, m_white, m_black, null);
                        break;
                    }
                    else
                    {
                        // should not be
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }
        /*private static void Play(Object a_whiteSocket, Object a_blackSocket, Object a_initialGameState);*/

        /// <summary>
        ///     This function will add 8 spaces to tempStateHolder and call ProcessClientRequests()
        /// </summary>
        ///     ChessServer.ChessServer.Main()
        ///         
        ///     NAME
        ///          
        ///         ChessServer.ChessServer.Main - function to start the program
        ///         
        ///     SYNOPSIS
        ///          
        ///         void Main();
        ///               
        ///     RETURNS
        ///         
        ///         void
        ///    
        ///     AUTHOR
        ///           
        ///         Elliott Barinberg
        ///               
        ///     DATE
        ///         
        ///         10:22 AM 3/27/2018
        ///         
        static void Main()
        {
            try
            {
                for (int i = 0; i < 7; i++)
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
        /*static void Main();*/
    }
}