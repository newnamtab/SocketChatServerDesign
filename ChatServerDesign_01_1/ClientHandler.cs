using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

// I denne udgave er klient behandlingen er her udskildt i selvst�ndig klasse
// Klassen bliver s�ledes en "aktiv" klasse, hvor en t�d afvilker RunClient metoden
// Klassen fungerer som en tr�d-monitor

// Udgaven er ikke tr�dsikker med hensyn til samlingen af klienternes SteamWriter's

// Udvidelse med opdeling i flere metoder
// S�l�nge den oprindelige samlede behandling er s� simpel f�s den store effekt ikke
// men den reviderede opdeling kan genbruges som skabelon og g�r den nemmere at
// udvide antallet af komandoer ud over de 2: bye / alt andet

namespace ChatServerDesign_01_1
{
    public class ClientHandler
    {
        private Socket clientSocket;

        private List<StreamWriter> clientWriters;    // aktive klienter       NY i forhold til echoserver

        public ClientHandler(Socket clientSocket, List<StreamWriter> clientWriters)
        {
            this.clientSocket = clientSocket;
            this.clientWriters = clientWriters;
        }

        public void RunClient()
        {
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

                writer.Close();
                reader.Close();
                netStream.Close();
            }
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }

        //// alternativ - med using der tager sig af at lukke ogs� ved fejl (try og finaly med close )
        //public void RunClientMedUsing ()
        //{
        //    using (NetworkStream netStream = new NetworkStream(clientSocket))
        //    using (StreamWriter writer = new StreamWriter(netStream))
        //    using (StreamReader reader = new StreamReader(netStream))
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
