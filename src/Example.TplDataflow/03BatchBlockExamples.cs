using System.Threading.Tasks.Dataflow;

namespace Example.TplDataflow
{
	internal static class BatchBlockExamples
	{
		internal static void BatchBlockBadExample()
		{
			var batchBlock = new BatchBlock<int>(3);

			for (int i = 0; i < 10; i++) 
			{ 
				batchBlock.SendAsync(i);
			}

			batchBlock.Complete();
			for (int i = 0; i < 5; i++)
			{
				int[] result = batchBlock.Receive();

                Console.Write($"Received batch {i}: ");
				foreach (var r in result)
				{
                    Console.Write(r + " ");
                }

                Console.Write("\n");
            }

            Console.WriteLine("Finished!");

        }

		internal static void BatchBlockFixedExample()
		{
			var batchBlock = new BatchBlock<int>(3);

			for (int i = 0; i < 10; i++)
			{
				batchBlock.Post(i);
			}

			batchBlock.Complete();
			for (int i = 0; i < 5; i++)
			{
				int[] result;
				if (batchBlock.TryReceive(out result))
				{
					Console.Write($"Received batch {i}: ");
					foreach (var r in result)
					{
						Console.Write(r + " ");
					}

					Console.Write("\n");
				}
				else
				{
                    Console.WriteLine("The block finished");
					break;
                }				
			}

			Console.WriteLine("Finished!");

		}

		internal static async Task BatchBlockFixedAsyncExample()
		{
			var batchBlock = new BatchBlock<int>(3);

			for (int i = 0; i < 10; i++)
			{
				await batchBlock.SendAsync(i);
			}

			batchBlock.Complete();
			for (int i = 0; i < 5; i++)
			{				
				var isAvailable = await batchBlock.OutputAvailableAsync();
				if (isAvailable)
				{
					var result = await batchBlock.ReceiveAsync();
					Console.Write($"Received batch {i}: ");
					foreach (var r in result)
					{
						Console.Write(r + " ");
					}

					Console.Write("\n");
				}
				else
				{
					Console.WriteLine("The block finished");
					break;
				}
			}

			Console.WriteLine("Finished!");

		}
	}
}
