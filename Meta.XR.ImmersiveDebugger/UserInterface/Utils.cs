using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.UserInterface
{
	internal static class Utils
	{
		internal static string ToDisplayText(this string input, int characterLimit = 22)
		{
			string text = Regex.Replace(input, "([a-z])([A-Z])", "$1 $2");
			text = Regex.Replace(text, "([A-Z]+)([A-Z][a-z])", "$1 $2");
			text = text.Replace("_", " ");
			text = char.ToUpper(text[0]).ToString() + text.Substring(1);
			if (text.Length > characterLimit)
			{
				text = text.Substring(0, characterLimit);
			}
			return text;
		}

		internal static Vector3 LerpPosition(Vector3 current, Vector3 target, float lerpSpeed)
		{
			if (Vector3.Distance(current, target) < 0.01f)
			{
				return target;
			}
			current = Vector3.Lerp(current, target, Time.deltaTime * lerpSpeed);
			return current;
		}

		internal static string ClampText(string text, int limit)
		{
			if (text.Length <= limit)
			{
				return text;
			}
			return text.Substring(0, limit);
		}

		private const int MaxLetterCountForTitle = 22;

		internal const int MaxLetterCountForMethod = 64;

		public const int DropDownMenuSortOrder = 5;

		public const int CursorSortOrder = 31000;
	}
}
