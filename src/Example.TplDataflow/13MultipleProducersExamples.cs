using System.Threading.Tasks.Dataflow;

namespace Example.TplDataflow
{
	internal class MultipleProducersExamples
	{
		internal static void MultipleProducersExample() 
		{
			var producer1 = new TransformBlock<string, string>(async n =>
			{
				await Task.Delay(150);
				return n;
			});

			var producer2 = new TransformBlock<string, string>(async n =>
			{
				await Task.Delay(500);
				return n;
			});

			var printBlock = new ActionBlock<string>(n => Console.WriteLine(n));

			producer1.LinkTo(printBlock);
			producer2.LinkTo(printBlock);

			for (int i = 0; i < 10; i++) 
			{
				producer1.Post($"Producer 1 Message: {i}");
				producer2.Post($"Producer 2 Message: {i}");
			}

            Console.WriteLine("Finished");
			Console.ReadKey();

        }

		internal static async Task MultipleProducersWithPropagationBadExample()
		{
			var producer1 = new TransformBlock<string, string>(async n =>
			{
				await Task.Delay(150);
				return n;
			});

			var producer2 = new TransformBlock<string, string>(async n =>
			{
				await Task.Delay(500);
				return n;
			});

			var printBlock = new ActionBlock<string>(n => Console.WriteLine(n));

			// Bug: the first completion will complete the subsequent block while the other one is still running
			producer1.LinkToWithPropagation(printBlock);
			producer2.LinkToWithPropagation(printBlock);

			for (int i = 0; i < 10; i++)
			{
				producer1.Post($"Producer 1 Message: {i}");
				producer2.Post($"Producer 2 Message: {i}");
			}

			producer1.Complete();
			producer2.Complete();

			await printBlock.Completion;

			Console.WriteLine("Finished");
			Console.ReadKey();
		}

		internal static async Task MultipleProducersWithPropagationFixedExample()
		{
			var producer1 = new TransformBlock<string, string>(async n =>
			{
				await Task.Delay(150);
				return n;
			});

			var producer2 = new TransformBlock<string, string>(async n =>
			{
				await Task.Delay(500);
				return n;
			});

			var printBlock = new ActionBlock<string>(n => Console.WriteLine(n));

			producer1.LinkTo(printBlock);
			producer2.LinkTo(printBlock);

			for (int i = 0; i < 10; i++)
			{
				await producer1.SendAsync($"Producer 1 Message: {i}");
				await producer2.SendAsync($"Producer 2 Message: {i}");
			}

			producer1.Complete();
			producer2.Complete();

			await Task.WhenAll(producer1.Completion, producer2.Completion);
			printBlock.Complete();
			await printBlock.Completion;

			Console.WriteLine("Finished");
			Console.ReadKey();
		}
	}
}
