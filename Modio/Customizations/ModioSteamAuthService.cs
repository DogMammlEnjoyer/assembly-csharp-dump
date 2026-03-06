using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio.API;
using Modio.Authentication;
using Modio.Errors;

namespace Modio.Customizations
{
	public class ModioSteamAuthService : IModioAuthService, IGetActiveUserIdentifier, IPotentialModioEmailAuthService
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
				return ModioAPI.Portal.Steam;
			}
		}

		public Task<string> GetActiveUserIdentifier()
		{
			return Task.FromResult<string>("steam_user");
		}

		public ModioSteamAuthService(ISteamCredentialProvider credentialProvider)
		{
			this._credentialProvider = credentialProvider;
		}

		public ModioSteamAuthService()
		{
		}

		public Task<Error> Authenticate(bool displayedTerms, string thirdPartyEmail = null)
		{
			ModioSteamAuthService.<Authenticate>d__11 <Authenticate>d__;
			<Authenticate>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<Authenticate>d__.<>4__this = this;
			<Authenticate>d__.displayedTerms = displayedTerms;
			<Authenticate>d__.thirdPartyEmail = thirdPartyEmail;
			<Authenticate>d__.<>1__state = -1;
			<Authenticate>d__.<>t__builder.Start<ModioSteamAuthService.<Authenticate>d__11>(ref <Authenticate>d__);
			return <Authenticate>d__.<>t__builder.Task;
		}

		private void OnGetEncryptedAppTicket(bool success, string encryptedAppTicketOrError)
		{
			ModioLog verbose = ModioLog.Verbose;
			if (verbose != null)
			{
				verbose.Log(string.Format("Got Steam Encrypted App Ticket: {0} | Ticket: {1} | ", success, encryptedAppTicketOrError) + string.Format("attemptInProgress: {0} | ", this._isAttemptInProgress));
			}
			if (!this._isAttemptInProgress)
			{
				return;
			}
			if (!success)
			{
				ModioLog error = ModioLog.Error;
				if (error != null)
				{
					error.Log(encryptedAppTicketOrError);
				}
				this._encryptedAppTicketError = new Error(ErrorCode.STEAM_FAILED_TO_GET_APP_TICKET, encryptedAppTicketOrError);
				return;
			}
			this._encryptedAppTicket = encryptedAppTicketOrError;
		}

		public void SetCredentialProvider(ISteamCredentialProvider credentialProvider)
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
			this._encryptedAppTicketError = Error.None;
			this._encryptedAppTicket = null;
			this._isAttemptInProgress = false;
			return error;
		}

		private ISteamCredentialProvider _credentialProvider;

		private bool _isAttemptInProgress;

		private string _encryptedAppTicket;

		private Error _encryptedAppTicketError = Error.None;
	}
}
