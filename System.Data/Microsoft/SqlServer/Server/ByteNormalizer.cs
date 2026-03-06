using System;
using System.IO;
using System.Reflection;

namespace Microsoft.SqlServer.Server
{
	internal sealed class ByteNormalizer : Normalizer
	{
		internal override void Normalize(FieldInfo fi, object obj, Stream s)
		{
			byte value = (byte)base.GetValue(fi, obj);
			s.WriteByte(value);
		}

		internal override void DeNormalize(FieldInfo fi, object recvr, Stream s)
		{
			byte b = (byte)s.ReadByte();
			base.SetValue(fi, recvr, b);
		}

		internal override int Size
		{
			get
			{
				return 1;
			}
		}
	}
}
