using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

// I denne udgave er klient behandlingen er her udskildt i selvstændig klasse
// Klassen bliver således en "aktiv" klasse, hvor en tåd afvilker RunClient metoden
// Klassen fungerer som en tråd-monitor

// Udgaven er ikke trådsikker med hensyn til samlingen af klienternes SteamWriter's

// Udvidelse med opdeling i flere metoder
// Sålænge den oprindelige samlede behandling er så simpel fås den store effekt ikke
// men den reviderede opdeling kan genbruges som skabelon og gør den nemmere at
// udvide antallet af komandoer ud over de 2: bye / alt andet

namespace ChatServerDesign_05_NoLock
{
	public class ClientHandler
	{
		private Socket clientSocket;
        private NetworkStream netStream;    // nu objekt data og ikke lokale - så de kan bruges på tværs af metoder
        private StreamWriter writer;        // nu objekt data og ikke lokale - så de kan bruges på tværs af metoder
        private StreamReader reader;        // nu objekt data og ikke lokale - så de kan bruges på tværs af metoder

        private ChatService chatService;    // Chat funktioner og samtidig monitor for fælles resource

        public ClientHandler(Socket clientSocket, ChatService chatService)
		{
			this.clientSocket = clientSocket;
            this.chatService = chatService;
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

                chatService.BroardCastEvent += this.BroadcastAction;      // tilføj klientens metode til broadcast tjeneste

                while (executeCommand()) ;       // fortsæt udførsel sålænge tue
            }
            catch
            { }
            finally
            {
                chatService.BroardCastEvent -= this.BroadcastAction;      // fjern klientens metode til broadcast tjeneste
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

            chatService.BroadCastBesked(input);

            return true;
        }

        public void BroadcastAction(string msg)
        {
            sendToClient("Broadcast:" + msg);
        }
    }
}
