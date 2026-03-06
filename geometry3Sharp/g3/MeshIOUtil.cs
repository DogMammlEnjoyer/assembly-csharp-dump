using System;
using System.Collections.Generic;

namespace g3
{
	public static class MeshIOUtil
	{
		public static List<GenericMaterial> FindUniqueMaterialList(List<WriteMesh> meshes)
		{
			List<GenericMaterial> list = new List<GenericMaterial>();
			foreach (WriteMesh writeMesh in meshes)
			{
				if (writeMesh.Materials != null)
				{
					foreach (GenericMaterial item in writeMesh.Materials)
					{
						if (!list.Contains(item))
						{
							list.Add(item);
						}
					}
				}
			}
			return list;
		}
	}
}
