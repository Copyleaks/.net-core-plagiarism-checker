using Copyleaks.SDK.Demo.Helpers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace SampleWebApplication
{
	public class Program
    {
        public static void Main(string[] args)
        {
			ResultFile.CreateResultDirectory();
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
