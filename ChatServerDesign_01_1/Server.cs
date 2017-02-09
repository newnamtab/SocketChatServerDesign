using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

// I denne udgave er klient-håndteringen udskildt i en selvstændig klasse
// Der oprettes et selvstændigt ClientHandler objekt for hver forbindelse
// og der behandlingen startes så op i hver sin tråd
// Hver ClientHandler objekt har een aktiv tråd, sålænge forbindelsen holdes åben

// Server objektet har en collection med udgående forbindelser(streamWriter).
// ClientHandler-objekter får adgang til en serverens collection med udgående forbindelser(streamWriter)
// som alle ClientHandler-objekter objekter så direkte kan tilgå

// I Forhold til ChatServerDesign_00 er eneste forskel, at ClientHandler tråde afvilkes
// i hver sit selvstændige objekt.

// Udgaven er ikke trådsikker med hensyn til samlingen af klienternes SteamWriter's

namespace ChatServerDesign_01_1
{
    public class Server
    {
        private List<StreamWriter> clientWriters = new List<StreamWriter>();    // aktive klienter       NY i forhold til echoserver

        public Server(int port)
        {
            System.Console.WriteLine("Server startet på port:" + port);

            IPAddress ip = IPAddress.Parse("127.0.0.1");
            TcpListener listener = new TcpListener(ip, port);
            listener.Start();

            while (true)
            {
                System.Console.WriteLine("Server klar");

                Socket clientSocket = listener.AcceptSocket();

                System.Console.WriteLine("Der er gået en i fælden");

				ClientHandler handler = new ClientHandler(clientSocket, clientWriters);
								
				// Her sættes op til af behandling foregår parallel
				// så der kan fortsættes med en ny
                Thread clientTråd = new Thread(handler.RunClient);
				clientTråd.Start();
            }
        }
	}
}
