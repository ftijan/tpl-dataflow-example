using System.Threading.Tasks.Dataflow;

namespace Example.TplDataflow
{
	internal static class WriteOnceBlockExamples
	{
		internal static async Task WriteOnceBlockExample()
		{
			var block = new WriteOnceBlock<int>(a => a);

			for (int i = 0; i < 10; i++) 
			{
				if (block.Post(i))
				{
					// Will only accept the first message - 0
                    Console.WriteLine($"Message {i} was accepted.");
                }
				else
				{
					Console.WriteLine($"Message {i} was rejected.");
				}
			}

			for (int i = 0; i < 15; i++)
			{
				if (block.TryReceive(out var output))
				{
					// Will always show the first message - 0
					Console.WriteLine($"Message {output} was received, iteration {i}.");
				}
				else
				{
					Console.WriteLine($"No messages left.");
				}
			}

			block.Complete();
			await block.Completion;

			Console.WriteLine("Finished");
		}

		internal static async Task WriteOnceBlockPipelineExample()
		{
			var block = new WriteOnceBlock<int>(a => a);
			var printBlock = new ActionBlock<int>(a => Console.WriteLine($"Message {a} was received."));
			block.LinkTo(printBlock);

			for (int i = 0; i < 10; i++)
			{
				if (block.Post(i))
				{
					// Will only accept the first message - 0
					Console.WriteLine($"Message {i} was accepted.");
				}
				else
				{
					Console.WriteLine($"Message {i} was rejected.");
				}
			}

			for (int i = 0; i < 15; i++)
			{
				if (block.TryReceive(out var output))
				{
					// Will always show the first message - 0
					Console.WriteLine($"Message {output} was received, iteration {i}.");
				}
				else
				{
					Console.WriteLine($"No messages left.");
				}
			}

			block.Complete();
			await block.Completion;

			printBlock.Complete();
			await printBlock.Completion;

			Console.WriteLine("Finished");
		}
	}
}
