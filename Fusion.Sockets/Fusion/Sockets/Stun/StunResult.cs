using System;

namespace Fusion.Sockets.Stun
{
	internal class StunResult
	{
		public bool IsValid
		{
			get
			{
				return this.PublicEndPoint.IsValid && this.PrivateEndPoint.IsValid;
			}
		}

		public NetAddress PublicEndPoint { get; private set; } = default(NetAddress);

		public NetAddress PrivateEndPoint { get; private set; } = default(NetAddress);

		private StunResult(NetAddress publicEndPoint = default(NetAddress), NetAddress privateEndPoint = default(NetAddress))
		{
			this.PublicEndPoint = publicEndPoint;
			this.PrivateEndPoint = privateEndPoint;
		}

		public static StunResult BuildStunResult(NetAddress publicEndPoint1, NetAddress publicEndPoint2, NetAddress privateEndPoint)
		{
			StunResult stunResult = new StunResult(publicEndPoint1, privateEndPoint)
			{
				NatType = NATType.Invalid
			};
			bool flag = publicEndPoint1.Equals(NetAddress.AnyIPv4Addr) && publicEndPoint2.Equals(NetAddress.AnyIPv4Addr);
			if (flag)
			{
				stunResult.NatType = NATType.UdpBlocked;
			}
			else
			{
				bool flag2 = publicEndPoint1.Equals(privateEndPoint);
				if (flag2)
				{
					stunResult.NatType = NATType.OpenInternet;
				}
				else
				{
					bool flag3 = publicEndPoint1.Equals(publicEndPoint2);
					if (flag3)
					{
						stunResult.NatType = NATType.FullCone;
					}
					else
					{
						stunResult.NatType = NATType.Symmetric;
					}
				}
			}
			return stunResult;
		}

		public override string ToString()
		{
			return string.Format("[{0}: {1}={2}, {3}={4}, {5}={6}]", new object[]
			{
				"StunResult",
				"PublicEndPoint",
				this.PublicEndPoint,
				"PrivateEndPoint",
				this.PrivateEndPoint,
				"NatType",
				this.NatType
			});
		}

		public NATType NatType = NATType.Invalid;

		public static readonly StunResult Invalid = new StunResult(NetAddress.AnyIPv4Addr, NetAddress.AnyIPv4Addr);
	}
}
