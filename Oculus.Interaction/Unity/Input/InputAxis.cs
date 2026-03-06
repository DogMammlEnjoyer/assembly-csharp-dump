using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.Unity.Input
{
	public class InputAxis : MonoBehaviour, IAxis1D
	{
		public float Value()
		{
			return Input.GetAxis(this._axisName);
		}

		[SerializeField]
		private string _axisName;
	}
}
