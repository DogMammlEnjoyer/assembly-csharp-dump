using System;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Primitives;

namespace UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Transformation
{
	[AddComponentMenu("Affordance System/Receiver/Transformation/Uniform Transform Scale Affordance Receiver", 12)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Transformation.UniformTransformScaleAffordanceReceiver.html")]
	[Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
	public class UniformTransformScaleAffordanceReceiver : FloatAffordanceReceiver
	{
		public Transform transformToScale
		{
			get
			{
				return this.m_TransformToScale;
			}
			set
			{
				this.m_TransformToScale = value;
				this.m_HasTransformToScale = (this.m_TransformToScale != null);
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_HasTransformToScale = (this.m_TransformToScale != null);
		}

		protected override float GetCurrentValueForCapture()
		{
			if (this.m_HasTransformToScale)
			{
				this.m_InitialScale = this.m_TransformToScale.localScale;
			}
			return 1f;
		}

		protected override void OnAffordanceValueUpdated(float newValue)
		{
			if (this.m_HasTransformToScale)
			{
				this.m_TransformToScale.localScale = this.m_InitialScale * newValue;
			}
			base.OnAffordanceValueUpdated(newValue);
		}

		private void OnValidate()
		{
			if (this.m_TransformToScale == null)
			{
				this.m_TransformToScale = base.transform;
			}
		}

		[SerializeField]
		[Tooltip("Transform on which to apply scale value.")]
		private Transform m_TransformToScale;

		private bool m_HasTransformToScale;

		private Vector3 m_InitialScale = Vector3.one;
	}
}
