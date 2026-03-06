using System;

namespace Modio.Authentication
{
	public interface IPotentialModioEmailAuthService
	{
		bool IsEmailPlatform { get; }
	}
}
