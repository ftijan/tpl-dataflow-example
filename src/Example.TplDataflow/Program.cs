namespace Example.TplDataflow;
internal class Program
{
	static async Task Main(string[] args)
	{
		Console.WriteLine("Starting dataflow examples");

		//ActionBlockExamples.ActionBlockExample();		
		//await ActionBlockExamples.ActionBlockAsyncExample();
		TransformBlockExamples.TransformBlockExample();
		await TransformBlockExamples.TransformBlockAsyncExample();
	}	
}	