using System;
using DefaultNamespace;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag.CosmeticSystem;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Events;

public class EvolvingCosmetic : MonoBehaviour, ICosmeticStateSync
{
	public int StateValue
	{
		get
		{
			return this.SelectedObjectIndex;
		}
	}

	public int SelectedObjectIndex { get; private set; } = -1;

	public string PlayfabId
	{
		get
		{
			return base.gameObject.name;
		}
	}

	private void Awake()
	{
		int num;
		if (EvolvingCosmeticSaveData.Instance.SelectedIndices.TryGetValue(this.PlayfabId, out num) && this.IsIndexAvailable(num))
		{
			this.SelectedObjectIndex = num;
			this.ActivateSelectedIndex();
		}
	}

	private void OnEnable()
	{
		VRRig vrrig = base.GetComponentInParent<VRRig>();
		if (vrrig == null)
		{
			if (base.GetComponentInParent<GTPlayer>() == null)
			{
				return;
			}
			vrrig = VRRig.LocalRig;
		}
		if (vrrig == null)
		{
			return;
		}
		VRRigReliableState reliableState = vrrig.reliableState;
		if (reliableState != null)
		{
			reliableState.RegisterCosmeticStateSyncTarget(this.GetStateSyncSlot(), this);
		}
		SubscriptionManager.SubscriptionDetails subscriptionDetails = SubscriptionManager.GetSubscriptionDetails(vrrig);
		this._daysAccrued = new int?(0);
		switch (this.ageRule)
		{
		case EvolvingCosmetic.SubscriptionAgeRule.ItemAge:
			this._daysAccrued = new int?(vrrig.CheckCosmeticAge(base.name));
			break;
		case EvolvingCosmetic.SubscriptionAgeRule.MinItemSubscriptionAge:
			this._daysAccrued = new int?(Mathf.Min(subscriptionDetails.daysAccrued, vrrig.CheckCosmeticAge(base.name)));
			break;
		case EvolvingCosmetic.SubscriptionAgeRule.SubscriptionAge:
			this._daysAccrued = new int?(subscriptionDetails.daysAccrued);
			break;
		case EvolvingCosmetic.SubscriptionAgeRule.MinItemSubscriptionAgeActive:
			if (subscriptionDetails.active)
			{
				this._daysAccrued = new int?(Mathf.Min(subscriptionDetails.daysAccrued, vrrig.CheckCosmeticAge(base.name)));
			}
			break;
		case EvolvingCosmetic.SubscriptionAgeRule.SubscriptionAgeActive:
			if (subscriptionDetails.active)
			{
				this._daysAccrued = new int?(subscriptionDetails.daysAccrued);
			}
			break;
		}
		for (int i = 0; i < this.ageAwareGameObjects.Length; i++)
		{
			int? daysAccrued = this._daysAccrued;
			int minActiveDays = this.ageAwareGameObjects[i].minActiveDays;
			if (daysAccrued.GetValueOrDefault() < minActiveDays & daysAccrued != null)
			{
				break;
			}
			this.SelectedObjectIndex = i;
		}
		this.ActivateSelectedIndex();
		if (this._daysAccrued != null)
		{
			UnityEvent<int> dispatchDaysOnEnable = this.DispatchDaysOnEnable;
			if (dispatchDaysOnEnable != null)
			{
				dispatchDaysOnEnable.Invoke(Mathf.Min(this._daysAccrued.Value, this.capDays));
			}
			if (this.maxDays > 0)
			{
				UnityEvent<float> dispatchDaysOnEnableNormalized = this.DispatchDaysOnEnableNormalized;
				if (dispatchDaysOnEnableNormalized == null)
				{
					return;
				}
				dispatchDaysOnEnableNormalized.Invoke(Mathf.Min((float)this._daysAccrued.Value / (float)this.maxDays, 1f) * (float)this.multiplier);
			}
			return;
		}
		throw new NullReferenceException("_daysAccrued was not set by end of OnEnable.");
	}

	private void OnDisable()
	{
		VRRig componentInParent = base.GetComponentInParent<VRRig>();
		VRRigReliableState vrrigReliableState = (componentInParent != null) ? componentInParent.reliableState : null;
		if (vrrigReliableState != null)
		{
			vrrigReliableState.UnRegisterCosmeticStateSyncTarget(this.GetStateSyncSlot(), this);
		}
	}

	private void ActivateSelectedIndex()
	{
		if (!this.IsSelectedIndexAvailable())
		{
			return;
		}
		for (int i = 0; i < this.ageAwareGameObjects.Length; i++)
		{
			this.ageAwareGameObjects[i].gameObject.SetActive(i == this.SelectedObjectIndex);
		}
	}

	private bool IsSelectedIndexAvailable()
	{
		return this.IsIndexAvailable(this.SelectedObjectIndex);
	}

	private bool IsIndexAvailable(int index)
	{
		if (index < 0 || index >= this.ageAwareGameObjects.Length)
		{
			return false;
		}
		EvolvingCosmetic.AgeAwareGameObject ageAwareGameObject = this.ageAwareGameObjects[index];
		return this._daysAccrued.Value >= ageAwareGameObject.minActiveDays;
	}

	public void GoBack()
	{
		if (!this.CanGoBack())
		{
			return;
		}
		int selectedObjectIndex = this.SelectedObjectIndex - 1;
		this.SelectedObjectIndex = selectedObjectIndex;
		this.ActivateSelectedIndex();
	}

	public void GoForward()
	{
		if (!this.CanGoForward())
		{
			return;
		}
		int selectedObjectIndex = this.SelectedObjectIndex + 1;
		this.SelectedObjectIndex = selectedObjectIndex;
		this.ActivateSelectedIndex();
	}

	public void MatchStage(EvolvingCosmetic other)
	{
		while (this.SelectedObjectIndex > other.SelectedObjectIndex)
		{
			int selectedObjectIndex;
			if (!this.CanGoBack())
			{
				IL_42:
				while (this.SelectedObjectIndex < other.SelectedObjectIndex && this.CanGoForward())
				{
					selectedObjectIndex = this.SelectedObjectIndex + 1;
					this.SelectedObjectIndex = selectedObjectIndex;
				}
				this.ActivateSelectedIndex();
				return;
			}
			selectedObjectIndex = this.SelectedObjectIndex - 1;
			this.SelectedObjectIndex = selectedObjectIndex;
		}
		goto IL_42;
	}

	private void UnselectAll()
	{
		this.SelectedObjectIndex = -1;
		EvolvingCosmetic.AgeAwareGameObject[] array = this.ageAwareGameObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(false);
		}
	}

	public bool CanGoBack()
	{
		return this.IsIndexAvailable(this.SelectedObjectIndex - 1);
	}

	public bool CanGoForward()
	{
		return this.IsIndexAvailable(this.SelectedObjectIndex + 1);
	}

	public void OnStateUpdate(int state)
	{
		if (!this.IsIndexAvailable(state))
		{
			return;
		}
		this.SelectedObjectIndex = state;
		this.ActivateSelectedIndex();
	}

	private VRRigReliableState.StateSyncSlots GetStateSyncSlot()
	{
		CosmeticSO cosmeticSOFromDisplayName = CosmeticsController.instance.GetCosmeticSOFromDisplayName(this.PlayfabId);
		CosmeticsController.CosmeticCategory value = cosmeticSOFromDisplayName.info.category.Value;
		VRRigReliableState.StateSyncSlots result;
		if (value != CosmeticsController.CosmeticCategory.Hat)
		{
			if (value != CosmeticsController.CosmeticCategory.Shirt)
			{
				throw new Exception(string.Format("Unhandled CosmeticCategory {0}", cosmeticSOFromDisplayName.info.category.Value));
			}
			result = VRRigReliableState.StateSyncSlots.Shirt;
		}
		else
		{
			result = VRRigReliableState.StateSyncSlots.Hat;
		}
		return result;
	}

	[SerializeField]
	private EvolvingCosmetic.SubscriptionAgeRule ageRule;

	[SerializeField]
	private EvolvingCosmetic.AgeAwareGameObject[] ageAwareGameObjects;

	[SerializeField]
	private int capDays = 1;

	[SerializeField]
	private UnityEvent<int> DispatchDaysOnEnable;

	[SerializeField]
	private int maxDays = 1;

	[SerializeField]
	private int multiplier = 1;

	[SerializeField]
	private UnityEvent<float> DispatchDaysOnEnableNormalized;

	private int? _daysAccrued;

	private enum SubscriptionAgeRule
	{
		ItemAge,
		MinItemSubscriptionAge,
		SubscriptionAge,
		MinItemSubscriptionAgeActive,
		SubscriptionAgeActive
	}

	[Serializable]
	private struct AgeAwareGameObject
	{
		public GameObject gameObject;

		public int minActiveDays;

		public int maxActiveDays;

		public bool requireCurrentSubscription;
	}
}
