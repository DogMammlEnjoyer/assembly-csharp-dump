using System;
using UnityEngine.LowLevel;

namespace Fusion
{
	internal static class UnityPlayerLoopSystemUtils
	{
		public static bool AddToPlayerLoop(ref PlayerLoopSystem parentSystem, Type referenceSystemType, UnityPlayerLoopSystemAddMode addMode, Type ownerType, PlayerLoopSystem.UpdateFunction updateDelegate)
		{
			ref PlayerLoopSystem[] ptr = ref parentSystem.subSystemList;
			PlayerLoopSystem[] array = ptr;
			int num = (array != null) ? array.Length : 0;
			bool flag = parentSystem.type == referenceSystemType;
			bool result;
			if (flag)
			{
				bool flag2 = addMode == UnityPlayerLoopSystemAddMode.FirstChild;
				if (flag2)
				{
					UnityPlayerLoopSystemUtils.InsertSystem(ref ptr, 0, ownerType, updateDelegate);
				}
				else
				{
					bool flag3 = addMode == UnityPlayerLoopSystemAddMode.LastChild;
					if (!flag3)
					{
						throw new InvalidOperationException(string.Format("Unable to add with a mode {0} once a system has been entered", addMode));
					}
					UnityPlayerLoopSystemUtils.InsertSystem(ref ptr, num, ownerType, updateDelegate);
				}
				result = true;
			}
			else
			{
				for (int i = 0; i < num; i++)
				{
					PlayerLoopSystem playerLoopSystem = ptr[i];
					bool flag4 = playerLoopSystem.type == referenceSystemType;
					if (flag4)
					{
						bool flag5 = addMode == UnityPlayerLoopSystemAddMode.Before;
						if (flag5)
						{
							UnityPlayerLoopSystemUtils.InsertSystem(ref ptr, i, ownerType, updateDelegate);
							return true;
						}
						bool flag6 = addMode == UnityPlayerLoopSystemAddMode.After;
						if (flag6)
						{
							UnityPlayerLoopSystemUtils.InsertSystem(ref ptr, i + 1, ownerType, updateDelegate);
							return true;
						}
					}
					bool flag7 = UnityPlayerLoopSystemUtils.AddToPlayerLoop(ref ptr[i], referenceSystemType, addMode, ownerType, updateDelegate);
					if (flag7)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		public static bool RemoveFromPlayerLoop(ref PlayerLoopSystem parentSystem, Type type)
		{
			ref PlayerLoopSystem[] ptr = ref parentSystem.subSystemList;
			bool flag = ptr == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < ptr.Length; i++)
				{
					PlayerLoopSystem playerLoopSystem = ptr[i];
					bool flag2 = playerLoopSystem.type == type;
					if (flag2)
					{
						for (int j = i + 1; j < ptr.Length; j++)
						{
							ptr[j - 1] = ptr[j];
						}
						Array.Resize<PlayerLoopSystem>(ref ptr, ptr.Length - 1);
						return true;
					}
					bool flag3 = UnityPlayerLoopSystemUtils.RemoveFromPlayerLoop(ref ptr[i], type);
					if (flag3)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		private static void InsertSystem(ref PlayerLoopSystem[] systems, int position, Type ownerType, PlayerLoopSystem.UpdateFunction updateDelegate)
		{
			PlayerLoopSystem[] array = systems;
			int num = (array != null) ? array.Length : 0;
			bool flag = position < 0 || position > num;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("position");
			}
			PlayerLoopSystem playerLoopSystem = new PlayerLoopSystem
			{
				type = ownerType,
				updateDelegate = updateDelegate
			};
			Array.Resize<PlayerLoopSystem>(ref systems, num + 1);
			bool flag2 = position < num;
			if (flag2)
			{
				Array.Copy(systems, position, systems, position + 1, systems.Length - position - 1);
			}
			systems[position] = playerLoopSystem;
		}
	}
}
