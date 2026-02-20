using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Cysharp.Text;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaNetworking.Store;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CosmeticsV2Spawner_Dirty : IDelayedExecListener
{
	public static bool completed { get; private set; }

	void IDelayedExecListener.OnDelayedAction(int contextId)
	{
		if (contextId >= 0 && contextId < 1000000)
		{
			CosmeticsV2Spawner_Dirty._RetryDownload(contextId);
			return;
		}
		if (contextId == -Mathf.Abs("_Step5_InitializeVRRigsAndCosmeticsControllerFinalize".GetHashCode()))
		{
			CosmeticsV2Spawner_Dirty._Step5_InitializeVRRigsAndCosmeticsControllerFinalize();
		}
	}

	public static void StartInstantiatingPrefabs()
	{
		if (CosmeticsV2Spawner_Dirty.completed)
		{
			return;
		}
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (CosmeticsV2Spawner_Dirty._instance == null)
		{
			CosmeticsV2Spawner_Dirty._instance = new CosmeticsV2Spawner_Dirty();
		}
		CosmeticsV2Spawner_Dirty.k_stopwatch.Restart();
		CosmeticsV2Spawner_Dirty.g_gorillaPlayer = Object.FindAnyObjectByType<GTPlayer>();
		foreach (SnowballMaker snowballMaker in CosmeticsV2Spawner_Dirty.g_gorillaPlayer.GetComponentsInChildren<SnowballMaker>(true))
		{
			if (snowballMaker.isLeftHand)
			{
				CosmeticsV2Spawner_Dirty._gSnowballMakerLeft = snowballMaker;
			}
			else
			{
				CosmeticsV2Spawner_Dirty._gSnowballMakerRight = snowballMaker;
			}
		}
		if (!CosmeticsController.hasInstance)
		{
			Debug.LogError("(should never happen) cannot instantiate prefabs before cosmetics controller instance is available.");
			return;
		}
		if (!CosmeticsController.instance.v2_allCosmeticsInfoAssetRef.IsValid())
		{
			Debug.LogError("(should never happen) cannot load prefabs before v2_allCosmeticsInfoAssetRef is loaded.");
			return;
		}
		AllCosmeticsArraySO allCosmeticsArraySO = CosmeticsController.instance.v2_allCosmeticsInfoAssetRef.Asset as AllCosmeticsArraySO;
		if (allCosmeticsArraySO == null)
		{
			Debug.LogError("(should never happen) v2_allCosmeticsInfoAssetRef is valid but null.");
			return;
		}
		Transform[] boneXforms;
		string str;
		if (!GTHardCodedBones.TryGetBoneXforms(VRRig.LocalRig, out boneXforms, out str))
		{
			Debug.LogError("CosmeticsV2Spawner_Dirty: Error getting bone Transforms from local VRRig: " + str, VRRig.LocalRig);
			return;
		}
		CosmeticsV2Spawner_Dirty._gVRRigDatas.Add(new CosmeticsV2Spawner_Dirty.VRRigData(VRRig.LocalRig, boneXforms));
		CosmeticsV2Spawner_Dirty._gVRRigDatasIndexByRig[VRRig.LocalRig] = 0;
		int vrRigIndex = 0;
		foreach (VRRig vrrig in VRRigCache.Instance.GetAllRigs())
		{
			Transform[] boneXforms2;
			if (!GTHardCodedBones.TryGetBoneXforms(vrrig, out boneXforms2, out str))
			{
				Debug.LogError("CosmeticsV2Spawner_Dirty: Error getting bone Transforms from cached VRRig: " + str, VRRig.LocalRig);
				return;
			}
			CosmeticsV2Spawner_Dirty._gVRRigDatasIndexByRig[vrrig] = CosmeticsV2Spawner_Dirty._gVRRigDatas.Count;
			CosmeticsV2Spawner_Dirty._gVRRigDatas.Add(new CosmeticsV2Spawner_Dirty.VRRigData(vrrig, boneXforms2));
		}
		CosmeticsV2Spawner_Dirty._gDeactivatedSpawnParent = GlobalDeactivatedSpawnRoot.GetOrCreate();
		GTDelayedExec.Add(CosmeticsV2Spawner_Dirty._instance, 2f, -100);
		CosmeticsV2Spawner_Dirty.materialIndexToSnowballThrowablePlayfabIdStringLeft = new Dictionary<int, string>();
		CosmeticsV2Spawner_Dirty.materialIndexToSnowballThrowablePlayfabIdStringRight = new Dictionary<int, string>();
		CosmeticsV2Spawner_Dirty.throwableIndexPlayfabIdStringLeft = new Dictionary<int, string>();
		CosmeticsV2Spawner_Dirty.throwableIndexPlayfabIdStringRight = new Dictionary<int, string>();
		CosmeticsV2Spawner_Dirty._g_loadOpInfosForRigAndCosmeticIDDicts = new Dictionary<string, List<CosmeticsV2Spawner_Dirty.LoadOpInfo>>[20];
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		foreach (GTDirectAssetRef<CosmeticSO> gtdirectAssetRef in allCosmeticsArraySO.sturdyAssetRefs)
		{
			CosmeticInfoV2 info = gtdirectAssetRef.obj.info;
			if (info.hasHoldableParts)
			{
				for (int k = 0; k < CosmeticsV2Spawner_Dirty._gVRRigDatas.Count; k++)
				{
					for (int l = 0; l < info.holdableParts.Length; l++)
					{
						CosmeticPart cosmeticPart = info.holdableParts[l];
						if (!cosmeticPart.prefabAssetRef.RuntimeKeyIsValid())
						{
							if (k == 0)
							{
								GTDev.LogError<string>("Cosmetic " + info.displayName + " has missing object reference in wearable parts, skipping load", null);
							}
						}
						else
						{
							CosmeticsV2Spawner_Dirty.AddEachAttachInfoToLoadOpInfosList(cosmeticPart, l, info, k, ref num);
						}
					}
				}
			}
			if (info.hasFunctionalParts)
			{
				for (int m = 0; m < CosmeticsV2Spawner_Dirty._gVRRigDatas.Count; m++)
				{
					for (int n = 0; n < info.functionalParts.Length; n++)
					{
						CosmeticPart cosmeticPart2 = info.functionalParts[n];
						if (!cosmeticPart2.prefabAssetRef.RuntimeKeyIsValid())
						{
							if (m == 0)
							{
								GTDev.LogError<string>("Cosmetic " + info.displayName + " has missing object reference in functional parts, skipping load", null);
							}
						}
						else
						{
							CosmeticsV2Spawner_Dirty.AddEachAttachInfoToLoadOpInfosList(cosmeticPart2, n, info, m, ref num);
						}
					}
				}
			}
			if (info.hasFirstPersonViewParts)
			{
				for (int num4 = 0; num4 < info.firstPersonViewParts.Length; num4++)
				{
					CosmeticPart cosmeticPart3 = info.firstPersonViewParts[num4];
					if (!cosmeticPart3.prefabAssetRef.RuntimeKeyIsValid())
					{
						GTDev.LogError<string>("Cosmetic " + info.displayName + " has missing object reference in first person parts, skipping load", null);
					}
					else
					{
						CosmeticsV2Spawner_Dirty.AddEachAttachInfoToLoadOpInfosList(cosmeticPart3, num4, info, vrRigIndex, ref num2);
					}
				}
			}
			if (info.hasLocalRigParts)
			{
				for (int num5 = 0; num5 < info.localRigParts.Length; num5++)
				{
					CosmeticPart cosmeticPart4 = info.localRigParts[num5];
					if (!cosmeticPart4.prefabAssetRef.RuntimeKeyIsValid())
					{
						GTDev.LogError<string>("Cosmetic " + info.displayName + " has missing object reference in local rig parts, skipping load", null);
					}
					else
					{
						CosmeticsV2Spawner_Dirty.AddEachAttachInfoToLoadOpInfosList(cosmeticPart4, num5, info, vrRigIndex, ref num3);
					}
				}
			}
		}
		CosmeticsV2Spawner_Dirty._Step4_PopulateAllArrays();
	}

	private static void AddEachAttachInfoToLoadOpInfosList(CosmeticPart part, int partIndex, CosmeticInfoV2 cosmeticInfo, int vrRigIndex, ref int partCount)
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		for (int i = 0; i < part.attachAnchors.Length; i++)
		{
			CosmeticsV2Spawner_Dirty.LoadOpInfo loadOpInfo = new CosmeticsV2Spawner_Dirty.LoadOpInfo(part.attachAnchors[i], part, partIndex, cosmeticInfo, vrRigIndex);
			if (cosmeticInfo.isThrowable)
			{
				if (GTHardCodedBones.GetHandednessFromBone(loadOpInfo.attachInfo.parentBone) == EHandedness.Right)
				{
					for (int j = 0; j < cosmeticInfo.throwableMaterialGrabIndices.Length; j++)
					{
						int key = cosmeticInfo.throwableMaterialGrabIndices[j];
						if (!CosmeticsV2Spawner_Dirty.materialIndexToSnowballThrowablePlayfabIdStringRight.ContainsKey(key))
						{
							CosmeticsV2Spawner_Dirty.materialIndexToSnowballThrowablePlayfabIdStringRight[key] = cosmeticInfo.playFabID;
						}
					}
					CosmeticsV2Spawner_Dirty.throwableIndexPlayfabIdStringRight.TryAdd(cosmeticInfo.throwableIndex, cosmeticInfo.playFabID);
				}
				else
				{
					for (int k = 0; k < cosmeticInfo.throwableMaterialGrabIndices.Length; k++)
					{
						int key2 = cosmeticInfo.throwableMaterialGrabIndices[k];
						if (!CosmeticsV2Spawner_Dirty.materialIndexToSnowballThrowablePlayfabIdStringLeft.ContainsKey(key2))
						{
							CosmeticsV2Spawner_Dirty.materialIndexToSnowballThrowablePlayfabIdStringLeft[key2] = cosmeticInfo.playFabID;
						}
					}
					CosmeticsV2Spawner_Dirty.throwableIndexPlayfabIdStringLeft.TryAdd(cosmeticInfo.throwableIndex, cosmeticInfo.playFabID);
				}
			}
			CosmeticsV2Spawner_Dirty._g_loadOpInfos.Add(loadOpInfo);
			if (CosmeticsV2Spawner_Dirty._g_loadOpInfosForRigAndCosmeticIDDicts[vrRigIndex] == null)
			{
				CosmeticsV2Spawner_Dirty._g_loadOpInfosForRigAndCosmeticIDDicts[vrRigIndex] = new Dictionary<string, List<CosmeticsV2Spawner_Dirty.LoadOpInfo>>();
			}
			if (!CosmeticsV2Spawner_Dirty._g_loadOpInfosForRigAndCosmeticIDDicts[vrRigIndex].ContainsKey(cosmeticInfo.playFabID))
			{
				CosmeticsV2Spawner_Dirty._g_loadOpInfosForRigAndCosmeticIDDicts[vrRigIndex].Add(cosmeticInfo.playFabID, new List<CosmeticsV2Spawner_Dirty.LoadOpInfo>());
			}
			CosmeticsV2Spawner_Dirty._g_loadOpInfosForRigAndCosmeticIDDicts[vrRigIndex][cosmeticInfo.playFabID].Add(loadOpInfo);
			partCount++;
			if (part.partType == ECosmeticPartType.Holdable && i == 0)
			{
				break;
			}
		}
	}

	public static bool GetPlayfabIdFromThrowableIndex(bool isLeft, int throwableIndex, out string playfabId)
	{
		if (CosmeticsV2Spawner_Dirty.throwableIndexPlayfabIdStringLeft == null || CosmeticsV2Spawner_Dirty.throwableIndexPlayfabIdStringRight == null)
		{
			playfabId = "null";
			return false;
		}
		if (isLeft && CosmeticsV2Spawner_Dirty.throwableIndexPlayfabIdStringLeft.TryGetValue(throwableIndex, out playfabId))
		{
			return true;
		}
		if (!isLeft && CosmeticsV2Spawner_Dirty.throwableIndexPlayfabIdStringRight.TryGetValue(throwableIndex, out playfabId))
		{
			return true;
		}
		playfabId = "null";
		return false;
	}

	public static bool GetThrowableIDFromMaterialIndex(bool isLeft, int matIndex, out string throwableId)
	{
		if (CosmeticsV2Spawner_Dirty.materialIndexToSnowballThrowablePlayfabIdStringLeft == null || CosmeticsV2Spawner_Dirty.materialIndexToSnowballThrowablePlayfabIdStringRight == null)
		{
			throwableId = "null";
			return false;
		}
		if (isLeft && CosmeticsV2Spawner_Dirty.materialIndexToSnowballThrowablePlayfabIdStringLeft.TryGetValue(matIndex, out throwableId))
		{
			return true;
		}
		if (!isLeft && CosmeticsV2Spawner_Dirty.materialIndexToSnowballThrowablePlayfabIdStringRight.TryGetValue(matIndex, out throwableId))
		{
			return true;
		}
		throwableId = "null";
		return false;
	}

	public static void ProcessLoadOpInfos(VRRig rig, string playfabId, CosmeticItemRegistry registry)
	{
		CosmeticsV2Spawner_Dirty.<>c__DisplayClass40_0 CS$<>8__locals1 = new CosmeticsV2Spawner_Dirty.<>c__DisplayClass40_0();
		CS$<>8__locals1.registry = registry;
		if (!CosmeticsV2Spawner_Dirty.processedIdsByRig.ContainsKey(rig))
		{
			CosmeticsV2Spawner_Dirty.processedIdsByRig.Add(rig, new HashSet<string>());
		}
		else if (CosmeticsV2Spawner_Dirty.processedIdsByRig[rig].Contains(playfabId))
		{
			return;
		}
		CosmeticsV2Spawner_Dirty.processedIdsByRig[rig].Add(playfabId);
		if (!CosmeticsV2Spawner_Dirty.currentGOBatchByRegistry.ContainsKey(CS$<>8__locals1.registry))
		{
			CosmeticsV2Spawner_Dirty.currentGOBatchByRegistry[CS$<>8__locals1.registry] = new List<GameObject>();
		}
		if (!CosmeticsV2Spawner_Dirty.sides.ContainsKey(CS$<>8__locals1.registry))
		{
			CosmeticsV2Spawner_Dirty.sides[CS$<>8__locals1.registry] = new List<StringEnum<ECosmeticSelectSide>>();
		}
		if (!CosmeticsV2Spawner_Dirty.overrides.ContainsKey(CS$<>8__locals1.registry))
		{
			CosmeticsV2Spawner_Dirty.overrides[CS$<>8__locals1.registry] = new List<bool>();
		}
		List<CosmeticsV2Spawner_Dirty.LoadOpInfo> list = CosmeticsV2Spawner_Dirty._g_loadOpInfosForRigAndCosmeticIDDicts[CosmeticsV2Spawner_Dirty._gVRRigDatasIndexByRig[rig]][playfabId];
		for (int i = 0; i < list.Count; i++)
		{
			CosmeticsV2Spawner_Dirty.<>c__DisplayClass40_1 CS$<>8__locals2 = new CosmeticsV2Spawner_Dirty.<>c__DisplayClass40_1();
			CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
			CS$<>8__locals2.currentIndex = CosmeticsV2Spawner_Dirty._g_loadOp_to_index.Count;
			CosmeticsV2Spawner_Dirty._ProcessLoadOpInfo(CS$<>8__locals2.currentIndex, list[i]);
			CosmeticsV2Spawner_Dirty._g_loadOpInfos[CS$<>8__locals2.currentIndex].loadOp.Completed += CS$<>8__locals2.<ProcessLoadOpInfos>g__AddToRegistryWhenCompleted|2;
		}
	}

	private static void _ProcessLoadOpInfo(int currentIndex, CosmeticsV2Spawner_Dirty.LoadOpInfo loadOpInfo)
	{
		try
		{
			loadOpInfo.loadOp = loadOpInfo.part.prefabAssetRef.InstantiateAsync(CosmeticsV2Spawner_Dirty._gDeactivatedSpawnParent, false);
			loadOpInfo.isStarted = true;
			CosmeticsV2Spawner_Dirty._g_loadOp_to_index.Add(loadOpInfo.loadOp, currentIndex);
			loadOpInfo.loadOp.Completed += CosmeticsV2Spawner_Dirty._Step3_HandleLoadOpCompleted;
			CosmeticsV2Spawner_Dirty._g_loadOpInfos[currentIndex] = loadOpInfo;
		}
		catch (InvalidKeyException ex)
		{
			Debug.LogError("CosmeticsV2Spawner_Dirty: Missing Addressable for " + string.Format("\"{0}\" part index {1}. Skipping. {2}", loadOpInfo.cosmeticInfoV2.displayName, loadOpInfo.partIndex, ex.Message));
			loadOpInfo.isStarted = true;
			loadOpInfo.resultGObj = null;
			CosmeticsV2Spawner_Dirty._g_loadOpInfos[currentIndex] = loadOpInfo;
			CosmeticsV2Spawner_Dirty._g_loadOpsCountCompleted++;
		}
		catch (ArgumentException ex2)
		{
			Debug.LogError("CosmeticsV2Spawner_Dirty: Invalid Addressable key/config for " + string.Format("\"{0}\" part index {1}. Skipping. {2}", loadOpInfo.cosmeticInfoV2.displayName, loadOpInfo.partIndex, ex2.Message));
			loadOpInfo.isStarted = true;
			loadOpInfo.resultGObj = null;
			CosmeticsV2Spawner_Dirty._g_loadOpInfos[currentIndex] = loadOpInfo;
			CosmeticsV2Spawner_Dirty._g_loadOpsCountCompleted++;
		}
	}

	private static void _Step3_HandleLoadOpCompleted(AsyncOperationHandle<GameObject> loadOp)
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		int index;
		if (!CosmeticsV2Spawner_Dirty._g_loadOp_to_index.TryGetValue(loadOp, out index))
		{
			throw new Exception("(this should never happen) could not find LoadOpInfo in `_g_loadOpInfos`.");
		}
		CosmeticsV2Spawner_Dirty.LoadOpInfo loadOpInfo = CosmeticsV2Spawner_Dirty._g_loadOpInfos[index];
		if (loadOp.Status == AsyncOperationStatus.Failed)
		{
			Debug.LogWarning("CosmeticsV2Spawner_Dirty: Failed to load part " + string.Format("\"{0}\" (key: {1}). Skipping.", loadOpInfo.cosmeticInfoV2.displayName, loadOpInfo.part.prefabAssetRef.RuntimeKey));
			CosmeticsV2Spawner_Dirty._g_loadOpsCountCompleted++;
			CosmeticsV2Spawner_Dirty._g_loadOp_to_index.Remove(loadOp);
			return;
		}
		CosmeticsV2Spawner_Dirty._g_loadOpsCountCompleted++;
		ECosmeticSelectSide ecosmeticSelectSide = loadOpInfo.attachInfo.selectSide;
		string name = loadOpInfo.cosmeticInfoV2.playFabID;
		if (ecosmeticSelectSide != ECosmeticSelectSide.Both)
		{
			string playFabID = loadOpInfo.cosmeticInfoV2.playFabID;
			string arg;
			if (ecosmeticSelectSide != ECosmeticSelectSide.Left)
			{
				if (ecosmeticSelectSide != ECosmeticSelectSide.Right)
				{
					arg = "";
				}
				else
				{
					arg = " RIGHT.";
				}
			}
			else
			{
				arg = " LEFT.";
			}
			name = ZString.Concat<string, string>(playFabID, arg);
		}
		loadOpInfo.resultGObj = loadOp.Result;
		loadOpInfo.resultGObj.SetActive(false);
		Transform transform = loadOpInfo.resultGObj.transform;
		Transform transform2 = transform;
		CosmeticPart[] holdableParts = loadOpInfo.cosmeticInfoV2.holdableParts;
		if (holdableParts != null && holdableParts.Length > 0)
		{
			TransferrableObject componentInChildren = loadOpInfo.resultGObj.GetComponentInChildren<TransferrableObject>(true);
			if (componentInChildren && componentInChildren.gameObject != loadOpInfo.resultGObj)
			{
				transform2 = componentInChildren.transform;
				transform2.gameObject.SetActive(false);
				loadOpInfo.resultGObj.SetActive(true);
			}
		}
		if (loadOpInfo.cosmeticInfoV2.isThrowable)
		{
			SnowballThrowable componentInChildren2 = loadOpInfo.resultGObj.GetComponentInChildren<SnowballThrowable>(true);
			if (componentInChildren2 && componentInChildren2.gameObject != loadOpInfo.resultGObj)
			{
				transform2 = componentInChildren2.transform;
				transform2.gameObject.SetActive(false);
				loadOpInfo.resultGObj.SetActive(true);
			}
		}
		transform2.name = name;
		CosmeticsV2Spawner_Dirty.VRRigData vrrigData = (loadOpInfo.vrRigIndex != -1) ? CosmeticsV2Spawner_Dirty._gVRRigDatas[loadOpInfo.vrRigIndex] : default(CosmeticsV2Spawner_Dirty.VRRigData);
		Transform transform3;
		switch (loadOpInfo.part.partType)
		{
		case ECosmeticPartType.Holdable:
			transform3 = ((loadOpInfo.attachInfo.parentBone != GTHardCodedBones.EBone.body_AnchorFront_StowSlot) ? vrrigData.parentOfDeactivatedHoldables : vrrigData.boneXforms[(int)loadOpInfo.attachInfo.parentBone]);
			goto IL_2C1;
		case ECosmeticPartType.Functional:
			transform3 = vrrigData.boneXforms[(int)loadOpInfo.attachInfo.parentBone];
			goto IL_2C1;
		case ECosmeticPartType.FirstPerson:
			transform3 = CosmeticsV2Spawner_Dirty.g_gorillaPlayer.CosmeticsHeadTarget;
			goto IL_2C1;
		case ECosmeticPartType.LocalRig:
			transform3 = vrrigData.boneXforms[(int)loadOpInfo.attachInfo.parentBone];
			goto IL_2C1;
		}
		throw new ArgumentOutOfRangeException("partType", "unhandled part type.");
		IL_2C1:
		Transform transform4 = transform3;
		if (transform4)
		{
			transform.SetParent(transform4, false);
			transform.localPosition = loadOpInfo.attachInfo.offset.pos;
			Transform transform5 = transform;
			XformOffset offset = loadOpInfo.attachInfo.offset;
			transform5.localRotation = offset.rot;
			transform.localScale = loadOpInfo.attachInfo.offset.scale;
		}
		else
		{
			Debug.LogError(string.Concat(new string[]
			{
				string.Format("Bone transform not found for cosmetic part type {0}. Cosmetic: ", loadOpInfo.part.partType),
				"\"",
				loadOpInfo.cosmeticInfoV2.displayName,
				"\",",
				string.Format("part: \"{0}\"", loadOpInfo.part.prefabAssetRef.RuntimeKey)
			}));
		}
		switch (loadOpInfo.part.partType)
		{
		case ECosmeticPartType.Holdable:
		{
			vrrigData.vrRig_cosmetics.Add(transform2.gameObject);
			HoldableObject componentInChildren3 = loadOpInfo.resultGObj.GetComponentInChildren<HoldableObject>(true);
			SnowballThrowable snowballThrowable = componentInChildren3 as SnowballThrowable;
			if (snowballThrowable != null)
			{
				CosmeticsV2Spawner_Dirty.AddPartToThrowableLists(loadOpInfo, snowballThrowable);
				goto IL_61D;
			}
			TransferrableObject transferrableObject = componentInChildren3 as TransferrableObject;
			if (transferrableObject == null)
			{
				if (componentInChildren3 != null)
				{
					throw new Exception("Encountered unexpected HoldableObject derived type on cosmetic part: \"" + loadOpInfo.cosmeticInfoV2.displayName + "\"");
				}
				goto IL_61D;
			}
			else
			{
				string text = loadOpInfo.cosmeticInfoV2.playFabID;
				int[] array;
				if (CosmeticsLegacyV1Info.TryGetBodyDockAllObjectsIndexes(text, out array))
				{
					if (loadOpInfo.partIndex < array.Length && loadOpInfo.partIndex >= 0)
					{
						transferrableObject.myIndex = array[loadOpInfo.partIndex];
					}
				}
				else if (text.Length >= 5 && text[0] == 'L')
				{
					if (text[1] != 'M')
					{
						throw new Exception("(this should never happen) A TransferrableObject cosmetic added sometime after 2024-06 does not use the expected PlayFabID format where the string starts with \"LM\" and ends with \".\". Path: " + transform2.GetPathQ());
					}
					string text2 = text;
					text = ((text2[text2.Length - 1] == '.') ? text : (text + "."));
					int num = 224;
					transferrableObject.myIndex = num + CosmeticIDUtils.PlayFabIdToIndexInCategory(text);
				}
				else
				{
					transferrableObject.myIndex = -2;
					if (!(text == "STICKABLE TARGET"))
					{
						Debug.LogError(string.Concat(new string[]
						{
							"Cosmetic \"",
							loadOpInfo.cosmeticInfoV2.displayName,
							"\" cannot derive `TransferrableObject.myIndex` from playFabId \"",
							text,
							"\" and so will not be included in `BodyDockPositions.allObjects` array."
						}));
					}
				}
				ProjectileWeapon projectileWeapon = transferrableObject as ProjectileWeapon;
				if (projectileWeapon != null && loadOpInfo.cosmeticInfoV2.playFabID == "Slingshot")
				{
					vrrigData.vrRig.projectileWeapon = projectileWeapon;
				}
				if (transferrableObject.myIndex > 0 && transferrableObject.myIndex < vrrigData.bdPositions_allObjects_length)
				{
					vrrigData.bdPositionsComp._allObjects[transferrableObject.myIndex] = transferrableObject;
					goto IL_61D;
				}
				goto IL_61D;
			}
			break;
		}
		case ECosmeticPartType.Functional:
			vrrigData.vrRig_cosmetics.Add(transform2.gameObject);
			goto IL_61D;
		case ECosmeticPartType.FirstPerson:
		case ECosmeticPartType.LocalRig:
			vrrigData.vrRig_override.Add(transform2.gameObject);
			goto IL_61D;
		}
		throw new ArgumentOutOfRangeException("Unexpected ECosmeticPartType value encountered: " + string.Format("{0}, ", loadOpInfo.part.partType) + string.Format("int: {0}.", (int)loadOpInfo.part.partType));
		IL_61D:
		if (loadOpInfo.vrRigIndex > -1)
		{
			CosmeticsV2Spawner_Dirty._gVRRigDatas[loadOpInfo.vrRigIndex] = vrrigData;
		}
		CosmeticRefRegistry cosmeticReferences = CosmeticsV2Spawner_Dirty._gVRRigDatas[loadOpInfo.vrRigIndex].vrRig.cosmeticReferences;
		CosmeticRefTarget[] componentsInChildren = loadOpInfo.resultGObj.GetComponentsInChildren<CosmeticRefTarget>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			cosmeticReferences.Register(componentsInChildren[i].id, componentsInChildren[i].gameObject);
		}
		CosmeticsV2Spawner_Dirty._g_loadOpInfos[index] = loadOpInfo;
	}

	private static void _RetryDownload(int loadOpIndex)
	{
		if (loadOpIndex < 0 || loadOpIndex >= CosmeticsV2Spawner_Dirty._g_loadOpInfos.Count)
		{
			Debug.LogError("(should never happen) Unexpected! While trying to recover from a failed download, the value " + string.Format("{0}={1} was out of range of ", "loadOpIndex", loadOpIndex) + string.Format("{0}.Count={1}.", "_g_loadOpInfos", CosmeticsV2Spawner_Dirty._g_loadOpInfos.Count));
			return;
		}
		CosmeticsV2Spawner_Dirty.LoadOpInfo loadOpInfo = CosmeticsV2Spawner_Dirty._g_loadOpInfos[loadOpIndex];
		if (!CosmeticsV2Spawner_Dirty._g_loadOp_to_index.Remove(loadOpInfo.loadOp))
		{
			Debug.LogWarning(string.Concat(new string[]
			{
				"(should never happen) Unexpected! Could not find the loadOp to remove it in the _g_loadOp_to_index. If you see this message then comparison does not work the way I thought and we need a different way to store/retrieve loadOpInfos. Happened while trying to retry failed download prefab part of cosmetic \"",
				loadOpInfo.cosmeticInfoV2.displayName,
				"\" with guid \"",
				loadOpInfo.part.prefabAssetRef.AssetGUID,
				"\"."
			}));
		}
		Debug.Log(string.Concat(new string[]
		{
			"Retrying prefab part of cosmetic \"",
			loadOpInfo.cosmeticInfoV2.displayName,
			"\" with guid \"",
			loadOpInfo.part.prefabAssetRef.AssetGUID,
			"\"."
		}));
		loadOpInfo.loadOp = loadOpInfo.part.prefabAssetRef.InstantiateAsync(CosmeticsV2Spawner_Dirty._gDeactivatedSpawnParent, false);
		CosmeticsV2Spawner_Dirty._g_loadOpInfos[loadOpIndex] = loadOpInfo;
		CosmeticsV2Spawner_Dirty._g_loadOp_to_index[loadOpInfo.loadOp] = loadOpIndex;
		loadOpInfo.loadOp.Completed += CosmeticsV2Spawner_Dirty._Step3_HandleLoadOpCompleted;
	}

	private static void AddPartToThrowableLists(CosmeticsV2Spawner_Dirty.LoadOpInfo loadOpInfo, SnowballThrowable throwable)
	{
		CosmeticsV2Spawner_Dirty.VRRigData vrrigData = CosmeticsV2Spawner_Dirty._gVRRigDatas[loadOpInfo.vrRigIndex];
		EHandedness handednessFromBone = GTHardCodedBones.GetHandednessFromBone(loadOpInfo.attachInfo.parentBone);
		bool flag = vrrigData.vrRig == CosmeticsV2Spawner_Dirty._gVRRigDatas[0].vrRig;
		throwable.SpawnOffset = loadOpInfo.attachInfo.offset;
		switch (handednessFromBone)
		{
		case EHandedness.None:
			throw new ArgumentException(string.Concat(new string[]
			{
				"Encountered throwable cosmetic \"",
				loadOpInfo.cosmeticInfoV2.displayName,
				"\" where handedness ",
				string.Format("could not be determined from bone `{0}`. ", loadOpInfo.attachInfo.parentBone),
				"Path: \"",
				throwable.transform.GetPath(),
				"\""
			}));
		case EHandedness.Left:
			CosmeticsV2Spawner_Dirty.ResizeAndSetAtIndex<GameObject>(vrrigData.bdPositions_leftHandThrowables, throwable.gameObject, throwable.throwableMakerIndex);
			if (flag)
			{
				CosmeticsV2Spawner_Dirty.ResizeAndSetAtIndex<SnowballThrowable>(CosmeticsV2Spawner_Dirty._gSnowballMakerLeft_throwables, throwable, throwable.throwableMakerIndex);
			}
			vrrigData.bdPositionsComp.leftHandThrowables = vrrigData.bdPositions_leftHandThrowables.ToArray();
			CosmeticsV2Spawner_Dirty._gSnowballMakerLeft.SetupThrowables(CosmeticsV2Spawner_Dirty._gSnowballMakerLeft_throwables.ToArray());
			return;
		case EHandedness.Right:
			CosmeticsV2Spawner_Dirty.ResizeAndSetAtIndex<GameObject>(vrrigData.bdPositions_rightHandThrowables, throwable.gameObject, throwable.throwableMakerIndex);
			if (flag)
			{
				CosmeticsV2Spawner_Dirty.ResizeAndSetAtIndex<SnowballThrowable>(CosmeticsV2Spawner_Dirty._gSnowballMakerRight_throwables, throwable, throwable.throwableMakerIndex);
			}
			vrrigData.bdPositionsComp.rightHandThrowables = vrrigData.bdPositions_rightHandThrowables.ToArray();
			CosmeticsV2Spawner_Dirty._gSnowballMakerRight.SetupThrowables(CosmeticsV2Spawner_Dirty._gSnowballMakerRight_throwables.ToArray());
			return;
		default:
			throw new ArgumentOutOfRangeException("Unexpected ECosmeticSelectSide value encountered: " + string.Format("{0}, ", handednessFromBone) + string.Format("int: {0}.", (int)handednessFromBone));
		}
	}

	private static void ResizeAndSetAtIndex<T>(List<T> list, T item, int index)
	{
		if (index >= list.Count)
		{
			int num = index - list.Count + 1;
			for (int i = 0; i < num; i++)
			{
				list.Add(default(T));
			}
		}
		list[index] = item;
	}

	private static void _Step4_PopulateAllArrays()
	{
		foreach (CosmeticsV2Spawner_Dirty.VRRigData vrrigData in CosmeticsV2Spawner_Dirty._gVRRigDatas)
		{
			vrrigData.bdPositionsComp._allObjects = new TransferrableObject[2000];
		}
		GTDelayedExec.Add(CosmeticsV2Spawner_Dirty._instance, 1f, -Mathf.Abs("_Step5_InitializeVRRigsAndCosmeticsControllerFinalize".GetHashCode()));
	}

	private static void _Step5_InitializeVRRigsAndCosmeticsControllerFinalize()
	{
		CosmeticsController.instance.UpdateWardrobeModelsAndButtons();
		try
		{
			Action onPostInstantiateAllPrefabs = CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs;
			if (onPostInstantiateAllPrefabs != null)
			{
				onPostInstantiateAllPrefabs();
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		try
		{
			CosmeticsController.instance.InitializeCosmeticStands();
		}
		catch (Exception exception2)
		{
			Debug.LogException(exception2);
		}
		try
		{
			Action onPostInstantiateAllPrefabs2 = CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs2;
			if (onPostInstantiateAllPrefabs2 != null)
			{
				onPostInstantiateAllPrefabs2();
			}
		}
		catch (Exception exception3)
		{
			Debug.LogException(exception3);
		}
		try
		{
			CosmeticsController.instance.UpdateWornCosmetics();
			CosmeticsV2Spawner_Dirty.<_Step5_InitializeVRRigsAndCosmeticsControllerFinalize>g__StartupRerun|47_0();
		}
		catch (Exception exception4)
		{
			Debug.LogException(exception4);
		}
		foreach (CosmeticsV2Spawner_Dirty.VRRigData vrrigData in CosmeticsV2Spawner_Dirty._gVRRigDatas)
		{
			try
			{
				if (vrrigData.bdPositionsComp.isActiveAndEnabled)
				{
					vrrigData.bdPositionsComp.RefreshTransferrableItems();
				}
			}
			catch (Exception exception5)
			{
				Debug.LogException(exception5, vrrigData.vrRig);
			}
		}
		try
		{
			StoreController.instance.InitalizeCosmeticStands();
		}
		catch (Exception exception6)
		{
			Debug.LogException(exception6);
		}
		CosmeticsV2Spawner_Dirty.completed = true;
		CosmeticsV2Spawner_Dirty.k_stopwatch.Stop();
		Debug.Log("_Step5_InitializeVRRigsAndCosmeticsControllerFinalize" + string.Format(": Done instantiating cosmetics in {0:0.0000} seconds.", (double)CosmeticsV2Spawner_Dirty.k_stopwatch.ElapsedMilliseconds / 1000.0));
	}

	public static CosmeticsV2Spawner_Dirty.VRRigData RigDataForRig(VRRig rig)
	{
		return CosmeticsV2Spawner_Dirty._gVRRigDatas[CosmeticsV2Spawner_Dirty._gVRRigDatasIndexByRig[rig]];
	}

	[CompilerGenerated]
	internal static void <ProcessLoadOpInfos>g__PostCompletionProcess|40_0()
	{
		foreach (KeyValuePair<CosmeticItemRegistry, List<GameObject>> keyValuePair in CosmeticsV2Spawner_Dirty.currentGOBatchByRegistry)
		{
			List<GameObject> value = keyValuePair.Value;
			CosmeticItemRegistry key = keyValuePair.Key;
			for (int i = 0; i < value.Count; i++)
			{
				if (!(value[i] == null) && CosmeticsV2Spawner_Dirty.overrides[key][i])
				{
					key.InitializeCosmetic(value[i], true);
				}
			}
			for (int j = 0; j < value.Count; j++)
			{
				if (!(value[j] == null) && !CosmeticsV2Spawner_Dirty.overrides[key][j])
				{
					key.InitializeCosmetic(value[j], false);
				}
			}
			for (int k = 0; k < value.Count; k++)
			{
				if (!(value[k] == null))
				{
					ISpawnable[] componentsInChildren = value[k].GetComponentsInChildren<ISpawnable>(true);
					for (int l = 0; l < componentsInChildren.Length; l++)
					{
						if (!componentsInChildren[l].IsSpawned)
						{
							try
							{
								componentsInChildren[l].IsSpawned = true;
								componentsInChildren[l].CosmeticSelectedSide = CosmeticsV2Spawner_Dirty.sides[key][k];
								componentsInChildren[l].OnSpawn(key.Rig);
							}
							catch (Exception exception)
							{
								Debug.LogException(exception);
							}
						}
					}
				}
			}
			value.Clear();
			CosmeticsV2Spawner_Dirty.sides[key].Clear();
			CosmeticsV2Spawner_Dirty.overrides[key].Clear();
			key.RefreshRig();
		}
	}

	[CompilerGenerated]
	internal static GameObject <ProcessLoadOpInfos>g__ObjectToInitialize|40_1(CosmeticsV2Spawner_Dirty.LoadOpInfo loadOpInfo)
	{
		if (loadOpInfo.resultGObj == null)
		{
			return null;
		}
		Transform transform = loadOpInfo.resultGObj.transform;
		CosmeticPart[] holdableParts = loadOpInfo.cosmeticInfoV2.holdableParts;
		if (holdableParts != null && holdableParts.Length > 0)
		{
			TransferrableObject componentInChildren = loadOpInfo.resultGObj.GetComponentInChildren<TransferrableObject>(true);
			if (componentInChildren && componentInChildren.gameObject != loadOpInfo.resultGObj)
			{
				transform = componentInChildren.transform;
				transform.gameObject.SetActive(false);
				loadOpInfo.resultGObj.SetActive(true);
			}
		}
		if (loadOpInfo.cosmeticInfoV2.isThrowable)
		{
			SnowballThrowable componentInChildren2 = loadOpInfo.resultGObj.GetComponentInChildren<SnowballThrowable>(true);
			if (componentInChildren2 && componentInChildren2.gameObject != loadOpInfo.resultGObj)
			{
				transform = componentInChildren2.transform;
				transform.gameObject.SetActive(false);
				loadOpInfo.resultGObj.SetActive(true);
			}
		}
		return transform.gameObject;
	}

	[CompilerGenerated]
	internal static void <_Step5_InitializeVRRigsAndCosmeticsControllerFinalize>g__StartupRerun|47_0()
	{
		CosmeticsV2Spawner_Dirty.<<_Step5_InitializeVRRigsAndCosmeticsControllerFinalize>g__StartupRerun|47_0>d <<_Step5_InitializeVRRigsAndCosmeticsControllerFinalize>g__StartupRerun|47_0>d;
		<<_Step5_InitializeVRRigsAndCosmeticsControllerFinalize>g__StartupRerun|47_0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
		<<_Step5_InitializeVRRigsAndCosmeticsControllerFinalize>g__StartupRerun|47_0>d.<>1__state = -1;
		<<_Step5_InitializeVRRigsAndCosmeticsControllerFinalize>g__StartupRerun|47_0>d.<>t__builder.Start<CosmeticsV2Spawner_Dirty.<<_Step5_InitializeVRRigsAndCosmeticsControllerFinalize>g__StartupRerun|47_0>d>(ref <<_Step5_InitializeVRRigsAndCosmeticsControllerFinalize>g__StartupRerun|47_0>d);
	}

	private static CosmeticsV2Spawner_Dirty _instance;

	public static Action OnPostInstantiateAllPrefabs;

	public static Action OnPostInstantiateAllPrefabs2;

	[OnEnterPlay_SetNull]
	private static Transform _gDeactivatedSpawnParent;

	[OnEnterPlay_Set(0)]
	private static int _g_loadOpsCountCompleted = 0;

	[OnEnterPlay_Set(0)]
	private static int _g_loadOpsImmediatelyCountCompleted = 0;

	private const int _k_maxActiveLoadOps = 1000000;

	private const int _k_maxTotalLoadOps = 1000000;

	private const int _k_delayedStatusCheckContextId = -100;

	[OnEnterPlay_Clear]
	private static readonly List<CosmeticsV2Spawner_Dirty.LoadOpInfo> _g_loadOpInfos = new List<CosmeticsV2Spawner_Dirty.LoadOpInfo>(100000);

	[OnEnterPlay_Clear]
	private static Dictionary<string, List<CosmeticsV2Spawner_Dirty.LoadOpInfo>>[] _g_loadOpInfosForRigAndCosmeticIDDicts;

	[OnEnterPlay_Clear]
	private static readonly Dictionary<AsyncOperationHandle<GameObject>, int> _g_loadOp_to_index = new Dictionary<AsyncOperationHandle<GameObject>, int>(100000);

	[OnEnterPlay_SetNull]
	private static SnowballMaker _gSnowballMakerLeft;

	[OnEnterPlay_Clear]
	private static readonly List<SnowballThrowable> _gSnowballMakerLeft_throwables = new List<SnowballThrowable>(20);

	[OnEnterPlay_SetNull]
	private static SnowballMaker _gSnowballMakerRight;

	[OnEnterPlay_Clear]
	private static readonly List<SnowballThrowable> _gSnowballMakerRight_throwables = new List<SnowballThrowable>(20);

	[OnEnterPlay_SetNull]
	private static GTPlayer g_gorillaPlayer;

	[OnEnterPlay_SetNull]
	private static Transform[] g_allInstantiatedParts;

	private static Stopwatch k_stopwatch = new Stopwatch();

	[OnEnterPlay_Clear]
	public static readonly List<CosmeticsV2Spawner_Dirty.VRRigData> _gVRRigDatas = new List<CosmeticsV2Spawner_Dirty.VRRigData>(20);

	private static Dictionary<VRRig, int> _gVRRigDatasIndexByRig = new Dictionary<VRRig, int>();

	[OnEnterPlay_Clear]
	private static Dictionary<int, string> materialIndexToSnowballThrowablePlayfabIdStringLeft;

	[OnEnterPlay_Clear]
	private static Dictionary<int, string> materialIndexToSnowballThrowablePlayfabIdStringRight;

	[OnEnterPlay_Clear]
	private static Dictionary<int, string> throwableIndexPlayfabIdStringRight;

	[OnEnterPlay_Clear]
	private static Dictionary<int, string> throwableIndexPlayfabIdStringLeft;

	private static Dictionary<VRRig, HashSet<string>> processedIdsByRig = new Dictionary<VRRig, HashSet<string>>();

	private static Dictionary<CosmeticItemRegistry, List<GameObject>> currentGOBatchByRegistry = new Dictionary<CosmeticItemRegistry, List<GameObject>>();

	private static Dictionary<CosmeticItemRegistry, List<StringEnum<ECosmeticSelectSide>>> sides = new Dictionary<CosmeticItemRegistry, List<StringEnum<ECosmeticSelectSide>>>();

	private static Dictionary<CosmeticItemRegistry, List<bool>> overrides = new Dictionary<CosmeticItemRegistry, List<bool>>();

	private struct LoadOpInfo
	{
		public LoadOpInfo(CosmeticAttachInfo attachInfo, CosmeticPart part, int partIndex, CosmeticInfoV2 cosmeticInfoV2, int vrRigIndex)
		{
			this.isStarted = false;
			this.loadOp = default(AsyncOperationHandle<GameObject>);
			this.resultGObj = null;
			this.attachInfo = attachInfo;
			this.part = part;
			this.partIndex = partIndex;
			this.cosmeticInfoV2 = cosmeticInfoV2;
			this.vrRigIndex = vrRigIndex;
		}

		public bool isStarted;

		public AsyncOperationHandle<GameObject> loadOp;

		public GameObject resultGObj;

		public readonly CosmeticAttachInfo attachInfo;

		public readonly CosmeticPart part;

		public readonly int partIndex;

		public readonly CosmeticInfoV2 cosmeticInfoV2;

		public readonly int vrRigIndex;
	}

	public struct VRRigData
	{
		public VRRigData(VRRig vrRig, Transform[] boneXforms)
		{
			this.vrRig = vrRig;
			this.boneXforms = boneXforms;
			if (!vrRig.transform.TryFindByPath("./**/Holdables", out this.parentOfDeactivatedHoldables, false))
			{
				Debug.LogError("Could not find parent for deactivated holdables. Falling back to VRRig transform: \"" + vrRig.transform.GetPath() + "\"");
			}
			this.bdPositionsComp = vrRig.GetComponentInChildren<BodyDockPositions>(true);
			this.vrRig_cosmetics = new List<GameObject>(500);
			this.vrRig_override = new List<GameObject>(500);
			this.bdPositions_leftHandThrowables = new List<GameObject>(20);
			this.bdPositions_rightHandThrowables = new List<GameObject>(20);
			this.bdPositions_allObjects_length = 2000;
		}

		public readonly VRRig vrRig;

		public readonly Transform[] boneXforms;

		public readonly BodyDockPositions bdPositionsComp;

		public readonly List<GameObject> vrRig_cosmetics;

		public readonly List<GameObject> vrRig_override;

		public readonly Transform parentOfDeactivatedHoldables;

		public int bdPositions_allObjects_length;

		public readonly List<GameObject> bdPositions_leftHandThrowables;

		public readonly List<GameObject> bdPositions_rightHandThrowables;
	}
}
