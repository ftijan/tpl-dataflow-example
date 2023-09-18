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
		TransformManyBlockExamples.TransformManyBlockExample();
		await TransformManyBlockExamples.TransformManyBlockAsyncExample();
	}	
}	