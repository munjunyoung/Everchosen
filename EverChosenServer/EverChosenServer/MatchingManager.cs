﻿using System.Collections.Generic;
using System.Linq;

namespace EverChosenServer
{
    internal static class MatchingManager
    {
        // Clients that 
        public static Queue<Client> Clients = new Queue<Client>();

        internal static void MatchProcess(Client client)
        {
            if (Clients.Any())
            {
                var opponent = Clients.Dequeue();

                opponent.MatchingData.TeamColor = 1;
                client.MatchingData.TeamColor = 2;
                opponent.SendPacket("OnSucceedMatching", client);
                client.SendPacket("OnSucceedMatching", opponent);
            }
            else
            {
                Clients.Enqueue(client);
            }
        }
    }
}