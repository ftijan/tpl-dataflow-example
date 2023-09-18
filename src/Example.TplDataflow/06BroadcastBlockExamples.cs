using System.Threading.Tasks.Dataflow;

namespace Example.TplDataflow
{
	internal static class BroadcastBlockExamples
	{
		internal static async Task BroadcastBlockAsyncExample()
		{
			// Removed bounded capacity, because the simple example causes droppped messages
			// (when capacity = 1)
			var broadcastBlock = new BroadcastBlock<int>(a => a);				

			var consumer1 = new ActionBlock<int>(async a => 
			{ 
				Console.WriteLine($"Message {a} was processed by Consumer 1");
				await Task.Delay(300); 
			});			

			var consumer2 = new ActionBlock<int>(async a =>
			{
				Console.WriteLine($"Message {a} was processed by Consumer 2");
				await Task.Delay(300);
			});			

			broadcastBlock.LinkTo(consumer1);
			broadcastBlock.LinkTo(consumer2);

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
	}
}
