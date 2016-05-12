using System;
using System.Net;
using System.IO;
using Ionic.Zip;
using Newtonsoft.Json;

namespace THCGamingOTTD
{
	public class Updater
	{

		static public string Endpoint {
			get { return ""; }
		}

		static public string APIRequest(string url)
		{
			HttpWebRequest webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
			webRequest.Method = "GET";
			webRequest.UserAgent = "THCGamingOTTD";
			webRequest.ServicePoint.Expect100Continue = false;
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
			string version = "none";
			try {
				version = Release.GetLatestRelease().TagName;
			} catch(Exception) { }

			return version;
		}

		static public string GetLocalVersion() {
			string hash_file = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "current.version";
			if(!File.Exists(hash_file))
				return "none";

			return File.ReadAllText(hash_file).Trim();
		}

		static public void PrepareBundle(string source) {
			THCGamingOTTD.WriteHeader("Creating Bundle");
			Console.WriteLine();

			using (ZipFile zip = new ZipFile())
			{
				zip.AddDirectory(source, "");
				zip.Comment = "THCGamingOTTD Bundle. Created: " + System.DateTime.Now.ToString("G");
				zip.Save("latest.zip");
			}

			THCGamingOTTD.WriteHeader("Bundle Created!!!");
			Console.WriteLine();
		}

		static public void PerformUpdate() {
			string new_version = Release.GetLatestRelease().TagName;

			string local_file = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "release.zip";
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

					Console.Write("     Extracting 'release.zip': 000%");

					if(Directory.Exists(extract_path_latest))
						Directory.Delete(extract_path_latest);

					Directory.CreateDirectory(extract_path_latest);

					using (ZipFile zip = ZipFile.Read(local_file))
					{
						foreach (ZipEntry file in zip)
						{
							file.Extract(extract_path_latest, ExtractExistingFileAction.OverwriteSilently);
						}
					}
					Console.WriteLine("\b\b\b\b100% [DONE]");
					File.WriteAllText("current.version", new_version);
					File.Delete(local_file);

					Console.WriteLine();
					THCGamingOTTD.WriteHeader("Executing OpenTTD");
					Game.Run();
				};
				string url = Release.GetLatestRelease().GetReleaseURL();
				if(url != "") {
					Console.Write("     Downloading 'release.zip': 000%");
					client.DownloadFileAsync(new Uri(url), local_file);
				} else {
					THCGamingOTTD.WriteError("unable to obtain latest release url");
				}
			} catch (Exception e) {
				THCGamingOTTD.WriteError("http error: " + e.Message);
			}
		}
	}
}
