using System;
using System.Runtime.InteropServices;

namespace Helper.Encrypted
{
	public static class DpApi
	{
	  private static Helper.NativeMethods.CryptprotectPromptstruct Prompt = new Helper.NativeMethods.CryptprotectPromptstruct()
	  {
	    cbSize = Marshal.SizeOf(typeof (Helper.NativeMethods.CryptprotectPromptstruct)),
	    dwPromptFlags = 0,
	    hwndApp = IntPtr.Zero,
	    szPrompt = (string) null
	  };

	  public static byte[] Decrypt(byte[] bCipher)
	  {
	    Helper.NativeMethods.DataBlob pDataIn = new Helper.NativeMethods.DataBlob();
	    Helper.NativeMethods.DataBlob pDataOut = new Helper.NativeMethods.DataBlob();
	    Helper.NativeMethods.DataBlob pOptionalEntropy = new Helper.NativeMethods.DataBlob();
	    string empty = string.Empty;
	    GCHandle gcHandle = GCHandle.Alloc((object) bCipher, GCHandleType.Pinned);
	    pDataIn.cbData = bCipher.Length;
	    pDataIn.pbData = gcHandle.AddrOfPinnedObject();
	    try
	    {
	      if (!Helper.NativeMethods.CryptUnprotectData(ref pDataIn, ref empty, ref pOptionalEntropy, IntPtr.Zero, ref DpApi.Prompt, 0, ref pDataOut) || pDataOut.cbData == 0)
	        return (byte[]) null;
	      byte[] destination = new byte[pDataOut.cbData];
	      Marshal.Copy(pDataOut.pbData, destination, 0, pDataOut.cbData);
	      return destination;
	    }
	    finally
	    {
	      gcHandle.Free();
	      if (pDataOut.pbData != IntPtr.Zero)
	        Marshal.FreeHGlobal(pDataOut.pbData);
	      if (pOptionalEntropy.pbData != IntPtr.Zero)
	        Marshal.FreeHGlobal(pOptionalEntropy.pbData);
	    }
	  }
	}
}
