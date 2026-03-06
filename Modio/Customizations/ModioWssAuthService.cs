using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio.API;
using Modio.Authentication;
using Modio.Errors;

namespace Modio.Customizations
{
	public class ModioWssAuthService : IModioAuthService, IGetActiveUserIdentifier, IPotentialModioEmailAuthService
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
			return Task.FromResult<string>("linked_account_user");
		}

		public ModioWssAuthService(IWssAuthPrompter prompter)
		{
			this._authPrompter = prompter;
		}

		public ModioWssAuthService()
		{
		}

		public Task<Error> Authenticate(bool displayedTerms, string thirdPartyEmail = null)
		{
			ModioWssAuthService.<Authenticate>d__10 <Authenticate>d__;
			<Authenticate>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<Authenticate>d__.<>4__this = this;
			<Authenticate>d__.<>1__state = -1;
			<Authenticate>d__.<>t__builder.Start<ModioWssAuthService.<Authenticate>d__10>(ref <Authenticate>d__);
			return <Authenticate>d__.<>t__builder.Task;
		}

		private Error ValidateAttempt()
		{
			if (this._authPrompter != null)
			{
				return Error.None;
			}
			ModioLog error = ModioLog.Error;
			if (error != null)
			{
				error.Log(string.Format("{0} cannot authenticate as no Prompter has been set! ", typeof(ModioWssAuthService)) + "Call ModioWssAuthService.SetPrompter before calling Authenticate or use a constructor that takes a Prompter parameter..");
			}
			return new Error(ErrorCode.NOT_INITIALIZED);
		}

		private Error ReturnErrorAndReset(Error error)
		{
			this._isAttemptInProgress = false;
			return error;
		}

		public void SetPrompter(IWssAuthPrompter prompter)
		{
			this._authPrompter = prompter;
		}

		public bool InProgress()
		{
			return this._isAttemptInProgress;
		}

		public void Cancel()
		{
			if (!this._isAttemptInProgress)
			{
				return;
			}
			if (this._authToken.task == null)
			{
				return;
			}
			this._authToken.Cancel();
		}

		private IWssAuthPrompter _authPrompter;

		private bool _isAttemptInProgress;

		private ExternalAuthenticationToken _authToken;
	}
}
