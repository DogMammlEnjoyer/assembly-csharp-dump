using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTagScripts.VirtualStumpCustomMaps.UI;
using Modio.Mods;
using Modio.Users;
using PlayFab;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CustomMapsListScreen : CustomMapsTerminalScreen
{
	public bool OfficialMapsOnly
	{
		get
		{
			return this.officialMapsOnly;
		}
	}

	public int CurrentModPage
	{
		get
		{
			return this.currentModPage;
		}
	}

	public int ModsPerPage
	{
		get
		{
			return this.modsPerPage;
		}
	}

	public SortModsBy SortType
	{
		get
		{
			return this.sortType;
		}
		set
		{
			if (this.sortType != value)
			{
				this.currentAvailableModsRequestPage = 0;
			}
			this.sortType = value;
			switch (this.sortType)
			{
			case SortModsBy.Name:
				this.isAscendingOrder = true;
				return;
			case SortModsBy.Price:
				break;
			case SortModsBy.Rating:
				this.isAscendingOrder = false;
				return;
			case SortModsBy.Popular:
				this.isAscendingOrder = false;
				return;
			case SortModsBy.Downloads:
				this.isAscendingOrder = false;
				return;
			case SortModsBy.Subscribers:
				this.isAscendingOrder = false;
				return;
			case SortModsBy.DateSubmitted:
				this.isAscendingOrder = false;
				break;
			default:
				return;
			}
		}
	}

	private void Awake()
	{
		this.subscribedBttnPosition = this.subscribedMapsButton.transform.position;
		this.searchBttnPosition = this.searchButton.transform.position;
	}

	public override void Initialize()
	{
	}

	public override void Show()
	{
		base.Show();
		ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedIn.AddListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		ModIOManager.OnModIOLoggedOut.AddListener(new UnityAction(this.OnModIOLoggedOut));
		ModIOManager.OnModIOUserChanged.RemoveListener(new UnityAction<User>(this.OnModIOUserChanged));
		ModIOManager.OnModIOUserChanged.AddListener(new UnityAction<User>(this.OnModIOUserChanged));
		ModIOManager.OnModIOCacheRefreshing.RemoveListener(new UnityAction(this.OnModCacheRefreshing));
		ModIOManager.OnModIOCacheRefreshing.AddListener(new UnityAction(this.OnModCacheRefreshing));
		ModIOManager.OnModIOCacheRefreshed.RemoveListener(new UnityAction(this.OnModCacheRefreshed));
		ModIOManager.OnModIOCacheRefreshed.AddListener(new UnityAction(this.OnModCacheRefreshed));
		if (this.featuredMods.IsNullOrEmpty<Mod>())
		{
			this.RetrieveFeaturedMods();
		}
		if (this.availableMods.IsNullOrEmpty<Mod>())
		{
			this.RetrieveAvailableMods();
		}
		this.RetrieveInstalledMods(false);
		this.RetrieveFavoriteMods(false);
		this.RetrieveSubscribedMods();
		this.RefreshScreenState();
	}

	public override void Hide()
	{
		base.Hide();
		ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		ModIOManager.OnModIOUserChanged.RemoveListener(new UnityAction<User>(this.OnModIOUserChanged));
		ModIOManager.OnModIOCacheRefreshing.RemoveListener(new UnityAction(this.OnModCacheRefreshing));
		ModIOManager.OnModIOCacheRefreshed.RemoveListener(new UnityAction(this.OnModCacheRefreshed));
	}

	private void OnModIOLoggedIn()
	{
		if (CustomMapsTerminal.IsDriver)
		{
			this.subscribedMapsButton.gameObject.SetActive(true);
		}
		this.subscribedMods = null;
		this.filteredSubscribedMods.Clear();
		this.totalSubscribedMods = 0;
		this.RetrieveSubscribedMods();
	}

	private void OnModIOLoggedOut()
	{
		this.subscribedMapsButton.gameObject.SetActive(false);
		this.subscribedMods = null;
		this.filteredSubscribedMods.Clear();
		this.totalSubscribedMods = 0;
	}

	private void OnModIOUserChanged(User user)
	{
	}

	private void OnModCacheRefreshing()
	{
		this.RefreshScreenState();
	}

	private void OnModCacheRefreshed()
	{
		this.RetrieveFavoriteMods(false);
		this.RetrieveInstalledMods(false);
		if (ModIOManager.IsLoggedIn())
		{
			this.RetrieveSubscribedMods();
		}
	}

	public override void PressButton(CustomMapKeyboardBinding buttonPressed)
	{
		if (Time.time < this.showTime + this.activationTime)
		{
			return;
		}
		GTDev.Log<string>("[CustomMapsListScreen::PressButton] Is Driver: " + CustomMapsTerminal.IsDriver.ToString() + ", Button Pressed: " + buttonPressed.ToString(), null);
		if (!CustomMapsTerminal.IsDriver)
		{
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.goback)
		{
			return;
		}
		if (this.loadingText.gameObject.activeSelf)
		{
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.option3)
		{
			ModIOManager.RefreshUserProfile(delegate(bool result)
			{
				if (result)
				{
					this.Refresh();
				}
			}, false);
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.option4)
		{
			CustomMapsTerminal.ShowSearchScreen();
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.up)
		{
			this.currentModPage--;
			this.RefreshScreenState();
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.down)
		{
			this.currentModPage++;
			this.RefreshScreenState();
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.all)
		{
			bool flag = this.officialMapsOnly;
			this.officialMapsOnly = false;
			this.displayFeaturedMods = (this.sortType == SortModsBy.Popular);
			if (flag)
			{
				this.RefreshModSearch();
			}
			this.SwapListDisplay(CustomMapsListScreen.ListScreenState.AvailableMods, flag);
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.mustplay)
		{
			bool flag2 = !this.officialMapsOnly;
			this.officialMapsOnly = true;
			this.displayFeaturedMods = false;
			if (flag2)
			{
				this.RefreshModSearch();
			}
			this.SwapListDisplay(CustomMapsListScreen.ListScreenState.AvailableMods, flag2);
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.sub)
		{
			this.SwapListDisplay(CustomMapsListScreen.ListScreenState.SubscribedMods, false);
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.fav)
		{
			this.SwapListDisplay(CustomMapsListScreen.ListScreenState.FavoriteMods, false);
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.inst)
		{
			this.SwapListDisplay(CustomMapsListScreen.ListScreenState.InstalledMods, false);
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.sort)
		{
			this.SetSortType();
			this.RefreshModSearch();
			return;
		}
		if (CustomMapKeyboardBinding.one <= buttonPressed && buttonPressed <= CustomMapKeyboardBinding.nine && !this.customMapsGalleryView.IsNull())
		{
			this.customMapsGalleryView.ShowDetailsForEntry(buttonPressed - CustomMapKeyboardBinding.one);
		}
	}

	private void SetSortType()
	{
		this.currentAvailableModsRequestPage = 0;
		this.sortTypeIndex++;
		if (this.sortTypeIndex >= 6)
		{
			this.sortTypeIndex = 0;
		}
		switch (this.sortTypeIndex)
		{
		case 0:
			this.SortType = SortModsBy.Popular;
			this.useMapName = true;
			this.displayFeaturedMods = !this.officialMapsOnly;
			return;
		case 1:
			this.SortType = SortModsBy.DateSubmitted;
			this.useMapName = true;
			this.displayFeaturedMods = false;
			return;
		case 2:
			this.SortType = SortModsBy.Rating;
			this.useMapName = false;
			this.displayFeaturedMods = false;
			return;
		case 3:
			this.SortType = SortModsBy.Downloads;
			this.useMapName = true;
			this.displayFeaturedMods = false;
			return;
		case 4:
			this.SortType = SortModsBy.Subscribers;
			this.useMapName = true;
			this.displayFeaturedMods = false;
			return;
		case 5:
			this.SortType = SortModsBy.Name;
			this.useMapName = true;
			this.displayFeaturedMods = false;
			return;
		default:
			this.sortTypeIndex = 0;
			this.SortType = SortModsBy.Popular;
			this.useMapName = true;
			this.displayFeaturedMods = !this.officialMapsOnly;
			return;
		}
	}

	public void SwapListDisplay(CustomMapsListScreen.ListScreenState newState, bool force = false)
	{
		if (this.currentState == newState && !force)
		{
			return;
		}
		if (newState == CustomMapsListScreen.ListScreenState.SubscribedMods && !ModIOManager.IsLoggedIn())
		{
			return;
		}
		this.currentState = newState;
		this.currentModPage = 0;
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			this.allMapsButton.SetButtonActive(!this.officialMapsOnly);
			this.officialMapsButton.SetButtonActive(this.officialMapsOnly);
			this.favoriteMapsButton.SetButtonActive(false);
			this.installedMapsButton.SetButtonActive(false);
			this.subscribedMapsButton.SetButtonActive(false);
			this.searchButton.SetButtonActive(false);
			break;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			this.allMapsButton.SetButtonActive(false);
			this.officialMapsButton.SetButtonActive(false);
			this.favoriteMapsButton.SetButtonActive(false);
			this.subscribedMapsButton.SetButtonActive(false);
			this.searchButton.SetButtonActive(false);
			this.installedMapsButton.SetButtonActive(true);
			break;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			this.allMapsButton.SetButtonActive(false);
			this.officialMapsButton.SetButtonActive(false);
			this.installedMapsButton.SetButtonActive(false);
			this.subscribedMapsButton.SetButtonActive(false);
			this.searchButton.SetButtonActive(false);
			this.favoriteMapsButton.SetButtonActive(true);
			break;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			this.allMapsButton.SetButtonActive(false);
			this.officialMapsButton.SetButtonActive(false);
			this.installedMapsButton.SetButtonActive(false);
			this.favoriteMapsButton.SetButtonActive(false);
			this.searchButton.SetButtonActive(false);
			this.subscribedMapsButton.SetButtonActive(true);
			break;
		}
		this.RefreshScreenState();
	}

	public void RefreshModSearch()
	{
		if (this.loadingAvailableMods || this.loadingFavoriteMods || this.loadingInstalledMods || this.loadingSubscribedMods)
		{
			return;
		}
		this.currentModPage = 0;
		this.availableMods.Clear();
		this.filteredAvailableMods.Clear();
		this.currentAvailableModsRequestPage = 0;
		this.errorLoadingAvailableMods = false;
		this.totalAvailableMods = 0;
		this.RetrieveAvailableMods();
	}

	public void Refresh()
	{
		if (this.loadingAvailableMods || this.loadingFavoriteMods || this.loadingFeaturedMods || this.loadingInstalledMods || this.loadingSubscribedMods)
		{
			return;
		}
		this.currentModPage = 0;
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			this.featuredMods.Clear();
			this.availableMods.Clear();
			this.filteredAvailableMods.Clear();
			this.currentAvailableModsRequestPage = 0;
			this.errorLoadingAvailableMods = false;
			this.totalAvailableMods = 0;
			this.RetrieveFeaturedMods();
			this.RetrieveAvailableMods();
			return;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			this.RetrieveInstalledMods(true);
			return;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			this.RetrieveFavoriteMods(true);
			return;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			this.RetrieveSubscribedMods();
			return;
		default:
			return;
		}
	}

	private void RetrieveFeaturedMods()
	{
		if (this.loadingFeaturedMods || this.featuredMods.Count > 0)
		{
			return;
		}
		this.loadingFeaturedMods = true;
		PlayFabTitleDataCache.Instance.GetTitleData(this.featuredModsPlayFabKey, new Action<string>(this.OnGetFeaturedModsTitleData), delegate(PlayFabError error)
		{
			this.loadingFeaturedMods = false;
			this.RefreshScreenState();
		}, false);
	}

	private void OnGetFeaturedModsTitleData(string data)
	{
		CustomMapsListScreen.<OnGetFeaturedModsTitleData>d__102 <OnGetFeaturedModsTitleData>d__;
		<OnGetFeaturedModsTitleData>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnGetFeaturedModsTitleData>d__.<>4__this = this;
		<OnGetFeaturedModsTitleData>d__.data = data;
		<OnGetFeaturedModsTitleData>d__.<>1__state = -1;
		<OnGetFeaturedModsTitleData>d__.<>t__builder.Start<CustomMapsListScreen.<OnGetFeaturedModsTitleData>d__102>(ref <OnGetFeaturedModsTitleData>d__);
	}

	private void RetrieveAvailableMods()
	{
		CustomMapsListScreen.<RetrieveAvailableMods>d__103 <RetrieveAvailableMods>d__;
		<RetrieveAvailableMods>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RetrieveAvailableMods>d__.<>4__this = this;
		<RetrieveAvailableMods>d__.<>1__state = -1;
		<RetrieveAvailableMods>d__.<>t__builder.Start<CustomMapsListScreen.<RetrieveAvailableMods>d__103>(ref <RetrieveAvailableMods>d__);
	}

	private void FilterAvailableMods()
	{
		this.filteredAvailableMods.Clear();
		if (this.availableMods.IsNullOrEmpty<Mod>())
		{
			return;
		}
		this.totalAvailableMods = Mathf.Max(0, this.totalAvailableMods - 1);
		foreach (Mod mod in this.availableMods)
		{
			ModId right;
			ModIOManager.TryGetNewMapsModId(out right);
			if (!(mod.Id == right) && (!this.displayFeaturedMods || this.featuredModIds.IsNullOrEmpty<long>() || !this.featuredModIds.Contains(mod.Id)))
			{
				this.filteredAvailableMods.Add(mod);
			}
		}
		if (this.displayFeaturedMods && !this.featuredMods.IsNullOrEmpty<Mod>())
		{
			this.filteredAvailableMods.InsertRange(0, this.featuredMods);
		}
	}

	private Task RetrieveSubscribedMods()
	{
		CustomMapsListScreen.<RetrieveSubscribedMods>d__105 <RetrieveSubscribedMods>d__;
		<RetrieveSubscribedMods>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<RetrieveSubscribedMods>d__.<>4__this = this;
		<RetrieveSubscribedMods>d__.<>1__state = -1;
		<RetrieveSubscribedMods>d__.<>t__builder.Start<CustomMapsListScreen.<RetrieveSubscribedMods>d__105>(ref <RetrieveSubscribedMods>d__);
		return <RetrieveSubscribedMods>d__.<>t__builder.Task;
	}

	private void FilterSubscribedMods()
	{
		this.filteredSubscribedMods.Clear();
		if (this.subscribedMods.IsNullOrEmpty<Mod>())
		{
			return;
		}
		foreach (Mod mod in this.subscribedMods)
		{
			ModId right;
			ModIOManager.TryGetNewMapsModId(out right);
			if (!(mod.Id == right))
			{
				this.filteredSubscribedMods.Add(mod);
			}
		}
	}

	private Task RetrieveInstalledMods(bool forceRefresh = false)
	{
		CustomMapsListScreen.<RetrieveInstalledMods>d__107 <RetrieveInstalledMods>d__;
		<RetrieveInstalledMods>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<RetrieveInstalledMods>d__.<>4__this = this;
		<RetrieveInstalledMods>d__.forceRefresh = forceRefresh;
		<RetrieveInstalledMods>d__.<>1__state = -1;
		<RetrieveInstalledMods>d__.<>t__builder.Start<CustomMapsListScreen.<RetrieveInstalledMods>d__107>(ref <RetrieveInstalledMods>d__);
		return <RetrieveInstalledMods>d__.<>t__builder.Task;
	}

	private void FilterInstalledMods()
	{
		this.filteredInstalledMods.Clear();
		if (this.installedMods.IsNullOrEmpty<Mod>())
		{
			return;
		}
		foreach (Mod mod in this.installedMods)
		{
			ModId right;
			if (!ModIOManager.TryGetNewMapsModId(out right) || !(mod.Id == right))
			{
				this.filteredInstalledMods.Add(mod);
			}
		}
	}

	private Task RetrieveFavoriteMods(bool forceRefresh = false)
	{
		CustomMapsListScreen.<RetrieveFavoriteMods>d__109 <RetrieveFavoriteMods>d__;
		<RetrieveFavoriteMods>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<RetrieveFavoriteMods>d__.<>4__this = this;
		<RetrieveFavoriteMods>d__.forceRefresh = forceRefresh;
		<RetrieveFavoriteMods>d__.<>1__state = -1;
		<RetrieveFavoriteMods>d__.<>t__builder.Start<CustomMapsListScreen.<RetrieveFavoriteMods>d__109>(ref <RetrieveFavoriteMods>d__);
		return <RetrieveFavoriteMods>d__.<>t__builder.Task;
	}

	private void FilterFavoriteMods()
	{
		this.filteredFavoriteMods.Clear();
		if (this.favoriteMods.IsNullOrEmpty<Mod>())
		{
			return;
		}
		foreach (Mod mod in this.favoriteMods)
		{
			ModId right;
			if (!ModIOManager.TryGetNewMapsModId(out right) || !(mod.Id == right))
			{
				this.filteredFavoriteMods.Add(mod);
			}
		}
	}

	public void GetDisplayedModList(out long[] modList)
	{
		if (this.displayedModProfiles.IsNullOrEmpty<Mod>())
		{
			modList = Array.Empty<long>();
			return;
		}
		modList = new long[this.displayedModProfiles.Count];
		for (int i = 0; i < this.displayedModProfiles.Count; i++)
		{
			modList[i] = this.displayedModProfiles[i].Id;
		}
	}

	private void RefreshScreenState()
	{
		this.displayedModProfiles.Clear();
		this.errorText.gameObject.SetActive(false);
		this.sortTypeText.gameObject.SetActive(false);
		this.modPageText.gameObject.SetActive(false);
		this.titleText.text = this.GetTitleForCurrentState();
		this.loadingText.gameObject.SetActive(true);
		if (CustomMapsTerminal.IsDriver && ModIOManager.IsLoggedIn())
		{
			this.subscribedMapsButton.gameObject.SetActive(true);
			this.subscribedMapsButton.transform.position = this.subscribedBttnPosition;
			this.searchButton.transform.position = this.searchBttnPosition;
		}
		else
		{
			this.subscribedMapsButton.gameObject.SetActive(false);
			this.subscribedMapsButton.transform.position = this.searchBttnPosition;
			this.searchButton.transform.position = this.subscribedBttnPosition;
		}
		if (this.currentState == CustomMapsListScreen.ListScreenState.AvailableMods)
		{
			this.RefreshScreenForAvailableMods();
			return;
		}
		this.sortByButton.SetActive(false);
		this.RefreshScreenForCurrentState();
	}

	private void RefreshScreenForAvailableMods()
	{
		string text = (this.sortType == SortModsBy.DateSubmitted) ? "NEWEST" : this.sortType.ToString().ToUpper();
		this.sortByButton.SetActive(true);
		this.sortTypeText.gameObject.SetActive(true);
		this.sortTypeText.text = text;
		this.customMapsGalleryView.ResetGallery();
		if (this.loadingAvailableMods)
		{
			return;
		}
		if (this.errorLoadingAvailableMods)
		{
			this.errorText.text = this.failedToRetrieveModsString;
			this.loadingText.gameObject.SetActive(false);
			this.errorText.gameObject.SetActive(true);
			return;
		}
		this.UpdatePageCount(this.totalAvailableMods);
		int num = 0;
		int num2 = this.modsPerPage - 1;
		if (!this.IsOnFirstPage())
		{
			num = this.currentModPage * this.modsPerPage;
			num2 = num + this.modsPerPage - 1;
			this.pageUpButton.gameObject.SetActive(true);
		}
		else
		{
			this.pageUpButton.gameObject.SetActive(false);
		}
		if (!this.IsOnLastPage())
		{
			this.pageDownButton.gameObject.SetActive(true);
		}
		else
		{
			this.pageDownButton.gameObject.SetActive(false);
		}
		if (this.filteredAvailableMods.Count <= num2 && this.totalAvailableMods > this.availableMods.Count)
		{
			this.displayedModProfiles.Clear();
			this.RetrieveAvailableMods();
			return;
		}
		int num3 = num;
		while (num3 <= num2 && this.filteredAvailableMods.Count > num3)
		{
			this.displayedModProfiles.Add(this.filteredAvailableMods[num3]);
			num3++;
		}
		string text2;
		if (!this.customMapsGalleryView.DisplayGallery(this.displayedModProfiles, this.useMapName, out text2))
		{
			this.errorText.text = text2;
			this.loadingText.gameObject.SetActive(false);
			this.errorText.gameObject.SetActive(true);
			return;
		}
		if (this.displayFeaturedMods && !this.featuredModIds.IsNullOrEmpty<long>())
		{
			for (int i = 0; i < this.displayedModProfiles.Count; i++)
			{
				if (this.featuredModIds.Contains(this.displayedModProfiles[i].Id))
				{
					this.customMapsGalleryView.HighlightTileAtIndex(num + i);
				}
			}
		}
		this.loadingText.gameObject.SetActive(false);
	}

	private void RefreshScreenForCurrentState()
	{
		this.customMapsGalleryView.ResetGallery();
		if (this.GetLoadingStatusForCurrentState())
		{
			return;
		}
		if (this.HasModLoadingErrorForCurrentState())
		{
			this.modPageText.gameObject.SetActive(false);
			if (CustomMapsTerminal.IsDriver)
			{
				this.currentModPage = -1;
			}
			this.errorText.text = this.failedToRetrieveModsString;
			this.loadingText.gameObject.SetActive(false);
			this.errorText.gameObject.SetActive(true);
			return;
		}
		this.UpdatePageCount(this.GetTotalModsForCurrentState());
		if (!this.IsOnFirstPage())
		{
			this.pageUpButton.gameObject.SetActive(true);
		}
		else
		{
			this.pageUpButton.gameObject.SetActive(false);
		}
		if (!this.IsOnLastPage())
		{
			this.pageDownButton.gameObject.SetActive(true);
		}
		else
		{
			this.pageDownButton.gameObject.SetActive(false);
		}
		List<Mod> modListForCurrentState = this.GetModListForCurrentState();
		if (modListForCurrentState != null)
		{
			if (this.currentState == CustomMapsListScreen.ListScreenState.CustomModList)
			{
				this.displayedModProfiles.AddRange(modListForCurrentState);
			}
			else
			{
				int num = this.currentModPage * this.modsPerPage;
				int num2 = num;
				while (num2 < num + this.modsPerPage && modListForCurrentState.Count > num2)
				{
					this.displayedModProfiles.Add(modListForCurrentState[num2]);
					num2++;
				}
			}
		}
		string text;
		if (!this.customMapsGalleryView.DisplayGallery(this.displayedModProfiles, true, out text))
		{
			this.errorText.text = text;
			this.loadingText.gameObject.SetActive(false);
			this.errorText.gameObject.SetActive(true);
			return;
		}
		this.loadingText.gameObject.SetActive(false);
	}

	private bool GetLoadingStatusForCurrentState()
	{
		if (ModIOManager.IsRefreshing())
		{
			return true;
		}
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			return this.loadingAvailableMods;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			return this.loadingInstalledMods;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			return this.loadingFavoriteMods;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			return this.loadingSubscribedMods;
		default:
			return false;
		}
	}

	private bool HasModLoadingErrorForCurrentState()
	{
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			return this.errorLoadingAvailableMods;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			return this.errorLoadingInstalledMods;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			return this.errorLoadingFavoriteMods;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			return this.errorLoadingSubscribedMods;
		default:
			return false;
		}
	}

	private List<Mod> GetModListForCurrentState()
	{
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			return this.filteredAvailableMods;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			return this.filteredInstalledMods;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			return this.filteredFavoriteMods;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			return this.filteredSubscribedMods;
		default:
			return null;
		}
	}

	private int GetTotalModsForCurrentState()
	{
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			return this.totalAvailableMods;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			return this.totalInstalledMods;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			return this.totalFavoriteMods;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			return this.totalSubscribedMods;
		default:
			return 0;
		}
	}

	private string GetTitleForCurrentState()
	{
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			if (this.officialMapsOnly)
			{
				return this.officialModsTitle;
			}
			return this.browseModsTitle;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			return this.installedModsTitle;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			return this.favoriteModsTitle;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			return this.subscribedModsTitle;
		default:
			return "";
		}
	}

	private void UpdatePageCount(int totalMods)
	{
		this.totalModCount = totalMods;
		this.modPageText.gameObject.SetActive(false);
		if (this.totalModCount != 0)
		{
			int numPages = this.GetNumPages();
			if (numPages > 1)
			{
				this.modPageText.text = string.Format("{0} / {1}", this.currentModPage + 1, numPages);
				this.modPageText.gameObject.SetActive(true);
			}
			return;
		}
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			this.errorText.text = this.noModsAvailableString;
			return;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			this.errorText.text = this.noInstalledModsString;
			return;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			this.errorText.text = this.noFavoriteModsString;
			return;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			this.errorText.text = this.noSubscribedModsString;
			return;
		case CustomMapsListScreen.ListScreenState.CustomModList:
			this.errorText.text = this.noModsFoundGenericString;
			return;
		default:
			return;
		}
	}

	public int GetNumPages()
	{
		int num = this.totalModCount % this.modsPerPage;
		int num2 = this.totalModCount / this.modsPerPage;
		if (num > 0)
		{
			num2++;
		}
		return num2;
	}

	private bool IsOnFirstPage()
	{
		return this.currentModPage == 0;
	}

	private bool IsOnLastPage()
	{
		long num = (long)this.GetNumPages();
		return (long)(this.currentModPage + 1) == num;
	}

	public void RefreshDriverNickname(string driverNickname)
	{
		if (this.currentState == CustomMapsListScreen.ListScreenState.CustomModList)
		{
			this.titleText.text = driverNickname;
		}
	}

	[SerializeField]
	private TMP_Text loadingText;

	[SerializeField]
	private TMP_Text errorText;

	[SerializeField]
	private TMP_Text modPageText;

	[SerializeField]
	private TMP_Text titleText;

	[SerializeField]
	private TMP_Text sortTypeText;

	[SerializeField]
	private GameObject sortByButton;

	[SerializeField]
	private CustomMapsScreenButton allMapsButton;

	[SerializeField]
	private CustomMapsScreenButton officialMapsButton;

	[SerializeField]
	private CustomMapsScreenButton favoriteMapsButton;

	[SerializeField]
	private CustomMapsScreenButton installedMapsButton;

	[SerializeField]
	private CustomMapsScreenButton subscribedMapsButton;

	[SerializeField]
	private CustomMapsScreenButton searchButton;

	[SerializeField]
	private CustomMapsScreenButton pageUpButton;

	[SerializeField]
	private CustomMapsScreenButton pageDownButton;

	[SerializeField]
	private CustomMapsGalleryView customMapsGalleryView;

	[SerializeField]
	private string browseModsTitle = "AVAILABLE MODS";

	[SerializeField]
	private string officialModsTitle = "OFFICIAL MODS";

	[SerializeField]
	private string installedModsTitle = "INSTALLED MODS";

	[SerializeField]
	private string favoriteModsTitle = "FAVORITE MODS";

	[SerializeField]
	private string subscribedModsTitle = "SUBSCRIBED MODS";

	[SerializeField]
	private string noModsAvailableString = "NO MODS AVAILABLE";

	[SerializeField]
	private string noModsFoundGenericString = "NO MODS FOUND";

	[SerializeField]
	private string noSubscribedModsString = "NOT SUBSCRIBED TO ANY MODS";

	[SerializeField]
	private string noInstalledModsString = "NO MODS INSTALLED";

	[SerializeField]
	private string noFavoriteModsString = "NO FAVORITE MODS FOUND";

	[SerializeField]
	private string failedToRetrieveModsString = "FAILED TO RETRIEVE MODS FROM MOD.IO \nPRESS THE 'REFRESH' BUTTON TO RETRY";

	[SerializeField]
	private int modsPerPage = 12;

	[SerializeField]
	private int numModsPerRequest = 24;

	[SerializeField]
	private int maxModListItemLength = 25;

	[SerializeField]
	private string officialMapsTag = "Official Maps";

	[SerializeField]
	private string featuredModsPlayFabKey = "VStumpFeaturedMaps";

	private bool loadingFeaturedMods;

	private bool displayFeaturedMods = true;

	private int totalFeaturedMods;

	private List<long> featuredModIds = new List<long>();

	private List<Mod> featuredMods = new List<Mod>();

	private int currentAvailableModsRequestPage;

	private bool loadingAvailableMods;

	private int totalAvailableMods;

	private bool errorLoadingAvailableMods;

	private List<Mod> availableMods = new List<Mod>();

	private List<Mod> filteredAvailableMods = new List<Mod>();

	private bool loadingInstalledMods;

	private bool errorLoadingInstalledMods;

	private int totalInstalledMods;

	private Mod[] installedMods;

	private List<Mod> filteredInstalledMods = new List<Mod>();

	private bool loadingFavoriteMods;

	private bool errorLoadingFavoriteMods;

	private int totalFavoriteMods;

	private List<Mod> favoriteMods = new List<Mod>();

	private List<Mod> filteredFavoriteMods = new List<Mod>();

	private bool loadingSubscribedMods;

	private bool errorLoadingSubscribedMods;

	private int totalSubscribedMods;

	private Mod[] subscribedMods;

	private List<Mod> filteredSubscribedMods = new List<Mod>();

	private int currentModPage;

	private int totalModCount;

	private List<Mod> displayedModProfiles = new List<Mod>();

	private int sortTypeIndex;

	private SortModsBy sortType = SortModsBy.Popular;

	private const int MAX_SORT_TYPES = 6;

	private List<string> searchTags = new List<string>();

	private bool isAscendingOrder;

	private bool officialMapsOnly;

	private bool useMapName = true;

	private Vector3 subscribedBttnPosition;

	private Vector3 searchBttnPosition;

	private bool restartCustomModListRetrieval;

	private bool restartCustomModListRetrievalForceRefresh;

	private bool restartInstalledModsRetrieval;

	private bool restartInstalledModsRetrievalForceRefresh;

	private bool restartFavoriteModsRetrieval;

	private bool restartFavoriteModsRetrievalForceRefresh;

	private bool restartSubscribedModsRetrieval;

	public CustomMapsListScreen.ListScreenState currentState;

	public enum ListScreenState
	{
		AvailableMods,
		InstalledMods,
		FavoriteMods,
		SubscribedMods,
		CustomModList
	}
}
