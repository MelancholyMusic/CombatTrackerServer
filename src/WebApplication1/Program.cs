using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace CombatTrackerServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
			var configuration = new ConfigurationBuilder()
				.AddCommandLine(args)
				.Build();

            var host = new WebHostBuilder()
				.CaptureStartupErrors(true)
				.UseSetting("detailedErrors", "true")
				.UseKestrel()
				.UseUrls("http://0.0.0.0:5000")
				.UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
