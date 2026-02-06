using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GorillaNetworking;
using JetBrains.Annotations;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Networking;

namespace GorillaTagScripts.Builder
{
	public class SharedBlocksManager : MonoBehaviour
	{
		public event Action<string> OnGetTableConfiguration;

		public event Action<string> OnGetTitleDataBuildComplete;

		public event Action<int> OnSavePrivateScanSuccess;

		public event Action<int, string> OnSavePrivateScanFailed;

		public event Action<int, bool> OnFetchPrivateScanComplete;

		public event Action<bool, SharedBlocksManager.SharedBlocksMap> OnFoundDefaultSharedBlocksMap;

		public event Action<bool> OnGetPopularMapsComplete;

		public static event Action OnRecentMapIdsUpdated;

		public static event Action OnSaveTimeUpdated;

		public List<SharedBlocksManager.SharedBlocksMap> LatestPopularMaps
		{
			get
			{
				return this.latestPopularMaps;
			}
		}

		public string[] BuildData
		{
			get
			{
				return this.privateScanDataCache;
			}
		}

		public bool IsWaitingOnRequest()
		{
			return this.saveScanInProgress || this.getScanInProgress;
		}

		private void Awake()
		{
			if (SharedBlocksManager.instance == null)
			{
				SharedBlocksManager.instance = this;
				for (int i = 0; i < BuilderScanKiosk.NUM_SAVE_SLOTS; i++)
				{
					this.privateScanDataCache[i] = string.Empty;
					this.hasPulledPrivateScanMothership[i] = false;
				}
				return;
			}
			Object.Destroy(this);
		}

		public void Start()
		{
			SharedBlocksManager.<Start>d__100 <Start>d__;
			<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Start>d__.<>4__this = this;
			<Start>d__.<>1__state = -1;
			<Start>d__.<>t__builder.Start<SharedBlocksManager.<Start>d__100>(ref <Start>d__);
		}

		private bool TryGetCachedSharedBlocksMapByMapID(string mapID, out SharedBlocksManager.SharedBlocksMap result)
		{
			foreach (SharedBlocksManager.SharedBlocksMap sharedBlocksMap in this.mapResponseCache)
			{
				if (sharedBlocksMap.MapID.Equals(mapID))
				{
					result = sharedBlocksMap;
					return true;
				}
			}
			result = null;
			return false;
		}

		private void AddMapToResponseCache(SharedBlocksManager.SharedBlocksMap map)
		{
			if (map == null)
			{
				return;
			}
			try
			{
				int num = this.mapResponseCache.FindIndex((SharedBlocksManager.SharedBlocksMap x) => x.MapID.Equals(map.MapID));
				if (num < 0)
				{
					this.mapResponseCache.Add(map);
				}
				else
				{
					this.mapResponseCache[num] = map;
				}
			}
			catch (Exception ex)
			{
				GTDev.LogError<string>("SharedBlocksManager AddMapToResponseCache Exception " + ex.ToString(), null);
			}
			if (this.mapResponseCache.Count >= 5)
			{
				this.mapResponseCache.RemoveAt(0);
			}
		}

		public static bool IsMapIDValid(string mapID)
		{
			if (mapID.IsNullOrEmpty())
			{
				return false;
			}
			if (mapID.Length != 8)
			{
				return false;
			}
			if (!Regex.IsMatch(mapID, "^[CFGHKMNPRTWXZ256789]+$"))
			{
				GTDev.LogError<string>("Invalid Characters in SharedBlocksManager IsMapIDValid map " + mapID, null);
				return false;
			}
			return true;
		}

		public static LinkedList<string> GetRecentUpVotes()
		{
			return SharedBlocksManager.recentUpVotes;
		}

		public static List<string> GetLocalMapIDs()
		{
			return SharedBlocksManager.localMapIds;
		}

		private static void SetPublishTimeForSlot(int slotID, DateTime time)
		{
			SharedBlocksManager.LocalPublishInfo value;
			if (SharedBlocksManager.localPublishData.TryGetValue(slotID, out value))
			{
				value.publishTime = time.ToBinary();
				SharedBlocksManager.localPublishData[slotID] = value;
				return;
			}
			SharedBlocksManager.LocalPublishInfo value2 = new SharedBlocksManager.LocalPublishInfo
			{
				mapID = null,
				publishTime = time.ToBinary()
			};
			SharedBlocksManager.localPublishData.Add(slotID, value2);
		}

		private static void SetMapIDAndPublishTimeForSlot(int slotID, string mapID, DateTime time)
		{
			SharedBlocksManager.LocalPublishInfo value = new SharedBlocksManager.LocalPublishInfo
			{
				mapID = mapID,
				publishTime = time.ToBinary()
			};
			SharedBlocksManager.localPublishData.AddOrUpdate(slotID, value);
		}

		public static SharedBlocksManager.LocalPublishInfo GetPublishInfoForSlot(int slot)
		{
			SharedBlocksManager.LocalPublishInfo result;
			if (SharedBlocksManager.localPublishData.TryGetValue(slot, out result))
			{
				return result;
			}
			return new SharedBlocksManager.LocalPublishInfo
			{
				mapID = null,
				publishTime = DateTime.MinValue.ToBinary()
			};
		}

		private void LoadPlayerPrefs()
		{
			string recentVotesPrefsKey = this.serializationConfig.recentVotesPrefsKey;
			string localMapsPrefsKey = this.serializationConfig.localMapsPrefsKey;
			string @string = PlayerPrefs.GetString(recentVotesPrefsKey, null);
			string string2 = PlayerPrefs.GetString(localMapsPrefsKey, null);
			if (!@string.IsNullOrEmpty())
			{
				try
				{
					SharedBlocksManager.recentUpVotes = JsonConvert.DeserializeObject<LinkedList<string>>(@string);
					while (SharedBlocksManager.recentUpVotes.Count > 10)
					{
						SharedBlocksManager.recentUpVotes.RemoveLast();
					}
					goto IL_82;
				}
				catch (Exception ex)
				{
					GTDev.LogWarning<string>("SharedBlocksManager failed to deserialize Recent Up Votes " + ex.Message, null);
					SharedBlocksManager.recentUpVotes.Clear();
					goto IL_82;
				}
			}
			SharedBlocksManager.recentUpVotes.Clear();
			IL_82:
			if (!string2.IsNullOrEmpty())
			{
				SharedBlocksManager.localPublishData.Clear();
				SharedBlocksManager.localMapIds.Clear();
				try
				{
					SharedBlocksManager.localPublishData = JsonConvert.DeserializeObject<Dictionary<int, SharedBlocksManager.LocalPublishInfo>>(string2);
				}
				catch (Exception ex2)
				{
					GTDev.LogWarning<string>("SharedBlocksManager failed to deserialize localMapIDs " + ex2.Message, null);
					this.GetPlayfabLastSaveTime();
				}
				foreach (KeyValuePair<int, SharedBlocksManager.LocalPublishInfo> keyValuePair in SharedBlocksManager.localPublishData)
				{
					if (!keyValuePair.Value.mapID.IsNullOrEmpty() && SharedBlocksManager.IsMapIDValid(keyValuePair.Value.mapID))
					{
						SharedBlocksManager.localMapIds.Add(keyValuePair.Value.mapID);
					}
				}
				Action onSaveTimeUpdated = SharedBlocksManager.OnSaveTimeUpdated;
				if (onSaveTimeUpdated != null)
				{
					onSaveTimeUpdated();
				}
			}
			else
			{
				SharedBlocksManager.localMapIds.Clear();
				this.GetPlayfabLastSaveTime();
			}
			Action onRecentMapIdsUpdated = SharedBlocksManager.OnRecentMapIdsUpdated;
			if (onRecentMapIdsUpdated == null)
			{
				return;
			}
			onRecentMapIdsUpdated();
		}

		private void SaveRecentVotesToPlayerPrefs()
		{
			PlayerPrefs.SetString(this.serializationConfig.recentVotesPrefsKey, JsonConvert.SerializeObject(SharedBlocksManager.recentUpVotes));
			PlayerPrefs.Save();
		}

		private void SaveLocalMapIdsToPlayerPrefs()
		{
			PlayerPrefs.SetString(this.serializationConfig.localMapsPrefsKey, JsonConvert.SerializeObject(SharedBlocksManager.localPublishData));
			PlayerPrefs.Save();
		}

		public void RequestVote(string mapID, bool up, Action<bool, string> callback)
		{
			if (!MothershipClientContext.IsClientLoggedIn())
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestVote Client Not Logged into Mothership", null);
				if (callback != null)
				{
					callback(false, 1.ToString());
				}
				return;
			}
			if (this.voteInProgress)
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestVote already in progress", null);
				return;
			}
			this.voteInProgress = true;
			base.StartCoroutine(this.PostVote(new SharedBlocksManager.VoteRequest
			{
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
				mapId = mapID,
				vote = (up ? 1 : -1)
			}, callback));
		}

		private IEnumerator PostVote(SharedBlocksManager.VoteRequest data, Action<bool, string> callback)
		{
			UnityWebRequest request = new UnityWebRequest(this.serializationConfig.sharedBlocksApiBaseURL + "/api/MapVote", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			if (request.result == UnityWebRequest.Result.Success)
			{
				string mapId = data.mapId;
				if (data.vote == -1)
				{
					if (SharedBlocksManager.recentUpVotes.Remove(mapId))
					{
						this.SaveRecentVotesToPlayerPrefs();
						Action onRecentMapIdsUpdated = SharedBlocksManager.OnRecentMapIdsUpdated;
						if (onRecentMapIdsUpdated != null)
						{
							onRecentMapIdsUpdated();
						}
					}
				}
				else if (!SharedBlocksManager.recentUpVotes.Contains(mapId))
				{
					if (SharedBlocksManager.recentUpVotes.Count >= 10)
					{
						SharedBlocksManager.recentUpVotes.RemoveLast();
					}
					SharedBlocksManager.recentUpVotes.AddFirst(mapId);
					this.SaveRecentVotesToPlayerPrefs();
					Action onRecentMapIdsUpdated2 = SharedBlocksManager.OnRecentMapIdsUpdated;
					if (onRecentMapIdsUpdated2 != null)
					{
						onRecentMapIdsUpdated2();
					}
				}
				this.voteInProgress = false;
				if (callback != null)
				{
					callback(true, "");
				}
			}
			else
			{
				GTDev.LogError<string>(string.Format("PostVote Error: {0} -- raw response: ", request.responseCode) + request.downloadHandler.text, null);
				if (request.result != UnityWebRequest.Result.ProtocolError)
				{
					retry = true;
				}
				else
				{
					long responseCode = request.responseCode;
					if (responseCode >= 500L)
					{
						if (responseCode >= 600L)
						{
							goto IL_207;
						}
					}
					else if (responseCode != 408L && responseCode != 429L)
					{
						goto IL_207;
					}
					bool flag = true;
					goto IL_20A;
					IL_207:
					flag = false;
					IL_20A:
					if (flag)
					{
						retry = true;
					}
					else
					{
						this.voteInProgress = false;
						if (callback != null)
						{
							callback(false, "REQUEST ERROR");
						}
					}
				}
			}
			if (retry)
			{
				if (this.voteRetryCount < this.maxRetriesOnFail)
				{
					float seconds = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.voteRetryCount + 1)));
					this.voteRetryCount++;
					yield return new WaitForSeconds(seconds);
					this.voteInProgress = false;
					this.RequestVote(data.mapId, data.vote == 1, callback);
				}
				else
				{
					this.voteRetryCount = 0;
					this.voteInProgress = false;
					if (callback != null)
					{
						callback(false, "CONNECTION ERROR");
					}
				}
			}
			yield break;
		}

		private void RequestPublishMap(string userMetadataKey)
		{
			if (!MothershipClientContext.IsClientLoggedIn())
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestPublishMap Client Not Logged into Mothership", null);
				this.PublishMapComplete(false, userMetadataKey, string.Empty, 0L);
				return;
			}
			if (this.publishRequestInProgress)
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestPublishMap Publish Request in progress", null);
				return;
			}
			this.publishRequestInProgress = true;
			base.StartCoroutine(this.PostPublishMapRequest(new SharedBlocksManager.PublishMapRequestData
			{
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
				userdataMetadataKey = userMetadataKey,
				playerNickname = GorillaTagger.Instance.offlineVRRig.playerNameVisible
			}, new SharedBlocksManager.PublishMapRequestCallback(this.PublishMapComplete)));
		}

		private void PublishMapComplete(bool success, string key, [CanBeNull] string mapID, long response)
		{
			this.publishRequestInProgress = false;
			if (success)
			{
				int num = this.serializationConfig.scanSlotMothershipKeys.IndexOf(key);
				if (num >= 0)
				{
					SharedBlocksManager.LocalPublishInfo localPublishInfo;
					if (SharedBlocksManager.localPublishData.TryGetValue(num, out localPublishInfo))
					{
						SharedBlocksManager.localMapIds.Remove(localPublishInfo.mapID);
					}
					SharedBlocksManager.SetMapIDAndPublishTimeForSlot(num, mapID, DateTime.Now);
					this.SaveLocalMapIdsToPlayerPrefs();
				}
				if (!SharedBlocksManager.localMapIds.Contains(mapID))
				{
					SharedBlocksManager.localMapIds.Add(mapID);
					Action onRecentMapIdsUpdated = SharedBlocksManager.OnRecentMapIdsUpdated;
					if (onRecentMapIdsUpdated != null)
					{
						onRecentMapIdsUpdated();
					}
				}
				SharedBlocksManager.SharedBlocksMap map = new SharedBlocksManager.SharedBlocksMap
				{
					MapID = mapID,
					MapData = this.privateScanDataCache[num],
					CreatorNickName = GorillaTagger.Instance.offlineVRRig.playerNameVisible,
					UpdateTime = DateTime.Now
				};
				this.AddMapToResponseCache(map);
				Action<int> onSavePrivateScanSuccess = this.OnSavePrivateScanSuccess;
				if (onSavePrivateScanSuccess != null)
				{
					onSavePrivateScanSuccess(this.currentSaveScanIndex);
				}
			}
			else
			{
				Action<int, string> onSavePrivateScanFailed = this.OnSavePrivateScanFailed;
				if (onSavePrivateScanFailed != null)
				{
					onSavePrivateScanFailed(this.currentSaveScanIndex, "ERROR PUBLISHING: " + response.ToString());
				}
			}
			this.currentSaveScanIndex = -1;
			this.currentSaveScanData = string.Empty;
		}

		private IEnumerator PostPublishMapRequest(SharedBlocksManager.PublishMapRequestData data, SharedBlocksManager.PublishMapRequestCallback callback)
		{
			UnityWebRequest request = new UnityWebRequest(this.serializationConfig.sharedBlocksApiBaseURL + "/api/Publish", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			if (request.result == UnityWebRequest.Result.Success)
			{
				GTDev.Log<string>("PostPublishMapRequest Success: raw response: " + request.downloadHandler.text, null);
				try
				{
					string text = request.downloadHandler.text;
					bool success = !text.IsNullOrEmpty() && SharedBlocksManager.IsMapIDValid(text);
					if (callback != null)
					{
						callback(success, data.userdataMetadataKey, text, request.responseCode);
					}
					goto IL_21D;
				}
				catch (Exception ex)
				{
					GTDev.LogError<string>("SharedBlocksManager PostPublishMapRequest " + ex.Message, null);
					if (callback != null)
					{
						callback(false, data.userdataMetadataKey, null, request.responseCode);
					}
					goto IL_21D;
				}
			}
			if (request.result != UnityWebRequest.Result.ProtocolError)
			{
				retry = true;
			}
			else
			{
				long responseCode = request.responseCode;
				if (responseCode >= 500L)
				{
					if (responseCode >= 600L)
					{
						goto IL_1E0;
					}
				}
				else if (responseCode != 408L && responseCode != 429L)
				{
					goto IL_1E0;
				}
				bool flag = true;
				goto IL_1E3;
				IL_1E0:
				flag = false;
				IL_1E3:
				if (flag)
				{
					retry = true;
				}
				else if (callback != null)
				{
					callback(false, data.userdataMetadataKey, string.Empty, request.responseCode);
				}
			}
			IL_21D:
			if (retry)
			{
				if (this.postPublishMapRetryCount < this.maxRetriesOnFail)
				{
					float seconds = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.postPublishMapRetryCount + 1)));
					this.postPublishMapRetryCount++;
					yield return new WaitForSeconds(seconds);
					this.publishRequestInProgress = false;
					this.RequestPublishMap(data.userdataMetadataKey);
				}
				else
				{
					this.postPublishMapRetryCount = 0;
					if (callback != null)
					{
						callback(false, data.userdataMetadataKey, string.Empty, request.responseCode);
					}
				}
			}
			yield break;
		}

		public void RequestMapDataFromID(string mapID, SharedBlocksManager.BlocksMapRequestCallback callback)
		{
			if (!MothershipClientContext.IsClientLoggedIn())
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestMapDataFromID Client Not Logged into Mothership", null);
				if (callback != null)
				{
					callback(null);
				}
				return;
			}
			SharedBlocksManager.SharedBlocksMap response;
			if (this.TryGetCachedSharedBlocksMapByMapID(mapID, out response))
			{
				if (callback != null)
				{
					callback(response);
				}
				return;
			}
			if (this.getMapDataFromIDInProgress)
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestMapDataFromID Fetch already in progress", null);
				return;
			}
			this.getMapDataFromIDInProgress = true;
			base.StartCoroutine(this.GetMapDataFromID(new SharedBlocksManager.GetMapDataFromIDRequest
			{
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
				mapId = mapID
			}, callback));
		}

		private IEnumerator GetMapDataFromID(SharedBlocksManager.GetMapDataFromIDRequest data, SharedBlocksManager.BlocksMapRequestCallback callback)
		{
			UnityWebRequest request = new UnityWebRequest(this.serializationConfig.sharedBlocksApiBaseURL + "/api/GetMapData", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			if (request.result == UnityWebRequest.Result.Success)
			{
				string text = request.downloadHandler.text;
				this.GetMapDataFromIDComplete(data.mapId, text, callback);
			}
			else if (request.result != UnityWebRequest.Result.ProtocolError)
			{
				retry = true;
			}
			else
			{
				long responseCode = request.responseCode;
				if (responseCode >= 500L)
				{
					if (responseCode >= 600L)
					{
						goto IL_14E;
					}
				}
				else if (responseCode != 408L && responseCode != 429L)
				{
					goto IL_14E;
				}
				bool flag = true;
				goto IL_151;
				IL_14E:
				flag = false;
				IL_151:
				if (flag)
				{
					retry = true;
				}
				else
				{
					this.GetMapDataFromIDComplete(data.mapId, null, callback);
				}
			}
			if (retry)
			{
				if (this.getMapDataFromIDRetryCount < this.maxRetriesOnFail)
				{
					float seconds = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.getMapDataFromIDRetryCount + 1)));
					this.getMapDataFromIDRetryCount++;
					yield return new WaitForSeconds(seconds);
					this.getMapDataFromIDInProgress = false;
					this.RequestMapDataFromID(data.mapId, callback);
				}
				else
				{
					this.getMapDataFromIDRetryCount = 0;
					this.GetMapDataFromIDComplete(data.mapId, null, callback);
				}
			}
			yield break;
		}

		private void GetMapDataFromIDComplete(string mapID, [CanBeNull] string response, SharedBlocksManager.BlocksMapRequestCallback callback)
		{
			this.getMapDataFromIDInProgress = false;
			if (response == null)
			{
				if (callback != null)
				{
					callback(null);
					return;
				}
			}
			else
			{
				SharedBlocksManager.SharedBlocksMap sharedBlocksMap = new SharedBlocksManager.SharedBlocksMap
				{
					MapID = mapID,
					MapData = response
				};
				this.AddMapToResponseCache(sharedBlocksMap);
				if (callback != null)
				{
					callback(sharedBlocksMap);
				}
			}
		}

		public bool RequestGetTopMaps(int pageNum, int pageSize, string sort)
		{
			if (!MothershipClientContext.IsClientLoggedIn())
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestFetchPopularBlocksMaps Client Not Logged into Mothership", null);
				return false;
			}
			if (this.getTopMapsInProgress)
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestFetchPopularBlocksMaps already in progress", null);
				return false;
			}
			this.getTopMapsInProgress = true;
			this.lastGetTopMapsTime = Time.timeAsDouble;
			base.StartCoroutine(this.GetTopMaps(new SharedBlocksManager.GetMapsRequest
			{
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
				page = pageNum,
				pageSize = pageSize,
				sort = sort,
				ShowInactive = false
			}, new Action<List<SharedBlocksManager.SharedBlocksMapMetaData>>(this.GetTopMapsComplete)));
			return true;
		}

		private IEnumerator GetTopMaps(SharedBlocksManager.GetMapsRequest data, Action<List<SharedBlocksManager.SharedBlocksMapMetaData>> callback)
		{
			UnityWebRequest request = new UnityWebRequest(this.serializationConfig.sharedBlocksApiBaseURL + "/api/GetMaps", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			if (request.result == UnityWebRequest.Result.Success)
			{
				try
				{
					List<SharedBlocksManager.SharedBlocksMapMetaData> obj = JsonConvert.DeserializeObject<List<SharedBlocksManager.SharedBlocksMapMetaData>>(request.downloadHandler.text);
					if (callback != null)
					{
						callback(obj);
					}
					goto IL_187;
				}
				catch (Exception)
				{
					if (callback != null)
					{
						callback(null);
					}
					goto IL_187;
				}
			}
			if (request.result != UnityWebRequest.Result.ProtocolError)
			{
				retry = true;
			}
			else
			{
				long responseCode = request.responseCode;
				if (responseCode >= 500L)
				{
					if (responseCode >= 600L)
					{
						goto IL_165;
					}
				}
				else if (responseCode != 408L && responseCode != 429L)
				{
					goto IL_165;
				}
				bool flag = true;
				goto IL_168;
				IL_165:
				flag = false;
				IL_168:
				if (flag)
				{
					retry = true;
				}
				else if (callback != null)
				{
					callback(null);
				}
			}
			IL_187:
			if (retry)
			{
				if (this.getTopMapsRetryCount < this.maxRetriesOnFail)
				{
					float seconds = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.getTopMapsRetryCount + 1)));
					this.getTopMapsRetryCount++;
					yield return new WaitForSeconds(seconds);
					this.getTopMapsInProgress = false;
					this.RequestGetTopMaps(data.page, data.pageSize, data.sort);
				}
				else
				{
					this.getTopMapsRetryCount = 0;
					if (callback != null)
					{
						callback(null);
					}
				}
			}
			yield break;
		}

		private void GetTopMapsComplete([CanBeNull] List<SharedBlocksManager.SharedBlocksMapMetaData> maps)
		{
			this.getTopMapsInProgress = false;
			if (maps != null)
			{
				this.latestPopularMaps.Clear();
				foreach (SharedBlocksManager.SharedBlocksMapMetaData sharedBlocksMapMetaData in maps)
				{
					if (sharedBlocksMapMetaData != null && SharedBlocksManager.IsMapIDValid(sharedBlocksMapMetaData.mapId))
					{
						DateTime createTime = DateTime.MinValue;
						DateTime updateTime = DateTime.MinValue;
						try
						{
							createTime = DateTime.Parse(sharedBlocksMapMetaData.createdTime);
							updateTime = DateTime.Parse(sharedBlocksMapMetaData.updatedTime);
						}
						catch (Exception ex)
						{
							GTDev.LogWarning<string>("SharedBlocksManager GetTopMaps bad update or create time" + ex.Message, null);
						}
						SharedBlocksManager.SharedBlocksMap item = new SharedBlocksManager.SharedBlocksMap
						{
							MapID = sharedBlocksMapMetaData.mapId,
							CreatorID = null,
							CreatorNickName = sharedBlocksMapMetaData.nickname,
							CreateTime = createTime,
							UpdateTime = updateTime,
							MapData = null
						};
						this.latestPopularMaps.Add(item);
					}
				}
				this.hasCachedTopMaps = true;
				Action<bool> onGetPopularMapsComplete = this.OnGetPopularMapsComplete;
				if (onGetPopularMapsComplete == null)
				{
					return;
				}
				onGetPopularMapsComplete(true);
				return;
			}
			else
			{
				Action<bool> onGetPopularMapsComplete2 = this.OnGetPopularMapsComplete;
				if (onGetPopularMapsComplete2 == null)
				{
					return;
				}
				onGetPopularMapsComplete2(false);
				return;
			}
		}

		private void RequestUpdateMapActive(string userMetadataKey, bool active)
		{
			if (!MothershipClientContext.IsClientLoggedIn())
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestUpdateMapActive Client Not Logged into Mothership", null);
				return;
			}
			if (this.updateMapActiveInProgress)
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestUpdateMapActive already in progress", null);
				return;
			}
			this.updateMapActiveInProgress = true;
			base.StartCoroutine(this.PostUpdateMapActive(new SharedBlocksManager.UpdateMapActiveRequest
			{
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
				userdataMetadataKey = userMetadataKey,
				setActive = active
			}, new Action<bool>(this.OnUpdatedMapActiveComplete)));
		}

		private IEnumerator PostUpdateMapActive(SharedBlocksManager.UpdateMapActiveRequest data, Action<bool> callback)
		{
			UnityWebRequest request = new UnityWebRequest(this.serializationConfig.sharedBlocksApiBaseURL + "/api/UpdateMapActive", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			if (request.result == UnityWebRequest.Result.Success)
			{
				if (callback != null)
				{
					callback(true);
				}
			}
			else if (request.result != UnityWebRequest.Result.ProtocolError)
			{
				retry = true;
			}
			else
			{
				long responseCode = request.responseCode;
				if (responseCode >= 500L)
				{
					if (responseCode >= 600L)
					{
						goto IL_132;
					}
				}
				else if (responseCode != 408L && responseCode != 429L)
				{
					goto IL_132;
				}
				bool flag = true;
				goto IL_135;
				IL_132:
				flag = false;
				IL_135:
				if (flag)
				{
					retry = true;
				}
				else if (callback != null)
				{
					callback(false);
				}
			}
			if (retry)
			{
				if (this.updateMapActiveRetryCount < this.maxRetriesOnFail)
				{
					float seconds = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.updateMapActiveRetryCount + 1)));
					this.updateMapActiveRetryCount++;
					yield return new WaitForSeconds(seconds);
					this.updateMapActiveInProgress = false;
					this.RequestUpdateMapActive(data.userdataMetadataKey, data.setActive);
				}
				else
				{
					this.updateMapActiveRetryCount = 0;
					if (callback != null)
					{
						callback(false);
					}
				}
			}
			yield break;
		}

		private void OnUpdatedMapActiveComplete(bool success)
		{
			this.updateMapActiveInProgress = false;
		}

		private Task WaitForPlayfabSessionToken()
		{
			SharedBlocksManager.<WaitForPlayfabSessionToken>d__126 <WaitForPlayfabSessionToken>d__;
			<WaitForPlayfabSessionToken>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WaitForPlayfabSessionToken>d__.<>1__state = -1;
			<WaitForPlayfabSessionToken>d__.<>t__builder.Start<SharedBlocksManager.<WaitForPlayfabSessionToken>d__126>(ref <WaitForPlayfabSessionToken>d__);
			return <WaitForPlayfabSessionToken>d__.<>t__builder.Task;
		}

		public void RequestTableConfiguration()
		{
			if (this.fetchedTableConfig)
			{
				Action<string> onGetTableConfiguration = this.OnGetTableConfiguration;
				if (onGetTableConfiguration == null)
				{
					return;
				}
				onGetTableConfiguration(this.tableConfigResponse);
			}
		}

		private void FetchConfigurationFromTitleData()
		{
			PlayFabClientAPI.GetTitleData(new GetTitleDataRequest
			{
				Keys = new List<string>
				{
					this.serializationConfig.tableConfigurationKey
				}
			}, new Action<GetTitleDataResult>(this.OnGetConfigurationSuccess), new Action<PlayFabError>(this.OnGetConfigurationFail), null, null);
		}

		private void OnGetConfigurationSuccess(GetTitleDataResult result)
		{
			GTDev.Log<string>("SharedBlocksManager OnGetConfigurationSuccess", null);
			string text;
			if (result.Data.TryGetValue(this.serializationConfig.tableConfigurationKey, out text))
			{
				this.tableConfigResponse = text;
				this.fetchedTableConfig = true;
				Action<string> onGetTableConfiguration = this.OnGetTableConfiguration;
				if (onGetTableConfiguration == null)
				{
					return;
				}
				onGetTableConfiguration(this.tableConfigResponse);
			}
		}

		private void OnGetConfigurationFail(PlayFabError error)
		{
			GTDev.LogWarning<string>("SharedBlocksManager OnGetConfigurationFail " + error.Error.ToString(), null);
			if (error.Error == PlayFabErrorCode.ConnectionError && this.fetchTableConfigRetryCount < this.maxRetriesOnFail)
			{
				float waitTime = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.fetchTableConfigRetryCount + 1)));
				this.fetchTableConfigRetryCount++;
				base.StartCoroutine(this.RetryAfterWaitTime(waitTime, new Action(this.FetchConfigurationFromTitleData)));
				return;
			}
			this.tableConfigResponse = string.Empty;
			this.fetchedTableConfig = true;
			Action<string> onGetTableConfiguration = this.OnGetTableConfiguration;
			if (onGetTableConfiguration == null)
			{
				return;
			}
			onGetTableConfiguration(this.tableConfigResponse);
		}

		private IEnumerator RetryAfterWaitTime(float waitTime, Action function)
		{
			yield return new WaitForSeconds(waitTime);
			if (function != null)
			{
				function();
			}
			yield break;
		}

		public void FetchTitleDataBuild()
		{
			if (!this.fetchTitleDataBuildComplete)
			{
				if (!this.fetchTitleDataBuildInProgress)
				{
					this.fetchTitleDataBuildInProgress = true;
					base.StartCoroutine(this.SendTitleDataRequest(new GetTitleDataRequest
					{
						Keys = new List<string>
						{
							this.serializationConfig.titleDataKey
						}
					}, new Action<GetTitleDataResult>(this.OnGetTitleDataBuildSuccess), new Action<PlayFabError>(this.OnGetTitleDataBuildFail)));
				}
				return;
			}
			Action<string> onGetTitleDataBuildComplete = this.OnGetTitleDataBuildComplete;
			if (onGetTitleDataBuildComplete == null)
			{
				return;
			}
			onGetTitleDataBuildComplete(this.titleDataBuildCache);
		}

		private IEnumerator SendTitleDataRequest(GetTitleDataRequest request, Action<GetTitleDataResult> successCallback, Action<PlayFabError> failCallback)
		{
			while (!PlayFabSettings.staticPlayer.IsClientLoggedIn())
			{
				yield return new WaitForSeconds(5f);
			}
			PlayFabClientAPI.GetTitleData(request, successCallback, failCallback, null, null);
			yield break;
		}

		private void OnGetTitleDataBuildSuccess(GetTitleDataResult result)
		{
			this.fetchTitleDataBuildInProgress = false;
			GTDev.Log<string>("SharedBlocksManager OnGetTitleDataBuildSuccess", null);
			string s;
			if (result.Data.TryGetValue(this.serializationConfig.titleDataKey, out s) && !s.IsNullOrEmpty())
			{
				this.titleDataBuildCache = s;
				this.fetchTitleDataBuildComplete = true;
				Action<string> onGetTitleDataBuildComplete = this.OnGetTitleDataBuildComplete;
				if (onGetTitleDataBuildComplete == null)
				{
					return;
				}
				onGetTitleDataBuildComplete(this.titleDataBuildCache);
				return;
			}
			else
			{
				this.titleDataBuildCache = string.Empty;
				this.fetchTitleDataBuildComplete = true;
				Action<string> onGetTitleDataBuildComplete2 = this.OnGetTitleDataBuildComplete;
				if (onGetTitleDataBuildComplete2 == null)
				{
					return;
				}
				onGetTitleDataBuildComplete2(this.titleDataBuildCache);
				return;
			}
		}

		private void OnGetTitleDataBuildFail(PlayFabError error)
		{
			this.fetchTitleDataBuildInProgress = false;
			GTDev.LogWarning<string>("SharedBlocksManager FetchTitleDataBuildFail " + error.Error.ToString(), null);
			if (error.Error == PlayFabErrorCode.ConnectionError && this.fetchTitleDataRetryCount < this.maxRetriesOnFail)
			{
				float waitTime = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.fetchTitleDataRetryCount + 1)));
				this.fetchTitleDataRetryCount++;
				base.StartCoroutine(this.RetryAfterWaitTime(waitTime, new Action(this.FetchTitleDataBuild)));
				return;
			}
			this.titleDataBuildCache = string.Empty;
			this.fetchTitleDataBuildComplete = true;
			Action<string> onGetTitleDataBuildComplete = this.OnGetTitleDataBuildComplete;
			if (onGetTitleDataBuildComplete == null)
			{
				return;
			}
			onGetTitleDataBuildComplete(this.titleDataBuildCache);
		}

		private string GetPlayfabKeyForSlot(int slot)
		{
			return this.serializationConfig.playfabScanKey + slot.ToString("D2");
		}

		private string GetPlayfabSlotTimeKey(int slot)
		{
			return this.serializationConfig.playfabScanKey + slot.ToString("D2") + this.serializationConfig.timeAppend;
		}

		private void GetPlayfabLastSaveTime()
		{
			if (!this.hasQueriedSaveTime)
			{
				PlayFab.ClientModels.GetUserDataRequest request = new PlayFab.ClientModels.GetUserDataRequest
				{
					PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
					Keys = SharedBlocksManager.saveDateKeys
				};
				try
				{
					PlayFabClientAPI.GetUserData(request, new Action<GetUserDataResult>(this.OnGetLastSaveTimeSuccess), new Action<PlayFabError>(this.OnGetLastSaveTimeFailure), null, null);
				}
				catch (PlayFabException ex)
				{
					this.OnGetLastSaveTimeFailure(new PlayFabError
					{
						Error = PlayFabErrorCode.Unknown,
						ErrorMessage = ex.Message
					});
				}
				this.hasQueriedSaveTime = true;
				return;
			}
			Action onSaveTimeUpdated = SharedBlocksManager.OnSaveTimeUpdated;
			if (onSaveTimeUpdated == null)
			{
				return;
			}
			onSaveTimeUpdated();
		}

		private void OnGetLastSaveTimeSuccess(GetUserDataResult result)
		{
			bool flag = false;
			for (int i = 0; i < BuilderScanKiosk.NUM_SAVE_SLOTS; i++)
			{
				UserDataRecord userDataRecord;
				if (result.Data.TryGetValue(this.GetPlayfabSlotTimeKey(i), out userDataRecord))
				{
					flag = true;
					DateTime lastUpdated = userDataRecord.LastUpdated;
					SharedBlocksManager.SetPublishTimeForSlot(i, lastUpdated + DateTimeOffset.Now.Offset);
				}
			}
			if (flag)
			{
				this.SaveLocalMapIdsToPlayerPrefs();
			}
			Action onSaveTimeUpdated = SharedBlocksManager.OnSaveTimeUpdated;
			if (onSaveTimeUpdated == null)
			{
				return;
			}
			onSaveTimeUpdated();
		}

		private void OnGetLastSaveTimeFailure(PlayFabError error)
		{
			string str = ((error != null) ? error.ErrorMessage : null) ?? "Null";
			GTDev.LogError<string>("SharedBlocksManager GetLastSaveTimeFailure " + str, null);
		}

		private void FetchBuildFromPlayfab()
		{
			if (this.hasPulledPrivateScanPlayfab[this.currentGetScanIndex])
			{
				Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
				if (onFetchPrivateScanComplete != null)
				{
					onFetchPrivateScanComplete(this.currentGetScanIndex, true);
				}
				this.currentGetScanIndex = -1;
				this.getScanInProgress = false;
				return;
			}
			PlayFab.ClientModels.GetUserDataRequest request = new PlayFab.ClientModels.GetUserDataRequest
			{
				PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
				Keys = new List<string>
				{
					this.GetPlayfabKeyForSlot(this.currentGetScanIndex)
				}
			};
			base.StartCoroutine(this.SendPlayfabUserDataRequest(request, new Action<GetUserDataResult>(this.OnFetchBuildFromPlayfabSuccess), new Action<PlayFabError>(this.OnFetchBuildFromPlayfabFail)));
		}

		private IEnumerator SendPlayfabUserDataRequest(PlayFab.ClientModels.GetUserDataRequest request, Action<GetUserDataResult> resultCallback, Action<PlayFabError> errorCallback)
		{
			while (!PlayFabSettings.staticPlayer.IsClientLoggedIn())
			{
				yield return new WaitForSeconds(5f);
			}
			try
			{
				PlayFabClientAPI.GetUserData(request, resultCallback, errorCallback, null, null);
				yield break;
			}
			catch (PlayFabException ex)
			{
				if (errorCallback != null)
				{
					errorCallback(new PlayFabError
					{
						Error = PlayFabErrorCode.Unknown,
						ErrorMessage = ex.Message
					});
				}
				yield break;
			}
			yield break;
		}

		private void OnFetchBuildFromPlayfabSuccess(GetUserDataResult result)
		{
			this.getScanInProgress = false;
			GTDev.Log<string>("SharedBlocksManager OnFetchBuildsFromPlayfabSuccess", null);
			UserDataRecord userDataRecord;
			if (result != null && result.Data != null && result.Data.TryGetValue(this.GetPlayfabKeyForSlot(this.currentGetScanIndex), out userDataRecord))
			{
				this.privateScanDataCache[this.currentGetScanIndex] = userDataRecord.Value;
				this.hasPulledPrivateScanPlayfab[this.currentGetScanIndex] = true;
				if (!userDataRecord.Value.IsNullOrEmpty())
				{
					this.RequestSavePrivateScan(this.currentGetScanIndex, userDataRecord.Value);
				}
			}
			else
			{
				this.privateScanDataCache[this.currentGetScanIndex] = string.Empty;
				this.hasPulledPrivateScanPlayfab[this.currentGetScanIndex] = true;
			}
			Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
			if (onFetchPrivateScanComplete != null)
			{
				onFetchPrivateScanComplete(this.currentGetScanIndex, true);
			}
			this.currentGetScanIndex = -1;
		}

		private void OnFetchBuildFromPlayfabFail(PlayFabError error)
		{
			GTDev.LogWarning<string>("SharedBlocksManager OnFetchBuildsFromPlayfabFail " + (((error != null) ? error.ErrorMessage : null) ?? "Null"), null);
			if (error != null && error.Error == PlayFabErrorCode.ConnectionError && this.fetchPlayfabBuildsRetryCount < this.maxRetriesOnFail)
			{
				float waitTime = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.fetchPlayfabBuildsRetryCount + 1)));
				this.fetchPlayfabBuildsRetryCount++;
				base.StartCoroutine(this.RetryAfterWaitTime(waitTime, new Action(this.FetchBuildFromPlayfab)));
				return;
			}
			this.privateScanDataCache[this.currentGetScanIndex] = string.Empty;
			this.hasPulledPrivateScanPlayfab[this.currentGetScanIndex] = true;
			this.getScanInProgress = false;
			Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
			if (onFetchPrivateScanComplete != null)
			{
				onFetchPrivateScanComplete(this.currentGetScanIndex, false);
			}
			this.currentGetScanIndex = -1;
		}

		private Task WaitForMothership()
		{
			SharedBlocksManager.<WaitForMothership>d__145 <WaitForMothership>d__;
			<WaitForMothership>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WaitForMothership>d__.<>1__state = -1;
			<WaitForMothership>d__.<>t__builder.Start<SharedBlocksManager.<WaitForMothership>d__145>(ref <WaitForMothership>d__);
			return <WaitForMothership>d__.<>t__builder.Task;
		}

		public void RequestSavePrivateScan(int scanIndex, string scanData)
		{
			if (scanIndex < 0 || scanIndex >= this.serializationConfig.scanSlotMothershipKeys.Count)
			{
				GTDev.LogError<string>(string.Format("SharedBlocksManager RequestSaveScanToMothership: scan index {0} out of bounds", scanIndex), null);
				return;
			}
			this.currentSaveScanIndex = scanIndex;
			this.currentSaveScanData = scanData;
			if (!this.hasPulledPrivateScanMothership[scanIndex])
			{
				this.PullMothershipPrivateScanThenPush(scanIndex);
				return;
			}
			this.privateScanDataCache[scanIndex] = scanData;
			this.RequestSetMothershipUserData(this.serializationConfig.scanSlotMothershipKeys[scanIndex], scanData);
		}

		private void PullMothershipPrivateScanThenPush(int scanIndex)
		{
			if (this.getScanInProgress && this.currentGetScanIndex != scanIndex)
			{
				GTDev.LogWarning<string>("SharedBLocksManager PullMothershipPrivateScanThenPush GetScan in progress", null);
				Action<int, string> onSavePrivateScanFailed = this.OnSavePrivateScanFailed;
				if (onSavePrivateScanFailed != null)
				{
					onSavePrivateScanFailed(scanIndex, "ERROR SAVING: BUSY");
				}
				this.currentSaveScanIndex = -1;
				this.currentSaveScanData = string.Empty;
				return;
			}
			this.OnFetchPrivateScanComplete += this.PushMothershipPrivateScan;
			this.RequestFetchPrivateScan(scanIndex);
		}

		private void PushMothershipPrivateScan(int scan, bool success)
		{
			if (scan == this.currentSaveScanIndex)
			{
				this.OnFetchPrivateScanComplete -= this.PushMothershipPrivateScan;
				this.privateScanDataCache[this.currentSaveScanIndex] = this.currentSaveScanData;
				this.RequestSetMothershipUserData(this.serializationConfig.scanSlotMothershipKeys[this.currentSaveScanIndex], this.currentSaveScanData);
			}
		}

		private void RequestSetMothershipUserData(string keyName, string value)
		{
			if (this.saveScanInProgress)
			{
				Debug.LogError("SharedBlocksManager RequestSetMothershipUserData: request already in progress");
				return;
			}
			this.saveScanInProgress = true;
			try
			{
				if (!MothershipClientApiUnity.SetUserDataValue(keyName, value, new Action<SetUserDataResponse>(this.OnSetMothershipUserDataSuccess), new Action<MothershipError, int>(this.OnSetMothershipUserDataFail), ""))
				{
					Debug.LogError("SharedBlocksManager RequestSetMothershipUserData: SetUserDataValue Fail");
					this.OnSetMothershipDataComplete(false);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("SharedBlocksManager RequestSetMothershipUserData: exception " + ex.Message);
				this.OnSetMothershipDataComplete(false);
			}
		}

		private void OnSetMothershipUserDataSuccess(SetUserDataResponse response)
		{
			GTDev.Log<string>("SharedBlocksManager OnSetMothershipUserDataSuccess", null);
			this.OnSetMothershipDataComplete(true);
			response.Dispose();
		}

		private void OnSetMothershipUserDataFail(MothershipError error, int status)
		{
			string str = (error == null) ? status.ToString() : error.Message;
			GTDev.LogError<string>("SharedBlocksManager OnSetMothershipUserDataFail: " + str, null);
			this.OnSetMothershipDataComplete(false);
			if (error != null)
			{
				error.Dispose();
			}
		}

		private void OnSetMothershipDataComplete(bool success)
		{
			this.saveScanInProgress = false;
			if (!BuilderScanKiosk.IsSaveSlotValid(this.currentSaveScanIndex))
			{
				this.currentSaveScanIndex = -1;
				this.currentSaveScanData = string.Empty;
				return;
			}
			if (success)
			{
				this.RequestPublishMap(this.serializationConfig.scanSlotMothershipKeys[this.currentSaveScanIndex]);
				return;
			}
			Action<int, string> onSavePrivateScanFailed = this.OnSavePrivateScanFailed;
			if (onSavePrivateScanFailed != null)
			{
				onSavePrivateScanFailed(this.currentSaveScanIndex, "ERROR SAVING");
			}
			this.currentSaveScanIndex = -1;
			this.currentSaveScanData = string.Empty;
		}

		public bool TryGetPrivateScanResponse(int scanSlot, out string scanData)
		{
			if (scanSlot < 0 || scanSlot >= this.privateScanDataCache.Length || !this.hasPulledPrivateScanMothership[scanSlot])
			{
				scanData = string.Empty;
				return false;
			}
			scanData = this.privateScanDataCache[scanSlot];
			return true;
		}

		public void RequestFetchPrivateScan(int slot)
		{
			if (!BuilderScanKiosk.IsSaveSlotValid(slot))
			{
				GTDev.LogError<string>(string.Format("SharedBlocksManager RequestSaveScan: slot {0} OOB", slot), null);
				slot = Mathf.Clamp(slot, 0, BuilderScanKiosk.NUM_SAVE_SLOTS - 1);
			}
			if (this.hasPulledPrivateScanMothership[slot])
			{
				bool arg = this.privateScanDataCache[slot].Length > 0;
				Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
				if (onFetchPrivateScanComplete == null)
				{
					return;
				}
				onFetchPrivateScanComplete(slot, arg);
				return;
			}
			else
			{
				if (this.getScanInProgress)
				{
					Debug.LogError("SharedBlocksManager RequestFetchPrivateScan: request already in progress");
					if (slot != this.currentGetScanIndex)
					{
						Action<int, bool> onFetchPrivateScanComplete2 = this.OnFetchPrivateScanComplete;
						if (onFetchPrivateScanComplete2 == null)
						{
							return;
						}
						onFetchPrivateScanComplete2(slot, false);
					}
					return;
				}
				this.currentGetScanIndex = slot;
				this.getScanInProgress = true;
				try
				{
					if (!MothershipClientApiUnity.GetUserDataValue(this.serializationConfig.scanSlotMothershipKeys[slot], new Action<MothershipUserData>(this.OnGetMothershipPrivateScanSuccess), new Action<MothershipError, int>(this.OnGetMothershipPrivateScanFail), ""))
					{
						Debug.LogError("SharedBlocksManager RequestFetchPrivateScan failed ");
						this.currentGetScanIndex = -1;
						this.getScanInProgress = false;
						Action<int, bool> onFetchPrivateScanComplete3 = this.OnFetchPrivateScanComplete;
						if (onFetchPrivateScanComplete3 != null)
						{
							onFetchPrivateScanComplete3(slot, false);
						}
					}
				}
				catch (Exception ex)
				{
					Debug.LogError("SharedBlocksManager RequestFetchPrivateScan exception " + ex.Message);
					this.currentGetScanIndex = -1;
					this.getScanInProgress = false;
					Action<int, bool> onFetchPrivateScanComplete4 = this.OnFetchPrivateScanComplete;
					if (onFetchPrivateScanComplete4 != null)
					{
						onFetchPrivateScanComplete4(slot, false);
					}
				}
				return;
			}
		}

		private void OnGetMothershipPrivateScanSuccess(MothershipUserData response)
		{
			GTDev.Log<string>("SharedBlocksManager OnGetMothershipPrivateScanSuccess", null);
			bool flag = response != null && response.value != null && response.value.Length > 0;
			int arg = this.currentGetScanIndex;
			if (response != null)
			{
				this.privateScanDataCache[this.currentGetScanIndex] = response.value;
				this.hasPulledPrivateScanMothership[this.currentGetScanIndex] = true;
				if (flag)
				{
					SharedBlocksManager.LocalPublishInfo publishInfoForSlot = SharedBlocksManager.GetPublishInfoForSlot(this.currentGetScanIndex);
					if (publishInfoForSlot.mapID != null)
					{
						SharedBlocksManager.SharedBlocksMap map = new SharedBlocksManager.SharedBlocksMap
						{
							MapID = publishInfoForSlot.mapID,
							MapData = this.privateScanDataCache[this.currentGetScanIndex],
							CreatorNickName = GorillaTagger.Instance.offlineVRRig.playerNameVisible,
							UpdateTime = DateTime.Now
						};
						this.AddMapToResponseCache(map);
					}
					this.currentGetScanIndex = -1;
					this.getScanInProgress = false;
					Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
					if (onFetchPrivateScanComplete != null)
					{
						onFetchPrivateScanComplete(arg, true);
					}
				}
				else
				{
					this.FetchBuildFromPlayfab();
				}
			}
			else
			{
				this.currentGetScanIndex = -1;
				this.getScanInProgress = false;
				Action<int, bool> onFetchPrivateScanComplete2 = this.OnFetchPrivateScanComplete;
				if (onFetchPrivateScanComplete2 != null)
				{
					onFetchPrivateScanComplete2(arg, false);
				}
			}
			if (response != null)
			{
				response.Dispose();
			}
		}

		private void OnGetMothershipPrivateScanFail(MothershipError error, int status)
		{
			string str = (error == null) ? status.ToString() : error.Message;
			GTDev.LogError<string>("SharedBlocksManager OnGetMothershipPrivateScanFail: " + str, null);
			int arg = this.currentGetScanIndex;
			if (BuilderScanKiosk.IsSaveSlotValid(this.currentGetScanIndex))
			{
				this.privateScanDataCache[this.currentGetScanIndex] = string.Empty;
				this.hasPulledPrivateScanMothership[this.currentGetScanIndex] = true;
			}
			this.getScanInProgress = false;
			this.currentGetScanIndex = -1;
			Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
			if (onFetchPrivateScanComplete != null)
			{
				onFetchPrivateScanComplete(arg, false);
			}
			if (error != null)
			{
				error.Dispose();
			}
		}

		public static SharedBlocksManager instance;

		[SerializeField]
		private BuilderTableSerializationConfig serializationConfig;

		private int maxRetriesOnFail = 3;

		public const int MAP_ID_LENGTH = 8;

		private const string MAP_ID_PATTERN = "^[CFGHKMNPRTWXZ256789]+$";

		public const float MINIMUM_REFRESH_DELAY = 60f;

		public const int VOTE_HISTORY_LENGTH = 10;

		private const int NUM_CACHED_MAP_RESULTS = 5;

		private SharedBlocksManager.StartingMapConfig startingMapConfig = new SharedBlocksManager.StartingMapConfig
		{
			pageNumber = 0,
			pageSize = 10,
			sortMethod = SharedBlocksManager.MapSortMethod.Top.ToString(),
			useMapID = false,
			mapID = null
		};

		private bool hasQueriedSaveTime;

		private static List<string> saveDateKeys = new List<string>(BuilderScanKiosk.NUM_SAVE_SLOTS);

		private bool fetchedTableConfig;

		private int fetchTableConfigRetryCount;

		private string tableConfigResponse;

		private bool fetchTitleDataBuildInProgress;

		private bool fetchTitleDataBuildComplete;

		private int fetchTitleDataRetryCount;

		private string titleDataBuildCache = string.Empty;

		private bool[] hasPulledPrivateScanPlayfab = new bool[BuilderScanKiosk.NUM_SAVE_SLOTS];

		private int fetchPlayfabBuildsRetryCount;

		private readonly int publicSlotIndex = BuilderScanKiosk.NUM_SAVE_SLOTS;

		private string[] privateScanDataCache = new string[BuilderScanKiosk.NUM_SAVE_SLOTS];

		private bool[] hasPulledPrivateScanMothership = new bool[BuilderScanKiosk.NUM_SAVE_SLOTS];

		private bool hasPulledDevScan;

		private string devScanDataCache;

		private bool saveScanInProgress;

		private int currentSaveScanIndex = -1;

		private string currentSaveScanData = string.Empty;

		private bool getScanInProgress;

		private int currentGetScanIndex = -1;

		private int voteRetryCount;

		private bool voteInProgress;

		private bool publishRequestInProgress;

		private int postPublishMapRetryCount;

		private bool getMapDataFromIDInProgress;

		private int getMapDataFromIDRetryCount;

		private bool getTopMapsInProgress;

		private int getTopMapsRetryCount;

		private bool hasCachedTopMaps;

		private double lastGetTopMapsTime = double.MinValue;

		private bool updateMapActiveInProgress;

		private int updateMapActiveRetryCount;

		private List<SharedBlocksManager.SharedBlocksMap> latestPopularMaps = new List<SharedBlocksManager.SharedBlocksMap>();

		private static LinkedList<string> recentUpVotes = new LinkedList<string>();

		private static Dictionary<int, SharedBlocksManager.LocalPublishInfo> localPublishData = new Dictionary<int, SharedBlocksManager.LocalPublishInfo>(BuilderScanKiosk.NUM_SAVE_SLOTS);

		private static List<string> localMapIds = new List<string>(BuilderScanKiosk.NUM_SAVE_SLOTS);

		private List<SharedBlocksManager.SharedBlocksMap> mapResponseCache = new List<SharedBlocksManager.SharedBlocksMap>(5);

		private SharedBlocksManager.SharedBlocksMap defaultMap;

		private bool hasDefaultMap;

		private double defaultMapCacheTime = double.MinValue;

		private bool getDefaultMapInProgress;

		[Serializable]
		public class SharedBlocksMap
		{
			public string MapID { get; set; }

			public string CreatorID { get; set; }

			public string CreatorNickName { get; set; }

			public DateTime CreateTime { get; set; }

			public DateTime UpdateTime { get; set; }

			public string MapData { get; set; }
		}

		[Serializable]
		public struct LocalPublishInfo
		{
			public string mapID;

			public long publishTime;
		}

		[Serializable]
		private class SharedBlocksRequestBase
		{
			public string mothershipId;

			public string mothershipToken;

			public string mothershipEnvId;
		}

		[Serializable]
		private class VoteRequest : SharedBlocksManager.SharedBlocksRequestBase
		{
			public string mapId;

			public int vote;
		}

		[Serializable]
		private class PublishMapRequestData : SharedBlocksManager.SharedBlocksRequestBase
		{
			public string userdataMetadataKey;

			public string playerNickname;
		}

		public enum MapSortMethod
		{
			Top,
			NewlyCreated,
			RecentlyUpdated
		}

		public struct StartingMapConfig
		{
			public int pageNumber;

			public int pageSize;

			public string sortMethod;

			public bool useMapID;

			public string mapID;
		}

		[Serializable]
		private class GetMapsRequest : SharedBlocksManager.SharedBlocksRequestBase
		{
			public int page;

			public int pageSize;

			public string sort;

			public bool ShowInactive;
		}

		[Serializable]
		private class GetMapDataFromIDRequest : SharedBlocksManager.SharedBlocksRequestBase
		{
			public string mapId;
		}

		[Serializable]
		private class GetMapIDFromPlayerRequest : SharedBlocksManager.SharedBlocksRequestBase
		{
			public string requestId;

			public string requestUserDataMetaKey;
		}

		[Serializable]
		private class GetMapIDFromPlayerResponse
		{
			public SharedBlocksManager.SharedBlocksMapMetaData result;

			public int statusCode;

			public string error;
		}

		[Serializable]
		private class SharedBlocksMapMetaData
		{
			public string mapId;

			public string mothershipId;

			public string userDataMetadataKey;

			public string nickname;

			public string createdTime;

			public string updatedTime;

			public int voteCount;

			public bool isActive;
		}

		[Serializable]
		private struct GetMapDataFromPlayerRequestData
		{
			public string CreatorID;

			public string MapScan;

			public SharedBlocksManager.BlocksMapRequestCallback Callback;
		}

		[Serializable]
		private class UpdateMapActiveRequest : SharedBlocksManager.SharedBlocksRequestBase
		{
			public string userdataMetadataKey;

			public bool setActive;
		}

		public delegate void PublishMapRequestCallback(bool success, string key, string mapID, long responseCode);

		public delegate void BlocksMapRequestCallback(SharedBlocksManager.SharedBlocksMap response);
	}
}
