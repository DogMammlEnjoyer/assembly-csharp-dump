using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
			CosmeticSwapper.<TriggerSwap>d__22 <TriggerSwap>d__;
			<TriggerSwap>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<TriggerSwap>d__.<>4__this = this;
			<TriggerSwap>d__.rig = rig;
			<TriggerSwap>d__.<>1__state = -1;
			<TriggerSwap>d__.<>t__builder.Start<CosmeticSwapper.<TriggerSwap>d__22>(ref <TriggerSwap>d__);
		}

		private Awaitable<CosmeticSwapper.CosmeticState?> SwapInCosmeticWithReturn(string nameOrId, VRRig rig)
		{
			CosmeticSwapper.<SwapInCosmeticWithReturn>d__23 <SwapInCosmeticWithReturn>d__;
			<SwapInCosmeticWithReturn>d__.<>t__builder = Awaitable.AwaitableAsyncMethodBuilder<CosmeticSwapper.CosmeticState?>.Create();
			<SwapInCosmeticWithReturn>d__.<>4__this = this;
			<SwapInCosmeticWithReturn>d__.nameOrId = nameOrId;
			<SwapInCosmeticWithReturn>d__.<>1__state = -1;
			<SwapInCosmeticWithReturn>d__.<>t__builder.Start<CosmeticSwapper.<SwapInCosmeticWithReturn>d__23>(ref <SwapInCosmeticWithReturn>d__);
			return <SwapInCosmeticWithReturn>d__.<>t__builder.Task;
		}

		private Awaitable RestorePreviousCosmetic(CosmeticSwapper.CosmeticState state)
		{
			CosmeticSwapper.<RestorePreviousCosmetic>d__24 <RestorePreviousCosmetic>d__;
			<RestorePreviousCosmetic>d__.<>t__builder = Awaitable.AwaitableAsyncMethodBuilder.Create();
			<RestorePreviousCosmetic>d__.<>4__this = this;
			<RestorePreviousCosmetic>d__.state = state;
			<RestorePreviousCosmetic>d__.<>1__state = -1;
			<RestorePreviousCosmetic>d__.<>t__builder.Start<CosmeticSwapper.<RestorePreviousCosmetic>d__24>(ref <RestorePreviousCosmetic>d__);
			return <RestorePreviousCosmetic>d__.<>t__builder.Task;
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
			CosmeticSwapper.<Tick>d__31 <Tick>d__;
			<Tick>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Tick>d__.<>4__this = this;
			<Tick>d__.<>1__state = -1;
			<Tick>d__.<>t__builder.Start<CosmeticSwapper.<Tick>d__31>(ref <Tick>d__);
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
