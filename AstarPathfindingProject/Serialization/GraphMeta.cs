using System;
using System.Collections.Generic;

namespace Pathfinding.Serialization
{
	public class GraphMeta
	{
		public Type GetGraphType(int index, Type[] availableGraphTypes)
		{
			if (string.IsNullOrEmpty(this.typeNames[index]))
			{
				return null;
			}
			for (int i = 0; i < availableGraphTypes.Length; i++)
			{
				if (availableGraphTypes[i].FullName == this.typeNames[index])
				{
					return availableGraphTypes[i];
				}
			}
			throw new Exception("No graph of type '" + this.typeNames[index] + "' could be created, type does not exist");
		}

		public Version version;

		public int graphs;

		public List<string> guids;

		public List<string> typeNames;
	}
}
