using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
//using System.IO;


namespace BuildMonitor
{
    public class Program
    {
        const string jenkinsUrl = "http://83.241.200.213/latestsmecbuild/";
        const string VMurl = "http://www.velvetmetrics.com/log?path=filip.netduino.test&power=";
        const int pollInterval = 60;

        public static void Main()
        {
            while (true)
            {
                Debug.Print("IT'S ALIVE!");
                OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);
                //string myIp = Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0].IPAddress;
                //Debug.Print("My IP: " + myIp);
                //Debug.GC(true);

                HttpWebRequest webRequest;
                HttpWebResponse webResponse;
                String response = "";

                webRequest = (HttpWebRequest)WebRequest.Create(jenkinsUrl);
                webRequest.Method = "GET";
                Blink(led, 1, 300);
                //Thread.Sleep(2000);
                Debug.Print("Requesting " + webRequest.Address.AbsoluteUri);

                try
                {
                    webResponse = (HttpWebResponse)webRequest.GetResponse();
                    using (var s = webResponse.GetResponseStream())
                    {
                        var buffer = new byte[s.Length];
                        s.Read(buffer, 0, (int)s.Length);
                        response = new String(Encoding.UTF8.GetChars(buffer));
                    }
                }
                catch { }


                int power = -1;
                if (response != null && response.Length == 1)
                {
                    power = Int32.Parse(response);
                }

                Blink(led, 2, 300);
                //Thread.Sleep(2000);

                webRequest = (HttpWebRequest)WebRequest.Create(VMurl + power);
                webRequest.Method = "GET";

                Debug.Print("Requesting " + webRequest.Address.AbsoluteUri);
                try
                {
                    var webResponse2 = (HttpWebResponse)webRequest.GetResponse();
                }
                catch { }

                Blink(led, 3, 300);
                //Thread.Sleep(2000);

                response = "";
            }
        }

        private static void Blink(OutputPort led, int numberOfTimes, int delayMilliseconds)
        {
            if (delayMilliseconds * numberOfTimes > 10000)
            {
                Debug.Print("Too long blink loop! Aborting blink");
                return;
            }

            Debug.Print("Blinking LED " + numberOfTimes + " times with delay " + delayMilliseconds + "ms.");
            var initialState = led.Read();
            for (int i = 0; i < numberOfTimes; i++)
            {
                led.Write(false);
                Thread.Sleep(delayMilliseconds);
                led.Write(true);
                Thread.Sleep(delayMilliseconds);
            }
            led.Write(initialState);
        }   
    }
}
