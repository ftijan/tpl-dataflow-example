using System.Threading.Tasks.Dataflow;

namespace Example.TplDataflow
{
	internal static class TransformManyBlockExamples
	{
		internal static void TransformManyBlockExample()
		{ 
			var transformManyBlock = new TransformManyBlock<int, string>(a => FindEvenNumbers(a),
				new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 5 }); // Note: will preserve order
			var printBlock = new ActionBlock<string>(a => Console.WriteLine($"Received message: {a}"));

			transformManyBlock.LinkTo(printBlock);

            for (int i = 0; i < 10; i++)
            {
				transformManyBlock.Post(i);
            }

			transformManyBlock.Complete();
			transformManyBlock.Completion.Wait();

			printBlock.Complete();
			printBlock.Completion.Wait();

			Console.WriteLine("Finished");
			//Console.ReadKey();
        }

		internal static async Task TransformManyBlockAsyncExample()
		{
			var transformManyBlock = new TransformManyBlock<int, string>(a => FindEvenNumbers(a), 
				new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 5 }); // Note: will preserve order
			var printBlock = new ActionBlock<string>(a => Console.WriteLine($"Received message: {a}"));

			transformManyBlock.LinkTo(printBlock);

			for (int i = 0; i < 10; i++)
			{
				await transformManyBlock.SendAsync(i);
			}

			transformManyBlock.Complete();
			await transformManyBlock.Completion;

			printBlock.Complete();
			await printBlock.Completion;

			Console.WriteLine("Finished");
		}

		private static IEnumerable<string> FindEvenNumbers(int number)
		{
			for (int i = 0; i < number; i++)
			{
				if (i % 2 == 0)
				{					
					yield return $"{number}:{i}";
				}
			}
		}
	}
}
