﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchingServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();

            server.StartServer(8889);
            while (true)
            {
                
            }
        }
    }
}
