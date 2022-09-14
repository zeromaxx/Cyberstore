using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eshop
{
   
        [HubName("chathub")]
        public class ChatHub : Hub
        {
            public void Send(string name, string message)
            {
            // Call the broadcastMessage method to update clients.
            Clients.All.addtopage(name, message);
            }
        }
    
}