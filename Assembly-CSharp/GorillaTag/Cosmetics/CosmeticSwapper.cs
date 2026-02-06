using System;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	public class CosmeticSwapper : MonoBehaviour, ITickSystemTick
	{
		private int CosmeticStepIndex
		{
			get
			{
				return this.newSwappedCosmetics.Count;
			}
		}

		private void Awake()
		{
			this.controller = CosmeticsController.instance;
		}

		private void OnEnable()
		{
			TickSystem<object>.AddTickCallback(this);
			PlayerCosmeticsSystem.UnlockTemporaryCosmeticsGlobal(this.cosmeticIDs);
		}

		private void OnDisable()
		{
			PlayerCosmeticsSystem.LockTemporaryCosmeticsGlobal(this.cosmeticIDs);
			TickSystem<object>.RemoveTickCallback(this);
		}

		public void SwapInCosmetic(VRRig vrRig)
		{
			this.TriggerSwap(vrRig);
		}

		private CosmeticSwapper.SwapMode GetCurrentMode()
		{
			return this.swapMode;
		}

		private bool ShouldHoldFinalStep()
		{
			return this.holdFinalStep;
		}

		public int GetCurrentStepIndex(VRRig rig)
		{
			if (rig == null)
			{
				return 0;
			}
			return this.CosmeticStepIndex;
		}

		public int GetNumberOfSteps()
		{
			return this.cosmeticIDs.Count;
		}

		private void TriggerSwap(VRRig rig)
		{
			if (GorillaGameManager.instance != null && this.gameModeExclusion.Contains(GorillaGameManager.instance.GameType()))
			{
				return;
			}
			if (rig == null || this.controller == null || this.cosmeticIDs.Count == 0)
			{
				return;
			}
			if (rig != GorillaTagger.Instance.offlineVRRig)
			{
				return;
			}
			if (this.swapMode == CosmeticSwapper.SwapMode.AllAtOnce)
			{
				foreach (string nameOrId in this.cosmeticIDs)
				{
					CosmeticSwapper.CosmeticState? cosmeticState = this.SwapInCosmeticWithReturn(nameOrId, rig);
					if (cosmeticState != null)
					{
						this.AddNewSwappedCosmetic(cosmeticState.Value);
					}
				}
				return;
			}
			int cosmeticStepIndex = this.CosmeticStepIndex;
			if (cosmeticStepIndex < 0 || cosmeticStepIndex >= this.cosmeticIDs.Count)
			{
				return;
			}
			string nameOrId2 = this.cosmeticIDs[cosmeticStepIndex];
			CosmeticSwapper.CosmeticState? cosmeticState2 = this.SwapInCosmeticWithReturn(nameOrId2, rig);
			if (cosmeticState2 != null)
			{
				this.AddNewSwappedCosmetic(cosmeticState2.Value);
				if (cosmeticStepIndex == this.cosmeticIDs.Count - 1)
				{
					if (this.holdFinalStep)
					{
						this.MarkFinalCosmeticStep();
					}
					if (this.OnSwappingSequenceCompleted != null)
					{
						this.OnSwappingSequenceCompleted.Invoke(rig);
						return;
					}
				}
				else
				{
					this.UnmarkFinalCosmeticStep();
				}
			}
		}

		private CosmeticSwapper.CosmeticState? SwapInCosmeticWithReturn(string nameOrId, VRRig rig)
		{
			if (this.controller == null)
			{
				return null;
			}
			CosmeticsController.CosmeticItem cosmeticItem = this.FindItem(nameOrId);
			if (cosmeticItem.isNullItem)
			{
				Debug.LogWarning("Cosmetic not found: " + nameOrId);
				return null;
			}
			bool isLeftHand;
			CosmeticsController.CosmeticSlots cosmeticSlot = this.GetCosmeticSlot(cosmeticItem, out isLeftHand);
			if (cosmeticSlot == CosmeticsController.CosmeticSlots.Count)
			{
				Debug.LogWarning("Could not determine slot for: " + cosmeticItem.displayName);
				return null;
			}
			CosmeticsController.CosmeticItem cosmeticItem2 = this.controller.currentWornSet.items[(int)cosmeticSlot];
			if (!cosmeticItem2.isNullItem && cosmeticItem2.itemName == cosmeticItem.itemName)
			{
				return null;
			}
			this.controller.ApplyCosmeticItemToSet(this.controller.tempUnlockedSet, cosmeticItem, isLeftHand, false);
			this.controller.UpdateWornCosmetics(true);
			return new CosmeticSwapper.CosmeticState?(new CosmeticSwapper.CosmeticState
			{
				cosmeticId = nameOrId,
				replacedItem = cosmeticItem2,
				slot = cosmeticSlot,
				isLeftHand = isLeftHand
			});
		}

		private void RestorePreviousCosmetic(CosmeticSwapper.CosmeticState state)
		{
			if (this.controller == null)
			{
				return;
			}
			CosmeticsController.CosmeticItem cosmeticItem = this.FindItem(state.cosmeticId);
			if (cosmeticItem.isNullItem)
			{
				return;
			}
			this.controller.RemoveCosmeticItemFromSet(this.controller.tempUnlockedSet, cosmeticItem.displayName, false);
			if (!state.replacedItem.isNullItem)
			{
				this.controller.ApplyCosmeticItemToSet(this.controller.tempUnlockedSet, state.replacedItem, state.isLeftHand, false);
			}
			this.controller.UpdateWornCosmetics(true);
		}

		private CosmeticsController.CosmeticItem FindItem(string nameOrId)
		{
			CosmeticsController.CosmeticItem result;
			if (this.controller.allCosmeticsDict.TryGetValue(nameOrId, out result))
			{
				return result;
			}
			string itemID;
			if (this.controller.allCosmeticsItemIDsfromDisplayNamesDict.TryGetValue(nameOrId, out itemID))
			{
				return this.controller.GetItemFromDict(itemID);
			}
			return this.controller.nullItem;
		}

		private CosmeticsController.CosmeticSlots GetCosmeticSlot(CosmeticsController.CosmeticItem item, out bool isLeftHand)
		{
			isLeftHand = false;
			if (!item.isHoldable)
			{
				return CosmeticsController.CategoryToNonTransferrableSlot(item.itemCategory);
			}
			CosmeticsController.CosmeticSet currentWornSet = this.controller.currentWornSet;
			CosmeticsController.CosmeticItem cosmeticItem = currentWornSet.items[7];
			CosmeticsController.CosmeticItem cosmeticItem2 = currentWornSet.items[8];
			if (cosmeticItem.isNullItem || (!cosmeticItem2.isNullItem && item.itemName == cosmeticItem.itemName))
			{
				isLeftHand = true;
			}
			if (!isLeftHand)
			{
				return CosmeticsController.CosmeticSlots.HandRight;
			}
			return CosmeticsController.CosmeticSlots.HandLeft;
		}

		public bool TickRunning { get; set; }

		public void Tick()
		{
			if (this.newSwappedCosmetics.Count > 0)
			{
				if (this.GetCurrentMode() == CosmeticSwapper.SwapMode.StepByStep)
				{
					if (this.isAtFinalCosmeticStep && this.ShouldHoldFinalStep())
					{
						if (Time.time - this.lastCosmeticSwapTime <= this.stepTimeout)
						{
							return;
						}
						this.isAtFinalCosmeticStep = false;
					}
					if (Time.time - this.lastCosmeticSwapTime > this.stepTimeout)
					{
						while (this.newSwappedCosmetics.Count > 0)
						{
							CosmeticSwapper.CosmeticState state = this.newSwappedCosmetics.Pop();
							this.RestorePreviousCosmetic(state);
						}
						this.isAtFinalCosmeticStep = false;
						this.lastCosmeticSwapTime = float.PositiveInfinity;
						return;
					}
				}
				else if (this.GetCurrentMode() == CosmeticSwapper.SwapMode.AllAtOnce && Time.time - this.lastCosmeticSwapTime > this.stepTimeout)
				{
					while (this.newSwappedCosmetics.Count > 0)
					{
						CosmeticSwapper.CosmeticState state2 = this.newSwappedCosmetics.Pop();
						this.RestorePreviousCosmetic(state2);
					}
					this.lastCosmeticSwapTime = float.PositiveInfinity;
					this.isAtFinalCosmeticStep = false;
				}
			}
		}

		private void AddNewSwappedCosmetic(CosmeticSwapper.CosmeticState state)
		{
			this.newSwappedCosmetics.Push(state);
			this.lastCosmeticSwapTime = Time.time;
		}

		private void MarkFinalCosmeticStep()
		{
			this.isAtFinalCosmeticStep = true;
			this.lastCosmeticSwapTime = Time.time;
		}

		private void UnmarkFinalCosmeticStep()
		{
			this.isAtFinalCosmeticStep = false;
		}

		[SerializeField]
		private List<string> cosmeticIDs = new List<string>();

		[SerializeField]
		private CosmeticSwapper.SwapMode swapMode = CosmeticSwapper.SwapMode.StepByStep;

		[SerializeField]
		private float stepTimeout = 10f;

		[Tooltip("Hold final step as long as the swapper is being called within the timeframe")]
		[SerializeField]
		private bool holdFinalStep = true;

		[SerializeField]
		private UnityEvent<VRRig> OnSwappingSequenceCompleted;

		[SerializeField]
		private List<GameModeType> gameModeExclusion = new List<GameModeType>();

		private CosmeticsController controller;

		private Stack<CosmeticSwapper.CosmeticState> newSwappedCosmetics = new Stack<CosmeticSwapper.CosmeticState>();

		private float lastCosmeticSwapTime = float.PositiveInfinity;

		private bool isAtFinalCosmeticStep;

		private enum SwapMode
		{
			AllAtOnce,
			StepByStep
		}

		private struct CosmeticState
		{
			public string cosmeticId;

			public CosmeticsController.CosmeticItem replacedItem;

			public CosmeticsController.CosmeticSlots slot;

			public bool isLeftHand;
		}
	}
}
