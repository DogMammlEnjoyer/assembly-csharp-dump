using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Steamworks
{
	public static class SteamUGC
	{
		public static UGCQueryHandle_t CreateQueryUserUGCRequest(AccountID_t unAccountID, EUserUGCList eListType, EUGCMatchingUGCType eMatchingUGCType, EUserUGCListSortOrder eSortOrder, AppId_t nCreatorAppID, AppId_t nConsumerAppID, uint unPage)
		{
			InteropHelp.TestIfAvailableClient();
			return (UGCQueryHandle_t)NativeMethods.ISteamUGC_CreateQueryUserUGCRequest(CSteamAPIContext.GetSteamUGC(), unAccountID, eListType, eMatchingUGCType, eSortOrder, nCreatorAppID, nConsumerAppID, unPage);
		}

		public static UGCQueryHandle_t CreateQueryAllUGCRequest(EUGCQuery eQueryType, EUGCMatchingUGCType eMatchingeMatchingUGCTypeFileType, AppId_t nCreatorAppID, AppId_t nConsumerAppID, uint unPage)
		{
			InteropHelp.TestIfAvailableClient();
			return (UGCQueryHandle_t)NativeMethods.ISteamUGC_CreateQueryAllUGCRequestPage(CSteamAPIContext.GetSteamUGC(), eQueryType, eMatchingeMatchingUGCTypeFileType, nCreatorAppID, nConsumerAppID, unPage);
		}

		public static UGCQueryHandle_t CreateQueryAllUGCRequest(EUGCQuery eQueryType, EUGCMatchingUGCType eMatchingeMatchingUGCTypeFileType, AppId_t nCreatorAppID, AppId_t nConsumerAppID, string pchCursor = null)
		{
			InteropHelp.TestIfAvailableClient();
			UGCQueryHandle_t result;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchCursor))
			{
				result = (UGCQueryHandle_t)NativeMethods.ISteamUGC_CreateQueryAllUGCRequestCursor(CSteamAPIContext.GetSteamUGC(), eQueryType, eMatchingeMatchingUGCTypeFileType, nCreatorAppID, nConsumerAppID, utf8StringHandle);
			}
			return result;
		}

		public static UGCQueryHandle_t CreateQueryUGCDetailsRequest(PublishedFileId_t[] pvecPublishedFileID, uint unNumPublishedFileIDs)
		{
			InteropHelp.TestIfAvailableClient();
			return (UGCQueryHandle_t)NativeMethods.ISteamUGC_CreateQueryUGCDetailsRequest(CSteamAPIContext.GetSteamUGC(), pvecPublishedFileID, unNumPublishedFileIDs);
		}

		public static SteamAPICall_t SendQueryUGCRequest(UGCQueryHandle_t handle)
		{
			InteropHelp.TestIfAvailableClient();
			return (SteamAPICall_t)NativeMethods.ISteamUGC_SendQueryUGCRequest(CSteamAPIContext.GetSteamUGC(), handle);
		}

		public static bool GetQueryUGCResult(UGCQueryHandle_t handle, uint index, out SteamUGCDetails_t pDetails)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_GetQueryUGCResult(CSteamAPIContext.GetSteamUGC(), handle, index, out pDetails);
		}

		public static uint GetQueryUGCNumTags(UGCQueryHandle_t handle, uint index)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_GetQueryUGCNumTags(CSteamAPIContext.GetSteamUGC(), handle, index);
		}

		public static bool GetQueryUGCTag(UGCQueryHandle_t handle, uint index, uint indexTag, out string pchValue, uint cchValueSize)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr intPtr = Marshal.AllocHGlobal((int)cchValueSize);
			bool flag = NativeMethods.ISteamUGC_GetQueryUGCTag(CSteamAPIContext.GetSteamUGC(), handle, index, indexTag, intPtr, cchValueSize);
			pchValue = (flag ? InteropHelp.PtrToStringUTF8(intPtr) : null);
			Marshal.FreeHGlobal(intPtr);
			return flag;
		}

		public static bool GetQueryUGCTagDisplayName(UGCQueryHandle_t handle, uint index, uint indexTag, out string pchValue, uint cchValueSize)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr intPtr = Marshal.AllocHGlobal((int)cchValueSize);
			bool flag = NativeMethods.ISteamUGC_GetQueryUGCTagDisplayName(CSteamAPIContext.GetSteamUGC(), handle, index, indexTag, intPtr, cchValueSize);
			pchValue = (flag ? InteropHelp.PtrToStringUTF8(intPtr) : null);
			Marshal.FreeHGlobal(intPtr);
			return flag;
		}

		public static bool GetQueryUGCPreviewURL(UGCQueryHandle_t handle, uint index, out string pchURL, uint cchURLSize)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr intPtr = Marshal.AllocHGlobal((int)cchURLSize);
			bool flag = NativeMethods.ISteamUGC_GetQueryUGCPreviewURL(CSteamAPIContext.GetSteamUGC(), handle, index, intPtr, cchURLSize);
			pchURL = (flag ? InteropHelp.PtrToStringUTF8(intPtr) : null);
			Marshal.FreeHGlobal(intPtr);
			return flag;
		}

		public static bool GetQueryUGCMetadata(UGCQueryHandle_t handle, uint index, out string pchMetadata, uint cchMetadatasize)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr intPtr = Marshal.AllocHGlobal((int)cchMetadatasize);
			bool flag = NativeMethods.ISteamUGC_GetQueryUGCMetadata(CSteamAPIContext.GetSteamUGC(), handle, index, intPtr, cchMetadatasize);
			pchMetadata = (flag ? InteropHelp.PtrToStringUTF8(intPtr) : null);
			Marshal.FreeHGlobal(intPtr);
			return flag;
		}

		public static bool GetQueryUGCChildren(UGCQueryHandle_t handle, uint index, PublishedFileId_t[] pvecPublishedFileID, uint cMaxEntries)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_GetQueryUGCChildren(CSteamAPIContext.GetSteamUGC(), handle, index, pvecPublishedFileID, cMaxEntries);
		}

		public static bool GetQueryUGCStatistic(UGCQueryHandle_t handle, uint index, EItemStatistic eStatType, out ulong pStatValue)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_GetQueryUGCStatistic(CSteamAPIContext.GetSteamUGC(), handle, index, eStatType, out pStatValue);
		}

		public static uint GetQueryUGCNumAdditionalPreviews(UGCQueryHandle_t handle, uint index)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_GetQueryUGCNumAdditionalPreviews(CSteamAPIContext.GetSteamUGC(), handle, index);
		}

		public static bool GetQueryUGCAdditionalPreview(UGCQueryHandle_t handle, uint index, uint previewIndex, out string pchURLOrVideoID, uint cchURLSize, out string pchOriginalFileName, uint cchOriginalFileNameSize, out EItemPreviewType pPreviewType)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr intPtr = Marshal.AllocHGlobal((int)cchURLSize);
			IntPtr intPtr2 = Marshal.AllocHGlobal((int)cchOriginalFileNameSize);
			bool flag = NativeMethods.ISteamUGC_GetQueryUGCAdditionalPreview(CSteamAPIContext.GetSteamUGC(), handle, index, previewIndex, intPtr, cchURLSize, intPtr2, cchOriginalFileNameSize, out pPreviewType);
			pchURLOrVideoID = (flag ? InteropHelp.PtrToStringUTF8(intPtr) : null);
			Marshal.FreeHGlobal(intPtr);
			pchOriginalFileName = (flag ? InteropHelp.PtrToStringUTF8(intPtr2) : null);
			Marshal.FreeHGlobal(intPtr2);
			return flag;
		}

		public static uint GetQueryUGCNumKeyValueTags(UGCQueryHandle_t handle, uint index)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_GetQueryUGCNumKeyValueTags(CSteamAPIContext.GetSteamUGC(), handle, index);
		}

		public static bool GetQueryUGCKeyValueTag(UGCQueryHandle_t handle, uint index, uint keyValueTagIndex, out string pchKey, uint cchKeySize, out string pchValue, uint cchValueSize)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr intPtr = Marshal.AllocHGlobal((int)cchKeySize);
			IntPtr intPtr2 = Marshal.AllocHGlobal((int)cchValueSize);
			bool flag = NativeMethods.ISteamUGC_GetQueryUGCKeyValueTag(CSteamAPIContext.GetSteamUGC(), handle, index, keyValueTagIndex, intPtr, cchKeySize, intPtr2, cchValueSize);
			pchKey = (flag ? InteropHelp.PtrToStringUTF8(intPtr) : null);
			Marshal.FreeHGlobal(intPtr);
			pchValue = (flag ? InteropHelp.PtrToStringUTF8(intPtr2) : null);
			Marshal.FreeHGlobal(intPtr2);
			return flag;
		}

		public static bool GetQueryUGCKeyValueTag(UGCQueryHandle_t handle, uint index, string pchKey, out string pchValue, uint cchValueSize)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr intPtr = Marshal.AllocHGlobal((int)cchValueSize);
			bool result;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchKey))
			{
				bool flag = NativeMethods.ISteamUGC_GetQueryFirstUGCKeyValueTag(CSteamAPIContext.GetSteamUGC(), handle, index, utf8StringHandle, intPtr, cchValueSize);
				pchValue = (flag ? InteropHelp.PtrToStringUTF8(intPtr) : null);
				Marshal.FreeHGlobal(intPtr);
				result = flag;
			}
			return result;
		}

		public static uint GetNumSupportedGameVersions(UGCQueryHandle_t handle, uint index)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_GetNumSupportedGameVersions(CSteamAPIContext.GetSteamUGC(), handle, index);
		}

		public static bool GetSupportedGameVersionData(UGCQueryHandle_t handle, uint index, uint versionIndex, out string pchGameBranchMin, out string pchGameBranchMax, uint cchGameBranchSize)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr intPtr = Marshal.AllocHGlobal((int)cchGameBranchSize);
			IntPtr intPtr2 = Marshal.AllocHGlobal((int)cchGameBranchSize);
			bool flag = NativeMethods.ISteamUGC_GetSupportedGameVersionData(CSteamAPIContext.GetSteamUGC(), handle, index, versionIndex, intPtr, intPtr2, cchGameBranchSize);
			pchGameBranchMin = (flag ? InteropHelp.PtrToStringUTF8(intPtr) : null);
			Marshal.FreeHGlobal(intPtr);
			pchGameBranchMax = (flag ? InteropHelp.PtrToStringUTF8(intPtr2) : null);
			Marshal.FreeHGlobal(intPtr2);
			return flag;
		}

		public static uint GetQueryUGCContentDescriptors(UGCQueryHandle_t handle, uint index, EUGCContentDescriptorID[] pvecDescriptors, uint cMaxEntries)
		{
			InteropHelp.TestIfAvailableClient();
			if (pvecDescriptors != null && (long)pvecDescriptors.Length != (long)((ulong)cMaxEntries))
			{
				throw new ArgumentException("pvecDescriptors must be the same size as cMaxEntries!");
			}
			return NativeMethods.ISteamUGC_GetQueryUGCContentDescriptors(CSteamAPIContext.GetSteamUGC(), handle, index, pvecDescriptors, cMaxEntries);
		}

		public static bool ReleaseQueryUGCRequest(UGCQueryHandle_t handle)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_ReleaseQueryUGCRequest(CSteamAPIContext.GetSteamUGC(), handle);
		}

		public static bool AddRequiredTag(UGCQueryHandle_t handle, string pTagName)
		{
			InteropHelp.TestIfAvailableClient();
			bool result;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pTagName))
			{
				result = NativeMethods.ISteamUGC_AddRequiredTag(CSteamAPIContext.GetSteamUGC(), handle, utf8StringHandle);
			}
			return result;
		}

		public static bool AddRequiredTagGroup(UGCQueryHandle_t handle, IList<string> pTagGroups)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_AddRequiredTagGroup(CSteamAPIContext.GetSteamUGC(), handle, new InteropHelp.SteamParamStringArray(pTagGroups));
		}

		public static bool AddExcludedTag(UGCQueryHandle_t handle, string pTagName)
		{
			InteropHelp.TestIfAvailableClient();
			bool result;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pTagName))
			{
				result = NativeMethods.ISteamUGC_AddExcludedTag(CSteamAPIContext.GetSteamUGC(), handle, utf8StringHandle);
			}
			return result;
		}

		public static bool SetReturnOnlyIDs(UGCQueryHandle_t handle, bool bReturnOnlyIDs)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_SetReturnOnlyIDs(CSteamAPIContext.GetSteamUGC(), handle, bReturnOnlyIDs);
		}

		public static bool SetReturnKeyValueTags(UGCQueryHandle_t handle, bool bReturnKeyValueTags)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_SetReturnKeyValueTags(CSteamAPIContext.GetSteamUGC(), handle, bReturnKeyValueTags);
		}

		public static bool SetReturnLongDescription(UGCQueryHandle_t handle, bool bReturnLongDescription)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_SetReturnLongDescription(CSteamAPIContext.GetSteamUGC(), handle, bReturnLongDescription);
		}

		public static bool SetReturnMetadata(UGCQueryHandle_t handle, bool bReturnMetadata)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_SetReturnMetadata(CSteamAPIContext.GetSteamUGC(), handle, bReturnMetadata);
		}

		public static bool SetReturnChildren(UGCQueryHandle_t handle, bool bReturnChildren)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_SetReturnChildren(CSteamAPIContext.GetSteamUGC(), handle, bReturnChildren);
		}

		public static bool SetReturnAdditionalPreviews(UGCQueryHandle_t handle, bool bReturnAdditionalPreviews)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_SetReturnAdditionalPreviews(CSteamAPIContext.GetSteamUGC(), handle, bReturnAdditionalPreviews);
		}

		public static bool SetReturnTotalOnly(UGCQueryHandle_t handle, bool bReturnTotalOnly)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_SetReturnTotalOnly(CSteamAPIContext.GetSteamUGC(), handle, bReturnTotalOnly);
		}

		public static bool SetReturnPlaytimeStats(UGCQueryHandle_t handle, uint unDays)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_SetReturnPlaytimeStats(CSteamAPIContext.GetSteamUGC(), handle, unDays);
		}

		public static bool SetLanguage(UGCQueryHandle_t handle, string pchLanguage)
		{
			InteropHelp.TestIfAvailableClient();
			bool result;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchLanguage))
			{
				result = NativeMethods.ISteamUGC_SetLanguage(CSteamAPIContext.GetSteamUGC(), handle, utf8StringHandle);
			}
			return result;
		}

		public static bool SetAllowCachedResponse(UGCQueryHandle_t handle, uint unMaxAgeSeconds)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_SetAllowCachedResponse(CSteamAPIContext.GetSteamUGC(), handle, unMaxAgeSeconds);
		}

		public static bool SetAdminQuery(UGCUpdateHandle_t handle, bool bAdminQuery)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_SetAdminQuery(CSteamAPIContext.GetSteamUGC(), handle, bAdminQuery);
		}

		public static bool SetCloudFileNameFilter(UGCQueryHandle_t handle, string pMatchCloudFileName)
		{
			InteropHelp.TestIfAvailableClient();
			bool result;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pMatchCloudFileName))
			{
				result = NativeMethods.ISteamUGC_SetCloudFileNameFilter(CSteamAPIContext.GetSteamUGC(), handle, utf8StringHandle);
			}
			return result;
		}

		public static bool SetMatchAnyTag(UGCQueryHandle_t handle, bool bMatchAnyTag)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_SetMatchAnyTag(CSteamAPIContext.GetSteamUGC(), handle, bMatchAnyTag);
		}

		public static bool SetSearchText(UGCQueryHandle_t handle, string pSearchText)
		{
			InteropHelp.TestIfAvailableClient();
			bool result;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pSearchText))
			{
				result = NativeMethods.ISteamUGC_SetSearchText(CSteamAPIContext.GetSteamUGC(), handle, utf8StringHandle);
			}
			return result;
		}

		public static bool SetRankedByTrendDays(UGCQueryHandle_t handle, uint unDays)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_SetRankedByTrendDays(CSteamAPIContext.GetSteamUGC(), handle, unDays);
		}

		public static bool SetTimeCreatedDateRange(UGCQueryHandle_t handle, uint rtStart, uint rtEnd)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_SetTimeCreatedDateRange(CSteamAPIContext.GetSteamUGC(), handle, rtStart, rtEnd);
		}

		public static bool SetTimeUpdatedDateRange(UGCQueryHandle_t handle, uint rtStart, uint rtEnd)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_SetTimeUpdatedDateRange(CSteamAPIContext.GetSteamUGC(), handle, rtStart, rtEnd);
		}

		public static bool AddRequiredKeyValueTag(UGCQueryHandle_t handle, string pKey, string pValue)
		{
			InteropHelp.TestIfAvailableClient();
			bool result;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pKey))
			{
				using (InteropHelp.UTF8StringHandle utf8StringHandle2 = new InteropHelp.UTF8StringHandle(pValue))
				{
					result = NativeMethods.ISteamUGC_AddRequiredKeyValueTag(CSteamAPIContext.GetSteamUGC(), handle, utf8StringHandle, utf8StringHandle2);
				}
			}
			return result;
		}

		public static SteamAPICall_t RequestUGCDetails(PublishedFileId_t nPublishedFileID, uint unMaxAgeSeconds)
		{
			InteropHelp.TestIfAvailableClient();
			return (SteamAPICall_t)NativeMethods.ISteamUGC_RequestUGCDetails(CSteamAPIContext.GetSteamUGC(), nPublishedFileID, unMaxAgeSeconds);
		}

		public static SteamAPICall_t CreateItem(AppId_t nConsumerAppId, EWorkshopFileType eFileType)
		{
			InteropHelp.TestIfAvailableClient();
			return (SteamAPICall_t)NativeMethods.ISteamUGC_CreateItem(CSteamAPIContext.GetSteamUGC(), nConsumerAppId, eFileType);
		}

		public static UGCUpdateHandle_t StartItemUpdate(AppId_t nConsumerAppId, PublishedFileId_t nPublishedFileID)
		{
			InteropHelp.TestIfAvailableClient();
			return (UGCUpdateHandle_t)NativeMethods.ISteamUGC_StartItemUpdate(CSteamAPIContext.GetSteamUGC(), nConsumerAppId, nPublishedFileID);
		}

		public static bool SetItemTitle(UGCUpdateHandle_t handle, string pchTitle)
		{
			InteropHelp.TestIfAvailableClient();
			bool result;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchTitle))
			{
				result = NativeMethods.ISteamUGC_SetItemTitle(CSteamAPIContext.GetSteamUGC(), handle, utf8StringHandle);
			}
			return result;
		}

		public static bool SetItemDescription(UGCUpdateHandle_t handle, string pchDescription)
		{
			InteropHelp.TestIfAvailableClient();
			bool result;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchDescription))
			{
				result = NativeMethods.ISteamUGC_SetItemDescription(CSteamAPIContext.GetSteamUGC(), handle, utf8StringHandle);
			}
			return result;
		}

		public static bool SetItemUpdateLanguage(UGCUpdateHandle_t handle, string pchLanguage)
		{
			InteropHelp.TestIfAvailableClient();
			bool result;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchLanguage))
			{
				result = NativeMethods.ISteamUGC_SetItemUpdateLanguage(CSteamAPIContext.GetSteamUGC(), handle, utf8StringHandle);
			}
			return result;
		}

		public static bool SetItemMetadata(UGCUpdateHandle_t handle, string pchMetaData)
		{
			InteropHelp.TestIfAvailableClient();
			bool result;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchMetaData))
			{
				result = NativeMethods.ISteamUGC_SetItemMetadata(CSteamAPIContext.GetSteamUGC(), handle, utf8StringHandle);
			}
			return result;
		}

		public static bool SetItemVisibility(UGCUpdateHandle_t handle, ERemoteStoragePublishedFileVisibility eVisibility)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_SetItemVisibility(CSteamAPIContext.GetSteamUGC(), handle, eVisibility);
		}

		public static bool SetItemTags(UGCUpdateHandle_t updateHandle, IList<string> pTags, bool bAllowAdminTags = false)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_SetItemTags(CSteamAPIContext.GetSteamUGC(), updateHandle, new InteropHelp.SteamParamStringArray(pTags), bAllowAdminTags);
		}

		public static bool SetItemContent(UGCUpdateHandle_t handle, string pszContentFolder)
		{
			InteropHelp.TestIfAvailableClient();
			bool result;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pszContentFolder))
			{
				result = NativeMethods.ISteamUGC_SetItemContent(CSteamAPIContext.GetSteamUGC(), handle, utf8StringHandle);
			}
			return result;
		}

		public static bool SetItemPreview(UGCUpdateHandle_t handle, string pszPreviewFile)
		{
			InteropHelp.TestIfAvailableClient();
			bool result;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pszPreviewFile))
			{
				result = NativeMethods.ISteamUGC_SetItemPreview(CSteamAPIContext.GetSteamUGC(), handle, utf8StringHandle);
			}
			return result;
		}

		public static bool SetAllowLegacyUpload(UGCUpdateHandle_t handle, bool bAllowLegacyUpload)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_SetAllowLegacyUpload(CSteamAPIContext.GetSteamUGC(), handle, bAllowLegacyUpload);
		}

		public static bool RemoveAllItemKeyValueTags(UGCUpdateHandle_t handle)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_RemoveAllItemKeyValueTags(CSteamAPIContext.GetSteamUGC(), handle);
		}

		public static bool RemoveItemKeyValueTags(UGCUpdateHandle_t handle, string pchKey)
		{
			InteropHelp.TestIfAvailableClient();
			bool result;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchKey))
			{
				result = NativeMethods.ISteamUGC_RemoveItemKeyValueTags(CSteamAPIContext.GetSteamUGC(), handle, utf8StringHandle);
			}
			return result;
		}

		public static bool AddItemKeyValueTag(UGCUpdateHandle_t handle, string pchKey, string pchValue)
		{
			InteropHelp.TestIfAvailableClient();
			bool result;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchKey))
			{
				using (InteropHelp.UTF8StringHandle utf8StringHandle2 = new InteropHelp.UTF8StringHandle(pchValue))
				{
					result = NativeMethods.ISteamUGC_AddItemKeyValueTag(CSteamAPIContext.GetSteamUGC(), handle, utf8StringHandle, utf8StringHandle2);
				}
			}
			return result;
		}

		public static bool AddItemPreviewFile(UGCUpdateHandle_t handle, string pszPreviewFile, EItemPreviewType type)
		{
			InteropHelp.TestIfAvailableClient();
			bool result;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pszPreviewFile))
			{
				result = NativeMethods.ISteamUGC_AddItemPreviewFile(CSteamAPIContext.GetSteamUGC(), handle, utf8StringHandle, type);
			}
			return result;
		}

		public static bool AddItemPreviewVideo(UGCUpdateHandle_t handle, string pszVideoID)
		{
			InteropHelp.TestIfAvailableClient();
			bool result;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pszVideoID))
			{
				result = NativeMethods.ISteamUGC_AddItemPreviewVideo(CSteamAPIContext.GetSteamUGC(), handle, utf8StringHandle);
			}
			return result;
		}

		public static bool UpdateItemPreviewFile(UGCUpdateHandle_t handle, uint index, string pszPreviewFile)
		{
			InteropHelp.TestIfAvailableClient();
			bool result;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pszPreviewFile))
			{
				result = NativeMethods.ISteamUGC_UpdateItemPreviewFile(CSteamAPIContext.GetSteamUGC(), handle, index, utf8StringHandle);
			}
			return result;
		}

		public static bool UpdateItemPreviewVideo(UGCUpdateHandle_t handle, uint index, string pszVideoID)
		{
			InteropHelp.TestIfAvailableClient();
			bool result;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pszVideoID))
			{
				result = NativeMethods.ISteamUGC_UpdateItemPreviewVideo(CSteamAPIContext.GetSteamUGC(), handle, index, utf8StringHandle);
			}
			return result;
		}

		public static bool RemoveItemPreview(UGCUpdateHandle_t handle, uint index)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_RemoveItemPreview(CSteamAPIContext.GetSteamUGC(), handle, index);
		}

		public static bool AddContentDescriptor(UGCUpdateHandle_t handle, EUGCContentDescriptorID descid)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_AddContentDescriptor(CSteamAPIContext.GetSteamUGC(), handle, descid);
		}

		public static bool RemoveContentDescriptor(UGCUpdateHandle_t handle, EUGCContentDescriptorID descid)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_RemoveContentDescriptor(CSteamAPIContext.GetSteamUGC(), handle, descid);
		}

		public static bool SetRequiredGameVersions(UGCUpdateHandle_t handle, string pszGameBranchMin, string pszGameBranchMax)
		{
			InteropHelp.TestIfAvailableClient();
			bool result;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pszGameBranchMin))
			{
				using (InteropHelp.UTF8StringHandle utf8StringHandle2 = new InteropHelp.UTF8StringHandle(pszGameBranchMax))
				{
					result = NativeMethods.ISteamUGC_SetRequiredGameVersions(CSteamAPIContext.GetSteamUGC(), handle, utf8StringHandle, utf8StringHandle2);
				}
			}
			return result;
		}

		public static SteamAPICall_t SubmitItemUpdate(UGCUpdateHandle_t handle, string pchChangeNote)
		{
			InteropHelp.TestIfAvailableClient();
			SteamAPICall_t result;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchChangeNote))
			{
				result = (SteamAPICall_t)NativeMethods.ISteamUGC_SubmitItemUpdate(CSteamAPIContext.GetSteamUGC(), handle, utf8StringHandle);
			}
			return result;
		}

		public static EItemUpdateStatus GetItemUpdateProgress(UGCUpdateHandle_t handle, out ulong punBytesProcessed, out ulong punBytesTotal)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_GetItemUpdateProgress(CSteamAPIContext.GetSteamUGC(), handle, out punBytesProcessed, out punBytesTotal);
		}

		public static SteamAPICall_t SetUserItemVote(PublishedFileId_t nPublishedFileID, bool bVoteUp)
		{
			InteropHelp.TestIfAvailableClient();
			return (SteamAPICall_t)NativeMethods.ISteamUGC_SetUserItemVote(CSteamAPIContext.GetSteamUGC(), nPublishedFileID, bVoteUp);
		}

		public static SteamAPICall_t GetUserItemVote(PublishedFileId_t nPublishedFileID)
		{
			InteropHelp.TestIfAvailableClient();
			return (SteamAPICall_t)NativeMethods.ISteamUGC_GetUserItemVote(CSteamAPIContext.GetSteamUGC(), nPublishedFileID);
		}

		public static SteamAPICall_t AddItemToFavorites(AppId_t nAppId, PublishedFileId_t nPublishedFileID)
		{
			InteropHelp.TestIfAvailableClient();
			return (SteamAPICall_t)NativeMethods.ISteamUGC_AddItemToFavorites(CSteamAPIContext.GetSteamUGC(), nAppId, nPublishedFileID);
		}

		public static SteamAPICall_t RemoveItemFromFavorites(AppId_t nAppId, PublishedFileId_t nPublishedFileID)
		{
			InteropHelp.TestIfAvailableClient();
			return (SteamAPICall_t)NativeMethods.ISteamUGC_RemoveItemFromFavorites(CSteamAPIContext.GetSteamUGC(), nAppId, nPublishedFileID);
		}

		public static SteamAPICall_t SubscribeItem(PublishedFileId_t nPublishedFileID)
		{
			InteropHelp.TestIfAvailableClient();
			return (SteamAPICall_t)NativeMethods.ISteamUGC_SubscribeItem(CSteamAPIContext.GetSteamUGC(), nPublishedFileID);
		}

		public static SteamAPICall_t UnsubscribeItem(PublishedFileId_t nPublishedFileID)
		{
			InteropHelp.TestIfAvailableClient();
			return (SteamAPICall_t)NativeMethods.ISteamUGC_UnsubscribeItem(CSteamAPIContext.GetSteamUGC(), nPublishedFileID);
		}

		public static uint GetNumSubscribedItems()
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_GetNumSubscribedItems(CSteamAPIContext.GetSteamUGC());
		}

		public static uint GetSubscribedItems(PublishedFileId_t[] pvecPublishedFileID, uint cMaxEntries)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_GetSubscribedItems(CSteamAPIContext.GetSteamUGC(), pvecPublishedFileID, cMaxEntries);
		}

		public static uint GetItemState(PublishedFileId_t nPublishedFileID)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_GetItemState(CSteamAPIContext.GetSteamUGC(), nPublishedFileID);
		}

		public static bool GetItemInstallInfo(PublishedFileId_t nPublishedFileID, out ulong punSizeOnDisk, out string pchFolder, uint cchFolderSize, out uint punTimeStamp)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr intPtr = Marshal.AllocHGlobal((int)cchFolderSize);
			bool flag = NativeMethods.ISteamUGC_GetItemInstallInfo(CSteamAPIContext.GetSteamUGC(), nPublishedFileID, out punSizeOnDisk, intPtr, cchFolderSize, out punTimeStamp);
			pchFolder = (flag ? InteropHelp.PtrToStringUTF8(intPtr) : null);
			Marshal.FreeHGlobal(intPtr);
			return flag;
		}

		public static bool GetItemDownloadInfo(PublishedFileId_t nPublishedFileID, out ulong punBytesDownloaded, out ulong punBytesTotal)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_GetItemDownloadInfo(CSteamAPIContext.GetSteamUGC(), nPublishedFileID, out punBytesDownloaded, out punBytesTotal);
		}

		public static bool DownloadItem(PublishedFileId_t nPublishedFileID, bool bHighPriority)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_DownloadItem(CSteamAPIContext.GetSteamUGC(), nPublishedFileID, bHighPriority);
		}

		public static bool BInitWorkshopForGameServer(DepotId_t unWorkshopDepotID, string pszFolder)
		{
			InteropHelp.TestIfAvailableClient();
			bool result;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pszFolder))
			{
				result = NativeMethods.ISteamUGC_BInitWorkshopForGameServer(CSteamAPIContext.GetSteamUGC(), unWorkshopDepotID, utf8StringHandle);
			}
			return result;
		}

		public static void SuspendDownloads(bool bSuspend)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamUGC_SuspendDownloads(CSteamAPIContext.GetSteamUGC(), bSuspend);
		}

		public static SteamAPICall_t StartPlaytimeTracking(PublishedFileId_t[] pvecPublishedFileID, uint unNumPublishedFileIDs)
		{
			InteropHelp.TestIfAvailableClient();
			return (SteamAPICall_t)NativeMethods.ISteamUGC_StartPlaytimeTracking(CSteamAPIContext.GetSteamUGC(), pvecPublishedFileID, unNumPublishedFileIDs);
		}

		public static SteamAPICall_t StopPlaytimeTracking(PublishedFileId_t[] pvecPublishedFileID, uint unNumPublishedFileIDs)
		{
			InteropHelp.TestIfAvailableClient();
			return (SteamAPICall_t)NativeMethods.ISteamUGC_StopPlaytimeTracking(CSteamAPIContext.GetSteamUGC(), pvecPublishedFileID, unNumPublishedFileIDs);
		}

		public static SteamAPICall_t StopPlaytimeTrackingForAllItems()
		{
			InteropHelp.TestIfAvailableClient();
			return (SteamAPICall_t)NativeMethods.ISteamUGC_StopPlaytimeTrackingForAllItems(CSteamAPIContext.GetSteamUGC());
		}

		public static SteamAPICall_t AddDependency(PublishedFileId_t nParentPublishedFileID, PublishedFileId_t nChildPublishedFileID)
		{
			InteropHelp.TestIfAvailableClient();
			return (SteamAPICall_t)NativeMethods.ISteamUGC_AddDependency(CSteamAPIContext.GetSteamUGC(), nParentPublishedFileID, nChildPublishedFileID);
		}

		public static SteamAPICall_t RemoveDependency(PublishedFileId_t nParentPublishedFileID, PublishedFileId_t nChildPublishedFileID)
		{
			InteropHelp.TestIfAvailableClient();
			return (SteamAPICall_t)NativeMethods.ISteamUGC_RemoveDependency(CSteamAPIContext.GetSteamUGC(), nParentPublishedFileID, nChildPublishedFileID);
		}

		public static SteamAPICall_t AddAppDependency(PublishedFileId_t nPublishedFileID, AppId_t nAppID)
		{
			InteropHelp.TestIfAvailableClient();
			return (SteamAPICall_t)NativeMethods.ISteamUGC_AddAppDependency(CSteamAPIContext.GetSteamUGC(), nPublishedFileID, nAppID);
		}

		public static SteamAPICall_t RemoveAppDependency(PublishedFileId_t nPublishedFileID, AppId_t nAppID)
		{
			InteropHelp.TestIfAvailableClient();
			return (SteamAPICall_t)NativeMethods.ISteamUGC_RemoveAppDependency(CSteamAPIContext.GetSteamUGC(), nPublishedFileID, nAppID);
		}

		public static SteamAPICall_t GetAppDependencies(PublishedFileId_t nPublishedFileID)
		{
			InteropHelp.TestIfAvailableClient();
			return (SteamAPICall_t)NativeMethods.ISteamUGC_GetAppDependencies(CSteamAPIContext.GetSteamUGC(), nPublishedFileID);
		}

		public static SteamAPICall_t DeleteItem(PublishedFileId_t nPublishedFileID)
		{
			InteropHelp.TestIfAvailableClient();
			return (SteamAPICall_t)NativeMethods.ISteamUGC_DeleteItem(CSteamAPIContext.GetSteamUGC(), nPublishedFileID);
		}

		public static bool ShowWorkshopEULA()
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_ShowWorkshopEULA(CSteamAPIContext.GetSteamUGC());
		}

		public static SteamAPICall_t GetWorkshopEULAStatus()
		{
			InteropHelp.TestIfAvailableClient();
			return (SteamAPICall_t)NativeMethods.ISteamUGC_GetWorkshopEULAStatus(CSteamAPIContext.GetSteamUGC());
		}

		public static uint GetUserContentDescriptorPreferences(EUGCContentDescriptorID[] pvecDescriptors, uint cMaxEntries)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUGC_GetUserContentDescriptorPreferences(CSteamAPIContext.GetSteamUGC(), pvecDescriptors, cMaxEntries);
		}
	}
}
