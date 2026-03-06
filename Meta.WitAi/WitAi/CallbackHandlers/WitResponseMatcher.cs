using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Meta.WitAi.Attributes;
using Meta.WitAi.Json;
using Meta.WitAi.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Meta.WitAi.CallbackHandlers
{
	[AddComponentMenu("Wit.ai/Response Matchers/Response Matcher")]
	public class WitResponseMatcher : WitIntentMatcher
	{
		protected override string OnValidateResponse(WitResponseNode response, bool isEarlyResponse)
		{
			string text = base.OnValidateResponse(response, isEarlyResponse);
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			if (isEarlyResponse && !this.ValueMatches(response))
			{
				return "No value matches";
			}
			return string.Empty;
		}

		protected override void OnResponseInvalid(WitResponseNode response, string error)
		{
			if (response.GetIntents().Length != 0 || response.EntityCount() > 0)
			{
				StringEvent stringEvent = this.onDidNotMatch;
				if (stringEvent != null)
				{
					stringEvent.Invoke(response.GetTranscription());
				}
			}
			if (response.GetIntents().Length == 0)
			{
				StringEvent stringEvent2 = this.onOutOfDomain;
				if (stringEvent2 == null)
				{
					return;
				}
				stringEvent2.Invoke(response.GetTranscription());
			}
		}

		protected override void OnResponseSuccess(WitResponseNode response)
		{
			if (this.ValueMatches(response))
			{
				for (int i = 0; i < this.formattedValueEvents.Length; i++)
				{
					FormattedValueEvents formattedValueEvents = this.formattedValueEvents[i];
					string text = formattedValueEvents.format;
					for (int j = 0; j < this.valueMatchers.Length; j++)
					{
						string stringValue = this.valueMatchers[j].Reference.GetStringValue(response);
						if (!string.IsNullOrEmpty(formattedValueEvents.format))
						{
							if (!string.IsNullOrEmpty(stringValue))
							{
								text = WitResponseMatcher.valueRegex.Replace(text, stringValue, 1);
								text = text.Replace("{" + j.ToString() + "}", stringValue);
							}
							else if (text.Contains("{" + j.ToString() + "}"))
							{
								text = "";
								break;
							}
						}
					}
					if (!string.IsNullOrEmpty(text))
					{
						ValueEvent onFormattedValueEvent = formattedValueEvents.onFormattedValueEvent;
						if (onFormattedValueEvent != null)
						{
							onFormattedValueEvent.Invoke(text);
						}
					}
				}
			}
			else
			{
				StringEvent stringEvent = this.onDidNotMatch;
				if (stringEvent != null)
				{
					stringEvent.Invoke(response.GetTranscription());
				}
			}
			List<string> list = new List<string>();
			foreach (ValuePathMatcher valuePathMatcher in this.valueMatchers)
			{
				string stringValue2 = valuePathMatcher.Reference.GetStringValue(response);
				list.Add(stringValue2);
				if (valuePathMatcher.ConfidenceReference != null)
				{
					WitResponseHandler.RefreshConfidenceRange(this.ValueMatches(response, valuePathMatcher) ? valuePathMatcher.ConfidenceReference.GetFloatValue(response) : 0f, valuePathMatcher.confidenceRanges, valuePathMatcher.allowConfidenceOverlap);
				}
			}
			this.onMultiValueEvent.Invoke(list.ToArray());
		}

		private bool ValueMatches(WitResponseNode response)
		{
			bool flag = true;
			int num = 0;
			while (num < this.valueMatchers.Length && flag)
			{
				flag &= this.ValueMatches(response, this.valueMatchers[num]);
				num++;
			}
			return flag;
		}

		private bool ValueMatches(WitResponseNode response, ValuePathMatcher matcher)
		{
			string stringValue = matcher.Reference.GetStringValue(response);
			bool flag = !matcher.contentRequired || !string.IsNullOrEmpty(stringValue);
			switch (matcher.matchMethod)
			{
			case MatchMethod.Text:
				flag &= (stringValue == matcher.matchValue);
				break;
			case MatchMethod.RegularExpression:
				flag &= Regex.Match(stringValue, matcher.matchValue).Success;
				break;
			case MatchMethod.IntegerComparison:
				flag &= this.CompareInt(stringValue, matcher);
				break;
			case MatchMethod.FloatComparison:
				flag &= this.CompareFloat(stringValue, matcher);
				break;
			case MatchMethod.DoubleComparison:
				flag &= this.CompareDouble(stringValue, matcher);
				break;
			}
			return flag;
		}

		private bool CompareDouble(string value, ValuePathMatcher matcher)
		{
			double num;
			if (!double.TryParse(value, out num))
			{
				return false;
			}
			double num2 = double.Parse(matcher.matchValue);
			switch (matcher.comparisonMethod)
			{
			case ComparisonMethod.Equals:
				return Math.Abs(num - num2) < matcher.floatingPointComparisonTolerance;
			case ComparisonMethod.NotEquals:
				return Math.Abs(num - num2) > matcher.floatingPointComparisonTolerance;
			case ComparisonMethod.Greater:
				return num > num2;
			case ComparisonMethod.GreaterThanOrEqualTo:
				return num >= num2;
			case ComparisonMethod.Less:
				return num < num2;
			case ComparisonMethod.LessThanOrEqualTo:
				return num <= num2;
			default:
				return false;
			}
		}

		private bool CompareFloat(string value, ValuePathMatcher matcher)
		{
			float num;
			if (!float.TryParse(value, out num))
			{
				return false;
			}
			float num2 = float.Parse(matcher.matchValue);
			switch (matcher.comparisonMethod)
			{
			case ComparisonMethod.Equals:
				return (double)Math.Abs(num - num2) < matcher.floatingPointComparisonTolerance;
			case ComparisonMethod.NotEquals:
				return (double)Math.Abs(num - num2) > matcher.floatingPointComparisonTolerance;
			case ComparisonMethod.Greater:
				return num > num2;
			case ComparisonMethod.GreaterThanOrEqualTo:
				return num >= num2;
			case ComparisonMethod.Less:
				return num < num2;
			case ComparisonMethod.LessThanOrEqualTo:
				return num <= num2;
			default:
				return false;
			}
		}

		private bool CompareInt(string value, ValuePathMatcher matcher)
		{
			int num;
			if (!int.TryParse(value, out num))
			{
				return false;
			}
			int num2 = int.Parse(matcher.matchValue);
			switch (matcher.comparisonMethod)
			{
			case ComparisonMethod.Equals:
				return num == num2;
			case ComparisonMethod.NotEquals:
				return num != num2;
			case ComparisonMethod.Greater:
				return num > num2;
			case ComparisonMethod.GreaterThanOrEqualTo:
				return num >= num2;
			case ComparisonMethod.Less:
				return num < num2;
			case ComparisonMethod.LessThanOrEqualTo:
				return num <= num2;
			default:
				return false;
			}
		}

		[FormerlySerializedAs("valuePaths")]
		[Header("Value Matching")]
		[SerializeField]
		public ValuePathMatcher[] valueMatchers;

		[Header("Output")]
		[SerializeField]
		private FormattedValueEvents[] formattedValueEvents;

		[SerializeField]
		private MultiValueEvent onMultiValueEvent = new MultiValueEvent();

		[TooltipBox("Triggered if the matching conditions did not match. The parameter will be the transcription that was received. This will only trigger if there were values for intents or entities, but those values didn't match this matcher.")]
		[SerializeField]
		private StringEvent onDidNotMatch = new StringEvent();

		[TooltipBox("Triggered if a request was checked and no intents were found. This will still trigger if entities match and only applies to intents. The parameter will be the transcription.")]
		[SerializeField]
		private StringEvent onOutOfDomain = new StringEvent();

		private static Regex valueRegex = new Regex(Regex.Escape("{value}"), RegexOptions.Compiled);
	}
}
