using System;

namespace UnityEngine.Rendering.Universal
{
	[Obsolete("Moved to UniversalRenderPipelineRuntimeXRResources on GraphicsSettings. #from(2023.3)", false)]
	[Serializable]
	public class XRSystemData : ScriptableObject
	{
		[Obsolete("Moved to UniversalRenderPipelineRuntimeXRResources on GraphicsSettings. #from(2023.3)", false)]
		public XRSystemData.ShaderResources shaders;

		[ReloadGroup]
		[Obsolete("Moved to UniversalRenderPipelineRuntimeXRResources on GraphicsSettings. #from(2023.3)", false)]
		[Serializable]
		public sealed class ShaderResources
		{
			[Reload("Shaders/XR/XROcclusionMesh.shader", ReloadAttribute.Package.Root)]
			public Shader xrOcclusionMeshPS;

			[Reload("Shaders/XR/XRMirrorView.shader", ReloadAttribute.Package.Root)]
			public Shader xrMirrorViewPS;
		}
	}
}
