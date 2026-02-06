using System;
using System.Text;
using ExitGames.Client.Photon;

namespace Fusion.Photon.Realtime
{
	internal class SystemConnectionSummary
	{
		public SystemConnectionSummary(LoadBalancingClient client)
		{
			bool flag = client != null;
			if (flag)
			{
				this.UsedProtocol = (byte)(client.LoadBalancingPeer.UsedProtocol & (ConnectionProtocol)7);
				this.SocketErrorCode = client.LoadBalancingPeer.SocketErrorCode;
			}
			this.AppQuits = ConnectionHandler.AppQuits;
			this.AppPause = ConnectionHandler.AppPause;
			this.AppPauseRecent = ConnectionHandler.AppPauseRecent;
			this.AppOutOfFocus = ConnectionHandler.AppOutOfFocus;
			this.AppOutOfFocusRecent = ConnectionHandler.AppOutOfFocusRecent;
			this.NetworkReachable = ConnectionHandler.IsNetworkReachableUnity();
			this.ErrorCodeFits = (this.SocketErrorCode <= 32767);
			this.ErrorCodeWinSock = true;
		}

		public SystemConnectionSummary(int summary)
		{
			this.Version = SystemConnectionSummary.GetBits(ref summary, 28, 15);
			this.UsedProtocol = SystemConnectionSummary.GetBits(ref summary, 25, 7);
			this.AppQuits = SystemConnectionSummary.GetBit(ref summary, 23);
			this.AppPause = SystemConnectionSummary.GetBit(ref summary, 22);
			this.AppPauseRecent = SystemConnectionSummary.GetBit(ref summary, 21);
			this.AppOutOfFocus = SystemConnectionSummary.GetBit(ref summary, 20);
			this.AppOutOfFocusRecent = SystemConnectionSummary.GetBit(ref summary, 19);
			this.NetworkReachable = SystemConnectionSummary.GetBit(ref summary, 18);
			this.ErrorCodeFits = SystemConnectionSummary.GetBit(ref summary, 17);
			this.ErrorCodeWinSock = SystemConnectionSummary.GetBit(ref summary, 16);
			this.SocketErrorCode = (summary & 65535);
		}

		public int ToInt()
		{
			int num = 0;
			SystemConnectionSummary.SetBits(ref num, this.Version, 28);
			SystemConnectionSummary.SetBits(ref num, this.UsedProtocol, 25);
			SystemConnectionSummary.SetBit(ref num, this.AppQuits, 23);
			SystemConnectionSummary.SetBit(ref num, this.AppPause, 22);
			SystemConnectionSummary.SetBit(ref num, this.AppPauseRecent, 21);
			SystemConnectionSummary.SetBit(ref num, this.AppOutOfFocus, 20);
			SystemConnectionSummary.SetBit(ref num, this.AppOutOfFocusRecent, 19);
			SystemConnectionSummary.SetBit(ref num, this.NetworkReachable, 18);
			SystemConnectionSummary.SetBit(ref num, this.ErrorCodeFits, 17);
			SystemConnectionSummary.SetBit(ref num, this.ErrorCodeWinSock, 16);
			int num2 = this.SocketErrorCode & 65535;
			num |= num2;
			return num;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			string arg = SystemConnectionSummary.ProtocolIdToName[(int)this.UsedProtocol];
			stringBuilder.Append(string.Format("SCS v{0} {1} SocketErrorCode: {2} ", this.Version, arg, this.SocketErrorCode));
			bool appQuits = this.AppQuits;
			if (appQuits)
			{
				stringBuilder.Append("AppQuits ");
			}
			bool appPause = this.AppPause;
			if (appPause)
			{
				stringBuilder.Append("AppPause ");
			}
			bool flag = !this.AppPause && this.AppPauseRecent;
			if (flag)
			{
				stringBuilder.Append("AppPauseRecent ");
			}
			bool appOutOfFocus = this.AppOutOfFocus;
			if (appOutOfFocus)
			{
				stringBuilder.Append("AppOutOfFocus ");
			}
			bool flag2 = !this.AppOutOfFocus && this.AppOutOfFocusRecent;
			if (flag2)
			{
				stringBuilder.Append("AppOutOfFocusRecent ");
			}
			bool flag3 = !this.NetworkReachable;
			if (flag3)
			{
				stringBuilder.Append("NetworkUnreachable ");
			}
			bool flag4 = !this.ErrorCodeFits;
			if (flag4)
			{
				stringBuilder.Append("ErrorCodeRangeExceeded ");
			}
			bool errorCodeWinSock = this.ErrorCodeWinSock;
			if (errorCodeWinSock)
			{
				stringBuilder.Append("WinSock");
			}
			else
			{
				stringBuilder.Append("BSDSock");
			}
			return stringBuilder.ToString();
		}

		public static bool GetBit(ref int value, int bitpos)
		{
			int num = value >> bitpos & 1;
			return num != 0;
		}

		public static byte GetBits(ref int value, int bitpos, byte mask)
		{
			int num = value >> bitpos & (int)mask;
			return (byte)num;
		}

		public static void SetBit(ref int value, bool bitval, int bitpos)
		{
			if (bitval)
			{
				value |= 1 << bitpos;
			}
			else
			{
				value &= ~(1 << bitpos);
			}
		}

		public static void SetBits(ref int value, byte bitvals, int bitpos)
		{
			value |= (int)bitvals << bitpos;
		}

		public readonly byte Version = 0;

		public byte UsedProtocol;

		public bool AppQuits;

		public bool AppPause;

		public bool AppPauseRecent;

		public bool AppOutOfFocus;

		public bool AppOutOfFocusRecent;

		public bool NetworkReachable;

		public bool ErrorCodeFits;

		public bool ErrorCodeWinSock;

		public int SocketErrorCode;

		private static readonly string[] ProtocolIdToName = new string[]
		{
			"UDP",
			"TCP",
			"2(N/A)",
			"3(N/A)",
			"WS",
			"WSS",
			"6(N/A)",
			"7WebRTC"
		};

		private class SCSBitPos
		{
			public const int Version = 28;

			public const int UsedProtocol = 25;

			public const int EmptyBit = 24;

			public const int AppQuits = 23;

			public const int AppPause = 22;

			public const int AppPauseRecent = 21;

			public const int AppOutOfFocus = 20;

			public const int AppOutOfFocusRecent = 19;

			public const int NetworkReachable = 18;

			public const int ErrorCodeFits = 17;

			public const int ErrorCodeWinSock = 16;
		}
	}
}
