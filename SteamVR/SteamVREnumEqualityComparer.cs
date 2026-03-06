using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Valve.VR
{
	internal struct SteamVREnumEqualityComparer<TEnum> : IEqualityComparer<TEnum> where TEnum : struct
	{
		public bool Equals(TEnum firstEnum, TEnum secondEnum)
		{
			return SteamVREnumEqualityComparer<TEnum>.BoxAvoidance.ToInt(firstEnum) == SteamVREnumEqualityComparer<TEnum>.BoxAvoidance.ToInt(secondEnum);
		}

		public int GetHashCode(TEnum firstEnum)
		{
			return SteamVREnumEqualityComparer<TEnum>.BoxAvoidance.ToInt(firstEnum);
		}

		private static class BoxAvoidance
		{
			public static int ToInt(TEnum enu)
			{
				return SteamVREnumEqualityComparer<TEnum>.BoxAvoidance._wrapper(enu);
			}

			static BoxAvoidance()
			{
				ParameterExpression parameterExpression = Expression.Parameter(typeof(TEnum), null);
				SteamVREnumEqualityComparer<TEnum>.BoxAvoidance._wrapper = Expression.Lambda<Func<TEnum, int>>(Expression.ConvertChecked(parameterExpression, typeof(int)), new ParameterExpression[]
				{
					parameterExpression
				}).Compile();
			}

			private static readonly Func<TEnum, int> _wrapper;
		}
	}
}
