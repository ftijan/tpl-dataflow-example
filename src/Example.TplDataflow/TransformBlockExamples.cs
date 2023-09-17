using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Example.TplDataflow
{
	internal static class TransformBlockExamples
	{
		internal static void TransformBlockExample()
		{
			// Run with max 2 threads. Default is 1.
			var transformBlock = new TransformBlock<int, string>(n =>
			{
				Task.Delay(500).Wait();
				return n.ToString();
			}, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 2 });

			for (int i = 0; i < 10; i++) 
			{
				transformBlock.Post(i);
				Shared.LogMessage(transformBlock.InputCount);				
            }

			for (int i = 0; i < 10; i++)
			{
				var result = transformBlock.Receive();
                Console.WriteLine($"Received: {result}");
                Shared.LogMessage(transformBlock.OutputCount, "output");
			}

            Console.WriteLine("Finished");			
        }

		internal static async Task TransformBlockAsyncExample()
		{
			// Run with max 2 threads. Default is 1.
			var transformBlock = new TransformBlock<int, string>(async n =>
			{
				await Task.Delay(500);
				return n.ToString();
			}, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 2 });

			for (int i = 0; i < 10; i++)
			{
				await transformBlock.SendAsync(i);
				Shared.LogMessage(transformBlock.InputCount);
			}

			for (int i = 0; i < 10; i++)
			{
				var result = await transformBlock.ReceiveAsync();
				Console.WriteLine($"Received: {result}");
				Shared.LogMessage(transformBlock.OutputCount, "output");
			}

			transformBlock.Complete();
			await transformBlock.Completion;
			Console.WriteLine("Finished");
		}
	}
}
