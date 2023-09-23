using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Example.TplDataflow
{
	internal static class ErrorHandlingExamples
	{
		internal static async Task ErrorHandlingExample()
		{
			var block = new ActionBlock<int>(n =>
			{
				// Will empty the queue and make to block go into 'Faulted' completed state
				if (n == 5) throw new Exception("Something went wrong");
				// Note: The best way of handling error states is to not throwing exceptions when using TPL.
				// Use special message types that indicate error states so that subsequent blocks don't process them, etc.

                Console.WriteLine($"Message {n} processed");
            });

			for ( int i = 0; i < 10; i++ ) 
			{
				if (block.Post(i)) 
				{
                    Console.WriteLine($"Message {i} was accepted");
                }
				else 
				{
					Console.WriteLine($"Message {i} was rejected");
				}
			}
			block.Complete();
			await block.Completion; // makes the exception bubble up
            Console.WriteLine($"Input queue size {block.InputCount}");

            Console.WriteLine("Finished");
			Console.ReadKey();
        }

		internal static async Task ErrorHandling2Example()
		{
			var block = new TransformBlock<int,string>(n =>
			{
				// Will empty the queue and make to block go into 'Faulted' completed state
				if (n == 5) throw new Exception("Something went wrong");

				Console.WriteLine($"Message {n} processed");
				return n.ToString();
			});
			var printBlock = new ActionBlock<string>(a => Console.WriteLine($"Message {a} was processed"));
			block.LinkTo(printBlock, new DataflowLinkOptions { PropagateCompletion = true }); // Will also propagate the error

			for (int i = 0; i < 10; i++)
			{
				if (block.Post(i))
				{
					Console.WriteLine($"Message {i} was accepted");
				}
				else
				{
					Console.WriteLine($"Message {i} was rejected");
				}
			}
			block.Complete();
			try
			{
				await printBlock.Completion;
			}
			catch (AggregateException ex)
			{
				throw ex.Flatten().InnerException;
			}
			
			Console.WriteLine($"Input queue size {block.InputCount}");

			Console.WriteLine("Finished");
			Console.ReadKey();
		}
	}
}
