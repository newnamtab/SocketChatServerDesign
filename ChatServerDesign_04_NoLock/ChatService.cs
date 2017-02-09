using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

// Her er ChatService behandlingen udskildt fra ClientHandleren
// Herved opnås dels en adskildelse af funktionaliteten og grænsefladen (dialogen) til klienten.
// Desuden udskildes en fælles resource, således at ChatService kan fungere som en resource-monitor
// for en resource delt af flere tråde

// I denne udgave erstattes klienternes StreamWritere med ClientHandler'ne, således at de selv får besked
// når en besked skal broardcastes, hermed kan de også selv tage ansvaret for skrivningen og evt låse i den forbindelse

// Løsningen her bygger på observer-mønsteret, hvor der abonneres på en hændelse / forandring i et objekt forekommer.

// Der er ikke trådsikring med lock / Monitor i denne udgave

namespace ChatServerDesign_04_NoLock
{
    public class ChatService
    {
        private List<ClientHandler> broardcastClients = new List<ClientHandler>();
        private string name;

        public ChatService(string name)
        {
            this.name = name;
        }
        public string Name
        {
            get { return this.name; }
        }

        public void TilmeldBroardcasting (ClientHandler client)
        {
            broardcastClients.Add(client);
        }
        public void AfmeldBroardcasting(ClientHandler client)
        {
            broardcastClients.Remove(client);
        }

        public void BroadCastBesked (string msg)
        {
            foreach (ClientHandler client in broardcastClients)
            {
                client.BroadcastAction(msg);
            }
        }
    }
}
