using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio.API;
using Modio.Errors;

namespace Modio.Authentication
{
	public class ModioEmailAuthService : IModioAuthService, IGetActiveUserIdentifier, IPotentialModioEmailAuthService
	{
		public bool IsEmailPlatform
		{
			get
			{
				return true;
			}
		}

		public ModioAPI.Portal Portal
		{
			get
			{
				return ModioAPI.Portal.None;
			}
		}

		public ModioEmailAuthService(Func<Task<string>> codePrompter)
		{
			this._codePrompter = new ModioEmailAuthService.EmailCodePrompter(codePrompter);
		}

		public ModioEmailAuthService(IEmailCodePrompter codePrompter)
		{
			this._codePrompter = codePrompter;
		}

		public ModioEmailAuthService()
		{
		}

		public Task<Error> Authenticate(bool displayedTerms, string thirdPartyEmail = null)
		{
			ModioEmailAuthService.<Authenticate>d__9 <Authenticate>d__;
			<Authenticate>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<Authenticate>d__.<>4__this = this;
			<Authenticate>d__.thirdPartyEmail = thirdPartyEmail;
			<Authenticate>d__.<>1__state = -1;
			<Authenticate>d__.<>t__builder.Start<ModioEmailAuthService.<Authenticate>d__9>(ref <Authenticate>d__);
			return <Authenticate>d__.<>t__builder.Task;
		}

		public Task<Error> AuthenticateWithoutEmailRequest()
		{
			ModioEmailAuthService.<AuthenticateWithoutEmailRequest>d__10 <AuthenticateWithoutEmailRequest>d__;
			<AuthenticateWithoutEmailRequest>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<AuthenticateWithoutEmailRequest>d__.<>4__this = this;
			<AuthenticateWithoutEmailRequest>d__.<>1__state = -1;
			<AuthenticateWithoutEmailRequest>d__.<>t__builder.Start<ModioEmailAuthService.<AuthenticateWithoutEmailRequest>d__10>(ref <AuthenticateWithoutEmailRequest>d__);
			return <AuthenticateWithoutEmailRequest>d__.<>t__builder.Task;
		}

		private Task<Error> ExchangeCode(string code)
		{
			ModioEmailAuthService.<ExchangeCode>d__11 <ExchangeCode>d__;
			<ExchangeCode>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<ExchangeCode>d__.<>4__this = this;
			<ExchangeCode>d__.code = code;
			<ExchangeCode>d__.<>1__state = -1;
			<ExchangeCode>d__.<>t__builder.Start<ModioEmailAuthService.<ExchangeCode>d__11>(ref <ExchangeCode>d__);
			return <ExchangeCode>d__.<>t__builder.Task;
		}

		private Error ValidateAttempt()
		{
			if (this._codePrompter != null)
			{
				return Error.None;
			}
			ModioLog error = ModioLog.Error;
			if (error != null)
			{
				error.Log(string.Format("{0} cannot authenticate as no Code Prompter has been set! Call ModioEmailAuthPlatform.SetCodePrompter before calling Authenticate or use a constructor that takes a Code Prompter parameter..", typeof(ModioEmailAuthService)));
			}
			return new Error(ErrorCode.NOT_INITIALIZED);
		}

		private Error ReturnErrorAndReset(Error error)
		{
			this._isAttemptInProgress = false;
			return error;
		}

		public void SetCodePrompter(IEmailCodePrompter codePrompter)
		{
			this._codePrompter = codePrompter;
		}

		public void SetCodePrompter(Func<Task<string>> codePrompter)
		{
			this._codePrompter = new ModioEmailAuthService.EmailCodePrompter(codePrompter);
		}

		public Task<string> GetActiveUserIdentifier()
		{
			return Task.FromResult<string>("user");
		}

		private IEmailCodePrompter _codePrompter;

		private bool _isAttemptInProgress;

		private class EmailCodePrompter : IEmailCodePrompter
		{
			public EmailCodePrompter(Func<Task<string>> codePrompt)
			{
				this._codePrompt = codePrompt;
			}

			public Task<string> ShowCodePrompt()
			{
				return this._codePrompt();
			}

			private readonly Func<Task<string>> _codePrompt;
		}
	}
}
