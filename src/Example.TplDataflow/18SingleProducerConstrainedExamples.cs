using System.Diagnostics;
using System.Threading.Tasks.Dataflow;

namespace Example.TplDataflow
{
	internal static class SingleProducerConstrainedExamples
	{
		// Note: sequential problems are bad candidates for parallel solutions with TPL
		internal static void SingleProducerConstrainedExample()
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
			}, new ExecutionDataflowBlockOptions { SingleProducerConstrained = true });

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

		// SingleProducerConstrained shouldn't be used with multiple threads: will lose messages, throw exceptions.
		internal static async Task SingleProducerConstrainedMultipleProducersBugExample()
		{
			var stopwatch = new Stopwatch();
			const int Iterations = 6 * 1000 * 1000;

			var autoResetEvent = new AutoResetEvent(false);

			int processedMessageCounter = 0;
			var actionBlock = new ActionBlock<int>(i =>
			{
				if (i == Iterations)
				{
					processedMessageCounter++;
					autoResetEvent.Set();
				}
			}, new ExecutionDataflowBlockOptions { SingleProducerConstrained = true });

			for (int j = 0; j < 10; j++)
			{
				stopwatch.Restart();
				Task[] tasks = new Task[6];
				for (int k = 1; k <= Iterations; k++)
				{
					tasks[k] = new TaskFactory().StartNew(() => 
					{
						for (int i = 0; i <= Iterations / 6; i++)
						{
							actionBlock.Post(i);
						}						
					});
				}

				Task.WaitAll(tasks);
				actionBlock.Complete();
				await actionBlock.Completion;

				autoResetEvent.WaitOne();
				stopwatch.Stop();

				Console.WriteLine("Messages / sec: {0:N0}", Iterations / stopwatch.Elapsed.TotalSeconds);
			}

		}
	}
}
