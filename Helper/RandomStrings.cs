using System;
using System.Linq;

namespace Helper
{
	public static class RandomStrings
	{
	  private const string Ascii = "abcdefghijklmnopqrstuvwxyz";
	  private static readonly Random Random = new Random();

	  public static string GenerateHashTag() => " #" + RandomStrings.GenerateString();

	  public static string GenerateString() => RandomStrings.GenerateString(5);

	  public static string GenerateString(int length)
	  {
	    char ch = "abcdefghijklmnopqrstuvwxyz"[RandomStrings.Random.Next("abcdefghijklmnopqrstuvwxyz".Length)];
	    char[] array = Enumerable.Repeat<string>("abcdefghijklmnopqrstuvwxyz", length - 1).Select<string, char>((Func<string, char>) (s => s[RandomStrings.Random.Next(s.Length)])).ToArray<char>();
	    return ch.ToString() + new string(array);
	  }
	}
}
