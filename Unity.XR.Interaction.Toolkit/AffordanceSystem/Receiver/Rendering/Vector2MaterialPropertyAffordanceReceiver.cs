using System;
using Unity.Mathematics;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Primitives;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Rendering;

namespace UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Rendering
{
	[AddComponentMenu("Affordance System/Receiver/Rendering/Vector2 Material Property Affordance Receiver", 12)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Rendering.Vector2MaterialPropertyAffordanceReceiver.html")]
	[RequireComponent(typeof(MaterialPropertyBlockHelper))]
	[Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
	public class Vector2MaterialPropertyAffordanceReceiver : Vector2AffordanceReceiver
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

		public string vector2PropertyName
		{
			get
			{
				return this.m_Vector2PropertyName;
			}
			set
			{
				this.m_Vector2PropertyName = value;
				this.m_Vector2Property = Shader.PropertyToID(this.m_Vector2PropertyName);
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
			this.m_Vector2Property = Shader.PropertyToID(this.m_Vector2PropertyName);
		}

		protected override void OnAffordanceValueUpdated(float2 newValue)
		{
			MaterialPropertyBlock materialPropertyBlock = this.m_MaterialPropertyBlockHelper.GetMaterialPropertyBlock(true);
			if (materialPropertyBlock != null)
			{
				materialPropertyBlock.SetVector(this.m_Vector2Property, newValue);
			}
			base.OnAffordanceValueUpdated(newValue);
		}

		protected override float2 GetCurrentValueForCapture()
		{
			return this.m_MaterialPropertyBlockHelper.GetSharedMaterialForTarget().GetVector(this.m_Vector2Property);
		}

		[SerializeField]
		[Tooltip("Material Property Block Helper component reference used to set material properties.")]
		private MaterialPropertyBlockHelper m_MaterialPropertyBlockHelper;

		[SerializeField]
		[Tooltip("Shader property name to set the vector value of.")]
		private string m_Vector2PropertyName;

		private int m_Vector2Property;
	}
}
