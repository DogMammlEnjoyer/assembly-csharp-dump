using System;

namespace UnityEngine.VFX
{
	internal class VFXRuntimeResources : ScriptableObject
	{
		internal ComputeShader sdfRayMapCS
		{
			get
			{
				return this.m_SDFRayMapCS;
			}
			set
			{
				this.m_SDFRayMapCS = value;
			}
		}

		internal ComputeShader sdfNormalsCS
		{
			get
			{
				return this.m_SDFNormalsCS;
			}
			set
			{
				this.m_SDFNormalsCS = value;
			}
		}

		internal Shader sdfRayMapShader
		{
			get
			{
				return this.m_SDFRayMapShader;
			}
			set
			{
				this.m_SDFRayMapShader = value;
			}
		}

		public static VFXRuntimeResources runtimeResources
		{
			get
			{
				return VFXManager.runtimeResources as VFXRuntimeResources;
			}
		}

		[SerializeField]
		private ComputeShader m_SDFRayMapCS;

		[SerializeField]
		private ComputeShader m_SDFNormalsCS;

		[SerializeField]
		private Shader m_SDFRayMapShader;
	}
}
