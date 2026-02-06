using System;
using ExitGames.Client.Photon;

namespace Photon.Realtime
{
	public class ErrorInfo
	{
		public ErrorInfo(EventData eventData)
		{
			this.Info = (eventData[218] as string);
		}

		public override string ToString()
		{
			return string.Format("ErrorInfo: {0}", this.Info);
		}

		public readonly string Info;
	}
}
