using System.Runtime.InteropServices;
using System.Threading.Tasks.Dataflow;

namespace Example.TplDataflow
{
	internal static class ActionBlockExamples
	{
		internal static void ActionBlockExample()
		{
			var actionBlock = new ActionBlock<int>(n =>
			{
				Task.Delay(500).Wait();
				Console.WriteLine(n);
			});

			for (int i = 0; i < 10; i++)
			{	
				actionBlock.Post(i);
                Console.WriteLine($"There are {actionBlock.InputCount} messages in the input queue.");
            }

			actionBlock.Complete();
			actionBlock.Completion.Wait();
			Console.WriteLine($"Finished {nameof(ActionBlockExample)} method.");
		}

		internal static async Task ActionBlockAsyncExample()
		{
			var actionBlock = new ActionBlock<int>(async n =>
			{
				await Task.Delay(500);
				Console.WriteLine(n);
			});

			for (int i = 0; i < 10; i++)
			{				
				await actionBlock.SendAsync(i);
				Console.WriteLine($"There are {actionBlock.InputCount} messages in the input queue.");
			}

			actionBlock.Complete();
			await actionBlock.Completion;
			Console.WriteLine($"Finished {nameof(ActionBlockAsyncExample)} method.");
		}
	}
}
