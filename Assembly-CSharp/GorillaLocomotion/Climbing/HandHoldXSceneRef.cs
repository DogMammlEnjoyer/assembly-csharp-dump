using System;
using UnityEngine;

namespace GorillaLocomotion.Climbing
{
	public class HandHoldXSceneRef : MonoBehaviour
	{
		public HandHold target
		{
			get
			{
				HandHold result;
				if (this.reference.TryResolve<HandHold>(out result))
				{
					return result;
				}
				return null;
			}
		}

		public GameObject targetObject
		{
			get
			{
				GameObject result;
				if (this.reference.TryResolve(out result))
				{
					return result;
				}
				return null;
			}
		}

		[SerializeField]
		public XSceneRef reference;
	}
}
