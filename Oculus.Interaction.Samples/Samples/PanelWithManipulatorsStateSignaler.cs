using System;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
	public class PanelWithManipulatorsStateSignaler : MonoBehaviour
	{
		public event Action<PanelWithManipulatorsStateSignaler.State> WhenStateChanged = delegate(PanelWithManipulatorsStateSignaler.State newState)
		{
		};

		public PanelWithManipulatorsStateSignaler.State CurrentState
		{
			get
			{
				return this._state;
			}
			set
			{
				if (value != this._state)
				{
					this._state = value;
					this.WhenStateChanged(this._state);
				}
			}
		}

		private PanelWithManipulatorsStateSignaler.State _state;

		public enum State
		{
			Default,
			Selected,
			Idle
		}
	}
}
