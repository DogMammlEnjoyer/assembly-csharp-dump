using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.Unity.Input
{
	public class InputButton : MonoBehaviour, IButton
	{
		public bool Value()
		{
			return Input.GetButton(this._buttonName);
		}

		[SerializeField]
		private string _buttonName;
	}
}
