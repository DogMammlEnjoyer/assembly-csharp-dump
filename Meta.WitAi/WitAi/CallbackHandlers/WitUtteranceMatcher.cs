using System;
using System.Text.RegularExpressions;
using Meta.WitAi.Json;
using Meta.WitAi.Utilities;
using UnityEngine;

namespace Meta.WitAi.CallbackHandlers
{
	[AddComponentMenu("Wit.ai/Response Matchers/Utterance Matcher")]
	public class WitUtteranceMatcher : WitResponseHandler
	{
		protected override string OnValidateResponse(WitResponseNode response, bool isEarlyResponse)
		{
			string value = response["text"].Value;
			if (!this.IsMatch(value))
			{
				return "Required utterance does not match";
			}
			return "";
		}

		protected override void OnResponseInvalid(WitResponseNode response, string error)
		{
		}

		protected override void OnResponseSuccess(WitResponseNode response)
		{
			string value = response["text"].Value;
			StringEvent stringEvent = this.onUtteranceMatched;
			if (stringEvent == null)
			{
				return;
			}
			stringEvent.Invoke(value);
		}

		private bool IsMatch(string text)
		{
			if (this.useRegex)
			{
				if (this.regex == null)
				{
					this.regex = new Regex(this.searchText, RegexOptions.IgnoreCase | RegexOptions.Compiled);
				}
				Match match = this.regex.Match(text);
				if (match.Success)
				{
					if (this.exactMatch)
					{
						match.Value == text;
						return true;
					}
					return true;
				}
			}
			else
			{
				if (this.exactMatch && text.ToLower() == this.searchText.ToLower())
				{
					return true;
				}
				if (text.ToLower().Contains(this.searchText.ToLower()))
				{
					return true;
				}
			}
			return false;
		}

		[SerializeField]
		private string searchText;

		[SerializeField]
		private bool exactMatch = true;

		[SerializeField]
		private bool useRegex;

		[SerializeField]
		private StringEvent onUtteranceMatched = new StringEvent();

		private Regex regex;
	}
}
