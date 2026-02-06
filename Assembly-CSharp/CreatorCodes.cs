using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public static class CreatorCodes
{
	public static string getCurrentCreatorCode(string id)
	{
		if (id.IsNullOrEmpty())
		{
			return string.Empty;
		}
		if (CreatorCodes.data.currentCreatorCode == null)
		{
			return string.Empty;
		}
		if (!CreatorCodes.data.currentCreatorCode.ContainsKey(id))
		{
			return string.Empty;
		}
		return CreatorCodes.data.currentCreatorCode[id];
	}

	public static CreatorCodes.CreatorCodeStatus getCurrentCreatorCodeStatus(string id)
	{
		if (id == null)
		{
			return CreatorCodes.CreatorCodeStatus.Empty;
		}
		if (CreatorCodes.creatorCodeStatus == null)
		{
			return CreatorCodes.CreatorCodeStatus.Empty;
		}
		if (!CreatorCodes.creatorCodeStatus.ContainsKey(id))
		{
			return CreatorCodes.CreatorCodeStatus.Empty;
		}
		return CreatorCodes.creatorCodeStatus[id];
	}

	public static event Action<string> OnCreatorCodeChangedEvent;

	public static event Action InitializedEvent;

	public static event Action<string, string, NexusGroupId> OnCreatorCodeValidEvent;

	public static event Action<string> OnCreatorCodeFailureEvent;

	public static void Initialize()
	{
		CreatorCodes.ValidatedCreatorCode = new Dictionary<string, NexusManager.MemberCode>();
		CreatorCodes.creatorCodeStatus = new Dictionary<string, CreatorCodes.CreatorCodeStatus>();
		CreatorCodes.LoadData();
		CreatorCodes.Intialized = true;
		Action initializedEvent = CreatorCodes.InitializedEvent;
		if (initializedEvent == null)
		{
			return;
		}
		initializedEvent();
	}

	public static void DeleteCharacter(string id)
	{
		if (CreatorCodes.data.currentCreatorCode.ContainsKey(id) && CreatorCodes.data.currentCreatorCode[id].Length > 0)
		{
			CreatorCodes.data.currentCreatorCode[id] = CreatorCodes.data.currentCreatorCode[id].Substring(0, CreatorCodes.data.currentCreatorCode[id].Length - 1);
			CreatorCodes.ValidatedCreatorCode[id] = null;
			CreatorCodes.creatorCodeStatus[id] = ((CreatorCodes.data.currentCreatorCode[id].Length == 0) ? CreatorCodes.CreatorCodeStatus.Empty : CreatorCodes.CreatorCodeStatus.Unchecked);
			Action<string> onCreatorCodeChangedEvent = CreatorCodes.OnCreatorCodeChangedEvent;
			if (onCreatorCodeChangedEvent == null)
			{
				return;
			}
			onCreatorCodeChangedEvent(id);
		}
	}

	public static void AppendKey(string id, string input)
	{
		if (!CreatorCodes.data.currentCreatorCode.ContainsKey(id))
		{
			CreatorCodes.data.currentCreatorCode[id] = string.Empty;
		}
		if (CreatorCodes.data.currentCreatorCode[id].Length < 10)
		{
			Dictionary<string, string> currentCreatorCode = CreatorCodes.data.currentCreatorCode;
			currentCreatorCode[id] += input;
			CreatorCodes.ValidatedCreatorCode[id] = null;
			CreatorCodes.creatorCodeStatus[id] = CreatorCodes.CreatorCodeStatus.Unchecked;
			Action<string> onCreatorCodeChangedEvent = CreatorCodes.OnCreatorCodeChangedEvent;
			if (onCreatorCodeChangedEvent == null)
			{
				return;
			}
			onCreatorCodeChangedEvent(id);
		}
	}

	public static void ResetCreatorCode(string id)
	{
		Debug.Log("Resetting creator code");
		CreatorCodes.data.currentCreatorCode[id] = "";
		CreatorCodes.creatorCodeStatus[id] = CreatorCodes.CreatorCodeStatus.Empty;
		CreatorCodes.supportedMember = default(Member);
		CreatorCodes.ValidatedCreatorCode[id] = null;
		CreatorCodes.SaveData();
		Action<string> onCreatorCodeChangedEvent = CreatorCodes.OnCreatorCodeChangedEvent;
		if (onCreatorCodeChangedEvent == null)
		{
			return;
		}
		onCreatorCodeChangedEvent(id);
	}

	public static Task<NexusManager.MemberCode> CheckValidationCoroutineJIT(string terminalId, string code, NexusGroupId[] group)
	{
		CreatorCodes.<CheckValidationCoroutineJIT>d__27 <CheckValidationCoroutineJIT>d__;
		<CheckValidationCoroutineJIT>d__.<>t__builder = AsyncTaskMethodBuilder<NexusManager.MemberCode>.Create();
		<CheckValidationCoroutineJIT>d__.terminalId = terminalId;
		<CheckValidationCoroutineJIT>d__.code = code;
		<CheckValidationCoroutineJIT>d__.group = group;
		<CheckValidationCoroutineJIT>d__.<>1__state = -1;
		<CheckValidationCoroutineJIT>d__.<>t__builder.Start<CreatorCodes.<CheckValidationCoroutineJIT>d__27>(ref <CheckValidationCoroutineJIT>d__);
		return <CheckValidationCoroutineJIT>d__.<>t__builder.Task;
	}

	private static void SaveData()
	{
		PlayerPrefs.SetString("CreatorCodes_Store", JsonConvert.SerializeObject(CreatorCodes.data));
	}

	private static void LoadData()
	{
		string @string = PlayerPrefs.GetString("CreatorCodes_Store", string.Empty);
		if (@string.Length == 0)
		{
			return;
		}
		CreatorCodes.data = JsonConvert.DeserializeObject<CreatorCodes.CreatorCodesData>(@string);
		foreach (string key in CreatorCodes.data.currentCreatorCode.Keys)
		{
			if (CreatorCodes.data.codeFirstUsedTime.ContainsKey(key) && DateTime.UtcNow.Subtract(CreatorCodes.data.codeFirstUsedTime[key]).Days > 14)
			{
				CreatorCodes.data.currentCreatorCode[key] = string.Empty;
			}
		}
	}

	private const int MAX_CODE_LENGTH = 10;

	private const string PLAYER_PREF_KEY = "CreatorCodes_Store";

	private const int DAYS_TO_STORE_CODE = 14;

	private static CreatorCodes.CreatorCodesData data = new CreatorCodes.CreatorCodesData();

	private static Dictionary<string, NexusManager.MemberCode> ValidatedCreatorCode;

	private static Dictionary<string, CreatorCodes.CreatorCodeStatus> creatorCodeStatus;

	public static bool Intialized = false;

	public static Member supportedMember;

	public enum CreatorCodeStatus
	{
		Empty,
		Unchecked,
		Validating,
		Valid
	}

	[Serializable]
	private class CreatorCodesData
	{
		public Dictionary<string, string> currentCreatorCode = new Dictionary<string, string>();

		public Dictionary<string, DateTime> codeFirstUsedTime = new Dictionary<string, DateTime>();
	}
}
