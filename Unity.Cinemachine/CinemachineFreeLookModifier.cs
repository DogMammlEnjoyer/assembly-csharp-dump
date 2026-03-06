using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Cinemachine
{
	[AddComponentMenu("Cinemachine/Procedural/Extensions/Cinemachine FreeLook Modifier")]
	[SaveDuringPlay]
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineFreeLookModifier.html")]
	public class CinemachineFreeLookModifier : CinemachineExtension
	{
		private void OnValidate()
		{
			CinemachineVirtualCameraBase componentOwner = base.ComponentOwner;
			for (int i = 0; i < this.Modifiers.Count; i++)
			{
				CinemachineFreeLookModifier.Modifier modifier = this.Modifiers[i];
				if (modifier != null)
				{
					modifier.Validate(componentOwner);
				}
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.RefreshComponentCache();
		}

		private static void TryGetVcamComponent<T>(CinemachineVirtualCameraBase vcam, out T component)
		{
			if (vcam == null || !vcam.TryGetComponent<T>(out component))
			{
				component = default(T);
			}
		}

		private void RefreshComponentCache()
		{
			CinemachineVirtualCameraBase componentOwner = base.ComponentOwner;
			CinemachineFreeLookModifier.TryGetVcamComponent<CinemachineFreeLookModifier.IModifierValueSource>(componentOwner, out this.m_ValueSource);
			for (int i = 0; i < this.Modifiers.Count; i++)
			{
				CinemachineFreeLookModifier.Modifier modifier = this.Modifiers[i];
				if (modifier != null)
				{
					modifier.RefreshCache(componentOwner);
				}
			}
		}

		internal bool HasValueSource()
		{
			this.RefreshComponentCache();
			return this.m_ValueSource != null;
		}

		public override void PrePipelineMutateCameraStateCallback(CinemachineVirtualCameraBase vcam, ref CameraState curState, float deltaTime)
		{
			if (this.m_ValueSource != null && vcam == base.ComponentOwner)
			{
				if (this.m_EasingCurve == null || this.m_CachedEasingValue != this.Easing)
				{
					if (this.m_EasingCurve == null)
					{
						this.m_EasingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
					}
					Keyframe[] keys = this.m_EasingCurve.keys;
					keys[0].outTangent = 1f - this.Easing;
					keys[1].inTangent = 1f + 2f * this.Easing;
					this.m_EasingCurve.keys = keys;
					this.m_CachedEasingValue = this.Easing;
				}
				float normalizedModifierValue = this.m_ValueSource.NormalizedModifierValue;
				float num = Mathf.Sign(normalizedModifierValue);
				this.m_CurrentValue = num * this.m_EasingCurve.Evaluate(Mathf.Abs(normalizedModifierValue));
				for (int i = 0; i < this.Modifiers.Count; i++)
				{
					CinemachineFreeLookModifier.Modifier modifier = this.Modifiers[i];
					if (modifier != null)
					{
						modifier.BeforePipeline(vcam, ref curState, deltaTime, this.m_CurrentValue);
					}
				}
			}
		}

		protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
		{
			if (this.m_ValueSource != null && stage == CinemachineCore.Stage.Finalize && vcam == base.ComponentOwner)
			{
				for (int i = 0; i < this.Modifiers.Count; i++)
				{
					CinemachineFreeLookModifier.Modifier modifier = this.Modifiers[i];
					if (modifier != null)
					{
						modifier.AfterPipeline(vcam, ref state, deltaTime, this.m_CurrentValue);
					}
				}
			}
		}

		[Tooltip("The amount of easing to apply towards the center value. Zero easing blends linearly through the center value, while an easing of 1 smooths the result as it passes over the center value.")]
		[Range(0f, 1f)]
		public float Easing;

		[Tooltip("These will modify settings as a function of the FreeLook's Vertical axis value")]
		[SerializeReference]
		public List<CinemachineFreeLookModifier.Modifier> Modifiers = new List<CinemachineFreeLookModifier.Modifier>();

		private CinemachineFreeLookModifier.IModifierValueSource m_ValueSource;

		private float m_CurrentValue;

		private AnimationCurve m_EasingCurve;

		private float m_CachedEasingValue;

		public interface IModifierValueSource
		{
			float NormalizedModifierValue { get; }
		}

		public interface IModifiablePositionDamping
		{
			Vector3 PositionDamping { get; set; }
		}

		public interface IModifiableComposition
		{
			ScreenComposerSettings Composition { get; set; }
		}

		public interface IModifiableDistance
		{
			float Distance { get; set; }
		}

		public interface IModifiableNoise
		{
			ValueTuple<float, float> NoiseAmplitudeFrequency { get; set; }
		}

		[Serializable]
		public abstract class Modifier
		{
			public virtual void Validate(CinemachineVirtualCameraBase vcam)
			{
			}

			public virtual void Reset(CinemachineVirtualCameraBase vcam)
			{
			}

			public virtual Type CachedComponentType
			{
				get
				{
					return null;
				}
			}

			public virtual bool HasRequiredComponent
			{
				get
				{
					return true;
				}
			}

			public virtual void RefreshCache(CinemachineVirtualCameraBase vcam)
			{
			}

			public virtual void BeforePipeline(CinemachineVirtualCameraBase vcam, ref CameraState state, float deltaTime, float modifierValue)
			{
			}

			public virtual void AfterPipeline(CinemachineVirtualCameraBase vcam, ref CameraState state, float deltaTime, float modifierValue)
			{
			}
		}

		public abstract class ComponentModifier<T> : CinemachineFreeLookModifier.Modifier
		{
			public override bool HasRequiredComponent
			{
				get
				{
					return this.CachedComponent != null;
				}
			}

			public override Type CachedComponentType
			{
				get
				{
					return typeof(T);
				}
			}

			public override void RefreshCache(CinemachineVirtualCameraBase vcam)
			{
				CinemachineFreeLookModifier.TryGetVcamComponent<T>(vcam, out this.CachedComponent);
			}

			protected T CachedComponent;
		}

		public class TiltModifier : CinemachineFreeLookModifier.Modifier
		{
			public override void Validate(CinemachineVirtualCameraBase vcam)
			{
				this.Tilt.Top = Mathf.Clamp(this.Tilt.Top, -30f, 30f);
				this.Tilt.Bottom = Mathf.Clamp(this.Tilt.Bottom, -30f, 30f);
			}

			public override void Reset(CinemachineVirtualCameraBase vcam)
			{
				this.Tilt = new CinemachineFreeLookModifier.TopBottomRigs<float>
				{
					Top = -5f,
					Bottom = 5f
				};
			}

			public override void AfterPipeline(CinemachineVirtualCameraBase vcam, ref CameraState state, float deltaTime, float modifierValue)
			{
				float angle = (modifierValue > 0f) ? Mathf.Lerp(0f, this.Tilt.Top, modifierValue) : Mathf.Lerp(this.Tilt.Bottom, 0f, modifierValue + 1f);
				Quaternion rhs = state.RawOrientation * Quaternion.AngleAxis(angle, Vector3.right);
				state.OrientationCorrection = Quaternion.Inverse(state.GetCorrectedOrientation()) * rhs;
			}

			[HideFoldout]
			public CinemachineFreeLookModifier.TopBottomRigs<float> Tilt;
		}

		public class LensModifier : CinemachineFreeLookModifier.Modifier
		{
			public override void Validate(CinemachineVirtualCameraBase vcam)
			{
				this.Top.Validate();
				this.Bottom.Validate();
			}

			public override void Reset(CinemachineVirtualCameraBase vcam)
			{
				if (vcam == null)
				{
					this.Top = (this.Bottom = LensSettings.Default);
					return;
				}
				CameraState state = vcam.State;
				this.Top = (this.Bottom = state.Lens);
				this.Top.CopyCameraMode(ref state.Lens);
				this.Bottom.CopyCameraMode(ref state.Lens);
			}

			public override void BeforePipeline(CinemachineVirtualCameraBase vcam, ref CameraState state, float deltaTime, float modifierValue)
			{
				this.Top.CopyCameraMode(ref state.Lens);
				this.Bottom.CopyCameraMode(ref state.Lens);
				if (modifierValue >= 0f)
				{
					state.Lens.Lerp(this.Top, modifierValue);
					return;
				}
				state.Lens.Lerp(this.Bottom, -modifierValue);
			}

			[Tooltip("Value to take at the top of the axis range")]
			[LensSettingsHideModeOverrideProperty]
			public LensSettings Top;

			[Tooltip("Value to take at the bottom of the axis range")]
			[LensSettingsHideModeOverrideProperty]
			public LensSettings Bottom;
		}

		public class PositionDampingModifier : CinemachineFreeLookModifier.ComponentModifier<CinemachineFreeLookModifier.IModifiablePositionDamping>
		{
			public override void Validate(CinemachineVirtualCameraBase vcam)
			{
				this.Damping.Top = new Vector3(Mathf.Max(0f, this.Damping.Top.x), Mathf.Max(0f, this.Damping.Top.y), Mathf.Max(0f, this.Damping.Top.z));
				this.Damping.Bottom = new Vector3(Mathf.Max(0f, this.Damping.Bottom.x), Mathf.Max(0f, this.Damping.Bottom.y), Mathf.Max(0f, this.Damping.Bottom.z));
			}

			public override void Reset(CinemachineVirtualCameraBase vcam)
			{
				if (this.CachedComponent != null)
				{
					this.Damping.Top = (this.Damping.Bottom = this.CachedComponent.PositionDamping);
				}
			}

			public override void BeforePipeline(CinemachineVirtualCameraBase vcam, ref CameraState state, float deltaTime, float modifierValue)
			{
				if (this.CachedComponent != null)
				{
					this.m_CenterDamping = this.CachedComponent.PositionDamping;
					this.CachedComponent.PositionDamping = ((modifierValue >= 0f) ? Vector3.Lerp(this.m_CenterDamping, this.Damping.Top, modifierValue) : Vector3.Lerp(this.Damping.Bottom, this.m_CenterDamping, modifierValue + 1f));
				}
			}

			public override void AfterPipeline(CinemachineVirtualCameraBase vcam, ref CameraState state, float deltaTime, float modifierValue)
			{
				if (this.CachedComponent != null)
				{
					this.CachedComponent.PositionDamping = this.m_CenterDamping;
				}
			}

			[HideFoldout]
			public CinemachineFreeLookModifier.TopBottomRigs<Vector3> Damping;

			private Vector3 m_CenterDamping;
		}

		public class CompositionModifier : CinemachineFreeLookModifier.ComponentModifier<CinemachineFreeLookModifier.IModifiableComposition>
		{
			public override void Validate(CinemachineVirtualCameraBase vcam)
			{
				this.Composition.Top.Validate();
				this.Composition.Bottom.Validate();
			}

			public override void Reset(CinemachineVirtualCameraBase vcam)
			{
				if (this.CachedComponent != null)
				{
					this.Composition.Top = (this.Composition.Bottom = this.CachedComponent.Composition);
				}
			}

			public override void BeforePipeline(CinemachineVirtualCameraBase vcam, ref CameraState state, float deltaTime, float modifierValue)
			{
				if (this.CachedComponent != null)
				{
					this.m_SavedComposition = this.CachedComponent.Composition;
					this.CachedComponent.Composition = ((modifierValue >= 0f) ? ScreenComposerSettings.Lerp(this.m_SavedComposition, this.Composition.Top, modifierValue) : ScreenComposerSettings.Lerp(this.Composition.Bottom, this.m_SavedComposition, modifierValue + 1f));
				}
			}

			public override void AfterPipeline(CinemachineVirtualCameraBase vcam, ref CameraState state, float deltaTime, float modifierValue)
			{
				if (this.CachedComponent != null)
				{
					this.CachedComponent.Composition = this.m_SavedComposition;
				}
			}

			[HideFoldout]
			public CinemachineFreeLookModifier.TopBottomRigs<ScreenComposerSettings> Composition;

			private ScreenComposerSettings m_SavedComposition;
		}

		public class DistanceModifier : CinemachineFreeLookModifier.ComponentModifier<CinemachineFreeLookModifier.IModifiableDistance>
		{
			public override void Validate(CinemachineVirtualCameraBase vcam)
			{
				this.Distance.Top = Mathf.Max(0f, this.Distance.Top);
				this.Distance.Bottom = Mathf.Max(0f, this.Distance.Bottom);
			}

			public override void Reset(CinemachineVirtualCameraBase vcam)
			{
				if (this.CachedComponent != null)
				{
					this.Distance.Top = (this.Distance.Bottom = this.CachedComponent.Distance);
				}
			}

			public override void BeforePipeline(CinemachineVirtualCameraBase vcam, ref CameraState state, float deltaTime, float modifierValue)
			{
				if (this.CachedComponent != null)
				{
					this.m_CenterDistance = this.CachedComponent.Distance;
					this.CachedComponent.Distance = ((modifierValue >= 0f) ? Mathf.Lerp(this.m_CenterDistance, this.Distance.Top, modifierValue) : Mathf.Lerp(this.Distance.Bottom, this.m_CenterDistance, modifierValue + 1f));
				}
			}

			public override void AfterPipeline(CinemachineVirtualCameraBase vcam, ref CameraState state, float deltaTime, float modifierValue)
			{
				if (this.CachedComponent != null)
				{
					this.CachedComponent.Distance = this.m_CenterDistance;
				}
			}

			[HideFoldout]
			public CinemachineFreeLookModifier.TopBottomRigs<float> Distance;

			private float m_CenterDistance;
		}

		public class NoiseModifier : CinemachineFreeLookModifier.ComponentModifier<CinemachineFreeLookModifier.IModifiableNoise>
		{
			public override void Reset(CinemachineVirtualCameraBase vcam)
			{
				if (this.CachedComponent != null)
				{
					ValueTuple<float, float> noiseAmplitudeFrequency = this.CachedComponent.NoiseAmplitudeFrequency;
					this.Noise.Top = (this.Noise.Bottom = new CinemachineFreeLookModifier.NoiseModifier.NoiseSettings
					{
						Amplitude = noiseAmplitudeFrequency.Item1,
						Frequency = noiseAmplitudeFrequency.Item2
					});
				}
			}

			public override void BeforePipeline(CinemachineVirtualCameraBase vcam, ref CameraState state, float deltaTime, float modifierValue)
			{
				if (this.CachedComponent != null)
				{
					this.m_CenterNoise = this.CachedComponent.NoiseAmplitudeFrequency;
					if (modifierValue >= 0f)
					{
						this.CachedComponent.NoiseAmplitudeFrequency = new ValueTuple<float, float>(Mathf.Lerp(this.m_CenterNoise.Item1, this.Noise.Top.Amplitude, modifierValue), Mathf.Lerp(this.m_CenterNoise.Item2, this.Noise.Top.Frequency, modifierValue));
						return;
					}
					this.CachedComponent.NoiseAmplitudeFrequency = new ValueTuple<float, float>(Mathf.Lerp(this.Noise.Bottom.Amplitude, this.m_CenterNoise.Item1, modifierValue + 1f), Mathf.Lerp(this.Noise.Bottom.Frequency, this.m_CenterNoise.Item2, modifierValue + 1f));
				}
			}

			public override void AfterPipeline(CinemachineVirtualCameraBase vcam, ref CameraState state, float deltaTime, float modifierValue)
			{
				if (this.CachedComponent != null)
				{
					this.CachedComponent.NoiseAmplitudeFrequency = this.m_CenterNoise;
				}
			}

			[HideFoldout]
			public CinemachineFreeLookModifier.TopBottomRigs<CinemachineFreeLookModifier.NoiseModifier.NoiseSettings> Noise;

			private ValueTuple<float, float> m_CenterNoise;

			[Serializable]
			public struct NoiseSettings
			{
				[Tooltip("Multiplier for the noise amplitude")]
				public float Amplitude;

				[Tooltip("Multiplier for the noise frequency")]
				public float Frequency;
			}
		}

		[Serializable]
		public struct TopBottomRigs<T>
		{
			[Tooltip("Value to take at the top of the axis range")]
			public T Top;

			[Tooltip("Value to take at the bottom of the axis range")]
			public T Bottom;
		}
	}
}
