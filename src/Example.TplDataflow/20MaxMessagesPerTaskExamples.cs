using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;

namespace Example.TplDataflow
{
	internal static class MaxMessagesPerTaskExamples
	{
		private static Stopwatch _stopwatch = new();
		private static ConcurrentDictionary<int, ConcurrentBag<Tuple<long, string>>> _timestampledList = new();

		internal static void MaxMessagesPerTaskExample()
		{
			var inputBlock = new BroadcastBlock<int>(a => a);
			var consumerBlocks = new List<ActionBlock<int>>();

			const int consumerCount = 10;
			for (int i = 0; i < consumerCount; i++) 
			{
				var actionBlock = CreateConsumingBlock(i);
				inputBlock.LinkToWithPropagation(actionBlock);
				consumerBlocks.Add(actionBlock);
			}

			_stopwatch.Start();
			for (int i = 0;i < 100; i++) 
			{
				inputBlock.Post(i);
			}

			inputBlock.Complete();
			Task.WaitAll(consumerBlocks.Select(a => a.Completion).ToArray());
			_stopwatch.Stop();


            Console.WriteLine($"Elapsed ticks: {_stopwatch.ElapsedTicks}");

        }
		
		private static ActionBlock<int> CreateConsumingBlock(int id)
		{
			var actionBLock = new ActionBlock<int>(a =>
			{
				var blockLog = Tuple.Create(_stopwatch.ElapsedTicks, id.ToString());
				var bag = _timestampledList.GetOrAdd(
					Thread.CurrentThread.ManagedThreadId,
					new ConcurrentBag<Tuple<long, string>>());
				bag.Add(blockLog);
			
				
			//});
			// Better latency but slower work (for this use case) with:
			}, new ExecutionDataflowBlockOptions { MaxMessagesPerTask = 5 });
			// -> with this change, .Net runtime will switch between threads at periodic intervals,
			// sacrificing overall speed for per-thread latency improvement
			/*
			 * BEFORE:
			 * Thread 1 work....end#Thread 3 work....end
			 * Thread 2 work....end#Thread 4 work....end
			 * other threads' work here...
			 * ............
			 * AFTER:
			 * Thread 1 work#Thread 3 work#Thread 1 work#Thread 3 work#....
			 * Thread 2 work#Thread 4 work#Thread 2 work#Thread 4 work#....
			 * other threads' work here...
			 */

			return actionBLock;
			
		}
	}
}
