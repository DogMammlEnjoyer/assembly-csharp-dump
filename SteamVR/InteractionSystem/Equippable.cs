using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	[RequireComponent(typeof(Throwable))]
	public class Equippable : MonoBehaviour
	{
		[HideInInspector]
		public SteamVR_Input_Sources attachedHandType
		{
			get
			{
				if (this.interactable.attachedToHand)
				{
					return this.interactable.attachedToHand.handType;
				}
				return SteamVR_Input_Sources.Any;
			}
		}

		private void Start()
		{
			this.initialScale = base.transform.localScale;
			this.interactable = base.GetComponent<Interactable>();
		}

		private void Update()
		{
			if (this.interactable.attachedToHand)
			{
				Vector3 localScale = this.initialScale;
				if ((this.attachedHandType == SteamVR_Input_Sources.RightHand && this.defaultHand == WhichHand.Right) || (this.attachedHandType == SteamVR_Input_Sources.LeftHand && this.defaultHand == WhichHand.Left))
				{
					localScale.x *= 1f;
					for (int i = 0; i < this.antiFlip.Length; i++)
					{
						this.antiFlip[i].localScale = new Vector3(1f, 1f, 1f);
					}
				}
				else
				{
					localScale.x *= -1f;
					for (int j = 0; j < this.antiFlip.Length; j++)
					{
						this.antiFlip[j].localScale = new Vector3(-1f, 1f, 1f);
					}
				}
				base.transform.localScale = localScale;
			}
		}

		[Tooltip("Array of children you do not want to be mirrored. Text, logos, etc.")]
		public Transform[] antiFlip;

		public WhichHand defaultHand = WhichHand.Right;

		private Vector3 initialScale;

		private Interactable interactable;
	}
}
