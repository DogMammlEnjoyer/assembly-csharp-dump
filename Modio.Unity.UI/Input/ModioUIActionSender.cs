using System;
using UnityEngine;

namespace Modio.Unity.UI.Input
{
	public class ModioUIActionSender : MonoBehaviour
	{
		public void PressedAction()
		{
			ModioUIInput.PressedAction(this._action);
		}

		[SerializeField]
		private ModioUIInput.ModioAction _action;
	}
}
