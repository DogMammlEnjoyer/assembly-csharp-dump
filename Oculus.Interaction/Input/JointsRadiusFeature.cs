using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public class JointsRadiusFeature : MonoBehaviour
	{
		public float GetJointRadius(HandJointId id)
		{
			return this._hand.GetData().JointRadii[(int)id];
		}

		[SerializeField]
		private Hand _hand;
	}
}
