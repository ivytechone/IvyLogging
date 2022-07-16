using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;

namespace IvyTech.Logging
{
	public class RequestTagEnricher : ILogEventEnricher
	{
        private readonly IHttpContextAccessor _httpContextAccessor;
        private HashSet<string> _supportedTags;        

        public RequestTagEnricher() : this(new HttpContextAccessor())
        {
        }

		public RequestTagEnricher(IHttpContextAccessor httpContextAccessor)
		{
            _supportedTags = new HashSet<string>();
            _httpContextAccessor = httpContextAccessor;
            foreach (var tag in Constants.SupportedTags)
            {
                _supportedTags.Add(tag);
            }
		}

		public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
		{
            var requestTag = _httpContextAccessor.HttpContext?.GetRequestTag();

            if (requestTag is not null && _supportedTags.Contains(requestTag)) 
            {
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("requestTag", requestTag));
            }
		}
	}
}