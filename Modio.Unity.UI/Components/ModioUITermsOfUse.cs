using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio.Extensions;
using Modio.Platforms;
using TMPro;
using UnityEngine;

namespace Modio.Unity.UI.Components
{
	public class ModioUITermsOfUse : MonoBehaviour
	{
		public void Start()
		{
			if (ModioUITermsOfUse._termsOfUse != null)
			{
				this.ApplyTermsOfUse();
				return;
			}
			ModioClient.OnInitialized += this.OnPluginReady;
		}

		private void OnPluginReady()
		{
			this.GetTermsOfUse().ForgetTaskSafely();
			ModioUITermsOfUse._browserHandler = ModioServices.Resolve<IWebBrowserHandler>();
		}

		private Task GetTermsOfUse()
		{
			ModioUITermsOfUse.<GetTermsOfUse>d__9 <GetTermsOfUse>d__;
			<GetTermsOfUse>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<GetTermsOfUse>d__.<>4__this = this;
			<GetTermsOfUse>d__.<>1__state = -1;
			<GetTermsOfUse>d__.<>t__builder.Start<ModioUITermsOfUse.<GetTermsOfUse>d__9>(ref <GetTermsOfUse>d__);
			return <GetTermsOfUse>d__.<>t__builder.Task;
		}

		private void ApplyTermsOfUse()
		{
			if (ModioUITermsOfUse._termsOfUse != null)
			{
				if (this._termsOfUseText != null)
				{
					this._termsOfUseText.text = ModioUITermsOfUse._termsOfUse.TermsText;
				}
				if (this._agreeText != null)
				{
					this._agreeText.text = ModioUITermsOfUse._termsOfUse.AgreeText;
				}
				if (this._disagreeText != null)
				{
					this._disagreeText.text = ModioUITermsOfUse._termsOfUse.DisagreeText;
				}
				if (this._termsOfUseLinkButtonText != null)
				{
					this._termsOfUseLinkButtonText.text = this.GetLinkButtonText(LinkType.Terms);
				}
				if (this._privacyPolicyLinkButtonText != null)
				{
					this._privacyPolicyLinkButtonText.text = this.GetLinkButtonText(LinkType.Privacy);
				}
				return;
			}
			ModioLog error = ModioLog.Error;
			if (error == null)
			{
				return;
			}
			error.Log("Attempted to apply terms of use before they were loaded");
		}

		public void HyperLinkToTOS()
		{
			ModioUITermsOfUse.HyperlinkTo(LinkType.Terms, "https://mod.io/terms");
		}

		public void HyperLinkToPrivacyPolicy()
		{
			ModioUITermsOfUse.HyperlinkTo(LinkType.Privacy, "https://mod.io/privacy");
		}

		public void HyperLinkToRefundPolicy()
		{
			ModioUITermsOfUse.HyperlinkTo(LinkType.Refund, "https://mod.io/refund");
		}

		private string GetLinkButtonText(LinkType type)
		{
			if (ModioUITermsOfUse._termsOfUse == null)
			{
				ModioLog error = ModioLog.Error;
				if (error != null)
				{
					error.Log(string.Format("Attempted to get button text for {0} before terms of use loaded", type));
				}
				return type.ToString();
			}
			foreach (TermsOfUseLink termsOfUseLink in ModioUITermsOfUse._termsOfUse.Links)
			{
				if (termsOfUseLink.type == type)
				{
					return termsOfUseLink.text;
				}
			}
			ModioLog error2 = ModioLog.Error;
			if (error2 != null)
			{
				error2.Log(string.Format("Couldn't find TermsOfUseLink button text for {0}", type));
			}
			return type.ToString();
		}

		private static void HyperlinkTo(LinkType type, string fallbackLink)
		{
			string text = null;
			if (ModioUITermsOfUse._termsOfUse == null)
			{
				Debug.LogError(string.Format("Attempted to open hyperlink to {0} before terms of use loaded. Using fallback", type));
			}
			else
			{
				text = ModioUITermsOfUse._termsOfUse.GetLink(type).url;
			}
			if (string.IsNullOrWhiteSpace(text))
			{
				text = fallbackLink;
			}
			IWebBrowserHandler browserHandler = ModioUITermsOfUse._browserHandler;
			if (browserHandler == null)
			{
				return;
			}
			browserHandler.OpenUrl(text);
		}

		[SerializeField]
		private TMP_Text _termsOfUseText;

		[SerializeField]
		private TMP_Text _agreeText;

		[SerializeField]
		private TMP_Text _disagreeText;

		[SerializeField]
		private TMP_Text _termsOfUseLinkButtonText;

		[SerializeField]
		private TMP_Text _privacyPolicyLinkButtonText;

		private static TermsOfUse _termsOfUse;

		private static IWebBrowserHandler _browserHandler;
	}
}
