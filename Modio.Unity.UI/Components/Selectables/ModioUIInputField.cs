using System;
using Modio.Platforms;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Modio.Unity.UI.Components.Selectables
{
	public class ModioUIInputField : TMP_InputField, IModioUISelectable
	{
		public override int layoutPriority
		{
			get
			{
				return this._layoutPriority;
			}
		}

		public event IModioUISelectable.SelectableStateChangeDelegate StateChanged;

		public IModioUISelectable.SelectionState State { get; private set; }

		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			IVirtualKeyboardHandler virtualKeyboardHandler;
			if (!ModioServices.TryResolve<IVirtualKeyboardHandler>(out virtualKeyboardHandler))
			{
				return;
			}
			ModioVirtualKeyboardType virtualKeyboardType = ModioVirtualKeyboardType.Default;
			if (base.contentType == TMP_InputField.ContentType.EmailAddress)
			{
				virtualKeyboardType = ModioVirtualKeyboardType.EmailAddress;
			}
			virtualKeyboardHandler.OpenVirtualKeyboard(null, null, base.text, virtualKeyboardType, base.characterLimit, base.multiLine, delegate(string s)
			{
				base.text = s;
				this.OnSubmit(null);
				this.OnDeselect(null);
			});
		}

		protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
		{
			base.DoStateTransition(state, instant);
			this.State = (IModioUISelectable.SelectionState)state;
			IModioUISelectable.SelectableStateChangeDelegate stateChanged = this.StateChanged;
			if (stateChanged == null)
			{
				return;
			}
			stateChanged(this.State, instant);
		}

		public void DoVisualOnlyStateTransition(IModioUISelectable.SelectionState state, bool instant)
		{
			this.DoStateTransition((Selectable.SelectionState)state, instant);
		}

		[SerializeField]
		private int _layoutPriority = 1;
	}
}
