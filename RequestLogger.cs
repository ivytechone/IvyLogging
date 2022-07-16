using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Collections.Specialized;

namespace IvyTech.Logging
{
	public class RequestLogger 
	{
		private readonly RequestDelegate _requestDelegate;
		private static Serilog.ILogger? _logger;

		public RequestLogger(RequestDelegate requestDelegate, ConfigurationManager configuration)
		{
			if (_logger is null)
			{
				var requestLoggerConfig = configuration.GetSection("IvyLogging").Get<IvyLoggingConfig>();

				if (requestLoggerConfig != null)
				{
					_logger = GetLogger(requestLoggerConfig);
				}
			}

			_requestDelegate = requestDelegate;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			var requestLoggerContext = new RequestLoggerContext();
			context.Items[Constants.RequestLoggerContextKey] = requestLoggerContext;

			try
			{
				await _requestDelegate(context);
			}
			catch (Exception)
			{
				requestLoggerContext.Diag = DiagCodes.UnhandledException;
				context.Response.StatusCode = 500;
				throw;
			}
			finally
			{
				var messageTemplate = "{method} {path} {responseCode}";

				if (context.Response.StatusCode < 400)
				{
					_logger?.Information(messageTemplate,
						context.Request.Method,
						context.Request.Path.Value,
						context.Response.StatusCode);
				}
				else if (context.Response.StatusCode < 500)
				{
					_logger?.Warning(messageTemplate,
						context.Request.Method,
						context.Request.Path.Value,
						context.Response.StatusCode);
				}
				else
				{
					_logger?.Error(messageTemplate,
						context.Request.Method,
						context.Request.Path.Value,
						context.Response.StatusCode);
				}
			}
		}

		public static Serilog.Core.Logger? GetLogger(IvyLoggingConfig config)
		{
			if (config is null)
			{
				return null;
			}

			var ElasticSearchURL = config.ElasticSearchURL ?? throw new ArgumentNullException("IvyLoggingConfig.ElasticSearchURL");
			var ApiKey = config.ApiKey ?? throw new ArgumentNullException("IvyLoggingConfig.ApiKey");
			var Environment = config.Environment ?? throw new ArgumentNullException("IvyLoggingConfig.Environment");

			var elasticSearchOptions = new ElasticsearchSinkOptions(new Uri(ElasticSearchURL));
			elasticSearchOptions.AutoRegisterTemplate = false;
			elasticSearchOptions.TypeName = null;
			elasticSearchOptions.InlineFields = true;
			elasticSearchOptions.BatchAction = ElasticOpType.Create;
			elasticSearchOptions.IndexFormat = $"request-log-{Environment}-index-{{0:yyyy.MM}}";
			elasticSearchOptions.ModifyConnectionSettings = c => c.GlobalHeaders(new NameValueCollection { { "Authorization", $"ApiKey {ApiKey}" } });

			return new LoggerConfiguration()
						.WriteTo.Elasticsearch(elasticSearchOptions)
						.Enrich.With<RequestLoggerContextEnricher>()
						.Enrich.With<RequestTagEnricher>()
						.Enrich.WithProperty("appName", AppContext.AppName ?? throw new ArgumentNullException("AppContext.AppName"))
						.Enrich.WithProperty("version", AppContext.Version ?? throw new ArgumentNullException("AppContext.Version"))
						.CreateLogger();
		}
	}
}
