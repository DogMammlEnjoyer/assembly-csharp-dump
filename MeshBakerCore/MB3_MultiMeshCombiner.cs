using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	[Serializable]
	public class MB3_MultiMeshCombiner : MB3_MeshCombiner
	{
		public override MB2_LogLevel LOG_LEVEL
		{
			get
			{
				return this._LOG_LEVEL;
			}
			set
			{
				this._LOG_LEVEL = value;
				for (int i = 0; i < this.meshCombiners.Count; i++)
				{
					this.meshCombiners[i].combinedMesh.LOG_LEVEL = value;
				}
			}
		}

		public override MB2_ValidationLevel validationLevel
		{
			get
			{
				return this._validationLevel;
			}
			set
			{
				this._validationLevel = value;
				for (int i = 0; i < this.meshCombiners.Count; i++)
				{
					this.meshCombiners[i].combinedMesh.validationLevel = this._validationLevel;
				}
			}
		}

		public int maxVertsInMesh
		{
			get
			{
				return this._maxVertsInMesh;
			}
			set
			{
				if (this.obj2MeshCombinerMap.Count > 0)
				{
					return;
				}
				if (value < 3)
				{
					Debug.LogError("Max verts in mesh must be greater than three.");
					return;
				}
				if (value > MBVersion.MaxMeshVertexCount())
				{
					Debug.LogError("MultiMeshCombiner error in maxVertsInMesh. Meshes in unity cannot have more than " + MBVersion.MaxMeshVertexCount().ToString() + " vertices. " + value.ToString());
					return;
				}
				this._maxVertsInMesh = value;
			}
		}

		public override int GetNumObjectsInCombined()
		{
			return this.obj2MeshCombinerMap.Count;
		}

		public override List<GameObject> GetObjectsInCombined()
		{
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < this.meshCombiners.Count; i++)
			{
				list.AddRange(this.meshCombiners[i].combinedMesh.GetObjectsInCombined());
			}
			return list;
		}

		public override int GetLightmapIndex()
		{
			if (this.meshCombiners.Count > 0)
			{
				return this.meshCombiners[0].combinedMesh.GetLightmapIndex();
			}
			return -1;
		}

		public override bool CombinedMeshContains(GameObject go)
		{
			return this.obj2MeshCombinerMap.ContainsKey(go.GetInstanceID());
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

		public override bool Apply(MB3_MeshCombiner.GenerateUV2Delegate uv2GenerationMethod)
		{
			bool flag = true;
			if (this._bakeStatus != MB3_MeshCombiner.MeshCombiningStatus.readyForApply)
			{
				Debug.LogError("Apply was called when combiner was not in 'readyForApply' state. Did you call AddDelete(), Update() or ShowHide()");
				return false;
			}
			for (int i = 0; i < this.meshCombiners.Count; i++)
			{
				if (this.meshCombiners[i].isDirty)
				{
					flag &= this.meshCombiners[i].combinedMesh.Apply(uv2GenerationMethod);
					this.meshCombiners[i].isDirty = false;
				}
			}
			if (base.settings.clearBuffersAfterBake)
			{
				this.obj2MeshCombinerMap.Clear();
			}
			this._bakeStatus = MB3_MeshCombiner.MeshCombiningStatus.preAddDeleteOrUpdate;
			return flag;
		}

		public override bool Apply(bool triangles, bool vertices, bool normals, bool tangents, bool uvs, bool uv2, bool uv3, bool uv4, bool colors, bool bones = false, bool blendShapeFlag = false, MB3_MeshCombiner.GenerateUV2Delegate uv2GenerationMethod = null)
		{
			return this.Apply(triangles, vertices, normals, tangents, uvs, uv2, uv3, uv4, false, false, false, false, colors, bones, blendShapeFlag, uv2GenerationMethod);
		}

		public override bool Apply(bool triangles, bool vertices, bool normals, bool tangents, bool uvs, bool uv2, bool uv3, bool uv4, bool uv5, bool uv6, bool uv7, bool uv8, bool colors, bool bones = false, bool blendShapesFlag = false, MB3_MeshCombiner.GenerateUV2Delegate uv2GenerationMethod = null)
		{
			if (this._bakeStatus != MB3_MeshCombiner.MeshCombiningStatus.readyForApply)
			{
				Debug.LogError("Apply was called when combiner was not in 'readyForApply' state. Did you call AddDelete(), Update() or ShowHide()");
				return false;
			}
			bool flag = true;
			for (int i = 0; i < this.meshCombiners.Count; i++)
			{
				if (this.meshCombiners[i].isDirty)
				{
					flag &= this.meshCombiners[i].combinedMesh.Apply(triangles, vertices, normals, tangents, uvs, uv2, uv3, uv4, colors, bones, blendShapesFlag, uv2GenerationMethod);
					this.meshCombiners[i].isDirty = false;
				}
			}
			if (base.settings.clearBuffersAfterBake)
			{
				this.obj2MeshCombinerMap.Clear();
			}
			this._bakeStatus = MB3_MeshCombiner.MeshCombiningStatus.preAddDeleteOrUpdate;
			return flag;
		}

		public override void UpdateSkinnedMeshApproximateBounds()
		{
			for (int i = 0; i < this.meshCombiners.Count; i++)
			{
				this.meshCombiners[i].combinedMesh.UpdateSkinnedMeshApproximateBounds();
			}
		}

		public override void UpdateSkinnedMeshApproximateBoundsFromBones()
		{
			for (int i = 0; i < this.meshCombiners.Count; i++)
			{
				this.meshCombiners[i].combinedMesh.UpdateSkinnedMeshApproximateBoundsFromBones();
			}
		}

		public override void UpdateSkinnedMeshApproximateBoundsFromBounds()
		{
			for (int i = 0; i < this.meshCombiners.Count; i++)
			{
				this.meshCombiners[i].combinedMesh.UpdateSkinnedMeshApproximateBoundsFromBounds();
			}
		}

		public override bool UpdateGameObjects(GameObject[] gos, bool recalcBounds, bool updateVertices, bool updateNormals, bool updateTangents, bool updateUV, bool updateUV2, bool updateUV3, bool updateUV4, bool updateColors, bool updateSkinningInfo)
		{
			return this.UpdateGameObjects(gos, recalcBounds, updateVertices, updateNormals, updateTangents, updateUV, updateUV2, updateUV3, updateUV4, false, false, false, false, updateColors, updateSkinningInfo);
		}

		public override bool UpdateGameObjects(GameObject[] gos, bool recalcBounds, bool updateVertices, bool updateNormals, bool updateTangents, bool updateUV, bool updateUV2, bool updateUV3, bool updateUV4, bool updateUV5, bool updateUV6, bool updateUV7, bool updateUV8, bool updateColors, bool updateSkinningInfo)
		{
			if (gos == null)
			{
				Debug.LogError("list of game objects cannot be null");
				return false;
			}
			if (this._bakeStatus != MB3_MeshCombiner.MeshCombiningStatus.preAddDeleteOrUpdate)
			{
				Debug.LogError("Bake Status of combiner was not 'preAddDeleteOrUpdate'. This can happen if AddDeleteGameObjects or UpdateGameObjects is called twice without calling Apply. You can call 'ClearBuffers' to reset the combiner.");
				return false;
			}
			for (int i = 0; i < this.meshCombiners.Count; i++)
			{
				this.meshCombiners[i].gosToUpdate.Clear();
			}
			for (int j = 0; j < gos.Length; j++)
			{
				MB3_MultiMeshCombiner.CombinedMesh combinedMesh = null;
				this.obj2MeshCombinerMap.TryGetValue(gos[j].GetInstanceID(), out combinedMesh);
				if (combinedMesh != null)
				{
					combinedMesh.gosToUpdate.Add(gos[j]);
				}
				else
				{
					string str = "Object ";
					GameObject gameObject = gos[j];
					Debug.LogWarning(str + ((gameObject != null) ? gameObject.ToString() : null) + " is not in the combined mesh.");
				}
			}
			bool flag = true;
			for (int k = 0; k < this.meshCombiners.Count; k++)
			{
				if (this.meshCombiners[k].gosToUpdate.Count > 0)
				{
					this.meshCombiners[k].isDirty = true;
					GameObject[] gos2 = this.meshCombiners[k].gosToUpdate.ToArray();
					flag = (flag && this.meshCombiners[k].combinedMesh.UpdateGameObjects(gos2, recalcBounds, updateVertices, updateNormals, updateTangents, updateUV, updateUV2, updateUV3, updateUV4, updateUV5, updateUV6, updateUV7, updateUV8, updateColors, updateSkinningInfo));
				}
			}
			this._bakeStatus = MB3_MeshCombiner.MeshCombiningStatus.readyForApply;
			return flag;
		}

		public override bool AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs, bool disableRendererInSource = true)
		{
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
			return this.AddDeleteGameObjectsByID(gos, array, disableRendererInSource);
		}

		public override bool AddDeleteGameObjectsByID(GameObject[] gos, int[] deleteGOinstanceIDs, bool disableRendererInSource = true)
		{
			if (this._bakeStatus != MB3_MeshCombiner.MeshCombiningStatus.preAddDeleteOrUpdate)
			{
				Debug.LogError("Bake Status of combiner was not 'preAddDeleteOrUpdate'. This can happen if AddDeleteGameObjects or UpdateGameObjects is called twice without calling Apply. You can call 'ClearBuffers' to reset the combiner.");
				return false;
			}
			if (deleteGOinstanceIDs == null)
			{
				deleteGOinstanceIDs = MB3_MultiMeshCombiner.emptyIDs;
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
			if (!this._validate(gos, deleteGOinstanceIDs))
			{
				return false;
			}
			this._distributeAmongBakers(gos, deleteGOinstanceIDs);
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				MB2_Log.LogDebug(string.Concat(new string[]
				{
					"MB2_MultiMeshCombiner.AddDeleteGameObjects numCombinedMeshes: ",
					this.meshCombiners.Count.ToString(),
					" added:",
					((gos == null) ? 0 : gos.Length).ToString(),
					" deleted:",
					((deleteGOinstanceIDs == null) ? 0 : deleteGOinstanceIDs.Length).ToString(),
					" disableRendererInSource:",
					disableRendererInSource.ToString(),
					" maxVertsPerCombined:",
					this._maxVertsInMesh.ToString()
				}), Array.Empty<object>());
			}
			bool result = this._bakeStep1(gos, deleteGOinstanceIDs, disableRendererInSource);
			this._bakeStatus = MB3_MeshCombiner.MeshCombiningStatus.readyForApply;
			return result;
		}

		private bool _validate(GameObject[] gos, int[] deleteGOinstanceIDs)
		{
			if (this._validationLevel == MB2_ValidationLevel.none)
			{
				return true;
			}
			if (this._maxVertsInMesh < 3)
			{
				Debug.LogError("Invalid value for maxVertsInMesh=" + this._maxVertsInMesh.ToString());
			}
			this._validateTextureBakeResults();
			int num = 0;
			for (int i = 0; i < this.meshCombiners.Count; i++)
			{
				num += this.meshCombiners[i].combinedMesh.GetNumObjectsInCombined();
			}
			if (this.obj2MeshCombinerMap.Count != num)
			{
				this.obj2MeshCombinerMap.Clear();
				for (int j = 0; j < this.meshCombiners.Count; j++)
				{
					List<MB3_MeshCombinerSingle.MB_DynamicGameObject> mbDynamicObjectsInCombinedMesh = this.meshCombiners[j].combinedMesh.mbDynamicObjectsInCombinedMesh;
					for (int k = 0; k < mbDynamicObjectsInCombinedMesh.Count; k++)
					{
						if (mbDynamicObjectsInCombinedMesh[k].gameObject != null)
						{
							int instanceID = mbDynamicObjectsInCombinedMesh[k].gameObject.GetInstanceID();
							mbDynamicObjectsInCombinedMesh[k].instanceID = instanceID;
						}
						this.obj2MeshCombinerMap.Add(mbDynamicObjectsInCombinedMesh[k].instanceID, this.meshCombiners[j]);
					}
				}
			}
			if (gos != null)
			{
				for (int l = 0; l < gos.Length; l++)
				{
					if (gos[l] == null)
					{
						Debug.LogError("The " + l.ToString() + "th object on the list of objects to combine is 'None'. Use Command-Delete on Mac OS X; Delete or Shift-Delete on Windows to remove this one element.");
						return false;
					}
					if (this._validationLevel >= MB2_ValidationLevel.robust)
					{
						for (int m = l + 1; m < gos.Length; m++)
						{
							if (gos[l] == gos[m])
							{
								string str = "GameObject ";
								GameObject gameObject = gos[l];
								Debug.LogError(str + ((gameObject != null) ? gameObject.ToString() : null) + "appears twice in list of game objects to add");
								return false;
							}
						}
						if (this.obj2MeshCombinerMap.ContainsKey(gos[l].GetInstanceID()))
						{
							bool flag = false;
							if (deleteGOinstanceIDs != null)
							{
								for (int n = 0; n < deleteGOinstanceIDs.Length; n++)
								{
									if (deleteGOinstanceIDs[n] == gos[l].GetInstanceID())
									{
										flag = true;
									}
								}
							}
							if (!flag)
							{
								string str2 = "GameObject ";
								GameObject gameObject2 = gos[l];
								Debug.LogError(str2 + ((gameObject2 != null) ? gameObject2.ToString() : null) + " is already in the combined mesh " + gos[l].GetInstanceID().ToString());
								return false;
							}
						}
					}
				}
			}
			if (deleteGOinstanceIDs != null && this._validationLevel >= MB2_ValidationLevel.robust)
			{
				for (int num2 = 0; num2 < deleteGOinstanceIDs.Length; num2++)
				{
					for (int num3 = num2 + 1; num3 < deleteGOinstanceIDs.Length; num3++)
					{
						if (deleteGOinstanceIDs[num2] == deleteGOinstanceIDs[num3])
						{
							Debug.LogError("GameObject " + deleteGOinstanceIDs[num2].ToString() + "appears twice in list of game objects to delete");
							return false;
						}
					}
					if (!this.obj2MeshCombinerMap.ContainsKey(deleteGOinstanceIDs[num2]))
					{
						Debug.LogWarning("GameObject with instance ID " + deleteGOinstanceIDs[num2].ToString() + " on the list of objects to delete is not in the combined mesh.");
					}
				}
			}
			return true;
		}

		private void _distributeAmongBakers(GameObject[] gos, int[] deleteGOinstanceIDs)
		{
			if (gos == null)
			{
				gos = MB3_MultiMeshCombiner.empty;
			}
			if (deleteGOinstanceIDs == null)
			{
				deleteGOinstanceIDs = MB3_MultiMeshCombiner.emptyIDs;
			}
			if (this.resultSceneObject == null)
			{
				this.resultSceneObject = new GameObject("CombinedMesh-" + base.name);
			}
			for (int i = 0; i < this.meshCombiners.Count; i++)
			{
				this.meshCombiners[i].extraSpace = this._maxVertsInMesh - this.meshCombiners[i].combinedMesh.GetMesh().vertexCount;
			}
			for (int j = 0; j < deleteGOinstanceIDs.Length; j++)
			{
				MB3_MultiMeshCombiner.CombinedMesh combinedMesh = null;
				if (this.obj2MeshCombinerMap.TryGetValue(deleteGOinstanceIDs[j], out combinedMesh))
				{
					if (this.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug("MB2_MultiMeshCombiner.Removing " + deleteGOinstanceIDs[j].ToString() + " from meshCombiner " + this.meshCombiners.IndexOf(combinedMesh).ToString(), Array.Empty<object>());
					}
					MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject;
					combinedMesh.combinedMesh.InstanceID2DGO(deleteGOinstanceIDs[j], out mb_DynamicGameObject);
					combinedMesh.numVertsInListToDelete += mb_DynamicGameObject.numVerts;
					combinedMesh.gosToDelete.Add(deleteGOinstanceIDs[j]);
				}
				else
				{
					Debug.LogWarning("Object " + deleteGOinstanceIDs[j].ToString() + " in the list of objects to delete is not in the combined mesh.");
				}
			}
			for (int k = 0; k < gos.Length; k++)
			{
				GameObject gameObject = gos[k];
				int vertexCount = MB_Utility.GetMesh(gameObject).vertexCount;
				MB3_MultiMeshCombiner.CombinedMesh combinedMesh2 = null;
				int l = 0;
				while (l < this.meshCombiners.Count)
				{
					if (this.meshCombiners[l].extraSpace + this.meshCombiners[l].numVertsInListToDelete - this.meshCombiners[l].numVertsInListToAdd > vertexCount)
					{
						combinedMesh2 = this.meshCombiners[l];
						if (this.LOG_LEVEL >= MB2_LogLevel.debug)
						{
							string str = "MB2_MultiMeshCombiner.Added ";
							GameObject gameObject2 = gos[k];
							MB2_Log.LogDebug(str + ((gameObject2 != null) ? gameObject2.ToString() : null) + " to combinedMesh " + l.ToString(), new object[]
							{
								this.LOG_LEVEL
							});
							break;
						}
						break;
					}
					else
					{
						l++;
					}
				}
				if (combinedMesh2 == null)
				{
					combinedMesh2 = new MB3_MultiMeshCombiner.CombinedMesh(this.maxVertsInMesh, this._resultSceneObject, this._LOG_LEVEL);
					this._setMBValues(combinedMesh2.combinedMesh);
					this.meshCombiners.Add(combinedMesh2);
					if (this.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug("MB2_MultiMeshCombiner.Created new combinedMesh", Array.Empty<object>());
					}
				}
				combinedMesh2.gosToAdd.Add(gameObject);
				combinedMesh2.numVertsInListToAdd += vertexCount;
			}
		}

		private bool _bakeStep1(GameObject[] gos, int[] deleteGOinstanceIDs, bool disableRendererInSource)
		{
			for (int i = 0; i < this.meshCombiners.Count; i++)
			{
				MB3_MultiMeshCombiner.CombinedMesh combinedMesh = this.meshCombiners[i];
				if (combinedMesh.combinedMesh.targetRenderer == null)
				{
					combinedMesh.combinedMesh.resultSceneObject = this._resultSceneObject;
					combinedMesh.combinedMesh.BuildSceneMeshObject(gos, true);
					if (this._LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug("BuildSO combiner {0} goID {1} targetRenID {2} meshID {3}", new object[]
						{
							i,
							combinedMesh.combinedMesh.targetRenderer.gameObject.GetInstanceID(),
							combinedMesh.combinedMesh.targetRenderer.GetInstanceID(),
							combinedMesh.combinedMesh.GetMesh().GetInstanceID()
						});
					}
				}
				else if (this.resultSceneObject != null && combinedMesh.combinedMesh.targetRenderer.transform.parent != this.resultSceneObject.transform)
				{
					Debug.LogError("targetRender objects must be children of resultSceneObject");
					return false;
				}
				if (combinedMesh.gosToAdd.Count > 0 || combinedMesh.gosToDelete.Count > 0)
				{
					combinedMesh.combinedMesh.AddDeleteGameObjectsByID(combinedMesh.gosToAdd.ToArray(), combinedMesh.gosToDelete.ToArray(), disableRendererInSource);
					if (this._LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug("Baked combiner {0} obsAdded {1} objsRemoved {2} goID {3} targetRenID {4} meshID {5}", new object[]
						{
							i,
							combinedMesh.gosToAdd.Count,
							combinedMesh.gosToDelete.Count,
							combinedMesh.combinedMesh.targetRenderer.gameObject.GetInstanceID(),
							combinedMesh.combinedMesh.targetRenderer.GetInstanceID(),
							combinedMesh.combinedMesh.GetMesh().GetInstanceID()
						});
					}
				}
				Renderer targetRenderer = combinedMesh.combinedMesh.targetRenderer;
				Mesh mesh = combinedMesh.combinedMesh.GetMesh();
				if (targetRenderer is MeshRenderer)
				{
					targetRenderer.gameObject.GetComponent<MeshFilter>().sharedMesh = mesh;
				}
				else
				{
					((SkinnedMeshRenderer)targetRenderer).sharedMesh = mesh;
				}
			}
			for (int j = 0; j < this.meshCombiners.Count; j++)
			{
				MB3_MultiMeshCombiner.CombinedMesh combinedMesh2 = this.meshCombiners[j];
				for (int k = 0; k < combinedMesh2.gosToDelete.Count; k++)
				{
					this.obj2MeshCombinerMap.Remove(combinedMesh2.gosToDelete[k]);
				}
			}
			for (int l = 0; l < this.meshCombiners.Count; l++)
			{
				MB3_MultiMeshCombiner.CombinedMesh combinedMesh3 = this.meshCombiners[l];
				for (int m = 0; m < combinedMesh3.gosToAdd.Count; m++)
				{
					this.obj2MeshCombinerMap.Add(combinedMesh3.gosToAdd[m].GetInstanceID(), combinedMesh3);
				}
				if (combinedMesh3.gosToAdd.Count > 0 || combinedMesh3.gosToDelete.Count > 0)
				{
					combinedMesh3.gosToDelete.Clear();
					combinedMesh3.gosToAdd.Clear();
					combinedMesh3.numVertsInListToDelete = 0;
					combinedMesh3.numVertsInListToAdd = 0;
					combinedMesh3.isDirty = true;
				}
			}
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				string text = "Meshes in combined:";
				for (int n = 0; n < this.meshCombiners.Count; n++)
				{
					text = string.Concat(new string[]
					{
						text,
						" mesh",
						n.ToString(),
						"(",
						this.meshCombiners[n].combinedMesh.GetObjectsInCombined().Count.ToString(),
						")\n"
					});
				}
				text = text + "children in result: " + this.resultSceneObject.transform.childCount.ToString();
				MB2_Log.LogDebug(text, new object[]
				{
					this.LOG_LEVEL
				});
			}
			return this.meshCombiners.Count > 0;
		}

		public override void ClearBuffers()
		{
			for (int i = 0; i < this.meshCombiners.Count; i++)
			{
				this.meshCombiners[i].combinedMesh.ClearBuffers();
			}
			this.obj2MeshCombinerMap.Clear();
			this._bakeStatus = MB3_MeshCombiner.MeshCombiningStatus.preAddDeleteOrUpdate;
		}

		public override void ClearMesh()
		{
			this.DestroyMesh();
			this.ClearBuffers();
		}

		public override void ClearMesh(MB2_EditorMethodsInterface editorMethods)
		{
			this.DestroyMeshEditor(editorMethods);
			this.ClearBuffers();
		}

		internal override void _DisposeRuntimeCreated()
		{
			for (int i = 0; i < this.meshCombiners.Count; i++)
			{
				this.meshCombiners[i].combinedMesh._DisposeRuntimeCreated();
			}
		}

		public override void DestroyMesh()
		{
			for (int i = 0; i < this.meshCombiners.Count; i++)
			{
				if (this.meshCombiners[i].combinedMesh.targetRenderer != null)
				{
					MB_Utility.Destroy(this.meshCombiners[i].combinedMesh.targetRenderer.gameObject);
				}
				this.meshCombiners[i].combinedMesh.Dispose();
			}
			this.obj2MeshCombinerMap.Clear();
			this.meshCombiners.Clear();
		}

		public override void DestroyMeshEditor(MB2_EditorMethodsInterface editorMethods)
		{
			editorMethods.Destroy(this.resultSceneObject);
			for (int i = 0; i < this.meshCombiners.Count; i++)
			{
				this.meshCombiners[i].combinedMesh.ClearMesh();
			}
			this.obj2MeshCombinerMap.Clear();
			this.meshCombiners.Clear();
		}

		private void _setMBValues(MB3_MeshCombinerSingle targ)
		{
			targ.validationLevel = this._validationLevel;
			targ.textureBakeResults = this.textureBakeResults;
			targ.outputOption = MB2_OutputOptions.bakeIntoSceneObject;
			if (this.settingsHolder != null)
			{
				targ.settingsHolder = this.settingsHolder;
				return;
			}
			targ.renderType = this.renderType;
			targ.lightmapOption = this.lightmapOption;
			targ.doNorm = this.doNorm;
			targ.doTan = this.doTan;
			targ.doCol = this.doCol;
			targ.doUV = this.doUV;
			targ.doUV3 = this.doUV3;
			targ.doUV4 = this.doUV4;
			targ.doUV5 = this.doUV5;
			targ.doUV6 = this.doUV6;
			targ.doUV7 = this.doUV7;
			targ.doUV8 = this.doUV8;
			targ.doBlendShapes = this.doBlendShapes;
			targ.optimizeAfterBake = base.optimizeAfterBake;
			targ.pivotLocationType = this.pivotLocationType;
			targ.uv2UnwrappingParamsHardAngle = base.uv2UnwrappingParamsHardAngle;
			targ.uv2UnwrappingParamsPackMargin = base.uv2UnwrappingParamsPackMargin;
			targ.assignToMeshCustomizer = base.assignToMeshCustomizer;
		}

		public override List<Material> GetMaterialsOnTargetRenderer()
		{
			HashSet<Material> hashSet = new HashSet<Material>();
			for (int i = 0; i < this.meshCombiners.Count; i++)
			{
				hashSet.UnionWith(this.meshCombiners[i].combinedMesh.GetMaterialsOnTargetRenderer());
			}
			return new List<Material>(hashSet);
		}

		public override void CheckIntegrity()
		{
			if (!MB_Utility.DO_INTEGRITY_CHECKS)
			{
				return;
			}
			for (int i = 0; i < this.meshCombiners.Count; i++)
			{
				this.meshCombiners[i].combinedMesh.CheckIntegrity();
			}
		}

		private static GameObject[] empty = new GameObject[0];

		private static int[] emptyIDs = new int[0];

		public Dictionary<int, MB3_MultiMeshCombiner.CombinedMesh> obj2MeshCombinerMap = new Dictionary<int, MB3_MultiMeshCombiner.CombinedMesh>();

		[SerializeField]
		public List<MB3_MultiMeshCombiner.CombinedMesh> meshCombiners = new List<MB3_MultiMeshCombiner.CombinedMesh>();

		[SerializeField]
		private int _maxVertsInMesh = 65535;

		[Serializable]
		public class CombinedMesh
		{
			public CombinedMesh(int maxNumVertsInMesh, GameObject resultSceneObject, MB2_LogLevel ll)
			{
				this.combinedMesh = new MB3_MeshCombinerSingle();
				this.combinedMesh.resultSceneObject = resultSceneObject;
				this.combinedMesh.LOG_LEVEL = ll;
				this.extraSpace = maxNumVertsInMesh;
				this.numVertsInListToDelete = 0;
				this.numVertsInListToAdd = 0;
				this.gosToAdd = new List<GameObject>();
				this.gosToDelete = new List<int>();
				this.gosToUpdate = new List<GameObject>();
			}

			public bool isEmpty()
			{
				List<GameObject> list = new List<GameObject>();
				list.AddRange(this.combinedMesh.GetObjectsInCombined());
				for (int i = 0; i < this.gosToDelete.Count; i++)
				{
					for (int j = 0; j < list.Count; j++)
					{
						if (list[j].GetInstanceID() == this.gosToDelete[i])
						{
							list.RemoveAt(j);
							break;
						}
					}
				}
				return list.Count == 0;
			}

			public MB3_MeshCombinerSingle combinedMesh;

			public int extraSpace = -1;

			public int numVertsInListToDelete;

			public int numVertsInListToAdd;

			public List<GameObject> gosToAdd;

			public List<int> gosToDelete;

			public List<GameObject> gosToUpdate;

			public bool isDirty;
		}
	}
}
