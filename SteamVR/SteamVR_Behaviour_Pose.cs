using System;
using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Behaviour_Pose : MonoBehaviour
	{
		public bool isValid
		{
			get
			{
				return this.poseAction[this.inputSource].poseIsValid;
			}
		}

		public bool isActive
		{
			get
			{
				return this.poseAction[this.inputSource].active;
			}
		}

		protected virtual void Start()
		{
			if (this.poseAction == null)
			{
				Debug.LogError("<b>[SteamVR]</b> No pose action set for this component", this);
				return;
			}
			this.CheckDeviceIndex();
			if (this.origin == null)
			{
				this.origin = base.transform.parent;
			}
		}

		protected virtual void OnEnable()
		{
			SteamVR.Initialize(false);
			if (this.poseAction != null)
			{
				this.poseAction[this.inputSource].onUpdate += this.SteamVR_Behaviour_Pose_OnUpdate;
				this.poseAction[this.inputSource].onDeviceConnectedChanged += this.OnDeviceConnectedChanged;
				this.poseAction[this.inputSource].onTrackingChanged += this.OnTrackingChanged;
				this.poseAction[this.inputSource].onChange += this.SteamVR_Behaviour_Pose_OnChange;
			}
		}

		protected virtual void OnDisable()
		{
			if (this.poseAction != null)
			{
				this.poseAction[this.inputSource].onUpdate -= this.SteamVR_Behaviour_Pose_OnUpdate;
				this.poseAction[this.inputSource].onDeviceConnectedChanged -= this.OnDeviceConnectedChanged;
				this.poseAction[this.inputSource].onTrackingChanged -= this.OnTrackingChanged;
				this.poseAction[this.inputSource].onChange -= this.SteamVR_Behaviour_Pose_OnChange;
			}
			this.historyBuffer.Clear();
		}

		private void SteamVR_Behaviour_Pose_OnUpdate(SteamVR_Action_Pose fromAction, SteamVR_Input_Sources fromSource)
		{
			this.UpdateHistoryBuffer();
			this.UpdateTransform();
			if (this.onTransformUpdated != null)
			{
				this.onTransformUpdated.Invoke(this, this.inputSource);
			}
			if (this.onTransformUpdatedEvent != null)
			{
				this.onTransformUpdatedEvent(this, this.inputSource);
			}
		}

		protected virtual void UpdateTransform()
		{
			this.CheckDeviceIndex();
			if (this.origin != null)
			{
				base.transform.position = this.origin.transform.TransformPoint(this.poseAction[this.inputSource].localPosition);
				base.transform.rotation = this.origin.rotation * this.poseAction[this.inputSource].localRotation;
				return;
			}
			base.transform.localPosition = this.poseAction[this.inputSource].localPosition;
			base.transform.localRotation = this.poseAction[this.inputSource].localRotation;
		}

		private void SteamVR_Behaviour_Pose_OnChange(SteamVR_Action_Pose fromAction, SteamVR_Input_Sources fromSource)
		{
			if (this.onTransformChanged != null)
			{
				this.onTransformChanged.Invoke(this, fromSource);
			}
			if (this.onTransformChangedEvent != null)
			{
				this.onTransformChangedEvent(this, fromSource);
			}
		}

		protected virtual void OnDeviceConnectedChanged(SteamVR_Action_Pose changedAction, SteamVR_Input_Sources changedSource, bool connected)
		{
			this.CheckDeviceIndex();
			if (this.onConnectedChanged != null)
			{
				this.onConnectedChanged.Invoke(this, this.inputSource, connected);
			}
			if (this.onConnectedChangedEvent != null)
			{
				this.onConnectedChangedEvent(this, this.inputSource, connected);
			}
		}

		protected virtual void OnTrackingChanged(SteamVR_Action_Pose changedAction, SteamVR_Input_Sources changedSource, ETrackingResult trackingChanged)
		{
			if (this.onTrackingChanged != null)
			{
				this.onTrackingChanged.Invoke(this, this.inputSource, trackingChanged);
			}
			if (this.onTrackingChangedEvent != null)
			{
				this.onTrackingChangedEvent(this, this.inputSource, trackingChanged);
			}
		}

		protected virtual void CheckDeviceIndex()
		{
			if (this.poseAction[this.inputSource].active && this.poseAction[this.inputSource].deviceIsConnected)
			{
				int trackedDeviceIndex = (int)this.poseAction[this.inputSource].trackedDeviceIndex;
				if (this.deviceIndex != trackedDeviceIndex)
				{
					this.deviceIndex = trackedDeviceIndex;
					if (this.broadcastDeviceChanges)
					{
						base.gameObject.BroadcastMessage("SetInputSource", this.inputSource, SendMessageOptions.DontRequireReceiver);
						base.gameObject.BroadcastMessage("SetDeviceIndex", this.deviceIndex, SendMessageOptions.DontRequireReceiver);
					}
					if (this.onDeviceIndexChanged != null)
					{
						this.onDeviceIndexChanged.Invoke(this, this.inputSource, this.deviceIndex);
					}
					if (this.onDeviceIndexChangedEvent != null)
					{
						this.onDeviceIndexChangedEvent(this, this.inputSource, this.deviceIndex);
					}
				}
			}
		}

		public int GetDeviceIndex()
		{
			if (this.deviceIndex == -1)
			{
				this.CheckDeviceIndex();
			}
			return this.deviceIndex;
		}

		public Vector3 GetVelocity()
		{
			return this.poseAction[this.inputSource].velocity;
		}

		public Vector3 GetAngularVelocity()
		{
			return this.poseAction[this.inputSource].angularVelocity;
		}

		public bool GetVelocitiesAtTimeOffset(float secondsFromNow, out Vector3 velocity, out Vector3 angularVelocity)
		{
			return this.poseAction[this.inputSource].GetVelocitiesAtTimeOffset(secondsFromNow, out velocity, out angularVelocity);
		}

		public void GetEstimatedPeakVelocities(out Vector3 velocity, out Vector3 angularVelocity)
		{
			int topVelocity = this.historyBuffer.GetTopVelocity(10, 1);
			this.historyBuffer.GetAverageVelocities(out velocity, out angularVelocity, 2, topVelocity);
		}

		protected void UpdateHistoryBuffer()
		{
			int frameCount = Time.frameCount;
			if (this.lastFrameUpdated != frameCount)
			{
				this.historyBuffer.Update(this.poseAction[this.inputSource].localPosition, this.poseAction[this.inputSource].localRotation, this.poseAction[this.inputSource].velocity, this.poseAction[this.inputSource].angularVelocity);
				this.lastFrameUpdated = frameCount;
			}
		}

		public string GetLocalizedName(params EVRInputStringBits[] localizedParts)
		{
			if (this.poseAction != null)
			{
				return this.poseAction.GetLocalizedOriginPart(this.inputSource, localizedParts);
			}
			return null;
		}

		public SteamVR_Action_Pose poseAction = SteamVR_Input.GetAction<SteamVR_Action_Pose>("Pose", false);

		[Tooltip("The device this action should apply to. Any if the action is not device specific.")]
		public SteamVR_Input_Sources inputSource;

		[Tooltip("If not set, relative to parent")]
		public Transform origin;

		public SteamVR_Behaviour_PoseEvent onTransformUpdated;

		public SteamVR_Behaviour_PoseEvent onTransformChanged;

		public SteamVR_Behaviour_Pose_ConnectedChangedEvent onConnectedChanged;

		public SteamVR_Behaviour_Pose_TrackingChangedEvent onTrackingChanged;

		public SteamVR_Behaviour_Pose_DeviceIndexChangedEvent onDeviceIndexChanged;

		public SteamVR_Behaviour_Pose.UpdateHandler onTransformUpdatedEvent;

		public SteamVR_Behaviour_Pose.ChangeHandler onTransformChangedEvent;

		public SteamVR_Behaviour_Pose.DeviceConnectedChangeHandler onConnectedChangedEvent;

		public SteamVR_Behaviour_Pose.TrackingChangeHandler onTrackingChangedEvent;

		public SteamVR_Behaviour_Pose.DeviceIndexChangedHandler onDeviceIndexChangedEvent;

		[Tooltip("Can be disabled to stop broadcasting bound device status changes")]
		public bool broadcastDeviceChanges = true;

		protected int deviceIndex = -1;

		protected SteamVR_HistoryBuffer historyBuffer = new SteamVR_HistoryBuffer(30);

		protected int lastFrameUpdated;

		public delegate void ActiveChangeHandler(SteamVR_Behaviour_Pose fromAction, SteamVR_Input_Sources fromSource, bool active);

		public delegate void ChangeHandler(SteamVR_Behaviour_Pose fromAction, SteamVR_Input_Sources fromSource);

		public delegate void UpdateHandler(SteamVR_Behaviour_Pose fromAction, SteamVR_Input_Sources fromSource);

		public delegate void TrackingChangeHandler(SteamVR_Behaviour_Pose fromAction, SteamVR_Input_Sources fromSource, ETrackingResult trackingState);

		public delegate void ValidPoseChangeHandler(SteamVR_Behaviour_Pose fromAction, SteamVR_Input_Sources fromSource, bool validPose);

		public delegate void DeviceConnectedChangeHandler(SteamVR_Behaviour_Pose fromAction, SteamVR_Input_Sources fromSource, bool deviceConnected);

		public delegate void DeviceIndexChangedHandler(SteamVR_Behaviour_Pose fromAction, SteamVR_Input_Sources fromSource, int newDeviceIndex);
	}
}
