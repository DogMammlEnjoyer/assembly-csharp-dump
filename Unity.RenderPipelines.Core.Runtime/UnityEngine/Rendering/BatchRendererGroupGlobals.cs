using System;

namespace UnityEngine.Rendering
{
	[Obsolete("BatchRendererGroupGlobals and associated cbuffer are now set automatically by Unity. Setting it manually is no longer necessary or supported.")]
	[Serializable]
	public struct BatchRendererGroupGlobals : IEquatable<BatchRendererGroupGlobals>
	{
		public static BatchRendererGroupGlobals Default
		{
			get
			{
				BatchRendererGroupGlobals batchRendererGroupGlobals = default(BatchRendererGroupGlobals);
				batchRendererGroupGlobals.ProbesOcclusion = Vector4.one;
				batchRendererGroupGlobals.SpecCube0_HDR = ReflectionProbe.defaultTextureHDRDecodeValues;
				batchRendererGroupGlobals.SpecCube1_HDR = batchRendererGroupGlobals.SpecCube0_HDR;
				batchRendererGroupGlobals.SHCoefficients = new SHCoefficients(RenderSettings.ambientProbe);
				return batchRendererGroupGlobals;
			}
		}

		public bool Equals(BatchRendererGroupGlobals other)
		{
			return this.ProbesOcclusion.Equals(other.ProbesOcclusion) && this.SpecCube0_HDR.Equals(other.SpecCube0_HDR) && this.SpecCube1_HDR.Equals(other.SpecCube1_HDR) && this.SHCoefficients.Equals(other.SHCoefficients);
		}

		public override bool Equals(object obj)
		{
			if (obj is BatchRendererGroupGlobals)
			{
				BatchRendererGroupGlobals other = (BatchRendererGroupGlobals)obj;
				return this.Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<Vector4, Vector4, Vector4, SHCoefficients>(this.ProbesOcclusion, this.SpecCube0_HDR, this.SpecCube1_HDR, this.SHCoefficients);
		}

		public static bool operator ==(BatchRendererGroupGlobals left, BatchRendererGroupGlobals right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(BatchRendererGroupGlobals left, BatchRendererGroupGlobals right)
		{
			return !left.Equals(right);
		}

		public const string kGlobalsPropertyName = "unity_DOTSInstanceGlobalValues";

		public static readonly int kGlobalsPropertyId = Shader.PropertyToID("unity_DOTSInstanceGlobalValues");

		public Vector4 ProbesOcclusion;

		public Vector4 SpecCube0_HDR;

		public Vector4 SpecCube1_HDR;

		public SHCoefficients SHCoefficients;
	}
}
