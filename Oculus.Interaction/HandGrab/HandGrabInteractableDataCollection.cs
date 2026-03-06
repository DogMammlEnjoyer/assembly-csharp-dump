using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.HandGrab
{
	[CreateAssetMenu(menuName = "Meta/Interaction/SDK/Pose Authoring/HandGrabInteractable Data Collection")]
	public class HandGrabInteractableDataCollection : ScriptableObject
	{
		public List<HandGrabUtils.HandGrabInteractableData> InteractablesData
		{
			get
			{
				return this._interactablesData;
			}
		}

		public void StoreInteractables(List<HandGrabUtils.HandGrabInteractableData> interactablesData)
		{
			this._interactablesData = interactablesData;
		}

		[SerializeField]
		[Tooltip("Do not modify this manually unless you are sure! Instead load the HandGrabInteractable and use the tools provided.")]
		private List<HandGrabUtils.HandGrabInteractableData> _interactablesData;
	}
}
