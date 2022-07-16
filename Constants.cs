namespace IvyTech.Logging
{
	public class Constants
	{
		public static string RequestLoggerContextKey => "ivyTechRequestLogContext";
		public static string RequestTagHeaderName => "x-ivytech-request-tag";
		public static string[] SupportedTags 
		{
			get
			{
				string[] tags = {"test", "monitor"};
				return tags;
			}
		}
	}
}
