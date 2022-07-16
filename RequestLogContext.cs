using Serilog.Core;
using Serilog.Events;

namespace IvyTech.Logging
{
	public class RequestLoggerContext
	{
		public RequestLoggerContext()
		{
			RequestId = Guid.NewGuid().ToString();
		}

		public string RequestId { get; }
		public string? Diag { get; set; }
		public string? Identity { get; set; }
	}
}
