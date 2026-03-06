using System;
using System.Text;
using Fusion;
using UnityEngine.Scripting;

namespace Meta.XR.MultiplayerBlocks.Colocation.Fusion
{
	[NetworkBehaviourWeaved(76)]
	internal class FusionMessenger : NetworkBehaviour, INetworkMessenger
	{
		[Networked]
		[Capacity(10)]
		[NetworkedWeaved(0, 33)]
		[NetworkedWeavedLinkedList(10, 1, typeof(ElementReaderWriterInt32))]
		private unsafe NetworkLinkedList<int> _networkIds
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing FusionMessenger._networkIds. Networked properties can only be accessed when Spawned() has been called.");
				}
				return new NetworkLinkedList<int>((byte*)(this.Ptr + 0), 10, ElementReaderWriterInt32.GetInstance());
			}
		}

		[Networked]
		[Capacity(10)]
		[NetworkedWeaved(33, 43)]
		[NetworkedWeavedLinkedList(10, 2, typeof(ElementReaderWriterUInt64))]
		private unsafe NetworkLinkedList<ulong> _playerIds
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing FusionMessenger._playerIds. Networked properties can only be accessed when Spawned() has been called.");
				}
				return new NetworkLinkedList<ulong>((byte*)(this.Ptr + 33), 10, ElementReaderWriterUInt64.GetInstance());
			}
		}

		public event Action<ShareAndLocalizeParams> AnchorShareRequestReceived;

		public event Action<ShareAndLocalizeParams> AnchorShareRequestCompleted;

		public void RegisterLocalPlayer(ulong localPlayerId)
		{
			Logger.Log(string.Format("{0}: RegisterLocalPlayer: localPlayerId {1}", "FusionMessenger", localPlayerId), LogLevel.Verbose);
			Logger.Log(string.Format("{0} RegisterLocalPlayer: fusionId {1}", "FusionMessenger", base.Runner.LocalPlayer.PlayerId), LogLevel.Verbose);
			this.AddPlayerIdHostRPC(localPlayerId, base.Runner.LocalPlayer.PlayerId);
		}

		[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		private unsafe void AddPlayerIdHostRPC(ulong localPlayerId, int localNetworkId)
		{
			if (!this.InvokeRpc)
			{
				NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
				if (base.Runner.Stage != SimulationStages.Resimulate)
				{
					int localAuthorityMask = base.Object.GetLocalAuthorityMask();
					if ((localAuthorityMask & 7) != 0)
					{
						if ((localAuthorityMask & 1) != 1)
						{
							int num = 8;
							num += 8;
							num += 4;
							if (!SimulationMessage.CanAllocateUserPayload(num))
							{
								NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionMessenger::AddPlayerIdHostRPC(System.UInt64,System.Int32)", num);
								return;
							}
							if (base.Runner.HasAnyActiveConnections())
							{
								SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
								byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
								*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1);
								int num2 = 8;
								*(long*)(ptr2 + num2) = (long)localPlayerId;
								num2 += 8;
								*(int*)(ptr2 + num2) = localNetworkId;
								num2 += 4;
								ptr->Offset = num2 * 8;
								base.Runner.SendRpc(ptr);
							}
							if ((localAuthorityMask & 1) == 0)
							{
								return;
							}
						}
						goto IL_12;
					}
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionMessenger::AddPlayerIdHostRPC(System.UInt64,System.Int32)", base.Object, 7);
				}
				return;
			}
			this.InvokeRpc = false;
			IL_12:
			Logger.Log("Add Player Id Host RPC: player id", LogLevel.Verbose);
			this._playerIds.Add(localPlayerId);
			Logger.Log("Add Player Id Host RPC: network id", LogLevel.Verbose);
			this._networkIds.Add(localNetworkId);
			this.PrintIDDictionary();
		}

		private bool TryGetNetworkId(ulong playerId, out int networkId)
		{
			for (int i = 0; i < this._playerIds.Count; i++)
			{
				if (playerId == this._playerIds[i])
				{
					networkId = this._networkIds[i];
					return true;
				}
			}
			networkId = 0;
			Logger.Log(string.Format("FusionMessenger: playerId {0} got invalid networkId {1}", playerId, networkId), LogLevel.Error);
			return false;
		}

		public void SendAnchorShareRequest(ulong targetPlayerId, ShareAndLocalizeParams shareAndLocalizeParams)
		{
			Logger.Log(string.Format("{0}: Sending anchor share request to player {1}. (anchorID {2})", "FusionMessenger", targetPlayerId, shareAndLocalizeParams.anchorUUID), LogLevel.Verbose);
			FusionShareAndLocalizeParams fusionData = new FusionShareAndLocalizeParams(shareAndLocalizeParams);
			this.SendMessageToPlayer(FusionMessenger.MessageEvent.AnchorShareRequest, targetPlayerId, fusionData);
		}

		public void SendAnchorShareCompleted(ulong targetPlayerId, ShareAndLocalizeParams shareAndLocalizeParams)
		{
			Logger.Log(string.Format("{0}: Sending anchor share completed to player {1}. (anchorID {2})", "FusionMessenger", targetPlayerId, shareAndLocalizeParams.anchorUUID), LogLevel.Verbose);
			FusionShareAndLocalizeParams fusionData = new FusionShareAndLocalizeParams(shareAndLocalizeParams);
			this.SendMessageToPlayer(FusionMessenger.MessageEvent.AnchorShareComplete, targetPlayerId, fusionData);
		}

		private void SendMessageToPlayer(FusionMessenger.MessageEvent eventCode, ulong playerId, FusionShareAndLocalizeParams fusionData)
		{
			Logger.Log(string.Format("Calling SendMessageToPlayer with MessageEvent: {0}, to playerId {1}", eventCode, playerId), LogLevel.Verbose);
			int num;
			if (this.TryGetNetworkId(playerId, out num))
			{
				Logger.Log(string.Format("Calling FindRPCToCallServerRPC playerId {0} maps to fusionId {1}", playerId, num), LogLevel.Verbose);
				this.FindRPCToCallServerRPC(eventCode, num, fusionData, default(RpcInfo));
				return;
			}
			Logger.Log(string.Format("Could not find fusionId for playerId {0}", playerId), LogLevel.Error);
		}

		[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		private unsafe void FindRPCToCallServerRPC(FusionMessenger.MessageEvent eventCode, int fusionId, FusionShareAndLocalizeParams fusionData, RpcInfo info = default(RpcInfo))
		{
			if (!this.InvokeRpc)
			{
				NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
				if (base.Runner.Stage != SimulationStages.Resimulate)
				{
					int localAuthorityMask = base.Object.GetLocalAuthorityMask();
					if ((localAuthorityMask & 7) != 0)
					{
						if ((localAuthorityMask & 1) != 1)
						{
							int num = 8;
							num += 4;
							num += 4;
							num += 280;
							if (!SimulationMessage.CanAllocateUserPayload(num))
							{
								NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionMessenger::FindRPCToCallServerRPC(Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionMessenger/MessageEvent,System.Int32,Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionShareAndLocalizeParams,Fusion.RpcInfo)", num);
								return;
							}
							if (base.Runner.HasAnyActiveConnections())
							{
								SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
								byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
								*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 2);
								int num2 = 8;
								*(FusionMessenger.MessageEvent*)(ptr2 + num2) = eventCode;
								num2 += 4;
								*(int*)(ptr2 + num2) = fusionId;
								num2 += 4;
								*(FusionShareAndLocalizeParams*)(ptr2 + num2) = fusionData;
								num2 += 280;
								ptr->Offset = num2 * 8;
								base.Runner.SendRpc(ptr);
							}
							if ((localAuthorityMask & 1) == 0)
							{
								return;
							}
						}
						info = RpcInfo.FromLocal(base.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
						goto IL_12;
					}
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionMessenger::FindRPCToCallServerRPC(Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionMessenger/MessageEvent,System.Int32,Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionShareAndLocalizeParams,Fusion.RpcInfo)", base.Object, 7);
				}
				return;
			}
			this.InvokeRpc = false;
			IL_12:
			Logger.Log("FindRPCToCallServerRPC called", LogLevel.Verbose);
			PlayerRef playerRef = PlayerRef.FromIndex(fusionId);
			Logger.Log("Created PlayerRef right before calling HandleMessageClientRPC", LogLevel.Verbose);
			this.HandleMessageClientRPC(playerRef, eventCode, fusionData);
		}

		[Rpc(RpcSources.All, RpcTargets.All)]
		private unsafe void HandleMessageClientRPC([RpcTarget] PlayerRef playerRef, FusionMessenger.MessageEvent eventCode, FusionShareAndLocalizeParams fusionData)
		{
			if (!this.InvokeRpc)
			{
				NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
				if (base.Runner.Stage != SimulationStages.Resimulate)
				{
					int localAuthorityMask = base.Object.GetLocalAuthorityMask();
					RpcTargetStatus rpcTargetStatus = base.Runner.GetRpcTargetStatus(playerRef);
					if (rpcTargetStatus == RpcTargetStatus.Unreachable)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(playerRef, "System.Void Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionMessenger::HandleMessageClientRPC(Fusion.PlayerRef,Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionMessenger/MessageEvent,Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionShareAndLocalizeParams)");
					}
					else if (rpcTargetStatus != RpcTargetStatus.Self)
					{
						if ((localAuthorityMask & 7) == 0)
						{
							NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionMessenger::HandleMessageClientRPC(Fusion.PlayerRef,Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionMessenger/MessageEvent,Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionShareAndLocalizeParams)", base.Object, 7);
						}
						else
						{
							int num = 8;
							num += 4;
							num += 280;
							if (!SimulationMessage.CanAllocateUserPayload(num))
							{
								NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionMessenger::HandleMessageClientRPC(Fusion.PlayerRef,Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionMessenger/MessageEvent,Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionShareAndLocalizeParams)", num);
							}
							else
							{
								SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
								byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
								*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 3);
								int num2 = 8;
								*(FusionMessenger.MessageEvent*)(ptr2 + num2) = eventCode;
								num2 += 4;
								*(FusionShareAndLocalizeParams*)(ptr2 + num2) = fusionData;
								num2 += 280;
								ptr->Offset = num2 * 8;
								NetworkRunner runner = base.Runner;
								ptr->SetTarget(playerRef);
								runner.SendRpc(ptr);
							}
						}
					}
					else if ((localAuthorityMask & 7) != 0)
					{
						goto IL_12;
					}
				}
				return;
			}
			this.InvokeRpc = false;
			IL_12:
			Logger.Log("HandleMessageClientRPC: " + eventCode.ToString(), LogLevel.Verbose);
			if (eventCode != FusionMessenger.MessageEvent.AnchorShareRequest)
			{
				if (eventCode != FusionMessenger.MessageEvent.AnchorShareComplete)
				{
					throw new ArgumentOutOfRangeException("eventCode", eventCode, null);
				}
				Action<ShareAndLocalizeParams> anchorShareRequestCompleted = this.AnchorShareRequestCompleted;
				if (anchorShareRequestCompleted == null)
				{
					return;
				}
				anchorShareRequestCompleted(fusionData.GetShareAndLocalizeParams());
				return;
			}
			else
			{
				Action<ShareAndLocalizeParams> anchorShareRequestReceived = this.AnchorShareRequestReceived;
				if (anchorShareRequestReceived == null)
				{
					return;
				}
				anchorShareRequestReceived(fusionData.GetShareAndLocalizeParams());
				return;
			}
		}

		private void PrintIDDictionary()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this._playerIds.Count; i++)
			{
				stringBuilder.Append(string.Format("[{0},{1}]", this._playerIds[i], this._networkIds[i]));
				if (i < this._playerIds.Count - 1)
				{
					stringBuilder.Append(",");
				}
			}
			Logger.Log("FusionMessenger: ID dictionary is " + stringBuilder.ToString(), LogLevel.Verbose);
		}

		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			NetworkBehaviourUtils.InitializeNetworkList<int>(this._networkIds, this.__networkIds, "_networkIds");
			NetworkBehaviourUtils.InitializeNetworkList<ulong>(this._playerIds, this.__playerIds, "_playerIds");
		}

		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			NetworkBehaviourUtils.CopyFromNetworkList<int>(this._networkIds, ref this.__networkIds);
			NetworkBehaviourUtils.CopyFromNetworkList<ulong>(this._playerIds, ref this.__playerIds);
		}

		[NetworkRpcWeavedInvoker(1, 7, 1)]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void AddPlayerIdHostRPC@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			int num = 8;
			ulong num2 = (ulong)(*(long*)(ptr + num));
			num += 8;
			ulong localPlayerId = num2;
			int num3 = *(int*)(ptr + num);
			num += 4;
			int localNetworkId = num3;
			behaviour.InvokeRpc = true;
			((FusionMessenger)behaviour).AddPlayerIdHostRPC(localPlayerId, localNetworkId);
		}

		[NetworkRpcWeavedInvoker(2, 7, 1)]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void FindRPCToCallServerRPC@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			int num = 8;
			FusionMessenger.MessageEvent messageEvent = *(FusionMessenger.MessageEvent*)(ptr + num);
			num += 4;
			FusionMessenger.MessageEvent eventCode = messageEvent;
			int num2 = *(int*)(ptr + num);
			num += 4;
			int fusionId = num2;
			FusionShareAndLocalizeParams fusionShareAndLocalizeParams = *(FusionShareAndLocalizeParams*)(ptr + num);
			num += 280;
			FusionShareAndLocalizeParams fusionData = fusionShareAndLocalizeParams;
			RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
			behaviour.InvokeRpc = true;
			((FusionMessenger)behaviour).FindRPCToCallServerRPC(eventCode, fusionId, fusionData, info);
		}

		[NetworkRpcWeavedInvoker(3, 7, 7)]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void HandleMessageClientRPC@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			int num = 8;
			PlayerRef target = message->Target;
			FusionMessenger.MessageEvent messageEvent = *(FusionMessenger.MessageEvent*)(ptr + num);
			num += 4;
			FusionMessenger.MessageEvent eventCode = messageEvent;
			FusionShareAndLocalizeParams fusionShareAndLocalizeParams = *(FusionShareAndLocalizeParams*)(ptr + num);
			num += 280;
			FusionShareAndLocalizeParams fusionData = fusionShareAndLocalizeParams;
			behaviour.InvokeRpc = true;
			((FusionMessenger)behaviour).HandleMessageClientRPC(target, eventCode, fusionData);
		}

		[WeaverGenerated]
		[DefaultForProperty("_networkIds", 0, 33)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private int[] __networkIds;

		[WeaverGenerated]
		[DefaultForProperty("_playerIds", 33, 43)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private ulong[] __playerIds;

		private enum MessageEvent
		{
			AnchorShareRequest,
			AnchorShareComplete
		}
	}
}
