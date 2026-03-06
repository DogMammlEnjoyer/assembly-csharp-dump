using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace UnityEngine.Rendering
{
	public static class RemoveRangeExtensions
	{
		[CollectionAccess(CollectionAccessType.ModifyExistingContent)]
		[MustUseReturnValue]
		public static bool TryRemoveElementsInRange<TValue>([DisallowNull] this IList<TValue> list, int index, int count, [NotNullWhen(false)] out Exception error)
		{
			try
			{
				List<TValue> list2 = list as List<TValue>;
				if (list2 != null)
				{
					list2.RemoveRange(index, count);
				}
				else
				{
					for (int i = count; i > 0; i--)
					{
						list.RemoveAt(index);
					}
				}
			}
			catch (Exception ex)
			{
				error = ex;
				return false;
			}
			error = null;
			return true;
		}
	}
}
