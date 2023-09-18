using System.Threading.Tasks.Dataflow;

namespace Example.TplDataflow
{
	internal static class BatchedJoinBlockExamples
	{
		internal static async Task BatchedJoinBlockAsyncExample()
		{			
			var broadcastBlock = new BroadcastBlock<int>(a => a);

			var consumer1 = new TransformBlock<int, int>(async a =>
			{
				Console.WriteLine($"Message {a} was processed by Consumer 1");
				if (a % 2 == 0)
				{
					await Task.Delay(300);
				}
				else
				{
					await Task.Delay(50);
				}				
				return -1 * a;
			}, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 3 });

			var consumer2 = new TransformBlock<int, int>(async a =>
			{
				Console.WriteLine($"Message {a} was processed by Consumer 2");
				if (a % 2 == 0)
				{
					await Task.Delay(50);
				}
				else
				{
					await Task.Delay(300);
				}
				return a;
			}, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 3 });

			broadcastBlock.LinkTo(consumer1);
			broadcastBlock.LinkTo(consumer2);

			var batchedJoinBlock = new BatchedJoinBlock<int, int>(3); // batch size param
			consumer1.LinkTo(batchedJoinBlock.Target1);
			consumer2.LinkTo(batchedJoinBlock.Target2);

			var printBlock = new ActionBlock<Tuple<IList<int>, IList<int>>>(a => Console.WriteLine($"Message [{string.Join(",", a.Item1)}] [{string.Join(",", a.Item2)}] was processed."));
			batchedJoinBlock.LinkTo(printBlock);

			for (int i = 0; i < 10; i++)
			{
				await broadcastBlock.SendAsync(i)
					.ContinueWith(a =>
					{
						if (a.Result)
						{
							Console.WriteLine($"Message {i} was accepted");
						}
						else
						{
							Console.WriteLine($"Message {i} was rejected");
						}
					});
			}

			broadcastBlock.Complete();
			await broadcastBlock.Completion;

			consumer1.Complete();
			consumer2.Complete();
			await consumer1.Completion;
			await consumer2.Completion;

			batchedJoinBlock.Complete();
			await batchedJoinBlock.Completion;

			Console.WriteLine("Finished");
		}
	}
}
