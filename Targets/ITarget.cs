using Helper.Data;

namespace Targets
{
	public interface ITarget
	{
	  void Collect(InMemoryZip zip, Counter counter);
	}
}
