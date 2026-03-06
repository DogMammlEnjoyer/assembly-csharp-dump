using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Action_Pose_Source : SteamVR_Action_In_Source, ISteamVR_Action_Pose, ISteamVR_Action_In_Source, ISteamVR_Action_Source
	{
		public event SteamVR_Action_Pose.ActiveChangeHandler onActiveChange;

		public event SteamVR_Action_Pose.ActiveChangeHandler onActiveBindingChange;

		public event SteamVR_Action_Pose.ChangeHandler onChange;

		public event SteamVR_Action_Pose.UpdateHandler onUpdate;

		public event SteamVR_Action_Pose.TrackingChangeHandler onTrackingChanged;

		public event SteamVR_Action_Pose.ValidPoseChangeHandler onValidPoseChanged;

		public event SteamVR_Action_Pose.DeviceConnectedChangeHandler onDeviceConnectedChanged;

		public override bool changed { get; protected set; }

		public override bool lastChanged { get; protected set; }

		public override ulong activeOrigin
		{
			get
			{
				if (this.active)
				{
					return this.poseActionData.activeOrigin;
				}
				return 0UL;
			}
		}

		public override ulong lastActiveOrigin
		{
			get
			{
				return this.lastPoseActionData.activeOrigin;
			}
		}

		public override bool active
		{
			get
			{
				return this.activeBinding && this.action.actionSet.IsActive(base.inputSource);
			}
		}

		public override bool activeBinding
		{
			get
			{
				return this.poseActionData.bActive;
			}
		}

		public override bool lastActive { get; protected set; }

		public override bool lastActiveBinding
		{
			get
			{
				return this.lastPoseActionData.bActive;
			}
		}

		public ETrackingResult trackingState
		{
			get
			{
				return this.poseActionData.pose.eTrackingResult;
			}
		}

		public ETrackingResult lastTrackingState
		{
			get
			{
				return this.lastPoseActionData.pose.eTrackingResult;
			}
		}

		public bool poseIsValid
		{
			get
			{
				return this.poseActionData.pose.bPoseIsValid;
			}
		}

		public bool lastPoseIsValid
		{
			get
			{
				return this.lastPoseActionData.pose.bPoseIsValid;
			}
		}

		public bool deviceIsConnected
		{
			get
			{
				return this.poseActionData.pose.bDeviceIsConnected;
			}
		}

		public bool lastDeviceIsConnected
		{
			get
			{
				return this.lastPoseActionData.pose.bDeviceIsConnected;
			}
		}

		public Vector3 localPosition { get; protected set; }

		public Quaternion localRotation { get; protected set; }

		public Vector3 lastLocalPosition { get; protected set; }

		public Quaternion lastLocalRotation { get; protected set; }

		public Vector3 velocity { get; protected set; }

		public Vector3 lastVelocity { get; protected set; }

		public Vector3 angularVelocity { get; protected set; }

		public Vector3 lastAngularVelocity { get; protected set; }

		public override void Preinitialize(SteamVR_Action wrappingAction, SteamVR_Input_Sources forInputSource)
		{
			base.Preinitialize(wrappingAction, forInputSource);
			this.poseAction = (wrappingAction as SteamVR_Action_Pose);
		}

		public override void Initialize()
		{
			base.Initialize();
			if (SteamVR_Action_Pose_Source.poseActionData_size == 0U)
			{
				SteamVR_Action_Pose_Source.poseActionData_size = (uint)Marshal.SizeOf(typeof(InputPoseActionData_t));
			}
		}

		public virtual void RemoveAllListeners()
		{
			if (this.onActiveChange != null)
			{
				Delegate[] invocationList = this.onActiveChange.GetInvocationList();
				if (invocationList != null)
				{
					foreach (Delegate @delegate in invocationList)
					{
						this.onActiveChange -= (SteamVR_Action_Pose.ActiveChangeHandler)@delegate;
					}
				}
			}
			if (this.onChange != null)
			{
				Delegate[] invocationList = this.onChange.GetInvocationList();
				if (invocationList != null)
				{
					foreach (Delegate delegate2 in invocationList)
					{
						this.onChange -= (SteamVR_Action_Pose.ChangeHandler)delegate2;
					}
				}
			}
			if (this.onUpdate != null)
			{
				Delegate[] invocationList = this.onUpdate.GetInvocationList();
				if (invocationList != null)
				{
					foreach (Delegate delegate3 in invocationList)
					{
						this.onUpdate -= (SteamVR_Action_Pose.UpdateHandler)delegate3;
					}
				}
			}
			if (this.onTrackingChanged != null)
			{
				Delegate[] invocationList = this.onTrackingChanged.GetInvocationList();
				if (invocationList != null)
				{
					foreach (Delegate delegate4 in invocationList)
					{
						this.onTrackingChanged -= (SteamVR_Action_Pose.TrackingChangeHandler)delegate4;
					}
				}
			}
			if (this.onValidPoseChanged != null)
			{
				Delegate[] invocationList = this.onValidPoseChanged.GetInvocationList();
				if (invocationList != null)
				{
					foreach (Delegate delegate5 in invocationList)
					{
						this.onValidPoseChanged -= (SteamVR_Action_Pose.ValidPoseChangeHandler)delegate5;
					}
				}
			}
			if (this.onDeviceConnectedChanged != null)
			{
				Delegate[] invocationList = this.onDeviceConnectedChanged.GetInvocationList();
				if (invocationList != null)
				{
					foreach (Delegate delegate6 in invocationList)
					{
						this.onDeviceConnectedChanged -= (SteamVR_Action_Pose.DeviceConnectedChangeHandler)delegate6;
					}
				}
			}
		}

		public override void UpdateValue()
		{
			this.UpdateValue(false);
		}

		public virtual void UpdateValue(bool skipStateAndEventUpdates)
		{
			this.lastChanged = this.changed;
			this.lastPoseActionData = this.poseActionData;
			this.lastLocalPosition = this.localPosition;
			this.lastLocalRotation = this.localRotation;
			this.lastVelocity = this.velocity;
			this.lastAngularVelocity = this.angularVelocity;
			EVRInputError evrinputError;
			if (SteamVR_Action_Pose_Source.framesAhead == 0f)
			{
				evrinputError = OpenVR.Input.GetPoseActionDataForNextFrame(base.handle, this.universeOrigin, ref this.poseActionData, SteamVR_Action_Pose_Source.poseActionData_size, this.inputSourceHandle);
			}
			else
			{
				evrinputError = OpenVR.Input.GetPoseActionDataRelativeToNow(base.handle, this.universeOrigin, SteamVR_Action_Pose_Source.framesAhead * (Time.timeScale / SteamVR.instance.hmd_DisplayFrequency), ref this.poseActionData, SteamVR_Action_Pose_Source.poseActionData_size, this.inputSourceHandle);
			}
			if (evrinputError != EVRInputError.None)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"<b>[SteamVR]</b> GetPoseActionData error (",
					base.fullPath,
					"): ",
					evrinputError.ToString(),
					" Handle: ",
					base.handle.ToString(),
					". Input source: ",
					base.inputSource.ToString()
				}));
			}
			if (this.active)
			{
				this.SetCacheVariables();
				this.changed = this.GetChanged();
			}
			if (this.changed)
			{
				base.changedTime = base.updateTime;
			}
			if (!skipStateAndEventUpdates)
			{
				this.CheckAndSendEvents();
			}
		}

		protected void SetCacheVariables()
		{
			this.localPosition = this.poseActionData.pose.mDeviceToAbsoluteTracking.GetPosition();
			this.localRotation = this.poseActionData.pose.mDeviceToAbsoluteTracking.GetRotation();
			this.velocity = this.GetUnityCoordinateVelocity(this.poseActionData.pose.vVelocity);
			this.angularVelocity = this.GetUnityCoordinateAngularVelocity(this.poseActionData.pose.vAngularVelocity);
			base.updateTime = Time.realtimeSinceStartup;
		}

		protected bool GetChanged()
		{
			return Vector3.Distance(this.localPosition, this.lastLocalPosition) > this.changeTolerance || Mathf.Abs(Quaternion.Angle(this.localRotation, this.lastLocalRotation)) > this.changeTolerance || Vector3.Distance(this.velocity, this.lastVelocity) > this.changeTolerance || Vector3.Distance(this.angularVelocity, this.lastAngularVelocity) > this.changeTolerance;
		}

		public bool GetVelocitiesAtTimeOffset(float secondsFromNow, out Vector3 velocityAtTime, out Vector3 angularVelocityAtTime)
		{
			EVRInputError poseActionDataRelativeToNow = OpenVR.Input.GetPoseActionDataRelativeToNow(base.handle, this.universeOrigin, secondsFromNow, ref this.tempPoseActionData, SteamVR_Action_Pose_Source.poseActionData_size, this.inputSourceHandle);
			if (poseActionDataRelativeToNow != EVRInputError.None)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"<b>[SteamVR]</b> GetPoseActionData error (",
					base.fullPath,
					"): ",
					poseActionDataRelativeToNow.ToString(),
					" handle: ",
					base.handle.ToString()
				}));
				velocityAtTime = Vector3.zero;
				angularVelocityAtTime = Vector3.zero;
				return false;
			}
			velocityAtTime = this.GetUnityCoordinateVelocity(this.tempPoseActionData.pose.vVelocity);
			angularVelocityAtTime = this.GetUnityCoordinateAngularVelocity(this.tempPoseActionData.pose.vAngularVelocity);
			return true;
		}

		public bool GetPoseAtTimeOffset(float secondsFromNow, out Vector3 positionAtTime, out Quaternion rotationAtTime, out Vector3 velocityAtTime, out Vector3 angularVelocityAtTime)
		{
			EVRInputError poseActionDataRelativeToNow = OpenVR.Input.GetPoseActionDataRelativeToNow(base.handle, this.universeOrigin, secondsFromNow, ref this.tempPoseActionData, SteamVR_Action_Pose_Source.poseActionData_size, this.inputSourceHandle);
			if (poseActionDataRelativeToNow != EVRInputError.None)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"<b>[SteamVR]</b> GetPoseActionData error (",
					base.fullPath,
					"): ",
					poseActionDataRelativeToNow.ToString(),
					" handle: ",
					base.handle.ToString()
				}));
				velocityAtTime = Vector3.zero;
				angularVelocityAtTime = Vector3.zero;
				positionAtTime = Vector3.zero;
				rotationAtTime = Quaternion.identity;
				return false;
			}
			velocityAtTime = this.GetUnityCoordinateVelocity(this.tempPoseActionData.pose.vVelocity);
			angularVelocityAtTime = this.GetUnityCoordinateAngularVelocity(this.tempPoseActionData.pose.vAngularVelocity);
			positionAtTime = this.tempPoseActionData.pose.mDeviceToAbsoluteTracking.GetPosition();
			rotationAtTime = this.tempPoseActionData.pose.mDeviceToAbsoluteTracking.GetRotation();
			return true;
		}

		public void UpdateTransform(Transform transformToUpdate)
		{
			transformToUpdate.localPosition = this.localPosition;
			transformToUpdate.localRotation = this.localRotation;
		}

		protected virtual void CheckAndSendEvents()
		{
			if (this.trackingState != this.lastTrackingState && this.onTrackingChanged != null)
			{
				this.onTrackingChanged(this.poseAction, base.inputSource, this.trackingState);
			}
			if (this.poseIsValid != this.lastPoseIsValid && this.onValidPoseChanged != null)
			{
				this.onValidPoseChanged(this.poseAction, base.inputSource, this.poseIsValid);
			}
			if (this.deviceIsConnected != this.lastDeviceIsConnected && this.onDeviceConnectedChanged != null)
			{
				this.onDeviceConnectedChanged(this.poseAction, base.inputSource, this.deviceIsConnected);
			}
			if (this.changed && this.onChange != null)
			{
				this.onChange(this.poseAction, base.inputSource);
			}
			if (this.active != this.lastActive && this.onActiveChange != null)
			{
				this.onActiveChange(this.poseAction, base.inputSource, this.active);
			}
			if (this.activeBinding != this.lastActiveBinding && this.onActiveBindingChange != null)
			{
				this.onActiveBindingChange(this.poseAction, base.inputSource, this.activeBinding);
			}
			if (this.onUpdate != null)
			{
				this.onUpdate(this.poseAction, base.inputSource);
			}
		}

		protected Vector3 GetUnityCoordinateVelocity(HmdVector3_t vector)
		{
			return this.GetUnityCoordinateVelocity(vector.v0, vector.v1, vector.v2);
		}

		protected Vector3 GetUnityCoordinateAngularVelocity(HmdVector3_t vector)
		{
			return this.GetUnityCoordinateAngularVelocity(vector.v0, vector.v1, vector.v2);
		}

		protected Vector3 GetUnityCoordinateVelocity(float x, float y, float z)
		{
			return new Vector3
			{
				x = x,
				y = y,
				z = -z
			};
		}

		protected Vector3 GetUnityCoordinateAngularVelocity(float x, float y, float z)
		{
			return new Vector3
			{
				x = -x,
				y = -y,
				z = z
			};
		}

		public ETrackingUniverseOrigin universeOrigin = ETrackingUniverseOrigin.TrackingUniverseRawAndUncalibrated;

		protected static uint poseActionData_size = 0U;

		public float changeTolerance = Mathf.Epsilon;

		protected InputPoseActionData_t poseActionData;

		protected InputPoseActionData_t lastPoseActionData;

		protected InputPoseActionData_t tempPoseActionData;

		protected SteamVR_Action_Pose poseAction;

		public static float framesAhead = 2f;
	}
}
