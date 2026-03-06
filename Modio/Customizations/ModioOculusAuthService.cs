using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio.API;
using Modio.Authentication;
using Modio.Errors;

namespace Modio.Customizations
{
	public class ModioOculusAuthService : IModioAuthService, IGetActiveUserIdentifier, IPotentialModioEmailAuthService
	{
		public bool IsEmailPlatform
		{
			get
			{
				return false;
			}
		}

		public ModioAPI.Portal Portal
		{
			get
			{
				return ModioAPI.Portal.None;
			}
		}

		public Task<string> GetActiveUserIdentifier()
		{
			return Task.FromResult<string>("oculus_user");
		}

		public ModioOculusAuthService(IOculusCredentialProvider credentialProvider)
		{
			this._credentialProvider = credentialProvider;
		}

		public ModioOculusAuthService()
		{
		}

		public Task<Error> Authenticate(bool displayedTerms, string thirdPartyEmail = null)
		{
			ModioOculusAuthService.<Authenticate>d__9 <Authenticate>d__;
			<Authenticate>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<Authenticate>d__.<>4__this = this;
			<Authenticate>d__.thirdPartyEmail = thirdPartyEmail;
			<Authenticate>d__.<>1__state = -1;
			<Authenticate>d__.<>t__builder.Start<ModioOculusAuthService.<Authenticate>d__9>(ref <Authenticate>d__);
			return <Authenticate>d__.<>t__builder.Task;
		}

		public void SetCredentialProvider(IOculusCredentialProvider credentialProvider)
		{
			this._credentialProvider = credentialProvider;
		}

		private Error ValidateAttempt()
		{
			if (this._credentialProvider != null)
			{
				return Error.None;
			}
			ModioLog error = ModioLog.Error;
			if (error != null)
			{
				error.Log(string.Format("{0} cannot authenticate as no Steam Credential Provider", typeof(ModioSteamAuthService)) + " has been set! Call ModioSteamAuthService.SetCredentialProvider before calling Authenticate or use a constructor that takes a Credential Provider parameter..");
			}
			return new Error(ErrorCode.NOT_INITIALIZED);
		}

		private Error ReturnErrorAndReset(Error error)
		{
			this._isAttemptInProgress = false;
			return error;
		}

		private IOculusCredentialProvider _credentialProvider;

		private bool _isAttemptInProgress;
	}
}
