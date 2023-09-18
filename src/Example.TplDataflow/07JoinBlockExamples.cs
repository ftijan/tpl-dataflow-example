using System.Security.Cryptography;
using System.Threading.Tasks.Dataflow;

namespace Example.TplDataflow
{
	internal static class JoinBlockExamples
	{
		internal static async Task JoinBlockAsyncExample()
		{
			// Removed bounded capacity, because the simple example causes droppped messages
			// (when capacity = 1)
			var broadcastBlock = new BroadcastBlock<int>(a => a);				

			var consumer1 = new TransformBlock<int, int>(async a => 
			{ 
				Console.WriteLine($"Message {a} was processed by Consumer 1");
				await Task.Delay(300);
				return a;
			});			

			var consumer2 = new TransformBlock<int, int>(async a =>
			{
				Console.WriteLine($"Message {a} was processed by Consumer 2");
				await Task.Delay(300);
				return a;
			});

			broadcastBlock.LinkTo(consumer1);
			broadcastBlock.LinkTo(consumer2);

			var joinBlock = new JoinBlock<int, int>();
			consumer1.LinkTo(joinBlock.Target1);
			consumer2.LinkTo(joinBlock.Target2);

			var printBlock = new ActionBlock<Tuple<int,int>>(a => Console.WriteLine($"Message {a} was processed."));
			joinBlock.LinkTo(printBlock);

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

			Console.WriteLine("Finished");			
		}

		internal static async Task JoinBlockComplexAsyncExample()
		{
			// Removed bounded capacity, because the simple example causes droppped messages
			// (when capacity = 1)
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

			var joinBlock = new JoinBlock<int, int>();
			consumer1.LinkTo(joinBlock.Target1);
			consumer2.LinkTo(joinBlock.Target2);

			var printBlock = new ActionBlock<Tuple<int, int>>(a => Console.WriteLine($"Message {a} was processed. Sum was {a.Item1 + a.Item2}"));
			joinBlock.LinkTo(printBlock);

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

			//printBlock.Complete();
			//await printBlock.Completion;

			Console.WriteLine("Finished");
		}
	}
}
