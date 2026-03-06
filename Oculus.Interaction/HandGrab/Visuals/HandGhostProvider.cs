using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.HandGrab.Visuals
{
	[CreateAssetMenu(menuName = "Meta/Interaction/SDK/Pose Authoring/Hand Ghost Provider")]
	public class HandGhostProvider : ScriptableObject
	{
		public HandGhost GetHand(Handedness handedness)
		{
			if (handedness != Handedness.Left)
			{
				return this._rightHand;
			}
			return this._leftHand;
		}

		[SerializeField]
		private HandGhost _leftHand;

		[SerializeField]
		private HandGhost _rightHand;
	}
}
