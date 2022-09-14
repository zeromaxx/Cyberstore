using Microsoft.Owin;
using Owin;
using System;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(Eshop.Startup))]

namespace Eshop
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        
            //SignalR 
            app.MapSignalR();
        }

    }
}
