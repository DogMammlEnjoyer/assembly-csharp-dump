using System;
using UnityEngine.Categorization;

namespace UnityEngine.Rendering.Universal
{
	[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
	[CategoryInfo(Name = "R: 2D Renderer", Order = 1000)]
	[HideInInspector]
	[Serializable]
	internal class Renderer2DResources : IRenderPipelineResources, IRenderPipelineGraphicsSettings
	{
		public int version
		{
			get
			{
				return this.m_Version;
			}
		}

		bool IRenderPipelineGraphicsSettings.isAvailableInPlayerBuild
		{
			get
			{
				return true;
			}
		}

		internal Shader lightShader
		{
			get
			{
				return this.m_LightShader;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_LightShader, value, "m_LightShader");
			}
		}

		internal Shader projectedShadowShader
		{
			get
			{
				return this.m_ProjectedShadowShader;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_ProjectedShadowShader, value, "m_ProjectedShadowShader");
			}
		}

		internal Shader spriteShadowShader
		{
			get
			{
				return this.m_SpriteShadowShader;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_SpriteShadowShader, value, "m_SpriteShadowShader");
			}
		}

		internal Shader spriteUnshadowShader
		{
			get
			{
				return this.m_SpriteUnshadowShader;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_SpriteUnshadowShader, value, "m_SpriteUnshadowShader");
			}
		}

		internal Shader geometryShadowShader
		{
			get
			{
				return this.m_GeometryShadowShader;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_GeometryShadowShader, value, "m_GeometryShadowShader");
			}
		}

		internal Shader geometryUnshadowShader
		{
			get
			{
				return this.m_GeometryUnshadowShader;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_GeometryUnshadowShader, value, "m_GeometryUnshadowShader");
			}
		}

		internal Texture2D fallOffLookup
		{
			get
			{
				return this.m_FallOffLookup;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_FallOffLookup, value, "m_FallOffLookup");
			}
		}

		internal Shader copyDepthPS
		{
			get
			{
				return this.m_CopyDepthPS;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_CopyDepthPS, value, "m_CopyDepthPS");
			}
		}

		[SerializeField]
		[HideInInspector]
		private int m_Version;

		[SerializeField]
		[ResourcePath("Shaders/2D/Light2D.shader", SearchType.ProjectPath)]
		private Shader m_LightShader;

		[SerializeField]
		[ResourcePath("Shaders/2D/Shadow2D-Projected.shader", SearchType.ProjectPath)]
		private Shader m_ProjectedShadowShader;

		[SerializeField]
		[ResourcePath("Shaders/2D/Shadow2D-Shadow-Sprite.shader", SearchType.ProjectPath)]
		private Shader m_SpriteShadowShader;

		[SerializeField]
		[ResourcePath("Shaders/2D/Shadow2D-Unshadow-Sprite.shader", SearchType.ProjectPath)]
		private Shader m_SpriteUnshadowShader;

		[SerializeField]
		[ResourcePath("Shaders/2D/Shadow2D-Shadow-Geometry.shader", SearchType.ProjectPath)]
		private Shader m_GeometryShadowShader;

		[SerializeField]
		[ResourcePath("Shaders/2D/Shadow2D-Unshadow-Geometry.shader", SearchType.ProjectPath)]
		private Shader m_GeometryUnshadowShader;

		[SerializeField]
		[ResourcePath("Runtime/2D/Data/Textures/FalloffLookupTexture.png", SearchType.ProjectPath)]
		[HideInInspector]
		private Texture2D m_FallOffLookup;

		[SerializeField]
		[ResourcePath("Shaders/Utils/CopyDepth.shader", SearchType.ProjectPath)]
		private Shader m_CopyDepthPS;
	}
}
