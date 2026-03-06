using System;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public class VirtualActiveState : MonoBehaviour, IActiveState
	{
		public bool Active
		{
			get
			{
				return this._active;
			}
			set
			{
				this._active = value;
			}
		}

		[SerializeField]
		private bool _active;
	}
}
