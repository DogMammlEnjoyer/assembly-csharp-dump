using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace GT_CustomMapSupportRuntime
{
	[NullableContext(2)]
	[Nullable(0)]
	public class MapPackageInfo
	{
		[JsonConstructor]
		public MapPackageInfo()
		{
		}

		public VirtualStumpReturnWatchProps GetReturnToVStumpWatchProps()
		{
			VirtualStumpReturnWatchProps result;
			result.holdDuration = this.watchHoldDuration;
			result.shouldTagPlayer = this.watchShouldTagPlayer;
			result.shouldKickPlayer = this.watchShouldKickPlayer;
			result.infectionOverride = this.watchInfectionOverride;
			result.holdDuration_Infection = this.watchHoldDuration_Infection;
			result.shouldTagPlayer_Infection = this.watchShouldTagPlayer_Infection;
			result.shouldKickPlayer_Infection = this.watchShouldKickPlayer_Infection;
			result.customModeOverride = this.watchCustomModeOverride;
			result.holdDuration_Custom = this.watchHoldDuration_CustomMode;
			result.shouldTagPlayer_CustomMode = this.watchShouldTagPlayer_CustomMode;
			result.shouldKickPlayer_CustomMode = this.watchShouldKickPlayer_CustomMode;
			return result;
		}

		[JsonProperty(PropertyName = "pcFileName")]
		public string pcFileName;

		[JsonProperty(PropertyName = "androidFileName")]
		public string androidFileName;

		[JsonProperty(PropertyName = "descriptor")]
		public Descriptor descriptor;

		[JsonProperty(PropertyName = "initialScene")]
		public string initialScene;

		[Nullable(new byte[]
		{
			2,
			1
		})]
		[JsonProperty(PropertyName = "initialScenes")]
		public string[] initialScenes;

		[JsonProperty(PropertyName = "customMapSupportVersion")]
		public int customMapSupportVersion;

		[JsonProperty(PropertyName = "maxPlayers")]
		public int maxPlayers = 10;

		[JsonProperty(PropertyName = "availableGameModes")]
		public int[] availableGameModes;

		[JsonProperty(PropertyName = "defaultGameMode")]
		public int defaultGameMode;

		[JsonProperty(PropertyName = "disableHoldingHandsAllModes")]
		public bool disableHoldingHandsAllModes;

		[JsonProperty(PropertyName = "disableHoldingHandsCustomMode")]
		public bool disableHoldingHandsCustomMode;

		[JsonProperty(PropertyName = "watchHoldDuration")]
		public float watchHoldDuration = 0.5f;

		[JsonProperty(PropertyName = "watchShouldTagPlayer")]
		public bool watchShouldTagPlayer;

		[JsonProperty(PropertyName = "watchShouldKickPlayer")]
		public bool watchShouldKickPlayer;

		[JsonProperty(PropertyName = "watchInfectionOverride")]
		public bool watchInfectionOverride;

		[JsonProperty(PropertyName = "watchHoldDuration_Infection")]
		public float watchHoldDuration_Infection = 0.5f;

		[JsonProperty(PropertyName = "watchShouldTagPlayer_Infection")]
		public bool watchShouldTagPlayer_Infection;

		[JsonProperty(PropertyName = "watchShouldKickPlayer_Infection")]
		public bool watchShouldKickPlayer_Infection;

		[JsonProperty(PropertyName = "watchCustomModeOverride")]
		public bool watchCustomModeOverride;

		[JsonProperty(PropertyName = "watchHoldDuration_CustomMode")]
		public float watchHoldDuration_CustomMode = 0.5f;

		[JsonProperty(PropertyName = "watchShouldTagPlayer_CustomMode")]
		public bool watchShouldTagPlayer_CustomMode;

		[JsonProperty(PropertyName = "watchShouldKickPlayer_CustomMode")]
		public bool watchShouldKickPlayer_CustomMode;

		[JsonProperty(PropertyName = "useUberShaderDynamicLighting")]
		public bool useUberShaderDynamicLighting;

		[JsonProperty(PropertyName = "uberShaderAmbientDynamicLight_R")]
		public float uberShaderAmbientDynamicLight_R;

		[JsonProperty(PropertyName = "uberShaderAmbientDynamicLight_G")]
		public float uberShaderAmbientDynamicLight_G;

		[JsonProperty(PropertyName = "uberShaderAmbientDynamicLight_B")]
		public float uberShaderAmbientDynamicLight_B;

		[JsonProperty(PropertyName = "uberShaderAmbientDynamicLight_A")]
		public float uberShaderAmbientDynamicLight_A;

		[JsonProperty(PropertyName = "customGamemodeScript")]
		public string customGamemodeScript;

		[JsonProperty(PropertyName = "luauDevMode")]
		public bool devMode;
	}
}
