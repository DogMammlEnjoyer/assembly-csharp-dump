using System;

namespace UnityEngine.XR.OpenXR.Features.MetaQuestSupport
{
	public class MetaQuestFeature : OpenXRFeature, ISerializationCallbackReceiver
	{
		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
		}

		public const string featureId = "com.unity.openxr.feature.metaquest";

		private const string ambientOcclusionScriptName = "ScreenSpaceAmbientOcclusion";

		[Serializable]
		internal struct TargetDevice
		{
			public string visibleName;

			public string manifestName;

			public bool enabled;

			[NonSerialized]
			public bool active;
		}
	}
}
