using System;

namespace ShowAvailableItems
{
	public static class Logger
	{
        private static string LogTag = "[ShowAvailableItems] ";

		public static void Log(string message) => Console.WriteLine(LogTag+message);
	}
}
