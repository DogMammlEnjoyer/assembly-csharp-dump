using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio.API;
using Modio.API.SchemaDefinitions;
using Modio.Caching;
using Modio.Extensions;
using Modio.Images;
using Modio.Mods.Builder;
using Modio.Reports;
using Modio.Users;
using UnityEngine;

namespace Modio.Mods
{
	public class Mod
	{
		public event Action OnModUpdated;

		public static void AddChangeListener(ModChangeType subscribedChange, Action<Mod, ModChangeType> listener)
		{
			Action<Mod, ModChangeType> a;
			if (Mod.ChangeSubscribers.TryGetValue(subscribedChange, out a))
			{
				listener = (Action<Mod, ModChangeType>)Delegate.Combine(a, listener);
			}
			Mod.ChangeSubscribers[subscribedChange] = listener;
		}

		public static void RemoveChangeListener(ModChangeType subscribedChange, Action<Mod, ModChangeType> listener)
		{
			Action<Mod, ModChangeType> action;
			if (Mod.ChangeSubscribers.TryGetValue(subscribedChange, out action))
			{
				Dictionary<ModChangeType, Action<Mod, ModChangeType>> changeSubscribers = Mod.ChangeSubscribers;
				changeSubscribers[subscribedChange] = (Action<Mod, ModChangeType>)Delegate.Remove(changeSubscribers[subscribedChange], listener);
			}
		}

		public static ModBuilder Create()
		{
			return new ModBuilder();
		}

		public ModBuilder Edit()
		{
			return new ModBuilder(this);
		}

		public ModId Id { get; }

		public string Name { get; private set; }

		public string Summary
		{
			get
			{
				string result;
				if ((result = this._summaryDecoded) == null)
				{
					result = (this._summaryDecoded = WebUtility.HtmlDecode(this._summaryHtmlEncoded));
				}
				return result;
			}
		}

		public string Description { get; private set; }

		public DateTime DateLive { get; private set; }

		public DateTime DateUpdated { get; private set; }

		public ModTag[] Tags { get; private set; }

		public string MetadataBlob { get; private set; }

		public Dictionary<string, string> MetadataKvps { get; private set; }

		public ModCommunityOptions CommunityOptions { get; private set; }

		public ModMaturityOptions MaturityOptions { get; private set; }

		public Modfile File { get; private set; }

		public ModStats Stats { get; private set; }

		public long Price { get; private set; }

		public bool IsMonetized { get; private set; }

		public ModioImageSource<Mod.LogoResolution> Logo { get; private set; }

		public ModioImageSource<Mod.GalleryResolution>[] Gallery { get; private set; }

		public UserProfile Creator { get; private set; }

		public ModDependencies Dependencies { get; private set; }

		public ModRating CurrentUserRating { get; private set; }

		public bool IsSubscribed { get; private set; }

		public bool IsPurchased { get; private set; }

		public bool IsEnabled { get; internal set; } = true;

		internal ModObject LastModObject { get; private set; }

		internal Mod(ModId id)
		{
			this.Id = id;
		}

		internal Mod(ModObject modObject)
		{
			this.Id = modObject.Id;
			Debug.Log(string.Format("[mod.io] creating mod from modObject, ID:{0}", this.Id));
			this.ApplyDetailsFromModObject(modObject);
		}

		public static Mod Get(long id)
		{
			return ModCache.GetMod(new ModId(id));
		}

		internal Mod ApplyDetailsFromModObject(ModObject modObject)
		{
			this.LastModObject = modObject;
			this.Name = WebUtility.HtmlDecode(modObject.Name);
			this._summaryHtmlEncoded = modObject.Summary;
			this._summaryDecoded = null;
			this.Description = modObject.DescriptionPlaintext;
			if (this.File == null)
			{
				this.File = new Modfile(modObject.Modfile);
			}
			else if (modObject.Modfile.Id != 0L)
			{
				this.File.ApplyDetailsFromModfileObject(modObject.Modfile);
			}
			if (modObject.SubmittedBy.Id > 0L)
			{
				this.Creator = UserProfile.Get(modObject.SubmittedBy);
				this.DateLive = modObject.DateLive.GetUtcDateTime();
				this.DateUpdated = modObject.DateUpdated.GetUtcDateTime();
				this.Tags = modObject.Tags.Select(new Func<ModTagObject, ModTag>(ModTag.Get)).ToArray<ModTag>();
				this.MetadataBlob = modObject.MetadataBlob;
				this.MetadataKvps = modObject.MetadataKvp.ToDictionary((MetadataKvpObject kvp) => kvp.Metakey, (MetadataKvpObject kvp) => kvp.Metavalue);
				this.CommunityOptions = (ModCommunityOptions)modObject.CommunityOptions;
				this.MaturityOptions = (ModMaturityOptions)modObject.MaturityOption;
				this.Stats = new ModStats(modObject.Stats, this.CurrentUserRating);
				this.IsMonetized = (((int)modObject.MonetizationOptions & 1) != 0 && ((int)modObject.MonetizationOptions & 2) != 0);
				this.Price = modObject.Price;
				this.Logo = new ModioImageSource<Mod.LogoResolution>(modObject.Logo.Filename, new string[]
				{
					modObject.Logo.Thumb320X180,
					modObject.Logo.Thumb640X360,
					modObject.Logo.Thumb1280X720,
					modObject.Logo.Original
				});
				this.Gallery = (from imageObject in modObject.Media.Images
				select new ModioImageSource<Mod.GalleryResolution>(imageObject.Filename, new string[]
				{
					imageObject.Thumb320X180,
					imageObject.Thumb1280X720,
					imageObject.Original
				})).ToArray<ModioImageSource<Mod.GalleryResolution>>();
				this.Dependencies = new ModDependencies(this, modObject.Dependencies);
			}
			else
			{
				this.Creator = null;
				this.DateLive = 0L.GetUtcDateTime();
				this.DateUpdated = 0L.GetUtcDateTime();
				this.Tags = Array.Empty<ModTag>();
				this.MetadataBlob = "";
				this.MetadataKvps = null;
				this.CommunityOptions = ModCommunityOptions.None;
				this.MaturityOptions = ModMaturityOptions.None;
				this.Stats = null;
				this.IsMonetized = false;
				this.Price = 0L;
				this.Logo = null;
				this.Gallery = null;
			}
			this.InvokeModUpdated(ModChangeType.Everything);
			return this;
		}

		public Task<Error> Subscribe(bool includeDependencies = true)
		{
			return this.SetSubscribed(true, includeDependencies);
		}

		public Task<Error> Unsubscribe()
		{
			return this.SetSubscribed(false, true);
		}

		private Task<Error> SetSubscribed(bool subscribed, bool includeDependencies = true)
		{
			Mod.<SetSubscribed>d__111 <SetSubscribed>d__;
			<SetSubscribed>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<SetSubscribed>d__.<>4__this = this;
			<SetSubscribed>d__.subscribed = subscribed;
			<SetSubscribed>d__.includeDependencies = includeDependencies;
			<SetSubscribed>d__.<>1__state = -1;
			<SetSubscribed>d__.<>t__builder.Start<Mod.<SetSubscribed>d__111>(ref <SetSubscribed>d__);
			return <SetSubscribed>d__.<>t__builder.Task;
		}

		public void SetIsEnabled(bool isEnabled)
		{
			this.UpdateLocalEnabledStatus(isEnabled);
		}

		internal void UpdateLocalSubscriptionStatus(bool isSubscribed)
		{
			if (this.IsSubscribed == isSubscribed)
			{
				return;
			}
			this.IsSubscribed = isSubscribed;
			this.InvokeModUpdated(ModChangeType.IsSubscribed);
		}

		internal void UpdateLocalEnabledStatus(bool isEnabled)
		{
			this.IsEnabled = isEnabled;
			this.InvokeModUpdated(ModChangeType.IsEnabled);
		}

		internal void UpdateLocalPurchaseStatus(bool isPurchased)
		{
			this.IsPurchased = isPurchased;
			this.InvokeModUpdated(ModChangeType.IsPurchased);
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"page"
		})]
		public static Task<ValueTuple<Error, ModioPage<Mod>>> GetMods(ModSearchFilter filter)
		{
			return Mod.GetMods(filter.GetModsFilter(), false);
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"page"
		})]
		public static Task<ValueTuple<Error, ModioPage<Mod>>> GetMods(ModioAPI.Mods.GetModsFilter filter, bool forceRefresh = false)
		{
			Mod.<GetMods>d__117 <GetMods>d__;
			<GetMods>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, ModioPage<Mod>>>.Create();
			<GetMods>d__.filter = filter;
			<GetMods>d__.forceRefresh = forceRefresh;
			<GetMods>d__.<>1__state = -1;
			<GetMods>d__.<>t__builder.Start<Mod.<GetMods>d__117>(ref <GetMods>d__);
			return <GetMods>d__.<>t__builder.Task;
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"result"
		})]
		public Task<ValueTuple<Error, Mod>> GetModDetailsFromServer()
		{
			Mod.<GetModDetailsFromServer>d__118 <GetModDetailsFromServer>d__;
			<GetModDetailsFromServer>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Mod>>.Create();
			<GetModDetailsFromServer>d__.<>4__this = this;
			<GetModDetailsFromServer>d__.<>1__state = -1;
			<GetModDetailsFromServer>d__.<>t__builder.Start<Mod.<GetModDetailsFromServer>d__118>(ref <GetModDetailsFromServer>d__);
			return <GetModDetailsFromServer>d__.<>t__builder.Task;
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"result"
		})]
		public static Task<ValueTuple<Error, Mod>> GetMod(ModId modId, bool forceRefresh = false, ModIndex tempIndex = null, bool deferModInstallManagementRefresh = false)
		{
			Mod.<GetMod>d__119 <GetMod>d__;
			<GetMod>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Mod>>.Create();
			<GetMod>d__.modId = modId;
			<GetMod>d__.forceRefresh = forceRefresh;
			<GetMod>d__.tempIndex = tempIndex;
			<GetMod>d__.deferModInstallManagementRefresh = deferModInstallManagementRefresh;
			<GetMod>d__.<>1__state = -1;
			<GetMod>d__.<>t__builder.Start<Mod.<GetMod>d__119>(ref <GetMod>d__);
			return <GetMod>d__.<>t__builder.Task;
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			null
		})]
		public static Task<ValueTuple<Error, ICollection<Mod>>> GetMods(ICollection<long> neededModIds, bool forceRefresh = false, ModIndex tempIndex = null)
		{
			Mod.<GetMods>d__120 <GetMods>d__;
			<GetMods>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, ICollection<Mod>>>.Create();
			<GetMods>d__.neededModIds = neededModIds;
			<GetMods>d__.forceRefresh = forceRefresh;
			<GetMods>d__.tempIndex = tempIndex;
			<GetMods>d__.<>1__state = -1;
			<GetMods>d__.<>t__builder.Start<Mod.<GetMods>d__120>(ref <GetMods>d__);
			return <GetMods>d__.<>t__builder.Task;
		}

		public Task<Error> RateMod(ModRating rating)
		{
			Mod.<RateMod>d__121 <RateMod>d__;
			<RateMod>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<RateMod>d__.<>4__this = this;
			<RateMod>d__.rating = rating;
			<RateMod>d__.<>1__state = -1;
			<RateMod>d__.<>t__builder.Start<Mod.<RateMod>d__121>(ref <RateMod>d__);
			return <RateMod>d__.<>t__builder.Task;
		}

		internal void SetCurrentUserRating(ModRating rating)
		{
			this.CurrentUserRating = rating;
			ModStats stats = this.Stats;
			if (stats != null)
			{
				stats.UpdatePreviousRating(rating);
			}
			this.InvokeModUpdated(ModChangeType.Rating);
		}

		public Task<Error> Report(ReportType reportType, ModNotWorkingReason reportReason, string contact, string summary)
		{
			Mod.<Report>d__123 <Report>d__;
			<Report>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<Report>d__.<>4__this = this;
			<Report>d__.reportType = reportType;
			<Report>d__.reportReason = reportReason;
			<Report>d__.contact = contact;
			<Report>d__.summary = summary;
			<Report>d__.<>1__state = -1;
			<Report>d__.<>t__builder.Start<Mod.<Report>d__123>(ref <Report>d__);
			return <Report>d__.<>t__builder.Task;
		}

		internal void InvokeModUpdated(ModChangeType changeFlags)
		{
			foreach (KeyValuePair<ModChangeType, Action<Mod, ModChangeType>> keyValuePair in Mod.ChangeSubscribers)
			{
				ModChangeType modChangeType;
				Action<Mod, ModChangeType> action;
				keyValuePair.Deconstruct(out modChangeType, out action);
				bool flag = modChangeType != (ModChangeType)0;
				Action<Mod, ModChangeType> action2 = action;
				if (((flag ? ModChangeType.Modfile : ((ModChangeType)0)) & changeFlags) != (ModChangeType)0 && action2 != null)
				{
					action2(this, changeFlags);
				}
			}
			Action onModUpdated = this.OnModUpdated;
			if (onModUpdated == null)
			{
				return;
			}
			onModUpdated();
		}

		internal void UpdateModfile(Modfile modfile)
		{
			this.File = modfile;
			this.InvokeModUpdated(ModChangeType.Modfile);
		}

		public Task<Error> Purchase(bool subscribeOnPurchase)
		{
			Mod.<Purchase>d__126 <Purchase>d__;
			<Purchase>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<Purchase>d__.<>4__this = this;
			<Purchase>d__.subscribeOnPurchase = subscribeOnPurchase;
			<Purchase>d__.<>1__state = -1;
			<Purchase>d__.<>t__builder.Start<Mod.<Purchase>d__126>(ref <Purchase>d__);
			return <Purchase>d__.<>t__builder.Task;
		}

		public void UninstallOtherUserMod(bool force = false)
		{
			if (force || !this.IsSubscribed)
			{
				ModInstallationManagement.MarkModForUninstallation(this);
				return;
			}
			ModioLog warning = ModioLog.Warning;
			if (warning == null)
			{
				return;
			}
			warning.Log(string.Format("Attempting to uninstall mod {0}, but its subscribed. Cancelling.", this.Id));
		}

		public override string ToString()
		{
			return string.Format("Mod({0}:{1})", this.Id, this.Name);
		}

		public bool IsHidden()
		{
			return this.LastModObject.Status == 0L || this.LastModObject.Visible == 0L;
		}

		public static Task<Error> RefreshPotentiallyHiddenCachedMods()
		{
			Mod.<RefreshPotentiallyHiddenCachedMods>d__130 <RefreshPotentiallyHiddenCachedMods>d__;
			<RefreshPotentiallyHiddenCachedMods>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<RefreshPotentiallyHiddenCachedMods>d__.<>1__state = -1;
			<RefreshPotentiallyHiddenCachedMods>d__.<>t__builder.Start<Mod.<RefreshPotentiallyHiddenCachedMods>d__130>(ref <RefreshPotentiallyHiddenCachedMods>d__);
			return <RefreshPotentiallyHiddenCachedMods>d__.<>t__builder.Task;
		}

		[CompilerGenerated]
		private void <RateMod>g__UpdateStatsWithUserRating|121_0(ModRating userRating)
		{
			this.Stats.UpdateEstimateFromLocalRatingChange(userRating);
			this.CurrentUserRating = userRating;
			this.InvokeModUpdated(ModChangeType.Rating);
		}

		private static readonly Dictionary<ModChangeType, Action<Mod, ModChangeType>> ChangeSubscribers = new Dictionary<ModChangeType, Action<Mod, ModChangeType>>();

		private string _summaryDecoded;

		private string _summaryHtmlEncoded;

		public enum LogoResolution
		{
			X320_Y180,
			X640_Y360,
			X1280_Y720,
			Original
		}

		public enum GalleryResolution
		{
			X320_Y180,
			X1280_Y720,
			Original
		}
	}
}
