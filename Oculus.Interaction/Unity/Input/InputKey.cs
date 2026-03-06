using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.Unity.Input
{
	public class InputKey : MonoBehaviour, IButton
	{
		public bool Value()
		{
			return Input.GetKey(this._key);
		}

		[SerializeField]
		private KeyCode _key;
	}
}
