using System;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit
{
	[AddComponentMenu("XR/Debug/XR Controller Recorder", 11)]
	[DisallowMultipleComponent]
	[DefaultExecutionOrder(-30000)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.XRControllerRecorder.html")]
	public class XRControllerRecorder : MonoBehaviour
	{
		public bool playOnStart
		{
			get
			{
				return this.m_PlayOnStart;
			}
			set
			{
				this.m_PlayOnStart = value;
			}
		}

		public XRControllerRecording recording
		{
			get
			{
				return this.m_Recording;
			}
			set
			{
				this.m_Recording = value;
			}
		}

		public bool visitEachFrame
		{
			get
			{
				return this.m_VisitEachFrame;
			}
			set
			{
				this.m_VisitEachFrame = value;
			}
		}

		public bool isRecording
		{
			get
			{
				return this.m_IsRecording;
			}
			set
			{
				if (this.m_IsRecording != value)
				{
					this.recordingStartTime = Time.time;
					this.isPlaying = false;
					this.m_CurrentTime = 0.0;
					if (this.m_Recording)
					{
						if (value)
						{
							this.m_Recording.InitRecording();
						}
						else
						{
							this.m_Recording.SaveRecording();
						}
					}
					this.m_IsRecording = value;
				}
			}
		}

		public bool isPlaying
		{
			get
			{
				return this.m_IsPlaying;
			}
			set
			{
				if (this.m_IsPlaying != value)
				{
					this.isRecording = false;
					if (this.m_Recording)
					{
						this.ResetPlayback();
					}
					this.m_CurrentTime = 0.0;
					this.m_IsPlaying = value;
					if (value)
					{
						this.StartPlaying();
						return;
					}
					this.StopPlaying();
				}
			}
		}

		public double currentTime
		{
			get
			{
				return this.m_CurrentTime;
			}
		}

		public double duration
		{
			get
			{
				if (!(this.m_Recording != null))
				{
					return 0.0;
				}
				return this.m_Recording.duration;
			}
		}

		protected float recordingStartTime { get; set; }

		protected void Awake()
		{
			if (this.m_XRController == null)
			{
				this.m_XRController = base.GetComponentInParent<XRBaseController>(true);
			}
			if (this.m_InteractorObject == null)
			{
				this.m_InteractorObject = (base.GetComponentInParent<IXRInteractor>(true) as Object);
			}
			this.m_CurrentTime = 0.0;
			if (this.m_PlayOnStart)
			{
				this.isPlaying = true;
			}
		}

		protected virtual void Update()
		{
			if (this.isRecording)
			{
				IXRInteractor interactor = this.GetInteractor();
				float num = Time.time - this.recordingStartTime;
				XRControllerState xrcontrollerState;
				if (this.m_XRController != null)
				{
					xrcontrollerState = new XRControllerState(this.m_XRController.currentControllerState);
				}
				else if (interactor != null)
				{
					Pose localPose = interactor.transform.GetLocalPose();
					xrcontrollerState = new XRControllerState
					{
						inputTrackingState = InputTrackingState.All,
						isTracked = true,
						position = localPose.position,
						rotation = localPose.rotation
					};
				}
				else
				{
					xrcontrollerState = new XRControllerState();
				}
				xrcontrollerState.time = (double)num;
				if (interactor != null)
				{
					XRBaseInputInteractor xrbaseInputInteractor = interactor as XRBaseInputInteractor;
					if (xrbaseInputInteractor != null)
					{
						XRInputButtonReader selectInput = xrbaseInputInteractor.selectInput;
						xrcontrollerState.selectInteractionState = new InteractionState
						{
							value = selectInput.ReadValue(),
							active = selectInput.ReadIsPerformed(),
							activatedThisFrame = selectInput.ReadWasPerformedThisFrame(),
							deactivatedThisFrame = selectInput.ReadWasCompletedThisFrame()
						};
						XRInputButtonReader activateInput = xrbaseInputInteractor.activateInput;
						xrcontrollerState.activateInteractionState = new InteractionState
						{
							value = activateInput.ReadValue(),
							active = activateInput.ReadIsPerformed(),
							activatedThisFrame = activateInput.ReadWasPerformedThisFrame(),
							deactivatedThisFrame = activateInput.ReadWasCompletedThisFrame()
						};
					}
					else
					{
						xrcontrollerState.selectInteractionState = default(InteractionState);
						xrcontrollerState.activateInteractionState = default(InteractionState);
					}
					XRRayInteractor xrrayInteractor = interactor as XRRayInteractor;
					if (xrrayInteractor != null)
					{
						XRInputButtonReader uiPressInput = xrrayInteractor.uiPressInput;
						xrcontrollerState.uiPressInteractionState = new InteractionState
						{
							value = uiPressInput.ReadValue(),
							active = uiPressInput.ReadIsPerformed(),
							activatedThisFrame = uiPressInput.ReadWasPerformedThisFrame(),
							deactivatedThisFrame = uiPressInput.ReadWasCompletedThisFrame()
						};
						xrcontrollerState.uiScrollValue = xrrayInteractor.uiScrollInput.ReadValue();
					}
					else
					{
						xrcontrollerState.uiPressInteractionState = default(InteractionState);
						xrcontrollerState.uiScrollValue = Vector2.zero;
					}
				}
				this.m_Recording.AddRecordingFrameNonAlloc(xrcontrollerState);
			}
			else if (this.isPlaying)
			{
				this.UpdatePlaybackTime(this.m_CurrentTime);
			}
			if (this.isRecording || this.isPlaying)
			{
				this.m_CurrentTime += (double)Time.deltaTime;
			}
			if (this.isPlaying && this.m_CurrentTime > this.m_Recording.duration && (!this.m_VisitEachFrame || this.m_LastFrameIdx >= this.m_Recording.frames.Count - 1))
			{
				this.isPlaying = false;
			}
		}

		protected void OnDestroy()
		{
			this.isRecording = false;
			this.isPlaying = false;
		}

		public IXRInteractor GetInteractor()
		{
			return this.m_Interactor.Get(this.m_InteractorObject);
		}

		public void SetInteractor(IXRInteractor interactor)
		{
			this.m_Interactor.Set(ref this.m_InteractorObject, interactor);
		}

		public void ResetPlayback()
		{
			this.m_LastPlaybackTime = 0.0;
			this.m_LastFrameIdx = 0;
		}

		private void StartPlaying()
		{
			if (this.m_XRController != null)
			{
				this.m_PrevEnableInputActions = this.m_XRController.enableInputActions;
				this.m_PrevEnableInputTracking = this.m_XRController.enableInputTracking;
				this.m_XRController.enableInputActions = false;
				this.m_XRController.enableInputTracking = false;
			}
			IXRInteractor interactor = this.GetInteractor();
			if (interactor != null)
			{
				XRBaseInputInteractor xrbaseInputInteractor = interactor as XRBaseInputInteractor;
				if (xrbaseInputInteractor != null)
				{
					this.m_PrevSelectBypass = xrbaseInputInteractor.selectInput.bypass;
					this.m_PrevActivateBypass = xrbaseInputInteractor.activateInput.bypass;
					xrbaseInputInteractor.selectInput.bypass = this.m_SelectBypass;
					xrbaseInputInteractor.activateInput.bypass = this.m_ActivateBypass;
				}
				else
				{
					this.m_PrevSelectBypass = null;
					this.m_PrevActivateBypass = null;
				}
				XRRayInteractor xrrayInteractor = interactor as XRRayInteractor;
				if (xrrayInteractor != null)
				{
					this.m_PrevUIPressBypass = ((xrrayInteractor != null) ? xrrayInteractor.uiPressInput.bypass : null);
					this.m_PrevUIScrollBypass = ((xrrayInteractor != null) ? xrrayInteractor.uiScrollInput.bypass : null);
					xrrayInteractor.uiPressInput.bypass = this.m_UIPressBypass;
					xrrayInteractor.uiScrollInput.bypass = this.m_UIScrollBypass;
					return;
				}
				this.m_PrevUIPressBypass = null;
				this.m_PrevUIScrollBypass = null;
			}
		}

		private void StopPlaying()
		{
			if (this.m_XRController != null)
			{
				this.m_XRController.enableInputActions = this.m_PrevEnableInputActions;
				this.m_XRController.enableInputTracking = this.m_PrevEnableInputTracking;
			}
			IXRInteractor interactor = this.GetInteractor();
			if (this.m_Interactor != null)
			{
				XRBaseInputInteractor xrbaseInputInteractor = interactor as XRBaseInputInteractor;
				if (xrbaseInputInteractor != null)
				{
					xrbaseInputInteractor.selectInput.bypass = this.m_PrevSelectBypass;
					xrbaseInputInteractor.activateInput.bypass = this.m_PrevActivateBypass;
				}
				XRRayInteractor xrrayInteractor = interactor as XRRayInteractor;
				if (xrrayInteractor != null)
				{
					xrrayInteractor.uiPressInput.bypass = this.m_PrevUIPressBypass;
					xrrayInteractor.uiScrollInput.bypass = this.m_PrevUIScrollBypass;
				}
			}
		}

		private void UpdatePlaybackTime(double playbackTime)
		{
			if (!this.m_Recording || this.m_Recording == null || this.m_Recording.frames.Count == 0 || this.m_LastFrameIdx >= this.m_Recording.frames.Count)
			{
				return;
			}
			XRControllerState xrcontrollerState = this.m_Recording.frames[this.m_LastFrameIdx];
			int num = this.m_LastFrameIdx;
			if (xrcontrollerState.time < playbackTime)
			{
				while (num < this.m_Recording.frames.Count && this.m_Recording.frames[num].time >= this.m_LastPlaybackTime && this.m_Recording.frames[num].time <= playbackTime)
				{
					num++;
					if (this.m_VisitEachFrame)
					{
						if (num < this.m_Recording.frames.Count)
						{
							playbackTime = this.m_Recording.frames[num].time;
							break;
						}
						break;
					}
				}
			}
			if (num >= this.m_Recording.frames.Count)
			{
				return;
			}
			XRControllerState xrcontrollerState2 = this.m_Recording.frames[num];
			if (this.m_XRController != null)
			{
				this.m_XRController.currentControllerState = xrcontrollerState2;
			}
			IXRInteractor interactor = this.GetInteractor();
			if (interactor != null)
			{
				this.m_SelectBypass.state = xrcontrollerState2.selectInteractionState;
				this.m_ActivateBypass.state = xrcontrollerState2.activateInteractionState;
				this.m_UIPressBypass.state = xrcontrollerState2.uiPressInteractionState;
				this.m_UIScrollBypass.state = xrcontrollerState2.uiScrollValue;
				if (this.m_XRController == null)
				{
					Transform transform = interactor.transform;
					bool flag = (xrcontrollerState2.inputTrackingState & InputTrackingState.Position) > InputTrackingState.None;
					bool flag2 = (xrcontrollerState2.inputTrackingState & InputTrackingState.Rotation) > InputTrackingState.None;
					if (flag && flag2)
					{
						transform.SetLocalPose(new Pose(xrcontrollerState2.position, xrcontrollerState2.rotation));
					}
					else if (flag)
					{
						transform.localPosition = xrcontrollerState2.position;
					}
					else if (flag2)
					{
						transform.localRotation = xrcontrollerState2.rotation;
					}
				}
			}
			this.m_LastFrameIdx = num;
			this.m_LastPlaybackTime = playbackTime;
		}

		public virtual bool GetControllerState(out XRControllerState controllerState)
		{
			if (this.isPlaying)
			{
				if (this.m_Recording.frames.Count > this.m_LastFrameIdx)
				{
					controllerState = this.m_Recording.frames[this.m_LastFrameIdx];
					return true;
				}
			}
			else if (this.isRecording && this.m_Recording.frames.Count > 0)
			{
				controllerState = this.m_Recording.frames[this.m_Recording.frames.Count - 1];
				return true;
			}
			controllerState = null;
			return false;
		}

		[Obsolete("xrController has been deprecated in version 3.0.0. Use interactor to allow the recorder to read and playback button input instead.")]
		public XRBaseController xrController
		{
			get
			{
				return this.m_XRController;
			}
			set
			{
				this.m_XRController = value;
			}
		}

		[Header("Input Recording/Playback")]
		[SerializeField]
		[Tooltip("Controls whether this recording will start playing when the component's Awake() method is called.")]
		private bool m_PlayOnStart;

		[SerializeField]
		[Tooltip("Controller Recording asset for recording and playback of controller events.")]
		private XRControllerRecording m_Recording;

		[SerializeField]
		[Tooltip("Interactor whose input will be recorded and played back.")]
		[RequireInterface(typeof(IXRInteractor))]
		private Object m_InteractorObject;

		[SerializeField]
		[Tooltip("If true, every frame of the recording must be visited even if a larger time period has passed.")]
		private bool m_VisitEachFrame;

		private double m_CurrentTime;

		private readonly UnityObjectReferenceCache<IXRInteractor, Object> m_Interactor = new UnityObjectReferenceCache<IXRInteractor, Object>();

		private bool m_IsRecording;

		private bool m_IsPlaying;

		private double m_LastPlaybackTime;

		private int m_LastFrameIdx;

		private bool m_PrevEnableInputActions;

		private bool m_PrevEnableInputTracking;

		private IXRInputButtonReader m_PrevSelectBypass;

		private IXRInputButtonReader m_PrevActivateBypass;

		private IXRInputButtonReader m_PrevUIPressBypass;

		private IXRInputValueReader<Vector2> m_PrevUIScrollBypass;

		private readonly XRControllerRecorder.ButtonBypass m_SelectBypass = new XRControllerRecorder.ButtonBypass();

		private readonly XRControllerRecorder.ButtonBypass m_ActivateBypass = new XRControllerRecorder.ButtonBypass();

		private readonly XRControllerRecorder.ButtonBypass m_UIPressBypass = new XRControllerRecorder.ButtonBypass();

		private readonly XRControllerRecorder.ValueBypass<Vector2> m_UIScrollBypass = new XRControllerRecorder.ValueBypass<Vector2>();

		[SerializeField]
		[Tooltip("(Deprecated) XR Controller whose output will be recorded and played back.")]
		[Obsolete("m_XRController has been deprecated in version 3.0.0.")]
		private XRBaseController m_XRController;

		private class ButtonBypass : IXRInputButtonReader, IXRInputValueReader<float>, IXRInputValueReader
		{
			public InteractionState state { get; set; }

			public bool ReadIsPerformed()
			{
				return this.state.active;
			}

			public bool ReadWasPerformedThisFrame()
			{
				return this.state.activatedThisFrame;
			}

			public bool ReadWasCompletedThisFrame()
			{
				return this.state.deactivatedThisFrame;
			}

			public float ReadValue()
			{
				return this.state.value;
			}

			public bool TryReadValue(out float value)
			{
				value = this.state.value;
				return true;
			}
		}

		private class ValueBypass<TValue> : IXRInputValueReader<TValue>, IXRInputValueReader where TValue : struct
		{
			public TValue state { get; set; }

			public TValue ReadValue()
			{
				return this.state;
			}

			public bool TryReadValue(out TValue value)
			{
				value = this.state;
				return true;
			}
		}
	}
}
