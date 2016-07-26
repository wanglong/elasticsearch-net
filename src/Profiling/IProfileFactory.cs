using System.Threading.Tasks;

namespace Profiling
{
	internal interface IProfileFactory
	{
		void Run();

		Task RunAsync();
	}
}
