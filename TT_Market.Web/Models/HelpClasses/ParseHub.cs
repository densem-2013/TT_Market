using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace TT_Market.Web.Models.HelpClasses
{
    [HubName("parseprogress")]
    public class ParseHub : Hub
    {
        public void Hello()
        {
            
            Clients.All.hello();
        }
    }
}