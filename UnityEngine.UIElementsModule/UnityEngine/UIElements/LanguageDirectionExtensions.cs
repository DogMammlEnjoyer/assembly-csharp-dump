using System;
using UnityEngine.TextCore;

namespace UnityEngine.UIElements
{
	internal static class LanguageDirectionExtensions
	{
		internal static LanguageDirection toTextCore(this LanguageDirection dir)
		{
			LanguageDirection result;
			if (dir > LanguageDirection.LTR)
			{
				if (dir != LanguageDirection.RTL)
				{
					throw new ArgumentOutOfRangeException("dir", dir, "impossible to convert value");
				}
				result = LanguageDirection.RTL;
			}
			else
			{
				result = LanguageDirection.LTR;
			}
			return result;
		}
	}
}
