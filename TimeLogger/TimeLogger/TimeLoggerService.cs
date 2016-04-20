using System.ServiceProcess;
using System.ComponentModel;
using System.Timers;
using System.Diagnostics;

namespace TimeLogger
{
    [RunInstallerAttribute(true)]
    public partial class TimeLoggerService : ServiceBase
    {
        Timer tmrLog;
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
            Logic.writeOut("Shutdown");
            tmrLog.Enabled = false;
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            if (changeDescription.Reason == SessionChangeReason.SessionLock)
            {
                Logic.writeOut("Locked");
                tmrLog.Enabled = false;
            }
            else if (changeDescription.Reason == SessionChangeReason.SessionUnlock)
            {
                Logic.writeOut("Unlocked");
                tmrLog.Enabled = true;
            }
            else if (changeDescription.Reason == SessionChangeReason.SessionLogoff)
            {
                Logic.writeOut("Logged off");
                tmrLog.Enabled = false;
            }
            else if (changeDescription.Reason == SessionChangeReason.SessionLogon)
            {
                Logic.writeOut("Logged on");
                tmrLog.Enabled = true;
            }
        }

        public void Start()
        {
            Logic.writeOut("Startup");
            tmrLog = new Timer(300000D);
            tmrLog.AutoReset = true;
            tmrLog.Elapsed += new ElapsedEventHandler(tmrLog_Elapsed);
            tmrLog.Start();
        }

        private void tmrLog_Elapsed(object sender, ElapsedEventArgs e)
        {
            Process currentProcess = WindowLogger.GetActiveProcess();
            if (currentProcess != null)
            {
                Logic.writeOut(string.Format("Still on - {0} - {1}", currentProcess.ProcessName.ToString(), currentProcess.MainWindowTitle.ToString()));
            }
        }
    }
}
