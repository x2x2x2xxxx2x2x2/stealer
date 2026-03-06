using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace Helper
{
	public static class RegistryParser
	{
	  public static List<string> ParseKey(RegistryKey key)
	  {
	    List<string> key1 = new List<string>();
	    if (key == null)
	      return key1;
	    foreach (string valueName in key.GetValueNames())
	    {
	      object obj = key.GetValue(valueName);
	      string str;
	      switch (key.GetValueKind(valueName))
	      {
	        case RegistryValueKind.String:
	        case RegistryValueKind.ExpandString:
	          str = obj?.ToString() ?? "null";
	          break;
	        case RegistryValueKind.Binary:
	          str = !(obj is byte[] numArray) ? "null" : BitConverter.ToString(numArray).Replace("-", "");
	          break;
	        case RegistryValueKind.DWord:
	        case RegistryValueKind.QWord:
	          str = obj.ToString();
	          break;
	        case RegistryValueKind.MultiString:
	          str = !(obj is string[] strArray) ? "null" : string.Join(", ", strArray);
	          break;
	        default:
	          str = obj?.ToString() ?? "null";
	          break;
	      }
	      key1.Add($"{valueName}: {str}");
	    }
	    return key1;
	  }
	}
}
