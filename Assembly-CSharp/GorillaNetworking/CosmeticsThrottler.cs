using System;
using UnityEngine;

namespace GorillaNetworking
{
	public class CosmeticsThrottler : MonoBehaviour, IGorillaSliceableSimple
	{
		private void Awake()
		{
			this._cosmeticSlots = 16;
			VRRig[] allRigs = VRRigCache.Instance.GetAllRigs();
			this._rigHelpers = new GorillaRigHelper[allRigs.Length];
			for (int i = 0; i < allRigs.Length; i++)
			{
				this._rigHelpers[i] = new GorillaRigHelper
				{
					rig = allRigs[i],
					state = CosmeticsThrottler.RigDrawState.Startup,
					sqrDistance = 9999f,
					prevSqrDistance = 9999f
				};
			}
			RoomSystem.JoinedRoomEvent += new Action(this.UpdatePlayerCount);
			RoomSystem.LeftRoomEvent += new Action(this.UpdatePlayerCount);
		}

		private void UpdatePlayerCount()
		{
			int num = NetworkSystem.Instance.AllNetPlayers.Length;
			if (num < this.ThrottlePlayerCountThreshold && this.lastPlayerCount >= this.ThrottlePlayerCountThreshold)
			{
				this.EnableAllRenderers();
			}
			this.lastPlayerCount = num;
		}

		private void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this);
		}

		private void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this);
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(base.transform.position, this.DrawAllDistance);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(base.transform.position, this.MaxDrawDistance);
		}

		public void SliceUpdate()
		{
			if (this.lastPlayerCount < this.ThrottlePlayerCountThreshold)
			{
				return;
			}
			if (this.mainCamera == null)
			{
				this.mainCamera = Camera.main;
				return;
			}
			Vector3 position = base.transform.position;
			for (int i = 0; i < this._rigHelpers.Length; i++)
			{
				this._rigHelpers[i].prevSqrDistance = this._rigHelpers[i].sqrDistance;
				if (!this._rigHelpers[i].rig.isActiveAndEnabled || this._rigHelpers[i].rig.isLocal)
				{
					this._rigHelpers[i].sqrDistance = 9999f;
				}
				else
				{
					Vector3 position2 = this._rigHelpers[i].rig.transform.position;
					if (this.mainCamera.WorldToScreenPoint(position2).z <= 0f)
					{
						this._rigHelpers[i].sqrDistance = 9999f;
					}
					else
					{
						float sqrMagnitude = (position2 - position).sqrMagnitude;
						this._rigHelpers[i].sqrDistance = sqrMagnitude;
					}
				}
			}
			Array.Sort<GorillaRigHelper>(this._rigHelpers);
			float num = this.DrawAllDistance * this.DrawAllDistance;
			float num2 = this.MaxDrawDistance * this.MaxDrawDistance;
			for (int j = 0; j < this._rigHelpers.Length; j++)
			{
				if (this._rigHelpers[j].rig.cosmeticsObjectRegistry.isInitialized)
				{
					if (this._rigHelpers[j].sqrDistance >= 9999f)
					{
						this._rigHelpers[j] = this.UpdateRigState(this._rigHelpers[j], CosmeticsThrottler.RigDrawState.Min);
					}
					else
					{
						if (this.DrawOnPlayerCount)
						{
							if (j < this.DrawAllCount)
							{
								this._rigHelpers[j] = this.UpdateRigState(this._rigHelpers[j], CosmeticsThrottler.RigDrawState.All);
								goto IL_2B4;
							}
							if (j >= this.DrawMaxCount)
							{
								this._rigHelpers[j] = this.UpdateRigState(this._rigHelpers[j], CosmeticsThrottler.RigDrawState.Min);
								goto IL_2B4;
							}
						}
						if (this._rigHelpers[j].sqrDistance <= num)
						{
							this._rigHelpers[j] = this.UpdateRigState(this._rigHelpers[j], CosmeticsThrottler.RigDrawState.All);
						}
						else if (this._rigHelpers[j].sqrDistance <= num2)
						{
							this._rigHelpers[j] = this.UpdateRigState(this._rigHelpers[j], CosmeticsThrottler.RigDrawState.Partial);
						}
						else
						{
							this._rigHelpers[j] = this.UpdateRigState(this._rigHelpers[j], CosmeticsThrottler.RigDrawState.Min);
						}
					}
				}
				IL_2B4:;
			}
		}

		private GorillaRigHelper UpdateRigState(GorillaRigHelper helper, CosmeticsThrottler.RigDrawState newState)
		{
			CosmeticsThrottler.RigDrawState state = helper.state;
			if (newState == state)
			{
				return helper;
			}
			switch (newState)
			{
			case CosmeticsThrottler.RigDrawState.All:
				if (state != CosmeticsThrottler.RigDrawState.All)
				{
					this.ToggleRenderersOnRig(helper.rig, true);
					helper.rig.ToggleMatParticles(true);
				}
				break;
			case CosmeticsThrottler.RigDrawState.Partial:
				if (state <= CosmeticsThrottler.RigDrawState.All)
				{
					this.ToggleRenderersOnRigForSlots(helper.rig, false, true);
					helper.rig.ToggleMatParticles(false);
				}
				else if (state == CosmeticsThrottler.RigDrawState.Min)
				{
					this.ToggleRenderersOnRigForSlots(helper.rig, true, false);
				}
				break;
			case CosmeticsThrottler.RigDrawState.Min:
				if (state != CosmeticsThrottler.RigDrawState.Min)
				{
					this.ToggleRenderersOnRig(helper.rig, false);
					helper.rig.ToggleMatParticles(false);
				}
				break;
			}
			helper.state = newState;
			return helper;
		}

		private void ToggleRenderersOnRig(VRRig rig, bool toggle)
		{
			CosmeticsController.CosmeticSet cosmeticSet = rig.cosmeticSet;
			int num = cosmeticSet.items.Length;
			for (int i = 0; i < num; i++)
			{
				CosmeticItemInstance cosmeticItemInstance = rig.cosmeticsObjectRegistry.Cosmetic(cosmeticSet.items[i].displayName);
				if (cosmeticItemInstance != null)
				{
					cosmeticItemInstance.ToggleRenderers(toggle);
					cosmeticItemInstance.ToggleParticles(toggle);
				}
			}
		}

		private void ToggleRenderersOnRigForSlots(VRRig rig, bool toggle, bool includesSlots = true)
		{
			CosmeticsController.CosmeticSet cosmeticSet = rig.cosmeticSet;
			int num = cosmeticSet.items.Length;
			for (int i = 0; i < num; i++)
			{
				CosmeticItemInstance cosmeticItemInstance = rig.cosmeticsObjectRegistry.Cosmetic(cosmeticSet.items[i].displayName);
				if (cosmeticItemInstance != null)
				{
					cosmeticItemInstance.ToggleParticles(toggle);
					if (this.ContainsSlot(cosmeticItemInstance.ActiveSlot) == includesSlots)
					{
						cosmeticItemInstance.ToggleRenderers(toggle);
					}
				}
			}
		}

		private bool ContainsSlot(CosmeticsController.CosmeticSlots slot)
		{
			for (int i = 0; i < this.ToggleSlots.Length; i++)
			{
				if (this.ToggleSlots[i] == slot)
				{
					return true;
				}
			}
			return false;
		}

		public void EnableAllRenderers()
		{
			for (int i = 0; i < this._rigHelpers.Length; i++)
			{
				this.ToggleRenderersOnRig(this._rigHelpers[i].rig, true);
			}
		}

		public float DrawAllDistance = 5f;

		public float MaxDrawDistance = 10f;

		public bool DrawOnPlayerCount = true;

		public int DrawAllCount = 6;

		public int DrawMaxCount = 14;

		public int ThrottlePlayerCountThreshold = 11;

		private int lastPlayerCount;

		public CosmeticsController.CosmeticSlots[] ToggleSlots;

		[SerializeField]
		private GorillaRigHelper[] _rigHelpers;

		private int _cosmeticSlots;

		private float _update;

		private Camera mainCamera;

		public enum RigDrawState
		{
			All,
			Partial,
			Min,
			Startup = -1
		}
	}
}
