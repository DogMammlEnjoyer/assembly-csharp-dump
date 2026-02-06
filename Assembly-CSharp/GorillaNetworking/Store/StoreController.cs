using System;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using GorillaTag.CosmeticSystem;
using PlayFab;
using UnityEngine;

namespace GorillaNetworking.Store
{
	public class StoreController : MonoBehaviour
	{
		public void Awake()
		{
			if (StoreController.instance == null)
			{
				StoreController.instance = this;
			}
			else if (StoreController.instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			this.CosmeticStandsDict = new Dictionary<string, DynamicCosmeticStand>();
			this.StandsByPlayfabID = new Dictionary<string, List<DynamicCosmeticStand>>();
		}

		public void RefreshCosmeticStandsDictionaryFromDepartments()
		{
			foreach (StoreDepartment storeDepartment in this.Departments)
			{
				if (!(storeDepartment == null) && !storeDepartment.departmentName.IsNullOrEmpty())
				{
					foreach (StoreDisplay storeDisplay in storeDepartment.Displays)
					{
						if (!storeDisplay.displayName.IsNullOrEmpty())
						{
							foreach (DynamicCosmeticStand dynamicCosmeticStand in storeDisplay.Stands)
							{
								if (!dynamicCosmeticStand.StandName.IsNullOrEmpty())
								{
									string text = string.Concat(new string[]
									{
										storeDepartment.departmentName,
										"|",
										storeDisplay.displayName,
										"|",
										dynamicCosmeticStand.StandName
									});
									if (this.CosmeticStandsDict.ContainsKey(text))
									{
										Debug.LogError(string.Concat(new string[]
										{
											"StoreStuff: Duplicate Stand Name: ",
											text,
											" Please Fix Gameobject : ",
											dynamicCosmeticStand.gameObject.GetPath(),
											dynamicCosmeticStand.gameObject.name
										}), base.gameObject);
									}
									else
									{
										this.CosmeticStandsDict.Add(text, dynamicCosmeticStand);
									}
								}
							}
						}
					}
				}
			}
		}

		public void AddStandToCosmeticStandsDictionary(DynamicCosmeticStand stand)
		{
			if (stand.parentDepartment == null || stand.parentDepartment.departmentName.IsNullOrEmpty() || stand.parentDisplay == null || stand.parentDisplay.displayName.IsNullOrEmpty() || stand.StandName.IsNullOrEmpty() || this.CosmeticStandsDict == null)
			{
				return;
			}
			string text = string.Concat(new string[]
			{
				stand.parentDepartment.departmentName,
				"|",
				stand.parentDisplay.displayName,
				"|",
				stand.StandName
			});
			if (this.CosmeticStandsDict.ContainsKey(text))
			{
				Debug.LogError(string.Concat(new string[]
				{
					"StoreStuff: Duplicate Stand Name: ",
					text,
					" Please Fix Gameobject : ",
					stand.gameObject.GetPath(),
					stand.gameObject.name
				}), base.gameObject);
				return;
			}
			this.CosmeticStandsDict.Add(text, stand);
		}

		public void RemoveStandFromDynamicCosmeticStandsDictionary(DynamicCosmeticStand stand)
		{
			if (stand.parentDepartment == null || stand.parentDepartment.departmentName.IsNullOrEmpty() || stand.parentDisplay == null || stand.parentDisplay.displayName.IsNullOrEmpty() || stand.StandName.IsNullOrEmpty() || this.CosmeticStandsDict == null)
			{
				return;
			}
			string text = string.Concat(new string[]
			{
				stand.parentDepartment.departmentName,
				"|",
				stand.parentDisplay.displayName,
				"|",
				stand.StandName
			});
			if (!this.CosmeticStandsDict.ContainsKey(text))
			{
				Debug.LogError(string.Concat(new string[]
				{
					"StoreStuff: StoreController doesn't have stand in its dict. that's weird!: ",
					text,
					" Please Fix Gameobject : ",
					stand.gameObject.GetPath(),
					stand.gameObject.name
				}), base.gameObject);
				return;
			}
			this.CosmeticStandsDict.Remove(text);
		}

		private void Create_StandsByPlayfabIDDictionary()
		{
			foreach (DynamicCosmeticStand dynamicCosmeticStand in this.CosmeticStandsDict.Values)
			{
				this.AddStandToPlayfabIDDictionary(dynamicCosmeticStand);
			}
		}

		public void AddStandToPlayfabIDDictionary(DynamicCosmeticStand dynamicCosmeticStand)
		{
			if (!dynamicCosmeticStand.StandName.IsNullOrEmpty())
			{
				if (dynamicCosmeticStand.thisCosmeticName.IsNullOrEmpty())
				{
					return;
				}
				if (this.StandsByPlayfabID.ContainsKey(dynamicCosmeticStand.thisCosmeticName))
				{
					this.StandsByPlayfabID[dynamicCosmeticStand.thisCosmeticName].Add(dynamicCosmeticStand);
					return;
				}
				this.StandsByPlayfabID.Add(dynamicCosmeticStand.thisCosmeticName, new List<DynamicCosmeticStand>
				{
					dynamicCosmeticStand
				});
			}
		}

		public void RemoveStandFromPlayFabIDDictionary(DynamicCosmeticStand dynamicCosmeticStand)
		{
			List<DynamicCosmeticStand> list;
			if (this.StandsByPlayfabID.TryGetValue(dynamicCosmeticStand.thisCosmeticName, out list))
			{
				list.Remove(dynamicCosmeticStand);
			}
		}

		public void ExportCosmeticStandLayoutWithItems()
		{
		}

		public void ExportCosmeticStandLayoutWITHOUTItems()
		{
		}

		public void ImportCosmeticStandLayout()
		{
		}

		private void InitializeFromTitleData()
		{
			PlayFabTitleDataCache.Instance.GetTitleData("StoreLayoutData", delegate(string data)
			{
				this.ImportCosmeticStandLayoutFromTitleData(data);
			}, delegate(PlayFabError e)
			{
				Debug.LogError(string.Format("Error getting StoreLayoutData data: {0}", e));
			}, false);
		}

		private void ImportCosmeticStandLayoutFromTitleData(string TSVData)
		{
			this.standImport = new StandImport();
			this.standImport.DecomposeFromTitleDataString(TSVData);
			foreach (StandTypeData standTypeData in this.standImport.standData)
			{
				string key = string.Concat(new string[]
				{
					standTypeData.departmentID,
					"|",
					standTypeData.displayID,
					"|",
					standTypeData.standID
				});
				this.standImport.standKeyToDataDict.Add(key, standTypeData);
				if (this.CosmeticStandsDict.ContainsKey(key))
				{
					this.CosmeticStandsDict[key].SetStandTypeString(standTypeData.bustType);
					this.CosmeticStandsDict[key].SpawnItemOntoStand(standTypeData.playFabID);
					this.CosmeticStandsDict[key].InitializeCosmetic();
				}
			}
		}

		public void InitializeStandFromTitleData(DynamicCosmeticStand stand)
		{
			if (stand.parentDepartment == null || stand.parentDepartment.departmentName.IsNullOrEmpty() || stand.parentDisplay == null || stand.parentDisplay.displayName.IsNullOrEmpty() || stand.StandName.IsNullOrEmpty() || this.CosmeticStandsDict == null)
			{
				Debug.LogError("Stand " + stand.name + " is missing important setup data somehow, please fix!", stand.gameObject);
				return;
			}
			string key = string.Concat(new string[]
			{
				stand.parentDepartment.departmentName,
				"|",
				stand.parentDisplay.displayName,
				"|",
				stand.StandName
			});
			if (!this.CosmeticStandsDict.ContainsKey(key) || !this.standImport.standKeyToDataDict.ContainsKey(key))
			{
				return;
			}
			StandTypeData standTypeData = this.standImport.standKeyToDataDict[key];
			this.CosmeticStandsDict[key].SetStandTypeString(standTypeData.bustType);
			this.CosmeticStandsDict[key].SpawnItemOntoStand(standTypeData.playFabID);
			this.CosmeticStandsDict[key].InitializeCosmetic();
		}

		public void InitalizeCosmeticStands()
		{
			this.cosmeticsInitialized = true;
			this.RefreshCosmeticStandsDictionaryFromDepartments();
			if (this.LoadFromTitleData)
			{
				this.InitializeFromTitleData();
			}
		}

		public void LoadCosmeticOntoStand(string standID, string playFabId)
		{
			if (this.CosmeticStandsDict.ContainsKey(standID))
			{
				this.CosmeticStandsDict[standID].SpawnItemOntoStand(playFabId);
				Debug.Log("StoreStuff: Cosmetic Loaded Onto Stand: " + standID + " | " + playFabId);
			}
		}

		public void ClearCosmetics()
		{
			foreach (StoreDepartment storeDepartment in this.Departments)
			{
				StoreDisplay[] displays = storeDepartment.Displays;
				for (int i = 0; i < displays.Length; i++)
				{
					DynamicCosmeticStand[] stands = displays[i].Stands;
					for (int j = 0; j < stands.Length; j++)
					{
						stands[j].ClearCosmetics();
					}
				}
			}
		}

		public static CosmeticSO FindCosmeticInAllCosmeticsArraySO(string playfabId)
		{
			if (StoreController.instance == null)
			{
				StoreController.instance = Object.FindAnyObjectByType<StoreController>();
			}
			return StoreController.instance.AllCosmeticsArraySO.SearchForCosmeticSO(playfabId);
		}

		public DynamicCosmeticStand FindCosmeticStandByCosmeticName(string PlayFabID)
		{
			foreach (DynamicCosmeticStand dynamicCosmeticStand in this.CosmeticStandsDict.Values)
			{
				if (dynamicCosmeticStand.thisCosmeticName == PlayFabID)
				{
					return dynamicCosmeticStand;
				}
			}
			return null;
		}

		public void FindAllDepartments()
		{
			this.Departments = Object.FindObjectsByType<StoreDepartment>(FindObjectsSortMode.None).ToList<StoreDepartment>();
		}

		public void SaveAllCosmeticsPositions()
		{
			foreach (StoreDepartment storeDepartment in this.Departments)
			{
				foreach (StoreDisplay storeDisplay in storeDepartment.Displays)
				{
					foreach (DynamicCosmeticStand dynamicCosmeticStand in storeDisplay.Stands)
					{
						Debug.Log(string.Concat(new string[]
						{
							"StoreStuff: Saving Items mount transform: ",
							storeDepartment.departmentName,
							"|",
							storeDisplay.displayName,
							"|",
							dynamicCosmeticStand.StandName,
							"|",
							dynamicCosmeticStand.DisplayHeadModel.bustType.ToString(),
							"|",
							dynamicCosmeticStand.thisCosmeticName
						}));
						dynamicCosmeticStand.UpdateCosmeticsMountPositions();
					}
				}
			}
		}

		public static void SetForGame()
		{
			if (StoreController.instance == null)
			{
				StoreController.instance = Object.FindAnyObjectByType<StoreController>();
			}
			StoreController.instance.RefreshCosmeticStandsDictionaryFromDepartments();
			foreach (DynamicCosmeticStand dynamicCosmeticStand in StoreController.instance.CosmeticStandsDict.Values)
			{
				dynamicCosmeticStand.SetStandType(dynamicCosmeticStand.DisplayHeadModel.bustType);
				dynamicCosmeticStand.SpawnItemOntoStand(dynamicCosmeticStand.thisCosmeticName);
			}
		}

		[OnEnterPlay_Clear]
		public static volatile StoreController instance;

		public List<StoreDepartment> Departments;

		private Dictionary<string, DynamicCosmeticStand> CosmeticStandsDict;

		public Dictionary<string, List<DynamicCosmeticStand>> StandsByPlayfabID;

		public AllCosmeticsArraySO AllCosmeticsArraySO;

		public bool cosmeticsInitialized;

		public bool LoadFromTitleData;

		private string exportHeader = "Department ID\tDisplay ID\tStand ID\tStand Type\tPlayFab ID";

		private StandImport standImport;
	}
}
