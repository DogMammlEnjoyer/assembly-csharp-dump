using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Modio.API.Interfaces;
using Modio.API.SchemaDefinitions;
using Modio.Authentication;
using Modio.Errors;
using Newtonsoft.Json.Linq;

namespace Modio.API
{
	public static class ModioAPI
	{
		public static event Action<bool> OnOfflineStatusChanged;

		public static bool IsOffline { get; private set; }

		public static ModioAPI.Portal CurrentPortal { get; private set; } = ModioAPI.Portal.None;

		public static string LanguageCodeResponse { get; private set; } = "en";

		public static void Init()
		{
			ModioAPI._modioSettings = ModioServices.Resolve<ModioSettings>();
			ModioAPI._serverURL = (string.IsNullOrWhiteSpace(ModioAPI._modioSettings.ServerURL) ? string.Format("https://g-{0}.modapi.io/v1", ModioAPI._modioSettings.GameId) : ModioAPI._modioSettings.ServerURL);
			ModioLog verbose = ModioLog.Verbose;
			if (verbose != null)
			{
				verbose.Log("Initialized " + Version.GetCurrent());
			}
			ModioLog verbose2 = ModioLog.Verbose;
			if (verbose2 != null)
			{
				verbose2.Log((ModioAPI._modioSettings.ServerURL == null) ? ModioAPI._serverURL : string.Format("{0}; {1}", ModioAPI._modioSettings.GameId, ModioAPI._modioSettings.ServerURL));
			}
			ModioServices.IResolveType<IModioAPIInterface> bindings = ModioServices.GetBindings<IModioAPIInterface>(false);
			ModioAPI.SetAPIInterface(bindings.Resolve());
			bindings.OnNewBinding -= ModioAPI.SetAPIInterface;
			bindings.OnNewBinding += ModioAPI.SetAPIInterface;
			ModioServices.IResolveType<IModioAuthService> bindings2 = ModioServices.GetBindings<IModioAuthService>(false);
			ModioAPI.SetPortalFromAuthService(bindings2.Resolve());
			bindings2.OnNewBinding -= ModioAPI.SetPortalFromAuthService;
			bindings2.OnNewBinding += ModioAPI.SetPortalFromAuthService;
		}

		public static void SetResponseLanguage(string languageCode)
		{
			if (string.IsNullOrWhiteSpace(languageCode))
			{
				ModioLog message = ModioLog.Message;
				if (message != null)
				{
					message.Log("ModioAPI response language is invalid (\"" + languageCode + "\"). Use ModioAPI.SetResponseLanguage to set a valid language code. Defaulting to [en]");
				}
				languageCode = "en";
			}
			ModioAPI.LanguageCodeResponse = languageCode;
			if (ModioAPI._apiInterface == null)
			{
				return;
			}
			ModioAPI._apiInterface.RemoveDefaultHeader("Accept-Language");
			ModioAPI._apiInterface.SetDefaultHeader("Accept-Language", languageCode);
		}

		public static void SetPlatform(ModioAPI.Platform platform)
		{
			if (platform == ModioAPI.Platform.None)
			{
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					platform = ModioAPI.Platform.Windows;
				}
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				{
					platform = ModioAPI.Platform.Mac;
				}
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				{
					platform = ModioAPI.Platform.Linux;
				}
			}
			ModioAPI._platform = platform;
			if (ModioAPI._apiInterface == null)
			{
				return;
			}
			ModioAPI._apiInterface.RemoveDefaultHeader("X-Modio-Platform");
			if (platform.GetHeader() != null)
			{
				ModioAPI._apiInterface.SetDefaultHeader("X-Modio-Platform", platform.GetHeader());
			}
		}

		public static void SetPortal(ModioAPI.Portal portal)
		{
			ModioAPI.CurrentPortal = portal;
			if (ModioAPI._apiInterface == null)
			{
				return;
			}
			ModioAPI._apiInterface.RemoveDefaultHeader("X-Modio-Portal");
			string header = portal.GetHeader();
			if (header != null)
			{
				ModioAPI._apiInterface.SetDefaultHeader("X-Modio-Portal", header);
			}
		}

		private static void SetPortalFromAuthService(IModioAuthService authService)
		{
			ModioAPI.SetPortal((authService != null) ? authService.Portal : ModioAPI.Portal.None);
		}

		public static void SetAPIInterface(IModioAPIInterface apiInterface)
		{
			apiInterface.ResetConfiguration();
			ModioAPI._apiInterface = apiInterface;
			apiInterface.SetDefaultHeader("Accept", "application/json");
			ModioAPI.SetResponseLanguage(ModioAPI.LanguageCodeResponse);
			apiInterface.SetDefaultHeader("User-Agent", Version.GetCurrent());
			ModioAPI.SetPlatform(ModioAPI._platform);
			ModioAPI.SetPortal(ModioAPI.CurrentPortal);
			ModioAPI._apiInterface.AddDefaultParameter("api_key=" + ModioAPI._modioSettings.APIKey);
			ModioAPI._apiInterface.SetBasePath(ModioAPI._serverURL);
			ModioAPI._apiInterface.AddDefaultPathParameter("game-id", string.Format("{0}", ModioAPI._modioSettings.GameId));
			ModioLog verbose = ModioLog.Verbose;
			if (verbose == null)
			{
				return;
			}
			verbose.Log("ModioAPI.SetAPIInterface(" + ModioAPI._apiInterface.GetType().Name + ")");
		}

		public static void SetOfflineStatus(bool isOffline)
		{
			if (ModioAPI.IsOffline == isOffline)
			{
				return;
			}
			ModioAPI.IsOffline = isOffline;
			Action<bool> onOfflineStatusChanged = ModioAPI.OnOfflineStatusChanged;
			if (onOfflineStatusChanged == null)
			{
				return;
			}
			onOfflineStatusChanged(isOffline);
		}

		private static bool IsInitialized()
		{
			if (ModioAPI._modioSettings.GameId != 0L)
			{
				return true;
			}
			ModioLog error = ModioLog.Error;
			if (error != null)
			{
				error.Log(ErrorCode.API_NOT_INITIALIZED.GetMessage(null));
			}
			return false;
		}

		public static Task<bool> Ping()
		{
			ModioAPI.<Ping>d__56 <Ping>d__;
			<Ping>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<Ping>d__.<>1__state = -1;
			<Ping>d__.<>t__builder.Start<ModioAPI.<Ping>d__56>(ref <Ping>d__);
			return <Ping>d__.<>t__builder.Task;
		}

		private static string GetHeader(this ModioAPI.Platform platform)
		{
			string result;
			switch (platform)
			{
			case ModioAPI.Platform.Source:
				result = "source";
				break;
			case ModioAPI.Platform.Windows:
				result = "windows";
				break;
			case ModioAPI.Platform.Mac:
				result = "mac";
				break;
			case ModioAPI.Platform.Linux:
				result = "linux";
				break;
			case ModioAPI.Platform.Android:
				result = "android";
				break;
			case ModioAPI.Platform.IOS:
				result = "ios";
				break;
			case ModioAPI.Platform.XboxOne:
				result = "xboxone";
				break;
			case ModioAPI.Platform.XboxSeriesX:
				result = "xboxseriesx";
				break;
			case ModioAPI.Platform.PlayStation4:
				result = "ps4";
				break;
			case ModioAPI.Platform.PlayStation5:
				result = "ps5";
				break;
			case ModioAPI.Platform.Switch:
				result = "switch";
				break;
			case ModioAPI.Platform.Oculus:
				result = "oculus";
				break;
			default:
				result = null;
				break;
			}
			return result;
		}

		private static string GetHeader(this ModioAPI.Portal portal)
		{
			string result;
			switch (portal)
			{
			case ModioAPI.Portal.Apple:
				result = "apple";
				break;
			case ModioAPI.Portal.Discord:
				result = "discord";
				break;
			case ModioAPI.Portal.EpicGamesStore:
				result = "epicgames";
				break;
			case ModioAPI.Portal.Facebook:
				result = "facebook";
				break;
			case ModioAPI.Portal.GOG:
				result = "gog";
				break;
			case ModioAPI.Portal.Google:
				result = "google";
				break;
			case ModioAPI.Portal.Itchio:
				result = "itchio";
				break;
			case ModioAPI.Portal.Nintendo:
				result = "nintendo";
				break;
			case ModioAPI.Portal.PlayStationNetwork:
				result = "psn";
				break;
			case ModioAPI.Portal.SSO:
				result = "sso";
				break;
			case ModioAPI.Portal.Steam:
				result = "steam";
				break;
			case ModioAPI.Portal.XboxLive:
				result = "xboxlive";
				break;
			default:
				result = null;
				break;
			}
			return result;
		}

		private const string HEADER_LANGUAGE_RESPONSE = "Accept-Language";

		private const string HEADER_PLATFORM = "X-Modio-Platform";

		private const string HEADER_PORTAL = "X-Modio-Portal";

		private static string _serverURL;

		private static ModioSettings _modioSettings;

		private static ModioAPI.Platform _platform = ModioAPI.Platform.None;

		private static IModioAPIInterface _apiInterface;

		public static class Media
		{
			[return: TupleElementNames(new string[]
			{
				"error",
				"updateGameMediaResponse"
			})]
			internal static Task<ValueTuple<Error, JToken>> AddGameMediaAsJToken(AddGameMediaRequest? body = null)
			{
				ModioAPI.Media.<AddGameMediaAsJToken>d__0 <AddGameMediaAsJToken>d__;
				<AddGameMediaAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<AddGameMediaAsJToken>d__.body = body;
				<AddGameMediaAsJToken>d__.<>1__state = -1;
				<AddGameMediaAsJToken>d__.<>t__builder.Start<ModioAPI.Media.<AddGameMediaAsJToken>d__0>(ref <AddGameMediaAsJToken>d__);
				return <AddGameMediaAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"updateGameMediaResponse"
			})]
			internal static Task<ValueTuple<Error, UpdateGameMediaResponse?>> AddGameMedia(AddGameMediaRequest? body = null)
			{
				ModioAPI.Media.<AddGameMedia>d__1 <AddGameMedia>d__;
				<AddGameMedia>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, UpdateGameMediaResponse?>>.Create();
				<AddGameMedia>d__.body = body;
				<AddGameMedia>d__.<>1__state = -1;
				<AddGameMedia>d__.<>t__builder.Start<ModioAPI.Media.<AddGameMedia>d__1>(ref <AddGameMedia>d__);
				return <AddGameMedia>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"updateModMediaResponse"
			})]
			internal static Task<ValueTuple<Error, JToken>> AddModMediaAsJToken(long modId, AddModMediaRequest? body = null)
			{
				ModioAPI.Media.<AddModMediaAsJToken>d__2 <AddModMediaAsJToken>d__;
				<AddModMediaAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<AddModMediaAsJToken>d__.modId = modId;
				<AddModMediaAsJToken>d__.body = body;
				<AddModMediaAsJToken>d__.<>1__state = -1;
				<AddModMediaAsJToken>d__.<>t__builder.Start<ModioAPI.Media.<AddModMediaAsJToken>d__2>(ref <AddModMediaAsJToken>d__);
				return <AddModMediaAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"updateModMediaResponse"
			})]
			internal static Task<ValueTuple<Error, UpdateModMediaResponse?>> AddModMedia(long modId, AddModMediaRequest? body = null)
			{
				ModioAPI.Media.<AddModMedia>d__3 <AddModMedia>d__;
				<AddModMedia>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, UpdateModMediaResponse?>>.Create();
				<AddModMedia>d__.modId = modId;
				<AddModMedia>d__.body = body;
				<AddModMedia>d__.<>1__state = -1;
				<AddModMedia>d__.<>t__builder.Start<ModioAPI.Media.<AddModMedia>d__3>(ref <AddModMedia>d__);
				return <AddModMedia>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, JToken>> DeleteModMediaAsJToken(long modId, DeleteModMediaRequest? body = null)
			{
				ModioAPI.Media.<DeleteModMediaAsJToken>d__4 <DeleteModMediaAsJToken>d__;
				<DeleteModMediaAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<DeleteModMediaAsJToken>d__.modId = modId;
				<DeleteModMediaAsJToken>d__.body = body;
				<DeleteModMediaAsJToken>d__.<>1__state = -1;
				<DeleteModMediaAsJToken>d__.<>t__builder.Start<ModioAPI.Media.<DeleteModMediaAsJToken>d__4>(ref <DeleteModMediaAsJToken>d__);
				return <DeleteModMediaAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, Response204?>> DeleteModMedia(long modId, DeleteModMediaRequest? body = null)
			{
				ModioAPI.Media.<DeleteModMedia>d__5 <DeleteModMedia>d__;
				<DeleteModMedia>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Response204?>>.Create();
				<DeleteModMedia>d__.modId = modId;
				<DeleteModMedia>d__.body = body;
				<DeleteModMedia>d__.<>1__state = -1;
				<DeleteModMedia>d__.<>t__builder.Start<ModioAPI.Media.<DeleteModMedia>d__5>(ref <DeleteModMedia>d__);
				return <DeleteModMedia>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, JToken>> ReorderModMediaAsJToken(long modId, DeleteModMediaRequest? body = null)
			{
				ModioAPI.Media.<ReorderModMediaAsJToken>d__6 <ReorderModMediaAsJToken>d__;
				<ReorderModMediaAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<ReorderModMediaAsJToken>d__.modId = modId;
				<ReorderModMediaAsJToken>d__.body = body;
				<ReorderModMediaAsJToken>d__.<>1__state = -1;
				<ReorderModMediaAsJToken>d__.<>t__builder.Start<ModioAPI.Media.<ReorderModMediaAsJToken>d__6>(ref <ReorderModMediaAsJToken>d__);
				return <ReorderModMediaAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, Response204?>> ReorderModMedia(long modId, DeleteModMediaRequest? body = null)
			{
				ModioAPI.Media.<ReorderModMedia>d__7 <ReorderModMedia>d__;
				<ReorderModMedia>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Response204?>>.Create();
				<ReorderModMedia>d__.modId = modId;
				<ReorderModMedia>d__.body = body;
				<ReorderModMedia>d__.<>1__state = -1;
				<ReorderModMedia>d__.<>t__builder.Start<ModioAPI.Media.<ReorderModMedia>d__7>(ref <ReorderModMedia>d__);
				return <ReorderModMedia>d__.<>t__builder.Task;
			}
		}

		public static class Mods
		{
			[return: TupleElementNames(new string[]
			{
				"error",
				"modObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> AddModAsJToken(AddModRequest? body = null)
			{
				ModioAPI.Mods.<AddModAsJToken>d__0 <AddModAsJToken>d__;
				<AddModAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<AddModAsJToken>d__.body = body;
				<AddModAsJToken>d__.<>1__state = -1;
				<AddModAsJToken>d__.<>t__builder.Start<ModioAPI.Mods.<AddModAsJToken>d__0>(ref <AddModAsJToken>d__);
				return <AddModAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modObject"
			})]
			internal static Task<ValueTuple<Error, ModObject?>> AddMod(AddModRequest? body = null)
			{
				ModioAPI.Mods.<AddMod>d__1 <AddMod>d__;
				<AddMod>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, ModObject?>>.Create();
				<AddMod>d__.body = body;
				<AddMod>d__.<>1__state = -1;
				<AddMod>d__.<>t__builder.Start<ModioAPI.Mods.<AddMod>d__1>(ref <AddMod>d__);
				return <AddMod>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, JToken>> DeleteModAsJToken(long modId)
			{
				ModioAPI.Mods.<DeleteModAsJToken>d__2 <DeleteModAsJToken>d__;
				<DeleteModAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<DeleteModAsJToken>d__.modId = modId;
				<DeleteModAsJToken>d__.<>1__state = -1;
				<DeleteModAsJToken>d__.<>t__builder.Start<ModioAPI.Mods.<DeleteModAsJToken>d__2>(ref <DeleteModAsJToken>d__);
				return <DeleteModAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, Response204?>> DeleteMod(long modId)
			{
				ModioAPI.Mods.<DeleteMod>d__3 <DeleteMod>d__;
				<DeleteMod>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Response204?>>.Create();
				<DeleteMod>d__.modId = modId;
				<DeleteMod>d__.<>1__state = -1;
				<DeleteMod>d__.<>t__builder.Start<ModioAPI.Mods.<DeleteMod>d__3>(ref <DeleteMod>d__);
				return <DeleteMod>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> EditModAsJToken(long modId, EditModRequest? body = null)
			{
				ModioAPI.Mods.<EditModAsJToken>d__4 <EditModAsJToken>d__;
				<EditModAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<EditModAsJToken>d__.modId = modId;
				<EditModAsJToken>d__.body = body;
				<EditModAsJToken>d__.<>1__state = -1;
				<EditModAsJToken>d__.<>t__builder.Start<ModioAPI.Mods.<EditModAsJToken>d__4>(ref <EditModAsJToken>d__);
				return <EditModAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modObject"
			})]
			internal static Task<ValueTuple<Error, ModObject?>> EditMod(long modId, EditModRequest? body = null)
			{
				ModioAPI.Mods.<EditMod>d__5 <EditMod>d__;
				<EditMod>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, ModObject?>>.Create();
				<EditMod>d__.modId = modId;
				<EditMod>d__.body = body;
				<EditMod>d__.<>1__state = -1;
				<EditMod>d__.<>t__builder.Start<ModioAPI.Mods.<EditMod>d__5>(ref <EditMod>d__);
				return <EditMod>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetModAsJToken(long modId)
			{
				ModioAPI.Mods.<GetModAsJToken>d__6 <GetModAsJToken>d__;
				<GetModAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetModAsJToken>d__.modId = modId;
				<GetModAsJToken>d__.<>1__state = -1;
				<GetModAsJToken>d__.<>t__builder.Start<ModioAPI.Mods.<GetModAsJToken>d__6>(ref <GetModAsJToken>d__);
				return <GetModAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modObject"
			})]
			internal static Task<ValueTuple<Error, ModObject?>> GetMod(long modId)
			{
				ModioAPI.Mods.<GetMod>d__7 <GetMod>d__;
				<GetMod>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, ModObject?>>.Create();
				<GetMod>d__.modId = modId;
				<GetMod>d__.<>1__state = -1;
				<GetMod>d__.<>t__builder.Start<ModioAPI.Mods.<GetMod>d__7>(ref <GetMod>d__);
				return <GetMod>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modObjects"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetModsAsJToken(ModioAPI.Mods.GetModsFilter filter)
			{
				ModioAPI.Mods.<GetModsAsJToken>d__8 <GetModsAsJToken>d__;
				<GetModsAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetModsAsJToken>d__.filter = filter;
				<GetModsAsJToken>d__.<>1__state = -1;
				<GetModsAsJToken>d__.<>t__builder.Start<ModioAPI.Mods.<GetModsAsJToken>d__8>(ref <GetModsAsJToken>d__);
				return <GetModsAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modObjects"
			})]
			internal static Task<ValueTuple<Error, Pagination<ModObject[]>?>> GetMods(ModioAPI.Mods.GetModsFilter filter)
			{
				ModioAPI.Mods.<GetMods>d__9 <GetMods>d__;
				<GetMods>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<ModObject[]>?>>.Create();
				<GetMods>d__.filter = filter;
				<GetMods>d__.<>1__state = -1;
				<GetMods>d__.<>t__builder.Start<ModioAPI.Mods.<GetMods>d__9>(ref <GetMods>d__);
				return <GetMods>d__.<>t__builder.Task;
			}

			public static ModioAPI.Mods.GetModsFilter FilterGetMods(int pageIndex = 0, int pageSize = 100)
			{
				return new ModioAPI.Mods.GetModsFilter(pageIndex, pageSize);
			}

			public class GetModsFilter : SearchFilter<ModioAPI.Mods.GetModsFilter>
			{
				internal GetModsFilter(int pageIndex, int pageSize) : base(pageIndex, pageSize)
				{
				}

				public ModioAPI.Mods.GetModsFilter Id(long id, Filtering condition = Filtering.None)
				{
					this.Parameters["id" + condition.ClearText()] = id;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter Id(ICollection<long> id, Filtering condition = Filtering.None)
				{
					this.Parameters["id" + condition.ClearText()] = id;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter GameId(long gameId, Filtering condition = Filtering.None)
				{
					this.Parameters["game_id" + condition.ClearText()] = gameId;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter GameId(ICollection<long> gameId, Filtering condition = Filtering.None)
				{
					this.Parameters["game_id" + condition.ClearText()] = gameId;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter Status(long status, Filtering condition = Filtering.None)
				{
					this.Parameters["status" + condition.ClearText()] = status;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter Status(ICollection<long> status, Filtering condition = Filtering.None)
				{
					this.Parameters["status" + condition.ClearText()] = status;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter Visible(long visible, Filtering condition = Filtering.None)
				{
					this.Parameters["visible" + condition.ClearText()] = visible;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter Visible(ICollection<long> visible, Filtering condition = Filtering.None)
				{
					this.Parameters["visible" + condition.ClearText()] = visible;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter SubmittedBy(long submittedBy, Filtering condition = Filtering.None)
				{
					this.Parameters["submitted_by" + condition.ClearText()] = submittedBy;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter SubmittedBy(ICollection<long> submittedBy, Filtering condition = Filtering.None)
				{
					this.Parameters["submitted_by" + condition.ClearText()] = submittedBy;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter SubmittedByDisplayName(string submittedByDisplayName, Filtering condition = Filtering.None)
				{
					this.Parameters["submitted_by_display_name" + condition.ClearText()] = submittedByDisplayName;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter SubmittedByDisplayName(ICollection<string> submittedByDisplayName, Filtering condition = Filtering.None)
				{
					this.Parameters["submitted_by_display_name" + condition.ClearText()] = submittedByDisplayName;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter DateAdded(long dateAdded, Filtering condition = Filtering.None)
				{
					this.Parameters["date_added" + condition.ClearText()] = dateAdded;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter DateAdded(ICollection<long> dateAdded, Filtering condition = Filtering.None)
				{
					this.Parameters["date_added" + condition.ClearText()] = dateAdded;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter DateUpdated(long dateUpdated, Filtering condition = Filtering.None)
				{
					this.Parameters["date_updated" + condition.ClearText()] = dateUpdated;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter DateUpdated(ICollection<long> dateUpdated, Filtering condition = Filtering.None)
				{
					this.Parameters["date_updated" + condition.ClearText()] = dateUpdated;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter DateLive(long dateLive, Filtering condition = Filtering.None)
				{
					this.Parameters["date_live" + condition.ClearText()] = dateLive;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter DateLive(ICollection<long> dateLive, Filtering condition = Filtering.None)
				{
					this.Parameters["date_live" + condition.ClearText()] = dateLive;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter CommunityOptions(long communityOptions, Filtering condition = Filtering.None)
				{
					this.Parameters["community_options" + condition.ClearText()] = communityOptions;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter CommunityOptions(ICollection<long> communityOptions, Filtering condition = Filtering.None)
				{
					this.Parameters["community_options" + condition.ClearText()] = communityOptions;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter MaturityOption(long maturityOption, Filtering condition = Filtering.None)
				{
					this.Parameters["maturity_option" + condition.ClearText()] = maturityOption;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter MaturityOption(ICollection<long> maturityOption, Filtering condition = Filtering.None)
				{
					this.Parameters["maturity_option" + condition.ClearText()] = maturityOption;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter MonetizationOptions(long monetizationOptions, Filtering condition = Filtering.None)
				{
					this.Parameters["monetization_options" + condition.ClearText()] = monetizationOptions;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter MonetizationOptions(ICollection<long> monetizationOptions, Filtering condition = Filtering.None)
				{
					this.Parameters["monetization_options" + condition.ClearText()] = monetizationOptions;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter Name(string name, Filtering condition = Filtering.None)
				{
					this.Parameters["name" + condition.ClearText()] = name;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter Name(ICollection<string> name, Filtering condition = Filtering.None)
				{
					this.Parameters["name" + condition.ClearText()] = name;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter NameId(string nameId, Filtering condition = Filtering.None)
				{
					this.Parameters["name_id" + condition.ClearText()] = nameId;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter NameId(ICollection<string> nameId, Filtering condition = Filtering.None)
				{
					this.Parameters["name_id" + condition.ClearText()] = nameId;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter Modfile(long modfile, Filtering condition = Filtering.None)
				{
					this.Parameters["modfile" + condition.ClearText()] = modfile;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter Modfile(ICollection<long> modfile, Filtering condition = Filtering.None)
				{
					this.Parameters["modfile" + condition.ClearText()] = modfile;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter MetadataBlob(string metadataBlob, Filtering condition = Filtering.None)
				{
					this.Parameters["metadata_blob" + condition.ClearText()] = metadataBlob;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter MetadataBlob(ICollection<string> metadataBlob, Filtering condition = Filtering.None)
				{
					this.Parameters["metadata_blob" + condition.ClearText()] = metadataBlob;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter MetadataKvp(string metadataKvp, Filtering condition = Filtering.None)
				{
					this.Parameters["metadata_kvp" + condition.ClearText()] = metadataKvp;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter MetadataKvp(ICollection<string> metadataKvp, Filtering condition = Filtering.None)
				{
					this.Parameters["metadata_kvp" + condition.ClearText()] = metadataKvp;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter Tags(string tags, Filtering condition = Filtering.None)
				{
					this.Parameters["tags" + condition.ClearText()] = tags;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter Tags(ICollection<string> tags, Filtering condition = Filtering.None)
				{
					this.Parameters["tags" + condition.ClearText()] = tags;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter PlatformStatus(string platformStatus, Filtering condition = Filtering.None)
				{
					this.Parameters["platform_status" + condition.ClearText()] = platformStatus;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter PlatformStatus(ICollection<string> platformStatus, Filtering condition = Filtering.None)
				{
					this.Parameters["platform_status" + condition.ClearText()] = platformStatus;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter RevenueType(long revenueType, Filtering condition = Filtering.None)
				{
					this.Parameters["revenue_type" + condition.ClearText()] = revenueType;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter RevenueType(ICollection<long> revenueType, Filtering condition = Filtering.None)
				{
					this.Parameters["revenue_type" + condition.ClearText()] = revenueType;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter Stock(long stock, Filtering condition = Filtering.None)
				{
					this.Parameters["stock" + condition.ClearText()] = stock;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter Stock(ICollection<long> stock, Filtering condition = Filtering.None)
				{
					this.Parameters["stock" + condition.ClearText()] = stock;
					return this;
				}

				public ModioAPI.Mods.GetModsFilter SortByStringType(string key, bool ascending = true)
				{
					this.Parameters["_sort"] = (ascending ? "" : "-") + key;
					return this;
				}
			}
		}

		public static class Comments
		{
			[return: TupleElementNames(new string[]
			{
				"error",
				"commentObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> AddModCommentAsJToken(long modId, AddCommentRequest? body = null)
			{
				ModioAPI.Comments.<AddModCommentAsJToken>d__0 <AddModCommentAsJToken>d__;
				<AddModCommentAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<AddModCommentAsJToken>d__.modId = modId;
				<AddModCommentAsJToken>d__.body = body;
				<AddModCommentAsJToken>d__.<>1__state = -1;
				<AddModCommentAsJToken>d__.<>t__builder.Start<ModioAPI.Comments.<AddModCommentAsJToken>d__0>(ref <AddModCommentAsJToken>d__);
				return <AddModCommentAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"commentObject"
			})]
			internal static Task<ValueTuple<Error, CommentObject?>> AddModComment(long modId, AddCommentRequest? body = null)
			{
				ModioAPI.Comments.<AddModComment>d__1 <AddModComment>d__;
				<AddModComment>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, CommentObject?>>.Create();
				<AddModComment>d__.modId = modId;
				<AddModComment>d__.body = body;
				<AddModComment>d__.<>1__state = -1;
				<AddModComment>d__.<>t__builder.Start<ModioAPI.Comments.<AddModComment>d__1>(ref <AddModComment>d__);
				return <AddModComment>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"commentObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> AddModCommentKarmaAsJToken(long modId, long commentId, UpdateCommentKarmaRequest? body = null)
			{
				ModioAPI.Comments.<AddModCommentKarmaAsJToken>d__2 <AddModCommentKarmaAsJToken>d__;
				<AddModCommentKarmaAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<AddModCommentKarmaAsJToken>d__.modId = modId;
				<AddModCommentKarmaAsJToken>d__.commentId = commentId;
				<AddModCommentKarmaAsJToken>d__.body = body;
				<AddModCommentKarmaAsJToken>d__.<>1__state = -1;
				<AddModCommentKarmaAsJToken>d__.<>t__builder.Start<ModioAPI.Comments.<AddModCommentKarmaAsJToken>d__2>(ref <AddModCommentKarmaAsJToken>d__);
				return <AddModCommentKarmaAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"commentObject"
			})]
			internal static Task<ValueTuple<Error, CommentObject?>> AddModCommentKarma(long modId, long commentId, UpdateCommentKarmaRequest? body = null)
			{
				ModioAPI.Comments.<AddModCommentKarma>d__3 <AddModCommentKarma>d__;
				<AddModCommentKarma>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, CommentObject?>>.Create();
				<AddModCommentKarma>d__.modId = modId;
				<AddModCommentKarma>d__.commentId = commentId;
				<AddModCommentKarma>d__.body = body;
				<AddModCommentKarma>d__.<>1__state = -1;
				<AddModCommentKarma>d__.<>t__builder.Start<ModioAPI.Comments.<AddModCommentKarma>d__3>(ref <AddModCommentKarma>d__);
				return <AddModCommentKarma>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, JToken>> DeleteModCommentAsJToken(long modId, long commentId)
			{
				ModioAPI.Comments.<DeleteModCommentAsJToken>d__4 <DeleteModCommentAsJToken>d__;
				<DeleteModCommentAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<DeleteModCommentAsJToken>d__.modId = modId;
				<DeleteModCommentAsJToken>d__.commentId = commentId;
				<DeleteModCommentAsJToken>d__.<>1__state = -1;
				<DeleteModCommentAsJToken>d__.<>t__builder.Start<ModioAPI.Comments.<DeleteModCommentAsJToken>d__4>(ref <DeleteModCommentAsJToken>d__);
				return <DeleteModCommentAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, Response204?>> DeleteModComment(long modId, long commentId)
			{
				ModioAPI.Comments.<DeleteModComment>d__5 <DeleteModComment>d__;
				<DeleteModComment>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Response204?>>.Create();
				<DeleteModComment>d__.modId = modId;
				<DeleteModComment>d__.commentId = commentId;
				<DeleteModComment>d__.<>1__state = -1;
				<DeleteModComment>d__.<>t__builder.Start<ModioAPI.Comments.<DeleteModComment>d__5>(ref <DeleteModComment>d__);
				return <DeleteModComment>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"commentObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetModCommentAsJToken(long modId, long commentId)
			{
				ModioAPI.Comments.<GetModCommentAsJToken>d__6 <GetModCommentAsJToken>d__;
				<GetModCommentAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetModCommentAsJToken>d__.modId = modId;
				<GetModCommentAsJToken>d__.commentId = commentId;
				<GetModCommentAsJToken>d__.<>1__state = -1;
				<GetModCommentAsJToken>d__.<>t__builder.Start<ModioAPI.Comments.<GetModCommentAsJToken>d__6>(ref <GetModCommentAsJToken>d__);
				return <GetModCommentAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"commentObject"
			})]
			internal static Task<ValueTuple<Error, CommentObject?>> GetModComment(long modId, long commentId)
			{
				ModioAPI.Comments.<GetModComment>d__7 <GetModComment>d__;
				<GetModComment>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, CommentObject?>>.Create();
				<GetModComment>d__.modId = modId;
				<GetModComment>d__.commentId = commentId;
				<GetModComment>d__.<>1__state = -1;
				<GetModComment>d__.<>t__builder.Start<ModioAPI.Comments.<GetModComment>d__7>(ref <GetModComment>d__);
				return <GetModComment>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"commentObjects"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetModCommentsAsJToken(long modId, ModioAPI.Comments.GetModCommentsFilter filter)
			{
				ModioAPI.Comments.<GetModCommentsAsJToken>d__8 <GetModCommentsAsJToken>d__;
				<GetModCommentsAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetModCommentsAsJToken>d__.modId = modId;
				<GetModCommentsAsJToken>d__.<>1__state = -1;
				<GetModCommentsAsJToken>d__.<>t__builder.Start<ModioAPI.Comments.<GetModCommentsAsJToken>d__8>(ref <GetModCommentsAsJToken>d__);
				return <GetModCommentsAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"commentObjects"
			})]
			internal static Task<ValueTuple<Error, Pagination<CommentObject[]>?>> GetModComments(long modId, ModioAPI.Comments.GetModCommentsFilter filter)
			{
				ModioAPI.Comments.<GetModComments>d__9 <GetModComments>d__;
				<GetModComments>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<CommentObject[]>?>>.Create();
				<GetModComments>d__.modId = modId;
				<GetModComments>d__.filter = filter;
				<GetModComments>d__.<>1__state = -1;
				<GetModComments>d__.<>t__builder.Start<ModioAPI.Comments.<GetModComments>d__9>(ref <GetModComments>d__);
				return <GetModComments>d__.<>t__builder.Task;
			}

			public static ModioAPI.Comments.GetModCommentsFilter FilterGetModComments(int pageIndex = 0, int pageSize = 100)
			{
				return new ModioAPI.Comments.GetModCommentsFilter(pageIndex, pageSize);
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"commentObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> UpdateModCommentAsJToken(long modId, long commentId, UpdateCommentRequest? body = null)
			{
				ModioAPI.Comments.<UpdateModCommentAsJToken>d__12 <UpdateModCommentAsJToken>d__;
				<UpdateModCommentAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<UpdateModCommentAsJToken>d__.modId = modId;
				<UpdateModCommentAsJToken>d__.commentId = commentId;
				<UpdateModCommentAsJToken>d__.body = body;
				<UpdateModCommentAsJToken>d__.<>1__state = -1;
				<UpdateModCommentAsJToken>d__.<>t__builder.Start<ModioAPI.Comments.<UpdateModCommentAsJToken>d__12>(ref <UpdateModCommentAsJToken>d__);
				return <UpdateModCommentAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"commentObject"
			})]
			internal static Task<ValueTuple<Error, CommentObject?>> UpdateModComment(long modId, long commentId, UpdateCommentRequest? body = null)
			{
				ModioAPI.Comments.<UpdateModComment>d__13 <UpdateModComment>d__;
				<UpdateModComment>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, CommentObject?>>.Create();
				<UpdateModComment>d__.modId = modId;
				<UpdateModComment>d__.commentId = commentId;
				<UpdateModComment>d__.body = body;
				<UpdateModComment>d__.<>1__state = -1;
				<UpdateModComment>d__.<>t__builder.Start<ModioAPI.Comments.<UpdateModComment>d__13>(ref <UpdateModComment>d__);
				return <UpdateModComment>d__.<>t__builder.Task;
			}

			public class GetModCommentsFilter : SearchFilter<ModioAPI.Comments.GetModCommentsFilter>
			{
				internal GetModCommentsFilter(int pageIndex, int pageSize) : base(pageIndex, pageSize)
				{
				}

				public ModioAPI.Comments.GetModCommentsFilter Id(long id, Filtering condition = Filtering.None)
				{
					this.Parameters["id" + condition.ClearText()] = id;
					return this;
				}

				public ModioAPI.Comments.GetModCommentsFilter Id(ICollection<long> id, Filtering condition = Filtering.None)
				{
					this.Parameters["id" + condition.ClearText()] = id;
					return this;
				}

				public ModioAPI.Comments.GetModCommentsFilter ModId(long modId, Filtering condition = Filtering.None)
				{
					this.Parameters["mod_id" + condition.ClearText()] = modId;
					return this;
				}

				public ModioAPI.Comments.GetModCommentsFilter ModId(ICollection<long> modId, Filtering condition = Filtering.None)
				{
					this.Parameters["mod_id" + condition.ClearText()] = modId;
					return this;
				}

				public ModioAPI.Comments.GetModCommentsFilter ResourceId(long resourceId, Filtering condition = Filtering.None)
				{
					this.Parameters["resource_id" + condition.ClearText()] = resourceId;
					return this;
				}

				public ModioAPI.Comments.GetModCommentsFilter ResourceId(ICollection<long> resourceId, Filtering condition = Filtering.None)
				{
					this.Parameters["resource_id" + condition.ClearText()] = resourceId;
					return this;
				}

				public ModioAPI.Comments.GetModCommentsFilter SubmittedBy(long submittedBy, Filtering condition = Filtering.None)
				{
					this.Parameters["submitted_by" + condition.ClearText()] = submittedBy;
					return this;
				}

				public ModioAPI.Comments.GetModCommentsFilter SubmittedBy(ICollection<long> submittedBy, Filtering condition = Filtering.None)
				{
					this.Parameters["submitted_by" + condition.ClearText()] = submittedBy;
					return this;
				}

				public ModioAPI.Comments.GetModCommentsFilter DateAdded(long dateAdded, Filtering condition = Filtering.None)
				{
					this.Parameters["date_added" + condition.ClearText()] = dateAdded;
					return this;
				}

				public ModioAPI.Comments.GetModCommentsFilter DateAdded(ICollection<long> dateAdded, Filtering condition = Filtering.None)
				{
					this.Parameters["date_added" + condition.ClearText()] = dateAdded;
					return this;
				}

				public ModioAPI.Comments.GetModCommentsFilter ReplyId(long replyId, Filtering condition = Filtering.None)
				{
					this.Parameters["reply_id" + condition.ClearText()] = replyId;
					return this;
				}

				public ModioAPI.Comments.GetModCommentsFilter ReplyId(ICollection<long> replyId, Filtering condition = Filtering.None)
				{
					this.Parameters["reply_id" + condition.ClearText()] = replyId;
					return this;
				}

				public ModioAPI.Comments.GetModCommentsFilter ThreadPosition(string threadPosition, Filtering condition = Filtering.None)
				{
					this.Parameters["thread_position" + condition.ClearText()] = threadPosition;
					return this;
				}

				public ModioAPI.Comments.GetModCommentsFilter ThreadPosition(ICollection<string> threadPosition, Filtering condition = Filtering.None)
				{
					this.Parameters["thread_position" + condition.ClearText()] = threadPosition;
					return this;
				}

				public ModioAPI.Comments.GetModCommentsFilter Karma(long karma, Filtering condition = Filtering.None)
				{
					this.Parameters["karma" + condition.ClearText()] = karma;
					return this;
				}

				public ModioAPI.Comments.GetModCommentsFilter Karma(ICollection<long> karma, Filtering condition = Filtering.None)
				{
					this.Parameters["karma" + condition.ClearText()] = karma;
					return this;
				}

				public ModioAPI.Comments.GetModCommentsFilter Content(string content, Filtering condition = Filtering.None)
				{
					this.Parameters["content" + condition.ClearText()] = content;
					return this;
				}

				public ModioAPI.Comments.GetModCommentsFilter Content(ICollection<string> content, Filtering condition = Filtering.None)
				{
					this.Parameters["content" + condition.ClearText()] = content;
					return this;
				}
			}
		}

		public static class Dependencies
		{
			[return: TupleElementNames(new string[]
			{
				"error",
				"addModDependenciesResponse"
			})]
			internal static Task<ValueTuple<Error, JToken>> AddModDependenciesAsJToken(long modId, AddModDependenciesRequest? body = null)
			{
				ModioAPI.Dependencies.<AddModDependenciesAsJToken>d__0 <AddModDependenciesAsJToken>d__;
				<AddModDependenciesAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<AddModDependenciesAsJToken>d__.modId = modId;
				<AddModDependenciesAsJToken>d__.body = body;
				<AddModDependenciesAsJToken>d__.<>1__state = -1;
				<AddModDependenciesAsJToken>d__.<>t__builder.Start<ModioAPI.Dependencies.<AddModDependenciesAsJToken>d__0>(ref <AddModDependenciesAsJToken>d__);
				return <AddModDependenciesAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"addModDependenciesResponse"
			})]
			internal static Task<ValueTuple<Error, AddModDependenciesResponse?>> AddModDependencies(long modId, AddModDependenciesRequest? body = null)
			{
				ModioAPI.Dependencies.<AddModDependencies>d__1 <AddModDependencies>d__;
				<AddModDependencies>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, AddModDependenciesResponse?>>.Create();
				<AddModDependencies>d__.modId = modId;
				<AddModDependencies>d__.body = body;
				<AddModDependencies>d__.<>1__state = -1;
				<AddModDependencies>d__.<>t__builder.Start<ModioAPI.Dependencies.<AddModDependencies>d__1>(ref <AddModDependencies>d__);
				return <AddModDependencies>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, JToken>> DeleteModDependenciesAsJToken(long modId, DeleteModDependenciesRequest? body = null)
			{
				ModioAPI.Dependencies.<DeleteModDependenciesAsJToken>d__2 <DeleteModDependenciesAsJToken>d__;
				<DeleteModDependenciesAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<DeleteModDependenciesAsJToken>d__.modId = modId;
				<DeleteModDependenciesAsJToken>d__.body = body;
				<DeleteModDependenciesAsJToken>d__.<>1__state = -1;
				<DeleteModDependenciesAsJToken>d__.<>t__builder.Start<ModioAPI.Dependencies.<DeleteModDependenciesAsJToken>d__2>(ref <DeleteModDependenciesAsJToken>d__);
				return <DeleteModDependenciesAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, Response204?>> DeleteModDependencies(long modId, DeleteModDependenciesRequest? body = null)
			{
				ModioAPI.Dependencies.<DeleteModDependencies>d__3 <DeleteModDependencies>d__;
				<DeleteModDependencies>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Response204?>>.Create();
				<DeleteModDependencies>d__.modId = modId;
				<DeleteModDependencies>d__.body = body;
				<DeleteModDependencies>d__.<>1__state = -1;
				<DeleteModDependencies>d__.<>t__builder.Start<ModioAPI.Dependencies.<DeleteModDependencies>d__3>(ref <DeleteModDependencies>d__);
				return <DeleteModDependencies>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modDependantsObjects"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetModDependantsAsJToken(long modId, ModioAPI.Dependencies.GetModDependantsFilter filter)
			{
				ModioAPI.Dependencies.<GetModDependantsAsJToken>d__4 <GetModDependantsAsJToken>d__;
				<GetModDependantsAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetModDependantsAsJToken>d__.modId = modId;
				<GetModDependantsAsJToken>d__.<>1__state = -1;
				<GetModDependantsAsJToken>d__.<>t__builder.Start<ModioAPI.Dependencies.<GetModDependantsAsJToken>d__4>(ref <GetModDependantsAsJToken>d__);
				return <GetModDependantsAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modDependantsObjects"
			})]
			internal static Task<ValueTuple<Error, Pagination<ModDependantsObject[]>?>> GetModDependants(long modId, ModioAPI.Dependencies.GetModDependantsFilter filter)
			{
				ModioAPI.Dependencies.<GetModDependants>d__5 <GetModDependants>d__;
				<GetModDependants>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<ModDependantsObject[]>?>>.Create();
				<GetModDependants>d__.modId = modId;
				<GetModDependants>d__.filter = filter;
				<GetModDependants>d__.<>1__state = -1;
				<GetModDependants>d__.<>t__builder.Start<ModioAPI.Dependencies.<GetModDependants>d__5>(ref <GetModDependants>d__);
				return <GetModDependants>d__.<>t__builder.Task;
			}

			public static ModioAPI.Dependencies.GetModDependantsFilter FilterGetModDependants(int pageIndex = 0, int pageSize = 100)
			{
				return new ModioAPI.Dependencies.GetModDependantsFilter(pageIndex, pageSize);
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modDependenciesObjects"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetModDependenciesAsJToken(long modId, ModioAPI.Dependencies.GetModDependenciesFilter filter)
			{
				ModioAPI.Dependencies.<GetModDependenciesAsJToken>d__8 <GetModDependenciesAsJToken>d__;
				<GetModDependenciesAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetModDependenciesAsJToken>d__.modId = modId;
				<GetModDependenciesAsJToken>d__.<>1__state = -1;
				<GetModDependenciesAsJToken>d__.<>t__builder.Start<ModioAPI.Dependencies.<GetModDependenciesAsJToken>d__8>(ref <GetModDependenciesAsJToken>d__);
				return <GetModDependenciesAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modDependenciesObjects"
			})]
			internal static Task<ValueTuple<Error, Pagination<ModDependenciesObject[]>?>> GetModDependencies(long modId, ModioAPI.Dependencies.GetModDependenciesFilter filter)
			{
				ModioAPI.Dependencies.<GetModDependencies>d__9 <GetModDependencies>d__;
				<GetModDependencies>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<ModDependenciesObject[]>?>>.Create();
				<GetModDependencies>d__.modId = modId;
				<GetModDependencies>d__.filter = filter;
				<GetModDependencies>d__.<>1__state = -1;
				<GetModDependencies>d__.<>t__builder.Start<ModioAPI.Dependencies.<GetModDependencies>d__9>(ref <GetModDependencies>d__);
				return <GetModDependencies>d__.<>t__builder.Task;
			}

			public static ModioAPI.Dependencies.GetModDependenciesFilter FilterGetModDependencies(int pageIndex = 0, int pageSize = 100)
			{
				return new ModioAPI.Dependencies.GetModDependenciesFilter(pageIndex, pageSize);
			}

			public class GetModDependantsFilter : SearchFilter<ModioAPI.Dependencies.GetModDependantsFilter>
			{
				internal GetModDependantsFilter(int pageIndex, int pageSize) : base(pageIndex, pageSize)
				{
				}
			}

			public class GetModDependenciesFilter : SearchFilter<ModioAPI.Dependencies.GetModDependenciesFilter>
			{
				internal GetModDependenciesFilter(int pageIndex, int pageSize) : base(pageIndex, pageSize)
				{
				}

				public ModioAPI.Dependencies.GetModDependenciesFilter Recursive(bool recursive, Filtering condition = Filtering.None)
				{
					this.Parameters["recursive" + condition.ClearText()] = recursive;
					return this;
				}

				public ModioAPI.Dependencies.GetModDependenciesFilter Recursive(ICollection<bool> recursive, Filtering condition = Filtering.None)
				{
					this.Parameters["recursive" + condition.ClearText()] = recursive;
					return this;
				}
			}
		}

		public static class Files
		{
			[return: TupleElementNames(new string[]
			{
				"error",
				"modfileObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> AddModfileAsJToken(long modId, AddModfileRequest? body = null)
			{
				ModioAPI.Files.<AddModfileAsJToken>d__0 <AddModfileAsJToken>d__;
				<AddModfileAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<AddModfileAsJToken>d__.modId = modId;
				<AddModfileAsJToken>d__.body = body;
				<AddModfileAsJToken>d__.<>1__state = -1;
				<AddModfileAsJToken>d__.<>t__builder.Start<ModioAPI.Files.<AddModfileAsJToken>d__0>(ref <AddModfileAsJToken>d__);
				return <AddModfileAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modfileObject"
			})]
			internal static Task<ValueTuple<Error, ModfileObject?>> AddModfile(long modId, AddModfileRequest? body = null)
			{
				ModioAPI.Files.<AddModfile>d__1 <AddModfile>d__;
				<AddModfile>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, ModfileObject?>>.Create();
				<AddModfile>d__.modId = modId;
				<AddModfile>d__.body = body;
				<AddModfile>d__.<>1__state = -1;
				<AddModfile>d__.<>t__builder.Start<ModioAPI.Files.<AddModfile>d__1>(ref <AddModfile>d__);
				return <AddModfile>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, JToken>> DeleteModfileAsJToken(long modId, long fileId)
			{
				ModioAPI.Files.<DeleteModfileAsJToken>d__2 <DeleteModfileAsJToken>d__;
				<DeleteModfileAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<DeleteModfileAsJToken>d__.modId = modId;
				<DeleteModfileAsJToken>d__.fileId = fileId;
				<DeleteModfileAsJToken>d__.<>1__state = -1;
				<DeleteModfileAsJToken>d__.<>t__builder.Start<ModioAPI.Files.<DeleteModfileAsJToken>d__2>(ref <DeleteModfileAsJToken>d__);
				return <DeleteModfileAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, Response204?>> DeleteModfile(long modId, long fileId)
			{
				ModioAPI.Files.<DeleteModfile>d__3 <DeleteModfile>d__;
				<DeleteModfile>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Response204?>>.Create();
				<DeleteModfile>d__.modId = modId;
				<DeleteModfile>d__.fileId = fileId;
				<DeleteModfile>d__.<>1__state = -1;
				<DeleteModfile>d__.<>t__builder.Start<ModioAPI.Files.<DeleteModfile>d__3>(ref <DeleteModfile>d__);
				return <DeleteModfile>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modfileObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> EditModfileAsJToken(long modId, long fileId)
			{
				ModioAPI.Files.<EditModfileAsJToken>d__4 <EditModfileAsJToken>d__;
				<EditModfileAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<EditModfileAsJToken>d__.modId = modId;
				<EditModfileAsJToken>d__.fileId = fileId;
				<EditModfileAsJToken>d__.<>1__state = -1;
				<EditModfileAsJToken>d__.<>t__builder.Start<ModioAPI.Files.<EditModfileAsJToken>d__4>(ref <EditModfileAsJToken>d__);
				return <EditModfileAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modfileObject"
			})]
			internal static Task<ValueTuple<Error, ModfileObject?>> EditModfile(long modId, long fileId)
			{
				ModioAPI.Files.<EditModfile>d__5 <EditModfile>d__;
				<EditModfile>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, ModfileObject?>>.Create();
				<EditModfile>d__.modId = modId;
				<EditModfile>d__.fileId = fileId;
				<EditModfile>d__.<>1__state = -1;
				<EditModfile>d__.<>t__builder.Start<ModioAPI.Files.<EditModfile>d__5>(ref <EditModfile>d__);
				return <EditModfile>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modfileObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetModfileAsJToken(long modId, long fileId)
			{
				ModioAPI.Files.<GetModfileAsJToken>d__6 <GetModfileAsJToken>d__;
				<GetModfileAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetModfileAsJToken>d__.modId = modId;
				<GetModfileAsJToken>d__.fileId = fileId;
				<GetModfileAsJToken>d__.<>1__state = -1;
				<GetModfileAsJToken>d__.<>t__builder.Start<ModioAPI.Files.<GetModfileAsJToken>d__6>(ref <GetModfileAsJToken>d__);
				return <GetModfileAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modfileObject"
			})]
			internal static Task<ValueTuple<Error, ModfileObject?>> GetModfile(long modId, long fileId)
			{
				ModioAPI.Files.<GetModfile>d__7 <GetModfile>d__;
				<GetModfile>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, ModfileObject?>>.Create();
				<GetModfile>d__.modId = modId;
				<GetModfile>d__.fileId = fileId;
				<GetModfile>d__.<>1__state = -1;
				<GetModfile>d__.<>t__builder.Start<ModioAPI.Files.<GetModfile>d__7>(ref <GetModfile>d__);
				return <GetModfile>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modfileObjects"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetModfilesAsJToken(long modId, ModioAPI.Files.GetModfilesFilter filter)
			{
				ModioAPI.Files.<GetModfilesAsJToken>d__8 <GetModfilesAsJToken>d__;
				<GetModfilesAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetModfilesAsJToken>d__.modId = modId;
				<GetModfilesAsJToken>d__.<>1__state = -1;
				<GetModfilesAsJToken>d__.<>t__builder.Start<ModioAPI.Files.<GetModfilesAsJToken>d__8>(ref <GetModfilesAsJToken>d__);
				return <GetModfilesAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modfileObjects"
			})]
			internal static Task<ValueTuple<Error, Pagination<ModfileObject[]>?>> GetModfiles(long modId, ModioAPI.Files.GetModfilesFilter filter)
			{
				ModioAPI.Files.<GetModfiles>d__9 <GetModfiles>d__;
				<GetModfiles>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<ModfileObject[]>?>>.Create();
				<GetModfiles>d__.modId = modId;
				<GetModfiles>d__.filter = filter;
				<GetModfiles>d__.<>1__state = -1;
				<GetModfiles>d__.<>t__builder.Start<ModioAPI.Files.<GetModfiles>d__9>(ref <GetModfiles>d__);
				return <GetModfiles>d__.<>t__builder.Task;
			}

			public static ModioAPI.Files.GetModfilesFilter FilterGetModfiles(int pageIndex = 0, int pageSize = 100)
			{
				return new ModioAPI.Files.GetModfilesFilter(pageIndex, pageSize);
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modfileObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> ManagePlatformStatusAsJToken(long modId, long fileId)
			{
				ModioAPI.Files.<ManagePlatformStatusAsJToken>d__12 <ManagePlatformStatusAsJToken>d__;
				<ManagePlatformStatusAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<ManagePlatformStatusAsJToken>d__.modId = modId;
				<ManagePlatformStatusAsJToken>d__.fileId = fileId;
				<ManagePlatformStatusAsJToken>d__.<>1__state = -1;
				<ManagePlatformStatusAsJToken>d__.<>t__builder.Start<ModioAPI.Files.<ManagePlatformStatusAsJToken>d__12>(ref <ManagePlatformStatusAsJToken>d__);
				return <ManagePlatformStatusAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modfileObject"
			})]
			internal static Task<ValueTuple<Error, ModfileObject?>> ManagePlatformStatus(long modId, long fileId)
			{
				ModioAPI.Files.<ManagePlatformStatus>d__13 <ManagePlatformStatus>d__;
				<ManagePlatformStatus>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, ModfileObject?>>.Create();
				<ManagePlatformStatus>d__.modId = modId;
				<ManagePlatformStatus>d__.fileId = fileId;
				<ManagePlatformStatus>d__.<>1__state = -1;
				<ManagePlatformStatus>d__.<>t__builder.Start<ModioAPI.Files.<ManagePlatformStatus>d__13>(ref <ManagePlatformStatus>d__);
				return <ManagePlatformStatus>d__.<>t__builder.Task;
			}

			public class GetModfilesFilter : SearchFilter<ModioAPI.Files.GetModfilesFilter>
			{
				internal GetModfilesFilter(int pageIndex, int pageSize) : base(pageIndex, pageSize)
				{
				}

				public ModioAPI.Files.GetModfilesFilter Id(long id, Filtering condition = Filtering.None)
				{
					this.Parameters["id" + condition.ClearText()] = id;
					return this;
				}

				public ModioAPI.Files.GetModfilesFilter Id(ICollection<long> id, Filtering condition = Filtering.None)
				{
					this.Parameters["id" + condition.ClearText()] = id;
					return this;
				}

				public ModioAPI.Files.GetModfilesFilter ModId(long modId, Filtering condition = Filtering.None)
				{
					this.Parameters["mod_id" + condition.ClearText()] = modId;
					return this;
				}

				public ModioAPI.Files.GetModfilesFilter ModId(ICollection<long> modId, Filtering condition = Filtering.None)
				{
					this.Parameters["mod_id" + condition.ClearText()] = modId;
					return this;
				}

				public ModioAPI.Files.GetModfilesFilter DateAdded(long dateAdded, Filtering condition = Filtering.None)
				{
					this.Parameters["date_added" + condition.ClearText()] = dateAdded;
					return this;
				}

				public ModioAPI.Files.GetModfilesFilter DateAdded(ICollection<long> dateAdded, Filtering condition = Filtering.None)
				{
					this.Parameters["date_added" + condition.ClearText()] = dateAdded;
					return this;
				}

				public ModioAPI.Files.GetModfilesFilter DateScanned(long dateScanned, Filtering condition = Filtering.None)
				{
					this.Parameters["date_scanned" + condition.ClearText()] = dateScanned;
					return this;
				}

				public ModioAPI.Files.GetModfilesFilter DateScanned(ICollection<long> dateScanned, Filtering condition = Filtering.None)
				{
					this.Parameters["date_scanned" + condition.ClearText()] = dateScanned;
					return this;
				}

				public ModioAPI.Files.GetModfilesFilter VirusStatus(long virusStatus, Filtering condition = Filtering.None)
				{
					this.Parameters["virus_status" + condition.ClearText()] = virusStatus;
					return this;
				}

				public ModioAPI.Files.GetModfilesFilter VirusStatus(ICollection<long> virusStatus, Filtering condition = Filtering.None)
				{
					this.Parameters["virus_status" + condition.ClearText()] = virusStatus;
					return this;
				}

				public ModioAPI.Files.GetModfilesFilter VirusPositive(long virusPositive, Filtering condition = Filtering.None)
				{
					this.Parameters["virus_positive" + condition.ClearText()] = virusPositive;
					return this;
				}

				public ModioAPI.Files.GetModfilesFilter VirusPositive(ICollection<long> virusPositive, Filtering condition = Filtering.None)
				{
					this.Parameters["virus_positive" + condition.ClearText()] = virusPositive;
					return this;
				}

				public ModioAPI.Files.GetModfilesFilter Filesize(long filesize, Filtering condition = Filtering.None)
				{
					this.Parameters["filesize" + condition.ClearText()] = filesize;
					return this;
				}

				public ModioAPI.Files.GetModfilesFilter Filesize(ICollection<long> filesize, Filtering condition = Filtering.None)
				{
					this.Parameters["filesize" + condition.ClearText()] = filesize;
					return this;
				}

				public ModioAPI.Files.GetModfilesFilter Filehash(string filehash, Filtering condition = Filtering.None)
				{
					this.Parameters["filehash" + condition.ClearText()] = filehash;
					return this;
				}

				public ModioAPI.Files.GetModfilesFilter Filehash(ICollection<string> filehash, Filtering condition = Filtering.None)
				{
					this.Parameters["filehash" + condition.ClearText()] = filehash;
					return this;
				}

				public ModioAPI.Files.GetModfilesFilter Filename(string filename, Filtering condition = Filtering.None)
				{
					this.Parameters["filename" + condition.ClearText()] = filename;
					return this;
				}

				public ModioAPI.Files.GetModfilesFilter Filename(ICollection<string> filename, Filtering condition = Filtering.None)
				{
					this.Parameters["filename" + condition.ClearText()] = filename;
					return this;
				}

				public ModioAPI.Files.GetModfilesFilter Version(string version, Filtering condition = Filtering.None)
				{
					this.Parameters["version" + condition.ClearText()] = version;
					return this;
				}

				public ModioAPI.Files.GetModfilesFilter Version(ICollection<string> version, Filtering condition = Filtering.None)
				{
					this.Parameters["version" + condition.ClearText()] = version;
					return this;
				}

				public ModioAPI.Files.GetModfilesFilter Changelog(string changelog, Filtering condition = Filtering.None)
				{
					this.Parameters["changelog" + condition.ClearText()] = changelog;
					return this;
				}

				public ModioAPI.Files.GetModfilesFilter Changelog(ICollection<string> changelog, Filtering condition = Filtering.None)
				{
					this.Parameters["changelog" + condition.ClearText()] = changelog;
					return this;
				}

				public ModioAPI.Files.GetModfilesFilter MetadataBlob(string metadataBlob, Filtering condition = Filtering.None)
				{
					this.Parameters["metadata_blob" + condition.ClearText()] = metadataBlob;
					return this;
				}

				public ModioAPI.Files.GetModfilesFilter MetadataBlob(ICollection<string> metadataBlob, Filtering condition = Filtering.None)
				{
					this.Parameters["metadata_blob" + condition.ClearText()] = metadataBlob;
					return this;
				}

				public ModioAPI.Files.GetModfilesFilter PlatformStatus(string platformStatus, Filtering condition = Filtering.None)
				{
					this.Parameters["platform_status" + condition.ClearText()] = platformStatus;
					return this;
				}

				public ModioAPI.Files.GetModfilesFilter PlatformStatus(ICollection<string> platformStatus, Filtering condition = Filtering.None)
				{
					this.Parameters["platform_status" + condition.ClearText()] = platformStatus;
					return this;
				}
			}
		}

		public static class Metadata
		{
			[return: TupleElementNames(new string[]
			{
				"error",
				"addModMetadataResponse"
			})]
			internal static Task<ValueTuple<Error, JToken>> AddModKvpMetadataAsJToken(long modId, AddModMetadataRequest? body = null)
			{
				ModioAPI.Metadata.<AddModKvpMetadataAsJToken>d__0 <AddModKvpMetadataAsJToken>d__;
				<AddModKvpMetadataAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<AddModKvpMetadataAsJToken>d__.modId = modId;
				<AddModKvpMetadataAsJToken>d__.body = body;
				<AddModKvpMetadataAsJToken>d__.<>1__state = -1;
				<AddModKvpMetadataAsJToken>d__.<>t__builder.Start<ModioAPI.Metadata.<AddModKvpMetadataAsJToken>d__0>(ref <AddModKvpMetadataAsJToken>d__);
				return <AddModKvpMetadataAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"addModMetadataResponse"
			})]
			internal static Task<ValueTuple<Error, AddModMetadataResponse?>> AddModKvpMetadata(long modId, AddModMetadataRequest? body = null)
			{
				ModioAPI.Metadata.<AddModKvpMetadata>d__1 <AddModKvpMetadata>d__;
				<AddModKvpMetadata>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, AddModMetadataResponse?>>.Create();
				<AddModKvpMetadata>d__.modId = modId;
				<AddModKvpMetadata>d__.body = body;
				<AddModKvpMetadata>d__.<>1__state = -1;
				<AddModKvpMetadata>d__.<>t__builder.Start<ModioAPI.Metadata.<AddModKvpMetadata>d__1>(ref <AddModKvpMetadata>d__);
				return <AddModKvpMetadata>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, JToken>> DeleteModKvpMetadataAsJToken(long modId, DeleteModMetadataRequest? body = null)
			{
				ModioAPI.Metadata.<DeleteModKvpMetadataAsJToken>d__2 <DeleteModKvpMetadataAsJToken>d__;
				<DeleteModKvpMetadataAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<DeleteModKvpMetadataAsJToken>d__.modId = modId;
				<DeleteModKvpMetadataAsJToken>d__.body = body;
				<DeleteModKvpMetadataAsJToken>d__.<>1__state = -1;
				<DeleteModKvpMetadataAsJToken>d__.<>t__builder.Start<ModioAPI.Metadata.<DeleteModKvpMetadataAsJToken>d__2>(ref <DeleteModKvpMetadataAsJToken>d__);
				return <DeleteModKvpMetadataAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, Response204?>> DeleteModKvpMetadata(long modId, DeleteModMetadataRequest? body = null)
			{
				ModioAPI.Metadata.<DeleteModKvpMetadata>d__3 <DeleteModKvpMetadata>d__;
				<DeleteModKvpMetadata>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Response204?>>.Create();
				<DeleteModKvpMetadata>d__.modId = modId;
				<DeleteModKvpMetadata>d__.body = body;
				<DeleteModKvpMetadata>d__.<>1__state = -1;
				<DeleteModKvpMetadata>d__.<>t__builder.Start<ModioAPI.Metadata.<DeleteModKvpMetadata>d__3>(ref <DeleteModKvpMetadata>d__);
				return <DeleteModKvpMetadata>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"metadataKvpObjects"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetModKvpMetadataAsJToken(long modId, ModioAPI.Metadata.GetModKvpMetadataFilter filter)
			{
				ModioAPI.Metadata.<GetModKvpMetadataAsJToken>d__4 <GetModKvpMetadataAsJToken>d__;
				<GetModKvpMetadataAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetModKvpMetadataAsJToken>d__.modId = modId;
				<GetModKvpMetadataAsJToken>d__.<>1__state = -1;
				<GetModKvpMetadataAsJToken>d__.<>t__builder.Start<ModioAPI.Metadata.<GetModKvpMetadataAsJToken>d__4>(ref <GetModKvpMetadataAsJToken>d__);
				return <GetModKvpMetadataAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"metadataKvpObjects"
			})]
			internal static Task<ValueTuple<Error, Pagination<MetadataKvpObject[]>?>> GetModKvpMetadata(long modId, ModioAPI.Metadata.GetModKvpMetadataFilter filter = null)
			{
				ModioAPI.Metadata.<GetModKvpMetadata>d__5 <GetModKvpMetadata>d__;
				<GetModKvpMetadata>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<MetadataKvpObject[]>?>>.Create();
				<GetModKvpMetadata>d__.modId = modId;
				<GetModKvpMetadata>d__.filter = filter;
				<GetModKvpMetadata>d__.<>1__state = -1;
				<GetModKvpMetadata>d__.<>t__builder.Start<ModioAPI.Metadata.<GetModKvpMetadata>d__5>(ref <GetModKvpMetadata>d__);
				return <GetModKvpMetadata>d__.<>t__builder.Task;
			}

			public static ModioAPI.Metadata.GetModKvpMetadataFilter FilterGetModKvpMetadata(int pageIndex = 0, int pageSize = 100)
			{
				return new ModioAPI.Metadata.GetModKvpMetadataFilter(pageIndex, pageSize);
			}

			public class GetModKvpMetadataFilter : SearchFilter<ModioAPI.Metadata.GetModKvpMetadataFilter>
			{
				internal GetModKvpMetadataFilter(int pageIndex, int pageSize) : base(pageIndex, pageSize)
				{
				}
			}
		}

		public static class Ratings
		{
			[return: TupleElementNames(new string[]
			{
				"error",
				"addRatingResponse"
			})]
			internal static Task<ValueTuple<Error, JToken>> AddModRatingAsJToken(long modId, AddRatingRequest? body = null)
			{
				ModioAPI.Ratings.<AddModRatingAsJToken>d__0 <AddModRatingAsJToken>d__;
				<AddModRatingAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<AddModRatingAsJToken>d__.modId = modId;
				<AddModRatingAsJToken>d__.body = body;
				<AddModRatingAsJToken>d__.<>1__state = -1;
				<AddModRatingAsJToken>d__.<>t__builder.Start<ModioAPI.Ratings.<AddModRatingAsJToken>d__0>(ref <AddModRatingAsJToken>d__);
				return <AddModRatingAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"addRatingResponse"
			})]
			internal static Task<ValueTuple<Error, AddRatingResponse?>> AddModRating(long modId, AddRatingRequest? body = null)
			{
				ModioAPI.Ratings.<AddModRating>d__1 <AddModRating>d__;
				<AddModRating>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, AddRatingResponse?>>.Create();
				<AddModRating>d__.modId = modId;
				<AddModRating>d__.body = body;
				<AddModRating>d__.<>1__state = -1;
				<AddModRating>d__.<>t__builder.Start<ModioAPI.Ratings.<AddModRating>d__1>(ref <AddModRating>d__);
				return <AddModRating>d__.<>t__builder.Task;
			}
		}

		public static class Tags
		{
			[return: TupleElementNames(new string[]
			{
				"error",
				"messageObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> AddModTagsAsJToken(long modId, AddModTagsRequest? body = null)
			{
				ModioAPI.Tags.<AddModTagsAsJToken>d__0 <AddModTagsAsJToken>d__;
				<AddModTagsAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<AddModTagsAsJToken>d__.modId = modId;
				<AddModTagsAsJToken>d__.body = body;
				<AddModTagsAsJToken>d__.<>1__state = -1;
				<AddModTagsAsJToken>d__.<>t__builder.Start<ModioAPI.Tags.<AddModTagsAsJToken>d__0>(ref <AddModTagsAsJToken>d__);
				return <AddModTagsAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"messageObject"
			})]
			internal static Task<ValueTuple<Error, MessageObject?>> AddModTags(long modId, AddModTagsRequest? body = null)
			{
				ModioAPI.Tags.<AddModTags>d__1 <AddModTags>d__;
				<AddModTags>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, MessageObject?>>.Create();
				<AddModTags>d__.modId = modId;
				<AddModTags>d__.body = body;
				<AddModTags>d__.<>1__state = -1;
				<AddModTags>d__.<>t__builder.Start<ModioAPI.Tags.<AddModTags>d__1>(ref <AddModTags>d__);
				return <AddModTags>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, JToken>> DeleteModTagsAsJToken(long modId, DeleteModTagsRequest? body = null)
			{
				ModioAPI.Tags.<DeleteModTagsAsJToken>d__2 <DeleteModTagsAsJToken>d__;
				<DeleteModTagsAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<DeleteModTagsAsJToken>d__.modId = modId;
				<DeleteModTagsAsJToken>d__.body = body;
				<DeleteModTagsAsJToken>d__.<>1__state = -1;
				<DeleteModTagsAsJToken>d__.<>t__builder.Start<ModioAPI.Tags.<DeleteModTagsAsJToken>d__2>(ref <DeleteModTagsAsJToken>d__);
				return <DeleteModTagsAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, Response204?>> DeleteModTags(long modId, DeleteModTagsRequest? body = null)
			{
				ModioAPI.Tags.<DeleteModTags>d__3 <DeleteModTags>d__;
				<DeleteModTags>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Response204?>>.Create();
				<DeleteModTags>d__.modId = modId;
				<DeleteModTags>d__.body = body;
				<DeleteModTags>d__.<>1__state = -1;
				<DeleteModTags>d__.<>t__builder.Start<ModioAPI.Tags.<DeleteModTags>d__3>(ref <DeleteModTags>d__);
				return <DeleteModTags>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"gameTagOptionObjects"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetGameTagOptionsAsJToken()
			{
				ModioAPI.Tags.<GetGameTagOptionsAsJToken>d__4 <GetGameTagOptionsAsJToken>d__;
				<GetGameTagOptionsAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetGameTagOptionsAsJToken>d__.<>1__state = -1;
				<GetGameTagOptionsAsJToken>d__.<>t__builder.Start<ModioAPI.Tags.<GetGameTagOptionsAsJToken>d__4>(ref <GetGameTagOptionsAsJToken>d__);
				return <GetGameTagOptionsAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"gameTagOptionObjects"
			})]
			internal static Task<ValueTuple<Error, Pagination<GameTagOptionObject[]>?>> GetGameTagOptions()
			{
				ModioAPI.Tags.<GetGameTagOptions>d__5 <GetGameTagOptions>d__;
				<GetGameTagOptions>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<GameTagOptionObject[]>?>>.Create();
				<GetGameTagOptions>d__.<>1__state = -1;
				<GetGameTagOptions>d__.<>t__builder.Start<ModioAPI.Tags.<GetGameTagOptions>d__5>(ref <GetGameTagOptions>d__);
				return <GetGameTagOptions>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modTagObjects"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetModTagsAsJToken(long modId, ModioAPI.Tags.GetModTagsFilter filter)
			{
				ModioAPI.Tags.<GetModTagsAsJToken>d__6 <GetModTagsAsJToken>d__;
				<GetModTagsAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetModTagsAsJToken>d__.modId = modId;
				<GetModTagsAsJToken>d__.<>1__state = -1;
				<GetModTagsAsJToken>d__.<>t__builder.Start<ModioAPI.Tags.<GetModTagsAsJToken>d__6>(ref <GetModTagsAsJToken>d__);
				return <GetModTagsAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modTagObjects"
			})]
			internal static Task<ValueTuple<Error, Pagination<ModTagObject[]>?>> GetModTags(long modId, ModioAPI.Tags.GetModTagsFilter filter)
			{
				ModioAPI.Tags.<GetModTags>d__7 <GetModTags>d__;
				<GetModTags>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<ModTagObject[]>?>>.Create();
				<GetModTags>d__.modId = modId;
				<GetModTags>d__.filter = filter;
				<GetModTags>d__.<>1__state = -1;
				<GetModTags>d__.<>t__builder.Start<ModioAPI.Tags.<GetModTags>d__7>(ref <GetModTags>d__);
				return <GetModTags>d__.<>t__builder.Task;
			}

			public static ModioAPI.Tags.GetModTagsFilter FilterGetModTags(int pageIndex = 0, int pageSize = 100)
			{
				return new ModioAPI.Tags.GetModTagsFilter(pageIndex, pageSize);
			}

			public class GetModTagsFilter : SearchFilter<ModioAPI.Tags.GetModTagsFilter>
			{
				internal GetModTagsFilter(int pageIndex, int pageSize) : base(pageIndex, pageSize)
				{
				}

				public ModioAPI.Tags.GetModTagsFilter DateAdded(long dateAdded, Filtering condition = Filtering.None)
				{
					this.Parameters["date_added" + condition.ClearText()] = dateAdded;
					return this;
				}

				public ModioAPI.Tags.GetModTagsFilter DateAdded(ICollection<long> dateAdded, Filtering condition = Filtering.None)
				{
					this.Parameters["date_added" + condition.ClearText()] = dateAdded;
					return this;
				}

				public ModioAPI.Tags.GetModTagsFilter Tag(string tag, Filtering condition = Filtering.None)
				{
					this.Parameters["tag" + condition.ClearText()] = tag;
					return this;
				}

				public ModioAPI.Tags.GetModTagsFilter Tag(ICollection<string> tag, Filtering condition = Filtering.None)
				{
					this.Parameters["tag" + condition.ClearText()] = tag;
					return this;
				}
			}
		}

		public static class Teams
		{
			[return: TupleElementNames(new string[]
			{
				"error",
				"teamMemberObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> AddModTeamMemberAsJToken(long modId, AddTeamMemberRequest? body = null)
			{
				ModioAPI.Teams.<AddModTeamMemberAsJToken>d__0 <AddModTeamMemberAsJToken>d__;
				<AddModTeamMemberAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<AddModTeamMemberAsJToken>d__.modId = modId;
				<AddModTeamMemberAsJToken>d__.body = body;
				<AddModTeamMemberAsJToken>d__.<>1__state = -1;
				<AddModTeamMemberAsJToken>d__.<>t__builder.Start<ModioAPI.Teams.<AddModTeamMemberAsJToken>d__0>(ref <AddModTeamMemberAsJToken>d__);
				return <AddModTeamMemberAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"teamMemberObject"
			})]
			internal static Task<ValueTuple<Error, TeamMemberObject?>> AddModTeamMember(long modId, AddTeamMemberRequest? body = null)
			{
				ModioAPI.Teams.<AddModTeamMember>d__1 <AddModTeamMember>d__;
				<AddModTeamMember>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, TeamMemberObject?>>.Create();
				<AddModTeamMember>d__.modId = modId;
				<AddModTeamMember>d__.body = body;
				<AddModTeamMember>d__.<>1__state = -1;
				<AddModTeamMember>d__.<>t__builder.Start<ModioAPI.Teams.<AddModTeamMember>d__1>(ref <AddModTeamMember>d__);
				return <AddModTeamMember>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, JToken>> DeleteModTeamMemberAsJToken(long modId, long teamMemberId)
			{
				ModioAPI.Teams.<DeleteModTeamMemberAsJToken>d__2 <DeleteModTeamMemberAsJToken>d__;
				<DeleteModTeamMemberAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<DeleteModTeamMemberAsJToken>d__.modId = modId;
				<DeleteModTeamMemberAsJToken>d__.teamMemberId = teamMemberId;
				<DeleteModTeamMemberAsJToken>d__.<>1__state = -1;
				<DeleteModTeamMemberAsJToken>d__.<>t__builder.Start<ModioAPI.Teams.<DeleteModTeamMemberAsJToken>d__2>(ref <DeleteModTeamMemberAsJToken>d__);
				return <DeleteModTeamMemberAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, Response204?>> DeleteModTeamMember(long modId, long teamMemberId)
			{
				ModioAPI.Teams.<DeleteModTeamMember>d__3 <DeleteModTeamMember>d__;
				<DeleteModTeamMember>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Response204?>>.Create();
				<DeleteModTeamMember>d__.modId = modId;
				<DeleteModTeamMember>d__.teamMemberId = teamMemberId;
				<DeleteModTeamMember>d__.<>1__state = -1;
				<DeleteModTeamMember>d__.<>t__builder.Start<ModioAPI.Teams.<DeleteModTeamMember>d__3>(ref <DeleteModTeamMember>d__);
				return <DeleteModTeamMember>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"teamMemberObjects"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetModTeamMembersAsJToken(long modId, ModioAPI.Teams.GetModTeamMembersFilter filter)
			{
				ModioAPI.Teams.<GetModTeamMembersAsJToken>d__4 <GetModTeamMembersAsJToken>d__;
				<GetModTeamMembersAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetModTeamMembersAsJToken>d__.modId = modId;
				<GetModTeamMembersAsJToken>d__.<>1__state = -1;
				<GetModTeamMembersAsJToken>d__.<>t__builder.Start<ModioAPI.Teams.<GetModTeamMembersAsJToken>d__4>(ref <GetModTeamMembersAsJToken>d__);
				return <GetModTeamMembersAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"teamMemberObjects"
			})]
			internal static Task<ValueTuple<Error, Pagination<TeamMemberObject[]>?>> GetModTeamMembers(long modId, ModioAPI.Teams.GetModTeamMembersFilter filter)
			{
				ModioAPI.Teams.<GetModTeamMembers>d__5 <GetModTeamMembers>d__;
				<GetModTeamMembers>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<TeamMemberObject[]>?>>.Create();
				<GetModTeamMembers>d__.modId = modId;
				<GetModTeamMembers>d__.filter = filter;
				<GetModTeamMembers>d__.<>1__state = -1;
				<GetModTeamMembers>d__.<>t__builder.Start<ModioAPI.Teams.<GetModTeamMembers>d__5>(ref <GetModTeamMembers>d__);
				return <GetModTeamMembers>d__.<>t__builder.Task;
			}

			public static ModioAPI.Teams.GetModTeamMembersFilter FilterGetModTeamMembers(int pageIndex = 0, int pageSize = 100)
			{
				return new ModioAPI.Teams.GetModTeamMembersFilter(pageIndex, pageSize);
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"teamMemberObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> UpdateModTeamMemberAsJToken(long modId, long teamMemberId)
			{
				ModioAPI.Teams.<UpdateModTeamMemberAsJToken>d__8 <UpdateModTeamMemberAsJToken>d__;
				<UpdateModTeamMemberAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<UpdateModTeamMemberAsJToken>d__.modId = modId;
				<UpdateModTeamMemberAsJToken>d__.teamMemberId = teamMemberId;
				<UpdateModTeamMemberAsJToken>d__.<>1__state = -1;
				<UpdateModTeamMemberAsJToken>d__.<>t__builder.Start<ModioAPI.Teams.<UpdateModTeamMemberAsJToken>d__8>(ref <UpdateModTeamMemberAsJToken>d__);
				return <UpdateModTeamMemberAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"teamMemberObject"
			})]
			internal static Task<ValueTuple<Error, TeamMemberObject?>> UpdateModTeamMember(long modId, long teamMemberId)
			{
				ModioAPI.Teams.<UpdateModTeamMember>d__9 <UpdateModTeamMember>d__;
				<UpdateModTeamMember>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, TeamMemberObject?>>.Create();
				<UpdateModTeamMember>d__.modId = modId;
				<UpdateModTeamMember>d__.teamMemberId = teamMemberId;
				<UpdateModTeamMember>d__.<>1__state = -1;
				<UpdateModTeamMember>d__.<>t__builder.Start<ModioAPI.Teams.<UpdateModTeamMember>d__9>(ref <UpdateModTeamMember>d__);
				return <UpdateModTeamMember>d__.<>t__builder.Task;
			}

			public class GetModTeamMembersFilter : SearchFilter<ModioAPI.Teams.GetModTeamMembersFilter>
			{
				internal GetModTeamMembersFilter(int pageIndex, int pageSize) : base(pageIndex, pageSize)
				{
				}

				public ModioAPI.Teams.GetModTeamMembersFilter Id(long id, Filtering condition = Filtering.None)
				{
					this.Parameters["id" + condition.ClearText()] = id;
					return this;
				}

				public ModioAPI.Teams.GetModTeamMembersFilter Id(ICollection<long> id, Filtering condition = Filtering.None)
				{
					this.Parameters["id" + condition.ClearText()] = id;
					return this;
				}

				public ModioAPI.Teams.GetModTeamMembersFilter UserId(long userId, Filtering condition = Filtering.None)
				{
					this.Parameters["user_id" + condition.ClearText()] = userId;
					return this;
				}

				public ModioAPI.Teams.GetModTeamMembersFilter UserId(ICollection<long> userId, Filtering condition = Filtering.None)
				{
					this.Parameters["user_id" + condition.ClearText()] = userId;
					return this;
				}

				public ModioAPI.Teams.GetModTeamMembersFilter Username(string username, Filtering condition = Filtering.None)
				{
					this.Parameters["username" + condition.ClearText()] = username;
					return this;
				}

				public ModioAPI.Teams.GetModTeamMembersFilter Username(ICollection<string> username, Filtering condition = Filtering.None)
				{
					this.Parameters["username" + condition.ClearText()] = username;
					return this;
				}

				public ModioAPI.Teams.GetModTeamMembersFilter Level(long level, Filtering condition = Filtering.None)
				{
					this.Parameters["level" + condition.ClearText()] = level;
					return this;
				}

				public ModioAPI.Teams.GetModTeamMembersFilter Level(ICollection<long> level, Filtering condition = Filtering.None)
				{
					this.Parameters["level" + condition.ClearText()] = level;
					return this;
				}

				public ModioAPI.Teams.GetModTeamMembersFilter DateAdded(long dateAdded, Filtering condition = Filtering.None)
				{
					this.Parameters["date_added" + condition.ClearText()] = dateAdded;
					return this;
				}

				public ModioAPI.Teams.GetModTeamMembersFilter DateAdded(ICollection<long> dateAdded, Filtering condition = Filtering.None)
				{
					this.Parameters["date_added" + condition.ClearText()] = dateAdded;
					return this;
				}

				public ModioAPI.Teams.GetModTeamMembersFilter Pending(long pending, Filtering condition = Filtering.None)
				{
					this.Parameters["pending" + condition.ClearText()] = pending;
					return this;
				}

				public ModioAPI.Teams.GetModTeamMembersFilter Pending(ICollection<long> pending, Filtering condition = Filtering.None)
				{
					this.Parameters["pending" + condition.ClearText()] = pending;
					return this;
				}
			}
		}

		public static class FilesMultipartUploads
		{
			[return: TupleElementNames(new string[]
			{
				"error",
				"multipartUploadPartObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> AddMultipartUploadPartAsJToken(string uploadId, long modId, string contentRange, byte[] bytes, string digest = null)
			{
				ModioAPI.FilesMultipartUploads.<AddMultipartUploadPartAsJToken>d__0 <AddMultipartUploadPartAsJToken>d__;
				<AddMultipartUploadPartAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<AddMultipartUploadPartAsJToken>d__.uploadId = uploadId;
				<AddMultipartUploadPartAsJToken>d__.modId = modId;
				<AddMultipartUploadPartAsJToken>d__.contentRange = contentRange;
				<AddMultipartUploadPartAsJToken>d__.bytes = bytes;
				<AddMultipartUploadPartAsJToken>d__.digest = digest;
				<AddMultipartUploadPartAsJToken>d__.<>1__state = -1;
				<AddMultipartUploadPartAsJToken>d__.<>t__builder.Start<ModioAPI.FilesMultipartUploads.<AddMultipartUploadPartAsJToken>d__0>(ref <AddMultipartUploadPartAsJToken>d__);
				return <AddMultipartUploadPartAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"multipartUploadPartObject"
			})]
			internal static Task<ValueTuple<Error, MultipartUploadPartObject?>> AddMultipartUploadPart(string uploadId, long modId, string contentRange, byte[] bytes, string digest = null)
			{
				ModioAPI.FilesMultipartUploads.<AddMultipartUploadPart>d__1 <AddMultipartUploadPart>d__;
				<AddMultipartUploadPart>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, MultipartUploadPartObject?>>.Create();
				<AddMultipartUploadPart>d__.uploadId = uploadId;
				<AddMultipartUploadPart>d__.modId = modId;
				<AddMultipartUploadPart>d__.contentRange = contentRange;
				<AddMultipartUploadPart>d__.bytes = bytes;
				<AddMultipartUploadPart>d__.digest = digest;
				<AddMultipartUploadPart>d__.<>1__state = -1;
				<AddMultipartUploadPart>d__.<>t__builder.Start<ModioAPI.FilesMultipartUploads.<AddMultipartUploadPart>d__1>(ref <AddMultipartUploadPart>d__);
				return <AddMultipartUploadPart>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"multipartUploadObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> CompleteMultipartUploadSessionAsJToken(string uploadId, long modId)
			{
				ModioAPI.FilesMultipartUploads.<CompleteMultipartUploadSessionAsJToken>d__2 <CompleteMultipartUploadSessionAsJToken>d__;
				<CompleteMultipartUploadSessionAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<CompleteMultipartUploadSessionAsJToken>d__.uploadId = uploadId;
				<CompleteMultipartUploadSessionAsJToken>d__.modId = modId;
				<CompleteMultipartUploadSessionAsJToken>d__.<>1__state = -1;
				<CompleteMultipartUploadSessionAsJToken>d__.<>t__builder.Start<ModioAPI.FilesMultipartUploads.<CompleteMultipartUploadSessionAsJToken>d__2>(ref <CompleteMultipartUploadSessionAsJToken>d__);
				return <CompleteMultipartUploadSessionAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"multipartUploadObject"
			})]
			internal static Task<ValueTuple<Error, MultipartUploadObject?>> CompleteMultipartUploadSession(string uploadId, long modId)
			{
				ModioAPI.FilesMultipartUploads.<CompleteMultipartUploadSession>d__3 <CompleteMultipartUploadSession>d__;
				<CompleteMultipartUploadSession>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, MultipartUploadObject?>>.Create();
				<CompleteMultipartUploadSession>d__.uploadId = uploadId;
				<CompleteMultipartUploadSession>d__.modId = modId;
				<CompleteMultipartUploadSession>d__.<>1__state = -1;
				<CompleteMultipartUploadSession>d__.<>t__builder.Start<ModioAPI.FilesMultipartUploads.<CompleteMultipartUploadSession>d__3>(ref <CompleteMultipartUploadSession>d__);
				return <CompleteMultipartUploadSession>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"multipartUploadObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> CreateMultipartUploadSessionAsJToken(long modId, CreateMultipartUploadSessionRequest? body = null)
			{
				ModioAPI.FilesMultipartUploads.<CreateMultipartUploadSessionAsJToken>d__4 <CreateMultipartUploadSessionAsJToken>d__;
				<CreateMultipartUploadSessionAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<CreateMultipartUploadSessionAsJToken>d__.modId = modId;
				<CreateMultipartUploadSessionAsJToken>d__.body = body;
				<CreateMultipartUploadSessionAsJToken>d__.<>1__state = -1;
				<CreateMultipartUploadSessionAsJToken>d__.<>t__builder.Start<ModioAPI.FilesMultipartUploads.<CreateMultipartUploadSessionAsJToken>d__4>(ref <CreateMultipartUploadSessionAsJToken>d__);
				return <CreateMultipartUploadSessionAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"multipartUploadObject"
			})]
			internal static Task<ValueTuple<Error, MultipartUploadObject?>> CreateMultipartUploadSession(long modId, CreateMultipartUploadSessionRequest? body = null)
			{
				ModioAPI.FilesMultipartUploads.<CreateMultipartUploadSession>d__5 <CreateMultipartUploadSession>d__;
				<CreateMultipartUploadSession>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, MultipartUploadObject?>>.Create();
				<CreateMultipartUploadSession>d__.modId = modId;
				<CreateMultipartUploadSession>d__.body = body;
				<CreateMultipartUploadSession>d__.<>1__state = -1;
				<CreateMultipartUploadSession>d__.<>t__builder.Start<ModioAPI.FilesMultipartUploads.<CreateMultipartUploadSession>d__5>(ref <CreateMultipartUploadSession>d__);
				return <CreateMultipartUploadSession>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, JToken>> DeleteMultipartUploadSessionAsJToken(string uploadId, long modId)
			{
				ModioAPI.FilesMultipartUploads.<DeleteMultipartUploadSessionAsJToken>d__6 <DeleteMultipartUploadSessionAsJToken>d__;
				<DeleteMultipartUploadSessionAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<DeleteMultipartUploadSessionAsJToken>d__.uploadId = uploadId;
				<DeleteMultipartUploadSessionAsJToken>d__.modId = modId;
				<DeleteMultipartUploadSessionAsJToken>d__.<>1__state = -1;
				<DeleteMultipartUploadSessionAsJToken>d__.<>t__builder.Start<ModioAPI.FilesMultipartUploads.<DeleteMultipartUploadSessionAsJToken>d__6>(ref <DeleteMultipartUploadSessionAsJToken>d__);
				return <DeleteMultipartUploadSessionAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, Response204?>> DeleteMultipartUploadSession(string uploadId, long modId)
			{
				ModioAPI.FilesMultipartUploads.<DeleteMultipartUploadSession>d__7 <DeleteMultipartUploadSession>d__;
				<DeleteMultipartUploadSession>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Response204?>>.Create();
				<DeleteMultipartUploadSession>d__.uploadId = uploadId;
				<DeleteMultipartUploadSession>d__.modId = modId;
				<DeleteMultipartUploadSession>d__.<>1__state = -1;
				<DeleteMultipartUploadSession>d__.<>t__builder.Start<ModioAPI.FilesMultipartUploads.<DeleteMultipartUploadSession>d__7>(ref <DeleteMultipartUploadSession>d__);
				return <DeleteMultipartUploadSession>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"multipartUploadPartObjects"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetMultipartUploadPartsAsJToken(long modId, ModioAPI.FilesMultipartUploads.GetMultipartUploadPartsFilter filter)
			{
				ModioAPI.FilesMultipartUploads.<GetMultipartUploadPartsAsJToken>d__8 <GetMultipartUploadPartsAsJToken>d__;
				<GetMultipartUploadPartsAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetMultipartUploadPartsAsJToken>d__.modId = modId;
				<GetMultipartUploadPartsAsJToken>d__.<>1__state = -1;
				<GetMultipartUploadPartsAsJToken>d__.<>t__builder.Start<ModioAPI.FilesMultipartUploads.<GetMultipartUploadPartsAsJToken>d__8>(ref <GetMultipartUploadPartsAsJToken>d__);
				return <GetMultipartUploadPartsAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"multipartUploadPartObjects"
			})]
			internal static Task<ValueTuple<Error, Pagination<MultipartUploadPartObject[]>?>> GetMultipartUploadParts(long modId, ModioAPI.FilesMultipartUploads.GetMultipartUploadPartsFilter filter)
			{
				ModioAPI.FilesMultipartUploads.<GetMultipartUploadParts>d__9 <GetMultipartUploadParts>d__;
				<GetMultipartUploadParts>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<MultipartUploadPartObject[]>?>>.Create();
				<GetMultipartUploadParts>d__.modId = modId;
				<GetMultipartUploadParts>d__.filter = filter;
				<GetMultipartUploadParts>d__.<>1__state = -1;
				<GetMultipartUploadParts>d__.<>t__builder.Start<ModioAPI.FilesMultipartUploads.<GetMultipartUploadParts>d__9>(ref <GetMultipartUploadParts>d__);
				return <GetMultipartUploadParts>d__.<>t__builder.Task;
			}

			public static ModioAPI.FilesMultipartUploads.GetMultipartUploadPartsFilter FilterGetMultipartUploadParts(string uploadId, int pageIndex = 0, int pageSize = 100)
			{
				return new ModioAPI.FilesMultipartUploads.GetMultipartUploadPartsFilter(pageIndex, pageSize, uploadId);
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"multipartUploadObjects"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetMultipartUploadSessionsAsJToken(long modId, ModioAPI.FilesMultipartUploads.GetMultipartUploadSessionsFilter filter)
			{
				ModioAPI.FilesMultipartUploads.<GetMultipartUploadSessionsAsJToken>d__12 <GetMultipartUploadSessionsAsJToken>d__;
				<GetMultipartUploadSessionsAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetMultipartUploadSessionsAsJToken>d__.modId = modId;
				<GetMultipartUploadSessionsAsJToken>d__.<>1__state = -1;
				<GetMultipartUploadSessionsAsJToken>d__.<>t__builder.Start<ModioAPI.FilesMultipartUploads.<GetMultipartUploadSessionsAsJToken>d__12>(ref <GetMultipartUploadSessionsAsJToken>d__);
				return <GetMultipartUploadSessionsAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"multipartUploadObjects"
			})]
			internal static Task<ValueTuple<Error, Pagination<MultipartUploadObject[]>?>> GetMultipartUploadSessions(long modId, ModioAPI.FilesMultipartUploads.GetMultipartUploadSessionsFilter filter)
			{
				ModioAPI.FilesMultipartUploads.<GetMultipartUploadSessions>d__13 <GetMultipartUploadSessions>d__;
				<GetMultipartUploadSessions>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<MultipartUploadObject[]>?>>.Create();
				<GetMultipartUploadSessions>d__.modId = modId;
				<GetMultipartUploadSessions>d__.filter = filter;
				<GetMultipartUploadSessions>d__.<>1__state = -1;
				<GetMultipartUploadSessions>d__.<>t__builder.Start<ModioAPI.FilesMultipartUploads.<GetMultipartUploadSessions>d__13>(ref <GetMultipartUploadSessions>d__);
				return <GetMultipartUploadSessions>d__.<>t__builder.Task;
			}

			public static ModioAPI.FilesMultipartUploads.GetMultipartUploadSessionsFilter FilterGetMultipartUploadSessions(int pageIndex = 0, int pageSize = 100)
			{
				return new ModioAPI.FilesMultipartUploads.GetMultipartUploadSessionsFilter(pageIndex, pageSize);
			}

			public class GetMultipartUploadPartsFilter : SearchFilter<ModioAPI.FilesMultipartUploads.GetMultipartUploadPartsFilter>
			{
				internal GetMultipartUploadPartsFilter(int pageIndex, int pageSize, string uploadId) : base(pageIndex, pageSize)
				{
					this._uploadId = uploadId;
				}

				internal string _uploadId;
			}

			public class GetMultipartUploadSessionsFilter : SearchFilter<ModioAPI.FilesMultipartUploads.GetMultipartUploadSessionsFilter>
			{
				internal GetMultipartUploadSessionsFilter(int pageIndex, int pageSize) : base(pageIndex, pageSize)
				{
				}

				public ModioAPI.FilesMultipartUploads.GetMultipartUploadSessionsFilter Status(long status, Filtering condition = Filtering.None)
				{
					this.Parameters["status" + condition.ClearText()] = status;
					return this;
				}

				public ModioAPI.FilesMultipartUploads.GetMultipartUploadSessionsFilter Status(ICollection<long> status, Filtering condition = Filtering.None)
				{
					this.Parameters["status" + condition.ClearText()] = status;
					return this;
				}
			}
		}

		public static class Authentication
		{
			[return: TupleElementNames(new string[]
			{
				"error",
				"accessTokenObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> AuthenticateViaAppleAsJToken(AppleAuthenticationRequest? body = null)
			{
				ModioAPI.Authentication.<AuthenticateViaAppleAsJToken>d__0 <AuthenticateViaAppleAsJToken>d__;
				<AuthenticateViaAppleAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<AuthenticateViaAppleAsJToken>d__.body = body;
				<AuthenticateViaAppleAsJToken>d__.<>1__state = -1;
				<AuthenticateViaAppleAsJToken>d__.<>t__builder.Start<ModioAPI.Authentication.<AuthenticateViaAppleAsJToken>d__0>(ref <AuthenticateViaAppleAsJToken>d__);
				return <AuthenticateViaAppleAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"accessTokenObject"
			})]
			internal static Task<ValueTuple<Error, AccessTokenObject?>> AuthenticateViaApple(AppleAuthenticationRequest? body = null)
			{
				ModioAPI.Authentication.<AuthenticateViaApple>d__1 <AuthenticateViaApple>d__;
				<AuthenticateViaApple>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, AccessTokenObject?>>.Create();
				<AuthenticateViaApple>d__.body = body;
				<AuthenticateViaApple>d__.<>1__state = -1;
				<AuthenticateViaApple>d__.<>t__builder.Start<ModioAPI.Authentication.<AuthenticateViaApple>d__1>(ref <AuthenticateViaApple>d__);
				return <AuthenticateViaApple>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"accessTokenObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> AuthenticateViaDiscordAsJToken(DiscordAuthenticationRequest? body = null)
			{
				ModioAPI.Authentication.<AuthenticateViaDiscordAsJToken>d__2 <AuthenticateViaDiscordAsJToken>d__;
				<AuthenticateViaDiscordAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<AuthenticateViaDiscordAsJToken>d__.body = body;
				<AuthenticateViaDiscordAsJToken>d__.<>1__state = -1;
				<AuthenticateViaDiscordAsJToken>d__.<>t__builder.Start<ModioAPI.Authentication.<AuthenticateViaDiscordAsJToken>d__2>(ref <AuthenticateViaDiscordAsJToken>d__);
				return <AuthenticateViaDiscordAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"accessTokenObject"
			})]
			internal static Task<ValueTuple<Error, AccessTokenObject?>> AuthenticateViaDiscord(DiscordAuthenticationRequest? body = null)
			{
				ModioAPI.Authentication.<AuthenticateViaDiscord>d__3 <AuthenticateViaDiscord>d__;
				<AuthenticateViaDiscord>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, AccessTokenObject?>>.Create();
				<AuthenticateViaDiscord>d__.body = body;
				<AuthenticateViaDiscord>d__.<>1__state = -1;
				<AuthenticateViaDiscord>d__.<>t__builder.Start<ModioAPI.Authentication.<AuthenticateViaDiscord>d__3>(ref <AuthenticateViaDiscord>d__);
				return <AuthenticateViaDiscord>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"accessTokenObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> AuthenticateViaEpicgamesAsJToken(EpicGamesAuthenticationRequest? body = null)
			{
				ModioAPI.Authentication.<AuthenticateViaEpicgamesAsJToken>d__4 <AuthenticateViaEpicgamesAsJToken>d__;
				<AuthenticateViaEpicgamesAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<AuthenticateViaEpicgamesAsJToken>d__.body = body;
				<AuthenticateViaEpicgamesAsJToken>d__.<>1__state = -1;
				<AuthenticateViaEpicgamesAsJToken>d__.<>t__builder.Start<ModioAPI.Authentication.<AuthenticateViaEpicgamesAsJToken>d__4>(ref <AuthenticateViaEpicgamesAsJToken>d__);
				return <AuthenticateViaEpicgamesAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"accessTokenObject"
			})]
			internal static Task<ValueTuple<Error, AccessTokenObject?>> AuthenticateViaEpicgames(EpicGamesAuthenticationRequest? body = null)
			{
				ModioAPI.Authentication.<AuthenticateViaEpicgames>d__5 <AuthenticateViaEpicgames>d__;
				<AuthenticateViaEpicgames>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, AccessTokenObject?>>.Create();
				<AuthenticateViaEpicgames>d__.body = body;
				<AuthenticateViaEpicgames>d__.<>1__state = -1;
				<AuthenticateViaEpicgames>d__.<>t__builder.Start<ModioAPI.Authentication.<AuthenticateViaEpicgames>d__5>(ref <AuthenticateViaEpicgames>d__);
				return <AuthenticateViaEpicgames>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"accessTokenObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> AuthenticateViaFacebookAsJToken(FacebookAuthenticationRequest? body = null)
			{
				ModioAPI.Authentication.<AuthenticateViaFacebookAsJToken>d__6 <AuthenticateViaFacebookAsJToken>d__;
				<AuthenticateViaFacebookAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<AuthenticateViaFacebookAsJToken>d__.body = body;
				<AuthenticateViaFacebookAsJToken>d__.<>1__state = -1;
				<AuthenticateViaFacebookAsJToken>d__.<>t__builder.Start<ModioAPI.Authentication.<AuthenticateViaFacebookAsJToken>d__6>(ref <AuthenticateViaFacebookAsJToken>d__);
				return <AuthenticateViaFacebookAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"accessTokenObject"
			})]
			internal static Task<ValueTuple<Error, AccessTokenObject?>> AuthenticateViaFacebook(FacebookAuthenticationRequest? body = null)
			{
				ModioAPI.Authentication.<AuthenticateViaFacebook>d__7 <AuthenticateViaFacebook>d__;
				<AuthenticateViaFacebook>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, AccessTokenObject?>>.Create();
				<AuthenticateViaFacebook>d__.body = body;
				<AuthenticateViaFacebook>d__.<>1__state = -1;
				<AuthenticateViaFacebook>d__.<>t__builder.Start<ModioAPI.Authentication.<AuthenticateViaFacebook>d__7>(ref <AuthenticateViaFacebook>d__);
				return <AuthenticateViaFacebook>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"accessTokenObject"
			})]
			public static Task<ValueTuple<Error, JToken>> AuthenticateViaGogGalaxyAsJToken(GogAuthenticationRequest? body = null)
			{
				ModioAPI.Authentication.<AuthenticateViaGogGalaxyAsJToken>d__8 <AuthenticateViaGogGalaxyAsJToken>d__;
				<AuthenticateViaGogGalaxyAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<AuthenticateViaGogGalaxyAsJToken>d__.body = body;
				<AuthenticateViaGogGalaxyAsJToken>d__.<>1__state = -1;
				<AuthenticateViaGogGalaxyAsJToken>d__.<>t__builder.Start<ModioAPI.Authentication.<AuthenticateViaGogGalaxyAsJToken>d__8>(ref <AuthenticateViaGogGalaxyAsJToken>d__);
				return <AuthenticateViaGogGalaxyAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"accessTokenObject"
			})]
			public static Task<ValueTuple<Error, AccessTokenObject?>> AuthenticateViaGogGalaxy(GogAuthenticationRequest? body = null)
			{
				ModioAPI.Authentication.<AuthenticateViaGogGalaxy>d__9 <AuthenticateViaGogGalaxy>d__;
				<AuthenticateViaGogGalaxy>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, AccessTokenObject?>>.Create();
				<AuthenticateViaGogGalaxy>d__.body = body;
				<AuthenticateViaGogGalaxy>d__.<>1__state = -1;
				<AuthenticateViaGogGalaxy>d__.<>t__builder.Start<ModioAPI.Authentication.<AuthenticateViaGogGalaxy>d__9>(ref <AuthenticateViaGogGalaxy>d__);
				return <AuthenticateViaGogGalaxy>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"accessTokenObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> AuthenticateViaGoogleAsJToken(GoogleAuthenticationRequest? body = null)
			{
				ModioAPI.Authentication.<AuthenticateViaGoogleAsJToken>d__10 <AuthenticateViaGoogleAsJToken>d__;
				<AuthenticateViaGoogleAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<AuthenticateViaGoogleAsJToken>d__.body = body;
				<AuthenticateViaGoogleAsJToken>d__.<>1__state = -1;
				<AuthenticateViaGoogleAsJToken>d__.<>t__builder.Start<ModioAPI.Authentication.<AuthenticateViaGoogleAsJToken>d__10>(ref <AuthenticateViaGoogleAsJToken>d__);
				return <AuthenticateViaGoogleAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"accessTokenObject"
			})]
			internal static Task<ValueTuple<Error, AccessTokenObject?>> AuthenticateViaGoogle(GoogleAuthenticationRequest? body = null)
			{
				ModioAPI.Authentication.<AuthenticateViaGoogle>d__11 <AuthenticateViaGoogle>d__;
				<AuthenticateViaGoogle>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, AccessTokenObject?>>.Create();
				<AuthenticateViaGoogle>d__.body = body;
				<AuthenticateViaGoogle>d__.<>1__state = -1;
				<AuthenticateViaGoogle>d__.<>t__builder.Start<ModioAPI.Authentication.<AuthenticateViaGoogle>d__11>(ref <AuthenticateViaGoogle>d__);
				return <AuthenticateViaGoogle>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"accessTokenObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> AuthenticateViaItchioAsJToken(ItchioAuthenticationRequest? body = null)
			{
				ModioAPI.Authentication.<AuthenticateViaItchioAsJToken>d__12 <AuthenticateViaItchioAsJToken>d__;
				<AuthenticateViaItchioAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<AuthenticateViaItchioAsJToken>d__.body = body;
				<AuthenticateViaItchioAsJToken>d__.<>1__state = -1;
				<AuthenticateViaItchioAsJToken>d__.<>t__builder.Start<ModioAPI.Authentication.<AuthenticateViaItchioAsJToken>d__12>(ref <AuthenticateViaItchioAsJToken>d__);
				return <AuthenticateViaItchioAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"accessTokenObject"
			})]
			internal static Task<ValueTuple<Error, AccessTokenObject?>> AuthenticateViaItchio(ItchioAuthenticationRequest? body = null)
			{
				ModioAPI.Authentication.<AuthenticateViaItchio>d__13 <AuthenticateViaItchio>d__;
				<AuthenticateViaItchio>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, AccessTokenObject?>>.Create();
				<AuthenticateViaItchio>d__.body = body;
				<AuthenticateViaItchio>d__.<>1__state = -1;
				<AuthenticateViaItchio>d__.<>t__builder.Start<ModioAPI.Authentication.<AuthenticateViaItchio>d__13>(ref <AuthenticateViaItchio>d__);
				return <AuthenticateViaItchio>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"accessTokenObject"
			})]
			public static Task<ValueTuple<Error, JToken>> AuthenticateViaOculusAsJToken(MetaQuestAuthenticationRequest? body = null)
			{
				ModioAPI.Authentication.<AuthenticateViaOculusAsJToken>d__14 <AuthenticateViaOculusAsJToken>d__;
				<AuthenticateViaOculusAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<AuthenticateViaOculusAsJToken>d__.body = body;
				<AuthenticateViaOculusAsJToken>d__.<>1__state = -1;
				<AuthenticateViaOculusAsJToken>d__.<>t__builder.Start<ModioAPI.Authentication.<AuthenticateViaOculusAsJToken>d__14>(ref <AuthenticateViaOculusAsJToken>d__);
				return <AuthenticateViaOculusAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"accessTokenObject"
			})]
			public static Task<ValueTuple<Error, AccessTokenObject?>> AuthenticateViaOculus(MetaQuestAuthenticationRequest? body = null)
			{
				ModioAPI.Authentication.<AuthenticateViaOculus>d__15 <AuthenticateViaOculus>d__;
				<AuthenticateViaOculus>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, AccessTokenObject?>>.Create();
				<AuthenticateViaOculus>d__.body = body;
				<AuthenticateViaOculus>d__.<>1__state = -1;
				<AuthenticateViaOculus>d__.<>t__builder.Start<ModioAPI.Authentication.<AuthenticateViaOculus>d__15>(ref <AuthenticateViaOculus>d__);
				return <AuthenticateViaOculus>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"accessTokenObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> AuthenticateViaOpenidAsJToken(OpenIdAuthenticationRequest? body = null)
			{
				ModioAPI.Authentication.<AuthenticateViaOpenidAsJToken>d__16 <AuthenticateViaOpenidAsJToken>d__;
				<AuthenticateViaOpenidAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<AuthenticateViaOpenidAsJToken>d__.body = body;
				<AuthenticateViaOpenidAsJToken>d__.<>1__state = -1;
				<AuthenticateViaOpenidAsJToken>d__.<>t__builder.Start<ModioAPI.Authentication.<AuthenticateViaOpenidAsJToken>d__16>(ref <AuthenticateViaOpenidAsJToken>d__);
				return <AuthenticateViaOpenidAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"accessTokenObject"
			})]
			internal static Task<ValueTuple<Error, AccessTokenObject?>> AuthenticateViaOpenid(OpenIdAuthenticationRequest? body = null)
			{
				ModioAPI.Authentication.<AuthenticateViaOpenid>d__17 <AuthenticateViaOpenid>d__;
				<AuthenticateViaOpenid>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, AccessTokenObject?>>.Create();
				<AuthenticateViaOpenid>d__.body = body;
				<AuthenticateViaOpenid>d__.<>1__state = -1;
				<AuthenticateViaOpenid>d__.<>t__builder.Start<ModioAPI.Authentication.<AuthenticateViaOpenid>d__17>(ref <AuthenticateViaOpenid>d__);
				return <AuthenticateViaOpenid>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"accessTokenObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> AuthenticateViaPsnAsJToken(PsnAuthenticationRequest? body = null)
			{
				ModioAPI.Authentication.<AuthenticateViaPsnAsJToken>d__18 <AuthenticateViaPsnAsJToken>d__;
				<AuthenticateViaPsnAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<AuthenticateViaPsnAsJToken>d__.body = body;
				<AuthenticateViaPsnAsJToken>d__.<>1__state = -1;
				<AuthenticateViaPsnAsJToken>d__.<>t__builder.Start<ModioAPI.Authentication.<AuthenticateViaPsnAsJToken>d__18>(ref <AuthenticateViaPsnAsJToken>d__);
				return <AuthenticateViaPsnAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"accessTokenObject"
			})]
			public static Task<ValueTuple<Error, AccessTokenObject?>> AuthenticateViaPsn(PsnAuthenticationRequest? body = null)
			{
				ModioAPI.Authentication.<AuthenticateViaPsn>d__19 <AuthenticateViaPsn>d__;
				<AuthenticateViaPsn>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, AccessTokenObject?>>.Create();
				<AuthenticateViaPsn>d__.body = body;
				<AuthenticateViaPsn>d__.<>1__state = -1;
				<AuthenticateViaPsn>d__.<>t__builder.Start<ModioAPI.Authentication.<AuthenticateViaPsn>d__19>(ref <AuthenticateViaPsn>d__);
				return <AuthenticateViaPsn>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"accessTokenObject"
			})]
			public static Task<ValueTuple<Error, JToken>> AuthenticateViaSteamAsJToken(SteamAuthenticationRequest? body = null)
			{
				ModioAPI.Authentication.<AuthenticateViaSteamAsJToken>d__20 <AuthenticateViaSteamAsJToken>d__;
				<AuthenticateViaSteamAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<AuthenticateViaSteamAsJToken>d__.body = body;
				<AuthenticateViaSteamAsJToken>d__.<>1__state = -1;
				<AuthenticateViaSteamAsJToken>d__.<>t__builder.Start<ModioAPI.Authentication.<AuthenticateViaSteamAsJToken>d__20>(ref <AuthenticateViaSteamAsJToken>d__);
				return <AuthenticateViaSteamAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"accessTokenObject"
			})]
			public static Task<ValueTuple<Error, AccessTokenObject?>> AuthenticateViaSteam(SteamAuthenticationRequest? body = null)
			{
				ModioAPI.Authentication.<AuthenticateViaSteam>d__21 <AuthenticateViaSteam>d__;
				<AuthenticateViaSteam>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, AccessTokenObject?>>.Create();
				<AuthenticateViaSteam>d__.body = body;
				<AuthenticateViaSteam>d__.<>1__state = -1;
				<AuthenticateViaSteam>d__.<>t__builder.Start<ModioAPI.Authentication.<AuthenticateViaSteam>d__21>(ref <AuthenticateViaSteam>d__);
				return <AuthenticateViaSteam>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"accessTokenObject"
			})]
			public static Task<ValueTuple<Error, JToken>> AuthenticateViaSwitchAsJToken(SwitchAuthenticationRequest? body = null)
			{
				ModioAPI.Authentication.<AuthenticateViaSwitchAsJToken>d__22 <AuthenticateViaSwitchAsJToken>d__;
				<AuthenticateViaSwitchAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<AuthenticateViaSwitchAsJToken>d__.body = body;
				<AuthenticateViaSwitchAsJToken>d__.<>1__state = -1;
				<AuthenticateViaSwitchAsJToken>d__.<>t__builder.Start<ModioAPI.Authentication.<AuthenticateViaSwitchAsJToken>d__22>(ref <AuthenticateViaSwitchAsJToken>d__);
				return <AuthenticateViaSwitchAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"accessTokenObject"
			})]
			public static Task<ValueTuple<Error, AccessTokenObject?>> AuthenticateViaSwitch(SwitchAuthenticationRequest? body = null)
			{
				ModioAPI.Authentication.<AuthenticateViaSwitch>d__23 <AuthenticateViaSwitch>d__;
				<AuthenticateViaSwitch>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, AccessTokenObject?>>.Create();
				<AuthenticateViaSwitch>d__.body = body;
				<AuthenticateViaSwitch>d__.<>1__state = -1;
				<AuthenticateViaSwitch>d__.<>t__builder.Start<ModioAPI.Authentication.<AuthenticateViaSwitch>d__23>(ref <AuthenticateViaSwitch>d__);
				return <AuthenticateViaSwitch>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"accessTokenObject"
			})]
			public static Task<ValueTuple<Error, JToken>> AuthenticateViaXboxLiveAsJToken(XboxLiveAuthenticationRequest? body = null)
			{
				ModioAPI.Authentication.<AuthenticateViaXboxLiveAsJToken>d__24 <AuthenticateViaXboxLiveAsJToken>d__;
				<AuthenticateViaXboxLiveAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<AuthenticateViaXboxLiveAsJToken>d__.body = body;
				<AuthenticateViaXboxLiveAsJToken>d__.<>1__state = -1;
				<AuthenticateViaXboxLiveAsJToken>d__.<>t__builder.Start<ModioAPI.Authentication.<AuthenticateViaXboxLiveAsJToken>d__24>(ref <AuthenticateViaXboxLiveAsJToken>d__);
				return <AuthenticateViaXboxLiveAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"accessTokenObject"
			})]
			public static Task<ValueTuple<Error, AccessTokenObject?>> AuthenticateViaXboxLive(XboxLiveAuthenticationRequest? body = null)
			{
				ModioAPI.Authentication.<AuthenticateViaXboxLive>d__25 <AuthenticateViaXboxLive>d__;
				<AuthenticateViaXboxLive>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, AccessTokenObject?>>.Create();
				<AuthenticateViaXboxLive>d__.body = body;
				<AuthenticateViaXboxLive>d__.<>1__state = -1;
				<AuthenticateViaXboxLive>d__.<>t__builder.Start<ModioAPI.Authentication.<AuthenticateViaXboxLive>d__25>(ref <AuthenticateViaXboxLive>d__);
				return <AuthenticateViaXboxLive>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"accessTokenObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> ExchangeEmailSecurityCodeAsJToken(EmailAuthenticationSecurityCodeRequest? body = null)
			{
				ModioAPI.Authentication.<ExchangeEmailSecurityCodeAsJToken>d__26 <ExchangeEmailSecurityCodeAsJToken>d__;
				<ExchangeEmailSecurityCodeAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<ExchangeEmailSecurityCodeAsJToken>d__.body = body;
				<ExchangeEmailSecurityCodeAsJToken>d__.<>1__state = -1;
				<ExchangeEmailSecurityCodeAsJToken>d__.<>t__builder.Start<ModioAPI.Authentication.<ExchangeEmailSecurityCodeAsJToken>d__26>(ref <ExchangeEmailSecurityCodeAsJToken>d__);
				return <ExchangeEmailSecurityCodeAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"accessTokenObject"
			})]
			internal static Task<ValueTuple<Error, AccessTokenObject?>> ExchangeEmailSecurityCode(EmailAuthenticationSecurityCodeRequest? body = null)
			{
				ModioAPI.Authentication.<ExchangeEmailSecurityCode>d__27 <ExchangeEmailSecurityCode>d__;
				<ExchangeEmailSecurityCode>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, AccessTokenObject?>>.Create();
				<ExchangeEmailSecurityCode>d__.body = body;
				<ExchangeEmailSecurityCode>d__.<>1__state = -1;
				<ExchangeEmailSecurityCode>d__.<>t__builder.Start<ModioAPI.Authentication.<ExchangeEmailSecurityCode>d__27>(ref <ExchangeEmailSecurityCode>d__);
				return <ExchangeEmailSecurityCode>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"webMessageObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> LogoutAsJToken()
			{
				ModioAPI.Authentication.<LogoutAsJToken>d__28 <LogoutAsJToken>d__;
				<LogoutAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<LogoutAsJToken>d__.<>1__state = -1;
				<LogoutAsJToken>d__.<>t__builder.Start<ModioAPI.Authentication.<LogoutAsJToken>d__28>(ref <LogoutAsJToken>d__);
				return <LogoutAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"webMessageObject"
			})]
			internal static Task<ValueTuple<Error, WebMessageObject?>> Logout()
			{
				ModioAPI.Authentication.<Logout>d__29 <Logout>d__;
				<Logout>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, WebMessageObject?>>.Create();
				<Logout>d__.<>1__state = -1;
				<Logout>d__.<>t__builder.Start<ModioAPI.Authentication.<Logout>d__29>(ref <Logout>d__);
				return <Logout>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"emailRequestResponse"
			})]
			internal static Task<ValueTuple<Error, JToken>> RequestEmailSecurityCodeAsJToken(EmailAuthenticationRequest? body = null)
			{
				ModioAPI.Authentication.<RequestEmailSecurityCodeAsJToken>d__30 <RequestEmailSecurityCodeAsJToken>d__;
				<RequestEmailSecurityCodeAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<RequestEmailSecurityCodeAsJToken>d__.body = body;
				<RequestEmailSecurityCodeAsJToken>d__.<>1__state = -1;
				<RequestEmailSecurityCodeAsJToken>d__.<>t__builder.Start<ModioAPI.Authentication.<RequestEmailSecurityCodeAsJToken>d__30>(ref <RequestEmailSecurityCodeAsJToken>d__);
				return <RequestEmailSecurityCodeAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"emailRequestResponse"
			})]
			internal static Task<ValueTuple<Error, EmailRequestResponse?>> RequestEmailSecurityCode(EmailAuthenticationRequest? body = null)
			{
				ModioAPI.Authentication.<RequestEmailSecurityCode>d__31 <RequestEmailSecurityCode>d__;
				<RequestEmailSecurityCode>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, EmailRequestResponse?>>.Create();
				<RequestEmailSecurityCode>d__.body = body;
				<RequestEmailSecurityCode>d__.<>1__state = -1;
				<RequestEmailSecurityCode>d__.<>t__builder.Start<ModioAPI.Authentication.<RequestEmailSecurityCode>d__31>(ref <RequestEmailSecurityCode>d__);
				return <RequestEmailSecurityCode>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"termsObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> TermsAsJToken()
			{
				ModioAPI.Authentication.<TermsAsJToken>d__32 <TermsAsJToken>d__;
				<TermsAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<TermsAsJToken>d__.<>1__state = -1;
				<TermsAsJToken>d__.<>t__builder.Start<ModioAPI.Authentication.<TermsAsJToken>d__32>(ref <TermsAsJToken>d__);
				return <TermsAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"termsObject"
			})]
			internal static Task<ValueTuple<Error, TermsObject?>> Terms()
			{
				ModioAPI.Authentication.<Terms>d__33 <Terms>d__;
				<Terms>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, TermsObject?>>.Create();
				<Terms>d__.<>1__state = -1;
				<Terms>d__.<>t__builder.Start<ModioAPI.Authentication.<Terms>d__33>(ref <Terms>d__);
				return <Terms>d__.<>t__builder.Task;
			}
		}

		public static class Monetization
		{
			[return: TupleElementNames(new string[]
			{
				"error",
				"monetizationTeamAccountsObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> CreateModMonetizationTeamAsJToken(long modId)
			{
				ModioAPI.Monetization.<CreateModMonetizationTeamAsJToken>d__0 <CreateModMonetizationTeamAsJToken>d__;
				<CreateModMonetizationTeamAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<CreateModMonetizationTeamAsJToken>d__.modId = modId;
				<CreateModMonetizationTeamAsJToken>d__.<>1__state = -1;
				<CreateModMonetizationTeamAsJToken>d__.<>t__builder.Start<ModioAPI.Monetization.<CreateModMonetizationTeamAsJToken>d__0>(ref <CreateModMonetizationTeamAsJToken>d__);
				return <CreateModMonetizationTeamAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"monetizationTeamAccountsObject"
			})]
			internal static Task<ValueTuple<Error, MonetizationTeamAccountsObject?>> CreateModMonetizationTeam(long modId)
			{
				ModioAPI.Monetization.<CreateModMonetizationTeam>d__1 <CreateModMonetizationTeam>d__;
				<CreateModMonetizationTeam>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, MonetizationTeamAccountsObject?>>.Create();
				<CreateModMonetizationTeam>d__.modId = modId;
				<CreateModMonetizationTeam>d__.<>1__state = -1;
				<CreateModMonetizationTeam>d__.<>t__builder.Start<ModioAPI.Monetization.<CreateModMonetizationTeam>d__1>(ref <CreateModMonetizationTeam>d__);
				return <CreateModMonetizationTeam>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"gameTokenPackObject"
			})]
			public static Task<ValueTuple<Error, JToken>> GetGameTokenPacksAsJToken()
			{
				ModioAPI.Monetization.<GetGameTokenPacksAsJToken>d__2 <GetGameTokenPacksAsJToken>d__;
				<GetGameTokenPacksAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetGameTokenPacksAsJToken>d__.<>1__state = -1;
				<GetGameTokenPacksAsJToken>d__.<>t__builder.Start<ModioAPI.Monetization.<GetGameTokenPacksAsJToken>d__2>(ref <GetGameTokenPacksAsJToken>d__);
				return <GetGameTokenPacksAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"gameTokenPackObject"
			})]
			public static Task<ValueTuple<Error, Pagination<GameTokenPackObject[]>?>> GetGameTokenPacks()
			{
				ModioAPI.Monetization.<GetGameTokenPacks>d__3 <GetGameTokenPacks>d__;
				<GetGameTokenPacks>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<GameTokenPackObject[]>?>>.Create();
				<GetGameTokenPacks>d__.<>1__state = -1;
				<GetGameTokenPacks>d__.<>t__builder.Start<ModioAPI.Monetization.<GetGameTokenPacks>d__3>(ref <GetGameTokenPacks>d__);
				return <GetGameTokenPacks>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"monetizationTeamAccountsObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetUsersInModMonetizationTeamAsJToken(long modId)
			{
				ModioAPI.Monetization.<GetUsersInModMonetizationTeamAsJToken>d__4 <GetUsersInModMonetizationTeamAsJToken>d__;
				<GetUsersInModMonetizationTeamAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetUsersInModMonetizationTeamAsJToken>d__.modId = modId;
				<GetUsersInModMonetizationTeamAsJToken>d__.<>1__state = -1;
				<GetUsersInModMonetizationTeamAsJToken>d__.<>t__builder.Start<ModioAPI.Monetization.<GetUsersInModMonetizationTeamAsJToken>d__4>(ref <GetUsersInModMonetizationTeamAsJToken>d__);
				return <GetUsersInModMonetizationTeamAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"monetizationTeamAccountsObject"
			})]
			internal static Task<ValueTuple<Error, MonetizationTeamAccountsObject?>> GetUsersInModMonetizationTeam(long modId)
			{
				ModioAPI.Monetization.<GetUsersInModMonetizationTeam>d__5 <GetUsersInModMonetizationTeam>d__;
				<GetUsersInModMonetizationTeam>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, MonetizationTeamAccountsObject?>>.Create();
				<GetUsersInModMonetizationTeam>d__.modId = modId;
				<GetUsersInModMonetizationTeam>d__.<>1__state = -1;
				<GetUsersInModMonetizationTeam>d__.<>t__builder.Start<ModioAPI.Monetization.<GetUsersInModMonetizationTeam>d__5>(ref <GetUsersInModMonetizationTeam>d__);
				return <GetUsersInModMonetizationTeam>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"payObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> PurchaseAsJToken(long modId, PayRequest? body = null)
			{
				ModioAPI.Monetization.<PurchaseAsJToken>d__6 <PurchaseAsJToken>d__;
				<PurchaseAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<PurchaseAsJToken>d__.modId = modId;
				<PurchaseAsJToken>d__.body = body;
				<PurchaseAsJToken>d__.<>1__state = -1;
				<PurchaseAsJToken>d__.<>t__builder.Start<ModioAPI.Monetization.<PurchaseAsJToken>d__6>(ref <PurchaseAsJToken>d__);
				return <PurchaseAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"payObject"
			})]
			internal static Task<ValueTuple<Error, PayObject?>> Purchase(long modId, PayRequest? body = null)
			{
				ModioAPI.Monetization.<Purchase>d__7 <Purchase>d__;
				<Purchase>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, PayObject?>>.Create();
				<Purchase>d__.modId = modId;
				<Purchase>d__.body = body;
				<Purchase>d__.<>1__state = -1;
				<Purchase>d__.<>t__builder.Start<ModioAPI.Monetization.<Purchase>d__7>(ref <Purchase>d__);
				return <Purchase>d__.<>t__builder.Task;
			}
		}

		public static class Agreements
		{
			[return: TupleElementNames(new string[]
			{
				"error",
				"agreementVersionObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetAgreementVersionAsJToken(long agreementVersionId)
			{
				ModioAPI.Agreements.<GetAgreementVersionAsJToken>d__0 <GetAgreementVersionAsJToken>d__;
				<GetAgreementVersionAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetAgreementVersionAsJToken>d__.agreementVersionId = agreementVersionId;
				<GetAgreementVersionAsJToken>d__.<>1__state = -1;
				<GetAgreementVersionAsJToken>d__.<>t__builder.Start<ModioAPI.Agreements.<GetAgreementVersionAsJToken>d__0>(ref <GetAgreementVersionAsJToken>d__);
				return <GetAgreementVersionAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"agreementVersionObject"
			})]
			internal static Task<ValueTuple<Error, AgreementVersionObject?>> GetAgreementVersion(long agreementVersionId)
			{
				ModioAPI.Agreements.<GetAgreementVersion>d__1 <GetAgreementVersion>d__;
				<GetAgreementVersion>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, AgreementVersionObject?>>.Create();
				<GetAgreementVersion>d__.agreementVersionId = agreementVersionId;
				<GetAgreementVersion>d__.<>1__state = -1;
				<GetAgreementVersion>d__.<>t__builder.Start<ModioAPI.Agreements.<GetAgreementVersion>d__1>(ref <GetAgreementVersion>d__);
				return <GetAgreementVersion>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"agreementVersionObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetCurrentAgreementAsJToken(long agreementTypeId)
			{
				ModioAPI.Agreements.<GetCurrentAgreementAsJToken>d__2 <GetCurrentAgreementAsJToken>d__;
				<GetCurrentAgreementAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetCurrentAgreementAsJToken>d__.agreementTypeId = agreementTypeId;
				<GetCurrentAgreementAsJToken>d__.<>1__state = -1;
				<GetCurrentAgreementAsJToken>d__.<>t__builder.Start<ModioAPI.Agreements.<GetCurrentAgreementAsJToken>d__2>(ref <GetCurrentAgreementAsJToken>d__);
				return <GetCurrentAgreementAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"agreementVersionObject"
			})]
			internal static Task<ValueTuple<Error, AgreementVersionObject?>> GetCurrentAgreement(long agreementTypeId)
			{
				ModioAPI.Agreements.<GetCurrentAgreement>d__3 <GetCurrentAgreement>d__;
				<GetCurrentAgreement>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, AgreementVersionObject?>>.Create();
				<GetCurrentAgreement>d__.agreementTypeId = agreementTypeId;
				<GetCurrentAgreement>d__.<>1__state = -1;
				<GetCurrentAgreement>d__.<>t__builder.Start<ModioAPI.Agreements.<GetCurrentAgreement>d__3>(ref <GetCurrentAgreement>d__);
				return <GetCurrentAgreement>d__.<>t__builder.Task;
			}
		}

		public static class Me
		{
			[return: TupleElementNames(new string[]
			{
				"error",
				"userObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetAuthenticatedUserAsJToken(string xModioDelegationToken = null)
			{
				ModioAPI.Me.<GetAuthenticatedUserAsJToken>d__0 <GetAuthenticatedUserAsJToken>d__;
				<GetAuthenticatedUserAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetAuthenticatedUserAsJToken>d__.xModioDelegationToken = xModioDelegationToken;
				<GetAuthenticatedUserAsJToken>d__.<>1__state = -1;
				<GetAuthenticatedUserAsJToken>d__.<>t__builder.Start<ModioAPI.Me.<GetAuthenticatedUserAsJToken>d__0>(ref <GetAuthenticatedUserAsJToken>d__);
				return <GetAuthenticatedUserAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"userObject"
			})]
			internal static Task<ValueTuple<Error, UserObject?>> GetAuthenticatedUser(string xModioDelegationToken = null)
			{
				ModioAPI.Me.<GetAuthenticatedUser>d__1 <GetAuthenticatedUser>d__;
				<GetAuthenticatedUser>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, UserObject?>>.Create();
				<GetAuthenticatedUser>d__.xModioDelegationToken = xModioDelegationToken;
				<GetAuthenticatedUser>d__.<>1__state = -1;
				<GetAuthenticatedUser>d__.<>t__builder.Start<ModioAPI.Me.<GetAuthenticatedUser>d__1>(ref <GetAuthenticatedUser>d__);
				return <GetAuthenticatedUser>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"userEventObjects"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetUserEventsAsJToken(ModioAPI.Me.GetUserEventsFilter filter)
			{
				ModioAPI.Me.<GetUserEventsAsJToken>d__2 <GetUserEventsAsJToken>d__;
				<GetUserEventsAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetUserEventsAsJToken>d__.filter = filter;
				<GetUserEventsAsJToken>d__.<>1__state = -1;
				<GetUserEventsAsJToken>d__.<>t__builder.Start<ModioAPI.Me.<GetUserEventsAsJToken>d__2>(ref <GetUserEventsAsJToken>d__);
				return <GetUserEventsAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"userEventObjects"
			})]
			internal static Task<ValueTuple<Error, Pagination<UserEventObject[]>?>> GetUserEvents(ModioAPI.Me.GetUserEventsFilter filter)
			{
				ModioAPI.Me.<GetUserEvents>d__3 <GetUserEvents>d__;
				<GetUserEvents>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<UserEventObject[]>?>>.Create();
				<GetUserEvents>d__.filter = filter;
				<GetUserEvents>d__.<>1__state = -1;
				<GetUserEvents>d__.<>t__builder.Start<ModioAPI.Me.<GetUserEvents>d__3>(ref <GetUserEvents>d__);
				return <GetUserEvents>d__.<>t__builder.Task;
			}

			public static ModioAPI.Me.GetUserEventsFilter FilterGetUserEvents(int pageIndex = 0, int pageSize = 100)
			{
				return new ModioAPI.Me.GetUserEventsFilter(pageIndex, pageSize);
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"gameObjects"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetUserGamesAsJToken(ModioAPI.Me.GetUserGamesFilter filter)
			{
				ModioAPI.Me.<GetUserGamesAsJToken>d__6 <GetUserGamesAsJToken>d__;
				<GetUserGamesAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetUserGamesAsJToken>d__.filter = filter;
				<GetUserGamesAsJToken>d__.<>1__state = -1;
				<GetUserGamesAsJToken>d__.<>t__builder.Start<ModioAPI.Me.<GetUserGamesAsJToken>d__6>(ref <GetUserGamesAsJToken>d__);
				return <GetUserGamesAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"gameObjects"
			})]
			internal static Task<ValueTuple<Error, Pagination<GameObject[]>?>> GetUserGames(ModioAPI.Me.GetUserGamesFilter filter)
			{
				ModioAPI.Me.<GetUserGames>d__7 <GetUserGames>d__;
				<GetUserGames>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<GameObject[]>?>>.Create();
				<GetUserGames>d__.filter = filter;
				<GetUserGames>d__.<>1__state = -1;
				<GetUserGames>d__.<>t__builder.Start<ModioAPI.Me.<GetUserGames>d__7>(ref <GetUserGames>d__);
				return <GetUserGames>d__.<>t__builder.Task;
			}

			public static ModioAPI.Me.GetUserGamesFilter FilterGetUserGames(int pageIndex = 0, int pageSize = 100)
			{
				return new ModioAPI.Me.GetUserGamesFilter(pageIndex, pageSize);
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modfileObjects"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetUserModfilesAsJToken(ModioAPI.Me.GetUserModfilesFilter filter)
			{
				ModioAPI.Me.<GetUserModfilesAsJToken>d__10 <GetUserModfilesAsJToken>d__;
				<GetUserModfilesAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetUserModfilesAsJToken>d__.filter = filter;
				<GetUserModfilesAsJToken>d__.<>1__state = -1;
				<GetUserModfilesAsJToken>d__.<>t__builder.Start<ModioAPI.Me.<GetUserModfilesAsJToken>d__10>(ref <GetUserModfilesAsJToken>d__);
				return <GetUserModfilesAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modfileObjects"
			})]
			internal static Task<ValueTuple<Error, Pagination<ModfileObject[]>?>> GetUserModfiles(ModioAPI.Me.GetUserModfilesFilter filter)
			{
				ModioAPI.Me.<GetUserModfiles>d__11 <GetUserModfiles>d__;
				<GetUserModfiles>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<ModfileObject[]>?>>.Create();
				<GetUserModfiles>d__.filter = filter;
				<GetUserModfiles>d__.<>1__state = -1;
				<GetUserModfiles>d__.<>t__builder.Start<ModioAPI.Me.<GetUserModfiles>d__11>(ref <GetUserModfiles>d__);
				return <GetUserModfiles>d__.<>t__builder.Task;
			}

			public static ModioAPI.Me.GetUserModfilesFilter FilterGetUserModfiles(int pageIndex = 0, int pageSize = 100)
			{
				return new ModioAPI.Me.GetUserModfilesFilter(pageIndex, pageSize);
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modObjects"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetUserModsAsJToken(ModioAPI.Me.GetUserModsFilter filter)
			{
				ModioAPI.Me.<GetUserModsAsJToken>d__14 <GetUserModsAsJToken>d__;
				<GetUserModsAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetUserModsAsJToken>d__.filter = filter;
				<GetUserModsAsJToken>d__.<>1__state = -1;
				<GetUserModsAsJToken>d__.<>t__builder.Start<ModioAPI.Me.<GetUserModsAsJToken>d__14>(ref <GetUserModsAsJToken>d__);
				return <GetUserModsAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modObjects"
			})]
			internal static Task<ValueTuple<Error, Pagination<ModObject[]>?>> GetUserMods(ModioAPI.Me.GetUserModsFilter filter)
			{
				ModioAPI.Me.<GetUserMods>d__15 <GetUserMods>d__;
				<GetUserMods>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<ModObject[]>?>>.Create();
				<GetUserMods>d__.filter = filter;
				<GetUserMods>d__.<>1__state = -1;
				<GetUserMods>d__.<>t__builder.Start<ModioAPI.Me.<GetUserMods>d__15>(ref <GetUserMods>d__);
				return <GetUserMods>d__.<>t__builder.Task;
			}

			public static ModioAPI.Me.GetUserModsFilter FilterGetUserMods(int pageIndex = 0, int pageSize = 100)
			{
				return new ModioAPI.Me.GetUserModsFilter(pageIndex, pageSize);
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modObjects"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetUserPurchasesAsJToken(ModioAPI.Me.GetUserPurchasesFilter filter)
			{
				ModioAPI.Me.<GetUserPurchasesAsJToken>d__18 <GetUserPurchasesAsJToken>d__;
				<GetUserPurchasesAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetUserPurchasesAsJToken>d__.filter = filter;
				<GetUserPurchasesAsJToken>d__.<>1__state = -1;
				<GetUserPurchasesAsJToken>d__.<>t__builder.Start<ModioAPI.Me.<GetUserPurchasesAsJToken>d__18>(ref <GetUserPurchasesAsJToken>d__);
				return <GetUserPurchasesAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modObjects"
			})]
			internal static Task<ValueTuple<Error, Pagination<ModObject[]>?>> GetUserPurchases(ModioAPI.Me.GetUserPurchasesFilter filter)
			{
				ModioAPI.Me.<GetUserPurchases>d__19 <GetUserPurchases>d__;
				<GetUserPurchases>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<ModObject[]>?>>.Create();
				<GetUserPurchases>d__.filter = filter;
				<GetUserPurchases>d__.<>1__state = -1;
				<GetUserPurchases>d__.<>t__builder.Start<ModioAPI.Me.<GetUserPurchases>d__19>(ref <GetUserPurchases>d__);
				return <GetUserPurchases>d__.<>t__builder.Task;
			}

			public static ModioAPI.Me.GetUserPurchasesFilter FilterGetUserPurchases(int pageIndex = 0, int pageSize = 100)
			{
				return new ModioAPI.Me.GetUserPurchasesFilter(pageIndex, pageSize);
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"ratingObjects"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetUserRatingsAsJToken(ModioAPI.Me.GetUserRatingsFilter filter)
			{
				ModioAPI.Me.<GetUserRatingsAsJToken>d__22 <GetUserRatingsAsJToken>d__;
				<GetUserRatingsAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetUserRatingsAsJToken>d__.filter = filter;
				<GetUserRatingsAsJToken>d__.<>1__state = -1;
				<GetUserRatingsAsJToken>d__.<>t__builder.Start<ModioAPI.Me.<GetUserRatingsAsJToken>d__22>(ref <GetUserRatingsAsJToken>d__);
				return <GetUserRatingsAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"ratingObjects"
			})]
			internal static Task<ValueTuple<Error, Pagination<RatingObject[]>?>> GetUserRatings(ModioAPI.Me.GetUserRatingsFilter filter)
			{
				ModioAPI.Me.<GetUserRatings>d__23 <GetUserRatings>d__;
				<GetUserRatings>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<RatingObject[]>?>>.Create();
				<GetUserRatings>d__.filter = filter;
				<GetUserRatings>d__.<>1__state = -1;
				<GetUserRatings>d__.<>t__builder.Start<ModioAPI.Me.<GetUserRatings>d__23>(ref <GetUserRatings>d__);
				return <GetUserRatings>d__.<>t__builder.Task;
			}

			public static ModioAPI.Me.GetUserRatingsFilter FilterGetUserRatings(int pageIndex = 0, int pageSize = 100)
			{
				return new ModioAPI.Me.GetUserRatingsFilter(pageIndex, pageSize);
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"userObjects"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetUsersMutedAsJToken()
			{
				ModioAPI.Me.<GetUsersMutedAsJToken>d__26 <GetUsersMutedAsJToken>d__;
				<GetUsersMutedAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetUsersMutedAsJToken>d__.<>1__state = -1;
				<GetUsersMutedAsJToken>d__.<>t__builder.Start<ModioAPI.Me.<GetUsersMutedAsJToken>d__26>(ref <GetUsersMutedAsJToken>d__);
				return <GetUsersMutedAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"userObjects"
			})]
			internal static Task<ValueTuple<Error, Pagination<UserObject[]>?>> GetUsersMuted()
			{
				ModioAPI.Me.<GetUsersMuted>d__27 <GetUsersMuted>d__;
				<GetUsersMuted>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<UserObject[]>?>>.Create();
				<GetUsersMuted>d__.<>1__state = -1;
				<GetUsersMuted>d__.<>t__builder.Start<ModioAPI.Me.<GetUsersMuted>d__27>(ref <GetUsersMuted>d__);
				return <GetUsersMuted>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modObjects"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetUserSubscriptionsAsJToken(ModioAPI.Me.GetUserSubscriptionsFilter filter)
			{
				ModioAPI.Me.<GetUserSubscriptionsAsJToken>d__28 <GetUserSubscriptionsAsJToken>d__;
				<GetUserSubscriptionsAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetUserSubscriptionsAsJToken>d__.filter = filter;
				<GetUserSubscriptionsAsJToken>d__.<>1__state = -1;
				<GetUserSubscriptionsAsJToken>d__.<>t__builder.Start<ModioAPI.Me.<GetUserSubscriptionsAsJToken>d__28>(ref <GetUserSubscriptionsAsJToken>d__);
				return <GetUserSubscriptionsAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modObjects"
			})]
			internal static Task<ValueTuple<Error, Pagination<ModObject[]>?>> GetUserSubscriptions(ModioAPI.Me.GetUserSubscriptionsFilter filter)
			{
				ModioAPI.Me.<GetUserSubscriptions>d__29 <GetUserSubscriptions>d__;
				<GetUserSubscriptions>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<ModObject[]>?>>.Create();
				<GetUserSubscriptions>d__.filter = filter;
				<GetUserSubscriptions>d__.<>1__state = -1;
				<GetUserSubscriptions>d__.<>t__builder.Start<ModioAPI.Me.<GetUserSubscriptions>d__29>(ref <GetUserSubscriptions>d__);
				return <GetUserSubscriptions>d__.<>t__builder.Task;
			}

			public static ModioAPI.Me.GetUserSubscriptionsFilter FilterGetUserSubscriptions(int pageIndex = 0, int pageSize = 100)
			{
				return new ModioAPI.Me.GetUserSubscriptionsFilter(pageIndex, pageSize);
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"walletObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetUserWalletAsJToken(long gameId)
			{
				ModioAPI.Me.<GetUserWalletAsJToken>d__32 <GetUserWalletAsJToken>d__;
				<GetUserWalletAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetUserWalletAsJToken>d__.gameId = gameId;
				<GetUserWalletAsJToken>d__.<>1__state = -1;
				<GetUserWalletAsJToken>d__.<>t__builder.Start<ModioAPI.Me.<GetUserWalletAsJToken>d__32>(ref <GetUserWalletAsJToken>d__);
				return <GetUserWalletAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"walletObject"
			})]
			internal static Task<ValueTuple<Error, WalletObject?>> GetUserWallet(long gameId)
			{
				ModioAPI.Me.<GetUserWallet>d__33 <GetUserWallet>d__;
				<GetUserWallet>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, WalletObject?>>.Create();
				<GetUserWallet>d__.gameId = gameId;
				<GetUserWallet>d__.<>1__state = -1;
				<GetUserWallet>d__.<>t__builder.Start<ModioAPI.Me.<GetUserWallet>d__33>(ref <GetUserWallet>d__);
				return <GetUserWallet>d__.<>t__builder.Task;
			}

			public class GetUserEventsFilter : SearchFilter<ModioAPI.Me.GetUserEventsFilter>
			{
				internal GetUserEventsFilter(int pageIndex, int pageSize) : base(pageIndex, pageSize)
				{
				}

				public ModioAPI.Me.GetUserEventsFilter Id(long id, Filtering condition = Filtering.None)
				{
					this.Parameters["id" + condition.ClearText()] = id;
					return this;
				}

				public ModioAPI.Me.GetUserEventsFilter Id(ICollection<long> id, Filtering condition = Filtering.None)
				{
					this.Parameters["id" + condition.ClearText()] = id;
					return this;
				}

				public ModioAPI.Me.GetUserEventsFilter GameId(long gameId, Filtering condition = Filtering.None)
				{
					this.Parameters["game_id" + condition.ClearText()] = gameId;
					return this;
				}

				public ModioAPI.Me.GetUserEventsFilter GameId(ICollection<long> gameId, Filtering condition = Filtering.None)
				{
					this.Parameters["game_id" + condition.ClearText()] = gameId;
					return this;
				}

				public ModioAPI.Me.GetUserEventsFilter ModId(long modId, Filtering condition = Filtering.None)
				{
					this.Parameters["mod_id" + condition.ClearText()] = modId;
					return this;
				}

				public ModioAPI.Me.GetUserEventsFilter ModId(ICollection<long> modId, Filtering condition = Filtering.None)
				{
					this.Parameters["mod_id" + condition.ClearText()] = modId;
					return this;
				}

				public ModioAPI.Me.GetUserEventsFilter UserId(long userId, Filtering condition = Filtering.None)
				{
					this.Parameters["user_id" + condition.ClearText()] = userId;
					return this;
				}

				public ModioAPI.Me.GetUserEventsFilter UserId(ICollection<long> userId, Filtering condition = Filtering.None)
				{
					this.Parameters["user_id" + condition.ClearText()] = userId;
					return this;
				}

				public ModioAPI.Me.GetUserEventsFilter DateAdded(long dateAdded, Filtering condition = Filtering.None)
				{
					this.Parameters["date_added" + condition.ClearText()] = dateAdded;
					return this;
				}

				public ModioAPI.Me.GetUserEventsFilter DateAdded(ICollection<long> dateAdded, Filtering condition = Filtering.None)
				{
					this.Parameters["date_added" + condition.ClearText()] = dateAdded;
					return this;
				}

				public ModioAPI.Me.GetUserEventsFilter EventType(string eventType, Filtering condition = Filtering.None)
				{
					this.Parameters["event_type" + condition.ClearText()] = eventType;
					return this;
				}

				public ModioAPI.Me.GetUserEventsFilter EventType(ICollection<string> eventType, Filtering condition = Filtering.None)
				{
					this.Parameters["event_type" + condition.ClearText()] = eventType;
					return this;
				}
			}

			public class GetUserGamesFilter : SearchFilter<ModioAPI.Me.GetUserGamesFilter>
			{
				internal GetUserGamesFilter(int pageIndex, int pageSize) : base(pageIndex, pageSize)
				{
				}

				public ModioAPI.Me.GetUserGamesFilter Id(long id, Filtering condition = Filtering.None)
				{
					this.Parameters["id" + condition.ClearText()] = id;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter Id(ICollection<long> id, Filtering condition = Filtering.None)
				{
					this.Parameters["id" + condition.ClearText()] = id;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter Status(long status, Filtering condition = Filtering.None)
				{
					this.Parameters["status" + condition.ClearText()] = status;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter Status(ICollection<long> status, Filtering condition = Filtering.None)
				{
					this.Parameters["status" + condition.ClearText()] = status;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter SubmittedBy(long submittedBy, Filtering condition = Filtering.None)
				{
					this.Parameters["submitted_by" + condition.ClearText()] = submittedBy;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter SubmittedBy(ICollection<long> submittedBy, Filtering condition = Filtering.None)
				{
					this.Parameters["submitted_by" + condition.ClearText()] = submittedBy;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter DateAdded(long dateAdded, Filtering condition = Filtering.None)
				{
					this.Parameters["date_added" + condition.ClearText()] = dateAdded;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter DateAdded(ICollection<long> dateAdded, Filtering condition = Filtering.None)
				{
					this.Parameters["date_added" + condition.ClearText()] = dateAdded;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter DateUpdated(long dateUpdated, Filtering condition = Filtering.None)
				{
					this.Parameters["date_updated" + condition.ClearText()] = dateUpdated;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter DateUpdated(ICollection<long> dateUpdated, Filtering condition = Filtering.None)
				{
					this.Parameters["date_updated" + condition.ClearText()] = dateUpdated;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter DateLive(long dateLive, Filtering condition = Filtering.None)
				{
					this.Parameters["date_live" + condition.ClearText()] = dateLive;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter DateLive(ICollection<long> dateLive, Filtering condition = Filtering.None)
				{
					this.Parameters["date_live" + condition.ClearText()] = dateLive;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter Name(string name, Filtering condition = Filtering.None)
				{
					this.Parameters["name" + condition.ClearText()] = name;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter Name(ICollection<string> name, Filtering condition = Filtering.None)
				{
					this.Parameters["name" + condition.ClearText()] = name;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter NameId(string nameId, Filtering condition = Filtering.None)
				{
					this.Parameters["name_id" + condition.ClearText()] = nameId;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter NameId(ICollection<string> nameId, Filtering condition = Filtering.None)
				{
					this.Parameters["name_id" + condition.ClearText()] = nameId;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter Summary(string summary, Filtering condition = Filtering.None)
				{
					this.Parameters["summary" + condition.ClearText()] = summary;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter Summary(ICollection<string> summary, Filtering condition = Filtering.None)
				{
					this.Parameters["summary" + condition.ClearText()] = summary;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter InstructionsUrl(string instructionsUrl, Filtering condition = Filtering.None)
				{
					this.Parameters["instructions_url" + condition.ClearText()] = instructionsUrl;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter InstructionsUrl(ICollection<string> instructionsUrl, Filtering condition = Filtering.None)
				{
					this.Parameters["instructions_url" + condition.ClearText()] = instructionsUrl;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter UgcName(string ugcName, Filtering condition = Filtering.None)
				{
					this.Parameters["ugc_name" + condition.ClearText()] = ugcName;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter UgcName(ICollection<string> ugcName, Filtering condition = Filtering.None)
				{
					this.Parameters["ugc_name" + condition.ClearText()] = ugcName;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter PresentationOption(long presentationOption, Filtering condition = Filtering.None)
				{
					this.Parameters["presentation_option" + condition.ClearText()] = presentationOption;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter PresentationOption(ICollection<long> presentationOption, Filtering condition = Filtering.None)
				{
					this.Parameters["presentation_option" + condition.ClearText()] = presentationOption;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter SubmissionOption(long submissionOption, Filtering condition = Filtering.None)
				{
					this.Parameters["submission_option" + condition.ClearText()] = submissionOption;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter SubmissionOption(ICollection<long> submissionOption, Filtering condition = Filtering.None)
				{
					this.Parameters["submission_option" + condition.ClearText()] = submissionOption;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter CurationOption(long curationOption, Filtering condition = Filtering.None)
				{
					this.Parameters["curation_option" + condition.ClearText()] = curationOption;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter CurationOption(ICollection<long> curationOption, Filtering condition = Filtering.None)
				{
					this.Parameters["curation_option" + condition.ClearText()] = curationOption;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter DependencyOption(long dependencyOption, Filtering condition = Filtering.None)
				{
					this.Parameters["dependency_option" + condition.ClearText()] = dependencyOption;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter DependencyOption(ICollection<long> dependencyOption, Filtering condition = Filtering.None)
				{
					this.Parameters["dependency_option" + condition.ClearText()] = dependencyOption;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter CommunityOptions(long communityOptions, Filtering condition = Filtering.None)
				{
					this.Parameters["community_options" + condition.ClearText()] = communityOptions;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter CommunityOptions(ICollection<long> communityOptions, Filtering condition = Filtering.None)
				{
					this.Parameters["community_options" + condition.ClearText()] = communityOptions;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter MonetizationOptions(long monetizationOptions, Filtering condition = Filtering.None)
				{
					this.Parameters["monetization_options" + condition.ClearText()] = monetizationOptions;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter MonetizationOptions(ICollection<long> monetizationOptions, Filtering condition = Filtering.None)
				{
					this.Parameters["monetization_options" + condition.ClearText()] = monetizationOptions;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter ApiAccessOptions(long apiAccessOptions, Filtering condition = Filtering.None)
				{
					this.Parameters["api_access_options" + condition.ClearText()] = apiAccessOptions;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter ApiAccessOptions(ICollection<long> apiAccessOptions, Filtering condition = Filtering.None)
				{
					this.Parameters["api_access_options" + condition.ClearText()] = apiAccessOptions;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter MaturityOptions(long maturityOptions, Filtering condition = Filtering.None)
				{
					this.Parameters["maturity_options" + condition.ClearText()] = maturityOptions;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter MaturityOptions(ICollection<long> maturityOptions, Filtering condition = Filtering.None)
				{
					this.Parameters["maturity_options" + condition.ClearText()] = maturityOptions;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter ShowHiddenTags(bool showHiddenTags, Filtering condition = Filtering.None)
				{
					this.Parameters["show_hidden_tags" + condition.ClearText()] = showHiddenTags;
					return this;
				}

				public ModioAPI.Me.GetUserGamesFilter ShowHiddenTags(ICollection<bool> showHiddenTags, Filtering condition = Filtering.None)
				{
					this.Parameters["show_hidden_tags" + condition.ClearText()] = showHiddenTags;
					return this;
				}
			}

			public class GetUserModfilesFilter : SearchFilter<ModioAPI.Me.GetUserModfilesFilter>
			{
				internal GetUserModfilesFilter(int pageIndex, int pageSize) : base(pageIndex, pageSize)
				{
				}

				public ModioAPI.Me.GetUserModfilesFilter Id(long id, Filtering condition = Filtering.None)
				{
					this.Parameters["id" + condition.ClearText()] = id;
					return this;
				}

				public ModioAPI.Me.GetUserModfilesFilter Id(ICollection<long> id, Filtering condition = Filtering.None)
				{
					this.Parameters["id" + condition.ClearText()] = id;
					return this;
				}

				public ModioAPI.Me.GetUserModfilesFilter ModId(long modId, Filtering condition = Filtering.None)
				{
					this.Parameters["mod_id" + condition.ClearText()] = modId;
					return this;
				}

				public ModioAPI.Me.GetUserModfilesFilter ModId(ICollection<long> modId, Filtering condition = Filtering.None)
				{
					this.Parameters["mod_id" + condition.ClearText()] = modId;
					return this;
				}

				public ModioAPI.Me.GetUserModfilesFilter DateAdded(long dateAdded, Filtering condition = Filtering.None)
				{
					this.Parameters["date_added" + condition.ClearText()] = dateAdded;
					return this;
				}

				public ModioAPI.Me.GetUserModfilesFilter DateAdded(ICollection<long> dateAdded, Filtering condition = Filtering.None)
				{
					this.Parameters["date_added" + condition.ClearText()] = dateAdded;
					return this;
				}

				public ModioAPI.Me.GetUserModfilesFilter DateScanned(long dateScanned, Filtering condition = Filtering.None)
				{
					this.Parameters["date_scanned" + condition.ClearText()] = dateScanned;
					return this;
				}

				public ModioAPI.Me.GetUserModfilesFilter DateScanned(ICollection<long> dateScanned, Filtering condition = Filtering.None)
				{
					this.Parameters["date_scanned" + condition.ClearText()] = dateScanned;
					return this;
				}

				public ModioAPI.Me.GetUserModfilesFilter VirusStatus(long virusStatus, Filtering condition = Filtering.None)
				{
					this.Parameters["virus_status" + condition.ClearText()] = virusStatus;
					return this;
				}

				public ModioAPI.Me.GetUserModfilesFilter VirusStatus(ICollection<long> virusStatus, Filtering condition = Filtering.None)
				{
					this.Parameters["virus_status" + condition.ClearText()] = virusStatus;
					return this;
				}

				public ModioAPI.Me.GetUserModfilesFilter VirusPositive(long virusPositive, Filtering condition = Filtering.None)
				{
					this.Parameters["virus_positive" + condition.ClearText()] = virusPositive;
					return this;
				}

				public ModioAPI.Me.GetUserModfilesFilter VirusPositive(ICollection<long> virusPositive, Filtering condition = Filtering.None)
				{
					this.Parameters["virus_positive" + condition.ClearText()] = virusPositive;
					return this;
				}

				public ModioAPI.Me.GetUserModfilesFilter Filesize(long filesize, Filtering condition = Filtering.None)
				{
					this.Parameters["filesize" + condition.ClearText()] = filesize;
					return this;
				}

				public ModioAPI.Me.GetUserModfilesFilter Filesize(ICollection<long> filesize, Filtering condition = Filtering.None)
				{
					this.Parameters["filesize" + condition.ClearText()] = filesize;
					return this;
				}

				public ModioAPI.Me.GetUserModfilesFilter Filehash(string filehash, Filtering condition = Filtering.None)
				{
					this.Parameters["filehash" + condition.ClearText()] = filehash;
					return this;
				}

				public ModioAPI.Me.GetUserModfilesFilter Filehash(ICollection<string> filehash, Filtering condition = Filtering.None)
				{
					this.Parameters["filehash" + condition.ClearText()] = filehash;
					return this;
				}

				public ModioAPI.Me.GetUserModfilesFilter Filename(string filename, Filtering condition = Filtering.None)
				{
					this.Parameters["filename" + condition.ClearText()] = filename;
					return this;
				}

				public ModioAPI.Me.GetUserModfilesFilter Filename(ICollection<string> filename, Filtering condition = Filtering.None)
				{
					this.Parameters["filename" + condition.ClearText()] = filename;
					return this;
				}

				public ModioAPI.Me.GetUserModfilesFilter Version(string version, Filtering condition = Filtering.None)
				{
					this.Parameters["version" + condition.ClearText()] = version;
					return this;
				}

				public ModioAPI.Me.GetUserModfilesFilter Version(ICollection<string> version, Filtering condition = Filtering.None)
				{
					this.Parameters["version" + condition.ClearText()] = version;
					return this;
				}

				public ModioAPI.Me.GetUserModfilesFilter Changelog(string changelog, Filtering condition = Filtering.None)
				{
					this.Parameters["changelog" + condition.ClearText()] = changelog;
					return this;
				}

				public ModioAPI.Me.GetUserModfilesFilter Changelog(ICollection<string> changelog, Filtering condition = Filtering.None)
				{
					this.Parameters["changelog" + condition.ClearText()] = changelog;
					return this;
				}

				public ModioAPI.Me.GetUserModfilesFilter MetadataBlob(string metadataBlob, Filtering condition = Filtering.None)
				{
					this.Parameters["metadata_blob" + condition.ClearText()] = metadataBlob;
					return this;
				}

				public ModioAPI.Me.GetUserModfilesFilter MetadataBlob(ICollection<string> metadataBlob, Filtering condition = Filtering.None)
				{
					this.Parameters["metadata_blob" + condition.ClearText()] = metadataBlob;
					return this;
				}

				public ModioAPI.Me.GetUserModfilesFilter PlatformStatus(string platformStatus, Filtering condition = Filtering.None)
				{
					this.Parameters["platform_status" + condition.ClearText()] = platformStatus;
					return this;
				}

				public ModioAPI.Me.GetUserModfilesFilter PlatformStatus(ICollection<string> platformStatus, Filtering condition = Filtering.None)
				{
					this.Parameters["platform_status" + condition.ClearText()] = platformStatus;
					return this;
				}
			}

			public class GetUserModsFilter : SearchFilter<ModioAPI.Me.GetUserModsFilter>
			{
				internal GetUserModsFilter(int pageIndex, int pageSize) : base(pageIndex, pageSize)
				{
				}

				public ModioAPI.Me.GetUserModsFilter Id(long id, Filtering condition = Filtering.None)
				{
					this.Parameters["id" + condition.ClearText()] = id;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter Id(ICollection<long> id, Filtering condition = Filtering.None)
				{
					this.Parameters["id" + condition.ClearText()] = id;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter GameId(long gameId, Filtering condition = Filtering.None)
				{
					this.Parameters["game_id" + condition.ClearText()] = gameId;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter GameId(ICollection<long> gameId, Filtering condition = Filtering.None)
				{
					this.Parameters["game_id" + condition.ClearText()] = gameId;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter Status(long status, Filtering condition = Filtering.None)
				{
					this.Parameters["status" + condition.ClearText()] = status;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter Status(ICollection<long> status, Filtering condition = Filtering.None)
				{
					this.Parameters["status" + condition.ClearText()] = status;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter Visible(long visible, Filtering condition = Filtering.None)
				{
					this.Parameters["visible" + condition.ClearText()] = visible;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter Visible(ICollection<long> visible, Filtering condition = Filtering.None)
				{
					this.Parameters["visible" + condition.ClearText()] = visible;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter SubmittedBy(long submittedBy, Filtering condition = Filtering.None)
				{
					this.Parameters["submitted_by" + condition.ClearText()] = submittedBy;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter SubmittedBy(ICollection<long> submittedBy, Filtering condition = Filtering.None)
				{
					this.Parameters["submitted_by" + condition.ClearText()] = submittedBy;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter DateAdded(long dateAdded, Filtering condition = Filtering.None)
				{
					this.Parameters["date_added" + condition.ClearText()] = dateAdded;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter DateAdded(ICollection<long> dateAdded, Filtering condition = Filtering.None)
				{
					this.Parameters["date_added" + condition.ClearText()] = dateAdded;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter DateUpdated(long dateUpdated, Filtering condition = Filtering.None)
				{
					this.Parameters["date_updated" + condition.ClearText()] = dateUpdated;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter DateUpdated(ICollection<long> dateUpdated, Filtering condition = Filtering.None)
				{
					this.Parameters["date_updated" + condition.ClearText()] = dateUpdated;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter DateLive(long dateLive, Filtering condition = Filtering.None)
				{
					this.Parameters["date_live" + condition.ClearText()] = dateLive;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter DateLive(ICollection<long> dateLive, Filtering condition = Filtering.None)
				{
					this.Parameters["date_live" + condition.ClearText()] = dateLive;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter Name(string name, Filtering condition = Filtering.None)
				{
					this.Parameters["name" + condition.ClearText()] = name;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter Name(ICollection<string> name, Filtering condition = Filtering.None)
				{
					this.Parameters["name" + condition.ClearText()] = name;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter NameId(string nameId, Filtering condition = Filtering.None)
				{
					this.Parameters["name_id" + condition.ClearText()] = nameId;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter NameId(ICollection<string> nameId, Filtering condition = Filtering.None)
				{
					this.Parameters["name_id" + condition.ClearText()] = nameId;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter Modfile(long modfile, Filtering condition = Filtering.None)
				{
					this.Parameters["modfile" + condition.ClearText()] = modfile;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter Modfile(ICollection<long> modfile, Filtering condition = Filtering.None)
				{
					this.Parameters["modfile" + condition.ClearText()] = modfile;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter MetadataKvp(string metadataKvp, Filtering condition = Filtering.None)
				{
					this.Parameters["metadata_kvp" + condition.ClearText()] = metadataKvp;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter MetadataKvp(ICollection<string> metadataKvp, Filtering condition = Filtering.None)
				{
					this.Parameters["metadata_kvp" + condition.ClearText()] = metadataKvp;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter MetadataBlob(string metadataBlob, Filtering condition = Filtering.None)
				{
					this.Parameters["metadata_blob" + condition.ClearText()] = metadataBlob;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter MetadataBlob(ICollection<string> metadataBlob, Filtering condition = Filtering.None)
				{
					this.Parameters["metadata_blob" + condition.ClearText()] = metadataBlob;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter Tags(string tags, Filtering condition = Filtering.None)
				{
					this.Parameters["tags" + condition.ClearText()] = tags;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter Tags(ICollection<string> tags, Filtering condition = Filtering.None)
				{
					this.Parameters["tags" + condition.ClearText()] = tags;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter MaturityOption(long maturityOption, Filtering condition = Filtering.None)
				{
					this.Parameters["maturity_option" + condition.ClearText()] = maturityOption;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter MaturityOption(ICollection<long> maturityOption, Filtering condition = Filtering.None)
				{
					this.Parameters["maturity_option" + condition.ClearText()] = maturityOption;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter MonetizationOptions(long monetizationOptions, Filtering condition = Filtering.None)
				{
					this.Parameters["monetization_options" + condition.ClearText()] = monetizationOptions;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter MonetizationOptions(ICollection<long> monetizationOptions, Filtering condition = Filtering.None)
				{
					this.Parameters["monetization_options" + condition.ClearText()] = monetizationOptions;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter PlatformStatus(string platformStatus, Filtering condition = Filtering.None)
				{
					this.Parameters["platform_status" + condition.ClearText()] = platformStatus;
					return this;
				}

				public ModioAPI.Me.GetUserModsFilter PlatformStatus(ICollection<string> platformStatus, Filtering condition = Filtering.None)
				{
					this.Parameters["platform_status" + condition.ClearText()] = platformStatus;
					return this;
				}
			}

			public class GetUserPurchasesFilter : SearchFilter<ModioAPI.Me.GetUserPurchasesFilter>
			{
				internal GetUserPurchasesFilter(int pageIndex, int pageSize) : base(pageIndex, pageSize)
				{
				}

				public ModioAPI.Me.GetUserPurchasesFilter Id(long id, Filtering condition = Filtering.None)
				{
					this.Parameters["id" + condition.ClearText()] = id;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter Id(ICollection<long> id, Filtering condition = Filtering.None)
				{
					this.Parameters["id" + condition.ClearText()] = id;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter GameId(long gameId, Filtering condition = Filtering.None)
				{
					this.Parameters["game_id" + condition.ClearText()] = gameId;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter GameId(ICollection<long> gameId, Filtering condition = Filtering.None)
				{
					this.Parameters["game_id" + condition.ClearText()] = gameId;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter Status(long status, Filtering condition = Filtering.None)
				{
					this.Parameters["status" + condition.ClearText()] = status;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter Status(ICollection<long> status, Filtering condition = Filtering.None)
				{
					this.Parameters["status" + condition.ClearText()] = status;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter Visible(long visible, Filtering condition = Filtering.None)
				{
					this.Parameters["visible" + condition.ClearText()] = visible;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter Visible(ICollection<long> visible, Filtering condition = Filtering.None)
				{
					this.Parameters["visible" + condition.ClearText()] = visible;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter SubmittedBy(long submittedBy, Filtering condition = Filtering.None)
				{
					this.Parameters["submitted_by" + condition.ClearText()] = submittedBy;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter SubmittedBy(ICollection<long> submittedBy, Filtering condition = Filtering.None)
				{
					this.Parameters["submitted_by" + condition.ClearText()] = submittedBy;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter DateAdded(long dateAdded, Filtering condition = Filtering.None)
				{
					this.Parameters["date_added" + condition.ClearText()] = dateAdded;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter DateAdded(ICollection<long> dateAdded, Filtering condition = Filtering.None)
				{
					this.Parameters["date_added" + condition.ClearText()] = dateAdded;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter DateUpdated(long dateUpdated, Filtering condition = Filtering.None)
				{
					this.Parameters["date_updated" + condition.ClearText()] = dateUpdated;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter DateUpdated(ICollection<long> dateUpdated, Filtering condition = Filtering.None)
				{
					this.Parameters["date_updated" + condition.ClearText()] = dateUpdated;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter DateLive(long dateLive, Filtering condition = Filtering.None)
				{
					this.Parameters["date_live" + condition.ClearText()] = dateLive;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter DateLive(ICollection<long> dateLive, Filtering condition = Filtering.None)
				{
					this.Parameters["date_live" + condition.ClearText()] = dateLive;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter Name(string name, Filtering condition = Filtering.None)
				{
					this.Parameters["name" + condition.ClearText()] = name;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter Name(ICollection<string> name, Filtering condition = Filtering.None)
				{
					this.Parameters["name" + condition.ClearText()] = name;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter NameId(string nameId, Filtering condition = Filtering.None)
				{
					this.Parameters["name_id" + condition.ClearText()] = nameId;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter NameId(ICollection<string> nameId, Filtering condition = Filtering.None)
				{
					this.Parameters["name_id" + condition.ClearText()] = nameId;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter Modfile(long modfile, Filtering condition = Filtering.None)
				{
					this.Parameters["modfile" + condition.ClearText()] = modfile;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter Modfile(ICollection<long> modfile, Filtering condition = Filtering.None)
				{
					this.Parameters["modfile" + condition.ClearText()] = modfile;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter MetadataKvp(string metadataKvp, Filtering condition = Filtering.None)
				{
					this.Parameters["metadata_kvp" + condition.ClearText()] = metadataKvp;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter MetadataKvp(ICollection<string> metadataKvp, Filtering condition = Filtering.None)
				{
					this.Parameters["metadata_kvp" + condition.ClearText()] = metadataKvp;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter MetadataBlob(string metadataBlob, Filtering condition = Filtering.None)
				{
					this.Parameters["metadata_blob" + condition.ClearText()] = metadataBlob;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter MetadataBlob(ICollection<string> metadataBlob, Filtering condition = Filtering.None)
				{
					this.Parameters["metadata_blob" + condition.ClearText()] = metadataBlob;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter Tags(string tags, Filtering condition = Filtering.None)
				{
					this.Parameters["tags" + condition.ClearText()] = tags;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter Tags(ICollection<string> tags, Filtering condition = Filtering.None)
				{
					this.Parameters["tags" + condition.ClearText()] = tags;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter MaturityOption(long maturityOption, Filtering condition = Filtering.None)
				{
					this.Parameters["maturity_option" + condition.ClearText()] = maturityOption;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter MaturityOption(ICollection<long> maturityOption, Filtering condition = Filtering.None)
				{
					this.Parameters["maturity_option" + condition.ClearText()] = maturityOption;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter MonetizationOptions(long monetizationOptions, Filtering condition = Filtering.None)
				{
					this.Parameters["monetization_options" + condition.ClearText()] = monetizationOptions;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter MonetizationOptions(ICollection<long> monetizationOptions, Filtering condition = Filtering.None)
				{
					this.Parameters["monetization_options" + condition.ClearText()] = monetizationOptions;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter PlatformStatus(string platformStatus, Filtering condition = Filtering.None)
				{
					this.Parameters["platform_status" + condition.ClearText()] = platformStatus;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter PlatformStatus(ICollection<string> platformStatus, Filtering condition = Filtering.None)
				{
					this.Parameters["platform_status" + condition.ClearText()] = platformStatus;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter Platforms(string platforms, Filtering condition = Filtering.None)
				{
					this.Parameters["platforms" + condition.ClearText()] = platforms;
					return this;
				}

				public ModioAPI.Me.GetUserPurchasesFilter Platforms(ICollection<string> platforms, Filtering condition = Filtering.None)
				{
					this.Parameters["platforms" + condition.ClearText()] = platforms;
					return this;
				}
			}

			public class GetUserRatingsFilter : SearchFilter<ModioAPI.Me.GetUserRatingsFilter>
			{
				internal GetUserRatingsFilter(int pageIndex, int pageSize) : base(pageIndex, pageSize)
				{
				}

				public ModioAPI.Me.GetUserRatingsFilter GameId(long gameId, Filtering condition = Filtering.None)
				{
					this.Parameters["game_id" + condition.ClearText()] = gameId;
					return this;
				}

				public ModioAPI.Me.GetUserRatingsFilter GameId(ICollection<long> gameId, Filtering condition = Filtering.None)
				{
					this.Parameters["game_id" + condition.ClearText()] = gameId;
					return this;
				}

				public ModioAPI.Me.GetUserRatingsFilter ModId(long modId, Filtering condition = Filtering.None)
				{
					this.Parameters["mod_id" + condition.ClearText()] = modId;
					return this;
				}

				public ModioAPI.Me.GetUserRatingsFilter ModId(ICollection<long> modId, Filtering condition = Filtering.None)
				{
					this.Parameters["mod_id" + condition.ClearText()] = modId;
					return this;
				}

				public ModioAPI.Me.GetUserRatingsFilter Rating(long rating, Filtering condition = Filtering.None)
				{
					this.Parameters["rating" + condition.ClearText()] = rating;
					return this;
				}

				public ModioAPI.Me.GetUserRatingsFilter Rating(ICollection<long> rating, Filtering condition = Filtering.None)
				{
					this.Parameters["rating" + condition.ClearText()] = rating;
					return this;
				}

				public ModioAPI.Me.GetUserRatingsFilter DateAdded(long dateAdded, Filtering condition = Filtering.None)
				{
					this.Parameters["date_added" + condition.ClearText()] = dateAdded;
					return this;
				}

				public ModioAPI.Me.GetUserRatingsFilter DateAdded(ICollection<long> dateAdded, Filtering condition = Filtering.None)
				{
					this.Parameters["date_added" + condition.ClearText()] = dateAdded;
					return this;
				}
			}

			public class GetUserSubscriptionsFilter : SearchFilter<ModioAPI.Me.GetUserSubscriptionsFilter>
			{
				internal GetUserSubscriptionsFilter(int pageIndex, int pageSize) : base(pageIndex, pageSize)
				{
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter Id(long id, Filtering condition = Filtering.None)
				{
					this.Parameters["id" + condition.ClearText()] = id;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter Id(ICollection<long> id, Filtering condition = Filtering.None)
				{
					this.Parameters["id" + condition.ClearText()] = id;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter GameId(long gameId, Filtering condition = Filtering.None)
				{
					this.Parameters["game_id" + condition.ClearText()] = gameId;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter GameId(ICollection<long> gameId, Filtering condition = Filtering.None)
				{
					this.Parameters["game_id" + condition.ClearText()] = gameId;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter Status(long status, Filtering condition = Filtering.None)
				{
					this.Parameters["status" + condition.ClearText()] = status;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter Status(ICollection<long> status, Filtering condition = Filtering.None)
				{
					this.Parameters["status" + condition.ClearText()] = status;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter Visible(long visible, Filtering condition = Filtering.None)
				{
					this.Parameters["visible" + condition.ClearText()] = visible;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter Visible(ICollection<long> visible, Filtering condition = Filtering.None)
				{
					this.Parameters["visible" + condition.ClearText()] = visible;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter SubmittedBy(long submittedBy, Filtering condition = Filtering.None)
				{
					this.Parameters["submitted_by" + condition.ClearText()] = submittedBy;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter SubmittedBy(ICollection<long> submittedBy, Filtering condition = Filtering.None)
				{
					this.Parameters["submitted_by" + condition.ClearText()] = submittedBy;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter DateAdded(long dateAdded, Filtering condition = Filtering.None)
				{
					this.Parameters["date_added" + condition.ClearText()] = dateAdded;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter DateAdded(ICollection<long> dateAdded, Filtering condition = Filtering.None)
				{
					this.Parameters["date_added" + condition.ClearText()] = dateAdded;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter DateUpdated(long dateUpdated, Filtering condition = Filtering.None)
				{
					this.Parameters["date_updated" + condition.ClearText()] = dateUpdated;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter DateUpdated(ICollection<long> dateUpdated, Filtering condition = Filtering.None)
				{
					this.Parameters["date_updated" + condition.ClearText()] = dateUpdated;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter DateLive(long dateLive, Filtering condition = Filtering.None)
				{
					this.Parameters["date_live" + condition.ClearText()] = dateLive;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter DateLive(ICollection<long> dateLive, Filtering condition = Filtering.None)
				{
					this.Parameters["date_live" + condition.ClearText()] = dateLive;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter Name(string name, Filtering condition = Filtering.None)
				{
					this.Parameters["name" + condition.ClearText()] = name;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter Name(ICollection<string> name, Filtering condition = Filtering.None)
				{
					this.Parameters["name" + condition.ClearText()] = name;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter NameId(string nameId, Filtering condition = Filtering.None)
				{
					this.Parameters["name_id" + condition.ClearText()] = nameId;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter NameId(ICollection<string> nameId, Filtering condition = Filtering.None)
				{
					this.Parameters["name_id" + condition.ClearText()] = nameId;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter Modfile(long modfile, Filtering condition = Filtering.None)
				{
					this.Parameters["modfile" + condition.ClearText()] = modfile;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter Modfile(ICollection<long> modfile, Filtering condition = Filtering.None)
				{
					this.Parameters["modfile" + condition.ClearText()] = modfile;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter MetadataKvp(string metadataKvp, Filtering condition = Filtering.None)
				{
					this.Parameters["metadata_kvp" + condition.ClearText()] = metadataKvp;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter MetadataKvp(ICollection<string> metadataKvp, Filtering condition = Filtering.None)
				{
					this.Parameters["metadata_kvp" + condition.ClearText()] = metadataKvp;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter MetadataBlob(string metadataBlob, Filtering condition = Filtering.None)
				{
					this.Parameters["metadata_blob" + condition.ClearText()] = metadataBlob;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter MetadataBlob(ICollection<string> metadataBlob, Filtering condition = Filtering.None)
				{
					this.Parameters["metadata_blob" + condition.ClearText()] = metadataBlob;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter Tags(string tags, Filtering condition = Filtering.None)
				{
					this.Parameters["tags" + condition.ClearText()] = tags;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter Tags(ICollection<string> tags, Filtering condition = Filtering.None)
				{
					this.Parameters["tags" + condition.ClearText()] = tags;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter MaturityOption(long maturityOption, Filtering condition = Filtering.None)
				{
					this.Parameters["maturity_option" + condition.ClearText()] = maturityOption;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter MaturityOption(ICollection<long> maturityOption, Filtering condition = Filtering.None)
				{
					this.Parameters["maturity_option" + condition.ClearText()] = maturityOption;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter MonetizationOptions(long monetizationOptions, Filtering condition = Filtering.None)
				{
					this.Parameters["monetization_options" + condition.ClearText()] = monetizationOptions;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter MonetizationOptions(ICollection<long> monetizationOptions, Filtering condition = Filtering.None)
				{
					this.Parameters["monetization_options" + condition.ClearText()] = monetizationOptions;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter PlatformStatus(string platformStatus, Filtering condition = Filtering.None)
				{
					this.Parameters["platform_status" + condition.ClearText()] = platformStatus;
					return this;
				}

				public ModioAPI.Me.GetUserSubscriptionsFilter PlatformStatus(ICollection<string> platformStatus, Filtering condition = Filtering.None)
				{
					this.Parameters["platform_status" + condition.ClearText()] = platformStatus;
					return this;
				}
			}
		}

		public static class Games
		{
			[return: TupleElementNames(new string[]
			{
				"error",
				"gameObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetGameAsJToken(bool? showHiddenTags = null)
			{
				ModioAPI.Games.<GetGameAsJToken>d__0 <GetGameAsJToken>d__;
				<GetGameAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetGameAsJToken>d__.showHiddenTags = showHiddenTags;
				<GetGameAsJToken>d__.<>1__state = -1;
				<GetGameAsJToken>d__.<>t__builder.Start<ModioAPI.Games.<GetGameAsJToken>d__0>(ref <GetGameAsJToken>d__);
				return <GetGameAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"gameObject"
			})]
			internal static Task<ValueTuple<Error, GameObject?>> GetGame(bool? showHiddenTags = null)
			{
				ModioAPI.Games.<GetGame>d__1 <GetGame>d__;
				<GetGame>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, GameObject?>>.Create();
				<GetGame>d__.showHiddenTags = showHiddenTags;
				<GetGame>d__.<>1__state = -1;
				<GetGame>d__.<>t__builder.Start<ModioAPI.Games.<GetGame>d__1>(ref <GetGame>d__);
				return <GetGame>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"gameObjects"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetGamesAsJToken(ModioAPI.Games.GetGamesFilter filter)
			{
				ModioAPI.Games.<GetGamesAsJToken>d__2 <GetGamesAsJToken>d__;
				<GetGamesAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetGamesAsJToken>d__.filter = filter;
				<GetGamesAsJToken>d__.<>1__state = -1;
				<GetGamesAsJToken>d__.<>t__builder.Start<ModioAPI.Games.<GetGamesAsJToken>d__2>(ref <GetGamesAsJToken>d__);
				return <GetGamesAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"gameObjects"
			})]
			internal static Task<ValueTuple<Error, Pagination<GameObject[]>?>> GetGames(ModioAPI.Games.GetGamesFilter filter)
			{
				ModioAPI.Games.<GetGames>d__3 <GetGames>d__;
				<GetGames>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<GameObject[]>?>>.Create();
				<GetGames>d__.filter = filter;
				<GetGames>d__.<>1__state = -1;
				<GetGames>d__.<>t__builder.Start<ModioAPI.Games.<GetGames>d__3>(ref <GetGames>d__);
				return <GetGames>d__.<>t__builder.Task;
			}

			public static ModioAPI.Games.GetGamesFilter FilterGetGames(int pageIndex = 0, int pageSize = 100)
			{
				return new ModioAPI.Games.GetGamesFilter(pageIndex, pageSize);
			}

			public class GetGamesFilter : SearchFilter<ModioAPI.Games.GetGamesFilter>
			{
				internal GetGamesFilter(int pageIndex, int pageSize) : base(pageIndex, pageSize)
				{
				}

				public ModioAPI.Games.GetGamesFilter Id(long id, Filtering condition = Filtering.None)
				{
					this.Parameters["id" + condition.ClearText()] = id;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter Id(ICollection<long> id, Filtering condition = Filtering.None)
				{
					this.Parameters["id" + condition.ClearText()] = id;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter Status(long status, Filtering condition = Filtering.None)
				{
					this.Parameters["status" + condition.ClearText()] = status;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter Status(ICollection<long> status, Filtering condition = Filtering.None)
				{
					this.Parameters["status" + condition.ClearText()] = status;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter SubmittedBy(long submittedBy, Filtering condition = Filtering.None)
				{
					this.Parameters["submitted_by" + condition.ClearText()] = submittedBy;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter SubmittedBy(ICollection<long> submittedBy, Filtering condition = Filtering.None)
				{
					this.Parameters["submitted_by" + condition.ClearText()] = submittedBy;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter DateAdded(long dateAdded, Filtering condition = Filtering.None)
				{
					this.Parameters["date_added" + condition.ClearText()] = dateAdded;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter DateAdded(ICollection<long> dateAdded, Filtering condition = Filtering.None)
				{
					this.Parameters["date_added" + condition.ClearText()] = dateAdded;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter DateUpdated(long dateUpdated, Filtering condition = Filtering.None)
				{
					this.Parameters["date_updated" + condition.ClearText()] = dateUpdated;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter DateUpdated(ICollection<long> dateUpdated, Filtering condition = Filtering.None)
				{
					this.Parameters["date_updated" + condition.ClearText()] = dateUpdated;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter DateLive(long dateLive, Filtering condition = Filtering.None)
				{
					this.Parameters["date_live" + condition.ClearText()] = dateLive;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter DateLive(ICollection<long> dateLive, Filtering condition = Filtering.None)
				{
					this.Parameters["date_live" + condition.ClearText()] = dateLive;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter Name(string name, Filtering condition = Filtering.None)
				{
					this.Parameters["name" + condition.ClearText()] = name;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter Name(ICollection<string> name, Filtering condition = Filtering.None)
				{
					this.Parameters["name" + condition.ClearText()] = name;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter NameId(string nameId, Filtering condition = Filtering.None)
				{
					this.Parameters["name_id" + condition.ClearText()] = nameId;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter NameId(ICollection<string> nameId, Filtering condition = Filtering.None)
				{
					this.Parameters["name_id" + condition.ClearText()] = nameId;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter Summary(string summary, Filtering condition = Filtering.None)
				{
					this.Parameters["summary" + condition.ClearText()] = summary;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter Summary(ICollection<string> summary, Filtering condition = Filtering.None)
				{
					this.Parameters["summary" + condition.ClearText()] = summary;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter InstructionsUrl(string instructionsUrl, Filtering condition = Filtering.None)
				{
					this.Parameters["instructions_url" + condition.ClearText()] = instructionsUrl;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter InstructionsUrl(ICollection<string> instructionsUrl, Filtering condition = Filtering.None)
				{
					this.Parameters["instructions_url" + condition.ClearText()] = instructionsUrl;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter UgcName(string ugcName, Filtering condition = Filtering.None)
				{
					this.Parameters["ugc_name" + condition.ClearText()] = ugcName;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter UgcName(ICollection<string> ugcName, Filtering condition = Filtering.None)
				{
					this.Parameters["ugc_name" + condition.ClearText()] = ugcName;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter PresentationOption(long presentationOption, Filtering condition = Filtering.None)
				{
					this.Parameters["presentation_option" + condition.ClearText()] = presentationOption;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter PresentationOption(ICollection<long> presentationOption, Filtering condition = Filtering.None)
				{
					this.Parameters["presentation_option" + condition.ClearText()] = presentationOption;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter SubmissionOption(long submissionOption, Filtering condition = Filtering.None)
				{
					this.Parameters["submission_option" + condition.ClearText()] = submissionOption;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter SubmissionOption(ICollection<long> submissionOption, Filtering condition = Filtering.None)
				{
					this.Parameters["submission_option" + condition.ClearText()] = submissionOption;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter CurationOption(long curationOption, Filtering condition = Filtering.None)
				{
					this.Parameters["curation_option" + condition.ClearText()] = curationOption;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter CurationOption(ICollection<long> curationOption, Filtering condition = Filtering.None)
				{
					this.Parameters["curation_option" + condition.ClearText()] = curationOption;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter DependencyOption(long dependencyOption, Filtering condition = Filtering.None)
				{
					this.Parameters["dependency_option" + condition.ClearText()] = dependencyOption;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter DependencyOption(ICollection<long> dependencyOption, Filtering condition = Filtering.None)
				{
					this.Parameters["dependency_option" + condition.ClearText()] = dependencyOption;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter CommunityOptions(long communityOptions, Filtering condition = Filtering.None)
				{
					this.Parameters["community_options" + condition.ClearText()] = communityOptions;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter CommunityOptions(ICollection<long> communityOptions, Filtering condition = Filtering.None)
				{
					this.Parameters["community_options" + condition.ClearText()] = communityOptions;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter MonetizationOptions(long monetizationOptions, Filtering condition = Filtering.None)
				{
					this.Parameters["monetization_options" + condition.ClearText()] = monetizationOptions;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter MonetizationOptions(ICollection<long> monetizationOptions, Filtering condition = Filtering.None)
				{
					this.Parameters["monetization_options" + condition.ClearText()] = monetizationOptions;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter ApiAccessOptions(long apiAccessOptions, Filtering condition = Filtering.None)
				{
					this.Parameters["api_access_options" + condition.ClearText()] = apiAccessOptions;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter ApiAccessOptions(ICollection<long> apiAccessOptions, Filtering condition = Filtering.None)
				{
					this.Parameters["api_access_options" + condition.ClearText()] = apiAccessOptions;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter MaturityOptions(long maturityOptions, Filtering condition = Filtering.None)
				{
					this.Parameters["maturity_options" + condition.ClearText()] = maturityOptions;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter MaturityOptions(ICollection<long> maturityOptions, Filtering condition = Filtering.None)
				{
					this.Parameters["maturity_options" + condition.ClearText()] = maturityOptions;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter ShowHiddenTags(bool showHiddenTags, Filtering condition = Filtering.None)
				{
					this.Parameters["show_hidden_tags" + condition.ClearText()] = showHiddenTags;
					return this;
				}

				public ModioAPI.Games.GetGamesFilter ShowHiddenTags(ICollection<bool> showHiddenTags, Filtering condition = Filtering.None)
				{
					this.Parameters["show_hidden_tags" + condition.ClearText()] = showHiddenTags;
					return this;
				}
			}
		}

		public static class Stats
		{
			[return: TupleElementNames(new string[]
			{
				"error",
				"gameStatsObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetGameStatsAsJToken()
			{
				ModioAPI.Stats.<GetGameStatsAsJToken>d__0 <GetGameStatsAsJToken>d__;
				<GetGameStatsAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetGameStatsAsJToken>d__.<>1__state = -1;
				<GetGameStatsAsJToken>d__.<>t__builder.Start<ModioAPI.Stats.<GetGameStatsAsJToken>d__0>(ref <GetGameStatsAsJToken>d__);
				return <GetGameStatsAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"gameStatsObject"
			})]
			internal static Task<ValueTuple<Error, GameStatsObject?>> GetGameStats()
			{
				ModioAPI.Stats.<GetGameStats>d__1 <GetGameStats>d__;
				<GetGameStats>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, GameStatsObject?>>.Create();
				<GetGameStats>d__.<>1__state = -1;
				<GetGameStats>d__.<>t__builder.Start<ModioAPI.Stats.<GetGameStats>d__1>(ref <GetGameStats>d__);
				return <GetGameStats>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modStatsObjects"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetModsStatsAsJToken(ModioAPI.Stats.GetModsStatsFilter filter)
			{
				ModioAPI.Stats.<GetModsStatsAsJToken>d__2 <GetModsStatsAsJToken>d__;
				<GetModsStatsAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetModsStatsAsJToken>d__.filter = filter;
				<GetModsStatsAsJToken>d__.<>1__state = -1;
				<GetModsStatsAsJToken>d__.<>t__builder.Start<ModioAPI.Stats.<GetModsStatsAsJToken>d__2>(ref <GetModsStatsAsJToken>d__);
				return <GetModsStatsAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modStatsObjects"
			})]
			internal static Task<ValueTuple<Error, Pagination<ModStatsObject[]>?>> GetModsStats(ModioAPI.Stats.GetModsStatsFilter filter)
			{
				ModioAPI.Stats.<GetModsStats>d__3 <GetModsStats>d__;
				<GetModsStats>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<ModStatsObject[]>?>>.Create();
				<GetModsStats>d__.filter = filter;
				<GetModsStats>d__.<>1__state = -1;
				<GetModsStats>d__.<>t__builder.Start<ModioAPI.Stats.<GetModsStats>d__3>(ref <GetModsStats>d__);
				return <GetModsStats>d__.<>t__builder.Task;
			}

			public static ModioAPI.Stats.GetModsStatsFilter FilterGetModsStats(int pageIndex = 0, int pageSize = 100)
			{
				return new ModioAPI.Stats.GetModsStatsFilter(pageIndex, pageSize);
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modStatsObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetModStatsAsJToken(long modId)
			{
				ModioAPI.Stats.<GetModStatsAsJToken>d__6 <GetModStatsAsJToken>d__;
				<GetModStatsAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetModStatsAsJToken>d__.modId = modId;
				<GetModStatsAsJToken>d__.<>1__state = -1;
				<GetModStatsAsJToken>d__.<>t__builder.Start<ModioAPI.Stats.<GetModStatsAsJToken>d__6>(ref <GetModStatsAsJToken>d__);
				return <GetModStatsAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modStatsObject"
			})]
			internal static Task<ValueTuple<Error, ModStatsObject?>> GetModStats(long modId)
			{
				ModioAPI.Stats.<GetModStats>d__7 <GetModStats>d__;
				<GetModStats>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, ModStatsObject?>>.Create();
				<GetModStats>d__.modId = modId;
				<GetModStats>d__.<>1__state = -1;
				<GetModStats>d__.<>t__builder.Start<ModioAPI.Stats.<GetModStats>d__7>(ref <GetModStats>d__);
				return <GetModStats>d__.<>t__builder.Task;
			}

			public class GetModsStatsFilter : SearchFilter<ModioAPI.Stats.GetModsStatsFilter>
			{
				internal GetModsStatsFilter(int pageIndex, int pageSize) : base(pageIndex, pageSize)
				{
				}

				public ModioAPI.Stats.GetModsStatsFilter ModId(long modId, Filtering condition = Filtering.None)
				{
					this.Parameters["mod_id" + condition.ClearText()] = modId;
					return this;
				}

				public ModioAPI.Stats.GetModsStatsFilter ModId(ICollection<long> modId, Filtering condition = Filtering.None)
				{
					this.Parameters["mod_id" + condition.ClearText()] = modId;
					return this;
				}

				public ModioAPI.Stats.GetModsStatsFilter PopularityRankPosition(long popularityRankPosition, Filtering condition = Filtering.None)
				{
					this.Parameters["popularity_rank_position" + condition.ClearText()] = popularityRankPosition;
					return this;
				}

				public ModioAPI.Stats.GetModsStatsFilter PopularityRankPosition(ICollection<long> popularityRankPosition, Filtering condition = Filtering.None)
				{
					this.Parameters["popularity_rank_position" + condition.ClearText()] = popularityRankPosition;
					return this;
				}

				public ModioAPI.Stats.GetModsStatsFilter PopularityRankTotalMods(long popularityRankTotalMods, Filtering condition = Filtering.None)
				{
					this.Parameters["popularity_rank_total_mods" + condition.ClearText()] = popularityRankTotalMods;
					return this;
				}

				public ModioAPI.Stats.GetModsStatsFilter PopularityRankTotalMods(ICollection<long> popularityRankTotalMods, Filtering condition = Filtering.None)
				{
					this.Parameters["popularity_rank_total_mods" + condition.ClearText()] = popularityRankTotalMods;
					return this;
				}

				public ModioAPI.Stats.GetModsStatsFilter DownloadsTotal(long downloadsTotal, Filtering condition = Filtering.None)
				{
					this.Parameters["downloads_total" + condition.ClearText()] = downloadsTotal;
					return this;
				}

				public ModioAPI.Stats.GetModsStatsFilter DownloadsTotal(ICollection<long> downloadsTotal, Filtering condition = Filtering.None)
				{
					this.Parameters["downloads_total" + condition.ClearText()] = downloadsTotal;
					return this;
				}

				public ModioAPI.Stats.GetModsStatsFilter SubscribersTotal(long subscribersTotal, Filtering condition = Filtering.None)
				{
					this.Parameters["subscribers_total" + condition.ClearText()] = subscribersTotal;
					return this;
				}

				public ModioAPI.Stats.GetModsStatsFilter SubscribersTotal(ICollection<long> subscribersTotal, Filtering condition = Filtering.None)
				{
					this.Parameters["subscribers_total" + condition.ClearText()] = subscribersTotal;
					return this;
				}

				public ModioAPI.Stats.GetModsStatsFilter RatingsPositive(long ratingsPositive, Filtering condition = Filtering.None)
				{
					this.Parameters["ratings_positive" + condition.ClearText()] = ratingsPositive;
					return this;
				}

				public ModioAPI.Stats.GetModsStatsFilter RatingsPositive(ICollection<long> ratingsPositive, Filtering condition = Filtering.None)
				{
					this.Parameters["ratings_positive" + condition.ClearText()] = ratingsPositive;
					return this;
				}

				public ModioAPI.Stats.GetModsStatsFilter RatingsNegative(long ratingsNegative, Filtering condition = Filtering.None)
				{
					this.Parameters["ratings_negative" + condition.ClearText()] = ratingsNegative;
					return this;
				}

				public ModioAPI.Stats.GetModsStatsFilter RatingsNegative(ICollection<long> ratingsNegative, Filtering condition = Filtering.None)
				{
					this.Parameters["ratings_negative" + condition.ClearText()] = ratingsNegative;
					return this;
				}
			}
		}

		public static class Events
		{
			[return: TupleElementNames(new string[]
			{
				"error",
				"modEventObjects"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetModEventsAsJToken(long modId, ModioAPI.Events.GetModEventsFilter filter)
			{
				ModioAPI.Events.<GetModEventsAsJToken>d__0 <GetModEventsAsJToken>d__;
				<GetModEventsAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetModEventsAsJToken>d__.modId = modId;
				<GetModEventsAsJToken>d__.<>1__state = -1;
				<GetModEventsAsJToken>d__.<>t__builder.Start<ModioAPI.Events.<GetModEventsAsJToken>d__0>(ref <GetModEventsAsJToken>d__);
				return <GetModEventsAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modEventObjects"
			})]
			internal static Task<ValueTuple<Error, Pagination<ModEventObject[]>?>> GetModEvents(long modId, ModioAPI.Events.GetModEventsFilter filter)
			{
				ModioAPI.Events.<GetModEvents>d__1 <GetModEvents>d__;
				<GetModEvents>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<ModEventObject[]>?>>.Create();
				<GetModEvents>d__.modId = modId;
				<GetModEvents>d__.filter = filter;
				<GetModEvents>d__.<>1__state = -1;
				<GetModEvents>d__.<>t__builder.Start<ModioAPI.Events.<GetModEvents>d__1>(ref <GetModEvents>d__);
				return <GetModEvents>d__.<>t__builder.Task;
			}

			public static ModioAPI.Events.GetModEventsFilter FilterGetModEvents(int pageIndex = 0, int pageSize = 100)
			{
				return new ModioAPI.Events.GetModEventsFilter(pageIndex, pageSize);
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modEventObjects"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetModsEventsAsJToken(ModioAPI.Events.GetModsEventsFilter filter)
			{
				ModioAPI.Events.<GetModsEventsAsJToken>d__4 <GetModsEventsAsJToken>d__;
				<GetModsEventsAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetModsEventsAsJToken>d__.filter = filter;
				<GetModsEventsAsJToken>d__.<>1__state = -1;
				<GetModsEventsAsJToken>d__.<>t__builder.Start<ModioAPI.Events.<GetModsEventsAsJToken>d__4>(ref <GetModsEventsAsJToken>d__);
				return <GetModsEventsAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modEventObjects"
			})]
			internal static Task<ValueTuple<Error, Pagination<ModEventObject[]>?>> GetModsEvents(ModioAPI.Events.GetModsEventsFilter filter)
			{
				ModioAPI.Events.<GetModsEvents>d__5 <GetModsEvents>d__;
				<GetModsEvents>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<ModEventObject[]>?>>.Create();
				<GetModsEvents>d__.filter = filter;
				<GetModsEvents>d__.<>1__state = -1;
				<GetModsEvents>d__.<>t__builder.Start<ModioAPI.Events.<GetModsEvents>d__5>(ref <GetModsEvents>d__);
				return <GetModsEvents>d__.<>t__builder.Task;
			}

			public static ModioAPI.Events.GetModsEventsFilter FilterGetModsEvents(int pageIndex = 0, int pageSize = 100)
			{
				return new ModioAPI.Events.GetModsEventsFilter(pageIndex, pageSize);
			}

			public class GetModEventsFilter : SearchFilter<ModioAPI.Events.GetModEventsFilter>
			{
				internal GetModEventsFilter(int pageIndex, int pageSize) : base(pageIndex, pageSize)
				{
				}
			}

			public class GetModsEventsFilter : SearchFilter<ModioAPI.Events.GetModsEventsFilter>
			{
				internal GetModsEventsFilter(int pageIndex, int pageSize) : base(pageIndex, pageSize)
				{
				}

				public ModioAPI.Events.GetModsEventsFilter Id(long id, Filtering condition = Filtering.None)
				{
					this.Parameters["id" + condition.ClearText()] = id;
					return this;
				}

				public ModioAPI.Events.GetModsEventsFilter Id(ICollection<long> id, Filtering condition = Filtering.None)
				{
					this.Parameters["id" + condition.ClearText()] = id;
					return this;
				}

				public ModioAPI.Events.GetModsEventsFilter ModId(long modId, Filtering condition = Filtering.None)
				{
					this.Parameters["mod_id" + condition.ClearText()] = modId;
					return this;
				}

				public ModioAPI.Events.GetModsEventsFilter ModId(ICollection<long> modId, Filtering condition = Filtering.None)
				{
					this.Parameters["mod_id" + condition.ClearText()] = modId;
					return this;
				}

				public ModioAPI.Events.GetModsEventsFilter UserId(long userId, Filtering condition = Filtering.None)
				{
					this.Parameters["user_id" + condition.ClearText()] = userId;
					return this;
				}

				public ModioAPI.Events.GetModsEventsFilter UserId(ICollection<long> userId, Filtering condition = Filtering.None)
				{
					this.Parameters["user_id" + condition.ClearText()] = userId;
					return this;
				}

				public ModioAPI.Events.GetModsEventsFilter DateAdded(long dateAdded, Filtering condition = Filtering.None)
				{
					this.Parameters["date_added" + condition.ClearText()] = dateAdded;
					return this;
				}

				public ModioAPI.Events.GetModsEventsFilter DateAdded(ICollection<long> dateAdded, Filtering condition = Filtering.None)
				{
					this.Parameters["date_added" + condition.ClearText()] = dateAdded;
					return this;
				}

				public ModioAPI.Events.GetModsEventsFilter EventType(string eventType, Filtering condition = Filtering.None)
				{
					this.Parameters["event_type" + condition.ClearText()] = eventType;
					return this;
				}

				public ModioAPI.Events.GetModsEventsFilter EventType(ICollection<string> eventType, Filtering condition = Filtering.None)
				{
					this.Parameters["event_type" + condition.ClearText()] = eventType;
					return this;
				}

				public ModioAPI.Events.GetModsEventsFilter Latest(bool latest, Filtering condition = Filtering.None)
				{
					this.Parameters["latest" + condition.ClearText()] = latest;
					return this;
				}

				public ModioAPI.Events.GetModsEventsFilter Latest(ICollection<bool> latest, Filtering condition = Filtering.None)
				{
					this.Parameters["latest" + condition.ClearText()] = latest;
					return this;
				}

				public ModioAPI.Events.GetModsEventsFilter Subscribed(bool subscribed, Filtering condition = Filtering.None)
				{
					this.Parameters["subscribed" + condition.ClearText()] = subscribed;
					return this;
				}

				public ModioAPI.Events.GetModsEventsFilter Subscribed(ICollection<bool> subscribed, Filtering condition = Filtering.None)
				{
					this.Parameters["subscribed" + condition.ClearText()] = subscribed;
					return this;
				}
			}
		}

		public static class General
		{
			[return: TupleElementNames(new string[]
			{
				"error",
				"userObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> GetResourceOwnerAsJToken()
			{
				ModioAPI.General.<GetResourceOwnerAsJToken>d__0 <GetResourceOwnerAsJToken>d__;
				<GetResourceOwnerAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<GetResourceOwnerAsJToken>d__.<>1__state = -1;
				<GetResourceOwnerAsJToken>d__.<>t__builder.Start<ModioAPI.General.<GetResourceOwnerAsJToken>d__0>(ref <GetResourceOwnerAsJToken>d__);
				return <GetResourceOwnerAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"userObject"
			})]
			internal static Task<ValueTuple<Error, UserObject?>> GetResourceOwner()
			{
				ModioAPI.General.<GetResourceOwner>d__1 <GetResourceOwner>d__;
				<GetResourceOwner>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, UserObject?>>.Create();
				<GetResourceOwner>d__.<>1__state = -1;
				<GetResourceOwner>d__.<>t__builder.Start<ModioAPI.General.<GetResourceOwner>d__1>(ref <GetResourceOwner>d__);
				return <GetResourceOwner>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"webMessageObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> PingAsJToken()
			{
				ModioAPI.General.<PingAsJToken>d__2 <PingAsJToken>d__;
				<PingAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<PingAsJToken>d__.<>1__state = -1;
				<PingAsJToken>d__.<>t__builder.Start<ModioAPI.General.<PingAsJToken>d__2>(ref <PingAsJToken>d__);
				return <PingAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"webMessageObject"
			})]
			internal static Task<ValueTuple<Error, WebMessageObject?>> Ping()
			{
				ModioAPI.General.<Ping>d__3 <Ping>d__;
				<Ping>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, WebMessageObject?>>.Create();
				<Ping>d__.<>1__state = -1;
				<Ping>d__.<>t__builder.Start<ModioAPI.General.<Ping>d__3>(ref <Ping>d__);
				return <Ping>d__.<>t__builder.Task;
			}
		}

		public static class Metrics
		{
			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, JToken>> MetricsSessionEndAsJToken(MetricsSessionRequest sessionRequest)
			{
				ModioAPI.Metrics.<MetricsSessionEndAsJToken>d__0 <MetricsSessionEndAsJToken>d__;
				<MetricsSessionEndAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<MetricsSessionEndAsJToken>d__.sessionRequest = sessionRequest;
				<MetricsSessionEndAsJToken>d__.<>1__state = -1;
				<MetricsSessionEndAsJToken>d__.<>t__builder.Start<ModioAPI.Metrics.<MetricsSessionEndAsJToken>d__0>(ref <MetricsSessionEndAsJToken>d__);
				return <MetricsSessionEndAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, Response204?>> MetricsSessionEnd(MetricsSessionRequest sessionRequest)
			{
				ModioAPI.Metrics.<MetricsSessionEnd>d__1 <MetricsSessionEnd>d__;
				<MetricsSessionEnd>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Response204?>>.Create();
				<MetricsSessionEnd>d__.sessionRequest = sessionRequest;
				<MetricsSessionEnd>d__.<>1__state = -1;
				<MetricsSessionEnd>d__.<>t__builder.Start<ModioAPI.Metrics.<MetricsSessionEnd>d__1>(ref <MetricsSessionEnd>d__);
				return <MetricsSessionEnd>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, JToken>> MetricsSessionHeartbeatAsJToken(MetricsSessionRequest sessionRequest)
			{
				ModioAPI.Metrics.<MetricsSessionHeartbeatAsJToken>d__2 <MetricsSessionHeartbeatAsJToken>d__;
				<MetricsSessionHeartbeatAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<MetricsSessionHeartbeatAsJToken>d__.sessionRequest = sessionRequest;
				<MetricsSessionHeartbeatAsJToken>d__.<>1__state = -1;
				<MetricsSessionHeartbeatAsJToken>d__.<>t__builder.Start<ModioAPI.Metrics.<MetricsSessionHeartbeatAsJToken>d__2>(ref <MetricsSessionHeartbeatAsJToken>d__);
				return <MetricsSessionHeartbeatAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, Response204?>> MetricsSessionHeartbeat(MetricsSessionRequest sessionRequest)
			{
				ModioAPI.Metrics.<MetricsSessionHeartbeat>d__3 <MetricsSessionHeartbeat>d__;
				<MetricsSessionHeartbeat>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Response204?>>.Create();
				<MetricsSessionHeartbeat>d__.sessionRequest = sessionRequest;
				<MetricsSessionHeartbeat>d__.<>1__state = -1;
				<MetricsSessionHeartbeat>d__.<>t__builder.Start<ModioAPI.Metrics.<MetricsSessionHeartbeat>d__3>(ref <MetricsSessionHeartbeat>d__);
				return <MetricsSessionHeartbeat>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, JToken>> MetricsSessionStartAsJToken(MetricsSessionRequest sessionRequest)
			{
				ModioAPI.Metrics.<MetricsSessionStartAsJToken>d__4 <MetricsSessionStartAsJToken>d__;
				<MetricsSessionStartAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<MetricsSessionStartAsJToken>d__.sessionRequest = sessionRequest;
				<MetricsSessionStartAsJToken>d__.<>1__state = -1;
				<MetricsSessionStartAsJToken>d__.<>t__builder.Start<ModioAPI.Metrics.<MetricsSessionStartAsJToken>d__4>(ref <MetricsSessionStartAsJToken>d__);
				return <MetricsSessionStartAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, Response204?>> MetricsSessionStart(MetricsSessionRequest sessionRequest)
			{
				ModioAPI.Metrics.<MetricsSessionStart>d__5 <MetricsSessionStart>d__;
				<MetricsSessionStart>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Response204?>>.Create();
				<MetricsSessionStart>d__.sessionRequest = sessionRequest;
				<MetricsSessionStart>d__.<>1__state = -1;
				<MetricsSessionStart>d__.<>t__builder.Start<ModioAPI.Metrics.<MetricsSessionStart>d__5>(ref <MetricsSessionStart>d__);
				return <MetricsSessionStart>d__.<>t__builder.Task;
			}
		}

		public static class Users
		{
			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, JToken>> MuteAUserAsJToken(long userId)
			{
				ModioAPI.Users.<MuteAUserAsJToken>d__0 <MuteAUserAsJToken>d__;
				<MuteAUserAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<MuteAUserAsJToken>d__.userId = userId;
				<MuteAUserAsJToken>d__.<>1__state = -1;
				<MuteAUserAsJToken>d__.<>t__builder.Start<ModioAPI.Users.<MuteAUserAsJToken>d__0>(ref <MuteAUserAsJToken>d__);
				return <MuteAUserAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, Response204?>> MuteAUser(long userId)
			{
				ModioAPI.Users.<MuteAUser>d__1 <MuteAUser>d__;
				<MuteAUser>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Response204?>>.Create();
				<MuteAUser>d__.userId = userId;
				<MuteAUser>d__.<>1__state = -1;
				<MuteAUser>d__.<>t__builder.Start<ModioAPI.Users.<MuteAUser>d__1>(ref <MuteAUser>d__);
				return <MuteAUser>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, JToken>> UnmuteAUserAsJToken(long userId)
			{
				ModioAPI.Users.<UnmuteAUserAsJToken>d__2 <UnmuteAUserAsJToken>d__;
				<UnmuteAUserAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<UnmuteAUserAsJToken>d__.userId = userId;
				<UnmuteAUserAsJToken>d__.<>1__state = -1;
				<UnmuteAUserAsJToken>d__.<>t__builder.Start<ModioAPI.Users.<UnmuteAUserAsJToken>d__2>(ref <UnmuteAUserAsJToken>d__);
				return <UnmuteAUserAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, Response204?>> UnmuteAUser(long userId)
			{
				ModioAPI.Users.<UnmuteAUser>d__3 <UnmuteAUser>d__;
				<UnmuteAUser>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Response204?>>.Create();
				<UnmuteAUser>d__.userId = userId;
				<UnmuteAUser>d__.<>1__state = -1;
				<UnmuteAUser>d__.<>t__builder.Start<ModioAPI.Users.<UnmuteAUser>d__3>(ref <UnmuteAUser>d__);
				return <UnmuteAUser>d__.<>t__builder.Task;
			}
		}

		public static class ServiceToService
		{
			[return: TupleElementNames(new string[]
			{
				"error",
				"userDelegationTokenObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> RequestUserDelegationTokenAsJToken()
			{
				ModioAPI.ServiceToService.<RequestUserDelegationTokenAsJToken>d__0 <RequestUserDelegationTokenAsJToken>d__;
				<RequestUserDelegationTokenAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<RequestUserDelegationTokenAsJToken>d__.<>1__state = -1;
				<RequestUserDelegationTokenAsJToken>d__.<>t__builder.Start<ModioAPI.ServiceToService.<RequestUserDelegationTokenAsJToken>d__0>(ref <RequestUserDelegationTokenAsJToken>d__);
				return <RequestUserDelegationTokenAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"userDelegationTokenObject"
			})]
			internal static Task<ValueTuple<Error, UserDelegationTokenObject?>> RequestUserDelegationToken()
			{
				ModioAPI.ServiceToService.<RequestUserDelegationToken>d__1 <RequestUserDelegationToken>d__;
				<RequestUserDelegationToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, UserDelegationTokenObject?>>.Create();
				<RequestUserDelegationToken>d__.<>1__state = -1;
				<RequestUserDelegationToken>d__.<>t__builder.Start<ModioAPI.ServiceToService.<RequestUserDelegationToken>d__1>(ref <RequestUserDelegationToken>d__);
				return <RequestUserDelegationToken>d__.<>t__builder.Task;
			}
		}

		public static class Reports
		{
			[return: TupleElementNames(new string[]
			{
				"error",
				"addReportResponse"
			})]
			internal static Task<ValueTuple<Error, JToken>> SubmitReportAsJToken(AddReportRequest? body = null)
			{
				ModioAPI.Reports.<SubmitReportAsJToken>d__0 <SubmitReportAsJToken>d__;
				<SubmitReportAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<SubmitReportAsJToken>d__.body = body;
				<SubmitReportAsJToken>d__.<>1__state = -1;
				<SubmitReportAsJToken>d__.<>t__builder.Start<ModioAPI.Reports.<SubmitReportAsJToken>d__0>(ref <SubmitReportAsJToken>d__);
				return <SubmitReportAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"addReportResponse"
			})]
			internal static Task<ValueTuple<Error, AddReportResponse?>> SubmitReport(AddReportRequest? body = null)
			{
				ModioAPI.Reports.<SubmitReport>d__1 <SubmitReport>d__;
				<SubmitReport>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, AddReportResponse?>>.Create();
				<SubmitReport>d__.body = body;
				<SubmitReport>d__.<>1__state = -1;
				<SubmitReport>d__.<>t__builder.Start<ModioAPI.Reports.<SubmitReport>d__1>(ref <SubmitReport>d__);
				return <SubmitReport>d__.<>t__builder.Task;
			}
		}

		public static class Subscribe
		{
			[return: TupleElementNames(new string[]
			{
				"error",
				"modObject"
			})]
			internal static Task<ValueTuple<Error, JToken>> SubscribeToModAsJToken(long modId, AddModSubscriptionRequest? body = null)
			{
				ModioAPI.Subscribe.<SubscribeToModAsJToken>d__0 <SubscribeToModAsJToken>d__;
				<SubscribeToModAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<SubscribeToModAsJToken>d__.modId = modId;
				<SubscribeToModAsJToken>d__.body = body;
				<SubscribeToModAsJToken>d__.<>1__state = -1;
				<SubscribeToModAsJToken>d__.<>t__builder.Start<ModioAPI.Subscribe.<SubscribeToModAsJToken>d__0>(ref <SubscribeToModAsJToken>d__);
				return <SubscribeToModAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"modObject"
			})]
			internal static Task<ValueTuple<Error, ModObject?>> SubscribeToMod(long modId, AddModSubscriptionRequest? body = null)
			{
				ModioAPI.Subscribe.<SubscribeToMod>d__1 <SubscribeToMod>d__;
				<SubscribeToMod>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, ModObject?>>.Create();
				<SubscribeToMod>d__.modId = modId;
				<SubscribeToMod>d__.body = body;
				<SubscribeToMod>d__.<>1__state = -1;
				<SubscribeToMod>d__.<>t__builder.Start<ModioAPI.Subscribe.<SubscribeToMod>d__1>(ref <SubscribeToMod>d__);
				return <SubscribeToMod>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, JToken>> UnsubscribeFromModAsJToken(long modId)
			{
				ModioAPI.Subscribe.<UnsubscribeFromModAsJToken>d__2 <UnsubscribeFromModAsJToken>d__;
				<UnsubscribeFromModAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<UnsubscribeFromModAsJToken>d__.modId = modId;
				<UnsubscribeFromModAsJToken>d__.<>1__state = -1;
				<UnsubscribeFromModAsJToken>d__.<>t__builder.Start<ModioAPI.Subscribe.<UnsubscribeFromModAsJToken>d__2>(ref <UnsubscribeFromModAsJToken>d__);
				return <UnsubscribeFromModAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"response204"
			})]
			internal static Task<ValueTuple<Error, Response204?>> UnsubscribeFromMod(long modId)
			{
				ModioAPI.Subscribe.<UnsubscribeFromMod>d__3 <UnsubscribeFromMod>d__;
				<UnsubscribeFromMod>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Response204?>>.Create();
				<UnsubscribeFromMod>d__.modId = modId;
				<UnsubscribeFromMod>d__.<>1__state = -1;
				<UnsubscribeFromMod>d__.<>t__builder.Start<ModioAPI.Subscribe.<UnsubscribeFromMod>d__3>(ref <UnsubscribeFromMod>d__);
				return <UnsubscribeFromMod>d__.<>t__builder.Task;
			}
		}

		public static class InAppPurchases
		{
			[return: TupleElementNames(new string[]
			{
				"error",
				"entitlementFulfillmentObjects"
			})]
			internal static Task<ValueTuple<Error, JToken>> SyncAppleEntitlementAsJToken(SyncAppleEntitlementsRequest? body = null)
			{
				ModioAPI.InAppPurchases.<SyncAppleEntitlementAsJToken>d__0 <SyncAppleEntitlementAsJToken>d__;
				<SyncAppleEntitlementAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<SyncAppleEntitlementAsJToken>d__.body = body;
				<SyncAppleEntitlementAsJToken>d__.<>1__state = -1;
				<SyncAppleEntitlementAsJToken>d__.<>t__builder.Start<ModioAPI.InAppPurchases.<SyncAppleEntitlementAsJToken>d__0>(ref <SyncAppleEntitlementAsJToken>d__);
				return <SyncAppleEntitlementAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"entitlementFulfillmentObjects"
			})]
			internal static Task<ValueTuple<Error, Pagination<EntitlementFulfillmentObject[]>?>> SyncAppleEntitlement(SyncAppleEntitlementsRequest? body = null)
			{
				ModioAPI.InAppPurchases.<SyncAppleEntitlement>d__1 <SyncAppleEntitlement>d__;
				<SyncAppleEntitlement>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<EntitlementFulfillmentObject[]>?>>.Create();
				<SyncAppleEntitlement>d__.body = body;
				<SyncAppleEntitlement>d__.<>1__state = -1;
				<SyncAppleEntitlement>d__.<>t__builder.Start<ModioAPI.InAppPurchases.<SyncAppleEntitlement>d__1>(ref <SyncAppleEntitlement>d__);
				return <SyncAppleEntitlement>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"entitlementFulfillmentObjects"
			})]
			internal static Task<ValueTuple<Error, JToken>> SyncGoogleEntitlementsAsJToken()
			{
				ModioAPI.InAppPurchases.<SyncGoogleEntitlementsAsJToken>d__2 <SyncGoogleEntitlementsAsJToken>d__;
				<SyncGoogleEntitlementsAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<SyncGoogleEntitlementsAsJToken>d__.<>1__state = -1;
				<SyncGoogleEntitlementsAsJToken>d__.<>t__builder.Start<ModioAPI.InAppPurchases.<SyncGoogleEntitlementsAsJToken>d__2>(ref <SyncGoogleEntitlementsAsJToken>d__);
				return <SyncGoogleEntitlementsAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"entitlementFulfillmentObjects"
			})]
			internal static Task<ValueTuple<Error, Pagination<EntitlementFulfillmentObject[]>?>> SyncGoogleEntitlements()
			{
				ModioAPI.InAppPurchases.<SyncGoogleEntitlements>d__3 <SyncGoogleEntitlements>d__;
				<SyncGoogleEntitlements>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<EntitlementFulfillmentObject[]>?>>.Create();
				<SyncGoogleEntitlements>d__.<>1__state = -1;
				<SyncGoogleEntitlements>d__.<>t__builder.Start<ModioAPI.InAppPurchases.<SyncGoogleEntitlements>d__3>(ref <SyncGoogleEntitlements>d__);
				return <SyncGoogleEntitlements>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"entitlementFulfillmentObjects"
			})]
			public static Task<ValueTuple<Error, JToken>> SyncMetaEntitlementAsJToken()
			{
				ModioAPI.InAppPurchases.<SyncMetaEntitlementAsJToken>d__4 <SyncMetaEntitlementAsJToken>d__;
				<SyncMetaEntitlementAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<SyncMetaEntitlementAsJToken>d__.<>1__state = -1;
				<SyncMetaEntitlementAsJToken>d__.<>t__builder.Start<ModioAPI.InAppPurchases.<SyncMetaEntitlementAsJToken>d__4>(ref <SyncMetaEntitlementAsJToken>d__);
				return <SyncMetaEntitlementAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"entitlementFulfillmentObjects"
			})]
			public static Task<ValueTuple<Error, Pagination<EntitlementFulfillmentObject[]>?>> SyncMetaEntitlement(long userId)
			{
				ModioAPI.InAppPurchases.<SyncMetaEntitlement>d__5 <SyncMetaEntitlement>d__;
				<SyncMetaEntitlement>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<EntitlementFulfillmentObject[]>?>>.Create();
				<SyncMetaEntitlement>d__.userId = userId;
				<SyncMetaEntitlement>d__.<>1__state = -1;
				<SyncMetaEntitlement>d__.<>t__builder.Start<ModioAPI.InAppPurchases.<SyncMetaEntitlement>d__5>(ref <SyncMetaEntitlement>d__);
				return <SyncMetaEntitlement>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"entitlementFulfillmentObjects"
			})]
			public static Task<ValueTuple<Error, JToken>> SyncPlaystationNetworkEntitlementsAsJToken(SyncPlayStationNetworkEntitlementsRequest? body = null)
			{
				ModioAPI.InAppPurchases.<SyncPlaystationNetworkEntitlementsAsJToken>d__6 <SyncPlaystationNetworkEntitlementsAsJToken>d__;
				<SyncPlaystationNetworkEntitlementsAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<SyncPlaystationNetworkEntitlementsAsJToken>d__.body = body;
				<SyncPlaystationNetworkEntitlementsAsJToken>d__.<>1__state = -1;
				<SyncPlaystationNetworkEntitlementsAsJToken>d__.<>t__builder.Start<ModioAPI.InAppPurchases.<SyncPlaystationNetworkEntitlementsAsJToken>d__6>(ref <SyncPlaystationNetworkEntitlementsAsJToken>d__);
				return <SyncPlaystationNetworkEntitlementsAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"entitlementFulfillmentObjects"
			})]
			public static Task<ValueTuple<Error, Pagination<EntitlementFulfillmentObject[]>?>> SyncPlaystationNetworkEntitlements(SyncPlayStationNetworkEntitlementsRequest? body = null)
			{
				ModioAPI.InAppPurchases.<SyncPlaystationNetworkEntitlements>d__7 <SyncPlaystationNetworkEntitlements>d__;
				<SyncPlaystationNetworkEntitlements>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<EntitlementFulfillmentObject[]>?>>.Create();
				<SyncPlaystationNetworkEntitlements>d__.body = body;
				<SyncPlaystationNetworkEntitlements>d__.<>1__state = -1;
				<SyncPlaystationNetworkEntitlements>d__.<>t__builder.Start<ModioAPI.InAppPurchases.<SyncPlaystationNetworkEntitlements>d__7>(ref <SyncPlaystationNetworkEntitlements>d__);
				return <SyncPlaystationNetworkEntitlements>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"entitlementFulfillmentObjects"
			})]
			public static Task<ValueTuple<Error, JToken>> SyncSteamEntitlementAsJToken()
			{
				ModioAPI.InAppPurchases.<SyncSteamEntitlementAsJToken>d__8 <SyncSteamEntitlementAsJToken>d__;
				<SyncSteamEntitlementAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<SyncSteamEntitlementAsJToken>d__.<>1__state = -1;
				<SyncSteamEntitlementAsJToken>d__.<>t__builder.Start<ModioAPI.InAppPurchases.<SyncSteamEntitlementAsJToken>d__8>(ref <SyncSteamEntitlementAsJToken>d__);
				return <SyncSteamEntitlementAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"entitlementFulfillmentObjects"
			})]
			public static Task<ValueTuple<Error, Pagination<EntitlementFulfillmentObject[]>?>> SyncSteamEntitlement()
			{
				ModioAPI.InAppPurchases.<SyncSteamEntitlement>d__9 <SyncSteamEntitlement>d__;
				<SyncSteamEntitlement>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<EntitlementFulfillmentObject[]>?>>.Create();
				<SyncSteamEntitlement>d__.<>1__state = -1;
				<SyncSteamEntitlement>d__.<>t__builder.Start<ModioAPI.InAppPurchases.<SyncSteamEntitlement>d__9>(ref <SyncSteamEntitlement>d__);
				return <SyncSteamEntitlement>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"entitlementFulfillmentObjects"
			})]
			public static Task<ValueTuple<Error, JToken>> SyncXboxLiveEntitlementsAsJToken(SyncXboxEntitlementsRequest? body = null)
			{
				ModioAPI.InAppPurchases.<SyncXboxLiveEntitlementsAsJToken>d__10 <SyncXboxLiveEntitlementsAsJToken>d__;
				<SyncXboxLiveEntitlementsAsJToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, JToken>>.Create();
				<SyncXboxLiveEntitlementsAsJToken>d__.body = body;
				<SyncXboxLiveEntitlementsAsJToken>d__.<>1__state = -1;
				<SyncXboxLiveEntitlementsAsJToken>d__.<>t__builder.Start<ModioAPI.InAppPurchases.<SyncXboxLiveEntitlementsAsJToken>d__10>(ref <SyncXboxLiveEntitlementsAsJToken>d__);
				return <SyncXboxLiveEntitlementsAsJToken>d__.<>t__builder.Task;
			}

			[return: TupleElementNames(new string[]
			{
				"error",
				"entitlementFulfillmentObjects"
			})]
			public static Task<ValueTuple<Error, Pagination<EntitlementFulfillmentObject[]>?>> SyncXboxLiveEntitlements(SyncXboxEntitlementsRequest? body = null)
			{
				ModioAPI.InAppPurchases.<SyncXboxLiveEntitlements>d__11 <SyncXboxLiveEntitlements>d__;
				<SyncXboxLiveEntitlements>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Pagination<EntitlementFulfillmentObject[]>?>>.Create();
				<SyncXboxLiveEntitlements>d__.body = body;
				<SyncXboxLiveEntitlements>d__.<>1__state = -1;
				<SyncXboxLiveEntitlements>d__.<>t__builder.Start<ModioAPI.InAppPurchases.<SyncXboxLiveEntitlements>d__11>(ref <SyncXboxLiveEntitlements>d__);
				return <SyncXboxLiveEntitlements>d__.<>t__builder.Task;
			}
		}

		public enum Platform
		{
			None = -1,
			Source,
			Windows,
			Mac,
			Linux,
			Android,
			IOS,
			XboxOne,
			XboxSeriesX,
			PlayStation4,
			PlayStation5,
			Switch,
			Oculus
		}

		public enum Portal
		{
			None = -1,
			Apple,
			Discord,
			EpicGamesStore,
			Facebook,
			GOG,
			Google,
			Itchio,
			Nintendo,
			PlayStationNetwork,
			SSO,
			Steam,
			XboxLive
		}
	}
}
