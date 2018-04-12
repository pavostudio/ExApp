using OpenHardwareMonitor;
using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ResourceMonitor
{
    class ResMonitor
    {
        private Computer computer;
        private UpdateVisitor updateVisitor = new UpdateVisitor();
        private Timer timer;

        public void Start()
        {
            timer = new Timer(2000);
            timer.Elapsed += Timer_Elapsed;

            computer = new Computer();
            //computer.HardwareAdded += new HardwareEventHandler(HardwareAdded);
            //computer.HardwareRemoved += new HardwareEventHandler(HardwareRemoved);
            computer.CPUEnabled = true;
            computer.GPUEnabled = true;
            computer.HDDEnabled = true;
            computer.RAMEnabled = true;
            computer.NICEnabled = true;

            computer.Open();
            timer.Enabled = true;
            Timer_Elapsed(this, null);
        }

        public void SetInterval(int interval) {
            timer.Interval = interval * 1000;
        }

        public void Stop() {
            if (timer != null)
                timer.Enabled = false;
            if (computer != null)
                computer.Close();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            computer.Accept(updateVisitor);
            ExApiClient.SyncToLive2DViewerEX(computer);
        }

    }
}
