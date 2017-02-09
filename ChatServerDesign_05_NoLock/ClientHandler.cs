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

namespace ChatServerDesign_05_NoLock
{
	public class ClientHandler
	{
		private Socket clientSocket;
        private NetworkStream netStream;    // nu objekt data og ikke lokale - s� de kan bruges p� tv�rs af metoder
        private StreamWriter writer;        // nu objekt data og ikke lokale - s� de kan bruges p� tv�rs af metoder
        private StreamReader reader;        // nu objekt data og ikke lokale - s� de kan bruges p� tv�rs af metoder

        private ChatService chatService;    // Chat funktioner og samtidig monitor for f�lles resource

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

        //// alternativ - med using der tager sig af at lukke ogs� ved fejl (try og finaly med close )
        //public void RunClient()
        //{
        //    using (this.netStream = new NetworkStream(clientSocket))  // bem�rk at disse nu er "lever" p� objekt
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

                chatService.BroardCastEvent += this.BroadcastAction;      // tilf�j klientens metode til broadcast tjeneste

                while (executeCommand()) ;       // forts�t udf�rsel s�l�nge tue
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

            // sendToClient(("Echo:" + input);        // �ndring fra echo server - ikke med i chat

            chatService.BroadCastBesked(input);

            return true;
        }

        public void BroadcastAction(string msg)
        {
            sendToClient("Broadcast:" + msg);
        }
    }
}
