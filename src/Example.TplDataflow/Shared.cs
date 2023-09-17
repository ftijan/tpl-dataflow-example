namespace Example.TplDataflow
{
	internal static class Shared
	{
		internal static void LogMessage(int number, string direction = "input") => Console.WriteLine($"There are {number} message(s) in the {direction} queue.");
	}
}
