using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TimeLogger
{
    public partial class Service1 : ServiceBase
    {
        StreamWriter currFile;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            DateTime now = DateTime.Now;
            currFile.WriteLine("{0} Startup", now.ToShortTimeString());
        }

        protected override void OnStop()
        {
            DateTime now = DateTime.Now;
            currFile.WriteLine("{0} Shutdown", now.ToShortTimeString());
        }

        public void build()
        {
            DateTime now = DateTime.Now;
            string currMonth = string.Format("{0:MMMM}", now);
            string currDay = string.Format("{0:dd}", now);
            string filename = string.Format("{0:hhmmss}", now);
            string filePath = string.Format("C:\\Temp\\Timesheets\\{0}\\{1}\\Individual\\{2}.txt", currMonth, currDay, filename);
            buildPath(filePath);
            currFile = new StreamWriter(filePath);
        }

        private static void buildPath(string inputPath)
        {
            FileInfo fileInfo = new FileInfo(inputPath);
            if (!fileInfo.Directory.Exists)
            {
                System.IO.Directory.CreateDirectory(fileInfo.DirectoryName);
            }
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            if (changeDescription.Reason == SessionChangeReason.SessionLock)
            {
                //I left my desk
            }
            else if (changeDescription.Reason == SessionChangeReason.SessionUnlock)
            {
                //I returned to my desk
            }
        }

    }
}
