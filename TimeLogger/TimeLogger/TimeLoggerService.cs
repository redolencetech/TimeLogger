using System;
using System.ServiceProcess;
using System.IO;
using System.ComponentModel;
using System.Linq;
using System.Globalization;
using System.Text;

namespace TimeLogger
{
    [RunInstallerAttribute(true)]
    public partial class TimeLoggerService : ServiceBase
    {
        DateTime now;
        string dailyFolder;
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
            now = DateTime.Now;
            string currMonth = string.Format("{0:MMMM}", now);
            string currDay = string.Format("{0:dd}", now);
            string filename = string.Format("{0:HHmmss}", now);
            dailyFolder = string.Format("C:\\Temp\\Timesheets\\{0}\\{1}", currMonth, currDay);
            string filePath = string.Format("{0}\\Individual\\{1}.txt", dailyFolder, filename);
            string fileText = string.Format("{0} {1}", now.ToShortTimeString(), toWrite);
            buildPath(filePath);
            writeFile(filePath, fileText);
            summariseDay();
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

        private string readFile(string fileToRead, bool wholeFile)
        {
            StreamReader file = new StreamReader(fileToRead);
            string r;
            if (wholeFile)
            {
                r = file.ReadToEnd().ToString();
            }
            else
            {
                r = file.ReadToEnd().ToString().Substring(0, 8);
            }
            file.Close();
            return r.Trim();
        }

        private void writeFile(string filePath, string textToWrite)
        {
            var currFile = new StreamWriter(filePath, true);
            currFile.WriteLine(textToWrite.Trim());
            currFile.Close();
        }

        private void summariseDay()
        {
            now = DateTime.Now;
            var files = from file in Directory.GetFiles(string.Format("{0}\\Individual", dailyFolder))
                        orderby file ascending
                        select file;

            string dailySummaryFilename = string.Format("{0}\\{1}", dailyFolder, string.Format("{0:dd} {1:MMMM}.txt", now, now));

            File.Delete(dailySummaryFilename);

            foreach (var file in files)
            {
                writeFile(dailySummaryFilename, readFile(file, true));
            }

            string oldestFile = readFile(files.First(), false);
            string newestFile = readFile(files.Last(), false);

            DateTime startTime = DateTime.Parse(oldestFile, CultureInfo.InvariantCulture);
            DateTime endTime = DateTime.Parse(newestFile, CultureInfo.InvariantCulture);

            TimeSpan timeDiff = endTime - startTime;

            StringBuilder endString = new StringBuilder();

            endString.AppendLine("\r\n---------------------------------\r\n");
            endString.AppendLine(string.Format("{0} - {1} hours, {2} minutes", timeDiff, timeDiff.Hours, timeDiff.Minutes));

            writeFile(dailySummaryFilename, endString.ToString());
        }
    }
}
