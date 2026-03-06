using System;
using System.Collections.Generic;
using UnityEngine;

namespace Modio.Unity.UI.Panels
{
	public class ModioPanelManager : MonoBehaviour
	{
		public ModioPanelBase CurrentFocusedPanel
		{
			get
			{
				if (this._openWindows.Count <= 0)
				{
					return null;
				}
				return this._openWindows[this._openWindows.Count - 1];
			}
		}

		public static ModioPanelManager GetInstance()
		{
			if (ModioPanelManager._instance != null)
			{
				return ModioPanelManager._instance;
			}
			ModioPanelManager._instance = Object.FindObjectOfType<ModioPanelManager>();
			if (ModioPanelManager._instance != null)
			{
				return ModioPanelManager._instance;
			}
			ModioPanelManager._instance = new GameObject("ModioPanelManager").AddComponent<ModioPanelManager>();
			return ModioPanelManager._instance;
		}

		private void Awake()
		{
			ModioPanelManager._instance = this;
		}

		public void OpenPanel(ModioPanelBase modioPanelBase)
		{
			if (this._openWindows.Count > 0)
			{
				this._openWindows[this._openWindows.Count - 1].OnLostFocus();
			}
			this._openWindows.Add(modioPanelBase);
			modioPanelBase.OnGainedFocus(ModioPanelBase.GainedFocusCause.OpeningFromClosed);
		}

		public void ClosePanel(ModioPanelBase modioPanelBase)
		{
			bool flag = false;
			for (int i = this._openWindows.Count - 1; i >= 0; i--)
			{
				if (this._openWindows[i] == modioPanelBase)
				{
					if (i == this._openWindows.Count - 1)
					{
						flag = true;
					}
					this._openWindows.RemoveAt(i);
				}
			}
			if (flag)
			{
				modioPanelBase.OnLostFocus();
				if (this._openWindows.Count > 0)
				{
					this._openWindows[this._openWindows.Count - 1].OnGainedFocus(ModioPanelBase.GainedFocusCause.RegainingFocusFromStackedPanel);
				}
			}
		}

		public void PushFocusSuppression()
		{
			if (this._openWindows.Count > 0)
			{
				ModioPanelBase modioPanelBase = this._openWindows[this._openWindows.Count - 1];
				if (modioPanelBase.HasFocus)
				{
					modioPanelBase.OnLostFocus();
				}
			}
		}

		public void PopFocusSuppression(ModioPanelBase.GainedFocusCause gainedFocusCause)
		{
			if (this._openWindows.Count > 0)
			{
				ModioPanelBase modioPanelBase = this._openWindows[this._openWindows.Count - 1];
				if (!modioPanelBase.HasFocus)
				{
					modioPanelBase.OnGainedFocus(gainedFocusCause);
				}
			}
		}

		private void LateUpdate()
		{
			if (this._openWindows.Count > 0 && this._openWindows[this._openWindows.Count - 1].HasFocus)
			{
				this._openWindows[this._openWindows.Count - 1].FocusedPanelLateUpdate();
			}
		}

		public void RegisterPanel(ModioPanelBase modioPanelBase)
		{
			this._allPotentialPanels.Add(modioPanelBase);
		}

		public static T GetPanelOfType<T>() where T : ModioPanelBase
		{
			foreach (ModioPanelBase modioPanelBase in ModioPanelManager.GetInstance()._allPotentialPanels)
			{
				T t = modioPanelBase as T;
				if (t != null)
				{
					return t;
				}
			}
			return default(T);
		}

		private readonly List<ModioPanelBase> _allPotentialPanels = new List<ModioPanelBase>();

		private readonly List<ModioPanelBase> _openWindows = new List<ModioPanelBase>();

		private static ModioPanelManager _instance;
	}
}
