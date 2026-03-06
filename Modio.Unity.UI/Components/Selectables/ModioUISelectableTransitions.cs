using System;
using System.Linq;
using Modio.Unity.UI.Components.Selectables.Transitions;
using Modio.Unity.UI.Input;
using UnityEngine;

namespace Modio.Unity.UI.Components.Selectables
{
	public class ModioUISelectableTransitions : MonoBehaviour
	{
		private void Awake()
		{
			this._owner = base.GetComponentInParent<IModioUISelectable>();
			this._toggle = (this._owner as ModioUIToggle);
			this._monoBehaviourEvents = (this._transitions.Any((ISelectableTransition property) => property is IPropertyMonoBehaviourEvents) ? this._transitions.OfType<IPropertyMonoBehaviourEvents>().ToArray<IPropertyMonoBehaviourEvents>() : Array.Empty<IPropertyMonoBehaviourEvents>());
			if (this._owner == null && base.enabled)
			{
				Debug.Log(base.GetType().Name + " " + base.gameObject.name + " could not find an IModioUISelectable, disabling.", this);
				base.enabled = false;
			}
		}

		private void Start()
		{
			IPropertyMonoBehaviourEvents[] monoBehaviourEvents = this._monoBehaviourEvents;
			for (int i = 0; i < monoBehaviourEvents.Length; i++)
			{
				monoBehaviourEvents[i].Start();
			}
		}

		private void OnEnable()
		{
			IPropertyMonoBehaviourEvents[] monoBehaviourEvents = this._monoBehaviourEvents;
			for (int i = 0; i < monoBehaviourEvents.Length; i++)
			{
				monoBehaviourEvents[i].OnEnable();
			}
			if (this._owner != null)
			{
				this._owner.StateChanged += this.OnSelectionStateChanged;
				this.OnSelectionStateChanged(this._owner.State, true);
			}
		}

		private void OnDisable()
		{
			if (this._owner != null)
			{
				this._owner.StateChanged -= this.OnSelectionStateChanged;
			}
			ModioUIInput.SwappedControlScheme -= this.OnSwappedToController;
			IPropertyMonoBehaviourEvents[] monoBehaviourEvents = this._monoBehaviourEvents;
			for (int i = 0; i < monoBehaviourEvents.Length; i++)
			{
				monoBehaviourEvents[i].OnDisable();
			}
		}

		private void OnDestroy()
		{
			if (this._owner != null)
			{
				this._owner.StateChanged -= this.OnSelectionStateChanged;
			}
			ModioUIInput.SwappedControlScheme -= this.OnSwappedToController;
			IPropertyMonoBehaviourEvents[] monoBehaviourEvents = this._monoBehaviourEvents;
			for (int i = 0; i < monoBehaviourEvents.Length; i++)
			{
				monoBehaviourEvents[i].OnDestroy();
			}
		}

		private void OnSelectionStateChanged(IModioUISelectable.SelectionState state, bool instant)
		{
			if (this._toggle != null)
			{
				ModioUISelectableTransitions.ToggleFilter toggleFilter = this._toggle.isOn ? ModioUISelectableTransitions.ToggleFilter.OnlyOn : ModioUISelectableTransitions.ToggleFilter.OnlyOff;
				if (!this._toggleFilter.HasFlag(toggleFilter))
				{
					return;
				}
			}
			else if (this._toggleFilter == ModioUISelectableTransitions.ToggleFilter.OnlyOn)
			{
				return;
			}
			ModioUIInput.SwappedControlScheme -= this.OnSwappedToController;
			if (state == IModioUISelectable.SelectionState.Highlighted)
			{
				if (ModioUIInput.IsUsingGamepad)
				{
					state = IModioUISelectable.SelectionState.Normal;
				}
				ModioUIInput.SwappedControlScheme += this.OnSwappedToController;
			}
			ISelectableTransition[] transitions = this._transitions;
			for (int i = 0; i < transitions.Length; i++)
			{
				transitions[i].OnSelectionStateChanged(state, instant);
			}
		}

		private void OnSwappedToController(bool isController)
		{
			if (this._owner != null && this._owner.State == IModioUISelectable.SelectionState.Highlighted)
			{
				this.OnSelectionStateChanged(IModioUISelectable.SelectionState.Highlighted, false);
			}
		}

		[SerializeField]
		[Tooltip("Use to limit transitions to a toggle value.\ne.g. \"Only On\" will only trigger if the toggle is on. ")]
		private ModioUISelectableTransitions.ToggleFilter _toggleFilter = ModioUISelectableTransitions.ToggleFilter.Any;

		[SerializeReference]
		private ISelectableTransition[] _transitions;

		private IPropertyMonoBehaviourEvents[] _monoBehaviourEvents;

		private IModioUISelectable _owner;

		private ModioUIToggle _toggle;

		public enum ToggleFilter
		{
			Any = 3,
			OnlyOn = 1,
			OnlyOff
		}
	}
}
