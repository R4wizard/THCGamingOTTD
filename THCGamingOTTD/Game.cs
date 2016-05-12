using System;
using System.Diagnostics;
using System.IO;

namespace THCGamingOTTD
{
	public class Game
	{
		static public void Run() {
			string game_file = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + Updater.GetLocalVersion() + Path.DirectorySeparatorChar +  "openttd.exe";
			Process.Start(game_file);
			THCGamingOTTD.Done = true;
		}
	}
}

