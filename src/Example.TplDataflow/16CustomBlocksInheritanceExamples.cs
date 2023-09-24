using System.Threading.Tasks.Dataflow;

namespace Example.TplDataflow
{
	internal static class CustomBlocksInheritanceExamples
	{
		internal static async Task GuaranteedDeliveryBroadcastBlockExample()
		{			
			var broadcastBlock = new GuaranteedDeliveryBroadcastBlock<int>(a => a);

			var consumer1 = new ActionBlock<int>(a =>
			{
				Console.WriteLine($"Message {a} was processed by Consumer 1");
				Task.Delay(500).Wait();
			}, new ExecutionDataflowBlockOptions { BoundedCapacity = 1});

			var consumer2 = new ActionBlock<int>(a =>
			{
				Console.WriteLine($"Message {a} was processed by Consumer 2");
				Task.Delay(150).Wait();
			}, new ExecutionDataflowBlockOptions { BoundedCapacity = 1 });

			broadcastBlock.LinkToWithPropagation(consumer1);
			broadcastBlock.LinkToWithPropagation(consumer2);

			for (int i = 0; i < 10; i++)
			{
				await broadcastBlock.SendAsync(i);
			}

			broadcastBlock.Complete();
			
			await consumer1.Completion;
			await consumer2.Completion;

			Console.WriteLine("Finished");
		}
	}

	internal class GuaranteedDeliveryBroadcastBlock<T> : IPropagatorBlock<T, T>
	{
		private BroadcastBlock<T> _broadcastBlock;
		private Task _completion;

        public GuaranteedDeliveryBroadcastBlock(Func<T, T> cloningFunction)
        {
            _broadcastBlock = new BroadcastBlock<T>(cloningFunction);
			_completion = _broadcastBlock.Completion;
        }

        public Task Completion => _completion;

		public void Complete()
		{
			((ITargetBlock<T>)_broadcastBlock).Complete();			
		}

		public T? ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<T> target, out bool messageConsumed)
		{
			throw new NotSupportedException("This method should not be called. The producer is a BufferBlock.");
		}

		public void Fault(Exception exception)
		{
			((ITargetBlock<T>)_broadcastBlock).Fault(exception);			
		}

		public IDisposable LinkTo(ITargetBlock<T> target, DataflowLinkOptions linkOptions)
		{
			var bufferBlock = new BufferBlock<T>();

			var disposable1 = _broadcastBlock.LinkTo(bufferBlock, linkOptions);
			var disposable2 = bufferBlock.LinkTo(target, linkOptions);

			_completion.ContinueWith(_ => bufferBlock.Completion);

			return new LinkToDisposer(disposable1, disposable2);
		}

		public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, T messageValue, ISourceBlock<T>? source, bool consumeToAccept)
		{
			return ((ITargetBlock<T>)_broadcastBlock).OfferMessage(messageHeader, messageValue, source, consumeToAccept);
		}

		public void ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<T> target)
		{
			throw new NotSupportedException("This method should not be called. The producer is a BufferBlock.");
		}

		public bool ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<T> target)
		{
			throw new NotSupportedException("This method should not be called. The producer is a BufferBlock.");
		}

		class LinkToDisposer : IDisposable
		{
			private readonly IDisposable[] _disposables;

			public LinkToDisposer(params IDisposable[] disposables)
            {
				_disposables = disposables;
			}

            public void Dispose()
			{
				foreach (var disposable in _disposables) 
				{
					disposable.Dispose(); 
				}
				
			}
		}
	}
}
