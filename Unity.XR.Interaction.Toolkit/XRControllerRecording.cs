using System;
using System.Collections.Generic;

namespace UnityEngine.XR.Interaction.Toolkit
{
	[CreateAssetMenu(menuName = "XR/XR Controller Recording")]
	[PreferBinarySerialization]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.XRControllerRecording.html")]
	[Serializable]
	public class XRControllerRecording : ScriptableObject, ISerializationCallbackReceiver
	{
		public List<XRControllerState> frames
		{
			get
			{
				return this.m_Frames;
			}
		}

		public double duration
		{
			get
			{
				if (this.m_Frames.Count != 0)
				{
					return this.m_Frames[this.m_Frames.Count - 1].time;
				}
				return 0.0;
			}
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			if (this.m_Frames == null || this.m_Frames.Count <= 0)
			{
				return;
			}
			XRControllerState xrcontrollerState = this.m_Frames[0];
			this.m_SelectActivatedInFirstFrame = xrcontrollerState.selectInteractionState.activatedThisFrame;
			this.m_ActivateActivatedInFirstFrame = xrcontrollerState.activateInteractionState.activatedThisFrame;
			this.m_FirstUIPressActivatedInFirstFrame = xrcontrollerState.uiPressInteractionState.activatedThisFrame;
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			this.SetFrameDependentData();
		}

		internal void SetFrameDependentData()
		{
			if (this.m_Frames == null || this.m_Frames.Count <= 0)
			{
				return;
			}
			XRControllerState xrcontrollerState = this.m_Frames[0];
			xrcontrollerState.selectInteractionState.SetFrameDependent(!this.m_SelectActivatedInFirstFrame && xrcontrollerState.selectInteractionState.active);
			xrcontrollerState.activateInteractionState.SetFrameDependent(!this.m_ActivateActivatedInFirstFrame && xrcontrollerState.activateInteractionState.active);
			xrcontrollerState.uiPressInteractionState.SetFrameDependent(!this.m_FirstUIPressActivatedInFirstFrame && xrcontrollerState.uiPressInteractionState.active);
			for (int i = 1; i < this.m_Frames.Count; i++)
			{
				XRControllerState xrcontrollerState2 = this.m_Frames[i];
				XRControllerState xrcontrollerState3 = this.m_Frames[i - 1];
				xrcontrollerState2.selectInteractionState.SetFrameDependent(xrcontrollerState3.selectInteractionState.active);
				xrcontrollerState2.activateInteractionState.SetFrameDependent(xrcontrollerState3.activateInteractionState.active);
				xrcontrollerState2.uiPressInteractionState.SetFrameDependent(xrcontrollerState3.uiPressInteractionState.active);
			}
		}

		public void AddRecordingFrame(XRControllerState state)
		{
			this.frames.Add(new XRControllerState(state));
		}

		public void AddRecordingFrameNonAlloc(XRControllerState state)
		{
			this.frames.Add(state);
		}

		public void InitRecording()
		{
			this.m_SelectActivatedInFirstFrame = false;
			this.m_ActivateActivatedInFirstFrame = false;
			this.m_FirstUIPressActivatedInFirstFrame = false;
			this.m_Frames.Clear();
		}

		public void SaveRecording()
		{
		}

		[Obsolete("AddRecordingFrame has been deprecated. Use the overload method with the XRControllerState parameter or the method AddRecordingFrameNonAlloc.", true)]
		public void AddRecordingFrame(double time, Vector3 position, Quaternion rotation, bool selectActive, bool activateActive, bool pressActive)
		{
		}

		[SerializeField]
		private bool m_SelectActivatedInFirstFrame;

		[SerializeField]
		private bool m_ActivateActivatedInFirstFrame;

		[SerializeField]
		private bool m_FirstUIPressActivatedInFirstFrame;

		[SerializeField]
		private List<XRControllerState> m_Frames = new List<XRControllerState>();
	}
}
