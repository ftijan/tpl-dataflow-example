using System.Collections.Concurrent;
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

		internal static MockTelemetryClient CreateTelemetryClient()
		{
			return new MockTelemetryClient();
		}

		internal class MockEvent
		{ 
			public string EventName { get; set; }
			public Dictionary<string, double>? Metrics { get; set; }
		}

		internal class MockTelemetryClient
		{
			ConcurrentBag<MockEvent> _events = new();

			internal void DisplayEventsStats()
			{
				var count = _events.Count;
				var totalTime = _events
					.Where(x => x.Metrics != null && x.Metrics.ContainsKey("ProcessingTime"))
					.Select(x => x.Metrics["ProcessingTime"])
					.Sum();
                Console.WriteLine($"Events count: {count}, total processing time: {totalTime} milliseconds.");                
			}

			internal void TrackEvent(string eventName, Dictionary<string, double>? metrics = null)
			{
				_events.Add(new MockEvent 
				{ 
					EventName = eventName,
					Metrics = metrics
				});
			}
		}
	}
}
