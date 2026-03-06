using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class VirtualSelector : MonoBehaviour, ISelector
	{
		public event Action WhenSelected = delegate()
		{
		};

		public event Action WhenUnselected = delegate()
		{
		};

		public void Select()
		{
			this._selectFlag = true;
			this.UpdateSelection();
		}

		public void Unselect()
		{
			this._selectFlag = false;
			this.UpdateSelection();
		}

		protected virtual void OnValidate()
		{
			this.UpdateSelection();
		}

		protected void UpdateSelection()
		{
			if (this._currentlySelected != this._selectFlag)
			{
				this._currentlySelected = this._selectFlag;
				if (this._currentlySelected)
				{
					this.WhenSelected();
					return;
				}
				this.WhenUnselected();
			}
		}

		[Tooltip("Toggles the selector from within the Unity inspector.")]
		[SerializeField]
		private bool _selectFlag;

		private bool _currentlySelected;
	}
}
