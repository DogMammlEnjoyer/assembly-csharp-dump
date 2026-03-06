using System;
using System.Threading.Tasks;
using Modio.API;

namespace Modio.Authentication
{
	public interface IModioAuthService
	{
		Task<Error> Authenticate(bool displayedTerms, string thirdPartyEmail = null);

		ModioAPI.Portal Portal { get; }
	}
}
