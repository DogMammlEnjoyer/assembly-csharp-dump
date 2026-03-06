using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace NexusSDK
{
	public static class AttributionAPI
	{
		public static IEnumerator StartGetMembersRequest(AttributionAPI.GetMembersRequestParams RequestParams, AttributionAPI.OnGetMembers200ResponseDelegate ResponseCallback, AttributionAPI.ErrorDelegate ErrorCallback)
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
			string text = SDKInitializer.ApiBaseUrl + "/manage/members";
			List<string> list = new List<string>();
			list.Add("page=" + RequestParams.page.ToString());
			list.Add("pageSize=" + RequestParams.pageSize.ToString());
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
				if (webRequest.responseCode == 200L)
				{
					if (ResponseCallback != null)
					{
						AttributionAPI.GetMembers200Response param = JsonConvert.DeserializeObject<AttributionAPI.GetMembers200Response>(webRequest.downloadHandler.text, new JsonSerializerSettings
						{
							NullValueHandling = NullValueHandling.Ignore
						});
						if (ResponseCallback != null)
						{
							ResponseCallback(param);
						}
					}
				}
				else if (ErrorCallback != null)
				{
					ErrorCallback(webRequest.responseCode);
				}
			}
			UnityWebRequest webRequest = null;
			yield break;
			yield break;
		}

		public static IEnumerator StartGetMemberByCodeOrUuidRequest(AttributionAPI.GetMemberByCodeOrUuidRequestParams RequestParams, AttributionAPI.OnGetMemberByCodeOrUuid200ResponseDelegate ResponseCallback, AttributionAPI.ErrorDelegate ErrorCallback)
		{
			string text = SDKInitializer.ApiBaseUrl + "/manage/members/{memberCodeOrID}";
			text = text.Replace("{memberCodeOrID}", RequestParams.memberCodeOrID);
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
				if (webRequest.responseCode == 200L)
				{
					if (ResponseCallback != null)
					{
						AttributionAPI.Member param = JsonConvert.DeserializeObject<AttributionAPI.Member>(webRequest.downloadHandler.text, new JsonSerializerSettings
						{
							NullValueHandling = NullValueHandling.Ignore
						});
						if (ResponseCallback != null)
						{
							ResponseCallback(param);
						}
					}
				}
				else if (ErrorCallback != null)
				{
					ErrorCallback(webRequest.responseCode);
				}
			}
			UnityWebRequest webRequest = null;
			yield break;
			yield break;
		}

		public static IEnumerator StartGetMemberByPlayerIdRequest(AttributionAPI.GetMemberByPlayerIdRequestParams RequestParams, AttributionAPI.OnGetMemberByPlayerId200ResponseDelegate ResponseCallback, AttributionAPI.ErrorDelegate ErrorCallback)
		{
			string text = SDKInitializer.ApiBaseUrl + "/manage/members/player/{playerId}";
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
				if (webRequest.responseCode == 200L)
				{
					if (ResponseCallback != null)
					{
						AttributionAPI.Member param = JsonConvert.DeserializeObject<AttributionAPI.Member>(webRequest.downloadHandler.text, new JsonSerializerSettings
						{
							NullValueHandling = NullValueHandling.Ignore
						});
						if (ResponseCallback != null)
						{
							ResponseCallback(param);
						}
					}
				}
				else if (ErrorCallback != null)
				{
					ErrorCallback(webRequest.responseCode);
				}
			}
			UnityWebRequest webRequest = null;
			yield break;
			yield break;
		}

		public static IEnumerator StartGetAttributionsPingRequest(AttributionAPI.OnGetAttributionsPing200ResponseDelegate ResponseCallback, AttributionAPI.ErrorDelegate ErrorCallback)
		{
			string uri = SDKInitializer.ApiBaseUrl + "/attributions/ping";
			using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
			{
				webRequest.SetRequestHeader("x-shared-secret", SDKInitializer.ApiKey);
				yield return webRequest.SendWebRequest();
				if (webRequest.responseCode == 200L)
				{
					if (ResponseCallback != null)
					{
						if (ResponseCallback != null)
						{
							ResponseCallback();
						}
					}
				}
				else if (ErrorCallback != null)
				{
					ErrorCallback(webRequest.responseCode);
				}
			}
			UnityWebRequest webRequest = null;
			yield break;
			yield break;
		}

		public struct Code
		{
			public string code { readonly get; set; }

			public bool isPrimary { readonly get; set; }

			public bool isGenerated { readonly get; set; }

			public bool isManaged { readonly get; set; }
		}

		public enum Currency
		{
			AED,
			AFN,
			ALL,
			AMD,
			ANG,
			AOA,
			ARS,
			AUD,
			AWG,
			AZN,
			BAM,
			BBD,
			BDT,
			BGN,
			BHD,
			BIF,
			BMD,
			BND,
			BOB,
			BRL,
			BSD,
			BTC,
			BTN,
			BWP,
			BYN,
			BYR,
			BZD,
			CAD,
			CDF,
			CHF,
			CLF,
			CLP,
			CNY,
			COP,
			CRC,
			CUC,
			CUP,
			CVE,
			CZK,
			DJF,
			DKK,
			DOP,
			DZD,
			EGP,
			ERN,
			ETB,
			EUR,
			FJD,
			FKP,
			GBP,
			GEL,
			GGP,
			GHS,
			GIP,
			GMD,
			GNF,
			GTQ,
			GYD,
			HKD,
			HNL,
			HRK,
			HTG,
			HUF,
			IDR,
			ILS,
			IMP,
			INR,
			IQD,
			IRR,
			ISK,
			JEP,
			JMD,
			JOD,
			JPY,
			KES,
			KGS,
			KHR,
			KMF,
			KPW,
			KRW,
			KWD,
			KYD,
			KZT,
			LAK,
			LBP,
			LKR,
			LRD,
			LSL,
			LTL,
			LVL,
			LYD,
			MAD,
			MDL,
			MGA,
			MKD,
			MMK,
			MNT,
			MOP,
			MRO,
			MUR,
			MVR,
			MWK,
			MXN,
			MYR,
			MZN,
			NAD,
			NGN,
			NIO,
			NOK,
			NPR,
			NZD,
			OMR,
			PAB,
			PEN,
			PGK,
			PHP,
			PKR,
			PLN,
			PYG,
			QAR,
			RON,
			RSD,
			RUB,
			RWF,
			SAR,
			SBD,
			SCR,
			SDG,
			SEK,
			SGD,
			SHP,
			SLL,
			SOS,
			SRD,
			STD,
			SVC,
			SYP,
			SZL,
			THB,
			TJS,
			TMT,
			TND,
			TOP,
			TRY,
			TTD,
			TWD,
			TZS,
			UAH,
			UGX,
			USD,
			UYU,
			UZS,
			VEF,
			VND,
			VUV,
			WST,
			XAF,
			XAG,
			XAU,
			XCD,
			XDR,
			XOF,
			XPF,
			YER,
			ZAR,
			ZMK,
			ZMW,
			ZWL
		}

		public struct Metrics
		{
			public DateTime joinDate { readonly get; set; }

			public AttributionAPI.Metrics.conversion_Struct conversion { readonly get; set; }

			public struct conversion_Struct
			{
				public DateTime lastPurchaseDate { readonly get; set; }

				public AttributionAPI.Metrics.conversion_Struct.totalSpendToDate_Struct totalSpendToDate { readonly get; set; }

				public struct totalSpendToDate_Struct
				{
					public double total { readonly get; set; }

					public string currency { readonly get; set; }
				}
			}
		}

		public struct Transaction
		{
			public string id { readonly get; set; }

			public string memberId { readonly get; set; }

			public string code { readonly get; set; }

			public string memberPlayerId { readonly get; set; }

			public string description { readonly get; set; }

			public string status { readonly get; set; }

			public double subtotal { readonly get; set; }

			public string currency { readonly get; set; }

			public double total { readonly get; set; }

			public string totalCurrency { readonly get; set; }

			public string transactionId { readonly get; set; }

			public DateTime transactionDate { readonly get; set; }

			public string platform { readonly get; set; }

			public string playerId { readonly get; set; }

			public string playerName { readonly get; set; }

			public AttributionAPI.Metrics metrics { readonly get; set; }

			public double memberShareAmount { readonly get; set; }

			public double memberSharePercent { readonly get; set; }

			public bool memberPaid { readonly get; set; }

			public string skuId { readonly get; set; }
		}

		public struct Member
		{
			public string id { readonly get; set; }

			public string name { readonly get; set; }

			public string playerId { readonly get; set; }

			public AttributionAPI.PlayerMetadata playerMetadata { readonly get; set; }

			public string logoImage { readonly get; set; }

			public string profileImage { readonly get; set; }

			public AttributionAPI.Code[] codes { readonly get; set; }
		}

		public struct PlayerMetadata
		{
			public string displayName { readonly get; set; }
		}

		public struct CreatorGroup
		{
			public string name { readonly get; set; }

			public string id { readonly get; set; }

			public bool isDefault { readonly get; set; }
		}

		public struct ScheduledRevShare
		{
			public string id { readonly get; set; }

			public double revShare { readonly get; set; }

			public DateTime startDate { readonly get; set; }

			public DateTime endDate { readonly get; set; }

			public string groupId { readonly get; set; }

			public string groupName { readonly get; set; }

			public AttributionAPI.TierRevenueShare[] tierRevenueShares { readonly get; set; }
		}

		public struct CreatorGroupTier
		{
			public string id { readonly get; set; }

			public string name { readonly get; set; }

			public double revShare { readonly get; set; }

			public double memberCount { readonly get; set; }
		}

		public struct TierRevenueShare
		{
			public double revShare { readonly get; set; }

			public string tierId { readonly get; set; }

			public string tierName { readonly get; set; }
		}

		public struct APIError
		{
			public string code { readonly get; set; }

			public string message { readonly get; set; }
		}

		public delegate void ErrorDelegate(long ErrorCode);

		public struct GetMembersRequestParams
		{
			public int page { readonly get; set; }

			public int pageSize { readonly get; set; }

			public string groupId { readonly get; set; }
		}

		public struct GetMembers200Response
		{
			public string groupId { readonly get; set; }

			public string groupName { readonly get; set; }

			public int currentPage { readonly get; set; }

			public int currentPageSize { readonly get; set; }

			public int totalCount { readonly get; set; }

			public AttributionAPI.Member[] members { readonly get; set; }
		}

		public delegate void OnGetMembers200ResponseDelegate(AttributionAPI.GetMembers200Response Param0);

		public struct GetMemberByCodeOrUuidRequestParams
		{
			public string memberCodeOrID { readonly get; set; }

			public string groupId { readonly get; set; }
		}

		public delegate void OnGetMemberByCodeOrUuid200ResponseDelegate(AttributionAPI.Member Param0);

		public struct GetMemberByPlayerIdRequestParams
		{
			public string playerId { readonly get; set; }

			public string groupId { readonly get; set; }
		}

		public delegate void OnGetMemberByPlayerId200ResponseDelegate(AttributionAPI.Member Param0);

		public delegate void OnGetAttributionsPing200ResponseDelegate();
	}
}
