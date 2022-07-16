using Microsoft.Extensions.Primitives;

namespace IvyTech.Logging
{
	public static class HttpContextExtensions
	{
		public static RequestLoggerContext GetRequestLoggerContext(this HttpContext httpContext)
		{
			if (httpContext.Items.TryGetValue(Constants.RequestLoggerContextKey, out object? requestLoggerContext) && requestLoggerContext is RequestLoggerContext)
			{
				return (RequestLoggerContext)requestLoggerContext;
			}
			throw new MissingRequestLoggerContextException();
		}

		public static string? GetRequestTag(this HttpContext httpContext)
		{
			httpContext.Request.Headers.TryGetValue(Constants.RequestTagHeaderName, out StringValues value);

			return value.FirstOrDefault();
		}
	}
}
