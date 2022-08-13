using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;

namespace bcrwss
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ///Generate Host Builder and Register the Services for DI
            var builder = new HostBuilder()
               .ConfigureServices((hostContext, services) =>
               {
                   //Register all your services here
                   services.AddScoped<fMain>();

               }).ConfigureLogging(logBuilder =>
               {
                   logBuilder.SetMinimumLevel(LogLevel.Trace);
                   logBuilder.AddLog4Net("log4net.config");

               });

            var host = builder.Build();

            using (var serviceScope  = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                try
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    var frm = services.GetRequiredService<fMain>();
                    Application.Run(frm);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
