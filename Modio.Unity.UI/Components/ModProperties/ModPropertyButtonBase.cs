using System;
using Modio.Mods;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Modio.Unity.UI.Components.ModProperties
{
	[Serializable]
	public abstract class ModPropertyButtonBase<T> : IModProperty, IPropertyMonoBehaviourEvents
	{
		public void OnModUpdate(Mod mod)
		{
			this._mod = mod;
		}

		public void Start()
		{
		}

		public void OnDestroy()
		{
			if (this._button != null)
			{
				this._button.onClick.RemoveListener(new UnityAction(this.OnButtonClick));
			}
		}

		public void OnEnable()
		{
			if ((this._ignoreWhileDisabled || !this._addedListener) && this._button != null)
			{
				this._button.onClick.AddListener(new UnityAction(this.OnButtonClick));
			}
			this._addedListener = true;
		}

		public void OnDisable()
		{
			if (this._ignoreWhileDisabled && this._button != null)
			{
				this._button.onClick.RemoveListener(new UnityAction(this.OnButtonClick));
			}
		}

		protected void OnButtonClick()
		{
			if (this._mod != null)
			{
				UnityEvent<T> onClick = this._onClick;
				if (onClick == null)
				{
					return;
				}
				onClick.Invoke(this.GetProperty(this._mod));
			}
		}

		protected abstract T GetProperty(Mod mod);

		[SerializeField]
		private Button _button;

		[SerializeField]
		[Tooltip("If true, button presses are ignored while the Component or GameObject are disabled.")]
		private bool _ignoreWhileDisabled = true;

		[Space]
		[SerializeField]
		private UnityEvent<T> _onClick;

		private Mod _mod;

		private bool _addedListener;
	}
}
