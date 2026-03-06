using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Oculus.Interaction.Samples
{
	public class MenuWristButton : MonoBehaviour
	{
		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this._toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleValueChanged));
			}
		}

		private void OnToggleValueChanged(bool value)
		{
			this._menuManager.ToggleMenu();
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this._toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnToggleValueChanged));
			}
		}

		public void InjectAllMenuWrist(Toggle toggle, ISDKSceneMenuManager manager)
		{
			this.InjectToggle(toggle);
			this.InjectManager(manager);
		}

		public void InjectToggle(Toggle toggle)
		{
			this._toggle = toggle;
		}

		public void InjectManager(ISDKSceneMenuManager manager)
		{
			this._menuManager = manager;
		}

		[Header("The Toggle Button")]
		[Tooltip("Place the toggle on the wrist here")]
		[SerializeField]
		private Toggle _toggle;

		[Header("The Menu Manager")]
		[Tooltip("There should only be 1 ISDK manager in the scene loacted on the ISDKMenuManager.prefab")]
		[SerializeField]
		private ISDKSceneMenuManager _menuManager;

		protected bool _started;
	}
}
