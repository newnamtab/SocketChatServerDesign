using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

// Her er ChatService behandlingen udskildt fra ClientHandleren
// Herved opn�s dels en adskildelse af funktionaliteten og gr�nsefladen (dialogen) til klienten.
// Desuden udskildes en f�lles resource, s�ledes at ChatService kan fungere som en resource-monitor
// for en resource delt af flere tr�de

// I denne udgave erstattes samlingen af klienter og genneml�b af disse, med brugen af delegater til de metoder der broadcastes til
// Med brug af "event" foran delegatet kan det g�res public og man kan undg� metoder for tilmelding og afmelding
// uden at bryde indkapslingen da man kun kan tilf�je metoder og fjerne metoder der angives og s�ledes er kendte. 

// Hermed anvendes den i .NET indbyggede elegante l�sning for et observer-m�nster.

// Der er ikke tr�dsikring med lock / Monitor i denne udgave

namespace ChatServerDesign_05_NoLock
{
    public class ChatService
    {
        public delegate void BroardCastEventHandler(string msg);  // Intern type-declaration ~ intern klasse - husk public

        public event BroardCastEventHandler BroardCastEvent;      // collection af metoder der skal kaldes
                                                                  // tilmelding med ....BroardCastEvent += metode
                                                                  // afmelding med ....BroardCastEvent -= metode
                                                                  // oprettes ved f�rste +=
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
            if (this.BroardCastEvent != null)   // check at objekt findes - mindst een har v�ret tilmeldt med +=
                BroardCastEvent(msg);           // aktiver alle tilmeldte metoder
        }
    }
}
