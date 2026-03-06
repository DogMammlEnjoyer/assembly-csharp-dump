using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions
{
	internal static class CachedReflectionInfo
	{
		public static MethodInfo String_Format_String_ObjectArray
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_String_Format_String_ObjectArray) == null)
				{
					result = (CachedReflectionInfo.s_String_Format_String_ObjectArray = typeof(string).GetMethod("Format", new Type[]
					{
						typeof(string),
						typeof(object[])
					}));
				}
				return result;
			}
		}

		public static ConstructorInfo InvalidCastException_Ctor_String
		{
			get
			{
				ConstructorInfo result;
				if ((result = CachedReflectionInfo.s_InvalidCastException_Ctor_String) == null)
				{
					result = (CachedReflectionInfo.s_InvalidCastException_Ctor_String = typeof(InvalidCastException).GetConstructor(new Type[]
					{
						typeof(string)
					}));
				}
				return result;
			}
		}

		public static MethodInfo CallSiteOps_SetNotMatched
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_CallSiteOps_SetNotMatched) == null)
				{
					result = (CachedReflectionInfo.s_CallSiteOps_SetNotMatched = typeof(CallSiteOps).GetMethod("SetNotMatched"));
				}
				return result;
			}
		}

		public static MethodInfo CallSiteOps_CreateMatchmaker
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_CallSiteOps_CreateMatchmaker) == null)
				{
					result = (CachedReflectionInfo.s_CallSiteOps_CreateMatchmaker = typeof(CallSiteOps).GetMethod("CreateMatchmaker"));
				}
				return result;
			}
		}

		public static MethodInfo CallSiteOps_GetMatch
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_CallSiteOps_GetMatch) == null)
				{
					result = (CachedReflectionInfo.s_CallSiteOps_GetMatch = typeof(CallSiteOps).GetMethod("GetMatch"));
				}
				return result;
			}
		}

		public static MethodInfo CallSiteOps_ClearMatch
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_CallSiteOps_ClearMatch) == null)
				{
					result = (CachedReflectionInfo.s_CallSiteOps_ClearMatch = typeof(CallSiteOps).GetMethod("ClearMatch"));
				}
				return result;
			}
		}

		public static MethodInfo CallSiteOps_UpdateRules
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_CallSiteOps_UpdateRules) == null)
				{
					result = (CachedReflectionInfo.s_CallSiteOps_UpdateRules = typeof(CallSiteOps).GetMethod("UpdateRules"));
				}
				return result;
			}
		}

		public static MethodInfo CallSiteOps_GetRules
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_CallSiteOps_GetRules) == null)
				{
					result = (CachedReflectionInfo.s_CallSiteOps_GetRules = typeof(CallSiteOps).GetMethod("GetRules"));
				}
				return result;
			}
		}

		public static MethodInfo CallSiteOps_GetRuleCache
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_CallSiteOps_GetRuleCache) == null)
				{
					result = (CachedReflectionInfo.s_CallSiteOps_GetRuleCache = typeof(CallSiteOps).GetMethod("GetRuleCache"));
				}
				return result;
			}
		}

		public static MethodInfo CallSiteOps_GetCachedRules
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_CallSiteOps_GetCachedRules) == null)
				{
					result = (CachedReflectionInfo.s_CallSiteOps_GetCachedRules = typeof(CallSiteOps).GetMethod("GetCachedRules"));
				}
				return result;
			}
		}

		public static MethodInfo CallSiteOps_AddRule
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_CallSiteOps_AddRule) == null)
				{
					result = (CachedReflectionInfo.s_CallSiteOps_AddRule = typeof(CallSiteOps).GetMethod("AddRule"));
				}
				return result;
			}
		}

		public static MethodInfo CallSiteOps_MoveRule
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_CallSiteOps_MoveRule) == null)
				{
					result = (CachedReflectionInfo.s_CallSiteOps_MoveRule = typeof(CallSiteOps).GetMethod("MoveRule"));
				}
				return result;
			}
		}

		public static MethodInfo CallSiteOps_Bind
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_CallSiteOps_Bind) == null)
				{
					result = (CachedReflectionInfo.s_CallSiteOps_Bind = typeof(CallSiteOps).GetMethod("Bind"));
				}
				return result;
			}
		}

		public static MethodInfo DynamicObject_TryGetMember
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_DynamicObject_TryGetMember) == null)
				{
					result = (CachedReflectionInfo.s_DynamicObject_TryGetMember = typeof(DynamicObject).GetMethod("TryGetMember"));
				}
				return result;
			}
		}

		public static MethodInfo DynamicObject_TrySetMember
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_DynamicObject_TrySetMember) == null)
				{
					result = (CachedReflectionInfo.s_DynamicObject_TrySetMember = typeof(DynamicObject).GetMethod("TrySetMember"));
				}
				return result;
			}
		}

		public static MethodInfo DynamicObject_TryDeleteMember
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_DynamicObject_TryDeleteMember) == null)
				{
					result = (CachedReflectionInfo.s_DynamicObject_TryDeleteMember = typeof(DynamicObject).GetMethod("TryDeleteMember"));
				}
				return result;
			}
		}

		public static MethodInfo DynamicObject_TryGetIndex
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_DynamicObject_TryGetIndex) == null)
				{
					result = (CachedReflectionInfo.s_DynamicObject_TryGetIndex = typeof(DynamicObject).GetMethod("TryGetIndex"));
				}
				return result;
			}
		}

		public static MethodInfo DynamicObject_TrySetIndex
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_DynamicObject_TrySetIndex) == null)
				{
					result = (CachedReflectionInfo.s_DynamicObject_TrySetIndex = typeof(DynamicObject).GetMethod("TrySetIndex"));
				}
				return result;
			}
		}

		public static MethodInfo DynamicObject_TryDeleteIndex
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_DynamicObject_TryDeleteIndex) == null)
				{
					result = (CachedReflectionInfo.s_DynamicObject_TryDeleteIndex = typeof(DynamicObject).GetMethod("TryDeleteIndex"));
				}
				return result;
			}
		}

		public static MethodInfo DynamicObject_TryConvert
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_DynamicObject_TryConvert) == null)
				{
					result = (CachedReflectionInfo.s_DynamicObject_TryConvert = typeof(DynamicObject).GetMethod("TryConvert"));
				}
				return result;
			}
		}

		public static MethodInfo DynamicObject_TryInvoke
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_DynamicObject_TryInvoke) == null)
				{
					result = (CachedReflectionInfo.s_DynamicObject_TryInvoke = typeof(DynamicObject).GetMethod("TryInvoke"));
				}
				return result;
			}
		}

		public static MethodInfo DynamicObject_TryInvokeMember
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_DynamicObject_TryInvokeMember) == null)
				{
					result = (CachedReflectionInfo.s_DynamicObject_TryInvokeMember = typeof(DynamicObject).GetMethod("TryInvokeMember"));
				}
				return result;
			}
		}

		public static MethodInfo DynamicObject_TryBinaryOperation
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_DynamicObject_TryBinaryOperation) == null)
				{
					result = (CachedReflectionInfo.s_DynamicObject_TryBinaryOperation = typeof(DynamicObject).GetMethod("TryBinaryOperation"));
				}
				return result;
			}
		}

		public static MethodInfo DynamicObject_TryUnaryOperation
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_DynamicObject_TryUnaryOperation) == null)
				{
					result = (CachedReflectionInfo.s_DynamicObject_TryUnaryOperation = typeof(DynamicObject).GetMethod("TryUnaryOperation"));
				}
				return result;
			}
		}

		public static MethodInfo DynamicObject_TryCreateInstance
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_DynamicObject_TryCreateInstance) == null)
				{
					result = (CachedReflectionInfo.s_DynamicObject_TryCreateInstance = typeof(DynamicObject).GetMethod("TryCreateInstance"));
				}
				return result;
			}
		}

		public static ConstructorInfo Nullable_Boolean_Ctor
		{
			get
			{
				ConstructorInfo result;
				if ((result = CachedReflectionInfo.s_Nullable_Boolean_Ctor) == null)
				{
					result = (CachedReflectionInfo.s_Nullable_Boolean_Ctor = typeof(bool?).GetConstructor(new Type[]
					{
						typeof(bool)
					}));
				}
				return result;
			}
		}

		public static ConstructorInfo Decimal_Ctor_Int32
		{
			get
			{
				ConstructorInfo result;
				if ((result = CachedReflectionInfo.s_Decimal_Ctor_Int32) == null)
				{
					result = (CachedReflectionInfo.s_Decimal_Ctor_Int32 = typeof(decimal).GetConstructor(new Type[]
					{
						typeof(int)
					}));
				}
				return result;
			}
		}

		public static ConstructorInfo Decimal_Ctor_UInt32
		{
			get
			{
				ConstructorInfo result;
				if ((result = CachedReflectionInfo.s_Decimal_Ctor_UInt32) == null)
				{
					result = (CachedReflectionInfo.s_Decimal_Ctor_UInt32 = typeof(decimal).GetConstructor(new Type[]
					{
						typeof(uint)
					}));
				}
				return result;
			}
		}

		public static ConstructorInfo Decimal_Ctor_Int64
		{
			get
			{
				ConstructorInfo result;
				if ((result = CachedReflectionInfo.s_Decimal_Ctor_Int64) == null)
				{
					result = (CachedReflectionInfo.s_Decimal_Ctor_Int64 = typeof(decimal).GetConstructor(new Type[]
					{
						typeof(long)
					}));
				}
				return result;
			}
		}

		public static ConstructorInfo Decimal_Ctor_UInt64
		{
			get
			{
				ConstructorInfo result;
				if ((result = CachedReflectionInfo.s_Decimal_Ctor_UInt64) == null)
				{
					result = (CachedReflectionInfo.s_Decimal_Ctor_UInt64 = typeof(decimal).GetConstructor(new Type[]
					{
						typeof(ulong)
					}));
				}
				return result;
			}
		}

		public static ConstructorInfo Decimal_Ctor_Int32_Int32_Int32_Bool_Byte
		{
			get
			{
				ConstructorInfo result;
				if ((result = CachedReflectionInfo.s_Decimal_Ctor_Int32_Int32_Int32_Bool_Byte) == null)
				{
					result = (CachedReflectionInfo.s_Decimal_Ctor_Int32_Int32_Int32_Bool_Byte = typeof(decimal).GetConstructor(new Type[]
					{
						typeof(int),
						typeof(int),
						typeof(int),
						typeof(bool),
						typeof(byte)
					}));
				}
				return result;
			}
		}

		public static FieldInfo Decimal_One
		{
			get
			{
				FieldInfo result;
				if ((result = CachedReflectionInfo.s_Decimal_One) == null)
				{
					result = (CachedReflectionInfo.s_Decimal_One = typeof(decimal).GetField("One"));
				}
				return result;
			}
		}

		public static FieldInfo Decimal_MinusOne
		{
			get
			{
				FieldInfo result;
				if ((result = CachedReflectionInfo.s_Decimal_MinusOne) == null)
				{
					result = (CachedReflectionInfo.s_Decimal_MinusOne = typeof(decimal).GetField("MinusOne"));
				}
				return result;
			}
		}

		public static FieldInfo Decimal_MinValue
		{
			get
			{
				FieldInfo result;
				if ((result = CachedReflectionInfo.s_Decimal_MinValue) == null)
				{
					result = (CachedReflectionInfo.s_Decimal_MinValue = typeof(decimal).GetField("MinValue"));
				}
				return result;
			}
		}

		public static FieldInfo Decimal_MaxValue
		{
			get
			{
				FieldInfo result;
				if ((result = CachedReflectionInfo.s_Decimal_MaxValue) == null)
				{
					result = (CachedReflectionInfo.s_Decimal_MaxValue = typeof(decimal).GetField("MaxValue"));
				}
				return result;
			}
		}

		public static FieldInfo Decimal_Zero
		{
			get
			{
				FieldInfo result;
				if ((result = CachedReflectionInfo.s_Decimal_Zero) == null)
				{
					result = (CachedReflectionInfo.s_Decimal_Zero = typeof(decimal).GetField("Zero"));
				}
				return result;
			}
		}

		public static FieldInfo DateTime_MinValue
		{
			get
			{
				FieldInfo result;
				if ((result = CachedReflectionInfo.s_DateTime_MinValue) == null)
				{
					result = (CachedReflectionInfo.s_DateTime_MinValue = typeof(DateTime).GetField("MinValue"));
				}
				return result;
			}
		}

		public static MethodInfo MethodBase_GetMethodFromHandle_RuntimeMethodHandle
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_MethodBase_GetMethodFromHandle_RuntimeMethodHandle) == null)
				{
					result = (CachedReflectionInfo.s_MethodBase_GetMethodFromHandle_RuntimeMethodHandle = typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[]
					{
						typeof(RuntimeMethodHandle)
					}));
				}
				return result;
			}
		}

		public static MethodInfo MethodBase_GetMethodFromHandle_RuntimeMethodHandle_RuntimeTypeHandle
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_MethodBase_GetMethodFromHandle_RuntimeMethodHandle_RuntimeTypeHandle) == null)
				{
					result = (CachedReflectionInfo.s_MethodBase_GetMethodFromHandle_RuntimeMethodHandle_RuntimeTypeHandle = typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[]
					{
						typeof(RuntimeMethodHandle),
						typeof(RuntimeTypeHandle)
					}));
				}
				return result;
			}
		}

		public static MethodInfo MethodInfo_CreateDelegate_Type_Object
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_MethodInfo_CreateDelegate_Type_Object) == null)
				{
					result = (CachedReflectionInfo.s_MethodInfo_CreateDelegate_Type_Object = typeof(MethodInfo).GetMethod("CreateDelegate", new Type[]
					{
						typeof(Type),
						typeof(object)
					}));
				}
				return result;
			}
		}

		public static MethodInfo String_op_Equality_String_String
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_String_op_Equality_String_String) == null)
				{
					result = (CachedReflectionInfo.s_String_op_Equality_String_String = typeof(string).GetMethod("op_Equality", new Type[]
					{
						typeof(string),
						typeof(string)
					}));
				}
				return result;
			}
		}

		public static MethodInfo String_Equals_String_String
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_String_Equals_String_String) == null)
				{
					result = (CachedReflectionInfo.s_String_Equals_String_String = typeof(string).GetMethod("Equals", new Type[]
					{
						typeof(string),
						typeof(string)
					}));
				}
				return result;
			}
		}

		public static MethodInfo DictionaryOfStringInt32_Add_String_Int32
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_DictionaryOfStringInt32_Add_String_Int32) == null)
				{
					result = (CachedReflectionInfo.s_DictionaryOfStringInt32_Add_String_Int32 = typeof(Dictionary<string, int>).GetMethod("Add", new Type[]
					{
						typeof(string),
						typeof(int)
					}));
				}
				return result;
			}
		}

		public static ConstructorInfo DictionaryOfStringInt32_Ctor_Int32
		{
			get
			{
				ConstructorInfo result;
				if ((result = CachedReflectionInfo.s_DictionaryOfStringInt32_Ctor_Int32) == null)
				{
					result = (CachedReflectionInfo.s_DictionaryOfStringInt32_Ctor_Int32 = typeof(Dictionary<string, int>).GetConstructor(new Type[]
					{
						typeof(int)
					}));
				}
				return result;
			}
		}

		public static MethodInfo Type_GetTypeFromHandle
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_Type_GetTypeFromHandle) == null)
				{
					result = (CachedReflectionInfo.s_Type_GetTypeFromHandle = typeof(Type).GetMethod("GetTypeFromHandle"));
				}
				return result;
			}
		}

		public static MethodInfo Object_GetType
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_Object_GetType) == null)
				{
					result = (CachedReflectionInfo.s_Object_GetType = typeof(object).GetMethod("GetType"));
				}
				return result;
			}
		}

		public static MethodInfo Decimal_op_Implicit_Byte
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_Decimal_op_Implicit_Byte) == null)
				{
					result = (CachedReflectionInfo.s_Decimal_op_Implicit_Byte = typeof(decimal).GetMethod("op_Implicit", new Type[]
					{
						typeof(byte)
					}));
				}
				return result;
			}
		}

		public static MethodInfo Decimal_op_Implicit_SByte
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_Decimal_op_Implicit_SByte) == null)
				{
					result = (CachedReflectionInfo.s_Decimal_op_Implicit_SByte = typeof(decimal).GetMethod("op_Implicit", new Type[]
					{
						typeof(sbyte)
					}));
				}
				return result;
			}
		}

		public static MethodInfo Decimal_op_Implicit_Int16
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_Decimal_op_Implicit_Int16) == null)
				{
					result = (CachedReflectionInfo.s_Decimal_op_Implicit_Int16 = typeof(decimal).GetMethod("op_Implicit", new Type[]
					{
						typeof(short)
					}));
				}
				return result;
			}
		}

		public static MethodInfo Decimal_op_Implicit_UInt16
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_Decimal_op_Implicit_UInt16) == null)
				{
					result = (CachedReflectionInfo.s_Decimal_op_Implicit_UInt16 = typeof(decimal).GetMethod("op_Implicit", new Type[]
					{
						typeof(ushort)
					}));
				}
				return result;
			}
		}

		public static MethodInfo Decimal_op_Implicit_Int32
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_Decimal_op_Implicit_Int32) == null)
				{
					result = (CachedReflectionInfo.s_Decimal_op_Implicit_Int32 = typeof(decimal).GetMethod("op_Implicit", new Type[]
					{
						typeof(int)
					}));
				}
				return result;
			}
		}

		public static MethodInfo Decimal_op_Implicit_UInt32
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_Decimal_op_Implicit_UInt32) == null)
				{
					result = (CachedReflectionInfo.s_Decimal_op_Implicit_UInt32 = typeof(decimal).GetMethod("op_Implicit", new Type[]
					{
						typeof(uint)
					}));
				}
				return result;
			}
		}

		public static MethodInfo Decimal_op_Implicit_Int64
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_Decimal_op_Implicit_Int64) == null)
				{
					result = (CachedReflectionInfo.s_Decimal_op_Implicit_Int64 = typeof(decimal).GetMethod("op_Implicit", new Type[]
					{
						typeof(long)
					}));
				}
				return result;
			}
		}

		public static MethodInfo Decimal_op_Implicit_UInt64
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_Decimal_op_Implicit_UInt64) == null)
				{
					result = (CachedReflectionInfo.s_Decimal_op_Implicit_UInt64 = typeof(decimal).GetMethod("op_Implicit", new Type[]
					{
						typeof(ulong)
					}));
				}
				return result;
			}
		}

		public static MethodInfo Decimal_op_Implicit_Char
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_Decimal_op_Implicit_Char) == null)
				{
					result = (CachedReflectionInfo.s_Decimal_op_Implicit_Char = typeof(decimal).GetMethod("op_Implicit", new Type[]
					{
						typeof(char)
					}));
				}
				return result;
			}
		}

		public static MethodInfo Math_Pow_Double_Double
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_Math_Pow_Double_Double) == null)
				{
					result = (CachedReflectionInfo.s_Math_Pow_Double_Double = typeof(Math).GetMethod("Pow", new Type[]
					{
						typeof(double),
						typeof(double)
					}));
				}
				return result;
			}
		}

		public static ConstructorInfo Closure_ObjectArray_ObjectArray
		{
			get
			{
				ConstructorInfo result;
				if ((result = CachedReflectionInfo.s_Closure_ObjectArray_ObjectArray) == null)
				{
					result = (CachedReflectionInfo.s_Closure_ObjectArray_ObjectArray = typeof(Closure).GetConstructor(new Type[]
					{
						typeof(object[]),
						typeof(object[])
					}));
				}
				return result;
			}
		}

		public static FieldInfo Closure_Constants
		{
			get
			{
				FieldInfo result;
				if ((result = CachedReflectionInfo.s_Closure_Constants) == null)
				{
					result = (CachedReflectionInfo.s_Closure_Constants = typeof(Closure).GetField("Constants"));
				}
				return result;
			}
		}

		public static FieldInfo Closure_Locals
		{
			get
			{
				FieldInfo result;
				if ((result = CachedReflectionInfo.s_Closure_Locals) == null)
				{
					result = (CachedReflectionInfo.s_Closure_Locals = typeof(Closure).GetField("Locals"));
				}
				return result;
			}
		}

		public static MethodInfo RuntimeOps_CreateRuntimeVariables_ObjectArray_Int64Array
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_RuntimeOps_CreateRuntimeVariables_ObjectArray_Int64Array) == null)
				{
					result = (CachedReflectionInfo.s_RuntimeOps_CreateRuntimeVariables_ObjectArray_Int64Array = typeof(RuntimeOps).GetMethod("CreateRuntimeVariables", new Type[]
					{
						typeof(object[]),
						typeof(long[])
					}));
				}
				return result;
			}
		}

		public static MethodInfo RuntimeOps_CreateRuntimeVariables
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_RuntimeOps_CreateRuntimeVariables) == null)
				{
					result = (CachedReflectionInfo.s_RuntimeOps_CreateRuntimeVariables = typeof(RuntimeOps).GetMethod("CreateRuntimeVariables", Type.EmptyTypes));
				}
				return result;
			}
		}

		public static MethodInfo RuntimeOps_MergeRuntimeVariables
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_RuntimeOps_MergeRuntimeVariables) == null)
				{
					result = (CachedReflectionInfo.s_RuntimeOps_MergeRuntimeVariables = typeof(RuntimeOps).GetMethod("MergeRuntimeVariables"));
				}
				return result;
			}
		}

		public static MethodInfo RuntimeOps_Quote
		{
			get
			{
				MethodInfo result;
				if ((result = CachedReflectionInfo.s_RuntimeOps_Quote) == null)
				{
					result = (CachedReflectionInfo.s_RuntimeOps_Quote = typeof(RuntimeOps).GetMethod("Quote"));
				}
				return result;
			}
		}

		private static MethodInfo s_String_Format_String_ObjectArray;

		private static ConstructorInfo s_InvalidCastException_Ctor_String;

		private static MethodInfo s_CallSiteOps_SetNotMatched;

		private static MethodInfo s_CallSiteOps_CreateMatchmaker;

		private static MethodInfo s_CallSiteOps_GetMatch;

		private static MethodInfo s_CallSiteOps_ClearMatch;

		private static MethodInfo s_CallSiteOps_UpdateRules;

		private static MethodInfo s_CallSiteOps_GetRules;

		private static MethodInfo s_CallSiteOps_GetRuleCache;

		private static MethodInfo s_CallSiteOps_GetCachedRules;

		private static MethodInfo s_CallSiteOps_AddRule;

		private static MethodInfo s_CallSiteOps_MoveRule;

		private static MethodInfo s_CallSiteOps_Bind;

		private static MethodInfo s_DynamicObject_TryGetMember;

		private static MethodInfo s_DynamicObject_TrySetMember;

		private static MethodInfo s_DynamicObject_TryDeleteMember;

		private static MethodInfo s_DynamicObject_TryGetIndex;

		private static MethodInfo s_DynamicObject_TrySetIndex;

		private static MethodInfo s_DynamicObject_TryDeleteIndex;

		private static MethodInfo s_DynamicObject_TryConvert;

		private static MethodInfo s_DynamicObject_TryInvoke;

		private static MethodInfo s_DynamicObject_TryInvokeMember;

		private static MethodInfo s_DynamicObject_TryBinaryOperation;

		private static MethodInfo s_DynamicObject_TryUnaryOperation;

		private static MethodInfo s_DynamicObject_TryCreateInstance;

		private static ConstructorInfo s_Nullable_Boolean_Ctor;

		private static ConstructorInfo s_Decimal_Ctor_Int32;

		private static ConstructorInfo s_Decimal_Ctor_UInt32;

		private static ConstructorInfo s_Decimal_Ctor_Int64;

		private static ConstructorInfo s_Decimal_Ctor_UInt64;

		private static ConstructorInfo s_Decimal_Ctor_Int32_Int32_Int32_Bool_Byte;

		private static FieldInfo s_Decimal_One;

		private static FieldInfo s_Decimal_MinusOne;

		private static FieldInfo s_Decimal_MinValue;

		private static FieldInfo s_Decimal_MaxValue;

		private static FieldInfo s_Decimal_Zero;

		private static FieldInfo s_DateTime_MinValue;

		private static MethodInfo s_MethodBase_GetMethodFromHandle_RuntimeMethodHandle;

		private static MethodInfo s_MethodBase_GetMethodFromHandle_RuntimeMethodHandle_RuntimeTypeHandle;

		private static MethodInfo s_MethodInfo_CreateDelegate_Type_Object;

		private static MethodInfo s_String_op_Equality_String_String;

		private static MethodInfo s_String_Equals_String_String;

		private static MethodInfo s_DictionaryOfStringInt32_Add_String_Int32;

		private static ConstructorInfo s_DictionaryOfStringInt32_Ctor_Int32;

		private static MethodInfo s_Type_GetTypeFromHandle;

		private static MethodInfo s_Object_GetType;

		private static MethodInfo s_Decimal_op_Implicit_Byte;

		private static MethodInfo s_Decimal_op_Implicit_SByte;

		private static MethodInfo s_Decimal_op_Implicit_Int16;

		private static MethodInfo s_Decimal_op_Implicit_UInt16;

		private static MethodInfo s_Decimal_op_Implicit_Int32;

		private static MethodInfo s_Decimal_op_Implicit_UInt32;

		private static MethodInfo s_Decimal_op_Implicit_Int64;

		private static MethodInfo s_Decimal_op_Implicit_UInt64;

		private static MethodInfo s_Decimal_op_Implicit_Char;

		private static MethodInfo s_Math_Pow_Double_Double;

		private static ConstructorInfo s_Closure_ObjectArray_ObjectArray;

		private static FieldInfo s_Closure_Constants;

		private static FieldInfo s_Closure_Locals;

		private static MethodInfo s_RuntimeOps_CreateRuntimeVariables_ObjectArray_Int64Array;

		private static MethodInfo s_RuntimeOps_CreateRuntimeVariables;

		private static MethodInfo s_RuntimeOps_MergeRuntimeVariables;

		private static MethodInfo s_RuntimeOps_Quote;
	}
}
