using Microsoft.ApplicationInsights;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceProcess;
using System.Timers;

namespace DemoAppInsights
{
    public partial class DemoService : ServiceBase
    {
        Timer _timer; 
        TelemetryClient _aiClient;

        public DemoService()
        {
            _timer = new Timer();
            _aiClient = new TelemetryClient();

            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _aiClient.TrackTrace("Service started working at " + DateTime.Now);
            WriteToFile("Service is started at " + DateTime.Now);
            _timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            _timer.Interval = 10000; //number in milisecinds  
            _timer.Enabled = true;
        }

        protected override void OnStop()
        {
            WriteToFile("Service stopped working at " + DateTime.Now);
            _aiClient.TrackTrace("Service is stopped at " + DateTime.Now);
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            var loggerDict = new Dictionary<string, string>();

            try
            {
                var deleteResult = Delete().ToString();

                loggerDict.Add("DeleteResult", deleteResult);
                _aiClient.TrackEvent("DeleteEvent", loggerDict);
                _aiClient.TrackTrace("I am still working! :) " + DateTime.Now);
                WriteToFile("I am still working! :) " + DateTime.Now);
                WriteToFile("DeletEvent: " + deleteResult + ";" + DateTime.Now);
            }
            catch (DivideByZeroException ex)
            {
                loggerDict.Add("DevByZeroEx", ex.ToString());
                _aiClient.TrackException(ex, loggerDict);
                WriteToFile("DevByZeroEx: " + ex + ";" + DateTime.Now);
            }
            catch (Exception ex)
            {
                loggerDict.Add("CommonEx", ex.ToString());
                _aiClient.TrackException(ex, loggerDict);
                WriteToFile("CommonEx: " + ex + ";" + DateTime.Now);
            }
        }

        private double Delete()
        {
            var rnd = new Random();
            var x = rnd.Next(1, 10);
            var y = rnd.Next(0, 10);
            int result;
            try
            {
                result = (x / y);
            }
            catch (DivideByZeroException ex)
            {
                throw new DivideByZeroException(ex.Message, ex.InnerException);
            }

            return result;
        }

        public void WriteToFile(string Message)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";

            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}
