namespace IvyTech.Logging
{
	public class ExceptionLogger 
	{
		private readonly RequestDelegate _requestDelegate;

		public ExceptionLogger(RequestDelegate requestDelegate)
		{
			_requestDelegate = requestDelegate;
		}

		public async Task InvokeAsync(HttpContext context)
		{
				try
				{
					await _requestDelegate(context);
				}
				catch (Exception ex)
				{
					
			
					DebugLogger.Logger?.Error(ex, "Unhandled Exception");

					throw;
				}
		}
	}
}
