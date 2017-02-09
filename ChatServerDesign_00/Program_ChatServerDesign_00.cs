using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

// I denne udgave er alt lavet i samme klasse (bortset fra opstarts-klassen)
// Klient behandlingen er udskildt i selvst�ndig metode af hensyn til at der skal kunne afvilkers flere parallelt

// Server objektet har en collection med udg�ende forbindelser(streamWriter)
// som kan tilg�s fra ClientHandler metoden og hermed fra alle klient-handler tr�dene.

// Udgaven er ikke tr�dsikker med hensyn til samlingen af klienternes SteamWriter's

namespace ChatServerDesign_00
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Indtast Port - telnet er 23");
            string strPort = Console.ReadLine();
            if (strPort.Trim() == "") strPort = "12000";
            int port = int.Parse(strPort);
            new Server(port);
        }
    }

    public class Server
    {
        private List<StreamWriter> clientWriters = new List<StreamWriter>();    // aktive klienter       NY i forhold til echoserver

        public Server(int port)
        {
            System.Console.WriteLine("Server startet p� port:" + port);

            IPAddress ip = IPAddress.Parse("127.0.0.1");
            TcpListener listener = new TcpListener(ip, port);
            listener.Start();

            while (true)
            {
                System.Console.WriteLine("Server klar");

                Socket clientSocket = listener.AcceptSocket();

                System.Console.WriteLine("Der er g�et en i f�lden");

                // Her s�ttes op til af behandling foreg�r parallel
                // s� der kan forts�ttes med en ny
                Thread clientTr�d = new Thread(ClientHandler);
                clientTr�d.Start(clientSocket);
            }
        }

        private void ClientHandler(object clientSocketObject)
        {
            Socket clientSocket = clientSocketObject as Socket;

            NetworkStream netStream = new NetworkStream(clientSocket);
            StreamWriter writer = new StreamWriter(netStream);
            StreamReader reader = new StreamReader(netStream);

            try
            {
                writer.WriteLine("Server Klar - tast bye for at afslutte");

                clientWriters.Add(writer);      // tilf�j klientens streamwriter til samling   - NY i forhold til echoserver
                while (true)
                {
                    string input = reader.ReadLine();
                    if (input.Trim().ToLower() == "bye")
                        break;

                    //writer.WriteLine("Echo:" + input);        // ikke med i chat
                    //writer.Flush();                           // ikke med i chat

                    foreach (StreamWriter cw in this.clientWriters)    // gemmenl�b alle klientes output stream
                    {
                        try
                        {
                            cw.WriteLine("Broadcast:" + input);       // ikke med i chat
                            cw.Flush();                               // ikke med i chat
                        }
                        catch
                        { }
                    }
                }
            }
            catch
            { }
            finally
            {
                clientWriters.Remove(writer);      // tilf�j klientens streamwriter til samling  - NY i forhold til echoserver
            }
            writer.Close();
            reader.Close();
            netStream.Close();

            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }

        //// alternativ - med using der tager sig af at lukke ogs� ved fejl (try og finaly med close )
        //private void ClientHandler(object clientSocketObject)
        //{
        //    Socket clientSocket = clientSocketObject as Socket;
        //    using ( NetworkStream netStream = new NetworkStream(clientSocket))
        //    using ( StreamWriter writer = new StreamWriter(netStream))
        //    using ( StreamReader reader = new StreamReader(netStream))
        //    {
        //        try
        //        {
        //            writer.WriteLine("Server Klar - tast bye for at afslutte");

        //            clientWriters.Add(writer);      // tilf�j klientens streamwriter til samling   - NY i forhold til echoserver
        //            while (true)
        //            {
        //                string input = reader.ReadLine();
        //                if (input.Trim().ToLower() == "bye")
        //                    break;

        //                //writer.WriteLine("Echo:" + input);        // ikke med i chat
        //                //writer.Flush();                           // ikke med i chat

        //                foreach (StreamWriter cw in this.clientWriters)    // gemmenl�b alle klientes output stream
        //                {
        //                    try
        //                    {
        //                        cw.WriteLine("Broadcast:" + input);       // ikke med i chat
        //                        cw.Flush();                               // ikke med i chat
        //                    }
        //                    catch
        //                    { }
        //                }
        //            }
        //        }
        //        catch
        //        { }
        //        finally
        //        {
        //            clientWriters.Remove(writer);      // tilf�j klientens streamwriter til samling  - NY i forhold til echoserver
        //        }
        //    }
        //    clientSocket.Shutdown(SocketShutdown.Both);
        //    clientSocket.Close();
        //}
    }
}
