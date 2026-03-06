using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class JoeJeffController : MonoBehaviour
	{
		private void Start()
		{
			this.interactable = base.GetComponent<Interactable>();
		}

		private void Update()
		{
			if (this.interactable.attachedToHand)
			{
				this.hand = this.interactable.attachedToHand.handType;
				Vector2 axis = this.moveAction[this.hand].axis;
				this.movement = new Vector3(axis.x, 0f, axis.y);
				this.jump = this.jumpAction[this.hand].stateDown;
				this.glow = Mathf.Lerp(this.glow, this.jumpAction[this.hand].state ? 1.5f : 1f, Time.deltaTime * 20f);
			}
			else
			{
				this.movement = Vector2.zero;
				this.jump = false;
				this.glow = 0f;
			}
			this.Joystick.localPosition = this.movement * this.joyMove;
			float y = base.transform.eulerAngles.y;
			this.movement = Quaternion.AngleAxis(y, Vector3.up) * this.movement;
			this.jumpHighlight.sharedMaterial.SetColor("_EmissionColor", Color.white * this.glow);
			this.character.Move(this.movement * 2f, this.jump);
		}

		public Transform Joystick;

		public float joyMove = 0.1f;

		public SteamVR_Action_Vector2 moveAction = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("platformer", "Move", false, false);

		public SteamVR_Action_Boolean jumpAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("platformer", "Jump", false, false);

		public JoeJeff character;

		public Renderer jumpHighlight;

		private Vector3 movement;

		private bool jump;

		private float glow;

		private SteamVR_Input_Sources hand;

		private Interactable interactable;
	}
}
