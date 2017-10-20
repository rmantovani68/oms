using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Topshelf;

namespace DotNetMQ
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new DotNetMqService() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }

    public class DotNetMQService
    {
        MDS.MDSServer Server = new MDS.MDSServer();

        public void Start() { Server.Start(); }
        public void Stop() { Server.Stop(true); }
    }
    /*
     * esperimento per cambiare il lancio del servizio utilizzando HostFactory ...
     * 
    public class Program
    {
        public static void Main()
        {
            HostFactory.Run(x =>
            {
                x.Service<DotNetMQService>(s =>
                {
                    s.ConstructUsing(name => new DotNetMQService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("DotNetMQ Message Broker");
                x.SetDisplayName("DotNetMQ");
                x.SetServiceName("DotNetMQ");
            });
        }
    }
    */




}






