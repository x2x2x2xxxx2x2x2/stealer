using System.Text;

namespace Helper.Encrypted
{
	public static class Xor
	{
	  public static string DecryptString(string input, byte key)
	  {
	    byte[] bytes = Encoding.UTF8.GetBytes(input);
	    for (int index = 0; index < bytes.Length; ++index)
	      bytes[index] ^= key;
	    return Encoding.UTF8.GetString(bytes);
	  }
	}
}
