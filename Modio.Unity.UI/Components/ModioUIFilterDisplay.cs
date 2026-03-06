using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Modio.Mods;
using Modio.Unity.UI.Components.Selectables;
using Modio.Unity.UI.Search;
using UnityEngine;
using UnityEngine.Events;

namespace Modio.Unity.UI.Components
{
	public class ModioUIFilterDisplay : MonoBehaviour
	{
		private void Start()
		{
			ModioClient.OnInitialized += this.UpdateTags;
			if (!this._hasRegisteredListener)
			{
				this.RegisterListener();
			}
		}

		private void OnDestroy()
		{
			ModioClient.OnInitialized -= this.UpdateTags;
		}

		private void OnEnable()
		{
			this.RegisterListener();
		}

		private void RegisterListener()
		{
			if (ModioUISearch.Default == null)
			{
				return;
			}
			this._hasRegisteredListener = true;
			ModioUISearch.Default.OnSearchUpdatedUnityEvent.AddListener(new UnityAction(this.UpdateActiveTags));
			this.UpdateActiveTags();
		}

		private void OnDisable()
		{
			ModioUISearch.Default.OnSearchUpdatedUnityEvent.RemoveListener(new UnityAction(this.UpdateActiveTags));
		}

		public GameObject GetDefaultSelection()
		{
			if (this.checkboxTagItems.Count <= 0)
			{
				return null;
			}
			return (this.checkboxTagItems.FirstOrDefault((ModioUIFilterDisplay.TagEntry t) => t.Toggle.isOn) ?? this.checkboxTagItems.First<ModioUIFilterDisplay.TagEntry>()).Toggle.gameObject;
		}

		public void UpdateActiveTags()
		{
			if (this._hasLocalChanges)
			{
				return;
			}
			ModSearchFilter lastSearchFilter = ModioUISearch.Default.LastSearchFilter;
			foreach (ModioUIFilterDisplay.TagEntry tagEntry in this.checkboxTagItems)
			{
				tagEntry.Toggle.isOn = lastSearchFilter.GetTags().Contains(tagEntry.TagName);
			}
		}

		public void ApplyFilter()
		{
			IEnumerable<string> tags = from tagItem in this.checkboxTagItems
			where tagItem.Toggle.isOn
			select tagItem.TagName;
			this._hasLocalChanges = false;
			ModioUISearch.Default.ApplyTagsToSearch(tags);
		}

		public void ClearFilter()
		{
			foreach (ModioUIFilterDisplay.TagEntry tagEntry in this.checkboxTagItems)
			{
				tagEntry.Toggle.isOn = false;
			}
			this._hasLocalChanges = false;
		}

		private void UpdateTags()
		{
			ModioUIFilterDisplay.<UpdateTags>d__18 <UpdateTags>d__;
			<UpdateTags>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<UpdateTags>d__.<>4__this = this;
			<UpdateTags>d__.<>1__state = -1;
			<UpdateTags>d__.<>t__builder.Start<ModioUIFilterDisplay.<UpdateTags>d__18>(ref <UpdateTags>d__);
		}

		[CompilerGenerated]
		internal static void <UpdateTags>g__HideListItems|18_0<T>(ref List<T> pool) where T : MonoBehaviour
		{
			foreach (T t in pool)
			{
				Object.Destroy(t.gameObject);
			}
			pool.Clear();
		}

		[CompilerGenerated]
		internal static void <UpdateTags>g__HideListCheckboxItems|18_1(List<ModioUIFilterDisplay.TagEntry> pool)
		{
			foreach (ModioUIFilterDisplay.TagEntry tagEntry in pool)
			{
				Object.Destroy(tagEntry.Toggle.gameObject);
			}
			pool.Clear();
		}

		[SerializeField]
		private ModioUIToggle checkboxTagItemPrefab;

		private List<ModioUIFilterDisplay.TagEntry> checkboxTagItems = new List<ModioUIFilterDisplay.TagEntry>();

		[SerializeField]
		private ModioUIToggle radioTagItemPrefab;

		[SerializeField]
		private ModioUIToggle categoryItemPrefab;

		[SerializeField]
		private Transform _contentContainer;

		private List<ModioUIFilterTagCategory> categoryItems = new List<ModioUIFilterTagCategory>();

		private bool _hasRegisteredListener;

		private bool _hasLocalChanges;

		private class TagEntry
		{
			public ModioUIToggle Toggle;

			public string TagName;
		}
	}
}
