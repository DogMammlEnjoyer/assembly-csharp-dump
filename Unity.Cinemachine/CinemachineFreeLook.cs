using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Cinemachine.TargetTracking;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[Obsolete("This is deprecated. Use Create -> Cinemachine -> FreeLook camera, or create a CinemachineCamera with appropriate components")]
	[DisallowMultipleComponent]
	[ExecuteAlways]
	[ExcludeFromPreset]
	[AddComponentMenu("")]
	public class CinemachineFreeLook : CinemachineVirtualCameraBase, AxisState.IRequiresInput, ICinemachineMixer, ICinemachineCamera
	{
		protected internal override void PerformLegacyUpgrade(int streamedVersion)
		{
			base.PerformLegacyUpgrade(streamedVersion);
			if (streamedVersion < 20221011)
			{
				if (this.m_LegacyHeadingBias != 3.4028235E+38f)
				{
					this.m_Heading.m_Bias = this.m_LegacyHeadingBias;
					this.m_LegacyHeadingBias = float.MaxValue;
					int definition = (int)this.m_Heading.m_Definition;
					if (this.m_RecenterToTargetHeading.LegacyUpgrade(ref definition, ref this.m_Heading.m_VelocityFilterStrength))
					{
						this.m_Heading.m_Definition = (CinemachineOrbitalTransposer.Heading.HeadingDefinition)definition;
					}
					this.mUseLegacyRigDefinitions = true;
				}
				if (this.m_LegacyTransitions.m_BlendHint != 0)
				{
					if (this.m_LegacyTransitions.m_BlendHint == 3)
					{
						this.BlendHint = CinemachineCore.BlendHints.ScreenSpaceAimWhenTargetsDiffer;
					}
					else
					{
						this.BlendHint = (CinemachineCore.BlendHints)this.m_LegacyTransitions.m_BlendHint;
					}
					this.m_LegacyTransitions.m_BlendHint = 0;
				}
				if (this.m_LegacyTransitions.m_InheritPosition)
				{
					this.BlendHint |= CinemachineCore.BlendHints.InheritPosition;
					this.m_LegacyTransitions.m_InheritPosition = false;
				}
				if (this.m_LegacyTransitions.m_OnCameraLive != null)
				{
					this.m_OnCameraLiveEvent = this.m_LegacyTransitions.m_OnCameraLive;
					this.m_LegacyTransitions.m_OnCameraLive = null;
				}
			}
		}

		protected internal override bool IsDprecated
		{
			get
			{
				return true;
			}
		}

		private void OnValidate()
		{
			this.m_YAxis.Validate();
			this.m_XAxis.Validate();
			this.m_RecenterToTargetHeading.Validate();
			this.m_YAxisRecentering.Validate();
			this.m_Lens.Validate();
			this.InvalidateRigCache();
		}

		public CinemachineVirtualCamera GetRig(int i)
		{
			if (!this.UpdateRigCache() || i < 0 || i >= 3)
			{
				return null;
			}
			return this.m_Rigs[i];
		}

		internal bool RigsAreCreated
		{
			get
			{
				return this.m_Rigs != null && this.m_Rigs.Length == 3;
			}
		}

		public static string[] RigNames
		{
			get
			{
				return new string[]
				{
					"TopRig",
					"MiddleRig",
					"BottomRig"
				};
			}
		}

		protected override void OnEnable()
		{
			this.mIsDestroyed = false;
			base.OnEnable();
			this.InvalidateRigCache();
			this.UpdateInputAxisProvider();
		}

		internal void UpdateInputAxisProvider()
		{
			this.m_XAxis.SetInputAxisProvider(0, null);
			this.m_YAxis.SetInputAxisProvider(1, null);
			AxisState.IInputAxisProvider component = base.GetComponent<AxisState.IInputAxisProvider>();
			if (component != null)
			{
				this.m_XAxis.SetInputAxisProvider(0, component);
				this.m_YAxis.SetInputAxisProvider(1, component);
			}
		}

		protected override void OnDestroy()
		{
			if (this.m_Rigs != null)
			{
				foreach (CinemachineVirtualCamera cinemachineVirtualCamera in this.m_Rigs)
				{
					if (cinemachineVirtualCamera != null && cinemachineVirtualCamera.gameObject != null)
					{
						cinemachineVirtualCamera.gameObject.hideFlags &= ~(HideFlags.HideInHierarchy | HideFlags.HideInInspector);
					}
				}
			}
			this.mIsDestroyed = true;
			base.OnDestroy();
		}

		private void OnTransformChildrenChanged()
		{
			this.InvalidateRigCache();
		}

		private void Reset()
		{
			this.DestroyRigs();
			this.UpdateRigCache();
			this.Priority = default(PrioritySettings);
			this.OutputChannel = OutputChannels.Default;
		}

		public override bool PreviousStateIsValid
		{
			get
			{
				return base.PreviousStateIsValid;
			}
			set
			{
				if (!value)
				{
					int num = 0;
					while (this.m_Rigs != null && num < this.m_Rigs.Length)
					{
						if (this.m_Rigs[num] != null)
						{
							this.m_Rigs[num].PreviousStateIsValid = value;
						}
						num++;
					}
				}
				base.PreviousStateIsValid = value;
			}
		}

		public override CameraState State
		{
			get
			{
				return this.m_State;
			}
		}

		public override Transform LookAt
		{
			get
			{
				return base.ResolveLookAt(this.m_LookAt);
			}
			set
			{
				this.m_LookAt = value;
			}
		}

		public override Transform Follow
		{
			get
			{
				return base.ResolveFollow(this.m_Follow);
			}
			set
			{
				this.m_Follow = value;
			}
		}

		public bool IsLiveChild(ICinemachineCamera vcam, bool dominantChildOnly = false)
		{
			if (!this.RigsAreCreated)
			{
				return false;
			}
			float yaxisValue = this.GetYAxisValue();
			if (dominantChildOnly)
			{
				if (vcam == this.m_Rigs[0])
				{
					return yaxisValue > 0.666f;
				}
				if (vcam == this.m_Rigs[2])
				{
					return (double)yaxisValue < 0.333;
				}
				return vcam == this.m_Rigs[1] && yaxisValue >= 0.333f && yaxisValue <= 0.666f;
			}
			else
			{
				if (vcam == this.m_Rigs[1])
				{
					return true;
				}
				if (yaxisValue < 0.5f)
				{
					return vcam == this.m_Rigs[2];
				}
				return vcam == this.m_Rigs[0];
			}
		}

		public override void OnTargetObjectWarped(Transform target, Vector3 positionDelta)
		{
			this.UpdateRigCache();
			if (this.RigsAreCreated)
			{
				CinemachineVirtualCamera[] rigs = this.m_Rigs;
				for (int i = 0; i < rigs.Length; i++)
				{
					rigs[i].OnTargetObjectWarped(target, positionDelta);
				}
			}
			base.OnTargetObjectWarped(target, positionDelta);
		}

		public override void ForceCameraPosition(Vector3 pos, Quaternion rot)
		{
			Vector3 referenceUp = this.State.ReferenceUp;
			this.m_YAxis.Value = this.GetYAxisClosestValue(pos, referenceUp);
			this.PreviousStateIsValid = true;
			base.transform.position = pos;
			base.transform.rotation = rot;
			this.m_State.RawPosition = pos;
			this.m_State.RawOrientation = rot;
			if (this.UpdateRigCache())
			{
				if (this.m_BindingMode != BindingMode.LazyFollow)
				{
					this.m_XAxis.Value = this.mOrbitals[1].GetAxisClosestValue(pos, referenceUp);
				}
				this.PushSettingsToRigs();
				for (int i = 0; i < 3; i++)
				{
					this.m_Rigs[i].ForceCameraPosition(pos, rot);
				}
				this.InternalUpdateCameraState(referenceUp, -1f);
			}
			base.ForceCameraPosition(pos, rot);
		}

		public override void InternalUpdateCameraState(Vector3 worldUp, float deltaTime)
		{
			base.UpdateTargetCache();
			this.UpdateRigCache();
			if (!this.RigsAreCreated)
			{
				return;
			}
			if (deltaTime < 0f)
			{
				this.PreviousStateIsValid = false;
			}
			this.m_State = this.CalculateNewState(worldUp, deltaTime);
			this.m_State.BlendHint = (CameraState.BlendHints)this.BlendHint;
			if (this.Follow != null)
			{
				Vector3 b = this.State.RawPosition - base.transform.position;
				base.transform.position = this.State.RawPosition;
				this.m_Rigs[0].transform.position -= b;
				this.m_Rigs[1].transform.position -= b;
				this.m_Rigs[2].transform.position -= b;
			}
			base.InvokePostPipelineStageCallback(this, CinemachineCore.Stage.Finalize, ref this.m_State, deltaTime);
			this.PreviousStateIsValid = true;
			if (this.PreviousStateIsValid && CinemachineCore.IsLive(this) && deltaTime >= 0f && this.m_YAxis.Update(deltaTime))
			{
				this.m_YAxisRecentering.CancelRecentering();
			}
			this.PushSettingsToRigs();
			if (this.m_BindingMode == BindingMode.LazyFollow)
			{
				this.m_XAxis.Value = 0f;
			}
		}

		public override void OnTransitionFromCamera(ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime)
		{
			base.OnTransitionFromCamera(fromCam, worldUp, deltaTime);
			if (!this.RigsAreCreated)
			{
				return;
			}
			base.InvokeOnTransitionInExtensions(fromCam, worldUp, deltaTime);
			bool flag = false;
			if (fromCam != null && (this.State.BlendHint & CameraState.BlendHints.InheritPosition) != CameraState.BlendHints.Nothing && !CinemachineCore.IsLiveInBlend(this))
			{
				Vector3 pos = fromCam.State.RawPosition;
				if (fromCam is CinemachineFreeLook)
				{
					CinemachineFreeLook cinemachineFreeLook = fromCam as CinemachineFreeLook;
					CinemachineOrbitalTransposer cinemachineOrbitalTransposer = (cinemachineFreeLook.mOrbitals != null) ? cinemachineFreeLook.mOrbitals[1] : null;
					if (cinemachineOrbitalTransposer != null)
					{
						pos = cinemachineOrbitalTransposer.GetTargetCameraPosition(worldUp);
					}
				}
				this.ForceCameraPosition(pos, fromCam.State.GetFinalOrientation());
			}
			if (flag)
			{
				for (int i = 0; i < 3; i++)
				{
					this.m_Rigs[i].InternalUpdateCameraState(worldUp, deltaTime);
				}
				this.InternalUpdateCameraState(worldUp, deltaTime);
			}
			else
			{
				base.UpdateCameraState(worldUp, deltaTime);
			}
			CinemachineLegacyCameraEvents.OnCameraLiveEvent onCameraLiveEvent = this.m_OnCameraLiveEvent;
			if (onCameraLiveEvent == null)
			{
				return;
			}
			onCameraLiveEvent.Invoke(this, fromCam);
		}

		bool AxisState.IRequiresInput.RequiresInput()
		{
			return true;
		}

		private float GetYAxisClosestValue(Vector3 cameraPos, Vector3 up)
		{
			if (this.Follow != null)
			{
				Vector3 vector = Quaternion.FromToRotation(up, Vector3.up) * (cameraPos - this.Follow.position);
				Vector3 vector2 = vector;
				vector2.y = 0f;
				if (!vector2.AlmostZero())
				{
					vector = Quaternion.AngleAxis(UnityVectorExtensions.SignedAngle(vector2, Vector3.back, Vector3.up), Vector3.up) * vector;
				}
				vector.x = 0f;
				return this.SteepestDescent(vector.normalized * (cameraPos - this.Follow.position).magnitude);
			}
			return this.m_YAxis.Value;
		}

		private float SteepestDescent(Vector3 cameraOffset)
		{
			CinemachineFreeLook.<>c__DisplayClass52_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.cameraOffset = cameraOffset;
			float num = this.<SteepestDescent>g__InitialGuess|52_2(ref CS$<>8__locals1);
			for (int i = 0; i < 5; i++)
			{
				float num2 = this.<SteepestDescent>g__AngleFunction|52_0(num, ref CS$<>8__locals1);
				float num3 = this.<SteepestDescent>g__SlopeOfAngleFunction|52_1(num, ref CS$<>8__locals1);
				if (Mathf.Abs(num3) < 0.005f || Mathf.Abs(num2) < 0.005f)
				{
					break;
				}
				num = Mathf.Clamp01(num - num2 / num3);
			}
			return num;
		}

		private void InvalidateRigCache()
		{
			this.mOrbitals = null;
		}

		private void DestroyRigs()
		{
			List<CinemachineVirtualCamera> list = new List<CinemachineVirtualCamera>(3);
			for (int i = 0; i < CinemachineFreeLook.RigNames.Length; i++)
			{
				foreach (object obj in base.transform)
				{
					Transform transform = (Transform)obj;
					if (transform.gameObject.name == CinemachineFreeLook.RigNames[i])
					{
						list.Add(transform.GetComponent<CinemachineVirtualCamera>());
					}
				}
			}
			foreach (CinemachineVirtualCamera cinemachineVirtualCamera in list)
			{
				if (cinemachineVirtualCamera != null)
				{
					if (CinemachineFreeLook.DestroyRigOverride != null)
					{
						CinemachineFreeLook.DestroyRigOverride(cinemachineVirtualCamera.gameObject);
					}
					else
					{
						cinemachineVirtualCamera.DestroyPipeline();
						Object.Destroy(cinemachineVirtualCamera);
						if (!RuntimeUtility.IsPrefab(base.gameObject))
						{
							Object.Destroy(cinemachineVirtualCamera.gameObject);
						}
					}
				}
			}
			this.mOrbitals = null;
			this.m_Rigs = null;
		}

		private CinemachineVirtualCamera[] CreateRigs(CinemachineVirtualCamera[] copyFrom)
		{
			float[] array = new float[]
			{
				0.5f,
				0.55f,
				0.6f
			};
			this.mOrbitals = null;
			this.m_Rigs = null;
			CinemachineVirtualCamera[] array2 = new CinemachineVirtualCamera[3];
			for (int i = 0; i < array2.Length; i++)
			{
				CinemachineVirtualCamera cinemachineVirtualCamera = (copyFrom != null && copyFrom.Length > i) ? copyFrom[i] : null;
				if (CinemachineFreeLook.CreateRigOverride != null)
				{
					array2[i] = CinemachineFreeLook.CreateRigOverride(this, CinemachineFreeLook.RigNames[i], cinemachineVirtualCamera);
				}
				else
				{
					GameObject gameObject = null;
					foreach (object obj in base.transform)
					{
						Transform transform = (Transform)obj;
						if (transform.gameObject.name == CinemachineFreeLook.RigNames[i])
						{
							gameObject = transform.gameObject;
							break;
						}
					}
					if (gameObject == null && !RuntimeUtility.IsPrefab(base.gameObject))
					{
						gameObject = new GameObject(CinemachineFreeLook.RigNames[i]);
						gameObject.transform.parent = base.transform;
					}
					if (gameObject == null)
					{
						array2[i] = null;
					}
					else
					{
						array2[i] = gameObject.AddComponent<CinemachineVirtualCamera>();
						array2[i].AddCinemachineComponent<CinemachineOrbitalTransposer>();
						array2[i].AddCinemachineComponent<CinemachineComposer>();
					}
				}
				if (array2[i] != null)
				{
					array2[i].InvalidateComponentPipeline();
					CinemachineOrbitalTransposer cinemachineOrbitalTransposer = array2[i].GetCinemachineComponent<CinemachineOrbitalTransposer>();
					if (cinemachineOrbitalTransposer == null)
					{
						cinemachineOrbitalTransposer = array2[i].AddCinemachineComponent<CinemachineOrbitalTransposer>();
					}
					if (cinemachineVirtualCamera == null)
					{
						cinemachineOrbitalTransposer.m_YawDamping = 0f;
						CinemachineComposer cinemachineComponent = array2[i].GetCinemachineComponent<CinemachineComposer>();
						if (cinemachineComponent != null)
						{
							cinemachineComponent.m_HorizontalDamping = (cinemachineComponent.m_VerticalDamping = 0f);
							cinemachineComponent.m_ScreenX = 0.5f;
							cinemachineComponent.m_ScreenY = array[i];
							cinemachineComponent.m_DeadZoneWidth = (cinemachineComponent.m_DeadZoneHeight = 0f);
							cinemachineComponent.m_SoftZoneWidth = (cinemachineComponent.m_SoftZoneHeight = 0.6f);
							cinemachineComponent.m_BiasX = (cinemachineComponent.m_BiasY = 0f);
						}
					}
				}
			}
			return array2;
		}

		private bool UpdateRigCache()
		{
			if (this.mIsDestroyed)
			{
				return false;
			}
			if (this.mOrbitals != null && this.mOrbitals.Length == 3)
			{
				return true;
			}
			this.m_CachedXAxisHeading = 0f;
			this.m_Rigs = null;
			this.mOrbitals = null;
			List<CinemachineVirtualCamera> list = this.LocateExistingRigs(false);
			if (list == null || list.Count != 3)
			{
				this.DestroyRigs();
				this.CreateRigs(null);
				list = this.LocateExistingRigs(true);
			}
			if (list != null && list.Count == 3)
			{
				this.m_Rigs = list.ToArray();
			}
			if (this.RigsAreCreated)
			{
				this.mOrbitals = new CinemachineOrbitalTransposer[this.m_Rigs.Length];
				for (int i = 0; i < this.m_Rigs.Length; i++)
				{
					this.mOrbitals[i] = this.m_Rigs[i].GetCinemachineComponent<CinemachineOrbitalTransposer>();
				}
				foreach (CinemachineVirtualCamera cinemachineVirtualCamera in this.m_Rigs)
				{
					if (!(cinemachineVirtualCamera == null))
					{
						CinemachineVirtualCamera cinemachineVirtualCamera2 = cinemachineVirtualCamera;
						string[] excludedPropertiesInInspector;
						if (!this.m_CommonLens)
						{
							string[] array = new string[8];
							array[0] = "m_Script";
							array[1] = "Header";
							array[2] = "Extensions";
							array[3] = "Priority";
							array[4] = "OutputChannel";
							array[5] = "m_Transitions";
							array[6] = "m_Follow";
							excludedPropertiesInInspector = array;
							array[7] = "m_StandbyUpdate";
						}
						else
						{
							string[] array2 = new string[9];
							array2[0] = "m_Script";
							array2[1] = "Header";
							array2[2] = "Extensions";
							array2[3] = "Priority";
							array2[4] = "OutputChannel";
							array2[5] = "m_Transitions";
							array2[6] = "m_Follow";
							array2[7] = "m_StandbyUpdate";
							excludedPropertiesInInspector = array2;
							array2[8] = "m_Lens";
						}
						cinemachineVirtualCamera2.m_ExcludedPropertiesInInspector = excludedPropertiesInInspector;
						cinemachineVirtualCamera.m_LockStageInInspector = new CinemachineCore.Stage[1];
					}
				}
				this.mBlendA = new CinemachineBlend
				{
					CamA = this.m_Rigs[1],
					CamB = this.m_Rigs[0],
					BlendCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f),
					Duration = 1f
				};
				this.mBlendB = new CinemachineBlend
				{
					CamA = this.m_Rigs[2],
					CamB = this.m_Rigs[1],
					BlendCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f),
					Duration = 1f
				};
				return true;
			}
			return false;
		}

		private List<CinemachineVirtualCamera> LocateExistingRigs(bool forceOrbital)
		{
			this.m_CachedXAxisHeading = this.m_XAxis.Value;
			this.m_LastHeadingUpdateFrame = -1f;
			List<CinemachineVirtualCamera> list = new List<CinemachineVirtualCamera>(3);
			foreach (object obj in base.transform)
			{
				Transform transform = (Transform)obj;
				CinemachineVirtualCamera component = transform.GetComponent<CinemachineVirtualCamera>();
				if (component != null)
				{
					GameObject gameObject = transform.gameObject;
					for (int i = 0; i < CinemachineFreeLook.RigNames.Length; i++)
					{
						if (!(gameObject.name != CinemachineFreeLook.RigNames[i]))
						{
							CinemachineOrbitalTransposer cinemachineOrbitalTransposer = component.GetCinemachineComponent<CinemachineOrbitalTransposer>();
							if (cinemachineOrbitalTransposer == null && forceOrbital)
							{
								cinemachineOrbitalTransposer = component.AddCinemachineComponent<CinemachineOrbitalTransposer>();
							}
							if (cinemachineOrbitalTransposer != null)
							{
								cinemachineOrbitalTransposer.m_HeadingIsDriven = true;
								cinemachineOrbitalTransposer.HideOffsetInInspector = true;
								cinemachineOrbitalTransposer.m_XAxis.m_InputAxisName = string.Empty;
								cinemachineOrbitalTransposer.HeadingUpdater = new CinemachineOrbitalTransposer.UpdateHeadingDelegate(this.UpdateXAxisHeading);
								cinemachineOrbitalTransposer.m_RecenterToTargetHeading.m_enabled = false;
								component.StandbyUpdate = this.StandbyUpdate;
								list.Add(component);
							}
						}
					}
				}
			}
			return list;
		}

		private float UpdateXAxisHeading(CinemachineOrbitalTransposer orbital, float deltaTime, Vector3 up)
		{
			if (this == null)
			{
				return 0f;
			}
			if (this.m_LastHeadingUpdateFrame != (float)CinemachineCore.CurrentUpdateFrame)
			{
				this.m_LastHeadingUpdateFrame = (float)CinemachineCore.CurrentUpdateFrame;
				float value = this.m_XAxis.Value;
				this.m_CachedXAxisHeading = orbital.UpdateHeading(this.PreviousStateIsValid ? deltaTime : -1f, up, ref this.m_XAxis, ref this.m_RecenterToTargetHeading, CinemachineCore.IsLive(this));
				if (this.m_BindingMode == BindingMode.LazyFollow)
				{
					this.m_XAxis.Value = value;
				}
			}
			return this.m_CachedXAxisHeading;
		}

		private void PushSettingsToRigs()
		{
			for (int i = 0; i < this.m_Rigs.Length; i++)
			{
				if (this.m_CommonLens)
				{
					this.m_Rigs[i].m_Lens = this.m_Lens;
				}
				if (this.mUseLegacyRigDefinitions)
				{
					this.mUseLegacyRigDefinitions = false;
					this.m_Orbits[i].m_Height = this.mOrbitals[i].m_FollowOffset.y;
					this.m_Orbits[i].m_Radius = -this.mOrbitals[i].m_FollowOffset.z;
					if (this.m_Rigs[i].Follow != null)
					{
						this.Follow = this.m_Rigs[i].Follow;
					}
				}
				this.m_Rigs[i].Follow = null;
				this.m_Rigs[i].StandbyUpdate = this.StandbyUpdate;
				this.m_Rigs[i].FollowTargetAttachment = this.FollowTargetAttachment;
				this.m_Rigs[i].LookAtTargetAttachment = this.LookAtTargetAttachment;
				if (!this.PreviousStateIsValid)
				{
					this.m_Rigs[i].PreviousStateIsValid = false;
					this.m_Rigs[i].transform.position = base.transform.position;
					this.m_Rigs[i].transform.rotation = base.transform.rotation;
				}
				this.mOrbitals[i].m_FollowOffset = this.GetLocalPositionForCameraFromInput(this.GetYAxisValue());
				this.mOrbitals[i].m_BindingMode = this.m_BindingMode;
				this.mOrbitals[i].m_Heading = this.m_Heading;
				this.mOrbitals[i].m_XAxis.Value = this.m_XAxis.Value;
				if (this.m_BindingMode == BindingMode.LazyFollow)
				{
					this.m_Rigs[i].SetStateRawPosition(this.State.RawPosition);
				}
			}
		}

		private float GetYAxisValue()
		{
			float num = this.m_YAxis.m_MaxValue - this.m_YAxis.m_MinValue;
			if (num <= 0.0001f)
			{
				return 0.5f;
			}
			return this.m_YAxis.Value / num;
		}

		private CameraState CalculateNewState(Vector3 worldUp, float deltaTime)
		{
			this.m_LensSettings = this.m_Lens.ToLensSettings();
			CameraState result = base.PullStateFromVirtualCamera(worldUp, ref this.m_LensSettings);
			this.m_YAxisRecentering.DoRecentering(ref this.m_YAxis, deltaTime, 0.5f);
			float yaxisValue = this.GetYAxisValue();
			if (yaxisValue > 0.5f)
			{
				if (this.mBlendA != null)
				{
					this.mBlendA.TimeInBlend = (yaxisValue - 0.5f) * 2f;
					this.mBlendA.UpdateCameraState(worldUp, deltaTime);
					result = this.mBlendA.State;
				}
			}
			else if (this.mBlendB != null)
			{
				this.mBlendB.TimeInBlend = yaxisValue * 2f;
				this.mBlendB.UpdateCameraState(worldUp, deltaTime);
				result = this.mBlendB.State;
			}
			return result;
		}

		public Vector3 GetLocalPositionForCameraFromInput(float t)
		{
			if (this.mOrbitals == null)
			{
				return Vector3.zero;
			}
			this.UpdateCachedSpline();
			int num = 1;
			if (t > 0.5f)
			{
				t -= 0.5f;
				num = 2;
			}
			return SplineHelpers.Bezier3(t * 2f, this.m_CachedKnots[num], this.m_CachedCtrl1[num], this.m_CachedCtrl2[num], this.m_CachedKnots[num + 1]);
		}

		private void UpdateCachedSpline()
		{
			bool flag = this.m_CachedOrbits != null && this.m_CachedOrbits.Length == 3 && this.m_CachedTension == this.m_SplineCurvature;
			int num = 0;
			while (num < 3 && flag)
			{
				flag = (this.m_CachedOrbits[num].m_Height == this.m_Orbits[num].m_Height && this.m_CachedOrbits[num].m_Radius == this.m_Orbits[num].m_Radius);
				num++;
			}
			if (!flag)
			{
				float splineCurvature = this.m_SplineCurvature;
				this.m_CachedKnots = new Vector4[5];
				this.m_CachedCtrl1 = new Vector4[5];
				this.m_CachedCtrl2 = new Vector4[5];
				this.m_CachedKnots[1] = new Vector4(0f, this.m_Orbits[2].m_Height, -this.m_Orbits[2].m_Radius, 0f);
				this.m_CachedKnots[2] = new Vector4(0f, this.m_Orbits[1].m_Height, -this.m_Orbits[1].m_Radius, 0f);
				this.m_CachedKnots[3] = new Vector4(0f, this.m_Orbits[0].m_Height, -this.m_Orbits[0].m_Radius, 0f);
				this.m_CachedKnots[0] = Vector4.Lerp(this.m_CachedKnots[1], Vector4.zero, splineCurvature);
				this.m_CachedKnots[4] = Vector4.Lerp(this.m_CachedKnots[3], Vector4.zero, splineCurvature);
				SplineHelpers.ComputeSmoothControlPoints(ref this.m_CachedKnots, ref this.m_CachedCtrl1, ref this.m_CachedCtrl2);
				this.m_CachedOrbits = new CinemachineFreeLook.Orbit[3];
				for (int i = 0; i < 3; i++)
				{
					this.m_CachedOrbits[i] = this.m_Orbits[i];
				}
				this.m_CachedTension = this.m_SplineCurvature;
			}
		}

		[CompilerGenerated]
		private float <SteepestDescent>g__AngleFunction|52_0(float input, ref CinemachineFreeLook.<>c__DisplayClass52_0 A_2)
		{
			Vector3 localPositionForCameraFromInput = this.GetLocalPositionForCameraFromInput(input);
			return Mathf.Abs(UnityVectorExtensions.SignedAngle(A_2.cameraOffset, localPositionForCameraFromInput, Vector3.right));
		}

		[CompilerGenerated]
		private float <SteepestDescent>g__SlopeOfAngleFunction|52_1(float input, ref CinemachineFreeLook.<>c__DisplayClass52_0 A_2)
		{
			float num = this.<SteepestDescent>g__AngleFunction|52_0(input - 0.005f, ref A_2);
			return (this.<SteepestDescent>g__AngleFunction|52_0(input + 0.005f, ref A_2) - num) / 0.01f;
		}

		[CompilerGenerated]
		private float <SteepestDescent>g__InitialGuess|52_2(ref CinemachineFreeLook.<>c__DisplayClass52_0 A_1)
		{
			this.UpdateCachedSpline();
			CinemachineFreeLook.<>c__DisplayClass52_1 CS$<>8__locals1;
			CS$<>8__locals1.best = 0.5f;
			CS$<>8__locals1.bestAngle = this.<SteepestDescent>g__AngleFunction|52_0(CS$<>8__locals1.best, ref A_1);
			for (int i = 0; i <= 5; i++)
			{
				float num = (float)i * 0.1f;
				this.<SteepestDescent>g__ChooseBestAngle|52_3(0.5f + num, ref A_1, ref CS$<>8__locals1);
				this.<SteepestDescent>g__ChooseBestAngle|52_3(0.5f - num, ref A_1, ref CS$<>8__locals1);
			}
			return CS$<>8__locals1.best;
		}

		[CompilerGenerated]
		private void <SteepestDescent>g__ChooseBestAngle|52_3(float x, ref CinemachineFreeLook.<>c__DisplayClass52_0 A_2, ref CinemachineFreeLook.<>c__DisplayClass52_1 A_3)
		{
			float num = this.<SteepestDescent>g__AngleFunction|52_0(x, ref A_2);
			if (num < A_3.bestAngle)
			{
				A_3.bestAngle = num;
				A_3.best = x;
			}
		}

		[Tooltip("Object for the camera children to look at (the aim target).")]
		[NoSaveDuringPlay]
		[VcamTargetProperty]
		public Transform m_LookAt;

		[Tooltip("Object for the camera children wants to move with (the body target).")]
		[NoSaveDuringPlay]
		[VcamTargetProperty]
		public Transform m_Follow;

		[Tooltip("If enabled, this lens setting will apply to all three child rigs, otherwise the child rig lens settings will be used")]
		[FormerlySerializedAs("m_UseCommonLensSetting")]
		public bool m_CommonLens = true;

		[Tooltip("Specifies the lens properties of this Virtual Camera.  This generally mirrors the Unity Camera's lens settings, and will be used to drive the Unity camera when the vcam is active")]
		[FormerlySerializedAs("m_LensAttributes")]
		public LegacyLensSettings m_Lens = LegacyLensSettings.Default;

		[Tooltip("Hint for transitioning to and from this CinemachineCamera.  Hints can be combined, although not all combinations make sense.  In the case of conflicting hints, Cinemachine will make an arbitrary choice.")]
		public CinemachineCore.BlendHints BlendHint;

		[Tooltip("This event fires when a transition occurs")]
		public CinemachineLegacyCameraEvents.OnCameraLiveEvent m_OnCameraLiveEvent = new CinemachineLegacyCameraEvents.OnCameraLiveEvent();

		[Header("Axis Control")]
		[Tooltip("The Vertical axis.  Value is 0..1.  Chooses how to blend the child rigs")]
		public AxisState m_YAxis = new AxisState(0f, 1f, false, true, 2f, 0.2f, 0.1f, "Mouse Y", false);

		[Tooltip("Controls how automatic recentering of the Y axis is accomplished")]
		public AxisState.Recentering m_YAxisRecentering = new AxisState.Recentering(false, 1f, 2f);

		[Tooltip("The Horizontal axis.  Value is -180...180.  This is passed on to the rigs' OrbitalTransposer component")]
		public AxisState m_XAxis = new AxisState(-180f, 180f, true, false, 300f, 0.1f, 0.1f, "Mouse X", true);

		[Tooltip("The definition of Forward.  Camera will follow behind.")]
		public CinemachineOrbitalTransposer.Heading m_Heading = new CinemachineOrbitalTransposer.Heading(CinemachineOrbitalTransposer.Heading.HeadingDefinition.TargetForward, 4, 0f);

		[Tooltip("Controls how automatic recentering of the X axis is accomplished")]
		public AxisState.Recentering m_RecenterToTargetHeading = new AxisState.Recentering(false, 1f, 2f);

		[Header("Orbits")]
		[Tooltip("The coordinate space to use when interpreting the offset from the target.  This is also used to set the camera's Up vector, which will be maintained when aiming the camera.")]
		public BindingMode m_BindingMode = BindingMode.LazyFollow;

		[Tooltip("Controls how taut is the line that connects the rigs' orbits, which determines final placement on the Y axis")]
		[Range(0f, 1f)]
		[FormerlySerializedAs("m_SplineTension")]
		public float m_SplineCurvature = 0.2f;

		[Tooltip("The radius and height of the three orbiting rigs.")]
		public CinemachineFreeLook.Orbit[] m_Orbits = new CinemachineFreeLook.Orbit[]
		{
			new CinemachineFreeLook.Orbit(4.5f, 1.75f),
			new CinemachineFreeLook.Orbit(2.5f, 3f),
			new CinemachineFreeLook.Orbit(0.4f, 1.3f)
		};

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("m_HeadingBias")]
		private float m_LegacyHeadingBias = float.MaxValue;

		private bool mUseLegacyRigDefinitions;

		[SerializeField]
		[HideInInspector]
		private CinemachineFreeLook.LegacyTransitionParams m_LegacyTransitions;

		private bool mIsDestroyed;

		private CameraState m_State = CameraState.Default;

		[SerializeField]
		[HideInInspector]
		[NoSaveDuringPlay]
		private CinemachineVirtualCamera[] m_Rigs = new CinemachineVirtualCamera[3];

		private CinemachineOrbitalTransposer[] mOrbitals;

		private CinemachineBlend mBlendA;

		private CinemachineBlend mBlendB;

		public static CinemachineFreeLook.CreateRigDelegate CreateRigOverride;

		public static CinemachineFreeLook.DestroyRigDelegate DestroyRigOverride;

		private float m_CachedXAxisHeading;

		private float m_LastHeadingUpdateFrame;

		private LensSettings m_LensSettings;

		private CinemachineFreeLook.Orbit[] m_CachedOrbits;

		private float m_CachedTension;

		private Vector4[] m_CachedKnots;

		private Vector4[] m_CachedCtrl1;

		private Vector4[] m_CachedCtrl2;

		[Serializable]
		public struct Orbit
		{
			public Orbit(float h, float r)
			{
				this.m_Height = h;
				this.m_Radius = r;
			}

			public float m_Height;

			public float m_Radius;
		}

		[Serializable]
		private struct LegacyTransitionParams
		{
			[FormerlySerializedAs("m_PositionBlending")]
			public int m_BlendHint;

			public bool m_InheritPosition;

			public CinemachineLegacyCameraEvents.OnCameraLiveEvent m_OnCameraLive;
		}

		public delegate CinemachineVirtualCamera CreateRigDelegate(CinemachineFreeLook vcam, string name, CinemachineVirtualCamera copyFrom);

		public delegate void DestroyRigDelegate(GameObject rig);
	}
}
