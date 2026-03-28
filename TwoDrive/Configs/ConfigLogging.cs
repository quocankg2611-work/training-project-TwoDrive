using Serilog;

namespace TwoDrive.Api.Configs;

public static class ConfigLogging
{
	public static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder)
	{
		try
		{
			// Logging
			builder.Host.UseSerilog((context, services, loggerConfiguration) =>
			{
				loggerConfiguration
					.ReadFrom.Configuration(context.Configuration)
					.ReadFrom.Services(services)
					.Enrich.FromLogContext();
			});
			return builder;
		}
		catch (Exception ex)
		{
			Log.Fatal(ex, "Application failed to start");
			throw;
		}
		finally
		{
			Log.CloseAndFlush();
		}
	}
}
