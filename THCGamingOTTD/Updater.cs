using System;
using System.Net;
using System.IO;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.GZip;

namespace THCGamingOTTD
{
	public class Updater
	{

		static public string APIRequest(string url)
		{
			HttpWebRequest webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
			webRequest.Method = "GET";
			webRequest.UserAgent = "THCGamingOTTD";
			webRequest.ServicePoint.Expect100Continue = false;
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
			try
			{
				using (StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
					return responseReader.ReadToEnd();
			}
			catch
			{
				return String.Empty;
			}
		}

		static public string GetLatestVersion() {
			try {
                string hash = Updater.APIRequest("https://openttd.gaming.shadowacre.ltd/bundles/latest.hash");
                return hash.Substring(0, 12);
			} catch(Exception) {
				return "none";
			}
		}

		static public string GetLocalVersion() {
			string hash_file = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "current.version";
			if(!File.Exists(hash_file))
				return "none";

			return File.ReadAllText(hash_file).Trim();
		}

		static public void PerformUpdate() {
            string new_version = Updater.GetLatestVersion();

            string filename = "";
            switch ((int)Environment.OSVersion.Platform)
            {
                case 4:   // mono
                case 6:   // mac os x ?
                case 128: // mono (1.0/1.1)
                    filename = "latest-linux.tar.gz";
                    break;
                case 2:   // Win32NT
                    filename = "latest-i686-w64-mingw32.tar.gz";
                    break;
                default:
                    THCGamingOTTD.WriteError("unable to detect platform. got: " + Environment.OSVersion.Platform + " (" + (int)Environment.OSVersion.Platform + ")");
                    break;
            }

            string local_file = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + filename;
			string extract_path_latest = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + new_version;

			try {
				bool finished = false;
				WebClient client = new WebClient();
				client.DownloadProgressChanged += (s, e) => {
					if(finished) return;
					Console.Write("\b\b\b\b{0:D3}%", e.ProgressPercentage);
				};
				client.DownloadFileCompleted += (s, e) => {
					finished = true;
					Console.WriteLine("\b\b\b\b100% [DONE]");
					if(e.Error != null) {
						THCGamingOTTD.WriteError("http error: " + e.Error.Message);
						return;
					}

					Console.Write("     Extracting '" + filename + "': 000%");

					if(Directory.Exists(extract_path_latest))
						Directory.Delete(extract_path_latest);

					Directory.CreateDirectory(extract_path_latest);

                    Updater.ExtractTGZ(local_file, extract_path_latest);
					Console.WriteLine("\b\b\b\b100% [DONE]");
					File.WriteAllText("current.version", new_version);
					File.Delete(local_file);

					Console.WriteLine();
					THCGamingOTTD.WriteHeader("Executing OpenTTD");
					Game.Run();
				};

                if (filename != "")
                {
					Console.Write("     Downloading '" + filename + "': 000%");
					client.DownloadFileAsync(new Uri("https://openttd.gaming.shadowacre.ltd/bundles/" + filename), local_file);
				} else {
					THCGamingOTTD.WriteError("unable to obtain latest release url");
				}
			} catch (Exception e) {
				THCGamingOTTD.WriteError("http error: " + e.Message);
			}
		}

        public static void ExtractTGZ(String gzArchiveName, String destFolder)
        {

            Stream inStream = File.OpenRead(gzArchiveName);
            Stream gzipStream = new GZipInputStream(inStream);

            TarArchive tarArchive = TarArchive.CreateInputTarArchive(gzipStream);
            tarArchive.ExtractContents(destFolder);
            tarArchive.Close();

            gzipStream.Close();
            inStream.Close();
        }
	}
}
