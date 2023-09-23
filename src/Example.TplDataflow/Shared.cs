using System.Threading.Tasks.Dataflow;

namespace Example.TplDataflow
{
	internal static class Shared
	{
		internal static void LogMessage(int number, string direction = "input") => Console.WriteLine($"There are {number} message(s) in the {direction} queue.");

		internal static IDisposable LinkToWithPropagation<T>(this ISourceBlock<T> source, ITargetBlock<T> target)
		{ 
			return source.LinkTo(target, new DataflowLinkOptions 
			{ 
				PropagateCompletion = true,
			});
		}
	}
}
