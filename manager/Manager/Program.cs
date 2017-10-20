using System;
using System.Reflection;

using log4net;

namespace Manager
{
    class Program
    {
        protected static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            Manager Manager = new Manager();

            Console.WriteLine("{0} has started.",Manager.ApplicationName);
            Console.WriteLine("Press enter to stop...");
            Console.ReadLine();

            Manager.Exit();            
        }
    }
}
