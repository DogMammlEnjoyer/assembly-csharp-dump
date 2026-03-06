using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Cinemachine.TargetTracking;
using UnityEngine;

namespace Unity.Cinemachine
{
	[AddComponentMenu("Cinemachine/Procedural/Position Control/Cinemachine Orbital Follow")]
	[SaveDuringPlay]
	[DisallowMultipleComponent]
	[CameraPipeline(CinemachineCore.Stage.Body)]
	[RequiredTarget(RequiredTargetAttribute.RequiredTargets.Tracking)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineOrbitalFollow.html")]
	public class CinemachineOrbitalFollow : CinemachineComponentBase, IInputAxisOwner, IInputAxisResetSource, CinemachineFreeLookModifier.IModifierValueSource, CinemachineFreeLookModifier.IModifiablePositionDamping, CinemachineFreeLookModifier.IModifiableDistance
	{
		internal Vector3 TrackedPoint { get; private set; }

		private void OnValidate()
		{
			this.Radius = Mathf.Max(0f, this.Radius);
			this.TrackerSettings.Validate();
			this.HorizontalAxis.Validate();
			this.VerticalAxis.Validate();
			this.RadialAxis.Validate();
			this.RadialAxis.Range.x = Mathf.Max(this.RadialAxis.Range.x, 0.0001f);
			this.HorizontalAxis.Restrictions = (this.HorizontalAxis.Restrictions & ~(InputAxis.RestrictionFlags.RangeIsDriven | InputAxis.RestrictionFlags.NoRecentering));
		}

		private void Reset()
		{
			this.TargetOffset = Vector3.zero;
			this.TrackerSettings = TrackerSettings.Default;
			this.OrbitStyle = CinemachineOrbitalFollow.OrbitStyles.Sphere;
			this.Radius = 10f;
			this.Orbits = Cinemachine3OrbitRig.Settings.Default;
			this.HorizontalAxis = CinemachineOrbitalFollow.DefaultHorizontal;
			this.VerticalAxis = CinemachineOrbitalFollow.DefaultVertical;
			this.RadialAxis = CinemachineOrbitalFollow.DefaultRadial;
		}

		private static InputAxis DefaultHorizontal
		{
			get
			{
				return new InputAxis
				{
					Value = 0f,
					Range = new Vector2(-180f, 180f),
					Wrap = true,
					Center = 0f,
					Recentering = InputAxis.RecenteringSettings.Default
				};
			}
		}

		private static InputAxis DefaultVertical
		{
			get
			{
				return new InputAxis
				{
					Value = 17.5f,
					Range = new Vector2(-10f, 45f),
					Wrap = false,
					Center = 17.5f,
					Recentering = InputAxis.RecenteringSettings.Default
				};
			}
		}

		private static InputAxis DefaultRadial
		{
			get
			{
				return new InputAxis
				{
					Value = 1f,
					Range = new Vector2(1f, 1f),
					Wrap = false,
					Center = 1f,
					Recentering = InputAxis.RecenteringSettings.Default
				};
			}
		}

		public override bool IsValid
		{
			get
			{
				return base.enabled && base.FollowTarget != null;
			}
		}

		public override CinemachineCore.Stage Stage
		{
			get
			{
				return CinemachineCore.Stage.Body;
			}
		}

		public override float GetMaxDampTime()
		{
			return this.TrackerSettings.GetMaxDampTime();
		}

		void IInputAxisOwner.GetInputAxes(List<IInputAxisOwner.AxisDescriptor> axes)
		{
			axes.Add(new IInputAxisOwner.AxisDescriptor
			{
				DrivenAxis = (() => ref this.HorizontalAxis),
				Name = "Look Orbit X",
				Hint = IInputAxisOwner.AxisDescriptor.Hints.X
			});
			axes.Add(new IInputAxisOwner.AxisDescriptor
			{
				DrivenAxis = (() => ref this.VerticalAxis),
				Name = "Look Orbit Y",
				Hint = IInputAxisOwner.AxisDescriptor.Hints.Y
			});
			axes.Add(new IInputAxisOwner.AxisDescriptor
			{
				DrivenAxis = (() => ref this.RadialAxis),
				Name = "Orbit Scale",
				Hint = IInputAxisOwner.AxisDescriptor.Hints.Y
			});
		}

		void IInputAxisResetSource.RegisterResetHandler(Action handler)
		{
			this.m_ResetHandler = (Action)Delegate.Combine(this.m_ResetHandler, handler);
		}

		void IInputAxisResetSource.UnregisterResetHandler(Action handler)
		{
			this.m_ResetHandler = (Action)Delegate.Remove(this.m_ResetHandler, handler);
		}

		bool IInputAxisResetSource.HasResetHandler
		{
			get
			{
				return this.m_ResetHandler != null;
			}
		}

		float CinemachineFreeLookModifier.IModifierValueSource.NormalizedModifierValue
		{
			get
			{
				return this.GetCameraPoint().w / Mathf.Max(0.0001f, this.RadialAxis.Value);
			}
		}

		Vector3 CinemachineFreeLookModifier.IModifiablePositionDamping.PositionDamping
		{
			get
			{
				return this.TrackerSettings.PositionDamping;
			}
			set
			{
				this.TrackerSettings.PositionDamping = value;
			}
		}

		float CinemachineFreeLookModifier.IModifiableDistance.Distance
		{
			get
			{
				return this.Radius;
			}
			set
			{
				this.Radius = value;
			}
		}

		internal Vector3 GetCameraOffsetForNormalizedAxisValue(float t)
		{
			return this.m_OrbitCache.SplineValue(Mathf.Clamp01((t + 1f) * 0.5f));
		}

		private Vector4 GetCameraPoint()
		{
			Vector3 vector2;
			float w;
			if (this.OrbitStyle == CinemachineOrbitalFollow.OrbitStyles.ThreeRing)
			{
				if (this.m_OrbitCache.SettingsChanged(this.Orbits))
				{
					this.m_OrbitCache.UpdateOrbitCache(this.Orbits);
				}
				Vector4 vector = this.m_OrbitCache.SplineValue(this.VerticalAxis.GetNormalizedValue());
				vector *= this.RadialAxis.Value;
				vector2 = Quaternion.AngleAxis(this.HorizontalAxis.Value, Vector3.up) * vector;
				w = vector.w;
			}
			else
			{
				vector2 = Quaternion.Euler(this.VerticalAxis.Value, this.HorizontalAxis.Value, 0f) * new Vector3(0f, 0f, -this.Radius * this.RadialAxis.Value);
				w = this.VerticalAxis.GetNormalizedValue() * 2f - 1f;
			}
			if (this.TrackerSettings.BindingMode == BindingMode.LazyFollow)
			{
				vector2.z = -Mathf.Abs(vector2.z);
			}
			return new Vector4(vector2.x, vector2.y, vector2.z, w);
		}

		public override bool OnTransitionFromCamera(ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime)
		{
			Action resetHandler = this.m_ResetHandler;
			if (resetHandler != null)
			{
				resetHandler();
			}
			if (fromCam != null && (base.VirtualCamera.State.BlendHint & CameraState.BlendHints.InheritPosition) != CameraState.BlendHints.Nothing && !CinemachineCore.IsLiveInBlend(base.VirtualCamera))
			{
				CameraState state = fromCam.State;
				this.ForceCameraPosition(state.GetFinalPosition(), state.GetFinalOrientation());
				return true;
			}
			return false;
		}

		public override void ForceCameraPosition(Vector3 pos, Quaternion rot)
		{
			Action resetHandler = this.m_ResetHandler;
			if (resetHandler != null)
			{
				resetHandler();
			}
			if (base.FollowTarget != null)
			{
				CameraState vcamState = base.VcamState;
				this.m_PreviousOffset = rot * Quaternion.Inverse(vcamState.GetFinalOrientation()) * this.m_PreviousOffset;
				vcamState.RawPosition = pos;
				vcamState.RawOrientation = rot;
				vcamState.PositionCorrection = Vector3.zero;
				vcamState.OrientationCorrection = Quaternion.identity;
				Vector3 vector = pos - base.FollowTarget.TransformPoint(this.TargetOffset);
				float magnitude = vector.magnitude;
				if (magnitude > 0.001f)
				{
					vector /= magnitude;
					if (this.OrbitStyle == CinemachineOrbitalFollow.OrbitStyles.ThreeRing)
					{
						this.InferAxesFromPosition_ThreeRing(vector, magnitude, ref vcamState);
					}
					else
					{
						this.InferAxesFromPosition_Sphere(vector, magnitude, ref vcamState);
					}
				}
				this.m_TargetTracker.OnForceCameraPosition(this, this.TrackerSettings.BindingMode, ref vcamState);
			}
		}

		private void InferAxesFromPosition_Sphere(Vector3 dir, float distance, ref CameraState state)
		{
			Vector3 referenceUp = state.ReferenceUp;
			Vector3 v = Quaternion.Inverse(this.m_TargetTracker.GetReferenceOrientation(this, this.TrackerSettings.BindingMode, referenceUp, ref state)) * dir;
			Vector3 eulerAngles = UnityVectorExtensions.SafeFromToRotation(Vector3.back, v, referenceUp).eulerAngles;
			this.VerticalAxis.Value = this.VerticalAxis.ClampValue(UnityVectorExtensions.NormalizeAngle(eulerAngles.x));
			this.HorizontalAxis.Value = this.HorizontalAxis.ClampValue(UnityVectorExtensions.NormalizeAngle(eulerAngles.y));
			this.RadialAxis.Value = this.RadialAxis.ClampValue(distance / this.Radius);
		}

		private void InferAxesFromPosition_ThreeRing(Vector3 dir, float distance, ref CameraState state)
		{
			CinemachineOrbitalFollow.<>c__DisplayClass50_0 CS$<>8__locals1;
			CS$<>8__locals1.dir = dir;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.distance = distance;
			CS$<>8__locals1.up = state.ReferenceUp;
			CS$<>8__locals1.orient = this.m_TargetTracker.GetReferenceOrientation(this, this.TrackerSettings.BindingMode, CS$<>8__locals1.up, ref state);
			this.HorizontalAxis.Value = this.<InferAxesFromPosition_ThreeRing>g__GetHorizontalAxis|50_0(ref CS$<>8__locals1);
			Vector3 vector;
			this.VerticalAxis.Value = this.<InferAxesFromPosition_ThreeRing>g__GetVerticalAxisClosestValue|50_1(out vector, ref CS$<>8__locals1);
			this.RadialAxis.Value = this.RadialAxis.ClampValue(CS$<>8__locals1.distance / vector.magnitude);
		}

		public override void OnTargetObjectWarped(Transform target, Vector3 positionDelta)
		{
			base.OnTargetObjectWarped(target, positionDelta);
			if (target == base.FollowTarget)
			{
				this.m_TargetTracker.OnTargetObjectWarped(positionDelta);
			}
		}

		public override void MutateCameraState(ref CameraState curState, float deltaTime)
		{
			this.m_TargetTracker.InitStateInfo(this, deltaTime, this.TrackerSettings.BindingMode, curState.ReferenceUp);
			if (!this.IsValid)
			{
				return;
			}
			if (deltaTime < 0f)
			{
				Action resetHandler = this.m_ResetHandler;
				if (resetHandler != null)
				{
					resetHandler();
				}
			}
			Vector3 vector = this.GetCameraPoint();
			bool flag = this.HorizontalAxis.TrackValueChange();
			bool flag2 = this.VerticalAxis.TrackValueChange();
			bool flag3 = this.RadialAxis.TrackValueChange();
			if (this.TrackerSettings.BindingMode == BindingMode.LazyFollow)
			{
				this.HorizontalAxis.SetValueAndLastValue(0f);
			}
			Vector3 vector2;
			Quaternion quaternion;
			this.m_TargetTracker.TrackTarget(this, deltaTime, curState.ReferenceUp, vector, this.TrackerSettings, ref curState, out vector2, out quaternion);
			vector = quaternion * vector;
			curState.ReferenceUp = quaternion * Vector3.up;
			Vector3 followTargetPosition = base.FollowTargetPosition;
			vector2 += this.m_TargetTracker.GetOffsetForMinimumTargetDistance(this, vector2, vector, curState.RawOrientation * Vector3.forward, curState.ReferenceUp, followTargetPosition);
			vector2 += quaternion * this.TargetOffset;
			this.TrackedPoint = vector2;
			curState.RawPosition = vector2 + vector;
			if (curState.HasLookAt())
			{
				Vector3 b = quaternion * (curState.ReferenceLookAt - (base.FollowTargetPosition + base.FollowTargetRotation * this.TargetOffset));
				vector = curState.RawPosition - (vector2 + b);
			}
			if (deltaTime >= 0f && base.VirtualCamera.PreviousStateIsValid && this.m_PreviousOffset.sqrMagnitude > 0.0001f && vector.sqrMagnitude > 0.0001f)
			{
				curState.RotationDampingBypass = UnityVectorExtensions.SafeFromToRotation(this.m_PreviousOffset, vector, curState.ReferenceUp);
			}
			this.m_PreviousOffset = vector;
			if (this.HorizontalAxis.Recentering.Enabled)
			{
				this.UpdateHorizontalCenter(quaternion);
			}
			flag |= (flag2 && this.HorizontalAxis.Recentering.Time == this.VerticalAxis.Recentering.Time);
			flag |= (flag3 && this.HorizontalAxis.Recentering.Time == this.RadialAxis.Recentering.Time);
			flag2 |= (flag && this.VerticalAxis.Recentering.Time == this.HorizontalAxis.Recentering.Time);
			flag2 |= (flag3 && this.VerticalAxis.Recentering.Time == this.RadialAxis.Recentering.Time);
			flag3 |= (flag && this.RadialAxis.Recentering.Time == this.HorizontalAxis.Recentering.Time);
			flag3 |= (flag2 && this.RadialAxis.Recentering.Time == this.VerticalAxis.Recentering.Time);
			this.HorizontalAxis.UpdateRecentering(deltaTime, flag);
			this.VerticalAxis.UpdateRecentering(deltaTime, flag2);
			this.RadialAxis.UpdateRecentering(deltaTime, flag3);
		}

		private void UpdateHorizontalCenter(Quaternion referenceOrientation)
		{
			Vector3 forward = Vector3.forward;
			switch (this.RecenteringTarget)
			{
			case CinemachineOrbitalFollow.ReferenceFrames.AxisCenter:
				if (this.TrackerSettings.BindingMode == BindingMode.LazyFollow)
				{
					this.HorizontalAxis.Center = 0f;
				}
				return;
			case CinemachineOrbitalFollow.ReferenceFrames.ParentObject:
				if (base.transform.parent != null)
				{
					forward = base.transform.parent.forward;
				}
				break;
			case CinemachineOrbitalFollow.ReferenceFrames.TrackingTarget:
				if (base.FollowTarget != null)
				{
					forward = base.FollowTarget.forward;
				}
				break;
			case CinemachineOrbitalFollow.ReferenceFrames.LookAtTarget:
				if (base.LookAtTarget != null)
				{
					forward = base.LookAtTarget.forward;
				}
				break;
			}
			Vector3 vector = referenceOrientation * Vector3.up;
			forward.ProjectOntoPlane(vector);
			this.HorizontalAxis.Center = -Vector3.SignedAngle(forward, referenceOrientation * Vector3.forward, vector);
		}

		internal Quaternion GetReferenceOrientation()
		{
			return this.m_TargetTracker.PreviousReferenceOrientation.normalized;
		}

		[CompilerGenerated]
		private float <InferAxesFromPosition_ThreeRing>g__GetHorizontalAxis|50_0(ref CinemachineOrbitalFollow.<>c__DisplayClass50_0 A_1)
		{
			Vector3 vector = (A_1.orient * Vector3.back).ProjectOntoPlane(A_1.up);
			if (!vector.AlmostZero())
			{
				return UnityVectorExtensions.SignedAngle(vector, A_1.dir.ProjectOntoPlane(A_1.up), A_1.up);
			}
			return this.HorizontalAxis.Value;
		}

		[CompilerGenerated]
		private float <InferAxesFromPosition_ThreeRing>g__GetVerticalAxisClosestValue|50_1(out Vector3 splinePoint, ref CinemachineOrbitalFollow.<>c__DisplayClass50_0 A_2)
		{
			Vector3 vector = UnityVectorExtensions.SafeFromToRotation(A_2.up, Vector3.up, A_2.up) * A_2.dir;
			Vector3 vector2 = vector;
			vector2.y = 0f;
			if (!vector2.AlmostZero())
			{
				vector = Quaternion.AngleAxis(UnityVectorExtensions.SignedAngle(vector2, Vector3.back, Vector3.up), Vector3.up) * vector;
			}
			vector.x = 0f;
			vector.Normalize();
			float num = this.<InferAxesFromPosition_ThreeRing>g__SteepestDescent|50_2(vector * A_2.distance, ref A_2);
			splinePoint = this.m_OrbitCache.SplineValue(num);
			if (num > 0.5f)
			{
				return Mathf.Lerp(this.VerticalAxis.Center, this.VerticalAxis.Range.y, CinemachineOrbitalFollow.<InferAxesFromPosition_ThreeRing>g__MapTo01|50_3(num, 0.5f, 1f));
			}
			return Mathf.Lerp(this.VerticalAxis.Range.x, this.VerticalAxis.Center, CinemachineOrbitalFollow.<InferAxesFromPosition_ThreeRing>g__MapTo01|50_3(num, 0f, 0.5f));
		}

		[CompilerGenerated]
		private float <InferAxesFromPosition_ThreeRing>g__SteepestDescent|50_2(Vector3 cameraOffset, ref CinemachineOrbitalFollow.<>c__DisplayClass50_0 A_2)
		{
			CinemachineOrbitalFollow.<>c__DisplayClass50_1 CS$<>8__locals1;
			CS$<>8__locals1.cameraOffset = cameraOffset;
			float num = this.<InferAxesFromPosition_ThreeRing>g__InitialGuess|50_6(ref A_2, ref CS$<>8__locals1);
			for (int i = 0; i < 5; i++)
			{
				float num2 = this.<InferAxesFromPosition_ThreeRing>g__AngleFunction|50_4(num, ref A_2, ref CS$<>8__locals1);
				float num3 = this.<InferAxesFromPosition_ThreeRing>g__SlopeOfAngleFunction|50_5(num, ref A_2, ref CS$<>8__locals1);
				if (Mathf.Abs(num3) < 0.005f || Mathf.Abs(num2) < 0.005f)
				{
					break;
				}
				num = Mathf.Clamp01(num - num2 / num3);
			}
			return num;
		}

		[CompilerGenerated]
		private float <InferAxesFromPosition_ThreeRing>g__AngleFunction|50_4(float input, ref CinemachineOrbitalFollow.<>c__DisplayClass50_0 A_2, ref CinemachineOrbitalFollow.<>c__DisplayClass50_1 A_3)
		{
			Vector4 v = this.m_OrbitCache.SplineValue(input);
			return Mathf.Abs(UnityVectorExtensions.SignedAngle(A_3.cameraOffset, v, Vector3.right));
		}

		[CompilerGenerated]
		private float <InferAxesFromPosition_ThreeRing>g__SlopeOfAngleFunction|50_5(float input, ref CinemachineOrbitalFollow.<>c__DisplayClass50_0 A_2, ref CinemachineOrbitalFollow.<>c__DisplayClass50_1 A_3)
		{
			float num = this.<InferAxesFromPosition_ThreeRing>g__AngleFunction|50_4(input - 0.005f, ref A_2, ref A_3);
			return (this.<InferAxesFromPosition_ThreeRing>g__AngleFunction|50_4(input + 0.005f, ref A_2, ref A_3) - num) / 0.01f;
		}

		[CompilerGenerated]
		private float <InferAxesFromPosition_ThreeRing>g__InitialGuess|50_6(ref CinemachineOrbitalFollow.<>c__DisplayClass50_0 A_1, ref CinemachineOrbitalFollow.<>c__DisplayClass50_1 A_2)
		{
			if (this.m_OrbitCache.SettingsChanged(this.Orbits))
			{
				this.m_OrbitCache.UpdateOrbitCache(this.Orbits);
			}
			CinemachineOrbitalFollow.<>c__DisplayClass50_2 CS$<>8__locals1;
			CS$<>8__locals1.best = 0.5f;
			CS$<>8__locals1.bestAngle = this.<InferAxesFromPosition_ThreeRing>g__AngleFunction|50_4(CS$<>8__locals1.best, ref A_1, ref A_2);
			for (int i = 0; i <= 5; i++)
			{
				float num = (float)i * 0.1f;
				this.<InferAxesFromPosition_ThreeRing>g__ChooseBestAngle|50_7(0.5f + num, ref A_1, ref A_2, ref CS$<>8__locals1);
				this.<InferAxesFromPosition_ThreeRing>g__ChooseBestAngle|50_7(0.5f - num, ref A_1, ref A_2, ref CS$<>8__locals1);
			}
			return CS$<>8__locals1.best;
		}

		[CompilerGenerated]
		private void <InferAxesFromPosition_ThreeRing>g__ChooseBestAngle|50_7(float x, ref CinemachineOrbitalFollow.<>c__DisplayClass50_0 A_2, ref CinemachineOrbitalFollow.<>c__DisplayClass50_1 A_3, ref CinemachineOrbitalFollow.<>c__DisplayClass50_2 A_4)
		{
			float num = this.<InferAxesFromPosition_ThreeRing>g__AngleFunction|50_4(x, ref A_2, ref A_3);
			if (num < A_4.bestAngle)
			{
				A_4.bestAngle = num;
				A_4.best = x;
			}
		}

		[CompilerGenerated]
		internal static float <InferAxesFromPosition_ThreeRing>g__MapTo01|50_3(float valueToMap, float fMin, float fMax)
		{
			return (valueToMap - fMin) / (fMax - fMin);
		}

		[Tooltip("Offset from the target object's center in target-local space. Use this to fine-tune the orbit when the desired focus of the orbit is not the tracked object's center.")]
		public Vector3 TargetOffset;

		public TrackerSettings TrackerSettings = TrackerSettings.Default;

		[Tooltip("Defines the manner in which the orbit surface is constructed.")]
		public CinemachineOrbitalFollow.OrbitStyles OrbitStyle;

		[Tooltip("The camera will be placed at this distance from the Follow target.")]
		public float Radius = 10f;

		[Tooltip("Defines a complex surface rig from 3 horizontal rings.")]
		[HideFoldout]
		public Cinemachine3OrbitRig.Settings Orbits = Cinemachine3OrbitRig.Settings.Default;

		[Tooltip("Defines the reference frame for horizontal recentering.  The axis center will be dynamically updated to be behind the selected object.")]
		public CinemachineOrbitalFollow.ReferenceFrames RecenteringTarget = CinemachineOrbitalFollow.ReferenceFrames.TrackingTarget;

		[Tooltip("Axis representing the current horizontal rotation.  Value is in degrees and represents a rotation about the up vector.")]
		public InputAxis HorizontalAxis = CinemachineOrbitalFollow.DefaultHorizontal;

		[Tooltip("Axis representing the current vertical rotation.  Value is in degrees and represents a rotation about the right vector.")]
		public InputAxis VerticalAxis = CinemachineOrbitalFollow.DefaultVertical;

		[Tooltip("Axis controlling the scale of the current distance.  Value is a scalar multiplier and is applied to the specified camera distance.")]
		public InputAxis RadialAxis = CinemachineOrbitalFollow.DefaultRadial;

		private Vector3 m_PreviousOffset;

		private Tracker m_TargetTracker;

		private Cinemachine3OrbitRig.OrbitSplineCache m_OrbitCache;

		private Action m_ResetHandler;

		public enum OrbitStyles
		{
			Sphere,
			ThreeRing
		}

		public enum ReferenceFrames
		{
			AxisCenter,
			ParentObject,
			TrackingTarget,
			LookAtTarget
		}
	}
}
