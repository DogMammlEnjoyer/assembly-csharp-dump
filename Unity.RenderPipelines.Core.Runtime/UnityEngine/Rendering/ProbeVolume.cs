using System;

namespace UnityEngine.Rendering
{
	[ExecuteAlways]
	[AddComponentMenu("Rendering/Adaptive Probe Volume")]
	public class ProbeVolume : MonoBehaviour
	{
		private void Awake()
		{
			if (this.version == ProbeVolume.Version.Count)
			{
				return;
			}
			if (this.version == ProbeVolume.Version.Initial)
			{
				this.mode = (this.globalVolume ? ProbeVolume.Mode.Scene : ProbeVolume.Mode.Local);
				this.version++;
			}
			if (this.version == ProbeVolume.Version.LocalMode)
			{
				this.version++;
			}
		}

		[Tooltip("When set to Global this Probe Volume considers all renderers with Contribute Global Illumination enabled. Local only considers renderers in the scene.\nThis list updates every time the Scene is saved or the lighting is baked.")]
		public ProbeVolume.Mode mode = ProbeVolume.Mode.Local;

		public Vector3 size = new Vector3(10f, 10f, 10f);

		[HideInInspector]
		[Min(0f)]
		public bool overrideRendererFilters;

		[HideInInspector]
		[Min(0f)]
		public float minRendererVolumeSize = 0.1f;

		public LayerMask objectLayerMask = -1;

		[HideInInspector]
		public int lowestSubdivLevelOverride;

		[HideInInspector]
		public int highestSubdivLevelOverride = 7;

		[HideInInspector]
		public bool overridesSubdivLevels;

		[SerializeField]
		internal bool mightNeedRebaking;

		[SerializeField]
		internal Matrix4x4 cachedTransform;

		[SerializeField]
		internal int cachedHashCode;

		[HideInInspector]
		[Tooltip("Whether Unity should fill empty space between renderers with bricks at the highest subdivision level.")]
		public bool fillEmptySpaces;

		[SerializeField]
		private ProbeVolume.Version version;

		[SerializeField]
		[Obsolete("Use mode instead")]
		public bool globalVolume;

		public enum Mode
		{
			Global,
			Scene,
			Local
		}

		private enum Version
		{
			Initial,
			LocalMode,
			InvertOverrideLevels,
			Count
		}
	}
}
