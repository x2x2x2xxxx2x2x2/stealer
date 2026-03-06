using System.Threading;

namespace CvMega.Helper
{
	public static class MutexControl
	{
	  public static Mutex currentApp;
	  public static bool createdNew;

	  public static bool CreateMutex(string mtx)
	  {
	    MutexControl.currentApp = new Mutex(false, mtx, out MutexControl.createdNew);
	    return MutexControl.createdNew;
	  }
	}
}
