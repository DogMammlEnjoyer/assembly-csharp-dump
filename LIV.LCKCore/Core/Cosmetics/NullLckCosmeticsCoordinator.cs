using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Scripting;

namespace Liv.Lck.Core.Cosmetics
{
	[Preserve]
	public class NullLckCosmeticsCoordinator : ILckCosmeticsCoordinator
	{
		public event Action<LckAvailableCosmeticInfo> OnCosmeticAvailable;

		[Preserve]
		public NullLckCosmeticsCoordinator()
		{
		}

		public Task InitializeLocalCosmeticsAsync()
		{
			return Task.CompletedTask;
		}

		public Task<Result<bool>> GetUserCosmeticsForSessionAsync(IEnumerable<string> playerIds, string sessionId)
		{
			return Task.FromResult<Result<bool>>(Result<bool>.NewSuccess(true));
		}

		public Task<Result<bool>> AnnouncePlayerPresenceForSessionAsync(string playerId, string sessionId)
		{
			return Task.FromResult<Result<bool>>(Result<bool>.NewSuccess(true));
		}
	}
}
