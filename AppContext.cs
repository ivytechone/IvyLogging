namespace IvyTech.Logging
{
	public static class AppContext
	{
		public static string? _version;
		public static string? _appName;

		public static void SetVersion(string version)
		{
			_version = version;
		}

		public static void SetAppName(string appName)
		{
			_appName = appName;
		}

		public static string? Version => _version;
		public static string? AppName => _appName;
	}
}