using System;
using System.ServiceProcess;
using System.IO;
using System.ComponentModel;

namespace TimeLogger
{
    [RunInstallerAttribute(true)]
    public partial class TimeLoggerService : ServiceBase
    {
        public TimeLoggerService()
        {
            this.CanHandleSessionChangeEvent = true;
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Start();
        }

        protected override void OnStop()
        {
            writeOut("Shutdown");
            tmrLog.Enabled = false;
        }

        public void writeOut(string toWrite)
        {
            DateTime now = DateTime.Now;
            string currMonth = string.Format("{0:MMMM}", now);
            string currDay = string.Format("{0:dd}", now);
            string filename = string.Format("{0:HHmmss}", now);
            string filePath = string.Format("C:\\Temp\\Timesheets\\{0}\\{1}\\Individual\\{2}.txt", currMonth, currDay, filename);
            string fileText = string.Format("{0} {1}", now.ToShortTimeString(), toWrite);
            buildPath(filePath);
            var currFile = new StreamWriter(filePath, false);
            currFile.Write(fileText);
            currFile.Close();
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
                writeOut("Locked");
                tmrLog.Enabled = false;
            }
            else if (changeDescription.Reason == SessionChangeReason.SessionUnlock)
            {
                writeOut("Unlocked");
                tmrLog.Enabled = true;
            }
            else if (changeDescription.Reason == SessionChangeReason.SessionLogoff)
            {
                writeOut("Logged off");
                tmrLog.Enabled = false;
            }
            else if (changeDescription.Reason == SessionChangeReason.SessionLogon)
            {
                writeOut("Logged on");
                tmrLog.Enabled = true;
            }
        }

        public void Start()
        {
            writeOut("Startup");
            tmrLog.Interval = 30000;
            tmrLog.Enabled = true;
        }

        private void tmrLog_Tick(object sender, EventArgs e)
        {
            writeOut("Still on");
        }
    }
}
