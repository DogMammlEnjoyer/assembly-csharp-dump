using System;
using System.Collections.Generic;
using System.Globalization;
using Valve.Newtonsoft.Json.Utilities;

namespace Valve.Newtonsoft.Json.Linq.JsonPath
{
	internal class ArraySliceFilter : PathFilter
	{
		public int? Start { get; set; }

		public int? End { get; set; }

		public int? Step { get; set; }

		public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			if (this.Step == 0)
			{
				throw new JsonException("Step cannot be zero.");
			}
			foreach (JToken t in current)
			{
				JArray a = t as JArray;
				if (a != null)
				{
					int stepCount = this.Step ?? 1;
					int num = this.Start ?? ((stepCount > 0) ? 0 : (a.Count - 1));
					int stopIndex = this.End ?? ((stepCount > 0) ? a.Count : -1);
					if (this.Start < 0)
					{
						num = a.Count + num;
					}
					if (this.End < 0)
					{
						stopIndex = a.Count + stopIndex;
					}
					num = Math.Max(num, (stepCount > 0) ? 0 : int.MinValue);
					num = Math.Min(num, (stepCount > 0) ? a.Count : (a.Count - 1));
					stopIndex = Math.Max(stopIndex, -1);
					stopIndex = Math.Min(stopIndex, a.Count);
					bool positiveStep = stepCount > 0;
					if (this.IsValid(num, stopIndex, positiveStep))
					{
						int i = num;
						while (this.IsValid(i, stopIndex, positiveStep))
						{
							yield return a[i];
							i += stepCount;
						}
					}
					else if (errorWhenNoMatch)
					{
						throw new JsonException("Array slice of {0} to {1} returned no results.".FormatWith(CultureInfo.InvariantCulture, (this.Start != null) ? this.Start.GetValueOrDefault().ToString(CultureInfo.InvariantCulture) : "*", (this.End != null) ? this.End.GetValueOrDefault().ToString(CultureInfo.InvariantCulture) : "*"));
					}
				}
				else if (errorWhenNoMatch)
				{
					throw new JsonException("Array slice is not valid on {0}.".FormatWith(CultureInfo.InvariantCulture, t.GetType().Name));
				}
				a = null;
				t = null;
			}
			IEnumerator<JToken> enumerator = null;
			yield break;
			yield break;
		}

		private bool IsValid(int index, int stopIndex, bool positiveStep)
		{
			if (positiveStep)
			{
				return index < stopIndex;
			}
			return index > stopIndex;
		}
	}
}
