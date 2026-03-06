using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace NexusSDK
{
	public static class ReferralsAPI
	{
		public static IEnumerator StartGetReferralInfoByPlayerIdRequest(ReferralsAPI.GetReferralInfoByPlayerIdRequestParams RequestParams, ReferralsAPI.GetReferralInfoByPlayerIdResponseCallbacks ResponseCallback, ReferralsAPI.ErrorDelegate ErrorCallback)
		{
			if (RequestParams.page > 9999)
			{
				yield break;
			}
			if (RequestParams.page < 1)
			{
				yield break;
			}
			if (RequestParams.pageSize > 100)
			{
				yield break;
			}
			if (RequestParams.pageSize < 1)
			{
				yield break;
			}
			string text = SDKInitializer.ApiBaseUrl + "/player/{playerId}";
			text = text.Replace("{playerId}", RequestParams.playerId);
			List<string> list = new List<string>();
			if (RequestParams.groupId != "")
			{
				list.Add("groupId=" + RequestParams.groupId);
			}
			list.Add("page=" + RequestParams.page.ToString());
			list.Add("pageSize=" + RequestParams.pageSize.ToString());
			list.Add("excludeReferralList=" + RequestParams.excludeReferralList.ToString());
			text += "?";
			text += string.Join("&", list);
			using (UnityWebRequest webRequest = UnityWebRequest.Get(text))
			{
				webRequest.SetRequestHeader("x-shared-secret", SDKInitializer.ApiKey);
				yield return webRequest.SendWebRequest();
				long responseCode = webRequest.responseCode;
				if (responseCode != 200L)
				{
					if (responseCode != 400L)
					{
						if (ErrorCallback != null)
						{
							ErrorCallback(webRequest.responseCode);
						}
					}
					else if (ResponseCallback.OnGetReferralInfoByPlayerId400Response != null)
					{
						ReferralsAPI.ReferralError param = JsonConvert.DeserializeObject<ReferralsAPI.ReferralError>(webRequest.downloadHandler.text, new JsonSerializerSettings
						{
							NullValueHandling = NullValueHandling.Ignore
						});
						ResponseCallback.OnGetReferralInfoByPlayerId400Response(param);
					}
				}
				else if (ResponseCallback.OnGetReferralInfoByPlayerId200Response != null)
				{
					ReferralsAPI.GetReferralInfoByPlayerId200Response param2 = JsonConvert.DeserializeObject<ReferralsAPI.GetReferralInfoByPlayerId200Response>(webRequest.downloadHandler.text, new JsonSerializerSettings
					{
						NullValueHandling = NullValueHandling.Ignore
					});
					ResponseCallback.OnGetReferralInfoByPlayerId200Response(param2);
				}
			}
			UnityWebRequest webRequest = null;
			yield break;
			yield break;
		}

		public static IEnumerator StartGetPlayerCurrentReferralRequest(ReferralsAPI.GetPlayerCurrentReferralRequestParams RequestParams, ReferralsAPI.GetPlayerCurrentReferralResponseCallbacks ResponseCallback, ReferralsAPI.ErrorDelegate ErrorCallback)
		{
			string text = SDKInitializer.ApiBaseUrl + "/player/{playerId}/code";
			text = text.Replace("{playerId}", RequestParams.playerId);
			List<string> list = new List<string>();
			if (RequestParams.groupId != "")
			{
				list.Add("groupId=" + RequestParams.groupId);
			}
			text += "?";
			text += string.Join("&", list);
			using (UnityWebRequest webRequest = UnityWebRequest.Get(text))
			{
				webRequest.SetRequestHeader("x-shared-secret", SDKInitializer.ApiKey);
				yield return webRequest.SendWebRequest();
				long responseCode = webRequest.responseCode;
				if (responseCode != 200L)
				{
					if (responseCode != 404L)
					{
						if (ErrorCallback != null)
						{
							ErrorCallback(webRequest.responseCode);
						}
					}
					else if (ResponseCallback.OnGetPlayerCurrentReferral404Response != null)
					{
						ReferralsAPI.GetPlayerCurrentReferral404Response param = JsonConvert.DeserializeObject<ReferralsAPI.GetPlayerCurrentReferral404Response>(webRequest.downloadHandler.text, new JsonSerializerSettings
						{
							NullValueHandling = NullValueHandling.Ignore
						});
						ResponseCallback.OnGetPlayerCurrentReferral404Response(param);
					}
				}
				else if (ResponseCallback.OnGetPlayerCurrentReferral200Response != null)
				{
					string text2 = webRequest.downloadHandler.text;
					ResponseCallback.OnGetPlayerCurrentReferral200Response(text2);
				}
			}
			UnityWebRequest webRequest = null;
			yield break;
			yield break;
		}

		public static IEnumerator StartGetReferralInfoByCodeRequest(ReferralsAPI.GetReferralInfoByCodeRequestParams RequestParams, ReferralsAPI.GetReferralInfoByCodeResponseCallbacks ResponseCallback, ReferralsAPI.ErrorDelegate ErrorCallback)
		{
			if (RequestParams.page > 9999)
			{
				yield break;
			}
			if (RequestParams.page < 1)
			{
				yield break;
			}
			if (RequestParams.pageSize > 100)
			{
				yield break;
			}
			if (RequestParams.pageSize < 1)
			{
				yield break;
			}
			string text = SDKInitializer.ApiBaseUrl + "/code/{code}";
			text = text.Replace("{code}", RequestParams.code);
			List<string> list = new List<string>();
			if (RequestParams.groupId != "")
			{
				list.Add("groupId=" + RequestParams.groupId);
			}
			list.Add("page=" + RequestParams.page.ToString());
			list.Add("pageSize=" + RequestParams.pageSize.ToString());
			list.Add("excludeReferralList=" + RequestParams.excludeReferralList.ToString());
			text += "?";
			text += string.Join("&", list);
			using (UnityWebRequest webRequest = UnityWebRequest.Get(text))
			{
				webRequest.SetRequestHeader("x-shared-secret", SDKInitializer.ApiKey);
				yield return webRequest.SendWebRequest();
				long responseCode = webRequest.responseCode;
				if (responseCode != 200L)
				{
					if (responseCode != 400L)
					{
						if (ErrorCallback != null)
						{
							ErrorCallback(webRequest.responseCode);
						}
					}
					else if (ResponseCallback.OnGetReferralInfoByCode400Response != null)
					{
						ReferralsAPI.ReferralError param = JsonConvert.DeserializeObject<ReferralsAPI.ReferralError>(webRequest.downloadHandler.text, new JsonSerializerSettings
						{
							NullValueHandling = NullValueHandling.Ignore
						});
						ResponseCallback.OnGetReferralInfoByCode400Response(param);
					}
				}
				else if (ResponseCallback.OnGetReferralInfoByCode200Response != null)
				{
					ReferralsAPI.GetReferralInfoByCode200Response param2 = JsonConvert.DeserializeObject<ReferralsAPI.GetReferralInfoByCode200Response>(webRequest.downloadHandler.text, new JsonSerializerSettings
					{
						NullValueHandling = NullValueHandling.Ignore
					});
					ResponseCallback.OnGetReferralInfoByCode200Response(param2);
				}
			}
			UnityWebRequest webRequest = null;
			yield break;
			yield break;
		}

		public struct Referral
		{
			public string id { readonly get; set; }

			public string code { readonly get; set; }

			public string playerId { readonly get; set; }

			public string playerName { readonly get; set; }

			public DateTime referralDate { readonly get; set; }
		}

		public struct ReferralError
		{
			public string code { readonly get; set; }

			public string message { readonly get; set; }
		}

		public struct ReferralCodeResponse
		{
			public string code { readonly get; set; }

			public bool isPrimary { readonly get; set; }

			public bool isGenerated { readonly get; set; }

			public bool isManaged { readonly get; set; }
		}

		public delegate void ErrorDelegate(long ErrorCode);

		public struct GetReferralInfoByPlayerIdRequestParams
		{
			public string playerId { readonly get; set; }

			public string groupId { readonly get; set; }

			public int page { readonly get; set; }

			public int pageSize { readonly get; set; }

			public bool excludeReferralList { readonly get; set; }
		}

		public struct GetReferralInfoByPlayerId200Response
		{
			public string groupId { readonly get; set; }

			public string groupName { readonly get; set; }

			public ReferralsAPI.ReferralCodeResponse[] codes { readonly get; set; }

			public string playerId { readonly get; set; }

			public string memberId { readonly get; set; }

			public int currentPage { readonly get; set; }

			public int currentPageSize { readonly get; set; }

			public int totalCount { readonly get; set; }

			public ReferralsAPI.Referral[] referrals { readonly get; set; }
		}

		public delegate void OnGetReferralInfoByPlayerId200ResponseDelegate(ReferralsAPI.GetReferralInfoByPlayerId200Response Param0);

		public delegate void OnGetReferralInfoByPlayerId400ResponseDelegate(ReferralsAPI.ReferralError Param0);

		public struct GetReferralInfoByPlayerIdResponseCallbacks
		{
			public ReferralsAPI.OnGetReferralInfoByPlayerId200ResponseDelegate OnGetReferralInfoByPlayerId200Response { readonly get; set; }

			public ReferralsAPI.OnGetReferralInfoByPlayerId400ResponseDelegate OnGetReferralInfoByPlayerId400Response { readonly get; set; }
		}

		public struct GetPlayerCurrentReferralRequestParams
		{
			public string playerId { readonly get; set; }

			public string groupId { readonly get; set; }
		}

		public delegate void OnGetPlayerCurrentReferral200ResponseDelegate(string Param0);

		public struct GetPlayerCurrentReferral404Response
		{
			public string code { readonly get; set; }
		}

		public delegate void OnGetPlayerCurrentReferral404ResponseDelegate(ReferralsAPI.GetPlayerCurrentReferral404Response Param0);

		public struct GetPlayerCurrentReferralResponseCallbacks
		{
			public ReferralsAPI.OnGetPlayerCurrentReferral200ResponseDelegate OnGetPlayerCurrentReferral200Response { readonly get; set; }

			public ReferralsAPI.OnGetPlayerCurrentReferral404ResponseDelegate OnGetPlayerCurrentReferral404Response { readonly get; set; }
		}

		public struct GetReferralInfoByCodeRequestParams
		{
			public string code { readonly get; set; }

			public string groupId { readonly get; set; }

			public int page { readonly get; set; }

			public int pageSize { readonly get; set; }

			public bool excludeReferralList { readonly get; set; }
		}

		public struct GetReferralInfoByCode200Response
		{
			public string groupId { readonly get; set; }

			public string groupName { readonly get; set; }

			public ReferralsAPI.ReferralCodeResponse[] referralCodes { readonly get; set; }

			public string playerId { readonly get; set; }

			public int currentPage { readonly get; set; }

			public int currentPageSize { readonly get; set; }

			public int totalCount { readonly get; set; }

			public ReferralsAPI.Referral[] referrals { readonly get; set; }
		}

		public delegate void OnGetReferralInfoByCode200ResponseDelegate(ReferralsAPI.GetReferralInfoByCode200Response Param0);

		public delegate void OnGetReferralInfoByCode400ResponseDelegate(ReferralsAPI.ReferralError Param0);

		public struct GetReferralInfoByCodeResponseCallbacks
		{
			public ReferralsAPI.OnGetReferralInfoByCode200ResponseDelegate OnGetReferralInfoByCode200Response { readonly get; set; }

			public ReferralsAPI.OnGetReferralInfoByCode400ResponseDelegate OnGetReferralInfoByCode400Response { readonly get; set; }
		}
	}
}
