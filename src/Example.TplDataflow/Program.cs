﻿namespace Example.TplDataflow;
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
		await BatchedJoinBlockExamples.BatchedJoinBlockAsyncExample();
	}	
}	