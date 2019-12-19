using System;
using System.Configuration;
using System.ServiceProcess;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace DemoAppInsights
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var key =
                ConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY"];
            TelemetryConfiguration.Active.InstrumentationKey = key;
            TelemetryClient aiClient = new TelemetryClient();
            TelemetryConfiguration config = TelemetryConfiguration.Active;

            try
            {
                aiClient.TrackTrace("Our service is srunning just fine!");
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new DemoService()
                };
                ServiceBase.Run(ServicesToRun);
            }
            catch (Exception exception)
            {
                aiClient.TrackException(exception);
                throw;
            }

        }
    }
}
