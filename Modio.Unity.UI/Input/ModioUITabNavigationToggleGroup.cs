using System;
using Modio.Unity.UI.Panels;
using UnityEngine;
using UnityEngine.UI;

namespace Modio.Unity.UI.Input
{
	public class ModioUITabNavigationToggleGroup : ToggleGroup
	{
		protected override void Awake()
		{
			base.Awake();
			this._parentPanel = base.GetComponentInParent<ModioPanelBase>();
		}

		protected override void OnEnable()
		{
			if (this._parentPanel != null)
			{
				this._parentPanel.OnHasFocusChanged += this.OnPanelChangedFocus;
				if (this._parentPanel.HasFocus)
				{
					this.OnPanelChangedFocus(true);
				}
			}
			else
			{
				this.OnPanelChangedFocus(true);
			}
			base.OnEnable();
		}

		protected override void OnDisable()
		{
			if (this._parentPanel != null)
			{
				this._parentPanel.OnHasFocusChanged -= this.OnPanelChangedFocus;
			}
			this.OnPanelChangedFocus(false);
			base.OnDisable();
		}

		private void OnPanelChangedFocus(bool hasFocus)
		{
			if (hasFocus)
			{
				ModioUIInput.AddHandler(this._leftAction, new Action(this.TabLeft));
				ModioUIInput.AddHandler(this._rightAction, new Action(this.TabRight));
				return;
			}
			ModioUIInput.RemoveHandler(this._leftAction, new Action(this.TabLeft));
			ModioUIInput.RemoveHandler(this._rightAction, new Action(this.TabRight));
		}

		private void TabLeft()
		{
			this.m_Toggles[this.ClampIndex(this.IsOnIndex() - 1)].isOn = true;
		}

		private void TabRight()
		{
			this.m_Toggles[this.ClampIndex(this.IsOnIndex() + 1)].isOn = true;
		}

		private int ClampIndex(int newIndex)
		{
			if (this._loopSelection)
			{
				return (newIndex + this.m_Toggles.Count) % this.m_Toggles.Count;
			}
			return Mathf.Clamp(newIndex, 0, this.m_Toggles.Count - 1);
		}

		private int IsOnIndex()
		{
			this.m_Toggles.Sort((Toggle a, Toggle b) => a.transform.position.x.CompareTo(b.transform.position.x));
			for (int i = 0; i < this.m_Toggles.Count; i++)
			{
				if (this.m_Toggles[i].isOn)
				{
					return i;
				}
			}
			return 0;
		}

		[SerializeField]
		private ModioUIInput.ModioAction _leftAction = ModioUIInput.ModioAction.TabLeft;

		[SerializeField]
		private ModioUIInput.ModioAction _rightAction = ModioUIInput.ModioAction.TabRight;

		[SerializeField]
		private bool _loopSelection;

		private ModioPanelBase _parentPanel;
	}
}
