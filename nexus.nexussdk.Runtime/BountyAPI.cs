using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace NexusSDK
{
	public static class BountyAPI
	{
		public static IEnumerator StartGetBountiesRequest(BountyAPI.GetBountiesRequestParams RequestParams, BountyAPI.GetBountiesResponseCallbacks ResponseCallback, BountyAPI.ErrorDelegate ErrorCallback)
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
			string text = SDKInitializer.ApiBaseUrl + "/";
			List<string> list = new List<string>();
			if (RequestParams.groupId != "")
			{
				list.Add("groupId=" + RequestParams.groupId);
			}
			list.Add("page=" + RequestParams.page.ToString());
			list.Add("pageSize=" + RequestParams.pageSize.ToString());
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
					else if (ResponseCallback.OnGetBounties400Response != null)
					{
						BountyAPI.BountyError param = JsonConvert.DeserializeObject<BountyAPI.BountyError>(webRequest.downloadHandler.text, new JsonSerializerSettings
						{
							NullValueHandling = NullValueHandling.Ignore
						});
						ResponseCallback.OnGetBounties400Response(param);
					}
				}
				else if (ResponseCallback.OnGetBounties200Response != null)
				{
					BountyAPI.GetBounties200Response param2 = JsonConvert.DeserializeObject<BountyAPI.GetBounties200Response>(webRequest.downloadHandler.text, new JsonSerializerSettings
					{
						NullValueHandling = NullValueHandling.Ignore
					});
					ResponseCallback.OnGetBounties200Response(param2);
				}
			}
			UnityWebRequest webRequest = null;
			yield break;
			yield break;
		}

		public static IEnumerator StartGetBountyRequest(BountyAPI.GetBountyRequestParams RequestParams, BountyAPI.GetBountyResponseCallbacks ResponseCallback, BountyAPI.ErrorDelegate ErrorCallback)
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
			string text = SDKInitializer.ApiBaseUrl + "/{bountyId}";
			text = text.Replace("{bountyId}", RequestParams.bountyId);
			List<string> list = new List<string>();
			if (RequestParams.groupId != "")
			{
				list.Add("groupId=" + RequestParams.groupId);
			}
			list.Add("includeProgress=" + RequestParams.includeProgress.ToString());
			list.Add("page=" + RequestParams.page.ToString());
			list.Add("pageSize=" + RequestParams.pageSize.ToString());
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
					else if (ResponseCallback.OnGetBounty400Response != null)
					{
						BountyAPI.BountyError param = JsonConvert.DeserializeObject<BountyAPI.BountyError>(webRequest.downloadHandler.text, new JsonSerializerSettings
						{
							NullValueHandling = NullValueHandling.Ignore
						});
						ResponseCallback.OnGetBounty400Response(param);
					}
				}
				else if (ResponseCallback.OnGetBounty200Response != null)
				{
					BountyAPI.GetBounty200Response param2 = JsonConvert.DeserializeObject<BountyAPI.GetBounty200Response>(webRequest.downloadHandler.text, new JsonSerializerSettings
					{
						NullValueHandling = NullValueHandling.Ignore
					});
					ResponseCallback.OnGetBounty200Response(param2);
				}
			}
			UnityWebRequest webRequest = null;
			yield break;
			yield break;
		}

		public static IEnumerator StartGetMemberBountiesByIDRequest(BountyAPI.GetMemberBountiesByIDRequestParams RequestParams, BountyAPI.GetMemberBountiesByIDResponseCallbacks ResponseCallback, BountyAPI.ErrorDelegate ErrorCallback)
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
			string text = SDKInitializer.ApiBaseUrl + "/member/id/{memberId}";
			text = text.Replace("{memberId}", RequestParams.memberId);
			List<string> list = new List<string>();
			if (RequestParams.groupId != "")
			{
				list.Add("groupId=" + RequestParams.groupId);
			}
			list.Add("page=" + RequestParams.page.ToString());
			list.Add("pageSize=" + RequestParams.pageSize.ToString());
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
					else if (ResponseCallback.OnGetMemberBountiesByID400Response != null)
					{
						BountyAPI.BountyError param = JsonConvert.DeserializeObject<BountyAPI.BountyError>(webRequest.downloadHandler.text, new JsonSerializerSettings
						{
							NullValueHandling = NullValueHandling.Ignore
						});
						ResponseCallback.OnGetMemberBountiesByID400Response(param);
					}
				}
				else if (ResponseCallback.OnGetMemberBountiesByID200Response != null)
				{
					BountyAPI.GetMemberBountiesByID200Response param2 = JsonConvert.DeserializeObject<BountyAPI.GetMemberBountiesByID200Response>(webRequest.downloadHandler.text, new JsonSerializerSettings
					{
						NullValueHandling = NullValueHandling.Ignore
					});
					ResponseCallback.OnGetMemberBountiesByID200Response(param2);
				}
			}
			UnityWebRequest webRequest = null;
			yield break;
			yield break;
		}

		public struct Bounty
		{
			public string id { readonly get; set; }

			public string name { readonly get; set; }

			public string description { readonly get; set; }

			public string imageSrc { readonly get; set; }

			public string rewardDescription { readonly get; set; }

			public double limit { readonly get; set; }

			public DateTime startsAt { readonly get; set; }

			public DateTime endsAt { readonly get; set; }

			public DateTime lastProgressCheck { readonly get; set; }

			public BountyAPI.BountyObjective[] objectives { readonly get; set; }

			public BountyAPI.BountyReward[] rewards { readonly get; set; }

			public BountyAPI.Bounty.dependants_Struct_Element[] dependants { readonly get; set; }

			public BountyAPI.Bounty.prerequisites_Struct_Element[] prerequisites { readonly get; set; }

			public struct dependants_Struct_Element
			{
				public string id { readonly get; set; }

				public string name { readonly get; set; }
			}

			public struct prerequisites_Struct_Element
			{
				public string id { readonly get; set; }

				public string name { readonly get; set; }
			}
		}

		public struct BountySku
		{
			public string id { readonly get; set; }

			public string name { readonly get; set; }

			public string slug { readonly get; set; }
		}

		public struct BountyObjective
		{
			public string id { readonly get; set; }

			public string name { readonly get; set; }

			public string type { readonly get; set; }

			public string condition { readonly get; set; }

			public double count { readonly get; set; }

			public string eventType { readonly get; set; }

			public string eventCode { readonly get; set; }

			public string nexusPurchaseObjectiveType { readonly get; set; }

			public BountyAPI.BountySku[] skus { readonly get; set; }

			public BountyAPI.BountyObjective.category_Struct category { readonly get; set; }

			public BountyAPI.BountyObjective.publisher_Struct publisher { readonly get; set; }

			public struct category_Struct
			{
				public string id { readonly get; set; }

				public string name { readonly get; set; }

				public string slug { readonly get; set; }
			}

			public struct publisher_Struct
			{
				public string id { readonly get; set; }

				public string name { readonly get; set; }
			}
		}

		public struct BountyReward
		{
			public string id { readonly get; set; }

			public string name { readonly get; set; }

			public string type { readonly get; set; }

			public BountyAPI.BountySku sku { readonly get; set; }

			public double amount { readonly get; set; }

			public string currency { readonly get; set; }

			public string externalId { readonly get; set; }
		}

		public struct BountyProgress
		{
			public string id { readonly get; set; }

			public bool completed { readonly get; set; }

			public double completionCount { readonly get; set; }

			public DateTime lastProgressCheck { readonly get; set; }

			public string currentObjectiveGroupId { readonly get; set; }

			public BountyAPI.BountyObjectiveProgress[] currentObjectives { readonly get; set; }

			public BountyAPI.BountyProgress.completedObjectives_Struct_Element[] completedObjectives { readonly get; set; }

			public BountyAPI.BountyProgress.member_Struct member { readonly get; set; }

			public struct completedObjectives_Struct_Element
			{
				public string objectiveGroupId { readonly get; set; }

				public BountyAPI.BountyObjectiveProgress[] objectives { readonly get; set; }

				public BountyAPI.BountyProgressReward[] rewards { readonly get; set; }
			}

			public struct member_Struct
			{
				public string id { readonly get; set; }

				public string playerId { readonly get; set; }

				public string name { readonly get; set; }

				public BountyAPI.Code[] codes { readonly get; set; }
			}
		}

		public struct Code
		{
			public string code { readonly get; set; }

			public bool isPrimary { readonly get; set; }

			public bool isGenerated { readonly get; set; }

			public bool isManaged { readonly get; set; }
		}

		public struct BountyObjectiveProgress
		{
			public string id { readonly get; set; }

			public bool completed { readonly get; set; }

			public double count { readonly get; set; }

			public BountyAPI.BountyObjectiveProgress.objective_Struct objective { readonly get; set; }

			public struct objective_Struct
			{
				public string id { readonly get; set; }

				public string name { readonly get; set; }

				public double count { readonly get; set; }

				public string condition { readonly get; set; }
			}
		}

		public struct BountyProgressReward
		{
			public string id { readonly get; set; }

			public string name { readonly get; set; }

			public string externalId { readonly get; set; }

			public bool rewardCompleted { readonly get; set; }

			public string rewardReferenceId { readonly get; set; }
		}

		public struct Creator
		{
			public string id { readonly get; set; }

			public string playerId { readonly get; set; }

			public string name { readonly get; set; }

			public string[] slugs { readonly get; set; }
		}

		public struct CreatorGroupEvent
		{
			public string eventCode { readonly get; set; }

			public string playerId { readonly get; set; }

			public string referralCode { readonly get; set; }
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

		public struct BountyError
		{
			public string code { readonly get; set; }

			public string message { readonly get; set; }
		}

		public delegate void ErrorDelegate(long ErrorCode);

		public struct GetBountiesRequestParams
		{
			public string groupId { readonly get; set; }

			public int page { readonly get; set; }

			public int pageSize { readonly get; set; }
		}

		public struct GetBounties200Response
		{
			public string groupId { readonly get; set; }

			public string groupName { readonly get; set; }

			public int currentPage { readonly get; set; }

			public int currentPageSize { readonly get; set; }

			public int totalCount { readonly get; set; }

			public BountyAPI.Bounty[] bounties { readonly get; set; }
		}

		public delegate void OnGetBounties200ResponseDelegate(BountyAPI.GetBounties200Response Param0);

		public delegate void OnGetBounties400ResponseDelegate(BountyAPI.BountyError Param0);

		public struct GetBountiesResponseCallbacks
		{
			public BountyAPI.OnGetBounties200ResponseDelegate OnGetBounties200Response { readonly get; set; }

			public BountyAPI.OnGetBounties400ResponseDelegate OnGetBounties400Response { readonly get; set; }
		}

		public struct GetBountyRequestParams
		{
			public string groupId { readonly get; set; }

			public bool includeProgress { readonly get; set; }

			public int page { readonly get; set; }

			public int pageSize { readonly get; set; }

			public string bountyId { readonly get; set; }
		}

		public struct GetBounty200Response
		{
			public string groupId { readonly get; set; }

			public string groupName { readonly get; set; }

			public BountyAPI.Bounty bounty { readonly get; set; }

			public BountyAPI.GetBounty200Response.progress_Struct progress { readonly get; set; }

			public struct progress_Struct
			{
				public int currentPage { readonly get; set; }

				public int currentPageSize { readonly get; set; }

				public int totalCount { readonly get; set; }

				public BountyAPI.GetBounty200Response.progress_Struct.data_Struct_Element[] data { readonly get; set; }

				public struct data_Struct_Element
				{
					public string id { readonly get; set; }

					public bool completed { readonly get; set; }

					public double completionCount { readonly get; set; }

					public DateTime lastProgressCheck { readonly get; set; }

					public string currentObjectiveGroupId { readonly get; set; }

					public BountyAPI.BountyObjectiveProgress[] currentObjectives { readonly get; set; }

					public BountyAPI.BountyProgress.completedObjectives_Struct_Element[] completedObjectives { readonly get; set; }

					public BountyAPI.BountyProgress.member_Struct member { readonly get; set; }

					public BountyAPI.Creator creator { readonly get; set; }

					public struct completedObjectives_Struct_Element
					{
						public string objectiveGroupId { readonly get; set; }

						public BountyAPI.BountyObjectiveProgress[] objectives { readonly get; set; }

						public BountyAPI.BountyProgressReward[] rewards { readonly get; set; }
					}

					public struct member_Struct
					{
						public string id { readonly get; set; }

						public string playerId { readonly get; set; }

						public string name { readonly get; set; }

						public BountyAPI.Code[] codes { readonly get; set; }
					}
				}
			}
		}

		public delegate void OnGetBounty200ResponseDelegate(BountyAPI.GetBounty200Response Param0);

		public delegate void OnGetBounty400ResponseDelegate(BountyAPI.BountyError Param0);

		public struct GetBountyResponseCallbacks
		{
			public BountyAPI.OnGetBounty200ResponseDelegate OnGetBounty200Response { readonly get; set; }

			public BountyAPI.OnGetBounty400ResponseDelegate OnGetBounty400Response { readonly get; set; }
		}

		public struct GetMemberBountiesByIDRequestParams
		{
			public string groupId { readonly get; set; }

			public int page { readonly get; set; }

			public int pageSize { readonly get; set; }

			public string memberId { readonly get; set; }
		}

		public struct GetMemberBountiesByID200Response
		{
			public string groupId { readonly get; set; }

			public string groupName { readonly get; set; }

			public int currentPage { readonly get; set; }

			public int currentPageSize { readonly get; set; }

			public int totalCount { readonly get; set; }

			public string memberId { readonly get; set; }

			public string playerId { readonly get; set; }

			public BountyAPI.GetMemberBountiesByID200Response.progress_Struct_Element[] progress { readonly get; set; }

			public struct progress_Struct_Element
			{
				public string id { readonly get; set; }

				public bool completed { readonly get; set; }

				public double completionCount { readonly get; set; }

				public DateTime lastProgressCheck { readonly get; set; }

				public string currentObjectiveGroupId { readonly get; set; }

				public BountyAPI.BountyObjectiveProgress[] currentObjectives { readonly get; set; }

				public BountyAPI.BountyProgress.completedObjectives_Struct_Element[] completedObjectives { readonly get; set; }

				public BountyAPI.BountyProgress.member_Struct member { readonly get; set; }

				public string name { readonly get; set; }

				public double limit { readonly get; set; }

				public struct completedObjectives_Struct_Element
				{
					public string objectiveGroupId { readonly get; set; }

					public BountyAPI.BountyObjectiveProgress[] objectives { readonly get; set; }

					public BountyAPI.BountyProgressReward[] rewards { readonly get; set; }
				}

				public struct member_Struct
				{
					public string id { readonly get; set; }

					public string playerId { readonly get; set; }

					public string name { readonly get; set; }

					public BountyAPI.Code[] codes { readonly get; set; }
				}
			}
		}

		public delegate void OnGetMemberBountiesByID200ResponseDelegate(BountyAPI.GetMemberBountiesByID200Response Param0);

		public delegate void OnGetMemberBountiesByID400ResponseDelegate(BountyAPI.BountyError Param0);

		public struct GetMemberBountiesByIDResponseCallbacks
		{
			public BountyAPI.OnGetMemberBountiesByID200ResponseDelegate OnGetMemberBountiesByID200Response { readonly get; set; }

			public BountyAPI.OnGetMemberBountiesByID400ResponseDelegate OnGetMemberBountiesByID400Response { readonly get; set; }
		}
	}
}
