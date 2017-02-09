using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

// I denne udgave er klient-h�ndteringen udskildt i en selvst�ndig klasse
// Der oprettes et selvst�ndigt ClientHandler objekt for hver forbindelse
// og der behandlingen startes s� op i hver sin tr�d
// Hver ClientHandler objekt har een aktiv tr�d, s�l�nge forbindelsen holdes �ben

// Udenne udgave er ChatService behandlingen udskildt fra ClientHandleren
// Herved opn�s dels en adskildelse af funktionaliteten og gr�nsefladen (dialogen) til klienten.
// Desuden udskildes en f�lles resource (Liste med klienterne), 
// s�ledes at ChatService kan fungere som en resource-monitor for en resource delt af flere tr�de

namespace ChatServerDesign_04_NoLock
{
    public class Server
    {
        private ChatService chatService = new ChatService("Socket chat-rum");

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

				ClientHandler handler = new ClientHandler(clientSocket, chatService);
								
				// Her s�ttes op til af behandling foreg�r parallel
				// s� der kan forts�ttes med en ny
                Thread clientTr�d = new Thread(handler.RunClient);
				clientTr�d.Start();
            }
        }
	}
}
