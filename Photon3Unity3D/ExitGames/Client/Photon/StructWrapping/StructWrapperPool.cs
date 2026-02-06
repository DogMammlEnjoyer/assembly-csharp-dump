using System;

namespace ExitGames.Client.Photon.StructWrapping
{
	public class StructWrapperPool
	{
		public static WrappedType GetWrappedType(Type type)
		{
			bool flag = type == typeof(bool);
			WrappedType result;
			if (flag)
			{
				result = WrappedType.Bool;
			}
			else
			{
				bool flag2 = type == typeof(byte);
				if (flag2)
				{
					result = WrappedType.Byte;
				}
				else
				{
					bool flag3 = type == typeof(short);
					if (flag3)
					{
						result = WrappedType.Int16;
					}
					else
					{
						bool flag4 = type == typeof(int);
						if (flag4)
						{
							result = WrappedType.Int32;
						}
						else
						{
							bool flag5 = type == typeof(long);
							if (flag5)
							{
								result = WrappedType.Int64;
							}
							else
							{
								bool flag6 = type == typeof(float);
								if (flag6)
								{
									result = WrappedType.Single;
								}
								else
								{
									bool flag7 = type == typeof(double);
									if (flag7)
									{
										result = WrappedType.Double;
									}
									else
									{
										result = WrappedType.Unknown;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}
	}
}
