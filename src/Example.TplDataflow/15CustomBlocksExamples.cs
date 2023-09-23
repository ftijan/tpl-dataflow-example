using System.Threading.Tasks.Dataflow;

namespace Example.TplDataflow
{
	internal static class CustomBlocksExamples
	{
		internal static async Task CustomBlocksEncapsulateExample()
		{
			var inputBlock = new TransformBlock<int, int>(a =>
			{
				if (a == 2) throw new Exception("Did I propagate?");
				return a;
			});
			var increasingBlock = CreateFilteringBlock<int>();
			inputBlock.LinkToWithPropagation(increasingBlock);

			var printBlock = new ActionBlock<int>(a => {  Console.WriteLine($"Message {a} received"); });

			increasingBlock.LinkToWithPropagation(printBlock);

			//await increasingBlock.SendAsync(1);
			//await increasingBlock.SendAsync(2);
			//await increasingBlock.SendAsync(1);
			//await increasingBlock.SendAsync(3);
			//await increasingBlock.SendAsync(4);
			//await increasingBlock.SendAsync(2);
			await inputBlock.SendAsync(1);
			await inputBlock.SendAsync(2);
			await inputBlock.SendAsync(1);
			await inputBlock.SendAsync(3);
			await inputBlock.SendAsync(4);
			await inputBlock.SendAsync(2);

			//increasingBlock.Complete();
			inputBlock.Complete();

			await printBlock.Completion;

            Console.WriteLine("Finished");
			Console.ReadKey();
        }

		internal static IPropagatorBlock<T, T> CreateFilteringBlock<T>()
				where T : IComparable<T>, new()
		{
			T maxElement = default;

			var source = new BufferBlock<T>();
			var target = new ActionBlock<T>(async item => 
			{
				if (item.CompareTo(maxElement) > 0)
				{ 
					await source.SendAsync(item);
					maxElement = item;
				}
			});

			// Will not propagate faults:
			//target.Completion.ContinueWith(_ => source.Complete());
			target.Completion.ContinueWith(a => 
			{ 
				// Propagate faulted state's exception when faulted
				if(a.IsFaulted) 
				{
					((ITargetBlock<T>)source).Fault(a.Exception!);
				}
				else 
				{ 
					source.Complete();
				}
			});

			return DataflowBlock.Encapsulate(target, source);			
		}
	}
}
