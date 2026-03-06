using System;
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
		int num = 0;
		switch (this.ageRule)
		{
		case EvolvingCosmetic.SubscriptionAgeRule.ItemAge:
			num = vrrig.CheckCosmeticAge(base.name);
			break;
		case EvolvingCosmetic.SubscriptionAgeRule.MinItemSubscriptionAge:
			num = Mathf.Min(subscriptionDetails.daysAccrued, vrrig.CheckCosmeticAge(base.name));
			break;
		case EvolvingCosmetic.SubscriptionAgeRule.SubscriptionAge:
			num = subscriptionDetails.daysAccrued;
			break;
		case EvolvingCosmetic.SubscriptionAgeRule.MinItemSubscriptionAgeActive:
			if (subscriptionDetails.active)
			{
				num = Mathf.Min(subscriptionDetails.daysAccrued, vrrig.CheckCosmeticAge(base.name));
			}
			break;
		case EvolvingCosmetic.SubscriptionAgeRule.SubscriptionAgeActive:
			if (subscriptionDetails.active)
			{
				num = subscriptionDetails.daysAccrued;
			}
			break;
		}
		for (int i = 0; i < this.ageAwareGameObjects.Length; i++)
		{
			EvolvingCosmetic.AgeAwareGameObject ageAwareGameObject = this.ageAwareGameObjects[i];
			ageAwareGameObject.gameObject.SetActive((!ageAwareGameObject.requireCurrentSubscription || subscriptionDetails.active) && num >= ageAwareGameObject.minActiveDays && (ageAwareGameObject.maxActiveDays == 0 || num < ageAwareGameObject.maxActiveDays));
		}
		UnityEvent<int> dispatchDaysOnEnable = this.DispatchDaysOnEnable;
		if (dispatchDaysOnEnable != null)
		{
			dispatchDaysOnEnable.Invoke(num);
		}
		if (this.maxDays > 0)
		{
			UnityEvent<float> dispatchDaysOnEnableNormalized = this.DispatchDaysOnEnableNormalized;
			if (dispatchDaysOnEnableNormalized == null)
			{
				return;
			}
			dispatchDaysOnEnableNormalized.Invoke(Mathf.Min((float)num / (float)this.maxDays, 1f));
		}
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
