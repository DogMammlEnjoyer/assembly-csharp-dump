using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Cinemachine
{
	public struct CameraState
	{
		public static CameraState Default
		{
			get
			{
				return new CameraState
				{
					Lens = LensSettings.Default,
					ReferenceUp = Vector3.up,
					ReferenceLookAt = CameraState.kNoPoint,
					RawPosition = Vector3.zero,
					RawOrientation = Quaternion.identity,
					ShotQuality = 1f,
					PositionCorrection = Vector3.zero,
					OrientationCorrection = Quaternion.identity,
					RotationDampingBypass = Quaternion.identity,
					BlendHint = CameraState.BlendHints.Nothing
				};
			}
		}

		public void AddCustomBlendable(CameraState.CustomBlendableItems.Item b)
		{
			int num = this.FindCustomBlendable(b.Custom);
			if (num >= 0)
			{
				b.Weight += this.GetCustomBlendable(num).Weight;
			}
			else
			{
				int numItems = this.CustomBlendables.NumItems;
				this.CustomBlendables.NumItems = numItems + 1;
				num = numItems;
			}
			switch (num)
			{
			case 0:
				this.CustomBlendables.m_Item0 = b;
				return;
			case 1:
				this.CustomBlendables.m_Item1 = b;
				return;
			case 2:
				this.CustomBlendables.m_Item2 = b;
				return;
			case 3:
				this.CustomBlendables.m_Item3 = b;
				return;
			default:
			{
				num -= 4;
				ref List<CameraState.CustomBlendableItems.Item> ptr = ref this.CustomBlendables.m_Overflow;
				if (ptr == null)
				{
					ptr = new List<CameraState.CustomBlendableItems.Item>();
				}
				if (num < this.CustomBlendables.m_Overflow.Count)
				{
					this.CustomBlendables.m_Overflow[num] = b;
					return;
				}
				this.CustomBlendables.m_Overflow.Add(b);
				return;
			}
			}
		}

		public static CameraState Lerp(in CameraState stateA, in CameraState stateB, float t)
		{
			t = Mathf.Clamp01(t);
			float t2 = t;
			CameraState cameraState = default(CameraState);
			if ((stateA.BlendHint & stateB.BlendHint & CameraState.BlendHints.NoPosition) != CameraState.BlendHints.Nothing)
			{
				cameraState.BlendHint |= CameraState.BlendHints.NoPosition;
			}
			if ((stateA.BlendHint & stateB.BlendHint & CameraState.BlendHints.NoOrientation) != CameraState.BlendHints.Nothing)
			{
				cameraState.BlendHint |= CameraState.BlendHints.NoOrientation;
			}
			if ((stateA.BlendHint & stateB.BlendHint & CameraState.BlendHints.NoLens) != CameraState.BlendHints.Nothing)
			{
				cameraState.BlendHint |= CameraState.BlendHints.NoLens;
			}
			if (((stateA.BlendHint | stateB.BlendHint) & CameraState.BlendHints.SphericalPositionBlend) != CameraState.BlendHints.Nothing)
			{
				cameraState.BlendHint |= CameraState.BlendHints.SphericalPositionBlend;
			}
			if (((stateA.BlendHint | stateB.BlendHint) & CameraState.BlendHints.CylindricalPositionBlend) != CameraState.BlendHints.Nothing)
			{
				cameraState.BlendHint |= CameraState.BlendHints.CylindricalPositionBlend;
			}
			if (((stateA.BlendHint | stateB.BlendHint) & CameraState.BlendHints.FreezeWhenBlendingOut) != CameraState.BlendHints.Nothing)
			{
				cameraState.BlendHint |= CameraState.BlendHints.FreezeWhenBlendingOut;
			}
			if (((stateA.BlendHint | stateB.BlendHint) & CameraState.BlendHints.NoLens) == CameraState.BlendHints.Nothing)
			{
				cameraState.Lens = LensSettings.Lerp(stateA.Lens, stateB.Lens, t);
			}
			else if ((stateA.BlendHint & stateB.BlendHint & CameraState.BlendHints.NoLens) == CameraState.BlendHints.Nothing)
			{
				if ((stateA.BlendHint & CameraState.BlendHints.NoLens) != CameraState.BlendHints.Nothing)
				{
					cameraState.Lens = stateB.Lens;
				}
				else
				{
					cameraState.Lens = stateA.Lens;
				}
			}
			cameraState.ReferenceUp = Vector3.Slerp(stateA.ReferenceUp, stateB.ReferenceUp, t);
			cameraState.ShotQuality = Mathf.Lerp(stateA.ShotQuality, stateB.ShotQuality, t);
			cameraState.PositionCorrection = CameraState.ApplyPosBlendHint(stateA.PositionCorrection, stateA.BlendHint, stateB.PositionCorrection, stateB.BlendHint, cameraState.PositionCorrection, Vector3.Lerp(stateA.PositionCorrection, stateB.PositionCorrection, t));
			cameraState.OrientationCorrection = CameraState.ApplyRotBlendHint(stateA.OrientationCorrection, stateA.BlendHint, stateB.OrientationCorrection, stateB.BlendHint, cameraState.OrientationCorrection, Quaternion.Slerp(stateA.OrientationCorrection, stateB.OrientationCorrection, t));
			if (!stateA.HasLookAt() || !stateB.HasLookAt())
			{
				cameraState.ReferenceLookAt = CameraState.kNoPoint;
			}
			else
			{
				float fieldOfView = stateA.Lens.FieldOfView;
				float fieldOfView2 = stateB.Lens.FieldOfView;
				if (((stateA.BlendHint | stateB.BlendHint) & CameraState.BlendHints.NoLens) == CameraState.BlendHints.Nothing && !cameraState.Lens.Orthographic && !Mathf.Approximately(fieldOfView, fieldOfView2))
				{
					LensSettings lens = cameraState.Lens;
					lens.FieldOfView = CameraState.InterpolateFOV(fieldOfView, fieldOfView2, Mathf.Max((stateA.ReferenceLookAt - stateA.GetCorrectedPosition()).magnitude, stateA.Lens.NearClipPlane), Mathf.Max((stateB.ReferenceLookAt - stateB.GetCorrectedPosition()).magnitude, stateB.Lens.NearClipPlane), t);
					cameraState.Lens = lens;
					t2 = Mathf.Abs((lens.FieldOfView - fieldOfView) / (fieldOfView2 - fieldOfView));
				}
				cameraState.ReferenceLookAt = Vector3.Lerp(stateA.ReferenceLookAt, stateB.ReferenceLookAt, t2);
			}
			cameraState.RawPosition = CameraState.ApplyPosBlendHint(stateA.RawPosition, stateA.BlendHint, stateB.RawPosition, stateB.BlendHint, cameraState.RawPosition, CameraState.InterpolatePosition(stateA.RawPosition, stateA.ReferenceLookAt, stateB.RawPosition, stateB.ReferenceLookAt, t, cameraState.BlendHint, cameraState.ReferenceUp));
			if (cameraState.HasLookAt() && ((stateA.BlendHint | stateB.BlendHint) & CameraState.BlendHints.ScreenSpaceAimWhenTargetsDiffer) != CameraState.BlendHints.Nothing)
			{
				cameraState.ReferenceLookAt = cameraState.RawPosition + Vector3.Slerp(stateA.ReferenceLookAt - cameraState.RawPosition, stateB.ReferenceLookAt - cameraState.RawPosition, t2);
			}
			Quaternion quaternion = cameraState.RawOrientation;
			if (((stateA.BlendHint | stateB.BlendHint) & CameraState.BlendHints.NoOrientation) == CameraState.BlendHints.Nothing)
			{
				Vector3 vector = Vector3.zero;
				if (cameraState.HasLookAt() && Quaternion.Angle(stateA.RawOrientation, stateB.RawOrientation) > 0.0001f)
				{
					vector = cameraState.ReferenceLookAt - cameraState.GetCorrectedPosition();
				}
				if (vector.AlmostZero() || ((stateA.BlendHint | stateB.BlendHint) & CameraState.BlendHints.IgnoreLookAtTarget) != CameraState.BlendHints.Nothing)
				{
					quaternion = Quaternion.Slerp(stateA.RawOrientation, stateB.RawOrientation, t);
				}
				else
				{
					Vector3 vector2 = cameraState.ReferenceUp;
					vector.Normalize();
					if (Vector3.Cross(vector, vector2).AlmostZero())
					{
						quaternion = Quaternion.Slerp(stateA.RawOrientation, stateB.RawOrientation, t);
						vector2 = quaternion * Vector3.up;
					}
					quaternion = Quaternion.LookRotation(vector, vector2);
					Vector2 a = -stateA.RawOrientation.GetCameraRotationToTarget(stateA.ReferenceLookAt - stateA.GetCorrectedPosition(), vector2);
					Vector2 b = -stateB.RawOrientation.GetCameraRotationToTarget(stateB.ReferenceLookAt - stateB.GetCorrectedPosition(), vector2);
					quaternion = quaternion.ApplyCameraRotation(Vector2.Lerp(a, b, t2), vector2);
				}
			}
			cameraState.RawOrientation = CameraState.ApplyRotBlendHint(stateA.RawOrientation, stateA.BlendHint, stateB.RawOrientation, stateB.BlendHint, cameraState.RawOrientation, quaternion);
			for (int i = 0; i < stateA.CustomBlendables.NumItems; i++)
			{
				CameraState.CustomBlendableItems.Item customBlendable = stateA.GetCustomBlendable(i);
				customBlendable.Weight *= 1f - t;
				if (customBlendable.Weight > 0f)
				{
					cameraState.AddCustomBlendable(customBlendable);
				}
			}
			for (int j = 0; j < stateB.CustomBlendables.NumItems; j++)
			{
				CameraState.CustomBlendableItems.Item customBlendable2 = stateB.GetCustomBlendable(j);
				customBlendable2.Weight *= t;
				if (customBlendable2.Weight > 0f)
				{
					cameraState.AddCustomBlendable(customBlendable2);
				}
			}
			return cameraState;
		}

		private static float InterpolateFOV(float fovA, float fovB, float dA, float dB, float t)
		{
			float a = dA * 2f * Mathf.Tan(fovA * 0.017453292f / 2f);
			float b = dB * 2f * Mathf.Tan(fovB * 0.017453292f / 2f);
			float num = Mathf.Lerp(a, b, t);
			float value = 179f;
			float num2 = Mathf.Lerp(dA, dB, t);
			if (num2 > 0.0001f)
			{
				value = 2f * Mathf.Atan(num / (2f * num2)) * 57.29578f;
			}
			return Mathf.Clamp(value, Mathf.Min(fovA, fovB), Mathf.Max(fovA, fovB));
		}

		private static Vector3 ApplyPosBlendHint(Vector3 posA, CameraState.BlendHints hintA, Vector3 posB, CameraState.BlendHints hintB, Vector3 original, Vector3 blended)
		{
			if (((hintA | hintB) & CameraState.BlendHints.NoPosition) == CameraState.BlendHints.Nothing)
			{
				return blended;
			}
			if ((hintA & hintB & CameraState.BlendHints.NoPosition) != CameraState.BlendHints.Nothing)
			{
				return original;
			}
			if ((hintA & CameraState.BlendHints.NoPosition) != CameraState.BlendHints.Nothing)
			{
				return posB;
			}
			return posA;
		}

		private static Quaternion ApplyRotBlendHint(Quaternion rotA, CameraState.BlendHints hintA, Quaternion rotB, CameraState.BlendHints hintB, Quaternion original, Quaternion blended)
		{
			if (((hintA | hintB) & CameraState.BlendHints.NoOrientation) == CameraState.BlendHints.Nothing)
			{
				return blended;
			}
			if ((hintA & hintB & CameraState.BlendHints.NoOrientation) != CameraState.BlendHints.Nothing)
			{
				return original;
			}
			if ((hintA & CameraState.BlendHints.NoOrientation) != CameraState.BlendHints.Nothing)
			{
				return rotB;
			}
			return rotA;
		}

		private static Vector3 InterpolatePosition(Vector3 posA, Vector3 pivotA, Vector3 posB, Vector3 pivotB, float t, CameraState.BlendHints blendHint, Vector3 up)
		{
			if (pivotA == pivotA && pivotB == pivotB)
			{
				if ((blendHint & CameraState.BlendHints.CylindricalPositionBlend) != CameraState.BlendHints.Nothing)
				{
					Vector3 vector = Vector3.ProjectOnPlane(posA - pivotA, up);
					Vector3 b = Vector3.ProjectOnPlane(posB - pivotB, up);
					Vector3 b2 = Vector3.Slerp(vector, b, t);
					posA = posA - vector + b2;
					posB = posB - b + b2;
				}
				else if ((blendHint & CameraState.BlendHints.SphericalPositionBlend) != CameraState.BlendHints.Nothing)
				{
					Vector3 b3 = Vector3.Slerp(posA - pivotA, posB - pivotB, t);
					posA = pivotA + b3;
					posB = pivotB + b3;
				}
			}
			return Vector3.Lerp(posA, posB, t);
		}

		public LensSettings Lens;

		public Vector3 ReferenceUp;

		public Vector3 ReferenceLookAt;

		public static Vector3 kNoPoint = new Vector3(float.NaN, float.NaN, float.NaN);

		public Vector3 RawPosition;

		public Quaternion RawOrientation;

		public Quaternion RotationDampingBypass;

		public float ShotQuality;

		public Vector3 PositionCorrection;

		public Quaternion OrientationCorrection;

		public CameraState.BlendHints BlendHint;

		internal CameraState.CustomBlendableItems CustomBlendables;

		public enum BlendHints
		{
			Nothing,
			SphericalPositionBlend,
			CylindricalPositionBlend,
			ScreenSpaceAimWhenTargetsDiffer = 4,
			InheritPosition = 8,
			IgnoreLookAtTarget = 16,
			FreezeWhenBlendingOut = 32,
			NoPosition = 65536,
			NoOrientation = 131072,
			NoTransform = 196608,
			NoLens = 262144
		}

		public struct CustomBlendableItems
		{
			internal CameraState.CustomBlendableItems.Item m_Item0;

			internal CameraState.CustomBlendableItems.Item m_Item1;

			internal CameraState.CustomBlendableItems.Item m_Item2;

			internal CameraState.CustomBlendableItems.Item m_Item3;

			internal List<CameraState.CustomBlendableItems.Item> m_Overflow;

			internal int NumItems;

			public struct Item
			{
				public Object Custom;

				public float Weight;
			}
		}
	}
}
