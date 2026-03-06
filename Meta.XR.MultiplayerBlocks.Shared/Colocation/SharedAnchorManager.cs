using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Meta.XR.BuildingBlocks;
using UnityEngine;

namespace Meta.XR.MultiplayerBlocks.Colocation
{
	internal class SharedAnchorManager
	{
		public GameObject AnchorPrefab { get; set; }

		public IReadOnlyList<OVRSpatialAnchor> LocalAnchors
		{
			get
			{
				return this._localAnchors;
			}
		}

		public SharedAnchorManager(SharedSpatialAnchorCore ssaCore)
		{
			this._ssaCore = ssaCore;
		}

		public Task<OVRSpatialAnchor> CreateAlignmentAnchor()
		{
			SharedAnchorManager.<CreateAlignmentAnchor>d__19 <CreateAlignmentAnchor>d__;
			<CreateAlignmentAnchor>d__.<>t__builder = AsyncTaskMethodBuilder<OVRSpatialAnchor>.Create();
			<CreateAlignmentAnchor>d__.<>4__this = this;
			<CreateAlignmentAnchor>d__.<>1__state = -1;
			<CreateAlignmentAnchor>d__.<>t__builder.Start<SharedAnchorManager.<CreateAlignmentAnchor>d__19>(ref <CreateAlignmentAnchor>d__);
			return <CreateAlignmentAnchor>d__.<>t__builder.Task;
		}

		private Task<ValueTuple<OVRSpatialAnchor, OVRSpatialAnchor.OperationResult>> CreateAnchor(Vector3 position, Quaternion orientation)
		{
			SharedAnchorManager.<CreateAnchor>d__20 <CreateAnchor>d__;
			<CreateAnchor>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<OVRSpatialAnchor, OVRSpatialAnchor.OperationResult>>.Create();
			<CreateAnchor>d__.<>4__this = this;
			<CreateAnchor>d__.position = position;
			<CreateAnchor>d__.orientation = orientation;
			<CreateAnchor>d__.<>1__state = -1;
			<CreateAnchor>d__.<>t__builder.Start<SharedAnchorManager.<CreateAnchor>d__20>(ref <CreateAnchor>d__);
			return <CreateAnchor>d__.<>t__builder.Task;
		}

		private Task<ValueTuple<OVRSpatialAnchor, OVRSpatialAnchor.OperationResult>> AnchorCreationTask(Vector3 position, Quaternion orientation)
		{
			SharedAnchorManager.<AnchorCreationTask>d__21 <AnchorCreationTask>d__;
			<AnchorCreationTask>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<OVRSpatialAnchor, OVRSpatialAnchor.OperationResult>>.Create();
			<AnchorCreationTask>d__.<>4__this = this;
			<AnchorCreationTask>d__.position = position;
			<AnchorCreationTask>d__.orientation = orientation;
			<AnchorCreationTask>d__.<>1__state = -1;
			<AnchorCreationTask>d__.<>t__builder.Start<SharedAnchorManager.<AnchorCreationTask>d__21>(ref <AnchorCreationTask>d__);
			return <AnchorCreationTask>d__.<>t__builder.Task;
		}

		private void CheckIfSavingAnchorsServiceHung()
		{
			SharedAnchorManager.<CheckIfSavingAnchorsServiceHung>d__22 <CheckIfSavingAnchorsServiceHung>d__;
			<CheckIfSavingAnchorsServiceHung>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<CheckIfSavingAnchorsServiceHung>d__.<>4__this = this;
			<CheckIfSavingAnchorsServiceHung>d__.<>1__state = -1;
			<CheckIfSavingAnchorsServiceHung>d__.<>t__builder.Start<SharedAnchorManager.<CheckIfSavingAnchorsServiceHung>d__22>(ref <CheckIfSavingAnchorsServiceHung>d__);
		}

		public Task<IReadOnlyList<OVRSpatialAnchor>> RetrieveAnchorsFromGroup(Guid groupUuid)
		{
			SharedAnchorManager.<RetrieveAnchorsFromGroup>d__23 <RetrieveAnchorsFromGroup>d__;
			<RetrieveAnchorsFromGroup>d__.<>t__builder = AsyncTaskMethodBuilder<IReadOnlyList<OVRSpatialAnchor>>.Create();
			<RetrieveAnchorsFromGroup>d__.<>4__this = this;
			<RetrieveAnchorsFromGroup>d__.groupUuid = groupUuid;
			<RetrieveAnchorsFromGroup>d__.<>1__state = -1;
			<RetrieveAnchorsFromGroup>d__.<>t__builder.Start<SharedAnchorManager.<RetrieveAnchorsFromGroup>d__23>(ref <RetrieveAnchorsFromGroup>d__);
			return <RetrieveAnchorsFromGroup>d__.<>t__builder.Task;
		}

		public Task<IReadOnlyList<OVRSpatialAnchor>> RetrieveAnchors(List<Guid> anchorIds)
		{
			SharedAnchorManager.<RetrieveAnchors>d__24 <RetrieveAnchors>d__;
			<RetrieveAnchors>d__.<>t__builder = AsyncTaskMethodBuilder<IReadOnlyList<OVRSpatialAnchor>>.Create();
			<RetrieveAnchors>d__.<>4__this = this;
			<RetrieveAnchors>d__.anchorIds = anchorIds;
			<RetrieveAnchors>d__.<>1__state = -1;
			<RetrieveAnchors>d__.<>t__builder.Start<SharedAnchorManager.<RetrieveAnchors>d__24>(ref <RetrieveAnchors>d__);
			return <RetrieveAnchors>d__.<>t__builder.Task;
		}

		private void CheckIfRetrievingAnchorServiceHung()
		{
			SharedAnchorManager.<CheckIfRetrievingAnchorServiceHung>d__25 <CheckIfRetrievingAnchorServiceHung>d__;
			<CheckIfRetrievingAnchorServiceHung>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<CheckIfRetrievingAnchorServiceHung>d__.<>4__this = this;
			<CheckIfRetrievingAnchorServiceHung>d__.<>1__state = -1;
			<CheckIfRetrievingAnchorServiceHung>d__.<>t__builder.Start<SharedAnchorManager.<CheckIfRetrievingAnchorServiceHung>d__25>(ref <CheckIfRetrievingAnchorServiceHung>d__);
		}

		public Task<bool> ShareAnchorsWithGroup(Guid groupUuid)
		{
			SharedAnchorManager.<ShareAnchorsWithGroup>d__26 <ShareAnchorsWithGroup>d__;
			<ShareAnchorsWithGroup>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<ShareAnchorsWithGroup>d__.<>4__this = this;
			<ShareAnchorsWithGroup>d__.groupUuid = groupUuid;
			<ShareAnchorsWithGroup>d__.<>1__state = -1;
			<ShareAnchorsWithGroup>d__.<>t__builder.Start<SharedAnchorManager.<ShareAnchorsWithGroup>d__26>(ref <ShareAnchorsWithGroup>d__);
			return <ShareAnchorsWithGroup>d__.<>t__builder.Task;
		}

		public Task<bool> ShareAnchorsWithUser(ulong userId)
		{
			SharedAnchorManager.<ShareAnchorsWithUser>d__27 <ShareAnchorsWithUser>d__;
			<ShareAnchorsWithUser>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<ShareAnchorsWithUser>d__.<>4__this = this;
			<ShareAnchorsWithUser>d__.userId = userId;
			<ShareAnchorsWithUser>d__.<>1__state = -1;
			<ShareAnchorsWithUser>d__.<>t__builder.Start<SharedAnchorManager.<ShareAnchorsWithUser>d__27>(ref <ShareAnchorsWithUser>d__);
			return <ShareAnchorsWithUser>d__.<>t__builder.Task;
		}

		private void CheckIfSharingAnchorServiceHung()
		{
			SharedAnchorManager.<CheckIfSharingAnchorServiceHung>d__28 <CheckIfSharingAnchorServiceHung>d__;
			<CheckIfSharingAnchorServiceHung>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<CheckIfSharingAnchorServiceHung>d__.<>4__this = this;
			<CheckIfSharingAnchorServiceHung>d__.<>1__state = -1;
			<CheckIfSharingAnchorServiceHung>d__.<>t__builder.Start<SharedAnchorManager.<CheckIfSharingAnchorServiceHung>d__28>(ref <CheckIfSharingAnchorServiceHung>d__);
		}

		public void StopSharingAnchorsWithUser(ulong userId)
		{
			this._userShareList.RemoveWhere((OVRSpaceUser el) => el.Id == userId);
		}

		private readonly List<OVRSpatialAnchor> _localAnchors = new List<OVRSpatialAnchor>();

		private readonly List<OVRSpatialAnchor> _sharedAnchors = new List<OVRSpatialAnchor>();

		private readonly HashSet<OVRSpaceUser> _userShareList = new HashSet<OVRSpaceUser>();

		private const int SaveAnchorWaitTimeThreshold = 10000;

		private bool _saveAnchorSaveToCloudIsSuccessful;

		private const int ShareAnchorWaitTimeThreshold = 10000;

		private bool _shareAnchorIsSuccessful;

		private const int RetrieveAnchorWaitTimeThreshold = 10000;

		private bool _retrieveAnchorIsSuccessful;

		private List<Task> _localizationTasks;

		private List<TaskCompletionSource<bool>> _localizationTcsList;

		private SharedSpatialAnchorCore _ssaCore;
	}
}
