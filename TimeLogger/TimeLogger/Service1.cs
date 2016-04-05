using System;
using System.ServiceProcess;
using System.IO;

namespace TimeLogger
{
    public partial class Service1 : ServiceBase
    {
        StreamWriter currFile;
        DateTime now;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            build();
            currFile.WriteLine("{0} Startup", now.ToShortTimeString());
            tmrLog.Interval = 30000;
            tmrLog.Enabled = true;
        }

        protected override void OnStop()
        {
            build();
            currFile.WriteLine("{0} Shutdown", now.ToShortTimeString());
            tmrLog.Enabled = false;
        }

        public void build()
        {
            now = DateTime.Now;
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
                build();
                currFile.WriteLine("{0} Locked", now.ToShortTimeString());
            }
            else if (changeDescription.Reason == SessionChangeReason.SessionUnlock)
            {
                build();
                currFile.WriteLine("{0} Unlocked", now.ToShortTimeString());
            }
        }

        private void tmrLog_Tick(object sender, EventArgs e)
        {
            build();
            currFile.WriteLine("{0} Still on", now.ToShortTimeString());
        }
    }
}
