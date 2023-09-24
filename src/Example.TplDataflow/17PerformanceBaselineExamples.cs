using System.Diagnostics;
using System.Threading.Tasks.Dataflow;

namespace Example.TplDataflow
{
	internal static class PerformanceBaselineExamples
	{
		// Note: sequential problems are bad candidates for parallel solutions with TPL
		internal static void PerformanceBaselineExample()
		{
			var stopwatch = new Stopwatch();
			const int Iterations = 6 * 1000 * 1000;

			var autoResetEvent = new AutoResetEvent(false);

			var actionBlock = new ActionBlock<int>(i =>
			{
				if (i == Iterations) 
				{ 
					autoResetEvent.Set();
				}
			});

			for (int j = 0; j < 10; j++) 
			{ 
				stopwatch.Restart();
				for (int i = 1; i <= Iterations; i++) 
				{ 
					actionBlock.Post(i);
				}
				autoResetEvent.WaitOne();
				stopwatch.Stop();

                Console.WriteLine("Messages / sec: {0:N0}", Iterations / stopwatch.Elapsed.TotalSeconds);
            }

		}

		internal static void PerformanceBaselineMultithreadsExample()
		{
			var stopwatch = new Stopwatch();
			const int Iterations = 6 * 1000 * 1000;

			var autoResetEvent = new AutoResetEvent(false);

			var actionBlock = new ActionBlock<int>(i =>
			{
				if (i == Iterations)
				{
					autoResetEvent.Set();
				}
			}, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 5 });
			// Makes it even slower
			//}, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = ExecutionDataflowBlockOptions.Unbounded });
	

			for (int j = 0; j < 10; j++)
			{
				stopwatch.Restart();
				for (int i = 1; i <= Iterations; i++)
				{
					actionBlock.Post(i);
				}
				autoResetEvent.WaitOne();
				stopwatch.Stop();

				Console.WriteLine("Messages / sec: {0:N0}", Iterations / stopwatch.Elapsed.TotalSeconds);
			}

		}

		internal static void PerformanceBaselineNonTplExample()
		{
			var stopwatch = new Stopwatch();
			const int Iterations = 6 * 1000 * 1000;

			var autoResetEvent = new AutoResetEvent(false);

			for (int j = 0; j < 10; j++)
			{
				stopwatch.Restart();
				new TaskFactory().StartNew(() => 
				{
					for (int i = 1; i <= Iterations; i++)
					{
						if (i == Iterations)
						{ 
							autoResetEvent.Set();
						}						
					}
				});				
				autoResetEvent.WaitOne();
				stopwatch.Stop();

				Console.WriteLine("Messages / sec: {0:N0}", Iterations / stopwatch.Elapsed.TotalSeconds);
			}

		}
	}
}
