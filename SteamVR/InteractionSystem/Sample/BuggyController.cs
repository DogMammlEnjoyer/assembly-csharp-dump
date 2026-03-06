using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Valve.VR.InteractionSystem.Sample
{
	public class BuggyController : MonoBehaviour
	{
		private void Start()
		{
			this.joySRot = this.modelJoystick.localRotation;
			this.trigSRot = this.modelTrigger.localRotation;
			this.interactable = base.GetComponent<Interactable>();
			base.StartCoroutine(this.DoBuzz());
			this.buggy.controllerReference = base.transform;
			this.initialScale = this.buggy.transform.localScale;
		}

		private void Update()
		{
			Vector2 vector = Vector2.zero;
			float num = 0f;
			float handBrake = 0f;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (this.interactable.attachedToHand)
			{
				SteamVR_Input_Sources handType = this.interactable.attachedToHand.handType;
				vector = this.actionSteering.GetAxis(handType);
				num = this.actionThrottle.GetAxis(handType);
				flag2 = this.actionBrake.GetState(handType);
				flag3 = this.actionReset.GetState(handType);
				handBrake = (float)(flag2 ? 1 : 0);
				flag = this.actionReset.GetStateDown(handType);
			}
			if (flag && this.resettingRoutine == null)
			{
				this.resettingRoutine = base.StartCoroutine(this.DoReset());
			}
			if (this.ui_Canvas != null)
			{
				this.ui_Canvas.gameObject.SetActive(this.interactable.attachedToHand);
				this.usteer = Mathf.Lerp(this.usteer, vector.x, Time.deltaTime * 9f);
				this.ui_steer.localEulerAngles = Vector3.forward * this.usteer * -this.ui_steerangle;
				this.ui_rpm.fillAmount = Mathf.Lerp(this.ui_rpm.fillAmount, Mathf.Lerp(this.ui_fillAngles.x, this.ui_fillAngles.y, num), Time.deltaTime * 4f);
				float num2 = 40f;
				this.ui_speed.fillAmount = Mathf.Lerp(this.ui_fillAngles.x, this.ui_fillAngles.y, 1f - Mathf.Exp(-this.buggy.speed / num2));
			}
			this.modelJoystick.localRotation = this.joySRot;
			this.modelJoystick.Rotate(vector.y * -this.joystickRot, vector.x * -this.joystickRot, 0f, Space.Self);
			this.modelTrigger.localRotation = this.trigSRot;
			this.modelTrigger.Rotate(num * -this.triggerRot, 0f, 0f, Space.Self);
			this.buttonBrake.localScale = new Vector3(1f, 1f, flag2 ? 0.4f : 1f);
			this.buttonReset.localScale = new Vector3(1f, 1f, flag3 ? 0.4f : 1f);
			this.buggy.steer = vector;
			this.buggy.throttle = num;
			this.buggy.handBrake = handBrake;
			this.buggy.controllerReference = base.transform;
		}

		private IEnumerator DoReset()
		{
			float time = Time.time;
			float num = 1f;
			float endTime = time + num;
			this.buggy.transform.position = this.resetToPoint.transform.position;
			this.buggy.transform.rotation = this.resetToPoint.transform.rotation;
			this.buggy.transform.localScale = this.initialScale * 0.1f;
			while (Time.time < endTime)
			{
				this.buggy.transform.localScale = Vector3.Lerp(this.buggy.transform.localScale, this.initialScale, Time.deltaTime * 5f);
				yield return null;
			}
			this.buggy.transform.localScale = this.initialScale;
			this.resettingRoutine = null;
			yield break;
		}

		private IEnumerator DoBuzz()
		{
			for (;;)
			{
				if (this.buzztimer >= 1f)
				{
					this.buzztimer = 0f;
					if (this.interactable.attachedToHand)
					{
						this.interactable.attachedToHand.TriggerHapticPulse((ushort)Mathf.RoundToInt(300f * Mathf.Lerp(1f, 0.6f, this.buggy.mvol)));
					}
				}
				else
				{
					this.buzztimer += Time.deltaTime * this.buggy.mvol * 70f;
					yield return null;
				}
			}
			yield break;
		}

		public Transform modelJoystick;

		public float joystickRot = 20f;

		public Transform modelTrigger;

		public float triggerRot = 20f;

		public BuggyBuddy buggy;

		public Transform buttonBrake;

		public Transform buttonReset;

		public Canvas ui_Canvas;

		public Image ui_rpm;

		public Image ui_speed;

		public RectTransform ui_steer;

		public float ui_steerangle;

		public Vector2 ui_fillAngles;

		public Transform resetToPoint;

		public SteamVR_Action_Vector2 actionSteering = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("buggy", "Steering", false, false);

		public SteamVR_Action_Single actionThrottle = SteamVR_Input.GetAction<SteamVR_Action_Single>("buggy", "Throttle", false, false);

		public SteamVR_Action_Boolean actionBrake = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("buggy", "Brake", false, false);

		public SteamVR_Action_Boolean actionReset = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("buggy", "Reset", false, false);

		private float usteer;

		private Interactable interactable;

		private Quaternion trigSRot;

		private Quaternion joySRot;

		private Coroutine resettingRoutine;

		private Vector3 initialScale;

		private float buzztimer;
	}
}
