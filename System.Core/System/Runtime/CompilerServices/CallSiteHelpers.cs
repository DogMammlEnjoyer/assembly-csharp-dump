using System;
using System.Dynamic;
using System.Reflection;

namespace System.Runtime.CompilerServices
{
	/// <summary>Class that contains helper methods for DLR CallSites.</summary>
	public static class CallSiteHelpers
	{
		/// <summary>Checks if a <see cref="T:System.Reflection.MethodBase" /> is internally used by DLR and should not be displayed on the language code's stack.</summary>
		/// <param name="mb">The input <see cref="T:System.Reflection.MethodBase" /></param>
		/// <returns>True if the input <see cref="T:System.Reflection.MethodBase" /> is internally used by DLR and should not be displayed on the language code's stack. Otherwise, false.</returns>
		public static bool IsInternalFrame(MethodBase mb)
		{
			return (mb.Name == "CallSite.Target" && mb.GetType() != CallSiteHelpers.s_knownNonDynamicMethodType) || mb.DeclaringType == typeof(UpdateDelegates);
		}

		private static readonly Type s_knownNonDynamicMethodType = typeof(object).GetMethod("ToString").GetType();
	}
}
