using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Unity.Cinemachine
{
	internal class CameraBlendStack : ICameraOverrideStack
	{
		public Vector3 DefaultWorldUp
		{
			get
			{
				return Vector3.up;
			}
		}

		public int SetCameraOverride(int overrideId, int priority, ICinemachineCamera camA, ICinemachineCamera camB, float weightB, float deltaTime)
		{
			if (overrideId < 0)
			{
				int num = this.m_NextFrameId + 1;
				this.m_NextFrameId = num;
				overrideId = num;
			}
			if (this.m_FrameStack.Count == 0)
			{
				this.m_FrameStack.Add(new CameraBlendStack.StackFrame());
			}
			CameraBlendStack.StackFrame stackFrame = this.m_FrameStack[this.<SetCameraOverride>g__FindFrame|7_0(overrideId, priority)];
			stackFrame.DeltaTimeOverride = deltaTime;
			stackFrame.Source.TimeInBlend = weightB;
			if (stackFrame.Source.CamA != camA || stackFrame.Source.CamB != camB)
			{
				CinemachineBlend source = stackFrame.Source;
				CinemachineCore.GetCustomBlenderDelegate getCustomBlender = CinemachineCore.GetCustomBlender;
				source.CustomBlender = ((getCustomBlender != null) ? getCustomBlender(camA, camB) : null);
				stackFrame.Source.CamA = camA;
				stackFrame.Source.CamB = camB;
				CinemachineVirtualCameraBase cinemachineVirtualCameraBase = camA as CinemachineVirtualCameraBase;
				if (cinemachineVirtualCameraBase != null)
				{
					cinemachineVirtualCameraBase.EnsureStarted();
				}
				CinemachineVirtualCameraBase cinemachineVirtualCameraBase2 = camB as CinemachineVirtualCameraBase;
				if (cinemachineVirtualCameraBase2 != null)
				{
					cinemachineVirtualCameraBase2.EnsureStarted();
				}
			}
			return overrideId;
		}

		public void ReleaseCameraOverride(int overrideId)
		{
			for (int i = this.m_FrameStack.Count - 1; i > 0; i--)
			{
				if (this.m_FrameStack[i].Id == overrideId)
				{
					this.m_FrameStack.RemoveAt(i);
					return;
				}
			}
		}

		public virtual void OnEnable()
		{
			this.m_FrameStack.Clear();
			this.m_FrameStack.Add(new CameraBlendStack.StackFrame());
		}

		public virtual void OnDisable()
		{
			this.m_FrameStack.Clear();
			this.m_NextFrameId = 0;
		}

		public bool IsInitialized
		{
			get
			{
				return this.m_FrameStack.Count > 0;
			}
		}

		public CinemachineBlendDefinition.LookupBlendDelegate LookupBlendDelegate { get; set; }

		public void ResetRootFrame()
		{
			if (this.m_FrameStack.Count == 0)
			{
				this.m_FrameStack.Add(new CameraBlendStack.StackFrame());
				return;
			}
			CameraBlendStack.StackFrame stackFrame = this.m_FrameStack[0];
			stackFrame.Blend.ClearBlend();
			stackFrame.Blend.CamB = null;
			stackFrame.Source.ClearBlend();
			stackFrame.Source.CamB = null;
		}

		public void UpdateRootFrame(ICinemachineMixer context, ICinemachineCamera activeCamera, Vector3 up, float deltaTime)
		{
			if (this.m_FrameStack.Count == 0)
			{
				this.m_FrameStack.Add(new CameraBlendStack.StackFrame());
			}
			CameraBlendStack.StackFrame stackFrame = this.m_FrameStack[0];
			ICinemachineCamera camB = stackFrame.Source.CamB;
			if (activeCamera != camB)
			{
				bool flag = false;
				float num = 0f;
				float num2 = 0f;
				if (this.LookupBlendDelegate != null && activeCamera != null && activeCamera.IsValid && camB != null && camB.IsValid && deltaTime >= 0f)
				{
					CinemachineBlendDefinition cinemachineBlendDefinition = this.LookupBlendDelegate(camB, activeCamera);
					if (cinemachineBlendDefinition.BlendCurve != null && cinemachineBlendDefinition.BlendTime > 0.0001f)
					{
						flag = (stackFrame.Source.CamA == activeCamera && stackFrame.Source.CamB == camB);
						if (flag && stackFrame.Blend.Duration > 0.0001f)
						{
							num = stackFrame.Blend.TimeInBlend / stackFrame.Blend.Duration;
						}
						stackFrame.Source.CamA = camB;
						stackFrame.Source.BlendCurve = cinemachineBlendDefinition.BlendCurve;
						num2 = cinemachineBlendDefinition.BlendTime;
					}
					stackFrame.Source.Duration = num2;
					stackFrame.Source.TimeInBlend = 0f;
					stackFrame.Source.CustomBlender = null;
				}
				stackFrame.Source.CamB = activeCamera;
				if (num2 > 0f)
				{
					CinemachineBlend source = stackFrame.Source;
					CinemachineCore.GetCustomBlenderDelegate getCustomBlender = CinemachineCore.GetCustomBlender;
					source.CustomBlender = ((getCustomBlender != null) ? getCustomBlender(camB, activeCamera) : null);
				}
				ICinemachineCamera camA;
				if (stackFrame.Blend.IsComplete)
				{
					camA = stackFrame.GetSnapshotIfAppropriate(camB, 0f);
				}
				else
				{
					bool flag2 = (stackFrame.Blend.State.BlendHint & CameraState.BlendHints.FreezeWhenBlendingOut) > CameraState.BlendHints.Nothing;
					if (!flag2 && activeCamera != null && (activeCamera.State.BlendHint & CameraState.BlendHints.InheritPosition) != CameraState.BlendHints.Nothing && stackFrame.Blend.Uses(activeCamera))
					{
						flag2 = true;
					}
					NestedBlendSource nestedBlendSource = stackFrame.Blend.CamA as NestedBlendSource;
					if (!flag2 && nestedBlendSource != null)
					{
						NestedBlendSource nestedBlendSource2 = nestedBlendSource.Blend.CamA as NestedBlendSource;
						if (nestedBlendSource2 != null)
						{
							nestedBlendSource2.Blend.CamA = new CameraBlendStack.SnapshotBlendSource(nestedBlendSource2.Blend.CamA, 0f);
						}
					}
					if (flag)
					{
						flag2 = true;
						float num3 = stackFrame.Blend.TimeInBlend;
						if (nestedBlendSource != null)
						{
							num3 += nestedBlendSource.Blend.Duration - nestedBlendSource.Blend.TimeInBlend;
						}
						else
						{
							CameraBlendStack.SnapshotBlendSource snapshotBlendSource = stackFrame.Blend.CamA as CameraBlendStack.SnapshotBlendSource;
							if (snapshotBlendSource != null)
							{
								num3 += snapshotBlendSource.RemainingTimeInBlend;
							}
						}
						num2 = Mathf.Min(num2 * num, num3);
					}
					if (flag2)
					{
						camA = new CameraBlendStack.SnapshotBlendSource(stackFrame, stackFrame.Blend.Duration - stackFrame.Blend.TimeInBlend);
					}
					else
					{
						CinemachineBlend cinemachineBlend = new CinemachineBlend();
						cinemachineBlend.CopyFrom(stackFrame.Blend);
						camA = new NestedBlendSource(cinemachineBlend);
					}
				}
				stackFrame.Blend.CamA = camB;
				stackFrame.Blend.CamB = activeCamera;
				stackFrame.Blend.BlendCurve = stackFrame.Source.BlendCurve;
				stackFrame.Blend.Duration = num2;
				stackFrame.Blend.TimeInBlend = 0f;
				stackFrame.Blend.CustomBlender = stackFrame.Source.CustomBlender;
				if (num2 > 0f)
				{
					CinemachineCore.BlendCreatedEvent.Invoke(new CinemachineCore.BlendEventParams
					{
						Origin = context,
						Blend = stackFrame.Blend
					});
				}
				stackFrame.Blend.CamA = camA;
				stackFrame.Blend.CamB = activeCamera;
			}
			if (CameraBlendStack.<UpdateRootFrame>g__AdvanceBlend|18_0(stackFrame.Blend, deltaTime))
			{
				stackFrame.Source.ClearBlend();
			}
			stackFrame.UpdateCameraState(up, deltaTime);
		}

		public void ProcessOverrideFrames(ref CinemachineBlend outputBlend, int numTopLayersToExclude)
		{
			if (this.m_FrameStack.Count == 0)
			{
				this.m_FrameStack.Add(new CameraBlendStack.StackFrame());
			}
			int index = 0;
			int num = Mathf.Max(0, this.m_FrameStack.Count - numTopLayersToExclude);
			for (int i = 1; i < num; i++)
			{
				CameraBlendStack.StackFrame stackFrame = this.m_FrameStack[i];
				if (stackFrame.Active)
				{
					stackFrame.Blend.CopyFrom(stackFrame.Source);
					if (stackFrame.Source.CamA != null)
					{
						stackFrame.Blend.CamA = stackFrame.GetSnapshotIfAppropriate(stackFrame.Source.CamA, stackFrame.Source.TimeInBlend);
					}
					if (!stackFrame.Blend.IsComplete)
					{
						if (stackFrame.Blend.CamA == null)
						{
							stackFrame.Blend.CamA = stackFrame.GetSnapshotIfAppropriate(this.m_FrameStack[index], stackFrame.Blend.TimeInBlend);
							CinemachineBlend blend = stackFrame.Blend;
							CinemachineCore.GetCustomBlenderDelegate getCustomBlender = CinemachineCore.GetCustomBlender;
							blend.CustomBlender = ((getCustomBlender != null) ? getCustomBlender(stackFrame.Blend.CamA, stackFrame.Blend.CamB) : null);
						}
						if (stackFrame.Blend.CamB == null)
						{
							stackFrame.Blend.CamB = this.m_FrameStack[index];
							CinemachineBlend blend2 = stackFrame.Blend;
							CinemachineCore.GetCustomBlenderDelegate getCustomBlender2 = CinemachineCore.GetCustomBlender;
							blend2.CustomBlender = ((getCustomBlender2 != null) ? getCustomBlender2(stackFrame.Blend.CamA, stackFrame.Blend.CamB) : null);
						}
					}
					index = i;
				}
			}
			outputBlend.CopyFrom(this.m_FrameStack[index].Blend);
		}

		public void SetRootBlend(CinemachineBlend blend)
		{
			if (this.IsInitialized)
			{
				if (blend == null)
				{
					this.m_FrameStack[0].Blend.Duration = 0f;
					return;
				}
				this.m_FrameStack[0].Blend.BlendCurve = blend.BlendCurve;
				this.m_FrameStack[0].Blend.TimeInBlend = blend.TimeInBlend;
				this.m_FrameStack[0].Blend.Duration = blend.Duration;
				this.m_FrameStack[0].Blend.CustomBlender = blend.CustomBlender;
			}
		}

		public float GetDeltaTimeOverride()
		{
			for (int i = this.m_FrameStack.Count - 1; i > 0; i--)
			{
				CameraBlendStack.StackFrame stackFrame = this.m_FrameStack[i];
				if (stackFrame.Active)
				{
					return stackFrame.DeltaTimeOverride;
				}
			}
			return -1f;
		}

		[CompilerGenerated]
		private int <SetCameraOverride>g__FindFrame|7_0(int withId, int priority)
		{
			int count = this.m_FrameStack.Count;
			int i;
			for (i = count - 1; i > 0; i--)
			{
				if (this.m_FrameStack[i].Id == withId)
				{
					return i;
				}
			}
			i = 1;
			while (i < count && this.m_FrameStack[i].Priority <= priority)
			{
				i++;
			}
			CameraBlendStack.StackFrame stackFrame = new CameraBlendStack.StackFrame
			{
				Id = withId,
				Priority = priority
			};
			stackFrame.Source.Duration = 1f;
			stackFrame.Source.BlendCurve = CameraBlendStack.s_DefaultLinearAnimationCurve;
			this.m_FrameStack.Insert(i, stackFrame);
			return i;
		}

		[CompilerGenerated]
		internal static bool <UpdateRootFrame>g__AdvanceBlend|18_0(CinemachineBlend blend, float deltaTime)
		{
			bool result = false;
			if (blend.CamA != null)
			{
				blend.TimeInBlend += ((deltaTime >= 0f) ? deltaTime : blend.Duration);
				if (blend.IsComplete)
				{
					blend.ClearBlend();
					result = true;
				}
				else
				{
					NestedBlendSource nestedBlendSource = blend.CamA as NestedBlendSource;
					if (nestedBlendSource != null)
					{
						CameraBlendStack.<UpdateRootFrame>g__AdvanceBlend|18_0(nestedBlendSource.Blend, deltaTime);
					}
				}
			}
			return result;
		}

		private const float kEpsilon = 0.0001f;

		private readonly List<CameraBlendStack.StackFrame> m_FrameStack = new List<CameraBlendStack.StackFrame>();

		private int m_NextFrameId;

		private static readonly AnimationCurve s_DefaultLinearAnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		private class StackFrame : NestedBlendSource
		{
			public StackFrame() : base(new CinemachineBlend())
			{
			}

			public bool Active
			{
				get
				{
					return this.Source.IsValid;
				}
			}

			public ICinemachineCamera GetSnapshotIfAppropriate(ICinemachineCamera cam, float weight)
			{
				if (cam == null || (cam.State.BlendHint & CameraState.BlendHints.FreezeWhenBlendingOut) == CameraState.BlendHints.Nothing)
				{
					this.m_Snapshot.TakeSnapshot(null);
					this.m_SnapshotSource = null;
					this.m_SnapshotBlendWeight = 0f;
					return cam;
				}
				if (this.m_SnapshotSource != cam || this.m_SnapshotBlendWeight > weight)
				{
					this.m_Snapshot.TakeSnapshot(cam);
					this.m_SnapshotSource = cam;
					this.m_SnapshotBlendWeight = weight;
				}
				return this.m_Snapshot;
			}

			public int Id;

			public int Priority;

			public readonly CinemachineBlend Source = new CinemachineBlend();

			public float DeltaTimeOverride;

			private readonly CameraBlendStack.SnapshotBlendSource m_Snapshot = new CameraBlendStack.SnapshotBlendSource(null, 0f);

			private ICinemachineCamera m_SnapshotSource;

			private float m_SnapshotBlendWeight;
		}

		private class SnapshotBlendSource : ICinemachineCamera
		{
			public float RemainingTimeInBlend { get; set; }

			public SnapshotBlendSource(ICinemachineCamera source = null, float remainingTimeInBlend = 0f)
			{
				this.TakeSnapshot(source);
				this.RemainingTimeInBlend = remainingTimeInBlend;
			}

			public string Name
			{
				get
				{
					return this.m_Name;
				}
			}

			public string Description
			{
				get
				{
					return this.Name;
				}
			}

			public CameraState State
			{
				get
				{
					return this.m_State;
				}
			}

			public bool IsValid
			{
				get
				{
					return true;
				}
			}

			public ICinemachineMixer ParentCamera
			{
				get
				{
					return null;
				}
			}

			public void UpdateCameraState(Vector3 worldUp, float deltaTime)
			{
			}

			public void OnCameraActivated(ICinemachineCamera.ActivationEventParams evt)
			{
			}

			public void TakeSnapshot(ICinemachineCamera source)
			{
				this.m_State = ((source != null) ? source.State : CameraState.Default);
				this.m_State.BlendHint = (this.m_State.BlendHint & (CameraState.BlendHints)(-33));
				if (this.m_Name == null)
				{
					this.m_Name = ((source == null) ? "(null)" : ("*" + source.Name));
				}
			}

			private CameraState m_State;

			private string m_Name;
		}
	}
}
