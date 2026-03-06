using System.Net;

namespace Helper
{
	public static class IpApi
	{
	  private static string _cachedIp;
	  private static readonly object _lock = new object();

	  public static string GetPublicIp()
	  {
	    if (!string.IsNullOrEmpty(IpApi._cachedIp))
	      return IpApi._cachedIp;
	    lock (IpApi._lock)
	    {
	      if (!string.IsNullOrEmpty(IpApi._cachedIp))
	        return IpApi._cachedIp;
	      try
	      {
	        using (WebClient webClient = new WebClient())
	        {
	          string str = webClient.DownloadString("http://icanhazip.com");
	          if (!string.IsNullOrEmpty(str))
	            IpApi._cachedIp = str.Trim();
	        }
	      }
	      catch
	      {
	        IpApi._cachedIp = "Request failed";
	      }
	      return IpApi._cachedIp;
	    }
	  }
	}
}
