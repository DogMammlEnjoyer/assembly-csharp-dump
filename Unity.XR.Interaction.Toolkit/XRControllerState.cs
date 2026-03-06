using System;
using UnityEngine.SpatialTracking;

namespace UnityEngine.XR.Interaction.Toolkit
{
	[Serializable]
	public class XRControllerState
	{
		protected XRControllerState(double time, Vector3 position, Quaternion rotation, InputTrackingState inputTrackingState, bool isTracked)
		{
			this.time = time;
			this.position = position;
			this.rotation = rotation;
			this.inputTrackingState = inputTrackingState;
			this.isTracked = isTracked;
		}

		public XRControllerState() : this(0.0, Vector3.zero, Quaternion.identity, InputTrackingState.None, false)
		{
		}

		public XRControllerState(XRControllerState value)
		{
			this.time = value.time;
			this.position = value.position;
			this.rotation = value.rotation;
			this.inputTrackingState = value.inputTrackingState;
			this.isTracked = value.isTracked;
			this.selectInteractionState = value.selectInteractionState;
			this.activateInteractionState = value.activateInteractionState;
			this.uiPressInteractionState = value.uiPressInteractionState;
			this.uiScrollValue = value.uiScrollValue;
		}

		public XRControllerState(double time, Vector3 position, Quaternion rotation, InputTrackingState inputTrackingState, bool isTracked, bool selectActive, bool activateActive, bool pressActive) : this(time, position, rotation, inputTrackingState, isTracked)
		{
			this.selectInteractionState.SetFrameState(selectActive);
			this.activateInteractionState.SetFrameState(activateActive);
			this.uiPressInteractionState.SetFrameState(pressActive);
		}

		public XRControllerState(double time, Vector3 position, Quaternion rotation, InputTrackingState inputTrackingState, bool isTracked, bool selectActive, bool activateActive, bool pressActive, float selectValue, float activateValue, float pressValue) : this(time, position, rotation, inputTrackingState, isTracked)
		{
			this.selectInteractionState.SetFrameState(selectActive, selectValue);
			this.activateInteractionState.SetFrameState(activateActive, activateValue);
			this.uiPressInteractionState.SetFrameState(pressActive, pressValue);
		}

		public void ResetFrameDependentStates()
		{
			this.selectInteractionState.ResetFrameDependent();
			this.activateInteractionState.ResetFrameDependent();
			this.uiPressInteractionState.ResetFrameDependent();
		}

		public override string ToString()
		{
			return string.Format("time: {0}, position: {1}, rotation: {2}, selectActive: {3}, activateActive: {4}, pressActive: {5}, isTracked: {6}, inputTrackingState: {7}", new object[]
			{
				this.time,
				this.position,
				this.rotation,
				this.selectInteractionState.active,
				this.activateInteractionState.active,
				this.uiPressInteractionState.active,
				this.isTracked,
				this.inputTrackingState
			});
		}

		[Obsolete("poseDataFlags has been deprecated. Use inputTrackingState instead.", true)]
		public PoseDataFlags poseDataFlags
		{
			get
			{
				return PoseDataFlags.NoData;
			}
			set
			{
			}
		}

		[Obsolete("This constructor has been deprecated. Use the constructors with the inputTrackingState parameter.", true)]
		public XRControllerState(double time, Vector3 position, Quaternion rotation, bool selectActive, bool activateActive, bool pressActive) : this(time, position, rotation, InputTrackingState.Position | InputTrackingState.Rotation, selectActive, activateActive, pressActive)
		{
		}

		[Obsolete("This constructor has been deprecated. Use the constructor with the isTracked parameter.", true)]
		protected XRControllerState(double time, Vector3 position, Quaternion rotation, InputTrackingState inputTrackingState) : this(time, position, rotation, inputTrackingState, true)
		{
		}

		[Obsolete("This constructor has been deprecated. Use the constructor with the isTracked parameter.", true)]
		public XRControllerState(double time, Vector3 position, Quaternion rotation, InputTrackingState inputTrackingState, bool selectActive, bool activateActive, bool pressActive) : this(time, position, rotation, inputTrackingState, true)
		{
		}

		[Obsolete("This constructor has been deprecated. Use the constructor with the isTracked parameter.", true)]
		public XRControllerState(double time, Vector3 position, Quaternion rotation, InputTrackingState inputTrackingState, bool selectActive, bool activateActive, bool pressActive, float selectValue, float activateValue, float pressValue) : this(time, position, rotation, inputTrackingState, true)
		{
		}

		[Obsolete("ResetInputs has been renamed. Use ResetFrameDependentStates instead. (UnityUpgradable) -> ResetFrameDependentStates()", true)]
		public void ResetInputs()
		{
		}

		public double time;

		public InputTrackingState inputTrackingState;

		public bool isTracked;

		public Vector3 position;

		public Quaternion rotation;

		public InteractionState selectInteractionState;

		public InteractionState activateInteractionState;

		public InteractionState uiPressInteractionState;

		public Vector2 uiScrollValue;
	}
}
