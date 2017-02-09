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

// Server objektet har en collection med udg�ende forbindelser(streamWriter).
// ClientHandler-objekter f�r adgang til en serverens collection med udg�ende forbindelser(streamWriter)
// som alle ClientHandler-objekter objekter s� direkte kan tilg�

// I Forhold til ChatServerDesign_00 er eneste forskel, at ClientHandler tr�de afvilkes
// i hver sit selvst�ndige objekt.

// Udgaven er ikke tr�dsikker med hensyn til samlingen af klienternes SteamWriter's

namespace ChatServerDesign_01_1
{
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

				ClientHandler handler = new ClientHandler(clientSocket, clientWriters);
								
				// Her s�ttes op til af behandling foreg�r parallel
				// s� der kan forts�ttes med en ny
                Thread clientTr�d = new Thread(handler.RunClient);
				clientTr�d.Start();
            }
        }
	}
}
