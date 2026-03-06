using System;
using System.Collections.Generic;
using System.Globalization;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Properties;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	public static class ConverterGroups
	{
		internal static ConverterGroup globalConverters
		{
			get
			{
				return ConverterGroups.s_GlobalConverters;
			}
		}

		internal static ConverterGroup primitivesConverters
		{
			get
			{
				return ConverterGroups.s_PrimitivesConverters;
			}
		}

		static ConverterGroups()
		{
			ConverterGroups.RegisterPrimitivesConverter();
		}

		private static void RegisterPrimitivesConverter()
		{
			ConverterGroups.RegisterInt8Converters();
			ConverterGroups.RegisterInt16Converters();
			ConverterGroups.RegisterInt32Converters();
			ConverterGroups.RegisterInt64Converters();
			ConverterGroups.RegisterUInt8Converters();
			ConverterGroups.RegisterUInt16Converters();
			ConverterGroups.RegisterUInt32Converters();
			ConverterGroups.RegisterUInt64Converters();
			ConverterGroups.RegisterFloatConverters();
			ConverterGroups.RegisterDoubleConverters();
			ConverterGroups.RegisterBooleanConverters();
			ConverterGroups.RegisterCharConverters();
			ConverterGroups.RegisterColorConverters();
		}

		public static void RegisterGlobalConverter<TSource, TDestination>(TypeConverter<TSource, TDestination> converter)
		{
			ConverterGroups.s_GlobalConverters.registry.Register(typeof(TSource), typeof(TDestination), converter);
		}

		internal static void RegisterGlobal<TSource, TDestination>(TypeConverter<TSource, TDestination> converter)
		{
			ConverterGroups.s_GlobalConverters.registry.Register(typeof(TSource), typeof(TDestination), converter);
			TypeConversion.Register<TSource, TDestination>(converter);
		}

		internal static void AddConverterToGroup<TSource, TDestination>(string groupId, TypeConverter<TSource, TDestination> converter)
		{
			ConverterGroup converterGroup;
			bool flag = !ConverterGroups.s_BindingConverterGroups.TryGetValue(groupId, out converterGroup);
			if (flag)
			{
				converterGroup = new ConverterGroup(groupId, null, null);
				ConverterGroups.s_BindingConverterGroups.Add(groupId, converterGroup);
			}
			converterGroup.AddConverter<TSource, TDestination>(converter);
		}

		public static void RegisterConverterGroup(ConverterGroup converterGroup)
		{
			bool flag = string.IsNullOrWhiteSpace(converterGroup.id);
			if (flag)
			{
				Debug.LogWarning("[UI Toolkit] Cannot register a converter group with a 'null' or empty id.");
			}
			else
			{
				bool flag2 = ConverterGroups.s_BindingConverterGroups.ContainsKey(converterGroup.id);
				if (flag2)
				{
					Debug.LogWarning("[UI Toolkit] Replacing converter group with id: " + converterGroup.id);
				}
				ConverterGroups.s_BindingConverterGroups[converterGroup.id] = converterGroup;
			}
		}

		public static bool TryGetConverterGroup(string groupId, out ConverterGroup converterGroup)
		{
			return ConverterGroups.s_BindingConverterGroups.TryGetValue(groupId, out converterGroup);
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal static void GetAllConverterGroups(List<ConverterGroup> result)
		{
			foreach (ConverterGroup item in ConverterGroups.s_BindingConverterGroups.Values)
			{
				result.Add(item);
			}
		}

		public unsafe static bool TryConvert<TSource, TDestination>(ref TSource source, out TDestination destination)
		{
			Type typeFromHandle = typeof(TSource);
			Type typeFromHandle2 = typeof(TDestination);
			Delegate @delegate;
			bool flag = ConverterGroups.s_GlobalConverters.registry.TryGetConverter(typeof(TSource), typeFromHandle2, out @delegate);
			bool result;
			if (flag)
			{
				destination = ((TypeConverter<TSource, TDestination>)@delegate)(ref source);
				result = true;
			}
			else
			{
				bool flag2 = typeFromHandle.IsValueType && typeFromHandle2.IsValueType;
				if (flag2)
				{
					bool flag3 = typeFromHandle == typeFromHandle2;
					if (flag3)
					{
						destination = *UnsafeUtility.As<TSource, TDestination>(ref source);
						result = true;
					}
					else
					{
						bool flag4 = ConverterGroups.s_PrimitivesConverters.registry.TryGetConverter(typeFromHandle, typeFromHandle2, out @delegate);
						if (flag4)
						{
							destination = ((TypeConverter<TSource, TDestination>)@delegate)(ref source);
							result = true;
						}
						else
						{
							destination = default(TDestination);
							result = false;
						}
					}
				}
				else
				{
					TSource tsource = source;
					TDestination tdestination;
					bool flag5;
					if (tsource is TDestination)
					{
						tdestination = (tsource as TDestination);
						flag5 = true;
					}
					else
					{
						flag5 = false;
					}
					bool flag6 = flag5;
					if (flag6)
					{
						destination = tdestination;
						result = true;
					}
					else
					{
						bool flag7 = typeFromHandle2.IsAssignableFrom(typeFromHandle) && source == null;
						if (flag7)
						{
							destination = default(TDestination);
							result = true;
						}
						else
						{
							bool flag8 = typeFromHandle2 == typeof(string);
							if (flag8)
							{
								ref TSource ptr = ref source;
								tsource = default(TSource);
								object obj;
								if (tsource == null)
								{
									tsource = source;
									ptr = ref tsource;
									if (tsource == null)
									{
										obj = null;
										goto IL_19B;
									}
								}
								obj = ptr.ToString();
								IL_19B:
								destination = (TDestination)((object)obj);
								result = true;
							}
							else
							{
								bool flag9 = typeFromHandle2 == typeof(object);
								if (flag9)
								{
									destination = (TDestination)((object)source);
									result = true;
								}
								else
								{
									bool flag10 = !TypeTraits<TSource>.IsValueType && source == null && typeof(TSource) == typeof(object);
									if (flag10)
									{
										destination = default(TDestination);
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
			}
			return result;
		}

		public static bool TrySetValueGlobal<TContainer, TValue>(ref TContainer container, in PropertyPath path, TValue value, out VisitReturnCode returnCode)
		{
			bool isEmpty = path.IsEmpty;
			bool result;
			if (isEmpty)
			{
				returnCode = VisitReturnCode.InvalidPath;
				result = false;
			}
			else
			{
				SetValueVisitor<TValue> setValueVisitor = SetValueVisitor<TValue>.Pool.Get();
				setValueVisitor.Path = path;
				setValueVisitor.Value = value;
				try
				{
					bool flag = !PropertyContainer.TryAccept<TContainer>(setValueVisitor, ref container, out returnCode, default(VisitParameters));
					if (flag)
					{
						return false;
					}
					returnCode = setValueVisitor.ReturnCode;
				}
				finally
				{
					SetValueVisitor<TValue>.Pool.Release(setValueVisitor);
				}
				result = (returnCode == VisitReturnCode.Ok);
			}
			return result;
		}

		private static void RegisterInt8Converters()
		{
			TypeConverterRegistry registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(sbyte), typeof(bool), new TypeConverter<sbyte, bool>(delegate(ref sbyte v)
			{
				return (long)v > 0L;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(sbyte), typeof(char), new TypeConverter<sbyte, char>(delegate(ref sbyte v)
			{
				return (char)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(sbyte), typeof(short), new TypeConverter<sbyte, short>(delegate(ref sbyte v)
			{
				return (short)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(sbyte), typeof(int), new TypeConverter<sbyte, int>(delegate(ref sbyte v)
			{
				return (int)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(sbyte), typeof(long), new TypeConverter<sbyte, long>(delegate(ref sbyte v)
			{
				return (long)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(sbyte), typeof(byte), new TypeConverter<sbyte, byte>(delegate(ref sbyte v)
			{
				return (byte)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(sbyte), typeof(ushort), new TypeConverter<sbyte, ushort>(delegate(ref sbyte v)
			{
				return (ushort)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(sbyte), typeof(uint), new TypeConverter<sbyte, uint>(delegate(ref sbyte v)
			{
				return (uint)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(sbyte), typeof(ulong), new TypeConverter<sbyte, ulong>(delegate(ref sbyte v)
			{
				return (ulong)((long)v);
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(sbyte), typeof(float), new TypeConverter<sbyte, float>(delegate(ref sbyte v)
			{
				return (float)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(sbyte), typeof(double), new TypeConverter<sbyte, double>(delegate(ref sbyte v)
			{
				return (double)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(string), typeof(sbyte), new TypeConverter<string, sbyte>(delegate(ref string v)
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
					sbyte b2;
					result = ((double.TryParse(v, out num) && ConverterGroups.TryConvert<double, sbyte>(ref num, out b2)) ? b2 : 0);
				}
				return result;
			}));
		}

		private static void RegisterInt16Converters()
		{
			TypeConverterRegistry registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(short), typeof(bool), new TypeConverter<short, bool>(delegate(ref short v)
			{
				return (long)v > 0L;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(short), typeof(sbyte), new TypeConverter<short, sbyte>(delegate(ref short v)
			{
				return (sbyte)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(short), typeof(char), new TypeConverter<short, char>(delegate(ref short v)
			{
				return (char)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(short), typeof(int), new TypeConverter<short, int>(delegate(ref short v)
			{
				return (int)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(short), typeof(long), new TypeConverter<short, long>(delegate(ref short v)
			{
				return (long)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(short), typeof(byte), new TypeConverter<short, byte>(delegate(ref short v)
			{
				return (byte)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(short), typeof(ushort), new TypeConverter<short, ushort>(delegate(ref short v)
			{
				return (ushort)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(short), typeof(uint), new TypeConverter<short, uint>(delegate(ref short v)
			{
				return (uint)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(short), typeof(ulong), new TypeConverter<short, ulong>(delegate(ref short v)
			{
				return (ulong)((long)v);
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(short), typeof(float), new TypeConverter<short, float>(delegate(ref short v)
			{
				return (float)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(short), typeof(double), new TypeConverter<short, double>(delegate(ref short v)
			{
				return (double)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(string), typeof(short), new TypeConverter<string, short>(delegate(ref string v)
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
					short num3;
					result = ((double.TryParse(v, out num2) && ConverterGroups.TryConvert<double, short>(ref num2, out num3)) ? num3 : 0);
				}
				return result;
			}));
		}

		private static void RegisterInt32Converters()
		{
			TypeConverterRegistry registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(int), typeof(bool), new TypeConverter<int, bool>(delegate(ref int v)
			{
				return (long)v > 0L;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(int), typeof(sbyte), new TypeConverter<int, sbyte>(delegate(ref int v)
			{
				return (sbyte)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(int), typeof(char), new TypeConverter<int, char>(delegate(ref int v)
			{
				return (char)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(int), typeof(short), new TypeConverter<int, short>(delegate(ref int v)
			{
				return (short)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(int), typeof(long), new TypeConverter<int, long>(delegate(ref int v)
			{
				return (long)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(int), typeof(byte), new TypeConverter<int, byte>(delegate(ref int v)
			{
				return (byte)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(int), typeof(ushort), new TypeConverter<int, ushort>(delegate(ref int v)
			{
				return (ushort)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(int), typeof(uint), new TypeConverter<int, uint>(delegate(ref int v)
			{
				return (uint)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(int), typeof(ulong), new TypeConverter<int, ulong>(delegate(ref int v)
			{
				return (ulong)((long)v);
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(int), typeof(float), new TypeConverter<int, float>(delegate(ref int v)
			{
				return (float)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(int), typeof(double), new TypeConverter<int, double>(delegate(ref int v)
			{
				return (double)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(string), typeof(int), new TypeConverter<string, int>(delegate(ref string v)
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
					int num3;
					result = ((double.TryParse(v, out num2) && ConverterGroups.TryConvert<double, int>(ref num2, out num3)) ? num3 : 0);
				}
				return result;
			}));
		}

		private static void RegisterInt64Converters()
		{
			TypeConverterRegistry registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(long), typeof(bool), new TypeConverter<long, bool>(delegate(ref long v)
			{
				return v > 0L;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(long), typeof(sbyte), new TypeConverter<long, sbyte>(delegate(ref long v)
			{
				return (sbyte)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(long), typeof(char), new TypeConverter<long, char>(delegate(ref long v)
			{
				return (char)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(long), typeof(short), new TypeConverter<long, short>(delegate(ref long v)
			{
				return (short)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(long), typeof(int), new TypeConverter<long, int>(delegate(ref long v)
			{
				return (int)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(long), typeof(byte), new TypeConverter<long, byte>(delegate(ref long v)
			{
				return (byte)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(long), typeof(ushort), new TypeConverter<long, ushort>(delegate(ref long v)
			{
				return (ushort)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(long), typeof(uint), new TypeConverter<long, uint>(delegate(ref long v)
			{
				return (uint)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(long), typeof(ulong), new TypeConverter<long, ulong>(delegate(ref long v)
			{
				return (ulong)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(long), typeof(float), new TypeConverter<long, float>(delegate(ref long v)
			{
				return (float)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(long), typeof(double), new TypeConverter<long, double>(delegate(ref long v)
			{
				return (double)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(string), typeof(long), new TypeConverter<string, long>(delegate(ref string v)
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
					long num3;
					result = ((double.TryParse(v, out num2) && ConverterGroups.TryConvert<double, long>(ref num2, out num3)) ? num3 : 0L);
				}
				return result;
			}));
		}

		private static void RegisterUInt8Converters()
		{
			TypeConverterRegistry registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(byte), typeof(bool), new TypeConverter<byte, bool>(delegate(ref byte v)
			{
				return v > 0;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(byte), typeof(sbyte), new TypeConverter<byte, sbyte>(delegate(ref byte v)
			{
				return (sbyte)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(byte), typeof(char), new TypeConverter<byte, char>(delegate(ref byte v)
			{
				return (char)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(byte), typeof(short), new TypeConverter<byte, short>(delegate(ref byte v)
			{
				return (short)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(byte), typeof(int), new TypeConverter<byte, int>(delegate(ref byte v)
			{
				return (int)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(byte), typeof(long), new TypeConverter<byte, long>(delegate(ref byte v)
			{
				return (long)((ulong)v);
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(byte), typeof(ushort), new TypeConverter<byte, ushort>(delegate(ref byte v)
			{
				return (ushort)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(byte), typeof(uint), new TypeConverter<byte, uint>(delegate(ref byte v)
			{
				return (uint)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(byte), typeof(ulong), new TypeConverter<byte, ulong>(delegate(ref byte v)
			{
				return (ulong)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(byte), typeof(float), new TypeConverter<byte, float>(delegate(ref byte v)
			{
				return (float)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(byte), typeof(double), new TypeConverter<byte, double>(delegate(ref byte v)
			{
				return (double)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(byte), typeof(object), new TypeConverter<byte, object>(delegate(ref byte v)
			{
				return v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(string), typeof(byte), new TypeConverter<string, byte>(delegate(ref string v)
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
					byte b2;
					result = ((double.TryParse(v, out num) && ConverterGroups.TryConvert<double, byte>(ref num, out b2)) ? b2 : 0);
				}
				return result;
			}));
		}

		private static void RegisterUInt16Converters()
		{
			TypeConverterRegistry registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(ushort), typeof(bool), new TypeConverter<ushort, bool>(delegate(ref ushort v)
			{
				return v > 0;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(ushort), typeof(sbyte), new TypeConverter<ushort, sbyte>(delegate(ref ushort v)
			{
				return (sbyte)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(ushort), typeof(char), new TypeConverter<ushort, char>(delegate(ref ushort v)
			{
				return (char)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(ushort), typeof(short), new TypeConverter<ushort, short>(delegate(ref ushort v)
			{
				return (short)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(ushort), typeof(int), new TypeConverter<ushort, int>(delegate(ref ushort v)
			{
				return (int)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(ushort), typeof(long), new TypeConverter<ushort, long>(delegate(ref ushort v)
			{
				return (long)((ulong)v);
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(ushort), typeof(byte), new TypeConverter<ushort, byte>(delegate(ref ushort v)
			{
				return (byte)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(ushort), typeof(uint), new TypeConverter<ushort, uint>(delegate(ref ushort v)
			{
				return (uint)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(ushort), typeof(ulong), new TypeConverter<ushort, ulong>(delegate(ref ushort v)
			{
				return (ulong)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(ushort), typeof(float), new TypeConverter<ushort, float>(delegate(ref ushort v)
			{
				return (float)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(ushort), typeof(double), new TypeConverter<ushort, double>(delegate(ref ushort v)
			{
				return (double)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(string), typeof(ushort), new TypeConverter<string, ushort>(delegate(ref string v)
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
					ushort num3;
					result = ((double.TryParse(v, out num2) && ConverterGroups.TryConvert<double, ushort>(ref num2, out num3)) ? num3 : 0);
				}
				return result;
			}));
		}

		private static void RegisterUInt32Converters()
		{
			TypeConverterRegistry registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(uint), typeof(bool), new TypeConverter<uint, bool>(delegate(ref uint v)
			{
				return v > 0U;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(uint), typeof(sbyte), new TypeConverter<uint, sbyte>(delegate(ref uint v)
			{
				return (sbyte)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(uint), typeof(char), new TypeConverter<uint, char>(delegate(ref uint v)
			{
				return (char)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(uint), typeof(short), new TypeConverter<uint, short>(delegate(ref uint v)
			{
				return (short)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(uint), typeof(int), new TypeConverter<uint, int>(delegate(ref uint v)
			{
				return (int)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(uint), typeof(long), new TypeConverter<uint, long>(delegate(ref uint v)
			{
				return (long)((ulong)v);
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(uint), typeof(byte), new TypeConverter<uint, byte>(delegate(ref uint v)
			{
				return (byte)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(uint), typeof(ushort), new TypeConverter<uint, ushort>(delegate(ref uint v)
			{
				return (ushort)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(uint), typeof(ulong), new TypeConverter<uint, ulong>(delegate(ref uint v)
			{
				return (ulong)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(uint), typeof(float), new TypeConverter<uint, float>(delegate(ref uint v)
			{
				return v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(uint), typeof(double), new TypeConverter<uint, double>(delegate(ref uint v)
			{
				return v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(string), typeof(uint), new TypeConverter<string, uint>(delegate(ref string v)
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
					uint num3;
					result = ((double.TryParse(v, out num2) && ConverterGroups.TryConvert<double, uint>(ref num2, out num3)) ? num3 : 0U);
				}
				return result;
			}));
		}

		private static void RegisterUInt64Converters()
		{
			TypeConverterRegistry registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(ulong), typeof(bool), new TypeConverter<ulong, bool>(delegate(ref ulong v)
			{
				return v > 0UL;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(ulong), typeof(sbyte), new TypeConverter<ulong, sbyte>(delegate(ref ulong v)
			{
				return (sbyte)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(ulong), typeof(char), new TypeConverter<ulong, char>(delegate(ref ulong v)
			{
				return (char)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(ulong), typeof(short), new TypeConverter<ulong, short>(delegate(ref ulong v)
			{
				return (short)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(ulong), typeof(int), new TypeConverter<ulong, int>(delegate(ref ulong v)
			{
				return (int)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(ulong), typeof(long), new TypeConverter<ulong, long>(delegate(ref ulong v)
			{
				return (long)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(ulong), typeof(byte), new TypeConverter<ulong, byte>(delegate(ref ulong v)
			{
				return (byte)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(ulong), typeof(ushort), new TypeConverter<ulong, ushort>(delegate(ref ulong v)
			{
				return (ushort)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(ulong), typeof(uint), new TypeConverter<ulong, uint>(delegate(ref ulong v)
			{
				return (uint)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(ulong), typeof(float), new TypeConverter<ulong, float>(delegate(ref ulong v)
			{
				return v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(ulong), typeof(double), new TypeConverter<ulong, double>(delegate(ref ulong v)
			{
				return v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(string), typeof(ulong), new TypeConverter<string, ulong>(delegate(ref string v)
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
					ulong num3;
					result = ((double.TryParse(v, out num2) && ConverterGroups.TryConvert<double, ulong>(ref num2, out num3)) ? num3 : 0UL);
				}
				return result;
			}));
		}

		private static void RegisterFloatConverters()
		{
			TypeConverterRegistry registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(float), typeof(bool), new TypeConverter<float, bool>(delegate(ref float v)
			{
				return (double)v != 0.0;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(float), typeof(sbyte), new TypeConverter<float, sbyte>(delegate(ref float v)
			{
				return (sbyte)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(float), typeof(char), new TypeConverter<float, char>(delegate(ref float v)
			{
				return (char)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(float), typeof(short), new TypeConverter<float, short>(delegate(ref float v)
			{
				return (short)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(float), typeof(int), new TypeConverter<float, int>(delegate(ref float v)
			{
				return (int)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(float), typeof(long), new TypeConverter<float, long>(delegate(ref float v)
			{
				return (long)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(float), typeof(byte), new TypeConverter<float, byte>(delegate(ref float v)
			{
				return (byte)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(float), typeof(ushort), new TypeConverter<float, ushort>(delegate(ref float v)
			{
				return (ushort)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(float), typeof(uint), new TypeConverter<float, uint>(delegate(ref float v)
			{
				return (uint)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(float), typeof(ulong), new TypeConverter<float, ulong>(delegate(ref float v)
			{
				return (ulong)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(float), typeof(double), new TypeConverter<float, double>(delegate(ref float v)
			{
				return (double)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(float), typeof(string), new TypeConverter<float, string>(delegate(ref float v)
			{
				return v.ToString(CultureInfo.InvariantCulture);
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(string), typeof(float), new TypeConverter<string, float>(delegate(ref string v)
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
					float num3;
					result = ((double.TryParse(v, out num2) && ConverterGroups.TryConvert<double, float>(ref num2, out num3)) ? num3 : 0f);
				}
				return result;
			}));
		}

		private static void RegisterDoubleConverters()
		{
			TypeConverterRegistry registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(double), typeof(bool), new TypeConverter<double, bool>(delegate(ref double v)
			{
				return v != 0.0;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(double), typeof(sbyte), new TypeConverter<double, sbyte>(delegate(ref double v)
			{
				return (sbyte)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(double), typeof(char), new TypeConverter<double, char>(delegate(ref double v)
			{
				return (char)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(double), typeof(short), new TypeConverter<double, short>(delegate(ref double v)
			{
				return (short)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(double), typeof(int), new TypeConverter<double, int>(delegate(ref double v)
			{
				return (int)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(double), typeof(long), new TypeConverter<double, long>(delegate(ref double v)
			{
				return (long)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(double), typeof(byte), new TypeConverter<double, byte>(delegate(ref double v)
			{
				return (byte)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(double), typeof(ushort), new TypeConverter<double, ushort>(delegate(ref double v)
			{
				return (ushort)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(double), typeof(uint), new TypeConverter<double, uint>(delegate(ref double v)
			{
				return (uint)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(double), typeof(ulong), new TypeConverter<double, ulong>(delegate(ref double v)
			{
				return (ulong)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(double), typeof(float), new TypeConverter<double, float>(delegate(ref double v)
			{
				return (float)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(double), typeof(string), new TypeConverter<double, string>(delegate(ref double v)
			{
				return v.ToString(CultureInfo.InvariantCulture);
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(string), typeof(double), new TypeConverter<string, double>(delegate(ref string v)
			{
				double result;
				double.TryParse(v, out result);
				return result;
			}));
		}

		private static void RegisterBooleanConverters()
		{
			TypeConverterRegistry registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(bool), typeof(char), new TypeConverter<bool, char>(delegate(ref bool v)
			{
				return v ? '\u0001' : '\0';
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(bool), typeof(sbyte), new TypeConverter<bool, sbyte>(delegate(ref bool v)
			{
				return v ? 1 : 0;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(bool), typeof(short), new TypeConverter<bool, short>(delegate(ref bool v)
			{
				return v ? 1 : 0;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(bool), typeof(int), new TypeConverter<bool, int>(delegate(ref bool v)
			{
				return v ? 1 : 0;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(bool), typeof(long), new TypeConverter<bool, long>(delegate(ref bool v)
			{
				return v ? 1L : 0L;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(bool), typeof(byte), new TypeConverter<bool, byte>(delegate(ref bool v)
			{
				return v ? 1 : 0;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(bool), typeof(ushort), new TypeConverter<bool, ushort>(delegate(ref bool v)
			{
				return v ? 1 : 0;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(bool), typeof(uint), new TypeConverter<bool, uint>(delegate(ref bool v)
			{
				return v ? 1U : 0U;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(bool), typeof(ulong), new TypeConverter<bool, ulong>(delegate(ref bool v)
			{
				return v ? 1UL : 0UL;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(bool), typeof(float), new TypeConverter<bool, float>(delegate(ref bool v)
			{
				return v ? 1f : 0f;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(bool), typeof(double), new TypeConverter<bool, double>(delegate(ref bool v)
			{
				return v ? 1.0 : 0.0;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(string), typeof(bool), new TypeConverter<string, bool>(delegate(ref string v)
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
					bool flag3;
					result = (double.TryParse(v, out num) && ConverterGroups.TryConvert<double, bool>(ref num, out flag3) && flag3);
				}
				return result;
			}));
		}

		private static void RegisterCharConverters()
		{
			TypeConverterRegistry registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(char), typeof(bool), new TypeConverter<char, bool>(delegate(ref char v)
			{
				return v > '\0';
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(char), typeof(sbyte), new TypeConverter<char, sbyte>(delegate(ref char v)
			{
				return (sbyte)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(char), typeof(short), new TypeConverter<char, short>(delegate(ref char v)
			{
				return (short)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(char), typeof(int), new TypeConverter<char, int>(delegate(ref char v)
			{
				return (int)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(char), typeof(long), new TypeConverter<char, long>(delegate(ref char v)
			{
				return (long)((ulong)v);
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(char), typeof(byte), new TypeConverter<char, byte>(delegate(ref char v)
			{
				return (byte)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(char), typeof(ushort), new TypeConverter<char, ushort>(delegate(ref char v)
			{
				return (ushort)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(char), typeof(uint), new TypeConverter<char, uint>(delegate(ref char v)
			{
				return (uint)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(char), typeof(ulong), new TypeConverter<char, ulong>(delegate(ref char v)
			{
				return (ulong)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(char), typeof(float), new TypeConverter<char, float>(delegate(ref char v)
			{
				return (float)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(char), typeof(double), new TypeConverter<char, double>(delegate(ref char v)
			{
				return (double)v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(string), typeof(char), new TypeConverter<string, char>(delegate(ref string v)
			{
				return (!string.IsNullOrEmpty(v)) ? v[0] : '\0';
			}));
		}

		private static void RegisterColorConverters()
		{
			TypeConverterRegistry registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(Color), typeof(Color32), new TypeConverter<Color, Color32>(delegate(ref Color v)
			{
				return v;
			}));
			registry = ConverterGroups.s_PrimitivesConverters.registry;
			registry.Register(typeof(Color32), typeof(Color), new TypeConverter<Color32, Color>(delegate(ref Color32 v)
			{
				return v;
			}));
		}

		private static readonly ConverterGroup s_GlobalConverters = new ConverterGroup("__global_converters", null, null);

		private static readonly ConverterGroup s_PrimitivesConverters = new ConverterGroup("__primitives_converters", null, null);

		private static readonly Dictionary<string, ConverterGroup> s_BindingConverterGroups = new Dictionary<string, ConverterGroup>();
	}
}
