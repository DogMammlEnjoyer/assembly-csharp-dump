using System;
using System.Threading.Tasks;

namespace Modio.Customizations
{
	public interface IOculusCredentialProvider
	{
		Task<ValueTuple<Error, string>> GetOculusUserId();

		Task<string> GetOculusAccessToken();

		Task<string> GetOculusUserProof();

		string GetOculusDevice();
	}
}
