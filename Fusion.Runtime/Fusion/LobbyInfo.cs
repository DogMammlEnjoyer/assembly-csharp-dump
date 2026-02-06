using System;

namespace Fusion
{
	public class LobbyInfo
	{
		public bool IsValid { get; internal set; }

		public string Name { get; internal set; }

		public string Region { get; internal set; }

		internal void Reset()
		{
			this.IsValid = false;
			this.Name = null;
			this.Region = null;
		}
	}
}
