using Microsoft.Win32;
using System;

namespace Helper
{
	public static class CpuInfo
	{
	  public static string GetName()
	  {
	    try
	    {
	      using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0"))
	      {
	        if (!(registryKey?.GetValue("ProcessorNameString") is string name))
	          name = registryKey?.GetValue("VendorIdentifier") is string str ? str : "Unknown";
	        return name;
	      }
	    }
	    catch
	    {
	      return "Unknown";
	    }
	  }

	  public static int GetLogicalCores()
	  {
	    try
	    {
	      return Environment.ProcessorCount;
	    }
	    catch
	    {
	      return 0;
	    }
	  }
	}
}
