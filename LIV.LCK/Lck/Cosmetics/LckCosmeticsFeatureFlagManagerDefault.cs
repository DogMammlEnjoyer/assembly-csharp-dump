using System;
using System.Threading.Tasks;
using UnityEngine.Scripting;

namespace Liv.Lck.Cosmetics
{
	[Preserve]
	public class LckCosmeticsFeatureFlagManagerDefault : ILckCosmeticsFeatureFlagManager
	{
		[Preserve]
		public LckCosmeticsFeatureFlagManagerDefault()
		{
		}

		public Task<bool> IsEnabledAsync()
		{
			return Task.FromResult<bool>(true);
		}
	}
}
