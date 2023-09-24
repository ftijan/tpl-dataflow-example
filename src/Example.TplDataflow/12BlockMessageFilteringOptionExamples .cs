using System.Threading.Tasks.Dataflow;

namespace Example.TplDataflow
{
	internal static class BlockMessageFilteringOptionExamples 
	{	
		internal static async Task BlockMessageFilteringOptionExample()
		{
			// Buffer starts rejecting messages, but SendAsync() prevents message loss
			var bufferBlock = new BufferBlock<int>(new ExecutionDataflowBlockOptions { BoundedCapacity = 1 });

			var consumer1 = new ActionBlock<int>(a =>
			{
				Console.WriteLine($"Message {a} was processed by Consumer 1");
				Task.Delay(300).Wait();
			});
			

			var consumer2 = new ActionBlock<int>(a =>
			{
				Console.WriteLine($"Message {a} was processed by Consumer 2");
				Task.Delay(300).Wait();
			});

			// Message filter predicate:
			bufferBlock.LinkTo(consumer1, a => a % 2 == 0);
			bufferBlock.LinkTo(consumer2, new DataflowLinkOptions { 
				Append = false,
				MaxMessages = 5
			});
			// Discard block:
			//bufferBlock.LinkTo(DataflowBlock.NullTarget<int>());
			// Log discard block:
			bufferBlock.LinkTo(new ActionBlock<int>(a => Console.WriteLine($"Message {a} was discarded")));

			for (int i = 0; i < 10; i++)
			{
				await bufferBlock.SendAsync(i)
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

			Console.ReadKey();
			Console.WriteLine("Finished");
		}
	}
}
