using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using System.Runtime.CompilerServices;
using TwoDrive.Api.Common;

namespace TwoDrive.Api.Configs;

public static class ConfigAuthentication
{
	public static WebApplicationBuilder ConfigureAuthentication(this WebApplicationBuilder builder)
	{
		builder.Services
			.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddMicrosoftIdentityWebApi(builder.Configuration.GetSection(ApiConstants.SettingsSectionsKeys.AZURE_AD));
		return builder;
	}
}
