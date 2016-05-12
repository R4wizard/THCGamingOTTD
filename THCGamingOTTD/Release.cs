using System;
using Newtonsoft.Json;
using System.IO;

namespace THCGamingOTTD
{
	
	[JsonObject(MemberSerialization.OptIn)]
	public class Release
	{
		[JsonProperty("tag_name", Required = Required.Always)]
		public virtual string TagName { get; set; }

		[JsonProperty("assets", Required = Required.Always)]
		public virtual ReleaseAsset[] Assets { get; set; }

		static public Release GetLatestRelease() {
			try {
				string json = Updater.APIRequest("https://api.github.com/repos/R4wizard/OpenTTD-patches/releases/latest");

				using (var sr = new StringReader(json))
				using (var jr = new JsonTextReader(sr))
				{
					var js = new JsonSerializer();
					return js.Deserialize<Release>(jr);
				}
			} catch(Exception) {
				return new Release() {
					TagName = "none"
				};
			}
		}

		public string GetReleaseURL() {
			foreach (var asset in this.Assets) {
				if (asset.Name == "release.zip")
					return asset.BrowserDownloadURL;
			}

			return "";
		}
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class ReleaseAsset
	{
		[JsonProperty("name", Required = Required.Always)]
		public virtual string Name { get; set; }

		[JsonProperty("browser_download_url", Required = Required.Always)]
		public virtual string BrowserDownloadURL { get; set; }
	}

}

