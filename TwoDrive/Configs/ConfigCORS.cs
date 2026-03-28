namespace TwoDrive.Api.Configs;

public static class ConfigCORS
{
	public static WebApplicationBuilder ConfigureCORS(this WebApplicationBuilder builder)
	{
		if (builder.Environment.IsDevelopment())
		{
			builder.Services.AddCors(opt =>
			{
				opt.AddDefaultPolicy(policy =>
				{
					policy
						.AllowAnyOrigin()
						.AllowAnyMethod()
						.AllowAnyHeader();
				});
			});
		}
		return builder;
	}
}
