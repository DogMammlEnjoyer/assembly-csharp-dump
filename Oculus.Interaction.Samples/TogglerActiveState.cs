using System;
using UnityEngine;
using UnityEngine.UI;

namespace Oculus.Interaction
{
	public class TogglerActiveState : MonoBehaviour, IActiveState
	{
		public bool Active
		{
			get
			{
				return this._toggle.isOn;
			}
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		public void InjectAllTogglerActiveState(Toggle toggle)
		{
			this.InjectAllToggle(toggle);
		}

		public void InjectAllToggle(Toggle toggle)
		{
			this._toggle = toggle;
		}

		[SerializeField]
		private Toggle _toggle;

		protected bool _started;
	}
}
