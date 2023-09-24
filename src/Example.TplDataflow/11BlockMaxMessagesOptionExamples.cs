using System.Threading.Tasks.Dataflow;

namespace Example.TplDataflow
{
	internal static class BlockMaxMessagesOptionExamples
	{	
		internal static async Task BlockMaxMessagesOptionExample()
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

			bufferBlock.LinkTo(consumer1);
			// Note: accepted, not processed max messages
			bufferBlock.LinkTo(consumer2, new DataflowLinkOptions { 
				Append = false,
				MaxMessages = 5 // With max 10 messages and prepended consumer, messages will be evenly split now
			});

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
