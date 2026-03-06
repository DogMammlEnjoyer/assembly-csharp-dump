using System;
using Modio.Unity.UI.Input;
using UnityEngine;
using UnityEngine.Events;

namespace Modio.Unity.UI.Components.Selectables.Transitions
{
	[Serializable]
	public class SelectableTransitionActionListenerOnSelected : ISelectableTransition, IPropertyMonoBehaviourEvents
	{
		public void OnSelectionStateChanged(IModioUISelectable.SelectionState state, bool instant)
		{
			if (state == IModioUISelectable.SelectionState.Selected)
			{
				ModioUIInput.AddHandler(this._inputAction, new Action(this.ActionPressed));
				return;
			}
			if (state != IModioUISelectable.SelectionState.Pressed)
			{
				ModioUIInput.RemoveHandler(this._inputAction, new Action(this.ActionPressed));
			}
		}

		private void ActionPressed()
		{
			this._onPressed.Invoke();
		}

		public void Start()
		{
		}

		public void OnDestroy()
		{
			ModioUIInput.RemoveHandler(this._inputAction, new Action(this.ActionPressed));
		}

		public void OnEnable()
		{
		}

		public void OnDisable()
		{
			ModioUIInput.RemoveHandler(this._inputAction, new Action(this.ActionPressed));
		}

		[SerializeField]
		private ModioUIInput.ModioAction _inputAction;

		[SerializeField]
		private UnityEvent _onPressed;
	}
}
