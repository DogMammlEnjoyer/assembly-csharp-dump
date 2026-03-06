using System;
using System.Collections.Generic;
using Unity.Collections;

namespace UnityEngine.Rendering
{
	internal static class InstanceTypeInfo
	{
		static InstanceTypeInfo()
		{
			InstanceTypeInfo.InitParentTypes();
			InstanceTypeInfo.InitChildTypes();
			InstanceTypeInfo.ValidateTypeRelationsAreCorrectlySorted();
		}

		private static void InitParentTypes()
		{
			InstanceTypeInfo.s_ParentTypes = new InstanceType[2];
			InstanceTypeInfo.s_ParentTypes[0] = InstanceType.MeshRenderer;
			InstanceTypeInfo.s_ParentTypes[1] = InstanceType.MeshRenderer;
		}

		private static void InitChildTypes()
		{
			InstanceTypeInfo.s_ChildTypes = new List<InstanceType>[2];
			for (int i = 0; i < 2; i++)
			{
				InstanceTypeInfo.s_ChildTypes[i] = new List<InstanceType>();
			}
			for (int j = 0; j < 2; j++)
			{
				InstanceType instanceType = (InstanceType)j;
				InstanceType instanceType2 = InstanceTypeInfo.s_ParentTypes[(int)instanceType];
				if (instanceType != instanceType2)
				{
					InstanceTypeInfo.s_ChildTypes[(int)instanceType2].Add(instanceType);
				}
			}
		}

		private static InstanceType GetMaxChildTypeRecursively(InstanceType type)
		{
			InstanceType instanceType = type;
			foreach (InstanceType type2 in InstanceTypeInfo.s_ChildTypes[(int)type])
			{
				instanceType = (InstanceType)Mathf.Max((int)instanceType, (int)InstanceTypeInfo.GetMaxChildTypeRecursively(type2));
			}
			return instanceType;
		}

		private static void FlattenChildInstanceTypes(InstanceType instanceType, NativeList<InstanceType> instanceTypes)
		{
			instanceTypes.Add(instanceType);
			foreach (InstanceType instanceType2 in InstanceTypeInfo.s_ChildTypes[(int)instanceType])
			{
				InstanceTypeInfo.FlattenChildInstanceTypes(instanceType2, instanceTypes);
			}
		}

		private static void ValidateTypeRelationsAreCorrectlySorted()
		{
			NativeList<InstanceType> instanceTypes = new NativeList<InstanceType>(2, Allocator.Temp);
			for (int i = 0; i < 2; i++)
			{
				InstanceType instanceType = (InstanceType)i;
				if (instanceType == InstanceTypeInfo.s_ParentTypes[i])
				{
					InstanceTypeInfo.FlattenChildInstanceTypes(instanceType, instanceTypes);
				}
			}
			for (int j = 0; j < instanceTypes.Length; j++)
			{
			}
		}

		public static InstanceType GetParentType(InstanceType type)
		{
			return InstanceTypeInfo.s_ParentTypes[(int)type];
		}

		public static List<InstanceType> GetChildTypes(InstanceType type)
		{
			return InstanceTypeInfo.s_ChildTypes[(int)type];
		}

		public const int kInstanceTypeBitCount = 1;

		public const int kMaxInstanceTypesCount = 2;

		public const uint kInstanceTypeMask = 1U;

		private static InstanceType[] s_ParentTypes;

		private static List<InstanceType>[] s_ChildTypes;
	}
}
