using System;
using Unity.Mathematics;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Primitives
{
	[AddComponentMenu("Affordance System/Receiver/Primitives/Quaternion Euler Affordance Receiver", 12)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Primitives.QuaternionEulerAffordanceReceiver.html")]
	[Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
	public class QuaternionEulerAffordanceReceiver : Vector3AffordanceReceiver
	{
		public QuaternionUnityEvent quaternionValueUpdated
		{
			get
			{
				return this.m_QuaternionValueUpdated;
			}
			set
			{
				this.m_QuaternionValueUpdated = value;
			}
		}

		protected override void OnAffordanceValueUpdated(float3 newValue)
		{
			base.OnAffordanceValueUpdated(newValue);
			QuaternionUnityEvent quaternionValueUpdated = this.m_QuaternionValueUpdated;
			if (quaternionValueUpdated == null)
			{
				return;
			}
			quaternionValueUpdated.Invoke(Quaternion.Euler(newValue));
		}

		[SerializeField]
		[Tooltip("The event that is called when the current affordance value is updated, expressed as a quaternion generated from euler angles.")]
		private QuaternionUnityEvent m_QuaternionValueUpdated;
	}
}
