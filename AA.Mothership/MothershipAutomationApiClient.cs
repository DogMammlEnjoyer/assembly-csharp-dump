using System;
using System.Runtime.InteropServices;

public class MothershipAutomationApiClient : MothershipApiClient
{
	internal MothershipAutomationApiClient(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.MothershipAutomationApiClient_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(MothershipAutomationApiClient obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(MothershipAutomationApiClient obj)
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
					MothershipApiPINVOKE.delete_MothershipAutomationApiClient(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public MothershipAutomationApiClient(string baseUrl, string orgId, string apiKey, bool enableRetryQueue, string titleId, string envId) : this(MothershipApiPINVOKE.new_MothershipAutomationApiClient__SWIG_0(baseUrl, orgId, apiKey, enableRetryQueue, titleId, envId), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public MothershipAutomationApiClient(string baseUrl, string orgId, string apiKey, bool enableRetryQueue, string titleId) : this(MothershipApiPINVOKE.new_MothershipAutomationApiClient__SWIG_1(baseUrl, orgId, apiKey, enableRetryQueue, titleId), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public MothershipAutomationApiClient(string baseUrl, string orgId, string apiKey, bool enableRetryQueue) : this(MothershipApiPINVOKE.new_MothershipAutomationApiClient__SWIG_2(baseUrl, orgId, apiKey, enableRetryQueue), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void SetListTitleCompleteDelegateWrapper(ListTitlesCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetListTitleCompleteDelegateWrapper(this.swigCPtr, ListTitlesCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ListTitles(IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_ListTitles(this.swigCPtr, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetTitleCompleteDelegateWrapper(GetTitleCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetGetTitleCompleteDelegateWrapper(this.swigCPtr, GetTitleCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetTitle(string titleId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_GetTitle(this.swigCPtr, titleId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetCreateTitleCompleteDelegateWrapper(CreateTitleCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetCreateTitleCompleteDelegateWrapper(this.swigCPtr, CreateTitleCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool CreateTitle(string titleName, string titleId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_CreateTitle(this.swigCPtr, titleName, titleId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetCreateEnvironmentCompleteDelegateWrapper(CreateEnvironmentCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetCreateEnvironmentCompleteDelegateWrapper(this.swigCPtr, CreateEnvironmentCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool CreateEnvironment(string titleId, string envName, string envId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_CreateEnvironment(this.swigCPtr, titleId, envName, envId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetListEnvironmentsCompleteDelegateWrapper(ListEnvironmentsCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetListEnvironmentsCompleteDelegateWrapper(this.swigCPtr, ListEnvironmentsCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ListEnvironments(string titleId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_ListEnvironments(this.swigCPtr, titleId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetEnvironmentCompleteDelegateWrapper(GetEnvironmentCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetGetEnvironmentCompleteDelegateWrapper(this.swigCPtr, GetEnvironmentCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetEnvironment(string titleId, string envId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_GetEnvironment(this.swigCPtr, titleId, envId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetUpdateInsecure1ConfigCompleteDelegateWrapper(UpdateInsecure1ConfigCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetUpdateInsecure1ConfigCompleteDelegateWrapper(this.swigCPtr, UpdateInsecure1ConfigCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool UpdateInsecure1Config(string titleId, string envId, bool enabled, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_UpdateInsecure1Config(this.swigCPtr, titleId, envId, enabled, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetUpdateInsecure2ConfigCompleteDelegateWrapper(UpdateInsecure2ConfigCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetUpdateInsecure2ConfigCompleteDelegateWrapper(this.swigCPtr, UpdateInsecure2ConfigCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool UpdateInsecure2Config(string titleId, string envId, bool enabled, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_UpdateInsecure2Config(this.swigCPtr, titleId, envId, enabled, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetUpdateQuestConfigCompleteDelegateWrapper(UpdateQuestConfigCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetUpdateQuestConfigCompleteDelegateWrapper(this.swigCPtr, UpdateQuestConfigCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool UpdateQuestConfig(string titleId, string envId, string appId, string appSecret, bool enabled, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_UpdateQuestConfig(this.swigCPtr, titleId, envId, appId, appSecret, enabled, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetUpdateRiftConfigCompleteDelegateWrapper(UpdateRiftConfigCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetUpdateRiftConfigCompleteDelegateWrapper(this.swigCPtr, UpdateRiftConfigCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool UpdateRiftConfig(string titleId, string envId, string appId, string appSecret, bool enabled, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_UpdateRiftConfig(this.swigCPtr, titleId, envId, appId, appSecret, enabled, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetUpdateEnvRequiredTagsCompleteDelegateWrapper(UpdateEnvRequiredTagsCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetUpdateEnvRequiredTagsCompleteDelegateWrapper(this.swigCPtr, UpdateEnvRequiredTagsCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool UpdateEnvRequiredTags(string titleId, string envId, StringVector tags, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_UpdateEnvRequiredTags(this.swigCPtr, titleId, envId, StringVector.getCPtr(tags), userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetCreateDeploymentCompleteDelegateWrapper(CreateDeploymentCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetCreateDeploymentCompleteDelegateWrapper(this.swigCPtr, CreateDeploymentCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool CreateDeployment(string titleId, string envId, string deploymentName, string deploymentId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_CreateDeployment(this.swigCPtr, titleId, envId, deploymentName, deploymentId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetListDeploymentsCompleteDelegateWrapper(ListDeploymentsCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetListDeploymentsCompleteDelegateWrapper(this.swigCPtr, ListDeploymentsCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ListDeployments(string titleId, string envId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_ListDeployments(this.swigCPtr, titleId, envId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetDeploymentCompleteDelegateWrapper(GetDeploymentCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetGetDeploymentCompleteDelegateWrapper(this.swigCPtr, GetDeploymentCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetDeployment(string titleId, string envId, string deploymentId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_GetDeployment(this.swigCPtr, titleId, envId, deploymentId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetUpdateDeploymentRequiredTagsCompleteDelegateWrapper(UpdateDeploymentRequiredTagsCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetUpdateDeploymentRequiredTagsCompleteDelegateWrapper(this.swigCPtr, UpdateDeploymentRequiredTagsCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool UpdateDeploymentRequiredTags(string titleId, string envId, string deploymentId, StringVector tags, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_UpdateDeploymentRequiredTags(this.swigCPtr, titleId, envId, deploymentId, StringVector.getCPtr(tags), userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetListUsersCompleteDelegateWrapper(ListUsersCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetListUsersCompleteDelegateWrapper(this.swigCPtr, ListUsersCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ListUsers(string titleId, string envId, string lastSeenMothershipId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_ListUsers(this.swigCPtr, titleId, envId, lastSeenMothershipId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetUpdatePlayerTagsCompleteDelegateWrapper(UpdatePlayerTagsCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetUpdatePlayerTagsCompleteDelegateWrapper(this.swigCPtr, UpdatePlayerTagsCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool UpdatePlayerTags(string titleId, string envId, PlayerTagsUpdateMap tagUpdates, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_UpdatePlayerTags(this.swigCPtr, titleId, envId, PlayerTagsUpdateMap.getCPtr(tagUpdates), userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetCreateUserDataMetadataCompleteDelegateWrapper(CreateUserDataMetadataCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetCreateUserDataMetadataCompleteDelegateWrapper(this.swigCPtr, CreateUserDataMetadataCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool CreateUserDataMetadata(string titleId, string envId, string keyName, string keyPerms, string privacyNotes, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_CreateUserDataMetadata(this.swigCPtr, titleId, envId, keyName, keyPerms, privacyNotes, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetUpdateUserDataMetadataCompleteDelegateWrapper(UpdateUserDataMetadataCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetUpdateUserDataMetadataCompleteDelegateWrapper(this.swigCPtr, UpdateUserDataMetadataCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool UpdateUserDataMetadata(string titleId, string envId, string keyName, string metadataId, string keyPerms, string privacyNotes, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_UpdateUserDataMetadata(this.swigCPtr, titleId, envId, keyName, metadataId, keyPerms, privacyNotes, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetUserDataMetadataCompleteDelegateWrapper(GetUserDataMetadataCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetGetUserDataMetadataCompleteDelegateWrapper(this.swigCPtr, GetUserDataMetadataCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetUserDataMetadata(string titleId, string envId, string keyName, string metadataId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_GetUserDataMetadata(this.swigCPtr, titleId, envId, keyName, metadataId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetDeleteUserDataMetadataCompleteDelegateWrapper(DeleteUserDataMetadataCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetDeleteUserDataMetadataCompleteDelegateWrapper(this.swigCPtr, DeleteUserDataMetadataCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool DeleteUserDataMetadata(string titleId, string envId, string keyName, string metadataId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_DeleteUserDataMetadata(this.swigCPtr, titleId, envId, keyName, metadataId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetListUserDataMetadataCompleteDelegateWrapper(ListUserDataMetadataCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetListUserDataMetadataCompleteDelegateWrapper(this.swigCPtr, ListUserDataMetadataCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ListUserDataMetadata(string titleId, string envId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_ListUserDataMetadata(this.swigCPtr, titleId, envId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetSetUserDataCompleteAutomationDelegateWrapper(SetUserDataCompleteAutomationDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetSetUserDataCompleteAutomationDelegateWrapper(this.swigCPtr, SetUserDataCompleteAutomationDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool SetUserData(string titleId, string envId, string userId, string keyName, string value, int generation, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_SetUserData(this.swigCPtr, titleId, envId, userId, keyName, value, generation, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetUserDataCompleteAutomationDelegateWrapper(GetUserDataCompleteAutomationDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetGetUserDataCompleteAutomationDelegateWrapper(this.swigCPtr, GetUserDataCompleteAutomationDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetUserData(string titleId, string envId, string userId, string keyName, string metadataId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_GetUserData(this.swigCPtr, titleId, envId, userId, keyName, metadataId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetDeleteUserDataCompleteAutomationDelegateWrapper(DeleteUserDataCompleteAutomationDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetDeleteUserDataCompleteAutomationDelegateWrapper(this.swigCPtr, DeleteUserDataCompleteAutomationDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool DeleteUserData(string titleId, string envId, string userId, string keyName, string metadataId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_DeleteUserData(this.swigCPtr, titleId, envId, userId, keyName, metadataId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetListUserDataCompleteAutomationDelegateWrapper(ListUserDataCompleteAutomationDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetListUserDataCompleteAutomationDelegateWrapper(this.swigCPtr, ListUserDataCompleteAutomationDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ListUserData(string titleId, string envId, string userId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_ListUserData(this.swigCPtr, titleId, envId, userId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetCreateEntitlementCatalogItemCompleteDelegateWrapper(CreateEntitlementCatalogItemCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetCreateEntitlementCatalogItemCompleteDelegateWrapper(this.swigCPtr, CreateEntitlementCatalogItemCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool CreateEntitlementCatalogItem(string titleId, string envId, string name, string inGameId, string type, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_CreateEntitlementCatalogItem(this.swigCPtr, titleId, envId, name, inGameId, type, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetListEntitlementCatalogItemCompleteDelegateWrapper(ListEntitlementCatalogItemsCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetListEntitlementCatalogItemCompleteDelegateWrapper(this.swigCPtr, ListEntitlementCatalogItemsCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ListEntitlementCatalogItems(string titleId, string envId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_ListEntitlementCatalogItems(this.swigCPtr, titleId, envId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetEntitlementCatalogItemCompleteDelegateWrapper(GetEntitlementCatalogItemCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetGetEntitlementCatalogItemCompleteDelegateWrapper(this.swigCPtr, GetEntitlementCatalogItemCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetEntitlementCatalogItem(string titleId, string envId, string entitlementId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_GetEntitlementCatalogItem(this.swigCPtr, titleId, envId, entitlementId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetUpdateEntitlementCatalogItemCompleteDelegateWrapper(UpdateEntitlementCatalogItemCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetUpdateEntitlementCatalogItemCompleteDelegateWrapper(this.swigCPtr, UpdateEntitlementCatalogItemCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool UpdateEntitlementCatalogItem(string titleId, string envId, string name, string entitlementId, string inGameId, string type, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_UpdateEntitlementCatalogItem(this.swigCPtr, titleId, envId, name, entitlementId, inGameId, type, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetCreateTransactionCatalogItemCompleteDelegateWrapper(CreateTransactionCatalogItemCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetCreateTransactionCatalogItemCompleteDelegateWrapper(this.swigCPtr, CreateTransactionCatalogItemCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool CreateTransactionCatalogItem(string titleId, string envId, string name, string externalServiceName, string externalServiceEntitlementId, StringIntMap inventoryChanges, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_CreateTransactionCatalogItem(this.swigCPtr, titleId, envId, name, externalServiceName, externalServiceEntitlementId, StringIntMap.getCPtr(inventoryChanges), userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetListTransactionCatalogItemsCompleteDelegateWrapper(ListTransactionCatalogItemsCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetListTransactionCatalogItemsCompleteDelegateWrapper(this.swigCPtr, ListTransactionCatalogItemsCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ListTransactionCatalogItems(string titleId, string envId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_ListTransactionCatalogItems(this.swigCPtr, titleId, envId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetTransactionCatalogItemCompleteDelegateWrapper(GetTransactionCatalogItemCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetGetTransactionCatalogItemCompleteDelegateWrapper(this.swigCPtr, GetTransactionCatalogItemCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetTransactionCatalogItem(string titleId, string envId, string transactionId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_GetTransactionCatalogItem(this.swigCPtr, titleId, envId, transactionId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetUpdateTransactionCatalogItemCompleteDelegateWrapper(UpdateTransactionCatalogItemCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetUpdateTransactionCatalogItemCompleteDelegateWrapper(this.swigCPtr, UpdateTransactionCatalogItemCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool UpdateTransactionCatalogItem(string titleId, string envId, string transactionId, string name, string externalServiceName, string externalServiceEntitlementId, StringIntMap inventoryChanges, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_UpdateTransactionCatalogItem(this.swigCPtr, titleId, envId, transactionId, name, externalServiceName, externalServiceEntitlementId, StringIntMap.getCPtr(inventoryChanges), userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetUpdateTransactionCatalogItemSunsetStatusCompleteDelegateWrapper(UpdateTransactionCatalogItemSunsetStatusCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetUpdateTransactionCatalogItemSunsetStatusCompleteDelegateWrapper(this.swigCPtr, UpdateTransactionCatalogItemSunsetStatusCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool UpdateTransactionCatalogItemSunsetStatus(string titleId, string envId, string transactionId, bool sunsetStatus, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_UpdateTransactionCatalogItemSunsetStatus(this.swigCPtr, titleId, envId, transactionId, sunsetStatus, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetListReportsCompleteDelegateWrapper(ListReportsCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetListReportsCompleteDelegateWrapper(this.swigCPtr, ListReportsCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ListReports(string titleId, string envId, StringVector reportsBy, StringVector reportsAgainst, int category, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_ListReports(this.swigCPtr, titleId, envId, StringVector.getCPtr(reportsBy), StringVector.getCPtr(reportsAgainst), category, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetReportCompleteDelegateWrapper(GetReportCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetGetReportCompleteDelegateWrapper(this.swigCPtr, GetReportCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetReport(string titleId, string envId, string reportId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_GetReport(this.swigCPtr, titleId, envId, reportId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetDeleteReportCompleteDelegateWrapper(DeleteReportCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetDeleteReportCompleteDelegateWrapper(this.swigCPtr, DeleteReportCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool DeleteReport(string titleId, string envId, string reportId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_DeleteReport(this.swigCPtr, titleId, envId, reportId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetBanCompleteDelegateWrapper(GetBanCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetGetBanCompleteDelegateWrapper(this.swigCPtr, GetBanCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetBan(string titleId, string envId, string banId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_GetBan(this.swigCPtr, titleId, envId, banId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetListBansCompleteDelegateWrapper(ListBansCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetListBansCompleteDelegateWrapper(this.swigCPtr, ListBansCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ListBans(string titleId, string envId, string playerId, int category, bool includeExpired, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_ListBans(this.swigCPtr, titleId, envId, playerId, category, includeExpired, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetListBansBulkCompleteDelegateWrapper(ListBansBulkCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetListBansBulkCompleteDelegateWrapper(this.swigCPtr, ListBansBulkCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ListBansBulk(string titleId, string envId, StringVector playerIds, int category, bool includeExpired, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_ListBansBulk(this.swigCPtr, titleId, envId, StringVector.getCPtr(playerIds), category, includeExpired, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetCreateBanCompleteDelegateWrapper(CreateBanCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetCreateBanCompleteDelegateWrapper(this.swigCPtr, CreateBanCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool CreateBan(string titleId, string envId, string playerId, int category, string reason, int durationMinutes, bool orgWide, string metadata, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_CreateBan(this.swigCPtr, titleId, envId, playerId, category, reason, durationMinutes, orgWide, metadata, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetRevokeBanCompleteDelegateWrapper(RevokeBanCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetRevokeBanCompleteDelegateWrapper(this.swigCPtr, RevokeBanCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool RevokeBan(string titleId, string envId, string banId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_RevokeBan(this.swigCPtr, titleId, envId, banId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetCreateMuteCompleteDelegateWrapper(CreateMuteCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetCreateMuteCompleteDelegateWrapper(this.swigCPtr, CreateMuteCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool CreateMute(string titleId, string envId, string playerId, int durationMinutes, string source, string reason, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_CreateMute(this.swigCPtr, titleId, envId, playerId, durationMinutes, source, reason, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetPlayerMutesCompleteDelegateWrapper(GetPlayerMutesCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetGetPlayerMutesCompleteDelegateWrapper(this.swigCPtr, GetPlayerMutesCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetPlayerMutes(string titleId, string envId, string playerId, bool includeExpired, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_GetPlayerMutes(this.swigCPtr, titleId, envId, playerId, includeExpired, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetDeleteMuteCompleteDelegateWrapper(DeleteMuteCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetDeleteMuteCompleteDelegateWrapper(this.swigCPtr, DeleteMuteCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool DeleteMute(string titleId, string envId, string playerId, string muteId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_DeleteMute(this.swigCPtr, titleId, envId, playerId, muteId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetRevokeMuteCompleteDelegateWrapper(RevokeMuteCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetRevokeMuteCompleteDelegateWrapper(this.swigCPtr, RevokeMuteCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool RevokeMute(string titleId, string envId, string playerId, string muteId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_RevokeMute(this.swigCPtr, titleId, envId, playerId, muteId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetUserInventoryCompleteAutomationDelegateWrapper(GetUserInventoryCompleteAutomationDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetGetUserInventoryCompleteAutomationDelegateWrapper(this.swigCPtr, GetUserInventoryCompleteAutomationDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetUserInventory(string titleId, string envId, string userId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_GetUserInventory(this.swigCPtr, titleId, envId, userId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetRunTransactionCompleteAutomationDelegateWrapper(RunTransactionCompleteAutomationDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetRunTransactionCompleteAutomationDelegateWrapper(this.swigCPtr, RunTransactionCompleteAutomationDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool RunTransaction(string titleId, string envId, string userId, string transactionId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_RunTransaction(this.swigCPtr, titleId, envId, userId, transactionId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetLastTransactionRunCompleteAutomationDelegateWrapper(GetLastTransactionCompleteAutomationDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetGetLastTransactionRunCompleteAutomationDelegateWrapper(this.swigCPtr, GetLastTransactionCompleteAutomationDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetLastTransactionRun(string titleId, string envId, string userId, string transactionId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_GetLastTransactionRun(this.swigCPtr, titleId, envId, userId, transactionId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetCreateServerKeyCompleteDelegateWrapper(CreateServerKeyCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetCreateServerKeyCompleteDelegateWrapper(this.swigCPtr, CreateServerKeyCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool CreateServerKey(string titleId, string envId, string keyName, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_CreateServerKey(this.swigCPtr, titleId, envId, keyName, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetListMothershipTitleDataCompleteAutomationDelegateWrapper(ListMothershipTitleDataCompleteAutomationDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetListMothershipTitleDataCompleteAutomationDelegateWrapper(this.swigCPtr, ListMothershipTitleDataCompleteAutomationDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ListMothershipTitleData(string titleId, string envId, string deploymentId, StringVector keys, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_ListMothershipTitleData(this.swigCPtr, titleId, envId, deploymentId, StringVector.getCPtr(keys), userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetSetMothershipTitleDataCompleteDelegateWrapper(SetMothershipTitleDataCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetSetMothershipTitleDataCompleteDelegateWrapper(this.swigCPtr, SetMothershipTitleDataCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool SetMothershipTitleData(string titleId, string envId, string deploymentId, string key, string data, bool server_only, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_SetMothershipTitleData(this.swigCPtr, titleId, envId, deploymentId, key, data, server_only, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetDeleteMothershipTitleDataCompleteDelegateWrapper(DeleteMothershipTitleDataCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetDeleteMothershipTitleDataCompleteDelegateWrapper(this.swigCPtr, DeleteMothershipTitleDataCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool DeleteMothershipTitleData(string titleId, string envId, string deploymentId, string key, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_DeleteMothershipTitleData(this.swigCPtr, titleId, envId, deploymentId, key, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetPlayerAccountLinksCompleteDelegateWrapper(GetPlayerAccountLinksCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetGetPlayerAccountLinksCompleteDelegateWrapper(this.swigCPtr, GetPlayerAccountLinksCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetPlayerAccountLinks(string titleId, string envId, string playerId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_GetPlayerAccountLinks(this.swigCPtr, titleId, envId, playerId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetAddPlayerAccountLinkCompleteDelegateWrapper(AddPlayerAccountLinkCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetAddPlayerAccountLinkCompleteDelegateWrapper(this.swigCPtr, AddPlayerAccountLinkCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool AddPlayerAccountLink(string titleId, string envId, string otherToken, string targetPlayer_id, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_AddPlayerAccountLink(this.swigCPtr, titleId, envId, otherToken, targetPlayer_id, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetSetPrimaryAccountLinkCompleteDelegateWrapper(SetPrimaryAccountLinkCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetSetPrimaryAccountLinkCompleteDelegateWrapper(this.swigCPtr, SetPrimaryAccountLinkCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool SetPrimaryAccountLink(string titleId, string envId, string linkId, string targetPlayer_id, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_SetPrimaryAccountLink(this.swigCPtr, titleId, envId, linkId, targetPlayer_id, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetDeleteAccountLinkCompleteDelegateWrapper(DeleteAccountLinkCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetDeleteAccountLinkCompleteDelegateWrapper(this.swigCPtr, DeleteAccountLinkCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool DeleteAccountLink(string titleId, string envId, string playerId, string linkId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_DeleteAccountLink(this.swigCPtr, titleId, envId, playerId, linkId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetCreateExplicitAccountLinkCompleteDelegateWrapper(ExplicitAccountLinkCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetCreateExplicitAccountLinkCompleteDelegateWrapper(this.swigCPtr, ExplicitAccountLinkCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool CreateExplicitAccountLink(string titleId, string envId, string playerId, string externalServiceName, string appScopedAccountId, string orgScopedAccountId, string username, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_CreateExplicitAccountLink(this.swigCPtr, titleId, envId, playerId, externalServiceName, appScopedAccountId, orgScopedAccountId, username, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetListAccountAssociationsCompleteDelegateWrapper(ListAccountAssociationsCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetListAccountAssociationsCompleteDelegateWrapper(this.swigCPtr, ListAccountAssociationsCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ListAccountAssociationsForPlayer(string titleId, string envId, string playerId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_ListAccountAssociationsForPlayer(this.swigCPtr, titleId, envId, playerId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetDeleteAccountAssociationCompleteDelegateWrapper(DeleteAccountAssociationCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetDeleteAccountAssociationCompleteDelegateWrapper(this.swigCPtr, DeleteAccountAssociationCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool DeleteAccountAssociation(string titleId, string envId, string playerId, string associationId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_DeleteAccountAssociation(this.swigCPtr, titleId, envId, playerId, associationId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetListOfferCatalogItemsCompleteDelegateWrapper(ListOfferCatalogItemsCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetListOfferCatalogItemsCompleteDelegateWrapper(this.swigCPtr, ListOfferCatalogItemsCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ListOfferCatalogItems(string titleId, string envId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_ListOfferCatalogItems(this.swigCPtr, titleId, envId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetCreateOfferCatalogItemCompleteDelegateWrapper(CreateOfferCatalogItemCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetCreateOfferCatalogItemCompleteDelegateWrapper(this.swigCPtr, CreateOfferCatalogItemCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool CreateOfferCatalogItem(string titleId, string envId, string name, string transaction_id, OfferEntitlementMap bundle_pricing, int discount_percent, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_CreateOfferCatalogItem(this.swigCPtr, titleId, envId, name, transaction_id, OfferEntitlementMap.getCPtr(bundle_pricing), discount_percent, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetListOffersDisplayCompleteDelegateWrapper(ListOffersDisplayCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetListOffersDisplayCompleteDelegateWrapper(this.swigCPtr, ListOffersDisplayCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ListOfferDisplays(string titleId, string envid, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_ListOfferDisplays(this.swigCPtr, titleId, envid, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetCreateOfferDisplayCompleteDelegateWrapper(CreateOfferDisplayCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetCreateOfferDisplayCompleteDelegateWrapper(this.swigCPtr, CreateOfferDisplayCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool CreateOfferDisplay(string titleId, string envId, string name, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_CreateOfferDisplay(this.swigCPtr, titleId, envId, name, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetChangeCommitStatusOfOfferBindingsCompleteDelegateWrapper(ChangeCommitStatusOfOfferBindingsCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetChangeCommitStatusOfOfferBindingsCompleteDelegateWrapper(this.swigCPtr, ChangeCommitStatusOfOfferBindingsCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ChangeCommitStatusOfOfferBindings(string titleId, string envId, string deploymentId, string displayId, bool committed, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_ChangeCommitStatusOfOfferBindings(this.swigCPtr, titleId, envId, deploymentId, displayId, committed, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetListOfferBindingsCompleteDelegateWrapper(ListOfferBindingsCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetListOfferBindingsCompleteDelegateWrapper(this.swigCPtr, ListOfferBindingsCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ListOfferBindings(string titleId, string envId, string deploymentId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_ListOfferBindings(this.swigCPtr, titleId, envId, deploymentId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetCreateOfferBindingCompleteDelegateWrapper(CreateOfferBindingCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetCreateOfferBindingCompleteDelegateWrapper(this.swigCPtr, CreateOfferBindingCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool CreateOfferBinding(string titleId, string envId, string deploymentId, string offerDisplayId, string offerId, int displayIndex, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_CreateOfferBinding(this.swigCPtr, titleId, envId, deploymentId, offerDisplayId, offerId, displayIndex, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetCreateProgressionTrackCompleteDelegateWrapper(CreateProgressionTrackCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetCreateProgressionTrackCompleteDelegateWrapper(this.swigCPtr, CreateProgressionTrackCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool CreateProgressionTrack(string titleId, string envId, string name, int maximum, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_CreateProgressionTrack(this.swigCPtr, titleId, envId, name, maximum, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetUpdateProgressionTrackCompleteDelegateWrapper(UpdateProgressionTrackCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetUpdateProgressionTrackCompleteDelegateWrapper(this.swigCPtr, UpdateProgressionTrackCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool UpdateProgressionTrack(string trackId, string titleId, string envId, string name, int maximum, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_UpdateProgressionTrack(this.swigCPtr, trackId, titleId, envId, name, maximum, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetListProgressionTracksCompleteDelegateWrapper(ListProgressionTracksCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetListProgressionTracksCompleteDelegateWrapper(this.swigCPtr, ListProgressionTracksCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ListProgressionTracks(string titleId, string envId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_ListProgressionTracks(this.swigCPtr, titleId, envId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetDeleteProgressionTrackCompleteDelegateWrapper(DeleteProgressionTrackCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetDeleteProgressionTrackCompleteDelegateWrapper(this.swigCPtr, DeleteProgressionTrackCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool DeleteProgressionTrack(string titleId, string envId, string trackId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_DeleteProgressionTrack(this.swigCPtr, titleId, envId, trackId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetCreateTrackTriggerCompleteDelegateWrapper(CreateTrackTriggerCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetCreateTrackTriggerCompleteDelegateWrapper(this.swigCPtr, CreateTrackTriggerCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool CreateTrackTrigger(string trackId, string titleId, string envId, string transactionId, string name, int progression, string prerequisiteEntitlementId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_CreateTrackTrigger(this.swigCPtr, trackId, titleId, envId, transactionId, name, progression, prerequisiteEntitlementId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetUpdateTrackTriggerCompleteDelegateWrapper(UpdateTrackTriggerCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetUpdateTrackTriggerCompleteDelegateWrapper(this.swigCPtr, UpdateTrackTriggerCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool UpdateTrackTrigger(string trackId, string titleId, string envId, string triggerId, string transactionId, string name, int progression, string prerequisiteEntitlementId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_UpdateTrackTrigger(this.swigCPtr, trackId, titleId, envId, triggerId, transactionId, name, progression, prerequisiteEntitlementId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetDeleteTrackTriggerCompleteDelegateWrapper(DeleteTrackTriggerCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetDeleteTrackTriggerCompleteDelegateWrapper(this.swigCPtr, DeleteTrackTriggerCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool DeleteTrackTrigger(string trackId, string titleId, string envId, string triggerId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_DeleteTrackTrigger(this.swigCPtr, trackId, titleId, envId, triggerId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetCreateTrackLevelCompleteDelegateWrapper(CreateTrackLevelCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetCreateTrackLevelCompleteDelegateWrapper(this.swigCPtr, CreateTrackLevelCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool CreateTrackLevel(string trackId, string titleId, string envId, string name, int progressionAmount, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_CreateTrackLevel(this.swigCPtr, trackId, titleId, envId, name, progressionAmount, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetUpdateTrackLevelCompleteDelegateWrapper(UpdateTrackLevelCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetUpdateTrackLevelCompleteDelegateWrapper(this.swigCPtr, UpdateTrackLevelCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool UpdateTrackLevel(string trackId, string titleId, string envId, string levelId, string name, int progressionAmount, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_UpdateTrackLevel(this.swigCPtr, trackId, titleId, envId, levelId, name, progressionAmount, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetDeleteTrackLevelCompleteDelegateWrapper(DeleteTrackLevelCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetDeleteTrackLevelCompleteDelegateWrapper(this.swigCPtr, DeleteTrackLevelCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool DeleteTrackLevel(string trackId, string titleId, string envId, string levelId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_DeleteTrackLevel(this.swigCPtr, trackId, titleId, envId, levelId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetProgressionTrackValuesForPlayerCompleteAutomationDelegateWrapper(GetProgressionTrackValuesForPlayerCompleteAutomationDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetGetProgressionTrackValuesForPlayerCompleteAutomationDelegateWrapper(this.swigCPtr, GetProgressionTrackValuesForPlayerCompleteAutomationDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetProgressionTrackValuesForPlayer(string titleId, string envId, string playerId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_GetProgressionTrackValuesForPlayer(this.swigCPtr, titleId, envId, playerId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetCreateProgressionTreeCompleteDelegateWrapper(CreateProgressionTreeCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetCreateProgressionTreeCompleteDelegateWrapper(this.swigCPtr, CreateProgressionTreeCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool CreateProgressionTree(string titleId, string envId, string name, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_CreateProgressionTree(this.swigCPtr, titleId, envId, name, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetUpdateProgressionTreeCompleteDelegateWrapper(UpdateProgressionTreeCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetUpdateProgressionTreeCompleteDelegateWrapper(this.swigCPtr, UpdateProgressionTreeCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool UpdateProgressionTree(string titleId, string envId, string treeId, string name, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_UpdateProgressionTree(this.swigCPtr, titleId, envId, treeId, name, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetListProgressionTreesCompleteDelegateWrapper(ListProgressionTreesCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetListProgressionTreesCompleteDelegateWrapper(this.swigCPtr, ListProgressionTreesCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ListProgressionTrees(string titleId, string envId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_ListProgressionTrees(this.swigCPtr, titleId, envId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetDeleteProgressionTreeCompleteDelegateWrapper(DeleteProgressionTreeCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetDeleteProgressionTreeCompleteDelegateWrapper(this.swigCPtr, DeleteProgressionTreeCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool DeleteProgressionTree(string titleId, string envId, string treeId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_DeleteProgressionTree(this.swigCPtr, titleId, envId, treeId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetCreateProgressionTreeNodeCompleteDelegateWrapper(CreateProgressionTreeNodeCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetCreateProgressionTreeNodeCompleteDelegateWrapper(this.swigCPtr, CreateProgressionTreeNodeCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool CreateProgressionTreeNode(string titleId, string envId, string treeId, TreeNodeDefinition nodeDefinition, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_CreateProgressionTreeNode(this.swigCPtr, titleId, envId, treeId, TreeNodeDefinition.getCPtr(nodeDefinition), userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetUpdateProgressionTreeNodeCompleteDelegateWrapper(UpdateProgressionTreeNodeCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetUpdateProgressionTreeNodeCompleteDelegateWrapper(this.swigCPtr, UpdateProgressionTreeNodeCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool UpdateProgressionTreeNode(string titleId, string envId, string treeId, TreeNodeDefinition nodeDefinition, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_UpdateProgressionTreeNode(this.swigCPtr, titleId, envId, treeId, TreeNodeDefinition.getCPtr(nodeDefinition), userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetDeleteProgressionTreeNodeCompleteDelegateWrapper(DeleteProgressionTreeNodeCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetDeleteProgressionTreeNodeCompleteDelegateWrapper(this.swigCPtr, DeleteProgressionTreeNodeCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool DeleteProgressionTreeNode(string titleId, string envId, string treeId, string nodeId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_DeleteProgressionTreeNode(this.swigCPtr, titleId, envId, treeId, nodeId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetUnlockProgressionTreeNodeAutomationCompleteDelegateWrapper(UnlockProgressionTreeNodeAutomationCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetUnlockProgressionTreeNodeAutomationCompleteDelegateWrapper(this.swigCPtr, UnlockProgressionTreeNodeAutomationCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool UnlockProgressionTreeNode(string titleId, string envId, string treeId, string nodeId, string playerId, bool ignoreCost, bool ignorePrerequisites, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_UnlockProgressionTreeNode(this.swigCPtr, titleId, envId, treeId, nodeId, playerId, ignoreCost, ignorePrerequisites, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetLockProgressionTreeNodeCompleteDelegateWrapper(LockProgressionTreeNodeAutomationCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetLockProgressionTreeNodeCompleteDelegateWrapper(this.swigCPtr, LockProgressionTreeNodeAutomationCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool LockProgressionTreeNode(string titleId, string envId, string treeId, string nodeId, string playerId, bool refund_costs, bool rewind_rewards, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_LockProgressionTreeNode(this.swigCPtr, titleId, envId, treeId, nodeId, playerId, refund_costs, rewind_rewards, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetCreateProgressionTreeTrackCompleteDelegateWrapper(CreateProgressionTreeTrackCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetCreateProgressionTreeTrackCompleteDelegateWrapper(this.swigCPtr, CreateProgressionTreeTrackCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool CreateProgressionTreeTrack(string titleId, string envId, string treeId, string name, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_CreateProgressionTreeTrack(this.swigCPtr, titleId, envId, treeId, name, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetCreateProgressionTreeBindingsCompleteDelegateWrapper(CreateProgressionTreeBindingsCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetCreateProgressionTreeBindingsCompleteDelegateWrapper(this.swigCPtr, CreateProgressionTreeBindingsCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool CreateProgressionTreeBindings(string titleId, string envId, string deploymentId, string treeId, bool visible, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_CreateProgressionTreeBindings(this.swigCPtr, titleId, envId, deploymentId, treeId, visible, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetUpdateProgressionTreeBindingsCompleteDelegateWrapper(UpdateProgressionTreeBindingsCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetUpdateProgressionTreeBindingsCompleteDelegateWrapper(this.swigCPtr, UpdateProgressionTreeBindingsCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool UpdateProgressionTreeBindings(string titleId, string envId, string deploymentId, string treeId, string id, bool visible, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_UpdateProgressionTreeBindings(this.swigCPtr, titleId, envId, deploymentId, treeId, id, visible, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetListProgressionTreeBindingsCompleteDelegateWrapper(ListProgressionTreeBindingsCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetListProgressionTreeBindingsCompleteDelegateWrapper(this.swigCPtr, ListProgressionTreeBindingsCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ListProgressionTreeBindings(string titleId, string envId, string deploymentId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_ListProgressionTreeBindings(this.swigCPtr, titleId, envId, deploymentId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetDeleteProgressionTreeBindingsCompleteDelegateWrapper(DeleteProgressionTreeBindingsCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetDeleteProgressionTreeBindingsCompleteDelegateWrapper(this.swigCPtr, DeleteProgressionTreeBindingsCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool DeleteProgressionTreeBindings(string titleId, string envId, string deploymentId, string treeId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_DeleteProgressionTreeBindings(this.swigCPtr, titleId, envId, deploymentId, treeId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetProgressionTreeBindingsCompleteDelegateWrapper(GetProgressionTreeBindingsCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetGetProgressionTreeBindingsCompleteDelegateWrapper(this.swigCPtr, GetProgressionTreeBindingsCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetProgressionTreeBindings(string titleId, string envId, string deploymentId, string treeId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_GetProgressionTreeBindings(this.swigCPtr, titleId, envId, deploymentId, treeId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetCreateProgressionTrackBindingsCompleteDelegateWrapper(CreateProgressionTrackBindingsCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetCreateProgressionTrackBindingsCompleteDelegateWrapper(this.swigCPtr, CreateProgressionTrackBindingsCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool CreateProgressionTrackBindings(string titleId, string envId, string deploymentId, string trackId, bool visible, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_CreateProgressionTrackBindings(this.swigCPtr, titleId, envId, deploymentId, trackId, visible, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetUpdateProgressionTrackBindingsCompleteDelegateWrapper(UpdateProgressionTrackBindingsCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetUpdateProgressionTrackBindingsCompleteDelegateWrapper(this.swigCPtr, UpdateProgressionTrackBindingsCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool UpdateProgressionTrackBindings(string titleId, string envId, string deploymentId, string trackId, string id, bool visible, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_UpdateProgressionTrackBindings(this.swigCPtr, titleId, envId, deploymentId, trackId, id, visible, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetListProgressionTrackBindingsCompleteDelegateWrapper(ListProgressionTrackBindingsCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetListProgressionTrackBindingsCompleteDelegateWrapper(this.swigCPtr, ListProgressionTrackBindingsCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ListProgressionTrackBindings(string titleId, string envId, string deploymentId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_ListProgressionTrackBindings(this.swigCPtr, titleId, envId, deploymentId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetProgressionTrackBindingsCompleteDelegateWrapper(GetProgressionTrackBindingsCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetGetProgressionTrackBindingsCompleteDelegateWrapper(this.swigCPtr, GetProgressionTrackBindingsCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetProgressionTrackBindings(string titleId, string envId, string deploymentId, string trackId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_GetProgressionTrackBindings(this.swigCPtr, titleId, envId, deploymentId, trackId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void SetGetSubscriptionsForPlayerCompleteDelegateWrapper(AutomationGetPlayerSubscriptionCompleteDelegateWrapper wrapper)
	{
		MothershipApiPINVOKE.MothershipAutomationApiClient_SetGetSubscriptionsForPlayerCompleteDelegateWrapper(this.swigCPtr, AutomationGetPlayerSubscriptionCompleteDelegateWrapper.getCPtr(wrapper));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool GetSubscriptionsForPlayer(string titleId, string envId, string playerId, IntPtr userData)
	{
		bool result = MothershipApiPINVOKE.MothershipAutomationApiClient_GetSubscriptionsForPlayer(this.swigCPtr, titleId, envId, playerId, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private HandleRef swigCPtr;
}
