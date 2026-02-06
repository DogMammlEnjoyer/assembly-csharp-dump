using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

[BurstCompile]
public static class BurstClassInfo
{
	public unsafe static void NewClass<[IsUnmanaged] T>(string className, Dictionary<int, FieldInfo> fieldList, Dictionary<int, lua_CFunction> functionList, Dictionary<int, FunctionPointer<lua_CFunction>> functionPtrList) where T : struct, ValueType
	{
		if (!BurstClassInfo.ClassList.InfoFields.Data.IsCreated)
		{
			*BurstClassInfo.ClassList.InfoFields.Data = new NativeHashMap<int, BurstClassInfo.ClassInfo>(20, Allocator.Persistent);
		}
		BurstClassInfo.ClassList.MetatableNames<T>.Name = className;
		ReflectionMetaNames.ReflectedNames.TryAdd(typeof(T), className);
		BurstClassInfo.ClassInfo classInfo = default(BurstClassInfo.ClassInfo);
		classInfo.NameHash = LuaHashing.ByteHash(className);
		if (className.Length > 30)
		{
			throw new Exception("Name to long");
		}
		classInfo.Name = className;
		classInfo.Size = sizeof(T);
		classInfo.FieldList = new NativeHashMap<int, BurstClassInfo.BurstFieldInfo>(fieldList.Count, Allocator.Persistent);
		foreach (KeyValuePair<int, FieldInfo> keyValuePair in fieldList)
		{
			BurstClassInfo.BurstFieldInfo item = default(BurstClassInfo.BurstFieldInfo);
			item.NameHash = keyValuePair.Key;
			item.Name = keyValuePair.Value.Name;
			item.Offset = (int)Marshal.OffsetOf<T>(keyValuePair.Value.Name);
			Type fieldType = keyValuePair.Value.FieldType;
			if (fieldType == typeof(float))
			{
				item.FieldType = BurstClassInfo.EFieldTypes.Float;
			}
			else if (fieldType == typeof(int))
			{
				item.FieldType = BurstClassInfo.EFieldTypes.Int;
			}
			else if (fieldType == typeof(double))
			{
				item.FieldType = BurstClassInfo.EFieldTypes.Double;
			}
			else if (fieldType == typeof(bool))
			{
				item.FieldType = BurstClassInfo.EFieldTypes.Bool;
			}
			else if (fieldType == typeof(FixedString32Bytes))
			{
				item.FieldType = BurstClassInfo.EFieldTypes.String;
			}
			else if (!fieldType.IsPrimitive)
			{
				item.FieldType = BurstClassInfo.EFieldTypes.LightUserData;
				ReflectionMetaNames.ReflectedNames.TryGetValue(fieldType, out item.MetatableName);
			}
			item.Size = Marshal.SizeOf(fieldType);
			classInfo.FieldList.TryAdd(keyValuePair.Key, item);
		}
		classInfo.FunctionList = new NativeHashMap<int, IntPtr>(functionList.Count + functionPtrList.Count, Allocator.Persistent);
		foreach (KeyValuePair<int, lua_CFunction> keyValuePair2 in functionList)
		{
			classInfo.FunctionList.TryAdd(keyValuePair2.Key, Marshal.GetFunctionPointerForDelegate<lua_CFunction>(keyValuePair2.Value));
		}
		foreach (KeyValuePair<int, FunctionPointer<lua_CFunction>> keyValuePair3 in functionPtrList)
		{
			classInfo.FunctionList.TryAdd(keyValuePair3.Key, keyValuePair3.Value.Value);
		}
		BurstClassInfo.ClassList.InfoFields.Data.Add(classInfo.NameHash, classInfo);
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(BurstClassInfo.Index_00004923$PostfixBurstDelegate))]
	public unsafe static int Index(lua_State* L)
	{
		return BurstClassInfo.Index_00004923$BurstDirectCall.Invoke(L);
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(BurstClassInfo.NewIndex_00004924$PostfixBurstDelegate))]
	public unsafe static int NewIndex(lua_State* L)
	{
		return BurstClassInfo.NewIndex_00004924$BurstDirectCall.Invoke(L);
	}

	[BurstCompile]
	[MonoPInvokeCallback(typeof(BurstClassInfo.NameCall_00004925$PostfixBurstDelegate))]
	public unsafe static int NameCall(lua_State* L)
	{
		return BurstClassInfo.NameCall_00004925$BurstDirectCall.Invoke(L);
	}

	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static int Index$BurstManaged(lua_State* L)
	{
		FixedString32Bytes k_metatableLookup = BurstClassInfo._k_metatableLookup;
		byte* k = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref k_metatableLookup) + 2;
		Luau.luaL_getmetafield(L, 1, k);
		BurstClassInfo.ClassInfo classInfo;
		if (!BurstClassInfo.ClassList.InfoFields.Data.TryGetValue((int)Luau.luaL_checknumber(L, -1), out classInfo))
		{
			FixedString32Bytes fixedString32Bytes = "\"Internal Class Info Error\"";
			Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2));
			return 0;
		}
		Luau.lua_pop(L, 1);
		byte* tname = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref classInfo.Name) + 2;
		IntPtr pointer = IntPtr.Zero;
		Luau.lua_Types lua_Types = (Luau.lua_Types)Luau.lua_type(L, 1);
		if (lua_Types == Luau.lua_Types.LUA_TUSERDATA)
		{
			pointer = (IntPtr)Luau.luaL_checkudata(L, 1, tname);
		}
		else
		{
			if (lua_Types != Luau.lua_Types.LUA_TTABLE)
			{
				FixedString32Bytes fixedString32Bytes2 = "\"Unknown type for __index\"";
				Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
				return 0;
			}
			pointer = Luau.lua_light_ptr(L, 1);
		}
		int len = Luau.lua_objlen(L, 2);
		int key = LuaHashing.ByteHash(Luau.luaL_checkstring(L, 2), len);
		BurstClassInfo.BurstFieldInfo burstFieldInfo;
		if (classInfo.FieldList.TryGetValue(key, out burstFieldInfo))
		{
			IntPtr intPtr = pointer + burstFieldInfo.Offset;
			switch (burstFieldInfo.FieldType)
			{
			case BurstClassInfo.EFieldTypes.Float:
				Luau.lua_pushnumber(L, (double)(*(float*)((void*)intPtr)));
				return 1;
			case BurstClassInfo.EFieldTypes.Int:
				Luau.lua_pushnumber(L, (double)(*(int*)((void*)intPtr)));
				return 1;
			case BurstClassInfo.EFieldTypes.Double:
				Luau.lua_pushnumber(L, *(double*)((void*)intPtr));
				return 1;
			case BurstClassInfo.EFieldTypes.Bool:
				Luau.lua_pushboolean(L, (*(byte*)((void*)intPtr) != 0) ? 1 : 0);
				return 1;
			case BurstClassInfo.EFieldTypes.String:
				Luau.lua_pushstring(L, (byte*)((void*)intPtr) + 2);
				return 1;
			case BurstClassInfo.EFieldTypes.LightUserData:
				Luau.lua_class_push(L, burstFieldInfo.MetatableName, intPtr);
				return 1;
			}
		}
		IntPtr ptr;
		if (classInfo.FunctionList.TryGetValue(key, out ptr))
		{
			FunctionPointer<lua_CFunction> fn = new FunctionPointer<lua_CFunction>(ptr);
			FixedString32Bytes fixedString32Bytes3 = "";
			Luau.lua_pushcclosurek(L, fn, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes3) + 2, 0, null);
			return 1;
		}
		FixedString32Bytes fixedString32Bytes4 = "\"Unknown Type?\"";
		Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes4) + 2));
		return 0;
	}

	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static int NewIndex$BurstManaged(lua_State* L)
	{
		FixedString32Bytes k_metatableLookup = BurstClassInfo._k_metatableLookup;
		byte* k = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref k_metatableLookup) + 2;
		Luau.luaL_getmetafield(L, 1, k);
		BurstClassInfo.ClassInfo classInfo;
		if (!BurstClassInfo.ClassList.InfoFields.Data.TryGetValue((int)Luau.luaL_checknumber(L, -1), out classInfo))
		{
			FixedString32Bytes fixedString32Bytes = "\"Internal Class Info Error\"";
			Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2));
			return 0;
		}
		Luau.lua_pop(L, 1);
		byte* tname = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref classInfo.Name) + 2;
		IntPtr pointer = IntPtr.Zero;
		Luau.lua_Types lua_Types = (Luau.lua_Types)Luau.lua_type(L, 1);
		if (lua_Types == Luau.lua_Types.LUA_TUSERDATA)
		{
			pointer = (IntPtr)Luau.luaL_checkudata(L, 1, tname);
		}
		else
		{
			if (lua_Types != Luau.lua_Types.LUA_TTABLE)
			{
				FixedString32Bytes fixedString32Bytes2 = "\"Unknown type for __newindex\"";
				Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
				return 0;
			}
			pointer = Luau.lua_light_ptr(L, 1);
		}
		int len = Luau.lua_objlen(L, 2);
		int key = LuaHashing.ByteHash(Luau.luaL_checkstring(L, 2), len);
		BurstClassInfo.BurstFieldInfo burstFieldInfo;
		if (classInfo.FieldList.TryGetValue(key, out burstFieldInfo))
		{
			IntPtr value = pointer + burstFieldInfo.Offset;
			switch (burstFieldInfo.FieldType)
			{
			case BurstClassInfo.EFieldTypes.Float:
				*(float*)((void*)value) = (float)Luau.luaL_checknumber(L, 3);
				return 0;
			case BurstClassInfo.EFieldTypes.Int:
				*(int*)((void*)value) = (int)Luau.luaL_checknumber(L, 3);
				return 0;
			case BurstClassInfo.EFieldTypes.Double:
				*(double*)((void*)value) = Luau.luaL_checknumber(L, 3);
				return 0;
			case BurstClassInfo.EFieldTypes.Bool:
				*(byte*)((void*)value) = ((Luau.lua_toboolean(L, 3) != 0) ? 1 : 0);
				return 0;
			case BurstClassInfo.EFieldTypes.LightUserData:
				Buffer.MemoryCopy((void*)((IntPtr)((void*)Luau.lua_class_get(L, 3, burstFieldInfo.MetatableName))), (void*)value, (long)burstFieldInfo.Size, (long)burstFieldInfo.Size);
				return 0;
			}
		}
		FixedString32Bytes fixedString32Bytes3 = "\"Unknown Type\"";
		Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes3) + 2));
		return 0;
	}

	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static int NameCall$BurstManaged(lua_State* L)
	{
		FixedString32Bytes k_metatableLookup = BurstClassInfo._k_metatableLookup;
		byte* k = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref k_metatableLookup) + 2;
		Luau.luaL_getmetafield(L, 1, k);
		BurstClassInfo.ClassInfo classInfo;
		if (!BurstClassInfo.ClassList.InfoFields.Data.TryGetValue((int)Luau.luaL_checknumber(L, -1), out classInfo))
		{
			FixedString32Bytes fixedString32Bytes = "\"Internal Class Info Error\"";
			Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2));
			return 0;
		}
		Luau.lua_pop(L, 1);
		int key = LuaHashing.ByteHash(Luau.lua_namecallatom(L, null));
		IntPtr ptr;
		if (classInfo.FunctionList.TryGetValue(key, out ptr))
		{
			FunctionPointer<lua_CFunction> functionPointer = new FunctionPointer<lua_CFunction>(ptr);
			return functionPointer.Invoke(L);
		}
		FixedString32Bytes fixedString32Bytes2 = "\"Function not found in function list\"";
		Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
		return 0;
	}

	private static readonly FixedString32Bytes _k_metatableLookup = "metahash";

	public enum EFieldTypes
	{
		Float,
		Int,
		Double,
		Bool,
		String,
		LightUserData
	}

	[BurstCompile]
	public struct BurstFieldInfo
	{
		public int NameHash;

		public FixedString32Bytes Name;

		public FixedString32Bytes MetatableName;

		public int Offset;

		public BurstClassInfo.EFieldTypes FieldType;

		public int Size;
	}

	[BurstCompile]
	public struct ClassInfo
	{
		public int NameHash;

		public int Size;

		public FixedString32Bytes Name;

		public NativeHashMap<int, BurstClassInfo.BurstFieldInfo> FieldList;

		public NativeHashMap<int, IntPtr> FunctionList;
	}

	public abstract class ClassList
	{
		public static readonly SharedStatic<NativeHashMap<int, BurstClassInfo.ClassInfo>> InfoFields = SharedStatic<NativeHashMap<int, BurstClassInfo.ClassInfo>>.GetOrCreateUnsafe(0U, -7258312696341931442L, -7445903157129162016L);

		private class FieldKey
		{
		}

		public static class MetatableNames<T>
		{
			public static FixedString32Bytes Name;
		}
	}

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal unsafe delegate int Index_00004923$PostfixBurstDelegate(lua_State* L);

	internal static class Index_00004923$BurstDirectCall
	{
		[BurstDiscard]
		private static void GetFunctionPointerDiscard(ref IntPtr A_0)
		{
			if (BurstClassInfo.Index_00004923$BurstDirectCall.Pointer == 0)
			{
				BurstClassInfo.Index_00004923$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstClassInfo.Index_00004923$PostfixBurstDelegate>(new BurstClassInfo.Index_00004923$PostfixBurstDelegate(BurstClassInfo.Index)).Value;
			}
			A_0 = BurstClassInfo.Index_00004923$BurstDirectCall.Pointer;
		}

		private static IntPtr GetFunctionPointer()
		{
			IntPtr result = (IntPtr)0;
			BurstClassInfo.Index_00004923$BurstDirectCall.GetFunctionPointerDiscard(ref result);
			return result;
		}

		public unsafe static int Invoke(lua_State* L)
		{
			if (BurstCompiler.IsEnabled)
			{
				IntPtr functionPointer = BurstClassInfo.Index_00004923$BurstDirectCall.GetFunctionPointer();
				if (functionPointer != 0)
				{
					return calli(System.Int32(lua_State*), L, functionPointer);
				}
			}
			return BurstClassInfo.Index$BurstManaged(L);
		}

		private static IntPtr Pointer;
	}

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal unsafe delegate int NewIndex_00004924$PostfixBurstDelegate(lua_State* L);

	internal static class NewIndex_00004924$BurstDirectCall
	{
		[BurstDiscard]
		private static void GetFunctionPointerDiscard(ref IntPtr A_0)
		{
			if (BurstClassInfo.NewIndex_00004924$BurstDirectCall.Pointer == 0)
			{
				BurstClassInfo.NewIndex_00004924$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstClassInfo.NewIndex_00004924$PostfixBurstDelegate>(new BurstClassInfo.NewIndex_00004924$PostfixBurstDelegate(BurstClassInfo.NewIndex)).Value;
			}
			A_0 = BurstClassInfo.NewIndex_00004924$BurstDirectCall.Pointer;
		}

		private static IntPtr GetFunctionPointer()
		{
			IntPtr result = (IntPtr)0;
			BurstClassInfo.NewIndex_00004924$BurstDirectCall.GetFunctionPointerDiscard(ref result);
			return result;
		}

		public unsafe static int Invoke(lua_State* L)
		{
			if (BurstCompiler.IsEnabled)
			{
				IntPtr functionPointer = BurstClassInfo.NewIndex_00004924$BurstDirectCall.GetFunctionPointer();
				if (functionPointer != 0)
				{
					return calli(System.Int32(lua_State*), L, functionPointer);
				}
			}
			return BurstClassInfo.NewIndex$BurstManaged(L);
		}

		private static IntPtr Pointer;
	}

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal unsafe delegate int NameCall_00004925$PostfixBurstDelegate(lua_State* L);

	internal static class NameCall_00004925$BurstDirectCall
	{
		[BurstDiscard]
		private static void GetFunctionPointerDiscard(ref IntPtr A_0)
		{
			if (BurstClassInfo.NameCall_00004925$BurstDirectCall.Pointer == 0)
			{
				BurstClassInfo.NameCall_00004925$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstClassInfo.NameCall_00004925$PostfixBurstDelegate>(new BurstClassInfo.NameCall_00004925$PostfixBurstDelegate(BurstClassInfo.NameCall)).Value;
			}
			A_0 = BurstClassInfo.NameCall_00004925$BurstDirectCall.Pointer;
		}

		private static IntPtr GetFunctionPointer()
		{
			IntPtr result = (IntPtr)0;
			BurstClassInfo.NameCall_00004925$BurstDirectCall.GetFunctionPointerDiscard(ref result);
			return result;
		}

		public unsafe static int Invoke(lua_State* L)
		{
			if (BurstCompiler.IsEnabled)
			{
				IntPtr functionPointer = BurstClassInfo.NameCall_00004925$BurstDirectCall.GetFunctionPointer();
				if (functionPointer != 0)
				{
					return calli(System.Int32(lua_State*), L, functionPointer);
				}
			}
			return BurstClassInfo.NameCall$BurstManaged(L);
		}

		private static IntPtr Pointer;
	}
}
