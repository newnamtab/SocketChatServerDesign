using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

// Her er ChatService behandlingen udskildt fra ClientHandleren
// Herved opnås dels en adskildelse af funktionaliteten og grænsefladen (dialogen) til klienten.
// Desuden udskildes en fælles resource, således at ChatService kan fungere som en resource-monitor
// for en resource delt af flere tråde

// I denne udgave erstattes samlingen af klienter og gennemløb af disse, med brugen af delegater til de metoder der broadcastes til
// Med brug af "event" foran delegatet kan det gøres public og man kan undgå metoder for tilmelding og afmelding
// uden at bryde indkapslingen da man kun kan tilføje metoder og fjerne metoder der angives og således er kendte. 

// Hermed anvendes den i .NET indbyggede elegante løsning for et observer-mønster.

// Der er ikke trådsikring med lock / Monitor i denne udgave

namespace ChatServerDesign_05_NoLock
{
    public class ChatService
    {
        public delegate void BroardCastEventHandler(string msg);  // Intern type-declaration ~ intern klasse - husk public

        public event BroardCastEventHandler BroardCastEvent;      // collection af metoder der skal kaldes
                                                                  // tilmelding med ....BroardCastEvent += metode
                                                                  // afmelding med ....BroardCastEvent -= metode
                                                                  // oprettes ved første +=
        private string name;

        public ChatService(string name)
        {
            this.name = name;
        }
        public string Name
        {
            get { return this.name; }
        }

        public void BroadCastBesked (string msg)
        {
            if (this.BroardCastEvent != null)   // check at objekt findes - mindst een har været tilmeldt med +=
                BroardCastEvent(msg);           // aktiver alle tilmeldte metoder
        }
    }
}
