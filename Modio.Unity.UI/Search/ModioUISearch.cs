using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio.API;
using Modio.Mods;
using Modio.Unity.Settings;
using Modio.Unity.UI.Components;
using Modio.Users;
using UnityEngine;
using UnityEngine.Events;

namespace Modio.Unity.UI.Search
{
	public class ModioUISearch : MonoBehaviour, IModioUIPropertiesOwner
	{
		public static ModioUISearch Default { get; private set; }

		public ModSearchFilter LastSearchFilter { get; private set; } = new ModSearchFilter(0, 100);

		public SpecialSearchType LastSearchPreset
		{
			get
			{
				return this._searchPreset;
			}
		}

		public bool IsSearching { get; private set; }

		public bool IsAdditiveSearch { get; private set; }

		public IReadOnlyList<Mod> LastSearchResultMods { get; private set; } = new Collection<Mod>();

		public int LastSearchResultModCount { get; private set; }

		public int LastSearchResultPageCount
		{
			get
			{
				return Mathf.CeilToInt((float)this.LastSearchResultModCount / (float)Mathf.Max(this.LastSearchFilter.PageSize, this.LastSearchResultMods.Count));
			}
		}

		public bool CanGetMoreMods
		{
			get
			{
				return this.LastSearchResultMods != null && this.LastSearchResultModCount > this.LastSearchResultMods.Count;
			}
		}

		public Error LastSearchError { get; private set; } = Error.None;

		public int LastSearchSelectionIndex { get; private set; }

		public ModioUISearchSettings LastSearchSettingsFrom { get; private set; }

		public bool SortByOverriden { get; private set; }

		public int DefaultPageSize
		{
			get
			{
				return this._defaultPageSize;
			}
		}

		public event Action AppliedSearchPreset;

		private void Awake()
		{
			if (this._isDefault || ModioUISearch.Default == null)
			{
				ModioUISearch.Default = this;
			}
		}

		private void OnDestroy()
		{
			ModioClient.OnInitialized -= this.PluginReady;
			if (ModioUISearch.Default == this)
			{
				ModioUISearch.Default = null;
			}
			User.OnUserChanged -= this.PluginReady;
		}

		private void Start()
		{
			ModioClient.OnInitialized += this.PluginReady;
		}

		private void PluginReady(User _)
		{
			this.PluginReady();
		}

		private void PluginReady()
		{
			User.OnUserChanged -= this.PluginReady;
			if (!this._allowSearchWithoutUser && (User.Current == null || !User.Current.IsInitialized))
			{
				this.LastSearchResultMods = new Collection<Mod>();
				User.OnUserChanged += this.PluginReady;
				return;
			}
			if (this._resetToSearch.Item1 != null)
			{
				this.ClearSearch();
				return;
			}
			if (this._searchOnStart != null)
			{
				this._searchOnStart.Search(this);
				return;
			}
			this.LastSearchResultMods = new Collection<Mod>();
			this.OnSearchUpdatedUnityEvent.Invoke();
		}

		public void AddUpdatePropertiesListener(UnityAction listener)
		{
			this.OnSearchUpdatedUnityEvent.AddListener(listener);
		}

		public void RemoveUpdatePropertiesListener(UnityAction listener)
		{
			this.OnSearchUpdatedUnityEvent.RemoveListener(listener);
		}

		public void ApplySortBy(SortModsBy sortModsBy, bool ascending)
		{
			this.SortByOverriden = true;
			this.LastSearchFilter.SortBy = sortModsBy;
			this.LastSearchFilter.IsSortAscending = ascending;
			this.LastSearchFilter.PageIndex = 0;
			this.SetSearch(this.LastSearchFilter, false, null);
		}

		public void ApplySearchPhrase(string query)
		{
			ModSearchFilter modSearchFilter = this.LastSearchFilter;
			if (this._baseForCustomSearch.Item1 != null && this._baseForCustomSearch.Item1 != modSearchFilter)
			{
				modSearchFilter = this._baseForCustomSearch.Item1;
				this._searchPreset = this._baseForCustomSearch.Item2;
			}
			Filtering filtering = Filtering.Like;
			modSearchFilter.ClearSearchPhrases(filtering);
			if (!string.IsNullOrEmpty(query))
			{
				modSearchFilter.AddSearchPhrase(query, filtering);
			}
			modSearchFilter.PageIndex = 0;
			this.SetSearch(modSearchFilter, false, null);
		}

		public void ApplyTagsToSearch(IEnumerable<string> tags)
		{
			this.LastSearchFilter.ClearTags();
			this.LastSearchFilter.AddTags(tags);
			if (this._searchPreset == SpecialSearchType.SearchForTag && !this.LastSearchFilter.GetTags().Any<string>())
			{
				this.ClearSearch();
				return;
			}
			this.LastSearchFilter.PageIndex = 0;
			this.SetSearch(this.LastSearchFilter, false, null);
		}

		public bool HasCustomSearch()
		{
			return this.LastSearchFilter.GetUsers().Count > 0 || this.LastSearchFilter.GetSearchPhrase(Filtering.Like).Count > 0 || this._searchPreset == SpecialSearchType.SearchForTag || this._searchPreset == SpecialSearchType.SearchForUser;
		}

		public void ClearSearch()
		{
			if (this._resetToSearch.Item1 != null)
			{
				ModSearchFilter item = this._resetToSearch.Item1;
				item.ClearSearchPhrases(Filtering.Like);
				item.ClearTags();
				item.PageIndex = 0;
				this.SetSearch(item, this._resetToSearch.Item2, false, null, null);
				return;
			}
			Debug.LogWarning("No default search available to reset back to");
		}

		public void SetSearchForUser(UserProfile user)
		{
			ModSearchFilter modSearchFilter;
			if (this._searchForUser != null)
			{
				modSearchFilter = this._searchForUser.GetSearchFilter(this._defaultPageSize);
			}
			else
			{
				modSearchFilter = new ModSearchFilter(0, this._defaultPageSize)
				{
					RevenueType = this.LastSearchFilter.RevenueType,
					ShowMatureContent = this.LastSearchFilter.ShowMatureContent
				};
			}
			modSearchFilter.AddUser(user);
			this.SetSearch(modSearchFilter, SpecialSearchType.SearchForUser, false, null, null);
		}

		public void SetSearchForTag(ModTag tag)
		{
			ModSearchFilter modSearchFilter;
			if (this._searchForTag != null)
			{
				modSearchFilter = this._searchForTag.GetSearchFilter(this._defaultPageSize);
			}
			else
			{
				modSearchFilter = new ModSearchFilter(0, this._defaultPageSize)
				{
					RevenueType = this.LastSearchFilter.RevenueType,
					ShowMatureContent = this.LastSearchFilter.ShowMatureContent
				};
			}
			modSearchFilter.AddTag(tag.ApiName);
			this.SetSearch(modSearchFilter, SpecialSearchType.SearchForTag, false, null, null);
		}

		public void GetNextPageAdditivelyForLastSearch()
		{
			this.LastSearchFilter.PageIndex = this._lastPageIndex + 1;
			if (this._lastLocalQueryInFull != null)
			{
				this.LastSearchSelectionIndex = this.LastSearchResultMods.Count;
				int count = Mathf.Min(this._lastLocalQueryInFull.Count, this.LastSearchResultMods.Count + this.LastSearchFilter.PageSize);
				this.LastSearchResultMods = this._lastLocalQueryInFull.Take(count).ToList<Mod>();
				this.OnSearchUpdatedUnityEvent.Invoke();
				return;
			}
			this.SetSearch(this.LastSearchFilter, true, null);
		}

		public void SetPageForCurrentSearch(int page)
		{
			this.LastSearchFilter.PageIndex = page;
			this.SetSearch(this.LastSearchFilter, false, null);
		}

		public void SetSearch(ModSearchFilter searchFilter, SpecialSearchType specialSearchType, bool resetToThis = false, object shareFiltersWith = null, ModioUISearchSettings settingsFrom = null)
		{
			if (resetToThis)
			{
				this._resetToSearch = new ValueTuple<ModSearchFilter, SpecialSearchType, object>(searchFilter, specialSearchType, shareFiltersWith);
			}
			this.LastSearchSettingsFrom = settingsFrom;
			if (!ModioClient.IsInitialized || (!this._allowSearchWithoutUser && (User.Current == null || !User.Current.IsInitialized)))
			{
				if (resetToThis)
				{
					ModioLog verbose = ModioLog.Verbose;
					if (verbose == null)
					{
						return;
					}
					verbose.Log("Attempting to set search before plugin is ready. Search will run once plugin is ready");
					return;
				}
				else
				{
					ModioLog warning = ModioLog.Warning;
					if (warning == null)
					{
						return;
					}
					warning.Log("Attempting to set search before plugin is ready. As resetToThis is false, this search will be discarded");
					return;
				}
			}
			else
			{
				this._searchPreset = specialSearchType;
				this.SortByOverriden = false;
				if (shareFiltersWith != null && shareFiltersWith == this._shareFiltersWith)
				{
					searchFilter.AddTags(this.LastSearchFilter.GetTags());
					for (Filtering filtering = Filtering.None; filtering <= Filtering.BitwiseAnd; filtering++)
					{
						searchFilter.AddSearchPhrases(this.LastSearchFilter.GetSearchPhrase(filtering), filtering);
					}
				}
				this._shareFiltersWith = shareFiltersWith;
				ModioComponentUISettings platformSettings = ModioClient.Settings.GetPlatformSettings<ModioComponentUISettings>();
				if (platformSettings == null || !platformSettings.ShowMonetizationUI)
				{
					searchFilter.RevenueType = RevenueType.Free;
				}
				this.SetSearch(searchFilter, false, null);
				Action appliedSearchPreset = this.AppliedSearchPreset;
				if (appliedSearchPreset == null)
				{
					return;
				}
				appliedSearchPreset();
				return;
			}
		}

		public void SetCustomSearchBase(ModSearchFilter searchFilter, SpecialSearchType searchType)
		{
			this._baseForCustomSearch = new ValueTuple<ModSearchFilter, SpecialSearchType>(searchFilter, searchType);
		}

		private void SetSearch(ModSearchFilter searchFilter, bool isAdditiveSearch = false, [TupleElementNames(new string[]
		{
			"error",
			"mods",
			"totalCount"
		})] Task<ValueTuple<Error, IReadOnlyList<Mod>, int>> customResultProvider = null)
		{
			ModioUISearch.<SetSearch>d__83 <SetSearch>d__;
			<SetSearch>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<SetSearch>d__.<>4__this = this;
			<SetSearch>d__.searchFilter = searchFilter;
			<SetSearch>d__.isAdditiveSearch = isAdditiveSearch;
			<SetSearch>d__.customResultProvider = customResultProvider;
			<SetSearch>d__.<>1__state = -1;
			<SetSearch>d__.<>t__builder.Start<ModioUISearch.<SetSearch>d__83>(ref <SetSearch>d__);
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"mods",
			"totalCount"
		})]
		private Task<ValueTuple<Error, IReadOnlyList<Mod>, int>> GetModsViaStandardQuery()
		{
			ModioUISearch.<GetModsViaStandardQuery>d__84 <GetModsViaStandardQuery>d__;
			<GetModsViaStandardQuery>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, IReadOnlyList<Mod>, int>>.Create();
			<GetModsViaStandardQuery>d__.<>4__this = this;
			<GetModsViaStandardQuery>d__.<>1__state = -1;
			<GetModsViaStandardQuery>d__.<>t__builder.Start<ModioUISearch.<GetModsViaStandardQuery>d__84>(ref <GetModsViaStandardQuery>d__);
			return <GetModsViaStandardQuery>d__.<>t__builder.Task;
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"mods",
			"totalCount"
		})]
		private Task<ValueTuple<Error, IReadOnlyList<Mod>, int>> GetCurrentUserCreationsQuery()
		{
			ModioUISearch.<GetCurrentUserCreationsQuery>d__85 <GetCurrentUserCreationsQuery>d__;
			<GetCurrentUserCreationsQuery>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, IReadOnlyList<Mod>, int>>.Create();
			<GetCurrentUserCreationsQuery>d__.<>4__this = this;
			<GetCurrentUserCreationsQuery>d__.<>1__state = -1;
			<GetCurrentUserCreationsQuery>d__.<>t__builder.Start<ModioUISearch.<GetCurrentUserCreationsQuery>d__85>(ref <GetCurrentUserCreationsQuery>d__);
			return <GetCurrentUserCreationsQuery>d__.<>t__builder.Task;
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"mods",
			"totalCount"
		})]
		private Task<ValueTuple<Error, IReadOnlyList<Mod>, int>> GetModsViaLocalQuery()
		{
			ModioUISearch.<GetModsViaLocalQuery>d__86 <GetModsViaLocalQuery>d__;
			<GetModsViaLocalQuery>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, IReadOnlyList<Mod>, int>>.Create();
			<GetModsViaLocalQuery>d__.<>4__this = this;
			<GetModsViaLocalQuery>d__.<>1__state = -1;
			<GetModsViaLocalQuery>d__.<>t__builder.Start<ModioUISearch.<GetModsViaLocalQuery>d__86>(ref <GetModsViaLocalQuery>d__);
			return <GetModsViaLocalQuery>d__.<>t__builder.Task;
		}

		private bool MatchesFilter(Mod mod)
		{
			using (IEnumerator<string> enumerator = this.LastSearchFilter.GetTags().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string tag = enumerator.Current;
					if (mod.Tags.All((ModTag modTag) => modTag.ApiName != tag))
					{
						return false;
					}
				}
			}
			foreach (string value in this.LastSearchFilter.GetSearchPhrase(Filtering.None))
			{
				if (mod.Name.IndexOf(value, StringComparison.InvariantCultureIgnoreCase) < 0)
				{
					return false;
				}
			}
			return true;
		}

		private int SortModComparer(Mod x, Mod y)
		{
			int num;
			switch (this.LastSearchFilter.SortBy)
			{
			case SortModsBy.Name:
				num = string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
				break;
			case SortModsBy.Price:
				num = -x.Price.CompareTo(y.Price);
				break;
			case SortModsBy.Rating:
				num = -x.Stats.RatingsPercent.CompareTo(y.Stats.RatingsPercent);
				break;
			case SortModsBy.Popular:
				num = -x.Stats.RatingsPositive.CompareTo(y.Stats.RatingsPositive);
				break;
			case SortModsBy.Downloads:
				num = -x.Stats.Downloads.CompareTo(y.Stats.Downloads);
				break;
			case SortModsBy.Subscribers:
				num = -x.Stats.Subscribers.CompareTo(y.Stats.Subscribers);
				break;
			case SortModsBy.DateSubmitted:
				num = -x.DateLive.CompareTo(y.DateLive);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			int num2 = num;
			if (this.LastSearchFilter.IsSortAscending)
			{
				num2 = -num2;
			}
			return num2;
		}

		public void SetSearchForDependencies(Mod dependant)
		{
			ModioUISearch.<>c__DisplayClass89_0 CS$<>8__locals1 = new ModioUISearch.<>c__DisplayClass89_0();
			CS$<>8__locals1.dependant = dependant;
			this.SetSearch(new ModSearchFilter(0, 100), false, CS$<>8__locals1.<SetSearchForDependencies>g__GetModsViaDependencies|0());
		}

		[SerializeField]
		private bool _isDefault = true;

		[Header("Optional Overrides")]
		[SerializeField]
		private ModioUISearchSettings _searchOnStart;

		[SerializeField]
		private ModioUISearchSettings _searchForUser;

		[SerializeField]
		private ModioUISearchSettings _searchForTag;

		[SerializeField]
		private int _defaultPageSize = 24;

		[SerializeField]
		[Tooltip("Allow search to run before we have an authenticated user")]
		private bool _allowSearchWithoutUser;

		private SpecialSearchType _searchPreset;

		public UnityEvent OnSearchUpdatedUnityEvent;

		[TupleElementNames(new string[]
		{
			"searchFilter",
			"specialSearchType",
			"shareFiltersWith"
		})]
		private ValueTuple<ModSearchFilter, SpecialSearchType, object> _resetToSearch;

		[TupleElementNames(new string[]
		{
			"searchFilter",
			"specialSearchType"
		})]
		private ValueTuple<ModSearchFilter, SpecialSearchType> _baseForCustomSearch;

		private int _lastPageIndex;

		private int _asyncSearchIndex;

		private object _shareFiltersWith;

		private List<Mod> _lastLocalQueryInFull;
	}
}
