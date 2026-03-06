using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio.Authentication;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Modio.Unity.UI.Panels.Authentication
{
	public class ModioAuthenticationIEmailPanel : ModioPanelBase, IEmailCodePrompter
	{
		public override void OnGainedFocus(ModioPanelBase.GainedFocusCause context)
		{
			base.OnGainedFocus(context);
			this._authService = ModioServices.Resolve<ModioEmailAuthService>();
			this._authService.SetCodePrompter(this);
		}

		public void OnPressSubmitEmail()
		{
			ModioAuthenticationIEmailPanel.<OnPressSubmitEmail>d__6 <OnPressSubmitEmail>d__;
			<OnPressSubmitEmail>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<OnPressSubmitEmail>d__.<>4__this = this;
			<OnPressSubmitEmail>d__.<>1__state = -1;
			<OnPressSubmitEmail>d__.<>t__builder.Start<ModioAuthenticationIEmailPanel.<OnPressSubmitEmail>d__6>(ref <OnPressSubmitEmail>d__);
		}

		public void OnPressIHaveCode()
		{
			ModioAuthenticationIEmailPanel.<OnPressIHaveCode>d__7 <OnPressIHaveCode>d__;
			<OnPressIHaveCode>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<OnPressIHaveCode>d__.<>4__this = this;
			<OnPressIHaveCode>d__.<>1__state = -1;
			<OnPressIHaveCode>d__.<>t__builder.Start<ModioAuthenticationIEmailPanel.<OnPressIHaveCode>d__7>(ref <OnPressIHaveCode>d__);
		}

		private void OnCodeEntered(string code)
		{
			this._authCode = code;
			this._isCodeEntered = true;
		}

		private Task AuthenticationRequest(string email, Task<Error> authMethod)
		{
			ModioAuthenticationIEmailPanel.<AuthenticationRequest>d__9 <AuthenticationRequest>d__;
			<AuthenticationRequest>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AuthenticationRequest>d__.<>4__this = this;
			<AuthenticationRequest>d__.email = email;
			<AuthenticationRequest>d__.authMethod = authMethod;
			<AuthenticationRequest>d__.<>1__state = -1;
			<AuthenticationRequest>d__.<>t__builder.Start<ModioAuthenticationIEmailPanel.<AuthenticationRequest>d__9>(ref <AuthenticationRequest>d__);
			return <AuthenticationRequest>d__.<>t__builder.Task;
		}

		public Task<string> ShowCodePrompt()
		{
			ModioAuthenticationIEmailPanel.<ShowCodePrompt>d__10 <ShowCodePrompt>d__;
			<ShowCodePrompt>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<ShowCodePrompt>d__.<>4__this = this;
			<ShowCodePrompt>d__.<>1__state = -1;
			<ShowCodePrompt>d__.<>t__builder.Start<ModioAuthenticationIEmailPanel.<ShowCodePrompt>d__10>(ref <ShowCodePrompt>d__);
			return <ShowCodePrompt>d__.<>t__builder.Task;
		}

		[SerializeField]
		private TMP_InputField _emailField;

		[SerializeField]
		private UnityEvent<Error> _onError;

		private ModioEmailAuthService _authService;

		private bool _isCodeEntered;

		private string _authCode = string.Empty;
	}
}
