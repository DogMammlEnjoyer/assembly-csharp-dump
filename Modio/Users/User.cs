using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio.API;
using Modio.API.SchemaDefinitions;
using Modio.Extensions;
using Modio.FileIO;
using Modio.Mods;

namespace Modio.Users
{
	public class User
	{
		public static event Action<User> OnUserChanged;

		public static event Action OnUserSyncComplete;

		public static User Current { get; private set; }

		public string LocalUserId { get; private set; }

		public long UserId
		{
			get
			{
				return this.Profile.UserId;
			}
		}

		public bool IsInitialized { get; private set; }

		public bool HasAcceptedTermsOfUse { get; private set; }

		public bool IsAuthenticated { get; private set; }

		public bool IsUpdating { get; private set; }

		public UserProfile Profile { get; private set; }

		public Wallet Wallet { get; private set; }

		public ModRepository ModRepository { get; private set; }

		public string Token
		{
			get
			{
				return this._authentication.OAuthToken;
			}
		}

		public static Task InitializeNewUser()
		{
			User.<InitializeNewUser>d__49 <InitializeNewUser>d__;
			<InitializeNewUser>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<InitializeNewUser>d__.<>1__state = -1;
			<InitializeNewUser>d__.<>t__builder.Start<User.<InitializeNewUser>d__49>(ref <InitializeNewUser>d__);
			return <InitializeNewUser>d__.<>t__builder.Task;
		}

		private User()
		{
			this._authentication = new Authentication();
			this.Wallet = new Wallet();
			this.ModRepository = new ModRepository();
			this.Profile = new UserProfile();
			this.IsUpdating = false;
			this.IsAuthenticated = false;
			this.HasAcceptedTermsOfUse = false;
			this.IsInitialized = true;
		}

		private void ApplyDetailsFromSaveObject(UserSaveObject userObject)
		{
			this.Profile.Username = userObject.Username;
			this.Profile.UserId = userObject.UserId;
			if (userObject.SubscribedMods != null)
			{
				foreach (long id in userObject.SubscribedMods)
				{
					Mod.Get(id).UpdateLocalSubscriptionStatus(true);
				}
			}
			if (userObject.DisabledMods != null)
			{
				foreach (long id2 in userObject.DisabledMods)
				{
					Mod.Get(id2).UpdateLocalEnabledStatus(false);
				}
			}
			if (userObject.PurchasedMods != null)
			{
				foreach (long id3 in userObject.PurchasedMods)
				{
					Mod.Get(id3).UpdateLocalPurchaseStatus(true);
				}
			}
			this._authentication.OAuthToken = userObject.AuthToken;
			Action<User> onUserChanged = User.OnUserChanged;
			if (onUserChanged == null)
			{
				return;
			}
			onUserChanged(this);
		}

		private void ApplyDetailsFromLegacySaveObject(LegacyUserSaveObject userSaveObject)
		{
			this.Profile.Username = userSaveObject.userObject.username;
			this.Profile.UserId = userSaveObject.userObject.id;
			this._authentication.OAuthToken = userSaveObject.oAuthToken;
			Action<User> onUserChanged = User.OnUserChanged;
			if (onUserChanged == null)
			{
				return;
			}
			onUserChanged(this);
		}

		internal void OnAcceptedTermsOfUse()
		{
			this.HasAcceptedTermsOfUse = true;
		}

		public void OnAuthenticated(string oAuthToken)
		{
			this._authentication.OAuthToken = oAuthToken;
			this.HasAcceptedTermsOfUse = true;
			this.IsAuthenticated = true;
			this.Sync().ForgetTaskSafely();
		}

		internal string GetAuthToken()
		{
			return this._authentication.OAuthToken;
		}

		public Task<Error> Sync()
		{
			User.<Sync>d__56 <Sync>d__;
			<Sync>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<Sync>d__.<>4__this = this;
			<Sync>d__.<>1__state = -1;
			<Sync>d__.<>t__builder.Start<User.<Sync>d__56>(ref <Sync>d__);
			return <Sync>d__.<>t__builder.Task;
		}

		private void OnAnyModRepositoryChange()
		{
			this.SaveUserData().ForgetTaskSafely();
		}

		public Task<Error> SyncProfile()
		{
			User.<SyncProfile>d__58 <SyncProfile>d__;
			<SyncProfile>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<SyncProfile>d__.<>4__this = this;
			<SyncProfile>d__.<>1__state = -1;
			<SyncProfile>d__.<>t__builder.Start<User.<SyncProfile>d__58>(ref <SyncProfile>d__);
			return <SyncProfile>d__.<>t__builder.Task;
		}

		public Task<Error> SyncSubscriptions()
		{
			User.<SyncSubscriptions>d__59 <SyncSubscriptions>d__;
			<SyncSubscriptions>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<SyncSubscriptions>d__.<>4__this = this;
			<SyncSubscriptions>d__.<>1__state = -1;
			<SyncSubscriptions>d__.<>t__builder.Start<User.<SyncSubscriptions>d__59>(ref <SyncSubscriptions>d__);
			return <SyncSubscriptions>d__.<>t__builder.Task;
		}

		public Task<Error> SyncPurchases()
		{
			User.<SyncPurchases>d__60 <SyncPurchases>d__;
			<SyncPurchases>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<SyncPurchases>d__.<>4__this = this;
			<SyncPurchases>d__.<>1__state = -1;
			<SyncPurchases>d__.<>t__builder.Start<User.<SyncPurchases>d__60>(ref <SyncPurchases>d__);
			return <SyncPurchases>d__.<>t__builder.Task;
		}

		public Task<Error> SyncEntitlements()
		{
			User.<SyncEntitlements>d__61 <SyncEntitlements>d__;
			<SyncEntitlements>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<SyncEntitlements>d__.<>4__this = this;
			<SyncEntitlements>d__.<>1__state = -1;
			<SyncEntitlements>d__.<>t__builder.Start<User.<SyncEntitlements>d__61>(ref <SyncEntitlements>d__);
			return <SyncEntitlements>d__.<>t__builder.Task;
		}

		public Task<Error> SyncWallet()
		{
			User.<SyncWallet>d__62 <SyncWallet>d__;
			<SyncWallet>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<SyncWallet>d__.<>4__this = this;
			<SyncWallet>d__.<>1__state = -1;
			<SyncWallet>d__.<>t__builder.Start<User.<SyncWallet>d__62>(ref <SyncWallet>d__);
			return <SyncWallet>d__.<>t__builder.Task;
		}

		internal void ApplyWalletFromPurchase(PayObject payObject)
		{
			this.Wallet.UpdateBalance(payObject.Balance);
			Action<User> onUserChanged = User.OnUserChanged;
			if (onUserChanged == null)
			{
				return;
			}
			onUserChanged(this);
		}

		public Task<Error> SyncRatings()
		{
			User.<SyncRatings>d__64 <SyncRatings>d__;
			<SyncRatings>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<SyncRatings>d__.<>4__this = this;
			<SyncRatings>d__.<>1__state = -1;
			<SyncRatings>d__.<>t__builder.Start<User.<SyncRatings>d__64>(ref <SyncRatings>d__);
			return <SyncRatings>d__.<>t__builder.Task;
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"results"
		})]
		public Task<ValueTuple<Error, IReadOnlyList<UserProfile>>> GetMutedUsers()
		{
			User.<GetMutedUsers>d__65 <GetMutedUsers>d__;
			<GetMutedUsers>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, IReadOnlyList<UserProfile>>>.Create();
			<GetMutedUsers>d__.<>4__this = this;
			<GetMutedUsers>d__.<>1__state = -1;
			<GetMutedUsers>d__.<>t__builder.Start<User.<GetMutedUsers>d__65>(ref <GetMutedUsers>d__);
			return <GetMutedUsers>d__.<>t__builder.Task;
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"mods"
		})]
		public Task<ValueTuple<Error, IReadOnlyList<Mod>>> GetUserCreations(bool filterForGame = false)
		{
			User.<GetUserCreations>d__66 <GetUserCreations>d__;
			<GetUserCreations>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, IReadOnlyList<Mod>>>.Create();
			<GetUserCreations>d__.<>4__this = this;
			<GetUserCreations>d__.filterForGame = filterForGame;
			<GetUserCreations>d__.<>1__state = -1;
			<GetUserCreations>d__.<>t__builder.Start<User.<GetUserCreations>d__66>(ref <GetUserCreations>d__);
			return <GetUserCreations>d__.<>t__builder.Task;
		}

		[ModioDebugMenu(ShowInBrowserMenu = false, ShowInSettingsMenu = true)]
		public static void DeleteUserData()
		{
			ModioServices.Resolve<IModioDataStorage>().DeleteUserData(User.Current.LocalUserId).ForgetTaskSafely();
			User.LogOut();
		}

		public static void LogOut()
		{
			ModioLog verbose = ModioLog.Verbose;
			if (verbose != null)
			{
				string str = "Logging out ";
				User user = User.Current;
				verbose.Log(str + ((user != null) ? user.LocalUserId : null));
			}
			ModInstallationManagement.NotifyLoggingOut();
			User user2 = User.Current;
			if (user2 != null)
			{
				user2.ModRepository.Dispose();
			}
			User.Current = new User();
			Action<User> onUserChanged = User.OnUserChanged;
			if (onUserChanged == null)
			{
				return;
			}
			onUserChanged(User.Current);
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"results"
		})]
		private static Task<ValueTuple<Error, List<T>>> CrawlAllPages<F, T>(F filter, [TupleElementNames(new string[]
		{
			"error",
			null
		})] Func<F, Task<ValueTuple<Error, Pagination<T[]>?>>> method) where F : SearchFilter<F>
		{
			User.<CrawlAllPages>d__69<F, T> <CrawlAllPages>d__;
			<CrawlAllPages>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, List<T>>>.Create();
			<CrawlAllPages>d__.filter = filter;
			<CrawlAllPages>d__.method = method;
			<CrawlAllPages>d__.<>1__state = -1;
			<CrawlAllPages>d__.<>t__builder.Start<User.<CrawlAllPages>d__69<F, T>>(ref <CrawlAllPages>d__);
			return <CrawlAllPages>d__.<>t__builder.Task;
		}

		private Task SaveUserData()
		{
			User.<SaveUserData>d__70 <SaveUserData>d__;
			<SaveUserData>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SaveUserData>d__.<>4__this = this;
			<SaveUserData>d__.<>1__state = -1;
			<SaveUserData>d__.<>t__builder.Start<User.<SaveUserData>d__70>(ref <SaveUserData>d__);
			return <SaveUserData>d__.<>t__builder.Task;
		}

		private UserSaveObject GetWritable()
		{
			UserSaveObject userSaveObject = new UserSaveObject();
			userSaveObject.LocalUserId = this.LocalUserId;
			userSaveObject.Username = this.Profile.Username;
			userSaveObject.UserId = this.Profile.UserId;
			userSaveObject.AuthToken = this._authentication.OAuthToken;
			userSaveObject.SubscribedMods = (from mod in this.ModRepository.GetSubscribed()
			select mod.Id).ToList<long>();
			userSaveObject.DisabledMods = (from mod in this.ModRepository.GetDisabled()
			select mod.Id).ToList<long>();
			userSaveObject.PurchasedMods = (from mod in this.ModRepository.GetPurchased()
			select mod.Id).ToList<long>();
			return userSaveObject;
		}

		private readonly Authentication _authentication;

		private bool _isWritingToDisk;

		private bool _needsSavingToDisk;
	}
}
