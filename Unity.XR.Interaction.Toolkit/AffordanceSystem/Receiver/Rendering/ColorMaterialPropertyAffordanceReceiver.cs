using System;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Primitives;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Rendering;

namespace UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Rendering
{
	[AddComponentMenu("Affordance System/Receiver/Rendering/Color Material Property Affordance Receiver", 12)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Rendering.ColorMaterialPropertyAffordanceReceiver.html")]
	[RequireComponent(typeof(MaterialPropertyBlockHelper))]
	[Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
	public class ColorMaterialPropertyAffordanceReceiver : ColorAffordanceReceiver
	{
		public MaterialPropertyBlockHelper materialPropertyBlockHelper
		{
			get
			{
				return this.m_MaterialPropertyBlockHelper;
			}
			set
			{
				this.m_MaterialPropertyBlockHelper = value;
			}
		}

		public string colorPropertyName
		{
			get
			{
				return this.m_ColorPropertyName;
			}
			set
			{
				this.m_ColorPropertyName = value;
				this.UpdateColorPropertyID();
			}
		}

		protected void OnValidate()
		{
			if (this.m_MaterialPropertyBlockHelper == null)
			{
				this.m_MaterialPropertyBlockHelper = base.GetComponent<MaterialPropertyBlockHelper>();
			}
		}

		protected override void Awake()
		{
			base.Awake();
			if (this.m_MaterialPropertyBlockHelper == null)
			{
				this.m_MaterialPropertyBlockHelper = base.GetComponent<MaterialPropertyBlockHelper>();
			}
			this.UpdateColorPropertyID();
		}

		protected override void OnAffordanceValueUpdated(Color newValue)
		{
			MaterialPropertyBlock materialPropertyBlock = this.m_MaterialPropertyBlockHelper.GetMaterialPropertyBlock(true);
			if (materialPropertyBlock != null)
			{
				materialPropertyBlock.SetColor(this.m_ColorProperty, newValue);
			}
			base.OnAffordanceValueUpdated(newValue);
		}

		protected override Color GetCurrentValueForCapture()
		{
			return this.m_MaterialPropertyBlockHelper.GetSharedMaterialForTarget().GetColor(this.m_ColorProperty);
		}

		private void UpdateColorPropertyID()
		{
			if (!string.IsNullOrEmpty(this.m_ColorPropertyName))
			{
				this.m_ColorProperty = Shader.PropertyToID(this.m_ColorPropertyName);
				return;
			}
			this.m_ColorProperty = ((GraphicsSettings.currentRenderPipeline != null) ? ColorMaterialPropertyAffordanceReceiver.ShaderPropertyLookup.baseColor : ColorMaterialPropertyAffordanceReceiver.ShaderPropertyLookup.color);
		}

		[SerializeField]
		[Tooltip("Material Property Block Helper component reference used to set material properties.")]
		private MaterialPropertyBlockHelper m_MaterialPropertyBlockHelper;

		[SerializeField]
		[Tooltip("Shader property name to set the color of. When empty, the component will attempt to use the default for the current render pipeline.")]
		private string m_ColorPropertyName;

		private int m_ColorProperty;

		private readonly struct ShaderPropertyLookup
		{
			public static readonly int baseColor = Shader.PropertyToID("_BaseColor");

			public static readonly int color = Shader.PropertyToID("_Color");
		}
	}
}
