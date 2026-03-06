using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace DigitalOpus.MB.Core
{
	[Serializable]
	public class MB3_MeshCombinerSingle : MB3_MeshCombiner
	{
		public void StartProfile()
		{
			this.db_showHideGameObjects.Reset();
			this.db_addDeleteGameObjects.Reset();
			this.db_apply.Reset();
			this.db_applyShowHide.Reset();
			this.db_updateGameObjects.Reset();
		}

		public void PrintProfileInfo()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Timings  " + ((base.settings.meshAPI == MB_MeshCombineAPIType.betaNativeArrayAPI) ? "  newMeshAPI " : " oldMeshAPI"));
			stringBuilder.AppendLine("db_showHideGameObjects " + this.db_showHideGameObjects.Elapsed.Seconds.ToString());
			stringBuilder.AppendLine("db_addDeleteGameObjects " + this.db_addDeleteGameObjects.Elapsed.Seconds.ToString());
			stringBuilder.AppendLine("db_apply " + this.db_apply.Elapsed.Seconds.ToString());
			stringBuilder.AppendLine("db_applyShowHide " + this.db_applyShowHide.Elapsed.Seconds.ToString());
			stringBuilder.AppendLine("db_updateGameObjects " + this.db_updateGameObjects.Elapsed.Seconds.ToString());
			Debug.Log(stringBuilder.ToString());
		}

		protected override void Dispose(bool disposing)
		{
			if (base.IsDisposed())
			{
				return;
			}
			base.Dispose(disposing);
			if (this._boneProcessor != null)
			{
				this._boneProcessor.DisposeOfTemporarySMRData();
				this._boneProcessor.Dispose();
				this._boneProcessor = null;
			}
			if (this._blendShapeProcessor != null)
			{
				this._blendShapeProcessor.Dispose();
				this._blendShapeProcessor = null;
			}
			if (this._meshChannelsCache != null)
			{
				this._meshChannelsCache.Dispose();
				this._meshChannelsCache = null;
			}
			if (this._vertexAndTriProcessor != null)
			{
				this._vertexAndTriProcessor.Dispose();
			}
		}

		public override MB2_TextureBakeResults textureBakeResults
		{
			set
			{
				if (this.GetVertexCount() > 0 && this._textureBakeResults != value && this._textureBakeResults != null && this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("If Texture Bake Result is changed then objects currently in combined mesh may be invalid.");
				}
				this._textureBakeResults = value;
			}
		}

		public override MB_RenderType renderType
		{
			set
			{
				if (value == MB_RenderType.skinnedMeshRenderer && this._renderType == MB_RenderType.meshRenderer && this.GetVertexCount() > 0 && (this.bones == null || this.bones.Length == 0))
				{
					Debug.LogError("Can't set the render type to SkinnedMeshRenderer without clearing the mesh first. Try deleting the CombinedMesh scene object.");
				}
				this._renderType = value;
			}
		}

		public override GameObject resultSceneObject
		{
			set
			{
				if (this._resultSceneObject != value && this._resultSceneObject != null)
				{
					this._targetRenderer = null;
					if (this._mesh != null && this.LOG_LEVEL >= MB2_LogLevel.warn)
					{
						Debug.LogWarning("Result Scene Object was changed when this mesh baker component had a reference to a mesh. If mesh is being used by another object make sure to reset the mesh to none before baking to avoid overwriting the other mesh.");
					}
				}
				this._resultSceneObject = value;
			}
		}

		public int GetVertexCount()
		{
			return this.verts.Length;
		}

		private MB3_MeshCombinerSingle.MB_DynamicGameObject instance2Combined_MapGet(GameObject gameObjectID)
		{
			return this._instance2combined_map[gameObjectID];
		}

		private void instance2Combined_MapAdd(GameObject gameObjectID, MB3_MeshCombinerSingle.MB_DynamicGameObject dgo)
		{
			this._instance2combined_map.Add(gameObjectID, dgo);
		}

		private void instance2Combined_MapRemove(GameObject gameObjectID)
		{
			this._instance2combined_map.Remove(gameObjectID);
		}

		private bool instance2Combined_MapTryGetValue(GameObject gameObjectID, out MB3_MeshCombinerSingle.MB_DynamicGameObject dgo)
		{
			return this._instance2combined_map.TryGetValue(gameObjectID, out dgo);
		}

		private int instance2Combined_MapCount()
		{
			return this._instance2combined_map.Count;
		}

		private void instance2Combined_MapClear()
		{
			this._instance2combined_map.Clear();
		}

		private bool instance2Combined_MapContainsKey(GameObject gameObjectID)
		{
			return this._instance2combined_map.ContainsKey(gameObjectID);
		}

		public bool InstanceID2DGO(int instanceID, out MB3_MeshCombinerSingle.MB_DynamicGameObject dgoGameObject)
		{
			for (int i = 0; i < this.mbDynamicObjectsInCombinedMesh.Count; i++)
			{
				if (this.mbDynamicObjectsInCombinedMesh[i].gameObject != null)
				{
					if (this.mbDynamicObjectsInCombinedMesh[i].gameObject.GetInstanceID() == instanceID)
					{
						dgoGameObject = this.mbDynamicObjectsInCombinedMesh[i];
						return true;
					}
				}
				else if (this.mbDynamicObjectsInCombinedMesh[i].instanceID == instanceID)
				{
					dgoGameObject = this.mbDynamicObjectsInCombinedMesh[i];
					return true;
				}
			}
			Debug.LogError("Could not find a cached game object matching InstanceID: " + instanceID.ToString());
			dgoGameObject = null;
			return false;
		}

		public override int GetNumObjectsInCombined()
		{
			return this.mbDynamicObjectsInCombinedMesh.Count;
		}

		public override List<GameObject> GetObjectsInCombined()
		{
			List<GameObject> list = new List<GameObject>();
			list.AddRange(this.objectsInCombinedMesh);
			return list;
		}

		public Mesh GetMesh()
		{
			if (this._mesh == null)
			{
				this._mesh = this._NewMesh();
			}
			return this._mesh;
		}

		public MB3_MeshCombinerSingle.MeshCreationConditions SetMesh(Mesh m)
		{
			if (m == null)
			{
				this._meshBirth = MB3_MeshCombinerSingle.MeshCreationConditions.NoMesh;
			}
			else
			{
				this._meshBirth = MB3_MeshCombinerSingle.MeshCreationConditions.AssignedByUser;
			}
			this._mesh = m;
			return this._meshBirth;
		}

		public Transform[] GetBones()
		{
			return this.bones;
		}

		public override int GetLightmapIndex()
		{
			if (base.settings.lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout || base.settings.lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping)
			{
				return this.lightmapIndex;
			}
			return -1;
		}

		private bool _Initialize(int numResultMats)
		{
			if (this.mbDynamicObjectsInCombinedMesh.Count == 0)
			{
				this.lightmapIndex = -1;
			}
			if (this._mesh == null)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug("_initialize Creating new Mesh", Array.Empty<object>());
				}
				this._mesh = this.GetMesh();
			}
			if (this.instance2Combined_MapCount() != this.mbDynamicObjectsInCombinedMesh.Count)
			{
				this.instance2Combined_MapClear();
				for (int i = 0; i < this.mbDynamicObjectsInCombinedMesh.Count; i++)
				{
					if (this.mbDynamicObjectsInCombinedMesh[i] != null)
					{
						if (this.mbDynamicObjectsInCombinedMesh[i].gameObject == null)
						{
							Debug.LogError("This MeshBaker contains information from a previous bake that is incomlete. It may have been baked by a previous version of Mesh Baker. If you are trying to update/modify a previously baked combined mesh. Try doing the original bake.");
							return false;
						}
						this.instance2Combined_MapAdd(this.mbDynamicObjectsInCombinedMesh[i].gameObject, this.mbDynamicObjectsInCombinedMesh[i]);
					}
				}
			}
			if (this.LOG_LEVEL >= MB2_LogLevel.trace)
			{
				Debug.Log(string.Format("_initialize numObjsInCombined={0}", this.mbDynamicObjectsInCombinedMesh.Count));
			}
			return true;
		}

		private bool _collectMaterialTriangles(Mesh m, MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, Material[] sharedMaterials, OrderedDictionary sourceMats2submeshIdx_map)
		{
			int num = m.subMeshCount;
			if (sharedMaterials.Length < num)
			{
				num = sharedMaterials.Length;
			}
			dgo._tmpSubmeshTris = new MB3_MeshCombinerSingle.SerializableIntArray[num];
			dgo.targetSubmeshIdxs = new int[num];
			for (int i = 0; i < num; i++)
			{
				if (this._textureBakeResults.doMultiMaterial || this._textureBakeResults.resultType == MB2_TextureBakeResults.ResultType.textureArray)
				{
					if (!sourceMats2submeshIdx_map.Contains(sharedMaterials[i]))
					{
						string str = "Object ";
						string name = dgo.name;
						string str2 = " has a material that was not found in the result materials maping. ";
						Material material = sharedMaterials[i];
						Debug.LogError(str + name + str2 + ((material != null) ? material.ToString() : null));
						return false;
					}
					dgo.targetSubmeshIdxs[i] = (int)sourceMats2submeshIdx_map[sharedMaterials[i]];
				}
				else
				{
					dgo.targetSubmeshIdxs[i] = 0;
				}
				dgo._tmpSubmeshTris[i] = new MB3_MeshCombinerSingle.SerializableIntArray();
				dgo._tmpSubmeshTris[i].data = m.GetTriangles(i);
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug(string.Concat(new string[]
					{
						"Collecting triangles for: ",
						dgo.name,
						" submesh:",
						i.ToString(),
						" maps to submesh:",
						dgo.targetSubmeshIdxs[i].ToString(),
						" added:",
						dgo._tmpSubmeshTris[i].data.Length.ToString()
					}), new object[]
					{
						this.LOG_LEVEL
					});
				}
			}
			return true;
		}

		private bool _collectOutOfBoundsUVRects2(Mesh m, MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, Material[] sharedMaterials, OrderedDictionary sourceMats2submeshIdx_map, Dictionary<int, MB_Utility.MeshAnalysisResult[]> meshAnalysisResults)
		{
			if (this._textureBakeResults == null)
			{
				Debug.LogError("Need to bake textures into combined material");
				return false;
			}
			MB_Utility.MeshAnalysisResult[] array;
			if (!meshAnalysisResults.TryGetValue(m.GetInstanceID(), out array))
			{
				int subMeshCount = m.subMeshCount;
				array = new MB_Utility.MeshAnalysisResult[subMeshCount];
				for (int i = 0; i < subMeshCount; i++)
				{
					this._meshChannelsCache.hasOutOfBoundsUVs(m, ref array[i], i);
				}
				meshAnalysisResults.Add(m.GetInstanceID(), array);
			}
			int num = sharedMaterials.Length;
			if (num > m.subMeshCount)
			{
				num = m.subMeshCount;
			}
			dgo.obUVRects = new Rect[num];
			for (int j = 0; j < num; j++)
			{
				int idxInSrcMats = dgo.targetSubmeshIdxs[j];
				if (this._textureBakeResults.GetConsiderMeshUVs(idxInSrcMats, sharedMaterials[j]))
				{
					dgo.obUVRects[j] = array[j].uvRect;
				}
			}
			return true;
		}

		private bool _validateTextureBakeResults()
		{
			if (this._textureBakeResults == null)
			{
				Debug.LogError("Texture Bake Results is null. Can't combine meshes.");
				return false;
			}
			if (this._textureBakeResults.materialsAndUVRects == null || this._textureBakeResults.materialsAndUVRects.Length == 0)
			{
				Debug.LogError("Texture Bake Results has no materials in material to sourceUVRect map. Try baking materials. Can't combine meshes. If you are trying to combine meshes without combining materials, try removing the Texture Bake Result.");
				return false;
			}
			if (this._textureBakeResults.NumResultMaterials() == 0)
			{
				Debug.LogError("Texture Bake Results has no result materials. Try baking materials. Can't combine meshes.");
				return false;
			}
			return true;
		}

		internal bool _ShowHide(GameObject[] goToShow, GameObject[] goToHide)
		{
			if (goToShow == null)
			{
				goToShow = this.empty;
			}
			if (goToHide == null)
			{
				goToHide = this.empty;
			}
			int numResultMats = this._textureBakeResults.NumResultMaterials();
			if (!this._Initialize(numResultMats))
			{
				return false;
			}
			for (int i = 0; i < goToHide.Length; i++)
			{
				if (!this.instance2Combined_MapContainsKey(goToHide[i]))
				{
					if (this.LOG_LEVEL >= MB2_LogLevel.warn)
					{
						string str = "Trying to hide an object ";
						GameObject gameObject = goToHide[i];
						Debug.LogWarning(str + ((gameObject != null) ? gameObject.ToString() : null) + " that is not in combined mesh. Did you initially bake with 'clear buffers after bake' enabled?");
					}
					return false;
				}
			}
			for (int j = 0; j < goToShow.Length; j++)
			{
				if (!this.instance2Combined_MapContainsKey(goToShow[j]))
				{
					if (this.LOG_LEVEL >= MB2_LogLevel.warn)
					{
						string str2 = "Trying to show an object ";
						GameObject gameObject2 = goToShow[j];
						Debug.LogWarning(str2 + ((gameObject2 != null) ? gameObject2.ToString() : null) + " that is not in combined mesh. Did you initially bake with 'clear buffers after bake' enabled?");
					}
					return false;
				}
			}
			for (int k = 0; k < goToHide.Length; k++)
			{
				this._instance2combined_map[goToHide[k]].show = false;
			}
			for (int l = 0; l < goToShow.Length; l++)
			{
				this._instance2combined_map[goToShow[l]].show = true;
			}
			if (this._vertexAndTriProcessor != null && !this._vertexAndTriProcessor.IsDisposed())
			{
				this._vertexAndTriProcessor.Dispose();
			}
			bool flag = this._UseNativeArrayAPIorNot();
			this._vertexAndTriProcessor = MB3_MeshCombinerSingle.Create_VertexAndTriangleProcessor(flag);
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				if (flag)
				{
					Debug.Log("using NativeArray mesh API");
				}
				else
				{
					Debug.Log("using simple mesh API");
				}
			}
			bool flag2 = false;
			try
			{
				flag2 = MB3_MeshCombinerSingle.MB_MeshCombinerSingle_SubCombiner._ShowHideGameObjects(this);
				if (flag2)
				{
					this._bakeStatus = MB3_MeshCombiner.MeshCombiningStatus.readyForApply;
				}
			}
			catch
			{
				flag2 = false;
				throw;
			}
			return flag2;
		}

		internal bool _AddToCombined(GameObject[] goToAdd, int[] goToDelete, bool disableRendererInSource)
		{
			Stopwatch stopwatch = null;
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				stopwatch = new Stopwatch();
				stopwatch.Start();
			}
			if (!this._validateTextureBakeResults())
			{
				return false;
			}
			if (!this.ValidateTargRendererAndMeshAndResultSceneObj())
			{
				return false;
			}
			if (this.outputOption != MB2_OutputOptions.bakeMeshAssetsInPlace && base.settings.renderType == MB_RenderType.skinnedMeshRenderer && (this._targetRenderer == null || !(this._targetRenderer is SkinnedMeshRenderer)))
			{
				Debug.LogError("Target renderer must be set and must be a SkinnedMeshRenderer");
				return false;
			}
			if (base.settings.doBlendShapes && base.settings.renderType != MB_RenderType.skinnedMeshRenderer)
			{
				Debug.LogError("If doBlendShapes is set then RenderType must be skinnedMeshRenderer.");
				return false;
			}
			GameObject[] array;
			if (goToAdd == null)
			{
				array = this.empty;
			}
			else
			{
				array = (GameObject[])goToAdd.Clone();
			}
			int[] array2;
			if (goToDelete == null)
			{
				array2 = this.emptyIDs;
			}
			else
			{
				array2 = (int[])goToDelete.Clone();
			}
			if (this._mesh == null)
			{
				this.DestroyMesh();
			}
			int numResultMats = this._textureBakeResults.NumResultMaterials();
			if (!this._Initialize(numResultMats))
			{
				return false;
			}
			if (this._mesh.vertexCount > 0 && this._instance2combined_map.Count == 0)
			{
				Debug.LogWarning("There were vertices in the combined mesh but nothing in the MeshBaker buffers. If you are trying to bake in the editor and modify at runtime, make sure 'Clear Buffers After Bake' is unchecked.");
			}
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(string.Concat(new string[]
				{
					"==== Calling _addToCombined objs adding:",
					array.Length.ToString(),
					" objs deleting:",
					array2.Length.ToString(),
					" fixOutOfBounds:",
					this.textureBakeResults.DoAnyResultMatsUseConsiderMeshUVs().ToString(),
					" doMultiMaterial:",
					this.textureBakeResults.doMultiMaterial.ToString(),
					" disableRenderersInSource:",
					disableRendererInSource.ToString()
				}));
				if (this.LOG_LEVEL >= MB2_LogLevel.trace)
				{
					for (int i = 0; i < array.Length; i++)
					{
						StringBuilder stringBuilder2 = stringBuilder;
						string str = "    adding obj[";
						string str2 = i.ToString();
						string str3 = "]=";
						GameObject gameObject = array[i];
						stringBuilder2.AppendLine(str + str2 + str3 + ((gameObject != null) ? gameObject.ToString() : null));
					}
					HashSet<int> hashSet = new HashSet<int>(array2);
					for (int j = 0; j < this.objectsInCombinedMesh.Count; j++)
					{
						if (!hashSet.Contains(this.objectsInCombinedMesh[j].gameObject.GetInstanceID()))
						{
							StringBuilder stringBuilder3 = stringBuilder;
							string str4 = "    keeping in combined:";
							GameObject gameObject2 = this.objectsInCombinedMesh[j];
							stringBuilder3.AppendLine(str4 + ((gameObject2 != null) ? gameObject2.ToString() : null));
						}
						else
						{
							StringBuilder stringBuilder4 = stringBuilder;
							string str5 = "    deleting in combined:";
							GameObject gameObject3 = this.objectsInCombinedMesh[j];
							stringBuilder4.AppendLine(str5 + ((gameObject3 != null) ? gameObject3.ToString() : null));
						}
					}
				}
				Debug.Log(stringBuilder);
			}
			if (this._textureBakeResults.NumResultMaterials() == 0)
			{
				Debug.LogError("No resultMaterials in this TextureBakeResults. Try baking textures.");
				return false;
			}
			if (!base.settings.clearBuffersAfterBake && this.mbDynamicObjectsInCombinedMesh.Count > 0)
			{
				if (this._mesh == null)
				{
					Debug.LogError("Trying to add and delete to a combined mesh that was previously baked but the mesh is null.");
					return false;
				}
				if (this._mesh.vertexCount != this.bufferDataFromPrevious.numVertsBaked)
				{
					Debug.LogError("Trying to add and delete to a combined mesh that was previously baked but the mesh vertex count is different. " + this._mesh.vertexCount.ToString() + " != " + this.bufferDataFromPrevious.numVertsBaked.ToString());
					return false;
				}
			}
			OrderedDictionary orderedDictionary = this.BuildSourceMatsToSubmeshIdxMap(numResultMats);
			if (orderedDictionary == null)
			{
				return false;
			}
			bool uvsSliceIdx_w = base.settings.doUV && this.textureBakeResults.resultType == MB2_TextureBakeResults.ResultType.textureArray;
			MB_MeshVertexChannelFlags meshChannelsAsFlags = MeshBakerSettingsUtility.GetMeshChannelsAsFlags(base.settings, true, uvsSliceIdx_w);
			if (!base.settings.clearBuffersAfterBake && this.channelsLastBake != MB_MeshVertexChannelFlags.none && this.mbDynamicObjectsInCombinedMesh.Count > 0 && this.channelsLastBake != meshChannelsAsFlags)
			{
				Debug.LogError("There is data in the combined mesh and channels have changed since previous bake. Can't bake:\n channelsLastBake:" + this.channelsLastBake.ToString() + "\n channels current bake: " + meshChannelsAsFlags.ToString());
				return false;
			}
			if (this._vertexAndTriProcessor != null && !this._vertexAndTriProcessor.IsDisposed())
			{
				this._vertexAndTriProcessor.Dispose();
			}
			bool flag = this._UseNativeArrayAPIorNot();
			this._meshChannelsCache = MB3_MeshCombinerSingle.Create_MeshChannelsCache(flag, this.LOG_LEVEL, base.settings.lightmapOption);
			this._vertexAndTriProcessor = MB3_MeshCombinerSingle.Create_VertexAndTriangleProcessor(flag);
			MB3_MeshCombinerSingle.IVertexAndTriangleProcessor vertexAndTriangleProcessor = MB3_MeshCombinerSingle.Create_VertexAndTriangleProcessor(flag);
			this._blendShapeProcessor = new MB3_MeshCombinerSingle.MB_MeshCombinerSingle_BlendShapeProcessor(this);
			this._boneProcessor = this.Create_BoneProcessor(flag);
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				if (flag)
				{
					Debug.Log("using NativeArray mesh API");
				}
				else
				{
					Debug.Log("using simple mesh API");
				}
			}
			bool result = false;
			try
			{
				result = this.__AddToCombined(array, array2, disableRendererInSource, numResultMats, orderedDictionary, ref vertexAndTriangleProcessor, meshChannelsAsFlags, stopwatch);
			}
			catch
			{
				result = false;
				throw;
			}
			finally
			{
				this._meshChannelsCache.Dispose();
				this._boneProcessor.DisposeOfTemporarySMRData();
				vertexAndTriangleProcessor.Dispose();
				for (int k = 0; k < this.mbDynamicObjectsInCombinedMesh.Count; k++)
				{
					this.mbDynamicObjectsInCombinedMesh[k].UnInitialize();
				}
			}
			return result;
		}

		internal bool __AddToCombined(GameObject[] _goToAdd, int[] _goToDelete, bool disableRendererInSource, int numResultMats, OrderedDictionary sourceMats2submeshIdx_map, ref MB3_MeshCombinerSingle.IVertexAndTriangleProcessor oldMeshData, MB_MeshVertexChannelFlags newChannels, Stopwatch sw)
		{
			if (this.textureBakeResults.resultType == MB2_TextureBakeResults.ResultType.textureArray && base.settings.assignToMeshCustomizer == null)
			{
				Debug.LogError("Baking combined mesh failed because textures were baked into TextureArrays and no AssignToMeshCustomizer was assigned in the Mesh Baker Settings.");
				return false;
			}
			MB3_MeshCombinerSingle.UVAdjuster_Atlas uvadjuster_Atlas = new MB3_MeshCombinerSingle.UVAdjuster_Atlas(this.textureBakeResults, this.LOG_LEVEL);
			List<MB3_MeshCombinerSingle.MB_DynamicGameObject> list = new List<MB3_MeshCombinerSingle.MB_DynamicGameObject>();
			int i = 0;
			Predicate<int> <>9__0;
			while (i < _goToAdd.Length)
			{
				if (!this.instance2Combined_MapContainsKey(_goToAdd[i]))
				{
					goto IL_9A;
				}
				Predicate<int> match;
				if ((match = <>9__0) == null)
				{
					match = (<>9__0 = ((int o) => o == _goToAdd[i].GetInstanceID()));
				}
				if (Array.FindIndex<int>(_goToDelete, match) != -1)
				{
					goto IL_9A;
				}
				if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Object " + _goToAdd[i].name + " has already been added. This MeshBaker may have been baked previously with 'Clear Buffers After Bake' unchecked. You can clear the buffers by checking 'Clear Buffers After Bake' and baking. If you want to update a combined mesh by baking several times, you should uncheck 'Clear Buffers After Bake'.");
				}
				_goToAdd[i] = null;
				IL_2CF:
				int i2 = i;
				i = i2 + 1;
				continue;
				IL_9A:
				MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject = new MB3_MeshCombinerSingle.MB_DynamicGameObject();
				mb_DynamicGameObject.InitializeNew(false, _goToAdd[i]);
				if (mb_DynamicGameObject._renderer == null)
				{
					Debug.LogError("Object " + mb_DynamicGameObject.gameObject.name + " does not have a Renderer");
					_goToAdd[i] = null;
					return false;
				}
				Material[] sharedMaterials = mb_DynamicGameObject._renderer.sharedMaterials;
				if (this.LOG_LEVEL >= MB2_LogLevel.trace)
				{
					Debug.Log(string.Format("Getting {0} shared materials for {1}", sharedMaterials.Length, mb_DynamicGameObject.gameObject));
				}
				if (sharedMaterials == null)
				{
					Debug.LogError("Object " + mb_DynamicGameObject.name + " does not have a Renderer");
					_goToAdd[i] = null;
					return false;
				}
				Mesh mesh = mb_DynamicGameObject._mesh;
				if (mesh == null)
				{
					Debug.LogError("Object " + mb_DynamicGameObject.gameObject.name + " MeshFilter or SkinnedMeshRenderer had no mesh");
					_goToAdd[i] = null;
					return false;
				}
				if (MBVersion.IsRunningAndMeshNotReadWriteable(mesh))
				{
					Debug.LogError("Object " + mb_DynamicGameObject.gameObject.name + " Mesh Importer has read/write flag set to 'false'. This needs to be set to 'true' in order to read data from this mesh.");
					_goToAdd[i] = null;
					return false;
				}
				if (sharedMaterials.Length > mesh.subMeshCount)
				{
					Array.Resize<Material>(ref sharedMaterials, mesh.subMeshCount);
				}
				if (_goToAdd[i] != null)
				{
					list.Add(mb_DynamicGameObject);
					mb_DynamicGameObject.name = string.Format("{0} {1}", _goToAdd[i].ToString(), _goToAdd[i].GetInstanceID());
					mb_DynamicGameObject.instanceID = _goToAdd[i].GetInstanceID();
					mb_DynamicGameObject.gameObject = _goToAdd[i];
					mb_DynamicGameObject.numVerts = mesh.vertexCount;
					mb_DynamicGameObject.sourceSharedMaterials = sharedMaterials;
					goto IL_2CF;
				}
				goto IL_2CF;
			}
			for (int j = 0; j < this.mbDynamicObjectsInCombinedMesh.Count; j++)
			{
				if (!this.mbDynamicObjectsInCombinedMesh[j]._beingDeleted)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject2 = this.mbDynamicObjectsInCombinedMesh[j];
					if (!mb_DynamicGameObject2.Initialize(false))
					{
						Debug.LogError("Object " + mb_DynamicGameObject2.gameObject.name + " does not have a Renderer");
						return false;
					}
				}
			}
			this.db_addDeleteGameObjects_CollectMeshData.Start();
			this.db_addDeleteGameObjects_CollectMeshData_a.Start();
			this._meshChannelsCache.CollectChannelDataForAllMeshesInList(this.mbDynamicObjectsInCombinedMesh, list, newChannels, base.settings.renderType, base.settings.doBlendShapes);
			this.db_addDeleteGameObjects_CollectMeshData_a.Stop();
			int num = 0;
			int[] array = new int[numResultMats];
			int num2 = 0;
			this._boneProcessor.BuildBoneIdx2DGOMapIfNecessary(_goToDelete);
			for (int k = 0; k < _goToDelete.Length; k++)
			{
				MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject3 = null;
				this.InstanceID2DGO(_goToDelete[k], out mb_DynamicGameObject3);
				if (mb_DynamicGameObject3 != null)
				{
					mb_DynamicGameObject3.Initialize(true);
					num += mb_DynamicGameObject3.numVerts;
					num2 += mb_DynamicGameObject3.numBlendShapes;
					if (base.settings.renderType == MB_RenderType.skinnedMeshRenderer)
					{
						this._boneProcessor.RemoveBonesForDgosWeAreDeleting(mb_DynamicGameObject3);
					}
					for (int l = 0; l < mb_DynamicGameObject3.submeshNumTris.Length; l++)
					{
						array[l] += mb_DynamicGameObject3.submeshNumTris[l];
					}
				}
				else if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Trying to delete an object that is not in combined mesh");
				}
			}
			this.db_addDeleteGameObjects_CollectMeshData_b.Start();
			for (int m = 0; m < this.mbDynamicObjectsInCombinedMesh.Count; m++)
			{
				if (!this.mbDynamicObjectsInCombinedMesh[m]._beingDeleted)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject4 = this.mbDynamicObjectsInCombinedMesh[m];
					if (base.settings.renderType == MB_RenderType.skinnedMeshRenderer && !this._boneProcessor.GetCachedSMRMeshData(mb_DynamicGameObject4))
					{
						Debug.LogError("Object " + mb_DynamicGameObject4.gameObject.name + " could not retrieve skinning data");
						return false;
					}
				}
			}
			this.db_addDeleteGameObjects_CollectMeshData_b.Stop();
			this.db_addDeleteGameObjects_CollectMeshData.Stop();
			Dictionary<int, MB_Utility.MeshAnalysisResult[]> dictionary = new Dictionary<int, MB_Utility.MeshAnalysisResult[]>();
			int num3 = 0;
			int[] array2 = new int[numResultMats];
			int num4 = 0;
			for (int n = 0; n < list.Count; n++)
			{
				MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject5 = list[n];
				Mesh mesh2 = mb_DynamicGameObject5._mesh;
				Material[] sourceSharedMaterials = mb_DynamicGameObject5.sourceSharedMaterials;
				if (!uvadjuster_Atlas.MapSharedMaterialsToAtlasRects(sourceSharedMaterials, false, mesh2, this._meshChannelsCache, dictionary, sourceMats2submeshIdx_map, mb_DynamicGameObject5.gameObject, mb_DynamicGameObject5))
				{
					_goToAdd[n] = null;
					return false;
				}
				if (_goToAdd[n] != null)
				{
					if (base.settings.doBlendShapes)
					{
						mb_DynamicGameObject5.numBlendShapes = mesh2.blendShapeCount;
					}
					Renderer renderer = mb_DynamicGameObject5._renderer;
					if (this.lightmapIndex == -1)
					{
						this.lightmapIndex = renderer.lightmapIndex;
					}
					if (base.settings.lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping)
					{
						if (this.lightmapIndex != renderer.lightmapIndex && this.LOG_LEVEL >= MB2_LogLevel.warn)
						{
							Debug.LogWarning("Object " + mb_DynamicGameObject5.gameObject.name + " has a different lightmap index. Lightmapping will not work.");
						}
						if (!MBVersion.GetActive(mb_DynamicGameObject5.gameObject) && this.LOG_LEVEL >= MB2_LogLevel.warn)
						{
							Debug.LogWarning("Object " + mb_DynamicGameObject5.gameObject.name + " is inactive. Can only get lightmap index of active objects.");
						}
						if (renderer.lightmapIndex == -1 && this.LOG_LEVEL >= MB2_LogLevel.warn)
						{
							Debug.LogWarning("Object " + mb_DynamicGameObject5.gameObject.name + " does not have an index to a lightmap.");
						}
					}
					mb_DynamicGameObject5.lightmapIndex = renderer.lightmapIndex;
					mb_DynamicGameObject5.lightmapTilingOffset = MBVersion.GetLightmapTilingOffset(renderer);
					if (!this._collectMaterialTriangles(mesh2, mb_DynamicGameObject5, sourceSharedMaterials, sourceMats2submeshIdx_map))
					{
						return false;
					}
					mb_DynamicGameObject5.meshSize = renderer.bounds.size;
					mb_DynamicGameObject5.submeshNumTris = new int[numResultMats];
					mb_DynamicGameObject5.submeshTriIdxs = new int[numResultMats];
					mb_DynamicGameObject5.sourceSharedMaterials = sourceSharedMaterials;
					bool flag = this.textureBakeResults.DoAnyResultMatsUseConsiderMeshUVs();
					if (flag && !this._collectOutOfBoundsUVRects2(mesh2, mb_DynamicGameObject5, sourceSharedMaterials, sourceMats2submeshIdx_map, dictionary))
					{
						return false;
					}
					if (base.settings.renderType == MB_RenderType.skinnedMeshRenderer)
					{
						this.db_addDeleteGameObjects_CollectMeshData.Start();
						this.db_addDeleteGameObjects_CollectMeshData_c.Start();
						if (!this._boneProcessor.GetCachedSMRMeshData(mb_DynamicGameObject5))
						{
							return false;
						}
						this.db_addDeleteGameObjects_CollectMeshData_c.Stop();
						this.db_addDeleteGameObjects_CollectMeshData.Stop();
					}
					if (base.settings.assignToMeshCustomizer != null)
					{
						if (this._UseNativeArrayAPIorNot())
						{
							if (!(base.settings.assignToMeshCustomizer is IAssignToMeshCustomizer_NativeArrays))
							{
								Debug.LogError("Assign To Mesh Customizer must implement IAssignToMeshCustomizer_NativeArrays");
								return false;
							}
						}
						else if (!(base.settings.assignToMeshCustomizer is IAssignToMeshCustomizer_SimpleAPI))
						{
							Debug.LogError("Assign To Mesh Customizer must implemennt IAssignToMeshCustomizer_SimpleAPI");
							return false;
						}
					}
					num3 += mb_DynamicGameObject5.numVerts;
					num4 += mb_DynamicGameObject5.numBlendShapes;
					for (int num5 = 0; num5 < mb_DynamicGameObject5._tmpSubmeshTris.Length; num5++)
					{
						array2[mb_DynamicGameObject5.targetSubmeshIdxs[num5]] += mb_DynamicGameObject5._tmpSubmeshTris[num5].data.Length;
					}
					mb_DynamicGameObject5.invertTriangles = this.IsMirrored(mb_DynamicGameObject5.gameObject.transform.localToWorldMatrix);
				}
			}
			for (int num6 = 0; num6 < _goToAdd.Length; num6++)
			{
				if (_goToAdd[num6] != null && disableRendererInSource)
				{
					MB_Utility.DisableRendererInSource(_goToAdd[num6]);
					if (this.LOG_LEVEL == MB2_LogLevel.trace)
					{
						Debug.Log("Disabling renderer on " + _goToAdd[num6].name + " id=" + _goToAdd[num6].GetInstanceID().ToString());
					}
				}
			}
			bool flag2 = MB3_MeshCombinerSingle.MB_MeshCombinerSingle_SubCombiner._AddToCombined(this, newChannels, num3, num, numResultMats, num4, num2, array2, array, _goToDelete, list, _goToAdd, uvadjuster_Atlas, ref oldMeshData, sw);
			if (flag2)
			{
				this._bakeStatus = MB3_MeshCombiner.MeshCombiningStatus.readyForApply;
			}
			return flag2;
		}

		private Transform[] _getBones(Renderer r, bool isSkinnedMeshWithBones)
		{
			return MBVersion.GetBones(r, isSkinnedMeshWithBones);
		}

		public override bool Apply(MB3_MeshCombiner.GenerateUV2Delegate uv2GenerationMethod)
		{
			this.db_apply.Start();
			bool result = MB3_MeshCombinerSingle.MB_MeshCombinerSingle_SubCombiner.Apply(this, uv2GenerationMethod);
			this.db_apply.Stop();
			return result;
		}

		public virtual void ApplyShowHide()
		{
			this.db_applyShowHide.Start();
			if (this._validationLevel >= MB2_ValidationLevel.quick && !this.ValidateTargRendererAndMeshAndResultSceneObj())
			{
				return;
			}
			MB3_MeshCombinerSingle.MB_MeshCombinerSingle_SubCombiner.ApplyShowHide(this);
			this.db_applyShowHide.Stop();
		}

		public override bool Apply(bool triangles, bool vertices, bool normals, bool tangents, bool uvs, bool uv2, bool uv3, bool uv4, bool colors, bool bones = false, bool blendShapesFlag = false, MB3_MeshCombiner.GenerateUV2Delegate uv2GenerationMethod = null)
		{
			this.db_apply.Start();
			bool result = MB3_MeshCombinerSingle.MB_MeshCombinerSingle_SubCombiner.Apply(this, triangles, vertices, normals, tangents, uvs, uv2, uv3, uv4, colors, bones, blendShapesFlag, uv2GenerationMethod);
			this.db_apply.Stop();
			return result;
		}

		public override bool Apply(bool triangles, bool vertices, bool normals, bool tangents, bool uvs, bool uv2, bool uv3, bool uv4, bool uv5, bool uv6, bool uv7, bool uv8, bool colors, bool bones = false, bool blendShapesFlag = false, MB3_MeshCombiner.GenerateUV2Delegate uv2GenerationMethod = null)
		{
			this.db_apply.Start();
			bool result = MB3_MeshCombinerSingle.MB_MeshCombinerSingle_SubCombiner.Apply(this, triangles, vertices, normals, tangents, uvs, uv2, uv3, uv4, uv5, uv6, uv7, uv8, colors, bones, blendShapesFlag, false, uv2GenerationMethod);
			this.db_apply.Stop();
			return result;
		}

		public override bool UpdateGameObjects(GameObject[] gos, bool recalcBounds, bool updateVertices, bool updateNormals, bool updateTangents, bool updateUV, bool updateUV2, bool updateUV3, bool updateUV4, bool updateColors, bool updateSkinningInfo)
		{
			this.db_updateGameObjects.Start();
			bool result = this._UpdateGameObjects(gos, recalcBounds, updateVertices, updateNormals, updateTangents, updateUV, updateUV2, updateUV3, updateUV4, false, false, false, false, updateColors, updateSkinningInfo);
			this.db_updateGameObjects.Stop();
			return result;
		}

		public override bool UpdateGameObjects(GameObject[] gos, bool recalcBounds, bool updateVertices, bool updateNormals, bool updateTangents, bool updateUV, bool updateUV2, bool updateUV3, bool updateUV4, bool updateUV5, bool updateUV6, bool updateUV7, bool updateUV8, bool updateColors, bool updateSkinningInfo)
		{
			this.db_updateGameObjects.Start();
			bool result = this._UpdateGameObjects(gos, recalcBounds, updateVertices, updateNormals, updateTangents, updateUV, updateUV2, updateUV3, updateUV4, updateUV5, updateUV6, updateUV7, updateUV8, updateColors, updateSkinningInfo);
			this.db_updateGameObjects.Stop();
			return result;
		}

		internal bool _UpdateGameObjects(GameObject[] gos, bool recalcBounds, bool updateVertices, bool updateNormals, bool updateTangents, bool updateUV, bool updateUV2, bool updateUV3, bool updateUV4, bool updateUV5, bool updateUV6, bool updateUV7, bool updateUV8, bool updateColors, bool updateSkinningInfo)
		{
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log("UpdateGameObjects called on " + gos.Length.ToString() + " objects.");
			}
			int numResultMats = 1;
			if (this.textureBakeResults.doMultiMaterial)
			{
				numResultMats = this.textureBakeResults.NumResultMaterials();
			}
			if (!this._Initialize(numResultMats))
			{
				return false;
			}
			bool uvsSliceIdx_w = base.settings.doUV && this.textureBakeResults.resultType == MB2_TextureBakeResults.ResultType.textureArray;
			MB_MeshVertexChannelFlags meshChannelsAsFlags = MeshBakerSettingsUtility.GetMeshChannelsAsFlags(base.settings, true, uvsSliceIdx_w);
			if (this.channelsLastBake != meshChannelsAsFlags)
			{
				Debug.LogError("Channels changed since previous bake. Can't Update GameObjects.");
				return false;
			}
			if (this._bakeStatus != MB3_MeshCombiner.MeshCombiningStatus.preAddDeleteOrUpdate)
			{
				Debug.LogError("Bake Status of combiner was not 'preAddDeleteOrUpdate'. This can happen if AddDeleteGameObjects or UpdateGameObjects is called twice without calling Apply. You can call 'ClearBuffers' to reset the combiner.");
				return false;
			}
			if (this._mesh.vertexCount > 0 && this._instance2combined_map.Count == 0)
			{
				Debug.LogWarning("There were vertices in the combined mesh but nothing in the MeshBaker buffers. If you are trying to bake in the editor and modify at runtime, make sure 'Clear Buffers After Bake' is unchecked.");
			}
			if (base.settings.assignToMeshCustomizer != null)
			{
				if (this._UseNativeArrayAPIorNot())
				{
					if (!(base.settings.assignToMeshCustomizer is IAssignToMeshCustomizer_NativeArrays))
					{
						Debug.LogError("Assign To Mesh Customizer must implement IAssignToMeshCustomizer_NativeArrays");
						return false;
					}
				}
				else if (!(base.settings.assignToMeshCustomizer is IAssignToMeshCustomizer_SimpleAPI))
				{
					Debug.LogError("Assign To Mesh Customizer must implemennt IAssignToMeshCustomizer_SimpleAPI");
					return false;
				}
			}
			MB3_MeshCombinerSingle.UVAdjuster_Atlas uVAdjuster = null;
			OrderedDictionary orderedDictionary = null;
			Dictionary<int, MB_Utility.MeshAnalysisResult[]> meshAnalysisResultsCache = null;
			if (updateUV)
			{
				orderedDictionary = this.BuildSourceMatsToSubmeshIdxMap(numResultMats);
				if (orderedDictionary == null)
				{
					return false;
				}
				uVAdjuster = new MB3_MeshCombinerSingle.UVAdjuster_Atlas(this.textureBakeResults, this.LOG_LEVEL);
				meshAnalysisResultsCache = new Dictionary<int, MB_Utility.MeshAnalysisResult[]>();
			}
			if (this._vertexAndTriProcessor != null && !this._vertexAndTriProcessor.IsDisposed())
			{
				this._vertexAndTriProcessor.Dispose();
			}
			this._blendShapeProcessor = new MB3_MeshCombinerSingle.MB_MeshCombinerSingle_BlendShapeProcessor(this);
			bool flag = this._UseNativeArrayAPIorNot();
			this._meshChannelsCache = MB3_MeshCombinerSingle.Create_MeshChannelsCache(flag, this.LOG_LEVEL, base.settings.lightmapOption);
			this._vertexAndTriProcessor = MB3_MeshCombinerSingle.Create_VertexAndTriangleProcessor(flag);
			this._boneProcessor = this.Create_BoneProcessor(flag);
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				if (flag)
				{
					Debug.Log("using NativeArray mesh API");
				}
				else
				{
					Debug.Log("using simple mesh API");
				}
			}
			bool flag2 = true;
			try
			{
				flag2 = (flag2 && this.__UpdateGameObjects(gos, recalcBounds, meshChannelsAsFlags, updateVertices, updateNormals, updateTangents, updateUV, updateUV2, updateUV3, updateUV4, updateUV5, updateUV6, updateUV7, updateUV8, updateColors, updateSkinningInfo, meshAnalysisResultsCache, orderedDictionary, uVAdjuster));
			}
			catch
			{
				flag2 = false;
				throw;
			}
			finally
			{
				this._meshChannelsCache.Dispose();
				this._boneProcessor.DisposeOfTemporarySMRData();
				for (int i = 0; i < this.mbDynamicObjectsInCombinedMesh.Count; i++)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject = this.mbDynamicObjectsInCombinedMesh[i];
					if (mb_DynamicGameObject._initialized)
					{
						mb_DynamicGameObject.UnInitialize();
					}
				}
			}
			return flag2;
		}

		private bool __UpdateGameObjects(GameObject[] gos, bool recalcBounds, MB_MeshVertexChannelFlags newChannels, bool updateVertices, bool updateNormals, bool updateTangents, bool updateUV, bool updateUV2, bool updateUV3, bool updateUV4, bool updateUV5, bool updateUV6, bool updateUV7, bool updateUV8, bool updateColors, bool updateSkinningInfo, Dictionary<int, MB_Utility.MeshAnalysisResult[]> meshAnalysisResultsCache, OrderedDictionary sourceMats2submeshIdx_map, MB3_MeshCombinerSingle.UVAdjuster_Atlas uVAdjuster)
		{
			List<MB3_MeshCombinerSingle.MB_DynamicGameObject> list = new List<MB3_MeshCombinerSingle.MB_DynamicGameObject>();
			for (int i = 0; i < gos.Length; i++)
			{
				MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject = this._instance2combined_map[gos[i]];
				if (!mb_DynamicGameObject.Initialize(false))
				{
					Debug.LogError("Object " + mb_DynamicGameObject.name + " could not be initialized");
					return false;
				}
				list.Add(mb_DynamicGameObject);
				if (mb_DynamicGameObject._mesh == null)
				{
					Debug.LogError("Object " + mb_DynamicGameObject.name + " had no renderer");
					return false;
				}
				if (mb_DynamicGameObject._renderer == null)
				{
					Debug.LogError("Object " + mb_DynamicGameObject.name + " had no renderer");
					return false;
				}
				Mesh mesh = mb_DynamicGameObject._mesh;
				if (mb_DynamicGameObject.numVerts != mesh.vertexCount)
				{
					Debug.LogError("Object " + mb_DynamicGameObject.gameObject.name + " source mesh has been modified since being added. To update it must have the same number of verts");
					return false;
				}
			}
			for (int j = 0; j < this.mbDynamicObjectsInCombinedMesh.Count; j++)
			{
				if (!this.mbDynamicObjectsInCombinedMesh[j]._beingDeleted)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject2 = this.mbDynamicObjectsInCombinedMesh[j];
					if (!mb_DynamicGameObject2.Initialize(false))
					{
						Debug.LogError("Object " + mb_DynamicGameObject2.gameObject.name + " does not have a Renderer");
						return false;
					}
				}
			}
			this._meshChannelsCache.CollectChannelDataForAllMeshesInList(this.mbDynamicObjectsInCombinedMesh, list, newChannels, base.settings.renderType, base.settings.doBlendShapes);
			for (int k = 0; k < gos.Length; k++)
			{
				MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject3 = this._instance2combined_map[gos[k]];
				if (base.settings.doUV && updateUV)
				{
					Material[] sharedMaterials = mb_DynamicGameObject3._renderer.sharedMaterials;
					if (!uVAdjuster.MapSharedMaterialsToAtlasRects(sharedMaterials, true, mb_DynamicGameObject3._mesh, this._meshChannelsCache, meshAnalysisResultsCache, sourceMats2submeshIdx_map, mb_DynamicGameObject3.gameObject, mb_DynamicGameObject3))
					{
						return false;
					}
				}
			}
			this._boneProcessor.BuildBoneIdx2DGOMapIfNecessary(null);
			bool flag = MB3_MeshCombinerSingle.MB_MeshCombinerSingle_SubCombiner._UpdateGameObjects(this, list, newChannels, updateVertices, updateNormals, updateTangents, updateUV, updateUV2, updateUV3, updateUV4, updateUV5, updateUV6, updateUV7, updateUV8, updateColors, updateSkinningInfo, uVAdjuster, this.LOG_LEVEL);
			if (flag && recalcBounds)
			{
				this._mesh.RecalculateBounds();
			}
			return flag;
		}

		public bool ShowHideGameObjects(GameObject[] toShow, GameObject[] toHide)
		{
			this.db_showHideGameObjects.Start();
			if (this.textureBakeResults == null)
			{
				Debug.LogError("TextureBakeResults must be set.");
				return false;
			}
			bool result = this._ShowHide(toShow, toHide);
			this.db_showHideGameObjects.Stop();
			return result;
		}

		public override bool AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs, bool disableRendererInSource = true)
		{
			this.db_addDeleteGameObjects.Start();
			int[] array = null;
			if (deleteGOs != null)
			{
				array = new int[deleteGOs.Length];
				for (int i = 0; i < deleteGOs.Length; i++)
				{
					if (deleteGOs[i] == null)
					{
						Debug.LogError("The " + i.ToString() + "th object on the list of objects to delete is 'Null'");
					}
					else
					{
						array[i] = deleteGOs[i].GetInstanceID();
					}
				}
			}
			bool result = this.AddDeleteGameObjectsByID(gos, array, disableRendererInSource);
			this.db_addDeleteGameObjects.Stop();
			return result;
		}

		public override bool AddDeleteGameObjectsByID(GameObject[] gos, int[] deleteGOinstanceIDs, bool disableRendererInSource)
		{
			this.db_addDeleteGameObjects.Start();
			if (this.validationLevel > MB2_ValidationLevel.none)
			{
				if (gos != null)
				{
					for (int i = 0; i < gos.Length; i++)
					{
						if (gos[i] == null)
						{
							Debug.LogError("The " + i.ToString() + "th object on the list of objects to combine is 'None'. Use Command-Delete on Mac OS X; Delete or Shift-Delete on Windows to remove this one element.");
							return false;
						}
						if (this.validationLevel >= MB2_ValidationLevel.robust)
						{
							for (int j = i + 1; j < gos.Length; j++)
							{
								if (gos[i] == gos[j])
								{
									string str = "GameObject ";
									GameObject gameObject = gos[i];
									Debug.LogError(str + ((gameObject != null) ? gameObject.ToString() : null) + " appears twice in list of game objects to add");
									return false;
								}
							}
						}
					}
				}
				if (deleteGOinstanceIDs != null)
				{
					bool flag = true;
					HashSet<int> hashSet = new HashSet<int>(deleteGOinstanceIDs);
					for (int k = 0; k < this.mbDynamicObjectsInCombinedMesh.Count; k++)
					{
						if (!hashSet.Contains(this.mbDynamicObjectsInCombinedMesh[k].instanceID))
						{
							flag = false;
							break;
						}
					}
					if (!flag)
					{
						for (int l = 0; l < this.mbDynamicObjectsInCombinedMesh.Count; l++)
						{
							if (this.mbDynamicObjectsInCombinedMesh[l].gameObject == null)
							{
								Debug.LogError("An instanceID to be deleted does not match any of the cached instanceIDs from the bake and the  corresponding source game object has already been deleted. This can happen if objects were baked, then the scene was saved, closed, opened and a delete bake is attempted. Try deleting a source object from the baked mesh by passing in the source game object instead of the instance ID");
								return false;
							}
							this.mbDynamicObjectsInCombinedMesh[l].instanceID = this.mbDynamicObjectsInCombinedMesh[l].gameObject.GetInstanceID();
						}
					}
					if (this.validationLevel >= MB2_ValidationLevel.robust)
					{
						for (int m = 0; m < deleteGOinstanceIDs.Length; m++)
						{
							for (int n = m + 1; n < deleteGOinstanceIDs.Length; n++)
							{
								if (deleteGOinstanceIDs[m] == deleteGOinstanceIDs[n])
								{
									Debug.LogError("GameObject " + deleteGOinstanceIDs[m].ToString() + "appears twice in list of game objects to delete");
									return false;
								}
							}
						}
					}
				}
			}
			if (this._bakeStatus != MB3_MeshCombiner.MeshCombiningStatus.preAddDeleteOrUpdate)
			{
				Debug.LogError("Bake Status of combiner was not 'preAddDeleteOrUpdate'. This can happen if AddDeleteGameObjects or UpdateGameObjects is called twice without calling Apply. You can call 'ClearBuffers' to reset the combiner.");
				return false;
			}
			if (this._usingTemporaryTextureBakeResult && gos != null && gos.Length != 0)
			{
				MB_Utility.Destroy(this._textureBakeResults);
				this._textureBakeResults = null;
				this._usingTemporaryTextureBakeResult = false;
			}
			if (this._textureBakeResults == null && gos != null && gos.Length != 0 && gos[0] != null && !this._CreateTemporaryTextrueBakeResult(gos, this.GetMaterialsOnTargetRenderer()))
			{
				return false;
			}
			this.BuildSceneMeshObject(gos, false);
			if (!this._AddToCombined(gos, deleteGOinstanceIDs, disableRendererInSource))
			{
				Debug.LogError("Failed to add/delete objects to combined mesh");
				return false;
			}
			this.db_addDeleteGameObjects.Stop();
			return true;
		}

		public override bool CombinedMeshContains(GameObject go)
		{
			return this.objectsInCombinedMesh.Contains(go);
		}

		public override void ClearBuffers()
		{
			this.bones = new Transform[0];
			this.bindPoses = new Matrix4x4[0];
			this.blendShapes = new MB3_MeshCombinerSingle.MBBlendShape[0];
			this.mbDynamicObjectsInCombinedMesh.Clear();
			this.objectsInCombinedMesh.Clear();
			if (this._vertexAndTriProcessor != null && !this._vertexAndTriProcessor.IsDisposed())
			{
				this._vertexAndTriProcessor.Dispose();
			}
			this._vertexAndTriProcessor = MB3_MeshCombinerSingle.Create_VertexAndTriangleProcessor(this._UseNativeArrayAPIorNot());
			this.verts = new Vector3[0];
			this.normals = new Vector3[0];
			this.tangents = new Vector4[0];
			this.uvs = new Vector2[0];
			this.uvsSliceIdx = new float[0];
			this.uv2s = new Vector2[0];
			this.uv3s = new Vector2[0];
			this.uv4s = new Vector2[0];
			this.uv5s = new Vector2[0];
			this.uv6s = new Vector2[0];
			this.uv7s = new Vector2[0];
			this.uv8s = new Vector2[0];
			this.colors = new Color[0];
			this.submeshTris = new MB3_MeshCombinerSingle.SerializableIntArray[0];
			if (this.submeshTris != null)
			{
				for (int i = 0; i < this.submeshTris.Length; i++)
				{
					if (this.submeshTris[i].data == null)
					{
						this.submeshTris[i].data = new int[0];
					}
					else if (this.submeshTris[i].data.Length != 0)
					{
						this.submeshTris[i].data = new int[0];
					}
				}
				this.submeshTris = null;
			}
			this.instance2Combined_MapClear();
			if (this._usingTemporaryTextureBakeResult)
			{
				MB_Utility.Destroy(this._textureBakeResults);
				this._textureBakeResults = null;
				this._usingTemporaryTextureBakeResult = false;
			}
			this._bakeStatus = MB3_MeshCombiner.MeshCombiningStatus.preAddDeleteOrUpdate;
			if (this.LOG_LEVEL >= MB2_LogLevel.trace)
			{
				MB2_Log.LogDebug("ClearBuffers called", Array.Empty<object>());
			}
		}

		private Mesh _NewMesh()
		{
			if (Application.isPlaying)
			{
				this._meshBirth = MB3_MeshCombinerSingle.MeshCreationConditions.CreatedAtRuntime;
			}
			else
			{
				this._meshBirth = MB3_MeshCombinerSingle.MeshCreationConditions.CreatedInEditor;
			}
			return new Mesh();
		}

		public override void ClearMesh()
		{
			if (this._mesh != null)
			{
				MBVersion.MeshClear(this._mesh, false);
			}
			else
			{
				this._mesh = this._NewMesh();
			}
			this.ClearBuffers();
		}

		public override void ClearMesh(MB2_EditorMethodsInterface editorMethods)
		{
			this.ClearMesh();
		}

		internal override void _DisposeRuntimeCreated()
		{
			if (Application.isPlaying)
			{
				if (this._meshBirth == MB3_MeshCombinerSingle.MeshCreationConditions.CreatedAtRuntime)
				{
					if (!MBVersion.IsAssetInProject(this._mesh))
					{
						Object.Destroy(this._mesh);
					}
				}
				else if (this._meshBirth == MB3_MeshCombinerSingle.MeshCreationConditions.AssignedByUser)
				{
					this._mesh = null;
				}
				this.ClearBuffers();
			}
		}

		public override void DestroyMesh()
		{
			if (this._mesh != null)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug("Destroying Mesh", Array.Empty<object>());
				}
				MB_Utility.Destroy(this._mesh);
				this._meshBirth = MB3_MeshCombinerSingle.MeshCreationConditions.NoMesh;
			}
			this.ClearBuffers();
		}

		public override void DestroyMeshEditor(MB2_EditorMethodsInterface editorMethods)
		{
			if (this._mesh != null && editorMethods != null && !Application.isPlaying)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug("Destroying Mesh", Array.Empty<object>());
				}
				editorMethods.Destroy(this._mesh);
			}
			this.ClearBuffers();
		}

		public bool ValidateTargRendererAndMeshAndResultSceneObj()
		{
			if (this._resultSceneObject == null)
			{
				if (this._LOG_LEVEL >= MB2_LogLevel.error)
				{
					Debug.LogError("Result Scene Object was not set.");
				}
				return false;
			}
			if (this._targetRenderer == null)
			{
				if (this._LOG_LEVEL >= MB2_LogLevel.error)
				{
					Debug.LogError("Target Renderer was not set.");
				}
				return false;
			}
			if (this._resultSceneObject != null && this._targetRenderer.transform.parent != this._resultSceneObject.transform)
			{
				if (this._LOG_LEVEL >= MB2_LogLevel.error)
				{
					Debug.LogError("Target Renderer game object is not a child of Result Scene Object.");
				}
				return false;
			}
			if (base.settings.renderType == MB_RenderType.skinnedMeshRenderer && !(this._targetRenderer is SkinnedMeshRenderer))
			{
				if (this._LOG_LEVEL >= MB2_LogLevel.error)
				{
					Debug.LogError("Render Type is skinned mesh renderer but Target Renderer is not.");
				}
				return false;
			}
			if (base.settings.renderType == MB_RenderType.meshRenderer)
			{
				if (!(this._targetRenderer is MeshRenderer))
				{
					if (this._LOG_LEVEL >= MB2_LogLevel.error)
					{
						Debug.LogError("Render Type is mesh renderer but Target Renderer is not.");
					}
					return false;
				}
				MeshFilter component = this._targetRenderer.GetComponent<MeshFilter>();
				if (this._mesh != component.sharedMesh)
				{
					if (this._LOG_LEVEL >= MB2_LogLevel.error)
					{
						Debug.LogError("Target renderer mesh is not equal to mesh.");
					}
					return false;
				}
			}
			return true;
		}

		private OrderedDictionary BuildSourceMatsToSubmeshIdxMap(int numResultMats)
		{
			OrderedDictionary orderedDictionary = new OrderedDictionary();
			for (int i = 0; i < numResultMats; i++)
			{
				List<Material> sourceMaterialsUsedByResultMaterial = this._textureBakeResults.GetSourceMaterialsUsedByResultMaterial(i);
				for (int j = 0; j < sourceMaterialsUsedByResultMaterial.Count; j++)
				{
					if (sourceMaterialsUsedByResultMaterial[j] == null)
					{
						Debug.LogError("Found null material in source materials for combined mesh materials " + i.ToString());
						return null;
					}
					if (!orderedDictionary.Contains(sourceMaterialsUsedByResultMaterial[j]))
					{
						orderedDictionary.Add(sourceMaterialsUsedByResultMaterial[j], i);
					}
				}
			}
			return orderedDictionary;
		}

		internal Renderer BuildSceneHierarchPreBake(MB3_MeshCombinerSingle mom, GameObject root, Mesh m, bool createNewChild = false, GameObject[] objsToBeAdded = null)
		{
			if (mom._LOG_LEVEL >= MB2_LogLevel.trace)
			{
				Debug.Log("Building Scene Hierarchy createNewChild=" + createNewChild.ToString());
			}
			MeshFilter meshFilter = null;
			MeshRenderer meshRenderer = null;
			SkinnedMeshRenderer skinnedMeshRenderer = null;
			Transform transform = null;
			if (root == null)
			{
				Debug.LogError("root was null.");
				return null;
			}
			if (mom.textureBakeResults == null)
			{
				Debug.LogError("textureBakeResults must be set.");
				return null;
			}
			if (root.GetComponent<Renderer>() != null)
			{
				Debug.LogError("root game object cannot have a renderer component");
				return null;
			}
			if (!createNewChild)
			{
				if (mom.targetRenderer != null && mom.targetRenderer.transform.parent == root.transform)
				{
					transform = mom.targetRenderer.transform;
				}
				else
				{
					Renderer[] componentsInChildren = root.GetComponentsInChildren<Renderer>(true);
					if (componentsInChildren.Length == 1)
					{
						if (componentsInChildren[0].transform.parent != root.transform)
						{
							Debug.LogError("Target Renderer is not an immediate child of Result Scene Object. Try using a game object with no children as the Result Scene Object..");
						}
						transform = componentsInChildren[0].transform;
					}
				}
			}
			if (transform != null && transform.parent != root.transform)
			{
				transform = null;
			}
			if (transform == null)
			{
				transform = new GameObject(mom.name + "-mesh")
				{
					transform = 
					{
						parent = root.transform
					}
				}.transform;
			}
			transform.parent = root.transform;
			GameObject gameObject = transform.gameObject;
			if (base.settings.renderType == MB_RenderType.skinnedMeshRenderer)
			{
				MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
				if (component != null)
				{
					MB_Utility.Destroy(component);
				}
				MeshFilter component2 = gameObject.GetComponent<MeshFilter>();
				if (component2 != null)
				{
					MB_Utility.Destroy(component2);
				}
				skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
				if (skinnedMeshRenderer == null)
				{
					skinnedMeshRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
				}
			}
			else
			{
				SkinnedMeshRenderer component3 = gameObject.GetComponent<SkinnedMeshRenderer>();
				if (component3 != null)
				{
					MB_Utility.Destroy(component3);
				}
				meshFilter = gameObject.GetComponent<MeshFilter>();
				if (meshFilter == null)
				{
					meshFilter = gameObject.AddComponent<MeshFilter>();
				}
				meshRenderer = gameObject.GetComponent<MeshRenderer>();
				if (meshRenderer == null)
				{
					meshRenderer = gameObject.AddComponent<MeshRenderer>();
				}
			}
			if (base.settings.renderType == MB_RenderType.skinnedMeshRenderer)
			{
				skinnedMeshRenderer.bones = mom.GetBones();
				bool updateWhenOffscreen = skinnedMeshRenderer.updateWhenOffscreen;
				skinnedMeshRenderer.updateWhenOffscreen = true;
				skinnedMeshRenderer.updateWhenOffscreen = updateWhenOffscreen;
			}
			MB3_MeshCombinerSingle._ConfigureSceneHierarch(mom, root, meshRenderer, meshFilter, skinnedMeshRenderer, m, objsToBeAdded);
			if (base.settings.renderType == MB_RenderType.skinnedMeshRenderer)
			{
				return skinnedMeshRenderer;
			}
			return meshRenderer;
		}

		private static void _ConfigureSceneHierarch(MB3_MeshCombinerSingle mom, GameObject root, MeshRenderer mr, MeshFilter mf, SkinnedMeshRenderer smr, Mesh m, GameObject[] objsToBeAdded = null)
		{
			GameObject gameObject;
			if (mom.settings.renderType == MB_RenderType.skinnedMeshRenderer)
			{
				gameObject = smr.gameObject;
				smr.lightmapIndex = mom.GetLightmapIndex();
			}
			else
			{
				gameObject = mr.gameObject;
				mf.sharedMesh = m;
				mom._SetLightmapIndexIfPreserveLightmapping(mr);
			}
			if (mom.settings.lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping || mom.settings.lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout)
			{
				gameObject.isStatic = true;
			}
			if (objsToBeAdded != null && objsToBeAdded.Length != 0 && objsToBeAdded[0] != null)
			{
				bool flag = true;
				bool flag2 = true;
				string tag = objsToBeAdded[0].tag;
				int layer = objsToBeAdded[0].layer;
				for (int i = 0; i < objsToBeAdded.Length; i++)
				{
					if (objsToBeAdded[i] != null)
					{
						if (!objsToBeAdded[i].tag.Equals(tag))
						{
							flag = false;
						}
						if (objsToBeAdded[i].layer != layer)
						{
							flag2 = false;
						}
					}
				}
				if (flag)
				{
					root.tag = tag;
					gameObject.tag = tag;
				}
				if (flag2)
				{
					root.layer = layer;
					gameObject.layer = layer;
				}
			}
		}

		private void _SetLightmapIndexIfPreserveLightmapping(Renderer tr)
		{
			tr.lightmapIndex = this.GetLightmapIndex();
			tr.lightmapScaleOffset = new Vector4(1f, 1f, 0f, 0f);
			if (base.settings.lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping)
			{
				MB_PreserveLightmapData mb_PreserveLightmapData = tr.gameObject.GetComponent<MB_PreserveLightmapData>();
				if (mb_PreserveLightmapData == null)
				{
					mb_PreserveLightmapData = tr.gameObject.AddComponent<MB_PreserveLightmapData>();
				}
				mb_PreserveLightmapData.lightmapIndex = this.GetLightmapIndex();
				mb_PreserveLightmapData.lightmapScaleOffset = new Vector4(1f, 1f, 0f, 0f);
			}
		}

		public void BuildSceneMeshObject(GameObject[] gos = null, bool createNewChild = false)
		{
			if (this._resultSceneObject == null)
			{
				this._resultSceneObject = new GameObject("CombinedMesh-" + base.name);
			}
			this._targetRenderer = this.BuildSceneHierarchPreBake(this, this._resultSceneObject, this.GetMesh(), createNewChild, gos);
		}

		private bool IsMirrored(Matrix4x4 tm)
		{
			Vector3 lhs = tm.GetRow(0);
			Vector3 rhs = tm.GetRow(1);
			Vector3 rhs2 = tm.GetRow(2);
			lhs.Normalize();
			rhs.Normalize();
			rhs2.Normalize();
			return Vector3.Dot(Vector3.Cross(lhs, rhs), rhs2) < 0f;
		}

		public override void CheckIntegrity()
		{
			if (!MB_Utility.DO_INTEGRITY_CHECKS)
			{
				return;
			}
			if (this._boneProcessor != null)
			{
				this._boneProcessor.DB_CheckIntegrity();
			}
			if (base.settings.doBlendShapes && base.settings.renderType != MB_RenderType.skinnedMeshRenderer)
			{
				Debug.LogError("Blend shapes can only be used with skinned meshes.");
			}
		}

		public override List<Material> GetMaterialsOnTargetRenderer()
		{
			List<Material> list = new List<Material>();
			if (this._targetRenderer != null)
			{
				list.AddRange(this._targetRenderer.sharedMaterials);
			}
			return list;
		}

		private bool _UseNativeArrayAPIorNot()
		{
			return base.settings.meshAPI == MB_MeshCombineAPIType.betaNativeArrayAPI;
		}

		public MB_IMeshCombinerSingle_BoneProcessor Create_BoneProcessor(bool doNativeArrays)
		{
			MB_IMeshCombinerSingle_BoneProcessor result;
			if (doNativeArrays)
			{
				result = new MB3_MeshCombinerSingle.MB_MeshCombinerSingle_BoneProcessorNewAPI(this);
			}
			else
			{
				result = new MB3_MeshCombinerSingle.MB_MeshCombinerSingle_BoneProcessor(this);
			}
			return result;
		}

		public static MB3_MeshCombinerSingle.IVertexAndTriangleProcessor Create_VertexAndTriangleProcessor(bool doNativeArrays)
		{
			MB3_MeshCombinerSingle.IVertexAndTriangleProcessor result;
			if (doNativeArrays)
			{
				result = default(MB3_MeshCombinerSingle.VertexAndTriangleProcessorNativeArray);
			}
			else
			{
				result = default(MB3_MeshCombinerSingle.VertexAndTriangleProcessor);
			}
			return result;
		}

		public static MB3_MeshCombinerSingle.IMeshChannelsCacheTaggingInterface Create_MeshChannelsCache(bool doNativeArrays, MB2_LogLevel LOG_LEVEL, MB2_LightmapOptions lightmapOption)
		{
			MB3_MeshCombinerSingle.IMeshChannelsCacheTaggingInterface result;
			if (doNativeArrays)
			{
				result = new MB3_MeshCombinerSingle.MeshChannelsCache_NativeArray(LOG_LEVEL, lightmapOption);
			}
			else
			{
				result = new MB3_MeshCombinerSingle.MeshChannelsCache(LOG_LEVEL, lightmapOption);
			}
			return result;
		}

		public override void UpdateSkinnedMeshApproximateBounds()
		{
			this.UpdateSkinnedMeshApproximateBoundsFromBounds();
		}

		public override void UpdateSkinnedMeshApproximateBoundsFromBones()
		{
			if (this.outputOption == MB2_OutputOptions.bakeMeshAssetsInPlace)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Can't UpdateSkinnedMeshApproximateBounds when output type is bakeMeshAssetsInPlace");
				}
				return;
			}
			if (this.bones.Length == 0)
			{
				if (this.GetVertexCount() > 0 && this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("No bones in SkinnedMeshRenderer. Could not UpdateSkinnedMeshApproximateBounds.");
				}
				return;
			}
			if (this._targetRenderer == null)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Target Renderer is not set. No point in calling UpdateSkinnedMeshApproximateBounds.");
				}
				return;
			}
			if (!this._targetRenderer.GetType().Equals(typeof(SkinnedMeshRenderer)))
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Target Renderer is not a SkinnedMeshRenderer. No point in calling UpdateSkinnedMeshApproximateBounds.");
				}
				return;
			}
			MB3_MeshCombiner.UpdateSkinnedMeshApproximateBoundsFromBonesStatic(this.bones, (SkinnedMeshRenderer)this.targetRenderer);
		}

		public override void UpdateSkinnedMeshApproximateBoundsFromBounds()
		{
			if (this.outputOption == MB2_OutputOptions.bakeMeshAssetsInPlace)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Can't UpdateSkinnedMeshApproximateBoundsFromBounds when output type is bakeMeshAssetsInPlace");
				}
				return;
			}
			if (this.GetVertexCount() == 0 || this.mbDynamicObjectsInCombinedMesh.Count == 0)
			{
				if (this.GetVertexCount() > 0 && this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Nothing in SkinnedMeshRenderer. CoulddoBlendShapes not UpdateSkinnedMeshApproximateBoundsFromBounds.");
				}
				return;
			}
			if (this._targetRenderer == null)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Target Renderer is not set. No point in calling UpdateSkinnedMeshApproximateBoundsFromBounds.");
				}
				return;
			}
			if (!this._targetRenderer.GetType().Equals(typeof(SkinnedMeshRenderer)))
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Target Renderer is not a SkinnedMeshRenderer. No point in calling UpdateSkinnedMeshApproximateBoundsFromBounds.");
				}
				return;
			}
			MB3_MeshCombiner.UpdateSkinnedMeshApproximateBoundsFromBoundsStatic(this.objectsInCombinedMesh, (SkinnedMeshRenderer)this.targetRenderer);
		}

		private static void _UpdateMaterialsOnTargetRenderer(MB2_TextureBakeResults textureBakeResults, Renderer targetRenderer, MB3_MeshCombinerSingle.SerializableIntArray[] subTris, int numNonZeroLengthSubmeshTris)
		{
			if (subTris.Length != textureBakeResults.NumResultMaterials())
			{
				Debug.LogError("Mismatch between number of submeshes and number of result materials " + subTris.Length.ToString() + " " + textureBakeResults.NumResultMaterials().ToString());
			}
			Material[] array = new Material[numNonZeroLengthSubmeshTris];
			int num = 0;
			for (int i = 0; i < subTris.Length; i++)
			{
				if (subTris[i].data.Length != 0)
				{
					array[num] = textureBakeResults.GetCombinedMaterialForSubmesh(i);
					num++;
				}
			}
			targetRenderer.materials = array;
		}

		public Stopwatch db_showHideGameObjects = new Stopwatch();

		public Stopwatch db_addDeleteGameObjects = new Stopwatch();

		public Stopwatch db_addDeleteGameObjects_CollectMeshData = new Stopwatch();

		public Stopwatch db_addDeleteGameObjects_CollectMeshData_a = new Stopwatch();

		public Stopwatch db_addDeleteGameObjects_CollectMeshData_b = new Stopwatch();

		public Stopwatch db_addDeleteGameObjects_CollectMeshData_c = new Stopwatch();

		public Stopwatch db_addDeleteGameObjects_InitFromMeshCombiner = new Stopwatch();

		public Stopwatch db_addDeleteGameObjects_Init = new Stopwatch();

		public Stopwatch db_addDeleteGameObjects_CopyArraysFromPreviousBakeBuffersToNewBuffers = new Stopwatch();

		public Stopwatch db_addDeleteGameObjects_CopyFromDGOMeshToBuffers = new Stopwatch();

		public Stopwatch db_apply = new Stopwatch();

		public Stopwatch db_applyShowHide = new Stopwatch();

		public Stopwatch db_updateGameObjects = new Stopwatch();

		[SerializeField]
		protected List<GameObject> objectsInCombinedMesh = new List<GameObject>();

		[SerializeField]
		private int lightmapIndex = -1;

		[SerializeField]
		public List<MB3_MeshCombinerSingle.MB_DynamicGameObject> mbDynamicObjectsInCombinedMesh = new List<MB3_MeshCombinerSingle.MB_DynamicGameObject>();

		private Dictionary<GameObject, MB3_MeshCombinerSingle.MB_DynamicGameObject> _instance2combined_map = new Dictionary<GameObject, MB3_MeshCombinerSingle.MB_DynamicGameObject>();

		[SerializeField]
		private MB_MeshVertexChannelFlags channelsLastBake;

		[SerializeField]
		private Vector3[] verts = new Vector3[0];

		[SerializeField]
		private Vector3[] normals = new Vector3[0];

		[SerializeField]
		private Vector4[] tangents = new Vector4[0];

		[SerializeField]
		private Vector2[] uvs = new Vector2[0];

		[SerializeField]
		private float[] uvsSliceIdx = new float[0];

		[SerializeField]
		private Vector2[] uv2s = new Vector2[0];

		[SerializeField]
		private Vector2[] uv3s = new Vector2[0];

		[SerializeField]
		private Vector2[] uv4s = new Vector2[0];

		[SerializeField]
		private Vector2[] uv5s = new Vector2[0];

		[SerializeField]
		private Vector2[] uv6s = new Vector2[0];

		[SerializeField]
		private Vector2[] uv7s = new Vector2[0];

		[SerializeField]
		private Vector2[] uv8s = new Vector2[0];

		[SerializeField]
		private Color[] colors = new Color[0];

		[SerializeField]
		private MB3_MeshCombinerSingle.SerializableIntArray[] submeshTris = new MB3_MeshCombinerSingle.SerializableIntArray[0];

		[SerializeField]
		private Matrix4x4[] bindPoses = new Matrix4x4[0];

		[SerializeField]
		private Transform[] bones = new Transform[0];

		[SerializeField]
		internal MB3_MeshCombinerSingle.MBBlendShape[] blendShapes = new MB3_MeshCombinerSingle.MBBlendShape[0];

		[SerializeField]
		internal MB3_MeshCombinerSingle.BufferDataFromPreviousBake bufferDataFromPrevious;

		[SerializeField]
		private MB3_MeshCombinerSingle.MeshCreationConditions _meshBirth;

		[SerializeField]
		private Mesh _mesh;

		internal MB3_MeshCombinerSingle.IVertexAndTriangleProcessor _vertexAndTriProcessor;

		protected MB_IMeshCombinerSingle_BoneProcessor _boneProcessor;

		internal MB3_MeshCombinerSingle.MB_MeshCombinerSingle_BlendShapeProcessor _blendShapeProcessor;

		protected MB3_MeshCombinerSingle.IMeshChannelsCacheTaggingInterface _meshChannelsCache;

		private GameObject[] empty = new GameObject[0];

		private int[] emptyIDs = new int[0];

		internal class MB_MeshCombinerSingle_BlendShapeProcessor
		{
			protected void Dispose(bool disposing)
			{
				if (this._disposed)
				{
					return;
				}
				if (disposing)
				{
					this.combiner = null;
					this.nblendShapes = null;
				}
				this._disposed = true;
			}

			public void Dispose()
			{
				this.Dispose(true);
			}

			public MB_MeshCombinerSingle_BlendShapeProcessor(MB3_MeshCombinerSingle cm)
			{
				this.combiner = cm;
			}

			public static MB3_MeshCombinerSingle.MBBlendShape[] GetBlendShapes(Mesh m, GameObject gameObject, Dictionary<int, MB3_MeshCombinerSingle.MeshChannels> meshID2MeshChannels)
			{
				MB3_MeshCombinerSingle.MeshChannels meshChannels;
				if (!meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannels))
				{
					meshChannels = new MB3_MeshCombinerSingle.MeshChannels();
					meshID2MeshChannels.Add(m.GetInstanceID(), meshChannels);
				}
				if (meshChannels.blendShapes == null)
				{
					MB3_MeshCombinerSingle.MBBlendShape[] array = new MB3_MeshCombinerSingle.MBBlendShape[m.blendShapeCount];
					int vertexCount = m.vertexCount;
					for (int i = 0; i < array.Length; i++)
					{
						MB3_MeshCombinerSingle.MBBlendShape mbblendShape = array[i] = new MB3_MeshCombinerSingle.MBBlendShape();
						mbblendShape.frames = new MB3_MeshCombinerSingle.MBBlendShapeFrame[MBVersion.GetBlendShapeFrameCount(m, i)];
						mbblendShape.name = m.GetBlendShapeName(i);
						mbblendShape.indexInSource = i;
						mbblendShape.gameObject = gameObject;
						for (int j = 0; j < mbblendShape.frames.Length; j++)
						{
							MB3_MeshCombinerSingle.MBBlendShapeFrame mbblendShapeFrame = mbblendShape.frames[j] = new MB3_MeshCombinerSingle.MBBlendShapeFrame();
							mbblendShapeFrame.frameWeight = MBVersion.GetBlendShapeFrameWeight(m, i, j);
							mbblendShapeFrame.vertices = new Vector3[vertexCount];
							mbblendShapeFrame.normals = new Vector3[vertexCount];
							mbblendShapeFrame.tangents = new Vector3[vertexCount];
							MBVersion.GetBlendShapeFrameVertices(m, i, j, mbblendShapeFrame.vertices, mbblendShapeFrame.normals, mbblendShapeFrame.tangents);
						}
					}
					meshChannels.blendShapes = array;
					return meshChannels.blendShapes;
				}
				MB3_MeshCombinerSingle.MBBlendShape[] array2 = new MB3_MeshCombinerSingle.MBBlendShape[meshChannels.blendShapes.Length];
				for (int k = 0; k < array2.Length; k++)
				{
					array2[k] = new MB3_MeshCombinerSingle.MBBlendShape();
					array2[k].name = meshChannels.blendShapes[k].name;
					array2[k].indexInSource = meshChannels.blendShapes[k].indexInSource;
					array2[k].frames = meshChannels.blendShapes[k].frames;
					array2[k].gameObject = gameObject;
				}
				return array2;
			}

			internal void ApplyBlendShapeFramesToMeshAndBuildMap(int newVertCount)
			{
				Renderer targetRenderer = this.combiner._targetRenderer;
				Mesh mesh = this.combiner._mesh;
				if (this.combiner.blendShapes.Length != this.nblendShapes.Length)
				{
					this.combiner.blendShapes = new MB3_MeshCombinerSingle.MBBlendShape[this.nblendShapes.Length];
				}
				Vector3[] array = new Vector3[newVertCount];
				Vector3[] array2 = new Vector3[newVertCount];
				Vector3[] array3 = new Vector3[newVertCount];
				((SkinnedMeshRenderer)targetRenderer).sharedMesh = null;
				MBVersion.ClearBlendShapes(mesh);
				for (int i = 0; i < this.nblendShapes.Length; i++)
				{
					MB3_MeshCombinerSingle.MBBlendShape mbblendShape = this.nblendShapes[i];
					MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject = this.combiner.instance2Combined_MapGet(mbblendShape.gameObject);
					if (mb_DynamicGameObject != null)
					{
						int vertIdx = mb_DynamicGameObject.vertIdx;
						for (int j = 0; j < mbblendShape.frames.Length; j++)
						{
							MB3_MeshCombinerSingle.MBBlendShapeFrame mbblendShapeFrame = mbblendShape.frames[j];
							Array.Copy(mbblendShapeFrame.vertices, 0, array, vertIdx, mbblendShapeFrame.vertices.Length);
							Array.Copy(mbblendShapeFrame.normals, 0, array2, vertIdx, mbblendShapeFrame.normals.Length);
							Array.Copy(mbblendShapeFrame.tangents, 0, array3, vertIdx, mbblendShapeFrame.tangents.Length);
							MBVersion.AddBlendShapeFrame(mesh, MB3_MeshCombinerSingle.MB_MeshCombinerSingle_BlendShapeProcessor._ConvertBlendShapeNameToOutputName(mbblendShape.name) + mbblendShape.gameObject.GetInstanceID().ToString(), mbblendShapeFrame.frameWeight, array, array2, array3);
							MB3_MeshCombinerSingle.MB_MeshCombinerSingle_BlendShapeProcessor._ZeroArray(array, vertIdx, mbblendShapeFrame.vertices.Length);
							MB3_MeshCombinerSingle.MB_MeshCombinerSingle_BlendShapeProcessor._ZeroArray(array2, vertIdx, mbblendShapeFrame.normals.Length);
							MB3_MeshCombinerSingle.MB_MeshCombinerSingle_BlendShapeProcessor._ZeroArray(array3, vertIdx, mbblendShapeFrame.tangents.Length);
						}
					}
					else
					{
						Debug.LogError("InstanceID in blend shape that was not in instance2combinedMap");
					}
					this.combiner.blendShapes[i] = mbblendShape;
				}
				((SkinnedMeshRenderer)targetRenderer).sharedMesh = null;
				((SkinnedMeshRenderer)targetRenderer).sharedMesh = mesh;
				if (this.combiner.settings.doBlendShapes)
				{
					MB_BlendShape2CombinedMap mb_BlendShape2CombinedMap = targetRenderer.GetComponent<MB_BlendShape2CombinedMap>();
					if (mb_BlendShape2CombinedMap == null)
					{
						mb_BlendShape2CombinedMap = targetRenderer.gameObject.AddComponent<MB_BlendShape2CombinedMap>();
					}
					SerializableSourceBlendShape2Combined map = mb_BlendShape2CombinedMap.GetMap();
					MB3_MeshCombinerSingle.MB_MeshCombinerSingle_BlendShapeProcessor._BuildSrcShape2CombinedMap(this.combiner, map, this.nblendShapes);
				}
			}

			public void AllocateBlendShapeArrayIfNecessary(int nBlendShapeSize)
			{
				if (this.combiner.settings.doBlendShapes)
				{
					this.nblendShapes = new MB3_MeshCombinerSingle.MBBlendShape[nBlendShapeSize];
				}
			}

			public void AssignNewBlendShapesToCombinerIfNecessary()
			{
				if (this.combiner.settings.doBlendShapes)
				{
					this.combiner.blendShapes = this.nblendShapes;
				}
			}

			public void CopyBlendShapesInCurrentMeshIfNecessary(ref int targBlendShapeIdx, MB3_MeshCombinerSingle.MB_DynamicGameObject dgo)
			{
				if (this.combiner.settings.doBlendShapes)
				{
					Array.Copy(this.combiner.blendShapes, dgo.blendShapeIdx, this.nblendShapes, targBlendShapeIdx, dgo.numBlendShapes);
					dgo.blendShapeIdx = targBlendShapeIdx;
					targBlendShapeIdx += dgo.numBlendShapes;
				}
			}

			public void CopyBlendShapesForNewMeshIfNecessary(ref int targBlendShapeIdx, MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, Mesh mesh, MB3_MeshCombinerSingle.IMeshChannelsCacheTaggingInterface meshChannelCache)
			{
				if (this.combiner.settings.doBlendShapes)
				{
					int index = targBlendShapeIdx;
					MB3_MeshCombinerSingle.MBBlendShape[] blendShapes = meshChannelCache.GetBlendShapes(mesh, dgo.gameObject.GetInstanceID(), dgo.gameObject);
					blendShapes.CopyTo(this.nblendShapes, index);
					dgo.blendShapeIdx = targBlendShapeIdx;
					targBlendShapeIdx += blendShapes.Length;
				}
			}

			private static string _ConvertBlendShapeNameToOutputName(string bs)
			{
				string[] array = bs.Split('.', StringSplitOptions.None);
				return array[array.Length - 1];
			}

			internal void ApplyBlendShapeFramesToMeshAndBuildMap_MergeBlendShapesWithTheSameName(int newVertCount)
			{
				Renderer targetRenderer = this.combiner._targetRenderer;
				Mesh mesh = this.combiner._mesh;
				Vector3[] array = new Vector3[newVertCount];
				Vector3[] array2 = new Vector3[newVertCount];
				Vector3[] array3 = new Vector3[newVertCount];
				MBVersion.ClearBlendShapes(mesh);
				bool flag = false;
				Dictionary<string, List<MB3_MeshCombinerSingle.MBBlendShape>> dictionary = new Dictionary<string, List<MB3_MeshCombinerSingle.MBBlendShape>>();
				for (int i = 0; i < this.nblendShapes.Length; i++)
				{
					MB3_MeshCombinerSingle.MBBlendShape mbblendShape = this.nblendShapes[i];
					string key = MB3_MeshCombinerSingle.MB_MeshCombinerSingle_BlendShapeProcessor._ConvertBlendShapeNameToOutputName(mbblendShape.name);
					List<MB3_MeshCombinerSingle.MBBlendShape> list;
					if (!dictionary.TryGetValue(key, out list))
					{
						list = new List<MB3_MeshCombinerSingle.MBBlendShape>();
						dictionary.Add(key, list);
					}
					list.Add(mbblendShape);
					if (list.Count > 1 && list[0].frames.Length != mbblendShape.frames.Length)
					{
						Debug.LogError("BlendShapes with the same name must have the same number of frames.");
						flag = true;
					}
				}
				if (flag)
				{
					return;
				}
				if (this.combiner.blendShapes.Length != this.nblendShapes.Length)
				{
					this.combiner.blendShapes = new MB3_MeshCombinerSingle.MBBlendShape[dictionary.Keys.Count];
				}
				int num = 0;
				foreach (string text in dictionary.Keys)
				{
					List<MB3_MeshCombinerSingle.MBBlendShape> list2 = dictionary[text];
					MB3_MeshCombinerSingle.MBBlendShape mbblendShape2 = list2[0];
					int num2 = mbblendShape2.frames.Length;
					int num3 = 0;
					int num4 = 0;
					string text2 = "";
					for (int j = 0; j < num2; j++)
					{
						float frameWeight = mbblendShape2.frames[j].frameWeight;
						for (int k = 0; k < list2.Count; k++)
						{
							MB3_MeshCombinerSingle.MBBlendShape mbblendShape3 = list2[k];
							int vertIdx = this.combiner.instance2Combined_MapGet(mbblendShape3.gameObject).vertIdx;
							MB3_MeshCombinerSingle.MBBlendShapeFrame mbblendShapeFrame = mbblendShape3.frames[j];
							Array.Copy(mbblendShapeFrame.vertices, 0, array, vertIdx, mbblendShapeFrame.vertices.Length);
							Array.Copy(mbblendShapeFrame.normals, 0, array2, vertIdx, mbblendShapeFrame.normals.Length);
							Array.Copy(mbblendShapeFrame.tangents, 0, array3, vertIdx, mbblendShapeFrame.tangents.Length);
							if (j == 0)
							{
								num3 += mbblendShapeFrame.vertices.Length;
								text2 = string.Concat(new string[]
								{
									text2,
									mbblendShape3.gameObject.name,
									" ",
									vertIdx.ToString(),
									":",
									(vertIdx + mbblendShapeFrame.vertices.Length).ToString(),
									", "
								});
							}
						}
						num4 += list2.Count;
						MBVersion.AddBlendShapeFrame(mesh, text, frameWeight, array, array2, array3);
						MB3_MeshCombinerSingle.MB_MeshCombinerSingle_BlendShapeProcessor._ZeroArray(array, 0, array.Length);
						MB3_MeshCombinerSingle.MB_MeshCombinerSingle_BlendShapeProcessor._ZeroArray(array2, 0, array2.Length);
						MB3_MeshCombinerSingle.MB_MeshCombinerSingle_BlendShapeProcessor._ZeroArray(array3, 0, array3.Length);
					}
					this.combiner.blendShapes[num] = mbblendShape2;
					num++;
				}
				((SkinnedMeshRenderer)targetRenderer).sharedMesh = null;
				((SkinnedMeshRenderer)targetRenderer).sharedMesh = mesh;
				if (this.combiner.settings.doBlendShapes)
				{
					MB_BlendShape2CombinedMap mb_BlendShape2CombinedMap = targetRenderer.GetComponent<MB_BlendShape2CombinedMap>();
					if (mb_BlendShape2CombinedMap == null)
					{
						mb_BlendShape2CombinedMap = targetRenderer.gameObject.AddComponent<MB_BlendShape2CombinedMap>();
					}
					SerializableSourceBlendShape2Combined map = mb_BlendShape2CombinedMap.GetMap();
					MB3_MeshCombinerSingle.MB_MeshCombinerSingle_BlendShapeProcessor._BuildSrcShape2CombinedMap(this.combiner, map, this.combiner.blendShapes);
				}
			}

			private static void _BuildSrcShape2CombinedMap(MB3_MeshCombinerSingle combiner, SerializableSourceBlendShape2Combined map, MB3_MeshCombinerSingle.MBBlendShape[] bs)
			{
				MB3_MeshCombinerSingle.MBBlendShape[] blendShapes = combiner.blendShapes;
				Renderer targetRenderer = combiner._targetRenderer;
				if (combiner._mesh != null && combiner._mesh.blendShapeCount != combiner.blendShapes.Length)
				{
					Debug.LogError("Blend shapes in combiner did not match blend shapes in mesh. Map will probably be invalid.");
				}
				GameObject[] array = new GameObject[bs.Length];
				int[] array2 = new int[bs.Length];
				GameObject[] array3 = new GameObject[bs.Length];
				int[] array4 = new int[bs.Length];
				for (int i = 0; i < blendShapes.Length; i++)
				{
					array[i] = blendShapes[i].gameObject;
					array2[i] = blendShapes[i].indexInSource;
					array3[i] = targetRenderer.gameObject;
					array4[i] = i;
				}
				map.SetBuffers(array, array2, array3, array4);
			}

			private static void _ZeroArray(Vector3[] arr, int idx, int length)
			{
				int num = idx + length;
				for (int i = idx; i < num; i++)
				{
					arr[i] = Vector3.zero;
				}
			}

			private MB3_MeshCombinerSingle combiner;

			private MB3_MeshCombinerSingle.MBBlendShape[] nblendShapes;

			private bool _disposed;
		}

		public class MB_MeshCombinerSingle_BoneProcessor : MB_IMeshCombinerSingle_BoneProcessor, IDisposable
		{
			protected void Dispose(bool disposing)
			{
				if (this._disposed)
				{
					return;
				}
				if (disposing)
				{
					this.combiner = null;
					this.boneIdx2dgoMap = null;
					this.boneIdxsToDelete = null;
					this.bonesToAdd = null;
					this.boneAndBindPose2idx = null;
					this.boneWeights = null;
				}
				this._disposed = true;
			}

			public void Dispose()
			{
				this.Dispose(true);
			}

			public int GetNewBonesSize()
			{
				if (this.nbones != null)
				{
					return this.nbones.Length;
				}
				return 0;
			}

			public MB_MeshCombinerSingle_BoneProcessor(MB3_MeshCombinerSingle cm)
			{
				this.combiner = cm;
				this.oldBonesPreviousBake = this.combiner.bones;
				this.oldBindPosesPreviousBake = this.combiner.bindPoses;
			}

			public HashSet<MB3_MeshCombinerSingle.BoneAndBindpose> GetBonesToAdd()
			{
				return this.bonesToAdd;
			}

			public int GetNumBonesToDelete()
			{
				return this.boneIdxsToDelete.Count;
			}

			public void BuildBoneIdx2DGOMapIfNecessary(int[] _goToDelete)
			{
				this._didSetup = false;
				if (this.combiner.settings.renderType == MB_RenderType.skinnedMeshRenderer)
				{
					if (_goToDelete != null && _goToDelete.Length != 0)
					{
						this.boneIdx2dgoMap = this._buildBoneIdx2dgoMap();
					}
					for (int i = 0; i < this.oldBonesPreviousBake.Length; i++)
					{
						MB3_MeshCombinerSingle.BoneAndBindpose key = new MB3_MeshCombinerSingle.BoneAndBindpose(this.oldBonesPreviousBake[i], this.oldBindPosesPreviousBake[i]);
						this.boneAndBindPose2idx.Add(key, i);
					}
					this._didSetup = true;
				}
			}

			public void RemoveBonesForDgosWeAreDeleting(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo)
			{
				for (int i = 0; i < dgo.indexesOfBonesUsed.Length; i++)
				{
					int num = dgo.indexesOfBonesUsed[i];
					List<MB3_MeshCombinerSingle.MB_DynamicGameObject> list = this.boneIdx2dgoMap[num];
					if (list.Contains(dgo))
					{
						list.Remove(dgo);
						if (list.Count == 0)
						{
							this.boneIdxsToDelete.Add(num);
						}
					}
				}
			}

			public void AllocateAndSetupSMRDataStructures(List<MB3_MeshCombinerSingle.MB_DynamicGameObject> toAddDGOs, List<MB3_MeshCombinerSingle.MB_DynamicGameObject> mbDynamicObjectsInCombinedMesh, int newVertSize, MB3_MeshCombinerSingle.IVertexAndTriangleProcessor vertexAndTriangleProcessor)
			{
				if (this.combiner.settings.renderType == MB_RenderType.skinnedMeshRenderer)
				{
					this._CollectSkinningDataForDGOsInCombinedMesh(toAddDGOs);
					int newBonesLength = this.GetNewBonesLength();
					this.nbones = new Transform[newBonesLength];
					this.nbindPoses = new Matrix4x4[newBonesLength];
					this.nboneWeights = new BoneWeight[newVertSize];
					this._newBonesStartAtIdx = this.oldBindPosesPreviousBake.Length - this.GetNumBonesToDelete();
					this.boneWeights = this.combiner._mesh.boneWeights;
				}
			}

			public void UpdateGameObjects_ReadBoneWeightInfoFromCombinedMesh()
			{
				if (this.combiner.settings.renderType == MB_RenderType.skinnedMeshRenderer)
				{
					this.boneWeights = this.combiner._mesh.boneWeights;
					if (this.combiner.mbDynamicObjectsInCombinedMesh.Count > 0 && this.combiner.mbDynamicObjectsInCombinedMesh[0].indexesOfBonesUsed.Length == 0 && this.combiner.settings.renderType == MB_RenderType.skinnedMeshRenderer && this.boneWeights != null && this.boneWeights.Length != 0)
					{
						for (int i = 0; i < this.combiner.mbDynamicObjectsInCombinedMesh.Count; i++)
						{
							MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject = this.combiner.mbDynamicObjectsInCombinedMesh[i];
							HashSet<int> hashSet = new HashSet<int>();
							for (int j = mb_DynamicGameObject.vertIdx; j < mb_DynamicGameObject.vertIdx + mb_DynamicGameObject.numVerts; j++)
							{
								if (this.boneWeights[j].weight0 > 0f)
								{
									hashSet.Add(this.boneWeights[j].boneIndex0);
								}
								if (this.boneWeights[j].weight1 > 0f)
								{
									hashSet.Add(this.boneWeights[j].boneIndex1);
								}
								if (this.boneWeights[j].weight2 > 0f)
								{
									hashSet.Add(this.boneWeights[j].boneIndex2);
								}
								if (this.boneWeights[j].weight3 > 0f)
								{
									hashSet.Add(this.boneWeights[j].boneIndex3);
								}
							}
							mb_DynamicGameObject.indexesOfBonesUsed = new int[hashSet.Count];
							hashSet.CopyTo(mb_DynamicGameObject.indexesOfBonesUsed);
						}
						if (this.combiner.LOG_LEVEL >= MB2_LogLevel.debug)
						{
							Debug.Log("Baker used old systems that duplicated bones. Upgrading to new system by building indexesOfBonesUsed");
						}
					}
				}
			}

			public int GetNewBonesLength()
			{
				if (this.combiner.settings.renderType == MB_RenderType.skinnedMeshRenderer)
				{
					return this.oldBindPosesPreviousBake.Length + this.bonesToAdd.Count - this.boneIdxsToDelete.Count;
				}
				return 0;
			}

			internal void _CollectSkinningDataForDGOsInCombinedMesh(List<MB3_MeshCombinerSingle.MB_DynamicGameObject> objsToAdd)
			{
				for (int i = 0; i < objsToAdd.Count; i++)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject = objsToAdd[i];
					this.CollectBonesToAddForDGO(mb_DynamicGameObject, MB_Utility.GetRenderer(mb_DynamicGameObject.gameObject), this.combiner.settings.smrNoExtraBonesWhenCombiningMeshRenderers);
				}
			}

			public bool CollectBonesToAddForDGO(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, Renderer r, bool noExtraBonesForMeshRenderers)
			{
				bool flag = true;
				MB3_MeshCombinerSingle.MeshChannelsCache meshChannelsCache = (MB3_MeshCombinerSingle.MeshChannelsCache)this.combiner._meshChannelsCache;
				List<Matrix4x4> list = dgo._tmpSMR_CachedBindposes = meshChannelsCache.GetBindposes(r, out dgo.isSkinnedMeshWithBones);
				dgo._tmpSMR_CachedBoneWeights = meshChannelsCache.GetBoneWeights(r, dgo.numVerts, dgo.isSkinnedMeshWithBones);
				Transform[] array = dgo._tmpSMR_CachedBones = this.combiner._getBones(r, dgo.isSkinnedMeshWithBones);
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == null)
					{
						Debug.LogError("Source mesh r had a 'null' bone. Bones must not be null: " + ((r != null) ? r.ToString() : null));
						flag = false;
					}
				}
				if (!flag)
				{
					return flag;
				}
				if (noExtraBonesForMeshRenderers && MB_Utility.GetRenderer(dgo.gameObject) is MeshRenderer)
				{
					bool flag2 = false;
					MB3_MeshCombinerSingle.BoneAndBindpose boneAndBindpose = default(MB3_MeshCombinerSingle.BoneAndBindpose);
					Transform parent = dgo.gameObject.transform.parent;
					while (parent != null)
					{
						foreach (MB3_MeshCombinerSingle.BoneAndBindpose boneAndBindpose2 in this.boneAndBindPose2idx.Keys)
						{
							if (boneAndBindpose2.bone == parent)
							{
								boneAndBindpose = boneAndBindpose2;
								flag2 = true;
								break;
							}
						}
						foreach (MB3_MeshCombinerSingle.BoneAndBindpose boneAndBindpose3 in this.bonesToAdd)
						{
							if (boneAndBindpose3.bone == parent)
							{
								boneAndBindpose = boneAndBindpose3;
								flag2 = true;
								break;
							}
						}
						if (flag2)
						{
							break;
						}
						parent = parent.parent;
					}
					if (flag2)
					{
						array[0] = boneAndBindpose.bone;
						list[0] = boneAndBindpose.bindPose;
					}
				}
				int[] array2 = new int[array.Length];
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j] = j;
				}
				for (int k = 0; k < array.Length; k++)
				{
					bool flag3 = false;
					int num = array2[k];
					MB3_MeshCombinerSingle.BoneAndBindpose boneAndBindpose4 = new MB3_MeshCombinerSingle.BoneAndBindpose(array[num], list[num]);
					int num2;
					if (this.boneAndBindPose2idx.TryGetValue(boneAndBindpose4, out num2) && array[num] == this.oldBonesPreviousBake[num2] && !this.boneIdxsToDelete.Contains(num2) && list[num] == this.oldBindPosesPreviousBake[num2])
					{
						flag3 = true;
					}
					if (!flag3 && !this.bonesToAdd.Contains(boneAndBindpose4))
					{
						this.bonesToAdd.Add(boneAndBindpose4);
					}
				}
				dgo._tmpSMR_srcMeshBoneIdx2masterListBoneIdx = array2;
				return flag;
			}

			private List<MB3_MeshCombinerSingle.MB_DynamicGameObject>[] _buildBoneIdx2dgoMap()
			{
				List<MB3_MeshCombinerSingle.MB_DynamicGameObject>[] array = new List<MB3_MeshCombinerSingle.MB_DynamicGameObject>[this.oldBonesPreviousBake.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = new List<MB3_MeshCombinerSingle.MB_DynamicGameObject>();
				}
				for (int j = 0; j < this.combiner.mbDynamicObjectsInCombinedMesh.Count; j++)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject = this.combiner.mbDynamicObjectsInCombinedMesh[j];
					for (int k = 0; k < mb_DynamicGameObject.indexesOfBonesUsed.Length; k++)
					{
						array[mb_DynamicGameObject.indexesOfBonesUsed[k]].Add(mb_DynamicGameObject);
					}
				}
				return array;
			}

			public void CopyBonesWeAreKeepingToNewBonesArrayAndAdjustBWIndexes(int totalDeleteVerts)
			{
				if (this.boneIdxsToDelete.Count > 0)
				{
					int[] array = new int[this.boneIdxsToDelete.Count];
					this.boneIdxsToDelete.CopyTo(array);
					Array.Sort<int>(array);
					int[] array2 = new int[this.oldBonesPreviousBake.Length];
					int num = 0;
					int num2 = 0;
					for (int i = 0; i < this.oldBonesPreviousBake.Length; i++)
					{
						if (num2 < array.Length && array[num2] == i)
						{
							num2++;
							array2[i] = -1;
						}
						else
						{
							array2[i] = num;
							this.nbones[num] = this.oldBonesPreviousBake[i];
							this.nbindPoses[num] = this.oldBindPosesPreviousBake[i];
							num++;
						}
					}
					int num3 = this.boneWeights.Length - totalDeleteVerts;
					for (int j = 0; j < num3; j++)
					{
						BoneWeight boneWeight = this.nboneWeights[j];
						boneWeight.boneIndex0 = array2[boneWeight.boneIndex0];
						boneWeight.boneIndex1 = array2[boneWeight.boneIndex1];
						boneWeight.boneIndex2 = array2[boneWeight.boneIndex2];
						boneWeight.boneIndex3 = array2[boneWeight.boneIndex3];
						this.nboneWeights[j] = boneWeight;
					}
					for (int k = 0; k < this.combiner.mbDynamicObjectsInCombinedMesh.Count; k++)
					{
						MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject = this.combiner.mbDynamicObjectsInCombinedMesh[k];
						for (int l = 0; l < mb_DynamicGameObject.indexesOfBonesUsed.Length; l++)
						{
							mb_DynamicGameObject.indexesOfBonesUsed[l] = array2[mb_DynamicGameObject.indexesOfBonesUsed[l]];
						}
					}
					return;
				}
				Array.Copy(this.oldBonesPreviousBake, this.nbones, this.oldBonesPreviousBake.Length);
				Array.Copy(this.oldBindPosesPreviousBake, this.nbindPoses, this.oldBindPosesPreviousBake.Length);
			}

			public void InsertNewBonesIntoBonesArray()
			{
				if (this.combiner.settings.renderType == MB_RenderType.skinnedMeshRenderer)
				{
					this.boneWeights = this.nboneWeights;
					this.combiner.bindPoses = this.nbindPoses;
					this.combiner.bones = this.nbones;
					int num = 0;
					foreach (MB3_MeshCombinerSingle.BoneAndBindpose boneAndBindpose in this.GetBonesToAdd())
					{
						int num2 = this._newBonesStartAtIdx + num;
						this.nbones[num2] = boneAndBindpose.bone;
						this.nbindPoses[num2] = boneAndBindpose.bindPose;
						num++;
					}
				}
			}

			public void AddBonesToNewBonesArrayAndAdjustBWIndexes1(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, int vertsIdx)
			{
				Transform[] tmpSMR_CachedBones = dgo._tmpSMR_CachedBones;
				List<Matrix4x4> tmpSMR_CachedBindposes = dgo._tmpSMR_CachedBindposes;
				BoneWeight[] tmpSMR_CachedBoneWeights = dgo._tmpSMR_CachedBoneWeights;
				int[] array = new int[tmpSMR_CachedBones.Length];
				for (int i = 0; i < dgo._tmpSMR_srcMeshBoneIdx2masterListBoneIdx.Length; i++)
				{
					for (int j = 0; j < this.nbones.Length; j++)
					{
						if (tmpSMR_CachedBones[i] == this.nbones[j] && tmpSMR_CachedBindposes[i] == this.nbindPoses[j])
						{
							array[i] = j;
							break;
						}
					}
				}
				for (int k = 0; k < tmpSMR_CachedBoneWeights.Length; k++)
				{
					int num = vertsIdx + k;
					this.nboneWeights[num].boneIndex0 = array[tmpSMR_CachedBoneWeights[k].boneIndex0];
					this.nboneWeights[num].boneIndex1 = array[tmpSMR_CachedBoneWeights[k].boneIndex1];
					this.nboneWeights[num].boneIndex2 = array[tmpSMR_CachedBoneWeights[k].boneIndex2];
					this.nboneWeights[num].boneIndex3 = array[tmpSMR_CachedBoneWeights[k].boneIndex3];
					this.nboneWeights[num].weight0 = tmpSMR_CachedBoneWeights[k].weight0;
					this.nboneWeights[num].weight1 = tmpSMR_CachedBoneWeights[k].weight1;
					this.nboneWeights[num].weight2 = tmpSMR_CachedBoneWeights[k].weight2;
					this.nboneWeights[num].weight3 = tmpSMR_CachedBoneWeights[k].weight3;
				}
				for (int l = 0; l < dgo._tmpSMR_srcMeshBoneIdx2masterListBoneIdx.Length; l++)
				{
					dgo._tmpSMR_srcMeshBoneIdx2masterListBoneIdx[l] = array[dgo._tmpSMR_srcMeshBoneIdx2masterListBoneIdx[l]];
				}
				dgo.indexesOfBonesUsed = dgo._tmpSMR_srcMeshBoneIdx2masterListBoneIdx;
				dgo._tmpSMR_srcMeshBoneIdx2masterListBoneIdx = null;
				dgo._tmpSMR_CachedBones = null;
				dgo._tmpSMR_CachedBindposes = null;
				dgo._tmpSMR_CachedBoneWeights = null;
			}

			public void UpdateGameObjects_UpdateBWIndexes(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo)
			{
				Transform[] bones = MBVersion.GetBones(dgo._renderer, dgo.isSkinnedMeshWithBones);
				BoneWeight[] array = ((MB3_MeshCombinerSingle.MeshChannelsCache)this.combiner._meshChannelsCache).GetBoneWeights(dgo._renderer, dgo.numVerts, dgo.isSkinnedMeshWithBones);
				int num = dgo.vertIdx;
				bool flag = false;
				for (int i = 0; i < array.Length; i++)
				{
					if (bones[array[i].boneIndex0] != this.oldBonesPreviousBake[this.boneWeights[num].boneIndex0])
					{
						flag = true;
						break;
					}
					this.boneWeights[num].weight0 = array[i].weight0;
					this.boneWeights[num].weight1 = array[i].weight1;
					this.boneWeights[num].weight2 = array[i].weight2;
					this.boneWeights[num].weight3 = array[i].weight3;
					num++;
				}
				if (flag)
				{
					Debug.LogError("Detected that some of the boneweights reference different bones than when initial added. Boneweights must reference the same bones " + dgo.name);
				}
			}

			public void CopyVertsNormsTansToBuffers(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, MB_IMeshBakerSettings settings, int vertsIdx, NativeSlice<Vector3> nnorms, NativeSlice<Vector4> ntangs, NativeSlice<Vector3> nverts, NativeSlice<Vector3> normals, NativeSlice<Vector4> tangents, NativeSlice<Vector3> verts)
			{
				Debug.LogError("The simple bone processor doesn't use this.");
			}

			public void CopyVertsNormsTansToBuffers(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, MB_IMeshBakerSettings settings, int vertsIdx, Vector3[] nnorms, Vector4[] ntangs, Vector3[] nverts, Vector3[] normals, Vector4[] tangents, Vector3[] verts)
			{
				bool flag = dgo._renderer is MeshRenderer;
				if (settings.smrNoExtraBonesWhenCombiningMeshRenderers && flag && dgo._tmpSMR_CachedBones[0] != dgo.gameObject.transform)
				{
					Matrix4x4 matrix4x = dgo._tmpSMR_CachedBindposes[0].inverse * dgo._tmpSMR_CachedBones[0].worldToLocalMatrix * dgo.gameObject.transform.localToWorldMatrix;
					Matrix4x4 matrix4x2 = matrix4x;
					matrix4x2[0, 3] = (matrix4x2[1, 3] = (matrix4x2[2, 3] = 0f));
					matrix4x2 = matrix4x2.inverse.transpose;
					for (int i = 0; i < dgo._mesh.vertexCount; i++)
					{
						int num = vertsIdx + i;
						if (verts != null)
						{
							verts[vertsIdx + i] = matrix4x.MultiplyPoint3x4(nverts[i]);
						}
						if (settings.doNorm && nnorms != null)
						{
							normals[num] = matrix4x2.MultiplyPoint3x4(nnorms[i]).normalized;
						}
						if (settings.doTan && ntangs != null)
						{
							float w = ntangs[i].w;
							tangents[num] = matrix4x2.MultiplyPoint3x4(ntangs[i]).normalized;
							tangents[num].w = w;
						}
					}
					return;
				}
				if (settings.doNorm && nnorms != null)
				{
					nnorms.CopyTo(normals, vertsIdx);
				}
				if (settings.doTan && ntangs != null)
				{
					ntangs.CopyTo(tangents, vertsIdx);
				}
				if (verts != null)
				{
					nverts.CopyTo(verts, vertsIdx);
				}
			}

			public void DisposeOfTemporarySMRData()
			{
				if (this.boneIdxsToDelete != null)
				{
					this.boneIdxsToDelete.Clear();
				}
				if (this.boneAndBindPose2idx != null)
				{
					this.boneAndBindPose2idx.Clear();
				}
				this.boneIdxsToDelete = null;
				this.boneAndBindPose2idx = null;
				this.boneIdx2dgoMap = null;
			}

			public void CopyBoneWeightsFromMeshForDGOsInCombined(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, int targVidx)
			{
				Array.Copy(this.boneWeights, dgo.vertIdx, this.nboneWeights, targVidx, dgo.numVerts);
			}

			public void ApplySMRdataToMeshToBuffer()
			{
			}

			public void ApplySMRdataToMesh(MB3_MeshCombinerSingle combiner, Mesh mesh)
			{
				mesh.bindposes = combiner.bindPoses;
				mesh.boneWeights = this.boneWeights;
			}

			public bool GetCachedSMRMeshData(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo)
			{
				return true;
			}

			public bool DB_CheckIntegrity()
			{
				if (this.combiner.settings.renderType == MB_RenderType.skinnedMeshRenderer)
				{
					for (int i = 0; i < this.combiner.mbDynamicObjectsInCombinedMesh.Count; i++)
					{
						MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject = this.combiner.mbDynamicObjectsInCombinedMesh[i];
						HashSet<int> hashSet = new HashSet<int>();
						HashSet<int> hashSet2 = new HashSet<int>();
						for (int j = mb_DynamicGameObject.vertIdx; j < mb_DynamicGameObject.vertIdx + mb_DynamicGameObject.numVerts; j++)
						{
							hashSet.Add(this.boneWeights[j].boneIndex0);
							hashSet.Add(this.boneWeights[j].boneIndex1);
							hashSet.Add(this.boneWeights[j].boneIndex2);
							hashSet.Add(this.boneWeights[j].boneIndex3);
						}
						for (int k = 0; k < mb_DynamicGameObject.indexesOfBonesUsed.Length; k++)
						{
							hashSet2.Add(mb_DynamicGameObject.indexesOfBonesUsed[k]);
						}
						hashSet2.ExceptWith(hashSet);
						if (hashSet2.Count > 0)
						{
							Debug.LogError("The bone indexes were not the same. " + hashSet.Count.ToString() + " " + hashSet2.Count.ToString());
						}
						for (int l = 0; l < mb_DynamicGameObject.indexesOfBonesUsed.Length; l++)
						{
							if (l < 0 || l > this.oldBonesPreviousBake.Length)
							{
								Debug.LogError("Bone index was out of bounds.");
							}
						}
						if (mb_DynamicGameObject.indexesOfBonesUsed.Length < 1)
						{
							Debug.Log("DGO had no bones");
						}
					}
				}
				return true;
			}

			private MB3_MeshCombinerSingle combiner;

			private List<MB3_MeshCombinerSingle.MB_DynamicGameObject>[] boneIdx2dgoMap;

			private HashSet<int> boneIdxsToDelete = new HashSet<int>();

			private HashSet<MB3_MeshCombinerSingle.BoneAndBindpose> bonesToAdd = new HashSet<MB3_MeshCombinerSingle.BoneAndBindpose>();

			private Dictionary<MB3_MeshCombinerSingle.BoneAndBindpose, int> boneAndBindPose2idx = new Dictionary<MB3_MeshCombinerSingle.BoneAndBindpose, int>();

			private Transform[] oldBonesPreviousBake;

			private Matrix4x4[] oldBindPosesPreviousBake;

			private Transform[] nbones;

			private Matrix4x4[] nbindPoses;

			private BoneWeight[] nboneWeights;

			private BoneWeight[] boneWeights = new BoneWeight[0];

			private int _newBonesStartAtIdx;

			private bool _disposed;

			private bool _didSetup;
		}

		public class MB_MeshCombinerSingle_BoneProcessorNewAPI : MB_IMeshCombinerSingle_BoneProcessor, IDisposable
		{
			public MB_MeshCombinerSingle_BoneProcessorNewAPI(MB3_MeshCombinerSingle cm)
			{
				this.targBoneWeightIdx = 0;
				this.boneWeightSize = 0;
				this.combiner = cm;
				this.LOG_LEVEL = cm.LOG_LEVEL;
			}

			public int GetNewBonesSize()
			{
				return this.masterList.Count;
			}

			public void BuildBoneIdx2DGOMapIfNecessary(int[] _goToDelete)
			{
				this._initialized = false;
				this.masterList.Clear();
				if (this.combiner.settings.renderType == MB_RenderType.skinnedMeshRenderer)
				{
					this._initialized = true;
				}
			}

			public void RemoveBonesForDgosWeAreDeleting(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo)
			{
			}

			public bool GetCachedSMRMeshData(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo)
			{
				bool result = true;
				Renderer renderer = dgo._renderer;
				MB3_MeshCombinerSingle.MeshChannelsCache_NativeArray meshChannelsCache_NativeArray = (MB3_MeshCombinerSingle.MeshChannelsCache_NativeArray)this.combiner._meshChannelsCache;
				dgo._tmpSMR_CachedBindposes = meshChannelsCache_NativeArray.GetBindposes(renderer, out dgo.isSkinnedMeshWithBones);
				int count = dgo._tmpSMR_CachedBindposes.Count;
				dgo._tmpSMR_CachedBoneWeightData = meshChannelsCache_NativeArray.GetBoneWeightData(renderer, count, dgo.isSkinnedMeshWithBones);
				dgo.numBoneWeights = dgo._tmpSMR_CachedBoneWeightData.boneWeights.Length;
				Transform[] array = dgo._tmpSMR_CachedBones = this.combiner._getBones(renderer, dgo.isSkinnedMeshWithBones);
				if (array.Length > count)
				{
					Array.Resize<Transform>(ref dgo._tmpSMR_CachedBones, count);
					array = dgo._tmpSMR_CachedBones;
				}
				if (array.Length < count)
				{
					Debug.LogWarning(string.Concat(new string[]
					{
						dgo.name,
						" SkinnedMeshRenderer had fewer bones than mesh had bindposes. Mesh may not deform properly: ",
						array.Length.ToString(),
						"  ",
						count.ToString()
					}));
				}
				dgo._tmpSMR_CachedBoneAndBindPose = new MB3_MeshCombinerSingle.BoneAndBindpose[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == null)
					{
						string str = "Source mesh r had a 'null' bone. Bones must not be null: ";
						Renderer renderer2 = renderer;
						Debug.LogError(str + ((renderer2 != null) ? renderer2.ToString() : null));
						result = false;
					}
				}
				if (this.combiner.settings.smrNoExtraBonesWhenCombiningMeshRenderers)
				{
					for (int j = 0; j < array.Length; j++)
					{
						MB3_MeshCombinerSingle.BoneAndBindpose item = new MB3_MeshCombinerSingle.BoneAndBindpose(array[j], dgo._tmpSMR_CachedBindposes[j]);
						this.bonesToAddAndInCombined.Add(item);
					}
				}
				if (this.LOG_LEVEL >= MB2_LogLevel.trace)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.AppendLine("GetCachedSMRMeshData for : " + dgo.name);
					stringBuilder.AppendLine("   _tmpSMR_CachedBindposes: " + dgo._tmpSMR_CachedBindposes.Count.ToString());
					stringBuilder.AppendLine("   _tmpSMR_CachedBoneAndBindPose: " + dgo._tmpSMR_CachedBoneAndBindPose.Length.ToString());
					stringBuilder.AppendLine("   _tmpSMR_CachedBones: " + dgo._tmpSMR_CachedBones.Length.ToString());
					stringBuilder.AppendLine("   _tmpSMR_CachedBoneWeightData: " + dgo._tmpSMR_CachedBoneWeightData.boneWeights.Length.ToString());
					Debug.Log(stringBuilder.ToString());
				}
				return result;
			}

			public void AllocateAndSetupSMRDataStructures(List<MB3_MeshCombinerSingle.MB_DynamicGameObject> dgosToAdd, List<MB3_MeshCombinerSingle.MB_DynamicGameObject> dgosInCombinedMesh, int newVertSize, MB3_MeshCombinerSingle.IVertexAndTriangleProcessor vertexAndTriangleProcessor)
			{
				if (this.combiner.settings.renderType == MB_RenderType.skinnedMeshRenderer)
				{
					MB3_MeshCombinerSingle.MeshChannelsCache_NativeArray meshChannelsCache = (MB3_MeshCombinerSingle.MeshChannelsCache_NativeArray)this.combiner._meshChannelsCache;
					this._CollectSkinningDataForDGOsInCombinedMesh(dgosToAdd, dgosInCombinedMesh, meshChannelsCache);
					this._BuildMasterBonesArray(dgosToAdd, dgosInCombinedMesh);
					this._AllocateNewArraysForCombinedMesh(newVertSize, vertexAndTriangleProcessor);
				}
			}

			public void UpdateGameObjects_ReadBoneWeightInfoFromCombinedMesh()
			{
				if (this.combiner.settings.renderType == MB_RenderType.skinnedMeshRenderer)
				{
					if (this.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						Debug.Log("UpdateGameObjects_ReadBoneWeightInfoFromCombinedMesh");
					}
					NativeArray<BoneWeight1> allBoneWeights = this.combiner._mesh.GetAllBoneWeights();
					NativeArray<byte> bonesPerVertex = this.combiner._mesh.GetBonesPerVertex();
					this.boneWeight1s_nvarr = new NativeArray<BoneWeight1>(allBoneWeights, Allocator.Persistent);
					this.bonesPerVertex_nvarr = new NativeArray<byte>(bonesPerVertex, Allocator.Persistent);
					this.dgo2firstIdxInBoneWeightsArray.Clear();
					int num = 0;
					int num2 = 0;
					int num3 = 0;
					MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject = this.combiner.mbDynamicObjectsInCombinedMesh[num3];
					this.dgo2firstIdxInBoneWeightsArray[mb_DynamicGameObject] = 0;
					for (int i = 0; i < this.combiner._mesh.vertexCount; i++)
					{
						if (num2 >= mb_DynamicGameObject.numVerts)
						{
							num3++;
							mb_DynamicGameObject = this.combiner.mbDynamicObjectsInCombinedMesh[num3];
							this.dgo2firstIdxInBoneWeightsArray[mb_DynamicGameObject] = num;
							if (num3 == this.combiner.mbDynamicObjectsInCombinedMesh.Count - 1)
							{
								break;
							}
							num2 = 0;
						}
						num += (int)this.bonesPerVertex_nvarr[i];
						num2++;
					}
				}
			}

			public void CopyBoneWeightsFromMeshForDGOsInCombined(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, int targVidx)
			{
				this.AddBonesToNewBonesArrayAndAdjustBWIndexes1(dgo, targVidx);
			}

			public void AddBonesToNewBonesArrayAndAdjustBWIndexes1(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, int firstVertexIdxForThisDGO)
			{
				int[] tmpSMR_srcMeshBoneIdx2masterListBoneIdx = dgo._tmpSMR_srcMeshBoneIdx2masterListBoneIdx;
				int num = 0;
				for (int i = 0; i < dgo.numVerts; i++)
				{
					byte b = dgo._tmpSMR_CachedBoneWeightData.bonesPerVertex[i];
					this.bonesPerVertex_nvarr[firstVertexIdxForThisDGO + i] = b;
					for (int j = 0; j < (int)b; j++)
					{
						BoneWeight1 value = dgo._tmpSMR_CachedBoneWeightData.boneWeights[num];
						value.boneIndex = tmpSMR_srcMeshBoneIdx2masterListBoneIdx[value.boneIndex];
						this.boneWeight1s_nvarr[this.targBoneWeightIdx + num] = value;
						num++;
					}
				}
				for (int k = 0; k < dgo._tmpSMR_srcMeshBoneIdx2masterListBoneIdx.Length; k++)
				{
					int num2 = dgo._tmpSMR_srcMeshBoneIdx2masterListBoneIdx[k];
					dgo._tmpSMR_srcMeshBoneIdx2masterListBoneIdx[k] = num2;
				}
				dgo.indexesOfBonesUsed = dgo._tmpSMR_srcMeshBoneIdx2masterListBoneIdx;
				this.targBoneWeightIdx += dgo.numBoneWeights;
				if (this.LOG_LEVEL >= MB2_LogLevel.trace)
				{
					Debug.Log(string.Concat(new string[]
					{
						"AddBonesToNewBonesArrayAndAdjustBWIndexes1  ",
						dgo.name,
						"  remapped indexes for ",
						dgo._tmpSMR_CachedBoneWeightData.boneWeights.Length.ToString(),
						"  boneweigts."
					}));
				}
				dgo._tmpSMR_srcMeshBoneIdx2masterListBoneIdx = null;
				dgo._tmpSMR_CachedBones = null;
				dgo._tmpSMR_CachedBindposes = null;
				dgo._tmpSMR_CachedBoneWeights = null;
				dgo._tmpSMR_CachedBoneAndBindPose = null;
			}

			public void CopyBonesWeAreKeepingToNewBonesArrayAndAdjustBWIndexes(int totalDeleteVerts)
			{
			}

			public void CopyVertsNormsTansToBuffers(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, MB_IMeshBakerSettings settings, int vertsIdx, Vector3[] nnorms, Vector4[] ntangs, Vector3[] nverts, Vector3[] normals, Vector4[] tangents, Vector3[] verts)
			{
				Debug.LogError("TODO should call the non-native array version of this");
			}

			public void CopyVertsNormsTansToBuffers(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, MB_IMeshBakerSettings settings, int vertsIdx, NativeSlice<Vector3> nnorms, NativeSlice<Vector4> ntangs, NativeSlice<Vector3> nverts, NativeSlice<Vector3> normals, NativeSlice<Vector4> tangents, NativeSlice<Vector3> verts)
			{
				bool flag = dgo._renderer is MeshRenderer;
				if (settings.smrNoExtraBonesWhenCombiningMeshRenderers && flag && dgo._tmpSMR_CachedBones[0] != dgo.gameObject.transform)
				{
					Matrix4x4 matrix4x = dgo._tmpSMR_CachedBindposes[0].inverse * dgo._tmpSMR_CachedBones[0].worldToLocalMatrix * dgo.gameObject.transform.localToWorldMatrix;
					Matrix4x4 matrix4x2 = matrix4x;
					matrix4x2[0, 3] = (matrix4x2[1, 3] = (matrix4x2[2, 3] = 0f));
					matrix4x2 = matrix4x2.inverse.transpose;
					for (int i = 0; i < dgo._mesh.vertexCount; i++)
					{
						int index = vertsIdx + i;
						verts[vertsIdx + i] = matrix4x.MultiplyPoint3x4(nverts[i]);
						if (settings.doNorm)
						{
							normals[index] = matrix4x2.MultiplyPoint3x4(nnorms[i]).normalized;
						}
						if (settings.doTan)
						{
							float w = ntangs[i].w;
							Vector4 value = matrix4x2.MultiplyPoint3x4(ntangs[i]).normalized;
							value.w = w;
							tangents[index] = value;
						}
					}
					return;
				}
				if (settings.doNorm)
				{
					MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.NativeSliceCopyTo<Vector3>(nnorms, normals, vertsIdx);
				}
				if (settings.doTan)
				{
					MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.NativeSliceCopyTo<Vector4>(ntangs, tangents, vertsIdx);
				}
				MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.NativeSliceCopyTo<Vector3>(nverts, verts, vertsIdx);
			}

			public void InsertNewBonesIntoBonesArray()
			{
				if (this.combiner.settings.renderType == MB_RenderType.skinnedMeshRenderer)
				{
					if (this.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						Debug.Log("InsertNewBonesIntoBonesArray ");
					}
					this.combiner.bindPoses = this.nBindPoses;
					this.combiner.bones = this.nbones;
					return;
				}
				if (this.combiner.bindPoses == null || this.combiner.bindPoses.Length != 0)
				{
					this.combiner.bindPoses = new Matrix4x4[0];
				}
				if (this.combiner.bones == null || this.combiner.bones.Length != 0)
				{
					this.combiner.bones = new Transform[0];
				}
			}

			public void ApplySMRdataToMeshToBuffer()
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.trace)
				{
					Debug.Log("ApplySMRdataToMeshToBuffer ");
				}
			}

			public void ApplySMRdataToMesh(MB3_MeshCombinerSingle combiner, Mesh mesh)
			{
				mesh.bindposes = this.nBindPoses;
				mesh.SetBoneWeights(this.bonesPerVertex_nvarr, this.boneWeight1s_nvarr);
				this.nBindPoses = null;
				this.nbones = null;
				this.bonesPerVertex_nvarr.Dispose();
				this.boneWeight1s_nvarr.Dispose();
			}

			public void UpdateGameObjects_UpdateBWIndexes(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo)
			{
				NativeArray<BoneWeight1> boneWeights = dgo._tmpSMR_CachedBoneWeightData.boneWeights;
				bool flag = false;
				int num = this.dgo2firstIdxInBoneWeightsArray[dgo];
				for (int i = 0; i < boneWeights.Length; i++)
				{
					BoneWeight1 value = boneWeights[i];
					value.boneIndex = dgo.indexesOfBonesUsed[value.boneIndex];
					this.boneWeight1s_nvarr[num] = value;
					num++;
				}
				if (flag)
				{
					Debug.LogError("Detected that some of the boneweights reference different bones than when initial added. Boneweights must reference the same bones " + dgo.name);
				}
			}

			protected void Dispose(bool disposing)
			{
				if (this._disposed)
				{
					return;
				}
				if (disposing)
				{
					if (this.boneWeight1s_nvarr.IsCreated)
					{
						this.boneWeight1s_nvarr.Dispose();
					}
					if (this.bonesPerVertex_nvarr.IsCreated)
					{
						this.bonesPerVertex_nvarr.Dispose();
					}
				}
				this._disposed = true;
			}

			public void Dispose()
			{
				this.Dispose(true);
			}

			public void DisposeOfTemporarySMRData()
			{
				if (this.bonesToAddAndInCombined != null)
				{
					this.bonesToAddAndInCombined.Clear();
				}
				if (this.masterList != null)
				{
					this.masterList.Clear();
				}
				if (this.dgo2firstIdxInBoneWeightsArray != null)
				{
					this.dgo2firstIdxInBoneWeightsArray.Clear();
				}
				for (int i = 0; i < this.combiner.mbDynamicObjectsInCombinedMesh.Count; i++)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject = this.combiner.mbDynamicObjectsInCombinedMesh[i];
					mb_DynamicGameObject._tmpSMR_srcMeshBoneIdx2masterListBoneIdx = null;
					mb_DynamicGameObject._tmpSMR_CachedBindposes = null;
					mb_DynamicGameObject._tmpSMR_CachedBoneAndBindPose = null;
					mb_DynamicGameObject._tmpSMR_CachedBones = null;
					mb_DynamicGameObject._tmpSMR_CachedBoneWeightData.Dispose();
					mb_DynamicGameObject._tmpSMR_CachedBoneWeights = null;
				}
			}

			internal void _AllocateNewArraysForCombinedMesh(int newVertSize, MB3_MeshCombinerSingle.IVertexAndTriangleProcessor vertexAndTriangleProcessor)
			{
				if (this.boneWeight1s_nvarr.IsCreated)
				{
					this.boneWeight1s_nvarr.Dispose();
				}
				if (this.bonesPerVertex_nvarr.IsCreated)
				{
					this.bonesPerVertex_nvarr.Dispose();
				}
				this.boneWeight1s_nvarr = new NativeArray<BoneWeight1>(this.boneWeightSize, Allocator.Persistent, NativeArrayOptions.ClearMemory);
				this.bonesPerVertex_nvarr = new NativeArray<byte>(newVertSize, Allocator.Persistent, NativeArrayOptions.ClearMemory);
				this.nBindPoses = new Matrix4x4[this.masterList.Count];
				this.nbones = new Transform[this.masterList.Count];
				for (int i = 0; i < this.masterList.Count; i++)
				{
					this.nBindPoses[i] = this.masterList[i].bindPose;
					this.nbones[i] = this.masterList[i].bone;
				}
				if (this.LOG_LEVEL >= MB2_LogLevel.trace)
				{
					Debug.Log(string.Concat(new string[]
					{
						"  _AllocateNewArraysForCombinedMesh boneWeight1s_nvarr:",
						this.boneWeight1s_nvarr.Length.ToString(),
						" bonesPerVertex_nvarr:",
						this.bonesPerVertex_nvarr.Length.ToString(),
						"  numBones: ",
						this.masterList.Count.ToString()
					}));
				}
				this.targBoneWeightIdx = 0;
			}

			private bool _CollectBonesToAddForDGO_Pass2(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, bool noExtraBonesForMeshRenderers)
			{
				bool result = true;
				List<Matrix4x4> tmpSMR_CachedBindposes = dgo._tmpSMR_CachedBindposes;
				Transform[] tmpSMR_CachedBones = dgo._tmpSMR_CachedBones;
				if (noExtraBonesForMeshRenderers && dgo._renderer is MeshRenderer)
				{
					bool flag = false;
					MB3_MeshCombinerSingle.BoneAndBindpose boneAndBindpose = default(MB3_MeshCombinerSingle.BoneAndBindpose);
					Transform parent = dgo.gameObject.transform.parent;
					while (parent != null)
					{
						foreach (MB3_MeshCombinerSingle.BoneAndBindpose boneAndBindpose2 in this.bonesToAddAndInCombined)
						{
							if (boneAndBindpose2.bone == parent)
							{
								boneAndBindpose = boneAndBindpose2;
								flag = true;
								break;
							}
						}
						if (flag)
						{
							break;
						}
						parent = parent.parent;
					}
					if (flag)
					{
						tmpSMR_CachedBones[0] = boneAndBindpose.bone;
						tmpSMR_CachedBindposes[0] = boneAndBindpose.bindPose;
					}
				}
				for (int i = 0; i < tmpSMR_CachedBones.Length; i++)
				{
					if (dgo._tmpSMR_CachedBoneWeightData.UsedBoneIdxsInSrcMesh[i])
					{
						MB3_MeshCombinerSingle.BoneAndBindpose boneAndBindpose3 = new MB3_MeshCombinerSingle.BoneAndBindpose(tmpSMR_CachedBones[i], tmpSMR_CachedBindposes[i]);
						dgo._tmpSMR_CachedBoneAndBindPose[i] = boneAndBindpose3;
					}
				}
				return result;
			}

			private int _BuildMasterBonesArray(List<MB3_MeshCombinerSingle.MB_DynamicGameObject> dgosToAdd, List<MB3_MeshCombinerSingle.MB_DynamicGameObject> dgosInCombinedMesh)
			{
				this.boneWeightSize = 0;
				Dictionary<MB3_MeshCombinerSingle.BoneAndBindpose, int> dictionary = new Dictionary<MB3_MeshCombinerSingle.BoneAndBindpose, int>();
				this.masterList.Clear();
				StringBuilder stringBuilder = null;
				if (this.LOG_LEVEL >= MB2_LogLevel.trace)
				{
					stringBuilder = new StringBuilder();
					stringBuilder.AppendLine("_BuildMasterBonesArray");
				}
				for (int i = 0; i < dgosInCombinedMesh.Count; i++)
				{
					if (!dgosInCombinedMesh[i]._beingDeleted)
					{
						MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject = dgosInCombinedMesh[i];
						this.boneWeightSize += mb_DynamicGameObject.numBoneWeights;
						int num = mb_DynamicGameObject._tmpSMR_CachedBoneAndBindPose.Length;
						int[] array = new int[num];
						int num2 = 0;
						for (int j = 0; j < num; j++)
						{
							if (mb_DynamicGameObject._tmpSMR_CachedBoneWeightData.UsedBoneIdxsInSrcMesh[j])
							{
								MB3_MeshCombinerSingle.BoneAndBindpose boneAndBindpose = mb_DynamicGameObject._tmpSMR_CachedBoneAndBindPose[j];
								int count;
								if (!dictionary.TryGetValue(boneAndBindpose, out count))
								{
									dictionary.Add(boneAndBindpose, this.masterList.Count);
									count = this.masterList.Count;
									num2++;
									this.masterList.Add(boneAndBindpose);
								}
								array[j] = count;
							}
						}
						mb_DynamicGameObject._tmpSMR_srcMeshBoneIdx2masterListBoneIdx = array;
						if (this.LOG_LEVEL >= MB2_LogLevel.trace)
						{
							stringBuilder.AppendLine(string.Concat(new string[]
							{
								mb_DynamicGameObject.name,
								"  addedToMasterList: ",
								num2.ToString(),
								"    srcMeshBoneIdx2masterListBoneIdx: ",
								array.Length.ToString()
							}));
						}
					}
				}
				for (int k = 0; k < dgosToAdd.Count; k++)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject2 = dgosToAdd[k];
					this.boneWeightSize += mb_DynamicGameObject2.numBoneWeights;
					int num3 = mb_DynamicGameObject2._tmpSMR_CachedBoneAndBindPose.Length;
					int[] array2 = new int[num3];
					for (int l = 0; l < num3; l++)
					{
						if (mb_DynamicGameObject2._tmpSMR_CachedBoneWeightData.UsedBoneIdxsInSrcMesh[l])
						{
							MB3_MeshCombinerSingle.BoneAndBindpose boneAndBindpose2 = mb_DynamicGameObject2._tmpSMR_CachedBoneAndBindPose[l];
							int count2;
							if (!dictionary.TryGetValue(boneAndBindpose2, out count2))
							{
								dictionary.Add(boneAndBindpose2, this.masterList.Count);
								count2 = this.masterList.Count;
								this.masterList.Add(boneAndBindpose2);
							}
							array2[l] = count2;
						}
					}
					mb_DynamicGameObject2._tmpSMR_srcMeshBoneIdx2masterListBoneIdx = array2;
					if (this.LOG_LEVEL >= MB2_LogLevel.trace)
					{
						stringBuilder.AppendLine(mb_DynamicGameObject2.name + "    srcMeshBoneIdx2masterListBoneIdx: " + array2.Length.ToString());
					}
				}
				if (this.LOG_LEVEL >= MB2_LogLevel.trace)
				{
					stringBuilder.AppendLine("Master List Length: " + this.masterList.Count.ToString());
					Debug.Log(stringBuilder);
				}
				return this.masterList.Count;
			}

			internal void _CollectSkinningDataForDGOsInCombinedMesh(List<MB3_MeshCombinerSingle.MB_DynamicGameObject> dgosAdding, List<MB3_MeshCombinerSingle.MB_DynamicGameObject> dgosInCombinedMesh, MB3_MeshCombinerSingle.MeshChannelsCache_NativeArray meshChannelsCache)
			{
				for (int i = 0; i < dgosAdding.Count; i++)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject dgo = dgosAdding[i];
					this._CollectBonesToAddForDGO_Pass2(dgo, this.combiner.settings.smrNoExtraBonesWhenCombiningMeshRenderers);
				}
				int num = 0;
				for (int j = 0; j < dgosInCombinedMesh.Count; j++)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject = dgosInCombinedMesh[j];
					if (!mb_DynamicGameObject._beingDeleted)
					{
						num++;
						this._CollectBonesToAddForDGO_Pass2(mb_DynamicGameObject, this.combiner.settings.smrNoExtraBonesWhenCombiningMeshRenderers);
					}
				}
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					Debug.Log("_CollectSkinningDataForDGOsInCombinedMesh: dgosAdding:" + dgosAdding.Count.ToString() + " dgosInCombined:" + num.ToString());
				}
			}

			public bool DB_CheckIntegrity()
			{
				return true;
			}

			private MB2_LogLevel LOG_LEVEL;

			private bool _initialized;

			private bool _disposed;

			private MB3_MeshCombinerSingle combiner;

			private HashSet<MB3_MeshCombinerSingle.BoneAndBindpose> bonesToAddAndInCombined = new HashSet<MB3_MeshCombinerSingle.BoneAndBindpose>();

			private List<MB3_MeshCombinerSingle.BoneAndBindpose> masterList = new List<MB3_MeshCombinerSingle.BoneAndBindpose>();

			private Matrix4x4[] nBindPoses;

			private Transform[] nbones;

			private int boneWeightSize;

			private int targBoneWeightIdx;

			private Dictionary<MB3_MeshCombinerSingle.MB_DynamicGameObject, int> dgo2firstIdxInBoneWeightsArray = new Dictionary<MB3_MeshCombinerSingle.MB_DynamicGameObject, int>();

			private NativeArray<byte> bonesPerVertex_nvarr;

			private NativeArray<BoneWeight1> boneWeight1s_nvarr;
		}

		public enum MeshCreationConditions
		{
			NoMesh,
			CreatedInEditor,
			CreatedAtRuntime,
			AssignedByUser
		}

		[Serializable]
		public struct BufferDataFromPreviousBake
		{
			public int numVertsBaked;

			public Vector3 meshVerticesShift;

			public bool meshVerticiesWereShifted;
		}

		[Serializable]
		public class SerializableIntArray
		{
			public SerializableIntArray()
			{
				this.data = new int[0];
			}

			public SerializableIntArray(int len)
			{
				this.data = new int[len];
			}

			[SerializeField]
			public int[] data;
		}

		public struct BoneWeightDataForMesh
		{
			internal void Dispose()
			{
				this.Dispose(true);
			}

			private void Dispose(bool disposing)
			{
				if (this._disposed)
				{
					return;
				}
				this._disposed = true;
				this.initialized = false;
				if (this.bonesPerVertex.IsCreated && this.weMustDispose)
				{
					this.bonesPerVertex.Dispose();
				}
				if (this.boneWeights.IsCreated && this.weMustDispose)
				{
					this.boneWeights.Dispose();
				}
			}

			private bool _disposed;

			public bool initialized;

			public bool weMustDispose;

			public NativeArray<byte> bonesPerVertex;

			public NativeArray<BoneWeight1> boneWeights;

			public bool[] UsedBoneIdxsInSrcMesh;

			public int numUsedbones;
		}

		[Serializable]
		public class MB_DynamicGameObject : IComparable<MB3_MeshCombinerSingle.MB_DynamicGameObject>
		{
			public bool Initialize(bool beingDeleted)
			{
				this._initialized = true;
				this._beingDeleted = beingDeleted;
				if (!beingDeleted)
				{
					this._mesh = MB_Utility.GetMesh(this.gameObject);
					this._renderer = MB_Utility.GetRenderer(this.gameObject);
					return this._mesh != null && this._renderer != null;
				}
				return true;
			}

			public bool InitializeNew(bool beingDeleted, GameObject go)
			{
				this.gameObject = go;
				this.name = string.Format("{0} {1}", this.gameObject.ToString(), this.gameObject.GetInstanceID());
				if (go == null)
				{
					return false;
				}
				this.instanceID = this.gameObject.GetInstanceID();
				return this.Initialize(beingDeleted);
			}

			public void UnInitialize()
			{
				this._initialized = false;
				this._beingDeleted = false;
				this._mesh = null;
				this._renderer = null;
			}

			public int CompareTo(MB3_MeshCombinerSingle.MB_DynamicGameObject b)
			{
				return this.vertIdx - b.vertIdx;
			}

			public int instanceID;

			public GameObject gameObject;

			public string name;

			public int vertIdx;

			public int blendShapeIdx;

			public int numVerts;

			public int numBlendShapes;

			public int numBoneWeights;

			public bool isSkinnedMeshWithBones;

			public int[] indexesOfBonesUsed = new int[0];

			public int lightmapIndex = -1;

			public Vector4 lightmapTilingOffset = new Vector4(1f, 1f, 0f, 0f);

			public Vector3 meshSize = Vector3.one;

			public bool show = true;

			public bool invertTriangles;

			public int[] submeshTriIdxs;

			public int[] submeshNumTris;

			public int[] targetSubmeshIdxs;

			public Rect[] uvRects;

			public Rect[] encapsulatingRect;

			public Rect[] sourceMaterialTiling;

			public Rect[] obUVRects;

			public int[] textureArraySliceIdx;

			public Material[] sourceSharedMaterials;

			[NonSerialized]
			internal bool _initialized;

			[NonSerialized]
			internal bool _beingDeleted;

			[NonSerialized]
			internal Mesh _mesh;

			[NonSerialized]
			internal Renderer _renderer;

			[NonSerialized]
			internal MB3_MeshCombinerSingle.SerializableIntArray[] _tmpSubmeshTris;

			[NonSerialized]
			internal Transform[] _tmpSMR_CachedBones;

			[NonSerialized]
			internal List<Matrix4x4> _tmpSMR_CachedBindposes;

			[NonSerialized]
			internal MB3_MeshCombinerSingle.BoneAndBindpose[] _tmpSMR_CachedBoneAndBindPose;

			[NonSerialized]
			internal int[] _tmpSMR_srcMeshBoneIdx2masterListBoneIdx;

			[NonSerialized]
			internal BoneWeight[] _tmpSMR_CachedBoneWeights;

			[NonSerialized]
			internal MB3_MeshCombinerSingle.BoneWeightDataForMesh _tmpSMR_CachedBoneWeightData;
		}

		public class MeshChannels : IDisposable
		{
			public void Dispose()
			{
				this.Dispose(true);
			}

			public bool IsDisposed()
			{
				return this._disposed;
			}

			protected virtual void Dispose(bool disposing)
			{
				if (this._disposed)
				{
					return;
				}
				this._disposed = true;
				this.vertices = null;
				this.normals = null;
				this.tangents = null;
				this.uv0raw = null;
				this.uv0modified = null;
				this.uv2raw = null;
				this.uv2modified = null;
				this.uv3 = null;
				this.uv4 = null;
				this.uv5 = null;
				this.uv6 = null;
				this.uv7 = null;
				this.uv8 = null;
				this.colors = null;
				this.boneWeights = null;
				this.bindPoses = null;
				this.triangles = null;
				this.blendShapes = null;
			}

			private bool _disposed;

			public Vector3[] vertices;

			public Vector3[] normals;

			public Vector4[] tangents;

			public Vector2[] uv0raw;

			public Vector2[] uv0modified;

			public Vector2[] uv2raw;

			public Vector2[] uv2modified;

			public Vector2[] uv3;

			public Vector2[] uv4;

			public Vector2[] uv5;

			public Vector2[] uv6;

			public Vector2[] uv7;

			public Vector2[] uv8;

			public Color[] colors;

			public BoneWeight[] boneWeights;

			public List<Matrix4x4> bindPoses = new List<Matrix4x4>(128);

			public int[] triangles;

			public MB3_MeshCombinerSingle.MBBlendShape[] blendShapes;
		}

		[Serializable]
		public class MBBlendShapeFrame
		{
			public float frameWeight;

			public Vector3[] vertices;

			public Vector3[] normals;

			public Vector3[] tangents;
		}

		[Serializable]
		public class MBBlendShape
		{
			public GameObject gameObject;

			public string name;

			public int indexInSource;

			public MB3_MeshCombinerSingle.MBBlendShapeFrame[] frames;
		}

		public struct BoneAndBindpose
		{
			public BoneAndBindpose(Transform t, Matrix4x4 bp)
			{
				this.bone = t;
				this.bindPose = bp;
			}

			public override bool Equals(object obj)
			{
				return obj is MB3_MeshCombinerSingle.BoneAndBindpose && this.bone == ((MB3_MeshCombinerSingle.BoneAndBindpose)obj).bone && this.bindPose == ((MB3_MeshCombinerSingle.BoneAndBindpose)obj).bindPose;
			}

			public override int GetHashCode()
			{
				return this.bone.GetInstanceID() % int.MaxValue ^ (int)this.bindPose[0, 0];
			}

			public Transform bone;

			public Matrix4x4 bindPose;
		}

		public interface IMeshChannelsCacheTaggingInterface
		{
			void Dispose();

			bool HasCollectedMeshData();

			void CollectChannelDataForAllMeshesInList(List<MB3_MeshCombinerSingle.MB_DynamicGameObject> toUpdateDGOs, List<MB3_MeshCombinerSingle.MB_DynamicGameObject> toAddDGOs, MB_MeshVertexChannelFlags newChannels, MB_RenderType renderType, bool doBlendShapes);

			MB3_MeshCombinerSingle.MBBlendShape[] GetBlendShapes(Mesh mesh, int instanceID, GameObject gameObject);

			bool hasOutOfBoundsUVs(Mesh m, ref MB_Utility.MeshAnalysisResult mar, int submeshIdx);
		}

		public class MeshChannelsCache : IDisposable, MB3_MeshCombinerSingle.IMeshChannelsCacheTaggingInterface
		{
			public MeshChannelsCache(MB2_LogLevel ll, MB2_LightmapOptions lo)
			{
				this.LOG_LEVEL = ll;
				this.lightmapOption = lo;
			}

			public void Dispose()
			{
				this.Dispose(true);
			}

			protected virtual void Dispose(bool disposing)
			{
				if (this._disposed)
				{
					return;
				}
				foreach (MB3_MeshCombinerSingle.MeshChannels meshChannels in this.meshID2MeshChannels.Values)
				{
					meshChannels.Dispose();
				}
				this._collectedMeshData = false;
				this._disposed = true;
			}

			public bool HasCollectedMeshData()
			{
				return this._collectedMeshData;
			}

			public bool hasOutOfBoundsUVs(Mesh m, ref MB_Utility.MeshAnalysisResult mar, int submeshIdx)
			{
				return MB_Utility.hasOutOfBoundsUVs(this.GetUv0Raw(m), m, ref mar, submeshIdx);
			}

			internal Vector3[] GetVertices(Mesh m)
			{
				MB3_MeshCombinerSingle.MeshChannels meshChannels;
				if (!this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannels))
				{
					Debug.LogError("Could not find mesh in the MeshChannelsCache." + ((m != null) ? m.ToString() : null));
				}
				return meshChannels.vertices;
			}

			internal Vector3[] GetNormals(Mesh m)
			{
				MB3_MeshCombinerSingle.MeshChannels meshChannels;
				this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannels);
				return meshChannels.normals;
			}

			internal Vector4[] GetTangents(Mesh m)
			{
				MB3_MeshCombinerSingle.MeshChannels meshChannels;
				this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannels);
				return meshChannels.tangents;
			}

			internal Vector2[] GetUv0Raw(Mesh m)
			{
				MB3_MeshCombinerSingle.MeshChannels meshChannels;
				this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannels);
				return meshChannels.uv0raw;
			}

			internal Vector2[] GetUv0Modified(Mesh m)
			{
				MB3_MeshCombinerSingle.MeshChannels meshChannels;
				this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannels);
				return meshChannels.uv0modified;
			}

			internal Vector2[] GetUv2Modified(Mesh m)
			{
				MB3_MeshCombinerSingle.MeshChannels meshChannels;
				this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannels);
				return meshChannels.uv2modified;
			}

			internal Vector2[] GetUVChannel(int channel, Mesh m)
			{
				MB3_MeshCombinerSingle.MeshChannels meshChannels;
				this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannels);
				switch (channel)
				{
				case 0:
					return meshChannels.uv0raw;
				case 2:
					return meshChannels.uv2raw;
				case 3:
					return meshChannels.uv3;
				case 4:
					return meshChannels.uv4;
				case 5:
					return meshChannels.uv5;
				case 6:
					return meshChannels.uv6;
				case 7:
					return meshChannels.uv7;
				case 8:
					return meshChannels.uv8;
				}
				Debug.LogError("Error mesh channel " + channel.ToString() + " not supported");
				return null;
			}

			internal Color[] GetColors(Mesh m)
			{
				MB3_MeshCombinerSingle.MeshChannels meshChannels;
				this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannels);
				return meshChannels.colors;
			}

			public void CollectChannelDataForAllMeshesInList(List<MB3_MeshCombinerSingle.MB_DynamicGameObject> toUpdateDGOs, List<MB3_MeshCombinerSingle.MB_DynamicGameObject> toAddDGOs, MB_MeshVertexChannelFlags newChannels, MB_RenderType renderType, bool doBlendShapes)
			{
				bool flag = (newChannels & MB_MeshVertexChannelFlags.vertex) == MB_MeshVertexChannelFlags.vertex;
				bool flag2 = (newChannels & MB_MeshVertexChannelFlags.normal) == MB_MeshVertexChannelFlags.normal;
				bool flag3 = (newChannels & MB_MeshVertexChannelFlags.tangent) == MB_MeshVertexChannelFlags.tangent;
				bool flag4 = (newChannels & MB_MeshVertexChannelFlags.uv0) == MB_MeshVertexChannelFlags.uv0;
				bool flag5 = (newChannels & MB_MeshVertexChannelFlags.uv2) == MB_MeshVertexChannelFlags.uv2;
				bool flag6 = (newChannels & MB_MeshVertexChannelFlags.uv3) == MB_MeshVertexChannelFlags.uv3;
				bool flag7 = (newChannels & MB_MeshVertexChannelFlags.uv4) == MB_MeshVertexChannelFlags.uv4;
				bool flag8 = (newChannels & MB_MeshVertexChannelFlags.uv5) == MB_MeshVertexChannelFlags.uv5;
				bool flag9 = (newChannels & MB_MeshVertexChannelFlags.uv6) == MB_MeshVertexChannelFlags.uv6;
				bool flag10 = (newChannels & MB_MeshVertexChannelFlags.uv7) == MB_MeshVertexChannelFlags.uv7;
				bool flag11 = (newChannels & MB_MeshVertexChannelFlags.uv8) == MB_MeshVertexChannelFlags.uv8;
				bool flag12 = (newChannels & MB_MeshVertexChannelFlags.colors) == MB_MeshVertexChannelFlags.colors;
				List<MB3_MeshCombinerSingle.MB_DynamicGameObject> list = new List<MB3_MeshCombinerSingle.MB_DynamicGameObject>();
				list.AddRange(toUpdateDGOs);
				list.AddRange(toAddDGOs);
				for (int i = 0; i < list.Count; i++)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject = list[i];
					Mesh mesh = mb_DynamicGameObject._mesh;
					if (!this.meshID2MeshChannels.ContainsKey(mesh.GetInstanceID()))
					{
						MB3_MeshCombinerSingle.MeshChannels meshChannels = new MB3_MeshCombinerSingle.MeshChannels();
						this.meshID2MeshChannels.Add(mesh.GetInstanceID(), meshChannels);
						if (flag)
						{
							meshChannels.vertices = mesh.vertices;
						}
						if (flag4)
						{
							meshChannels.uv0raw = this._getMeshUVs(mesh);
						}
						if (flag5)
						{
							meshChannels.uv2raw = this._getMeshUV2s(mesh, ref meshChannels.uv2modified);
						}
						if (flag2)
						{
							meshChannels.normals = this._getMeshNormals(mesh);
						}
						if (flag3)
						{
							meshChannels.tangents = this._getMeshTangents(mesh);
						}
						if (flag6)
						{
							meshChannels.uv3 = MBVersion.GetMeshChannel(3, mesh, this.LOG_LEVEL);
						}
						if (flag7)
						{
							meshChannels.uv4 = MBVersion.GetMeshChannel(4, mesh, this.LOG_LEVEL);
						}
						if (flag8)
						{
							meshChannels.uv5 = MBVersion.GetMeshChannel(5, mesh, this.LOG_LEVEL);
						}
						if (flag9)
						{
							meshChannels.uv6 = MBVersion.GetMeshChannel(6, mesh, this.LOG_LEVEL);
						}
						if (flag10)
						{
							meshChannels.uv7 = MBVersion.GetMeshChannel(7, mesh, this.LOG_LEVEL);
						}
						if (flag11)
						{
							meshChannels.uv8 = MBVersion.GetMeshChannel(8, mesh, this.LOG_LEVEL);
						}
						if (flag12)
						{
							meshChannels.colors = this._getMeshColors(mesh);
						}
						if (renderType == MB_RenderType.skinnedMeshRenderer)
						{
							Renderer renderer = mb_DynamicGameObject._renderer;
							bool isSkinnedMeshWithBones;
							MB3_MeshCombinerSingle.MeshChannelsCache._getBindPoses(renderer, meshChannels.bindPoses, out isSkinnedMeshWithBones);
							meshChannels.boneWeights = MB3_MeshCombinerSingle.MeshChannelsCache._getBoneWeights(renderer, mesh.vertexCount, isSkinnedMeshWithBones);
							if (doBlendShapes)
							{
								MB3_MeshCombinerSingle.MBBlendShape[] array = new MB3_MeshCombinerSingle.MBBlendShape[mesh.blendShapeCount];
								int vertexCount = mesh.vertexCount;
								for (int j = 0; j < array.Length; j++)
								{
									MB3_MeshCombinerSingle.MBBlendShape mbblendShape = array[j] = new MB3_MeshCombinerSingle.MBBlendShape();
									mbblendShape.frames = new MB3_MeshCombinerSingle.MBBlendShapeFrame[MBVersion.GetBlendShapeFrameCount(mesh, j)];
									mbblendShape.name = mesh.GetBlendShapeName(j);
									mbblendShape.indexInSource = j;
									mbblendShape.gameObject = mb_DynamicGameObject.gameObject;
									for (int k = 0; k < mbblendShape.frames.Length; k++)
									{
										MB3_MeshCombinerSingle.MBBlendShapeFrame mbblendShapeFrame = mbblendShape.frames[k] = new MB3_MeshCombinerSingle.MBBlendShapeFrame();
										mbblendShapeFrame.frameWeight = MBVersion.GetBlendShapeFrameWeight(mesh, j, k);
										mbblendShapeFrame.vertices = new Vector3[vertexCount];
										mbblendShapeFrame.normals = new Vector3[vertexCount];
										mbblendShapeFrame.tangents = new Vector3[vertexCount];
										MBVersion.GetBlendShapeFrameVertices(mesh, j, k, mbblendShapeFrame.vertices, mbblendShapeFrame.normals, mbblendShapeFrame.tangents);
									}
								}
								meshChannels.blendShapes = array;
							}
						}
					}
				}
				this._collectedMeshData = true;
			}

			internal List<Matrix4x4> GetBindposes(Renderer r, out bool isSkinnedMeshWithBones)
			{
				Mesh mesh = MB_Utility.GetMesh(r.gameObject);
				MB3_MeshCombinerSingle.MeshChannels meshChannels;
				this.meshID2MeshChannels.TryGetValue(mesh.GetInstanceID(), out meshChannels);
				if (r is SkinnedMeshRenderer && meshChannels.bindPoses.Count > 0)
				{
					isSkinnedMeshWithBones = true;
				}
				else
				{
					isSkinnedMeshWithBones = false;
					SkinnedMeshRenderer skinnedMeshRenderer = r as SkinnedMeshRenderer;
				}
				return meshChannels.bindPoses;
			}

			internal BoneWeight[] GetBoneWeights(Renderer r, int numVertsInMeshBeingAdded, bool isSkinnedMeshWithBones)
			{
				Mesh mesh = MB_Utility.GetMesh(r.gameObject);
				MB3_MeshCombinerSingle.MeshChannels meshChannels;
				this.meshID2MeshChannels.TryGetValue(mesh.GetInstanceID(), out meshChannels);
				return meshChannels.boneWeights;
			}

			public MB3_MeshCombinerSingle.MBBlendShape[] GetBlendShapes(Mesh m, int gameObjectID, GameObject gameObject)
			{
				MB3_MeshCombinerSingle.MeshChannels meshChannels;
				this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannels);
				MB3_MeshCombinerSingle.MBBlendShape[] array = new MB3_MeshCombinerSingle.MBBlendShape[meshChannels.blendShapes.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = new MB3_MeshCombinerSingle.MBBlendShape();
					array[i].name = meshChannels.blendShapes[i].name;
					array[i].indexInSource = meshChannels.blendShapes[i].indexInSource;
					array[i].frames = meshChannels.blendShapes[i].frames;
					array[i].gameObject = gameObject;
				}
				return array;
			}

			private Color[] _getMeshColors(Mesh m)
			{
				Color[] array = m.colors;
				if (array.Length == 0)
				{
					if (this.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug("Mesh " + ((m != null) ? m.ToString() : null) + " has no colors. Generating", Array.Empty<object>());
					}
					if (this.LOG_LEVEL >= MB2_LogLevel.warn)
					{
						Debug.LogWarning("Mesh " + ((m != null) ? m.ToString() : null) + " didn't have colors. Generating an array of white colors");
					}
					array = new Color[m.vertexCount];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = Color.white;
					}
				}
				return array;
			}

			private Vector3[] _getMeshNormals(Mesh m)
			{
				Vector3[] normals = m.normals;
				if (normals.Length == 0)
				{
					if (this.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug("Mesh " + ((m != null) ? m.ToString() : null) + " has no normals. Generating", Array.Empty<object>());
					}
					if (this.LOG_LEVEL >= MB2_LogLevel.warn)
					{
						Debug.LogWarning("Mesh " + ((m != null) ? m.ToString() : null) + " didn't have normals. Generating normals.");
					}
					Mesh mesh = Object.Instantiate<Mesh>(m);
					mesh.RecalculateNormals();
					normals = mesh.normals;
					MB_Utility.Destroy(mesh);
				}
				return normals;
			}

			private Vector4[] _getMeshTangents(Mesh m)
			{
				Vector4[] array = m.tangents;
				if (array.Length == 0)
				{
					if (this.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug("Mesh " + ((m != null) ? m.ToString() : null) + " has no tangents. Generating", Array.Empty<object>());
					}
					if (this.LOG_LEVEL >= MB2_LogLevel.warn)
					{
						Debug.LogWarning("Mesh " + ((m != null) ? m.ToString() : null) + " didn't have tangents. Generating tangents.");
					}
					Vector3[] vertices = m.vertices;
					Vector2[] uv0Raw = this.GetUv0Raw(m);
					Vector3[] normals = this._getMeshNormals(m);
					array = new Vector4[m.vertexCount];
					for (int i = 0; i < m.subMeshCount; i++)
					{
						int[] triangles = m.GetTriangles(i);
						this._generateTangents(triangles, vertices, uv0Raw, normals, array);
					}
				}
				return array;
			}

			private Vector2[] _getMeshUVs(Mesh m)
			{
				Vector2[] array = m.uv;
				if (array.Length == 0)
				{
					array = new Vector2[m.vertexCount];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = this._HALF_UV;
					}
				}
				return array;
			}

			private Vector2[] _getMeshUV2s(Mesh m, ref Vector2[] uv2modified)
			{
				Vector2[] uv = m.uv2;
				if (uv.Length == 0)
				{
					uv2modified = new Vector2[m.vertexCount];
					for (int i = 0; i < uv2modified.Length; i++)
					{
						uv2modified[i] = this._HALF_UV;
					}
				}
				return uv;
			}

			private static void _getBindPoses(Renderer r, List<Matrix4x4> poses, out bool isSkinnedMeshWithBones)
			{
				poses.Clear();
				isSkinnedMeshWithBones = (r is SkinnedMeshRenderer);
				if (r is SkinnedMeshRenderer)
				{
					Mesh mesh = MB_Utility.GetMesh(r.gameObject);
					mesh.GetBindposes(poses);
					if (poses.Count == 0)
					{
						if (mesh.blendShapeCount > 0)
						{
							isSkinnedMeshWithBones = false;
						}
						else
						{
							Debug.LogError("Skinned mesh " + ((r != null) ? r.ToString() : null) + " had no bindposes AND no blend shapes");
						}
					}
				}
				if (r is MeshRenderer || (r is SkinnedMeshRenderer && !isSkinnedMeshWithBones))
				{
					poses.Clear();
					poses.Add(Matrix4x4.identity);
				}
				if (poses == null || poses.Count == 0)
				{
					Debug.LogError("Could not _getBindPoses. Object does not have a renderer");
				}
			}

			private static BoneWeight[] _getBoneWeights(Renderer r, int numVertsInMeshBeingAdded, bool isSkinnedMeshWithBones)
			{
				if (isSkinnedMeshWithBones)
				{
					return ((SkinnedMeshRenderer)r).sharedMesh.boneWeights;
				}
				if (r is MeshRenderer || (r is SkinnedMeshRenderer && !isSkinnedMeshWithBones))
				{
					BoneWeight boneWeight = default(BoneWeight);
					boneWeight.boneIndex0 = (boneWeight.boneIndex1 = (boneWeight.boneIndex2 = (boneWeight.boneIndex3 = 0)));
					boneWeight.weight0 = 1f;
					boneWeight.weight1 = (boneWeight.weight2 = (boneWeight.weight3 = 0f));
					BoneWeight[] array = new BoneWeight[numVertsInMeshBeingAdded];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = boneWeight;
					}
					return array;
				}
				Debug.LogError("Could not _getBoneWeights. Object does not have a renderer");
				return null;
			}

			private void _generateTangents(int[] triangles, Vector3[] verts, Vector2[] uvs, Vector3[] normals, Vector4[] outTangents)
			{
				int num = triangles.Length;
				int num2 = verts.Length;
				Vector3[] array = new Vector3[num2];
				Vector3[] array2 = new Vector3[num2];
				for (int i = 0; i < num; i += 3)
				{
					int num3 = triangles[i];
					int num4 = triangles[i + 1];
					int num5 = triangles[i + 2];
					Vector3 vector = verts[num3];
					Vector3 vector2 = verts[num4];
					Vector3 vector3 = verts[num5];
					Vector2 vector4 = uvs[num3];
					Vector2 vector5 = uvs[num4];
					Vector2 vector6 = uvs[num5];
					float num6 = vector2.x - vector.x;
					float num7 = vector3.x - vector.x;
					float num8 = vector2.y - vector.y;
					float num9 = vector3.y - vector.y;
					float num10 = vector2.z - vector.z;
					float num11 = vector3.z - vector.z;
					float num12 = vector5.x - vector4.x;
					float num13 = vector6.x - vector4.x;
					float num14 = vector5.y - vector4.y;
					float num15 = vector6.y - vector4.y;
					float num16 = num12 * num15 - num13 * num14;
					if (num16 == 0f)
					{
						Debug.LogError("Could not compute tangents. All UVs need to form a valid triangles in UV space. If any UV triangles are collapsed, tangents cannot be generated.");
						return;
					}
					float num17 = 1f / num16;
					Vector3 b = new Vector3((num15 * num6 - num14 * num7) * num17, (num15 * num8 - num14 * num9) * num17, (num15 * num10 - num14 * num11) * num17);
					Vector3 b2 = new Vector3((num12 * num7 - num13 * num6) * num17, (num12 * num9 - num13 * num8) * num17, (num12 * num11 - num13 * num10) * num17);
					array[num3] += b;
					array[num4] += b;
					array[num5] += b;
					array2[num3] += b2;
					array2[num4] += b2;
					array2[num5] += b2;
				}
				for (int j = 0; j < num2; j++)
				{
					Vector3 vector7 = normals[j];
					Vector3 vector8 = array[j];
					Vector3 normalized = (vector8 - vector7 * Vector3.Dot(vector7, vector8)).normalized;
					outTangents[j] = new Vector4(normalized.x, normalized.y, normalized.z);
					outTangents[j].w = ((Vector3.Dot(Vector3.Cross(vector7, vector8), array2[j]) < 0f) ? -1f : 1f);
				}
			}

			private MB2_LogLevel LOG_LEVEL;

			private MB2_LightmapOptions lightmapOption;

			protected Dictionary<int, MB3_MeshCombinerSingle.MeshChannels> meshID2MeshChannels = new Dictionary<int, MB3_MeshCombinerSingle.MeshChannels>();

			private bool _collectedMeshData;

			private bool _disposed;

			private Vector2 _HALF_UV = new Vector2(0.5f, 0.5f);
		}

		public interface IVertexAndTriangleProcessor : IDisposable
		{
			MB_MeshVertexChannelFlags channels { get; }

			bool IsInitialized();

			bool IsDisposed();

			void Init(MB3_MeshCombinerSingle combiner, MB_MeshVertexChannelFlags newChannels, int vertexCount, int[] newSubmeshTrisSize, int uvChannelWithExtraParameter, MB3_MeshCombinerSingle.IMeshChannelsCacheTaggingInterface meshChannelsCache, bool loadDataFromCombinedMesh, MB2_LogLevel logLevel);

			void InitShowHide(MB3_MeshCombinerSingle combiner);

			void InitFromMeshCombiner(MB3_MeshCombinerSingle combiner, MB_MeshVertexChannelFlags newChannels, int uvChannelWithExtraParameter);

			int GetVertexCount();

			int GetSubmeshCount();

			void TransferOwnershipOfSerializableBuffersToCombiner(MB3_MeshCombinerSingle c, MB_MeshVertexChannelFlags channelsToTransfer, MB3_MeshCombinerSingle.BufferDataFromPreviousBake serializableBufferData);

			void CopyArraysFromPreviousBakeBuffersToNewBuffers(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, ref MB3_MeshCombinerSingle.IVertexAndTriangleProcessor iOldBuffers, int destStartVertIdx, int triangleIdxAdjustment, int[] targSubmeshTidx, MB2_LogLevel LOG_LEVEL);

			void CopyFromDGOMeshToBuffers(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, int destStartVertsIdx, MB_MeshVertexChannelFlags channelsToUpdate, bool updateTris, bool updateBWdata, MB_IMeshBakerSettings settings, MB_IMeshCombinerSingle_BoneProcessor boneProcessor, int[] targSubmeshTidx, MB2_TextureBakeResults textureBakeResults, MB3_MeshCombinerSingle.UVAdjuster_Atlas uvAdjuster, MB2_LogLevel LOG_LEVEL, MB3_MeshCombinerSingle.IMeshChannelsCacheTaggingInterface meshChannelCache);

			void AssignBuffersToMesh(Mesh mesh, MB_IMeshBakerSettings settings, MB2_TextureBakeResults textureBakeResults, MB_MeshVertexChannelFlags channelsToWriteToMesh, bool doWriteTrisToMesh, IAssignToMeshCustomizer assignToMeshCustomizer, List<MB3_MeshCombinerSingle.MB_DynamicGameObject> mbDynamicObjectsInCombinedMesh, out MB3_MeshCombinerSingle.BufferDataFromPreviousBake serializableBufferData, out MB3_MeshCombinerSingle.SerializableIntArray[] submeshTrisToUse, out int numNonZeroLengthSubmeshes);

			void AssignTriangleDataForSubmeshes(Mesh mesh, List<MB3_MeshCombinerSingle.MB_DynamicGameObject> mbDynamicObjectsInCombinedMesh, ref MB3_MeshCombinerSingle.BufferDataFromPreviousBake serializableBufferData, out MB3_MeshCombinerSingle.SerializableIntArray[] submeshTrisToUse, out int numNonZeroLengthSubmeshes);

			void AssignTriangleDataForSubmeshes_ShowHide(Mesh mesh, List<MB3_MeshCombinerSingle.MB_DynamicGameObject> mbDynamicObjectsInCombinedMesh, ref MB3_MeshCombinerSingle.BufferDataFromPreviousBake serializableBufferData, out MB3_MeshCombinerSingle.SerializableIntArray[] submeshTrisToUse, out int numNonZeroLengthSubmeshes);

			void CopyUV2unchangedToSeparateRects(List<MB3_MeshCombinerSingle.MB_DynamicGameObject> mbDynamicObjectsInCombinedMesh, float uv2UnwrappingParamsPackMargin);

			int[] GetTriangleSizes();
		}

		public class MB_MeshCombinerSingle_SubCombiner
		{
			public static void instance2Combined_MapAdd(ref Dictionary<GameObject, MB3_MeshCombinerSingle.MB_DynamicGameObject> _instance2combined_map, GameObject gameObjectID, MB3_MeshCombinerSingle.MB_DynamicGameObject dgo)
			{
				_instance2combined_map.Add(gameObjectID, dgo);
			}

			public static void instance2Combined_MapRemove(ref Dictionary<GameObject, MB3_MeshCombinerSingle.MB_DynamicGameObject> _instance2combined_map, GameObject gameObjectID)
			{
				_instance2combined_map.Remove(gameObjectID);
			}

			internal static bool _ShowHideGameObjects(MB3_MeshCombinerSingle c)
			{
				c._vertexAndTriProcessor.InitShowHide(c);
				return true;
			}

			internal static bool _AddToCombined(MB3_MeshCombinerSingle c, MB_MeshVertexChannelFlags newChannels, int totalAddVerts, int totalDeleteVerts, int numResultMats, int totalAddBlendShapes, int totalDeleteBlendShapes, int[] totalAddSubmeshTris, int[] totalDeleteSubmeshTris, int[] _goToDelete, List<MB3_MeshCombinerSingle.MB_DynamicGameObject> toAddDGOs, GameObject[] _goToAdd, MB3_MeshCombinerSingle.UVAdjuster_Atlas uvAdjuster, ref MB3_MeshCombinerSingle.IVertexAndTriangleProcessor oldMeshData, Stopwatch sw)
			{
				MB_IMeshCombinerSingle_BoneProcessor boneProcessor = c._boneProcessor;
				MB3_MeshCombinerSingle.MB_MeshCombinerSingle_BlendShapeProcessor blendShapeProcessor = c._blendShapeProcessor;
				MB3_MeshCombinerSingle.IMeshChannelsCacheTaggingInterface meshChannelsCache = c._meshChannelsCache;
				MB_IMeshBakerSettings settings = c.settings;
				MB2_LogLevel log_LEVEL = c.LOG_LEVEL;
				MB2_TextureBakeResults textureBakeResults = c.textureBakeResults;
				List<MB3_MeshCombinerSingle.MB_DynamicGameObject> mbDynamicObjectsInCombinedMesh = c.mbDynamicObjectsInCombinedMesh;
				List<GameObject> objectsInCombinedMesh = c.objectsInCombinedMesh;
				Dictionary<GameObject, MB3_MeshCombinerSingle.MB_DynamicGameObject> instance2combined_map = c._instance2combined_map;
				int uvChannelWithExtraParameter;
				if (c.settings.assignToMeshCustomizer != null && c.settings.assignToMeshCustomizer is IAssignToMeshCustomizer_NativeArrays)
				{
					uvChannelWithExtraParameter = ((IAssignToMeshCustomizer_NativeArrays)c.settings.assignToMeshCustomizer).UVchannelWithExtraParameter();
				}
				else
				{
					uvChannelWithExtraParameter = -1;
				}
				c.db_addDeleteGameObjects_InitFromMeshCombiner.Start();
				int num;
				int[] array;
				if (!settings.clearBuffersAfterBake && mbDynamicObjectsInCombinedMesh.Count > 0)
				{
					oldMeshData.InitFromMeshCombiner(c, newChannels, uvChannelWithExtraParameter);
					num = oldMeshData.GetVertexCount();
					array = oldMeshData.GetTriangleSizes();
				}
				else
				{
					num = 0;
					array = new int[numResultMats];
				}
				c.db_addDeleteGameObjects_InitFromMeshCombiner.Stop();
				c.db_addDeleteGameObjects_Init.Start();
				int num2 = num + totalAddVerts - totalDeleteVerts;
				int nBlendShapeSize = 0;
				if (settings.doBlendShapes)
				{
					nBlendShapeSize = c.blendShapes.Length + totalAddBlendShapes - totalDeleteBlendShapes;
				}
				if (log_LEVEL >= MB2_LogLevel.debug)
				{
					Debug.Log(string.Concat(new string[]
					{
						"Verts adding:",
						totalAddVerts.ToString(),
						" deleting:",
						totalDeleteVerts.ToString(),
						" submeshes:",
						numResultMats.ToString(),
						" blendShapes:",
						nBlendShapeSize.ToString()
					}));
				}
				int[] array2 = new int[numResultMats];
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i] = array[i] + totalAddSubmeshTris[i] - totalDeleteSubmeshTris[i];
					if (log_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug(string.Concat(new string[]
						{
							"    submesh :",
							i.ToString(),
							" already contains:",
							array[i].ToString(),
							" tris to be Added:",
							totalAddSubmeshTris[i].ToString(),
							" tris to be Deleted:",
							totalDeleteSubmeshTris[i].ToString()
						}), Array.Empty<object>());
					}
				}
				if (num2 >= MBVersion.MaxMeshVertexCount())
				{
					Debug.LogError("Cannot add objects. Resulting mesh will have more than " + MBVersion.MaxMeshVertexCount().ToString() + " vertices. Try using a Multi-MeshBaker component. This will split the combined mesh into several meshes. You don't have to re-configure the MB2_TextureBaker. Just remove the MB2_MeshBaker component and add a MB2_MultiMeshBaker component.");
					return false;
				}
				MB3_MeshCombinerSingle.IVertexAndTriangleProcessor vertexAndTriProcessor = c._vertexAndTriProcessor;
				vertexAndTriProcessor.Init(c, newChannels, num2, array2, uvChannelWithExtraParameter, meshChannelsCache, false, c.LOG_LEVEL);
				boneProcessor.AllocateAndSetupSMRDataStructures(toAddDGOs, mbDynamicObjectsInCombinedMesh, num2, c._vertexAndTriProcessor);
				blendShapeProcessor.AllocateBlendShapeArrayIfNecessary(nBlendShapeSize);
				c.db_addDeleteGameObjects_Init.Stop();
				if (log_LEVEL >= MB2_LogLevel.debug)
				{
					Debug.Log("Allocating buffers: " + vertexAndTriProcessor.channels.ToString() + "  vertexCount:" + num2.ToString());
				}
				mbDynamicObjectsInCombinedMesh.Sort();
				int num3 = 0;
				int num4 = 0;
				int[] array3 = new int[numResultMats];
				int num5 = 0;
				c.db_addDeleteGameObjects_CopyArraysFromPreviousBakeBuffersToNewBuffers.Start();
				if (!settings.clearBuffersAfterBake && mbDynamicObjectsInCombinedMesh.Count > 0)
				{
					for (int j = 0; j < mbDynamicObjectsInCombinedMesh.Count; j++)
					{
						MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject = mbDynamicObjectsInCombinedMesh[j];
						if (!mb_DynamicGameObject._beingDeleted)
						{
							if (log_LEVEL >= MB2_LogLevel.debug)
							{
								MB2_Log.LogDebug("Copying obj in combined arrays idx:" + j.ToString(), new object[]
								{
									log_LEVEL
								});
							}
							vertexAndTriProcessor.CopyArraysFromPreviousBakeBuffersToNewBuffers(mb_DynamicGameObject, ref oldMeshData, num4, num5, array3, log_LEVEL);
							if (settings.doBlendShapes)
							{
								blendShapeProcessor.CopyBlendShapesInCurrentMeshIfNecessary(ref num3, mb_DynamicGameObject);
							}
							if (settings.renderType == MB_RenderType.skinnedMeshRenderer)
							{
								boneProcessor.CopyBoneWeightsFromMeshForDGOsInCombined(mb_DynamicGameObject, num4);
							}
							mb_DynamicGameObject.vertIdx = num4;
							for (int k = 0; k < array3.Length; k++)
							{
								mb_DynamicGameObject.submeshTriIdxs[k] = array3[k];
								array3[k] += mb_DynamicGameObject.submeshNumTris[k];
							}
							num4 += mb_DynamicGameObject.numVerts;
						}
						else
						{
							if (log_LEVEL >= MB2_LogLevel.debug)
							{
								MB2_Log.LogDebug("Not copying obj: " + j.ToString(), new object[]
								{
									log_LEVEL
								});
							}
							num5 += mb_DynamicGameObject.numVerts;
						}
					}
					if (settings.renderType == MB_RenderType.skinnedMeshRenderer)
					{
						boneProcessor.CopyBonesWeAreKeepingToNewBonesArrayAndAdjustBWIndexes(totalDeleteVerts);
					}
					for (int l = mbDynamicObjectsInCombinedMesh.Count - 1; l >= 0; l--)
					{
						if (mbDynamicObjectsInCombinedMesh[l]._beingDeleted)
						{
							MB3_MeshCombinerSingle.MB_MeshCombinerSingle_SubCombiner.instance2Combined_MapRemove(ref instance2combined_map, mbDynamicObjectsInCombinedMesh[l].gameObject);
							objectsInCombinedMesh.RemoveAt(l);
							mbDynamicObjectsInCombinedMesh.RemoveAt(l);
						}
					}
				}
				c.db_addDeleteGameObjects_CopyArraysFromPreviousBakeBuffersToNewBuffers.Stop();
				c.db_addDeleteGameObjects_CopyFromDGOMeshToBuffers.Start();
				if (settings.renderType == MB_RenderType.skinnedMeshRenderer)
				{
					boneProcessor.InsertNewBonesIntoBonesArray();
				}
				for (int m = 0; m < toAddDGOs.Count; m++)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject2 = toAddDGOs[m];
					GameObject gameObject = _goToAdd[m];
					int vertsIdx = num4;
					Mesh mesh = mb_DynamicGameObject2._mesh;
					bool updateBWdata = false;
					vertexAndTriProcessor.CopyFromDGOMeshToBuffers(mb_DynamicGameObject2, num4, vertexAndTriProcessor.channels, true, updateBWdata, settings, boneProcessor, array3, textureBakeResults, uvAdjuster, log_LEVEL, meshChannelsCache);
					int subMeshCount = mesh.subMeshCount;
					if (mb_DynamicGameObject2.uvRects.Length < subMeshCount)
					{
						if (log_LEVEL >= MB2_LogLevel.debug)
						{
							MB2_Log.LogDebug("Mesh " + mb_DynamicGameObject2.name + " has more submeshes than materials", Array.Empty<object>());
						}
					}
					else if (mb_DynamicGameObject2.uvRects.Length > subMeshCount && log_LEVEL >= MB2_LogLevel.warn)
					{
						Debug.LogWarning("Mesh " + mb_DynamicGameObject2.name + " has fewer submeshes than materials");
					}
					if (settings.doBlendShapes)
					{
						blendShapeProcessor.CopyBlendShapesForNewMeshIfNecessary(ref num3, mb_DynamicGameObject2, mesh, meshChannelsCache);
					}
					if (settings.renderType == MB_RenderType.skinnedMeshRenderer)
					{
						boneProcessor.AddBonesToNewBonesArrayAndAdjustBWIndexes1(mb_DynamicGameObject2, vertsIdx);
					}
					mb_DynamicGameObject2.vertIdx = num4;
					MB3_MeshCombinerSingle.MB_MeshCombinerSingle_SubCombiner.instance2Combined_MapAdd(ref instance2combined_map, gameObject, mb_DynamicGameObject2);
					objectsInCombinedMesh.Add(gameObject);
					mbDynamicObjectsInCombinedMesh.Add(mb_DynamicGameObject2);
					num4 += mb_DynamicGameObject2.numVerts;
					for (int n = 0; n < mb_DynamicGameObject2._tmpSubmeshTris.Length; n++)
					{
						mb_DynamicGameObject2._tmpSubmeshTris[n] = null;
					}
					mb_DynamicGameObject2._tmpSubmeshTris = null;
					if (log_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug(string.Concat(new string[]
						{
							"Added to combined:",
							mb_DynamicGameObject2.name,
							" verts:",
							vertexAndTriProcessor.GetVertexCount().ToString(),
							" bindPoses:",
							boneProcessor.GetNewBonesSize().ToString()
						}), new object[]
						{
							log_LEVEL
						});
					}
				}
				if (settings.lightmapOption == MB2_LightmapOptions.copy_UV2_unchanged_to_separate_rects)
				{
					vertexAndTriProcessor.CopyUV2unchangedToSeparateRects(mbDynamicObjectsInCombinedMesh, settings.uv2UnwrappingParamsPackMargin);
				}
				if (log_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug("===== _addToCombined completed. Verts in buffer: " + vertexAndTriProcessor.GetVertexCount().ToString() + " time(ms): " + sw.ElapsedMilliseconds.ToString(), new object[]
					{
						log_LEVEL
					});
				}
				c.db_addDeleteGameObjects_CopyFromDGOMeshToBuffers.Stop();
				return true;
			}

			public static bool _UpdateGameObjects(MB3_MeshCombinerSingle combiner, List<MB3_MeshCombinerSingle.MB_DynamicGameObject> dgosToUpdate, MB_MeshVertexChannelFlags newChannels, bool updateVertices, bool updateNormals, bool updateTangents, bool updateUV, bool updateUV2, bool updateUV3, bool updateUV4, bool updateUV5, bool updateUV6, bool updateUV7, bool updateUV8, bool updateColors, bool updateSkinningInfo, MB3_MeshCombinerSingle.UVAdjuster_Atlas uVAdjuster, MB2_LogLevel LOG_LEVEL)
			{
				MB3_MeshCombinerSingle.IMeshChannelsCacheTaggingInterface meshChannelsCache = combiner._meshChannelsCache;
				MB_IMeshBakerSettings settings = combiner.settings;
				MB3_MeshCombinerSingle.IVertexAndTriangleProcessor vertexAndTriProcessor = combiner._vertexAndTriProcessor;
				int uvChannelWithExtraParameter;
				if (combiner.settings.assignToMeshCustomizer != null && combiner.settings.assignToMeshCustomizer is IAssignToMeshCustomizer_NativeArrays)
				{
					uvChannelWithExtraParameter = ((IAssignToMeshCustomizer_NativeArrays)combiner.settings.assignToMeshCustomizer).UVchannelWithExtraParameter();
				}
				else
				{
					uvChannelWithExtraParameter = -1;
				}
				vertexAndTriProcessor.Init(combiner, newChannels, combiner._mesh.vertexCount, new int[0], uvChannelWithExtraParameter, combiner._meshChannelsCache, true, LOG_LEVEL);
				if (settings.renderType == MB_RenderType.skinnedMeshRenderer && updateSkinningInfo)
				{
					combiner._boneProcessor.UpdateGameObjects_ReadBoneWeightInfoFromCombinedMesh();
				}
				MB_MeshVertexChannelFlags channelsToUpdate = (updateVertices ? MB_MeshVertexChannelFlags.vertex : MB_MeshVertexChannelFlags.none) | (updateNormals ? MB_MeshVertexChannelFlags.normal : MB_MeshVertexChannelFlags.none) | (updateTangents ? MB_MeshVertexChannelFlags.tangent : MB_MeshVertexChannelFlags.none) | (updateColors ? MB_MeshVertexChannelFlags.colors : MB_MeshVertexChannelFlags.none) | (updateUV ? MB_MeshVertexChannelFlags.uv0 : MB_MeshVertexChannelFlags.none) | (updateUV2 ? MB_MeshVertexChannelFlags.uv2 : MB_MeshVertexChannelFlags.none) | (updateUV3 ? MB_MeshVertexChannelFlags.uv3 : MB_MeshVertexChannelFlags.none) | (updateUV4 ? MB_MeshVertexChannelFlags.uv4 : MB_MeshVertexChannelFlags.none) | (updateUV5 ? MB_MeshVertexChannelFlags.uv5 : MB_MeshVertexChannelFlags.none) | (updateUV6 ? MB_MeshVertexChannelFlags.uv6 : MB_MeshVertexChannelFlags.none) | (updateUV7 ? MB_MeshVertexChannelFlags.uv7 : MB_MeshVertexChannelFlags.none) | (updateUV8 ? MB_MeshVertexChannelFlags.uv8 : MB_MeshVertexChannelFlags.none);
				for (int i = 0; i < dgosToUpdate.Count; i++)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject = dgosToUpdate[i];
					bool updateBWdata = settings.renderType == MB_RenderType.skinnedMeshRenderer && updateSkinningInfo;
					vertexAndTriProcessor.CopyFromDGOMeshToBuffers(mb_DynamicGameObject, mb_DynamicGameObject.vertIdx, channelsToUpdate, false, updateBWdata, settings, combiner._boneProcessor, null, combiner.textureBakeResults, uVAdjuster, LOG_LEVEL, meshChannelsCache);
					mb_DynamicGameObject.UnInitialize();
				}
				combiner._bakeStatus = MB3_MeshCombiner.MeshCombiningStatus.readyForApply;
				if (settings.renderType == MB_RenderType.skinnedMeshRenderer)
				{
					((SkinnedMeshRenderer)combiner.targetRenderer).sharedMesh = null;
					((SkinnedMeshRenderer)combiner.targetRenderer).sharedMesh = combiner._mesh;
				}
				return true;
			}

			public static bool Apply(MB3_MeshCombinerSingle combiner, MB3_MeshCombiner.GenerateUV2Delegate uv2GenerationMethod)
			{
				MB_IMeshBakerSettings settings = combiner.settings;
				bool bones = false;
				if (settings.renderType == MB_RenderType.skinnedMeshRenderer)
				{
					bones = true;
				}
				return MB3_MeshCombinerSingle.MB_MeshCombinerSingle_SubCombiner.Apply(combiner, true, true, settings.doNorm, settings.doTan, settings.doUV, MeshBakerSettingsUtility.DoUV2getDataFromSourceMeshes(ref settings), settings.doUV3, settings.doUV4, settings.doUV5, settings.doUV6, settings.doUV7, settings.doUV8, settings.doCol, bones, settings.doBlendShapes, false, uv2GenerationMethod);
			}

			public static bool Apply(MB3_MeshCombinerSingle combiner, bool triangles, bool vertices, bool normals, bool tangents, bool uvs, bool uv2, bool uv3, bool uv4, bool colors, bool bones = false, bool blendShapesFlag = false, MB3_MeshCombiner.GenerateUV2Delegate uv2GenerationMethod = null)
			{
				return MB3_MeshCombinerSingle.MB_MeshCombinerSingle_SubCombiner.Apply(combiner, triangles, vertices, normals, tangents, uvs, uv2, uv3, uv4, false, false, false, false, colors, bones, blendShapesFlag, false, uv2GenerationMethod);
			}

			internal static bool Apply(MB3_MeshCombinerSingle combiner, bool triangles, bool vertices, bool normals, bool tangents, bool uvs, bool uv2, bool uv3, bool uv4, bool uv5, bool uv6, bool uv7, bool uv8, bool colors, bool bones = false, bool blendShapesFlag = false, bool suppressClearMesh = false, MB3_MeshCombiner.GenerateUV2Delegate uv2GenerationMethod = null)
			{
				MB2_LogLevel log_LEVEL = combiner.LOG_LEVEL;
				int validationLevel = (int)combiner._validationLevel;
				MB_IMeshBakerSettings settings = combiner.settings;
				bool flag = false;
				if (bones && combiner._boneProcessor == null)
				{
					Debug.LogError("Apply was called with 'bones = true', but the meshCombiner did not contain valid bone data. Was AddDelete(...), Update(...) or ShowHide() called with 'renderType = skinnedMeshRenderer'?");
					flag = true;
				}
				if (validationLevel >= 1 && !combiner.ValidateTargRendererAndMeshAndResultSceneObj())
				{
					flag = true;
				}
				if (combiner._bakeStatus != MB3_MeshCombiner.MeshCombiningStatus.readyForApply)
				{
					Debug.LogError("Apply was called when combiner was not in 'readyForApply' state. Did you call AddDelete(), Update() or ShowHide()");
					flag = true;
				}
				if (combiner._vertexAndTriProcessor != null && combiner._vertexAndTriProcessor.IsDisposed() && combiner._vertexAndTriProcessor.IsInitialized())
				{
					Debug.LogError("Apply was called with bad meshDataBuffer");
					flag = true;
				}
				if (flag)
				{
					return false;
				}
				Mesh mesh = combiner._mesh;
				Renderer targetRenderer = combiner.targetRenderer;
				MB2_TextureBakeResults textureBakeResults = combiner._textureBakeResults;
				MB2_TextureBakeResults textureBakeResults2 = combiner.textureBakeResults;
				Stopwatch stopwatch = null;
				if (log_LEVEL >= MB2_LogLevel.debug)
				{
					stopwatch = new Stopwatch();
					stopwatch.Start();
				}
				if (mesh != null)
				{
					MB3_MeshCombinerSingle.IVertexAndTriangleProcessor vertexAndTriProcessor = combiner._vertexAndTriProcessor;
					if (log_LEVEL >= MB2_LogLevel.trace)
					{
						Debug.Log(string.Format("Apply called:\n tri={0}\n vert={1}\n norm={2}\n tan={3}\n uv={4}\n col={5}\n uv3={6}\n uv4={7}\n uv2={8}\n bone={9}\n blendShape{10}\n meshID={11}\n", new object[]
						{
							triangles,
							vertices,
							normals,
							tangents,
							uvs,
							colors,
							uv3,
							uv4,
							uv2,
							bones,
							blendShapesFlag,
							mesh.GetInstanceID()
						}));
					}
					if (!suppressClearMesh && (triangles || mesh.vertexCount != vertexAndTriProcessor.GetVertexCount()))
					{
						bool justClearTriangles = triangles && !vertices && !normals && !tangents && !uvs && !colors && !uv3 && !uv4 && !uv2 && !bones;
						MBVersion.SetMeshIndexFormatAndClearMesh(mesh, vertexAndTriProcessor.GetVertexCount(), vertices, justClearTriangles);
					}
					MB_MeshVertexChannelFlags mb_MeshVertexChannelFlags = MB_MeshVertexChannelFlags.none;
					bool flag2 = false;
					if (vertices)
					{
						mb_MeshVertexChannelFlags |= MB_MeshVertexChannelFlags.vertex;
					}
					if (triangles && textureBakeResults)
					{
						if (textureBakeResults == null)
						{
							Debug.LogError("Texture Bake Result was not set.");
						}
						else
						{
							flag2 = true;
						}
					}
					if (normals)
					{
						if (settings.doNorm)
						{
							mb_MeshVertexChannelFlags |= MB_MeshVertexChannelFlags.normal;
						}
						else
						{
							Debug.LogError("normal flag was set in Apply but MeshBaker didn't generate normals");
						}
					}
					if (tangents)
					{
						if (settings.doTan)
						{
							mb_MeshVertexChannelFlags |= MB_MeshVertexChannelFlags.tangent;
						}
						else
						{
							Debug.LogError("tangent flag was set in Apply but MeshBaker didn't generate tangents");
						}
					}
					if (colors)
					{
						if (settings.doCol)
						{
							mb_MeshVertexChannelFlags |= MB_MeshVertexChannelFlags.colors;
						}
						else
						{
							Debug.LogError("color flag was set in Apply but MeshBaker didn't generate colors");
						}
					}
					if (uvs)
					{
						if (settings.doUV)
						{
							mb_MeshVertexChannelFlags |= MB_MeshVertexChannelFlags.uv0;
						}
						else
						{
							Debug.LogError("uv flag was set in Apply but MeshBaker didn't generate uvs");
						}
					}
					if (uv2)
					{
						if (MeshBakerSettingsUtility.DoUV2getDataFromSourceMeshes(ref settings))
						{
							mb_MeshVertexChannelFlags |= MB_MeshVertexChannelFlags.uv2;
						}
						else
						{
							Debug.LogError("uv2 flag was set in Apply but lightmapping option was set to " + settings.lightmapOption.ToString());
						}
					}
					if (uv3)
					{
						if (settings.doUV3)
						{
							mb_MeshVertexChannelFlags |= MB_MeshVertexChannelFlags.uv3;
						}
						else
						{
							Debug.LogError("uv3 flag was set in Apply but MeshBaker didn't generate uv3s");
						}
					}
					if (uv4)
					{
						if (settings.doUV4)
						{
							mb_MeshVertexChannelFlags |= MB_MeshVertexChannelFlags.uv4;
						}
						else
						{
							Debug.LogError("uv4 flag was set in Apply but MeshBaker didn't generate uv4s");
						}
					}
					if (uv5)
					{
						if (settings.doUV5)
						{
							mb_MeshVertexChannelFlags |= MB_MeshVertexChannelFlags.uv5;
						}
						else
						{
							Debug.LogError("uv5 flag was set in Apply but MeshBaker didn't generate uv5s");
						}
					}
					if (uv6)
					{
						if (settings.doUV6)
						{
							mb_MeshVertexChannelFlags |= MB_MeshVertexChannelFlags.uv6;
						}
						else
						{
							Debug.LogError("uv6 flag was set in Apply but MeshBaker didn't generate uv6s");
						}
					}
					if (uv7)
					{
						if (settings.doUV7)
						{
							mb_MeshVertexChannelFlags |= MB_MeshVertexChannelFlags.uv7;
						}
						else
						{
							Debug.LogError("uv7 flag was set in Apply but MeshBaker didn't generate uv7s");
						}
					}
					if (uv8)
					{
						if (settings.doUV8)
						{
							mb_MeshVertexChannelFlags |= MB_MeshVertexChannelFlags.uv8;
						}
						else
						{
							Debug.LogError("uv8 flag was set in Apply but MeshBaker didn't generate uv8s");
						}
					}
					if (bones)
					{
						combiner._boneProcessor.ApplySMRdataToMeshToBuffer();
					}
					MB3_MeshCombinerSingle.BufferDataFromPreviousBake bufferDataFromPreviousBake;
					MB3_MeshCombinerSingle.SerializableIntArray[] subTris;
					int numNonZeroLengthSubmeshTris;
					vertexAndTriProcessor.AssignBuffersToMesh(mesh, settings, textureBakeResults2, mb_MeshVertexChannelFlags, flag2, settings.assignToMeshCustomizer, combiner.mbDynamicObjectsInCombinedMesh, out bufferDataFromPreviousBake, out subTris, out numNonZeroLengthSubmeshTris);
					vertexAndTriProcessor.TransferOwnershipOfSerializableBuffersToCombiner(combiner, vertexAndTriProcessor.channels, bufferDataFromPreviousBake);
					vertexAndTriProcessor.Dispose();
					if ((mb_MeshVertexChannelFlags & MB_MeshVertexChannelFlags.vertex) == MB_MeshVertexChannelFlags.vertex)
					{
						targetRenderer.transform.position = bufferDataFromPreviousBake.meshVerticesShift;
					}
					if (flag2)
					{
						MB3_MeshCombinerSingle._UpdateMaterialsOnTargetRenderer(combiner.textureBakeResults, combiner.targetRenderer, subTris, numNonZeroLengthSubmeshTris);
					}
					bool flag3 = false;
					if (settings.renderType != MB_RenderType.skinnedMeshRenderer && settings.lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout)
					{
						if (uv2GenerationMethod != null)
						{
							uv2GenerationMethod(mesh, settings.uv2UnwrappingParamsHardAngle, settings.uv2UnwrappingParamsPackMargin);
							if (log_LEVEL >= MB2_LogLevel.trace)
							{
								Debug.Log("generating new UV2 layout for the combined mesh ");
							}
						}
						else
						{
							Debug.LogError("No GenerateUV2Delegate method was supplied. UV2 cannot be generated.");
						}
						flag3 = true;
					}
					else if (settings.renderType == MB_RenderType.skinnedMeshRenderer && settings.lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout && log_LEVEL >= MB2_LogLevel.warn)
					{
						Debug.LogWarning("UV2 cannot be generated for SkinnedMeshRenderer objects.");
					}
					if (settings.renderType != MB_RenderType.skinnedMeshRenderer && settings.lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout && !flag3)
					{
						Debug.LogError("Failed to generate new UV2 layout. Only works in editor.");
					}
					if (bones)
					{
						combiner._boneProcessor.ApplySMRdataToMesh(combiner, mesh);
						combiner._boneProcessor.Dispose();
						combiner._boneProcessor = null;
					}
					if (blendShapesFlag)
					{
						combiner._blendShapeProcessor.AssignNewBlendShapesToCombinerIfNecessary();
						if (settings.smrMergeBlendShapesWithSameNames)
						{
							combiner._blendShapeProcessor.ApplyBlendShapeFramesToMeshAndBuildMap_MergeBlendShapesWithTheSameName(combiner._mesh.vertexCount);
						}
						else
						{
							combiner._blendShapeProcessor.ApplyBlendShapeFramesToMeshAndBuildMap(combiner._mesh.vertexCount);
						}
						combiner._blendShapeProcessor.Dispose();
						combiner._blendShapeProcessor = null;
					}
					if (triangles || vertices)
					{
						if (log_LEVEL >= MB2_LogLevel.trace)
						{
							Debug.Log("recalculating bounds on mesh.");
						}
						mesh.RecalculateBounds();
					}
					if (settings.optimizeAfterBake && !Application.isPlaying)
					{
						MBVersion.OptimizeMesh(mesh);
					}
					combiner._SetLightmapIndexIfPreserveLightmapping(targetRenderer);
					if (settings.renderType == MB_RenderType.skinnedMeshRenderer)
					{
						if (combiner._mesh.vertexCount == 0)
						{
							if (log_LEVEL >= MB2_LogLevel.debug)
							{
								Debug.Log(" combined mesh had zero vertices. Disabling combined SkinnedMeshRenderer.");
							}
							targetRenderer.enabled = false;
						}
						else
						{
							targetRenderer.enabled = true;
							SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)targetRenderer;
							bool updateWhenOffscreen = skinnedMeshRenderer.updateWhenOffscreen;
							skinnedMeshRenderer.updateWhenOffscreen = true;
							skinnedMeshRenderer.updateWhenOffscreen = updateWhenOffscreen;
							skinnedMeshRenderer.sharedMesh = null;
							skinnedMeshRenderer.sharedMesh = mesh;
							skinnedMeshRenderer.bones = combiner.bones;
							if (log_LEVEL >= MB2_LogLevel.debug)
							{
								Debug.Log(" Applying bones and mesh to SkinnedMeshRenderer component  numbones: " + combiner.bones.Length.ToString());
							}
							MB3_MeshCombiner.UpdateSkinnedMeshApproximateBoundsFromBoundsStatic(combiner.objectsInCombinedMesh, skinnedMeshRenderer);
						}
					}
					combiner._boneProcessor = null;
				}
				else
				{
					Debug.LogError("Need to add objects to this meshbaker before calling Apply or ApplyAll");
				}
				if (log_LEVEL >= MB2_LogLevel.debug)
				{
					Debug.Log("Apply Complete time: " + stopwatch.ElapsedMilliseconds.ToString() + " vertices: " + mesh.vertexCount.ToString());
				}
				combiner._bakeStatus = MB3_MeshCombiner.MeshCombiningStatus.preAddDeleteOrUpdate;
				if (settings.clearBuffersAfterBake)
				{
					combiner.ClearBuffers();
				}
				return true;
			}

			public static bool ApplyShowHide(MB3_MeshCombinerSingle combiner)
			{
				MB_IMeshBakerSettings settings = combiner.settings;
				Renderer targetRenderer = combiner.targetRenderer;
				bool flag = false;
				if (combiner._bakeStatus != MB3_MeshCombiner.MeshCombiningStatus.readyForApply)
				{
					Debug.LogError("Apply was called when combiner was not in 'readyForApply' state. Did you call AddDelete(), Update() or ShowHide()");
					flag = true;
				}
				if (combiner._vertexAndTriProcessor != null && combiner._vertexAndTriProcessor.IsDisposed() && combiner._vertexAndTriProcessor.IsInitialized())
				{
					Debug.LogError("Apply was called with bad meshDataBuffer");
					flag = true;
				}
				if (flag)
				{
					return false;
				}
				MB3_MeshCombinerSingle.IVertexAndTriangleProcessor vertexAndTriProcessor = combiner._vertexAndTriProcessor;
				MB3_MeshCombinerSingle.BufferDataFromPreviousBake bufferDataFromPrevious = combiner.bufferDataFromPrevious;
				MB3_MeshCombinerSingle.SerializableIntArray[] subTris;
				int numNonZeroLengthSubmeshTris;
				vertexAndTriProcessor.AssignTriangleDataForSubmeshes_ShowHide(combiner._mesh, combiner.mbDynamicObjectsInCombinedMesh, ref bufferDataFromPrevious, out subTris, out numNonZeroLengthSubmeshTris);
				vertexAndTriProcessor.TransferOwnershipOfSerializableBuffersToCombiner(combiner, MB_MeshVertexChannelFlags.none, bufferDataFromPrevious);
				vertexAndTriProcessor.Dispose();
				MB3_MeshCombinerSingle._UpdateMaterialsOnTargetRenderer(combiner.textureBakeResults, combiner.targetRenderer, subTris, numNonZeroLengthSubmeshTris);
				if (settings.renderType == MB_RenderType.skinnedMeshRenderer)
				{
					if (combiner._mesh.vertexCount == 0)
					{
						if (combiner.LOG_LEVEL >= MB2_LogLevel.debug)
						{
							Debug.Log(" combined mesh had zero vertices. Disabling combined SkinnedMeshRenderer.");
						}
						targetRenderer.enabled = false;
					}
					else
					{
						targetRenderer.enabled = true;
						SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)targetRenderer;
						bool updateWhenOffscreen = skinnedMeshRenderer.updateWhenOffscreen;
						skinnedMeshRenderer.updateWhenOffscreen = true;
						skinnedMeshRenderer.updateWhenOffscreen = updateWhenOffscreen;
						skinnedMeshRenderer.sharedMesh = null;
						skinnedMeshRenderer.sharedMesh = combiner._mesh;
						skinnedMeshRenderer.bones = combiner.bones;
						if (combiner.LOG_LEVEL >= MB2_LogLevel.debug)
						{
							Debug.Log(" Applying bones and mesh to SkinnedMeshRenderer component  numbones: " + combiner.bones.Length.ToString());
						}
						MB3_MeshCombiner.UpdateSkinnedMeshApproximateBoundsFromBoundsStatic(combiner.objectsInCombinedMesh, skinnedMeshRenderer);
					}
				}
				if (combiner.LOG_LEVEL >= MB2_LogLevel.trace)
				{
					Debug.Log("ApplyShowHide");
				}
				return true;
			}
		}

		public class UVAdjuster_Atlas
		{
			public UVAdjuster_Atlas(MB2_TextureBakeResults tbr, MB2_LogLevel ll)
			{
				this.textureBakeResults = tbr;
				this.LOG_LEVEL = ll;
				this.matsAndSrcUVRect = tbr.materialsAndUVRects;
				this.compareNamesWhenComparingMaterials = false;
				if (MBVersion.IsUsingAddressables() && Application.isPlaying)
				{
					this.compareNamesWhenComparingMaterials = true;
				}
				else
				{
					this.compareNamesWhenComparingMaterials = false;
				}
				this.numTimesMatAppearsInAtlas = new int[this.matsAndSrcUVRect.Length];
				for (int i = 0; i < this.matsAndSrcUVRect.Length; i++)
				{
					if (this.numTimesMatAppearsInAtlas[i] <= 1)
					{
						int num = 1;
						for (int j = i + 1; j < this.matsAndSrcUVRect.Length; j++)
						{
							if (this.matsAndSrcUVRect[i].material == this.matsAndSrcUVRect[j].material)
							{
								num++;
							}
						}
						this.numTimesMatAppearsInAtlas[i] = num;
						if (num > 1)
						{
							for (int k = i + 1; k < this.matsAndSrcUVRect.Length; k++)
							{
								if (this.matsAndSrcUVRect[i].material == this.matsAndSrcUVRect[k].material)
								{
									this.numTimesMatAppearsInAtlas[k] = num;
								}
							}
						}
					}
				}
			}

			public bool MapSharedMaterialsToAtlasRects(Material[] sharedMaterials, bool checkTargetSubmeshIdxsFromPreviousBake, Mesh m, MB3_MeshCombinerSingle.IMeshChannelsCacheTaggingInterface meshChannelsCache, Dictionary<int, MB_Utility.MeshAnalysisResult[]> meshAnalysisResultsCache, OrderedDictionary sourceMats2submeshIdx_map, GameObject go, MB3_MeshCombinerSingle.MB_DynamicGameObject dgoOut)
			{
				MB_TextureTilingTreatment[] array = new MB_TextureTilingTreatment[sharedMaterials.Length];
				Rect[] array2 = new Rect[sharedMaterials.Length];
				Rect[] array3 = new Rect[sharedMaterials.Length];
				Rect[] array4 = new Rect[sharedMaterials.Length];
				int[] array5 = new int[sharedMaterials.Length];
				string message = "";
				for (int i = 0; i < sharedMaterials.Length; i++)
				{
					object obj = null;
					foreach (object obj2 in sourceMats2submeshIdx_map)
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)obj2;
						if (this.IsSameMaterialInTextureBakeResult(sharedMaterials[i], (Material)dictionaryEntry.Key))
						{
							obj = (int)dictionaryEntry.Value;
						}
					}
					if (obj == null)
					{
						string[] array6 = new string[5];
						array6[0] = "Source object ";
						array6[1] = go.name;
						array6[2] = " used a material ";
						int num = 3;
						Material material = sharedMaterials[i];
						array6[num] = ((material != null) ? material.ToString() : null);
						array6[4] = " that was not in the baked materials.";
						Debug.LogError(string.Concat(array6));
						if (sharedMaterials[i].name.Contains("(Instance)"))
						{
							Debug.LogError("The material may be a duplicate of a material that was baked. Materials on a Renderer can be duplicated if the .material field is accessed by a script.");
						}
						return false;
					}
					int num2 = (int)obj;
					if (checkTargetSubmeshIdxsFromPreviousBake && num2 != dgoOut.targetSubmeshIdxs[i])
					{
						Debug.LogError(string.Format("Update failed for object {0}. Material {1} is mapped to a different submesh in the combined mesh than the previous material. This is not supported. Try using AddDelete.", go.name, sharedMaterials[i]));
						return false;
					}
					if (!this.TryMapMaterialToUVRect(sharedMaterials[i], m, i, num2, meshChannelsCache, meshAnalysisResultsCache, out array[i], out array2[i], out array3[i], out array4[i], out array5[i], ref message, this.LOG_LEVEL))
					{
						Debug.LogError(message);
						return false;
					}
				}
				dgoOut.uvRects = array2;
				dgoOut.encapsulatingRect = array3;
				dgoOut.sourceMaterialTiling = array4;
				dgoOut.textureArraySliceIdx = array5;
				return true;
			}

			public bool IsSameMaterialInTextureBakeResult(Material a, Material b)
			{
				return a == b || (this.compareNamesWhenComparingMaterials && a != null && b != null && a.name.Equals(b.name));
			}

			public bool TryMapMaterialToUVRect(Material mat, Mesh m, int submeshIdx, int idxInResultMats, MB3_MeshCombinerSingle.IMeshChannelsCacheTaggingInterface meshChannelCache, Dictionary<int, MB_Utility.MeshAnalysisResult[]> meshAnalysisCache, out MB_TextureTilingTreatment tilingTreatment, out Rect rectInAtlas, out Rect encapsulatingRectOut, out Rect sourceMaterialTilingOut, out int sliceIdx, ref string errorMsg, MB2_LogLevel logLevel)
			{
				if (this.textureBakeResults.version < MB2_TextureBakeResults.VERSION)
				{
					this.textureBakeResults.UpgradeToCurrentVersion(this.textureBakeResults);
				}
				tilingTreatment = MB_TextureTilingTreatment.unknown;
				if (this.textureBakeResults.materialsAndUVRects.Length == 0)
				{
					errorMsg = "The 'Texture Bake Result' needs to be re-baked to be compatible with this version of Mesh Baker. Please re-bake using the MB3_TextureBaker.";
					rectInAtlas = default(Rect);
					encapsulatingRectOut = default(Rect);
					sourceMaterialTilingOut = default(Rect);
					sliceIdx = -1;
					return false;
				}
				if (mat == null)
				{
					rectInAtlas = default(Rect);
					encapsulatingRectOut = default(Rect);
					sourceMaterialTilingOut = default(Rect);
					sliceIdx = -1;
					errorMsg = string.Format("Mesh {0} Had no material on submesh {1} cannot map to a material in the atlas", m.name, submeshIdx);
					return false;
				}
				if (submeshIdx >= m.subMeshCount)
				{
					errorMsg = "Submesh index is greater than the number of submeshes";
					rectInAtlas = default(Rect);
					encapsulatingRectOut = default(Rect);
					sourceMaterialTilingOut = default(Rect);
					sliceIdx = -1;
					return false;
				}
				int num = -1;
				for (int i = 0; i < this.matsAndSrcUVRect.Length; i++)
				{
					if (this.IsSameMaterialInTextureBakeResult(mat, this.matsAndSrcUVRect[i].material))
					{
						num = i;
						break;
					}
				}
				if (num == -1)
				{
					rectInAtlas = default(Rect);
					encapsulatingRectOut = default(Rect);
					sourceMaterialTilingOut = default(Rect);
					sliceIdx = -1;
					errorMsg = string.Format("Material {0} could not be found in the Texture Bake Result", mat.name);
					return false;
				}
				if (!this.textureBakeResults.GetConsiderMeshUVs(idxInResultMats, mat))
				{
					if (this.numTimesMatAppearsInAtlas[num] != 1)
					{
						string str = "There is a problem with this TextureBakeResults. FixOutOfBoundsUVs is false and a material appears more than once: ";
						Material material = this.matsAndSrcUVRect[num].material;
						Debug.LogError(str + ((material != null) ? material.ToString() : null) + " appears: " + this.numTimesMatAppearsInAtlas[num].ToString());
					}
					MB_MaterialAndUVRect mb_MaterialAndUVRect = this.matsAndSrcUVRect[num];
					rectInAtlas = mb_MaterialAndUVRect.atlasRect;
					tilingTreatment = mb_MaterialAndUVRect.tilingTreatment;
					encapsulatingRectOut = mb_MaterialAndUVRect.GetEncapsulatingRect();
					sourceMaterialTilingOut = mb_MaterialAndUVRect.GetMaterialTilingRect();
					sliceIdx = mb_MaterialAndUVRect.textureArraySliceIdx;
					return true;
				}
				MB_Utility.MeshAnalysisResult[] array;
				if (!meshAnalysisCache.TryGetValue(m.GetInstanceID(), out array))
				{
					array = new MB_Utility.MeshAnalysisResult[m.subMeshCount];
					for (int j = 0; j < m.subMeshCount; j++)
					{
						meshChannelCache.hasOutOfBoundsUVs(m, ref array[j], j);
					}
					meshAnalysisCache.Add(m.GetInstanceID(), array);
				}
				bool flag = false;
				Rect samplingEncapsulatinRect = new Rect(0f, 0f, 0f, 0f);
				Rect allPropsUseSameTiling_sourceMaterialTiling = new Rect(0f, 0f, 0f, 0f);
				if (logLevel >= MB2_LogLevel.trace)
				{
					Debug.Log(string.Format("Trying to find a rectangle in atlas capable of holding tiled sampling rect for mesh {0} using material {1} meshUVrect={2}", m, mat, array[submeshIdx].uvRect.ToString("f5")));
				}
				for (int k = num; k < this.matsAndSrcUVRect.Length; k++)
				{
					MB_MaterialAndUVRect mb_MaterialAndUVRect2 = this.matsAndSrcUVRect[k];
					if (this.IsSameMaterialInTextureBakeResult(mat, mb_MaterialAndUVRect2.material))
					{
						if (mb_MaterialAndUVRect2.allPropsUseSameTiling)
						{
							samplingEncapsulatinRect = mb_MaterialAndUVRect2.allPropsUseSameTiling_samplingEncapsulatinRect;
							allPropsUseSameTiling_sourceMaterialTiling = mb_MaterialAndUVRect2.allPropsUseSameTiling_sourceMaterialTiling;
						}
						else
						{
							samplingEncapsulatinRect = mb_MaterialAndUVRect2.propsUseDifferntTiling_srcUVsamplingRect;
							allPropsUseSameTiling_sourceMaterialTiling = new Rect(0f, 0f, 1f, 1f);
						}
						if (MB2_TextureBakeResults.IsMeshAndMaterialRectEnclosedByAtlasRect(mb_MaterialAndUVRect2.tilingTreatment, array[submeshIdx].uvRect, allPropsUseSameTiling_sourceMaterialTiling, samplingEncapsulatinRect, logLevel))
						{
							if (logLevel >= MB2_LogLevel.trace)
							{
								Debug.Log("Found rect in atlas capable of containing tiled sampling rect for mesh " + ((m != null) ? m.ToString() : null) + " at idx=" + k.ToString());
							}
							num = k;
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					MB_MaterialAndUVRect mb_MaterialAndUVRect3 = this.matsAndSrcUVRect[num];
					rectInAtlas = mb_MaterialAndUVRect3.atlasRect;
					tilingTreatment = mb_MaterialAndUVRect3.tilingTreatment;
					encapsulatingRectOut = mb_MaterialAndUVRect3.GetEncapsulatingRect();
					sourceMaterialTilingOut = mb_MaterialAndUVRect3.GetMaterialTilingRect();
					sliceIdx = mb_MaterialAndUVRect3.textureArraySliceIdx;
					return true;
				}
				rectInAtlas = default(Rect);
				encapsulatingRectOut = default(Rect);
				sourceMaterialTilingOut = default(Rect);
				sliceIdx = -1;
				errorMsg = string.Format("Objects To Be Combined mesh {0} uses material {1} on submesh {2}. This material requires a rectangle in the atlas that tiles the texture {3}. However, MeshBaker could not find a rectangle in the atlas that can contain this tiled rectangle.\n\nTo explain in greater detail, suppose there are two meshes:\n\n - A single-brick mesh that uses a small UV rectangle in a brick-wall.png texture.\n - A brick-wall mesh that tiles the same brick-wall.png texture three times.\n\nIf TextureBaker is used to bake a texture atlas that includes only the single-brick mesh (NOT the brick-wall mesh) and the \"considerUVs\" feature is used, then the TextureBaker will copy only the small UV rectangle (the single brick) with the brick-wall.png texture to the texture atlas.\n\nTHE PROBLEM: If one now attempts to use the same atlas in a MeshBaker-bake with the brick-wall-mesh, this will not work because the brick-wall mesh requires more of the brick-wall.png texture than was copied to the atlas. The brick-wall mesh needs the entire brick-wall.png texture tiled three times.\n\nTHE SOLUTION: To resolve this issue, both the \"single-brick mesh\" and the \"brick-wall mesh\" in the original texture bake, then the TextureBaker will copy the entire brick-wall.png to the atlas tiled three times. This atlas rectangle will work for both the single-brick mesh and the brick-wall mesh.", new object[]
				{
					m.name,
					mat,
					submeshIdx,
					array[submeshIdx].uvRect.ToString()
				});
				return false;
			}

			private MB2_TextureBakeResults textureBakeResults;

			private MB2_LogLevel LOG_LEVEL;

			private int[] numTimesMatAppearsInAtlas;

			private MB_MaterialAndUVRect[] matsAndSrcUVRect;

			private bool compareNamesWhenComparingMaterials;
		}

		public struct VertexAndTriangleProcessor : MB3_MeshCombinerSingle.IVertexAndTriangleProcessor, IDisposable
		{
			public MB_MeshVertexChannelFlags channels { readonly get; private set; }

			public void Dispose()
			{
				if (this._disposed)
				{
					return;
				}
				this._isInitialized = false;
				this.channels = MB_MeshVertexChannelFlags.none;
				this.verticies = null;
				this.normals = null;
				this.tangents = null;
				this.colors = null;
				this.uv0s = null;
				this.uvsSliceIdx = null;
				this.uv2s = null;
				this.uv3s = null;
				this.uv4s = null;
				this.uv5s = null;
				this.uv6s = null;
				this.uv7s = null;
				this.uv8s = null;
				this.submeshTris = null;
				this._disposed = true;
			}

			public bool IsInitialized()
			{
				return this._isInitialized;
			}

			public bool IsDisposed()
			{
				return this._disposed;
			}

			public void Init(MB3_MeshCombinerSingle combiner, MB_MeshVertexChannelFlags newChannels, int vertexCount, int[] newSubmeshTrisSize, int uvChannelWithExtraParameter, MB3_MeshCombinerSingle.IMeshChannelsCacheTaggingInterface meshChannelsCache, bool loadDataFromCombinedMesh, MB2_LogLevel logLevel)
			{
				if (loadDataFromCombinedMesh)
				{
					this.InitFromMeshCombiner(combiner, newChannels, uvChannelWithExtraParameter);
				}
				else
				{
					this.channels = newChannels;
					if ((this.channels & MB_MeshVertexChannelFlags.vertex) != MB_MeshVertexChannelFlags.none)
					{
						this.verticies = new Vector3[vertexCount];
					}
					if ((this.channels & MB_MeshVertexChannelFlags.normal) != MB_MeshVertexChannelFlags.none)
					{
						this.normals = new Vector3[vertexCount];
					}
					if ((this.channels & MB_MeshVertexChannelFlags.tangent) != MB_MeshVertexChannelFlags.none)
					{
						this.tangents = new Vector4[vertexCount];
					}
					if ((this.channels & MB_MeshVertexChannelFlags.colors) != MB_MeshVertexChannelFlags.none)
					{
						this.colors = new Color[vertexCount];
					}
					if ((this.channels & MB_MeshVertexChannelFlags.uv0) != MB_MeshVertexChannelFlags.none)
					{
						this.uv0s = new Vector2[vertexCount];
					}
					if ((this.channels & MB_MeshVertexChannelFlags.nuvsSliceIdx) != MB_MeshVertexChannelFlags.none)
					{
						this.uvsSliceIdx = new float[vertexCount];
					}
					if ((this.channels & MB_MeshVertexChannelFlags.uv2) != MB_MeshVertexChannelFlags.none)
					{
						this.uv2s = new Vector2[vertexCount];
					}
					if ((this.channels & MB_MeshVertexChannelFlags.uv3) != MB_MeshVertexChannelFlags.none)
					{
						this.uv3s = new Vector2[vertexCount];
					}
					if ((this.channels & MB_MeshVertexChannelFlags.uv4) != MB_MeshVertexChannelFlags.none)
					{
						this.uv4s = new Vector2[vertexCount];
					}
					if ((this.channels & MB_MeshVertexChannelFlags.uv5) != MB_MeshVertexChannelFlags.none)
					{
						this.uv5s = new Vector2[vertexCount];
					}
					if ((this.channels & MB_MeshVertexChannelFlags.uv6) != MB_MeshVertexChannelFlags.none)
					{
						this.uv6s = new Vector2[vertexCount];
					}
					if ((this.channels & MB_MeshVertexChannelFlags.uv7) != MB_MeshVertexChannelFlags.none)
					{
						this.uv7s = new Vector2[vertexCount];
					}
					if ((this.channels & MB_MeshVertexChannelFlags.uv8) != MB_MeshVertexChannelFlags.none)
					{
						this.uv8s = new Vector2[vertexCount];
					}
					this.submeshTris = new MB3_MeshCombinerSingle.SerializableIntArray[newSubmeshTrisSize.Length];
					for (int i = 0; i < newSubmeshTrisSize.Length; i++)
					{
						this.submeshTris[i] = new MB3_MeshCombinerSingle.SerializableIntArray(newSubmeshTrisSize[i]);
					}
				}
				this._isInitialized = true;
			}

			public void InitShowHide(MB3_MeshCombinerSingle combiner)
			{
				this.channels = MB_MeshVertexChannelFlags.none;
				this.submeshTris = combiner.submeshTris;
				this._isInitialized = true;
			}

			public void InitFromMeshCombiner(MB3_MeshCombinerSingle combiner, MB_MeshVertexChannelFlags newChannels, int uvChannelWithExtraParameter)
			{
				if (combiner.channelsLastBake != newChannels)
				{
					if (combiner.channelsLastBake == MB_MeshVertexChannelFlags.none && combiner.verts.Length != 0)
					{
						combiner.channelsLastBake = newChannels;
					}
					else
					{
						Debug.LogError("Shouldn't change channels between bakes. \n" + combiner.channelsLastBake.ToString() + " \n" + newChannels.ToString());
					}
				}
				this.channels = combiner.channelsLastBake;
				if ((this.channels & MB_MeshVertexChannelFlags.vertex) != MB_MeshVertexChannelFlags.none)
				{
					this.verticies = combiner.verts;
				}
				if ((this.channels & MB_MeshVertexChannelFlags.normal) != MB_MeshVertexChannelFlags.none)
				{
					this.normals = combiner.normals;
				}
				if ((this.channels & MB_MeshVertexChannelFlags.tangent) != MB_MeshVertexChannelFlags.none)
				{
					this.tangents = combiner.tangents;
				}
				if ((this.channels & MB_MeshVertexChannelFlags.colors) != MB_MeshVertexChannelFlags.none)
				{
					this.colors = combiner.colors;
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv0) != MB_MeshVertexChannelFlags.none)
				{
					this.uv0s = combiner.uvs;
				}
				if ((this.channels & MB_MeshVertexChannelFlags.nuvsSliceIdx) != MB_MeshVertexChannelFlags.none)
				{
					this.uvsSliceIdx = combiner.uvsSliceIdx;
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv2) != MB_MeshVertexChannelFlags.none)
				{
					this.uv2s = combiner.uv2s;
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv3) != MB_MeshVertexChannelFlags.none)
				{
					this.uv3s = combiner.uv3s;
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv4) != MB_MeshVertexChannelFlags.none)
				{
					this.uv4s = combiner.uv4s;
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv5) != MB_MeshVertexChannelFlags.none)
				{
					this.uv5s = combiner.uv5s;
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv6) != MB_MeshVertexChannelFlags.none)
				{
					this.uv6s = combiner.uv6s;
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv7) != MB_MeshVertexChannelFlags.none)
				{
					this.uv7s = combiner.uv7s;
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv8) != MB_MeshVertexChannelFlags.none)
				{
					this.uv8s = combiner.uv8s;
				}
				this.submeshTris = combiner.submeshTris;
				this._isInitialized = true;
			}

			public int GetVertexCount()
			{
				return this.verticies.Length;
			}

			public int GetSubmeshCount()
			{
				return this.submeshTris.Length;
			}

			public void TransferOwnershipOfSerializableBuffersToCombiner(MB3_MeshCombinerSingle c, MB_MeshVertexChannelFlags channelsToTransfer, MB3_MeshCombinerSingle.BufferDataFromPreviousBake serializableBufferData)
			{
				c.channelsLastBake = this.channels;
				if ((channelsToTransfer & MB_MeshVertexChannelFlags.vertex) != MB_MeshVertexChannelFlags.none)
				{
					c.verts = this.verticies;
				}
				if ((channelsToTransfer & MB_MeshVertexChannelFlags.normal) != MB_MeshVertexChannelFlags.none)
				{
					c.normals = this.normals;
				}
				if ((channelsToTransfer & MB_MeshVertexChannelFlags.tangent) != MB_MeshVertexChannelFlags.none)
				{
					c.tangents = this.tangents;
				}
				if ((channelsToTransfer & MB_MeshVertexChannelFlags.uv0) != MB_MeshVertexChannelFlags.none)
				{
					c.uvs = this.uv0s;
				}
				if ((channelsToTransfer & MB_MeshVertexChannelFlags.nuvsSliceIdx) != MB_MeshVertexChannelFlags.none)
				{
					c.uvsSliceIdx = this.uvsSliceIdx;
				}
				if ((channelsToTransfer & MB_MeshVertexChannelFlags.uv2) != MB_MeshVertexChannelFlags.none)
				{
					c.uv2s = this.uv2s;
				}
				if ((channelsToTransfer & MB_MeshVertexChannelFlags.uv3) != MB_MeshVertexChannelFlags.none)
				{
					c.uv3s = this.uv3s;
				}
				if ((channelsToTransfer & MB_MeshVertexChannelFlags.uv4) != MB_MeshVertexChannelFlags.none)
				{
					c.uv4s = this.uv4s;
				}
				if ((channelsToTransfer & MB_MeshVertexChannelFlags.uv5) != MB_MeshVertexChannelFlags.none)
				{
					c.uv5s = this.uv5s;
				}
				if ((channelsToTransfer & MB_MeshVertexChannelFlags.uv6) != MB_MeshVertexChannelFlags.none)
				{
					c.uv6s = this.uv6s;
				}
				if ((channelsToTransfer & MB_MeshVertexChannelFlags.uv7) != MB_MeshVertexChannelFlags.none)
				{
					c.uv7s = this.uv7s;
				}
				if ((channelsToTransfer & MB_MeshVertexChannelFlags.uv8) != MB_MeshVertexChannelFlags.none)
				{
					c.uv8s = this.uv8s;
				}
				if ((channelsToTransfer & MB_MeshVertexChannelFlags.colors) != MB_MeshVertexChannelFlags.none)
				{
					c.colors = this.colors;
				}
				c.submeshTris = this.submeshTris;
				c.bufferDataFromPrevious = serializableBufferData;
				this.verticies = null;
				this.normals = null;
				this.tangents = null;
				this.uv0s = null;
				this.uvsSliceIdx = null;
				this.uv2s = null;
				this.uv3s = null;
				this.uv4s = null;
				this.uv5s = null;
				this.uv6s = null;
				this.uv7s = null;
				this.uv8s = null;
				this.colors = null;
				this.submeshTris = null;
				this._isInitialized = false;
			}

			public void CopyArraysFromPreviousBakeBuffersToNewBuffers(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, ref MB3_MeshCombinerSingle.IVertexAndTriangleProcessor iOldBuffers, int destStartVertIdx, int triangleIdxAdjustment, int[] targSubmeshTidx, MB2_LogLevel LOG_LEVEL)
			{
				MB3_MeshCombinerSingle.VertexAndTriangleProcessor vertexAndTriangleProcessor = (MB3_MeshCombinerSingle.VertexAndTriangleProcessor)iOldBuffers;
				int vertIdx = dgo.vertIdx;
				int numVerts = dgo.numVerts;
				if ((this.channels & MB_MeshVertexChannelFlags.vertex) == MB_MeshVertexChannelFlags.vertex)
				{
					Array.Copy(vertexAndTriangleProcessor.verticies, vertIdx, this.verticies, destStartVertIdx, numVerts);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.normal) == MB_MeshVertexChannelFlags.normal)
				{
					Array.Copy(vertexAndTriangleProcessor.normals, vertIdx, this.normals, destStartVertIdx, numVerts);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.tangent) == MB_MeshVertexChannelFlags.tangent)
				{
					Array.Copy(vertexAndTriangleProcessor.tangents, vertIdx, this.tangents, destStartVertIdx, numVerts);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv0) == MB_MeshVertexChannelFlags.uv0)
				{
					Array.Copy(vertexAndTriangleProcessor.uv0s, vertIdx, this.uv0s, destStartVertIdx, numVerts);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.nuvsSliceIdx) == MB_MeshVertexChannelFlags.nuvsSliceIdx)
				{
					Array.Copy(vertexAndTriangleProcessor.uvsSliceIdx, vertIdx, this.uvsSliceIdx, destStartVertIdx, numVerts);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv2) == MB_MeshVertexChannelFlags.uv2)
				{
					Array.Copy(vertexAndTriangleProcessor.uv2s, vertIdx, this.uv2s, destStartVertIdx, numVerts);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv3) == MB_MeshVertexChannelFlags.uv3)
				{
					Array.Copy(vertexAndTriangleProcessor.uv3s, vertIdx, this.uv3s, destStartVertIdx, numVerts);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv4) == MB_MeshVertexChannelFlags.uv4)
				{
					Array.Copy(vertexAndTriangleProcessor.uv4s, vertIdx, this.uv4s, destStartVertIdx, numVerts);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv5) == MB_MeshVertexChannelFlags.uv5)
				{
					Array.Copy(vertexAndTriangleProcessor.uv5s, vertIdx, this.uv5s, destStartVertIdx, numVerts);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv6) == MB_MeshVertexChannelFlags.uv6)
				{
					Array.Copy(vertexAndTriangleProcessor.uv6s, vertIdx, this.uv6s, destStartVertIdx, numVerts);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv7) == MB_MeshVertexChannelFlags.uv7)
				{
					Array.Copy(vertexAndTriangleProcessor.uv7s, vertIdx, this.uv7s, destStartVertIdx, numVerts);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv8) == MB_MeshVertexChannelFlags.uv8)
				{
					Array.Copy(vertexAndTriangleProcessor.uv8s, vertIdx, this.uv8s, destStartVertIdx, numVerts);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.colors) == MB_MeshVertexChannelFlags.colors)
				{
					Array.Copy(vertexAndTriangleProcessor.colors, vertIdx, this.colors, destStartVertIdx, numVerts);
				}
				for (int i = 0; i < this.submeshTris.Length; i++)
				{
					int[] data = vertexAndTriangleProcessor.submeshTris[i].data;
					int num = dgo.submeshTriIdxs[i];
					int num2 = dgo.submeshNumTris[i];
					if (LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug(string.Concat(new string[]
						{
							"    Adjusting submesh triangles submesh:",
							i.ToString(),
							" startIdx:",
							num.ToString(),
							" num:",
							num2.ToString(),
							" nsubmeshTris:",
							this.submeshTris.Length.ToString(),
							" targSubmeshTidx:",
							targSubmeshTidx.Length.ToString()
						}), new object[]
						{
							LOG_LEVEL
						});
					}
					for (int j = num; j < num + num2; j++)
					{
						data[j] -= triangleIdxAdjustment;
					}
					Array.Copy(data, num, this.submeshTris[i].data, targSubmeshTidx[i], num2);
				}
			}

			public void CopyFromDGOMeshToBuffers(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, int destStartVertsIdx, MB_MeshVertexChannelFlags channelsToUpdate, bool updateTris, bool updateBWdata, MB_IMeshBakerSettings settings, MB_IMeshCombinerSingle_BoneProcessor boneProcessor, int[] targSubmeshTidx, MB2_TextureBakeResults textureBakeResults, MB3_MeshCombinerSingle.UVAdjuster_Atlas uvAdjuster, MB2_LogLevel LOG_LEVEL, MB3_MeshCombinerSingle.IMeshChannelsCacheTaggingInterface meshChannelCacheParam)
			{
				MB3_MeshCombinerSingle.MeshChannelsCache meshChannelsCache = (MB3_MeshCombinerSingle.MeshChannelsCache)meshChannelCacheParam;
				bool flag = (this.channels & MB_MeshVertexChannelFlags.vertex) == MB_MeshVertexChannelFlags.vertex && (channelsToUpdate & MB_MeshVertexChannelFlags.vertex) == MB_MeshVertexChannelFlags.vertex;
				bool flag2 = (this.channels & MB_MeshVertexChannelFlags.normal) == MB_MeshVertexChannelFlags.normal && (channelsToUpdate & MB_MeshVertexChannelFlags.normal) == MB_MeshVertexChannelFlags.normal;
				bool flag3 = (this.channels & MB_MeshVertexChannelFlags.tangent) == MB_MeshVertexChannelFlags.tangent && (channelsToUpdate & MB_MeshVertexChannelFlags.tangent) == MB_MeshVertexChannelFlags.tangent;
				if (flag || flag2 || flag3)
				{
					Vector3[] array = null;
					Vector3[] array2 = null;
					Vector4[] array3 = null;
					if (flag)
					{
						array = meshChannelsCache.GetVertices(dgo._mesh);
					}
					if (flag2)
					{
						array2 = meshChannelsCache.GetNormals(dgo._mesh);
					}
					if (flag3)
					{
						array3 = meshChannelsCache.GetTangents(dgo._mesh);
					}
					if (settings.renderType != MB_RenderType.skinnedMeshRenderer)
					{
						this._LocalToWorld(dgo.gameObject.transform, flag2, flag3, destStartVertsIdx, array, array2, array3, this.verticies, this.normals, this.tangents);
					}
					else
					{
						boneProcessor.CopyVertsNormsTansToBuffers(dgo, settings, destStartVertsIdx, array2, array3, array, this.normals, this.tangents, this.verticies);
					}
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv0) == MB_MeshVertexChannelFlags.uv0 && (channelsToUpdate & MB_MeshVertexChannelFlags.uv0) == MB_MeshVertexChannelFlags.uv0)
				{
					this._copyAndAdjustUVsFromMesh(textureBakeResults, dgo, dgo._mesh, 0, destStartVertsIdx, this.uv0s, this.uvsSliceIdx, meshChannelsCache, LOG_LEVEL, textureBakeResults);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv2) == MB_MeshVertexChannelFlags.uv2 && (channelsToUpdate & MB_MeshVertexChannelFlags.uv2) == MB_MeshVertexChannelFlags.uv2)
				{
					this._CopyAndAdjustUV2FromMesh(settings, meshChannelsCache, dgo, destStartVertsIdx, LOG_LEVEL);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv3) == MB_MeshVertexChannelFlags.uv3 && (channelsToUpdate & MB_MeshVertexChannelFlags.uv3) == MB_MeshVertexChannelFlags.uv3)
				{
					meshChannelsCache.GetUVChannel(3, dgo._mesh).CopyTo(this.uv3s, destStartVertsIdx);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv4) == MB_MeshVertexChannelFlags.uv4 && (channelsToUpdate & MB_MeshVertexChannelFlags.uv4) == MB_MeshVertexChannelFlags.uv4)
				{
					meshChannelsCache.GetUVChannel(4, dgo._mesh).CopyTo(this.uv4s, destStartVertsIdx);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv5) == MB_MeshVertexChannelFlags.uv5 && (channelsToUpdate & MB_MeshVertexChannelFlags.uv5) == MB_MeshVertexChannelFlags.uv5)
				{
					meshChannelsCache.GetUVChannel(5, dgo._mesh).CopyTo(this.uv5s, destStartVertsIdx);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv6) == MB_MeshVertexChannelFlags.uv6 && (channelsToUpdate & MB_MeshVertexChannelFlags.uv6) == MB_MeshVertexChannelFlags.uv6)
				{
					meshChannelsCache.GetUVChannel(6, dgo._mesh).CopyTo(this.uv6s, destStartVertsIdx);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv7) == MB_MeshVertexChannelFlags.uv7 && (channelsToUpdate & MB_MeshVertexChannelFlags.uv7) == MB_MeshVertexChannelFlags.uv7)
				{
					meshChannelsCache.GetUVChannel(7, dgo._mesh).CopyTo(this.uv7s, destStartVertsIdx);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv8) == MB_MeshVertexChannelFlags.uv8 && (channelsToUpdate & MB_MeshVertexChannelFlags.uv8) == MB_MeshVertexChannelFlags.uv8)
				{
					meshChannelsCache.GetUVChannel(8, dgo._mesh).CopyTo(this.uv8s, destStartVertsIdx);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.colors) == MB_MeshVertexChannelFlags.colors && (channelsToUpdate & MB_MeshVertexChannelFlags.colors) == MB_MeshVertexChannelFlags.colors)
				{
					meshChannelsCache.GetColors(dgo._mesh).CopyTo(this.colors, destStartVertsIdx);
				}
				if (updateBWdata)
				{
					boneProcessor.UpdateGameObjects_UpdateBWIndexes(dgo);
				}
				if (updateTris)
				{
					for (int i = 0; i < targSubmeshTidx.Length; i++)
					{
						dgo.submeshTriIdxs[i] = targSubmeshTidx[i];
					}
					for (int j = 0; j < dgo._tmpSubmeshTris.Length; j++)
					{
						int[] data = dgo._tmpSubmeshTris[j].data;
						if (destStartVertsIdx != 0)
						{
							for (int k = 0; k < data.Length; k++)
							{
								data[k] += destStartVertsIdx;
							}
						}
						if (dgo.invertTriangles)
						{
							for (int l = 0; l < data.Length; l += 3)
							{
								int num = data[l];
								data[l] = data[l + 1];
								data[l + 1] = num;
							}
						}
						int num2 = dgo.targetSubmeshIdxs[j];
						data.CopyTo(this.submeshTris[num2].data, targSubmeshTidx[num2]);
						dgo.submeshNumTris[num2] += data.Length;
						targSubmeshTidx[num2] += data.Length;
					}
				}
			}

			public void AssignBuffersToMesh(Mesh mesh, MB_IMeshBakerSettings settings, MB2_TextureBakeResults textureBakeResults, MB_MeshVertexChannelFlags channelsToWriteToMesh, bool doWriteTrisToMesh, IAssignToMeshCustomizer assignToMeshCustomizer, List<MB3_MeshCombinerSingle.MB_DynamicGameObject> mbDynamicObjectsInCombinedMesh, out MB3_MeshCombinerSingle.BufferDataFromPreviousBake serializableBufferData, out MB3_MeshCombinerSingle.SerializableIntArray[] submeshTrisToUse, out int numNonZeroLengthSubmeshes)
			{
				if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.vertex) == MB_MeshVertexChannelFlags.vertex)
				{
					Vector3[] vertices;
					this.AdjustVertsToWriteAccordingToPivotPositionIfNecessary(settings.pivotLocationType, settings.renderType, settings.clearBuffersAfterBake, settings.pivotLocation, out serializableBufferData, out vertices);
					mesh.vertices = vertices;
				}
				else
				{
					serializableBufferData.numVertsBaked = mesh.vertexCount;
					serializableBufferData.meshVerticesShift = Vector3.zero;
					serializableBufferData.meshVerticiesWereShifted = false;
				}
				if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.normal) == MB_MeshVertexChannelFlags.normal)
				{
					mesh.normals = this.normals;
				}
				if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.tangent) == MB_MeshVertexChannelFlags.tangent)
				{
					mesh.tangents = this.tangents;
				}
				if (assignToMeshCustomizer != null)
				{
					IAssignToMeshCustomizer_SimpleAPI assignToMeshCustomizer_SimpleAPI = (IAssignToMeshCustomizer_SimpleAPI)assignToMeshCustomizer;
					if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.uv0) == MB_MeshVertexChannelFlags.uv0)
					{
						assignToMeshCustomizer_SimpleAPI.meshAssign_UV0(0, settings, textureBakeResults, mesh, this.uv0s, this.uvsSliceIdx);
					}
					if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.uv2) == MB_MeshVertexChannelFlags.uv2)
					{
						assignToMeshCustomizer_SimpleAPI.meshAssign_UV2(2, settings, textureBakeResults, mesh, this.uv2s, this.uvsSliceIdx);
					}
					if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.uv3) == MB_MeshVertexChannelFlags.uv3)
					{
						assignToMeshCustomizer_SimpleAPI.meshAssign_UV3(3, settings, textureBakeResults, mesh, this.uv3s, this.uvsSliceIdx);
					}
					if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.uv4) == MB_MeshVertexChannelFlags.uv4)
					{
						assignToMeshCustomizer_SimpleAPI.meshAssign_UV4(4, settings, textureBakeResults, mesh, this.uv4s, this.uvsSliceIdx);
					}
					if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.uv5) == MB_MeshVertexChannelFlags.uv5)
					{
						assignToMeshCustomizer_SimpleAPI.meshAssign_UV5(5, settings, textureBakeResults, mesh, this.uv5s, this.uvsSliceIdx);
					}
					if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.uv6) == MB_MeshVertexChannelFlags.uv6)
					{
						assignToMeshCustomizer_SimpleAPI.meshAssign_UV6(6, settings, textureBakeResults, mesh, this.uv6s, this.uvsSliceIdx);
					}
					if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.uv7) == MB_MeshVertexChannelFlags.uv7)
					{
						assignToMeshCustomizer_SimpleAPI.meshAssign_UV7(7, settings, textureBakeResults, mesh, this.uv7s, this.uvsSliceIdx);
					}
					if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.uv8) == MB_MeshVertexChannelFlags.uv8)
					{
						assignToMeshCustomizer_SimpleAPI.meshAssign_UV8(8, settings, textureBakeResults, mesh, this.uv8s, this.uvsSliceIdx);
					}
					if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.colors) == MB_MeshVertexChannelFlags.colors)
					{
						assignToMeshCustomizer_SimpleAPI.meshAssign_colors(settings, textureBakeResults, mesh, this.colors, this.uvsSliceIdx);
					}
				}
				else
				{
					if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.uv0) == MB_MeshVertexChannelFlags.uv0)
					{
						MBVersion.MeshAssignUVChannel(0, mesh, this.uv0s);
					}
					if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.uv2) == MB_MeshVertexChannelFlags.uv2)
					{
						MBVersion.MeshAssignUVChannel(2, mesh, this.uv2s);
					}
					if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.uv3) == MB_MeshVertexChannelFlags.uv3)
					{
						MBVersion.MeshAssignUVChannel(3, mesh, this.uv3s);
					}
					if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.uv4) == MB_MeshVertexChannelFlags.uv4)
					{
						MBVersion.MeshAssignUVChannel(4, mesh, this.uv4s);
					}
					if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.uv5) == MB_MeshVertexChannelFlags.uv5)
					{
						MBVersion.MeshAssignUVChannel(5, mesh, this.uv5s);
					}
					if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.uv6) == MB_MeshVertexChannelFlags.uv6)
					{
						MBVersion.MeshAssignUVChannel(6, mesh, this.uv6s);
					}
					if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.uv7) == MB_MeshVertexChannelFlags.uv7)
					{
						MBVersion.MeshAssignUVChannel(7, mesh, this.uv7s);
					}
					if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.uv8) == MB_MeshVertexChannelFlags.uv8)
					{
						MBVersion.MeshAssignUVChannel(8, mesh, this.uv8s);
					}
					if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.colors) == MB_MeshVertexChannelFlags.colors)
					{
						mesh.colors = this.colors;
					}
				}
				if (doWriteTrisToMesh)
				{
					this.AssignTriangleDataForSubmeshes(mesh, mbDynamicObjectsInCombinedMesh, ref serializableBufferData, out submeshTrisToUse, out numNonZeroLengthSubmeshes);
					return;
				}
				submeshTrisToUse = null;
				numNonZeroLengthSubmeshes = -1;
			}

			public void AssignTriangleDataForSubmeshes(Mesh mesh, List<MB3_MeshCombinerSingle.MB_DynamicGameObject> mbDynamicObjectsInCombinedMesh, ref MB3_MeshCombinerSingle.BufferDataFromPreviousBake serializableBufferData, out MB3_MeshCombinerSingle.SerializableIntArray[] submeshTrisToUse, out int numNonZeroLengthSubmeshes)
			{
				submeshTrisToUse = this.GetSubmeshTrisWithShowHideApplied(mbDynamicObjectsInCombinedMesh);
				int num = 0;
				numNonZeroLengthSubmeshes = MB3_MeshCombinerSingle.VertexAndTriangleProcessor._NumNonZeroLengthSubmeshTris(submeshTrisToUse, out num);
				mesh.subMeshCount = numNonZeroLengthSubmeshes;
				int num2 = 0;
				for (int i = 0; i < submeshTrisToUse.Length; i++)
				{
					if (submeshTrisToUse[i].data.Length != 0)
					{
						mesh.SetTriangles(submeshTrisToUse[i].data, num2);
						num2++;
					}
				}
			}

			public void AssignTriangleDataForSubmeshes_ShowHide(Mesh mesh, List<MB3_MeshCombinerSingle.MB_DynamicGameObject> mbDynamicObjectsInCombinedMesh, ref MB3_MeshCombinerSingle.BufferDataFromPreviousBake serializableBufferData, out MB3_MeshCombinerSingle.SerializableIntArray[] submeshTrisToUse, out int numNonZeroLengthSubmeshes)
			{
				this.AssignTriangleDataForSubmeshes(mesh, mbDynamicObjectsInCombinedMesh, ref serializableBufferData, out submeshTrisToUse, out numNonZeroLengthSubmeshes);
			}

			private void AdjustVertsToWriteAccordingToPivotPositionIfNecessary(MB_MeshPivotLocation pivotLocationType, MB_RenderType renderType, bool clearBuffersAfterBake, Vector3 pivotLocation_wld, out MB3_MeshCombinerSingle.BufferDataFromPreviousBake serializableBufferData, out Vector3[] verts2Write)
			{
				verts2Write = this.verticies;
				serializableBufferData.numVertsBaked = this.verticies.Length;
				if (this.verticies.Length == 0)
				{
					serializableBufferData.numVertsBaked = this.verticies.Length;
					serializableBufferData.meshVerticesShift = Vector3.zero;
					serializableBufferData.meshVerticiesWereShifted = false;
					return;
				}
				if (renderType == MB_RenderType.skinnedMeshRenderer)
				{
					serializableBufferData.numVertsBaked = this.verticies.Length;
					serializableBufferData.meshVerticesShift = Vector3.zero;
					serializableBufferData.meshVerticiesWereShifted = false;
					return;
				}
				if (pivotLocationType == MB_MeshPivotLocation.worldOrigin)
				{
					serializableBufferData.numVertsBaked = this.verticies.Length;
					serializableBufferData.meshVerticesShift = Vector3.zero;
					serializableBufferData.meshVerticiesWereShifted = false;
					return;
				}
				if (pivotLocationType == MB_MeshPivotLocation.boundsCenter || pivotLocationType == MB_MeshPivotLocation.customLocation)
				{
					Vector3 vector4;
					if (pivotLocationType == MB_MeshPivotLocation.boundsCenter)
					{
						Vector3 vector = this.verticies[0];
						Vector3 vector2 = this.verticies[0];
						for (int i = 1; i < this.verticies.Length; i++)
						{
							Vector3 vector3 = this.verticies[i];
							if (vector.x < vector3.x)
							{
								vector.x = vector3.x;
							}
							if (vector.y < vector3.y)
							{
								vector.y = vector3.y;
							}
							if (vector.z < vector3.z)
							{
								vector.z = vector3.z;
							}
							if (vector2.x > vector3.x)
							{
								vector2.x = vector3.x;
							}
							if (vector2.y > vector3.y)
							{
								vector2.y = vector3.y;
							}
							if (vector2.z > vector3.z)
							{
								vector2.z = vector3.z;
							}
						}
						vector4 = (vector + vector2) * 0.5f;
					}
					else
					{
						vector4 = pivotLocation_wld;
					}
					if (!clearBuffersAfterBake)
					{
						verts2Write = new Vector3[this.verticies.Length];
					}
					for (int j = 0; j < this.verticies.Length; j++)
					{
						verts2Write[j] = this.verticies[j] - vector4;
					}
					serializableBufferData.numVertsBaked = this.verticies.Length;
					serializableBufferData.meshVerticesShift = vector4;
					serializableBufferData.meshVerticiesWereShifted = true;
					return;
				}
				Debug.LogError("Unsupported Pivot Location Type: " + pivotLocationType.ToString());
				serializableBufferData.numVertsBaked = this.verticies.Length;
				serializableBufferData.meshVerticesShift = Vector3.zero;
				serializableBufferData.meshVerticiesWereShifted = false;
			}

			private static int _NumNonZeroLengthSubmeshTris(MB3_MeshCombinerSingle.SerializableIntArray[] subTris, out int numIndexes)
			{
				numIndexes = 0;
				int num = 0;
				for (int i = 0; i < subTris.Length; i++)
				{
					if (subTris[i].data.Length != 0)
					{
						num++;
						numIndexes += subTris[i].data.Length;
					}
				}
				return num;
			}

			private void _copyAndAdjustUVsFromMesh(MB2_TextureBakeResults tbr, MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, Mesh mesh, int uvChannel, int vertsIdx, Vector2[] uvsOut, float[] uvsSliceIdx, MB3_MeshCombinerSingle.MeshChannelsCache meshChannelsCache, MB2_LogLevel LOG_LEVEL, MB2_TextureBakeResults textureBakeResults)
			{
				Vector2[] uvchannel = meshChannelsCache.GetUVChannel(uvChannel, mesh);
				int[] array = new int[uvchannel.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = -1;
				}
				bool flag = false;
				bool flag2 = tbr.resultType == MB2_TextureBakeResults.ResultType.textureArray;
				for (int j = 0; j < dgo.targetSubmeshIdxs.Length; j++)
				{
					int[] array2;
					if (dgo._tmpSubmeshTris != null)
					{
						array2 = dgo._tmpSubmeshTris[j].data;
					}
					else
					{
						array2 = mesh.GetTriangles(j);
					}
					float num = (float)dgo.textureArraySliceIdx[j];
					int idxInSrcMats = dgo.targetSubmeshIdxs[j];
					if (LOG_LEVEL >= MB2_LogLevel.trace)
					{
						Debug.Log(string.Format("Build UV transform for mesh {0} submesh {1} encapsulatingRect {2}", dgo.name, j, dgo.encapsulatingRect[j]));
					}
					Rect rect = MB3_TextureCombinerMerging.BuildTransformMeshUV2AtlasRect(textureBakeResults.GetConsiderMeshUVs(idxInSrcMats, dgo.sourceSharedMaterials[j]), dgo.uvRects[j], (dgo.obUVRects == null || dgo.obUVRects.Length == 0) ? new Rect(0f, 0f, 1f, 1f) : dgo.obUVRects[j], dgo.sourceMaterialTiling[j], dgo.encapsulatingRect[j]);
					foreach (int num2 in array2)
					{
						if (array[num2] == -1)
						{
							array[num2] = j;
							Vector2 vector = uvchannel[num2];
							vector.x = rect.x + vector.x * rect.width;
							vector.y = rect.y + vector.y * rect.height;
							int num3 = vertsIdx + num2;
							uvsOut[num3] = vector;
							if (flag2)
							{
								uvsSliceIdx[num3] = num;
							}
						}
						if (array[num2] != j)
						{
							flag = true;
						}
					}
				}
				if (flag && LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning(dgo.name + "has submeshes which share verticies. Adjusted uvs may not map correctly in combined atlas.");
				}
				if (LOG_LEVEL >= MB2_LogLevel.trace)
				{
					Debug.Log(string.Format("_copyAndAdjustUVsFromMesh copied {0} verts", uvchannel.Length));
				}
			}

			private void _CopyAndAdjustUV2FromMesh(MB_IMeshBakerSettings settings, MB3_MeshCombinerSingle.MeshChannelsCache meshChannelsCache, MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, int vertsIdx, MB2_LogLevel LOG_LEVEL)
			{
				Vector2[] array = meshChannelsCache.GetUVChannel(2, dgo._mesh);
				if (settings.lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping)
				{
					if (array == null || array.Length == 0)
					{
						Vector2[] uvchannel = meshChannelsCache.GetUVChannel(0, dgo._mesh);
						if (uvchannel != null && uvchannel.Length != 0)
						{
							array = uvchannel;
						}
						else
						{
							if (LOG_LEVEL >= MB2_LogLevel.warn)
							{
								string str = "Mesh ";
								Mesh mesh = dgo._mesh;
								Debug.LogWarning(str + ((mesh != null) ? mesh.ToString() : null) + " didn't have uv2s. Generating uv2s.");
							}
							array = meshChannelsCache.GetUv2Modified(dgo._mesh);
						}
					}
					Vector4 lightmapTilingOffset = dgo.lightmapTilingOffset;
					Vector2 vector = new Vector2(lightmapTilingOffset.x, lightmapTilingOffset.y);
					Vector2 a = new Vector2(lightmapTilingOffset.z, lightmapTilingOffset.w);
					for (int i = 0; i < array.Length; i++)
					{
						Vector2 b;
						b.x = vector.x * array[i].x;
						b.y = vector.y * array[i].y;
						this.uv2s[vertsIdx + i] = a + b;
					}
					if (LOG_LEVEL >= MB2_LogLevel.trace)
					{
						Debug.Log("_copyAndAdjustUV2FromMesh copied and modify for preserve current lightmapping " + array.Length.ToString());
						return;
					}
				}
				else
				{
					if (array == null || array.Length == 0)
					{
						if (LOG_LEVEL >= MB2_LogLevel.warn)
						{
							string str2 = "Mesh ";
							Mesh mesh2 = dgo._mesh;
							Debug.LogWarning(str2 + ((mesh2 != null) ? mesh2.ToString() : null) + " didn't have uv2s. Generating uv2s.");
						}
						if (settings.lightmapOption == MB2_LightmapOptions.copy_UV2_unchanged_to_separate_rects && (array == null || array.Length == 0))
						{
							string str3 = "Mesh ";
							Mesh mesh3 = dgo._mesh;
							Debug.LogError(str3 + ((mesh3 != null) ? mesh3.ToString() : null) + " did not have a UV2 channel. Nothing to copy when trying to copy UV2 to separate rects. The combined mesh will not lightmap properly. Try using generate new uv2 layout.");
						}
						array = meshChannelsCache.GetUv2Modified(dgo._mesh);
					}
					array.CopyTo(this.uv2s, vertsIdx);
					if (LOG_LEVEL >= MB2_LogLevel.trace)
					{
						Debug.Log("_copyAndAdjustUV2FromMesh copied without modifying " + array.Length.ToString());
					}
				}
			}

			public void CopyUV2unchangedToSeparateRects(List<MB3_MeshCombinerSingle.MB_DynamicGameObject> mbDynamicObjectsInCombinedMesh, float uv2UnwrappingParamsPackMargin)
			{
				int num = Mathf.CeilToInt(8192f * uv2UnwrappingParamsPackMargin);
				if (num < 1)
				{
					num = 1;
				}
				List<Vector2> list = new List<Vector2>(mbDynamicObjectsInCombinedMesh.Count);
				float[] array = new float[mbDynamicObjectsInCombinedMesh.Count];
				Rect[] array2 = new Rect[mbDynamicObjectsInCombinedMesh.Count];
				float num2 = 0f;
				for (int i = 0; i < mbDynamicObjectsInCombinedMesh.Count; i++)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject = mbDynamicObjectsInCombinedMesh[i];
					float num3 = 1f;
					if (Application.isEditor && mb_DynamicGameObject._renderer is MeshRenderer)
					{
						num3 = MBVersion.GetScaleInLightmap((MeshRenderer)mb_DynamicGameObject._renderer);
						if (num3 <= 0f)
						{
							num3 = 1f;
						}
					}
					float magnitude = mb_DynamicGameObject.meshSize.magnitude;
					array[i] = num3 * magnitude;
					num2 += array[i];
				}
				for (int j = 0; j < array.Length; j++)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject2 = mbDynamicObjectsInCombinedMesh[j];
					int num4 = mb_DynamicGameObject2.vertIdx + mb_DynamicGameObject2.numVerts;
					float x;
					float num5 = x = this.uv2s[mb_DynamicGameObject2.vertIdx].x;
					float y;
					float num6 = y = this.uv2s[mb_DynamicGameObject2.vertIdx].y;
					for (int k = mb_DynamicGameObject2.vertIdx; k < num4; k++)
					{
						if (this.uv2s[k].x < x)
						{
							x = this.uv2s[k].x;
						}
						if (this.uv2s[k].x > num5)
						{
							num5 = this.uv2s[k].x;
						}
						if (this.uv2s[k].y < y)
						{
							y = this.uv2s[k].y;
						}
						if (this.uv2s[k].y > num6)
						{
							num6 = this.uv2s[k].y;
						}
					}
					array2[j] = new Rect(x, y, num5 - x, num6 - y);
					array[j] /= num2;
					Vector2 item = new Vector2(array2[j].width, array2[j].height) * (array[j] * 8192f);
					list.Add(item);
				}
				AtlasPackingResult atlasPackingResult = new MB2_TexturePackerRegular
				{
					atlasMustBePowerOfTwo = false
				}.GetRects(list, 8192, 8192, num)[0];
				for (int l = 0; l < mbDynamicObjectsInCombinedMesh.Count; l++)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject3 = mbDynamicObjectsInCombinedMesh[l];
					int num7 = mb_DynamicGameObject3.vertIdx + mb_DynamicGameObject3.numVerts;
					Rect rect = array2[l];
					Rect rect2 = atlasPackingResult.rects[l];
					for (int m = mb_DynamicGameObject3.vertIdx; m < num7; m++)
					{
						Vector2 vector;
						vector.x = (this.uv2s[m].x - rect.x) / rect.width * rect2.width + rect2.x;
						vector.y = (this.uv2s[m].y - rect.y) / rect.height * rect2.height + rect2.y;
						this.uv2s[m] = vector;
					}
					if (atlasPackingResult.atlasX != atlasPackingResult.atlasY)
					{
						if (atlasPackingResult.atlasX < atlasPackingResult.atlasY)
						{
							float num8 = (float)atlasPackingResult.atlasX / (float)atlasPackingResult.atlasY;
							for (int n = mb_DynamicGameObject3.vertIdx; n < num7; n++)
							{
								Vector2 vector2 = this.uv2s[n];
								vector2.x *= num8;
								this.uv2s[n] = vector2;
							}
						}
						else
						{
							float num9 = (float)atlasPackingResult.atlasY / (float)atlasPackingResult.atlasX;
							for (int num10 = mb_DynamicGameObject3.vertIdx; num10 < num7; num10++)
							{
								Vector2 vector3 = this.uv2s[num10];
								vector3.y *= num9;
								this.uv2s[num10] = vector3;
							}
						}
					}
				}
			}

			private MB3_MeshCombinerSingle.SerializableIntArray[] GetSubmeshTrisWithShowHideApplied(List<MB3_MeshCombinerSingle.MB_DynamicGameObject> mbDynamicObjectsInCombinedMesh)
			{
				bool flag = false;
				for (int i = 0; i < mbDynamicObjectsInCombinedMesh.Count; i++)
				{
					if (!mbDynamicObjectsInCombinedMesh[i].show)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					int[] array = new int[this.submeshTris.Length];
					MB3_MeshCombinerSingle.SerializableIntArray[] array2 = new MB3_MeshCombinerSingle.SerializableIntArray[this.submeshTris.Length];
					for (int j = 0; j < mbDynamicObjectsInCombinedMesh.Count; j++)
					{
						MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject = mbDynamicObjectsInCombinedMesh[j];
						if (mb_DynamicGameObject.show)
						{
							for (int k = 0; k < mb_DynamicGameObject.submeshNumTris.Length; k++)
							{
								array[k] += mb_DynamicGameObject.submeshNumTris[k];
							}
						}
					}
					for (int l = 0; l < array2.Length; l++)
					{
						array2[l] = new MB3_MeshCombinerSingle.SerializableIntArray(array[l]);
					}
					int[] array3 = new int[array2.Length];
					for (int m = 0; m < mbDynamicObjectsInCombinedMesh.Count; m++)
					{
						MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject2 = mbDynamicObjectsInCombinedMesh[m];
						if (mb_DynamicGameObject2.show)
						{
							for (int n = 0; n < this.submeshTris.Length; n++)
							{
								int[] data = this.submeshTris[n].data;
								int num = mb_DynamicGameObject2.submeshTriIdxs[n];
								int num2 = num + mb_DynamicGameObject2.submeshNumTris[n];
								for (int num3 = num; num3 < num2; num3++)
								{
									array2[n].data[array3[n]] = data[num3];
									array3[n]++;
								}
							}
						}
					}
					return array2;
				}
				return this.submeshTris;
			}

			public int[] GetTriangleSizes()
			{
				int[] array = new int[this.submeshTris.Length];
				for (int i = 0; i < this.submeshTris.Length; i++)
				{
					array[i] = this.submeshTris[i].data.Length;
				}
				return array;
			}

			private void _LocalToWorld(Transform t, bool doNorm, bool doTan, int destStartVertsIdx, Vector3[] dgoMeshVerts, Vector3[] dgoMeshNorms, Vector4[] dgoMeshTans, Vector3[] verticies, Vector3[] normals, Vector4[] tangents)
			{
				Vector3 lossyScale = t.lossyScale;
				if (lossyScale == Vector3.one)
				{
					MB3_MeshCombinerSingle.VertexAndTriangleProcessor._LocalToWorld_TR(t.rotation, t.position, doNorm, doTan, destStartVertsIdx, dgoMeshVerts, dgoMeshNorms, dgoMeshTans, verticies, normals, tangents);
					return;
				}
				if (lossyScale.x > Mathf.Epsilon && lossyScale.y > Mathf.Epsilon && lossyScale.z > Mathf.Epsilon)
				{
					Matrix4x4 localToWorldMatrix = t.localToWorldMatrix;
					MB3_MeshCombinerSingle.VertexAndTriangleProcessor._LocalToWorldMatrix_TRS(ref localToWorldMatrix, doNorm, doTan, destStartVertsIdx, dgoMeshVerts, dgoMeshNorms, dgoMeshTans, verticies, normals, tangents);
					return;
				}
				MB3_MeshCombinerSingle.VertexAndTriangleProcessor._LocalToWorld_TRS(t.rotation, t.position, t.lossyScale, doNorm, doTan, destStartVertsIdx, dgoMeshVerts, dgoMeshNorms, dgoMeshTans, verticies, normals, tangents);
			}

			private static void _LocalToWorldMatrix_TRS(ref Matrix4x4 wld_X_local, bool doNorm, bool doTan, int destStartVertsIdx, Vector3[] dgoMeshVerts, Vector3[] dgoMeshNorms, Vector4[] dgoMeshTans, Vector3[] verticies, Vector3[] normals, Vector4[] tangents)
			{
				Matrix4x4 matrix4x = Matrix4x4.zero;
				if (doNorm || doTan)
				{
					matrix4x = wld_X_local;
					matrix4x[0, 3] = (matrix4x[1, 3] = (matrix4x[2, 3] = 0f));
					matrix4x = matrix4x.inverse.transpose;
				}
				for (int i = 0; i < dgoMeshVerts.Length; i++)
				{
					int num = destStartVertsIdx + i;
					verticies[num] = wld_X_local.MultiplyPoint3x4(dgoMeshVerts[i]);
					if (doNorm)
					{
						normals[num] = matrix4x.MultiplyPoint3x4(dgoMeshNorms[i]).normalized;
					}
					if (doTan)
					{
						float w = dgoMeshTans[i].w;
						Vector4 vector = matrix4x.MultiplyPoint3x4(dgoMeshTans[i]).normalized;
						vector.w = w;
						tangents[num] = vector;
					}
				}
			}

			private static void _LocalToWorld_TR(Quaternion wld_Rot_local, Vector3 position_wld, bool doNorm, bool doTan, int destStartVertsIdx, Vector3[] dgoMeshVerts_local, Vector3[] dgoMeshNorms_local, Vector4[] dgoMeshTans_local, Vector3[] verticies, Vector3[] normals, Vector4[] tangents)
			{
				for (int i = 0; i < dgoMeshVerts_local.Length; i++)
				{
					int num = destStartVertsIdx + i;
					Vector3 vector = dgoMeshVerts_local[i];
					vector = wld_Rot_local * vector;
					vector += position_wld;
					verticies[num] = vector;
					if (doNorm)
					{
						Vector3 vector2 = dgoMeshNorms_local[i];
						vector2 = wld_Rot_local * vector2;
						normals[num] = vector2;
					}
					if (doTan)
					{
						Vector3 vector3 = dgoMeshTans_local[i];
						float w = dgoMeshTans_local[i].w;
						vector3 = wld_Rot_local * vector3;
						Vector4 vector4 = vector3;
						vector4.w = w;
						tangents[num] = vector4;
					}
				}
			}

			private static void _LocalToWorld_TRS(Quaternion wld_Rot_local, Vector3 position_wld, Vector3 scale, bool doNorm, bool doTan, int destStartVertsIdx, Vector3[] dgoMeshVerts_local, Vector3[] dgoMeshNorms_local, Vector4[] dgoMeshTans_local, Vector3[] verticies, Vector3[] normals, Vector4[] tangents)
			{
				Vector3 one = Vector3.one;
				if (doNorm || doTan)
				{
					one.x = ((scale.x < Mathf.Epsilon) ? 0f : (1f / scale.x));
					one.y = ((scale.y < Mathf.Epsilon) ? 0f : (1f / scale.y));
					one.z = ((scale.z < Mathf.Epsilon) ? 0f : (1f / scale.z));
				}
				for (int i = 0; i < dgoMeshVerts_local.Length; i++)
				{
					int num = destStartVertsIdx + i;
					Vector3 vector = dgoMeshVerts_local[i];
					vector.x *= scale.x;
					vector.y *= scale.y;
					vector.z *= scale.z;
					vector = wld_Rot_local * vector;
					vector += position_wld;
					verticies[num] = vector;
					if (doNorm)
					{
						Vector3 vector2 = dgoMeshNorms_local[i];
						vector2.x *= one.x;
						vector2.y *= one.y;
						vector2.z *= one.z;
						vector2 = wld_Rot_local * vector2;
						vector2.Normalize();
						normals[num] = vector2;
					}
					if (doTan)
					{
						Vector3 vector3 = dgoMeshTans_local[i];
						float w = dgoMeshTans_local[i].w;
						vector3.x *= one.x;
						vector3.y *= one.y;
						vector3.z *= one.z;
						vector3 = wld_Rot_local * vector3;
						vector3.Normalize();
						tangents[num] = new Vector4(vector3.x, vector3.y, vector3.z, w);
					}
				}
			}

			private bool _disposed;

			private bool _isInitialized;

			internal MB2_LogLevel LOG_LEVEL;

			private Vector3[] verticies;

			private Vector3[] normals;

			private Vector4[] tangents;

			private Color[] colors;

			private Vector2[] uv0s;

			private float[] uvsSliceIdx;

			private Vector2[] uv2s;

			private Vector2[] uv3s;

			private Vector2[] uv4s;

			private Vector2[] uv5s;

			private Vector2[] uv6s;

			private Vector2[] uv7s;

			private Vector2[] uv8s;

			private MB3_MeshCombinerSingle.SerializableIntArray[] submeshTris;
		}

		public class MeshChannelsCache_NativeArray : IDisposable, MB3_MeshCombinerSingle.IMeshChannelsCacheTaggingInterface
		{
			public MeshChannelsCache_NativeArray(MB2_LogLevel ll, MB2_LightmapOptions lo)
			{
				this.LOG_LEVEL = ll;
				this.lightmapOption = lo;
			}

			public void Dispose()
			{
				this.Dispose(true);
			}

			protected virtual void Dispose(bool disposing)
			{
				if (this._disposed)
				{
					return;
				}
				foreach (MB3_MeshCombinerSingle.MeshChannelsNativeArray meshChannelsNativeArray in this.meshID2MeshChannels.Values)
				{
					meshChannelsNativeArray.Dispose();
				}
				this._collectedMeshData = false;
				this._disposed = true;
			}

			public bool HasCollectedMeshData()
			{
				return this._collectedMeshData;
			}

			public bool hasOutOfBoundsUVs(Mesh m, ref MB_Utility.MeshAnalysisResult mar, int submeshIdx)
			{
				return MB_Utility.hasOutOfBoundsUVs(this.GetUv0RawAsNativeArray(m), m, ref mar, submeshIdx);
			}

			internal NativeArray<Vector3> GetVerticiesAsNativeArray(Mesh m)
			{
				MB3_MeshCombinerSingle.MeshChannelsNativeArray meshChannelsNativeArray;
				if (!this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannelsNativeArray))
				{
					Debug.LogError("Could not find mesh in the MeshChannelsCache." + ((m != null) ? m.ToString() : null));
				}
				return meshChannelsNativeArray.vertcies_NativeArray;
			}

			internal NativeArray<Vector3> GetNormalsAsNativeArray(Mesh m)
			{
				MB3_MeshCombinerSingle.MeshChannelsNativeArray meshChannelsNativeArray;
				if (!this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannelsNativeArray))
				{
					Debug.LogError("Could not find mesh in the MeshChannelsCache." + ((m != null) ? m.ToString() : null));
				}
				return meshChannelsNativeArray.normals_NativeArray;
			}

			internal NativeArray<Vector4> GetTangentsAsNativeArray(Mesh m)
			{
				MB3_MeshCombinerSingle.MeshChannelsNativeArray meshChannelsNativeArray;
				if (!this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannelsNativeArray))
				{
					Debug.LogError("Could not find mesh in the MeshChannelsCache." + ((m != null) ? m.ToString() : null));
				}
				return meshChannelsNativeArray.tangents_NativeArray;
			}

			internal NativeArray<Vector2> GetUv0RawAsNativeArray(Mesh m)
			{
				MB3_MeshCombinerSingle.MeshChannelsNativeArray meshChannelsNativeArray;
				this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannelsNativeArray);
				return meshChannelsNativeArray.uv0raw_NativeArray;
			}

			internal NativeArray<Vector2> GetUv0ModifiedAsNativeArray(Mesh m)
			{
				MB3_MeshCombinerSingle.MeshChannelsNativeArray meshChannelsNativeArray;
				this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannelsNativeArray);
				if (!meshChannelsNativeArray.uv0modified_NativeArray.IsCreated)
				{
					meshChannelsNativeArray.uv0modified_NativeArray = new NativeArray<Vector2>(meshChannelsNativeArray.vertcies_NativeArray.Length, Allocator.Temp, NativeArrayOptions.ClearMemory);
				}
				return meshChannelsNativeArray.uv0modified_NativeArray;
			}

			internal NativeArray<Vector2> GetUv2ModifiedAsNativeArray(Mesh m)
			{
				MB3_MeshCombinerSingle.MeshChannelsNativeArray meshChannelsNativeArray;
				this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannelsNativeArray);
				if (!meshChannelsNativeArray.uv2modified_NativeArray.IsCreated)
				{
					meshChannelsNativeArray.uv2modified_NativeArray = new NativeArray<Vector2>(meshChannelsNativeArray.vertcies_NativeArray.Length, Allocator.Temp, NativeArrayOptions.ClearMemory);
				}
				return meshChannelsNativeArray.uv2modified_NativeArray;
			}

			internal NativeArray<Vector2> GetUVChannelAsNativeArray(int channel, Mesh m)
			{
				MB3_MeshCombinerSingle.MeshChannelsNativeArray meshChannelsNativeArray;
				this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannelsNativeArray);
				switch (channel)
				{
				case 0:
					return meshChannelsNativeArray.uv0raw_NativeArray;
				case 2:
					return meshChannelsNativeArray.uv2raw_NativeArray;
				case 3:
					return meshChannelsNativeArray.uv3_NativeArray;
				case 4:
					return meshChannelsNativeArray.uv4_NativeArray;
				case 5:
					return meshChannelsNativeArray.uv5_NativeArray;
				case 6:
					return meshChannelsNativeArray.uv6_NativeArray;
				case 7:
					return meshChannelsNativeArray.uv7_NativeArray;
				case 8:
					return meshChannelsNativeArray.uv8_NativeArray;
				}
				Debug.LogError("Error mesh channel " + channel.ToString() + " not supported");
				return default(NativeArray<Vector2>);
			}

			internal NativeArray<Color> GetColorsAsNativeArray(Mesh m)
			{
				MB3_MeshCombinerSingle.MeshChannelsNativeArray meshChannelsNativeArray;
				this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannelsNativeArray);
				return meshChannelsNativeArray.colors_NativeArray;
			}

			public void CollectChannelDataForAllMeshesInList(List<MB3_MeshCombinerSingle.MB_DynamicGameObject> toUpdateDGOs, List<MB3_MeshCombinerSingle.MB_DynamicGameObject> toAddDGOs, MB_MeshVertexChannelFlags newChannels, MB_RenderType renderType, bool doBlendShapes)
			{
				bool flag = (newChannels & MB_MeshVertexChannelFlags.vertex) == MB_MeshVertexChannelFlags.vertex;
				bool flag2 = (newChannels & MB_MeshVertexChannelFlags.normal) == MB_MeshVertexChannelFlags.normal;
				bool flag3 = (newChannels & MB_MeshVertexChannelFlags.tangent) == MB_MeshVertexChannelFlags.tangent;
				bool flag4 = (newChannels & MB_MeshVertexChannelFlags.uv0) == MB_MeshVertexChannelFlags.uv0;
				bool flag5 = (newChannels & MB_MeshVertexChannelFlags.uv2) == MB_MeshVertexChannelFlags.uv2;
				bool flag6 = (newChannels & MB_MeshVertexChannelFlags.uv3) == MB_MeshVertexChannelFlags.uv3;
				bool flag7 = (newChannels & MB_MeshVertexChannelFlags.uv4) == MB_MeshVertexChannelFlags.uv4;
				bool flag8 = (newChannels & MB_MeshVertexChannelFlags.uv5) == MB_MeshVertexChannelFlags.uv5;
				bool flag9 = (newChannels & MB_MeshVertexChannelFlags.uv6) == MB_MeshVertexChannelFlags.uv6;
				bool flag10 = (newChannels & MB_MeshVertexChannelFlags.uv7) == MB_MeshVertexChannelFlags.uv7;
				bool flag11 = (newChannels & MB_MeshVertexChannelFlags.uv8) == MB_MeshVertexChannelFlags.uv8;
				bool flag12 = (newChannels & MB_MeshVertexChannelFlags.colors) == MB_MeshVertexChannelFlags.colors;
				List<MB3_MeshCombinerSingle.MB_DynamicGameObject> list = new List<MB3_MeshCombinerSingle.MB_DynamicGameObject>();
				list.AddRange(toUpdateDGOs);
				list.AddRange(toAddDGOs);
				for (int i = 0; i < list.Count; i++)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject = list[i];
					Mesh mesh = mb_DynamicGameObject._mesh;
					if (!this.meshID2MeshChannels.ContainsKey(mesh.GetInstanceID()))
					{
						MB3_MeshCombinerSingle.MeshChannelsNativeArray meshChannelsNativeArray = new MB3_MeshCombinerSingle.MeshChannelsNativeArray();
						this.meshID2MeshChannels.Add(mesh.GetInstanceID(), meshChannelsNativeArray);
						if (flag)
						{
							meshChannelsNativeArray.vertcies_NativeArray = new NativeArray<Vector3>(mesh.vertices, Allocator.Temp);
						}
						if (flag4)
						{
							meshChannelsNativeArray.uv0raw_NativeArray = new NativeArray<Vector2>(this._getMeshUVs(mesh), Allocator.Temp);
						}
						if (flag5)
						{
							meshChannelsNativeArray.uv2raw_NativeArray = new NativeArray<Vector2>(this._getMeshUV2s(mesh, ref meshChannelsNativeArray.uv2modified_NativeArray), Allocator.Temp);
						}
						if (flag2)
						{
							meshChannelsNativeArray.normals_NativeArray = new NativeArray<Vector3>(this._getMeshNormals(mesh), Allocator.Temp);
						}
						if (flag3)
						{
							meshChannelsNativeArray.tangents_NativeArray = new NativeArray<Vector4>(this._getMeshTangents(mesh), Allocator.Temp);
						}
						if (flag6)
						{
							meshChannelsNativeArray.uv3_NativeArray = new NativeArray<Vector2>(MBVersion.GetMeshChannel(3, mesh, this.LOG_LEVEL), Allocator.Temp);
						}
						if (flag7)
						{
							meshChannelsNativeArray.uv4_NativeArray = new NativeArray<Vector2>(MBVersion.GetMeshChannel(4, mesh, this.LOG_LEVEL), Allocator.Temp);
						}
						if (flag8)
						{
							meshChannelsNativeArray.uv5_NativeArray = new NativeArray<Vector2>(MBVersion.GetMeshChannel(5, mesh, this.LOG_LEVEL), Allocator.Temp);
						}
						if (flag9)
						{
							meshChannelsNativeArray.uv6_NativeArray = new NativeArray<Vector2>(MBVersion.GetMeshChannel(6, mesh, this.LOG_LEVEL), Allocator.Temp);
						}
						if (flag10)
						{
							meshChannelsNativeArray.uv7_NativeArray = new NativeArray<Vector2>(MBVersion.GetMeshChannel(7, mesh, this.LOG_LEVEL), Allocator.Temp);
						}
						if (flag11)
						{
							meshChannelsNativeArray.uv8_NativeArray = new NativeArray<Vector2>(MBVersion.GetMeshChannel(8, mesh, this.LOG_LEVEL), Allocator.Temp);
						}
						if (flag12)
						{
							meshChannelsNativeArray.colors_NativeArray = new NativeArray<Color>(this._getMeshColors(mesh), Allocator.Temp);
						}
						if (renderType == MB_RenderType.skinnedMeshRenderer)
						{
							bool isSkinnedMeshWithBones = false;
							Renderer renderer = mb_DynamicGameObject._renderer;
							if (meshChannelsNativeArray.bindPoses == null || meshChannelsNativeArray.bindPoses.Count == 0)
							{
								MB3_MeshCombinerSingle.MeshChannelsCache_NativeArray._getBindPoses(renderer, meshChannelsNativeArray.bindPoses, out isSkinnedMeshWithBones);
								MB3_MeshCombinerSingle.MeshChannelsCache_NativeArray._getBoneWeightData(ref meshChannelsNativeArray.boneWeightData, renderer, meshChannelsNativeArray.bindPoses.Count, isSkinnedMeshWithBones);
							}
							if (doBlendShapes)
							{
								MB3_MeshCombinerSingle.MBBlendShape[] array = new MB3_MeshCombinerSingle.MBBlendShape[mesh.blendShapeCount];
								int vertexCount = mesh.vertexCount;
								for (int j = 0; j < array.Length; j++)
								{
									MB3_MeshCombinerSingle.MBBlendShape mbblendShape = array[j] = new MB3_MeshCombinerSingle.MBBlendShape();
									mbblendShape.frames = new MB3_MeshCombinerSingle.MBBlendShapeFrame[MBVersion.GetBlendShapeFrameCount(mesh, j)];
									mbblendShape.name = mesh.GetBlendShapeName(j);
									mbblendShape.indexInSource = j;
									mbblendShape.gameObject = mb_DynamicGameObject.gameObject;
									for (int k = 0; k < mbblendShape.frames.Length; k++)
									{
										MB3_MeshCombinerSingle.MBBlendShapeFrame mbblendShapeFrame = mbblendShape.frames[k] = new MB3_MeshCombinerSingle.MBBlendShapeFrame();
										mbblendShapeFrame.frameWeight = MBVersion.GetBlendShapeFrameWeight(mesh, j, k);
										mbblendShapeFrame.vertices = new Vector3[vertexCount];
										mbblendShapeFrame.normals = new Vector3[vertexCount];
										mbblendShapeFrame.tangents = new Vector3[vertexCount];
										MBVersion.GetBlendShapeFrameVertices(mesh, j, k, mbblendShapeFrame.vertices, mbblendShapeFrame.normals, mbblendShapeFrame.tangents);
									}
								}
								meshChannelsNativeArray.blendShapes = array;
							}
						}
					}
				}
				this._collectedMeshData = true;
			}

			internal List<Matrix4x4> GetBindposes(Renderer r, out bool isSkinnedMeshWithBones)
			{
				Mesh mesh = MB_Utility.GetMesh(r.gameObject);
				MB3_MeshCombinerSingle.MeshChannelsNativeArray meshChannelsNativeArray;
				this.meshID2MeshChannels.TryGetValue(mesh.GetInstanceID(), out meshChannelsNativeArray);
				if (r is SkinnedMeshRenderer && meshChannelsNativeArray.bindPoses.Count > 0)
				{
					isSkinnedMeshWithBones = true;
				}
				else
				{
					isSkinnedMeshWithBones = false;
					SkinnedMeshRenderer skinnedMeshRenderer = r as SkinnedMeshRenderer;
				}
				return meshChannelsNativeArray.bindPoses;
			}

			internal MB3_MeshCombinerSingle.BoneWeightDataForMesh GetBoneWeightData(Renderer r, int numbones, bool isSkinnedMeshWithBones)
			{
				Mesh mesh = MB_Utility.GetMesh(r.gameObject);
				MB3_MeshCombinerSingle.MeshChannelsNativeArray meshChannelsNativeArray;
				this.meshID2MeshChannels.TryGetValue(mesh.GetInstanceID(), out meshChannelsNativeArray);
				return meshChannelsNativeArray.boneWeightData;
			}

			public MB3_MeshCombinerSingle.MBBlendShape[] GetBlendShapes(Mesh m, int gameObjectID, GameObject gameObject)
			{
				MB3_MeshCombinerSingle.MeshChannelsNativeArray meshChannelsNativeArray;
				this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannelsNativeArray);
				MB3_MeshCombinerSingle.MBBlendShape[] array = new MB3_MeshCombinerSingle.MBBlendShape[meshChannelsNativeArray.blendShapes.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = new MB3_MeshCombinerSingle.MBBlendShape();
					array[i].name = meshChannelsNativeArray.blendShapes[i].name;
					array[i].indexInSource = meshChannelsNativeArray.blendShapes[i].indexInSource;
					array[i].frames = meshChannelsNativeArray.blendShapes[i].frames;
					array[i].gameObject = gameObject;
				}
				return array;
			}

			private Color[] _getMeshColors(Mesh m)
			{
				Color[] array = m.colors;
				if (array.Length == 0)
				{
					if (this.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug("Mesh " + ((m != null) ? m.ToString() : null) + " has no colors. Generating", Array.Empty<object>());
					}
					if (this.LOG_LEVEL >= MB2_LogLevel.warn)
					{
						Debug.LogWarning("Mesh " + ((m != null) ? m.ToString() : null) + " didn't have colors. Generating an array of white colors");
					}
					array = new Color[m.vertexCount];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = Color.white;
					}
				}
				return array;
			}

			private Vector3[] _getMeshNormals(Mesh m)
			{
				Vector3[] normals = m.normals;
				if (normals.Length == 0)
				{
					if (this.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug("Mesh " + ((m != null) ? m.ToString() : null) + " has no normals. Generating", Array.Empty<object>());
					}
					if (this.LOG_LEVEL >= MB2_LogLevel.warn)
					{
						Debug.LogWarning("Mesh " + ((m != null) ? m.ToString() : null) + " didn't have normals. Generating normals.");
					}
					Mesh mesh = Object.Instantiate<Mesh>(m);
					mesh.RecalculateNormals();
					normals = mesh.normals;
					MB_Utility.Destroy(mesh);
				}
				return normals;
			}

			private Vector4[] _getMeshTangents(Mesh m)
			{
				Vector4[] array = m.tangents;
				if (array.Length == 0)
				{
					if (this.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug("Mesh " + ((m != null) ? m.ToString() : null) + " has no tangents. Generating", Array.Empty<object>());
					}
					if (this.LOG_LEVEL >= MB2_LogLevel.warn)
					{
						Debug.LogWarning("Mesh " + ((m != null) ? m.ToString() : null) + " didn't have tangents. Generating tangents.");
					}
					Vector3[] vertices = m.vertices;
					NativeArray<Vector2> uv0Raw = this.GetUv0Raw(m);
					Vector3[] normals = this._getMeshNormals(m);
					array = new Vector4[m.vertexCount];
					for (int i = 0; i < m.subMeshCount; i++)
					{
						int[] triangles = m.GetTriangles(i);
						this._generateTangents(triangles, vertices, uv0Raw, normals, array);
					}
				}
				return array;
			}

			private Vector2[] _getMeshUVs(Mesh m)
			{
				Vector2[] array = m.uv;
				if (array.Length == 0)
				{
					array = new Vector2[m.vertexCount];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = this._HALF_UV;
					}
				}
				return array;
			}

			private Vector2[] _getMeshUV2s(Mesh m, ref NativeArray<Vector2> uv2modified)
			{
				Vector2[] uv = m.uv2;
				if (uv.Length == 0)
				{
					uv2modified = new NativeArray<Vector2>(m.vertexCount, Allocator.TempJob, NativeArrayOptions.ClearMemory);
					for (int i = 0; i < uv2modified.Length; i++)
					{
						uv2modified[i] = this._HALF_UV;
					}
				}
				return uv;
			}

			private static void _getBindPoses(Renderer r, List<Matrix4x4> poses, out bool isSkinnedMeshWithBones)
			{
				poses.Clear();
				isSkinnedMeshWithBones = (r is SkinnedMeshRenderer);
				if (r is SkinnedMeshRenderer)
				{
					Mesh mesh = MB_Utility.GetMesh(r.gameObject);
					mesh.GetBindposes(poses);
					if (poses.Count == 0)
					{
						if (mesh.blendShapeCount > 0)
						{
							isSkinnedMeshWithBones = false;
						}
						else
						{
							Debug.LogError("Skinned mesh " + ((r != null) ? r.ToString() : null) + " had no bindposes AND no blend shapes");
						}
					}
				}
				if (r is MeshRenderer || (r is SkinnedMeshRenderer && !isSkinnedMeshWithBones))
				{
					poses.Clear();
					poses.Add(Matrix4x4.identity);
				}
				if (poses == null || poses.Count == 0)
				{
					Debug.LogError("Could not _getBindPoses. Object does not have a renderer");
				}
			}

			private static void _getBoneWeightData(ref MB3_MeshCombinerSingle.BoneWeightDataForMesh bwd, Renderer r, int numBones, bool isSkinnedMeshWithBones)
			{
				if (isSkinnedMeshWithBones)
				{
					Mesh sharedMesh = ((SkinnedMeshRenderer)r).sharedMesh;
					bwd.initialized = true;
					bwd.weMustDispose = false;
					bwd.bonesPerVertex = sharedMesh.GetBonesPerVertex();
					bwd.boneWeights = sharedMesh.GetAllBoneWeights();
				}
				else if (r is MeshRenderer || (r is SkinnedMeshRenderer && !isSkinnedMeshWithBones))
				{
					Mesh mesh = MB_Utility.GetMesh(r.gameObject);
					bwd.initialized = true;
					bwd.weMustDispose = true;
					bwd.boneWeights = new NativeArray<BoneWeight1>(mesh.vertexCount, Allocator.Temp, NativeArrayOptions.ClearMemory);
					bwd.bonesPerVertex = new NativeArray<byte>(mesh.vertexCount, Allocator.Temp, NativeArrayOptions.ClearMemory);
					BoneWeight1 value = default(BoneWeight1);
					value.boneIndex = 0;
					value.weight = 1f;
					for (int i = 0; i < mesh.vertexCount; i++)
					{
						bwd.bonesPerVertex[i] = 1;
						bwd.boneWeights[i] = value;
					}
				}
				else
				{
					Debug.LogError("Could not _getBoneWeights. Object does not have a renderer");
				}
				bwd.UsedBoneIdxsInSrcMesh = new bool[numBones];
				for (int j = 0; j < bwd.boneWeights.Length; j++)
				{
					bwd.UsedBoneIdxsInSrcMesh[bwd.boneWeights[j].boneIndex] = true;
				}
				bwd.numUsedbones = 0;
				for (int k = 0; k < bwd.UsedBoneIdxsInSrcMesh.Length; k++)
				{
					if (bwd.UsedBoneIdxsInSrcMesh[k])
					{
						bwd.numUsedbones++;
					}
				}
			}

			internal NativeArray<Vector2> GetUv0Raw(Mesh m)
			{
				MB3_MeshCombinerSingle.MeshChannelsNativeArray meshChannelsNativeArray;
				this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannelsNativeArray);
				return meshChannelsNativeArray.uv0raw_NativeArray;
			}

			private static BoneWeight[] _getBoneWeights(Renderer r, int numVertsInMeshBeingAdded, bool isSkinnedMeshWithBones)
			{
				if (isSkinnedMeshWithBones)
				{
					return ((SkinnedMeshRenderer)r).sharedMesh.boneWeights;
				}
				if (r is MeshRenderer || (r is SkinnedMeshRenderer && !isSkinnedMeshWithBones))
				{
					BoneWeight boneWeight = default(BoneWeight);
					boneWeight.boneIndex0 = (boneWeight.boneIndex1 = (boneWeight.boneIndex2 = (boneWeight.boneIndex3 = 0)));
					boneWeight.weight0 = 1f;
					boneWeight.weight1 = (boneWeight.weight2 = (boneWeight.weight3 = 0f));
					BoneWeight[] array = new BoneWeight[numVertsInMeshBeingAdded];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = boneWeight;
					}
					return array;
				}
				Debug.LogError("Could not _getBoneWeights. Object does not have a renderer");
				return null;
			}

			private void _generateTangents(int[] triangles, Vector3[] verts, NativeArray<Vector2> uvs, Vector3[] normals, Vector4[] outTangents)
			{
				int num = triangles.Length;
				int num2 = verts.Length;
				Vector3[] array = new Vector3[num2];
				Vector3[] array2 = new Vector3[num2];
				for (int i = 0; i < num; i += 3)
				{
					int num3 = triangles[i];
					int num4 = triangles[i + 1];
					int num5 = triangles[i + 2];
					Vector3 vector = verts[num3];
					Vector3 vector2 = verts[num4];
					Vector3 vector3 = verts[num5];
					Vector2 vector4 = uvs[num3];
					Vector2 vector5 = uvs[num4];
					Vector2 vector6 = uvs[num5];
					float num6 = vector2.x - vector.x;
					float num7 = vector3.x - vector.x;
					float num8 = vector2.y - vector.y;
					float num9 = vector3.y - vector.y;
					float num10 = vector2.z - vector.z;
					float num11 = vector3.z - vector.z;
					float num12 = vector5.x - vector4.x;
					float num13 = vector6.x - vector4.x;
					float num14 = vector5.y - vector4.y;
					float num15 = vector6.y - vector4.y;
					float num16 = num12 * num15 - num13 * num14;
					if (num16 == 0f)
					{
						Debug.LogError("Could not compute tangents. All UVs need to form a valid triangles in UV space. If any UV triangles are collapsed, tangents cannot be generated.");
						return;
					}
					float num17 = 1f / num16;
					Vector3 b = new Vector3((num15 * num6 - num14 * num7) * num17, (num15 * num8 - num14 * num9) * num17, (num15 * num10 - num14 * num11) * num17);
					Vector3 b2 = new Vector3((num12 * num7 - num13 * num6) * num17, (num12 * num9 - num13 * num8) * num17, (num12 * num11 - num13 * num10) * num17);
					array[num3] += b;
					array[num4] += b;
					array[num5] += b;
					array2[num3] += b2;
					array2[num4] += b2;
					array2[num5] += b2;
				}
				for (int j = 0; j < num2; j++)
				{
					Vector3 vector7 = normals[j];
					Vector3 vector8 = array[j];
					Vector3 normalized = (vector8 - vector7 * Vector3.Dot(vector7, vector8)).normalized;
					outTangents[j] = new Vector4(normalized.x, normalized.y, normalized.z);
					outTangents[j].w = ((Vector3.Dot(Vector3.Cross(vector7, vector8), array2[j]) < 0f) ? -1f : 1f);
				}
			}

			private MB2_LogLevel LOG_LEVEL;

			private MB2_LightmapOptions lightmapOption;

			protected Dictionary<int, MB3_MeshCombinerSingle.MeshChannelsNativeArray> meshID2MeshChannels = new Dictionary<int, MB3_MeshCombinerSingle.MeshChannelsNativeArray>();

			private bool _collectedMeshData;

			private bool _disposed;

			private Vector2 _HALF_UV = new Vector2(0.5f, 0.5f);
		}

		public class MeshChannelsNativeArray : IDisposable
		{
			public void Dispose()
			{
				this.Dispose(true);
			}

			public bool IsDisposed()
			{
				return this._disposed;
			}

			protected virtual void Dispose(bool disposing)
			{
				if (this._disposed)
				{
					return;
				}
				this._disposed = true;
				this.boneWeightData.Dispose();
				if (this.vertcies_NativeArray.IsCreated)
				{
					this.vertcies_NativeArray.Dispose();
				}
				if (this.normals_NativeArray.IsCreated)
				{
					this.normals_NativeArray.Dispose();
				}
				if (this.tangents_NativeArray.IsCreated)
				{
					this.tangents_NativeArray.Dispose();
				}
				if (this.colors_NativeArray.IsCreated)
				{
					this.colors_NativeArray.Dispose();
				}
				if (this.uv0raw_NativeArray.IsCreated)
				{
					this.uv0raw_NativeArray.Dispose();
				}
				if (this.uv0modified_NativeArray.IsCreated)
				{
					this.uv0modified_NativeArray.Dispose();
				}
				if (this.uv2raw_NativeArray.IsCreated)
				{
					this.uv2raw_NativeArray.Dispose();
				}
				if (this.uv2modified_NativeArray.IsCreated)
				{
					this.uv2modified_NativeArray.Dispose();
				}
				if (this.uv3_NativeArray.IsCreated)
				{
					this.uv3_NativeArray.Dispose();
				}
				if (this.uv4_NativeArray.IsCreated)
				{
					this.uv4_NativeArray.Dispose();
				}
				if (this.uv5_NativeArray.IsCreated)
				{
					this.uv5_NativeArray.Dispose();
				}
				if (this.uv6_NativeArray.IsCreated)
				{
					this.uv6_NativeArray.Dispose();
				}
				if (this.uv7_NativeArray.IsCreated)
				{
					this.uv7_NativeArray.Dispose();
				}
				if (this.uv8_NativeArray.IsCreated)
				{
					this.uv8_NativeArray.Dispose();
				}
			}

			private bool _disposed;

			public NativeArray<Vector3> vertcies_NativeArray;

			public NativeArray<Vector3> normals_NativeArray;

			public NativeArray<Vector4> tangents_NativeArray;

			public NativeArray<Color> colors_NativeArray;

			public NativeArray<Vector2> uv0raw_NativeArray;

			public NativeArray<Vector2> uv0modified_NativeArray;

			public NativeArray<Vector2> uv2raw_NativeArray;

			public NativeArray<Vector2> uv2modified_NativeArray;

			public NativeArray<Vector2> uv3_NativeArray;

			public NativeArray<Vector2> uv4_NativeArray;

			public NativeArray<Vector2> uv5_NativeArray;

			public NativeArray<Vector2> uv6_NativeArray;

			public NativeArray<Vector2> uv7_NativeArray;

			public NativeArray<Vector2> uv8_NativeArray;

			public List<Matrix4x4> bindPoses = new List<Matrix4x4>(128);

			public MB3_MeshCombinerSingle.BoneWeightDataForMesh boneWeightData;

			public MB3_MeshCombinerSingle.MBBlendShape[] blendShapes;
		}

		public struct MB_MeshCombinerSingle_MeshNativeArrayHelper
		{
			[Preserve]
			public void _ENSURE_IL2CPP_CREATES_NECESSARY_CODE(ref Mesh.MeshData m)
			{
				Debug.LogError("This should never be called directly. It is only here to ensure these methodes are generated by the il2cpp compiler and not stripped so that they can be found by reflection.");
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_4> vertexData = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_4>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_4> nativeSlice = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_4>(vertexData);
				nativeSlice.SliceWithStride<Vector2>(0);
				nativeSlice.SliceWithStride<Vector3>(0);
				nativeSlice.SliceWithStride<Vector4>(0);
				nativeSlice.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_8> vertexData2 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_8>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_8> nativeSlice2 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_8>(vertexData2);
				nativeSlice2.SliceWithStride<Vector2>(0);
				nativeSlice2.SliceWithStride<Vector3>(0);
				nativeSlice2.SliceWithStride<Vector4>(0);
				nativeSlice2.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_12> vertexData3 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_12>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_12> nativeSlice3 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_12>(vertexData3);
				nativeSlice3.SliceWithStride<Vector2>(0);
				nativeSlice3.SliceWithStride<Vector3>(0);
				nativeSlice3.SliceWithStride<Vector4>(0);
				nativeSlice3.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_16> vertexData4 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_16>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_16> nativeSlice4 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_16>(vertexData4);
				nativeSlice4.SliceWithStride<Vector2>(0);
				nativeSlice4.SliceWithStride<Vector3>(0);
				nativeSlice4.SliceWithStride<Vector4>(0);
				nativeSlice4.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_20> vertexData5 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_20>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_20> nativeSlice5 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_20>(vertexData5);
				nativeSlice5.SliceWithStride<Vector2>(0);
				nativeSlice5.SliceWithStride<Vector3>(0);
				nativeSlice5.SliceWithStride<Vector4>(0);
				nativeSlice5.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_24> vertexData6 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_24>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_24> nativeSlice6 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_24>(vertexData6);
				nativeSlice6.SliceWithStride<Vector2>(0);
				nativeSlice6.SliceWithStride<Vector3>(0);
				nativeSlice6.SliceWithStride<Vector4>(0);
				nativeSlice6.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_28> vertexData7 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_28>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_28> nativeSlice7 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_28>(vertexData7);
				nativeSlice7.SliceWithStride<Vector2>(0);
				nativeSlice7.SliceWithStride<Vector3>(0);
				nativeSlice7.SliceWithStride<Vector4>(0);
				nativeSlice7.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_32> vertexData8 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_32>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_32> nativeSlice8 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_32>(vertexData8);
				nativeSlice8.SliceWithStride<Vector2>(0);
				nativeSlice8.SliceWithStride<Vector3>(0);
				nativeSlice8.SliceWithStride<Vector4>(0);
				nativeSlice8.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_36> vertexData9 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_36>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_36> nativeSlice9 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_36>(vertexData9);
				nativeSlice9.SliceWithStride<Vector2>(0);
				nativeSlice9.SliceWithStride<Vector3>(0);
				nativeSlice9.SliceWithStride<Vector4>(0);
				nativeSlice9.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_40> vertexData10 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_40>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_40> nativeSlice10 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_40>(vertexData10);
				nativeSlice10.SliceWithStride<Vector2>(0);
				nativeSlice10.SliceWithStride<Vector3>(0);
				nativeSlice10.SliceWithStride<Vector4>(0);
				nativeSlice10.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_44> vertexData11 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_44>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_44> nativeSlice11 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_44>(vertexData11);
				nativeSlice11.SliceWithStride<Vector2>(0);
				nativeSlice11.SliceWithStride<Vector3>(0);
				nativeSlice11.SliceWithStride<Vector4>(0);
				nativeSlice11.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_48> vertexData12 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_48>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_48> nativeSlice12 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_48>(vertexData12);
				nativeSlice12.SliceWithStride<Vector2>(0);
				nativeSlice12.SliceWithStride<Vector3>(0);
				nativeSlice12.SliceWithStride<Vector4>(0);
				nativeSlice12.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_52> vertexData13 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_52>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_52> nativeSlice13 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_52>(vertexData13);
				nativeSlice13.SliceWithStride<Vector2>(0);
				nativeSlice13.SliceWithStride<Vector3>(0);
				nativeSlice13.SliceWithStride<Vector4>(0);
				nativeSlice13.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_56> vertexData14 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_56>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_56> nativeSlice14 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_56>(vertexData14);
				nativeSlice14.SliceWithStride<Vector2>(0);
				nativeSlice14.SliceWithStride<Vector3>(0);
				nativeSlice14.SliceWithStride<Vector4>(0);
				nativeSlice14.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_60> vertexData15 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_60>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_60> nativeSlice15 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_60>(vertexData15);
				nativeSlice15.SliceWithStride<Vector2>(0);
				nativeSlice15.SliceWithStride<Vector3>(0);
				nativeSlice15.SliceWithStride<Vector4>(0);
				nativeSlice15.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_64> vertexData16 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_64>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_64> nativeSlice16 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_64>(vertexData16);
				nativeSlice16.SliceWithStride<Vector2>(0);
				nativeSlice16.SliceWithStride<Vector3>(0);
				nativeSlice16.SliceWithStride<Vector4>(0);
				nativeSlice16.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_68> vertexData17 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_68>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_68> nativeSlice17 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_68>(vertexData17);
				nativeSlice17.SliceWithStride<Vector2>(0);
				nativeSlice17.SliceWithStride<Vector3>(0);
				nativeSlice17.SliceWithStride<Vector4>(0);
				nativeSlice17.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_72> vertexData18 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_72>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_72> nativeSlice18 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_72>(vertexData18);
				nativeSlice18.SliceWithStride<Vector2>(0);
				nativeSlice18.SliceWithStride<Vector3>(0);
				nativeSlice18.SliceWithStride<Vector4>(0);
				nativeSlice18.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_76> vertexData19 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_76>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_76> nativeSlice19 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_76>(vertexData19);
				nativeSlice19.SliceWithStride<Vector2>(0);
				nativeSlice19.SliceWithStride<Vector3>(0);
				nativeSlice19.SliceWithStride<Vector4>(0);
				nativeSlice19.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_80> vertexData20 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_80>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_80> nativeSlice20 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_80>(vertexData20);
				nativeSlice20.SliceWithStride<Vector2>(0);
				nativeSlice20.SliceWithStride<Vector3>(0);
				nativeSlice20.SliceWithStride<Vector4>(0);
				nativeSlice20.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_84> vertexData21 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_84>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_84> nativeSlice21 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_84>(vertexData21);
				nativeSlice21.SliceWithStride<Vector2>(0);
				nativeSlice21.SliceWithStride<Vector3>(0);
				nativeSlice21.SliceWithStride<Vector4>(0);
				nativeSlice21.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_88> vertexData22 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_88>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_88> nativeSlice22 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_88>(vertexData22);
				nativeSlice22.SliceWithStride<Vector2>(0);
				nativeSlice22.SliceWithStride<Vector3>(0);
				nativeSlice22.SliceWithStride<Vector4>(0);
				nativeSlice22.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_92> vertexData23 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_92>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_92> nativeSlice23 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_92>(vertexData23);
				nativeSlice23.SliceWithStride<Vector2>(0);
				nativeSlice23.SliceWithStride<Vector3>(0);
				nativeSlice23.SliceWithStride<Vector4>(0);
				nativeSlice23.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_96> vertexData24 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_96>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_96> nativeSlice24 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_96>(vertexData24);
				nativeSlice24.SliceWithStride<Vector2>(0);
				nativeSlice24.SliceWithStride<Vector3>(0);
				nativeSlice24.SliceWithStride<Vector4>(0);
				nativeSlice24.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_100> vertexData25 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_100>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_100> nativeSlice25 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_100>(vertexData25);
				nativeSlice25.SliceWithStride<Vector2>(0);
				nativeSlice25.SliceWithStride<Vector3>(0);
				nativeSlice25.SliceWithStride<Vector4>(0);
				nativeSlice25.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_104> vertexData26 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_104>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_104> nativeSlice26 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_104>(vertexData26);
				nativeSlice26.SliceWithStride<Vector2>(0);
				nativeSlice26.SliceWithStride<Vector3>(0);
				nativeSlice26.SliceWithStride<Vector4>(0);
				nativeSlice26.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_108> vertexData27 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_108>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_108> nativeSlice27 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_108>(vertexData27);
				nativeSlice27.SliceWithStride<Vector2>(0);
				nativeSlice27.SliceWithStride<Vector3>(0);
				nativeSlice27.SliceWithStride<Vector4>(0);
				nativeSlice27.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_112> vertexData28 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_112>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_112> nativeSlice28 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_112>(vertexData28);
				nativeSlice28.SliceWithStride<Vector2>(0);
				nativeSlice28.SliceWithStride<Vector3>(0);
				nativeSlice28.SliceWithStride<Vector4>(0);
				nativeSlice28.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_116> vertexData29 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_116>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_116> nativeSlice29 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_116>(vertexData29);
				nativeSlice29.SliceWithStride<Vector2>(0);
				nativeSlice29.SliceWithStride<Vector3>(0);
				nativeSlice29.SliceWithStride<Vector4>(0);
				nativeSlice29.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_120> vertexData30 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_120>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_120> nativeSlice30 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_120>(vertexData30);
				nativeSlice30.SliceWithStride<Vector2>(0);
				nativeSlice30.SliceWithStride<Vector3>(0);
				nativeSlice30.SliceWithStride<Vector4>(0);
				nativeSlice30.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_124> vertexData31 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_124>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_124> nativeSlice31 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_124>(vertexData31);
				nativeSlice31.SliceWithStride<Vector2>(0);
				nativeSlice31.SliceWithStride<Vector3>(0);
				nativeSlice31.SliceWithStride<Vector4>(0);
				nativeSlice31.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_128> vertexData32 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_128>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_128> nativeSlice32 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_128>(vertexData32);
				nativeSlice32.SliceWithStride<Vector2>(0);
				nativeSlice32.SliceWithStride<Vector3>(0);
				nativeSlice32.SliceWithStride<Vector4>(0);
				nativeSlice32.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_132> vertexData33 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_132>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_132> nativeSlice33 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_132>(vertexData33);
				nativeSlice33.SliceWithStride<Vector2>(0);
				nativeSlice33.SliceWithStride<Vector3>(0);
				nativeSlice33.SliceWithStride<Vector4>(0);
				nativeSlice33.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_136> vertexData34 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_136>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_136> nativeSlice34 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_136>(vertexData34);
				nativeSlice34.SliceWithStride<Vector2>(0);
				nativeSlice34.SliceWithStride<Vector3>(0);
				nativeSlice34.SliceWithStride<Vector4>(0);
				nativeSlice34.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_140> vertexData35 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_140>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_140> nativeSlice35 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_140>(vertexData35);
				nativeSlice35.SliceWithStride<Vector2>(0);
				nativeSlice35.SliceWithStride<Vector3>(0);
				nativeSlice35.SliceWithStride<Vector4>(0);
				nativeSlice35.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_144> vertexData36 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_144>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_144> nativeSlice36 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_144>(vertexData36);
				nativeSlice36.SliceWithStride<Vector2>(0);
				nativeSlice36.SliceWithStride<Vector3>(0);
				nativeSlice36.SliceWithStride<Vector4>(0);
				nativeSlice36.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_148> vertexData37 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_148>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_148> nativeSlice37 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_148>(vertexData37);
				nativeSlice37.SliceWithStride<Vector2>(0);
				nativeSlice37.SliceWithStride<Vector3>(0);
				nativeSlice37.SliceWithStride<Vector4>(0);
				nativeSlice37.SliceWithStride<Color32>(0);
				NativeArray<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_152> vertexData38 = m.GetVertexData<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_152>(0);
				NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_152> nativeSlice38 = new NativeSlice<MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_152>(vertexData38);
				nativeSlice38.SliceWithStride<Vector2>(0);
				nativeSlice38.SliceWithStride<Vector3>(0);
				nativeSlice38.SliceWithStride<Vector4>(0);
				nativeSlice38.SliceWithStride<Color32>(0);
			}

			public static int CalcStride(MB_MeshVertexChannelFlags channels, int uvChannelWithExtraParameter, out int strideVertexBuffer, out int strideUVbuffer)
			{
				strideVertexBuffer = 0;
				strideUVbuffer = 0;
				if ((channels & MB_MeshVertexChannelFlags.vertex) == MB_MeshVertexChannelFlags.vertex)
				{
					strideVertexBuffer += 12;
				}
				if ((channels & MB_MeshVertexChannelFlags.normal) == MB_MeshVertexChannelFlags.normal)
				{
					strideVertexBuffer += 12;
				}
				if ((channels & MB_MeshVertexChannelFlags.tangent) == MB_MeshVertexChannelFlags.tangent)
				{
					strideVertexBuffer += 16;
				}
				if ((channels & MB_MeshVertexChannelFlags.colors) == MB_MeshVertexChannelFlags.colors)
				{
					strideUVbuffer += 16;
				}
				if ((channels & MB_MeshVertexChannelFlags.uv0) == MB_MeshVertexChannelFlags.uv0)
				{
					strideUVbuffer += 8;
				}
				if ((channels & MB_MeshVertexChannelFlags.uv2) == MB_MeshVertexChannelFlags.uv2)
				{
					strideUVbuffer += 8;
				}
				if ((channels & MB_MeshVertexChannelFlags.uv3) == MB_MeshVertexChannelFlags.uv3)
				{
					strideUVbuffer += 8;
				}
				if ((channels & MB_MeshVertexChannelFlags.uv4) == MB_MeshVertexChannelFlags.uv4)
				{
					strideUVbuffer += 8;
				}
				if ((channels & MB_MeshVertexChannelFlags.uv5) == MB_MeshVertexChannelFlags.uv5)
				{
					strideUVbuffer += 8;
				}
				if ((channels & MB_MeshVertexChannelFlags.uv6) == MB_MeshVertexChannelFlags.uv6)
				{
					strideUVbuffer += 8;
				}
				if ((channels & MB_MeshVertexChannelFlags.uv7) == MB_MeshVertexChannelFlags.uv7)
				{
					strideUVbuffer += 8;
				}
				if ((channels & MB_MeshVertexChannelFlags.uv8) == MB_MeshVertexChannelFlags.uv8)
				{
					strideUVbuffer += 8;
				}
				MB_MeshVertexChannelFlags mb_MeshVertexChannelFlags = channels & MB_MeshVertexChannelFlags.blendWeight;
				if (uvChannelWithExtraParameter >= 0)
				{
					strideUVbuffer += 4;
				}
				return strideVertexBuffer + strideUVbuffer;
			}

			public static void Init(MB_MeshVertexChannelFlags channels, VertexAttributeDescriptor[] vertexAttributes, ref MB3_MeshCombinerSingle.VertexAndTriangleProcessorNativeArray nativeSlices, int vertexCount, int[] submeshCount, int uvChannelWithExtraParameter)
			{
				int strideVertexData;
				int num;
				MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.CalcStride(channels, uvChannelWithExtraParameter, out strideVertexData, out num);
				int num2 = 0;
				int stream = 1;
				int stream2 = 2;
				int num3 = 0;
				int stream3 = num3;
				num2++;
				num3++;
				if (num > 0)
				{
					stream = num3;
					num3++;
					num2++;
				}
				int num4 = 0;
				if ((channels & MB_MeshVertexChannelFlags.vertex) == MB_MeshVertexChannelFlags.vertex)
				{
					vertexAttributes[num4] = new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, stream3);
					num4++;
				}
				if ((channels & MB_MeshVertexChannelFlags.normal) == MB_MeshVertexChannelFlags.normal)
				{
					vertexAttributes[num4] = new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3, stream3);
					num4++;
				}
				if ((channels & MB_MeshVertexChannelFlags.tangent) == MB_MeshVertexChannelFlags.tangent)
				{
					vertexAttributes[num4] = new VertexAttributeDescriptor(VertexAttribute.Tangent, VertexAttributeFormat.Float32, 4, stream3);
					num4++;
				}
				if ((channels & MB_MeshVertexChannelFlags.colors) == MB_MeshVertexChannelFlags.colors)
				{
					vertexAttributes[num4] = new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.Float32, 4, stream);
					num4++;
				}
				if ((channels & MB_MeshVertexChannelFlags.uv0) == MB_MeshVertexChannelFlags.uv0)
				{
					int dimension = (uvChannelWithExtraParameter == 0) ? 3 : 2;
					vertexAttributes[num4] = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, dimension, stream);
					num4++;
				}
				if ((channels & MB_MeshVertexChannelFlags.uv2) == MB_MeshVertexChannelFlags.uv2)
				{
					int dimension2 = (uvChannelWithExtraParameter == 1) ? 3 : 2;
					vertexAttributes[num4] = new VertexAttributeDescriptor(VertexAttribute.TexCoord1, VertexAttributeFormat.Float32, dimension2, stream);
					num4++;
				}
				if ((channels & MB_MeshVertexChannelFlags.uv3) == MB_MeshVertexChannelFlags.uv3)
				{
					int dimension3 = (uvChannelWithExtraParameter == 2) ? 3 : 2;
					vertexAttributes[num4] = new VertexAttributeDescriptor(VertexAttribute.TexCoord2, VertexAttributeFormat.Float32, dimension3, stream);
					num4++;
				}
				if ((channels & MB_MeshVertexChannelFlags.uv4) == MB_MeshVertexChannelFlags.uv4)
				{
					int dimension4 = (uvChannelWithExtraParameter == 3) ? 3 : 2;
					vertexAttributes[num4] = new VertexAttributeDescriptor(VertexAttribute.TexCoord3, VertexAttributeFormat.Float32, dimension4, stream);
					num4++;
				}
				if ((channels & MB_MeshVertexChannelFlags.uv5) == MB_MeshVertexChannelFlags.uv5)
				{
					int dimension5 = (uvChannelWithExtraParameter == 4) ? 3 : 2;
					vertexAttributes[num4] = new VertexAttributeDescriptor(VertexAttribute.TexCoord4, VertexAttributeFormat.Float32, dimension5, stream);
					num4++;
				}
				if ((channels & MB_MeshVertexChannelFlags.uv6) == MB_MeshVertexChannelFlags.uv6)
				{
					int dimension6 = (uvChannelWithExtraParameter == 5) ? 3 : 2;
					vertexAttributes[num4] = new VertexAttributeDescriptor(VertexAttribute.TexCoord5, VertexAttributeFormat.Float32, dimension6, stream);
					num4++;
				}
				if ((channels & MB_MeshVertexChannelFlags.uv7) == MB_MeshVertexChannelFlags.uv7)
				{
					int dimension7 = (uvChannelWithExtraParameter == 6) ? 3 : 2;
					vertexAttributes[num4] = new VertexAttributeDescriptor(VertexAttribute.TexCoord6, VertexAttributeFormat.Float32, dimension7, stream);
					num4++;
				}
				if ((channels & MB_MeshVertexChannelFlags.uv8) == MB_MeshVertexChannelFlags.uv8)
				{
					int dimension8 = (uvChannelWithExtraParameter == 7) ? 3 : 2;
					vertexAttributes[num4] = new VertexAttributeDescriptor(VertexAttribute.TexCoord7, VertexAttributeFormat.Float32, dimension8, stream);
					num4++;
				}
				if ((channels & MB_MeshVertexChannelFlags.blendWeight) == MB_MeshVertexChannelFlags.blendWeight)
				{
					vertexAttributes[num4] = new VertexAttributeDescriptor(VertexAttribute.BlendWeight, VertexAttributeFormat.UNorm16, 4, stream2);
					num4++;
					vertexAttributes[num4] = new VertexAttributeDescriptor(VertexAttribute.BlendIndices, VertexAttributeFormat.UInt16, 4, stream2);
					num4++;
				}
				MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.AllocateWriteableMeshData(ref nativeSlices, vertexAttributes, vertexCount, num2);
				MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SetupNativeSlices(ref nativeSlices, strideVertexData, num, uvChannelWithExtraParameter);
				nativeSlices.triangleBuffer = nativeSlices.data.GetIndexData<ushort>();
			}

			public static void AllocateWriteableMeshData(ref MB3_MeshCombinerSingle.VertexAndTriangleProcessorNativeArray nativeSlices, VertexAttributeDescriptor[] channels, int vertexCount, int numBuffers)
			{
				nativeSlices.dataArray = Mesh.AllocateWritableMeshData(1);
				nativeSlices.dataArrayAllocated = true;
				nativeSlices.data = nativeSlices.dataArray[0];
				if (nativeSlices.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					string text = "Allocating VertexChannels for combined mesh: ";
					for (int i = 0; i < channels.Length; i++)
					{
						string str = text;
						string str2 = "\n   ";
						VertexAttributeDescriptor vertexAttributeDescriptor = channels[i];
						text = str + str2 + vertexAttributeDescriptor.ToString();
					}
					Debug.Log(text);
				}
				nativeSlices.data.SetVertexBufferParams(vertexCount, channels);
			}

			public static void SetupNativeSlices(ref MB3_MeshCombinerSingle.VertexAndTriangleProcessorNativeArray nativeSlices, int strideVertexData, int strideUVdata, int uvChannelWithExtraParameter)
			{
				ref Mesh.MeshData ptr = ref nativeSlices.data;
				nativeSlices.bufferStride_0 = strideVertexData;
				nativeSlices.bufferStride_1 = strideUVdata;
				int num = 0;
				Type type = nativeSlices.rawSliceSizerType_0 = MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper._TypeForStride[strideVertexData];
				object obj = ptr.GetType().GetMethod("GetVertexData", new Type[]
				{
					typeof(int)
				}).MakeGenericMethod(new Type[]
				{
					type
				}).Invoke(ptr, new object[]
				{
					num
				});
				Type type2 = typeof(NativeSlice<>).MakeGenericType(new Type[]
				{
					type
				});
				nativeSlices.rawSliceVertexStream_0 = Activator.CreateInstance(type2, new object[]
				{
					obj
				});
				int num2 = (int)nativeSlices.rawSliceVertexStream_0.GetType().GetProperty("Length").GetValue(nativeSlices.rawSliceVertexStream_0, null);
				nativeSlices.vertexCount = num2;
				MethodInfo method = nativeSlices.rawSliceVertexStream_0.GetType().GetMethod("SliceWithStride", new Type[]
				{
					typeof(int)
				});
				int num3 = 0;
				if ((nativeSlices.channels & MB_MeshVertexChannelFlags.vertex) == MB_MeshVertexChannelFlags.vertex)
				{
					MethodInfo methodInfo = method.MakeGenericMethod(new Type[]
					{
						typeof(Vector3)
					});
					nativeSlices.verticies = (NativeSlice<Vector3>)methodInfo.Invoke(nativeSlices.rawSliceVertexStream_0, new object[]
					{
						num3
					});
					num3 += sizeof(Vector3);
				}
				if ((nativeSlices.channels & MB_MeshVertexChannelFlags.normal) == MB_MeshVertexChannelFlags.normal)
				{
					MethodInfo methodInfo2 = method.MakeGenericMethod(new Type[]
					{
						typeof(Vector3)
					});
					nativeSlices.normals = (NativeSlice<Vector3>)methodInfo2.Invoke(nativeSlices.rawSliceVertexStream_0, new object[]
					{
						num3
					});
					num3 += sizeof(Vector3);
				}
				if ((nativeSlices.channels & MB_MeshVertexChannelFlags.tangent) == MB_MeshVertexChannelFlags.tangent)
				{
					MethodInfo methodInfo3 = method.MakeGenericMethod(new Type[]
					{
						typeof(Vector4)
					});
					nativeSlices.tangents = (NativeSlice<Vector4>)methodInfo3.Invoke(nativeSlices.rawSliceVertexStream_0, new object[]
					{
						num3
					});
					num3 += sizeof(Vector4);
				}
				if (strideUVdata > 0)
				{
					num++;
					Type type3 = nativeSlices.rawSliceSizerType_1 = MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper._TypeForStride[strideUVdata];
					object obj2 = ptr.GetType().GetMethod("GetVertexData", new Type[]
					{
						typeof(int)
					}).MakeGenericMethod(new Type[]
					{
						type3
					}).Invoke(ptr, new object[]
					{
						num
					});
					Type type4 = typeof(NativeSlice<>).MakeGenericType(new Type[]
					{
						type3
					});
					nativeSlices.rawSliceVertexStream_1 = Activator.CreateInstance(type4, new object[]
					{
						obj2
					});
					MethodInfo method2 = nativeSlices.rawSliceVertexStream_1.GetType().GetMethod("SliceWithStride", new Type[]
					{
						typeof(int)
					});
					int num4 = 0;
					if ((nativeSlices.channels & MB_MeshVertexChannelFlags.colors) == MB_MeshVertexChannelFlags.colors)
					{
						MethodInfo methodInfo4 = method2.MakeGenericMethod(new Type[]
						{
							typeof(Color)
						});
						nativeSlices.colors = (NativeSlice<Color>)methodInfo4.Invoke(nativeSlices.rawSliceVertexStream_1, new object[]
						{
							num4
						});
						num4 += sizeof(Color);
					}
					if ((nativeSlices.channels & MB_MeshVertexChannelFlags.uv0) == MB_MeshVertexChannelFlags.uv0)
					{
						MethodInfo methodInfo5 = method2.MakeGenericMethod(new Type[]
						{
							typeof(Vector2)
						});
						nativeSlices.uv0s = (NativeSlice<Vector2>)methodInfo5.Invoke(nativeSlices.rawSliceVertexStream_1, new object[]
						{
							num4
						});
						if (uvChannelWithExtraParameter == 0)
						{
							methodInfo5 = method2.MakeGenericMethod(new Type[]
							{
								typeof(Vector3)
							});
							nativeSlices.uvsWithExtraIndex = (NativeSlice<Vector3>)methodInfo5.Invoke(nativeSlices.rawSliceVertexStream_1, new object[]
							{
								num4
							});
							methodInfo5 = method2.MakeGenericMethod(new Type[]
							{
								typeof(float)
							});
							num4 += sizeof(Vector2);
							nativeSlices.uvsSliceIdx = (NativeSlice<float>)methodInfo5.Invoke(nativeSlices.rawSliceVertexStream_1, new object[]
							{
								num4
							});
							num4 += 4;
						}
						else
						{
							num4 += sizeof(Vector2);
						}
					}
					if ((nativeSlices.channels & MB_MeshVertexChannelFlags.uv2) == MB_MeshVertexChannelFlags.uv2)
					{
						MethodInfo methodInfo6 = method2.MakeGenericMethod(new Type[]
						{
							typeof(Vector2)
						});
						nativeSlices.uv2s = (NativeSlice<Vector2>)methodInfo6.Invoke(nativeSlices.rawSliceVertexStream_1, new object[]
						{
							num4
						});
						if (uvChannelWithExtraParameter == 1)
						{
							methodInfo6 = method2.MakeGenericMethod(new Type[]
							{
								typeof(Vector3)
							});
							nativeSlices.uvsWithExtraIndex = (NativeSlice<Vector3>)methodInfo6.Invoke(nativeSlices.rawSliceVertexStream_1, new object[]
							{
								num4
							});
							methodInfo6 = method2.MakeGenericMethod(new Type[]
							{
								typeof(float)
							});
							num4 += sizeof(Vector2);
							nativeSlices.uvsSliceIdx = (NativeSlice<float>)methodInfo6.Invoke(nativeSlices.rawSliceVertexStream_1, new object[]
							{
								num4
							});
							num4 += 4;
						}
						else
						{
							num4 += sizeof(Vector2);
						}
					}
					if ((nativeSlices.channels & MB_MeshVertexChannelFlags.uv3) == MB_MeshVertexChannelFlags.uv3)
					{
						MethodInfo methodInfo7 = method2.MakeGenericMethod(new Type[]
						{
							typeof(Vector2)
						});
						nativeSlices.uv3s = (NativeSlice<Vector2>)methodInfo7.Invoke(nativeSlices.rawSliceVertexStream_1, new object[]
						{
							num4
						});
						if (uvChannelWithExtraParameter == 2)
						{
							methodInfo7 = method2.MakeGenericMethod(new Type[]
							{
								typeof(Vector3)
							});
							nativeSlices.uvsWithExtraIndex = (NativeSlice<Vector3>)methodInfo7.Invoke(nativeSlices.rawSliceVertexStream_1, new object[]
							{
								num4
							});
							methodInfo7 = method2.MakeGenericMethod(new Type[]
							{
								typeof(float)
							});
							num4 += sizeof(Vector2);
							nativeSlices.uvsSliceIdx = (NativeSlice<float>)methodInfo7.Invoke(nativeSlices.rawSliceVertexStream_1, new object[]
							{
								num4
							});
							num4 += 4;
						}
						else
						{
							num4 += sizeof(Vector2);
						}
					}
					if ((nativeSlices.channels & MB_MeshVertexChannelFlags.uv4) == MB_MeshVertexChannelFlags.uv4)
					{
						MethodInfo methodInfo8 = method2.MakeGenericMethod(new Type[]
						{
							typeof(Vector2)
						});
						nativeSlices.uv4s = (NativeSlice<Vector2>)methodInfo8.Invoke(nativeSlices.rawSliceVertexStream_1, new object[]
						{
							num4
						});
						if (uvChannelWithExtraParameter == 3)
						{
							methodInfo8 = method2.MakeGenericMethod(new Type[]
							{
								typeof(Vector3)
							});
							nativeSlices.uvsWithExtraIndex = (NativeSlice<Vector3>)methodInfo8.Invoke(nativeSlices.rawSliceVertexStream_1, new object[]
							{
								num4
							});
							methodInfo8 = method2.MakeGenericMethod(new Type[]
							{
								typeof(float)
							});
							num4 += sizeof(Vector2);
							nativeSlices.uvsSliceIdx = (NativeSlice<float>)methodInfo8.Invoke(nativeSlices.rawSliceVertexStream_1, new object[]
							{
								num4
							});
							num4 += 4;
						}
						else
						{
							num4 += sizeof(Vector2);
						}
					}
					if ((nativeSlices.channels & MB_MeshVertexChannelFlags.uv5) == MB_MeshVertexChannelFlags.uv5)
					{
						MethodInfo methodInfo9 = method2.MakeGenericMethod(new Type[]
						{
							typeof(Vector2)
						});
						nativeSlices.uv5s = (NativeSlice<Vector2>)methodInfo9.Invoke(nativeSlices.rawSliceVertexStream_1, new object[]
						{
							num4
						});
						if (uvChannelWithExtraParameter == 4)
						{
							methodInfo9 = method2.MakeGenericMethod(new Type[]
							{
								typeof(Vector3)
							});
							nativeSlices.uvsWithExtraIndex = (NativeSlice<Vector3>)methodInfo9.Invoke(nativeSlices.rawSliceVertexStream_1, new object[]
							{
								num4
							});
							methodInfo9 = method2.MakeGenericMethod(new Type[]
							{
								typeof(float)
							});
							num4 += sizeof(Vector2);
							nativeSlices.uvsSliceIdx = (NativeSlice<float>)methodInfo9.Invoke(nativeSlices.rawSliceVertexStream_1, new object[]
							{
								num4
							});
							num4 += 4;
						}
						else
						{
							num4 += sizeof(Vector2);
						}
					}
					if ((nativeSlices.channels & MB_MeshVertexChannelFlags.uv6) == MB_MeshVertexChannelFlags.uv6)
					{
						MethodInfo methodInfo10 = method2.MakeGenericMethod(new Type[]
						{
							typeof(Vector2)
						});
						nativeSlices.uv6s = (NativeSlice<Vector2>)methodInfo10.Invoke(nativeSlices.rawSliceVertexStream_1, new object[]
						{
							num4
						});
						if (uvChannelWithExtraParameter == 5)
						{
							methodInfo10 = method2.MakeGenericMethod(new Type[]
							{
								typeof(Vector3)
							});
							nativeSlices.uvsWithExtraIndex = (NativeSlice<Vector3>)methodInfo10.Invoke(nativeSlices.rawSliceVertexStream_1, new object[]
							{
								num4
							});
							methodInfo10 = method2.MakeGenericMethod(new Type[]
							{
								typeof(float)
							});
							num4 += sizeof(Vector2);
							nativeSlices.uvsSliceIdx = (NativeSlice<float>)methodInfo10.Invoke(nativeSlices.rawSliceVertexStream_1, new object[]
							{
								num4
							});
							num4 += 4;
						}
						else
						{
							num4 += sizeof(Vector2);
						}
					}
					if ((nativeSlices.channels & MB_MeshVertexChannelFlags.uv7) == MB_MeshVertexChannelFlags.uv7)
					{
						MethodInfo methodInfo11 = method2.MakeGenericMethod(new Type[]
						{
							typeof(Vector2)
						});
						nativeSlices.uv7s = (NativeSlice<Vector2>)methodInfo11.Invoke(nativeSlices.rawSliceVertexStream_1, new object[]
						{
							num4
						});
						if (uvChannelWithExtraParameter == 6)
						{
							methodInfo11 = method2.MakeGenericMethod(new Type[]
							{
								typeof(Vector3)
							});
							nativeSlices.uvsWithExtraIndex = (NativeSlice<Vector3>)methodInfo11.Invoke(nativeSlices.rawSliceVertexStream_1, new object[]
							{
								num4
							});
							methodInfo11 = method2.MakeGenericMethod(new Type[]
							{
								typeof(float)
							});
							num4 += sizeof(Vector2);
							nativeSlices.uvsSliceIdx = (NativeSlice<float>)methodInfo11.Invoke(nativeSlices.rawSliceVertexStream_1, new object[]
							{
								num4
							});
							num4 += 4;
						}
						else
						{
							num4 += sizeof(Vector2);
						}
					}
					if ((nativeSlices.channels & MB_MeshVertexChannelFlags.uv8) == MB_MeshVertexChannelFlags.uv8)
					{
						MethodInfo methodInfo12 = method2.MakeGenericMethod(new Type[]
						{
							typeof(Vector2)
						});
						nativeSlices.uv8s = (NativeSlice<Vector2>)methodInfo12.Invoke(nativeSlices.rawSliceVertexStream_1, new object[]
						{
							num4
						});
						if (uvChannelWithExtraParameter == 7)
						{
							methodInfo12 = method2.MakeGenericMethod(new Type[]
							{
								typeof(Vector3)
							});
							nativeSlices.uvsWithExtraIndex = (NativeSlice<Vector3>)methodInfo12.Invoke(nativeSlices.rawSliceVertexStream_1, new object[]
							{
								num4
							});
							methodInfo12 = method2.MakeGenericMethod(new Type[]
							{
								typeof(float)
							});
							num4 += sizeof(Vector2);
							nativeSlices.uvsSliceIdx = (NativeSlice<float>)methodInfo12.Invoke(nativeSlices.rawSliceVertexStream_1, new object[]
							{
								num4
							});
							num4 += 4;
							return;
						}
						num4 += sizeof(Vector2);
					}
				}
			}

			public static void NativeSliceCopyFrom(object toHereSlice, Type toHereSizerType, object fromHereSlice, Type fromHereSizerType)
			{
				Type type = typeof(NativeSlice<>).MakeGenericType(new Type[]
				{
					fromHereSizerType
				});
				type.GetMethod("CopyFrom", new Type[]
				{
					type
				}).Invoke(toHereSlice, new object[]
				{
					fromHereSlice
				});
			}

			public static void NativeSliceCopy<T>(NativeSlice<T> srcArray, int srcStartIdx, NativeSlice<T> destArray, int destStartIdx, int length) where T : struct
			{
				NativeSlice<T> slice = srcArray.Slice(srcStartIdx, length);
				destArray.Slice(destStartIdx, length).CopyFrom(slice);
			}

			public static void NativeSliceCopyTo<T>(NativeSlice<T> srcArray, NativeSlice<T> destArray, int destStartIdx) where T : struct
			{
				destArray.Slice(destStartIdx, srcArray.Length).CopyFrom(srcArray);
			}

			private static Type[] _TypeForStride = new Type[]
			{
				null,
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_4),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_8),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_12),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_16),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_20),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_24),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_28),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_32),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_36),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_40),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_44),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_48),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_52),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_56),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_60),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_64),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_68),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_72),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_76),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_80),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_84),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_88),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_92),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_96),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_100),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_104),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_108),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_112),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_116),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_120),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_124),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_128),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_132),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_136),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_140),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_144),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_148),
				null,
				null,
				null,
				typeof(MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_152)
			};

			public Mesh.MeshDataArray dataArray;

			public Mesh.MeshData data;

			public int vertexCount;

			public struct SIZER_4
			{
				[FixedBuffer(typeof(byte), 4)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_4.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 4)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_8
			{
				[FixedBuffer(typeof(byte), 8)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_8.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 8)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_12
			{
				[FixedBuffer(typeof(byte), 12)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_12.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 12)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_16
			{
				[FixedBuffer(typeof(byte), 16)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_16.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 16)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_20
			{
				[FixedBuffer(typeof(byte), 20)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_20.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 20)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_24
			{
				[FixedBuffer(typeof(byte), 24)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_24.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 24)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_28
			{
				[FixedBuffer(typeof(byte), 28)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_28.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 28)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_32
			{
				[FixedBuffer(typeof(byte), 32)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_32.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 32)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_36
			{
				[FixedBuffer(typeof(byte), 36)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_36.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 36)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_40
			{
				[FixedBuffer(typeof(byte), 40)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_40.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 40)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_44
			{
				[FixedBuffer(typeof(byte), 44)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_44.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 44)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_48
			{
				[FixedBuffer(typeof(byte), 48)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_48.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 48)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_52
			{
				[FixedBuffer(typeof(byte), 52)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_52.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 52)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_56
			{
				[FixedBuffer(typeof(byte), 56)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_56.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 56)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_60
			{
				[FixedBuffer(typeof(byte), 60)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_60.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 60)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_64
			{
				[FixedBuffer(typeof(byte), 64)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_64.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 64)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_68
			{
				[FixedBuffer(typeof(byte), 68)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_68.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 68)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_72
			{
				[FixedBuffer(typeof(byte), 72)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_72.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 72)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_76
			{
				[FixedBuffer(typeof(byte), 72)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_76.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 72)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_80
			{
				[FixedBuffer(typeof(byte), 80)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_80.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 80)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_84
			{
				[FixedBuffer(typeof(byte), 84)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_84.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 84)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_88
			{
				[FixedBuffer(typeof(byte), 88)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_88.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 88)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_92
			{
				[FixedBuffer(typeof(byte), 92)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_92.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 92)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_96
			{
				[FixedBuffer(typeof(byte), 96)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_96.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 96)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_100
			{
				[FixedBuffer(typeof(byte), 100)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_100.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 100)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_104
			{
				[FixedBuffer(typeof(byte), 104)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_104.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 104)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_108
			{
				[FixedBuffer(typeof(byte), 108)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_108.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 108)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_112
			{
				[FixedBuffer(typeof(byte), 112)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_112.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 112)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_116
			{
				[FixedBuffer(typeof(byte), 116)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_116.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 116)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_120
			{
				[FixedBuffer(typeof(byte), 120)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_120.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 120)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_124
			{
				[FixedBuffer(typeof(byte), 124)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_124.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 124)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_128
			{
				[FixedBuffer(typeof(byte), 128)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_128.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 128)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_132
			{
				[FixedBuffer(typeof(byte), 132)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_132.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 132)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_136
			{
				[FixedBuffer(typeof(byte), 136)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_136.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 136)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_140
			{
				[FixedBuffer(typeof(byte), 140)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_140.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 140)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_144
			{
				[FixedBuffer(typeof(byte), 144)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_144.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 144)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_148
			{
				[FixedBuffer(typeof(byte), 148)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_148.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 148)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}

			public struct SIZER_152
			{
				[FixedBuffer(typeof(byte), 152)]
				public MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SIZER_152.<data>e__FixedBuffer data;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 152)]
				public struct <data>e__FixedBuffer
				{
					public byte FixedElementField;
				}
			}
		}

		public struct VertexAndTriangleProcessorNativeArray : MB3_MeshCombinerSingle.IVertexAndTriangleProcessor, IDisposable
		{
			public MB_MeshVertexChannelFlags channels { readonly get; private set; }

			public void Dispose()
			{
				if (this._disposed)
				{
					return;
				}
				this._isInitialized = false;
				this.channels = MB_MeshVertexChannelFlags.none;
				if (this.dataArrayAllocated)
				{
					this.dataArray.Dispose();
					this.dataArrayAllocated = false;
				}
				if (this.verticiesModified.IsCreated)
				{
					this.verticiesModified.Dispose();
				}
				this.submeshTris = null;
				this._disposed = true;
			}

			public bool IsInitialized()
			{
				return this._isInitialized;
			}

			public bool IsDisposed()
			{
				return this._disposed;
			}

			public void Init(MB3_MeshCombinerSingle combiner, MB_MeshVertexChannelFlags newChannels, int vertexCount, int[] newSubmeshTrisSize, int uvChannelWithExtraParameter, MB3_MeshCombinerSingle.IMeshChannelsCacheTaggingInterface meshChannelsCache, bool loadDataFromCombinedMesh, MB2_LogLevel logLevel)
			{
				this.channels = newChannels;
				this.LOG_LEVEL = logLevel;
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				if ((this.channels & MB_MeshVertexChannelFlags.vertex) == MB_MeshVertexChannelFlags.vertex)
				{
					num++;
				}
				if ((this.channels & MB_MeshVertexChannelFlags.normal) == MB_MeshVertexChannelFlags.normal)
				{
					num++;
				}
				if ((this.channels & MB_MeshVertexChannelFlags.tangent) == MB_MeshVertexChannelFlags.tangent)
				{
					num++;
				}
				if ((this.channels & MB_MeshVertexChannelFlags.colors) == MB_MeshVertexChannelFlags.colors)
				{
					num2++;
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv0) == MB_MeshVertexChannelFlags.uv0)
				{
					num2++;
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv2) == MB_MeshVertexChannelFlags.uv2)
				{
					num2++;
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv3) == MB_MeshVertexChannelFlags.uv3)
				{
					num2++;
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv4) == MB_MeshVertexChannelFlags.uv4)
				{
					num2++;
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv5) == MB_MeshVertexChannelFlags.uv5)
				{
					num2++;
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv6) == MB_MeshVertexChannelFlags.uv6)
				{
					num2++;
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv7) == MB_MeshVertexChannelFlags.uv7)
				{
					num2++;
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv8) == MB_MeshVertexChannelFlags.uv8)
				{
					num2++;
				}
				if ((this.channels & MB_MeshVertexChannelFlags.blendWeight) == MB_MeshVertexChannelFlags.blendWeight)
				{
					num3++;
				}
				if ((this.channels & MB_MeshVertexChannelFlags.blendIndices) == MB_MeshVertexChannelFlags.blendIndices)
				{
					num3++;
				}
				this.vertexAttributes = new VertexAttributeDescriptor[num + num2 + num3];
				MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.Init(this.channels, this.vertexAttributes, ref this, vertexCount, newSubmeshTrisSize, uvChannelWithExtraParameter);
				if (loadDataFromCombinedMesh)
				{
					this.submeshTris = combiner.submeshTris;
					MB3_MeshCombinerSingle.VertexAndTriangleProcessorNativeArray vertexAndTriangleProcessorNativeArray = default(MB3_MeshCombinerSingle.VertexAndTriangleProcessorNativeArray);
					vertexAndTriangleProcessorNativeArray.InitFromMeshCombiner(combiner, this.channels, -1);
					if (vertexAndTriangleProcessorNativeArray.bufferStride_0 > 0)
					{
						MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.NativeSliceCopyFrom(this.rawSliceVertexStream_0, this.rawSliceSizerType_0, vertexAndTriangleProcessorNativeArray.rawSliceVertexStream_0, vertexAndTriangleProcessorNativeArray.rawSliceSizerType_0);
					}
					if (vertexAndTriangleProcessorNativeArray.bufferStride_1 > 0)
					{
						MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.NativeSliceCopyFrom(this.rawSliceVertexStream_1, this.rawSliceSizerType_1, vertexAndTriangleProcessorNativeArray.rawSliceVertexStream_1, vertexAndTriangleProcessorNativeArray.rawSliceSizerType_1);
					}
					if (vertexAndTriangleProcessorNativeArray.data.indexFormat == IndexFormat.UInt16)
					{
						NativeArray<ushort> indexData = vertexAndTriangleProcessorNativeArray.data.GetIndexData<ushort>();
						this.data.SetIndexBufferParams(indexData.Length, IndexFormat.UInt16);
						this.data.GetIndexData<ushort>().CopyFrom(indexData);
						this.data.subMeshCount = vertexAndTriangleProcessorNativeArray.data.subMeshCount;
						for (int i = 0; i < this.data.subMeshCount; i++)
						{
							SubMeshDescriptor subMesh = vertexAndTriangleProcessorNativeArray.data.GetSubMesh(i);
							this.data.SetSubMesh(i, subMesh, MeshUpdateFlags.Default);
						}
					}
					else
					{
						NativeArray<uint> indexData2 = vertexAndTriangleProcessorNativeArray.data.GetIndexData<uint>();
						this.data.SetIndexBufferParams(indexData2.Length, IndexFormat.UInt32);
						this.data.GetIndexData<uint>().CopyFrom(indexData2);
						this.data.subMeshCount = vertexAndTriangleProcessorNativeArray.data.subMeshCount;
						for (int j = 0; j < this.data.subMeshCount; j++)
						{
							SubMeshDescriptor subMesh2 = vertexAndTriangleProcessorNativeArray.data.GetSubMesh(j);
							this.data.SetSubMesh(j, subMesh2, MeshUpdateFlags.Default);
						}
					}
					vertexAndTriangleProcessorNativeArray.Dispose();
				}
				else
				{
					this.submeshTris = new MB3_MeshCombinerSingle.SerializableIntArray[newSubmeshTrisSize.Length];
					for (int k = 0; k < newSubmeshTrisSize.Length; k++)
					{
						this.submeshTris[k] = new MB3_MeshCombinerSingle.SerializableIntArray(newSubmeshTrisSize[k]);
					}
				}
				this._isInitialized = true;
			}

			public void InitShowHide(MB3_MeshCombinerSingle combiner)
			{
				this.channels = combiner.channelsLastBake;
				this.submeshTris = combiner.submeshTris;
				this._isInitialized = true;
			}

			public void InitFromMeshCombiner(MB3_MeshCombinerSingle combiner, MB_MeshVertexChannelFlags newChannels, int uvChannelWithExtraParameter)
			{
				if (combiner.channelsLastBake != newChannels)
				{
					if (combiner.channelsLastBake == MB_MeshVertexChannelFlags.none && combiner.verts.Length != 0)
					{
						combiner.channelsLastBake = newChannels;
					}
					else
					{
						Debug.LogError("Shouldn't change channels between bakes. \n" + combiner.channelsLastBake.ToString() + " \n" + newChannels.ToString());
					}
				}
				this.channels = combiner.channelsLastBake;
				this.dataArray = Mesh.AcquireReadOnlyMeshData(combiner._mesh);
				this.dataArrayAllocated = true;
				this.data = this.dataArray[0];
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					string text = "Vertex attributes in combined mesh: ";
					for (int i = 0; i < combiner._mesh.vertexAttributeCount; i++)
					{
						VertexAttributeDescriptor vertexAttribute = combiner._mesh.GetVertexAttribute(i);
						string[] array = new string[5];
						array[0] = text;
						array[1] = "\n    ";
						array[2] = i.ToString();
						array[3] = "  VertexAttribute: ";
						int num = 4;
						VertexAttributeDescriptor vertexAttributeDescriptor = vertexAttribute;
						array[num] = vertexAttributeDescriptor.ToString();
						text = string.Concat(array);
					}
					Debug.Log(text);
				}
				int strideVertexData;
				int strideUVdata;
				MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.CalcStride(this.channels, uvChannelWithExtraParameter, out strideVertexData, out strideUVdata);
				MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.SetupNativeSlices(ref this, strideVertexData, strideUVdata, uvChannelWithExtraParameter);
				if (combiner.bufferDataFromPrevious.meshVerticiesWereShifted)
				{
					this.verticiesModified = new NativeArray<Vector3>(this.verticies.Length, Allocator.Temp, NativeArrayOptions.ClearMemory);
					Vector3 meshVerticesShift = combiner.bufferDataFromPrevious.meshVerticesShift;
					for (int j = 0; j < this.verticies.Length; j++)
					{
						this.verticiesModified[j] = this.verticies[j] + meshVerticesShift;
					}
					this.verticies = this.verticiesModified.Slice<Vector3>();
				}
				this.submeshTris = combiner.submeshTris;
				this._isInitialized = true;
			}

			public void ApplyDataBufferToMesh(Mesh m)
			{
				this.data.subMeshCount = 1;
				this.data.SetSubMesh(0, new SubMeshDescriptor(0, this.triangleBuffer.Length, MeshTopology.Triangles), MeshUpdateFlags.Default);
				Mesh.ApplyAndDisposeWritableMeshData(this.dataArray, m, MeshUpdateFlags.Default);
				this.dataArrayAllocated = false;
				m.RecalculateBounds();
			}

			public int GetVertexCount()
			{
				return this.verticies.Length;
			}

			public int GetSubmeshCount()
			{
				return this.submeshTris.Length;
			}

			public void TransferOwnershipOfSerializableBuffersToCombiner(MB3_MeshCombinerSingle c, MB_MeshVertexChannelFlags channelsToTransfer, MB3_MeshCombinerSingle.BufferDataFromPreviousBake serializableBufferData)
			{
				c.channelsLastBake = this.channels;
				c.bufferDataFromPrevious = serializableBufferData;
				c.submeshTris = this.submeshTris;
				this.submeshTris = null;
				this._isInitialized = false;
			}

			public void CopyArraysFromPreviousBakeBuffersToNewBuffers(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, ref MB3_MeshCombinerSingle.IVertexAndTriangleProcessor iOldBuffers, int destStartVertIdx, int triangleIdxAdjustment, int[] targSubmeshTidx, MB2_LogLevel LOG_LEVEL)
			{
				MB3_MeshCombinerSingle.VertexAndTriangleProcessorNativeArray vertexAndTriangleProcessorNativeArray = (MB3_MeshCombinerSingle.VertexAndTriangleProcessorNativeArray)iOldBuffers;
				int vertIdx = dgo.vertIdx;
				int numVerts = dgo.numVerts;
				if ((this.channels & MB_MeshVertexChannelFlags.vertex) == MB_MeshVertexChannelFlags.vertex)
				{
					MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.NativeSliceCopy<Vector3>(vertexAndTriangleProcessorNativeArray.verticies, vertIdx, this.verticies, destStartVertIdx, numVerts);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.normal) == MB_MeshVertexChannelFlags.normal)
				{
					MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.NativeSliceCopy<Vector3>(vertexAndTriangleProcessorNativeArray.normals, vertIdx, this.normals, destStartVertIdx, numVerts);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.tangent) == MB_MeshVertexChannelFlags.tangent)
				{
					MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.NativeSliceCopy<Vector4>(vertexAndTriangleProcessorNativeArray.tangents, vertIdx, this.tangents, destStartVertIdx, numVerts);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv0) == MB_MeshVertexChannelFlags.uv0)
				{
					MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.NativeSliceCopy<Vector2>(vertexAndTriangleProcessorNativeArray.uv0s, vertIdx, this.uv0s, destStartVertIdx, numVerts);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.nuvsSliceIdx) == MB_MeshVertexChannelFlags.nuvsSliceIdx)
				{
					MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.NativeSliceCopy<float>(vertexAndTriangleProcessorNativeArray.uvsSliceIdx, vertIdx, this.uvsSliceIdx, destStartVertIdx, numVerts);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv2) == MB_MeshVertexChannelFlags.uv2)
				{
					MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.NativeSliceCopy<Vector2>(vertexAndTriangleProcessorNativeArray.uv2s, vertIdx, this.uv2s, destStartVertIdx, numVerts);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv3) == MB_MeshVertexChannelFlags.uv3)
				{
					MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.NativeSliceCopy<Vector2>(vertexAndTriangleProcessorNativeArray.uv3s, vertIdx, this.uv3s, destStartVertIdx, numVerts);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv4) == MB_MeshVertexChannelFlags.uv4)
				{
					MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.NativeSliceCopy<Vector2>(vertexAndTriangleProcessorNativeArray.uv4s, vertIdx, this.uv4s, destStartVertIdx, numVerts);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv5) == MB_MeshVertexChannelFlags.uv5)
				{
					MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.NativeSliceCopy<Vector2>(vertexAndTriangleProcessorNativeArray.uv5s, vertIdx, this.uv5s, destStartVertIdx, numVerts);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv6) == MB_MeshVertexChannelFlags.uv6)
				{
					MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.NativeSliceCopy<Vector2>(vertexAndTriangleProcessorNativeArray.uv6s, vertIdx, this.uv6s, destStartVertIdx, numVerts);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv7) == MB_MeshVertexChannelFlags.uv7)
				{
					MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.NativeSliceCopy<Vector2>(vertexAndTriangleProcessorNativeArray.uv7s, vertIdx, this.uv7s, destStartVertIdx, numVerts);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv8) == MB_MeshVertexChannelFlags.uv8)
				{
					MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.NativeSliceCopy<Vector2>(vertexAndTriangleProcessorNativeArray.uv8s, vertIdx, this.uv8s, destStartVertIdx, numVerts);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.colors) == MB_MeshVertexChannelFlags.colors)
				{
					MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.NativeSliceCopy<Color>(vertexAndTriangleProcessorNativeArray.colors, vertIdx, this.colors, destStartVertIdx, numVerts);
				}
				for (int i = 0; i < this.submeshTris.Length; i++)
				{
					int[] array = vertexAndTriangleProcessorNativeArray.submeshTris[i].data;
					int num = dgo.submeshTriIdxs[i];
					int num2 = dgo.submeshNumTris[i];
					if (LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug(string.Concat(new string[]
						{
							"    Adjusting submesh triangles submesh:",
							i.ToString(),
							" startIdx:",
							num.ToString(),
							" num:",
							num2.ToString(),
							" nsubmeshTris:",
							this.submeshTris.Length.ToString(),
							" targSubmeshTidx:",
							targSubmeshTidx.Length.ToString()
						}), new object[]
						{
							LOG_LEVEL
						});
					}
					for (int j = num; j < num + num2; j++)
					{
						array[j] -= triangleIdxAdjustment;
					}
					Array.Copy(array, num, this.submeshTris[i].data, targSubmeshTidx[i], num2);
				}
			}

			public void CopyFromDGOMeshToBuffers(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, int destStartVertsIdx, MB_MeshVertexChannelFlags channelsToUpdate, bool updateTris, bool updateBWdata, MB_IMeshBakerSettings settings, MB_IMeshCombinerSingle_BoneProcessor boneProcessor, int[] targSubmeshTidx, MB2_TextureBakeResults textureBakeResults, MB3_MeshCombinerSingle.UVAdjuster_Atlas uvAdjuster, MB2_LogLevel LOG_LEVEL, MB3_MeshCombinerSingle.IMeshChannelsCacheTaggingInterface meshChannelCacheParam)
			{
				MB3_MeshCombinerSingle.MeshChannelsCache_NativeArray meshChannelsCache_NativeArray = (MB3_MeshCombinerSingle.MeshChannelsCache_NativeArray)meshChannelCacheParam;
				bool flag = (this.channels & MB_MeshVertexChannelFlags.vertex) == MB_MeshVertexChannelFlags.vertex && (channelsToUpdate & MB_MeshVertexChannelFlags.vertex) == MB_MeshVertexChannelFlags.vertex;
				bool flag2 = (this.channels & MB_MeshVertexChannelFlags.normal) == MB_MeshVertexChannelFlags.normal && (channelsToUpdate & MB_MeshVertexChannelFlags.normal) == MB_MeshVertexChannelFlags.normal;
				bool flag3 = (this.channels & MB_MeshVertexChannelFlags.tangent) == MB_MeshVertexChannelFlags.tangent && (channelsToUpdate & MB_MeshVertexChannelFlags.tangent) == MB_MeshVertexChannelFlags.tangent;
				if (flag || flag2 || flag3)
				{
					NativeArray<Vector3> nativeArray = default(NativeArray<Vector3>);
					NativeArray<Vector3> nativeArray2 = default(NativeArray<Vector3>);
					NativeArray<Vector4> nativeArray3 = default(NativeArray<Vector4>);
					if (flag)
					{
						nativeArray = meshChannelsCache_NativeArray.GetVerticiesAsNativeArray(dgo._mesh);
					}
					if (flag2)
					{
						nativeArray2 = meshChannelsCache_NativeArray.GetNormalsAsNativeArray(dgo._mesh);
					}
					if (flag3)
					{
						nativeArray3 = meshChannelsCache_NativeArray.GetTangentsAsNativeArray(dgo._mesh);
					}
					if (settings.renderType != MB_RenderType.skinnedMeshRenderer)
					{
						this._LocalToWorld(dgo.gameObject.transform, flag2, flag3, destStartVertsIdx, nativeArray, nativeArray2, nativeArray3, this.verticies, this.normals, this.tangents);
					}
					else
					{
						boneProcessor.CopyVertsNormsTansToBuffers(dgo, settings, destStartVertsIdx, nativeArray2, nativeArray3, nativeArray, this.normals, this.tangents, this.verticies);
					}
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv0) == MB_MeshVertexChannelFlags.uv0 && (channelsToUpdate & MB_MeshVertexChannelFlags.uv0) == MB_MeshVertexChannelFlags.uv0)
				{
					this._copyAndAdjustUVsFromMesh(textureBakeResults, dgo, dgo._mesh, 0, destStartVertsIdx, this.uv0s, this.uvsSliceIdx, meshChannelsCache_NativeArray, LOG_LEVEL, textureBakeResults);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv2) == MB_MeshVertexChannelFlags.uv2 && (channelsToUpdate & MB_MeshVertexChannelFlags.uv2) == MB_MeshVertexChannelFlags.uv2)
				{
					this._CopyAndAdjustUV2FromMesh(settings, meshChannelsCache_NativeArray, dgo, destStartVertsIdx, LOG_LEVEL);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv3) == MB_MeshVertexChannelFlags.uv3 && (channelsToUpdate & MB_MeshVertexChannelFlags.uv3) == MB_MeshVertexChannelFlags.uv3)
				{
					MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.NativeSliceCopyTo<Vector2>(meshChannelsCache_NativeArray.GetUVChannelAsNativeArray(3, dgo._mesh), this.uv3s, destStartVertsIdx);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv4) == MB_MeshVertexChannelFlags.uv4 && (channelsToUpdate & MB_MeshVertexChannelFlags.uv4) == MB_MeshVertexChannelFlags.uv4)
				{
					MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.NativeSliceCopyTo<Vector2>(meshChannelsCache_NativeArray.GetUVChannelAsNativeArray(4, dgo._mesh), this.uv4s, destStartVertsIdx);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv5) == MB_MeshVertexChannelFlags.uv5 && (channelsToUpdate & MB_MeshVertexChannelFlags.uv5) == MB_MeshVertexChannelFlags.uv5)
				{
					MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.NativeSliceCopyTo<Vector2>(meshChannelsCache_NativeArray.GetUVChannelAsNativeArray(5, dgo._mesh), this.uv5s, destStartVertsIdx);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv6) == MB_MeshVertexChannelFlags.uv6 && (channelsToUpdate & MB_MeshVertexChannelFlags.uv6) == MB_MeshVertexChannelFlags.uv6)
				{
					MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.NativeSliceCopyTo<Vector2>(meshChannelsCache_NativeArray.GetUVChannelAsNativeArray(6, dgo._mesh), this.uv6s, destStartVertsIdx);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv7) == MB_MeshVertexChannelFlags.uv7 && (channelsToUpdate & MB_MeshVertexChannelFlags.uv7) == MB_MeshVertexChannelFlags.uv7)
				{
					MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.NativeSliceCopyTo<Vector2>(meshChannelsCache_NativeArray.GetUVChannelAsNativeArray(7, dgo._mesh), this.uv7s, destStartVertsIdx);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.uv8) == MB_MeshVertexChannelFlags.uv8 && (channelsToUpdate & MB_MeshVertexChannelFlags.uv8) == MB_MeshVertexChannelFlags.uv8)
				{
					MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.NativeSliceCopyTo<Vector2>(meshChannelsCache_NativeArray.GetUVChannelAsNativeArray(8, dgo._mesh), this.uv8s, destStartVertsIdx);
				}
				if ((this.channels & MB_MeshVertexChannelFlags.colors) == MB_MeshVertexChannelFlags.colors && (channelsToUpdate & MB_MeshVertexChannelFlags.colors) == MB_MeshVertexChannelFlags.colors)
				{
					MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.NativeSliceCopyTo<Color>(meshChannelsCache_NativeArray.GetColorsAsNativeArray(dgo._mesh), this.colors, destStartVertsIdx);
				}
				if (updateBWdata)
				{
					boneProcessor.UpdateGameObjects_UpdateBWIndexes(dgo);
				}
				if (updateTris)
				{
					for (int i = 0; i < targSubmeshTidx.Length; i++)
					{
						dgo.submeshTriIdxs[i] = targSubmeshTidx[i];
					}
					for (int j = 0; j < dgo._tmpSubmeshTris.Length; j++)
					{
						int[] array = dgo._tmpSubmeshTris[j].data;
						if (destStartVertsIdx != 0)
						{
							for (int k = 0; k < array.Length; k++)
							{
								array[k] += destStartVertsIdx;
							}
						}
						if (dgo.invertTriangles)
						{
							for (int l = 0; l < array.Length; l += 3)
							{
								int num = array[l];
								array[l] = array[l + 1];
								array[l + 1] = num;
							}
						}
						int num2 = dgo.targetSubmeshIdxs[j];
						array.CopyTo(this.submeshTris[num2].data, targSubmeshTidx[num2]);
						dgo.submeshNumTris[num2] += array.Length;
						targSubmeshTidx[num2] += array.Length;
					}
				}
			}

			public void AssignBuffersToMesh(Mesh mesh, MB_IMeshBakerSettings settings, MB2_TextureBakeResults textureBakeResults, MB_MeshVertexChannelFlags channelsToWriteToMesh, bool doWriteTrisToMesh, IAssignToMeshCustomizer assignToMeshCustomizer, List<MB3_MeshCombinerSingle.MB_DynamicGameObject> mbDynamicObjectsInCombinedMesh, out MB3_MeshCombinerSingle.BufferDataFromPreviousBake serializableBufferData, out MB3_MeshCombinerSingle.SerializableIntArray[] submeshTrisToUse, out int numNonZeroLengthSubmeshes)
			{
				if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.vertex) == MB_MeshVertexChannelFlags.vertex)
				{
					this.AdjustVertsToWriteAccordingToPivotPositionIfNecessary(settings.pivotLocationType, settings.renderType, settings.clearBuffersAfterBake, settings.pivotLocation, out serializableBufferData);
				}
				else
				{
					serializableBufferData.numVertsBaked = this.data.vertexCount;
					serializableBufferData.meshVerticesShift = Vector3.zero;
					serializableBufferData.meshVerticiesWereShifted = false;
				}
				if (assignToMeshCustomizer != null)
				{
					IAssignToMeshCustomizer_NativeArrays assignToMeshCustomizer_NativeArrays = (IAssignToMeshCustomizer_NativeArrays)assignToMeshCustomizer;
					if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.uv0) == MB_MeshVertexChannelFlags.uv0)
					{
						assignToMeshCustomizer_NativeArrays.meshAssign_UV(0, settings, textureBakeResults, this.uvsWithExtraIndex, this.uvsSliceIdx);
					}
					if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.uv2) == MB_MeshVertexChannelFlags.uv2)
					{
						assignToMeshCustomizer_NativeArrays.meshAssign_UV(1, settings, textureBakeResults, this.uvsWithExtraIndex, this.uvsSliceIdx);
					}
					if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.uv3) == MB_MeshVertexChannelFlags.uv3)
					{
						assignToMeshCustomizer_NativeArrays.meshAssign_UV(2, settings, textureBakeResults, this.uvsWithExtraIndex, this.uvsSliceIdx);
					}
					if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.uv4) == MB_MeshVertexChannelFlags.uv4)
					{
						assignToMeshCustomizer_NativeArrays.meshAssign_UV(3, settings, textureBakeResults, this.uvsWithExtraIndex, this.uvsSliceIdx);
					}
					if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.uv5) == MB_MeshVertexChannelFlags.uv5)
					{
						assignToMeshCustomizer_NativeArrays.meshAssign_UV(4, settings, textureBakeResults, this.uvsWithExtraIndex, this.uvsSliceIdx);
					}
					if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.uv6) == MB_MeshVertexChannelFlags.uv6)
					{
						assignToMeshCustomizer_NativeArrays.meshAssign_UV(5, settings, textureBakeResults, this.uvsWithExtraIndex, this.uvsSliceIdx);
					}
					if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.uv7) == MB_MeshVertexChannelFlags.uv7)
					{
						assignToMeshCustomizer_NativeArrays.meshAssign_UV(6, settings, textureBakeResults, this.uvsWithExtraIndex, this.uvsSliceIdx);
					}
					if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.uv8) == MB_MeshVertexChannelFlags.uv8)
					{
						assignToMeshCustomizer_NativeArrays.meshAssign_UV(7, settings, textureBakeResults, this.uvsWithExtraIndex, this.uvsSliceIdx);
					}
					if ((channelsToWriteToMesh & MB_MeshVertexChannelFlags.colors) == MB_MeshVertexChannelFlags.colors)
					{
						assignToMeshCustomizer_NativeArrays.meshAssign_colors(settings, textureBakeResults, this.colors, this.uvsSliceIdx);
					}
				}
				else if (textureBakeResults.resultType == MB2_TextureBakeResults.ResultType.textureArray)
				{
					Debug.LogError("No AssignToMeshCustomizer was assigned.");
				}
				if (doWriteTrisToMesh)
				{
					this.AssignTriangleDataForSubmeshes(mesh, mbDynamicObjectsInCombinedMesh, ref serializableBufferData, out submeshTrisToUse, out numNonZeroLengthSubmeshes);
				}
				else
				{
					submeshTrisToUse = null;
					numNonZeroLengthSubmeshes = -1;
				}
				Mesh.ApplyAndDisposeWritableMeshData(this.dataArray, mesh, MeshUpdateFlags.Default);
				this.dataArrayAllocated = false;
			}

			public void AssignTriangleDataForSubmeshes(Mesh mmesh, List<MB3_MeshCombinerSingle.MB_DynamicGameObject> mbDynamicObjectsInCombinedMesh, ref MB3_MeshCombinerSingle.BufferDataFromPreviousBake serializableBufferData, out MB3_MeshCombinerSingle.SerializableIntArray[] submeshTrisToUse, out int numNonZeroLengthSubmeshes)
			{
				submeshTrisToUse = this.GetSubmeshTrisWithShowHideApplied(mbDynamicObjectsInCombinedMesh);
				int num;
				numNonZeroLengthSubmeshes = MB3_MeshCombinerSingle.VertexAndTriangleProcessorNativeArray._NumNonZeroLengthSubmeshTris(submeshTrisToUse, out num);
				IndexFormat indexFormat;
				if (num > 65535)
				{
					indexFormat = IndexFormat.UInt32;
				}
				else
				{
					indexFormat = IndexFormat.UInt16;
				}
				this.data.SetIndexBufferParams(num, indexFormat);
				if (indexFormat == IndexFormat.UInt16)
				{
					int num2 = 0;
					int num3 = 0;
					NativeArray<ushort> indexData = this.data.GetIndexData<ushort>();
					for (int i = 0; i < submeshTrisToUse.Length; i++)
					{
						if (submeshTrisToUse[i].data.Length != 0)
						{
							MB3_MeshCombinerSingle.SerializableIntArray serializableIntArray = submeshTrisToUse[i];
							for (int j = 0; j < serializableIntArray.data.Length; j++)
							{
								indexData[num3 + j] = (ushort)serializableIntArray.data[j];
							}
							num2++;
							num3 += serializableIntArray.data.Length;
						}
					}
				}
				else
				{
					int num4 = 0;
					int num5 = 0;
					NativeArray<uint> indexData2 = this.data.GetIndexData<uint>();
					for (int k = 0; k < submeshTrisToUse.Length; k++)
					{
						if (submeshTrisToUse[k].data.Length != 0)
						{
							MB3_MeshCombinerSingle.SerializableIntArray serializableIntArray2 = submeshTrisToUse[k];
							for (int l = 0; l < serializableIntArray2.data.Length; l++)
							{
								indexData2[num5 + l] = (uint)serializableIntArray2.data[l];
							}
							num4++;
							num5 += serializableIntArray2.data.Length;
						}
					}
				}
				this.data.subMeshCount = numNonZeroLengthSubmeshes;
				int num6 = 0;
				int num7 = 0;
				for (int m = 0; m < submeshTrisToUse.Length; m++)
				{
					if (submeshTrisToUse[m].data.Length != 0)
					{
						MB3_MeshCombinerSingle.SerializableIntArray serializableIntArray3 = submeshTrisToUse[m];
						SubMeshDescriptor desc = new SubMeshDescriptor(num7, serializableIntArray3.data.Length, MeshTopology.Triangles);
						this.data.SetSubMesh(num6, desc, MeshUpdateFlags.Default);
						num6++;
						num7 += serializableIntArray3.data.Length;
					}
				}
			}

			public void AssignTriangleDataForSubmeshes_ShowHide(Mesh mesh, List<MB3_MeshCombinerSingle.MB_DynamicGameObject> mbDynamicObjectsInCombinedMesh, ref MB3_MeshCombinerSingle.BufferDataFromPreviousBake serializableBufferData, out MB3_MeshCombinerSingle.SerializableIntArray[] submeshTrisToUse, out int numNonZeroLengthSubmeshes)
			{
				submeshTrisToUse = this.GetSubmeshTrisWithShowHideApplied(mbDynamicObjectsInCombinedMesh);
				int num;
				numNonZeroLengthSubmeshes = MB3_MeshCombinerSingle.VertexAndTriangleProcessorNativeArray._NumNonZeroLengthSubmeshTris(submeshTrisToUse, out num);
				IndexFormat indexFormat;
				if (num > 65535)
				{
					indexFormat = IndexFormat.UInt32;
				}
				else
				{
					indexFormat = IndexFormat.UInt16;
				}
				mesh.subMeshCount = 1;
				mesh.SetIndexBufferParams(num, indexFormat);
				if (indexFormat == IndexFormat.UInt16)
				{
					int num2 = 0;
					int num3 = 0;
					NativeArray<ushort> nativeArray = new NativeArray<ushort>(num, Allocator.Temp, NativeArrayOptions.ClearMemory);
					for (int i = 0; i < submeshTrisToUse.Length; i++)
					{
						if (submeshTrisToUse[i].data.Length != 0)
						{
							MB3_MeshCombinerSingle.SerializableIntArray serializableIntArray = submeshTrisToUse[i];
							for (int j = 0; j < serializableIntArray.data.Length; j++)
							{
								nativeArray[num3 + j] = (ushort)serializableIntArray.data[j];
							}
							num2++;
							num3 += serializableIntArray.data.Length;
						}
					}
					mesh.SetIndexBufferData<ushort>(nativeArray, 0, 0, nativeArray.Length, MeshUpdateFlags.DontValidateIndices);
					if (nativeArray.IsCreated)
					{
						nativeArray.Dispose();
					}
				}
				else
				{
					int num4 = 0;
					int num5 = 0;
					NativeArray<uint> nativeArray2 = new NativeArray<uint>(num, Allocator.Temp, NativeArrayOptions.ClearMemory);
					for (int k = 0; k < submeshTrisToUse.Length; k++)
					{
						if (submeshTrisToUse[k].data.Length != 0)
						{
							MB3_MeshCombinerSingle.SerializableIntArray serializableIntArray2 = submeshTrisToUse[k];
							for (int l = 0; l < serializableIntArray2.data.Length; l++)
							{
								nativeArray2[num5 + l] = (uint)serializableIntArray2.data[l];
							}
							num4++;
							num5 += serializableIntArray2.data.Length;
						}
					}
					mesh.SetIndexBufferData<uint>(nativeArray2, 0, 0, nativeArray2.Length, MeshUpdateFlags.DontValidateIndices);
					if (nativeArray2.IsCreated)
					{
						nativeArray2.Dispose();
					}
				}
				mesh.subMeshCount = numNonZeroLengthSubmeshes;
				int num6 = 0;
				int num7 = 0;
				for (int m = 0; m < submeshTrisToUse.Length; m++)
				{
					if (submeshTrisToUse[m].data.Length != 0)
					{
						MB3_MeshCombinerSingle.SerializableIntArray serializableIntArray3 = submeshTrisToUse[m];
						SubMeshDescriptor desc = new SubMeshDescriptor(num7, serializableIntArray3.data.Length, MeshTopology.Triangles);
						mesh.SetSubMesh(num6, desc, MeshUpdateFlags.Default);
						num6++;
						num7 += serializableIntArray3.data.Length;
					}
				}
			}

			private void AdjustVertsToWriteAccordingToPivotPositionIfNecessary(MB_MeshPivotLocation pivotLocationType, MB_RenderType renderType, bool clearBuffersAfterBake, Vector3 pivotLocation_wld, out MB3_MeshCombinerSingle.BufferDataFromPreviousBake serializableBufferData)
			{
				serializableBufferData.numVertsBaked = this.data.vertexCount;
				if (this.verticies.Length <= 0)
				{
					serializableBufferData.meshVerticesShift = Vector3.zero;
					serializableBufferData.meshVerticiesWereShifted = false;
					return;
				}
				if (renderType == MB_RenderType.skinnedMeshRenderer)
				{
					serializableBufferData.meshVerticesShift = Vector3.zero;
					serializableBufferData.meshVerticiesWereShifted = false;
					return;
				}
				if (pivotLocationType == MB_MeshPivotLocation.worldOrigin)
				{
					serializableBufferData.meshVerticesShift = Vector3.zero;
					serializableBufferData.meshVerticiesWereShifted = false;
					return;
				}
				if (pivotLocationType == MB_MeshPivotLocation.boundsCenter || pivotLocationType == MB_MeshPivotLocation.customLocation)
				{
					Vector3 vector4;
					if (pivotLocationType == MB_MeshPivotLocation.boundsCenter)
					{
						Vector3 vector = this.verticies[0];
						Vector3 vector2 = this.verticies[0];
						for (int i = 1; i < this.verticies.Length; i++)
						{
							Vector3 vector3 = this.verticies[i];
							if (vector.x < vector3.x)
							{
								vector.x = vector3.x;
							}
							if (vector.y < vector3.y)
							{
								vector.y = vector3.y;
							}
							if (vector.z < vector3.z)
							{
								vector.z = vector3.z;
							}
							if (vector2.x > vector3.x)
							{
								vector2.x = vector3.x;
							}
							if (vector2.y > vector3.y)
							{
								vector2.y = vector3.y;
							}
							if (vector2.z > vector3.z)
							{
								vector2.z = vector3.z;
							}
						}
						vector4 = (vector + vector2) * 0.5f;
					}
					else
					{
						vector4 = pivotLocation_wld;
					}
					for (int j = 0; j < this.verticies.Length; j++)
					{
						this.verticies[j] = this.verticies[j] - vector4;
					}
					serializableBufferData.meshVerticesShift = vector4;
					serializableBufferData.meshVerticiesWereShifted = true;
					return;
				}
				Debug.LogError("Unsupported Pivot Location Type: " + pivotLocationType.ToString());
				serializableBufferData.meshVerticesShift = Vector3.zero;
				serializableBufferData.meshVerticiesWereShifted = false;
			}

			private static int _NumNonZeroLengthSubmeshTris(MB3_MeshCombinerSingle.SerializableIntArray[] subTris, out int numIndexes)
			{
				numIndexes = 0;
				int num = 0;
				for (int i = 0; i < subTris.Length; i++)
				{
					if (subTris[i].data.Length != 0)
					{
						num++;
						numIndexes += subTris[i].data.Length;
					}
				}
				return num;
			}

			private void _copyAndAdjustUVsFromMesh(MB2_TextureBakeResults tbr, MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, Mesh mesh, int uvChannel, int vertsIdx, NativeSlice<Vector2> uvsOut, NativeSlice<float> uvsSliceIdx, MB3_MeshCombinerSingle.MeshChannelsCache_NativeArray meshChannelsCache, MB2_LogLevel LOG_LEVEL, MB2_TextureBakeResults textureBakeResults)
			{
				NativeArray<Vector2> uvchannelAsNativeArray = meshChannelsCache.GetUVChannelAsNativeArray(uvChannel, mesh);
				int[] array = new int[uvchannelAsNativeArray.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = -1;
				}
				bool flag = false;
				bool flag2 = tbr.resultType == MB2_TextureBakeResults.ResultType.textureArray;
				for (int j = 0; j < dgo.targetSubmeshIdxs.Length; j++)
				{
					int[] triangles;
					if (dgo._tmpSubmeshTris != null)
					{
						triangles = dgo._tmpSubmeshTris[j].data;
					}
					else
					{
						triangles = mesh.GetTriangles(j);
					}
					float value = (float)dgo.textureArraySliceIdx[j];
					int idxInSrcMats = dgo.targetSubmeshIdxs[j];
					if (LOG_LEVEL >= MB2_LogLevel.trace)
					{
						Debug.Log(string.Format("Build UV transform for mesh {0} submesh {1} encapsulatingRect {2}", dgo.name, j, dgo.encapsulatingRect[j]));
					}
					Rect rect = MB3_TextureCombinerMerging.BuildTransformMeshUV2AtlasRect(textureBakeResults.GetConsiderMeshUVs(idxInSrcMats, dgo.sourceSharedMaterials[j]), dgo.uvRects[j], (dgo.obUVRects == null || dgo.obUVRects.Length == 0) ? new Rect(0f, 0f, 1f, 1f) : dgo.obUVRects[j], dgo.sourceMaterialTiling[j], dgo.encapsulatingRect[j]);
					foreach (int num in triangles)
					{
						if (array[num] == -1)
						{
							array[num] = j;
							Vector2 vector = uvchannelAsNativeArray[num];
							vector.x = rect.x + vector.x * rect.width;
							vector.y = rect.y + vector.y * rect.height;
							int index = vertsIdx + num;
							uvsOut[index] = vector;
							if (flag2)
							{
								uvsSliceIdx[index] = value;
							}
						}
						if (array[num] != j)
						{
							flag = true;
						}
					}
				}
				if (flag && LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning(dgo.name + "has submeshes which share verticies. Adjusted uvs may not map correctly in combined atlas.");
				}
				if (LOG_LEVEL >= MB2_LogLevel.trace)
				{
					Debug.Log(string.Format("_copyAndAdjustUVsFromMesh copied {0} verts", uvchannelAsNativeArray.Length));
				}
			}

			private void _CopyAndAdjustUV2FromMesh(MB_IMeshBakerSettings settings, MB3_MeshCombinerSingle.MeshChannelsCache_NativeArray meshChannelsCache, MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, int vertsIdx, MB2_LogLevel LOG_LEVEL)
			{
				NativeArray<Vector2> array = meshChannelsCache.GetUVChannelAsNativeArray(2, dgo._mesh);
				if (settings.lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping)
				{
					if (array.Length == 0)
					{
						NativeArray<Vector2> uvchannelAsNativeArray = meshChannelsCache.GetUVChannelAsNativeArray(0, dgo._mesh);
						if (uvchannelAsNativeArray.Length > 0)
						{
							array = uvchannelAsNativeArray;
						}
						else
						{
							if (LOG_LEVEL >= MB2_LogLevel.warn)
							{
								string str = "Mesh ";
								Mesh mesh = dgo._mesh;
								Debug.LogWarning(str + ((mesh != null) ? mesh.ToString() : null) + " didn't have uv2s. Generating uv2s.");
							}
							array = meshChannelsCache.GetUv2ModifiedAsNativeArray(dgo._mesh);
						}
					}
					Vector4 lightmapTilingOffset = dgo.lightmapTilingOffset;
					Vector2 vector = new Vector2(lightmapTilingOffset.x, lightmapTilingOffset.y);
					Vector2 a = new Vector2(lightmapTilingOffset.z, lightmapTilingOffset.w);
					for (int i = 0; i < array.Length; i++)
					{
						Vector2 b;
						b.x = vector.x * array[i].x;
						b.y = vector.y * array[i].y;
						this.uv2s[vertsIdx + i] = a + b;
					}
					if (LOG_LEVEL >= MB2_LogLevel.trace)
					{
						Debug.Log("_copyAndAdjustUV2FromMesh copied and modify for preserve current lightmapping " + array.Length.ToString());
						return;
					}
				}
				else
				{
					if (array.Length == 0)
					{
						if (LOG_LEVEL >= MB2_LogLevel.warn)
						{
							string str2 = "Mesh ";
							Mesh mesh2 = dgo._mesh;
							Debug.LogWarning(str2 + ((mesh2 != null) ? mesh2.ToString() : null) + " didn't have uv2s. Generating uv2s.");
						}
						if (settings.lightmapOption == MB2_LightmapOptions.copy_UV2_unchanged_to_separate_rects && array.Length == 0)
						{
							string str3 = "Mesh ";
							Mesh mesh3 = dgo._mesh;
							Debug.LogError(str3 + ((mesh3 != null) ? mesh3.ToString() : null) + " did not have a UV2 channel. Nothing to copy when trying to copy UV2 to separate rects. The combined mesh will not lightmap properly. Try using generate new uv2 layout.");
						}
						array = meshChannelsCache.GetUv2ModifiedAsNativeArray(dgo._mesh);
					}
					MB3_MeshCombinerSingle.MB_MeshCombinerSingle_MeshNativeArrayHelper.NativeSliceCopyTo<Vector2>(array, this.uv2s, vertsIdx);
					if (LOG_LEVEL >= MB2_LogLevel.trace)
					{
						Debug.Log("_copyAndAdjustUV2FromMesh copied without modifying " + array.Length.ToString());
					}
				}
			}

			public void CopyUV2unchangedToSeparateRects(List<MB3_MeshCombinerSingle.MB_DynamicGameObject> mbDynamicObjectsInCombinedMesh, float uv2UnwrappingParamsPackMargin)
			{
				int num = Mathf.CeilToInt(8192f * uv2UnwrappingParamsPackMargin);
				if (num < 1)
				{
					num = 1;
				}
				List<Vector2> list = new List<Vector2>(mbDynamicObjectsInCombinedMesh.Count);
				float[] array = new float[mbDynamicObjectsInCombinedMesh.Count];
				Rect[] array2 = new Rect[mbDynamicObjectsInCombinedMesh.Count];
				float num2 = 0f;
				for (int i = 0; i < mbDynamicObjectsInCombinedMesh.Count; i++)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject = mbDynamicObjectsInCombinedMesh[i];
					float num3 = 1f;
					if (Application.isEditor && mb_DynamicGameObject._renderer is MeshRenderer)
					{
						num3 = MBVersion.GetScaleInLightmap((MeshRenderer)mb_DynamicGameObject._renderer);
						if (num3 <= 0f)
						{
							num3 = 1f;
						}
					}
					float magnitude = mb_DynamicGameObject.meshSize.magnitude;
					array[i] = num3 * magnitude;
					num2 += array[i];
				}
				for (int j = 0; j < array.Length; j++)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject2 = mbDynamicObjectsInCombinedMesh[j];
					int num4 = mb_DynamicGameObject2.vertIdx + mb_DynamicGameObject2.numVerts;
					float x;
					float num5 = x = this.uv2s[mb_DynamicGameObject2.vertIdx].x;
					float y;
					float num6 = y = this.uv2s[mb_DynamicGameObject2.vertIdx].y;
					for (int k = mb_DynamicGameObject2.vertIdx; k < num4; k++)
					{
						if (this.uv2s[k].x < x)
						{
							x = this.uv2s[k].x;
						}
						if (this.uv2s[k].x > num5)
						{
							num5 = this.uv2s[k].x;
						}
						if (this.uv2s[k].y < y)
						{
							y = this.uv2s[k].y;
						}
						if (this.uv2s[k].y > num6)
						{
							num6 = this.uv2s[k].y;
						}
					}
					array2[j] = new Rect(x, y, num5 - x, num6 - y);
					array[j] /= num2;
					Vector2 item = new Vector2(array2[j].width, array2[j].height) * (array[j] * 8192f);
					list.Add(item);
				}
				AtlasPackingResult atlasPackingResult = new MB2_TexturePackerRegular
				{
					atlasMustBePowerOfTwo = false
				}.GetRects(list, 8192, 8192, num)[0];
				for (int l = 0; l < mbDynamicObjectsInCombinedMesh.Count; l++)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject3 = mbDynamicObjectsInCombinedMesh[l];
					int num7 = mb_DynamicGameObject3.vertIdx + mb_DynamicGameObject3.numVerts;
					Rect rect = array2[l];
					Rect rect2 = atlasPackingResult.rects[l];
					for (int m = mb_DynamicGameObject3.vertIdx; m < num7; m++)
					{
						Vector2 value;
						value.x = (this.uv2s[m].x - rect.x) / rect.width * rect2.width + rect2.x;
						value.y = (this.uv2s[m].y - rect.y) / rect.height * rect2.height + rect2.y;
						this.uv2s[m] = value;
					}
					if (atlasPackingResult.atlasX != atlasPackingResult.atlasY)
					{
						if (atlasPackingResult.atlasX < atlasPackingResult.atlasY)
						{
							float num8 = (float)atlasPackingResult.atlasX / (float)atlasPackingResult.atlasY;
							for (int n = mb_DynamicGameObject3.vertIdx; n < num7; n++)
							{
								Vector2 value2 = this.uv2s[n];
								value2.x *= num8;
								this.uv2s[n] = value2;
							}
						}
						else
						{
							float num9 = (float)atlasPackingResult.atlasY / (float)atlasPackingResult.atlasX;
							for (int num10 = mb_DynamicGameObject3.vertIdx; num10 < num7; num10++)
							{
								Vector2 value3 = this.uv2s[num10];
								value3.y *= num9;
								this.uv2s[num10] = value3;
							}
						}
					}
				}
			}

			private MB3_MeshCombinerSingle.SerializableIntArray[] GetSubmeshTrisWithShowHideApplied(List<MB3_MeshCombinerSingle.MB_DynamicGameObject> mbDynamicObjectsInCombinedMesh)
			{
				bool flag = false;
				for (int i = 0; i < mbDynamicObjectsInCombinedMesh.Count; i++)
				{
					if (!mbDynamicObjectsInCombinedMesh[i].show)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					int[] array = new int[this.submeshTris.Length];
					MB3_MeshCombinerSingle.SerializableIntArray[] array2 = new MB3_MeshCombinerSingle.SerializableIntArray[this.submeshTris.Length];
					for (int j = 0; j < mbDynamicObjectsInCombinedMesh.Count; j++)
					{
						MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject = mbDynamicObjectsInCombinedMesh[j];
						if (mb_DynamicGameObject.show)
						{
							for (int k = 0; k < mb_DynamicGameObject.submeshNumTris.Length; k++)
							{
								array[k] += mb_DynamicGameObject.submeshNumTris[k];
							}
						}
					}
					for (int l = 0; l < array2.Length; l++)
					{
						array2[l] = new MB3_MeshCombinerSingle.SerializableIntArray(array[l]);
					}
					int[] array3 = new int[array2.Length];
					for (int m = 0; m < mbDynamicObjectsInCombinedMesh.Count; m++)
					{
						MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject2 = mbDynamicObjectsInCombinedMesh[m];
						if (mb_DynamicGameObject2.show)
						{
							for (int n = 0; n < this.submeshTris.Length; n++)
							{
								int[] array4 = this.submeshTris[n].data;
								int num = mb_DynamicGameObject2.submeshTriIdxs[n];
								int num2 = num + mb_DynamicGameObject2.submeshNumTris[n];
								for (int num3 = num; num3 < num2; num3++)
								{
									array2[n].data[array3[n]] = array4[num3];
									array3[n]++;
								}
							}
						}
					}
					return array2;
				}
				return this.submeshTris;
			}

			public int[] GetTriangleSizes()
			{
				int[] array = new int[this.submeshTris.Length];
				for (int i = 0; i < this.submeshTris.Length; i++)
				{
					array[i] = this.submeshTris[i].data.Length;
				}
				return array;
			}

			private void _LocalToWorld(Transform t, bool doNorm, bool doTan, int destStartVertsIdx, NativeArray<Vector3> dgoMeshVerts, NativeArray<Vector3> dgoMeshNorms, NativeArray<Vector4> dgoMeshTans, NativeSlice<Vector3> verticies, NativeSlice<Vector3> normals, NativeSlice<Vector4> tangents)
			{
				Vector3 lossyScale = t.lossyScale;
				if (lossyScale == Vector3.one)
				{
					MB3_MeshCombinerSingle.VertexAndTriangleProcessorNativeArray._LocalToWorld_TR(t.rotation, t.position, doNorm, doTan, destStartVertsIdx, dgoMeshVerts, dgoMeshNorms, dgoMeshTans, verticies, normals, tangents);
					return;
				}
				if (lossyScale.x > Mathf.Epsilon && lossyScale.y > Mathf.Epsilon && lossyScale.z > Mathf.Epsilon)
				{
					Matrix4x4 localToWorldMatrix = t.localToWorldMatrix;
					MB3_MeshCombinerSingle.VertexAndTriangleProcessorNativeArray._LocalToWorldMatrix_TRS(ref localToWorldMatrix, doNorm, doTan, destStartVertsIdx, dgoMeshVerts, dgoMeshNorms, dgoMeshTans, verticies, normals, tangents);
					return;
				}
				MB3_MeshCombinerSingle.VertexAndTriangleProcessorNativeArray._LocalToWorld_TRS(t.rotation, t.position, t.lossyScale, doNorm, doTan, destStartVertsIdx, dgoMeshVerts, dgoMeshNorms, dgoMeshTans, verticies, normals, tangents);
			}

			private static void _LocalToWorldMatrix_TRS(ref Matrix4x4 wld_X_local, bool doNorm, bool doTan, int destStartVertsIdx, NativeSlice<Vector3> dgoMeshVerts, NativeSlice<Vector3> dgoMeshNorms, NativeSlice<Vector4> dgoMeshTans, NativeSlice<Vector3> verticies, NativeSlice<Vector3> normals, NativeSlice<Vector4> tangents)
			{
				Matrix4x4 matrix4x = Matrix4x4.zero;
				if (doNorm || doTan)
				{
					matrix4x = wld_X_local;
					matrix4x[0, 3] = (matrix4x[1, 3] = (matrix4x[2, 3] = 0f));
					matrix4x = matrix4x.inverse.transpose;
				}
				for (int i = 0; i < dgoMeshVerts.Length; i++)
				{
					int index = destStartVertsIdx + i;
					verticies[index] = wld_X_local.MultiplyPoint3x4(dgoMeshVerts[i]);
					if (doNorm)
					{
						normals[index] = matrix4x.MultiplyPoint3x4(dgoMeshNorms[i]).normalized;
					}
					if (doTan)
					{
						float w = dgoMeshTans[i].w;
						Vector4 value = matrix4x.MultiplyPoint3x4(dgoMeshTans[i]).normalized;
						value.w = w;
						tangents[index] = value;
					}
				}
			}

			private static void _LocalToWorld_TR(Quaternion wld_Rot_local, Vector3 position_wld, bool doNorm, bool doTan, int destStartVertsIdx, NativeSlice<Vector3> dgoMeshVerts_local, NativeSlice<Vector3> dgoMeshNorms_local, NativeSlice<Vector4> dgoMeshTans_local, NativeSlice<Vector3> verticies, NativeSlice<Vector3> normals, NativeSlice<Vector4> tangents)
			{
				for (int i = 0; i < dgoMeshVerts_local.Length; i++)
				{
					int index = destStartVertsIdx + i;
					Vector3 vector = dgoMeshVerts_local[i];
					vector = wld_Rot_local * vector;
					vector += position_wld;
					verticies[index] = vector;
					if (doNorm)
					{
						Vector3 vector2 = dgoMeshNorms_local[i];
						vector2 = wld_Rot_local * vector2;
						normals[index] = vector2;
					}
					if (doTan)
					{
						Vector3 vector3 = dgoMeshTans_local[i];
						float w = dgoMeshTans_local[i].w;
						vector3 = wld_Rot_local * vector3;
						Vector4 value = vector3;
						value.w = w;
						tangents[index] = value;
					}
				}
			}

			private static void _LocalToWorld_TRS(Quaternion wld_Rot_local, Vector3 position_wld, Vector3 scale, bool doNorm, bool doTan, int destStartVertsIdx, NativeSlice<Vector3> dgoMeshVerts_local, NativeSlice<Vector3> dgoMeshNorms_local, NativeSlice<Vector4> dgoMeshTans_local, NativeSlice<Vector3> verticies, NativeSlice<Vector3> normals, NativeSlice<Vector4> tangents)
			{
				Vector3 one = Vector3.one;
				if (doNorm || doTan)
				{
					one.x = ((scale.x < Mathf.Epsilon) ? 0f : (1f / scale.x));
					one.y = ((scale.y < Mathf.Epsilon) ? 0f : (1f / scale.y));
					one.z = ((scale.z < Mathf.Epsilon) ? 0f : (1f / scale.z));
				}
				for (int i = 0; i < dgoMeshVerts_local.Length; i++)
				{
					int index = destStartVertsIdx + i;
					Vector3 vector = dgoMeshVerts_local[i];
					vector.x *= scale.x;
					vector.y *= scale.y;
					vector.z *= scale.z;
					vector = wld_Rot_local * vector;
					vector += position_wld;
					verticies[index] = vector;
					if (doNorm)
					{
						Vector3 vector2 = dgoMeshNorms_local[i];
						vector2.x *= one.x;
						vector2.y *= one.y;
						vector2.z *= one.z;
						vector2 = wld_Rot_local * vector2;
						vector2.Normalize();
						normals[index] = vector2;
					}
					if (doTan)
					{
						Vector3 vector3 = dgoMeshTans_local[i];
						float w = dgoMeshTans_local[i].w;
						vector3.x *= one.x;
						vector3.y *= one.y;
						vector3.z *= one.z;
						vector3 = wld_Rot_local * vector3;
						vector3.Normalize();
						tangents[index] = new Vector4(vector3.x, vector3.y, vector3.z, w);
					}
				}
			}

			private bool _disposed;

			private bool _isInitialized;

			internal MB2_LogLevel LOG_LEVEL;

			internal VertexAttributeDescriptor[] vertexAttributes;

			internal bool dataArrayAllocated;

			internal Mesh.MeshDataArray dataArray;

			internal Mesh.MeshData data;

			internal int vertexCount;

			internal NativeArray<Vector3> verticiesModified;

			internal NativeSlice<Vector3> verticies;

			internal NativeSlice<Vector3> normals;

			internal NativeSlice<Vector4> tangents;

			internal NativeSlice<Color> colors;

			internal NativeSlice<Vector2> uv0s;

			internal NativeSlice<Vector2> uv2s;

			internal NativeSlice<Vector2> uv3s;

			internal NativeSlice<Vector2> uv4s;

			internal NativeSlice<Vector2> uv5s;

			internal NativeSlice<Vector2> uv6s;

			internal NativeSlice<Vector2> uv7s;

			internal NativeSlice<Vector2> uv8s;

			internal NativeSlice<float> uvsSliceIdx;

			internal NativeSlice<Vector3> uvsWithExtraIndex;

			private MB3_MeshCombinerSingle.SerializableIntArray[] submeshTris;

			internal NativeArray<ushort> triangleBuffer;

			internal int bufferStride_0;

			internal int bufferStride_1;

			internal int bufferStride_2;

			internal Type rawSliceSizerType_0;

			internal Type rawSliceSizerType_1;

			internal object rawSliceVertexStream_0;

			internal object rawSliceVertexStream_1;
		}
	}
}
