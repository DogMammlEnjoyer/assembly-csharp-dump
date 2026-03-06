using System;
using System.Collections.Generic;

namespace g3
{
	public static class FaceGroupUtil
	{
		public static void SetGroupID(DMesh3 mesh, int to)
		{
			if (!mesh.HasTriangleGroups)
			{
				return;
			}
			foreach (int tid in mesh.TriangleIndices())
			{
				mesh.SetTriangleGroup(tid, to);
			}
		}

		public static void SetGroupID(DMesh3 mesh, IEnumerable<int> triangles, int to)
		{
			if (!mesh.HasTriangleGroups)
			{
				return;
			}
			foreach (int tid in triangles)
			{
				mesh.SetTriangleGroup(tid, to);
			}
		}

		public static void SetGroupToGroup(DMesh3 mesh, int from, int to)
		{
			if (!mesh.HasTriangleGroups)
			{
				return;
			}
			int maxTriangleID = mesh.MaxTriangleID;
			for (int i = 0; i < maxTriangleID; i++)
			{
				if (mesh.IsTriangle(i) && mesh.GetTriangleGroup(i) == from)
				{
					mesh.SetTriangleGroup(i, to);
				}
			}
		}

		public static HashSet<int> FindAllGroups(DMesh3 mesh)
		{
			HashSet<int> hashSet = new HashSet<int>();
			if (mesh.HasTriangleGroups)
			{
				int maxTriangleID = mesh.MaxTriangleID;
				for (int i = 0; i < maxTriangleID; i++)
				{
					if (mesh.IsTriangle(i))
					{
						int triangleGroup = mesh.GetTriangleGroup(i);
						hashSet.Add(triangleGroup);
					}
				}
			}
			return hashSet;
		}

		public static SparseList<int> CountAllGroups(DMesh3 mesh)
		{
			SparseList<int> sparseList = new SparseList<int>(mesh.MaxGroupID, 0, 0);
			if (mesh.HasTriangleGroups)
			{
				int maxTriangleID = mesh.MaxTriangleID;
				for (int i = 0; i < maxTriangleID; i++)
				{
					if (mesh.IsTriangle(i))
					{
						int triangleGroup = mesh.GetTriangleGroup(i);
						sparseList[triangleGroup]++;
					}
				}
			}
			return sparseList;
		}

		public static int[][] FindTriangleSetsByGroup(DMesh3 mesh, int ignoreGID = -2147483648)
		{
			if (!mesh.HasTriangleGroups)
			{
				return new int[0][];
			}
			SparseList<int> sparseList = FaceGroupUtil.CountAllGroups(mesh);
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, int> keyValuePair in sparseList.Values())
			{
				if (keyValuePair.Key != ignoreGID && keyValuePair.Value > 0)
				{
					list.Add(keyValuePair.Key);
				}
			}
			list.Sort();
			SparseList<int> sparseList2 = new SparseList<int>(mesh.MaxGroupID, list.Count, -1);
			int[][] array = new int[list.Count][];
			int[] array2 = new int[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				int idx = list[i];
				array[i] = new int[sparseList[idx]];
				array2[i] = 0;
				sparseList2[idx] = i;
			}
			int maxTriangleID = mesh.MaxTriangleID;
			for (int j = 0; j < maxTriangleID; j++)
			{
				if (mesh.IsTriangle(j))
				{
					int triangleGroup = mesh.GetTriangleGroup(j);
					int num = sparseList2[triangleGroup];
					if (num >= 0)
					{
						int[] array3 = array2;
						int num2 = num;
						int num3 = array3[num2];
						array3[num2] = num3 + 1;
						int num4 = num3;
						array[num][num4] = j;
					}
				}
			}
			return array;
		}

		public static List<int> FindTrianglesByGroup(IMesh mesh, int findGroupID)
		{
			List<int> list = new List<int>();
			if (!mesh.HasTriangleGroups)
			{
				return list;
			}
			foreach (int num in mesh.TriangleIndices())
			{
				if (mesh.GetTriangleGroup(num) == findGroupID)
				{
					list.Add(num);
				}
			}
			return list;
		}

		public static DMesh3[] SeparateMeshByGroups(DMesh3 mesh, out int[] groupIDs)
		{
			Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
			foreach (int num in mesh.TriangleIndices())
			{
				int triangleGroup = mesh.GetTriangleGroup(num);
				List<int> list;
				if (!dictionary.TryGetValue(triangleGroup, out list))
				{
					list = new List<int>();
					dictionary[triangleGroup] = list;
				}
				list.Add(num);
			}
			DMesh3[] array = new DMesh3[dictionary.Count];
			groupIDs = new int[dictionary.Count];
			int num2 = 0;
			foreach (KeyValuePair<int, List<int>> keyValuePair in dictionary)
			{
				groupIDs[num2] = keyValuePair.Key;
				List<int> value = keyValuePair.Value;
				array[num2++] = DSubmesh3.QuickSubmesh(mesh, value);
			}
			return array;
		}

		public static DMesh3[] SeparateMeshByGroups(DMesh3 mesh)
		{
			int[] array;
			return FaceGroupUtil.SeparateMeshByGroups(mesh, out array);
		}
	}
}
