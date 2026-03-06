using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.IO.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering
{
	public sealed class ProbeVolumeBakingSet : ScriptableObject, ISerializationCallbackReceiver
	{
		internal bool hasDilation
		{
			get
			{
				return this.settings.dilationSettings.enableDilation && this.settings.dilationSettings.dilationDistance > 0f;
			}
		}

		public IReadOnlyList<string> sceneGUIDs
		{
			get
			{
				return this.m_SceneGUIDs;
			}
		}

		public IReadOnlyList<string> lightingScenarios
		{
			get
			{
				return this.m_LightingScenarios;
			}
		}

		internal bool bakedSkyOcclusion
		{
			get
			{
				return this.bakedSkyOcclusionValue > 0;
			}
			set
			{
				this.bakedSkyOcclusionValue = (value ? 1 : 0);
			}
		}

		internal bool bakedSkyShadingDirection
		{
			get
			{
				return this.bakedSkyShadingDirectionValue > 0;
			}
			set
			{
				this.bakedSkyShadingDirectionValue = (value ? 1 : 0);
			}
		}

		internal string otherScenario
		{
			get
			{
				return this.m_OtherScenario;
			}
		}

		internal float scenarioBlendingFactor
		{
			get
			{
				return this.m_ScenarioBlendingFactor;
			}
		}

		public int cellSizeInBricks
		{
			get
			{
				return ProbeVolumeBakingSet.GetCellSizeInBricks(this.bakedSimplificationLevels);
			}
		}

		public int maxSubdivision
		{
			get
			{
				return ProbeVolumeBakingSet.GetMaxSubdivision(this.bakedSimplificationLevels);
			}
		}

		public float minBrickSize
		{
			get
			{
				return ProbeVolumeBakingSet.GetMinBrickSize(this.bakedMinDistanceBetweenProbes);
			}
		}

		public float cellSizeInMeters
		{
			get
			{
				return (float)this.cellSizeInBricks * this.minBrickSize;
			}
		}

		internal uint4 ComputeRegionMasks()
		{
			uint4 result = 0U;
			if (!this.useRenderingLayers || this.renderingLayerMasks == null)
			{
				result.x = uint.MaxValue;
			}
			else
			{
				for (int i = 0; i < this.renderingLayerMasks.Length; i++)
				{
					result[i] = this.renderingLayerMasks[i].mask;
				}
			}
			return result;
		}

		internal static int GetCellSizeInBricks(int simplificationLevels)
		{
			return (int)Mathf.Pow(3f, (float)simplificationLevels);
		}

		internal static int GetMaxSubdivision(int simplificationLevels)
		{
			return simplificationLevels + 1;
		}

		internal static float GetMinBrickSize(float minDistanceBetweenProbes)
		{
			return Mathf.Max(0.01f, minDistanceBetweenProbes * 3f);
		}

		private void OnValidate()
		{
			this.singleSceneMode &= (this.m_SceneGUIDs.Count <= 1);
			if (this.m_LightingScenarios.Count == 0)
			{
				this.m_LightingScenarios = new List<string>
				{
					ProbeReferenceVolume.defaultLightingScenario
				};
			}
			this.settings.Upgrade();
		}

		private void OnEnable()
		{
			this.Migrate();
			this.m_HasSupportData = this.ComputeHasSupportData();
			this.m_SharedDataIsValid = this.ComputeHasValidSharedData();
		}

		internal void Migrate()
		{
			if (this.version != CoreUtils.GetLastEnumValue<ProbeVolumeBakingSet.Version>())
			{
				ProbeVolumeBakingSet.Version version = this.version;
				if (this.version < ProbeVolumeBakingSet.Version.AssetsAlwaysReferenced)
				{
					bool isInitialized = ProbeReferenceVolume.instance.isInitialized;
				}
			}
			if (this.sharedValidityMaskChunkSize == 0)
			{
				this.sharedValidityMaskChunkSize = ProbeBrickPool.GetChunkSizeInProbeCount();
			}
			if (this.settings.virtualOffsetSettings.validityThreshold == 0f)
			{
				this.settings.virtualOffsetSettings.validityThreshold = 0.25f;
			}
		}

		private bool ComputeHasValidSharedData()
		{
			return this.cellSharedDataAsset != null && this.cellSharedDataAsset.FileExists() && this.cellBricksDataAsset.FileExists();
		}

		internal bool HasValidSharedData()
		{
			return this.m_SharedDataIsValid;
		}

		internal bool CheckCompatibleCellLayout()
		{
			return this.simplificationLevels == this.bakedSimplificationLevels && this.minDistanceBetweenProbes == this.bakedMinDistanceBetweenProbes && this.skyOcclusion == this.bakedSkyOcclusion && this.skyOcclusionShadingDirection == this.bakedSkyShadingDirection && this.settings.virtualOffsetSettings.useVirtualOffset == (this.supportOffsetsChunkSize != 0) && this.useRenderingLayers == (this.bakedMaskCount != 1);
		}

		private bool ComputeHasSupportData()
		{
			return this.cellSupportDataAsset != null && this.cellSupportDataAsset.IsValid() && this.cellSupportDataAsset.FileExists();
		}

		internal bool HasSupportData()
		{
			return this.m_HasSupportData;
		}

		public bool HasBakedData(string scenario = null)
		{
			if (scenario == null)
			{
				return this.scenarios.ContainsKey(ProbeReferenceVolume.defaultLightingScenario);
			}
			return (ProbeReferenceVolume.instance.supportLightingScenarios || !(scenario != ProbeReferenceVolume.defaultLightingScenario)) && this.scenarios.ContainsKey(scenario);
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			if (!this.m_LightingScenarios.Contains(this.lightingScenario))
			{
				if (this.m_LightingScenarios.Count != 0)
				{
					this.lightingScenario = this.m_LightingScenarios[0];
				}
				else
				{
					this.lightingScenario = ProbeReferenceVolume.defaultLightingScenario;
				}
			}
			this.perSceneCellLists.Clear();
			foreach (ProbeVolumeBakingSet.SerializedPerSceneCellList serializedPerSceneCellList in this.m_SerializedPerSceneCellList)
			{
				this.perSceneCellLists.Add(serializedPerSceneCellList.sceneGUID, serializedPerSceneCellList.cellList);
			}
			if (this.m_OtherScenario == "")
			{
				this.m_OtherScenario = null;
			}
			if (this.bakedSimplificationLevels == -1)
			{
				this.bakedSimplificationLevels = this.simplificationLevels;
				this.bakedMinDistanceBetweenProbes = this.minDistanceBetweenProbes;
			}
			if (this.bakedSkyOcclusionValue == -1)
			{
				this.bakedSkyOcclusion = false;
			}
			if (this.bakedSkyShadingDirectionValue == -1)
			{
				this.bakedSkyShadingDirection = false;
			}
			if (this.cellDescs.Count != 0)
			{
				Dictionary<int, ProbeReferenceVolume.CellDesc>.ValueCollection.Enumerator enumerator2 = this.cellDescs.Values.GetEnumerator();
				enumerator2.MoveNext();
				if (enumerator2.Current.bricksCount == 0)
				{
					foreach (ProbeReferenceVolume.CellDesc cellDesc in this.cellDescs.Values)
					{
						cellDesc.probeCount /= 64;
					}
				}
			}
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			this.m_SerializedPerSceneCellList = new List<ProbeVolumeBakingSet.SerializedPerSceneCellList>();
			foreach (KeyValuePair<string, List<int>> keyValuePair in this.perSceneCellLists)
			{
				this.m_SerializedPerSceneCellList.Add(new ProbeVolumeBakingSet.SerializedPerSceneCellList
				{
					sceneGUID = keyValuePair.Key,
					cellList = keyValuePair.Value
				});
			}
		}

		internal void Initialize(bool useStreamingAsset)
		{
			foreach (KeyValuePair<string, ProbeVolumeBakingSet.PerScenarioDataInfo> keyValuePair in this.scenarios)
			{
				keyValuePair.Value.Initialize(ProbeReferenceVolume.instance.shBands);
			}
			if (!useStreamingAsset)
			{
				this.m_UseStreamingAsset = false;
				this.m_TotalIndexList.Clear();
				foreach (int item in this.cellDescs.Keys)
				{
					this.m_TotalIndexList.Add(item);
				}
				this.ResolveAllCellData();
			}
			if (ProbeReferenceVolume.instance.supportScenarioBlending)
			{
				this.BlendLightingScenario(null, 0f);
			}
		}

		internal void Cleanup()
		{
			if (this.cellSharedDataAsset != null)
			{
				this.cellSharedDataAsset.Dispose();
				foreach (KeyValuePair<string, ProbeVolumeBakingSet.PerScenarioDataInfo> keyValuePair in this.scenarios)
				{
					if (keyValuePair.Value.IsValid())
					{
						keyValuePair.Value.cellDataAsset.Dispose();
						keyValuePair.Value.cellOptionalDataAsset.Dispose();
						keyValuePair.Value.cellProbeOcclusionDataAsset.Dispose();
					}
				}
			}
			if (this.m_ReadCommandBuffer.IsCreated)
			{
				this.m_ReadCommandBuffer.Dispose();
			}
			foreach (NativeArray<byte> nativeArray in this.m_ReadOperationScratchBuffers)
			{
				nativeArray.Dispose();
			}
			this.m_ReadOperationScratchBuffers.Clear();
		}

		internal void SetActiveScenario(string scenario, bool verbose = true)
		{
			if (this.lightingScenario == scenario)
			{
				return;
			}
			if (!this.m_LightingScenarios.Contains(scenario))
			{
				if (verbose)
				{
					Debug.LogError("Scenario '" + scenario + "' does not exist.");
				}
				return;
			}
			if (!this.scenarios.ContainsKey(scenario) && verbose)
			{
				Debug.LogError("Scenario '" + scenario + "' has not been baked.");
			}
			this.lightingScenario = scenario;
			this.m_ScenarioBlendingFactor = 0f;
			if (ProbeReferenceVolume.instance.supportScenarioBlending)
			{
				ProbeReferenceVolume.instance.ScenarioBlendingChanged(true);
				return;
			}
			ProbeReferenceVolume.instance.UnloadAllCells();
		}

		internal void BlendLightingScenario(string otherScenario, float blendingFactor)
		{
			if (!string.IsNullOrEmpty(otherScenario) && !ProbeReferenceVolume.instance.supportScenarioBlending)
			{
				return;
			}
			if (otherScenario != null && !this.m_LightingScenarios.Contains(otherScenario))
			{
				Debug.LogError("Scenario '" + otherScenario + "' does not exist.");
				return;
			}
			if (otherScenario != null && !this.scenarios.ContainsKey(otherScenario))
			{
				Debug.LogError("Scenario '" + otherScenario + "' has not been baked.");
				return;
			}
			blendingFactor = Mathf.Clamp01(blendingFactor);
			if (otherScenario == this.lightingScenario || string.IsNullOrEmpty(otherScenario))
			{
				otherScenario = null;
			}
			if (otherScenario == null)
			{
				blendingFactor = 0f;
			}
			if (otherScenario == this.m_OtherScenario && Mathf.Approximately(blendingFactor, this.m_ScenarioBlendingFactor))
			{
				return;
			}
			bool scenarioChanged = otherScenario != this.m_OtherScenario;
			this.m_OtherScenario = otherScenario;
			this.m_ScenarioBlendingFactor = blendingFactor;
			ProbeReferenceVolume.instance.ScenarioBlendingChanged(scenarioChanged);
		}

		internal int GetBakingHashCode()
		{
			return ((((this.maxCellPosition.GetHashCode() * 23 + this.minCellPosition.GetHashCode()) * 23 + this.globalBounds.GetHashCode()) * 23 + this.cellSizeInBricks.GetHashCode()) * 23 + this.simplificationLevels.GetHashCode()) * 23 + this.minDistanceBetweenProbes.GetHashCode();
		}

		private static int AlignUp16(int count)
		{
			int num = 16;
			int num2 = count % num;
			return count + ((num2 == 0) ? 0 : (num - num2));
		}

		private NativeArray<T> GetSubArray<T>(NativeArray<byte> input, int count, ref int offset) where T : struct
		{
			int num = count * UnsafeUtility.SizeOf<T>();
			if (offset + num > input.Length)
			{
				return default(NativeArray<T>);
			}
			NativeArray<T> result = input.GetSubArray(offset, num).Reinterpret<T>(1);
			offset = ProbeVolumeBakingSet.AlignUp16(offset + num);
			return result;
		}

		private NativeArray<byte> RequestScratchBuffer(int size)
		{
			if (this.m_ReadOperationScratchBuffers.Count == 0)
			{
				return new NativeArray<byte>(size, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			}
			NativeArray<byte> result = this.m_ReadOperationScratchBuffers.Pop();
			if (result.Length < size)
			{
				result.Dispose();
				return new NativeArray<byte>(size, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			}
			return result;
		}

		private unsafe bool FileExists(string path)
		{
			FileInfoResult fileInfoResult;
			AsyncReadManager.GetFileInfo(path, &fileInfoResult).JobHandle.Complete();
			return fileInfoResult.FileState == FileState.Exists;
		}

		private unsafe NativeArray<T> LoadStreambleAssetData<T>(ProbeVolumeStreamableAsset asset, List<int> cellIndices) where T : struct
		{
			if (!this.m_UseStreamingAsset)
			{
				return asset.asset.GetData<byte>().Reinterpret<T>(1);
			}
			if (!this.FileExists(asset.GetAssetPath()))
			{
				asset.RefreshAssetPath();
				if (!this.FileExists(asset.GetAssetPath()))
				{
					if (asset.HasValidAssetReference())
					{
						return asset.asset.GetData<byte>().Reinterpret<T>(1);
					}
					return default(NativeArray<T>);
				}
			}
			if (!this.m_ReadCommandBuffer.IsCreated || this.m_ReadCommandBuffer.Length < cellIndices.Count)
			{
				if (this.m_ReadCommandBuffer.IsCreated)
				{
					this.m_ReadCommandBuffer.Dispose();
				}
				this.m_ReadCommandBuffer = new NativeArray<ReadCommand>(cellIndices.Count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			}
			int num = 0;
			int index = 0;
			foreach (int key in cellIndices)
			{
				ProbeReferenceVolume.CellDesc cellDesc = this.cellDescs[key];
				ProbeVolumeStreamableAsset.StreamableCellDesc streamableCellDesc = asset.streamableCellDescs[key];
				ReadCommand readCommand = default(ReadCommand);
				readCommand.Offset = (long)streamableCellDesc.offset;
				readCommand.Size = (long)(streamableCellDesc.elementCount * asset.elementSize);
				readCommand.Buffer = null;
				this.m_ReadCommandBuffer[index++] = readCommand;
				num += (int)readCommand.Size;
			}
			NativeArray<byte> nativeArray = this.RequestScratchBuffer(num);
			index = 0;
			long num2 = 0L;
			byte* unsafePtr = (byte*)nativeArray.GetUnsafePtr<byte>();
			foreach (int num3 in cellIndices)
			{
				ReadCommand readCommand2 = this.m_ReadCommandBuffer[index];
				readCommand2.Buffer = (void*)(unsafePtr + num2);
				num2 += readCommand2.Size;
				this.m_ReadCommandBuffer[index++] = readCommand2;
			}
			this.m_ReadCommandArray.CommandCount = cellIndices.Count;
			this.m_ReadCommandArray.ReadCommands = (ReadCommand*)this.m_ReadCommandBuffer.GetUnsafePtr<ReadCommand>();
			FileHandle fileHandle = asset.OpenFile();
			ReadHandle readHandle = AsyncReadManager.Read(fileHandle, this.m_ReadCommandArray);
			readHandle.JobHandle.Complete();
			asset.CloseFile();
			readHandle.Dispose();
			return nativeArray.Reinterpret<T>(1);
		}

		private void ReleaseStreamableAssetData<T>(NativeArray<T> buffer) where T : struct
		{
			if (this.m_UseStreamingAsset)
			{
				this.m_ReadOperationScratchBuffers.Push(buffer.Reinterpret<byte>(UnsafeUtility.SizeOf<T>()));
			}
		}

		private void PruneCellIndexList(List<int> cellIndices, List<int> prunedIndexList)
		{
			prunedIndexList.Clear();
			foreach (int num in cellIndices)
			{
				if (!this.cellDataMap.ContainsKey(num))
				{
					prunedIndexList.Add(num);
				}
			}
		}

		private void PruneCellIndexListForScenario(List<int> cellIndices, ProbeVolumeBakingSet.PerScenarioDataInfo scenarioData, List<int> prunedIndexList)
		{
			prunedIndexList.Clear();
			foreach (int num in cellIndices)
			{
				if (scenarioData.cellDataAsset.streamableCellDescs.ContainsKey(num))
				{
					prunedIndexList.Add(num);
				}
			}
		}

		internal List<int> GetSceneCellIndexList(string sceneGUID)
		{
			List<int> result;
			if (this.perSceneCellLists.TryGetValue(sceneGUID, out result))
			{
				return result;
			}
			return null;
		}

		private bool ResolveAllCellData()
		{
			return this.ResolveSharedCellData(this.m_TotalIndexList) && this.ResolvePerScenarioCellData(this.m_TotalIndexList);
		}

		internal bool ResolveCellData(List<int> cellIndices)
		{
			if (!this.m_UseStreamingAsset)
			{
				return true;
			}
			if (cellIndices == null)
			{
				return false;
			}
			this.PruneCellIndexList(cellIndices, this.m_PrunedIndexList);
			if (ProbeReferenceVolume.instance.diskStreamingEnabled)
			{
				foreach (int key in this.m_PrunedIndexList)
				{
					ProbeReferenceVolume.CellData cellData = new ProbeReferenceVolume.CellData();
					foreach (KeyValuePair<string, ProbeVolumeBakingSet.PerScenarioDataInfo> keyValuePair in this.scenarios)
					{
						cellData.scenarios.Add(keyValuePair.Key, default(ProbeReferenceVolume.CellData.PerScenarioData));
					}
					this.cellDataMap.Add(key, cellData);
				}
				return true;
			}
			return this.ResolveSharedCellData(this.m_PrunedIndexList) && this.ResolvePerScenarioCellData(this.m_PrunedIndexList);
		}

		private void ResolveSharedCellData(List<int> cellIndices, NativeArray<ProbeBrickIndex.Brick> bricksData, NativeArray<byte> cellSharedData, NativeArray<byte> cellSupportData)
		{
			ProbeReferenceVolume instance = ProbeReferenceVolume.instance;
			bool flag = cellSupportData.Length != 0;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < cellIndices.Count; i++)
			{
				int key = cellIndices[i];
				ProbeReferenceVolume.CellData cellData = new ProbeReferenceVolume.CellData();
				ProbeReferenceVolume.CellDesc cellDesc = this.cellDescs[key];
				int bricksCount = cellDesc.bricksCount;
				int shChunkCount = cellDesc.shChunkCount;
				NativeArray<ProbeBrickIndex.Brick> subArray = bricksData.GetSubArray(num3, bricksCount);
				NativeArray<byte> subArray2 = cellSharedData.GetSubArray(num, this.sharedValidityMaskChunkSize * shChunkCount);
				num += this.sharedValidityMaskChunkSize * shChunkCount;
				cellData.bricks = (this.m_UseStreamingAsset ? new NativeArray<ProbeBrickIndex.Brick>(subArray, Allocator.Persistent) : subArray);
				cellData.validityNeighMaskData = (this.m_UseStreamingAsset ? new NativeArray<byte>(subArray2, Allocator.Persistent) : subArray2);
				if (this.bakedSkyOcclusion)
				{
					if (instance.skyOcclusion)
					{
						NativeArray<ushort> nativeArray = cellSharedData.GetSubArray(num, this.sharedSkyOcclusionL0L1ChunkSize * shChunkCount).Reinterpret<ushort>(1);
						cellData.skyOcclusionDataL0L1 = (this.m_UseStreamingAsset ? new NativeArray<ushort>(nativeArray, Allocator.Persistent) : nativeArray);
					}
					num += this.sharedSkyOcclusionL0L1ChunkSize * shChunkCount;
					if (this.bakedSkyShadingDirection)
					{
						if (instance.skyOcclusion && instance.skyOcclusionShadingDirection)
						{
							NativeArray<byte> subArray3 = cellSharedData.GetSubArray(num, this.sharedSkyShadingDirectionIndicesChunkSize * shChunkCount);
							cellData.skyShadingDirectionIndices = (this.m_UseStreamingAsset ? new NativeArray<byte>(subArray3, Allocator.Persistent) : subArray3);
						}
						num += this.sharedSkyShadingDirectionIndicesChunkSize * shChunkCount;
					}
				}
				if (flag)
				{
					NativeArray<Vector3> nativeArray2 = cellSupportData.GetSubArray(num2, shChunkCount * this.supportPositionChunkSize).Reinterpret<Vector3>(1);
					num2 += shChunkCount * this.supportPositionChunkSize;
					cellData.probePositions = (this.m_UseStreamingAsset ? new NativeArray<Vector3>(nativeArray2, Allocator.Persistent) : nativeArray2);
					NativeArray<float> nativeArray3 = cellSupportData.GetSubArray(num2, shChunkCount * this.supportValidityChunkSize).Reinterpret<float>(1);
					num2 += shChunkCount * this.supportValidityChunkSize;
					cellData.validity = (this.m_UseStreamingAsset ? new NativeArray<float>(nativeArray3, Allocator.Persistent) : nativeArray3);
					NativeArray<float> nativeArray4 = cellSupportData.GetSubArray(num2, shChunkCount * this.supportTouchupChunkSize).Reinterpret<float>(1);
					num2 += shChunkCount * this.supportTouchupChunkSize;
					cellData.touchupVolumeInteraction = (this.m_UseStreamingAsset ? new NativeArray<float>(nativeArray4, Allocator.Persistent) : nativeArray4);
					if (this.supportLayerMaskChunkSize != 0)
					{
						NativeArray<byte> nativeArray5 = cellSupportData.GetSubArray(num2, shChunkCount * this.supportLayerMaskChunkSize).Reinterpret<byte>(1);
						num2 += shChunkCount * this.supportLayerMaskChunkSize;
						cellData.layer = (this.m_UseStreamingAsset ? new NativeArray<byte>(nativeArray5, Allocator.Persistent) : nativeArray5);
					}
					if (this.supportOffsetsChunkSize != 0)
					{
						NativeArray<Vector3> nativeArray6 = cellSupportData.GetSubArray(num2, shChunkCount * this.supportOffsetsChunkSize).Reinterpret<Vector3>(1);
						num2 += shChunkCount * this.supportOffsetsChunkSize;
						cellData.offsetVectors = (this.m_UseStreamingAsset ? new NativeArray<Vector3>(nativeArray6, Allocator.Persistent) : nativeArray6);
					}
				}
				this.cellDataMap.Add(key, cellData);
				num3 += bricksCount;
				num4 += shChunkCount;
			}
		}

		internal bool ResolveSharedCellData(List<int> cellIndices)
		{
			if (this.cellSharedDataAsset == null || !this.cellSharedDataAsset.IsValid())
			{
				return false;
			}
			if (!this.HasValidSharedData())
			{
				Debug.LogError("One or more data file missing for baking set " + base.name + ". Cannot load shared data.");
				return false;
			}
			NativeArray<byte> nativeArray = this.LoadStreambleAssetData<byte>(this.cellSharedDataAsset, cellIndices);
			NativeArray<ProbeBrickIndex.Brick> nativeArray2 = this.LoadStreambleAssetData<ProbeBrickIndex.Brick>(this.cellBricksDataAsset, cellIndices);
			bool flag = this.HasSupportData();
			NativeArray<byte> nativeArray3 = flag ? this.LoadStreambleAssetData<byte>(this.cellSupportDataAsset, cellIndices) : default(NativeArray<byte>);
			this.ResolveSharedCellData(cellIndices, nativeArray2, nativeArray, nativeArray3);
			this.ReleaseStreamableAssetData<byte>(nativeArray);
			this.ReleaseStreamableAssetData<ProbeBrickIndex.Brick>(nativeArray2);
			if (flag)
			{
				this.ReleaseStreamableAssetData<byte>(nativeArray3);
			}
			return true;
		}

		internal bool ResolvePerScenarioCellData(List<int> cellIndices)
		{
			bool flag = ProbeReferenceVolume.instance.shBands == ProbeVolumeSHBands.SphericalHarmonicsL2;
			foreach (KeyValuePair<string, ProbeVolumeBakingSet.PerScenarioDataInfo> keyValuePair in this.scenarios)
			{
				string key = keyValuePair.Key;
				ProbeVolumeBakingSet.PerScenarioDataInfo value = keyValuePair.Value;
				this.PruneCellIndexListForScenario(cellIndices, value, this.m_PrunedScenarioIndexList);
				if (!value.HasValidData(ProbeReferenceVolume.instance.shBands))
				{
					Debug.LogError(string.Concat(new string[]
					{
						"One or more data file missing for baking set ",
						key,
						" scenario ",
						this.lightingScenario,
						". Cannot load scenario data."
					}));
					return false;
				}
				NativeArray<byte> nativeArray = this.LoadStreambleAssetData<byte>(value.cellDataAsset, this.m_PrunedScenarioIndexList);
				NativeArray<byte> nativeArray2 = flag ? this.LoadStreambleAssetData<byte>(value.cellOptionalDataAsset, this.m_PrunedScenarioIndexList) : default(NativeArray<byte>);
				NativeArray<byte> nativeArray3 = this.bakedProbeOcclusion ? this.LoadStreambleAssetData<byte>(value.cellProbeOcclusionDataAsset, this.m_PrunedScenarioIndexList) : default(NativeArray<byte>);
				if (!this.ResolvePerScenarioCellData(nativeArray, nativeArray2, nativeArray3, key, this.m_PrunedScenarioIndexList))
				{
					Debug.LogError("Baked data for scenario '" + key + "' cannot be loaded.");
					return false;
				}
				this.ReleaseStreamableAssetData<byte>(nativeArray);
				if (flag)
				{
					this.ReleaseStreamableAssetData<byte>(nativeArray2);
				}
				if (this.bakedProbeOcclusion)
				{
					this.ReleaseStreamableAssetData<byte>(nativeArray3);
				}
			}
			return true;
		}

		internal bool ResolvePerScenarioCellData(NativeArray<byte> cellData, NativeArray<byte> cellOptionalData, NativeArray<byte> cellProbeOcclusionData, string scenario, List<int> cellIndices)
		{
			if (!cellData.IsCreated)
			{
				return false;
			}
			bool isCreated = cellOptionalData.IsCreated;
			bool flag = cellProbeOcclusionData.IsCreated && cellProbeOcclusionData.Length > 0;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < cellIndices.Count; i++)
			{
				int key = cellIndices[i];
				ProbeReferenceVolume.CellData cellData2 = this.cellDataMap[key];
				ProbeReferenceVolume.CellDesc cellDesc = this.cellDescs[key];
				ProbeReferenceVolume.CellData.PerScenarioData value = default(ProbeReferenceVolume.CellData.PerScenarioData);
				int shChunkCount = cellDesc.shChunkCount;
				NativeArray<ushort> nativeArray = cellData.GetSubArray(num, this.L0ChunkSize * shChunkCount).Reinterpret<ushort>(1);
				NativeArray<byte> subArray = cellData.GetSubArray(num + this.L0ChunkSize * shChunkCount, this.L1ChunkSize * shChunkCount);
				NativeArray<byte> subArray2 = cellData.GetSubArray(num + (this.L0ChunkSize + this.L1ChunkSize) * shChunkCount, this.L1ChunkSize * shChunkCount);
				value.shL0L1RxData = (this.m_UseStreamingAsset ? new NativeArray<ushort>(nativeArray, Allocator.Persistent) : nativeArray);
				value.shL1GL1RyData = (this.m_UseStreamingAsset ? new NativeArray<byte>(subArray, Allocator.Persistent) : subArray);
				value.shL1BL1RzData = (this.m_UseStreamingAsset ? new NativeArray<byte>(subArray2, Allocator.Persistent) : subArray2);
				if (isCreated)
				{
					int num4 = shChunkCount * this.L2TextureChunkSize;
					NativeArray<byte> subArray3 = cellOptionalData.GetSubArray(num2, num4);
					NativeArray<byte> subArray4 = cellOptionalData.GetSubArray(num2 + num4, num4);
					NativeArray<byte> subArray5 = cellOptionalData.GetSubArray(num2 + num4 * 2, num4);
					NativeArray<byte> subArray6 = cellOptionalData.GetSubArray(num2 + num4 * 3, num4);
					value.shL2Data_0 = (this.m_UseStreamingAsset ? new NativeArray<byte>(subArray3, Allocator.Persistent) : subArray3);
					value.shL2Data_1 = (this.m_UseStreamingAsset ? new NativeArray<byte>(subArray4, Allocator.Persistent) : subArray4);
					value.shL2Data_2 = (this.m_UseStreamingAsset ? new NativeArray<byte>(subArray5, Allocator.Persistent) : subArray5);
					value.shL2Data_3 = (this.m_UseStreamingAsset ? new NativeArray<byte>(subArray6, Allocator.Persistent) : subArray6);
				}
				if (flag)
				{
					NativeArray<byte> subArray7 = cellProbeOcclusionData.GetSubArray(num3, this.ProbeOcclusionChunkSize * shChunkCount);
					value.probeOcclusion = (this.m_UseStreamingAsset ? new NativeArray<byte>(subArray7, Allocator.Persistent) : subArray7);
				}
				num += (this.L0ChunkSize + 2 * this.L1ChunkSize) * shChunkCount;
				num2 += this.L2TextureChunkSize * 4 * shChunkCount;
				num3 += this.ProbeOcclusionChunkSize * shChunkCount;
				cellData2.scenarios.Add(scenario, value);
			}
			return true;
		}

		internal void ReleaseCell(int cellIndex)
		{
			this.cellDataMap[cellIndex].Cleanup(true);
			this.cellDataMap.Remove(cellIndex);
		}

		internal ProbeReferenceVolume.CellDesc GetCellDesc(int cellIndex)
		{
			ProbeReferenceVolume.CellDesc result;
			if (this.cellDescs.TryGetValue(cellIndex, out result))
			{
				return result;
			}
			return null;
		}

		internal ProbeReferenceVolume.CellData GetCellData(int cellIndex)
		{
			ProbeReferenceVolume.CellData result;
			if (this.cellDataMap.TryGetValue(cellIndex, out result))
			{
				return result;
			}
			return null;
		}

		internal int GetChunkGPUMemory(ProbeVolumeSHBands shBands)
		{
			int num = this.L0ChunkSize + 2 * this.L1ChunkSize + this.sharedDataChunkSize;
			if (shBands == ProbeVolumeSHBands.SphericalHarmonicsL2)
			{
				num += 4 * this.L2TextureChunkSize;
			}
			if (this.bakedProbeOcclusion)
			{
				num += this.ProbeOcclusionChunkSize;
			}
			return num;
		}

		internal bool HasSameSceneGUIDs(ProbeVolumeBakingSet other)
		{
			IReadOnlyList<string> sceneGUIDs = other.sceneGUIDs;
			if (this.m_SceneGUIDs.Count != sceneGUIDs.Count)
			{
				return false;
			}
			for (int i = 0; i < this.m_SceneGUIDs.Count; i++)
			{
				if (this.m_SceneGUIDs[i] != sceneGUIDs[i])
				{
					return false;
				}
			}
			return true;
		}

		[SerializeField]
		internal bool singleSceneMode = true;

		[SerializeField]
		internal bool dialogNoProbeVolumeInSetShown;

		[SerializeField]
		internal ProbeVolumeBakingProcessSettings settings;

		[SerializeField]
		private List<string> m_SceneGUIDs = new List<string>();

		[SerializeField]
		[Obsolete("This is now contained in the SceneBakeData structure")]
		[FormerlySerializedAs("scenesToNotBake")]
		internal List<string> obsoleteScenesToNotBake = new List<string>();

		[SerializeField]
		[FormerlySerializedAs("lightingScenarios")]
		internal List<string> m_LightingScenarios = new List<string>();

		[SerializeField]
		internal SerializedDictionary<int, ProbeReferenceVolume.CellDesc> cellDescs = new SerializedDictionary<int, ProbeReferenceVolume.CellDesc>();

		internal Dictionary<int, ProbeReferenceVolume.CellData> cellDataMap = new Dictionary<int, ProbeReferenceVolume.CellData>();

		private List<int> m_TotalIndexList = new List<int>();

		[SerializeField]
		private List<ProbeVolumeBakingSet.SerializedPerSceneCellList> m_SerializedPerSceneCellList;

		internal Dictionary<string, List<int>> perSceneCellLists = new Dictionary<string, List<int>>();

		[SerializeField]
		internal ProbeVolumeStreamableAsset cellSharedDataAsset;

		[SerializeField]
		internal SerializedDictionary<string, ProbeVolumeBakingSet.PerScenarioDataInfo> scenarios = new SerializedDictionary<string, ProbeVolumeBakingSet.PerScenarioDataInfo>();

		[SerializeField]
		internal ProbeVolumeStreamableAsset cellBricksDataAsset;

		[SerializeField]
		internal ProbeVolumeStreamableAsset cellSupportDataAsset;

		[SerializeField]
		internal int chunkSizeInBricks;

		[SerializeField]
		internal Vector3Int maxCellPosition;

		[SerializeField]
		internal Vector3Int minCellPosition;

		[SerializeField]
		internal Bounds globalBounds;

		[SerializeField]
		internal int bakedSimplificationLevels = -1;

		[SerializeField]
		internal float bakedMinDistanceBetweenProbes = -1f;

		[SerializeField]
		internal bool bakedProbeOcclusion;

		[SerializeField]
		internal int bakedSkyOcclusionValue = -1;

		[SerializeField]
		internal int bakedSkyShadingDirectionValue = -1;

		[SerializeField]
		internal Vector3 bakedProbeOffset = Vector3.zero;

		[SerializeField]
		internal int bakedMaskCount = 1;

		[SerializeField]
		internal uint4 bakedLayerMasks;

		[SerializeField]
		internal int maxSHChunkCount = -1;

		[SerializeField]
		internal int L0ChunkSize;

		[SerializeField]
		internal int L1ChunkSize;

		[SerializeField]
		internal int L2TextureChunkSize;

		[SerializeField]
		internal int ProbeOcclusionChunkSize;

		[SerializeField]
		internal int sharedValidityMaskChunkSize;

		[SerializeField]
		internal int sharedSkyOcclusionL0L1ChunkSize;

		[SerializeField]
		internal int sharedSkyShadingDirectionIndicesChunkSize;

		[SerializeField]
		internal int sharedDataChunkSize;

		[SerializeField]
		internal int supportPositionChunkSize;

		[SerializeField]
		internal int supportValidityChunkSize;

		[SerializeField]
		internal int supportTouchupChunkSize;

		[SerializeField]
		internal int supportLayerMaskChunkSize;

		[SerializeField]
		internal int supportOffsetsChunkSize;

		[SerializeField]
		internal int supportDataChunkSize;

		[SerializeField]
		internal string lightingScenario = ProbeReferenceVolume.defaultLightingScenario;

		private string m_OtherScenario;

		private float m_ScenarioBlendingFactor;

		private ReadCommandArray m_ReadCommandArray;

		private NativeArray<ReadCommand> m_ReadCommandBuffer;

		private Stack<NativeArray<byte>> m_ReadOperationScratchBuffers = new Stack<NativeArray<byte>>();

		private List<int> m_PrunedIndexList = new List<int>();

		private List<int> m_PrunedScenarioIndexList = new List<int>();

		internal const int k_MaxSkyOcclusionBakingSamples = 8192;

		[SerializeField]
		private ProbeVolumeBakingSet.Version version = CoreUtils.GetLastEnumValue<ProbeVolumeBakingSet.Version>();

		[SerializeField]
		internal bool freezePlacement;

		[SerializeField]
		public Vector3 probeOffset = Vector3.zero;

		[Range(2f, 5f)]
		public int simplificationLevels = 3;

		[Min(0.1f)]
		public float minDistanceBetweenProbes = 1f;

		public LayerMask renderersLayerMask = -1;

		[Min(0f)]
		public float minRendererVolumeSize = 0.1f;

		public bool skyOcclusion;

		[Logarithmic(1, 8192)]
		public int skyOcclusionBakingSamples = 2048;

		[Range(0f, 5f)]
		public int skyOcclusionBakingBounces = 2;

		[Range(0f, 1f)]
		public float skyOcclusionAverageAlbedo = 0.6f;

		public bool skyOcclusionBackFaceCulling;

		public bool skyOcclusionShadingDirection;

		[SerializeField]
		internal bool useRenderingLayers;

		[SerializeField]
		internal ProbeVolumeBakingSet.ProbeLayerMask[] renderingLayerMasks;

		private bool m_HasSupportData;

		private bool m_SharedDataIsValid;

		private bool m_UseStreamingAsset = true;

		internal enum Version
		{
			Initial,
			RemoveProbeVolumeSceneData,
			AssetsAlwaysReferenced
		}

		[Serializable]
		internal class PerScenarioDataInfo
		{
			public void Initialize(ProbeVolumeSHBands shBands)
			{
				this.m_HasValidData = this.ComputeHasValidData(shBands);
			}

			public bool IsValid()
			{
				return this.cellDataAsset != null && this.cellDataAsset.IsValid();
			}

			public bool HasValidData(ProbeVolumeSHBands shBands)
			{
				return this.m_HasValidData;
			}

			public bool ComputeHasValidData(ProbeVolumeSHBands shBands)
			{
				return this.cellDataAsset.FileExists() && (shBands == ProbeVolumeSHBands.SphericalHarmonicsL1 || this.cellOptionalDataAsset.FileExists());
			}

			public int sceneHash;

			public ProbeVolumeStreamableAsset cellDataAsset;

			public ProbeVolumeStreamableAsset cellOptionalDataAsset;

			public ProbeVolumeStreamableAsset cellProbeOcclusionDataAsset;

			private bool m_HasValidData;
		}

		[Serializable]
		internal struct CellCounts
		{
			public void Add(ProbeVolumeBakingSet.CellCounts o)
			{
				this.bricksCount += o.bricksCount;
				this.chunksCount += o.chunksCount;
			}

			public int bricksCount;

			public int chunksCount;
		}

		[Serializable]
		private struct SerializedPerSceneCellList
		{
			public string sceneGUID;

			public List<int> cellList;
		}

		[Serializable]
		internal struct ProbeLayerMask
		{
			public RenderingLayerMask mask;

			public string name;
		}
	}
}
