using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Modio.Mods;
using Modio.Unity.UI.Components;
using Modio.Unity.UI.Search;
using UnityEngine.UI;

namespace Modio.Unity.UI.Panels
{
	public class ModSortPanel : ModioPanelBase
	{
		protected override void Awake()
		{
			base.Awake();
			this._toggles = base.GetComponentsInChildren<Toggle>(true);
		}

		public override void DoDefaultSelection()
		{
			Toggle toggle = this._toggles.FirstOrDefault((Toggle t) => t.isOn);
			this.SetSelectedGameObject(((toggle != null) ? toggle.gameObject : null) ?? this._toggles.First<Toggle>().gameObject);
		}

		public override void OnGainedFocus(ModioPanelBase.GainedFocusCause selectionBehaviour)
		{
			SortModsBy sortBy = ModioUISearch.Default.LastSearchFilter.SortBy;
			foreach (Toggle toggle in this._toggles)
			{
				bool isOn = toggle.GetComponent<ModioUISortModsToggle>().SortModsBy == sortBy;
				toggle.isOn = isOn;
			}
			base.OnGainedFocus(selectionBehaviour);
		}

		public void ApplySort()
		{
			ModSortPanel.<ApplySort>d__4 <ApplySort>d__;
			<ApplySort>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<ApplySort>d__.<>4__this = this;
			<ApplySort>d__.<>1__state = -1;
			<ApplySort>d__.<>t__builder.Start<ModSortPanel.<ApplySort>d__4>(ref <ApplySort>d__);
		}

		private Toggle[] _toggles;
	}
}
