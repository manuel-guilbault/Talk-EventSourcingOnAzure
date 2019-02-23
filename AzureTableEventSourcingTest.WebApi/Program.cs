﻿using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace AzureTableEventSourcingTest.WebApi
{
    public class Program
	{
		public static async Task Main(string[] args)
		{
            var host = CreateWebHostBuilder(args).Build();
            await host.BeforeApplicationStart();
            host.Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>();
	}
}