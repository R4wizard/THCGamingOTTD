using System;
using System.Net;
using System.IO;
using System.Threading;

namespace THCGamingOTTD
{
	public class THCGamingOTTD
	{
		public static void Main(string[] args)
		{
			ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

			try {
				new THCGamingOTTD(args);
				while (!THCGamingOTTD.Done) {
					Thread.Sleep (1000);
				}
			} catch(Exception e) {
				Console.WriteLine(e);
				THCGamingOTTD.WriteError("Fatal Unhandled Error");
			}
		}

		static public string Version {
			get { return "0.1.1"; }
		}

		static public void WriteHeader(string header) {
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine ("===> {0}", header);
			Console.ForegroundColor = ConsoleColor.Gray;
		}

		static public void WriteError(string error) {
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine();
			Console.WriteLine("!!!> {0}", error);
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write("     press any key to continue...");
			Console.ReadKey();
			Console.WriteLine();
			THCGamingOTTD.Done = true;
		}

		static public bool Done = false;

		public THCGamingOTTD(string[] args)
		{
			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.Write ("        _______ _______ ______ _______                  __        ");
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine ("v" + THCGamingOTTD.Version);
			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine ("       |_     _|   |   |      |     __|.---.-.--------.|__|.-----.-----.");
			Console.WriteLine ("         |   | |       |   ---|    |  ||  _  |        ||  ||     |  _  |");
			Console.WriteLine ("         |___| |___|___|______|_______||___._|__|__|__||__||__|__|___  |");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write ("       thcgaming - openttd ");
			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine ("                                      |_____|");
			Console.WriteLine ();
			Console.WriteLine ();
			Console.ForegroundColor = ConsoleColor.Gray;

			if (args.Length != 0) {
				this.HandleCLI (args);
				return;
			}

			WriteHeader("Checking Version");
			Console.Write("     Checking remote server for version: ");
			string remote_version = Updater.GetLatestVersion();
			Console.WriteLine(remote_version);

			Console.Write("     Checking for local version: ");
			string local_version = Updater.GetLocalVersion();
			Console.WriteLine(local_version);

			Console.WriteLine();

			if (remote_version == "none") {
				if(local_version == "none") {
					WriteError("ERROR: Unable to obtain remote version.");
					return;
				} else {
					WriteHeader("Press any key to launch installed version...");
					Console.ReadKey();
					this.Run();
				}
			} else if (remote_version != local_version) {
				WriteHeader("Downloading Latest Version");
				Updater.PerformUpdate();
			} else {
				WriteHeader("Executing OpenTTD");
				this.Run();
			}
		}

		public void HandleCLI(string[] args)
		{
			switch (args[0]) {
				case "run":
				case "--run":
				case "-r":
					this.Run();
					break;

				case "version":
				case "--version":
				case "-v":
					this.DisplayVersions();
					break;

				case "help":
				case "--help":
				case "-h":
				default:
					this.DisplayHelp();
					break;
			}
		}

		public void Run() {
			Game.Run();
		}

		public void DisplayVersions() {
			Console.WriteLine(" THCGaming OTTD: {0}", THCGamingOTTD.Version);
			Console.WriteLine("  Local Version: {0}", Updater.GetLocalVersion());
			Console.WriteLine(" Remote Version: {0}", Updater.GetLatestVersion());
		}

		public void DisplayHelp() {
			Console.WriteLine("THCGamingOTTD - version {0}", THCGamingOTTD.Version);
			Console.WriteLine();
			Console.WriteLine("Usage:  ./updater.exe <command> [arguments]");
			Console.WriteLine("Commands:");
			Console.WriteLine("   run                     run the installed version");
			Console.WriteLine("   version                 show version information");
			Console.WriteLine("   help                    show this help screen");
			Console.WriteLine("   prepare <bundle-dir>    prepares a zip file for release from a bundle dir.");
		}

	}
}
