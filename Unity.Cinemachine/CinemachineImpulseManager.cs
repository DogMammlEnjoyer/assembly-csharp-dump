using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	public class CinemachineImpulseManager
	{
		private CinemachineImpulseManager()
		{
		}

		public static CinemachineImpulseManager Instance
		{
			get
			{
				if (CinemachineImpulseManager.s_Instance == null)
				{
					CinemachineImpulseManager.s_Instance = new CinemachineImpulseManager();
				}
				return CinemachineImpulseManager.s_Instance;
			}
		}

		[RuntimeInitializeOnLoadMethod]
		private static void InitializeModule()
		{
			if (CinemachineImpulseManager.s_Instance != null)
			{
				CinemachineImpulseManager.s_Instance.Clear();
			}
		}

		internal static float EvaluateDissipationScale(float spread, float normalizedDistance)
		{
			float num = -0.8f + 1.6f * (1f - spread);
			num = (1f - num) * 0.5f;
			float t = Mathf.Clamp01(normalizedDistance) / ((1f / Mathf.Clamp01(num) - 2f) * (1f - normalizedDistance) + 1f);
			return 1f - SplineHelpers.Bezier1(t, 0f, 0f, 1f, 1f);
		}

		public bool GetImpulseAt(Vector3 listenerLocation, bool distance2D, int channelMask, out Vector3 pos, out Quaternion rot)
		{
			bool result = false;
			pos = Vector3.zero;
			rot = Quaternion.identity;
			if (this.m_ActiveEvents != null)
			{
				for (int i = this.m_ActiveEvents.Count - 1; i >= 0; i--)
				{
					CinemachineImpulseManager.ImpulseEvent impulseEvent = this.m_ActiveEvents[i];
					Vector3 b;
					Quaternion rhs;
					if (impulseEvent == null || impulseEvent.Expired)
					{
						this.m_ActiveEvents.RemoveAt(i);
						if (impulseEvent != null)
						{
							if (this.m_ExpiredEvents == null)
							{
								this.m_ExpiredEvents = new List<CinemachineImpulseManager.ImpulseEvent>();
							}
							impulseEvent.Clear();
							this.m_ExpiredEvents.Add(impulseEvent);
						}
					}
					else if ((impulseEvent.Channel & channelMask) != 0 && impulseEvent.GetDecayedSignal(listenerLocation, distance2D, out b, out rhs))
					{
						result = true;
						pos += b;
						rot *= rhs;
					}
				}
			}
			return result;
		}

		public bool GetStrongestImpulseAt(Vector3 listenerLocation, bool distance2D, int channelMask, out Vector3 pos, out Quaternion rot)
		{
			bool result = false;
			Vector3 vector = Vector3.zero;
			Quaternion quaternion = Quaternion.identity;
			if (this.m_ActiveEvents != null)
			{
				float num = 0f;
				for (int i = this.m_ActiveEvents.Count - 1; i >= 0; i--)
				{
					CinemachineImpulseManager.ImpulseEvent impulseEvent = this.m_ActiveEvents[i];
					Vector3 vector2;
					Quaternion quaternion2;
					if (impulseEvent == null || impulseEvent.Expired)
					{
						this.m_ActiveEvents.RemoveAt(i);
						if (impulseEvent != null)
						{
							if (this.m_ExpiredEvents == null)
							{
								this.m_ExpiredEvents = new List<CinemachineImpulseManager.ImpulseEvent>();
							}
							impulseEvent.Clear();
							this.m_ExpiredEvents.Add(impulseEvent);
						}
					}
					else if ((impulseEvent.Channel & channelMask) != 0 && impulseEvent.GetDecayedSignal(listenerLocation, distance2D, out vector2, out quaternion2))
					{
						result = true;
						float sqrMagnitude = vector2.sqrMagnitude;
						if (sqrMagnitude > num)
						{
							num = sqrMagnitude;
							vector = vector2;
							quaternion = quaternion2;
						}
					}
				}
			}
			pos = vector;
			rot = quaternion;
			return result;
		}

		public float CurrentTime
		{
			get
			{
				if (!this.IgnoreTimeScale)
				{
					return CinemachineCore.CurrentTime;
				}
				return Time.realtimeSinceStartup;
			}
		}

		public CinemachineImpulseManager.ImpulseEvent NewImpulseEvent()
		{
			if (this.m_ExpiredEvents == null || this.m_ExpiredEvents.Count == 0)
			{
				return new CinemachineImpulseManager.ImpulseEvent
				{
					CustomDissipation = -1f
				};
			}
			CinemachineImpulseManager.ImpulseEvent result = this.m_ExpiredEvents[this.m_ExpiredEvents.Count - 1];
			this.m_ExpiredEvents.RemoveAt(this.m_ExpiredEvents.Count - 1);
			return result;
		}

		public void AddImpulseEvent(CinemachineImpulseManager.ImpulseEvent e)
		{
			if (this.m_ActiveEvents == null)
			{
				this.m_ActiveEvents = new List<CinemachineImpulseManager.ImpulseEvent>();
			}
			if (e != null)
			{
				e.StartTime = this.CurrentTime;
				this.m_ActiveEvents.Add(e);
			}
		}

		public void Clear()
		{
			if (this.m_ActiveEvents != null)
			{
				for (int i = 0; i < this.m_ActiveEvents.Count; i++)
				{
					this.m_ActiveEvents[i].Clear();
				}
				this.m_ActiveEvents.Clear();
			}
		}

		private static CinemachineImpulseManager s_Instance;

		private const float Epsilon = 0.0001f;

		private List<CinemachineImpulseManager.ImpulseEvent> m_ExpiredEvents;

		private List<CinemachineImpulseManager.ImpulseEvent> m_ActiveEvents;

		public bool IgnoreTimeScale;

		[Serializable]
		public struct EnvelopeDefinition
		{
			public static CinemachineImpulseManager.EnvelopeDefinition Default
			{
				get
				{
					return new CinemachineImpulseManager.EnvelopeDefinition
					{
						DecayTime = 0.7f,
						SustainTime = 0.2f,
						ScaleWithImpact = true
					};
				}
			}

			public readonly float Duration
			{
				get
				{
					if (!this.HoldForever)
					{
						return this.AttackTime + this.SustainTime + this.DecayTime;
					}
					return -1f;
				}
			}

			public readonly float GetValueAt(float offset)
			{
				if (offset >= 0f)
				{
					if (offset < this.AttackTime && this.AttackTime > 0.0001f)
					{
						if (this.AttackShape == null || this.AttackShape.length < 2)
						{
							return Damper.Damp(1f, this.AttackTime, offset);
						}
						return this.AttackShape.Evaluate(offset / this.AttackTime);
					}
					else
					{
						offset -= this.AttackTime;
						if (this.HoldForever || offset < this.SustainTime)
						{
							return 1f;
						}
						offset -= this.SustainTime;
						if (offset < this.DecayTime && this.DecayTime > 0.0001f)
						{
							if (this.DecayShape == null || this.DecayShape.length < 2)
							{
								return 1f - Damper.Damp(1f, this.DecayTime, offset);
							}
							return this.DecayShape.Evaluate(offset / this.DecayTime);
						}
					}
				}
				return 0f;
			}

			public void ChangeStopTime(float offset, bool forceNoDecay)
			{
				if (offset < 0f)
				{
					offset = 0f;
				}
				if (offset < this.AttackTime)
				{
					this.AttackTime = 0f;
				}
				this.SustainTime = offset - this.AttackTime;
				if (forceNoDecay)
				{
					this.DecayTime = 0f;
				}
			}

			public void Clear()
			{
				this.AttackShape = (this.DecayShape = null);
				this.AttackTime = (this.SustainTime = (this.DecayTime = 0f));
			}

			public void Validate()
			{
				this.AttackTime = Mathf.Max(0f, this.AttackTime);
				this.DecayTime = Mathf.Max(0f, this.DecayTime);
				this.SustainTime = Mathf.Max(0f, this.SustainTime);
			}

			[Tooltip("Normalized curve defining the shape of the start of the envelope.  If blank a default curve will be used")]
			[FormerlySerializedAs("m_AttackShape")]
			public AnimationCurve AttackShape;

			[Tooltip("Normalized curve defining the shape of the end of the envelope.  If blank a default curve will be used")]
			[FormerlySerializedAs("m_DecayShape")]
			public AnimationCurve DecayShape;

			[Tooltip("Duration in seconds of the attack.  Attack curve will be scaled to fit.  Must be >= 0.")]
			[FormerlySerializedAs("m_AttackTime")]
			public float AttackTime;

			[Tooltip("Duration in seconds of the central fully-scaled part of the envelope.  Must be >= 0.")]
			[FormerlySerializedAs("m_SustainTime")]
			public float SustainTime;

			[Tooltip("Duration in seconds of the decay.  Decay curve will be scaled to fit.  Must be >= 0.")]
			[FormerlySerializedAs("m_DecayTime")]
			public float DecayTime;

			[Tooltip("If checked, signal amplitude scaling will also be applied to the time envelope of the signal.  Stronger signals will last longer.")]
			[FormerlySerializedAs("m_ScaleWithImpact")]
			public bool ScaleWithImpact;

			[Tooltip("If true, then duration is infinite.")]
			[FormerlySerializedAs("m_HoldForever")]
			public bool HoldForever;
		}

		public class ImpulseEvent
		{
			public bool Expired
			{
				get
				{
					float duration = this.Envelope.Duration;
					float num = this.Radius + this.DissipationDistance;
					float num2 = CinemachineImpulseManager.Instance.CurrentTime - num / Mathf.Max(1f, this.PropagationSpeed);
					return duration > 0f && this.StartTime + duration <= num2;
				}
			}

			public void Cancel(float time, bool forceNoDecay)
			{
				this.Envelope.HoldForever = false;
				this.Envelope.ChangeStopTime(time - this.StartTime, forceNoDecay);
			}

			public float DistanceDecay(float distance)
			{
				float num = Mathf.Max(this.Radius, 0f);
				if (distance < num)
				{
					return 1f;
				}
				distance -= num;
				if (distance >= this.DissipationDistance)
				{
					return 0f;
				}
				if (this.CustomDissipation >= 0f)
				{
					return CinemachineImpulseManager.EvaluateDissipationScale(this.CustomDissipation, distance / this.DissipationDistance);
				}
				switch (this.DissipationMode)
				{
				default:
					return Mathf.Lerp(1f, 0f, distance / this.DissipationDistance);
				case CinemachineImpulseManager.ImpulseEvent.DissipationModes.SoftDecay:
					return 0.5f * (1f + Mathf.Cos(3.1415927f * (distance / this.DissipationDistance)));
				case CinemachineImpulseManager.ImpulseEvent.DissipationModes.ExponentialDecay:
					return 1f - Damper.Damp(1f, this.DissipationDistance, distance);
				}
			}

			public bool GetDecayedSignal(Vector3 listenerPosition, bool use2D, out Vector3 pos, out Quaternion rot)
			{
				if (this.SignalSource != null)
				{
					float num = use2D ? Vector2.Distance(listenerPosition, this.Position) : Vector3.Distance(listenerPosition, this.Position);
					float num2 = CinemachineImpulseManager.Instance.CurrentTime - this.StartTime - num / Mathf.Max(1f, this.PropagationSpeed);
					float num3 = this.Envelope.GetValueAt(num2) * this.DistanceDecay(num);
					if (num3 != 0f)
					{
						this.SignalSource.GetSignal(num2, out pos, out rot);
						pos *= num3;
						rot = Quaternion.SlerpUnclamped(Quaternion.identity, rot, num3);
						if (this.DirectionMode == CinemachineImpulseManager.ImpulseEvent.DirectionModes.RotateTowardSource && num > 0.0001f)
						{
							Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, listenerPosition - this.Position);
							if (this.Radius > 0.0001f)
							{
								float num4 = Mathf.Clamp01(num / this.Radius);
								quaternion = Quaternion.Slerp(quaternion, Quaternion.identity, Mathf.Cos(3.1415927f * num4 / 2f));
							}
							pos = quaternion * pos;
						}
						return true;
					}
				}
				pos = Vector3.zero;
				rot = Quaternion.identity;
				return false;
			}

			public void Clear()
			{
				this.Envelope.Clear();
				this.StartTime = 0f;
				this.SignalSource = null;
				this.Position = Vector3.zero;
				this.Channel = 0;
				this.Radius = 0f;
				this.DissipationDistance = 100f;
				this.DissipationMode = CinemachineImpulseManager.ImpulseEvent.DissipationModes.ExponentialDecay;
				this.CustomDissipation = -1f;
			}

			internal ImpulseEvent()
			{
			}

			public float StartTime;

			public CinemachineImpulseManager.EnvelopeDefinition Envelope;

			public ISignalSource6D SignalSource;

			public Vector3 Position;

			public float Radius;

			public CinemachineImpulseManager.ImpulseEvent.DirectionModes DirectionMode;

			public int Channel;

			public CinemachineImpulseManager.ImpulseEvent.DissipationModes DissipationMode;

			public float DissipationDistance;

			public float CustomDissipation;

			public float PropagationSpeed;

			public enum DirectionModes
			{
				Fixed,
				RotateTowardSource
			}

			public enum DissipationModes
			{
				LinearDecay,
				SoftDecay,
				ExponentialDecay
			}
		}
	}
}
