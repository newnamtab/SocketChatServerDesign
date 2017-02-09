using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;


// I denne udgave er klient behandlingen er her udskildt i selvstændig klasse
// Klassen bliver således en "aktiv" klasse, hvor en tåd afvilker RunClient metoden
// Klassen fungerer som en tråd-monitor

// Udgaven er ikke trådsikker med hensyn til samlingen af klienternes SteamWriter's

namespace ChatServerDesign_01_2
{
    public class ClientHandler
    {
        private Socket clientSocket;
        private NetworkStream netStream;    // nu objekt data og ikke lokale - så de kan bruges på tværs af metoder
        private StreamWriter writer;        // nu objekt data og ikke lokale - så de kan bruges på tværs af metoder
        private StreamReader reader;        // nu objekt data og ikke lokale - så de kan bruges på tværs af metoder

        private List<StreamWriter> clientWriters;    // aktive klienter       NY i forhold til echoserver

        public ClientHandler(Socket clientSocket, List<StreamWriter> clientWriters)
        {
            this.clientSocket = clientSocket;
            this.clientWriters = clientWriters;
        }

        public void RunClient()
        {
            this.netStream = new NetworkStream(clientSocket);
            this.writer = new StreamWriter(netStream);
            this.reader = new StreamReader(netStream);

            doDialog();

            this.writer.Close();
            this.reader.Close();
            this.netStream.Close();
            this.clientSocket.Shutdown(SocketShutdown.Both);
            this.clientSocket.Close();
        }

        //// alternativ - med using der tager sig af at lukke også ved fejl (try og finaly med close )
        //public void RunClient()
        //{
        //    using (this.netStream = new NetworkStream(clientSocket))  // bemærk at disse nu er "lever" på objekt
        //    using (this.writer = new StreamWriter(netStream))
        //    using (this.reader = new StreamReader(netStream))
        //    {
        //        doDialog();
        //    }
        //    this.clientSocket.Shutdown(SocketShutdown.Both);
        //    this.clientSocket.Close();
        //}

        private void sendToClient(string text)
        {
            writer.WriteLine(text);
            writer.Flush();
        }

        private string receiveFromClient()
        {
            try
            {
                return reader.ReadLine();
            }
            catch
            {
                return null;
            }
        }

        private void doDialog()
        {
            try
            {
                sendToClient("Server Klar - tast bye for at afslutte");

                clientWriters.Add(writer);      // tilføj klientens streamwriter til samling   - NY i forhold til echoserver

                while (executeCommand()) ;       // fortsæt udførsel sålænge tue
            }
            catch
            { }
            finally
            {
                clientWriters.Remove(writer);      // tilføj klientens streamwriter til samling  - NY i forhold til echoserver
            }
        }

        private bool executeCommand()  //returner false hvis null eller bye
        {
            // Behandling af input fra klient
            string input = receiveFromClient();

            if (input == null)
                return false;
            if (input.Trim().ToLower() == "bye")
                return false;

            // Behandling af andre komandoer

            // sendToClient(("Echo:" + input);        // ændring fra echo server - ikke med i chat

            foreach (StreamWriter cw in this.clientWriters)    // gemmenløb alle klientes output stream
            {
                try
                {
                    cw.WriteLine("Broadcast:" + input);       // ikke med i echo server
                    cw.Flush();                               // ikke med i echo server
                }
                catch
                { }
            }

            return true;
        }
    }
}
