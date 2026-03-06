using System;

namespace UnityEngine.Rendering
{
	[ExecuteAlways]
	[AddComponentMenu("Rendering/Probe Adjustment Volume")]
	public class ProbeAdjustmentVolume : MonoBehaviour, ISerializationCallbackReceiver
	{
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			if (this.version == ProbeAdjustmentVolume.Version.Count)
			{
				this.version = ProbeAdjustmentVolume.Version.Mode;
			}
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			if (this.version == ProbeAdjustmentVolume.Version.Count)
			{
				this.version = ProbeAdjustmentVolume.Version.Initial;
			}
			if (this.version < ProbeAdjustmentVolume.Version.Mode)
			{
				if (this.invalidateProbes)
				{
					this.mode = ProbeAdjustmentVolume.Mode.InvalidateProbes;
				}
				else if (this.overrideDilationThreshold)
				{
					this.mode = ProbeAdjustmentVolume.Mode.OverrideValidityThreshold;
				}
				this.version = ProbeAdjustmentVolume.Version.Mode;
			}
		}

		[Tooltip("Select the shape used for this Probe Adjustment Volume.")]
		public ProbeAdjustmentVolume.Shape shape;

		[Min(0f)]
		[Tooltip("Modify the size of this Probe Adjustment Volume. This is unaffected by the GameObject's Transform's Scale property.")]
		public Vector3 size = new Vector3(1f, 1f, 1f);

		[Min(0f)]
		[Tooltip("Modify the radius of this Probe Adjustment Volume. This is unaffected by the GameObject's Transform's Scale property.")]
		public float radius = 1f;

		public ProbeAdjustmentVolume.Mode mode;

		[Range(0.0001f, 2f)]
		[Tooltip("A multiplier applied to the intensity of probes covered by this Probe Adjustment Volume.")]
		public float intensityScale = 1f;

		[Range(0f, 0.95f)]
		public float overriddenDilationThreshold = 0.75f;

		public Vector3 virtualOffsetRotation = Vector3.zero;

		[Min(0f)]
		public float virtualOffsetDistance = 1f;

		[Range(0f, 1f)]
		[Tooltip("Determines how far Unity pushes a probe out of geometry after a ray hit.")]
		public float geometryBias = 0.01f;

		[Range(0f, 0.95f)]
		public float virtualOffsetThreshold = 0.75f;

		[Range(-0.05f, 0f)]
		[Tooltip("Distance from the probe position used to determine the origin of the sampling ray.")]
		public float rayOriginBias = -0.001f;

		[Tooltip("The direction for sampling the ambient probe in worldspace when using the Sky Visibility feature.")]
		public Vector3 skyDirection = Vector3.zero;

		internal Vector3 skyShadingDirectionRotation = Vector3.zero;

		[Logarithmic(1, 1024)]
		[Tooltip("Number of samples for direct lighting computations.")]
		public int directSampleCount = 32;

		[Logarithmic(1, 8192)]
		[Tooltip("Number of samples for indirect lighting computations. This includes environment samples.")]
		public int indirectSampleCount = 512;

		[Min(0f)]
		[Tooltip("Multiplier for the number of samples specified above.")]
		public int sampleCountMultiplier = 4;

		[Min(0f)]
		[Tooltip("Maximum number of bounces for indirect lighting.")]
		public int maxBounces = 2;

		[Logarithmic(1, 8192)]
		public int skyOcclusionSampleCount = 2048;

		[Range(0f, 5f)]
		public int skyOcclusionMaxBounces = 2;

		public ProbeAdjustmentVolume.RenderingLayerMaskOperation renderingLayerMaskOperation;

		public byte renderingLayerMask;

		[SerializeField]
		private ProbeAdjustmentVolume.Version version = ProbeAdjustmentVolume.Version.Count;

		[Obsolete("This field is only kept for migration purpose. Use mode instead. #from(2023.1)")]
		public bool invalidateProbes;

		[Obsolete("This field is only kept for migration purpose. Use mode instead. #from(2023.1)")]
		public bool overrideDilationThreshold;

		public enum Shape
		{
			Box,
			Sphere
		}

		public enum Mode
		{
			InvalidateProbes,
			OverrideValidityThreshold,
			ApplyVirtualOffset,
			OverrideVirtualOffsetSettings,
			OverrideSkyDirection,
			OverrideSampleCount,
			OverrideRenderingLayerMask,
			IntensityScale = 99
		}

		public enum RenderingLayerMaskOperation
		{
			Override,
			Add,
			Remove
		}

		private enum Version
		{
			Initial,
			Mode,
			Count
		}
	}
}
