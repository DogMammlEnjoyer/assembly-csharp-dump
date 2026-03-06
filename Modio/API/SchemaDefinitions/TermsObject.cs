using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct TermsObject
	{
		[JsonConstructor]
		public TermsObject(string plaintext, string html, TermsObject.EmbeddedButtons buttons, TermsObject.EmbeddedLinks links)
		{
			this.Plaintext = plaintext;
			this.Html = html;
			this.Buttons = buttons;
			this.Links = links;
		}

		internal readonly string Plaintext;

		internal readonly string Html;

		internal readonly TermsObject.EmbeddedButtons Buttons;

		internal readonly TermsObject.EmbeddedLinks Links;

		[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		internal readonly struct EmbeddedButtons
		{
			[JsonConstructor]
			public EmbeddedButtons(TermsObject.EmbeddedButtons.EmbeddedAgree agree, TermsObject.EmbeddedButtons.EmbeddedDisagree disagree)
			{
				this.Agree = agree;
				this.Disagree = disagree;
			}

			internal readonly TermsObject.EmbeddedButtons.EmbeddedAgree Agree;

			internal readonly TermsObject.EmbeddedButtons.EmbeddedDisagree Disagree;

			[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
			internal readonly struct EmbeddedAgree
			{
				[JsonConstructor]
				public EmbeddedAgree(string text)
				{
					this.Text = text;
				}

				internal readonly string Text;
			}

			[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
			internal readonly struct EmbeddedDisagree
			{
				[JsonConstructor]
				public EmbeddedDisagree(string text)
				{
					this.Text = text;
				}

				internal readonly string Text;
			}
		}

		[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		internal readonly struct EmbeddedLinks
		{
			[JsonConstructor]
			public EmbeddedLinks(TermsObject.EmbeddedLinks.EmbeddedWebsite website, TermsObject.EmbeddedLinks.EmbeddedTerms terms, TermsObject.EmbeddedLinks.EmbeddedPrivacy privacy, TermsObject.EmbeddedLinks.EmbeddedRefund refund, TermsObject.EmbeddedLinks.EmbeddedManage manage)
			{
				this.Website = website;
				this.Terms = terms;
				this.Privacy = privacy;
				this.Refund = refund;
				this.Manage = manage;
			}

			internal readonly TermsObject.EmbeddedLinks.EmbeddedWebsite Website;

			internal readonly TermsObject.EmbeddedLinks.EmbeddedTerms Terms;

			internal readonly TermsObject.EmbeddedLinks.EmbeddedPrivacy Privacy;

			internal readonly TermsObject.EmbeddedLinks.EmbeddedRefund Refund;

			internal readonly TermsObject.EmbeddedLinks.EmbeddedManage Manage;

			[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
			internal readonly struct EmbeddedWebsite
			{
				[JsonConstructor]
				public EmbeddedWebsite(string text, string url, bool required)
				{
					this.Text = text;
					this.Url = url;
					this.Required = required;
				}

				internal readonly string Text;

				internal readonly string Url;

				internal readonly bool Required;
			}

			[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
			internal readonly struct EmbeddedTerms
			{
				[JsonConstructor]
				public EmbeddedTerms(string text, string url, bool required)
				{
					this.Text = text;
					this.Url = url;
					this.Required = required;
				}

				internal readonly string Text;

				internal readonly string Url;

				internal readonly bool Required;
			}

			[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
			internal readonly struct EmbeddedPrivacy
			{
				[JsonConstructor]
				public EmbeddedPrivacy(string text, string url, bool required)
				{
					this.Text = text;
					this.Url = url;
					this.Required = required;
				}

				internal readonly string Text;

				internal readonly string Url;

				internal readonly bool Required;
			}

			[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
			internal readonly struct EmbeddedRefund
			{
				[JsonConstructor]
				public EmbeddedRefund(string text, string url, bool required)
				{
					this.Text = text;
					this.Url = url;
					this.Required = required;
				}

				internal readonly string Text;

				internal readonly string Url;

				internal readonly bool Required;
			}

			[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
			internal readonly struct EmbeddedManage
			{
				[JsonConstructor]
				public EmbeddedManage(string text, string url, bool required)
				{
					this.Text = text;
					this.Url = url;
					this.Required = required;
				}

				internal readonly string Text;

				internal readonly string Url;

				internal readonly bool Required;
			}
		}
	}
}
