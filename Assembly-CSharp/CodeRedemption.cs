using System;
using System.Collections;
using GorillaNetworking;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class CodeRedemption : MonoBehaviour
{
	public void Awake()
	{
		if (CodeRedemption.Instance == null)
		{
			CodeRedemption.Instance = this;
			return;
		}
		if (CodeRedemption.Instance != this)
		{
			Object.Destroy(this);
		}
	}

	public void HandleCodeRedemption(string code)
	{
		string text = JsonConvert.SerializeObject(new CodeRedemption.CodeRedemptionRequest
		{
			itemGUID = code,
			playFabID = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			playFabSessionTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			mothershipEnvId = MothershipClientApiUnity.EnvironmentId
		});
		Debug.Log("[CodeRedemption] Web Request body: \n" + text);
		base.StartCoroutine(CodeRedemption.ProcessWebRequest(PlayFabAuthenticatorSettings.HpPromoApiBaseUrl + "/api/ConsumeCodeItem", text, "application/json", new Action<UnityWebRequest>(this.OnCodeRedemptionResponse)));
	}

	private void OnCodeRedemptionResponse(UnityWebRequest completedRequest)
	{
		if (completedRequest.result != UnityWebRequest.Result.Success)
		{
			Debug.LogError("[CodeRedemption] Web Request failed: " + completedRequest.error + "\nDetails: " + completedRequest.downloadHandler.text);
			GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.Invalid;
			return;
		}
		string text = string.Empty;
		try
		{
			CodeRedemption.CodeRedemptionResponse codeRedemptionResponse = JsonConvert.DeserializeObject<CodeRedemption.CodeRedemptionResponse>(completedRequest.downloadHandler.text);
			if (codeRedemptionResponse.result.Contains("AlreadyRedeemed", StringComparison.OrdinalIgnoreCase))
			{
				Debug.Log("[CodeRedemption] Code has already been redeemed!");
				GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.AlreadyUsed;
				return;
			}
			if (codeRedemptionResponse.result.Contains("TooEarly", StringComparison.OrdinalIgnoreCase))
			{
				Debug.Log(string.Format("[CodeRedemption] Code is not redeemable until {0}!", codeRedemptionResponse.startTime));
				GorillaComputer.instance.RedemptionRestrictionTime = codeRedemptionResponse.startTime;
				GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.TooEarly;
				return;
			}
			if (codeRedemptionResponse.result.Contains("TooLate", StringComparison.OrdinalIgnoreCase))
			{
				Debug.Log(string.Format("[CodeRedemption] Code expired at {0}!", codeRedemptionResponse.endTime));
				GorillaComputer.instance.RedemptionRestrictionTime = codeRedemptionResponse.endTime;
				GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.TooLate;
				return;
			}
			text = codeRedemptionResponse.playFabItemName;
		}
		catch (Exception ex)
		{
			string str = "[CodeRedemption] Error parsing JSON response: ";
			Exception ex2 = ex;
			Debug.LogError(str + ((ex2 != null) ? ex2.ToString() : null));
			GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.Invalid;
			return;
		}
		Debug.Log("[CodeRedemption] Item successfully granted, processing external unlock...");
		GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.Success;
		GorillaComputer.instance.RedemptionCode = "";
		base.StartCoroutine(this.CheckProcessExternalUnlock(new string[]
		{
			text
		}, true, true, true));
	}

	private IEnumerator CheckProcessExternalUnlock(string[] itemIDs, bool autoEquip, bool isLeftHand, bool destroyOnFinish)
	{
		Debug.Log("[CodeRedemption] Checking if we can process external cosmetic unlock...");
		while (!CosmeticsController.instance.allCosmeticsDict_isInitialized || !CosmeticsV2Spawner_Dirty.allPartsInstantiated)
		{
			yield return null;
		}
		Debug.Log("[CodeRedemption] Cosmetics initialized, proceeding to process external unlock...");
		foreach (string itemID in itemIDs)
		{
			CosmeticsController.instance.ProcessExternalUnlock(itemID, autoEquip, isLeftHand);
		}
		yield break;
	}

	private static IEnumerator ProcessWebRequest(string url, string data, string contentType, Action<UnityWebRequest> callback)
	{
		UnityWebRequest request = UnityWebRequest.Post(url, data, contentType);
		yield return request.SendWebRequest();
		callback(request);
		yield break;
	}

	public static volatile CodeRedemption Instance;

	private const string HiddenPathCollabEndpoint = "/api/ConsumeCodeItem";

	[Serializable]
	private class CodeRedemptionRequest
	{
		public string itemGUID;

		public string playFabID;

		public string playFabSessionTicket;

		public string mothershipId;

		public string mothershipToken;

		public string mothershipEnvId;
	}

	[Serializable]
	private class CodeRedemptionResponse
	{
		public string result;

		public string itemID;

		public string playFabItemName;

		public DateTimeOffset? startTime;

		public DateTimeOffset? endTime;
	}
}
