using System;
using System.Threading;
using System.IO;

namespace ChessServer
{
    /// <summary>
    /// This is a class which we use to read from a client we just accepted. Unfortunately because read is a blocking call and we need the data from the read later on to set up the game 
    /// If a client connects and does not send any data it would completely screw the server and freeze it. This class puts a 1.5s wrapper around the read function and allows the program
    /// to only read for 1.5s before we decide the client has nothing good to say and is trying to pull a man in the middle attack
    /// </summary>
    class TimedReader
    {
        private static Thread readThread;
        private static AutoResetEvent getMessage, gotMessage;
        private static string message;
        private static StreamReader streamReader;

        /// <summary>
        /// cunstructor for the TimedReader class, sets got and get Message to false and spawns a new background thread to call StreamReader.readline
        /// </summary>
        static TimedReader()
        {
            getMessage = new AutoResetEvent(false);
            gotMessage = new AutoResetEvent(false);
            readThread = new Thread(Reader)
            {
                IsBackground = true
            };
            readThread.Start();
        }

        /// <summary>
        /// Calls read line after the constructor spawns this thread, if it gets a message then it will set GotMessage to true
        /// </summary>
        private static void Reader()
        {
            while (true)
            {
                getMessage.WaitOne();
                message = streamReader.ReadLine();
                gotMessage.Set();
            }
        }

        /// <summary>
        /// sets getMessage to true which allows Reader to call readLine and then calls gotMessage.WaitOne(1500) -> waits for 1.5s for gotMessage to be true
        /// If the message came we return the message
        /// Otherwise we throw a Timeout Exception
        /// </summary>
        /// <param name="a_streamReader">StreamReader to read from</param>
        /// <returns>Message from server, or throws an exception</returns>
        public static string ReadLine(StreamReader a_streamReader)
        {
            streamReader = a_streamReader;
            getMessage.Set();
            bool success = gotMessage.WaitOne(1500);
            if (success)
            {
                return message;
            }
            else
            {
                throw new TimeoutException("There was no message to recieve");
            }
        }
    }
}
