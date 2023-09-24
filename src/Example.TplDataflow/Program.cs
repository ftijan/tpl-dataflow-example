namespace Example.TplDataflow;
internal class Program
{
	static async Task Main(string[] args)
	{
		Console.WriteLine("Starting dataflow examples");
		// 01
		//ActionBlockExamples.ActionBlockExample();		
		//await ActionBlockExamples.ActionBlockAsyncExample();
		// 02
		//TransformBlockExamples.TransformBlockExample();
		//await TransformBlockExamples.TransformBlockAsyncExample();
		// 03
		// Will throw:
		//BatchBlockExamples.BatchBlockBadExample();
		//BatchBlockExamples.BatchBlockFixedExample();
		//await BatchBlockExamples.BatchBlockFixedAsyncExample();
		// 04
		//TransformManyBlockExamples.TransformManyBlockExample();
		//await TransformManyBlockExamples.TransformManyBlockAsyncExample();
		// 05
		//BufferBlockExamples.BufferBlockSimpleExample();
		//BufferBlockExamples.BufferBlockProducerConsumerExample();
		//BufferBlockExamples.BufferBlockProducerConsumerWithBoundedProducerExample();
		//await BufferBlockExamples.BufferBlockProducerConsumerWithBoundedProducerFixedExample();
		// 06
		//await BroadcastBlockExamples.BroadcastBlockAsyncExample();
		// 07
		//await JoinBlockExamples.JoinBlockAsyncExample();
		//await JoinBlockExamples.JoinBlockComplexAsyncExample();
		// 08
		//await BatchedJoinBlockExamples.BatchedJoinBlockAsyncExample();
		// 09
		//await WriteOnceBlockExamples.WriteOnceBlockExample();
		//await WriteOnceBlockExamples.WriteOnceBlockPipelineExample(); // + cancellation propagation example
		// 10
		//await BlockAppendOptionExamples.BlockAppendOptionExample();
		// 11
		//await BlockMaxMessagesOptionExamples.BlockMaxMessagesOptionExample();
		// 12
		//await BlockMessageFilteringOptionExamples.BlockMessageFilteringOptionExample();
		// 13
		//MultipleProducersExamples.MultipleProducersExample();
		//await MultipleProducersExamples.MultipleProducersWithPropagationBadExample();
		//await MultipleProducersExamples.MultipleProducersWithPropagationFixedExample();
		// 14
		//await ErrorHandlingExamples.ErrorHandlingExample();
		//await ErrorHandlingExamples.ErrorHandling2Example();
		// 15
		//await CustomBlocksExamples.CustomBlocksEncapsulateExample();
		// 16
		//await CustomBlocksInheritanceExamples.GuaranteedDeliveryBroadcastBlockExample();
		// 17
		//PerformanceBaselineExamples.PerformanceBaselineExample();
		//PerformanceBaselineExamples.PerformanceBaselineMultithreadsExample();
		//PerformanceBaselineExamples.PerformanceBaselineNonTplExample();
		// 18
		//SingleProducerConstrainedExamples.SingleProducerConstrainedExample();
		await SingleProducerConstrainedExamples.SingleProducerConstrainedMultipleProducersBugExample();
	}	
}	