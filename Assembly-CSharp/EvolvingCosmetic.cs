using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Events;

public class EvolvingCosmetic : MonoBehaviour
{
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
			this._selectedObjectIndex = i;
		}
		this.ActivateSelectedIndex();
		UnityEvent<int> dispatchDaysOnEnable = this.DispatchDaysOnEnable;
		if (dispatchDaysOnEnable != null)
		{
			int? daysAccrued = this._daysAccrued;
			if (daysAccrued == null)
			{
				throw new NullReferenceException("_daysAccrued was not set by end of OnEnable.");
			}
			dispatchDaysOnEnable.Invoke(daysAccrued.GetValueOrDefault());
		}
		if (this.maxDays > 0)
		{
			UnityEvent<float> dispatchDaysOnEnableNormalized = this.DispatchDaysOnEnableNormalized;
			if (dispatchDaysOnEnableNormalized == null)
			{
				return;
			}
			dispatchDaysOnEnableNormalized.Invoke(Mathf.Min((float)this._daysAccrued.Value / (float)this.maxDays, 1f));
		}
	}

	public IEnumerable<GameObject> GetAvailableGameObjects()
	{
		if (this._daysAccrued == null)
		{
			throw new NullReferenceException("_daysAccrued is not calculated.");
		}
		bool hasSubscription = SubscriptionManager.IsLocalSubscribed();
		foreach (EvolvingCosmetic.AgeAwareGameObject ageAwareGameObject in this.ageAwareGameObjects)
		{
			if ((!ageAwareGameObject.requireCurrentSubscription || hasSubscription) && ageAwareGameObject.minActiveDays <= this._daysAccrued.Value)
			{
				yield return ageAwareGameObject.gameObject;
			}
		}
		EvolvingCosmetic.AgeAwareGameObject[] array = null;
		yield break;
	}

	private void ActivateSelectedIndex()
	{
		if (!this.IsSelectedIndexAvailable())
		{
			return;
		}
		for (int i = 0; i < this.ageAwareGameObjects.Length; i++)
		{
			this.ageAwareGameObjects[i].gameObject.SetActive(i == this._selectedObjectIndex);
		}
	}

	private bool IsSelectedIndexAvailable()
	{
		return this.IsIndexAvailable(this._selectedObjectIndex);
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

	private void GoBack()
	{
		if (!this.CanGoBack())
		{
			return;
		}
		this._selectedObjectIndex--;
		this.ActivateSelectedIndex();
	}

	private void GoForward()
	{
		if (!this.CanGoForward())
		{
			return;
		}
		this._selectedObjectIndex++;
		this.ActivateSelectedIndex();
	}

	private void UnselectAll()
	{
		this._selectedObjectIndex = 0;
		EvolvingCosmetic.AgeAwareGameObject[] array = this.ageAwareGameObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(false);
		}
	}

	private bool CanGoBack()
	{
		return this.IsIndexAvailable(this._selectedObjectIndex - 1);
	}

	private bool CanGoForward()
	{
		return this.IsIndexAvailable(this._selectedObjectIndex + 1);
	}

	[SerializeField]
	private EvolvingCosmetic.SubscriptionAgeRule ageRule;

	[SerializeField]
	private EvolvingCosmetic.AgeAwareGameObject[] ageAwareGameObjects;

	[SerializeField]
	private UnityEvent<int> DispatchDaysOnEnable;

	[SerializeField]
	private int maxDays = 1;

	[SerializeField]
	private UnityEvent<float> DispatchDaysOnEnableNormalized;

	private int _selectedObjectIndex;

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

		public bool requireCurrentSubscription;

		public int minActiveDays;

		public int maxActiveDays;
	}
}
