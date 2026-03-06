using System;
using System.Net;

namespace Mono.Net.Dns
{
	internal abstract class DnsResourceRecordIPAddress : DnsResourceRecord
	{
		internal DnsResourceRecordIPAddress(DnsResourceRecord rr, int address_size)
		{
			base.CopyFrom(rr);
			ArraySegment<byte> data = rr.Data;
			byte[] dst = new byte[address_size];
			Buffer.BlockCopy(data.Array, data.Offset, dst, 0, address_size);
			this.address = new IPAddress(dst);
		}

		public override string ToString()
		{
			string str = base.ToString();
			string str2 = " Address: ";
			IPAddress ipaddress = this.address;
			return str + str2 + ((ipaddress != null) ? ipaddress.ToString() : null);
		}

		public IPAddress Address
		{
			get
			{
				return this.address;
			}
		}

		private IPAddress address;
	}
}
