using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Lib.Wit.Runtime.Utilities.Logging;
using Meta.Voice.Logging;
using Meta.WitAi;
using Meta.WitAi.Json;
using UnityEngine;

namespace Meta.Conduit
{
	[LogCategory(LogCategory.Conduit)]
	internal class ManifestLoader : IManifestLoader, ILogSource
	{
		public IVLogger Logger { get; } = LoggerRegistry.Instance.GetLogger(LogCategory.Conduit, null);

		public Manifest LoadManifest(string manifestLocalPath)
		{
			TextAsset textAsset = Resources.Load<TextAsset>(Path.GetFileNameWithoutExtension(manifestLocalPath));
			if (textAsset == null)
			{
				VLog.E(base.GetType().Name, "No Manifest found at Resources/" + manifestLocalPath, null);
				return null;
			}
			return this.LoadManifestFromJson(textAsset.text);
		}

		public Manifest LoadManifestFromJson(string manifestText)
		{
			Manifest manifest = JsonConvert.DeserializeObject<Manifest>(manifestText, null, false);
			if (manifest.ResolveActions())
			{
				this.Logger.Info("Successfully Loaded Conduit manifest", null, null, null, null, "LoadManifestFromJson", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\Conduit\\Data\\ManifestLoader.cs", 49);
				return manifest;
			}
			VLog.E(base.GetType().Name, "Failed to resolve actions from Conduit manifest", null);
			return manifest;
		}

		public Task<Manifest> LoadManifestAsync(string manifestLocalPath)
		{
			ManifestLoader.<LoadManifestAsync>d__5 <LoadManifestAsync>d__;
			<LoadManifestAsync>d__.<>t__builder = AsyncTaskMethodBuilder<Manifest>.Create();
			<LoadManifestAsync>d__.<>4__this = this;
			<LoadManifestAsync>d__.manifestLocalPath = manifestLocalPath;
			<LoadManifestAsync>d__.<>1__state = -1;
			<LoadManifestAsync>d__.<>t__builder.Start<ManifestLoader.<LoadManifestAsync>d__5>(ref <LoadManifestAsync>d__);
			return <LoadManifestAsync>d__.<>t__builder.Task;
		}

		public Task<Manifest> LoadManifestFromJsonAsync(string manifestText)
		{
			ManifestLoader.<LoadManifestFromJsonAsync>d__6 <LoadManifestFromJsonAsync>d__;
			<LoadManifestFromJsonAsync>d__.<>t__builder = AsyncTaskMethodBuilder<Manifest>.Create();
			<LoadManifestFromJsonAsync>d__.<>4__this = this;
			<LoadManifestFromJsonAsync>d__.manifestText = manifestText;
			<LoadManifestFromJsonAsync>d__.<>1__state = -1;
			<LoadManifestFromJsonAsync>d__.<>t__builder.Start<ManifestLoader.<LoadManifestFromJsonAsync>d__6>(ref <LoadManifestFromJsonAsync>d__);
			return <LoadManifestFromJsonAsync>d__.<>t__builder.Task;
		}
	}
}
