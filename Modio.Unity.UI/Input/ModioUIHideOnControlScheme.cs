using System;
using UnityEngine;

namespace Modio.Unity.UI.Input
{
	public class ModioUIHideOnControlScheme : MonoBehaviour
	{
		private void OnEnable()
		{
			ModioUIInput.SwappedControlScheme += this.OnSwappedToController;
			this.OnSwappedToController(ModioUIInput.IsUsingGamepad);
		}

		private void OnDisable()
		{
			ModioUIInput.SwappedControlScheme -= this.OnSwappedToController;
		}

		private void OnSwappedToController(bool isController)
		{
			GameObject[] objectsToHide = this._objectsToHide;
			for (int i = 0; i < objectsToHide.Length; i++)
			{
				objectsToHide[i].SetActive(isController ? this._showOnController : this._showOnKBM);
			}
		}

		[SerializeField]
		private bool _showOnController;

		[SerializeField]
		private bool _showOnKBM;

		[SerializeField]
		private GameObject[] _objectsToHide;
	}
}
