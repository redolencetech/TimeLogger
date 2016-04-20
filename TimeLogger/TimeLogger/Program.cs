using System;
using System.ServiceProcess;

namespace TimeLogger
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var program = new TimeLoggerService();
            if (Environment.UserInteractive)
            {
                program.Start();
            }
            else
            {
                ServiceBase.Run(new ServiceBase[]
                {
                program
                });
            }
        }
    }
}
