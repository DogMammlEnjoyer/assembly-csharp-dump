using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public abstract class MB3_MeshBakerGrouperBehaviour
	{
		public abstract Dictionary<string, List<Renderer>> FilterIntoGroups(List<GameObject> selection, GrouperData d);

		public abstract void DrawGizmos(Bounds sourceObjectBounds, GrouperData d);

		public List<MB3_MeshBakerCommon> DoClustering(MB3_TextureBaker tb, MB3_MeshBakerGrouper grouper, GrouperData d)
		{
			List<MB3_MeshBakerCommon> list = new List<MB3_MeshBakerCommon>();
			if ((grouper.prefabOptions_autoGeneratePrefabs || grouper.prefabOptions_mergeOutputIntoSinglePrefab) && Application.isPlaying)
			{
				Debug.LogError("Cannot generate prefabs while playing. Prefabs can only be generated in the editor and not in play mode.");
				return list;
			}
			Dictionary<string, List<Renderer>> dictionary = this.FilterIntoGroups(tb.GetObjectsToCombine(), d);
			if (d.clusterOnLMIndex)
			{
				Dictionary<string, List<Renderer>> dictionary2 = new Dictionary<string, List<Renderer>>();
				foreach (string text in dictionary.Keys)
				{
					List<Renderer> gaws = dictionary[text];
					Dictionary<int, List<Renderer>> dictionary3 = this.GroupByLightmapIndex(gaws);
					foreach (int key in dictionary3.Keys)
					{
						string key2 = text + "-LM-" + key.ToString();
						dictionary2.Add(key2, dictionary3[key]);
					}
				}
				dictionary = dictionary2;
			}
			if (d.clusterByLODLevel)
			{
				Dictionary<string, List<Renderer>> dictionary4 = new Dictionary<string, List<Renderer>>();
				foreach (string text2 in dictionary.Keys)
				{
					using (List<Renderer>.Enumerator enumerator3 = dictionary[text2].GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							Renderer r = enumerator3.Current;
							if (!(r == null))
							{
								bool flag = false;
								LODGroup componentInParent = r.GetComponentInParent<LODGroup>();
								if (componentInParent != null)
								{
									LOD[] lods = componentInParent.GetLODs();
									Predicate<Renderer> <>9__0;
									for (int i = 0; i < lods.Length; i++)
									{
										Renderer[] renderers = lods[i].renderers;
										Predicate<Renderer> match;
										if ((match = <>9__0) == null)
										{
											match = (<>9__0 = ((Renderer x) => x == r));
										}
										if (Array.Find<Renderer>(renderers, match) != null)
										{
											flag = true;
											string key3 = string.Format("{0}_LOD{1}", text2, i);
											List<Renderer> list2;
											if (!dictionary4.TryGetValue(key3, out list2))
											{
												list2 = new List<Renderer>();
												dictionary4.Add(key3, list2);
											}
											if (!list2.Contains(r))
											{
												list2.Add(r);
											}
										}
									}
								}
								if (!flag)
								{
									string key4 = string.Format("{0}_LOD0", text2);
									List<Renderer> list3;
									if (!dictionary4.TryGetValue(key4, out list3))
									{
										list3 = new List<Renderer>();
										dictionary4.Add(key4, list3);
									}
									if (!list3.Contains(r))
									{
										list3.Add(r);
									}
								}
							}
						}
					}
				}
				dictionary = dictionary4;
			}
			int num = 0;
			foreach (string key5 in dictionary.Keys)
			{
				List<Renderer> list4 = dictionary[key5];
				if (list4.Count > 1 || grouper.data.includeCellsWithOnlyOneRenderer)
				{
					list.Add(this.AddMeshBaker(grouper, tb, key5, list4));
				}
				else
				{
					num++;
				}
			}
			Debug.Log(string.Format("Found {0} cells with Renderers. Not creating bakers for {1} because there is only one mesh in the cell. Creating {2} bakers.", dictionary.Count, num, dictionary.Count - num));
			return list;
		}

		private Dictionary<int, List<Renderer>> GroupByLightmapIndex(List<Renderer> gaws)
		{
			Dictionary<int, List<Renderer>> dictionary = new Dictionary<int, List<Renderer>>();
			for (int i = 0; i < gaws.Count; i++)
			{
				List<Renderer> list;
				if (dictionary.ContainsKey(gaws[i].lightmapIndex))
				{
					list = dictionary[gaws[i].lightmapIndex];
				}
				else
				{
					list = new List<Renderer>();
					dictionary.Add(gaws[i].lightmapIndex, list);
				}
				list.Add(gaws[i]);
			}
			return dictionary;
		}

		private MB3_MeshBakerCommon AddMeshBaker(MB3_MeshBakerGrouper grouper, MB3_TextureBaker tb, string key, List<Renderer> gaws)
		{
			int num = 0;
			for (int i = 0; i < gaws.Count; i++)
			{
				Mesh mesh = MB_Utility.GetMesh(gaws[i].gameObject);
				if (mesh != null)
				{
					num += mesh.vertexCount;
				}
			}
			GameObject gameObject = new GameObject("MeshBaker-" + key);
			gameObject.transform.position = Vector3.zero;
			MB3_MeshBakerCommon mb3_MeshBakerCommon;
			if (num >= 65535)
			{
				mb3_MeshBakerCommon = gameObject.AddComponent<MB3_MultiMeshBaker>();
				mb3_MeshBakerCommon.useObjsToMeshFromTexBaker = false;
			}
			else
			{
				mb3_MeshBakerCommon = gameObject.AddComponent<MB3_MeshBaker>();
				mb3_MeshBakerCommon.useObjsToMeshFromTexBaker = false;
			}
			mb3_MeshBakerCommon.textureBakeResults = tb.textureBakeResults;
			mb3_MeshBakerCommon.transform.parent = tb.transform;
			mb3_MeshBakerCommon.meshCombiner.settingsHolder = grouper;
			for (int j = 0; j < gaws.Count; j++)
			{
				mb3_MeshBakerCommon.GetObjectsToCombine().Add(gaws[j].gameObject);
			}
			return mb3_MeshBakerCommon;
		}

		public virtual MB3_MeshBakerGrouper.ClusterType GetClusterType()
		{
			return MB3_MeshBakerGrouper.ClusterType.none;
		}
	}
}
