using System.Collections.Specialized;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace IvyTech.Logging
{
	public static class DebugLogger 
	{
		private static Serilog.ILogger? _logger;

		public static Serilog.ILogger? Logger
		{
			get => _logger;
		}

		public static Serilog.ILogger? CreateLogger(ConfigurationManager appConfig)
		{
			if (_logger == null)
			{
				var config = appConfig.GetSection("IvyLogging").Get<IvyLoggingConfig>();

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
				elasticSearchOptions.IndexFormat = $"debug-log-{Environment}-index-{{0:yyyy.MM}}";
				elasticSearchOptions.ModifyConnectionSettings = c => c.GlobalHeaders(new NameValueCollection { { "Authorization", $"ApiKey {ApiKey}" } });

				_logger = new LoggerConfiguration()
							.WriteTo.Elasticsearch(elasticSearchOptions)
							.MinimumLevel.Information()
							.MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
							.Enrich.WithProperty("appName", AppContext.AppName ?? throw new ArgumentNullException("AppContext.AppName"))
							.Enrich.WithProperty("version", AppContext.Version ?? throw new ArgumentNullException("AppContext.Version"))
							.Enrich.With<RequestLoggerContextEnricher>()
							.Enrich.With<RequestTagEnricher>()
							.CreateLogger();
			}

			return _logger;
		}
	}
}
