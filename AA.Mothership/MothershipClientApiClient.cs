using System;
using System.Runtime.InteropServices;

public class MothershipClientApiClient : MothershipApiClient
{
	internal MothershipClientApiClient(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.MothershipClientApiClient_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(MothershipClientApiClient obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(MothershipClientApiClient obj)
	{
		if (obj == null)
		{
			return new HandleRef(null, IntPtr.Zero);
		}
		if (!obj.swigCMemOwn)
		{
			throw new ApplicationException("Cannot release ownership as memory is not owned");
		}
		HandleRef result = obj.swigCPtr;
		obj.swigCMemOwn = false;
		obj.Dispose();
		return result;
	}

	protected override void Dispose(bool disposing)
	{
		lock (this)
		{
			if (this.swigCPtr.Handle != IntPtr.Zero)
			{
				if (this.swigCMemOwn)
				{
					this.swigCMemOwn = false;
					MothershipApiPINVOKE.delete_MothershipClientApiClient(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public MothershipClientApiClient(string baseUrl, string titleId, string envId, string deploymentId, string websocketUrl, bool enableRetryQueue, string sessionIdUUID) : this(MothershipApiPINVOKE.new_MothershipClientApiClient(baseUrl, titleId, envId, deploymentId, websocketUrl, enableRetryQueue, sessionIdUUID), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void SetAuthRefreshRequiredDelegateWrapper(AuthRefreshRequiredDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipClientApiClient_SetAuthRefreshRequiredDelegateWrapper(this.swigCPtr, AuthRefreshRequiredDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void SetLoginCompleteDelegate(LoginCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipClientApiClient_SetLoginCompleteDelegate(this.swigCPtr, LoginCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool LoginWithInsecure1(string username, string accountId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_LoginWithInsecure1(this.swigCPtr, username, accountId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public bool LoginWithInsecure2(string username, string accountId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_LoginWithInsecure2(this.swigCPtr, username, accountId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public bool LoginWithQuest(string nonce, string userId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_LoginWithQuest(this.swigCPtr, nonce, userId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public bool LoginWithRift(string nonce, string userId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_LoginWithRift(this.swigCPtr, nonce, userId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public bool LoginWithGoogle(string token, string userId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_LoginWithGoogle(this.swigCPtr, token, userId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public bool LoginWithApple(string signature, string gamePlayerId, string teamPlayerId, string certUri, string salt, string timestamp, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_LoginWithApple(this.swigCPtr, signature, gamePlayerId, teamPlayerId, certUri, salt, timestamp, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetSetUserDataCompleteClientDelegateWrapper(SetUserDataCompleteClientDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipClientApiClient_SetSetUserDataCompleteClientDelegateWrapper(this.swigCPtr, SetUserDataCompleteClientDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool SetUserData(string callerId, string userId, string keyName, string value, int generation, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_SetUserData(this.swigCPtr, callerId, userId, keyName, value, generation, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetUserDataCompleteClientDelegateWrapper(GetUserDataCompleteClientDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipClientApiClient_SetGetUserDataCompleteClientDelegateWrapper(this.swigCPtr, GetUserDataCompleteClientDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetUserData(string callerId, string userId, string keyName, string metadataId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_GetUserData(this.swigCPtr, callerId, userId, keyName, metadataId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetDeleteUserDataCompleteClientDelegateWrapper(DeleteUserDataCompleteClientDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipClientApiClient_SetDeleteUserDataCompleteClientDelegateWrapper(this.swigCPtr, DeleteUserDataCompleteClientDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool DeleteUserData(string callerId, string userId, string keyName, string metadataId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_DeleteUserData(this.swigCPtr, callerId, userId, keyName, metadataId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetListUserDataCompleteClientDelegateWrapper(ListUserDataCompleteClientDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipClientApiClient_SetListUserDataCompleteClientDelegateWrapper(this.swigCPtr, ListUserDataCompleteClientDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ListUserData(string callerId, string userId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_ListUserData(this.swigCPtr, callerId, userId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetCreateReportCompleteClientDelegateWrapper(CreateReportCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipClientApiClient_SetCreateReportCompleteClientDelegateWrapper(this.swigCPtr, CreateReportCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool CreateReport(string callerId, string reportedUserId, int category, string platform, bool moddedClient, string metadata, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_CreateReport(this.swigCPtr, callerId, reportedUserId, category, platform, moddedClient, metadata, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetValidateUsernameCompleteClientDelegateWrapper(ValidateUsernameCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipClientApiClient_SetValidateUsernameCompleteClientDelegateWrapper(this.swigCPtr, ValidateUsernameCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ValidateUsername(string callerId, string username, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_ValidateUsername(this.swigCPtr, callerId, username, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetUserInventoryCompleteClientDelegateWrapper(GetUserInventoryCompleteClientDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipClientApiClient_SetGetUserInventoryCompleteClientDelegateWrapper(this.swigCPtr, GetUserInventoryCompleteClientDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetUserInventory(string callerId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_GetUserInventory(this.swigCPtr, callerId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetMergedInventoryCompleteClientDelegateWrapper(GetMergedInventoryCompleteClientDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipClientApiClient_SetGetMergedInventoryCompleteClientDelegateWrapper(this.swigCPtr, GetMergedInventoryCompleteClientDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetMergedInventory(string callerId, string targetId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_GetMergedInventory(this.swigCPtr, callerId, targetId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetStorefrontCompleteClientDelegateWrapper(GetStorefrontRequestCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipClientApiClient_SetGetStorefrontCompleteClientDelegateWrapper(this.swigCPtr, GetStorefrontRequestCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetStorefront(string callerId, StringVector offerDisplays, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_GetStorefront(this.swigCPtr, callerId, StringVector.getCPtr(offerDisplays), userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetPurchaseCompleteClientDelegateWrapper(PurchaseOfferRequestCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipClientApiClient_SetPurchaseCompleteClientDelegateWrapper(this.swigCPtr, PurchaseOfferRequestCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool PurchaseOffer(string callerId, string offerDisplayId, string offerId, int displayIndex, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_PurchaseOffer(this.swigCPtr, callerId, offerDisplayId, offerId, displayIndex, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetQuestAuthV2BeginRequestCompleteClientDelegateWrapper(QuestBeginLoginV2RequestCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipClientApiClient_SetQuestAuthV2BeginRequestCompleteClientDelegateWrapper(this.swigCPtr, QuestBeginLoginV2RequestCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool BeginQuestV2Auth(string userId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_BeginQuestV2Auth(this.swigCPtr, userId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public bool CompleteQuestV2Auth(string userId, string attestationToken, string metaNonce, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_CompleteQuestV2Auth(this.swigCPtr, userId, attestationToken, metaNonce, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetSteamBeginRequestCompleteClientDelegateWrapper(PlayerSteamBeginLoginResponseCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipClientApiClient_SetSteamBeginRequestCompleteClientDelegateWrapper(this.swigCPtr, PlayerSteamBeginLoginResponseCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool BeginSteamAuth(IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_BeginSteamAuth(this.swigCPtr, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public bool CompleteSteamAuth(string nonce, string ticket, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_CompleteSteamAuth(this.swigCPtr, nonce, ticket, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetListMothershipTitleDataCompleteClientDelegateWrapper(ListMothershipTitleDataCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipClientApiClient_SetListMothershipTitleDataCompleteClientDelegateWrapper(this.swigCPtr, ListMothershipTitleDataCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ListClientMothershipTitleData(string callerId, StringVector keys, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_ListClientMothershipTitleData(this.swigCPtr, callerId, StringVector.getCPtr(keys), userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetAcceptLanguage(string language)
	{
		MothershipApiPINVOKE.MothershipClientApiClient_SetAcceptLanguage(this.swigCPtr, language);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void SetCreateSharedGroupCompleteDelegateWrapper(CreateSharedGroupCompleteClientDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipClientApiClient_SetCreateSharedGroupCompleteDelegateWrapper(this.swigCPtr, CreateSharedGroupCompleteClientDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool CreateSharedGroup(string callerId, string titleId, string envId, string sharedGroupId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_CreateSharedGroup(this.swigCPtr, callerId, titleId, envId, sharedGroupId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetSharedGroupDataCompleteDelegateWrapper(GetSharedGroupDataCompleteClientDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipClientApiClient_SetGetSharedGroupDataCompleteDelegateWrapper(this.swigCPtr, GetSharedGroupDataCompleteClientDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetSharedGroupData(string callerId, string titleId, string envId, string sharedGroupId, StringVector keys, bool getMembers, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_GetSharedGroupData(this.swigCPtr, callerId, titleId, envId, sharedGroupId, StringVector.getCPtr(keys), getMembers, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetUpdateSharedGroupDataCompleteDelegateWrapper(UpdateSharedGroupDataCompleteClientDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipClientApiClient_SetUpdateSharedGroupDataCompleteDelegateWrapper(this.swigCPtr, UpdateSharedGroupDataCompleteClientDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool UpdateSharedGroupData(string callerId, string titleId, string envId, string sharedGroupId, StringKeyValueMap data, StringKeyValueMap customTags, StringVector keysToRemove, string permission, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_UpdateSharedGroupData(this.swigCPtr, callerId, titleId, envId, sharedGroupId, StringKeyValueMap.getCPtr(data), StringKeyValueMap.getCPtr(customTags), StringVector.getCPtr(keysToRemove), permission, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetAddSharedGroupMembersCompleteDelegateWrapper(AddSharedGroupMembersCompleteClientDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipClientApiClient_SetAddSharedGroupMembersCompleteDelegateWrapper(this.swigCPtr, AddSharedGroupMembersCompleteClientDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool AddSharedGroupMembers(string callerId, string titleId, string envId, string sharedGroupId, StringVector mothershipIds, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_AddSharedGroupMembers(this.swigCPtr, callerId, titleId, envId, sharedGroupId, StringVector.getCPtr(mothershipIds), userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetRemoveSharedGroupMembersCompleteDelegateWrapper(RemoveSharedGroupMembersCompleteClientDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipClientApiClient_SetRemoveSharedGroupMembersCompleteDelegateWrapper(this.swigCPtr, RemoveSharedGroupMembersCompleteClientDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool RemoveSharedGroupMembers(string callerId, string titleId, string envId, string sharedGroupId, StringVector mothershipIds, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_RemoveSharedGroupMembers(this.swigCPtr, callerId, titleId, envId, sharedGroupId, StringVector.getCPtr(mothershipIds), userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetMothershipRefreshIAPCompleteDelegateWrapper(MothershipRefreshIAPCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipClientApiClient_SetMothershipRefreshIAPCompleteDelegateWrapper(this.swigCPtr, MothershipRefreshIAPCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool RefreshIAP(string callerId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_RefreshIAP(this.swigCPtr, callerId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetWriteEventsCompleteClientDelegateWrapper(WriteEventsCompleteClientDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipClientApiClient_SetWriteEventsCompleteClientDelegateWrapper(this.swigCPtr, WriteEventsCompleteClientDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool WriteEvents(string callerId, MothershipWriteEventsRequest request, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_WriteEvents(this.swigCPtr, callerId, MothershipWriteEventsRequest.getCPtr(request), userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetNotificationsMessageDelegateWrapper(NotificationsMessageDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipClientApiClient_SetNotificationsMessageDelegateWrapper(this.swigCPtr, NotificationsMessageDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool OpenNotificationsSocket(string playerId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_OpenNotificationsSocket(this.swigCPtr, playerId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetProgressionTrackValuesForPlayerCompleteClientDelegateWrapper(GetProgressionTrackValuesForPlayerCompleteClientDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipClientApiClient_SetGetProgressionTrackValuesForPlayerCompleteClientDelegateWrapper(this.swigCPtr, GetProgressionTrackValuesForPlayerCompleteClientDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetProgressionTrackValuesForPlayer(string playerId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_GetProgressionTrackValuesForPlayer(this.swigCPtr, playerId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetProgressionTreesForPlayerCompleteClientDelegateWrapper(GetProgressionTreesForPlayerCompleteClientDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipClientApiClient_SetGetProgressionTreesForPlayerCompleteClientDelegateWrapper(this.swigCPtr, GetProgressionTreesForPlayerCompleteClientDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetProgressionTreesForPlayer(string playerId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_GetProgressionTreesForPlayer(this.swigCPtr, playerId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetClientGetPermissionsCompleteDelegateWrapper(ClientGetPermissionsCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipClientApiClient_SetClientGetPermissionsCompleteDelegateWrapper(this.swigCPtr, ClientGetPermissionsCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ClientGetPermissions(string callerId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_ClientGetPermissions(this.swigCPtr, callerId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetClientGetMySubscriptionsDelegateWrapper(ClientGetMySubscriptionCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipClientApiClient_SetClientGetMySubscriptionsDelegateWrapper(this.swigCPtr, ClientGetMySubscriptionCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ClientGetMySubscriptions(string callerId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_ClientGetMySubscriptions(this.swigCPtr, callerId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetClientBulkGetSubscriptionsDelegateWrapper(ClientGetBulkSubscriptionsCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipClientApiClient_SetClientBulkGetSubscriptionsDelegateWrapper(this.swigCPtr, ClientGetBulkSubscriptionsCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ClientBulkGetSubscriptions(string callerId, StringVector players, PlatformAndSkuVector platformSkus, StringVector catalogIds, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipClientApiClient_ClientBulkGetSubscriptions(this.swigCPtr, callerId, StringVector.getCPtr(players), PlatformAndSkuVector.getCPtr(platformSkus), StringVector.getCPtr(catalogIds), userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private HandleRef swigCPtr;
}
