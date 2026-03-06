using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.ProBuilder.MeshOperations
{
	public static class MergeElements
	{
		public static List<Face> MergePairs(ProBuilderMesh target, IEnumerable<SimpleTuple<Face, Face>> pairs, bool collapseCoincidentVertices = true)
		{
			HashSet<Face> remove = new HashSet<Face>();
			List<Face> list = new List<Face>();
			foreach (SimpleTuple<Face, Face> simpleTuple in pairs)
			{
				Face item = simpleTuple.item1;
				Face item2 = simpleTuple.item2;
				int num = item.indexesInternal.Length;
				int num2 = item2.indexesInternal.Length;
				int[] array = new int[num + num2];
				Array.Copy(item.indexesInternal, 0, array, 0, num);
				Array.Copy(item2.indexesInternal, 0, array, num, num2);
				list.Add(new Face(array, item.submeshIndex, item.uv, item.smoothingGroup, item.textureGroup, item.elementGroup, item.manualUV));
				remove.Add(item);
				remove.Add(item2);
			}
			List<Face> list2 = (from x in target.facesInternal
			where !remove.Contains(x)
			select x).ToList<Face>();
			list2.AddRange(list);
			target.faces = list2;
			if (collapseCoincidentVertices)
			{
				MergeElements.CollapseCoincidentVertices(target, list);
			}
			return list;
		}

		public static Face Merge(ProBuilderMesh target, IEnumerable<Face> faces)
		{
			int num = (faces != null) ? faces.Count<Face>() : 0;
			if (num < 1)
			{
				return null;
			}
			Face face = faces.First<Face>();
			Face face2 = new Face(faces.SelectMany((Face x) => x.indexesInternal).ToArray<int>(), face.submeshIndex, face.uv, face.smoothingGroup, face.textureGroup, face.elementGroup, face.manualUV);
			Face[] array = new Face[target.facesInternal.Length - num + 1];
			int num2 = 0;
			HashSet<Face> hashSet = new HashSet<Face>(faces);
			foreach (Face face3 in target.facesInternal)
			{
				if (!hashSet.Contains(face3))
				{
					array[num2++] = face3;
				}
			}
			array[num2] = face2;
			target.faces = array;
			MergeElements.CollapseCoincidentVertices(target, new Face[]
			{
				face2
			});
			return face2;
		}

		internal static void CollapseCoincidentVertices(ProBuilderMesh mesh, IEnumerable<Face> faces)
		{
			Dictionary<int, int> sharedVertexLookup = mesh.sharedVertexLookup;
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			foreach (Face face in faces)
			{
				dictionary.Clear();
				for (int i = 0; i < face.indexesInternal.Length; i++)
				{
					int key = sharedVertexLookup[face.indexesInternal[i]];
					if (dictionary.ContainsKey(key))
					{
						face.indexesInternal[i] = dictionary[key];
					}
					else
					{
						dictionary.Add(key, face.indexesInternal[i]);
					}
				}
				face.InvalidateCache();
			}
			MeshValidation.RemoveUnusedVertices(mesh, null);
		}
	}
}
