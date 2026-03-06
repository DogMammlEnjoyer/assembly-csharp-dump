using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Modio.Mods;
using Modio.Unity.UI.Panels;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Modio.Unity.UI.Components
{
	public class ModioUIGroup : MonoBehaviour
	{
		private void Awake()
		{
			this._template = base.GetComponentInChildren<ModioUIMod>();
			if (this._template != null)
			{
				this._template.gameObject.SetActive(false);
				this._inactive.Add(this._template);
				return;
			}
			Debug.LogWarning("ModioUIGroup " + base.gameObject.name + " could not find a child ModioUIMod template, disabling.", this);
			base.enabled = false;
		}

		private void OnEnable()
		{
			if (this._displayOnEnable.Item1 != null)
			{
				this.SetMods(this._displayOnEnable.Item1, this._displayOnEnable.Item2);
				this._displayOnEnable = default(ValueTuple<IReadOnlyList<Mod>, int>);
			}
		}

		public void SetMods(IReadOnlyList<Mod> mods, int selectionIndex = 0)
		{
			if (!base.enabled)
			{
				this._displayOnEnable = new ValueTuple<IReadOnlyList<Mod>, int>(mods, selectionIndex);
				return;
			}
			if (mods == null)
			{
				mods = Array.Empty<Mod>();
			}
			ModioUIGroup.TempActive.Clear();
			foreach (ModioUIMod modioUIMod in this._active)
			{
				if (mods.Contains(modioUIMod.Mod) && !ModioUIGroup.TempActive.ContainsKey(modioUIMod.Mod))
				{
					ModioUIGroup.TempActive.Add(modioUIMod.Mod, modioUIMod);
				}
				else
				{
					modioUIMod.gameObject.SetActive(false);
					modioUIMod.SetMod(null);
					this._inactive.Add(modioUIMod);
				}
			}
			this._active.Clear();
			for (int i = 0; i < mods.Count; i++)
			{
				ModioUIMod modioUIMod2;
				bool flag = ModioUIGroup.TempActive.Remove(mods[i], out modioUIMod2);
				if (!flag)
				{
					if (this._inactive.Any<ModioUIMod>())
					{
						int index = this._inactive.Count - 1;
						modioUIMod2 = this._inactive[index];
						this._inactive.RemoveAt(index);
					}
					else
					{
						modioUIMod2 = Object.Instantiate<GameObject>(this._template.gameObject, this._template.transform.parent).GetComponent<ModioUIMod>();
					}
					modioUIMod2.SetMod(mods[i]);
				}
				modioUIMod2.transform.SetSiblingIndex(i);
				if (!flag)
				{
					modioUIMod2.gameObject.SetActive(true);
				}
				this._active.Add(modioUIMod2);
			}
			EventSystem current = EventSystem.current;
			if (!(current == null))
			{
				GameObject currentSelectedGameObject = current.currentSelectedGameObject;
				bool flag2 = currentSelectedGameObject == null || !currentSelectedGameObject.activeInHierarchy;
				if (!flag2 && this._active.Count > 0 && selectionIndex == 0)
				{
					flag2 |= (currentSelectedGameObject.transform.parent == this._active[0].transform.parent);
				}
				if (flag2)
				{
					if (this._layoutRebuilder != null)
					{
						LayoutRebuilder.ForceRebuildLayoutImmediate(this._layoutRebuilder);
					}
					ModioPanelBase currentFocusedPanel = ModioPanelManager.GetInstance().CurrentFocusedPanel;
					if (this._active.Count > 0)
					{
						currentFocusedPanel.SetSelectedGameObject(this._active[Mathf.Min(selectionIndex, this._active.Count - 1)].gameObject);
						return;
					}
					if (currentFocusedPanel != null)
					{
						currentFocusedPanel.DoDefaultSelection();
					}
				}
				return;
			}
			ModioLog error = ModioLog.Error;
			if (error == null)
			{
				return;
			}
			error.Log("You are missing an event system, which the Modio UI requires to work. Consider adding ModioUI_InputCapture to your scene");
		}

		private static readonly Dictionary<Mod, ModioUIMod> TempActive = new Dictionary<Mod, ModioUIMod>();

		private ModioUIMod _template;

		private readonly List<ModioUIMod> _active = new List<ModioUIMod>();

		private readonly List<ModioUIMod> _inactive = new List<ModioUIMod>();

		[TupleElementNames(new string[]
		{
			"mods",
			"selectionIndex"
		})]
		private ValueTuple<IReadOnlyList<Mod>, int> _displayOnEnable;

		[SerializeField]
		[Tooltip("(Optional) The root layout to rebuild before performing selections")]
		private RectTransform _layoutRebuilder;
	}
}
