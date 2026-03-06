using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.Unity.Input
{
	public class InputMouseButton : MonoBehaviour, IButton
	{
		public bool Value()
		{
			return Input.GetMouseButton(this._button);
		}

		[SerializeField]
		private int _button;
	}
}
