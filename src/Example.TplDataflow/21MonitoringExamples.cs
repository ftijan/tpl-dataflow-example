using System.Diagnostics;
using System.Threading.Tasks.Dataflow;

namespace Example.TplDataflow
{
	internal static class MonitoringExamples
	{
		static Random _random = new Random();

		internal static void MonitoringExample()
		{
			CancellationTokenSource cts = new CancellationTokenSource();
			
            var telemetryClient = Shared.CreateTelemetryClient();

			var broadcastBlock = new BroadcastBlock<int>(null);
			var transformPositive = new TransformBlock<int, int>(x => 
			{
				using var dataflowMetric = new DataflowMetric("TransformPositive", telemetryClient);

				Task.Delay(_random.Next(200)).Wait();
				return x;
			});
			
			var transformNegative = new TransformBlock<int, int>(x => 
			{
				using var dataflowMetric = new DataflowMetric("TransformNegative", telemetryClient);

				Task.Delay(_random.Next(300)).Wait();				
				return x * -1;
			});

			var join = new JoinBlock<int, int>();
			var sumBlock = new ActionBlock<Tuple<int, int>>(tuple =>
			{
				using var dataflowMetric = new DataflowMetric("SumBlock", telemetryClient);
				Console.WriteLine("{0}+({1})={2}",
					tuple.Item1,
					tuple.Item2,
					tuple.Item1 + tuple.Item2);				
            });

			broadcastBlock.LinkToWithPropagation(transformPositive);
			broadcastBlock.LinkToWithPropagation(transformNegative);
			transformPositive.LinkToWithPropagation(join.Target1);
			transformNegative.LinkToWithPropagation(join.Target2);

			join.LinkToWithPropagation(sumBlock);

			Task.Run(() =>
			{
				while (!cts.IsCancellationRequested)
				{
					broadcastBlock.Post(_random.Next(100));
					Task.Delay(200).Wait();
				}
			});

            Console.WriteLine("Press any key to stop.");
            Console.ReadKey();
			cts.Cancel();

			telemetryClient.DisplayEventsStats();			
		}

		internal class DataflowMetric : IDisposable
		{
			private readonly string _blockName;
			private readonly Shared.MockTelemetryClient _telemetryClient;
			private Stopwatch _stopwatch = new Stopwatch();

            public DataflowMetric(string blockName, Shared.MockTelemetryClient telemetryClient)
            {
				_blockName = blockName;
				_telemetryClient = telemetryClient;
				_stopwatch.Start();
			}

            public void Dispose()
			{
				_stopwatch.Stop();
				_telemetryClient.TrackEvent(_blockName, new Dictionary<string, double>
				{
					{ "ProcessingTime", _stopwatch.ElapsedMilliseconds }
				});
			}
		}
	}
}
