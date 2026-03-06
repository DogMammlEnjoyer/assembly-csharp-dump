using System;
using System.Collections.Generic;
using Fusion;
using Fusion.CodeGen;
using UnityEngine.Scripting;

namespace Meta.XR.MultiplayerBlocks.Colocation.Fusion
{
	[NetworkBehaviourWeaved(797)]
	internal class FusionNetworkData : NetworkBehaviour, INetworkData
	{
		[Networked]
		[NetworkedWeaved(0, 1)]
		private unsafe uint ColocationGroupCount
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing FusionNetworkData.ColocationGroupCount. Networked properties can only be accessed when Spawned() has been called.");
				}
				return (uint)this.Ptr[0];
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing FusionNetworkData.ColocationGroupCount. Networked properties can only be accessed when Spawned() has been called.");
				}
				this.Ptr[0] = (int)value;
			}
		}

		[Networked]
		[Capacity(10)]
		[NetworkedWeaved(1, 723)]
		[NetworkedWeavedLinkedList(10, 70, typeof(ReaderWriter@Meta_XR_MultiplayerBlocks_Colocation_Fusion_FusionAnchor))]
		private unsafe NetworkLinkedList<FusionAnchor> AnchorList
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing FusionNetworkData.AnchorList. Networked properties can only be accessed when Spawned() has been called.");
				}
				return new NetworkLinkedList<FusionAnchor>((byte*)(this.Ptr + 1), 10, ReaderWriter@Meta_XR_MultiplayerBlocks_Colocation_Fusion_FusionAnchor.GetInstance());
			}
		}

		[Networked]
		[Capacity(10)]
		[NetworkedWeaved(724, 73)]
		[NetworkedWeavedLinkedList(10, 5, typeof(ReaderWriter@Meta_XR_MultiplayerBlocks_Colocation_Fusion_FusionPlayer))]
		private unsafe NetworkLinkedList<FusionPlayer> PlayerList
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing FusionNetworkData.PlayerList. Networked properties can only be accessed when Spawned() has been called.");
				}
				return new NetworkLinkedList<FusionPlayer>((byte*)(this.Ptr + 724), 10, ReaderWriter@Meta_XR_MultiplayerBlocks_Colocation_Fusion_FusionPlayer.GetInstance());
			}
		}

		public void AddPlayer(Player player)
		{
			this.AddFusionPlayer(new FusionPlayer(player));
		}

		public void RemovePlayer(Player player)
		{
			this.RemoveFusionPlayer(new FusionPlayer(player));
		}

		public Player? GetPlayerWithPlayerId(ulong playerId)
		{
			foreach (FusionPlayer fusionPlayer in this.PlayerList)
			{
				if (fusionPlayer.GetPlayer().playerId == playerId)
				{
					return new Player?(fusionPlayer.GetPlayer());
				}
			}
			return null;
		}

		public Player? GetPlayerWithOculusId(ulong oculusId)
		{
			foreach (FusionPlayer fusionPlayer in this.PlayerList)
			{
				if (fusionPlayer.GetPlayer().oculusId == oculusId)
				{
					return new Player?(fusionPlayer.GetPlayer());
				}
			}
			return null;
		}

		public List<Player> GetAllPlayers()
		{
			List<Player> list = new List<Player>();
			foreach (FusionPlayer fusionPlayer in this.PlayerList)
			{
				list.Add(fusionPlayer.GetPlayer());
			}
			return list;
		}

		public void AddAnchor(Anchor anchor)
		{
			this.AnchorList.Add(new FusionAnchor(anchor));
		}

		public void RemoveAnchor(Anchor anchor)
		{
			this.AnchorList.Remove(new FusionAnchor(anchor));
		}

		public Anchor? GetAnchor(ulong ownerOculusId)
		{
			foreach (FusionAnchor fusionAnchor in this.AnchorList)
			{
				if (fusionAnchor.GetAnchor().ownerOculusId == ownerOculusId)
				{
					return new Anchor?(fusionAnchor.GetAnchor());
				}
			}
			return null;
		}

		public List<Anchor> GetAllAnchors()
		{
			List<Anchor> list = new List<Anchor>();
			foreach (FusionAnchor fusionAnchor in this.AnchorList)
			{
				list.Add(fusionAnchor.GetAnchor());
			}
			return list;
		}

		public uint GetColocationGroupCount()
		{
			return this.ColocationGroupCount;
		}

		public void IncrementColocationGroupCount()
		{
			if (base.HasStateAuthority)
			{
				uint colocationGroupCount = this.ColocationGroupCount;
				this.ColocationGroupCount = colocationGroupCount + 1U;
				return;
			}
			this.IncrementColocationGroupCountRpc();
		}

		private void AddFusionPlayer(FusionPlayer player)
		{
			if (base.HasStateAuthority)
			{
				this.PlayerList.Add(player);
				return;
			}
			this.AddPlayerRpc(player);
		}

		private void RemoveFusionPlayer(FusionPlayer player)
		{
			if (base.HasStateAuthority)
			{
				this.PlayerList.Remove(player);
				return;
			}
			this.RemovePlayerRpc(player);
		}

		private void AddFusionAnchor(FusionAnchor anchor)
		{
			if (base.HasStateAuthority)
			{
				this.AnchorList.Add(anchor);
				return;
			}
			this.AddAnchorRpc(anchor);
		}

		private void RemoveFusionAnchor(FusionAnchor anchor)
		{
			if (base.HasStateAuthority)
			{
				this.AnchorList.Remove(anchor);
				return;
			}
			this.RemoveAnchorRpc(anchor);
		}

		[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		private unsafe void AddPlayerRpc(FusionPlayer player)
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
							num += 20;
							if (!SimulationMessage.CanAllocateUserPayload(num))
							{
								NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionNetworkData::AddPlayerRpc(Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionPlayer)", num);
								return;
							}
							if (base.Runner.HasAnyActiveConnections())
							{
								SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
								byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
								*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1);
								int num2 = 8;
								*(FusionPlayer*)(ptr2 + num2) = player;
								num2 += 20;
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
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionNetworkData::AddPlayerRpc(Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionPlayer)", base.Object, 7);
				}
				return;
			}
			this.InvokeRpc = false;
			IL_12:
			this.AddFusionPlayer(player);
		}

		[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		private unsafe void RemovePlayerRpc(FusionPlayer player)
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
							num += 20;
							if (!SimulationMessage.CanAllocateUserPayload(num))
							{
								NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionNetworkData::RemovePlayerRpc(Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionPlayer)", num);
								return;
							}
							if (base.Runner.HasAnyActiveConnections())
							{
								SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
								byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
								*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 2);
								int num2 = 8;
								*(FusionPlayer*)(ptr2 + num2) = player;
								num2 += 20;
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
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionNetworkData::RemovePlayerRpc(Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionPlayer)", base.Object, 7);
				}
				return;
			}
			this.InvokeRpc = false;
			IL_12:
			this.RemoveFusionPlayer(player);
		}

		[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		private unsafe void AddAnchorRpc(FusionAnchor anchor)
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
							num += 280;
							if (!SimulationMessage.CanAllocateUserPayload(num))
							{
								NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionNetworkData::AddAnchorRpc(Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionAnchor)", num);
								return;
							}
							if (base.Runner.HasAnyActiveConnections())
							{
								SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
								byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
								*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 3);
								int num2 = 8;
								*(FusionAnchor*)(ptr2 + num2) = anchor;
								num2 += 280;
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
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionNetworkData::AddAnchorRpc(Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionAnchor)", base.Object, 7);
				}
				return;
			}
			this.InvokeRpc = false;
			IL_12:
			this.AddFusionAnchor(anchor);
		}

		[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		private unsafe void RemoveAnchorRpc(FusionAnchor anchor)
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
							num += 280;
							if (!SimulationMessage.CanAllocateUserPayload(num))
							{
								NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionNetworkData::RemoveAnchorRpc(Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionAnchor)", num);
								return;
							}
							if (base.Runner.HasAnyActiveConnections())
							{
								SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
								byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
								*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 4);
								int num2 = 8;
								*(FusionAnchor*)(ptr2 + num2) = anchor;
								num2 += 280;
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
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionNetworkData::RemoveAnchorRpc(Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionAnchor)", base.Object, 7);
				}
				return;
			}
			this.InvokeRpc = false;
			IL_12:
			this.RemoveFusionAnchor(anchor);
		}

		[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		private unsafe void IncrementColocationGroupCountRpc()
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
							if (!SimulationMessage.CanAllocateUserPayload(num))
							{
								NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionNetworkData::IncrementColocationGroupCountRpc()", num);
								return;
							}
							if (base.Runner.HasAnyActiveConnections())
							{
								SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
								byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
								*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 5);
								int num2 = 8;
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
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void Meta.XR.MultiplayerBlocks.Colocation.Fusion.FusionNetworkData::IncrementColocationGroupCountRpc()", base.Object, 7);
				}
				return;
			}
			this.InvokeRpc = false;
			IL_12:
			this.IncrementColocationGroupCount();
		}

		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			this.ColocationGroupCount = this._ColocationGroupCount;
			NetworkBehaviourUtils.InitializeNetworkList<FusionAnchor>(this.AnchorList, this._AnchorList, "AnchorList");
			NetworkBehaviourUtils.InitializeNetworkList<FusionPlayer>(this.PlayerList, this._PlayerList, "PlayerList");
		}

		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			this._ColocationGroupCount = this.ColocationGroupCount;
			NetworkBehaviourUtils.CopyFromNetworkList<FusionAnchor>(this.AnchorList, ref this._AnchorList);
			NetworkBehaviourUtils.CopyFromNetworkList<FusionPlayer>(this.PlayerList, ref this._PlayerList);
		}

		[NetworkRpcWeavedInvoker(1, 7, 1)]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void AddPlayerRpc@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			int num = 8;
			FusionPlayer fusionPlayer = *(FusionPlayer*)(ptr + num);
			num += 20;
			FusionPlayer player = fusionPlayer;
			behaviour.InvokeRpc = true;
			((FusionNetworkData)behaviour).AddPlayerRpc(player);
		}

		[NetworkRpcWeavedInvoker(2, 7, 1)]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RemovePlayerRpc@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			int num = 8;
			FusionPlayer fusionPlayer = *(FusionPlayer*)(ptr + num);
			num += 20;
			FusionPlayer player = fusionPlayer;
			behaviour.InvokeRpc = true;
			((FusionNetworkData)behaviour).RemovePlayerRpc(player);
		}

		[NetworkRpcWeavedInvoker(3, 7, 1)]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void AddAnchorRpc@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			int num = 8;
			FusionAnchor fusionAnchor = *(FusionAnchor*)(ptr + num);
			num += 280;
			FusionAnchor anchor = fusionAnchor;
			behaviour.InvokeRpc = true;
			((FusionNetworkData)behaviour).AddAnchorRpc(anchor);
		}

		[NetworkRpcWeavedInvoker(4, 7, 1)]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RemoveAnchorRpc@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			int num = 8;
			FusionAnchor fusionAnchor = *(FusionAnchor*)(ptr + num);
			num += 280;
			FusionAnchor anchor = fusionAnchor;
			behaviour.InvokeRpc = true;
			((FusionNetworkData)behaviour).RemoveAnchorRpc(anchor);
		}

		[NetworkRpcWeavedInvoker(5, 7, 1)]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void IncrementColocationGroupCountRpc@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			behaviour.InvokeRpc = true;
			((FusionNetworkData)behaviour).IncrementColocationGroupCountRpc();
		}

		[WeaverGenerated]
		[DefaultForProperty("ColocationGroupCount", 0, 1)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private uint _ColocationGroupCount;

		[WeaverGenerated]
		[DefaultForProperty("AnchorList", 1, 723)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private FusionAnchor[] _AnchorList;

		[WeaverGenerated]
		[DefaultForProperty("PlayerList", 724, 73)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private FusionPlayer[] _PlayerList;
	}
}
