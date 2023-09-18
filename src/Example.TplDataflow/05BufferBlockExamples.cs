using System.Threading.Tasks.Dataflow;

namespace Example.TplDataflow
{
	internal static class BufferBlockExamples
	{
		internal static void BufferBlockSimpleExample()
		{ 
			var bufferBlock = new BufferBlock<int>();

			for (int i = 0; i < 10; i++)
			{ 
				bufferBlock.Post(i);
			}

			for (int i = 0; i < 10; i++)
			{
				int result = bufferBlock.Receive();
                Console.WriteLine(result);
            }

			bufferBlock.Complete();
			bufferBlock.Completion.Wait();
			Console.WriteLine("Finished");

		}

		internal static void BufferBlockProducerConsumerExample()
		{
			var bufferBlock = new BufferBlock<int>();

			var consumer1 = new ActionBlock<int>(a =>
			{
                Console.WriteLine($"Message {a} was processed by Consumer 1");
				Task.Delay(300).Wait();
            }, new ExecutionDataflowBlockOptions { BoundedCapacity = 1 });
			// Changes how many max messages can be processed at one time

			var consumer2 = new ActionBlock<int>(a =>
			{
				Console.WriteLine($"Message {a} was processed by Consumer 2");
				Task.Delay(300).Wait();
			}, new ExecutionDataflowBlockOptions { BoundedCapacity = 1 });
			// Changes how many max messages can be processed at one time

			bufferBlock.LinkTo(consumer1);
			bufferBlock.LinkTo(consumer2);

			for (int i = 0; i < 10; i++)
			{
				bufferBlock.Post(i);
			}

			bufferBlock.Complete();
			bufferBlock.Completion.Wait();

			consumer1.Complete();
			consumer2.Complete();
			consumer1.Completion.Wait();
			consumer2.Completion.Wait();

			Console.WriteLine("Finished");			
		}

		internal static void BufferBlockProducerConsumerWithBoundedProducerExample()
		{
			// Buffer starts rejecting messages
			var bufferBlock = new BufferBlock<int>(new ExecutionDataflowBlockOptions { BoundedCapacity = 1 });

			var consumer1 = new ActionBlock<int>(a =>
			{
				Console.WriteLine($"Message {a} was processed by Consumer 1");
				Task.Delay(300).Wait();
			}, new ExecutionDataflowBlockOptions { BoundedCapacity = 1 });
			// Changes how many max messages can be processed at one time

			var consumer2 = new ActionBlock<int>(a =>
			{
				Console.WriteLine($"Message {a} was processed by Consumer 2");
				Task.Delay(300).Wait();
			}, new ExecutionDataflowBlockOptions { BoundedCapacity = 1 });
			// Changes how many max messages can be processed at one time

			bufferBlock.LinkTo(consumer1);
			bufferBlock.LinkTo(consumer2);

			for (int i = 0; i < 10; i++)
			{
				if (bufferBlock.Post(i))
				{
					Console.WriteLine($"Message {i} was accepted");
				}
				else
				{
					Console.WriteLine($"Message {i} was rejected");
				}
			}

			bufferBlock.Complete();
			bufferBlock.Completion.Wait();

			consumer1.Complete();
			consumer2.Complete();
			consumer1.Completion.Wait();
			consumer2.Completion.Wait();

			Console.WriteLine("Finished");
		}

		internal static async Task BufferBlockProducerConsumerWithBoundedProducerFixedExample()
		{
			// Buffer starts rejecting messages, but SendAsync() prevents message loss
			var bufferBlock = new BufferBlock<int>(new ExecutionDataflowBlockOptions { BoundedCapacity = 1 });

			var consumer1 = new ActionBlock<int>(a =>
			{
				Console.WriteLine($"Message {a} was processed by Consumer 1");
				Task.Delay(300).Wait();
			}, new ExecutionDataflowBlockOptions { BoundedCapacity = 1 });
			// Changes how many max messages can be processed at one time

			var consumer2 = new ActionBlock<int>(a =>
			{
				Console.WriteLine($"Message {a} was processed by Consumer 2");
				Task.Delay(300).Wait();
			}, new ExecutionDataflowBlockOptions { BoundedCapacity = 1 });
			// Changes how many max messages can be processed at one time

			bufferBlock.LinkTo(consumer1);
			bufferBlock.LinkTo(consumer2);

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

			bufferBlock.Complete();
			await bufferBlock.Completion;

			consumer1.Complete();
			consumer2.Complete();
			await consumer1.Completion;
			await consumer2.Completion;

			Console.WriteLine("Finished");
		}
	}
}
