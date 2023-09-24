using System.Threading.Tasks.Dataflow;

namespace Example.TplDataflow
{
	internal static class SharedResourceSchedulerExamples
	{
		static Random _random = new Random();
		private static int _counter = 0;
		private static int GetSharedObjectValue() { return _counter; }
		private static void SetSharedObjectValue(int value) { _counter = value; }

		internal static async Task SharedResourceSchedulerExample()
		{
			var inputBlock = new BroadcastBlock<int>(a => a);

			// Can be used to force only one thread to execute at a time with ExclusiveScheduler property
			var taskScheduler = new System.Threading.Tasks.ConcurrentExclusiveSchedulerPair();

			Action<int> actionBlockFunction = (int a) =>
			{
				var counterValue = GetSharedObjectValue();
				// Simulate complex work:
				Task.Delay(_random.Next(300)).Wait();
                Console.WriteLine($@"
The counter value read was {counterValue}
Will set it to {counterValue + 1}
");
                SetSharedObjectValue(counterValue + 1);
			};

			var incrementingBlock1 = new ActionBlock<int>(actionBlockFunction, new ExecutionDataflowBlockOptions 
			{ 
				TaskScheduler = taskScheduler.ExclusiveScheduler
			});
			var incrementingBlock2 = new ActionBlock<int>(actionBlockFunction, new ExecutionDataflowBlockOptions 
			{
				TaskScheduler = taskScheduler.ExclusiveScheduler
			});
			

			inputBlock.LinkToWithPropagation(incrementingBlock1);
			inputBlock.LinkToWithPropagation(incrementingBlock2);

			for (int i = 0; i < 10; i++) 
			{
				inputBlock.Post(i);
			}

			inputBlock.Complete();
			await incrementingBlock1.Completion;
			await incrementingBlock2.Completion;

            Console.WriteLine($"Current counter value {GetSharedObjectValue()}");

        }
	}
}
