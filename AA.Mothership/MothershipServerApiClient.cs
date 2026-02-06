using System;
using System.Runtime.InteropServices;

public class MothershipServerApiClient : MothershipApiClient
{
	internal MothershipServerApiClient(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.MothershipServerApiClient_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(MothershipServerApiClient obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(MothershipServerApiClient obj)
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
					MothershipApiPINVOKE.delete_MothershipServerApiClient(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public MothershipServerApiClient(string baseUrl, string titleId, string envId, string apiKey, bool enableRetryQueue) : this(MothershipApiPINVOKE.new_MothershipServerApiClient(baseUrl, titleId, envId, apiKey, enableRetryQueue), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void SetVerifyTokenCompleteDelegateWrapper(VerifyTokenCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetVerifyTokenCompleteDelegateWrapper(this.swigCPtr, VerifyTokenCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool VerifyToken(string mothershipPlayerId, string token, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_VerifyToken(this.swigCPtr, mothershipPlayerId, token, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetBulkGetAccountLinksCompleteDelegateWrapper(BulkGetAccountLinksCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetBulkGetAccountLinksCompleteDelegateWrapper(this.swigCPtr, BulkGetAccountLinksCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool BulkGetAccountLinks(AccountLinkLookupVector lookups, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_BulkGetAccountLinks(this.swigCPtr, AccountLinkLookupVector.getCPtr(lookups), userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetBulkGetPlayersCompleteDelegateWrapper(BulkGetPlayersCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetBulkGetPlayersCompleteDelegateWrapper(this.swigCPtr, BulkGetPlayersCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool BulkGetPlayers(PlayerLookupVector lookups, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_BulkGetPlayers(this.swigCPtr, PlayerLookupVector.getCPtr(lookups), userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetCreateExplicitAccountLinkCompleteDelegateWrapper(ExplicitAccountLinkCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetCreateExplicitAccountLinkCompleteDelegateWrapper(this.swigCPtr, ExplicitAccountLinkCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool CreateExplicitAccountLink(string titleId, string envId, string playerId, string externalServiceName, string appScopedAccountId, string orgScopedAccountId, string username, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_CreateExplicitAccountLink(this.swigCPtr, titleId, envId, playerId, externalServiceName, appScopedAccountId, orgScopedAccountId, username, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetListAccountAssociationsCompleteDelegateWrapper(ListAccountAssociationsCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetListAccountAssociationsCompleteDelegateWrapper(this.swigCPtr, ListAccountAssociationsCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ListAccountAssociationsForPlayer(string mothershipPlayerId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_ListAccountAssociationsForPlayer(this.swigCPtr, mothershipPlayerId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetCreateAccountAssociationsCompleteDelegateWrapper(CreateAccountAssociationDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetCreateAccountAssociationsCompleteDelegateWrapper(this.swigCPtr, CreateAccountAssociationDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool CreateAccountAssociation(string mothershipPlayerId, string externalServiceName, string externalServiceOrgScopedId, string externalServiceUserId, string externalServiceUserName, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_CreateAccountAssociation(this.swigCPtr, mothershipPlayerId, externalServiceName, externalServiceOrgScopedId, externalServiceUserId, externalServiceUserName, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetSetUserDataCompleteServerDelegateWrapper(SetUserDataCompleteServerDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetSetUserDataCompleteServerDelegateWrapper(this.swigCPtr, SetUserDataCompleteServerDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool SetUserData(string userId, string keyName, string value, int generation, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_SetUserData(this.swigCPtr, userId, keyName, value, generation, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetUserDataCompleteServerDelegateWrapper(GetUserDataCompleteServerDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetGetUserDataCompleteServerDelegateWrapper(this.swigCPtr, GetUserDataCompleteServerDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetUserData(string userId, string keyName, string metadataId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_GetUserData(this.swigCPtr, userId, keyName, metadataId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetDeleteUserDataCompleteServerDelegateWrapper(DeleteUserDataCompleteServerDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetDeleteUserDataCompleteServerDelegateWrapper(this.swigCPtr, DeleteUserDataCompleteServerDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool DeleteUserData(string userId, string keyName, string metadataId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_DeleteUserData(this.swigCPtr, userId, keyName, metadataId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetListUserDataCompleteServerDelegateWrapper(ListUserDataCompleteServerDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetListUserDataCompleteServerDelegateWrapper(this.swigCPtr, ListUserDataCompleteServerDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ListUserData(string userId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_ListUserData(this.swigCPtr, userId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetCreateSharedGroupCompleteDelegateWrapper(CreateSharedGroupCompleteServerDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetCreateSharedGroupCompleteDelegateWrapper(this.swigCPtr, CreateSharedGroupCompleteServerDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool CreateSharedGroup(string titleId, string envId, string sharedGroupId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_CreateSharedGroup(this.swigCPtr, titleId, envId, sharedGroupId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetSharedGroupDataCompleteDelegateWrapper(GetSharedGroupDataCompleteServerDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetGetSharedGroupDataCompleteDelegateWrapper(this.swigCPtr, GetSharedGroupDataCompleteServerDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetSharedGroupData(string titleId, string envId, string sharedGroupId, StringVector keys, bool getMembers, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_GetSharedGroupData(this.swigCPtr, titleId, envId, sharedGroupId, StringVector.getCPtr(keys), getMembers, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetUpdateSharedGroupDataCompleteDelegateWrapper(UpdateSharedGroupDataCompleteServerDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetUpdateSharedGroupDataCompleteDelegateWrapper(this.swigCPtr, UpdateSharedGroupDataCompleteServerDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool UpdateSharedGroupData(string titleId, string envId, string sharedGroupId, StringKeyValueMap data, StringKeyValueMap customTags, StringVector keysToRemove, string permission, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_UpdateSharedGroupData(this.swigCPtr, titleId, envId, sharedGroupId, StringKeyValueMap.getCPtr(data), StringKeyValueMap.getCPtr(customTags), StringVector.getCPtr(keysToRemove), permission, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetAddSharedGroupMembersCompleteDelegateWrapper(AddSharedGroupMembersCompleteServerDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetAddSharedGroupMembersCompleteDelegateWrapper(this.swigCPtr, AddSharedGroupMembersCompleteServerDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool AddSharedGroupMembers(string titleId, string envId, string sharedGroupId, StringVector mothershipIds, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_AddSharedGroupMembers(this.swigCPtr, titleId, envId, sharedGroupId, StringVector.getCPtr(mothershipIds), userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetRemoveSharedGroupMembersCompleteDelegateWrapper(RemoveSharedGroupMembersCompleteServerDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetRemoveSharedGroupMembersCompleteDelegateWrapper(this.swigCPtr, RemoveSharedGroupMembersCompleteServerDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool RemoveSharedGroupMembers(string titleId, string envId, string sharedGroupId, StringVector mothershipIds, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_RemoveSharedGroupMembers(this.swigCPtr, titleId, envId, sharedGroupId, StringVector.getCPtr(mothershipIds), userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetDeleteSharedGroupCompleteDelegateWrapper(DeleteSharedGroupCompleteServerDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetDeleteSharedGroupCompleteDelegateWrapper(this.swigCPtr, DeleteSharedGroupCompleteServerDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool DeleteSharedGroup(string titleId, string envId, string sharedGroupId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_DeleteSharedGroup(this.swigCPtr, titleId, envId, sharedGroupId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetUserInventoryCompleteServerDelegateWrapper(GetUserInventoryCompleteServerDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetGetUserInventoryCompleteServerDelegateWrapper(this.swigCPtr, GetUserInventoryCompleteServerDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetUserInventory(string titleId, string envId, string userId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_GetUserInventory(this.swigCPtr, titleId, envId, userId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetRunTransactionCompleteServerDelegateWrapper(RunTransactionCompleteServerDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetRunTransactionCompleteServerDelegateWrapper(this.swigCPtr, RunTransactionCompleteServerDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool RunTransaction(string titleId, string envId, string userId, string transactionId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_RunTransaction(this.swigCPtr, titleId, envId, userId, transactionId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetLastTransactionRunCompleteServerDelegateWrapper(GetLastTransactionCompleteServerDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetGetLastTransactionRunCompleteServerDelegateWrapper(this.swigCPtr, GetLastTransactionCompleteServerDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetLastTransactionRun(string titleId, string envId, string userId, string transactionId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_GetLastTransactionRun(this.swigCPtr, titleId, envId, userId, transactionId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetListMothershipTitleDataCompleteServerDelegateWrapper(ListMothershipTitleDataCompleteServerDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetListMothershipTitleDataCompleteServerDelegateWrapper(this.swigCPtr, ListMothershipTitleDataCompleteServerDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ListMothershipTitleData(string titleId, string envId, string deploymentId, StringVector keys, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_ListMothershipTitleData(this.swigCPtr, titleId, envId, deploymentId, StringVector.getCPtr(keys), userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetAcceptLanguage(string language)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetAcceptLanguage(this.swigCPtr, language);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void SetWriteEventsCompleteServerDelegateWrapper(WriteEventsCompleteServerDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetWriteEventsCompleteServerDelegateWrapper(this.swigCPtr, WriteEventsCompleteServerDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool WriteEvents(MothershipWriteEventsRequest request, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_WriteEvents(this.swigCPtr, MothershipWriteEventsRequest.getCPtr(request), userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetListBansBulkCompleteServerDelegateWrapper(ListBansBulkCompleteServerDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetListBansBulkCompleteServerDelegateWrapper(this.swigCPtr, ListBansBulkCompleteServerDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ListBansBulk(StringVector playerIds, int category, bool includeExpired, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_ListBansBulk(this.swigCPtr, StringVector.getCPtr(playerIds), category, includeExpired, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetServerCreateReportCompleteDelegateWrapper(ServerCreateReportCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetServerCreateReportCompleteDelegateWrapper(this.swigCPtr, ServerCreateReportCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ServerCreateReport(string reportingUserId, string reportedUserId, int category, string platform, bool moddedClient, string metadata, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_ServerCreateReport(this.swigCPtr, reportingUserId, reportedUserId, category, platform, moddedClient, metadata, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetServerValidateUsernameCompleteDelegateWrapper(ServerValidateUsernameCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetServerValidateUsernameCompleteDelegateWrapper(this.swigCPtr, ServerValidateUsernameCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ServerValidateUsername(string username, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_ServerValidateUsername(this.swigCPtr, username, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetServerCreateBanCompleteDelegateWrapper(ServerCreateBanCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetServerCreateBanCompleteDelegateWrapper(this.swigCPtr, ServerCreateBanCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ServerCreateBan(string playerId, int category, string reason, int durationMinutes, bool orgWide, string metadata, string source, bool isHardwareBan, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_ServerCreateBan(this.swigCPtr, playerId, category, reason, durationMinutes, orgWide, metadata, source, isHardwareBan, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetSendNotificationServerDelegateWrapper(SendNotificationCompleteServerDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetSendNotificationServerDelegateWrapper(this.swigCPtr, SendNotificationCompleteServerDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool SendNotification(StringVector playerIds, string title, string body, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_SendNotification(this.swigCPtr, StringVector.getCPtr(playerIds), title, body, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetProgressionTrackValuesForPlayerCompleteServerDelegateWrapper(GetProgressionTrackValuesForPlayerCompleteServerDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetGetProgressionTrackValuesForPlayerCompleteServerDelegateWrapper(this.swigCPtr, GetProgressionTrackValuesForPlayerCompleteServerDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetProgressionTrackValuesForPlayer(string titleId, string envId, string playerId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_GetProgressionTrackValuesForPlayer(this.swigCPtr, titleId, envId, playerId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetIncrementProgressionTrackForPlayerCompleteServerDelegateWrapper(IncrementProgressionTrackForPlayerCompleteServerDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetIncrementProgressionTrackForPlayerCompleteServerDelegateWrapper(this.swigCPtr, IncrementProgressionTrackForPlayerCompleteServerDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool IncrementProgressionTrackForPlayer(string titleId, string envId, string playerId, string trackId, int additionalProgress, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_IncrementProgressionTrackForPlayer(this.swigCPtr, titleId, envId, playerId, trackId, additionalProgress, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetUnlockProgressionTreeNodeCompleteServerDelegateWrapper(UnlockProgressionTreeNodeCompleteServerDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetUnlockProgressionTreeNodeCompleteServerDelegateWrapper(this.swigCPtr, UnlockProgressionTreeNodeCompleteServerDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool UnlockProgressionTreeNode(string treeId, string nodeId, string playerId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_UnlockProgressionTreeNode(this.swigCPtr, treeId, nodeId, playerId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public bool ForceUnlockProgressionTreeNode(string treeId, string nodeId, string playerId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_ForceUnlockProgressionTreeNode(this.swigCPtr, treeId, nodeId, playerId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetProgressionTreesForPlayerCompleteServerDelegateWrapper(GetProgressionTreesForPlayerCompleteServerDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetGetProgressionTreesForPlayerCompleteServerDelegateWrapper(this.swigCPtr, GetProgressionTreesForPlayerCompleteServerDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetProgressionTreesForPlayer(string playerId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_GetProgressionTreesForPlayer(this.swigCPtr, playerId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetLockProgressionTreeNodeCompleteDelegateWrapper(LockProgressionTreeNodeServerCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetLockProgressionTreeNodeCompleteDelegateWrapper(this.swigCPtr, LockProgressionTreeNodeServerCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool LockProgressionTreeNode(string titleId, string envId, string treeId, string nodeId, string playerId, bool refund_costs, bool rewind_rewards, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_LockProgressionTreeNode(this.swigCPtr, titleId, envId, treeId, nodeId, playerId, refund_costs, rewind_rewards, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetServerGetPermissionsCompleteDelegateWrapper(ServerGetPermissionsCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipServerApiClient_SetServerGetPermissionsCompleteDelegateWrapper(this.swigCPtr, ServerGetPermissionsCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ServerGetPermissions(StringVector playerIds, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipServerApiClient_ServerGetPermissions(this.swigCPtr, StringVector.getCPtr(playerIds), userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private HandleRef swigCPtr;
}
