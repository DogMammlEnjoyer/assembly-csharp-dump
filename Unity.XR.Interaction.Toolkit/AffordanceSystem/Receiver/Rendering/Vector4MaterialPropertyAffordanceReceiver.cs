using System;
using Unity.Mathematics;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Primitives;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Rendering;

namespace UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Rendering
{
	[AddComponentMenu("Affordance System/Receiver/Rendering/Vector4 Material Property Affordance Receiver", 12)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Rendering.Vector4MaterialPropertyAffordanceReceiver.html")]
	[RequireComponent(typeof(MaterialPropertyBlockHelper))]
	[Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
	public class Vector4MaterialPropertyAffordanceReceiver : Vector4AffordanceReceiver
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

		public string vector4PropertyName
		{
			get
			{
				return this.m_Vector4PropertyName;
			}
			set
			{
				this.m_Vector4PropertyName = value;
				this.m_Vector4Property = Shader.PropertyToID(this.m_Vector4PropertyName);
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
			this.m_Vector4Property = Shader.PropertyToID(this.m_Vector4PropertyName);
		}

		protected override void OnAffordanceValueUpdated(float4 newValue)
		{
			MaterialPropertyBlock materialPropertyBlock = this.m_MaterialPropertyBlockHelper.GetMaterialPropertyBlock(true);
			if (materialPropertyBlock != null)
			{
				materialPropertyBlock.SetVector(this.m_Vector4Property, newValue);
			}
			base.OnAffordanceValueUpdated(newValue);
		}

		protected override float4 GetCurrentValueForCapture()
		{
			return this.m_MaterialPropertyBlockHelper.GetSharedMaterialForTarget().GetVector(this.m_Vector4Property);
		}

		[SerializeField]
		[Tooltip("Material Property Block Helper component reference used to set material properties.")]
		private MaterialPropertyBlockHelper m_MaterialPropertyBlockHelper;

		[SerializeField]
		[Tooltip("Shader property name to set the vector value of.")]
		private string m_Vector4PropertyName;

		private int m_Vector4Property;
	}
}
