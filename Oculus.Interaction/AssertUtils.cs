using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Oculus.Interaction
{
	public static class AssertUtils
	{
		[Conditional("UNITY_ASSERTIONS")]
		public static void AssertIsTrue(this Component component, bool value, string whyItFailed = null, string whereItFailed = null, string howToFix = null)
		{
			string name = component.name;
			string name2 = component.GetType().Name;
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AssertAspect<TValue>(this Component component, TValue aspect, string aspectLocation, string whyItFailed = null, string whereFailed = null, string howToFix = null) where TValue : class
		{
			string name = component.name;
			string name2 = component.GetType().Name;
			AssertUtils.Nicify(aspectLocation);
			string name3 = typeof(TValue).Name;
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AssertField<TValue>(this Component component, TValue value, string variableName, string whyItFailed = null, string whereItFailed = null, string howToFix = null) where TValue : class
		{
			string name = component.name;
			string name2 = component.GetType().Name;
			AssertUtils.Nicify(variableName);
			string name3 = typeof(TValue).Name;
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AssertCollectionField<TValue>(this Component component, IEnumerable<TValue> value, string variableName, string whyItFailed = null, string whereFailed = null, string howToFix = null)
		{
			string name = component.name;
			string name2 = component.GetType().Name;
			AssertUtils.Nicify(variableName);
			string name3 = typeof(TValue).Name;
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AssertCollectionItems<TValue>(this Component component, IEnumerable<TValue> value, string variableName, string whyItFailed = null, string whereItFailed = null, string howToFix = null)
		{
			string name = component.name;
			string name2 = component.GetType().Name;
			AssertUtils.Nicify(variableName);
			string name3 = typeof(TValue).Name;
			int num = 0;
			foreach (TValue tvalue in value)
			{
				num++;
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void WarnInspectorCollectionItems(this Component component, IEnumerable<Object> value, string variableName, string whyItFailed = null, string whereItFailed = null, string howToFix = null)
		{
			string name = component.name;
			string name2 = component.GetType().Name;
			string text = AssertUtils.Nicify(variableName);
			string name3 = typeof(Object).Name;
			int num = 0;
			foreach (Object x in value)
			{
				string message = (whereItFailed ?? string.Concat(new string[]
				{
					"At GameObject <color=#3366ff><b>",
					name,
					"</b></color>, component <b>",
					name2,
					"</b>. "
				})) + (whyItFailed ?? string.Format("Invalid item in the collection <b>{0}</b> at index <b>{1}</b>. ", text, num)) + (howToFix ?? string.Format("Assign a <b>{0}</b> to the collection <b>{1}</b> at index <b>{2}</b> or remove the entry. ", name3, text, num));
				if (x == null)
				{
					Debug.LogWarning(message, component);
				}
				num++;
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void LogWarning(this Component component, string whyItFailed = null, string whereItFailed = null, string howToFix = null)
		{
			string name = component.name;
			string name2 = component.GetType().Name;
			Debug.LogWarning((whereItFailed ?? string.Concat(new string[]
			{
				"At GameObject <color=#3366ff><b>",
				name,
				"</b></color>, component <b>",
				name2,
				"</b>. "
			})) + (whyItFailed ?? string.Empty) + (howToFix ?? string.Empty), component);
		}

		public static string Nicify(string variableName)
		{
			variableName = Regex.Replace(variableName, "_([a-z])", (Match match) => match.Value.ToUpper(), RegexOptions.Compiled);
			variableName = Regex.Replace(variableName, "m_|_", " ", RegexOptions.Compiled);
			variableName = Regex.Replace(variableName, "k([A-Z])", "$1", RegexOptions.Compiled);
			variableName = Regex.Replace(variableName, "([A-Z])", " $1", RegexOptions.Compiled);
			variableName = variableName.Trim();
			return variableName;
		}

		public const string HiglightColor = "#3366ff";
	}
}
