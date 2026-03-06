using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio.Authentication;
using Modio.Extensions;
using Modio.Users;
using UnityEngine;
using UnityEngine.Events;

namespace Modio.Unity.UI.Panels.Authentication
{
	public class ModioAuthenticationPanel : ModioPanelBase
	{
		[ModioDebugMenu(ShowInBrowserMenu = false, ShowInSettingsMenu = true)]
		private static bool ForceShowTermsOfUse { get; set; }

		public void OpenAuthFlow()
		{
			if (User.Current != null && User.Current.IsAuthenticated)
			{
				Debug.LogWarning("Attempted to open Auth Flow when already logged in");
				return;
			}
			if (!ModioClient.IsInitialized)
			{
				ModioWaitingPanelGeneric panelOfType = ModioPanelManager.GetPanelOfType<ModioWaitingPanelGeneric>();
				if (panelOfType != null && !panelOfType.HasFocus)
				{
					panelOfType.OpenPanel();
				}
			}
			ModioClient.OnInitialized -= this.OnPluginReady;
			ModioClient.OnInitialized += this.OnPluginReady;
		}

		protected override void OnDestroy()
		{
			ModioClient.OnInitialized -= this.OnPluginReady;
			base.OnDestroy();
		}

		private void OnPluginReady()
		{
			if (ModioClient.AuthService != null)
			{
				IPotentialModioEmailAuthService potentialModioEmailAuthService = ModioClient.AuthService as IPotentialModioEmailAuthService;
				if (potentialModioEmailAuthService == null || !potentialModioEmailAuthService.IsEmailPlatform)
				{
					base.OpenPanel();
					this.AttemptSso(false).ForgetTaskSafely();
					return;
				}
			}
			this.GetTermsAndShowPanel().ForgetTaskSafely();
		}

		private Task GetTermsAndShowPanel()
		{
			ModioAuthenticationPanel.<GetTermsAndShowPanel>d__10 <GetTermsAndShowPanel>d__;
			<GetTermsAndShowPanel>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<GetTermsAndShowPanel>d__.<>4__this = this;
			<GetTermsAndShowPanel>d__.<>1__state = -1;
			<GetTermsAndShowPanel>d__.<>t__builder.Start<ModioAuthenticationPanel.<GetTermsAndShowPanel>d__10>(ref <GetTermsAndShowPanel>d__);
			return <GetTermsAndShowPanel>d__.<>t__builder.Task;
		}

		private void LateUpdate()
		{
			if (!base.HasFocus)
			{
				return;
			}
			if (!this._fallbackToEmailAuth)
			{
				base.ClosePanel();
				return;
			}
			this._fallbackToEmailAuth = false;
			ModioAuthenticationIEmailPanel panelOfType = ModioPanelManager.GetPanelOfType<ModioAuthenticationIEmailPanel>();
			if (panelOfType == null)
			{
				return;
			}
			panelOfType.OpenPanel();
		}

		public Task AttemptSso(bool agreedToTerms)
		{
			ModioAuthenticationPanel.<AttemptSso>d__12 <AttemptSso>d__;
			<AttemptSso>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AttemptSso>d__.<>4__this = this;
			<AttemptSso>d__.agreedToTerms = agreedToTerms;
			<AttemptSso>d__.<>1__state = -1;
			<AttemptSso>d__.<>t__builder.Start<ModioAuthenticationPanel.<AttemptSso>d__12>(ref <AttemptSso>d__);
			return <AttemptSso>d__.<>t__builder.Task;
		}

		[SerializeField]
		private UnityEvent<Error> _onError;

		[SerializeField]
		private UnityEvent<Error> _onOffline;

		private bool _fallbackToEmailAuth;
	}
}
