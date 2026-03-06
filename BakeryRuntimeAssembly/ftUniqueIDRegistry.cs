using System;
using System.Collections.Generic;

public static class ftUniqueIDRegistry
{
	public static void Deregister(int id)
	{
		int instanceId = ftUniqueIDRegistry.GetInstanceId(id);
		if (instanceId < 0)
		{
			return;
		}
		ftUniqueIDRegistry.MappingInv.Remove(instanceId);
		ftUniqueIDRegistry.Mapping.Remove(id);
	}

	public static void Register(int id, int value)
	{
		if (!ftUniqueIDRegistry.Mapping.ContainsKey(id))
		{
			ftUniqueIDRegistry.Mapping[id] = value;
			ftUniqueIDRegistry.MappingInv[value] = id;
		}
	}

	public static int GetInstanceId(int id)
	{
		int result;
		if (!ftUniqueIDRegistry.Mapping.TryGetValue(id, out result))
		{
			return -1;
		}
		return result;
	}

	public static int GetUID(int instanceId)
	{
		int result;
		if (ftUniqueIDRegistry.MappingInv.TryGetValue(instanceId, out result))
		{
			return result;
		}
		return -1;
	}

	public static Dictionary<int, int> Mapping = new Dictionary<int, int>();

	public static Dictionary<int, int> MappingInv = new Dictionary<int, int>();
}
