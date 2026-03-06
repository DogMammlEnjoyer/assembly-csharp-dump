using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio.API.SchemaDefinitions;

namespace Modio
{
	public class TermsOfUse
	{
		public string TermsText { get; private set; }

		public string AgreeText { get; private set; }

		public string DisagreeText { get; private set; }

		public TermsOfUseLink[] Links { get; private set; }

		[return: TupleElementNames(new string[]
		{
			"error",
			"result"
		})]
		public static Task<ValueTuple<Error, TermsOfUse>> Get()
		{
			TermsOfUse.<Get>d__17 <Get>d__;
			<Get>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, TermsOfUse>>.Create();
			<Get>d__.<>1__state = -1;
			<Get>d__.<>t__builder.Start<TermsOfUse.<Get>d__17>(ref <Get>d__);
			return <Get>d__.<>t__builder.Task;
		}

		public TermsOfUseLink GetLink(LinkType type)
		{
			foreach (TermsOfUseLink termsOfUseLink in this.Links)
			{
				if (termsOfUseLink.type == type)
				{
					return termsOfUseLink;
				}
			}
			ModioLog error = ModioLog.Error;
			if (error != null)
			{
				error.Log("Could not find " + type.ToString() + " link in Terms of Use! The API may have changed!");
			}
			return default(TermsOfUseLink);
		}

		private static TermsOfUse ConvertTermsObjectToTermsOfUse(TermsObject termsObject)
		{
			TermsOfUse termsOfUse = new TermsOfUse();
			termsOfUse.TermsText = termsObject.Plaintext;
			termsOfUse.AgreeText = termsObject.Buttons.Agree.Text;
			termsOfUse.DisagreeText = termsObject.Buttons.Disagree.Text;
			TermsOfUseLink termsOfUseLink = new TermsOfUseLink
			{
				type = LinkType.Website,
				text = termsObject.Links.Website.Text,
				url = termsObject.Links.Website.Url,
				required = termsObject.Links.Website.Required
			};
			TermsOfUseLink termsOfUseLink2 = new TermsOfUseLink
			{
				type = LinkType.Terms,
				text = termsObject.Links.Terms.Text,
				url = termsObject.Links.Terms.Url,
				required = termsObject.Links.Terms.Required
			};
			TermsOfUseLink termsOfUseLink3 = new TermsOfUseLink
			{
				type = LinkType.Privacy,
				text = termsObject.Links.Privacy.Text,
				url = termsObject.Links.Privacy.Url,
				required = termsObject.Links.Privacy.Required
			};
			TermsOfUseLink termsOfUseLink4 = new TermsOfUseLink
			{
				type = LinkType.Manage,
				text = termsObject.Links.Manage.Text,
				url = termsObject.Links.Manage.Url,
				required = termsObject.Links.Manage.Required
			};
			termsOfUse.Links = new TermsOfUseLink[]
			{
				termsOfUseLink,
				termsOfUseLink2,
				termsOfUseLink3,
				termsOfUseLink4
			};
			return termsOfUse;
		}

		private static Dictionary<string, TermsOfUse> _termsCache = new Dictionary<string, TermsOfUse>();
	}
}
