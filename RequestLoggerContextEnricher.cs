using Serilog.Core;
using Serilog.Events;

namespace IvyTech.Logging
{
	public class RequestLoggerContextEnricher : ILogEventEnricher
	{
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestLoggerContextEnricher() : this(new HttpContextAccessor())
        {
        }

		public RequestLoggerContextEnricher(IHttpContextAccessor httpContextAccessor)
		{
           _httpContextAccessor = httpContextAccessor;
		}

		public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
		{
            var requestLoggerContext = _httpContextAccessor.HttpContext?.GetRequestLoggerContext();

            if (requestLoggerContext is not null)
            {
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("requestId", requestLoggerContext.RequestId));
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("diag", requestLoggerContext.Diag));
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("identity", requestLoggerContext.Identity));
            }
		}
	}
}
