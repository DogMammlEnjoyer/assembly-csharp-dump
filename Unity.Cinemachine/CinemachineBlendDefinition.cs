using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[Serializable]
	public struct CinemachineBlendDefinition
	{
		public float BlendTime
		{
			get
			{
				if (this.Style != CinemachineBlendDefinition.Styles.Cut)
				{
					return this.Time;
				}
				return 0f;
			}
		}

		public CinemachineBlendDefinition(CinemachineBlendDefinition.Styles style, float time)
		{
			this.Style = style;
			this.Time = time;
			this.CustomCurve = null;
		}

		private void CreateStandardCurves()
		{
			CinemachineBlendDefinition.s_StandardCurves = new AnimationCurve[7];
			CinemachineBlendDefinition.s_StandardCurves[0] = null;
			CinemachineBlendDefinition.s_StandardCurves[1] = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
			CinemachineBlendDefinition.s_StandardCurves[2] = AnimationCurve.Linear(0f, 0f, 1f, 1f);
			Keyframe[] keys = CinemachineBlendDefinition.s_StandardCurves[2].keys;
			keys[0].outTangent = 1.4f;
			keys[1].inTangent = 0f;
			CinemachineBlendDefinition.s_StandardCurves[2].keys = keys;
			CinemachineBlendDefinition.s_StandardCurves[3] = AnimationCurve.Linear(0f, 0f, 1f, 1f);
			keys = CinemachineBlendDefinition.s_StandardCurves[3].keys;
			keys[0].outTangent = 0f;
			keys[1].inTangent = 1.4f;
			CinemachineBlendDefinition.s_StandardCurves[3].keys = keys;
			CinemachineBlendDefinition.s_StandardCurves[4] = AnimationCurve.Linear(0f, 0f, 1f, 1f);
			keys = CinemachineBlendDefinition.s_StandardCurves[4].keys;
			keys[0].outTangent = 0f;
			keys[1].inTangent = 3f;
			CinemachineBlendDefinition.s_StandardCurves[4].keys = keys;
			CinemachineBlendDefinition.s_StandardCurves[5] = AnimationCurve.Linear(0f, 0f, 1f, 1f);
			keys = CinemachineBlendDefinition.s_StandardCurves[5].keys;
			keys[0].outTangent = 3f;
			keys[1].inTangent = 0f;
			CinemachineBlendDefinition.s_StandardCurves[5].keys = keys;
			CinemachineBlendDefinition.s_StandardCurves[6] = AnimationCurve.Linear(0f, 0f, 1f, 1f);
		}

		public AnimationCurve BlendCurve
		{
			get
			{
				if (this.Style == CinemachineBlendDefinition.Styles.Custom)
				{
					if (this.CustomCurve == null)
					{
						this.CustomCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
					}
					return this.CustomCurve;
				}
				if (CinemachineBlendDefinition.s_StandardCurves == null)
				{
					this.CreateStandardCurves();
				}
				return CinemachineBlendDefinition.s_StandardCurves[(int)this.Style];
			}
		}

		[Tooltip("Shape of the blend curve")]
		[FormerlySerializedAs("m_Style")]
		public CinemachineBlendDefinition.Styles Style;

		[Tooltip("Duration of the blend, in seconds")]
		[FormerlySerializedAs("m_Time")]
		public float Time;

		[FormerlySerializedAs("m_CustomCurve")]
		public AnimationCurve CustomCurve;

		private static AnimationCurve[] s_StandardCurves;

		public delegate CinemachineBlendDefinition LookupBlendDelegate(ICinemachineCamera outgoing, ICinemachineCamera incoming);

		public enum Styles
		{
			Cut,
			EaseInOut,
			EaseIn,
			EaseOut,
			HardIn,
			HardOut,
			Linear,
			Custom
		}
	}
}
