using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[Serializable]
	public class CinemachineImpulseDefinition
	{
		public void OnValidate()
		{
			RuntimeUtility.NormalizeCurve(this.CustomImpulseShape, true, false);
			this.ImpulseDuration = Mathf.Max(0.0001f, this.ImpulseDuration);
			this.DissipationDistance = Mathf.Max(0.0001f, this.DissipationDistance);
			this.DissipationRate = Mathf.Clamp01(this.DissipationRate);
			this.PropagationSpeed = Mathf.Max(1f, this.PropagationSpeed);
			this.ImpactRadius = Mathf.Max(0f, this.ImpactRadius);
			this.TimeEnvelope.Validate();
			this.PropagationSpeed = Mathf.Max(1f, this.PropagationSpeed);
		}

		private static void CreateStandardShapes()
		{
			int num = 0;
			foreach (object obj in Enum.GetValues(typeof(CinemachineImpulseDefinition.ImpulseShapes)))
			{
				num = Mathf.Max(num, (int)obj);
			}
			CinemachineImpulseDefinition.s_StandardShapes = new AnimationCurve[num + 1];
			CinemachineImpulseDefinition.s_StandardShapes[1] = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 1f, -3.2f, -3.2f),
				new Keyframe(1f, 0f, 0f, 0f)
			});
			CinemachineImpulseDefinition.s_StandardShapes[2] = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, -4.9f, -4.9f),
				new Keyframe(0.2f, 0f, 8.25f, 8.25f),
				new Keyframe(1f, 0f, -0.25f, -0.25f)
			});
			CinemachineImpulseDefinition.s_StandardShapes[3] = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, -1.4f, -7.9f, -7.9f),
				new Keyframe(0.27f, 0.78f, 23.4f, 23.4f),
				new Keyframe(0.54f, -0.12f, 22.6f, 22.6f),
				new Keyframe(0.75f, 0.042f, 9.23f, 9.23f),
				new Keyframe(0.9f, -0.02f, 5.8f, 5.8f),
				new Keyframe(0.95f, -0.006f, -3f, -3f),
				new Keyframe(1f, 0f, 0f, 0f)
			});
			CinemachineImpulseDefinition.s_StandardShapes[4] = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0f),
				new Keyframe(0.1f, 0.25f, 0f, 0f),
				new Keyframe(0.2f, 0f, 0f, 0f),
				new Keyframe(0.3f, 0.75f, 0f, 0f),
				new Keyframe(0.4f, 0f, 0f, 0f),
				new Keyframe(0.5f, 1f, 0f, 0f),
				new Keyframe(0.6f, 0f, 0f, 0f),
				new Keyframe(0.7f, 0.75f, 0f, 0f),
				new Keyframe(0.8f, 0f, 0f, 0f),
				new Keyframe(0.9f, 0.25f, 0f, 0f),
				new Keyframe(1f, 0f, 0f, 0f)
			});
		}

		internal static AnimationCurve GetStandardCurve(CinemachineImpulseDefinition.ImpulseShapes shape)
		{
			if (CinemachineImpulseDefinition.s_StandardShapes == null)
			{
				CinemachineImpulseDefinition.CreateStandardShapes();
			}
			return CinemachineImpulseDefinition.s_StandardShapes[(int)shape];
		}

		internal AnimationCurve ImpulseCurve
		{
			get
			{
				if (this.ImpulseShape == CinemachineImpulseDefinition.ImpulseShapes.Custom)
				{
					if (this.CustomImpulseShape == null)
					{
						this.CustomImpulseShape = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
					}
					return this.CustomImpulseShape;
				}
				return CinemachineImpulseDefinition.GetStandardCurve(this.ImpulseShape);
			}
		}

		public void CreateEvent(Vector3 position, Vector3 velocity)
		{
			this.CreateAndReturnEvent(position, velocity);
		}

		public CinemachineImpulseManager.ImpulseEvent CreateAndReturnEvent(Vector3 position, Vector3 velocity)
		{
			if (this.ImpulseType == CinemachineImpulseDefinition.ImpulseTypes.Legacy)
			{
				return this.LegacyCreateAndReturnEvent(position, velocity);
			}
			if ((this.ImpulseShape == CinemachineImpulseDefinition.ImpulseShapes.Custom && this.CustomImpulseShape == null) || Mathf.Abs(this.DissipationDistance) < 0.0001f || Mathf.Abs(this.ImpulseDuration) < 0.0001f)
			{
				return null;
			}
			CinemachineImpulseManager.ImpulseEvent impulseEvent = CinemachineImpulseManager.Instance.NewImpulseEvent();
			impulseEvent.Envelope = new CinemachineImpulseManager.EnvelopeDefinition
			{
				SustainTime = this.ImpulseDuration
			};
			impulseEvent.SignalSource = new CinemachineImpulseDefinition.SignalSource(this, velocity);
			impulseEvent.Position = position;
			impulseEvent.Radius = ((this.ImpulseType == CinemachineImpulseDefinition.ImpulseTypes.Uniform) ? 9999999f : 0f);
			impulseEvent.Channel = this.ImpulseChannel;
			impulseEvent.DirectionMode = CinemachineImpulseManager.ImpulseEvent.DirectionModes.Fixed;
			impulseEvent.DissipationDistance = ((this.ImpulseType == CinemachineImpulseDefinition.ImpulseTypes.Uniform) ? 0f : this.DissipationDistance);
			impulseEvent.PropagationSpeed = ((this.ImpulseType == CinemachineImpulseDefinition.ImpulseTypes.Propagating) ? this.PropagationSpeed : 9999999f);
			impulseEvent.CustomDissipation = this.DissipationRate;
			CinemachineImpulseManager.Instance.AddImpulseEvent(impulseEvent);
			return impulseEvent;
		}

		private CinemachineImpulseManager.ImpulseEvent LegacyCreateAndReturnEvent(Vector3 position, Vector3 velocity)
		{
			if (this.RawSignal == null || Mathf.Abs(this.TimeEnvelope.Duration) < 0.0001f)
			{
				return null;
			}
			CinemachineImpulseManager.ImpulseEvent impulseEvent = CinemachineImpulseManager.Instance.NewImpulseEvent();
			impulseEvent.Envelope = this.TimeEnvelope;
			impulseEvent.Envelope = this.TimeEnvelope;
			if (this.TimeEnvelope.ScaleWithImpact)
			{
				CinemachineImpulseManager.ImpulseEvent impulseEvent2 = impulseEvent;
				impulseEvent2.Envelope.DecayTime = impulseEvent2.Envelope.DecayTime * Mathf.Sqrt(velocity.magnitude);
			}
			impulseEvent.SignalSource = new CinemachineImpulseDefinition.LegacySignalSource(this, velocity);
			impulseEvent.Position = position;
			impulseEvent.Radius = this.ImpactRadius;
			impulseEvent.Channel = this.ImpulseChannel;
			impulseEvent.DirectionMode = this.DirectionMode;
			impulseEvent.DissipationMode = this.DissipationMode;
			impulseEvent.DissipationDistance = this.DissipationDistance;
			impulseEvent.PropagationSpeed = this.PropagationSpeed;
			CinemachineImpulseManager.Instance.AddImpulseEvent(impulseEvent);
			return impulseEvent;
		}

		[CinemachineImpulseChannelProperty]
		[Tooltip("Impulse events generated here will appear on the channels included in the mask.")]
		[FormerlySerializedAs("m_ImpulseChannel")]
		public int ImpulseChannel = 1;

		[Tooltip("Shape of the impact signal")]
		[FormerlySerializedAs("m_ImpulseShape")]
		public CinemachineImpulseDefinition.ImpulseShapes ImpulseShape;

		[Tooltip("Defines the custom shape of the impact signal that will be generated.")]
		[FormerlySerializedAs("m_CustomImpulseShape")]
		public AnimationCurve CustomImpulseShape = new AnimationCurve();

		[Tooltip("The time during which the impact signal will occur.  The signal shape will be stretched to fill that time.")]
		[FormerlySerializedAs("m_ImpulseDuration")]
		public float ImpulseDuration = 0.2f;

		[Tooltip("How the impulse travels through space and time.")]
		[FormerlySerializedAs("m_ImpulseType")]
		public CinemachineImpulseDefinition.ImpulseTypes ImpulseType = CinemachineImpulseDefinition.ImpulseTypes.Legacy;

		[Tooltip("This defines how the widely signal will spread within the effect radius before dissipating with distance from the impact point")]
		[Range(0f, 1f)]
		[FormerlySerializedAs("m_DissipationRate")]
		public float DissipationRate;

		[Header("Signal Shape")]
		[Tooltip("Legacy mode only: Defines the signal that will be generated.")]
		[CinemachineEmbeddedAssetProperty(true)]
		[FormerlySerializedAs("m_RawSignal")]
		public SignalSourceAsset RawSignal;

		[Tooltip("Legacy mode only: Gain to apply to the amplitudes defined in the signal source.  1 is normal.  Setting this to 0 completely mutes the signal.")]
		[FormerlySerializedAs("m_AmplitudeGain")]
		public float AmplitudeGain = 1f;

		[Tooltip("Legacy mode only: Scale factor to apply to the time axis.  1 is normal.  Larger magnitudes will make the signal progress more rapidly.")]
		[FormerlySerializedAs("m_FrequencyGain")]
		public float FrequencyGain = 1f;

		[Tooltip("Legacy mode only: How to fit the signal into the envelope time")]
		[FormerlySerializedAs("m_RepeatMode")]
		public CinemachineImpulseDefinition.RepeatModes RepeatMode;

		[Tooltip("Legacy mode only: Randomize the signal start time")]
		[FormerlySerializedAs("m_Randomize")]
		public bool Randomize = true;

		[Tooltip("Legacy mode only: This defines the time-envelope of the signal.  The raw signal will be time-scaled to fit in the envelope.")]
		[FormerlySerializedAs("m_TimeEnvelope")]
		public CinemachineImpulseManager.EnvelopeDefinition TimeEnvelope = CinemachineImpulseManager.EnvelopeDefinition.Default;

		[Header("Spatial Range")]
		[Tooltip("Legacy mode only: The signal will have full amplitude in this radius surrounding the impact point.  Beyond that it will dissipate with distance.")]
		[FormerlySerializedAs("m_ImpactRadius")]
		public float ImpactRadius = 100f;

		[Tooltip("Legacy mode only: How the signal direction behaves as the listener moves away from the origin.")]
		[FormerlySerializedAs("m_DirectionMode")]
		public CinemachineImpulseManager.ImpulseEvent.DirectionModes DirectionMode;

		[Tooltip("Legacy mode only: This defines how the signal will dissipate with distance beyond the impact radius.")]
		[FormerlySerializedAs("m_DissipationMode")]
		public CinemachineImpulseManager.ImpulseEvent.DissipationModes DissipationMode = CinemachineImpulseManager.ImpulseEvent.DissipationModes.ExponentialDecay;

		[Tooltip("The signal will have no effect outside this radius surrounding the impact point.")]
		[FormerlySerializedAs("m_DissipationDistance")]
		public float DissipationDistance = 100f;

		[Tooltip("The speed (m/s) at which the impulse propagates through space.  High speeds allow listeners to react instantaneously, while slower speeds allow listeners in the scene to react as if to a wave spreading from the source.")]
		[FormerlySerializedAs("m_PropagationSpeed")]
		public float PropagationSpeed = 343f;

		private static AnimationCurve[] s_StandardShapes;

		public enum ImpulseShapes
		{
			Custom,
			Recoil,
			Bump,
			Explosion,
			Rumble
		}

		public enum ImpulseTypes
		{
			Uniform,
			Dissipating,
			Propagating,
			Legacy
		}

		public enum RepeatModes
		{
			Stretch,
			Loop
		}

		private class SignalSource : ISignalSource6D
		{
			public SignalSource(CinemachineImpulseDefinition def, Vector3 velocity)
			{
				this.m_Def = def;
				this.m_Velocity = velocity;
			}

			public float SignalDuration
			{
				get
				{
					return this.m_Def.ImpulseDuration;
				}
			}

			public void GetSignal(float timeSinceSignalStart, out Vector3 pos, out Quaternion rot)
			{
				pos = this.m_Velocity * this.m_Def.ImpulseCurve.Evaluate(timeSinceSignalStart / this.SignalDuration);
				rot = Quaternion.identity;
			}

			private CinemachineImpulseDefinition m_Def;

			private Vector3 m_Velocity;
		}

		private class LegacySignalSource : ISignalSource6D
		{
			public LegacySignalSource(CinemachineImpulseDefinition def, Vector3 velocity)
			{
				this.m_Def = def;
				this.m_Velocity = velocity;
				if (this.m_Def.Randomize && this.m_Def.RawSignal.SignalDuration <= 0f)
				{
					this.m_StartTimeOffset = Random.Range(-1000f, 1000f);
				}
			}

			public float SignalDuration
			{
				get
				{
					return this.m_Def.RawSignal.SignalDuration;
				}
			}

			public void GetSignal(float timeSinceSignalStart, out Vector3 pos, out Quaternion rot)
			{
				float num = this.m_StartTimeOffset + timeSinceSignalStart * this.m_Def.FrequencyGain;
				float signalDuration = this.SignalDuration;
				if (signalDuration > 0f)
				{
					if (this.m_Def.RepeatMode == CinemachineImpulseDefinition.RepeatModes.Loop)
					{
						num %= signalDuration;
					}
					else if (this.m_Def.TimeEnvelope.Duration > 0.0001f)
					{
						num *= this.m_Def.TimeEnvelope.Duration / signalDuration;
					}
				}
				this.m_Def.RawSignal.GetSignal(num, out pos, out rot);
				float num2 = this.m_Velocity.magnitude;
				num2 *= this.m_Def.AmplitudeGain;
				pos *= num2;
				pos = Quaternion.FromToRotation(Vector3.down, this.m_Velocity) * pos;
				rot = Quaternion.SlerpUnclamped(Quaternion.identity, rot, num2);
			}

			private CinemachineImpulseDefinition m_Def;

			private Vector3 m_Velocity;

			private float m_StartTimeOffset;
		}
	}
}
