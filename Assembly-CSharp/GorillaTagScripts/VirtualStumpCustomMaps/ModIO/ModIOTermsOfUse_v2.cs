using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Modio;
using Modio.Customizations;
using UnityEngine;

namespace GorillaTagScripts.VirtualStumpCustomMaps.ModIO
{
	public class ModIOTermsOfUse_v2 : LegalAgreements
	{
		protected override void Awake()
		{
			if (ModIOTermsOfUse_v2.modioTermsInstance != null)
			{
				Debug.LogError("Trying to set [LegalAgreements] instance but it is not null", this);
				base.gameObject.SetActive(false);
				return;
			}
			ModIOTermsOfUse_v2.modioTermsInstance = this;
			this.stickHeldDuration = 0f;
			this.scrollSpeed = this._minScrollSpeed;
			base.enabled = false;
		}

		public Task<Error> ShowTerms()
		{
			ModIOTermsOfUse_v2.<ShowTerms>d__7 <ShowTerms>d__;
			<ShowTerms>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<ShowTerms>d__.<>4__this = this;
			<ShowTerms>d__.<>1__state = -1;
			<ShowTerms>d__.<>t__builder.Start<ModIOTermsOfUse_v2.<ShowTerms>d__7>(ref <ShowTerms>d__);
			return <ShowTerms>d__.<>t__builder.Task;
		}

		public override Task StartLegalAgreements()
		{
			ModIOTermsOfUse_v2.<StartLegalAgreements>d__8 <StartLegalAgreements>d__;
			<StartLegalAgreements>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<StartLegalAgreements>d__.<>4__this = this;
			<StartLegalAgreements>d__.<>1__state = -1;
			<StartLegalAgreements>d__.<>t__builder.Start<ModIOTermsOfUse_v2.<StartLegalAgreements>d__8>(ref <StartLegalAgreements>d__);
			return <StartLegalAgreements>d__.<>t__builder.Task;
		}

		private void UpdateTextFromTerms()
		{
			this.tmpTitle.text = "Mod.io Terms of Use";
			this.tmpBody.text = "Loading...";
			this.cachedTermsText = this.termsOfUse.TermsText + "\n\n";
			this.cachedTermsText = this.cachedTermsText + this.FormatAgreementText(this.fullTermsOfUse) + "\n\n\n";
			this.cachedTermsText += this.FormatAgreementText(this.fullPrivacyPolicy);
			this.tmpBody.text = this.cachedTermsText;
		}

		private string FormatAgreementText(Agreement agreement)
		{
			string text = string.Concat(new string[]
			{
				agreement.Name,
				"\n\nEffective Date: ",
				agreement.DateLive.ToLongDateString(),
				"\n\n",
				agreement.Content
			});
			text = Regex.Replace(text, "<!--[^>]*(-->)", "");
			text = text.Replace("<h1>", "<b>");
			text = text.Replace("</h1>", "</b>");
			text = text.Replace("<h2>", "<b>");
			text = text.Replace("</h2>", "</b>");
			text = text.Replace("<h3>", "<b>");
			text = text.Replace("</h3>", "</b>");
			text = text.Replace("<hr>", "");
			text = text.Replace("<br>", "\n");
			text = text.Replace("</li>", "</indent>\n");
			text = text.Replace("<strong>", "<b>");
			text = text.Replace("</strong>", "</b>");
			text = text.Replace("<em>", "<i>");
			text = text.Replace("</em>", "</i>");
			text = Regex.Replace(text, "<a[^>]*>{1}", "");
			text = text.Replace("</a>", "");
			Match match = Regex.Match(text, "<p[^>]*align:center[^>]*>{1}");
			while (match.Success)
			{
				text = text.Remove(match.Index, match.Length);
				text = text.Insert(match.Index, "\n<align=\"center\">");
				int startIndex = text.IndexOf("</p>", match.Index, StringComparison.Ordinal);
				text = text.Remove(startIndex, 4);
				text = text.Insert(startIndex, "</align>");
				match = Regex.Match(text, "<p[^>]*align:center[^>]*>{1}");
			}
			text = text.Replace("<p>", "\n");
			text = text.Replace("</p>", "");
			text = Regex.Replace(text, "<ol[^>]*>{1}", "<ol>");
			int num = text.IndexOf("<ol>", StringComparison.OrdinalIgnoreCase);
			bool flag = num != -1;
			while (flag)
			{
				int num2 = text.IndexOf("</ol>", num, StringComparison.OrdinalIgnoreCase);
				text = text.Remove(num, "<ol>".Length);
				int num3 = text.IndexOf("<li>", num, StringComparison.OrdinalIgnoreCase);
				bool flag2 = num3 != -1;
				int num4 = 0;
				while (flag2)
				{
					text = text.Remove(num3, "<li>".Length);
					text = text.Insert(num3, this.GetStringForListItemIdx_LowerAlpha(num4++));
					num2 = text.IndexOf("</ol>", num, StringComparison.OrdinalIgnoreCase);
					num3 = text.IndexOf("<li>", num, StringComparison.OrdinalIgnoreCase);
					flag2 = (num3 != -1 && num3 < num2);
				}
				text = text.Remove(num2, "</ol>".Length);
				num = text.IndexOf("<ol>", StringComparison.OrdinalIgnoreCase);
				flag = (num != -1);
			}
			text = Regex.Replace(text, "<ul[^>]*>{1}", "<ul>");
			int num5 = text.IndexOf("<ul>", StringComparison.OrdinalIgnoreCase);
			bool flag3 = num5 != -1;
			while (flag3)
			{
				int num6 = text.IndexOf("</ul>", num5, StringComparison.OrdinalIgnoreCase);
				text = text.Remove(num5, "<ul>".Length);
				int num7 = text.IndexOf("<li>", num5, StringComparison.OrdinalIgnoreCase);
				bool flag4 = num7 != -1;
				while (flag4)
				{
					text = text.Remove(num7, "<li>".Length);
					text = text.Insert(num7, "  - <indent=5%>");
					num6 = text.IndexOf("</ul>", num5, StringComparison.OrdinalIgnoreCase);
					num7 = text.IndexOf("<li>", num5, StringComparison.OrdinalIgnoreCase);
					flag4 = (num7 != -1 && num7 < num6);
				}
				text = text.Remove(num6, "</ul>".Length);
				num5 = text.IndexOf("<ul>", StringComparison.OrdinalIgnoreCase);
				flag3 = (num5 != -1);
			}
			text = Regex.Replace(text, "<table[^>]*>{1}", "");
			text = text.Replace("<tbody>", "");
			text = text.Replace("<tr>", "");
			text = text.Replace("<td>", "");
			text = text.Replace("<center>", "");
			text = text.Replace("</table>", "");
			text = text.Replace("</tbody>", "");
			text = text.Replace("</tr>", "\n");
			text = text.Replace("</td>", "");
			return text.Replace("</center>", "");
		}

		private string GetStringForListItemIdx_LowerAlpha(int idx)
		{
			switch (idx)
			{
			case 0:
				return "  a. <indent=5%>";
			case 1:
				return "  b. <indent=5%>";
			case 2:
				return "  c. <indent=5%>";
			case 3:
				return "  d. <indent=5%>";
			case 4:
				return "  e. <indent=5%>";
			case 5:
				return "  f. <indent=5%>";
			case 6:
				return "  g. <indent=5%>";
			case 7:
				return "  h. <indent=5%>";
			case 8:
				return "  i. <indent=5%>";
			case 9:
				return "  j. <indent=5%>";
			case 10:
				return "  k. <indent=5%>";
			case 11:
				return "  l. <indent=5%>";
			case 12:
				return "  m. <indent=5%>";
			case 13:
				return "  n. <indent=5%>";
			case 14:
				return "  o. <indent=5%>";
			case 15:
				return "  p. <indent=5%>";
			case 16:
				return "  q. <indent=5%>";
			case 17:
				return "  r. <indent=5%>";
			case 18:
				return "  s. <indent=5%>";
			case 19:
				return "  t. <indent=5%>";
			case 20:
				return "  u. <indent=5%>";
			case 21:
				return "  v. <indent=5%>";
			case 22:
				return "  w. <indent=5%>";
			case 23:
				return "  x. <indent=5%>";
			case 24:
				return "  y. <indent=5%>";
			case 25:
				return "  z. <indent=5%>";
			default:
				return "";
			}
		}

		[SerializeField]
		private string confirmString = "Press and Hold to Confirm";

		private static ModIOTermsOfUse_v2 modioTermsInstance;

		private TermsOfUse termsOfUse;

		private Agreement fullTermsOfUse;

		private Agreement fullPrivacyPolicy;

		private string cachedTermsText;
	}
}
