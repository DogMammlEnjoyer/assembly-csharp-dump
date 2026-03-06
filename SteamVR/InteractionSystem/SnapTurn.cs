using System;
using System.Collections;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class SnapTurn : MonoBehaviour
	{
		private void Start()
		{
			this.AllOff();
		}

		private void AllOff()
		{
			if (this.rotateLeftFX != null)
			{
				this.rotateLeftFX.SetActive(false);
			}
			if (this.rotateRightFX != null)
			{
				this.rotateRightFX.SetActive(false);
			}
		}

		private void Update()
		{
			Player instance = Player.instance;
			if (this.canRotate && this.snapLeftAction != null && this.snapRightAction != null && this.snapLeftAction.activeBinding && this.snapRightAction.activeBinding)
			{
				if (Time.time < SnapTurn.teleportLastActiveTime + this.canTurnEverySeconds)
				{
					return;
				}
				bool flag = instance.rightHand.currentAttachedObject == null || (instance.rightHand.currentAttachedObject != null && instance.rightHand.currentAttachedTeleportManager != null && instance.rightHand.currentAttachedTeleportManager.teleportAllowed);
				bool flag2 = instance.leftHand.currentAttachedObject == null || (instance.leftHand.currentAttachedObject != null && instance.leftHand.currentAttachedTeleportManager != null && instance.leftHand.currentAttachedTeleportManager.teleportAllowed);
				bool flag3 = this.snapLeftAction.GetStateDown(SteamVR_Input_Sources.LeftHand) && flag2;
				bool flag4 = this.snapLeftAction.GetStateDown(SteamVR_Input_Sources.RightHand) && flag;
				bool flag5 = this.snapRightAction.GetStateDown(SteamVR_Input_Sources.LeftHand) && flag2;
				bool flag6 = this.snapRightAction.GetStateDown(SteamVR_Input_Sources.RightHand) && flag;
				if (flag3 || flag4)
				{
					this.RotatePlayer(-this.snapAngle);
					return;
				}
				if (flag5 || flag6)
				{
					this.RotatePlayer(this.snapAngle);
				}
			}
		}

		public void RotatePlayer(float angle)
		{
			if (this.rotateCoroutine != null)
			{
				base.StopCoroutine(this.rotateCoroutine);
				this.AllOff();
			}
			this.rotateCoroutine = base.StartCoroutine(this.DoRotatePlayer(angle));
		}

		private IEnumerator DoRotatePlayer(float angle)
		{
			Player player = Player.instance;
			this.canRotate = false;
			this.snapTurnSource.panStereo = angle / 90f;
			this.snapTurnSource.PlayOneShot(this.rotateSound);
			if (this.fadeScreen)
			{
				SteamVR_Fade.Start(Color.clear, 0f, false);
				Color newColor = this.screenFadeColor;
				newColor = newColor.linear * 0.6f;
				SteamVR_Fade.Start(newColor, this.fadeTime, false);
			}
			yield return new WaitForSeconds(this.fadeTime);
			Vector3 vector = player.trackingOriginTransform.position - player.feetPositionGuess;
			player.trackingOriginTransform.position -= vector;
			player.transform.Rotate(Vector3.up, angle);
			vector = Quaternion.Euler(0f, angle, 0f) * vector;
			player.trackingOriginTransform.position += vector;
			GameObject fx = (angle > 0f) ? this.rotateRightFX : this.rotateLeftFX;
			if (this.showTurnAnimation)
			{
				this.ShowRotateFX(fx);
			}
			if (this.fadeScreen)
			{
				SteamVR_Fade.Start(Color.clear, this.fadeTime, false);
			}
			float time = Time.time;
			float endTime = time + this.canTurnEverySeconds;
			while (Time.time <= endTime)
			{
				yield return null;
				this.UpdateOrientation(fx);
			}
			fx.SetActive(false);
			this.canRotate = true;
			yield break;
		}

		private void ShowRotateFX(GameObject fx)
		{
			if (fx == null)
			{
				return;
			}
			fx.SetActive(false);
			this.UpdateOrientation(fx);
			fx.SetActive(true);
			this.UpdateOrientation(fx);
		}

		private void UpdateOrientation(GameObject fx)
		{
			Player instance = Player.instance;
			base.transform.position = instance.hmdTransform.position + instance.hmdTransform.forward * this.distanceFromFace;
			base.transform.rotation = Quaternion.LookRotation(instance.hmdTransform.position - base.transform.position, Vector3.up);
			base.transform.Translate(this.additionalOffset, Space.Self);
			base.transform.rotation = Quaternion.LookRotation(instance.hmdTransform.position - base.transform.position, Vector3.up);
		}

		public float snapAngle = 90f;

		public bool showTurnAnimation = true;

		public AudioSource snapTurnSource;

		public AudioClip rotateSound;

		public GameObject rotateRightFX;

		public GameObject rotateLeftFX;

		public SteamVR_Action_Boolean snapLeftAction = SteamVR_Input.GetBooleanAction("SnapTurnLeft", false);

		public SteamVR_Action_Boolean snapRightAction = SteamVR_Input.GetBooleanAction("SnapTurnRight", false);

		public bool fadeScreen = true;

		public float fadeTime = 0.1f;

		public Color screenFadeColor = Color.black;

		public float distanceFromFace = 1.3f;

		public Vector3 additionalOffset = new Vector3(0f, -0.3f, 0f);

		public static float teleportLastActiveTime;

		private bool canRotate = true;

		public float canTurnEverySeconds = 0.4f;

		private Coroutine rotateCoroutine;
	}
}
