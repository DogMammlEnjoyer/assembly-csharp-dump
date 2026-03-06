using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Modio.API;

namespace Modio.Authentication
{
	public class ModioMultiplatformAuthResolver : IModioAuthService, IGetActiveUserIdentifier, IPotentialModioEmailAuthService
	{
		public static IModioAuthService ServiceOverride { get; set; }

		public static IReadOnlyList<IModioAuthService> AuthBindings { get; private set; }

		public static void Initialize()
		{
			if (ModioMultiplatformAuthResolver._hasInitialized)
			{
				return;
			}
			ModioMultiplatformAuthResolver._hasInitialized = true;
			ModioMultiplatformAuthResolver.AuthBindings = (from tuple in ModioServices.GetBindings<IModioAuthService>(false).ResolveAll()
			orderby tuple.Item2 descending
			select tuple into platformPair
			select platformPair.Item1 into platform
			where platform is IGetActiveUserIdentifier
			select platform).ToList<IModioAuthService>();
			ModioMultiplatformAuthResolver.ServiceOverride = ModioMultiplatformAuthResolver.AuthBindings.FirstOrDefault<IModioAuthService>();
			ModioServices.Bind<ModioMultiplatformAuthResolver>().WithInterfaces<IModioAuthService>(new Func<bool>(ModioMultiplatformAuthResolver.IsActiveForConditional)).WithInterfaces<IGetActiveUserIdentifier>(new Func<bool>(ModioMultiplatformAuthResolver.IsActiveForConditional)).FromNew<ModioMultiplatformAuthResolver>((ModioServicePriority)50, null);
			ModioMultiplatformAuthResolver._resolveUsingThis = true;
		}

		private static bool IsActiveForConditional()
		{
			return ModioMultiplatformAuthResolver._resolveUsingThis;
		}

		public Task<Error> Authenticate(bool displayedTerms, string thirdPartyEmail = null)
		{
			return ModioMultiplatformAuthResolver.Get<IModioAuthService>().Authenticate(displayedTerms, thirdPartyEmail);
		}

		public Task<string> GetActiveUserIdentifier()
		{
			return ModioMultiplatformAuthResolver.Get<IGetActiveUserIdentifier>().GetActiveUserIdentifier();
		}

		private static T Get<T>()
		{
			IModioAuthService serviceOverride = ModioMultiplatformAuthResolver.ServiceOverride;
			if (serviceOverride is T)
			{
				return (T)((object)serviceOverride);
			}
			ModioMultiplatformAuthResolver._resolveUsingThis = false;
			T result = ModioServices.Resolve<T>();
			ModioMultiplatformAuthResolver._resolveUsingThis = true;
			return result;
		}

		public bool IsEmailPlatform
		{
			get
			{
				IPotentialModioEmailAuthService potentialModioEmailAuthService = ModioMultiplatformAuthResolver.Get<IModioAuthService>() as IPotentialModioEmailAuthService;
				return potentialModioEmailAuthService != null && potentialModioEmailAuthService.IsEmailPlatform;
			}
		}

		public ModioAPI.Portal Portal
		{
			get
			{
				IModioAuthService modioAuthService = ModioMultiplatformAuthResolver.Get<IModioAuthService>();
				if (modioAuthService == null)
				{
					return ModioAPI.Portal.None;
				}
				return modioAuthService.Portal;
			}
		}

		private const ModioServicePriority SERVICE_BINDING_PRIORITY = (ModioServicePriority)50;

		private static bool _resolveUsingThis;

		private static bool _hasInitialized;
	}
}
