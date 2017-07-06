using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace TCPConnection
{
    public class Pinger
    {
        Ping pinger = new Ping();
        System.Timers.Timer timer = new System.Timers.Timer();
        IPAddress ipAddress;
        int failedMessagesCount = 0;
        public Pinger(IPAddress ipAddress, int timeBetweenMessages)
        {
            timer.Interval = timeBetweenMessages;
            this.ipAddress = ipAddress;
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                var result = pinger.Send(ipAddress);
                if (result.Status == IPStatus.Success) failedMessagesCount = 0;
                else failedMessagesCount++;
            }
            catch (PingException)
            {
                failedMessagesCount++;
                return;
            }
            finally
            {
                if (failedMessagesCount >= 3)
                    OnEndPointDisconnected(this, EventArgs.Empty);
            }
        }

        public void StartPinging()
        {
            timer.Start();
        }

        public void StopPinging()
        {
            timer.Stop();
        }

        public event EventHandler EndPointDisonnected;
        protected void OnEndPointDisconnected(object sender, EventArgs args)
        {
            EndPointDisonnected?.Invoke(sender, args);
        }
    }
}
