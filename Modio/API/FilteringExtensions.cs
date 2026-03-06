using System;

namespace Modio.API
{
	public static class FilteringExtensions
	{
		public static string ClearText(this Filtering filtering)
		{
			string result;
			switch (filtering)
			{
			case Filtering.Like:
				result = "-lk";
				break;
			case Filtering.Not:
				result = "-not";
				break;
			case Filtering.NotLike:
				result = "-not-lk";
				break;
			case Filtering.In:
				result = "-in";
				break;
			case Filtering.NotIn:
				result = "-not-in";
				break;
			case Filtering.Max:
				result = "-max";
				break;
			case Filtering.Min:
				result = "-min";
				break;
			case Filtering.BitwiseAnd:
				result = "-bitwise-and";
				break;
			default:
				result = string.Empty;
				break;
			}
			return result;
		}
	}
}
