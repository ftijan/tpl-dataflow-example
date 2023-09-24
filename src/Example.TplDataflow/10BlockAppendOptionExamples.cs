using System.Threading.Tasks.Dataflow;

namespace Example.TplDataflow
{
	internal static class BlockAppendOptionExamples
	{	
		internal static async Task BlockAppendOptionExample()
		{
			// Buffer starts rejecting messages, but SendAsync() prevents message loss
			var bufferBlock = new BufferBlock<int>(new ExecutionDataflowBlockOptions { BoundedCapacity = 1 });

			var consumer1 = new ActionBlock<int>(a =>
			{
				Console.WriteLine($"Message {a} was processed by Consumer 1");
				Task.Delay(300).Wait();
			});// , new ExecutionDataflowBlockOptions { BoundedCapacity = 1 }); // Allows starvation of consumers
			

			var consumer2 = new ActionBlock<int>(a =>
			{
				Console.WriteLine($"Message {a} was processed by Consumer 2");
				Task.Delay(300).Wait();
			});//, new ExecutionDataflowBlockOptions { BoundedCapacity = 1 });
			   // Changes how many max messages can be processed at one time // Allows starvation of consumers

			bufferBlock.LinkTo(consumer1);
			// Will starve consumer 1, because #2 is now prepended to the consumer list:
			bufferBlock.LinkTo(consumer2, new DataflowLinkOptions { Append = false });

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
