using System;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Primitives;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Rendering;

namespace UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Rendering
{
	[AddComponentMenu("Affordance System/Receiver/Rendering/Float Material Property Affordance Receiver", 12)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Rendering.FloatMaterialPropertyAffordanceReceiver.html")]
	[RequireComponent(typeof(MaterialPropertyBlockHelper))]
	[Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
	public class FloatMaterialPropertyAffordanceReceiver : FloatAffordanceReceiver
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

		public string floatPropertyName
		{
			get
			{
				return this.m_FloatPropertyName;
			}
			set
			{
				this.m_FloatPropertyName = value;
				this.m_FloatProperty = Shader.PropertyToID(this.m_FloatPropertyName);
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
			this.m_FloatProperty = Shader.PropertyToID(this.m_FloatPropertyName);
		}

		protected override void OnAffordanceValueUpdated(float newValue)
		{
			MaterialPropertyBlock materialPropertyBlock = this.m_MaterialPropertyBlockHelper.GetMaterialPropertyBlock(true);
			if (materialPropertyBlock != null)
			{
				materialPropertyBlock.SetFloat(this.m_FloatProperty, newValue);
			}
			base.OnAffordanceValueUpdated(newValue);
		}

		protected override float GetCurrentValueForCapture()
		{
			return this.m_MaterialPropertyBlockHelper.GetSharedMaterialForTarget().GetFloat(this.m_FloatProperty);
		}

		[SerializeField]
		[Tooltip("Material Property Block Helper component reference used to set material properties.")]
		private MaterialPropertyBlockHelper m_MaterialPropertyBlockHelper;

		[SerializeField]
		[Tooltip("Shader property name to set the float value of.")]
		private string m_FloatPropertyName;

		private int m_FloatProperty;
	}
}
