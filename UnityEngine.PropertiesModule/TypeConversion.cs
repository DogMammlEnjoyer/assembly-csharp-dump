using System;
using System.Globalization;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Unity.Properties
{
	public static class TypeConversion
	{
		static TypeConversion()
		{
			TypeConversion.PrimitiveConverters.Register();
		}

		public static void Register<TSource, TDestination>(TypeConverter<TSource, TDestination> converter)
		{
			TypeConversion.s_GlobalConverters.Register(typeof(TSource), typeof(TDestination), converter);
		}

		public static TDestination Convert<TSource, TDestination>(ref TSource value)
		{
			TDestination result;
			bool flag = !TypeConversion.TryConvert<TSource, TDestination>(ref value, out result);
			if (flag)
			{
				throw new InvalidOperationException(string.Format("TypeConversion no converter has been registered for SrcType=[{0}] to DstType=[{1}]", typeof(TSource), typeof(TDestination)));
			}
			return result;
		}

		public unsafe static bool TryConvert<TSource, TDestination>(ref TSource source, out TDestination destination)
		{
			Delegate @delegate;
			bool flag = TypeConversion.s_GlobalConverters.TryGetConverter(typeof(TSource), typeof(TDestination), out @delegate);
			bool result;
			if (flag)
			{
				destination = ((TypeConverter<TSource, TDestination>)@delegate)(ref source);
				result = true;
			}
			else
			{
				bool flag2 = typeof(TSource).IsValueType && typeof(TSource) == typeof(TDestination);
				if (flag2)
				{
					destination = *UnsafeUtility.As<TSource, TDestination>(ref source);
					result = true;
				}
				else
				{
					bool isNullable = TypeTraits<TDestination>.IsNullable;
					if (isNullable)
					{
						bool flag3 = TypeTraits<TSource>.IsNullable && Nullable.GetUnderlyingType(typeof(TDestination)) != Nullable.GetUnderlyingType(typeof(TSource));
						if (flag3)
						{
							destination = default(TDestination);
							result = false;
						}
						else
						{
							Type underlyingType = Nullable.GetUnderlyingType(typeof(TDestination));
							bool isEnum = underlyingType.IsEnum;
							if (isEnum)
							{
								Type underlyingType2 = Enum.GetUnderlyingType(underlyingType);
								object value = System.Convert.ChangeType(source, underlyingType2);
								destination = (TDestination)((object)Enum.ToObject(underlyingType, value));
								result = true;
							}
							else
							{
								bool flag4 = source == null;
								if (flag4)
								{
									destination = default(TDestination);
									result = true;
								}
								else
								{
									destination = (TDestination)((object)System.Convert.ChangeType(source, underlyingType));
									result = true;
								}
							}
						}
					}
					else
					{
						bool flag5 = TypeTraits<TSource>.IsNullable && typeof(TDestination) == Nullable.GetUnderlyingType(typeof(TSource));
						if (flag5)
						{
							bool flag6 = source == null;
							if (flag6)
							{
								destination = default(TDestination);
								result = false;
							}
							else
							{
								destination = (TDestination)((object)source);
								result = true;
							}
						}
						else
						{
							bool isUnityObject = TypeTraits<TDestination>.IsUnityObject;
							if (isUnityObject)
							{
								bool flag7 = TypeConversion.TryConvertToUnityEngineObject<TSource, TDestination>(source, out destination);
								if (flag7)
								{
									return true;
								}
							}
							bool isEnum2 = TypeTraits<TDestination>.IsEnum;
							if (isEnum2)
							{
								bool flag8 = typeof(TSource) == typeof(string);
								if (flag8)
								{
									try
									{
										destination = (TDestination)((object)Enum.Parse(typeof(TDestination), (string)((object)source)));
									}
									catch (ArgumentException)
									{
										destination = default(TDestination);
										return false;
									}
									return true;
								}
								bool flag9 = TypeConversion.IsNumericType(typeof(TSource));
								if (flag9)
								{
									destination = *UnsafeUtility.As<TSource, TDestination>(ref source);
									return true;
								}
							}
							TSource tsource = source;
							TDestination tdestination;
							bool flag10;
							if (tsource is TDestination)
							{
								tdestination = (tsource as TDestination);
								flag10 = true;
							}
							else
							{
								flag10 = false;
							}
							bool flag11 = flag10;
							if (flag11)
							{
								destination = tdestination;
								result = true;
							}
							else
							{
								bool flag12 = typeof(TDestination).IsAssignableFrom(typeof(TSource));
								if (flag12)
								{
									destination = (TDestination)((object)source);
									result = true;
								}
								else
								{
									destination = default(TDestination);
									result = false;
								}
							}
						}
					}
				}
			}
			return result;
		}

		private static bool TryConvertToUnityEngineObject<TSource, TDestination>(TSource source, out TDestination destination)
		{
			bool flag = !typeof(Object).IsAssignableFrom(typeof(TDestination));
			bool result;
			if (flag)
			{
				destination = default(TDestination);
				result = false;
			}
			else
			{
				bool flag2 = typeof(Object).IsAssignableFrom(typeof(TSource)) || source is Object;
				if (flag2)
				{
					bool flag3 = source == null;
					if (flag3)
					{
						destination = default(TDestination);
						return true;
					}
					bool flag4 = typeof(TDestination) == typeof(Object);
					if (flag4)
					{
						destination = (TDestination)((object)source);
						return true;
					}
				}
				Delegate @delegate;
				bool flag5 = TypeConversion.s_GlobalConverters.TryGetConverter(typeof(TSource), typeof(Object), out @delegate);
				if (flag5)
				{
					Object @object = ((TypeConverter<TSource, Object>)@delegate)(ref source);
					destination = (TDestination)((object)@object);
					result = @object;
				}
				else
				{
					destination = default(TDestination);
					result = false;
				}
			}
			return result;
		}

		private static bool IsNumericType(Type t)
		{
			TypeCode typeCode = Type.GetTypeCode(t);
			TypeCode typeCode2 = typeCode;
			return typeCode2 - TypeCode.SByte <= 10;
		}

		private static readonly ConversionRegistry s_GlobalConverters = ConversionRegistry.Create();

		private static class PrimitiveConverters
		{
			public static void Register()
			{
				TypeConversion.PrimitiveConverters.RegisterInt8Converters();
				TypeConversion.PrimitiveConverters.RegisterInt16Converters();
				TypeConversion.PrimitiveConverters.RegisterInt32Converters();
				TypeConversion.PrimitiveConverters.RegisterInt64Converters();
				TypeConversion.PrimitiveConverters.RegisterUInt8Converters();
				TypeConversion.PrimitiveConverters.RegisterUInt16Converters();
				TypeConversion.PrimitiveConverters.RegisterUInt32Converters();
				TypeConversion.PrimitiveConverters.RegisterUInt64Converters();
				TypeConversion.PrimitiveConverters.RegisterFloat32Converters();
				TypeConversion.PrimitiveConverters.RegisterFloat64Converters();
				TypeConversion.PrimitiveConverters.RegisterBooleanConverters();
				TypeConversion.PrimitiveConverters.RegisterCharConverters();
				TypeConversion.PrimitiveConverters.RegisterStringConverters();
				TypeConversion.PrimitiveConverters.RegisterObjectConverters();
				TypeConversion.s_GlobalConverters.Register(typeof(string), typeof(Guid), new TypeConverter<string, Guid>(delegate(ref string g)
				{
					return new Guid(g);
				}));
			}

			private static void RegisterInt8Converters()
			{
				TypeConversion.s_GlobalConverters.Register(typeof(sbyte), typeof(char), new TypeConverter<sbyte, char>(delegate(ref sbyte v)
				{
					return (char)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(sbyte), typeof(bool), new TypeConverter<sbyte, bool>(delegate(ref sbyte v)
				{
					return v != 0;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(sbyte), typeof(short), new TypeConverter<sbyte, short>(delegate(ref sbyte v)
				{
					return (short)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(sbyte), typeof(int), new TypeConverter<sbyte, int>(delegate(ref sbyte v)
				{
					return (int)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(sbyte), typeof(long), new TypeConverter<sbyte, long>(delegate(ref sbyte v)
				{
					return (long)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(sbyte), typeof(byte), new TypeConverter<sbyte, byte>(delegate(ref sbyte v)
				{
					return (byte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(sbyte), typeof(ushort), new TypeConverter<sbyte, ushort>(delegate(ref sbyte v)
				{
					return (ushort)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(sbyte), typeof(uint), new TypeConverter<sbyte, uint>(delegate(ref sbyte v)
				{
					return (uint)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(sbyte), typeof(ulong), new TypeConverter<sbyte, ulong>(delegate(ref sbyte v)
				{
					return (ulong)((long)v);
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(sbyte), typeof(float), new TypeConverter<sbyte, float>(delegate(ref sbyte v)
				{
					return (float)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(sbyte), typeof(double), new TypeConverter<sbyte, double>(delegate(ref sbyte v)
				{
					return (double)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(sbyte), typeof(object), new TypeConverter<sbyte, object>(delegate(ref sbyte v)
				{
					return v;
				}));
			}

			private static void RegisterInt16Converters()
			{
				TypeConversion.s_GlobalConverters.Register(typeof(short), typeof(sbyte), new TypeConverter<short, sbyte>(delegate(ref short v)
				{
					return (sbyte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(short), typeof(char), new TypeConverter<short, char>(delegate(ref short v)
				{
					return (char)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(short), typeof(bool), new TypeConverter<short, bool>(delegate(ref short v)
				{
					return v != 0;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(short), typeof(int), new TypeConverter<short, int>(delegate(ref short v)
				{
					return (int)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(short), typeof(long), new TypeConverter<short, long>(delegate(ref short v)
				{
					return (long)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(short), typeof(byte), new TypeConverter<short, byte>(delegate(ref short v)
				{
					return (byte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(short), typeof(ushort), new TypeConverter<short, ushort>(delegate(ref short v)
				{
					return (ushort)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(short), typeof(uint), new TypeConverter<short, uint>(delegate(ref short v)
				{
					return (uint)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(short), typeof(ulong), new TypeConverter<short, ulong>(delegate(ref short v)
				{
					return (ulong)((long)v);
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(short), typeof(float), new TypeConverter<short, float>(delegate(ref short v)
				{
					return (float)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(short), typeof(double), new TypeConverter<short, double>(delegate(ref short v)
				{
					return (double)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(short), typeof(object), new TypeConverter<short, object>(delegate(ref short v)
				{
					return v;
				}));
			}

			private static void RegisterInt32Converters()
			{
				TypeConversion.s_GlobalConverters.Register(typeof(int), typeof(sbyte), new TypeConverter<int, sbyte>(delegate(ref int v)
				{
					return (sbyte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(int), typeof(char), new TypeConverter<int, char>(delegate(ref int v)
				{
					return (char)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(int), typeof(bool), new TypeConverter<int, bool>(delegate(ref int v)
				{
					return v != 0;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(int), typeof(short), new TypeConverter<int, short>(delegate(ref int v)
				{
					return (short)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(int), typeof(long), new TypeConverter<int, long>(delegate(ref int v)
				{
					return (long)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(int), typeof(byte), new TypeConverter<int, byte>(delegate(ref int v)
				{
					return (byte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(int), typeof(ushort), new TypeConverter<int, ushort>(delegate(ref int v)
				{
					return (ushort)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(int), typeof(uint), new TypeConverter<int, uint>(delegate(ref int v)
				{
					return (uint)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(int), typeof(ulong), new TypeConverter<int, ulong>(delegate(ref int v)
				{
					return (ulong)((long)v);
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(int), typeof(float), new TypeConverter<int, float>(delegate(ref int v)
				{
					return (float)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(int), typeof(double), new TypeConverter<int, double>(delegate(ref int v)
				{
					return (double)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(int), typeof(object), new TypeConverter<int, object>(delegate(ref int v)
				{
					return v;
				}));
			}

			private static void RegisterInt64Converters()
			{
				TypeConversion.s_GlobalConverters.Register(typeof(long), typeof(sbyte), new TypeConverter<long, sbyte>(delegate(ref long v)
				{
					return (sbyte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(long), typeof(char), new TypeConverter<long, char>(delegate(ref long v)
				{
					return (char)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(long), typeof(bool), new TypeConverter<long, bool>(delegate(ref long v)
				{
					return v != 0L;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(long), typeof(short), new TypeConverter<long, short>(delegate(ref long v)
				{
					return (short)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(long), typeof(int), new TypeConverter<long, int>(delegate(ref long v)
				{
					return (int)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(long), typeof(byte), new TypeConverter<long, byte>(delegate(ref long v)
				{
					return (byte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(long), typeof(ushort), new TypeConverter<long, ushort>(delegate(ref long v)
				{
					return (ushort)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(long), typeof(uint), new TypeConverter<long, uint>(delegate(ref long v)
				{
					return (uint)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(long), typeof(ulong), new TypeConverter<long, ulong>(delegate(ref long v)
				{
					return (ulong)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(long), typeof(float), new TypeConverter<long, float>(delegate(ref long v)
				{
					return (float)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(long), typeof(double), new TypeConverter<long, double>(delegate(ref long v)
				{
					return (double)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(long), typeof(object), new TypeConverter<long, object>(delegate(ref long v)
				{
					return v;
				}));
			}

			private static void RegisterUInt8Converters()
			{
				TypeConversion.s_GlobalConverters.Register(typeof(byte), typeof(sbyte), new TypeConverter<byte, sbyte>(delegate(ref byte v)
				{
					return (sbyte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(byte), typeof(char), new TypeConverter<byte, char>(delegate(ref byte v)
				{
					return (char)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(byte), typeof(bool), new TypeConverter<byte, bool>(delegate(ref byte v)
				{
					return v > 0;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(byte), typeof(short), new TypeConverter<byte, short>(delegate(ref byte v)
				{
					return (short)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(byte), typeof(int), new TypeConverter<byte, int>(delegate(ref byte v)
				{
					return (int)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(byte), typeof(long), new TypeConverter<byte, long>(delegate(ref byte v)
				{
					return (long)((ulong)v);
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(byte), typeof(ushort), new TypeConverter<byte, ushort>(delegate(ref byte v)
				{
					return (ushort)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(byte), typeof(uint), new TypeConverter<byte, uint>(delegate(ref byte v)
				{
					return (uint)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(byte), typeof(ulong), new TypeConverter<byte, ulong>(delegate(ref byte v)
				{
					return (ulong)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(byte), typeof(float), new TypeConverter<byte, float>(delegate(ref byte v)
				{
					return (float)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(byte), typeof(double), new TypeConverter<byte, double>(delegate(ref byte v)
				{
					return (double)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(byte), typeof(object), new TypeConverter<byte, object>(delegate(ref byte v)
				{
					return v;
				}));
			}

			private static void RegisterUInt16Converters()
			{
				TypeConversion.s_GlobalConverters.Register(typeof(ushort), typeof(sbyte), new TypeConverter<ushort, sbyte>(delegate(ref ushort v)
				{
					return (sbyte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ushort), typeof(char), new TypeConverter<ushort, char>(delegate(ref ushort v)
				{
					return (char)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ushort), typeof(bool), new TypeConverter<ushort, bool>(delegate(ref ushort v)
				{
					return v > 0;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ushort), typeof(short), new TypeConverter<ushort, short>(delegate(ref ushort v)
				{
					return (short)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ushort), typeof(int), new TypeConverter<ushort, int>(delegate(ref ushort v)
				{
					return (int)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ushort), typeof(long), new TypeConverter<ushort, long>(delegate(ref ushort v)
				{
					return (long)((ulong)v);
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ushort), typeof(byte), new TypeConverter<ushort, byte>(delegate(ref ushort v)
				{
					return (byte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ushort), typeof(uint), new TypeConverter<ushort, uint>(delegate(ref ushort v)
				{
					return (uint)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ushort), typeof(ulong), new TypeConverter<ushort, ulong>(delegate(ref ushort v)
				{
					return (ulong)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ushort), typeof(float), new TypeConverter<ushort, float>(delegate(ref ushort v)
				{
					return (float)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ushort), typeof(double), new TypeConverter<ushort, double>(delegate(ref ushort v)
				{
					return (double)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ushort), typeof(object), new TypeConverter<ushort, object>(delegate(ref ushort v)
				{
					return v;
				}));
			}

			private static void RegisterUInt32Converters()
			{
				TypeConversion.s_GlobalConverters.Register(typeof(uint), typeof(sbyte), new TypeConverter<uint, sbyte>(delegate(ref uint v)
				{
					return (sbyte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(uint), typeof(char), new TypeConverter<uint, char>(delegate(ref uint v)
				{
					return (char)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(uint), typeof(bool), new TypeConverter<uint, bool>(delegate(ref uint v)
				{
					return v > 0U;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(uint), typeof(short), new TypeConverter<uint, short>(delegate(ref uint v)
				{
					return (short)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(uint), typeof(int), new TypeConverter<uint, int>(delegate(ref uint v)
				{
					return (int)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(uint), typeof(long), new TypeConverter<uint, long>(delegate(ref uint v)
				{
					return (long)((ulong)v);
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(uint), typeof(byte), new TypeConverter<uint, byte>(delegate(ref uint v)
				{
					return (byte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(uint), typeof(ushort), new TypeConverter<uint, ushort>(delegate(ref uint v)
				{
					return (ushort)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(uint), typeof(ulong), new TypeConverter<uint, ulong>(delegate(ref uint v)
				{
					return (ulong)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(uint), typeof(float), new TypeConverter<uint, float>(delegate(ref uint v)
				{
					return v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(uint), typeof(double), new TypeConverter<uint, double>(delegate(ref uint v)
				{
					return v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(uint), typeof(object), new TypeConverter<uint, object>(delegate(ref uint v)
				{
					return v;
				}));
			}

			private static void RegisterUInt64Converters()
			{
				TypeConversion.s_GlobalConverters.Register(typeof(ulong), typeof(sbyte), new TypeConverter<ulong, sbyte>(delegate(ref ulong v)
				{
					return (sbyte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ulong), typeof(char), new TypeConverter<ulong, char>(delegate(ref ulong v)
				{
					return (char)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ulong), typeof(bool), new TypeConverter<ulong, bool>(delegate(ref ulong v)
				{
					return v > 0UL;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ulong), typeof(short), new TypeConverter<ulong, short>(delegate(ref ulong v)
				{
					return (short)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ulong), typeof(int), new TypeConverter<ulong, int>(delegate(ref ulong v)
				{
					return (int)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ulong), typeof(long), new TypeConverter<ulong, long>(delegate(ref ulong v)
				{
					return (long)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ulong), typeof(byte), new TypeConverter<ulong, byte>(delegate(ref ulong v)
				{
					return (byte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ulong), typeof(ushort), new TypeConverter<ulong, ushort>(delegate(ref ulong v)
				{
					return (ushort)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ulong), typeof(uint), new TypeConverter<ulong, uint>(delegate(ref ulong v)
				{
					return (uint)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ulong), typeof(float), new TypeConverter<ulong, float>(delegate(ref ulong v)
				{
					return v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ulong), typeof(double), new TypeConverter<ulong, double>(delegate(ref ulong v)
				{
					return v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ulong), typeof(object), new TypeConverter<ulong, object>(delegate(ref ulong v)
				{
					return v;
				}));
			}

			private static void RegisterFloat32Converters()
			{
				TypeConversion.s_GlobalConverters.Register(typeof(float), typeof(sbyte), new TypeConverter<float, sbyte>(delegate(ref float v)
				{
					return (sbyte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(float), typeof(char), new TypeConverter<float, char>(delegate(ref float v)
				{
					return (char)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(float), typeof(bool), new TypeConverter<float, bool>(delegate(ref float v)
				{
					return Math.Abs(v) > float.Epsilon;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(float), typeof(short), new TypeConverter<float, short>(delegate(ref float v)
				{
					return (short)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(float), typeof(int), new TypeConverter<float, int>(delegate(ref float v)
				{
					return (int)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(float), typeof(long), new TypeConverter<float, long>(delegate(ref float v)
				{
					return (long)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(float), typeof(byte), new TypeConverter<float, byte>(delegate(ref float v)
				{
					return (byte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(float), typeof(ushort), new TypeConverter<float, ushort>(delegate(ref float v)
				{
					return (ushort)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(float), typeof(uint), new TypeConverter<float, uint>(delegate(ref float v)
				{
					return (uint)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(float), typeof(ulong), new TypeConverter<float, ulong>(delegate(ref float v)
				{
					return (ulong)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(float), typeof(double), new TypeConverter<float, double>(delegate(ref float v)
				{
					return (double)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(float), typeof(object), new TypeConverter<float, object>(delegate(ref float v)
				{
					return v;
				}));
			}

			private static void RegisterFloat64Converters()
			{
				TypeConversion.s_GlobalConverters.Register(typeof(double), typeof(sbyte), new TypeConverter<double, sbyte>(delegate(ref double v)
				{
					return (sbyte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(double), typeof(char), new TypeConverter<double, char>(delegate(ref double v)
				{
					return (char)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(double), typeof(bool), new TypeConverter<double, bool>(delegate(ref double v)
				{
					return Math.Abs(v) > double.Epsilon;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(double), typeof(short), new TypeConverter<double, short>(delegate(ref double v)
				{
					return (short)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(double), typeof(int), new TypeConverter<double, int>(delegate(ref double v)
				{
					return (int)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(double), typeof(long), new TypeConverter<double, long>(delegate(ref double v)
				{
					return (long)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(double), typeof(byte), new TypeConverter<double, byte>(delegate(ref double v)
				{
					return (byte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(double), typeof(ushort), new TypeConverter<double, ushort>(delegate(ref double v)
				{
					return (ushort)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(double), typeof(uint), new TypeConverter<double, uint>(delegate(ref double v)
				{
					return (uint)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(double), typeof(ulong), new TypeConverter<double, ulong>(delegate(ref double v)
				{
					return (ulong)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(double), typeof(float), new TypeConverter<double, float>(delegate(ref double v)
				{
					return (float)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(double), typeof(object), new TypeConverter<double, object>(delegate(ref double v)
				{
					return v;
				}));
			}

			private static void RegisterBooleanConverters()
			{
				TypeConversion.s_GlobalConverters.Register(typeof(bool), typeof(char), new TypeConverter<bool, char>(delegate(ref bool v)
				{
					return v ? '\u0001' : '\0';
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(bool), typeof(sbyte), new TypeConverter<bool, sbyte>(delegate(ref bool v)
				{
					return v ? 1 : 0;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(bool), typeof(short), new TypeConverter<bool, short>(delegate(ref bool v)
				{
					return v ? 1 : 0;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(bool), typeof(int), new TypeConverter<bool, int>(delegate(ref bool v)
				{
					return v ? 1 : 0;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(bool), typeof(long), new TypeConverter<bool, long>(delegate(ref bool v)
				{
					return v ? 1L : 0L;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(bool), typeof(byte), new TypeConverter<bool, byte>(delegate(ref bool v)
				{
					return v ? 1 : 0;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(bool), typeof(ushort), new TypeConverter<bool, ushort>(delegate(ref bool v)
				{
					return v ? 1 : 0;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(bool), typeof(uint), new TypeConverter<bool, uint>(delegate(ref bool v)
				{
					return v ? 1U : 0U;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(bool), typeof(ulong), new TypeConverter<bool, ulong>(delegate(ref bool v)
				{
					return v ? 1UL : 0UL;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(bool), typeof(float), new TypeConverter<bool, float>(delegate(ref bool v)
				{
					return v ? 1f : 0f;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(bool), typeof(double), new TypeConverter<bool, double>(delegate(ref bool v)
				{
					return v ? 1.0 : 0.0;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(bool), typeof(object), new TypeConverter<bool, object>(delegate(ref bool v)
				{
					return v;
				}));
			}

			private static void RegisterCharConverters()
			{
				TypeConversion.s_GlobalConverters.Register(typeof(string), typeof(char), new TypeConverter<string, char>(delegate(ref string v)
				{
					bool flag = v.Length != 1;
					if (flag)
					{
						throw new Exception("Not a valid char");
					}
					return v[0];
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(char), typeof(bool), new TypeConverter<char, bool>(delegate(ref char v)
				{
					return v > '\0';
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(char), typeof(sbyte), new TypeConverter<char, sbyte>(delegate(ref char v)
				{
					return (sbyte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(char), typeof(short), new TypeConverter<char, short>(delegate(ref char v)
				{
					return (short)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(char), typeof(int), new TypeConverter<char, int>(delegate(ref char v)
				{
					return (int)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(char), typeof(long), new TypeConverter<char, long>(delegate(ref char v)
				{
					return (long)((ulong)v);
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(char), typeof(byte), new TypeConverter<char, byte>(delegate(ref char v)
				{
					return (byte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(char), typeof(ushort), new TypeConverter<char, ushort>(delegate(ref char v)
				{
					return (ushort)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(char), typeof(uint), new TypeConverter<char, uint>(delegate(ref char v)
				{
					return (uint)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(char), typeof(ulong), new TypeConverter<char, ulong>(delegate(ref char v)
				{
					return (ulong)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(char), typeof(float), new TypeConverter<char, float>(delegate(ref char v)
				{
					return (float)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(char), typeof(double), new TypeConverter<char, double>(delegate(ref char v)
				{
					return (double)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(char), typeof(object), new TypeConverter<char, object>(delegate(ref char v)
				{
					return v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(char), typeof(string), new TypeConverter<char, string>(delegate(ref char v)
				{
					return v.ToString();
				}));
			}

			private static void RegisterStringConverters()
			{
				TypeConversion.s_GlobalConverters.Register(typeof(string), typeof(char), new TypeConverter<string, char>(delegate(ref string v)
				{
					return (!string.IsNullOrEmpty(v)) ? v[0] : '\0';
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(char), typeof(string), new TypeConverter<char, string>(delegate(ref char v)
				{
					return v.ToString();
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(string), typeof(bool), new TypeConverter<string, bool>(delegate(ref string v)
				{
					bool flag2;
					bool flag = bool.TryParse(v, out flag2);
					bool result;
					if (flag)
					{
						result = flag2;
					}
					else
					{
						double num;
						result = (double.TryParse(v, out num) && TypeConversion.Convert<double, bool>(ref num));
					}
					return result;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(bool), typeof(string), new TypeConverter<bool, string>(delegate(ref bool v)
				{
					return v.ToString();
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(string), typeof(sbyte), new TypeConverter<string, sbyte>(delegate(ref string v)
				{
					sbyte b;
					bool flag = sbyte.TryParse(v, out b);
					sbyte result;
					if (flag)
					{
						result = b;
					}
					else
					{
						double num;
						result = (double.TryParse(v, out num) ? TypeConversion.Convert<double, sbyte>(ref num) : 0);
					}
					return result;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(sbyte), typeof(string), new TypeConverter<sbyte, string>(delegate(ref sbyte v)
				{
					return v.ToString();
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(string), typeof(short), new TypeConverter<string, short>(delegate(ref string v)
				{
					short num;
					bool flag = short.TryParse(v, out num);
					short result;
					if (flag)
					{
						result = num;
					}
					else
					{
						double num2;
						result = (double.TryParse(v, out num2) ? TypeConversion.Convert<double, short>(ref num2) : 0);
					}
					return result;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(short), typeof(string), new TypeConverter<short, string>(delegate(ref short v)
				{
					return v.ToString();
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(string), typeof(int), new TypeConverter<string, int>(delegate(ref string v)
				{
					int num;
					bool flag = int.TryParse(v, out num);
					int result;
					if (flag)
					{
						result = num;
					}
					else
					{
						double num2;
						result = (double.TryParse(v, out num2) ? TypeConversion.Convert<double, int>(ref num2) : 0);
					}
					return result;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(int), typeof(string), new TypeConverter<int, string>(delegate(ref int v)
				{
					return v.ToString();
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(string), typeof(long), new TypeConverter<string, long>(delegate(ref string v)
				{
					long num;
					bool flag = long.TryParse(v, out num);
					long result;
					if (flag)
					{
						result = num;
					}
					else
					{
						double num2;
						result = (double.TryParse(v, out num2) ? TypeConversion.Convert<double, long>(ref num2) : 0L);
					}
					return result;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(long), typeof(string), new TypeConverter<long, string>(delegate(ref long v)
				{
					return v.ToString();
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(string), typeof(byte), new TypeConverter<string, byte>(delegate(ref string v)
				{
					byte b;
					bool flag = byte.TryParse(v, out b);
					byte result;
					if (flag)
					{
						result = b;
					}
					else
					{
						double num;
						result = (double.TryParse(v, out num) ? TypeConversion.Convert<double, byte>(ref num) : 0);
					}
					return result;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(byte), typeof(string), new TypeConverter<byte, string>(delegate(ref byte v)
				{
					return v.ToString();
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(string), typeof(ushort), new TypeConverter<string, ushort>(delegate(ref string v)
				{
					ushort num;
					bool flag = ushort.TryParse(v, out num);
					ushort result;
					if (flag)
					{
						result = num;
					}
					else
					{
						double num2;
						result = (double.TryParse(v, out num2) ? TypeConversion.Convert<double, ushort>(ref num2) : 0);
					}
					return result;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ushort), typeof(string), new TypeConverter<ushort, string>(delegate(ref ushort v)
				{
					return v.ToString();
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(string), typeof(uint), new TypeConverter<string, uint>(delegate(ref string v)
				{
					uint num;
					bool flag = uint.TryParse(v, out num);
					uint result;
					if (flag)
					{
						result = num;
					}
					else
					{
						double num2;
						result = (double.TryParse(v, out num2) ? TypeConversion.Convert<double, uint>(ref num2) : 0U);
					}
					return result;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(uint), typeof(string), new TypeConverter<uint, string>(delegate(ref uint v)
				{
					return v.ToString();
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(string), typeof(ulong), new TypeConverter<string, ulong>(delegate(ref string v)
				{
					ulong num;
					bool flag = ulong.TryParse(v, out num);
					ulong result;
					if (flag)
					{
						result = num;
					}
					else
					{
						double num2;
						result = (double.TryParse(v, out num2) ? TypeConversion.Convert<double, ulong>(ref num2) : 0UL);
					}
					return result;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ulong), typeof(string), new TypeConverter<ulong, string>(delegate(ref ulong v)
				{
					return v.ToString();
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(string), typeof(float), new TypeConverter<string, float>(delegate(ref string v)
				{
					float num;
					bool flag = float.TryParse(v, out num);
					float result;
					if (flag)
					{
						result = num;
					}
					else
					{
						double num2;
						result = (double.TryParse(v, out num2) ? TypeConversion.Convert<double, float>(ref num2) : 0f);
					}
					return result;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(float), typeof(string), new TypeConverter<float, string>(delegate(ref float v)
				{
					return v.ToString(CultureInfo.InvariantCulture);
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(string), typeof(double), new TypeConverter<string, double>(delegate(ref string v)
				{
					double result;
					double.TryParse(v, out result);
					return result;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(double), typeof(string), new TypeConverter<double, string>(delegate(ref double v)
				{
					return v.ToString(CultureInfo.InvariantCulture);
				}));
			}

			private static void RegisterObjectConverters()
			{
				TypeConversion.s_GlobalConverters.Register(typeof(object), typeof(char), new TypeConverter<object, char>(delegate(ref object v)
				{
					object obj = v;
					char result;
					if (obj is char)
					{
						char c = (char)obj;
						result = c;
					}
					else
					{
						result = '\0';
					}
					return result;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(object), typeof(bool), new TypeConverter<object, bool>(delegate(ref object v)
				{
					object obj = v;
					bool result;
					if (obj is bool)
					{
						bool flag = (bool)obj;
						result = flag;
					}
					else
					{
						result = false;
					}
					return result;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(object), typeof(sbyte), new TypeConverter<object, sbyte>(delegate(ref object v)
				{
					object obj = v;
					sbyte result;
					if (obj is sbyte)
					{
						sbyte b = (sbyte)obj;
						result = b;
					}
					else
					{
						result = 0;
					}
					return result;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(object), typeof(short), new TypeConverter<object, short>(delegate(ref object v)
				{
					object obj = v;
					short result;
					if (obj is short)
					{
						short num = (short)obj;
						result = num;
					}
					else
					{
						result = 0;
					}
					return result;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(object), typeof(int), new TypeConverter<object, int>(delegate(ref object v)
				{
					object obj = v;
					int result;
					if (obj is int)
					{
						int num = (int)obj;
						result = num;
					}
					else
					{
						result = 0;
					}
					return result;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(object), typeof(long), new TypeConverter<object, long>(delegate(ref object v)
				{
					object obj = v;
					long result;
					if (obj is long)
					{
						long num = (long)obj;
						result = num;
					}
					else
					{
						result = 0L;
					}
					return result;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(object), typeof(byte), new TypeConverter<object, byte>(delegate(ref object v)
				{
					object obj = v;
					byte result;
					if (obj is byte)
					{
						byte b = (byte)obj;
						result = b;
					}
					else
					{
						result = 0;
					}
					return result;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(object), typeof(ushort), new TypeConverter<object, ushort>(delegate(ref object v)
				{
					object obj = v;
					ushort result;
					if (obj is ushort)
					{
						ushort num = (ushort)obj;
						result = num;
					}
					else
					{
						result = 0;
					}
					return result;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(object), typeof(uint), new TypeConverter<object, uint>(delegate(ref object v)
				{
					object obj = v;
					uint result;
					if (obj is uint)
					{
						uint num = (uint)obj;
						result = num;
					}
					else
					{
						result = 0U;
					}
					return result;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(object), typeof(ulong), new TypeConverter<object, ulong>(delegate(ref object v)
				{
					object obj = v;
					ulong result;
					if (obj is ulong)
					{
						ulong num = (ulong)obj;
						result = num;
					}
					else
					{
						result = 0UL;
					}
					return result;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(object), typeof(float), new TypeConverter<object, float>(delegate(ref object v)
				{
					object obj = v;
					float result;
					if (obj is float)
					{
						float num = (float)obj;
						result = num;
					}
					else
					{
						result = 0f;
					}
					return result;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(object), typeof(double), new TypeConverter<object, double>(delegate(ref object v)
				{
					object obj = v;
					double result;
					if (obj is double)
					{
						double num = (double)obj;
						result = num;
					}
					else
					{
						result = 0.0;
					}
					return result;
				}));
			}
		}
	}
}
