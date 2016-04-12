using System;
using System.ServiceProcess;
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
            tmrLog.Interval = 30000;
            tmrLog.Enabled = true;
        }

        private void tmrLog_Tick(object sender, EventArgs e)
        {
            Logic.writeOut("Still on");
        }
    }
}
