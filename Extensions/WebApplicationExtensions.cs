namespace IvyTech.Logging
{
	public static class WebApplicationExtensions
	{
		public static WebApplication UseIvyLogging(this WebApplication app)
		{
			app.UseMiddleware<RequestLogger>(app.Configuration);
			app.UseMiddleware<ExceptionLogger>();
			return app;
		}
	}
}
