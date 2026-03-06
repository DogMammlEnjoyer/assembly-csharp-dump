using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[SaveDuringPlay]
	[AddComponentMenu("Cinemachine/Helpers/Cinemachine External Impulse Listener")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineExternalImpulseListener.html")]
	public class CinemachineExternalImpulseListener : MonoBehaviour
	{
		private void Reset()
		{
			this.ChannelMask = 1;
			this.Gain = 1f;
			this.Use2DDistance = false;
			this.UseLocalSpace = true;
			this.ReactionSettings = new CinemachineImpulseListener.ImpulseReaction
			{
				AmplitudeGain = 1f,
				FrequencyGain = 1f,
				Duration = 1f
			};
		}

		private void OnEnable()
		{
			this.m_ImpulsePosLastFrame = Vector3.zero;
			this.m_ImpulseRotLastFrame = Quaternion.identity;
		}

		private void Update()
		{
			base.transform.position -= this.m_ImpulsePosLastFrame;
			base.transform.rotation = base.transform.rotation * Quaternion.Inverse(this.m_ImpulseRotLastFrame);
		}

		private void LateUpdate()
		{
			bool impulseAt = CinemachineImpulseManager.Instance.GetImpulseAt(base.transform.position, this.Use2DDistance, this.ChannelMask, out this.m_ImpulsePosLastFrame, out this.m_ImpulseRotLastFrame);
			Vector3 b;
			Quaternion rhs;
			bool reaction = this.ReactionSettings.GetReaction(Time.deltaTime, this.m_ImpulsePosLastFrame, out b, out rhs);
			if (impulseAt)
			{
				this.m_ImpulseRotLastFrame = Quaternion.SlerpUnclamped(Quaternion.identity, this.m_ImpulseRotLastFrame, this.Gain);
				this.m_ImpulsePosLastFrame *= this.Gain;
			}
			if (reaction)
			{
				this.m_ImpulsePosLastFrame += b;
				this.m_ImpulseRotLastFrame *= rhs;
			}
			if (impulseAt || reaction)
			{
				if (this.UseLocalSpace)
				{
					this.m_ImpulsePosLastFrame = base.transform.rotation * this.m_ImpulsePosLastFrame;
				}
				base.transform.position += this.m_ImpulsePosLastFrame;
				base.transform.rotation = base.transform.rotation * this.m_ImpulseRotLastFrame;
			}
		}

		private Vector3 m_ImpulsePosLastFrame;

		private Quaternion m_ImpulseRotLastFrame;

		[Tooltip("Impulse events on channels not included in the mask will be ignored.")]
		[CinemachineImpulseChannelProperty]
		[FormerlySerializedAs("m_ChannelMask")]
		public int ChannelMask;

		[Tooltip("Gain to apply to the Impulse signal.  1 is normal strength.  Setting this to 0 completely mutes the signal.")]
		[FormerlySerializedAs("m_Gain")]
		public float Gain;

		[Tooltip("Enable this to perform distance calculation in 2D (ignore Z)")]
		[FormerlySerializedAs("m_Use2DDistance")]
		public bool Use2DDistance;

		[Tooltip("Enable this to process all impulse signals in camera space")]
		[FormerlySerializedAs("m_UseLocalSpace")]
		public bool UseLocalSpace;

		[Tooltip("This controls the secondary reaction of the listener to the incoming impulse.  The impulse might be for example a sharp shock, and the secondary reaction could be a vibration whose amplitude and duration is controlled by the size of the original impulse.  This allows different listeners to respond in different ways to the same impulse signal.")]
		[FormerlySerializedAs("m_ReactionSettings")]
		public CinemachineImpulseListener.ImpulseReaction ReactionSettings;
	}
}
