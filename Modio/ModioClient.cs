using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio.API.HttpClient;
using Modio.API.Interfaces;
using Modio.Authentication;
using Modio.FileIO;

namespace Modio
{
	public static class ModioClient
	{
		public static IModioDataStorage DataStorage
		{
			get
			{
				return ModioServices.Resolve<IModioDataStorage>();
			}
		}

		public static IModioAPIInterface Api
		{
			get
			{
				return ModioServices.Resolve<IModioAPIInterface>();
			}
		}

		public static IModioAuthService AuthService
		{
			get
			{
				return ModioServices.Resolve<IModioAuthService>();
			}
		}

		public static ModioSettings Settings
		{
			get
			{
				return ModioServices.Resolve<ModioSettings>();
			}
		}

		public static bool IsInitialized { get; private set; }

		internal static bool IsCurrentlyInitializing
		{
			get
			{
				return ModioClient._initializingTCS != null;
			}
		}

		private static event Action InternalOnInitialized;

		public static event Action OnInitialized
		{
			add
			{
				ModioClient.InternalOnInitialized += value;
				if (ModioClient.IsInitialized && value != null)
				{
					value();
				}
			}
			remove
			{
				ModioClient.InternalOnInitialized -= value;
			}
		}

		public static event Action OnShutdown;

		public static Task<Error> Init(ModioSettings settings)
		{
			ModioServices.BindInstance<ModioSettings>(settings, ModioServicePriority.PlatformProvided);
			return ModioClient.Init();
		}

		public static Task<Error> Init()
		{
			ModioClient.<Init>d__26 <Init>d__;
			<Init>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<Init>d__.<>1__state = -1;
			<Init>d__.<>t__builder.Start<ModioClient.<Init>d__26>(ref <Init>d__);
			return <Init>d__.<>t__builder.Task;
		}

		public static Task Shutdown()
		{
			ModioClient.<Shutdown>d__27 <Shutdown>d__;
			<Shutdown>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Shutdown>d__.<>1__state = -1;
			<Shutdown>d__.<>t__builder.Start<ModioClient.<Shutdown>d__27>(ref <Shutdown>d__);
			return <Shutdown>d__.<>t__builder.Task;
		}

		private static void BindDefaultServices()
		{
			if (ModioClient._hasBoundDefaultServices)
			{
				return;
			}
			ModioClient._hasBoundDefaultServices = true;
			ModioServices.Bind<IModioAPIInterface>().FromNew<ModioAPIHttpClient>(ModioServicePriority.Default, null);
			ModioServices.Bind<IModioDataStorage>().FromNew<BaseDataStorage>(ModioServicePriority.Default, null);
			ModioServices.Bind<ModioEmailAuthService>().WithInterfaces<IGetActiveUserIdentifier>(null).WithInterfaces<IModioAuthService>(null).FromNew<ModioEmailAuthService>(ModioServicePriority.Default, null);
			ModioServices.BindErrorMessage<ModioSettings>("Please ensure you've bound a ModioSettings using ModioServices.BindInstance(settings); before trying to use Modio classes", ModioServicePriority.Fallback);
		}

		private static TaskCompletionSource<Error> _initializingTCS;

		private static bool _hasBoundDefaultServices;
	}
}
